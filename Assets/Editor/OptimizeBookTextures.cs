using UnityEngine;
using UnityEditor;
using System.IO;

public class OptimizeBookTextures
{
    [MenuItem("Tools/Optimize Book Textures")]
    public static void Optimize()
    {
        string folderPath = "Assets/BookPages";
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError($"Directory '{folderPath}' does not exist! Please check the folder path.");
            return;
        }

        string[] fileEntries = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
        int optimizedCount = 0;

        // Display progress bar
        EditorUtility.DisplayProgressBar("Optimizing Textures", "Finding textures...", 0f);

        try
        {
            for (int i = 0; i < fileEntries.Length; i++)
            {
                string filePath = fileEntries[i];
                string ext = Path.GetExtension(filePath).ToLower();

                // Process standard texture extensions, skip files like .meta, and skip raw PSD sources
                if ((ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".tga") && 
                    !filePath.EndsWith(".meta") && 
                    !filePath.EndsWith("pages.psd"))
                {
                    TextureImporter importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
                    if (importer != null)
                    {
                        EditorUtility.DisplayProgressBar("Optimizing Textures", $"Processing {Path.GetFileName(filePath)}...", (float)i / fileEntries.Length);

                        bool dirty = false;

                        // 1. Disable Mipmaps (reduces texture sizes in RAM/build by 33%; not needed for 2D UI/Page Flipbooks)
                        if (importer.mipmapEnabled)
                        {
                            importer.mipmapEnabled = false;
                            dirty = true;
                        }

                        // Set Non-Power-of-Two (NPOT) scale to ToNearest to enable Crunch Compression
                        if (importer.npotScale != TextureImporterNPOTScale.ToNearest)
                        {
                            importer.npotScale = TextureImporterNPOTScale.ToNearest;
                            dirty = true;
                        }

                        // 2. Disable Read/Write (saves graphics memory unless scripts need raw byte access)
                        if (importer.isReadable)
                        {
                            importer.isReadable = false;
                            dirty = true;
                        }

                        // 3. Configure Default Platform Settings
                        TextureImporterPlatformSettings defaultSettings = importer.GetDefaultPlatformTextureSettings();
                        
                        defaultSettings.textureCompression = TextureImporterCompression.Compressed;
                        defaultSettings.crunchedCompression = true;
                        defaultSettings.compressionQuality = 50; // 50 is the optimal quality-to-size sweet spot
                        defaultSettings.maxTextureSize = 2048;
                        defaultSettings.format = TextureImporterFormat.Automatic; // Default tab is automatic but compressed

                        // Check if WebGL settings are already matching our target configuration
                        TextureImporterPlatformSettings webGLSettings = importer.GetPlatformTextureSettings("WebGL");
                        bool webGLNeedsUpdate = !webGLSettings.overridden || 
                                                webGLSettings.format != TextureImporterFormat.ETC2_RGBA8Crunched ||
                                                webGLSettings.maxTextureSize != 2048 ||
                                                !webGLSettings.crunchedCompression ||
                                                webGLSettings.compressionQuality != 50;

                        if (dirty || defaultSettings.format != TextureImporterFormat.Automatic || webGLNeedsUpdate)
                        {
                            importer.SetPlatformTextureSettings(defaultSettings);
                            
                            // 4. Override WebGL specific settings to use ETC2 Crunched (highly compatible and tiny)
                            webGLSettings.overridden = true; 
                            webGLSettings.maxTextureSize = 2048;
                            webGLSettings.textureCompression = TextureImporterCompression.Compressed;
                            webGLSettings.crunchedCompression = true;
                            webGLSettings.compressionQuality = 50;
                            webGLSettings.format = TextureImporterFormat.ETC2_RGBA8Crunched; // WebGL-compatible crunched format
                            importer.SetPlatformTextureSettings(webGLSettings);

                            EditorUtility.SetDirty(importer);
                            importer.SaveAndReimport();
                            optimizedCount++;
                        }
                    }
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        Debug.Log($"Successfully optimized {optimizedCount} textures in Assets/BookPages!");
    }
}
