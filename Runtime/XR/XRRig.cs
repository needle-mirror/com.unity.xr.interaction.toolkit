using System;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// (Deprecated) The XR Rig component is typically attached to the base object of the XR Rig,
    /// and stores the <see cref="GameObject"/> that will be manipulated via locomotion.
    /// It is also used for offsetting the camera.
    /// </summary>
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    [Obsolete(k_ObsoleteMessage, true)]
    [HelpURL(XRHelpURLConstants.k_XRRig)]
    public class XRRig : XROrigin
    {
        const string k_ObsoleteMessage = "XRRig has been deprecated. Use the XROrigin component instead.";

        /// <summary>
        /// See <c>MonoBehaviour.Awake</c>.
        /// </summary>
        protected new void Awake()
        {
            Debug.LogError(k_ObsoleteMessage, this);
            throw new NotSupportedException(k_ObsoleteMessage);
        }

                /// <summary>
        /// (Deprecated) The "Rig" <see cref="GameObject"/> is used to refer to the base of the XR Rig, by default it is this <see cref="GameObject"/>.
        /// This is the <see cref="GameObject"/> that will be manipulated via locomotion.
        /// </summary>
        public GameObject rig
        {
            get => default;
            set => _ = value;
        }

        [SerializeField]
        GameObject m_CameraGameObject;

        /// <summary>
        /// (Deprecated) The <see cref="GameObject"/> that contains the camera, this is usually the "Head" of XR rigs.
        /// </summary>
        public GameObject cameraGameObject
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) The <see cref="GameObject"/> to move to desired height off the floor (defaults to this object if none provided).
        /// This is used to transform the XR device from camera space to XR Origin space.
        /// </summary>
        public GameObject cameraFloorOffsetObject
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) The type of tracking origin to use for this XROrigin. Tracking origins identify where (0, 0, 0) is in the world
        /// of tracking. Not all devices support all tracking origin modes.
        /// </summary>
        /// <seealso cref="XROrigin.TrackingOriginMode"/>
        public TrackingOriginMode requestedTrackingOriginMode
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Deprecated) Camera height to be used when in <c>Device</c> Tracking Origin Mode to define the height of the user from the floor.
        /// This is the amount that the camera is offset from the floor when moving the <see cref="XROrigin.CameraFloorOffsetObject"/>.
        /// </summary>
        public float cameraYOffset
        {
            get => default;
            set => _ = value;
        }

#if ENABLE_VR || UNITY_GAMECORE || PACKAGE_DOCS_GENERATION
        /// <summary>
        /// (Deprecated) (Read Only) The Tracking Origin Mode of this XR Origin.
        /// </summary>
        /// <seealso cref="XROrigin.RequestedTrackingOriginMode"/>
        public TrackingOriginModeFlags currentTrackingOriginMode => default;
#endif

        /// <summary>
        /// (Deprecated) (Read Only) The rig's local position in camera space.
        /// </summary>
        public Vector3 rigInCameraSpacePos => default;

        /// <summary>
        /// (Deprecated) (Read Only) The camera's local position in rig space.
        /// </summary>
        public Vector3 cameraInRigSpacePos => default;

        /// <summary>
        /// (Deprecated) (Read Only) The camera's height relative to the rig.
        /// </summary>
        public float cameraInRigSpaceHeight => default;

        /// <summary>
        /// (Deprecated) Rotates the rig object around the camera object by the provided <paramref name="angleDegrees"/>.
        /// This rotation only occurs around the rig's Up vector
        /// </summary>
        /// <param name="angleDegrees">The amount of rotation in degrees.</param>
        /// <returns>Returns <see langword="true"/> if the rotation is performed. Otherwise, returns <see langword="false"/>.</returns>
        public bool RotateAroundCameraUsingRigUp(float angleDegrees) => default;

        /// <summary>
        /// (Deprecated) This function will rotate the rig object such that the rig's up vector will match the provided vector.
        /// </summary>
        /// <param name="destinationUp">the vector to which the rig object's up vector will be matched.</param>
        /// <returns>Returns <see langword="true"/> if the rotation is performed or the vectors have already been matched.
        /// Otherwise, returns <see langword="false"/>.</returns>
        public bool MatchRigUp(Vector3 destinationUp) => default;

        /// <summary>
        /// (Deprecated) This function will rotate the rig object around the camera object using the <paramref name="destinationUp"/> vector such that:
        /// <list type="bullet">
        /// <item>
        /// <description>The camera will look at the area in the direction of <paramref name="destinationForward"/></description>
        /// </item>
        /// <item>
        /// <description>The projection of camera's forward vector on the plane with the normal <paramref name="destinationUp"/> will be in the direction of <paramref name="destinationForward"/></description>
        /// </item>
        /// <item>
        /// <description>The up vector of the rig object will match the provided <paramref name="destinationUp"/> vector (note that the camera's Up vector can not be manipulated)</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="destinationUp">The up vector that the rig's up vector will be matched to.</param>
        /// <param name="destinationForward">The forward vector that will be matched to the projection of the camera's forward vector on the plane with the normal <paramref name="destinationUp"/>.</param>
        /// <returns>Returns <see langword="true"/> if the rotation is performed. Otherwise, returns <see langword="false"/>.</returns>
        public bool MatchRigUpCameraForward(Vector3 destinationUp, Vector3 destinationForward) => default;

        /// <summary>
        /// (Deprecated) This function will rotate the rig object around the camera object using the <paramref name="destinationUp"/> vector such that:
        /// <list type="bullet">
        /// <item>
        /// <description>The forward vector of the rig object, which is the direction the player moves in Unity when walking forward in the physical world, will match the provided <paramref name="destinationUp"/> vector</description>
        /// </item>
        /// <item>
        /// <description>The up vector of the rig object will match the provided <paramref name="destinationUp"/> vector</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="destinationUp">The up vector that the rig's up vector will be matched to.</param>
        /// <param name="destinationForward">The forward vector that will be matched to the forward vector of the rig object,
        /// which is the direction the player moves in Unity when walking forward in the physical world.</param>
        /// <returns>Returns <see langword="true"/> if the rotation is performed. Otherwise, returns <see langword="false"/>.</returns>
        public bool MatchRigUpRigForward(Vector3 destinationUp, Vector3 destinationForward) => default;
    }
}
