using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Book))]
public class BookNavigation : MonoBehaviour
{
    private Book book;
    private AutoFlip autoFlip;

    [Header("Navigation Button Settings")]
    public float buttonSize = 80f;
    public float edgePadding = 40f;
    public Color buttonColor = new Color(0.1f, 0.1f, 0.1f, 0.6f);
    public Color hoverColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    public Color arrowColor = Color.white;
    public int fontSize = 48;

    private Button leftButton;
    private Button rightButton;

    void Start()
    {
        book = GetComponent<Book>();
        
        // Find or add AutoFlip component
        autoFlip = GetComponent<AutoFlip>();
        if (autoFlip == null)
        {
            autoFlip = gameObject.AddComponent<AutoFlip>();
            autoFlip.AutoStartFlip = false;
        }
        else
        {
            autoFlip.AutoStartFlip = false;
        }

        CreateNavigationButtons();
    }

    void CreateNavigationButtons()
    {
        // Determine the parent RectTransform
        RectTransform parent = book.canvas != null ? book.canvas.GetComponent<RectTransform>() : null;
        if (parent == null && transform.parent != null)
        {
            parent = transform.parent.GetComponent<RectTransform>();
        }
        if (parent == null)
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                parent = canvas.GetComponent<RectTransform>();
            }
        }

        if (parent == null)
        {
            Debug.LogError("BookNavigation: Could not find Canvas or parent RectTransform for UI buttons!");
            return;
        }

        // Get a font that works on all Unity versions (legacy or newer)
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
        {
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        // 1. Create Left Button
        GameObject leftBtnObj = new GameObject("FlipButton_Left");
        leftBtnObj.transform.SetParent(parent, false);
        RectTransform leftRect = leftBtnObj.AddComponent<RectTransform>();
        leftRect.anchorMin = new Vector2(0f, 0.5f);
        leftRect.anchorMax = new Vector2(0f, 0.5f);
        leftRect.pivot = new Vector2(0f, 0.5f);
        leftRect.anchoredPosition = new Vector2(edgePadding, 0f);
        leftRect.sizeDelta = new Vector2(buttonSize, buttonSize);

        Image leftImg = leftBtnObj.AddComponent<Image>();
        leftImg.sprite = SpriteFromColor(Color.white);
        leftImg.color = buttonColor;

        leftButton = leftBtnObj.AddComponent<Button>();
        leftButton.onClick.AddListener(() => autoFlip.FlipLeftPage());

        // Add Left Arrow Text
        GameObject leftTextObj = new GameObject("ArrowText");
        leftTextObj.transform.SetParent(leftBtnObj.transform, false);
        RectTransform leftTextRect = leftTextObj.AddComponent<RectTransform>();
        leftTextRect.anchorMin = Vector2.zero;
        leftTextRect.anchorMax = Vector2.one;
        leftTextRect.sizeDelta = Vector2.zero;
        Text leftText = leftTextObj.AddComponent<Text>();
        leftText.text = "<";
        leftText.font = font;
        leftText.alignment = TextAnchor.MiddleCenter;
        leftText.color = arrowColor;
        leftText.fontSize = fontSize;

        // Setup transitions
        ColorBlock cbLeft = leftButton.colors;
        cbLeft.normalColor = buttonColor;
        cbLeft.highlightedColor = hoverColor;
        cbLeft.pressedColor = new Color(0f, 0f, 0f, 0.9f);
        cbLeft.selectedColor = buttonColor;
        leftButton.colors = cbLeft;

        // 2. Create Right Button
        GameObject rightBtnObj = new GameObject("FlipButton_Right");
        rightBtnObj.transform.SetParent(parent, false);
        RectTransform rightRect = rightBtnObj.AddComponent<RectTransform>();
        rightRect.anchorMin = new Vector2(1f, 0.5f);
        rightRect.anchorMax = new Vector2(1f, 0.5f);
        rightRect.pivot = new Vector2(1f, 0.5f);
        rightRect.anchoredPosition = new Vector2(-edgePadding, 0f);
        rightRect.sizeDelta = new Vector2(buttonSize, buttonSize);

        Image rightImg = rightBtnObj.AddComponent<Image>();
        rightImg.sprite = SpriteFromColor(Color.white);
        rightImg.color = buttonColor;

        rightButton = rightBtnObj.AddComponent<Button>();
        rightButton.onClick.AddListener(() => autoFlip.FlipRightPage());

        // Add Right Arrow Text
        GameObject rightTextObj = new GameObject("ArrowText");
        rightTextObj.transform.SetParent(rightBtnObj.transform, false);
        RectTransform rightTextRect = rightTextObj.AddComponent<RectTransform>();
        rightTextRect.anchorMin = Vector2.zero;
        rightTextRect.anchorMax = Vector2.one;
        rightTextRect.sizeDelta = Vector2.zero;
        Text rightText = rightTextObj.AddComponent<Text>();
        rightText.text = ">";
        rightText.font = font;
        rightText.alignment = TextAnchor.MiddleCenter;
        rightText.color = arrowColor;
        rightText.fontSize = fontSize;

        // Setup transitions
        ColorBlock cbRight = rightButton.colors;
        cbRight.normalColor = buttonColor;
        cbRight.highlightedColor = hoverColor;
        cbRight.pressedColor = new Color(0f, 0f, 0f, 0.9f);
        cbRight.selectedColor = buttonColor;
        rightButton.colors = cbRight;
    }

    private Sprite SpriteFromColor(Color color)
    {
        Texture2D texture = new Texture2D(128, 128);
        for (int y = 0; y < 128; y++)
        {
            for (int x = 0; x < 128; x++)
            {
                float dx = x - 64f;
                float dy = y - 64f;
                if (dx * dx + dy * dy < 64f * 64f)
                {
                    texture.SetPixel(x, y, color);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));
    }
}
