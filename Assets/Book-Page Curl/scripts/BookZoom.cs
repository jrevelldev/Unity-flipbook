using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(Book))]
public class BookZoom : MonoBehaviour
{
    private Book book;
    private RectTransform bookPanel;
    private RectTransform canvasRect;

    [Header("Zoom Settings")]
    public float zoomScale = 2.0f;
    public float zoomSpeed = 15f; // Interpolation speed for smooth transitions

    private Vector3 originalScale;
    private Vector2 originalPosition;
    private bool isZoomed = false;

    private Vector2 targetZoomPosition;
    private Vector2 lastMousePositionInParent;

    // Track the initial interactable state before zoom
    private bool wasInteractable = true;

    void Start()
    {
        book = GetComponent<Book>();
        bookPanel = book.GetComponent<RectTransform>();
        
        Canvas canvas = book.canvas != null ? book.canvas : GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }

        originalScale = bookPanel.localScale;
        originalPosition = bookPanel.anchoredPosition;
    }

    void Update()
    {
        Camera eventCamera = (book.canvas != null && book.canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? book.canvas.worldCamera : null;

        // Detect click down
        if (Input.GetMouseButtonDown(0))
        {
            if (IsInsideBook() && !IsClickOnHotspotOrButton())
            {
                isZoomed = true;
                wasInteractable = book.interactable;
                book.interactable = false; // Disable page turning during zoom

                Vector3 localPos = book.transformPoint(Input.mousePosition);
                
                // Calculate target anchored position to center the click
                targetZoomPosition = -((Vector2)localPos) * zoomScale;
                ClampTargetPosition(ref targetZoomPosition, zoomScale);

                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, eventCamera, out lastMousePositionInParent);
            }
        }

        if (isZoomed)
        {
            // Handle drag/pan while zoomed
            if (Input.GetMouseButton(0))
            {
                Vector2 currentMousePosInParent;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, eventCamera, out currentMousePosInParent))
                {
                    Vector2 delta = currentMousePosInParent - lastMousePositionInParent;
                    targetZoomPosition += delta;
                    ClampTargetPosition(ref targetZoomPosition, zoomScale);
                    lastMousePositionInParent = currentMousePosInParent;
                }
            }

            // Detect release
            if (Input.GetMouseButtonUp(0))
            {
                isZoomed = false;
                book.interactable = wasInteractable; // Restore page turning interactable state
            }
        }

        // Smoothly interpolate scale and position towards the target values
        Vector3 currentTargetScale = isZoomed ? (originalScale * zoomScale) : originalScale;
        Vector2 currentTargetPos = isZoomed ? targetZoomPosition : originalPosition;

        bookPanel.localScale = Vector3.Lerp(bookPanel.localScale, currentTargetScale, Time.deltaTime * zoomSpeed);
        bookPanel.anchoredPosition = Vector2.Lerp(bookPanel.anchoredPosition, currentTargetPos, Time.deltaTime * zoomSpeed);
    }

    private bool IsInsideBook()
    {
        if (book == null || bookPanel == null) return false;
        Vector3 localPos = book.transformPoint(Input.mousePosition);
        return bookPanel.rect.Contains(localPos);
    }

    private bool IsClickOnHotspotOrButton()
    {
        if (EventSystem.current == null) return false;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            GameObject obj = result.gameObject;
            if (obj.name == "LeftHotSpot" || obj.name == "RightHotSpot")
            {
                return true;
            }
            if (obj.name.Contains("FlipButton") || obj.GetComponentInParent<Button>() != null)
            {
                return true;
            }
        }
        return false;
    }

    private void ClampTargetPosition(ref Vector2 position, float scale)
    {
        if (canvasRect == null || bookPanel == null) return;

        float bookW = bookPanel.rect.width;
        float bookH = bookPanel.rect.height;
        float canvasW = canvasRect.rect.width;
        float canvasH = canvasRect.rect.height;

        float maxOffsetX = Mathf.Max(0, (bookW * scale - canvasW) / 2f);
        float maxOffsetY = Mathf.Max(0, (bookH * scale - canvasH) / 2f);

        position.x = Mathf.Clamp(position.x, -maxOffsetX, maxOffsetX);
        position.y = Mathf.Clamp(position.y, -maxOffsetY, maxOffsetY);
    }
}
