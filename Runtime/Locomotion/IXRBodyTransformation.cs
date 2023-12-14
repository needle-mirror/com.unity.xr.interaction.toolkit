using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion
{
    /// <summary>
    /// Interface for a transformation that can be applied to an <see cref="XROrigin.Origin"/>, using the user's body
    /// as a frame of reference.
    /// </summary>
    /// <seealso cref="XRBodyTransformer"/>
    /// <seealso cref="XRMovableBody"/>
    public interface IXRBodyTransformation
    {
        /// <summary>
        /// Performs the transformation on the given body.
        /// </summary>
        /// <param name="body">The body whose <see cref="XRMovableBody.originTransform"/> to transform.</param>
        void Apply(XRMovableBody body);
    }
}