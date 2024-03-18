using System;
using UnityEngine.Assertions;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation
{
    /// <summary>
    /// The <see cref="TeleportationProvider"/> is responsible for moving the XR Origin
    /// to the desired location on the user's request.
    /// </summary>
    [AddComponentMenu("XR/Locomotion/Teleportation Provider", 11)]
    [HelpURL(XRHelpURLConstants.k_TeleportationProvider)]
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
    public class TeleportationProvider : LocomotionProvider
    {
        /// <summary>
        /// The current teleportation request.
        /// </summary>
        protected TeleportRequest currentRequest { get; set; }

        /// <summary>
        /// Whether the current teleportation request is valid.
        /// </summary>
        protected bool validRequest { get; set; }

        [SerializeField]
        [Tooltip("The time (in seconds) to delay the teleportation once it is activated.")]
        float m_DelayTime;

        /// <summary>
        /// The time (in seconds) to delay the teleportation once it is activated.
        /// This delay can be used, for example, as time to set a tunneling vignette effect as a VR comfort option.
        /// </summary>
        public float delayTime
        {
            get => m_DelayTime;
            set => m_DelayTime = value;
        }

        /// <inheritdoc/>
        public override bool canStartMoving => m_DelayTime <= 0f || Time.time - m_DelayStartTime >= m_DelayTime;

        /// <summary>
        /// This function will queue a teleportation request within the provider.
        /// </summary>
        /// <param name="teleportRequest">The teleportation request to queue.</param>
        /// <returns>Returns <see langword="true"/> if successfully queued. Otherwise, returns <see langword="false"/>.</returns>
        public virtual bool QueueTeleportRequest(TeleportRequest teleportRequest)
        {
            currentRequest = teleportRequest;
            validRequest = true;
            return true;
        }

        /// <summary>
        /// The transformation that is used by this component to apply up vector orientation.
        /// </summary>
        /// <seealso cref="MatchOrientation.WorldSpaceUp"/>
        /// <seealso cref="MatchOrientation.TargetUp"/>
        public XROriginUpAlignment upTransformation { get; set; } = new XROriginUpAlignment();

        /// <summary>
        /// The transformation that is used by this component to apply forward vector orientation.
        /// </summary>
        /// <seealso cref="MatchOrientation.TargetUpAndForward"/>
        public XRCameraForwardXZAlignment forwardTransformation { get; set; } = new XRCameraForwardXZAlignment();

        /// <summary>
        /// The transformation that is used by this component to apply teleport positioning movement.
        /// </summary>
        public XRBodyGroundPosition positionTransformation { get; set; } = new XRBodyGroundPosition();

        float m_DelayStartTime;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Update()
        {
            if (!validRequest)
                return;

            if (locomotionState == LocomotionState.Idle)
            {
                if (m_DelayTime > 0f)
                {
                    if (TryPrepareLocomotion())
                        m_DelayStartTime = Time.time;
                }
                else
                {
                    TryStartLocomotionImmediately();
                }
            }

            if (locomotionState == LocomotionState.Moving)
            {
                switch (currentRequest.matchOrientation)
                {
                    case MatchOrientation.WorldSpaceUp:
                        upTransformation.targetUp = Vector3.up;
                        TryQueueTransformation(upTransformation);
                        break;
                    case MatchOrientation.TargetUp:
                        upTransformation.targetUp = currentRequest.destinationRotation * Vector3.up;
                        TryQueueTransformation(upTransformation);
                        break;
                    case MatchOrientation.TargetUpAndForward:
                        upTransformation.targetUp = currentRequest.destinationRotation * Vector3.up;
                        TryQueueTransformation(upTransformation);
                        forwardTransformation.targetDirection = currentRequest.destinationRotation * Vector3.forward;
                        TryQueueTransformation(forwardTransformation);
                        break;
                    case MatchOrientation.None:
                        // Change nothing. Maintain current origin rotation.
                        break;
                    default:
                        Assert.IsTrue(false, $"Unhandled {nameof(MatchOrientation)}={currentRequest.matchOrientation}.");
                        break;
                }

                positionTransformation.targetPosition = currentRequest.destinationPosition;
                TryQueueTransformation(positionTransformation);

                TryEndLocomotion();
                validRequest = false;
            }
        }
    }
}
