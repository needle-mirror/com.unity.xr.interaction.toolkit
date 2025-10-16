using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.XR.CoreUtils.Editor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.ProjectValidation
{
    /// <summary>
    /// Utility class to help with project validation for XR Interaction Toolkit and Samples.
    /// </summary>
    internal static class ProjectValidationUtility
    {
        const string k_SamplesRootDirectoryName = "Samples";
        const float k_PackageSearchTimeout = 3;

        /// <summary>
        /// This is the minimum version of the Starter Assets sample for the XR Interaction Toolkit that enforces correct behavior
        /// of newly added properties, prefabs, or input actions.
        /// </summary>
        public static readonly PackageVersion minimumXRIStarterAssetsSampleVersion = new PackageVersion("3.1.0");

        /// <summary>
        /// Dictionary used to cache packages imported samples. The dictionary key is the package display name as displayed in package Samples directory.
        /// </summary>
        static Dictionary<string, PackageSampleData> s_PackageSampleCache;

        struct PackageSampleData
        {
            public string packageDisplayName;
            public Dictionary<string, SampleData> importedSamples;
        }

        /// <summary>
        /// Struct containing data about a sample.
        /// </summary>
        struct SampleData
        {
            /// <summary>
            /// The display name of the sample, matches the directory.
            /// </summary>
            public string sampleName;

            /// <summary>
            /// The display name of the package the sample is from.
            /// </summary>
            public string packageDisplayName;

            /// <summary>
            /// The version of the package the sample is imported from.
            /// </summary>
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
        /// Searches for the imported samples from a specified package.
        /// </summary>
        /// <param name="packageDisplayName">The display name of the package (directory that contains the samples).</param>
        /// <param name="results">The list to populate with the unordered collection of imported samples.</param>
        public static void GetImportedSamples(string packageDisplayName, List<(string sampleName, PackageVersion packageVersion)> results)
        {
            results.Clear();

            if (s_PackageSampleCache == null)
            {
                UpdatePackageSampleCache();
                if (s_PackageSampleCache == null)
                    return;
            }

            if (s_PackageSampleCache.TryGetValue(packageDisplayName, out var sampleData))
            {
                foreach (var importedSample in sampleData.importedSamples)
                {
                    results.Add((importedSample.Value.sampleName, importedSample.Value.packageVersion));
                }
            }
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
        /// Searches for a sample, <see cref="sampleDisplayName"/> from a specified package, <see cref="packageDisplayName"/>
        /// and compares the sample version to a min and max range.
        /// </summary>
        /// <param name="packageDisplayName">The name of the package directory that contains the sample.</param>
        /// <param name="sampleDisplayName">The name of the sample directory to search for.</param>
        /// <param name="minVersion">The minimum package version the sample should be imported from.</param>
        /// <param name="maxVersion">The maximum package version the sample should be imported from, which can either include this version or only values less than this version.</param>
        /// <param name="isMaxInclusive">Whether or not the range includes the max version.</param>
        /// <returns>Returns <see langword="true"/> if the sample is found in the Samples directory of the specified
        /// package and the imported sample's version is within the range between the <see cref="minVersion"/> and the <see cref="maxVersion"/>.</returns>
        public static bool SampleImportMeetsVersionRange(string packageDisplayName, string sampleDisplayName, PackageVersion minVersion, PackageVersion maxVersion, bool isMaxInclusive)
        {
            if (HasSampleImported(packageDisplayName, sampleDisplayName))
            {
                var packageVersion = s_PackageSampleCache[packageDisplayName].importedSamples[sampleDisplayName].packageVersion;

                if (isMaxInclusive)
                    return packageVersion >= minVersion && packageVersion <= maxVersion;
                else
                    return packageVersion >= minVersion && packageVersion < maxVersion;
            }

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

                            try
                            {
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
                            catch (FormatException)
                            {
                                // We may see non-version strings in these subfolders as sample dependencies for other
                                // packages. We catch and ignore the PackageVersion constructor errors here so we can skip them.
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

        /// <summary>
        /// Attempts to install or update the package with a given package version.
        /// </summary>
        /// <param name="packageName">The name of the package to install (com.unity.etc...).</param>
        /// <param name="targetVersion">The target version of the package to install.</param>
        /// <param name="packageAddRequest">The <see cref="AddRequest"/> reference that will contain the result.</param>
        /// <remarks>The <see cref="packageName"/> parameter should be provided in the com.unity.package format. Package display names will not work.</remarks>
        public static void InstallOrUpdatePackage(string packageName, PackageVersion targetVersion, ref AddRequest packageAddRequest)
        {
            try
            {
                // Set a 3-second timeout for request to avoid editor lockup
                var currentTime = DateTime.Now;
                var endTime = currentTime + TimeSpan.FromSeconds(k_PackageSearchTimeout);

                var request = Client.Search(packageName);
                if (request.Status == StatusCode.InProgress)
                {
                    Debug.Log($"Searching for ({packageName}) in Unity Package Registry.");
                    while (request.Status == StatusCode.InProgress && currentTime < endTime)
                        currentTime = DateTime.Now;
                }

                var addRequest = packageName;
                if (request.Status == StatusCode.Success && request.Result.Length > 0)
                {
                    var versions = request.Result[0].versions;
#if UNITY_2022_2_OR_NEWER
                    var recommendedVersion = new PackageVersion(versions.recommended);
#else
                    var recommendedVersion = new PackageVersion(versions.verified);
#endif
                    var latestCompatible = new PackageVersion(versions.latestCompatible);
                    if (recommendedVersion < targetVersion && targetVersion <= latestCompatible)
                        addRequest = $"{packageName}@{targetVersion}";
                }

                packageAddRequest = Client.Add(addRequest);
                if (packageAddRequest.Error != null)
                {
                    Debug.LogError($"Package installation error: {packageAddRequest.Error}: {packageAddRequest.Error.message}");
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
