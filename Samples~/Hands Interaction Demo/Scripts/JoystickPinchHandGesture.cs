using System.Collections;
using Unity.Mathematics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

#if XR_HANDS_1_5_OR_NEWER
using UnityEngine.XR.Hands;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.XRHandLocomotion
{
    /// <summary>
    /// Handles the joystick input mode enabled when pinching while the teleport visuals are activated
    /// Rotates the teleport ray reticle based on hand position
    /// </summary>
    public class JoystickPinchHandGesture : MonoBehaviour
    {
#if XR_HANDS_1_5_OR_NEWER
        /// <summary>
        /// The XRHandJointID used to retrieve the pose of the targeted joint
        /// </summary>
        [SerializeField]
        XRHandJointID m_JoystickAttachInputJointID = XRHandJointID.IndexTip;
#endif

        /// <summary>
        /// The radius used to normalize the projected planar pose data
        /// </summary>
        [SerializeField]
        float m_Radius = 0.02f;

        /// <summary>
        /// The amount by which the reticle rotation is smoothed.  Prevents input jitter
        /// </summary>
        [SerializeField]
        float m_RotationSmoothingValue = 0.2f;

        /// <summary>
        /// The XRInteractorLineVisual that draws the teleport line and reticle visuals
        /// </summary>
        [SerializeField]
        XRInteractorLineVisual m_XRInteractorLineVisual;

        /// <summary>
        /// XRRayInteractor whose selectEntered and selectExited state is controlled by the TeleportHandGesture controller
        /// </summary>
        [SerializeField]
        XRRayInteractor m_XRRayInteractor;

        /// <summary>
        /// The target transform that is rotated and offset from the joystick center position and used to position the LookAt target position used for reticle rotation
        /// </summary>
        [SerializeField]
        Transform m_AttachTransformRotationTarget;

        /// <summary>
        /// Transform that is the child of the stabilized aim origin; used to perform the final LookAt operation after processing the initial position and rotation changes
        /// </summary>
        [SerializeField]
        Transform m_AttachTransformForRotation;

        /// <summary>
        /// The UI visuals displayed above the hand performing reticle rotation
        /// </summary>
        [SerializeField]
        Transform m_NavigatorUI;

        /// <summary>
        /// The XROrigin transform used to source the position of the navigator UI visuals when enabling joystick input
        /// </summary>
        [SerializeField]
        Transform m_XROrigin;

#if XR_HANDS_1_5_OR_NEWER
        XRHandJointsUpdatedEventArgs m_LatestHandData;
        Coroutine m_ActivateGestureDetection;
        Pose m_CenterInTrackingSpace;
        Vector3 m_SmoothedRotationCurrent;
        Vector3 m_SmoothedRotationVelocity;
        bool m_JoystickCenterSet;
#endif

        Coroutine m_HoldGestureDetection;
        Transform m_ReticleTransform;
        bool m_SelectingTeleporationAnchor;

        void OnEnable()
        {
            m_XRRayInteractor.selectEntered.AddListener(OnSelectEntered);

            m_NavigatorUI.gameObject.SetActive(false);
        }

        void OnDisable()
        {
            m_XRRayInteractor.selectEntered.RemoveListener(OnSelectEntered);
        }

        /// <summary>
        /// Called when the TeleportHandGesture's XRRayInteractor's selectEntered event is called
        /// </summary>
        public void StartJoystickPinchGestureTracking()
        {
            if (m_HoldGestureDetection != null)
                StopCoroutine(m_HoldGestureDetection);

            if (m_ReticleTransform == null)
                m_ReticleTransform = m_XRInteractorLineVisual.reticle.transform;

            m_HoldGestureDetection = StartCoroutine(MaintainJoystickMode());
        }

        /// <summary>
        /// Called when the TeleportHandGesture's XRRayInteractor's selectExited event is called
        /// </summary>
        public void StopJoystickPinchHandGestureTracking()
        {
            StopCoroutine(m_HoldGestureDetection);

#if XR_HANDS_1_5_OR_NEWER
            m_JoystickCenterSet = false;
#endif

            m_AttachTransformForRotation.localRotation = quaternion.identity;

            m_NavigatorUI.gameObject.SetActive(false);
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

        void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (args.interactableObject is IXRSelectInteractable interactable)
            {
                // Check if the selected object is a TeleportationAnchor
                TeleportationAnchor anchor = interactable.transform.GetComponent<TeleportationAnchor>();
                m_SelectingTeleporationAnchor = anchor != null;
                m_NavigatorUI.gameObject.SetActive(!m_SelectingTeleporationAnchor);
            }
        }

        IEnumerator MaintainJoystickMode()
        {
#if XR_HANDS_1_5_OR_NEWER
            const float kInitialRotationTriggerDistanceThreshold = 0.00015f;
            var initialHandPosePosition = Vector3.zero;
            var rotationProcessingThresholdExceeded = false;

            while (isActiveAndEnabled)
            {
                if (m_LatestHandData != null && m_LatestHandData.hand.isTracked)
                {
                    if (m_JoystickAttachInputJointID != XRHandJointID.Invalid && m_LatestHandData.hand.GetJoint(m_JoystickAttachInputJointID).TryGetPose(out var pose))
                    {
                        // Store the initial palm-down rotation
                        var initialRotation = pose.rotation;

                        // Calculate and apply the palm-up rotation to invert the cylindrical scrubbing direction as opposed to palm-up
                        Quaternion palmUpRotation = Quaternion.Inverse(initialRotation);
                        pose.rotation = palmUpRotation;

                        var joystickForward = m_LatestHandData.hand.rootPose.forward;
                        joystickForward.y = 0f;

                        var planeNormal = Vector3.up;
                        if (!m_JoystickCenterSet)
                        {
                            m_CenterInTrackingSpace.position = pose.position;
                            m_CenterInTrackingSpace.rotation = Quaternion.LookRotation(joystickForward, planeNormal);
                            m_JoystickCenterSet = true;

                            initialHandPosePosition = pose.position;

                            var navigatorWorldPosition = m_XROrigin.TransformPoint(pose.position);
                            m_NavigatorUI.position = navigatorWorldPosition;

                            var currentDelta = pose.position - m_CenterInTrackingSpace.position;
                            var projectedDelta = Vector3.ProjectOnPlane(currentDelta, planeNormal);
                            var normalizedInputValue = projectedDelta / m_Radius;
                            var rotatedInputValue = Quaternion.Inverse(m_CenterInTrackingSpace.rotation) * normalizedInputValue;

                            // set the initial smoothed rotation value so the reticle has a gradual initial rotation starting by pointing forward
                            m_SmoothedRotationCurrent = rotatedInputValue;
                        }
                        else if (rotationProcessingThresholdExceeded)
                        {
                            var currentDelta = pose.position - m_CenterInTrackingSpace.position;
                            var projectedDelta = Vector3.ProjectOnPlane(currentDelta, planeNormal);
                            var normalizedInputValue = projectedDelta / m_Radius;
                            var rotatedInputValue = Quaternion.Inverse(m_CenterInTrackingSpace.rotation) * normalizedInputValue;

                            m_SmoothedRotationCurrent = Vector3.SmoothDamp(m_SmoothedRotationCurrent, rotatedInputValue, ref m_SmoothedRotationVelocity, m_RotationSmoothingValue);

                            var forwardOffsetPosition = m_ReticleTransform.TransformPoint(Vector3.forward * 0.5f);
                            m_AttachTransformRotationTarget.position = forwardOffsetPosition;
                            m_AttachTransformRotationTarget.localPosition -= m_SmoothedRotationCurrent * 0.5f;

                            var targetPositionPose = new Pose(m_AttachTransformRotationTarget.position, quaternion.identity);

                            m_AttachTransformForRotation.LookAt(targetPositionPose.position);
                            m_NavigatorUI.rotation = m_AttachTransformForRotation.rotation;
                        }
                        else if (!rotationProcessingThresholdExceeded)
                        {
                            // Allow for rotation processing after the hand has translated greater than a minimum distance, preventing initial jerky movement
                            // This also facilitates the ability to quickly snap/pinch quickly & repeatedly while maintaining a forward direction
                            rotationProcessingThresholdExceeded = Vector3.SqrMagnitude(initialHandPosePosition - pose.position) > kInitialRotationTriggerDistanceThreshold;
                        }
                    }
                }

                yield return null;
            }
#else
            yield return null;
#endif
        }
    }
}
