using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;


#if XR_HANDS_1_5_OR_NEWER
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.XRHandLocomotion
{
    /// <summary>
    /// Evaluates the hand joints/bones in order to control the state of the XRRayInteractor and perform further joystick pinch input that controls the teleport reticle
    /// </summary>
    public class TeleportHandGesture : MonoBehaviour
    {
        [Header("Activate Teleport Mode")]
#if XR_HANDS_1_5_OR_NEWER
        /// <summary>
        /// The XRHandshape used to activate the display of the teleport visuals
        /// </summary>
        [SerializeField]
        XRHandShape m_ActivateHandShape;

        /// <summary>
        /// Detect an activate gesture based on any user defined inspector conditions applied to the ActivateHandShape
        /// </summary>
        [SerializeField]
        XRHandRelativeOrientation m_ActivateHandOrientation;

        /// <summary>
        /// The XRHandshape used to deactivate the display of the teleport visuals and teleport mode
        /// </summary>
        [Header("Cancel Teleport Mode")]
        [SerializeField]
        XRHandShape m_CancelHandShape;
#endif

        /// <summary>
        /// The controller that handles the joystick input mode enabled when pinching while the teleport visuals are activated
        /// This controller handles the rotation targeting of the teleport ray reticle
        /// </summary>
        [SerializeField]
        JoystickPinchHandGesture m_JoystickPinchHandGesture;

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
        [SerializeField]
        NearFarInteractor m_nearFarInteractor;

        Transform m_HandTrackingOrigin;
#if XR_HANDS_1_5_OR_NEWER
        XRHandJointsUpdatedEventArgs m_LatestHandData;
#endif
        Coroutine m_ActivateGestureDetection;
        Coroutine m_HoldGestureDetection;

        void Awake()
        {
            if (m_TeleportInteractor.rotateReferenceFrame != null)
            {
                Debug.LogWarning("The Teleport Hand Gesture creates a dynamic reference frame for rotation, so the assigned one will be ignored.");
            }

            m_TeleportInteractor.rotateReferenceFrame = new GameObject("Dynamic Teleport Anchor Rotation Reference Frame").transform;
            m_TeleportInteractor.rotateReferenceFrame.SetParent(transform, false);
        }

        void OnEnable()
        {
            if (m_HandTrackingOrigin == null)
                m_HandTrackingOrigin = ComponentLocatorUtility<XROrigin>.TryFindComponent(out var xrOrigin) ? xrOrigin.transform : null;

            m_ActivateGestureDetection = StartCoroutine(ActivateGestureDetection());

            m_TeleportInteractor.gameObject.SetActive(false);
            m_TeleportInteractor.selectEntered.AddListener(OnTeleportSelectEntered);
            m_TeleportInteractor.selectExited.AddListener(OnTeleportSelectExited);
        }

        void OnDisable()
        {
#if XR_HANDS_1_5_OR_NEWER
            m_LatestHandData = null;
#endif
            m_TeleportInteractor.selectEntered.RemoveListener(OnTeleportSelectEntered);
            m_TeleportInteractor.selectExited.RemoveListener(OnTeleportSelectExited);
        }

#if XR_HANDS_1_5_OR_NEWER
        void OnActivateDetected()
        {
            StopCoroutine(m_ActivateGestureDetection);

            m_TeleportInteractor.gameObject.SetActive(true);
            m_HoldGestureDetection = StartCoroutine(MaintainTeleportMode());

            m_nearFarInteractor.enabled = false;
        }

        void OnCancelDetected()
        {
            if (m_HoldGestureDetection != null)
            {
                m_TeleportInteractor.gameObject.SetActive(false);
                StopCoroutine(m_HoldGestureDetection);
                m_ActivateGestureDetection = StartCoroutine(ActivateGestureDetection());

                m_nearFarInteractor.enabled = true;
            }
        }
#endif

        void OnTeleportSelectEntered(SelectEnterEventArgs eventArgs)
        {
            m_JoystickPinchHandGesture.StartJoystickPinchGestureTracking();
        }

        void OnTeleportSelectExited(SelectExitEventArgs eventArgs)
        {
            m_JoystickPinchHandGesture.StopJoystickPinchHandGestureTracking();
        }

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
                if (m_LatestHandData != null && m_LatestHandData.hand.isTracked)
                {
                    var newState = m_ActivateHandShape.CheckConditions(m_LatestHandData);
                    newState &= m_ActivateHandOrientation.CheckConditions(m_LatestHandData.hand.rootPose, m_LatestHandData.hand.handedness);
                    if (!shapeDetected && newState)
                        OnActivateDetected();

                    shapeDetected = newState;
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
                if (m_LatestHandData != null && m_LatestHandData.hand.isTracked)
                {
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
