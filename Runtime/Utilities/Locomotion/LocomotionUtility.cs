using Unity.XR.CoreUtils;

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
    }
}
