using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.Editor.Tests
{
    [TestFixture]
    class FileTests
    {
        // 140 is the reserved length that PVP tests check for.
        const int k_MaxFilePathLength = 140;

        // Root path for sample asset folder when imported into a project
        const string k_RootSearchPath = "Packages/com.unity.xr.interaction.toolkit/Samples~";

        // Root path used for template packing when Samples are included as part of a template
        const string k_TemplatePathPrefix = "ProjectData~/Assets";
        const string k_SampleRoot = "Samples";
        const string k_Package = "XR Interaction Toolkit";
        const string k_VersionPlaceholder = "X.X.XX";
        const string k_TemplateSamplePath = k_TemplatePathPrefix + "/" + k_SampleRoot + "/" + k_Package + "/" + k_VersionPlaceholder;

        [Test]
        public void PackageCacheFilePathLengthIsBelowMaxLimit()
        {
            var assembly = Assembly.Load("Unity.XR.Interaction.Toolkit");
            Assert.That(assembly, Is.Not.Null);

            // Get the root file path of the XRI package in the project.
            // Example: "C:\UnitySrc\Example-Unity-3D-Project-2021.3\Library\PackageCache\com.unity.xr.interaction.toolkit@3.0.0"
            var packageInfo = PackageManager.PackageInfo.FindForAssembly(assembly);
            Assert.That(packageInfo, Is.Not.Null);
            Assert.That(packageInfo.resolvedPath, Is.Not.Null);

            var maxLength = k_MaxFilePathLength;
            var filePathLengthExceeded = CheckPathLengthInFolderRecursively(string.Empty, packageInfo.resolvedPath, maxLength);
            Assert.That(filePathLengthExceeded, Is.False);
        }

        [Test]
        public void SampleAssetsTemplateFilePathLengthIsBelowMaxLimit()
        {
            var sampleDirectory = new DirectoryInfo(k_RootSearchPath);
            Assert.That(sampleDirectory, Is.Not.Null);
            Assert.That(sampleDirectory.Exists, Is.True);

            var maxLength = k_MaxFilePathLength - k_TemplateSamplePath.Length;
            var filePathLengthExceeded = CheckPathLengthInFolderRecursively(string.Empty, sampleDirectory.FullName, maxLength);
            Assert.That(filePathLengthExceeded, Is.False);
        }

        static string CombineAllowingEmpty(string path1, string path2)
        {
            if (string.IsNullOrEmpty(path1))
                return path2;
            if (string.IsNullOrEmpty(path2))
                return path1;
            return Path.Combine(path1, path2);
        }

        static bool CheckPathLengthInFolderRecursively(string relativeFolder, string absoluteBasePath, int maxLength)
        {
            try
            {
                var maxLimitExceeded = false;
                var fullFolder = CombineAllowingEmpty(absoluteBasePath, relativeFolder);

                foreach (string entry in Directory.GetFileSystemEntries(fullFolder))
                {
                    var fullPath = CombineAllowingEmpty(relativeFolder, Path.GetFileName(entry));

                    var diffLength = fullPath.Length - maxLength;
                    if (diffLength > 0)
                    {
                        maxLimitExceeded = true;
                        Debug.LogError($"{fullPath} is {diffLength} character(s) above the limit of {maxLength} characters ({maxLength} vs {fullPath.Length}). You must use a shorter name.");
                    }
                }

                foreach (string dir in Directory.GetDirectories(fullFolder))
                {
                    var wasExceeded = CheckPathLengthInFolderRecursively(CombineAllowingEmpty(relativeFolder, Path.GetFileName(dir)), absoluteBasePath, maxLength);
                    if (wasExceeded)
                        maxLimitExceeded = true;
                }

                return maxLimitExceeded;
            }
            catch (Exception e)
            {
                Debug.LogError("Exception " + e.Message);
            }

            return false;
        }
    }
}
