using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.Editor.Tests
{
    /// <summary>
    /// Validates texture assets within the XR Interaction Toolkit package to ensure
    /// they meet optimization standards for performance and memory usage.
    /// Checks both imported package assets AND source files in Samples~ folder.
    /// </summary>
    [TestFixture]
    class ImageTests
    {
        // Texture optimization constraints
        const int k_MaxTextureResolution = 4096;
        const float k_MaxTextureFileSizeMB = 2f;

        // Naming conventions for texture identification
        const string k_NormalMapIdentifier = "_norm";
        const string k_UIImageIdentifier = "_ui";
        const string k_MaskIdentifier = "_M";
        const string k_AlphaIdentifier = "_Alpha";
        const string k_CubemapImageIdentifier = "_Cube";
        const string k_LightingLightmapIdentifier = "Lightmap-";
        const string k_LightingReflectionProbeIdentifier = "ReflectionProbe-";

        // Format recommendations
        const string k_LightingExrExtension = ".exr";
        readonly string[] k_LosslessFormats = { ".png", ".tga" };
        readonly string[] k_LossyFormats = { ".jpg", ".jpeg" };
        readonly string[] k_AllTextureExtensions = { ".png", ".jpg", ".jpeg", ".tga", ".psd", ".tiff", ".tif", ".bmp" };
        readonly string[] k_TiffExtensions = { ".tiff", ".tif" };
        readonly string[] k_AlphaCheckSkipExtensions = { ".exr", ".tif", ".tiff" };

        // Package path constants
        const string k_PackageDisplayName = "XR Interaction Toolkit";
        const string k_SamplesFolderName = "Samples~";

        // Paths for samples when imported into a project
        const string k_ImportedSamplesRoot = "Assets/Samples";
        const string k_TemplatePathPrefix = "ProjectData~/Assets";
        const string k_SampleRoot = "Samples";

        /// <summary>
        /// Container for texture information that can be gathered from both
        /// imported assets (via AssetDatabase) and raw files (via file system).
        /// </summary>
        class TextureInfo
        {
            public string Path;              // Asset path or file system path
            public int Width;
            public int Height;
            public float FileSizeMB;
            public string Extension;
            public bool IsFromAssetDatabase; // True if imported asset, false if raw file
            public TextureImporter Importer; // Only available for imported assets

            public string DisplayName => IsFromAssetDatabase ? Path : $"{Path} ({k_SamplesFolderName} source)";
        }

        [Test]
        public void TextureDimensionsArePowerOfTwo()
        {
            var issues = new List<string>();
            var textures = FindAllTextureInfo();

            AssertTexturesFound(textures);

            foreach (var textureInfo in textures)
            {
                // Skip file size checks where the path contains "_Cube" (case-insensitive) for skyboxes
                if (textureInfo.Path.IndexOf(k_CubemapImageIdentifier, StringComparison.OrdinalIgnoreCase) >= 0)
                    continue;

                if (textureInfo.Path.Contains(k_UIImageIdentifier))
                    continue;

                // Skip generated lighting textures
                if (string.Equals(textureInfo.Extension, k_LightingExrExtension, StringComparison.OrdinalIgnoreCase))
                {
                    if (textureInfo.DisplayName.Contains(k_LightingLightmapIdentifier) ||
                        textureInfo.DisplayName.Contains(k_LightingReflectionProbeIdentifier))
                        continue;
                }

                // Handle 0x0 dimensions (could not be read or has no content)
                if (textureInfo.Width == 0 && textureInfo.Height == 0)
                {
                    if (k_TiffExtensions.Contains(textureInfo.Extension))
                    {
                        Debug.Log($"Skipping power-of-two check for {textureInfo.DisplayName}: TIF format dimensions could not be read and were not considered for the test.");
                    }
                    else
                    {
                        Debug.LogWarning($"Skipping power-of-two check for {textureInfo.DisplayName}: Image could not be read or has no content (0x0 dimensions).");
                    }
                    continue;
                }

                bool widthPot = IsPowerOfTwo(textureInfo.Width);
                bool heightPot = IsPowerOfTwo(textureInfo.Height);

                if (!widthPot || !heightPot)
                {
                    issues.Add($"{textureInfo.DisplayName}: {textureInfo.Width}x{textureInfo.Height} (non-POT)");
                    continue;
                }

                // Non-square textures are acceptable, but log their dimensions for future investigation
                if (textureInfo.Width != textureInfo.Height)
                {
                    Debug.Log($"Non-square texture detected: {textureInfo.DisplayName} is {textureInfo.Width}x{textureInfo.Height}");
                }
            }

            AssertNoIssues(issues, "Textures with non-power-of-2 dimensions");
        }

        [Test]
        public void TexturesDoNotExceedMaxResolution()
        {
            var issues = new List<string>();
            var textures = FindAllTextureInfo();

            AssertTexturesFound(textures);

            foreach (var textureInfo in textures)
            {
                if (textureInfo.Width > k_MaxTextureResolution || textureInfo.Height > k_MaxTextureResolution)
                {
                    issues.Add($"{textureInfo.DisplayName}: {textureInfo.Width}x{textureInfo.Height} (max: {k_MaxTextureResolution})");
                }
            }

            AssertNoIssues(issues, "Textures exceeding maximum resolution");
        }

        [Test]
        public void TexturesDoNotExceedMaxFileSize()
        {
            var issues = new List<string>();
            var textures = FindAllTextureInfo();

            AssertTexturesFound(textures);

            foreach (var textureInfo in textures)
            {
                // Skip file size checks where the path contains "_Cube" (case-insensitive) for skyboxes
                if (textureInfo.Path.IndexOf(k_CubemapImageIdentifier, StringComparison.OrdinalIgnoreCase) >= 0)
                    continue;

                if (string.Equals(textureInfo.Extension, k_LightingExrExtension, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log($"Skipping file size check for EXR {textureInfo.DisplayName}: {textureInfo.FileSizeMB:F2}MB");
                    continue;
                }

                if (textureInfo.FileSizeMB > k_MaxTextureFileSizeMB)
                {
                    issues.Add($"{textureInfo.DisplayName}: {textureInfo.FileSizeMB:F2}MB (max: {k_MaxTextureFileSizeMB}MB)");
                }
            }

            AssertNoIssues(issues, "Textures exceeding maximum file size");
        }

        [Test]
        public void NormalMapsUseCorrectFormat()
        {
            var issues = new List<string>();
            var textures = FindAllTextureInfo();

            AssertTexturesFound(textures);

            foreach (var textureInfo in textures)
            {
                if (!textureInfo.Path.Contains(k_NormalMapIdentifier))
                    continue;

                if (k_LossyFormats.Contains(textureInfo.Extension))
                {
                    issues.Add($"{textureInfo.DisplayName}: uses lossy format {textureInfo.Extension}");
                }
            }

            AssertNoIssues(issues, "Normal maps using lossy compression");
        }

        [Test]
        public void TexturesWithAlphaUseCorrectFormat()
        {
            var issues = new List<string>();
            var textures = FindAllTextureInfo();

            AssertTexturesFound(textures);

            foreach (var textureInfo in textures)
            {
                // Only check imported assets where we can definitively check for alpha
                if (!textureInfo.IsFromAssetDatabase || textureInfo.Importer == null)
                    continue;

                if (k_AlphaCheckSkipExtensions.Contains(textureInfo.Extension))
                {
                    Debug.Log($"Skipping alpha channel check for {textureInfo.DisplayName}: {textureInfo.Extension} is excluded");
                    continue;
                }

                if (textureInfo.Importer.DoesSourceTextureHaveAlpha())
                {
                    if (!k_LosslessFormats.Contains(textureInfo.Extension))
                    {
                        issues.Add($"{textureInfo.DisplayName}: has alpha but uses {textureInfo.Extension}");
                    }
                }
            }

            AssertNoIssues(issues, "Textures with alpha using inappropriate formats");
        }

        /// <summary>
        /// Validates texture import settings. Only applies to imported assets,
        /// not source files in Samples~ folder.
        /// </summary>
        [Test]
        public void TexturesUseRecommendedImportSettings()
        {
            var issues = new List<string>();
            var textures = FindAllTextureInfo();

            AssertTexturesFound(textures);

            // Filter to only imported assets
            var importedTextures = textures.Where(t => t.IsFromAssetDatabase && t.Importer != null);

            if (!importedTextures.Any())
                return;

            foreach (var textureInfo in importedTextures)
            {
                bool isUI = textureInfo.Path.Contains(k_UIImageIdentifier);
                bool isMaskOrAlpha = textureInfo.Path.Contains(k_MaskIdentifier) || textureInfo.Path.Contains(k_AlphaIdentifier);

                if (isUI && textureInfo.Importer.mipmapEnabled)
                {
                    issues.Add($"{textureInfo.DisplayName}: UI texture has unnecessary mipmaps enabled");
                }

                if (textureInfo.Importer.textureCompression == TextureImporterCompression.Uncompressed && !isMaskOrAlpha)
                {
                    issues.Add($"{textureInfo.DisplayName}: uses uncompressed format (consider compression)");
                }

                if (textureInfo.Importer.isReadable)
                {
                    issues.Add($"{textureInfo.DisplayName}: has Read/Write enabled (doubles memory usage)");
                }
            }

            AssertNoIssues(issues, "Textures with non-optimal import settings");
        }

        /// <summary>
        /// Checks mobile platform overrides. Only applies to imported assets.
        /// </summary>
        [Test]
        public void LargeTexturesHaveMobilePlatformOverrides()
        {
            var issues = new List<string>();
            var textures = FindAllTextureInfo();

            AssertTexturesFound(textures);

            var importedTextures = textures.Where(t => t.IsFromAssetDatabase && t.Importer != null);

            if (!importedTextures.Any())
                return;

            foreach (var textureInfo in importedTextures)
            {
                if (textureInfo.Importer.maxTextureSize <= 2048)
                    continue;

                var androidSettings = textureInfo.Importer.GetPlatformTextureSettings("Android");
                var iosSettings = textureInfo.Importer.GetPlatformTextureSettings("iPhone");

                if (!androidSettings.overridden || !iosSettings.overridden)
                {
                    var missing = new List<string>();
                    if (!androidSettings.overridden)
                        missing.Add("Android");
                    if (!iosSettings.overridden)
                        missing.Add("iOS");

                    issues.Add($"{textureInfo.DisplayName}: {textureInfo.Importer.maxTextureSize}px missing overrides for {string.Join(", ", missing)}");
                }
            }

            AssertNoIssues(issues, "Large textures missing mobile platform overrides");
        }

        #region Texture Discovery

        /// <summary>
        /// Finds all texture assets and source files in the package.
        /// Includes both AssetDatabase-indexed files AND raw files in Samples~ folder.
        /// </summary>
        List<TextureInfo> FindAllTextureInfo()
        {
            var result = new List<TextureInfo>();

            // 1. Find imported package assets via AssetDatabase
            result.AddRange(FindImportedTextureAssets());

            // 2. Find raw texture files in Samples~ folder
            result.AddRange(FindSamplesSourceTextures());

            Debug.Log($"Found {result.Count} total textures " +
                      $"({result.Count(t => t.IsFromAssetDatabase)} imported, " +
                      $"{result.Count(t => !t.IsFromAssetDatabase)} in {k_SamplesFolderName} source)");

            return result;
        }

        /// <summary>
        /// Finds texture assets that are indexed by AssetDatabase.
        /// Searches package path, imported samples, and template paths.
        /// </summary>
        List<TextureInfo> FindImportedTextureAssets()
        {
            var result = new List<TextureInfo>();
            var seenPaths = new HashSet<string>(); // Track unique paths
            var searchPaths = new List<string>();

            // Find package source path
            var packageInfo = PackageManager.PackageInfo.FindForAssembly(Assembly.GetExecutingAssembly());
            if (packageInfo != null)
            {
                string packagePath = $"Packages/{packageInfo.name}";
                if (AssetDatabase.IsValidFolder(packagePath))
                {
                    searchPaths.Add(packagePath);
                }
            }

            // Check for imported samples
            string importedSamplesPath = $"{k_ImportedSamplesRoot}/{k_PackageDisplayName}";
            if (AssetDatabase.IsValidFolder(importedSamplesPath))
            {
                searchPaths.Add(importedSamplesPath);
            }

            // Check for template paths
            string templatePath = $"{k_TemplatePathPrefix}/{k_SampleRoot}/{k_PackageDisplayName}";
            if (AssetDatabase.IsValidFolder(templatePath))
            {
                searchPaths.Add(templatePath);
            }

            // Search all paths
            foreach (var searchPath in searchPaths)
            {
                string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { searchPath });
                foreach (var guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                    // Skip if we've already processed this path
                    if (!seenPaths.Add(assetPath))
                        continue;

                    var textureInfo = CreateTextureInfoFromAsset(assetPath);
                    if (textureInfo != null)
                        result.Add(textureInfo);
                }
            }

            return result;
        }


        /// <summary>
        /// Finds texture source files in the Samples~ folder using file system operations.
        /// The Samples~ folder is excluded from AssetDatabase indexing.
        /// </summary>
        List<TextureInfo> FindSamplesSourceTextures()
        {
            var result = new List<TextureInfo>();

            // Get the physical path to the package on disk
            var packageInfo = PackageManager.PackageInfo.FindForAssembly(Assembly.GetExecutingAssembly());
            if (packageInfo == null || string.IsNullOrEmpty(packageInfo.resolvedPath))
            {
                Debug.LogWarning($"Could not resolve package path for {k_SamplesFolderName} search");
                return result;
            }

            string samplesPath = Path.Combine(packageInfo.resolvedPath, k_SamplesFolderName);

            if (!Directory.Exists(samplesPath))
            {
                Debug.Log($"{k_SamplesFolderName} folder not found at {samplesPath}");
                return result;
            }

            // Recursively find all image files
            foreach (var extension in k_AllTextureExtensions)
            {
                var files = Directory.GetFiles(samplesPath, $"*{extension}", SearchOption.AllDirectories);
                foreach (var filePath in files)
                {
                    var textureInfo = CreateTextureInfoFromFile(filePath, packageInfo.resolvedPath);
                    if (textureInfo != null)
                    {
                        result.Add(textureInfo);
                    }
                }
            }

            Debug.Log($"Found {result.Count} texture files in {samplesPath}");
            return result;
        }

        /// <summary>
        /// Creates TextureInfo from an imported asset using AssetDatabase APIs.
        /// </summary>
        TextureInfo CreateTextureInfoFromAsset(string assetPath)
        {
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
                return null;

            importer.GetSourceTextureWidthAndHeight(out int width, out int height);

            return new TextureInfo
            {
                Path = assetPath,
                Width = width,
                Height = height,
                FileSizeMB = GetFileSizeMB(assetPath),
                Extension = Path.GetExtension(assetPath).ToLower(),
                IsFromAssetDatabase = true,
                Importer = importer
            };
        }

        /// <summary>
        /// Creates TextureInfo from a raw file using file system and image parsing.
        /// </summary>
        TextureInfo CreateTextureInfoFromFile(string fullPath, string packageRoot)
        {
            // Create a relative path for display
            string relativePath = fullPath.Replace(packageRoot, "").Replace("\\", "/").TrimStart('/');

            var fileInfo = new FileInfo(fullPath);
            float sizeMB = fileInfo.Length / (1024f * 1024f);
            string extension = Path.GetExtension(fullPath).ToLower();

            // Try to read dimensions from file
            (int width, int height) = ReadImageDimensions(fullPath, extension);

            return new TextureInfo
            {
                Path = relativePath,
                Width = width,
                Height = height,
                FileSizeMB = sizeMB,
                Extension = extension,
                IsFromAssetDatabase = false,
                Importer = null
            };
        }

        #endregion

        #region Image File Parsing

        /// <summary>
        /// Reads image dimensions directly from file headers without importing.
        /// Supports PNG, JPEG, TGA formats.
        /// </summary>
        static (int width, int height) ReadImageDimensions(string filePath, string extension)
        {
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (var reader = new BinaryReader(stream))
                {
                    switch (extension)
                    {
                        case ".png":
                            return ReadPNGDimensions(reader);
                        case ".jpg":
                        case ".jpeg":
                            return ReadJPEGDimensions(reader);
                        case ".tga":
                            return ReadTGADimensions(reader);
                        case ".tif":
                        case ".tiff":
                            // TIF format reading not implemented - return 0x0 without warning
                            // Warnings will be handled in the test method
                            return (0, 0);
                        default:
                            Debug.LogWarning($"Unsupported format for dimension reading: {extension}");
                            return (0, 0);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Could not read dimensions from {filePath}: {e.Message}");
                return (0, 0);
            }
        }

        /// <summary>
        /// Reads PNG dimensions from IHDR chunk.
        /// PNG format: 8-byte signature, then chunks with [length][type][data][crc]
        /// </summary>
        static (int width, int height) ReadPNGDimensions(BinaryReader reader)
        {
            // Skip PNG signature (8 bytes)
            reader.BaseStream.Seek(8, SeekOrigin.Begin);

            // Read IHDR chunk length (4 bytes, big-endian)
            reader.BaseStream.Seek(4, SeekOrigin.Current);

            // Verify IHDR chunk type
            byte[] chunkType = reader.ReadBytes(4);
            string type = System.Text.Encoding.ASCII.GetString(chunkType);
            if (type != "IHDR")
                return (0, 0);

            // Read width and height (4 bytes each, big-endian)
            int width = ReadInt32BigEndian(reader);
            int height = ReadInt32BigEndian(reader);

            return (width, height);
        }

        /// <summary>
        /// Reads JPEG dimensions from SOF (Start of Frame) marker.
        /// JPEG format: Series of markers (0xFF + marker type) followed by data
        /// </summary>
        static (int width, int height) ReadJPEGDimensions(BinaryReader reader)
        {
            // Skip JPEG header (0xFFD8)
            reader.BaseStream.Seek(2, SeekOrigin.Begin);

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                // Find next marker
                byte marker = reader.ReadByte();
                if (marker != 0xFF)
                    continue;

                byte markerType = reader.ReadByte();

                // SOF markers (Start of Frame) - 0xC0 to 0xCF (except 0xC4, 0xC8, 0xCC)
                if (markerType >= 0xC0 && markerType <= 0xCF &&
                    markerType != 0xC4 && markerType != 0xC8 && markerType != 0xCC)
                {
                    // Skip segment length (2 bytes) and precision (1 byte)
                    reader.BaseStream.Seek(3, SeekOrigin.Current);

                    // Read height and width (2 bytes each, big-endian)
                    int height = ReadInt16BigEndian(reader);
                    int width = ReadInt16BigEndian(reader);

                    return (width, height);
                }

                // Skip to next marker
                int segmentLength = ReadInt16BigEndian(reader);
                reader.BaseStream.Seek(segmentLength - 2, SeekOrigin.Current);
            }

            return (0, 0);
        }

        /// <summary>
        /// Reads TGA dimensions from header.
        /// TGA format: 18-byte header with width/height at bytes 12-15
        /// </summary>
        static (int width, int height) ReadTGADimensions(BinaryReader reader)
        {
            // TGA header: width at bytes 12-13, height at bytes 14-15 (little-endian)
            reader.BaseStream.Seek(12, SeekOrigin.Begin);

            int width = reader.ReadInt16();  // Little-endian
            int height = reader.ReadInt16(); // Little-endian

            return (width, height);
        }

        static int ReadInt32BigEndian(BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        static int ReadInt16BigEndian(BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(2);
            Array.Reverse(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }

        #endregion

        #region Helper Methods

        static bool IsPowerOfTwo(int n)
        {
            return n > 0 && (n & (n - 1)) == 0;
        }

        static float GetFileSizeMB(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
                return 0;

            try
            {
                string fullPath = Path.GetFullPath(assetPath);
                if (File.Exists(fullPath))
                {
                    var fileInfo = new FileInfo(fullPath);
                    return fileInfo.Length / (1024f * 1024f);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Could not get file size for {assetPath}: {e.Message}");
            }

            return 0;
        }

        static void AssertTexturesFound(List<TextureInfo> textures)
        {
            if (textures.Count == 0)
            {
                Assert.Inconclusive(
                    "No texture assets found in package. " +
                    "This test cannot validate texture settings. " +
                    "If the package should contain textures, verify search paths are correct.");
            }
        }

        static void AssertNoIssues(List<string> issues, string issueCategory)
        {
            if (issues.Count > 0)
            {
                Assert.Fail($"Found {issues.Count} {issueCategory}:\n" + string.Join("\n", issues));
            }
        }

        #endregion
    }
}
