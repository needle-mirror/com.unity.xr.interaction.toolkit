using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion
{
    /// <summary>
    /// Container for an <see cref="XROrigin"/> that can be transformed using the user's body as a frame of reference.
    /// </summary>
    /// <seealso cref="IXRBodyTransformation"/>
    public class XRMovableBody
    {
        /// <summary>
        /// The XR Origin whose <see cref="XROrigin.Origin"/> is transformed to move the body.
        /// </summary>
        public XROrigin xrOrigin { get; private set; }

        /// <summary>
        /// The Transform component of the <see cref="XROrigin.Origin"/> of the <see cref="xrOrigin"/>.
        /// This is the Transform component that is manipulated to move the body.
        /// </summary>
        public Transform originTransform => xrOrigin.Origin.transform;

        /// <summary>
        /// The object that determines the position of the user's body.
        /// </summary>
        public IXRBodyPositionEvaluator bodyPositionEvaluator { get; private set; }

        /// <summary>
        /// Object that can be used to perform movement that is constrained by collision (optional, may be <see langword="null"/>).
        /// </summary>
        public IConstrainedXRBodyManipulator constrainedManipulator { get; private set; }

        /// <summary>
        /// Initializes a new instance of a movable body.
        /// </summary>
        /// <param name="xrOrigin">The XR Origin associated with the body.</param>
        /// <param name="bodyPositionEvaluator">The object that determines the position of the user's body.</param>
        public XRMovableBody(XROrigin xrOrigin, IXRBodyPositionEvaluator bodyPositionEvaluator)
        {
            this.xrOrigin = xrOrigin;
            this.bodyPositionEvaluator = bodyPositionEvaluator;
        }

        /// <summary>
        /// Gets the position of where the user's body is grounded (e.g. their feet), in the local space of the
        /// <see cref="originTransform"/>, based on the <see cref="bodyPositionEvaluator"/>.
        /// </summary>
        /// <returns>Returns the position of where the user's body is grounded, in the local space of the <see cref="originTransform"/>.</returns>
        public Vector3 GetBodyGroundLocalPosition()
        {
            return bodyPositionEvaluator.GetBodyGroundLocalPosition(xrOrigin);
        }

        /// <summary>
        /// Gets the world position of where the user's body is grounded (e.g. their feet), based on the
        /// <see cref="bodyPositionEvaluator"/>.
        /// </summary>
        /// <returns>Returns the world position of where the user's body is grounded.</returns>
        public Vector3 GetBodyGroundWorldPosition()
        {
            return bodyPositionEvaluator.GetBodyGroundWorldPosition(xrOrigin);
        }

        /// <summary>
        /// Links the given constrained manipulator to this body. This sets <see cref="constrainedManipulator"/> to
        /// <paramref name="manipulator"/> and calls <see cref="IConstrainedXRBodyManipulator.OnLinkedToBody"/> on the
        /// manipulator.
        /// </summary>
        /// <param name="manipulator">The constrained manipulator to link.</param>
        /// <remarks>
        /// If <see cref="constrainedManipulator"/> is already not <see langword="null"/> when this is called, this
        /// first calls <see cref="IConstrainedXRBodyManipulator.OnUnlinkedFromBody"/> on <see cref="constrainedManipulator"/>.
        /// Also, if the given <paramref name="manipulator"/> already has a <see cref="IConstrainedXRBodyManipulator.linkedBody"/>
        /// set, this calls <see cref="UnlinkConstrainedManipulator"/> on that body.
        /// </remarks>
        /// <seealso cref="UnlinkConstrainedManipulator"/>
        public void LinkConstrainedManipulator(IConstrainedXRBodyManipulator manipulator)
        {
            constrainedManipulator?.OnUnlinkedFromBody();
            manipulator.linkedBody?.UnlinkConstrainedManipulator();
            constrainedManipulator = manipulator;
            constrainedManipulator.OnLinkedToBody(this);
        }

        /// <summary>
        /// Unlinks the assigned constrained manipulator from this body, if there is one. This calls
        /// <see cref="IConstrainedXRBodyManipulator.OnUnlinkedFromBody"/> on the manipulator and sets
        /// <see cref="constrainedManipulator"/> to <see langword="null"/>.
        /// </summary>
        /// <seealso cref="LinkConstrainedManipulator"/>
        public void UnlinkConstrainedManipulator()
        {
            constrainedManipulator?.OnUnlinkedFromBody();
            constrainedManipulator = null;
        }
    }
}
