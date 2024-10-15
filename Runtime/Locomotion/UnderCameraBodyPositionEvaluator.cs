using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion
{
    /// <summary>
    /// Scriptable object that estimates the user's body position by projecting the position of the camera onto the
    /// XZ plane of the <see cref="XROrigin"/>.
    /// </summary>
    /// <remarks>
    /// This is the default <see cref="XRBodyTransformer.bodyPositionEvaluator"/> for an <see cref="XRBodyTransformer"/>.
    /// </remarks>
    [CreateAssetMenu(fileName = "UnderCameraBodyPositionEvaluator", menuName = "XR/Locomotion/Under Camera Body Position Evaluator")]
    [HelpURL(XRHelpURLConstants.k_UnderCameraBodyPositionEvaluator)]
    public class UnderCameraBodyPositionEvaluator : ScriptableObject, IXRBodyPositionEvaluator
    {
        /// <inheritdoc/>
        public Vector3 GetBodyGroundLocalPosition(XROrigin xrOrigin)
        {
            var bodyPosition = xrOrigin.CameraInOriginSpacePos;
            bodyPosition.y = 0f;
            return bodyPosition;
        }
    }
}
