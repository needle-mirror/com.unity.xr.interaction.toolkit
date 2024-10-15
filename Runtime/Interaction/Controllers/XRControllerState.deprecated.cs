using System;
#if XR_LEGACY_INPUT_HELPERS_2_1_OR_NEWER || PACKAGE_DOCS_GENERATION
using UnityEngine.SpatialTracking;
#endif

namespace UnityEngine.XR.Interaction.Toolkit
{
    public partial class XRControllerState
    {
#if XR_LEGACY_INPUT_HELPERS_2_1_OR_NEWER || PACKAGE_DOCS_GENERATION
        /// <summary>
        /// (Deprecated) The pose data flags of the controller.
        /// </summary>
        /// <seealso cref="inputTrackingState"/>
        [Obsolete("poseDataFlags has been deprecated. Use inputTrackingState instead.", true)]
        public PoseDataFlags poseDataFlags
        {
            get => default;
            set => _ = value;
        }
#endif

        /// <summary>
        /// (Deprecated) Initializes and returns an instance of <see cref="XRControllerState"/>.
        /// </summary>
        /// <param name="time">The time value for this controller.</param>
        /// <param name="position">The position for this controller.</param>
        /// <param name="rotation">The rotation for this controller.</param>
        /// <param name="selectActive">Whether select is active or not.</param>
        /// <param name="activateActive">Whether activate is active or not.</param>
        /// <param name="pressActive">Whether UI press is active or not.</param>
        [Obsolete("This constructor has been deprecated. Use the constructors with the inputTrackingState parameter.", true)]
        public XRControllerState(double time, Vector3 position, Quaternion rotation, bool selectActive, bool activateActive, bool pressActive)
            : this(time, position, rotation, InputTrackingState.Rotation | InputTrackingState.Position, selectActive, activateActive, pressActive)
        {
        }

        /// <summary>
        /// Initializes and returns an instance of <see cref="XRControllerState"/>.
        /// </summary>
        /// <param name="time">The time value for this controller.</param>
        /// <param name="position">The position for this controller.</param>
        /// <param name="rotation">The rotation for this controller.</param>
        /// <param name="inputTrackingState">The inputTrackingState for this controller.</param>
        [Obsolete("This constructor has been deprecated. Use the constructor with the isTracked parameter.", true)]
        protected XRControllerState(double time, Vector3 position, Quaternion rotation, InputTrackingState inputTrackingState)
            : this(time, position, rotation, inputTrackingState, true)
        {
        }

        /// <summary>
        /// Initializes and returns an instance of <see cref="XRControllerState"/>.
        /// </summary>
        /// <param name="time">The time value for this controller.</param>
        /// <param name="position">The position for this controller.</param>
        /// <param name="rotation">The rotation for this controller.</param>
        /// <param name="inputTrackingState">The inputTrackingState for this controller.</param>
        /// <param name="selectActive">Whether select is active or not.</param>
        /// <param name="activateActive">Whether activate is active or not.</param>
        /// <param name="pressActive">Whether UI press is active or not.</param>
        [Obsolete("This constructor has been deprecated. Use the constructor with the isTracked parameter.", true)]
        public XRControllerState(double time, Vector3 position, Quaternion rotation, InputTrackingState inputTrackingState,
            bool selectActive, bool activateActive, bool pressActive)
            : this(time, position, rotation, inputTrackingState, true)
        {
        }

        /// <summary>
        /// Initializes and returns an instance of <see cref="XRControllerState"/>.
        /// </summary>
        /// <param name="time">The time value for this controller.</param>
        /// <param name="position">The position for this controller.</param>
        /// <param name="rotation">The rotation for this controller.</param>
        /// <param name="inputTrackingState">The inputTrackingState for this controller.</param>
        /// <param name="selectActive">Whether select is active or not.</param>
        /// <param name="activateActive">Whether activate is active or not.</param>
        /// <param name="pressActive">Whether UI press is active or not.</param>
        /// <param name="selectValue">The select value.</param>
        /// <param name="activateValue">The activate value.</param>
        /// <param name="pressValue">The UI press value.</param>
        [Obsolete("This constructor has been deprecated. Use the constructor with the isTracked parameter.", true)]
        public XRControllerState(double time, Vector3 position, Quaternion rotation, InputTrackingState inputTrackingState,
            bool selectActive, bool activateActive, bool pressActive,
            float selectValue, float activateValue, float pressValue)
            : this(time, position, rotation, inputTrackingState, true)
        {
        }

        /// <summary>
        /// (Deprecated) Resets all the interaction states that are based on whether they occurred "this frame".
        /// </summary>
        /// <remarks>
        /// <c>ResetInputs</c> has been renamed. Use <see cref="ResetFrameDependentStates"/> instead.
        /// </remarks>
        [Obsolete("ResetInputs has been renamed. Use ResetFrameDependentStates instead. (UnityUpgradable) -> ResetFrameDependentStates()", true)]
        public void ResetInputs()
        {
        }
    }
}
