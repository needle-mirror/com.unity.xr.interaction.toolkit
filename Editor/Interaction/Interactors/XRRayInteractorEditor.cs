using System;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEditor.XR.Interaction.Toolkit.Interactors
{
    /// <summary>
    /// Custom editor for an <see cref="XRRayInteractor"/>.
    /// </summary>
    [MovedFrom("UnityEditor.XR.Interaction.Toolkit")]
    [CustomEditor(typeof(XRRayInteractor), true), CanEditMultipleObjects]
    public partial class XRRayInteractorEditor : XRBaseInputInteractorEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.maxRaycastDistance"/>.</summary>
        protected SerializedProperty m_MaxRaycastDistance;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.hitDetectionType"/>.</summary>
        protected SerializedProperty m_HitDetectionType;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.sphereCastRadius"/>.</summary>
        protected SerializedProperty m_SphereCastRadius;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.coneCastAngle"/>.</summary>
        protected SerializedProperty m_ConeCastAngle;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.raycastMask"/>.</summary>
        protected SerializedProperty m_RaycastMask;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.raycastTriggerInteraction"/>.</summary>
        protected SerializedProperty m_RaycastTriggerInteraction;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.raycastSnapVolumeInteraction"/>.</summary>
        protected SerializedProperty m_RaycastSnapVolumeInteraction;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.hitClosestOnly"/>.</summary>
        protected SerializedProperty m_HitClosestOnly;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.hoverToSelect"/>.</summary>
        protected SerializedProperty m_HoverToSelect;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.hoverTimeToSelect"/>.</summary>
        protected SerializedProperty m_HoverTimeToSelect;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.autoDeselect"/>.</summary>
        protected SerializedProperty m_AutoDeselect;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.timeToAutoDeselect"/>.</summary>
        protected SerializedProperty m_TimeToAutoDeselect;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.enableUIInteraction"/>.</summary>
        protected SerializedProperty m_EnableUIInteraction;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.blockUIOnInteractableSelection"/>.</summary>
        protected SerializedProperty m_BlockUIOnInteractableSelection;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.rayOriginTransform"/>.</summary>
        protected SerializedProperty m_RayOriginTransform;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.lineType"/>.</summary>
        protected SerializedProperty m_LineType;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.blendVisualLinePoints"/>.</summary>
        protected SerializedProperty m_BlendVisualLinePoints;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.endPointDistance"/>.</summary>
        protected SerializedProperty m_EndPointDistance;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.endPointHeight"/>.</summary>
        protected SerializedProperty m_EndPointHeight;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.controlPointDistance"/>.</summary>
        protected SerializedProperty m_ControlPointDistance;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.controlPointHeight"/>.</summary>
        protected SerializedProperty m_ControlPointHeight;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.sampleFrequency"/>.</summary>
        protected SerializedProperty m_SampleFrequency;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.velocity"/>.</summary>
        protected SerializedProperty m_Velocity;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.acceleration"/>.</summary>
        protected SerializedProperty m_Acceleration;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.additionalGroundHeight"/>.</summary>
        protected SerializedProperty m_AdditionalGroundHeight;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.additionalFlightTime"/>.</summary>
        protected SerializedProperty m_AdditionalFlightTime;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.referenceFrame"/>.</summary>
        protected SerializedProperty m_ReferenceFrame;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.manipulateAttachTransform"/>.</summary>
        protected SerializedProperty m_ManipulateAttachTransform;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.useForceGrab"/>.</summary>
        protected SerializedProperty m_UseForceGrab;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.rotateSpeed"/>.</summary>
        protected SerializedProperty m_RotateSpeed;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.translateSpeed"/>.</summary>
        protected SerializedProperty m_TranslateSpeed;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.rotateReferenceFrame"/>.</summary>
        protected SerializedProperty m_RotateReferenceFrame;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.rotateMode"/>.</summary>
        protected SerializedProperty m_RotateMode;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.scaleMode"/>.</summary>
        protected SerializedProperty m_ScaleMode;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.uiPressInput"/>.</summary>
        protected SerializedProperty m_UIPressInput;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.uiScrollInput"/>.</summary>
        protected SerializedProperty m_UIScrollInput;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.translateManipulationInput"/>.</summary>
        protected SerializedProperty m_TranslateManipulationInput;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.rotateManipulationInput"/>.</summary>
        protected SerializedProperty m_RotateManipulationInput;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.directionalManipulationInput"/>.</summary>
        protected SerializedProperty m_DirectionalManipulationInput;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.scaleToggleInput"/>.</summary>
        protected SerializedProperty m_ScaleToggleInput;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.scaleOverTimeInput"/>.</summary>
        protected SerializedProperty m_ScaleOverTimeInput;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.scaleDistanceDeltaInput"/>.</summary>
        protected SerializedProperty m_ScaleDistanceDeltaInput;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.uiHoverEntered"/>.</summary>
        protected SerializedProperty m_UIHoverEntered;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.uiHoverExited"/>.</summary>
        protected SerializedProperty m_UIHoverExited;

        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.enableARRaycasting"/>.</summary>
        protected SerializedProperty m_EnableARRaycasting;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.occludeARHitsWith3DObjects"/>.</summary>
        protected SerializedProperty m_OccludeARHitsWith3DObjects;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRRayInteractor.occludeARHitsWith2DObjects"/>.</summary>
        protected SerializedProperty m_OccludeARHitsWith2DObjects;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.maxRaycastDistance"/>.</summary>
            public static readonly GUIContent maxRaycastDistance = EditorGUIUtility.TrTextContent("Max Raycast Distance", "Max distance of ray cast. Increase this value will let you reach further.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.sphereCastRadius"/>.</summary>
            public static readonly GUIContent sphereCastRadius = EditorGUIUtility.TrTextContent("Sphere Cast Radius", "Radius of this Interactor's ray, used for sphere casting.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.coneCastAngle"/>.</summary>
            public static readonly GUIContent coneCastAngle = EditorGUIUtility.TrTextContent("Cone Cast Angle", "Angle in degrees of the interactor's selection cone, used for cone casting.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.raycastMask"/>.</summary>
            public static readonly GUIContent raycastMask = EditorGUIUtility.TrTextContent("Raycast Mask", "Layer mask used for limiting ray cast targets.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.raycastTriggerInteraction"/>.</summary>
            public static readonly GUIContent raycastTriggerInteraction = EditorGUIUtility.TrTextContent("Raycast Trigger Interaction", "Type of interaction with trigger colliders via ray cast.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.raycastSnapVolumeInteraction"/>.</summary>
            public static readonly GUIContent raycastSnapVolumeInteraction = EditorGUIUtility.TrTextContent("Raycast Snap Volume Interaction", "Whether ray cast should include or ignore hits on trigger colliders that are snap volume colliders, even if the ray cast is set to ignore triggers.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.hitClosestOnly"/>.</summary>
            public static readonly GUIContent hitClosestOnly = EditorGUIUtility.TrTextContent("Hit Closest Only", "Consider only the closest Interactable as a valid target for interaction. Enable this to make only the closest Interactable receive hover events.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.hoverToSelect"/>.</summary>
            public static readonly GUIContent hoverToSelect = EditorGUIUtility.TrTextContent("Hover To Select", "Automatically select an Interactable after hovering over it for a period of time.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.hoverTimeToSelect"/>.</summary>
            public static readonly GUIContent hoverTimeToSelect = EditorGUIUtility.TrTextContent("Hover Time To Select", "Number of seconds for which this Interactor must hover over an Interactable to select it.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.autoDeselect"/>.</summary>
            public static readonly GUIContent autoDeselect = EditorGUIUtility.TrTextContent("Auto Deselect", "Automatically deselect an Interactable after selecting it via hover to select for a period of time.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.timeToAutoDeselect"/>.</summary>
            public static readonly GUIContent timeToAutoDeselect = EditorGUIUtility.TrTextContent("Time To Auto Deselect", "Number of seconds for which this Interactor will select an Interactable before the Interactable is automatically deselected.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.enableUIInteraction"/>.</summary>
            public static readonly GUIContent enableUIInteraction = EditorGUIUtility.TrTextContent("UI Interaction", "Enable to affect Unity UI GameObjects in a way that is similar to a mouse pointer. Requires the XR UI Input Module on the Event System.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.blockUIOnInteractableSelection"/>.</summary>
            public static readonly GUIContent blockUIOnInteractableSelection = EditorGUIUtility.TrTextContent("Block UI on Interactable Selection", "Enabling this option will block UI interaction when selecting interactables.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.rayOriginTransform"/>.</summary>
            public static readonly GUIContent rayOriginTransform = EditorGUIUtility.TrTextContent("Ray Origin Transform", "The starting position and direction of any ray casts. If not set at startup, it will automatically be created based on the Attach Transform.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.lineType"/>.</summary>
            public static readonly GUIContent lineType = EditorGUIUtility.TrTextContent("Line Type", "Line type of the ray cast.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.blendVisualLinePoints"/>.</summary>
            public static readonly GUIContent blendVisualLinePoints = EditorGUIUtility.TrTextContent("Blend Visual Line Points", "Blend the line sample points used for ray casting with the current pose of the controller. Use this to make the line visual stay connected with the controller instead of lagging behind.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.endPointDistance"/>.</summary>
            public static readonly GUIContent endPointDistance = EditorGUIUtility.TrTextContent("End Point Distance", "Increase this value distance will make the end of curve further from the start point.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.controlPointDistance"/>.</summary>
            public static readonly GUIContent controlPointDistance = EditorGUIUtility.TrTextContent("Control Point Distance", "Increase this value will make the peak of the curve further from the start point.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.endPointHeight"/>.</summary>
            public static readonly GUIContent endPointHeight = EditorGUIUtility.TrTextContent("End Point Height", "Decrease this value will make the end of the curve drop lower relative to the start point.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.controlPointHeight"/>.</summary>
            public static readonly GUIContent controlPointHeight = EditorGUIUtility.TrTextContent("Control Point Height", "Increase this value will make the peak of the curve higher relative to the start point.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.sampleFrequency"/>.</summary>
            public static readonly GUIContent sampleFrequency = EditorGUIUtility.TrTextContent("Sample Frequency", "The number of sample points used to approximate curved paths. Larger values produce a better quality approximate at the cost of reduced performance due to the number of ray casts.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.velocity"/>.</summary>
            public static readonly GUIContent velocity = EditorGUIUtility.TrTextContent("Velocity", "Initial velocity of the projectile. Increase this value will make the curve reach further.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.acceleration"/>.</summary>
            public static readonly GUIContent acceleration = EditorGUIUtility.TrTextContent("Acceleration", "Gravity of the projectile in the reference frame.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.additionalGroundHeight"/>.</summary>
            public static readonly GUIContent additionalGroundHeight = EditorGUIUtility.TrTextContent("Additional Ground Height", "Additional height below ground level that the projectile will continue to. Increasing this value will make the end point drop lower in height.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.additionalFlightTime"/>.</summary>
            public static readonly GUIContent additionalFlightTime = EditorGUIUtility.TrTextContent("Additional Flight Time", "Additional flight time after the projectile lands at the adjusted ground level. Increasing this value will make the end point drop lower in height.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.referenceFrame"/>.</summary>
            public static readonly GUIContent referenceFrame = EditorGUIUtility.TrTextContent("Reference Frame", "The reference frame of the curve to define the ground plane and up. If not set at startup it will try to find the Rig GameObject, and if that does not exist it will use global up and origin by default.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.hitDetectionType"/>.</summary>
            public static readonly GUIContent hitDetectionType = EditorGUIUtility.TrTextContent("Hit Detection Type", "The type of hit detection used to hit interactable objects.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.manipulateAttachTransform"/>.</summary>
            public static readonly GUIContent manipulateAttachTransform = EditorGUIUtility.TrTextContent("Manipulate Attach Transform", "Allows the user to move the Attach Transform using the thumbstick.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.useForceGrab"/>.</summary>
            public static readonly GUIContent useForceGrab = EditorGUIUtility.TrTextContent("Force Grab", "Force grab moves the object to your hand rather than interacting with it at a distance.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.rotateSpeed"/>.</summary>
            public static readonly GUIContent rotateSpeed = EditorGUIUtility.TrTextContent("Rotate Speed", "Speed that the Attach Transform is rotated.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.translateSpeed"/>.</summary>
            public static readonly GUIContent translateSpeed = EditorGUIUtility.TrTextContent("Translate Speed", "Speed that the Attach Transform is translated along the ray.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.rotateReferenceFrame"/>.</summary>
            public static readonly GUIContent rotateReferenceFrame = EditorGUIUtility.TrTextContent("Rotate Reference Frame", "The optional reference frame to define the up axis when rotating the Attach Transform. When not set, rotates about the local up axis of the Attach Transform.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.rotateMode"/>.</summary>
            public static readonly GUIContent rotateMode = EditorGUIUtility.TrTextContent("Rotate Mode", "How the Attach Transform rotation manipulation is controlled.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.scaleMode"/>.</summary>
            public static readonly GUIContent scaleMode = EditorGUIUtility.TrTextContent("Scale Mode", "Determines how the Scale Value should be used by the interactable objects requesting it.");

            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.uiPressInput"/>.</summary>
            public static readonly GUIContent uiPressInput = EditorGUIUtility.TrTextContent("UI Press Input", "Input to use for pressing UI elements. Functions like a mouse button when pointing over UI.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.uiScrollInput"/>.</summary>
            public static readonly GUIContent uiScrollInput = EditorGUIUtility.TrTextContent("UI Scroll Input", "Input to use for scrolling UI elements. Functions like a mouse scroll wheel when pointing over UI.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.translateManipulationInput"/>.</summary>
            public static readonly GUIContent translateManipulationInput = EditorGUIUtility.TrTextContent("Translate Input", "Input to use for translating the attach point closer or further away from the interactor. This effectively moves the selected grab interactable along the ray.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.rotateManipulationInput"/>.</summary>
            public static readonly GUIContent rotateManipulationInput = EditorGUIUtility.TrTextContent("Rotate Input", "Input to use for rotating the attach point over time. This effectively rotates the selected grab interactable while the input is pushed in either direction.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.directionalManipulationInput"/>.</summary>
            public static readonly GUIContent directionalManipulationInput = EditorGUIUtility.TrTextContent("Directional Input", "Input to use for rotating the attach point to match the direction of the input. This effectively rotates the selected grab interactable or teleport target to match the direction of the input.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.scaleToggleInput"/>.</summary>
            public static readonly GUIContent scaleToggleInput = EditorGUIUtility.TrTextContent("Scale Toggle Input", "The input to use for toggling between Attach Transform manipulation modes to either scale or translate/rotate.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.scaleOverTimeInput"/>.</summary>
            public static readonly GUIContent scaleOverTimeInput = EditorGUIUtility.TrTextContent("Scale Over Time Input", "The input to use for providing a scale value to grab transformers for scaling over time. This effectively scales the selected grab interactable while the input is pushed in either direction.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.scaleDistanceDeltaInput"/>.</summary>
            public static readonly GUIContent scaleDistanceDeltaInput = EditorGUIUtility.TrTextContent("Scale Distance Delta Input", "The input to use for providing a scale value to grab transformers for scaling based on a distance delta from last frame. This input is typically used for scaling with a pinch gesture on mobile AR.");

            /// <summary><see cref="GUIContent"/> for the header label of UI events.</summary>
            public static readonly GUIContent uiEventsHeader = EditorGUIUtility.TrTextContent("UI", "Called when this Interactor begins hovering over UI (Entered), or ends hovering (Exited).");
      
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.enableARRaycasting"/>.</summary>
            public static readonly GUIContent enableARRaycasting = EditorGUIUtility.TrTextContent("Enable AR Raycasting", "Allow raycasts against the AR environment trackables.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.occludeARHitsWith3DObjects"/>.</summary>
            public static readonly GUIContent occludeARHitsWith3DObjects = EditorGUIUtility.TrTextContent("Occlude AR Hits With 3D Objects", "If enabled, AR Raycasts will be occluded by 3D objects.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.occludeARHitsWith2DObjects"/>.</summary>
            public static readonly GUIContent occludeARHitsWith2DObjects = EditorGUIUtility.TrTextContent("Occlude AR Hits With 2D Objects", "If enabled, AR Raycasts will be occluded by 2D world space objects.");

            /// <summary>The help box message when the hit detection type is cone cast but the line type is not straight line.</summary>
            public static readonly GUIContent coneCastRequiresStraightLineWarning = EditorGUIUtility.TrTextContent("Cone Cast requires Straight Line.");

            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.manipulateAttachTransform"/>.</summary>
            [Obsolete("allowAnchorControl has been renamed in version 3.0.0. Use manipulateAttachTransform instead.")]
            public static readonly GUIContent allowAnchorControl = manipulateAttachTransform;

            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.rotateReferenceFrame"/>.</summary>
            [Obsolete("anchorRotateReferenceFrame has been renamed in version 3.0.0. Use rotateReferenceFrame instead.")]
            public static readonly GUIContent anchorRotateReferenceFrame = rotateReferenceFrame;

            /// <summary><see cref="GUIContent"/> for <see cref="XRRayInteractor.rotateMode"/>.</summary>
            [Obsolete("anchorRotationMode has been renamed in version 3.0.0. Use rotateMode instead.")]
            public static readonly GUIContent anchorRotationMode = rotateMode;
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            m_MaxRaycastDistance = serializedObject.FindProperty("m_MaxRaycastDistance");
            m_HitDetectionType = serializedObject.FindProperty("m_HitDetectionType");
            m_SphereCastRadius = serializedObject.FindProperty("m_SphereCastRadius");
            m_ConeCastAngle = serializedObject.FindProperty("m_ConeCastAngle");
            m_RaycastMask = serializedObject.FindProperty("m_RaycastMask");
            m_RaycastTriggerInteraction = serializedObject.FindProperty("m_RaycastTriggerInteraction");
            m_RaycastSnapVolumeInteraction = serializedObject.FindProperty("m_RaycastSnapVolumeInteraction");
            m_HitClosestOnly = serializedObject.FindProperty("m_HitClosestOnly");
            m_HoverToSelect = serializedObject.FindProperty("m_HoverToSelect");
            m_HoverTimeToSelect = serializedObject.FindProperty("m_HoverTimeToSelect");
            m_AutoDeselect = serializedObject.FindProperty("m_AutoDeselect");
            m_TimeToAutoDeselect = serializedObject.FindProperty("m_TimeToAutoDeselect");
            m_EnableUIInteraction = serializedObject.FindProperty("m_EnableUIInteraction");
            m_BlockUIOnInteractableSelection = serializedObject.FindProperty("m_BlockUIOnInteractableSelection");
            m_RayOriginTransform = serializedObject.FindProperty("m_RayOriginTransform");

            m_LineType = serializedObject.FindProperty("m_LineType");
            m_BlendVisualLinePoints = serializedObject.FindProperty("m_BlendVisualLinePoints");
            m_EndPointDistance = serializedObject.FindProperty("m_EndPointDistance");
            m_EndPointHeight = serializedObject.FindProperty("m_EndPointHeight");
            m_ControlPointDistance = serializedObject.FindProperty("m_ControlPointDistance");
            m_ControlPointHeight = serializedObject.FindProperty("m_ControlPointHeight");
            m_SampleFrequency = serializedObject.FindProperty("m_SampleFrequency");

            m_Velocity = serializedObject.FindProperty("m_Velocity");
            m_Acceleration = serializedObject.FindProperty("m_Acceleration");
            m_AdditionalGroundHeight = serializedObject.FindProperty("m_AdditionalGroundHeight");
            m_AdditionalFlightTime = serializedObject.FindProperty("m_AdditionalFlightTime");
            m_ReferenceFrame = serializedObject.FindProperty("m_ReferenceFrame");

            m_ManipulateAttachTransform = serializedObject.FindProperty("m_ManipulateAttachTransform");
            m_UseForceGrab = serializedObject.FindProperty("m_UseForceGrab");

            m_RotateSpeed = serializedObject.FindProperty("m_RotateSpeed");
            m_TranslateSpeed = serializedObject.FindProperty("m_TranslateSpeed");
            m_RotateReferenceFrame = serializedObject.FindProperty("m_RotateReferenceFrame");
            m_RotateMode = serializedObject.FindProperty("m_RotateMode");
            m_ScaleMode = serializedObject.FindProperty("m_ScaleMode");

            m_UIPressInput = serializedObject.FindProperty("m_UIPressInput");
            m_UIScrollInput = serializedObject.FindProperty("m_UIScrollInput");
            m_TranslateManipulationInput = serializedObject.FindProperty("m_TranslateManipulationInput");
            m_RotateManipulationInput = serializedObject.FindProperty("m_RotateManipulationInput");
            m_DirectionalManipulationInput = serializedObject.FindProperty("m_DirectionalManipulationInput");
            m_ScaleToggleInput = serializedObject.FindProperty("m_ScaleToggleInput");
            m_ScaleOverTimeInput = serializedObject.FindProperty("m_ScaleOverTimeInput");
            m_ScaleDistanceDeltaInput = serializedObject.FindProperty("m_ScaleDistanceDeltaInput");

            m_UIHoverEntered = serializedObject.FindProperty("m_UIHoverEntered");
            m_UIHoverExited = serializedObject.FindProperty("m_UIHoverExited");      
            m_EnableARRaycasting = serializedObject.FindProperty("m_EnableARRaycasting");
            m_OccludeARHitsWith3DObjects = serializedObject.FindProperty("m_OccludeARHitsWith3DObjects");
            m_OccludeARHitsWith2DObjects = serializedObject.FindProperty("m_OccludeARHitsWith2DObjects");

#pragma warning disable CS0618 // Type or member is obsolete -- For backwards compatibility with existing projects
            m_AllowAnchorControl = m_ManipulateAttachTransform;
            m_AnchorRotateReferenceFrame = m_RotateReferenceFrame;
            m_AnchorRotationMode = m_RotateMode;
#pragma warning restore CS0618

            // Set default expanded for some foldouts
            const string initializedKey = "XRI." + nameof(XRRayInteractorEditor) + ".Initialized";
            if (!SessionState.GetBool(initializedKey, false))
            {
                SessionState.SetBool(initializedKey, true);
                m_EnableARRaycasting.isExpanded = true;
            }
        }

        /// <inheritdoc />
        protected override void DrawProperties()
        {
            // Not calling base method to completely override drawn properties

            DrawInteractionManagement();
            DrawInteractionConfiguration();

            EditorGUILayout.Space();

            DrawRaycastConfiguration();

            EditorGUILayout.Space();

            DrawSelectionConfiguration();

#if AR_FOUNDATION_PRESENT
            EditorGUILayout.Space();
            
            DrawARConfiguration();
#endif

            EditorGUILayout.Space();

            DrawInputConfiguration();
        }

        /// <inheritdoc />
        protected override void DrawDerivedProperties()
        {
            EditorGUILayout.Space();
            base.DrawDerivedProperties();
        }

        /// <summary>
        /// Draw the property fields related to interaction configuration.
        /// </summary>
        protected virtual void DrawInteractionConfiguration()
        {
            EditorGUILayout.PropertyField(m_Handedness, BaseContents.handedness);
            EditorGUILayout.PropertyField(m_EnableUIInteraction, Contents.enableUIInteraction);
            if (m_EnableUIInteraction.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_BlockUIOnInteractableSelection, Contents.blockUIOnInteractableSelection);
                    DrawUIInteractionInputConfiguration();
                }
            }

            EditorGUILayout.PropertyField(m_UseForceGrab, Contents.useForceGrab);
            EditorGUILayout.PropertyField(m_ManipulateAttachTransform, Contents.manipulateAttachTransform);
            if (m_ManipulateAttachTransform.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_TranslateSpeed, Contents.translateSpeed);
                    EditorGUILayout.PropertyField(m_RotateMode, Contents.rotateMode);
                    if (m_RotateMode.intValue == (int)XRRayInteractor.RotateMode.RotateOverTime)
                    {
                        using (new EditorGUI.IndentLevelScope())
                        {
                            EditorGUILayout.PropertyField(m_RotateSpeed, Contents.rotateSpeed);
                        }
                    }

                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(m_RotateReferenceFrame, Contents.rotateReferenceFrame);
                    }

                    EditorGUILayout.PropertyField(m_ScaleMode, Contents.scaleMode);

                    DrawAttachTransformManipulationInputConfiguration();
                }
            }

            EditorGUILayout.PropertyField(m_AttachTransform, BaseContents.attachTransform);
            EditorGUILayout.PropertyField(m_RayOriginTransform, Contents.rayOriginTransform);
            EditorGUILayout.PropertyField(m_DisableVisualsWhenBlockedInGroup, BaseContents.disableVisualsWhenBlockedInGroup);
        }

        /// <summary>
        /// Draw the Raycast Configuration foldout.
        /// </summary>
        /// <seealso cref="DrawRaycastConfigurationNested"/>
        protected virtual void DrawRaycastConfiguration()
        {
            m_LineType.isExpanded = EditorGUILayout.Foldout(m_LineType.isExpanded, EditorGUIUtility.TrTempContent("Raycast Configuration"), true);
            if (m_LineType.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    DrawRaycastConfigurationNested();
                }
            }
        }

        /// <summary>
        /// Draw the nested contents of the Raycast Configuration foldout.
        /// </summary>
        /// <seealso cref="DrawRaycastConfiguration"/>
        protected virtual void DrawRaycastConfigurationNested()
        {
            EditorGUILayout.PropertyField(m_LineType, Contents.lineType);

            if (!m_LineType.hasMultipleDifferentValues &&
                !m_HitDetectionType.hasMultipleDifferentValues &&
                m_LineType.intValue != (int)XRRayInteractor.LineType.StraightLine &&
                m_HitDetectionType.intValue == (int)XRRayInteractor.HitDetectionType.ConeCast)
            {
                EditorGUILayout.HelpBox(Contents.coneCastRequiresStraightLineWarning.text, MessageType.Warning);
            }

            using (new EditorGUI.IndentLevelScope())
            {
                switch (m_LineType.intValue)
                {
                    case (int)XRRayInteractor.LineType.StraightLine:
                        EditorGUILayout.PropertyField(m_MaxRaycastDistance, Contents.maxRaycastDistance);
                        break;
                    case (int)XRRayInteractor.LineType.ProjectileCurve:
                        EditorGUILayout.PropertyField(m_ReferenceFrame, Contents.referenceFrame);
                        EditorGUILayout.PropertyField(m_Velocity, Contents.velocity);
                        EditorGUILayout.PropertyField(m_Acceleration, Contents.acceleration);
                        EditorGUILayout.PropertyField(m_AdditionalGroundHeight, Contents.additionalGroundHeight);
                        EditorGUILayout.PropertyField(m_AdditionalFlightTime, Contents.additionalFlightTime);
                        EditorGUILayout.PropertyField(m_SampleFrequency, Contents.sampleFrequency);
                        break;
                    case (int)XRRayInteractor.LineType.BezierCurve:
                        EditorGUILayout.PropertyField(m_ReferenceFrame, Contents.referenceFrame);
                        EditorGUILayout.PropertyField(m_EndPointDistance, Contents.endPointDistance);
                        EditorGUILayout.PropertyField(m_EndPointHeight, Contents.endPointHeight);
                        EditorGUILayout.PropertyField(m_ControlPointDistance, Contents.controlPointDistance);
                        EditorGUILayout.PropertyField(m_ControlPointHeight, Contents.controlPointHeight);
                        EditorGUILayout.PropertyField(m_SampleFrequency, Contents.sampleFrequency);
                        break;
                }
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_RaycastMask, Contents.raycastMask);
            EditorGUILayout.PropertyField(m_RaycastTriggerInteraction, Contents.raycastTriggerInteraction);
            EditorGUILayout.PropertyField(m_RaycastSnapVolumeInteraction, Contents.raycastSnapVolumeInteraction);
            EditorGUILayout.PropertyField(m_HitDetectionType, Contents.hitDetectionType);
            switch (m_HitDetectionType.intValue)
            {
                case (int)XRRayInteractor.HitDetectionType.SphereCast:
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(m_SphereCastRadius, Contents.sphereCastRadius);
                    }

                    break;
                }
                case (int)XRRayInteractor.HitDetectionType.ConeCast:
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(m_ConeCastAngle, Contents.coneCastAngle);
                    }

                    break;
                }
            }
            EditorGUILayout.PropertyField(m_HitClosestOnly, Contents.hitClosestOnly);
            EditorGUILayout.PropertyField(m_BlendVisualLinePoints, Contents.blendVisualLinePoints);
        }

        /// <summary>
        /// Draw the Selection Configuration foldout.
        /// </summary>
        /// <seealso cref="DrawSelectionConfigurationNested"/>
        protected virtual void DrawSelectionConfiguration()
        {
            m_SelectActionTrigger.isExpanded = EditorGUILayout.Foldout(m_SelectActionTrigger.isExpanded, EditorGUIUtility.TrTempContent("Selection Configuration"), true);
            if (m_SelectActionTrigger.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    DrawSelectionConfigurationNested();
                }
            }
        }

        /// <summary>
        /// Draw the nested contents of the Selection Configuration foldout.
        /// </summary>
        /// <seealso cref="DrawSelectionConfiguration"/>
        protected virtual void DrawSelectionConfigurationNested()
        {
            DrawSelectActionTrigger();
            EditorGUILayout.PropertyField(m_KeepSelectedTargetValid, BaseContents.keepSelectedTargetValid);
            EditorGUILayout.PropertyField(m_AllowHoveredActivate, BaseInputContents.allowHoveredActivate);
            EditorGUILayout.PropertyField(m_HoverToSelect, Contents.hoverToSelect);
            if (m_HoverToSelect.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_HoverTimeToSelect, Contents.hoverTimeToSelect);
                }
                EditorGUILayout.PropertyField(m_AutoDeselect, Contents.autoDeselect);
                if (m_AutoDeselect.boolValue)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(m_TimeToAutoDeselect, Contents.timeToAutoDeselect);
                    }
                }
            }
            EditorGUILayout.PropertyField(m_TargetPriorityMode, BaseInputContents.targetPriorityMode);
            EditorGUILayout.PropertyField(m_StartingSelectedInteractable, BaseContents.startingSelectedInteractable);
        }

        /// <summary>
        /// Draw the input configuration for UI interactions.
        /// </summary>
        protected virtual void DrawUIInteractionInputConfiguration()
        {
            EditorGUILayout.PropertyField(m_UIPressInput, Contents.uiPressInput);
            EditorGUILayout.PropertyField(m_UIScrollInput, Contents.uiScrollInput);
        }

        /// <summary>
        /// Draw the input configuration for Attach Transform Manipulation.
        /// </summary>
        protected virtual void DrawAttachTransformManipulationInputConfiguration()
        {
            EditorGUILayout.PropertyField(m_TranslateManipulationInput, Contents.translateManipulationInput);
            EditorGUILayout.PropertyField(m_RotateManipulationInput, Contents.rotateManipulationInput);
            EditorGUILayout.PropertyField(m_DirectionalManipulationInput, Contents.directionalManipulationInput);
            EditorGUILayout.PropertyField(m_ScaleToggleInput, Contents.scaleToggleInput);
            EditorGUILayout.PropertyField(m_ScaleOverTimeInput, Contents.scaleOverTimeInput);
            EditorGUILayout.PropertyField(m_ScaleDistanceDeltaInput, Contents.scaleDistanceDeltaInput);
        }

        /// <inheritdoc />
        protected override void DrawInteractorEventsNested()
        {
            base.DrawInteractorEventsNested();

            EditorGUILayout.LabelField(Contents.uiEventsHeader, EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_UIHoverEntered);
            EditorGUILayout.PropertyField(m_UIHoverExited);
        }

        /// <summary>
        /// Draw the AR Configuration properties.
        /// </summary>
        protected virtual void DrawARConfiguration()
        {
            m_EnableARRaycasting.isExpanded = EditorGUILayout.Foldout(m_EnableARRaycasting.isExpanded, EditorGUIUtility.TrTempContent("AR Configuration"), true);
            if (m_EnableARRaycasting.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(m_EnableARRaycasting, Contents.enableARRaycasting);
                    EditorGUILayout.PropertyField(m_OccludeARHitsWith3DObjects, Contents.occludeARHitsWith3DObjects);
                    EditorGUILayout.PropertyField(m_OccludeARHitsWith2DObjects, Contents.occludeARHitsWith2DObjects);
                }
            }
        }
    }
}