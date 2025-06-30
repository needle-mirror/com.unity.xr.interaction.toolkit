using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
    /// <summary>
    /// Provides utility functions for locomotion of the XR Origin.
    /// </summary>
    public static class LocomotionUtility
    {
        /// <summary>
        /// Gets the world space position of the projection of the camera position onto the XZ plane of the XR Origin.
        /// This should generally be treated as the "user" position for the purposes of locomotion to a
        /// destination position.
        /// </summary>
        /// <param name="xrOrigin">The XR Origin.</param>
        /// <returns>
        /// Returns the world space position of the projection of the camera position onto the XZ plane of the XR Origin.
        /// </returns>
        public static Vector3 GetCameraFloorWorldPosition(this XROrigin xrOrigin)
        {
            var cameraInOriginSpacePos = xrOrigin.CameraInOriginSpacePos;
            var cameraFloorInOriginSpacePos = new Vector3(cameraInOriginSpacePos.x, 0f, cameraInOriginSpacePos.z);
            return xrOrigin.Origin.transform.TransformPoint(cameraFloorInOriginSpacePos);
        }

        internal static bool TryGetOriginTransform(LocomotionProvider locomotionProvider, out Transform originTransform)
        {
            // Correct version of locomotionProvider?.mediator?.xrOrigin?.Origin?.transform
            if (locomotionProvider != null)
            {
                var mediator = locomotionProvider.mediator;
                return TryGetOriginTransform(mediator, out originTransform);
            }

            originTransform = null;
            return false;
        }

        internal static bool TryGetOriginTransform(LocomotionMediator mediator, out Transform originTransform)
        {
            // Correct version of mediator?.xrOrigin?.Origin?.transform
            if (mediator != null)
            {
                var xrOrigin = mediator.xrOrigin;
                if (xrOrigin != null)
                {
                    var origin = xrOrigin.Origin;
                    if (origin != null)
                    {
                        originTransform = origin.transform;
                        return true;
                    }
                }
            }

            originTransform = null;
            return false;
        }

        internal static bool TryGetOriginTransform(XRBodyTransformer bodyTransformer, out Transform originTransform)
        {
            // Correct version of bodyTransformer?.xrOrigin?.Origin?.transform
            if (bodyTransformer != null)
            {
                var xrOrigin = bodyTransformer.xrOrigin;
                if (xrOrigin != null)
                {
                    var origin = xrOrigin.Origin;
                    if (origin != null)
                    {
                        originTransform = origin.transform;
                        return true;
                    }
                }
            }

            originTransform = null;
            return false;
        }
    }
}
