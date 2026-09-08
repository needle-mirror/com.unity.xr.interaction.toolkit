#if UIELEMENTS_MODULE_PRESENT && INPUT_SYSTEM_1_12_OR_NEWER
using System;
using UnityEditor.UIElements;
#endif
using UnityEngine;
using UnityEngine.InputSystem.Editor;
#if UIELEMENTS_MODULE_PRESENT && INPUT_SYSTEM_1_12_OR_NEWER
using UnityEngine.UIElements;
#endif
using UnityEngine.XR.Interaction.Toolkit.Inputs.Interactions;

namespace UnityEditor.XR.Interaction.Toolkit.Inputs.Interactions
{
    /// <summary>
    /// Custom editor for <see cref="SectorInteraction"/>.
    /// </summary>
    class SectorInteractionEditor : InputParameterEditor<SectorInteraction>
    {
        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            public static readonly GUIContent directionsLabel = EditorGUIUtility.TrTextContent("Directions",
                "Sets which cardinal directions to use when determining valid directions to perform the action.");
            public static readonly GUIContent sweepBehaviorLabel = EditorGUIUtility.TrTextContent("Sweep Behavior",
                "Determines when the action should perform or cancel when sweeping the stick around the cardinal directions without returning to center.");
            public static readonly GUIContent pressPointLabel = EditorGUIUtility.TrTextContent("Press Point",
                "Magnitude threshold that must be crossed by an actuated control for the control to be considered pressed.");
            public static readonly GUIContent defaultToggleLabel = EditorGUIUtility.TrTextContent("Default",
                "If enabled, the default value is used.");
        }

        /// <inheritdoc />
        public override void OnGUI()
        {
#if UIELEMENTS_MODULE_PRESENT && INPUT_SYSTEM_1_12_OR_NEWER
            // The UI Toolkit Input Actions editor renders parameters via OnDrawVisualElements but also
            // adds an IMGUIContainer that calls this OnGUI as a fallback for editors without a UITK path,
            // which would draw these fields a second time. Skip IMGUI when that editor is active so the
            // fields aren't duplicated; the legacy IMGUI editor still calls OnGUI when it is in use.
            // useIMGUIEditorForAssets was added in Input System 1.12.0. It was marked [Obsolete]
            // in Input System 1.17-1.18 (ISX-2397) then un-deprecated in 1.19; suppress CS0618 so
            // this still compiles clean against 1.17/1.18. While obsolete the property returns false,
            // so this early-return still prevents the fields from being drawn twice.
#pragma warning disable CS0618 // Type or member is obsolete -- Obsolete in Input System 1.17-1.18, un-deprecated in 1.19.
            if (!UnityEngine.InputSystem.InputSystem.settings.useIMGUIEditorForAssets)
                return;
#pragma warning restore CS0618
#endif
            target.directions = (SectorInteraction.Directions)EditorGUILayout.EnumFlagsField(Contents.directionsLabel, target.directions);

            target.sweepBehavior = (SectorInteraction.SweepBehavior)EditorGUILayout.EnumPopup(Contents.sweepBehaviorLabel, target.sweepBehavior);

            var useDefaultValue = target.pressPoint < 0f;

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(useDefaultValue);

            var newPressPoint = EditorGUILayout.Slider(Contents.pressPointLabel, target.pressPointOrDefault, 0f, 1f, GUILayout.ExpandWidth(false));
            if (!useDefaultValue)
            {
                target.pressPoint = newPressPoint;
            }

            EditorGUI.EndDisabledGroup();

            var newUseDefault = GUILayout.Toggle(useDefaultValue, Contents.defaultToggleLabel, GUILayout.ExpandWidth(false));
            if (newUseDefault != useDefaultValue)
            {
                target.pressPoint = newUseDefault ? -1f : SectorInteraction.defaultPressPoint;
            }

            EditorGUILayout.EndHorizontal();
        }

#if UIELEMENTS_MODULE_PRESENT && INPUT_SYSTEM_1_12_OR_NEWER
        /// <inheritdoc />
        public override void OnDrawVisualElements(VisualElement root, Action onChangedCallback)
        {
            // Directions
            var directionsDropdown = new EnumFlagsField(Contents.directionsLabel.text, target.directions)
            {
                tooltip = Contents.directionsLabel.tooltip,
            };
            directionsDropdown.RegisterValueChangedCallback(evt =>
            {
                target.directions = (SectorInteraction.Directions)evt.newValue;
                onChangedCallback?.Invoke();
            });

            // Sweep Behavior
            var sweepBehaviorDropdown = new EnumField(Contents.sweepBehaviorLabel.text, target.sweepBehavior)
            {
                tooltip = Contents.sweepBehaviorLabel.tooltip,
            };
            sweepBehaviorDropdown.RegisterValueChangedCallback(evt =>
            {
                target.sweepBehavior = (SectorInteraction.SweepBehavior)evt.newValue;
                onChangedCallback?.Invoke();
            });

            var useDefaultValue = target.pressPoint < 0f;

            // Press Point - Slider
            var pressPointSlider = new Slider(Contents.pressPointLabel.text, 0f, 1f)
            {
                value = target.pressPointOrDefault,
                showInputField = true,
                tooltip = Contents.pressPointLabel.tooltip,
                style =
                {
                    flexGrow = 1,
                },
            };
            pressPointSlider.RegisterValueChangedCallback(evt =>
            {
                target.pressPoint = evt.newValue;
                onChangedCallback?.Invoke();
            });
            pressPointSlider.SetEnabled(!useDefaultValue);

            // Press Point - Default toggle
            // Toggle.label is on the left.
            // Toggle.text is on the right.
            // Using Toggle.text so the style matches the IMGUI version and so the gap between the checkbox and text is minimal.
            var useDefaultToggle = new Toggle
            {
                value = useDefaultValue,
                text = Contents.defaultToggleLabel.text,
                tooltip = Contents.defaultToggleLabel.tooltip,
            };
            useDefaultToggle.RegisterValueChangedCallback(evt =>
            {
                target.pressPoint = evt.newValue ? -1f : SectorInteraction.defaultPressPoint;
                pressPointSlider.SetEnabled(!evt.newValue);
                onChangedCallback?.Invoke();
            });

            var pressPointContainer = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                },
            };

            root.Add(directionsDropdown);
            root.Add(sweepBehaviorDropdown);
            pressPointContainer.Add(pressPointSlider);
            pressPointContainer.Add(useDefaultToggle);
            root.Add(pressPointContainer);
        }
#endif
    }
}
