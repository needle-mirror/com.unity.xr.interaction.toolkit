using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Unity.XR.CoreUtils.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace UnityEditor.XR.Interaction.Toolkit.Editor.Tests
{
    /// <summary>
    /// This class contains tests to help identify potential property and method accessibility issues and
    /// potential namespace issues. It is left to the discretion of the developer to add any violations to
    /// appropriate exception list or to address the violation in the script.
    /// </summary>
    [TestFixture]
    class PackageScriptsTests
    {
        // Instructions to developers modifying exceptions lists:
        // - Use "All" to apply to all package versions.
        // - Use "3.0" or "3.1" etc. to apply the exception to a specific major.minor version.
        // This is used for new internal properties added in a 3.x patch that should be changed to public in 3.y.

        static readonly (string, string)[] s_NamespaceExceptionList =
        {
            ("All", "UnityEditor.XR.Interaction.Toolkit.Utilities.EditorComponentLocatorUtility"),
        };

        static readonly (string, string)[] s_InternalPropertyExceptionList =
        {
            ("All", "Property UnityEngine.XR.Interaction.Toolkit.Filtering.XRTargetFilter.evaluators has { internal get; }."),
            ("All", "Property UnityEngine.XR.Interaction.Toolkit.Filtering.XRTargetFilter.isProcessing has { internal get; private set; }."),
            ("All", "Property UnityEngine.XR.Interaction.Toolkit.Filtering.XRTargetFilter.linkedInteractors has { internal get; }."),
            ("All", "Property UnityEngine.XR.Interaction.Toolkit.Inputs.Interactions.SectorInteraction.pressPointOrDefault has { internal get; }."),
            ("All", "Property UnityEngine.XR.Interaction.Toolkit.Inputs.XRInputModalityManager.leftInputMode has { internal get; }."),
            ("All", "Property UnityEngine.XR.Interaction.Toolkit.Inputs.XRInputModalityManager.rightInputMode has { internal get; }."),
            ("All", "Property UnityEngine.XR.Interaction.Toolkit.Interactors.XRInteractionGroup.hasRegisteredStartingMembers has { internal get; private set; }."),
            ("All", "Property UnityEngine.XR.Interaction.Toolkit.Interactors.XRInteractionGroup.isRegisteredWithInteractionManager has { internal get; }."),
            ("All", "Property UnityEngine.XR.Interaction.Toolkit.Interactors.XRPokeInteractor.enableMultiPick has { internal get; internal set; }." ),
            ("All", "Property UnityEngine.XR.Interaction.Toolkit.Transformers.XRDualGrabFreeTransformer.lastInteractorAttachPose has { internal get; private set; }."),
            ("All", "Property UnityEngine.XR.Interaction.Toolkit.Transformers.XRSocketGrabTransformer.scaleOnlyMode has { internal get; internal set; }."),
            ("All", "Property UnityEngine.XR.Interaction.Toolkit.UI.TrackedDeviceEventData.pressWorldPosition has { internal get; internal set; }."),
        };

        static readonly (string, string)[] s_InternalMethodExceptionList =
        {
            ("All", "Method UnityEditor.XR.Interaction.Toolkit.Interactors.NearFarInteractorEditor.DrawSelectionConfigurationFoldout has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Theme.Audio.AudioAffordanceTheme.ValidateTheme has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Theme.BaseAffordanceTheme`1.ValidateTheme has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.AR.DragGesture.Reinitialize has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.AR.Gesture`1.Cancel has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.AR.Gesture`1.Reinitialize has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.AR.Gesture`1.Update has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.AR.PinchGesture.Reinitialize has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.AR.TapGesture.Reinitialize has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.AR.TwistGesture.Reinitialize has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.AR.TwoFingerDragGesture.Reinitialize has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.Filtering.XRTargetFilter.RegisterEvaluatorHandlers has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.Filtering.XRTargetFilter.UnregisterEvaluatorHandlers has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.SimulatedDeviceLifecycleManager.AddDevices has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.SimulatedDeviceLifecycleManager.ApplyControllerState has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.SimulatedDeviceLifecycleManager.ApplyHandState has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.SimulatedDeviceLifecycleManager.ApplyHMDState has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.SimulatedDeviceLifecycleManager.RemoveDevices has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.SimulatedDeviceLifecycleManager.SwitchDeviceMode has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.Interactors.XRPokeInteractor.UpdateUIRegistration has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.Interactors.NearFarInteractor.UpdateUIRegistration has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals.XRInteractorLineVisual.UpdateLineVisual has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.Interactors.XRInteractionGroup.RemoveMissingMembersFromStartingOverridesMap has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort.TunnelingVignetteController.PreviewInEditor has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.Locomotion.LocomotionMediator.TryEndLocomotion has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.Locomotion.LocomotionMediator.TryPrepareLocomotion has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.Locomotion.LocomotionMediator.TryStartLocomotion has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.Locomotion.LocomotionProvider.OnLocomotionStateChanging has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.Transformers.ARTransformer.MigratePlaneClassifications has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.Transformers.XRBaseGrabTransformer.GetRegistrationMode has internal."),
            ("All", "Method UnityEngine.XR.Interaction.Toolkit.XRControllerRecording.SetFrameDependentData has internal."),
        };

        static readonly (string, string)[] s_PrivatePropertyExceptionList =
        {
            ("All", "Property UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.XRDeviceSimulator.targetedDeviceInput has { private get; private set; }."),
            ("All", "Property UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable.isTransformDirty has { private get; private set; }."),
        };

        static readonly (string, string)[] s_EditorExceptionList =
        {
            ("All", "Field UnityEditor.XR.Interaction.Toolkit.Locomotion.Movement.GrabMoveProviderEditor.m_Reference has private instead of protected."),
            ("All", "Field UnityEditor.XR.Interaction.Toolkit.Locomotion.Movement.GrabMoveProviderEditor.m_SingletonActionBindings has private instead of protected."),
        };

        /// <summary>
        /// <c>major.minor</c> version of com.unity.xr.interaction.toolkit.
        /// </summary>
        string m_MajorMinorVersion;

        [OneTimeSetUp]
        public void SetUp()
        {
            var assembly = Assembly.Load("Unity.XR.Interaction.Toolkit");
            Assert.That(assembly, Is.Not.Null);

            PackageManager.PackageInfo packageInfo = PackageManager.PackageInfo.FindForAssembly(assembly);
            Assert.That(packageInfo, Is.Not.Null);

            // Parse the major.minor version
            Assert.That(packageInfo.version, Is.Not.Null);
            Assert.That(packageInfo.version, Is.Not.Empty);
            var secondDotIndex = packageInfo.version.IndexOf('.', packageInfo.version.IndexOf('.') + 1);
            Assert.That(secondDotIndex, Is.GreaterThan(0));
            m_MajorMinorVersion = packageInfo.version.Substring(0, secondDotIndex);
        }

        [UnityTest]
        public IEnumerator NamespaceMatchesAssembly()
        {
            var outputList = new HashSetList<string>();

            PopulateWrongNamespaceTypes(Assembly.Load("Unity.XR.Interaction.Toolkit"), "UnityEngine.XR.Interaction.Toolkit", outputList);
            PopulateWrongNamespaceTypes(Assembly.Load("Unity.XR.Interaction.Toolkit.Editor"), "UnityEditor.XR.Interaction.Toolkit", outputList);

            FilterExceptions(outputList, s_NamespaceExceptionList);

            Assert.That(outputList, Is.Empty, $"Contains {outputList.Count} incorrect namespaces that have not been excluded in {nameof(PackageScriptsTests)}.cs:\n" + string.Join("\n", outputList) + "\n");

            yield return null;
        }

        [UnityTest]
        public IEnumerator LimitInternalProperties()
        {
            var outputList = new HashSetList<string>();

            PopulateInternalProperties(Assembly.Load("Unity.XR.Interaction.Toolkit"), outputList);
            PopulateInternalProperties(Assembly.Load("Unity.XR.Interaction.Toolkit.Editor"), outputList);

            FilterExceptions(outputList, s_InternalPropertyExceptionList);

            Assert.That(outputList, Is.Empty, $"Contains {outputList.Count} internal properties that have not been excluded in {nameof(PackageScriptsTests)}.cs:\n" + string.Join("\n", outputList) + "\n");

            yield return null;
        }

        [UnityTest]
        public IEnumerator LimitInternalMethods()
        {
            var outputList = new HashSetList<string>();

            PopulateInternalMethods(Assembly.Load("Unity.XR.Interaction.Toolkit"), outputList);
            PopulateInternalMethods(Assembly.Load("Unity.XR.Interaction.Toolkit.Editor"), outputList);

            FilterExceptions(outputList, s_InternalMethodExceptionList);

            Assert.That(outputList, Is.Empty, $"Contains {outputList.Count} internal methods that have not been excluded in {nameof(PackageScriptsTests)}.cs:\n" + string.Join("\n", outputList) + "\n");

            yield return null;
        }

        [UnityTest]
        public IEnumerator LimitPrivateProperties()
        {
            var outputList = new HashSetList<string>();

            PopulatePrivateGetSetProperties(Assembly.Load("Unity.XR.Interaction.Toolkit"), outputList);
            PopulatePrivateGetSetProperties(Assembly.Load("Unity.XR.Interaction.Toolkit.Editor"), outputList);

            FilterExceptions(outputList, s_PrivatePropertyExceptionList);

            Assert.That(outputList, Is.Empty, $"Contains {outputList.Count} private properties that have not been excluded in {nameof(PackageScriptsTests)}.cs:\n" + string.Join("\n", outputList) + "\n");

            yield return null;
        }

        [UnityTest]
        public IEnumerator EditorSerializedPropertyAndContentFieldsShouldBeProtected()
        {
            var outputList = new HashSetList<string>();

            PopulateWrongSerializedPropertyAndContentFields(Assembly.Load("Unity.XR.Interaction.Toolkit.Editor"), outputList);

            FilterExceptions(outputList, s_EditorExceptionList);

            Assert.That(outputList, Is.Empty, $"Contains {outputList.Count} non-protected SerializedProperty/GUIContent fields that have not been excluded in {nameof(PackageScriptsTests)}.cs:\n" + string.Join("\n", outputList) + "\n");

            yield return null;
        }

        static void PopulateWrongNamespaceTypes(Assembly assembly, string rootNamespace, HashSetList<string> outputList)
        {
            if (assembly == null)
                Assert.Fail($"Could not load assembly.");

            foreach (var type in assembly.GetTypes())
            {
                if (type.FullName != null && type.Namespace != null)
                {
                    if (!type.Namespace.StartsWith(rootNamespace))
                        outputList.Add(type.FullName);
                }
            }
        }

        static void PopulateInternalProperties(Assembly assembly, HashSetList<string> outputList)
        {
            if (assembly == null)
                Assert.Fail($"Could not load assembly.");

            var publicClasses = assembly.GetExportedTypes()
                .Where(type => type.IsClass && type.IsPublic && !type.IsInterface)
                .OrderBy(type => type.FullName)
                .ToArray();

            foreach (var type in publicClasses)
            {
                // Get all properties that have internal get and/or set methods.
                // The main purpose of this test is for finding properties that should be public but are internal.
                // The goal is to minimize the number of internal properties, so each should be evaluated and added
                // to the exception list if it is determined that it should remain internal.
                var properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(property => property.PropertyType.IsPublic || property.PropertyType.IsNestedPublic)
                    .Where(property =>
                        property.GetMethod is { IsAssembly: true } ||
                        property.SetMethod is { IsAssembly: true })
                    .OrderBy(property => property.Name)
                    .ToArray();

                foreach (var property in properties)
                {
                    outputList.Add(GetStringOutput(type, property));
                }
            }
        }

        static void PopulateInternalMethods(Assembly assembly, HashSetList<string> outputList)
        {
            if (assembly == null)
                Assert.Fail($"Could not load assembly.");

            var publicClasses = assembly.GetExportedTypes()
                .Where(type => type.IsClass && type.IsPublic && !type.IsInterface)
                .OrderBy(type => type.FullName)
                .ToArray();

            foreach (var type in publicClasses)
            {
                // Get all methods that have internal protection.
                // The main purpose of this test is for finding methods that should be public/protected/private protected but are internal.
                // The goal is to minimize the number of internal methods, so each should be evaluated and added
                // to the exception list if it is determined that it should remain internal.
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(method => method.IsAssembly)
                    .Where(method =>
                        !method.Name.StartsWith("add_") &&
                        !method.Name.StartsWith("remove_") &&
                        !method.Name.StartsWith("get_") &&
                        !method.Name.StartsWith("set_") &&
                        !method.Name.StartsWith("Internal") &&
                        !method.Name.EndsWith("Internal"))
                    .OrderBy(method => method.Name)
                    .ToArray();

                foreach (var method in methods)
                {
                    outputList.Add(GetStringOutput(type, method));
                }
            }
        }

        static void PopulatePrivateGetSetProperties(Assembly assembly, HashSetList<string> outputList)
        {
            if (assembly == null)
                Assert.Fail($"Could not load assembly.");

            var publicClasses = assembly.GetExportedTypes()
                .Where(type => type.IsClass && type.IsPublic && !type.IsInterface)
                .OrderBy(type => type.FullName)
                .ToArray();

            foreach (var type in publicClasses)
            {
                // Get all properties that have both private get and set methods.
                // The main purpose of this test is for finding properties that should be public but are private,
                // especially those that are wrapping a SerializeField field that were introduced in patch versions.
                // Exclude explicit interface implementation methods by checking !IsFinal.
                var properties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(property =>
                        property.GetMethod is { IsPrivate: true, IsFinal: false } &&
                        property.SetMethod is { IsPrivate: true, IsFinal: false })
                    .OrderBy(property => property.Name)
                    .ToArray();

                foreach (var property in properties)
                {
                    outputList.Add(GetStringOutput(type, property));
                }
            }
        }

        static void PopulateWrongSerializedPropertyAndContentFields(Assembly assembly, HashSetList<string> outputList)
        {
            if (assembly == null)
                Assert.Fail($"Could not load assembly.");

            var editorClasses = assembly.GetExportedTypes()
                .Where(type => type.IsSubclassOf(typeof(UnityEditor.Editor)))
                .OrderBy(type => type.FullName)
                .ToArray();

            foreach (var type in editorClasses)
            {
                // Skip if the type is Obsolete.
                if (type.GetCustomAttribute<ObsoleteAttribute>() != null)
                    continue;

                // Get all SerializedProperty fields that are private or internal.
                // The main purpose of this test is for finding fields that should be protected instead,
                // especially those that were introduced in patch versions.
                var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(field =>
                        (field.FieldType == typeof(SerializedProperty)) &&
                        field is { IsPublic: false, IsFamily: false })
                    .OrderBy(property => property.Name)
                    .ToArray();

                foreach (var field in fields)
                {
                    outputList.Add(GetStringOutput(type, field));
                }

                // Get all GUIContent fields in nested Contents classes that are private or internal.
                var nestedTypes = type.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly)
                    .ToArray();
                foreach (var nestedType in nestedTypes)
                {
                    // Get the nested type's fields that are of type GUIContent.
                    var nestedFields = nestedType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly)
                        .Where(field =>
                            field.FieldType == typeof(GUIContent) &&
                            field is { IsPublic: false, IsFamily: false })
                        .OrderBy(property => property.Name)
                        .ToArray();

                    foreach (var nestedField in nestedFields)
                    {
                        outputList.Add(GetStringOutput(nestedType, nestedField));
                    }
                }
            }
        }

        static string GetAccessibility(MethodBase info)
        {
            if (info.IsPublic)
                return "public";
            if (info.IsFamily)
                return "protected";
            if (info.IsPrivate)
                return "private";
            if (info.IsAssembly)
                return "internal";
            if (info.IsFamilyOrAssembly)
                return "protected internal";
            if (info.IsFamilyAndAssembly)
                return "private protected";
            return "unknown";
        }

        static string GetAccessibility(FieldInfo info)
        {
            if (info.IsPublic)
                return "public";
            if (info.IsFamily)
                return "protected";
            if (info.IsPrivate)
                return "private";
            if (info.IsAssembly)
                return "internal";
            if (info.IsFamilyOrAssembly)
                return "protected internal";
            if (info.IsFamilyAndAssembly)
                return "private protected";
            return "unknown";
        }

        static string GetStringOutput(Type type, PropertyInfo property)
        {
            if (property.GetMethod != null && property.SetMethod != null)
                return $"Property {type.FullName}.{property.Name} has {{ {GetAccessibility(property.GetMethod)} get; {GetAccessibility(property.SetMethod)} set; }}.";
            if (property.GetMethod != null)
                return $"Property {type.FullName}.{property.Name} has {{ {GetAccessibility(property.GetMethod)} get; }}.";
            if (property.SetMethod != null)
                return $"Property {type.FullName}.{property.Name} has {{ {GetAccessibility(property.SetMethod)} set; }}.";
            return null;
        }

        static string GetStringOutput(Type type, FieldInfo field)
        {
            return $"Field {type.FullName}.{field.Name} has {GetAccessibility(field)} instead of protected.";
        }

        static string GetStringOutput(Type type, MethodInfo method)
        {
            return $"Method {type.FullName}.{method.Name} has {GetAccessibility(method)}.";
        }

        void FilterExceptions(HashSetList<string> outputList, (string, string)[] exceptionList)
        {
            // Many types are not defined without these #if guards defined, so avoid
            // failing the test due to "Stale exception" warnings due to the missing method on this platform.
#if ENABLE_VR || UNITY_GAMECORE
            var hasStaleException = false;
            foreach (var exception in exceptionList)
            {
                if (exception.Item1 == "All" || exception.Item1 == m_MajorMinorVersion)
                {
                    // Ignore AR types for now since those are optionally included with AR Foundation,
                    // so this avoids failing the test when that package is not installed.
                    if (!outputList.Contains(exception.Item2) && !exception.Item2.Contains(".AR"))
                    {
                        Debug.LogWarning($"Stale exception not found in source list: {exception.Item2}");
                        hasStaleException = true;
                    }
                }
            }

            Assert.That(hasStaleException, Is.False, "Stale exception not found in source list. See warnings.");
#endif

            outputList.ExceptWith(exceptionList.Where(e => e.Item1 == "All").Select(e => e.Item2));
            outputList.ExceptWith(exceptionList.Where(e => e.Item1 == m_MajorMinorVersion).Select(e => e.Item2));
        }
    }
}
