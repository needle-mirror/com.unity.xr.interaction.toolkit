namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// An area is a teleportation destination which teleports the user to their pointed
    /// location on a surface.
    /// </summary>
    /// <seealso cref="TeleportationAnchor"/>
    [AddComponentMenu("XR/Teleportation Area", 11)]
    [HelpURL(XRHelpURLConstants.k_TeleportationArea)]
    public class TeleportationArea : BaseTeleportationInteractable
    {
        /// <inheritdoc />
        protected override bool GenerateTeleportRequest(IXRInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
        {
            if (raycastHit.collider == null)
                return false;

            // If the sphere cast overlaps with the teleportation area at the start of the sweep,
            // do not teleport because the RaycastHit.point is assigned the zero vector
            // instead of a usable collision point. If the cast type isn't sphere cast,
            // then we don't need to bother checking the RaycastHit.
            if (IsSphereCastRay(interactor, out _) &&
                IsSphereCastOverlap(raycastHit))
            {
                return false;
            }

            teleportRequest.destinationPosition = raycastHit.point;
            teleportRequest.destinationRotation = transform.rotation;
            return true;
        }

        /// <inheritdoc />
        public override bool IsSelectableBy(IXRSelectInteractor interactor)
        {
            // Don't allow selection if the sphere cast overlaps with the teleportation area at the start of the sweep.
            // This will allow the line visual to use the blocked style and reticle since the teleport request
            // would be rejected since there isn't a usable collision point.
            var isSelectable = base.IsSelectableBy(interactor);
            if (isSelectable &&
                IsSphereCastRay(interactor, out var rayInteractor) &&
                rayInteractor.TryGetCurrent3DRaycastHit(out var raycastHit) &&
                IsSphereCastOverlap(raycastHit))
            {
                return false;
            }

            return isSelectable;
        }

        static bool IsSphereCastRay(IXRInteractor interactor, out XRRayInteractor rayInteractor)
        {
            rayInteractor = interactor as XRRayInteractor;
            return rayInteractor != null && rayInteractor.hitDetectionType == XRRayInteractor.HitDetectionType.SphereCast;
        }

        static bool IsSphereCastOverlap(RaycastHit raycastHit)
        {
            // Unity returns the RaycastHit from a sphere cast that overlaps with the collider at the start of the sweep
            // with a distance of zero and the point at zero vector.
            // See https://docs.unity3d.com/ScriptReference/Physics.SphereCastAll.html
            if (raycastHit.distance != 0f)
                return false;

            var point = raycastHit.point;
            return point.x == 0f && point.y == 0f && point.z == 0f;
        }
    }
}
