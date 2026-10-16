using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

#if XR_HANDS_1_5_OR_NEWER
using UnityEngine.XR.Hands;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.XRHandLocomotion
{
    /// <summary>
    /// Handles the joystick input mode enabled when pinching while the teleport visuals are activated.
    /// Rotates the teleport ray reticle based on hand position.
    /// </summary>
    public class JoystickPinchHandGesture : MonoBehaviour
    {
#if XR_HANDS_1_5_OR_NEWER
        /// <summary>
        /// The XRHandJointID used to retrieve the pose of the targeted joint.
        /// </summary>
        [SerializeField]
        XRHandJointID m_JoystickAttachInputJointID = XRHandJointID.Palm;
#endif

        /// <summary>
        /// The radius used to normalize the projected planar pose data.
        /// </summary>
        [SerializeField]
#pragma warning disable CS0414 // Field assigned but its value is never used -- Keep to retain serialized value when XR Hands is not installed
        float m_Radius = 0.07f;
#pragma warning restore CS0414

        /// <summary>
        /// The amount by which the reticle rotation is smoothed. Prevents input jitter.
        /// </summary>
        [SerializeField]
#pragma warning disable CS0414 // Field assigned but its value is never used -- Keep to retain serialized value when XR Hands is not installed
        float m_RotationSmoothingValue = 0.2f;
#pragma warning restore CS0414

        /// <summary>
        /// The XRInteractorLineVisual that draws the teleport line and reticle visuals
        /// </summary>
        [SerializeField]
        XRInteractorLineVisual m_XRInteractorLineVisual;

        /// <summary>
        /// XRRayInteractor whose selectEntered and selectExited state is controlled by the TeleportHandGesture controller.
        /// </summary>
        [SerializeField]
        XRRayInteractor m_XRRayInteractor;

        /// <summary>
        /// The target transform that is rotated and offset from the joystick center position and used to position the LookAt target position used for reticle rotation.
        /// </summary>
        [SerializeField]
        Transform m_AttachTransformRotationTarget;

        /// <summary>
        /// Transform that is the child of the stabilized aim origin; used to perform the final LookAt operation after processing the initial position and rotation changes.
        /// </summary>
        [SerializeField]
        Transform m_AttachTransformForRotation;

        /// <summary>
        /// The UI visuals displayed above the hand performing reticle rotation.
        /// </summary>
        [SerializeField]
        Transform m_NavigatorUI;

        /// <summary>
        /// The XROrigin "Origin" transform used to source the position of the navigator UI visuals when enabling joystick input.
        /// </summary>
        [SerializeField]
        Transform m_XROrigin;

#if XR_HANDS_1_5_OR_NEWER
        XRHandJointsUpdatedEventArgs m_LatestHandData;
        Coroutine m_ActivateGestureDetection;
        Pose m_CenterInTrackingSpace;
        Quaternion m_SteerFrameRotation;
        Vector3 m_SmoothedRotationCurrent;
        Vector3 m_SmoothedRotationVelocity;
#endif

        Coroutine m_HoldGestureDetection;
        Transform m_ReticleTransform;
        Quaternion m_AttachTransformOriginalLocalRotation = Quaternion.identity;

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

            m_AttachTransformOriginalLocalRotation = m_AttachTransformForRotation.localRotation;

            m_HoldGestureDetection = StartCoroutine(MaintainJoystickMode());
        }

        /// <summary>
        /// Called when the TeleportHandGesture's XRRayInteractor's selectExited event is called
        /// </summary>
        public void StopJoystickPinchHandGestureTracking()
        {
            if (m_HoldGestureDetection != null)
                StopCoroutine(m_HoldGestureDetection);

            m_AttachTransformForRotation.localRotation = m_AttachTransformOriginalLocalRotation;

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
            // Show the UI visual if the rotation after teleport can be controlled.
            var showNavigatorUI = false;
            if (args.interactableObject.transform.TryGetComponent<BaseTeleportationInteractable>(out var teleportationInteractable))
            {
                var matchOrientation = teleportationInteractable.matchOrientation;
                if (matchOrientation == MatchOrientation.WorldSpaceUp || matchOrientation == MatchOrientation.TargetUp)
                    showNavigatorUI = teleportationInteractable.matchDirectionalInput;
            }

            m_NavigatorUI.gameObject.SetActive(showNavigatorUI);
        }

#if XR_HANDS_1_5_OR_NEWER
        Quaternion UpdateAttachTransform(Vector3 planeNormal)
        {
            const float kMinAimSqrMagnitude = 0.0001f;

            m_AttachTransformForRotation.LookAt(m_AttachTransformRotationTarget.position, planeNormal);
            var projectedForward = Vector3.ProjectOnPlane(m_AttachTransformForRotation.forward, planeNormal);
            var steerRotation = projectedForward.sqrMagnitude > kMinAimSqrMagnitude
                ? Quaternion.LookRotation(projectedForward, planeNormal)
                : m_XROrigin.rotation * m_CenterInTrackingSpace.rotation;

            return steerRotation;
        }
#endif

        IEnumerator MaintainJoystickMode()
        {
#if XR_HANDS_1_5_OR_NEWER
            const float kInitialRotationTriggerDistanceThreshold = 0.00015f;
            var joystickCenterSet = false;
            var rotationProcessingThresholdExceeded = false;

            while (isActiveAndEnabled)
            {
                // Check that the hand subsystem is still running to guard against accessing a destroyed joints
                // array due to the hand subsystem being destroyed between the time of the joints updated callback
                // and this coroutine executes.
                if (m_LatestHandData != null && m_LatestHandData.subsystem.running &&
                    m_LatestHandData.hand.isTracked && m_JoystickAttachInputJointID != XRHandJointID.Invalid &&
                    m_LatestHandData.hand.GetJoint(m_JoystickAttachInputJointID).TryGetPose(out var pose))
                {
                    var planeNormal = Vector3.up;
                    if (!joystickCenterSet)
                    {
                        joystickCenterSet = true;

                        var joystickForward = m_LatestHandData.hand.rootPose.forward;
                        joystickForward.y = 0f;

                        m_CenterInTrackingSpace.position = pose.position;
                        m_CenterInTrackingSpace.rotation = Quaternion.LookRotation(joystickForward, planeNormal);

                        // Set the initial smoothed rotation value so the reticle has a gradual initial rotation starting by pointing forward
                        m_SmoothedRotationCurrent = Vector3.zero;
                        m_SmoothedRotationVelocity = Vector3.zero;

                        var forwardOffsetPosition = m_ReticleTransform.TransformPoint(Vector3.forward * 0.5f);
                        m_AttachTransformRotationTarget.position = forwardOffsetPosition;

                        // Capture the reference frame the input value will be applied in for the duration of the gesture.
                        m_SteerFrameRotation = UpdateAttachTransform(planeNormal);

                        m_NavigatorUI.SetPositionAndRotation(m_XROrigin.TransformPoint(pose.position), m_SteerFrameRotation);
                    }
                    else
                    {
                        var currentDelta = pose.position - m_CenterInTrackingSpace.position;

                        // Allow for rotation processing after the hand has translated greater than a minimum distance, preventing initial jerky movement
                        // This also facilitates the ability to quickly snap/pinch quickly & repeatedly while maintaining a forward direction
                        rotationProcessingThresholdExceeded |= Vector3.SqrMagnitude(currentDelta) > kInitialRotationTriggerDistanceThreshold;

                        if (rotationProcessingThresholdExceeded)
                        {
                            var projectedDelta = Vector3.ProjectOnPlane(currentDelta, planeNormal);
                            var normalizedInputValue = projectedDelta / m_Radius;
                            var rotatedInputValue = Quaternion.Inverse(m_CenterInTrackingSpace.rotation) * normalizedInputValue;

                            m_SmoothedRotationCurrent = Vector3.SmoothDamp(m_SmoothedRotationCurrent, rotatedInputValue, ref m_SmoothedRotationVelocity, m_RotationSmoothingValue);

                            var forwardOffsetPosition = m_ReticleTransform.TransformPoint(Vector3.forward * 0.5f);
                            m_AttachTransformRotationTarget.position = forwardOffsetPosition - m_SteerFrameRotation * (m_SmoothedRotationCurrent * 0.5f);

                            m_NavigatorUI.rotation = UpdateAttachTransform(planeNormal);
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
