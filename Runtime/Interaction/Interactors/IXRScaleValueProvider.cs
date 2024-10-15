using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
    /// <summary>
    /// Enum representing the two modes of scaling: <see cref="ScaleOverTime"/> and <see cref="DistanceDelta"/>.
    /// </summary>
    /// <seealso cref="IXRScaleValueProvider.scaleMode"/>
    /// <seealso cref="XRRayInteractor.scaleMode"/>
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
    public enum ScaleMode
    {
        /// <summary>
        /// No scale mode is active or supported.
        /// Use this when a controller does not support scaling or when scaling is not needed.
        /// </summary>
        None,

        /// <summary>
        /// Scale over time mode: The scale is resized over time and represented by input in range of -1 to 1.
        /// This mode is typically used with a thumbstick input on a controller.
        /// </summary>
        ScaleOverTime,

        /// <summary>
        /// (Deprecated) Use <see cref="ScaleOverTime"/> instead.
        /// </summary>
        [Obsolete("Input has been renamed in version 3.0.0. Use ScaleOverTime instead. (UnityUpgradable) -> ScaleOverTime")]
        Input = ScaleOverTime,

        /// <summary>
        /// Distance scale mode: The scale is based on the delta distance between 2 physical (or virtual) inputs, such as
        /// the pinch gap between fingers where the distance is calculated based on the screen DPI, and delta from the previous frame.
        /// This mode is typically used with a touchscreen for mobile AR.
        /// </summary>
        DistanceDelta,

        /// <summary>
        /// (Deprecated) Use <see cref="DistanceDelta"/> instead.
        /// </summary>
        [Obsolete("Distance has been renamed in version 3.0.0. Use DistanceDelta instead. (UnityUpgradable) -> DistanceDelta")]
        Distance = DistanceDelta,
    }

    /// <summary>
    /// Defines an interface for scale value providers.
    /// </summary>
    /// <remarks>
    /// Implementations of this interface provide a mechanism to get a scale value (a change in scale)
    /// from an input control, such as a gesture or controller stick movement. The provided scale value is in the
    /// mode supported by the upstream controller.
    /// </remarks>
    /// <seealso cref="XRRayInteractor"/>
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
    public interface IXRScaleValueProvider
    {
        /// <summary>
        /// Property representing the scale mode that is supported by the implementation of the interface.
        /// </summary>
        /// <seealso cref="ScaleMode"/>
        ScaleMode scaleMode { get; set; }

        /// <summary>
        /// This is the current scale value for the specified scale mode. This value should be updated
        /// by the implementing class when other inputs are handled during the standard interaction processing loop.
        /// </summary>
        /// <seealso cref="scaleMode"/>
        float scaleValue { get; }
    }
}
