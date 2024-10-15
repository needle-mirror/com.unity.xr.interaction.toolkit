using Unity.Collections;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals
{
    /// <summary>
    /// Enum representing the the various types of interaction results that dictate the behavior and world space location of the endpoint.
    /// </summary>
    public enum EndPointType
    {
        /// <summary>
        /// No interaction result.
        /// </summary>
        None,
        /// <summary>
        /// Endpoint is located on the surface of hit 3D geometry that is not a valid interactable target.
        /// </summary>
        EmptyCastHit,
        /// <summary>
        /// Endpoint is located on the surface of hit 3D geometry that is a valid interactable target.
        /// </summary>
        ValidCastHit,
        /// <summary>
        /// End point is snapped to the attach point of a selected interactable.
        /// </summary>
        AttachPoint,
        /// <summary>
        /// End point is located at the world intersection of a UI graphics raycast.
        /// </summary>
        UI,
    }

    /// <summary>
    /// Abstracts the interaction data required to render a curve from an origin to an endpoint.
    /// </summary>
    /// <seealso cref="NearFarInteractor"/>
    public interface ICurveInteractionDataProvider
    {
        /// <summary>
        /// Indicates if the data provider is active. If <see langword="false"/>, the data in the other fields may be stale.
        /// </summary>
        bool isActive { get; }

        /// <summary>
        /// Indicates if the data provider has a valid selection.
        /// </summary>
        bool hasValidSelect { get; }

        /// <summary>
        /// The transform used to determine the origin of the curve.
        /// </summary>
        Transform curveOrigin { get; }

        /// <summary>
        /// The curve sample points used to determine interaction results.
        /// </summary>
        NativeArray<Vector3> samplePoints { get; }

        /// <summary>
        /// Sample point at the last index of <see cref="samplePoints"/>.
        /// </summary>
        Vector3 lastSamplePoint { get; }

        /// <summary>
        /// Attempts to determine the end point of the curve. The end point can be set to either an attach point or the end of a raycast hit.
        /// </summary>
        /// <param name="endPoint">Output parameter that will hold the end point vector.</param>
        /// <param name="snapToSelectedAttachIfAvailable">If set to <see langword="true"/>, the method will try to snap to the selected attach point, if available.</param>
        /// <param name="snapToSnapVolumeIfAvailable">If set to <see langword="true"/>, the method will try to snap to the nearest snap volume, if available.</param>
        /// <returns>Returns an <see cref="EndPointType"/> indicating the type of the end point determined.</returns>
        EndPointType TryGetCurveEndPoint(out Vector3 endPoint, bool snapToSelectedAttachIfAvailable = false, bool snapToSnapVolumeIfAvailable = false);

        /// <summary>
        /// Attempts to determine the normal at the endpoint of the curve. This method will transform the captured local direction of the normal on select if tracking a snapped attach point.
        /// </summary>
        /// <param name="endNormal">Output parameter that will hold the normal vector at the curve's endpoint.</param>
        /// <param name="snapToSelectedAttachIfAvailable">If <see langword="true"/>, the method will attempt to snap to the selected attach point, if it's available.</param>
        /// <returns>Returns an <see cref="EndPointType"/> indicating the type of the endpoint where the normal was determined.</returns>
        EndPointType TryGetCurveEndNormal(out Vector3 endNormal, bool snapToSelectedAttachIfAvailable = false);
    }
}
