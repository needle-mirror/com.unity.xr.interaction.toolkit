using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.XR.CoreUtils.Editor;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.ProjectValidation
{
    /// <summary>
    /// Utility class to help with project validation for XR Interaction Toolkit and Samples.
    /// </summary>
    internal static class ProjectValidationUtility
    {
        const string k_SamplesRootDirectoryName = "Samples";

        /// <summary>
        /// Dictionary used to cache packages imported samples. The dictionary key is the package display name as displayed in package Samples directory.
        /// </summary>
        static Dictionary<string, PackageSampleData> s_PackageSampleCache;

        struct PackageSampleData
        {
            public string packageDisplayName;
            public Dictionary<string, SampleData> importedSamples;
        }

        struct SampleData
        {
            public string sampleName;
            public string packageDisplayName;
            public PackageVersion packageVersion;
        }

        /// <summary>
        /// Searches for a sample, <see cref="sampleDisplayName"/> from a specified package, <see cref="packageDisplayName"/>.
        /// The search is done by iterating through the Samples directory, but will utilize cached data if available.
        /// This function is version agnostic and will return true if the sample is found in any version of the package's
        /// sample directory.
        /// </summary>
        /// <param name="packageDisplayName">The name of the package directory that contains the sample.</param>
        /// <param name="sampleDisplayName">The name of the sample directory to search for.</param>
        /// <returns>Returns <see langword="true"/> if the sample is found in the Samples directory of the specified
        /// package. Otherwise, returns <see langword="false"/>.</returns>
        /// <seealso cref="SampleImportMeetsMinimumVersion"/>
        public static bool HasSampleImported(string packageDisplayName, string sampleDisplayName)
        {
            if (s_PackageSampleCache == null)
            {
                UpdatePackageSampleCache();
                if (s_PackageSampleCache == null)
                    return false;
            }

            return s_PackageSampleCache.TryGetValue(packageDisplayName, out var sampleData) && sampleData.importedSamples.ContainsKey(sampleDisplayName);
        }

        /// <summary>
        /// Searches for a sample, <see cref="sampleDisplayName"/> from a specified package, <see cref="packageDisplayName"/>
        /// and compares the sample version.
        /// </summary>
        /// <param name="packageDisplayName">The name of the package directory that contains the sample.</param>
        /// <param name="sampleDisplayName">The name of the sample directory to search for.</param>
        /// <param name="minVersion">The minimum package version the sample should be imported from.</param>
        /// <returns>Returns <see langword="true"/> if the sample is found in the Samples directory of the specified
        /// package and the imported sample's version is greater than or equal to the <see cref="minVersion"/>.</returns>
        public static bool SampleImportMeetsMinimumVersion(string packageDisplayName, string sampleDisplayName, PackageVersion minVersion)
        {
            if (HasSampleImported(packageDisplayName, sampleDisplayName))
                return s_PackageSampleCache[packageDisplayName].importedSamples[sampleDisplayName].packageVersion >= minVersion;

            return false;
        }

        /// <summary>
        /// Iterates through Samples directory and caches sample and package data.
        /// </summary>
        static void UpdatePackageSampleCache()
        {
            if (s_PackageSampleCache == null)
            {
                try
                {
                    s_PackageSampleCache = new Dictionary<string, PackageSampleData>();

                    var delimiter = Path.DirectorySeparatorChar;
                    var sampleRootPath = Path.Combine(Path.GetFileName(Application.dataPath), k_SamplesRootDirectoryName);

                    if (!Directory.Exists(sampleRootPath))
                    {
                        Debug.LogWarning($"Could not find Samples directory ({sampleRootPath}). Failed to update package sample cache.");
                        return;
                    }

                    // Iterate through all package directories in Samples directory
                    var allSamplePackagesPaths = Directory.GetDirectories(sampleRootPath);
                    foreach (var packageDirectoryPath in allSamplePackagesPaths)
                    {
                        var packageDisplayName = packageDirectoryPath.Split(delimiter).Last();

                        // To contain all samples for this package
                        var sampleMap = new Dictionary<string, SampleData>();

                        // Iterate through all version directories in package
                        var versionDirectoryPaths = Directory.GetDirectories(packageDirectoryPath);
                        foreach (var versionDirectoryPath in versionDirectoryPaths)
                        {
                            var versionName = versionDirectoryPath.Split(delimiter).Last();
                            var packageVersion = new PackageVersion(versionName);

                            // Iterate through all sample directories in version
                            var samplesDirectoryPaths = Directory.GetDirectories(versionDirectoryPath);
                            foreach (var sampleDirectoryPath in samplesDirectoryPaths)
                            {
                                var sampleName = sampleDirectoryPath.Split(delimiter).Last();
                                sampleMap.Add(sampleName, new SampleData
                                {
                                    sampleName = sampleName,
                                    packageDisplayName = packageDisplayName,
                                    packageVersion = packageVersion,
                                });
                            }
                        }

                        var packageCacheData = new PackageSampleData { packageDisplayName = packageDisplayName, importedSamples = sampleMap, };
                        s_PackageSampleCache[packageDisplayName] = packageCacheData;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to update package sample cache. " + e.Message);
                }
            }
        }
    }
}
