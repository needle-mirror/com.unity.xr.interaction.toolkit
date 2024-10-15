using System;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion
{
    /// <summary>
    /// Body transformation that invokes a delegate when applied.
    /// </summary>
    public class DelegateXRBodyTransformation : IXRBodyTransformation
    {
        /// <summary>
        /// Invoked when <see cref="Apply"/> is called. Use this to perform the actual transformation.
        /// </summary>
        public event Action<XRMovableBody> transformation;

        /// <summary>
        /// Constructs a new transformation.
        /// </summary>
        public DelegateXRBodyTransformation()
        {
        }

        /// <summary>
        /// Constructs a new transformation with the specified delegate.
        /// </summary>
        /// <param name="transformation">The delegate to be invoked when <see cref="Apply"/> is called.</param>
        public DelegateXRBodyTransformation(Action<XRMovableBody> transformation)
        {
            this.transformation = transformation;
        }

        /// <inheritdoc/>
        public void Apply(XRMovableBody body)
        {
            transformation?.Invoke(body);
        }
    }

    /// <summary>
    /// Transformation that translates the target's <see cref="XRMovableBody.originTransform"/> by the specified amount.
    /// </summary>
    public class XROriginMovement : IXRBodyTransformation
    {
        /// <summary>
        /// Amount of translation to apply to the <see cref="XRMovableBody.originTransform"/>.
        /// </summary>
        public Vector3 motion { get; set; }

        /// <summary>
        /// Whether to ignore <see cref="XRMovableBody.constrainedManipulator"/> even if it is set.
        /// Defaults to <see langword="false"/> to use the movement constraints if configured to.
        /// </summary>
        /// <remarks>
        /// Setting this to <see langword="true"/> will mean the body will always be moved using the Transform component directly.
        /// Setting this to <see langword="false"/> will use <see cref="IConstrainedXRBodyManipulator.MoveBody"/> to move the Origin
        /// if the <see cref="XRMovableBody.constrainedManipulator"/> is not <see langword="null"/>, and otherwise use the Transform component.
        /// </remarks>
        public bool forceUnconstrained { get; set; }

        /// <inheritdoc/>
        public virtual void Apply(XRMovableBody body)
        {
            if (body.constrainedManipulator != null && !forceUnconstrained)
                body.constrainedManipulator.MoveBody(motion);
            else
                body.originTransform.position += motion;
        }
    }

    /// <summary>
    /// Transformation that moves the target's <see cref="XRMovableBody.originTransform"/> such that the world position
    /// of where the user's body is grounded matches the specified position. The body ground position is determined by
    /// the target's <see cref="XRMovableBody.bodyPositionEvaluator"/>.
    /// </summary>
    public class XRBodyGroundPosition : IXRBodyTransformation
    {
        /// <summary>
        /// World position that the user's body ground position should be at.
        /// </summary>
        public Vector3 targetPosition { get; set; }

        /// <inheritdoc/>
        public virtual void Apply(XRMovableBody body)
        {
            var origin = body.originTransform;
            origin.position = targetPosition + origin.position - body.GetBodyGroundWorldPosition();
        }
    }

    /// <summary>
    /// Transformation that rotates the target's <see cref="XRMovableBody.originTransform"/> such that its up vector
    /// matches the specified vector. Note that this does not maintain the world position of the user's body.
    /// </summary>
    public class XROriginUpAlignment : IXRBodyTransformation
    {
        /// <summary>
        /// Vector that the <see cref="XRMovableBody.originTransform"/>'s up vector should match.
        /// </summary>
        public Vector3 targetUp { get; set; }

        /// <inheritdoc/>
        public virtual void Apply(XRMovableBody body)
        {
            body.xrOrigin.MatchOriginUp(targetUp);
        }
    }

    /// <summary>
    /// Transformation that rotates the target's <see cref="XRMovableBody.originTransform"/> by the specified amount
    /// about the axis aligned with the Origin's up vector and passing through the world position of where the user's
    /// body is grounded. The body ground position is determined by the target's <see cref="XRMovableBody.bodyPositionEvaluator"/>.
    /// </summary>
    public class XRBodyYawRotation : IXRBodyTransformation
    {
        /// <summary>
        /// Amount in degrees to rotate the <see cref="XRMovableBody.originTransform"/>.
        /// </summary>
        public float angleDelta { get; set; }

        /// <inheritdoc/>
        public virtual void Apply(XRMovableBody body)
        {
            var origin = body.originTransform;
            origin.RotateAround(body.GetBodyGroundWorldPosition(), origin.up, angleDelta);
        }
    }

    /// <summary>
    /// Transformation that rotates the target's <see cref="XRMovableBody.originTransform"/> about the axis aligned with
    /// the Origin's up vector and passing through the world position of the <see cref="XROrigin.Camera"/>, such that
    /// the projection of the camera's forward vector onto the Origin's XZ plane matches the projection of the specified
    /// vector onto the Origin's XZ plane.
    /// </summary>
    public class XRCameraForwardXZAlignment : IXRBodyTransformation
    {
        /// <summary>
        /// Vector that the forward vector of the <see cref="XROrigin.Camera"/> should match when both are projected
        /// onto the XZ plane of the <see cref="XRMovableBody.originTransform"/>.
        /// </summary>
        public Vector3 targetDirection { get; set; }

        /// <inheritdoc/>
        public virtual void Apply(XRMovableBody body)
        {
            var xrOrigin = body.xrOrigin;
            var originUp = body.originTransform.up;
            var projectedCamForward = Vector3.ProjectOnPlane(xrOrigin.Camera.transform.forward, originUp).normalized;
            var projectedTargetForward = Vector3.ProjectOnPlane(targetDirection, originUp).normalized;
            var signedAngle = Vector3.SignedAngle(projectedCamForward, projectedTargetForward, originUp);
            xrOrigin.RotateAroundCameraPosition(originUp, signedAngle);
        }
    }

    /// <summary>
    /// Transformation that sets the uniform local scale of the target's <see cref="XRMovableBody.originTransform"/> to
    /// the specified value, and then repositions the Origin such that the world position of where the user's body is
    /// grounded remains the same. The body ground position is determined by the target's
    /// <see cref="XRMovableBody.bodyPositionEvaluator"/>.
    /// </summary>
    public class XRBodyScale : IXRBodyTransformation
    {
        /// <summary>
        /// Uniform value to scale the <see cref="XRMovableBody.originTransform"/> to. The local scale of the Origin
        /// will be set to <see cref="Vector3.one"/> multiplied by this value.
        /// </summary>
        public float uniformScale { get; set; }

        /// <inheritdoc/>
        public virtual void Apply(XRMovableBody body)
        {
            var bodyGroundPositionBeforeScale = body.GetBodyGroundWorldPosition();
            var origin = body.originTransform;
            origin.localScale = Vector3.one * uniformScale;
            var bodyGroundPositionAfterScale = body.GetBodyGroundWorldPosition();
            origin.position = bodyGroundPositionBeforeScale + origin.position - bodyGroundPositionAfterScale;
        }
    }
}
