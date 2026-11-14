using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.InputSystem.XR;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Utilities;


#if XR_HANDS_1_5_OR_NEWER
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.XRHandLocomotion
{
    /// <summary>
    /// Evaluates the hand joints/bones, or an activate input such as a thumb tap, to control the state of the XRRayInteractor.
    /// In hand shape teleport mode, a pinch teleports and joystick pinch input controls the teleport reticle.
    /// In microgesture teleport mode, the teleport input teleports and the locomotion providers are enabled.
    /// </summary>
    public class TeleportHandGesture : MonoBehaviour
    {
        [Header("Hand Shape Teleport Mode")]
#if XR_HANDS_1_5_OR_NEWER
        /// <summary>
        /// The XRHandshape used to activate the display of the teleport visuals
        /// </summary>
        [SerializeField]
        [Tooltip("The XRHandShape used to activate teleport mode.")]
        XRHandShape m_ActivateHandShape;

        /// <summary>
        /// Detect an activate gesture based on any user defined inspector conditions applied to the ActivateHandShape
        /// </summary>
        [SerializeField]
        XRHandRelativeOrientation m_ActivateHandOrientation;
#endif

        /// <summary>
        /// The controller that handles the joystick input mode enabled when pinching while the teleport visuals are activated
        /// This controller handles the rotation targeting of the teleport ray reticle
        /// </summary>
        [SerializeField]
        JoystickPinchHandGesture m_JoystickPinchHandGesture;

        /// <summary>
        /// The locomotion providers enabled only in microgesture teleport mode, such as the Snap Turn Provider and Step Move Provider.
        /// </summary>
        [Header("Microgesture Teleport Mode")]
        [SerializeField]
        [Tooltip("The locomotion providers enabled only in microgesture teleport mode, such as the Snap Turn Provider and Step Move Provider.")]
        List<LocomotionProvider> m_LocomotionProvidersToEnable = new List<LocomotionProvider>();

        /// <summary>
        /// The input that activates microgesture teleport mode, such as a thumb tap. Teleport mode does not start while the hand points at interactive UI or holds an object.
        /// </summary>
        [SerializeField]
        [Tooltip("The input that activates microgesture teleport mode, such as a thumb tap.")]
        XRInputButtonReader m_ActivateInput = new XRInputButtonReader("Activate");

        /// <summary>
        /// The input that teleports in microgesture teleport mode.
        /// When teleport mode is activated by the hand shape, the teleport interactor's own select input is used.
        /// </summary>
        [SerializeField]
        [Tooltip("The input that teleports in microgesture teleport mode.")]
        XRInputButtonReader m_TeleportInput = new XRInputButtonReader("Teleport");

#if XR_HANDS_1_5_OR_NEWER
        /// <summary>
        /// The XRHandShape used to deactivate both hand shape and microgesture teleport mode.
        /// </summary>
        [Header("Cancel Teleport Mode")]
        [SerializeField]
        [Tooltip("The XRHandShape used to deactivate both hand shape and microgesture teleport mode.")]
        XRHandShape m_CancelHandShape;
#endif

        /// <summary>
        /// The teleport XRRayInteractor that will be activated and deactivated by the hand gesture. This interactor drives the teleport ray visuals.
        /// </summary>
        [Header("References")]
        [SerializeField]
        [Tooltip("The teleport XRRayInteractor that will be activated and deactivated by the hand gesture.")]
        XRRayInteractor m_TeleportInteractor;

        /// <summary>
        /// The TrackedPoseDriver used to drive the pose of the teleport ray visuals
        /// </summary>
        [SerializeField]
        TrackedPoseDriver m_TrackedPoseDriver;

        /// <summary>
        /// The hand's NearFarInteractor that is disabled when teleport mode is active, and reactivated when teleport mode is deactivated
        /// </summary>
        [SerializeField, FormerlySerializedAs("m_nearFarInteractor")]
        NearFarInteractor m_NearFarInteractor;

        Transform m_HandTrackingOrigin;
#if XR_HANDS_1_5_OR_NEWER
        XRHandJointsUpdatedEventArgs m_LatestHandData;
#endif
        Coroutine m_ActivateGestureDetection;
        Coroutine m_HoldGestureDetection;
        XRInputButtonReader m_HandShapeTeleportInput;
        bool m_InMicrogestureTeleportMode;

        void Awake()
        {
            if (m_TeleportInteractor.rotateReferenceFrame != null)
            {
                Debug.LogWarning("The Teleport Hand Gesture creates a dynamic reference frame for rotation, so the assigned one will be ignored.");
            }

            m_TeleportInteractor.rotateReferenceFrame = new GameObject("Dynamic Teleport Anchor Rotation Reference Frame").transform;
            m_TeleportInteractor.rotateReferenceFrame.SetParent(transform, false);
            m_HandShapeTeleportInput = m_TeleportInteractor.selectInput;
        }

        void OnEnable()
        {
            if (m_HandTrackingOrigin == null)
                m_HandTrackingOrigin = ComponentLocatorUtility<XROrigin>.TryFindComponent(out var xrOrigin) ? xrOrigin.transform : null;

            m_ActivateGestureDetection = StartCoroutine(ActivateGestureDetection());

            m_TeleportInteractor.gameObject.SetActive(false);
            m_TeleportInteractor.selectEntered.AddListener(OnTeleportSelectEntered);
            m_TeleportInteractor.selectExited.AddListener(OnTeleportSelectExited);

            m_ActivateInput.EnableDirectActionIfModeUsed();
            m_TeleportInput.EnableDirectActionIfModeUsed();
            SetLocomotionProvidersEnabled(false);
        }

        void OnDisable()
        {
            if (m_TeleportInteractor != null)
            {
                // Deactivate before removing the listeners so that ending an active select still stops the joystick tracking.
                m_TeleportInteractor.gameObject.SetActive(false);
                m_TeleportInteractor.selectEntered.RemoveListener(OnTeleportSelectEntered);
                m_TeleportInteractor.selectExited.RemoveListener(OnTeleportSelectExited);
                m_TeleportInteractor.selectInput = m_HandShapeTeleportInput;
            }

            if (m_NearFarInteractor != null)
                m_NearFarInteractor.enabled = true;

            if (m_TrackedPoseDriver != null)
                m_TrackedPoseDriver.enabled = true;

            m_ActivateInput.DisableDirectActionIfModeUsed();
            m_TeleportInput.DisableDirectActionIfModeUsed();
            SetLocomotionProvidersEnabled(false);
        }

#if XR_HANDS_1_5_OR_NEWER
        void OnActivateDetected(bool activatedByInput)
        {
            StopCoroutine(m_ActivateGestureDetection);

            m_InMicrogestureTeleportMode = activatedByInput;
            m_TeleportInteractor.selectInput = activatedByInput ? m_TeleportInput : m_HandShapeTeleportInput;
            SetLocomotionProvidersEnabled(activatedByInput);

            // The activate input and the teleport input might be the same,
            // so the teleport interactor stays disabled to avoid teleporting immediately after activating.
            if (!activatedByInput)
                m_TeleportInteractor.gameObject.SetActive(true);

            m_HoldGestureDetection = StartCoroutine(MaintainTeleportMode());

            m_NearFarInteractor.enabled = false;
        }

        void OnCancelDetected()
        {
            if (m_HoldGestureDetection != null)
            {
                m_TeleportInteractor.gameObject.SetActive(false);
                SetLocomotionProvidersEnabled(false);
                StopCoroutine(m_HoldGestureDetection);
                m_ActivateGestureDetection = StartCoroutine(ActivateGestureDetection());

                m_NearFarInteractor.enabled = true;
            }
        }
#endif

        void OnTeleportSelectEntered(SelectEnterEventArgs eventArgs)
        {
            if (!m_InMicrogestureTeleportMode)
                m_JoystickPinchHandGesture.StartJoystickPinchGestureTracking();
        }

        void OnTeleportSelectExited(SelectExitEventArgs eventArgs)
        {
            m_JoystickPinchHandGesture.StopJoystickPinchHandGestureTracking();
        }

        void SetLocomotionProvidersEnabled(bool enable)
        {
            foreach (var provider in m_LocomotionProvidersToEnable)
            {
                if (provider != null)
                    provider.enabled = enable;
            }
        }

        bool IsNearFarInteracting() =>
            m_NearFarInteractor.hasSelection ||
            (m_NearFarInteractor.TryGetCurrentUIRaycastResult(out _) &&
                m_NearFarInteractor.TryGetUIModel(out var model) &&
                (model.selectableObject != null || model.isScrollable));

#if XR_HANDS_1_5_OR_NEWER
        /// <summary>
        /// Receives joint data from the XRHandTrackingEvents component for use in gesture detection.
        /// </summary>
        /// <param name="jointEventArgs">Event args containing the joint data.</param>
        public void OnJointsUpdated(XRHandJointsUpdatedEventArgs jointEventArgs)
        {
            m_LatestHandData = jointEventArgs;
        }
#endif

        IEnumerator ActivateGestureDetection()
        {
#if XR_HANDS_1_5_OR_NEWER
            var shapeDetected = false;
            while (isActiveAndEnabled)
            {
                // Check that the hand subsystem is still running to guard against accessing a destroyed joints
                // array due to the hand subsystem being destroyed between the time of the joints updated callback
                // and this coroutine executes.
                if (m_LatestHandData != null && m_LatestHandData.subsystem.running && m_LatestHandData.hand.isTracked)
                {
                    if (m_ActivateInput.ReadWasPerformedThisFrame() && !IsNearFarInteracting())
                    {
                        OnActivateDetected(true);
                    }
                    else
                    {
                        var newState = m_ActivateHandShape.CheckConditions(m_LatestHandData);
                        newState &= m_ActivateHandOrientation.CheckConditions(m_LatestHandData.hand.rootPose, m_LatestHandData.hand.handedness);
                        if (!shapeDetected && newState)
                            OnActivateDetected(false);

                        shapeDetected = newState;
                    }
                }

                yield return null;
            }
#else
            yield return null;
#endif
        }

#if XR_HANDS_1_5_OR_NEWER
        IEnumerator MaintainTeleportMode()
        {
            while (isActiveAndEnabled)
            {
                // Check that the hand subsystem is still running to guard against accessing a destroyed joints
                // array due to the hand subsystem being destroyed between the time of the joints updated callback
                // and this coroutine executes.
                if (m_LatestHandData != null && m_LatestHandData.subsystem.running && m_LatestHandData.hand.isTracked)
                {
                    // Enable the teleport interactor once the activate input is no longer performed, so it can't also trigger the teleport.
                    if (m_InMicrogestureTeleportMode && !m_TeleportInteractor.gameObject.activeSelf && !m_ActivateInput.ReadIsPerformed())
                        m_TeleportInteractor.gameObject.SetActive(true);

                    var shapeDetected = m_CancelHandShape.CheckConditions(m_LatestHandData);
                    if (shapeDetected)
                        OnCancelDetected();

                    // While the interactor is selecting, disable the input tracking so that the teleport ray doesn't move
                    var interactorIsSelecting = m_TeleportInteractor.hasSelection;
                    m_TrackedPoseDriver.enabled = !interactorIsSelecting;
                }

                yield return null;
            }
        }
#endif
    }
}
