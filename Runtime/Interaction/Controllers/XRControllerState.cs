using System;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Represents a serializable state of the XR Interactor for use with recording and playback.
    /// </summary>
    /// <remarks>
    /// Use of this class should be avoided outside of the context of playback and recording.
    /// Instead, use <see cref="XRBaseInputInteractor.LogicalInputState"/> properties in <see cref="XRBaseInputInteractor"/> when possible.
    /// </remarks>
    /// <seealso cref="XRControllerRecording"/>
    [Serializable]
    public partial class XRControllerState
    {
        /// <summary>
        /// The time value for this controller.
        /// </summary>
        public double time;

        /// <summary>
        /// The input tracking state of the controller.
        /// </summary>
        public InputTrackingState inputTrackingState;

        /// <summary>
        /// Whether the controller is actively tracked.
        /// </summary>
        public bool isTracked;

        /// <summary>
        /// The position of the controller.
        /// </summary>
        public Vector3 position;

        /// <summary>
        /// The rotation of the controller.
        /// </summary>
        public Quaternion rotation;

        /// <summary>
        /// The selection interaction state.
        /// </summary>
        public InteractionState selectInteractionState;

        /// <summary>
        /// The activate interaction state.
        /// </summary>
        public InteractionState activateInteractionState;

        /// <summary>
        /// The UI press interaction state.
        /// </summary>
        public InteractionState uiPressInteractionState;

        /// <summary>
        /// The UI scroll value.
        /// </summary>
        public Vector2 uiScrollValue;

        /// <summary>
        /// Initializes and returns an instance of <see cref="XRControllerState"/>.
        /// </summary>
        /// <param name="time">The time value for this controller.</param>
        /// <param name="position">The position for this controller.</param>
        /// <param name="rotation">The rotation for this controller.</param>
        /// <param name="inputTrackingState">The inputTrackingState for this controller.</param>
        /// <param name="isTracked">Whether the controller is tracked this frame.</param>
        protected XRControllerState(double time, Vector3 position, Quaternion rotation, InputTrackingState inputTrackingState, bool isTracked)
        {
            this.time = time;
            this.position = position;
            this.rotation = rotation;
            this.inputTrackingState = inputTrackingState;
            this.isTracked = isTracked;
        }

        /// <summary>
        /// Initializes and returns an instance of <see cref="XRControllerState"/>.
        /// </summary>
        public XRControllerState() : this(0d, Vector3.zero, Quaternion.identity, InputTrackingState.None, false)
        {
        }

        /// <summary>
        /// Initializes and returns an instance of <see cref="XRControllerState"/>.
        /// </summary>
        /// <param name="value"> The <see cref="XRControllerState"/> object used to create this object.</param>
        public XRControllerState(XRControllerState value)
        {
            this.time = value.time;
            this.position = value.position;
            this.rotation = value.rotation;
            this.inputTrackingState = value.inputTrackingState;
            this.isTracked = value.isTracked;
            this.selectInteractionState = value.selectInteractionState;
            this.activateInteractionState = value.activateInteractionState;
            this.uiPressInteractionState = value.uiPressInteractionState;
            this.uiScrollValue = value.uiScrollValue;
        }

        /// <summary>
        /// Initializes and returns an instance of <see cref="XRControllerState"/>.
        /// </summary>
        /// <param name="time">The time value for this controller.</param>
        /// <param name="position">The position for this controller.</param>
        /// <param name="rotation">The rotation for this controller.</param>
        /// <param name="inputTrackingState">The inputTrackingState for this controller.</param>
        /// <param name="isTracked">Whether the controller is tracked this frame.</param>
        /// <param name="selectActive">Whether select is active or not.</param>
        /// <param name="activateActive">Whether activate is active or not.</param>
        /// <param name="pressActive">Whether UI press is active or not.</param>
        public XRControllerState(double time, Vector3 position, Quaternion rotation, InputTrackingState inputTrackingState, bool isTracked,
            bool selectActive, bool activateActive, bool pressActive)
            : this(time, position, rotation, inputTrackingState, isTracked)
        {
            this.selectInteractionState.SetFrameState(selectActive);
            this.activateInteractionState.SetFrameState(activateActive);
            this.uiPressInteractionState.SetFrameState(pressActive);
        }

        /// <summary>
        /// Initializes and returns an instance of <see cref="XRControllerState"/>.
        /// </summary>
        /// <param name="time">The time value for this controller.</param>
        /// <param name="position">The position for this controller.</param>
        /// <param name="rotation">The rotation for this controller.</param>
        /// <param name="inputTrackingState">The inputTrackingState for this controller.</param>
        /// <param name="isTracked">Whether the controller is tracked this frame.</param>
        /// <param name="selectActive">Whether select is active or not.</param>
        /// <param name="activateActive">Whether activate is active or not.</param>
        /// <param name="pressActive">Whether UI press is active or not.</param>
        /// <param name="selectValue">The select value.</param>
        /// <param name="activateValue">The activate value.</param>
        /// <param name="pressValue">The UI press value.</param>
        public XRControllerState(double time, Vector3 position, Quaternion rotation, InputTrackingState inputTrackingState, bool isTracked,
            bool selectActive, bool activateActive, bool pressActive,
            float selectValue, float activateValue, float pressValue)
            : this(time, position, rotation, inputTrackingState, isTracked)
        {
            this.selectInteractionState.SetFrameState(selectActive, selectValue);
            this.activateInteractionState.SetFrameState(activateActive, activateValue);
            this.uiPressInteractionState.SetFrameState(pressActive, pressValue);
        }

        /// <summary>
        /// Resets all the interaction states that are based on whether they occurred "this frame".
        /// </summary>
        /// <seealso cref="InteractionState.ResetFrameDependent"/>
        public void ResetFrameDependentStates()
        {
            selectInteractionState.ResetFrameDependent();
            activateInteractionState.ResetFrameDependent();
            uiPressInteractionState.ResetFrameDependent();
        }

        /// <summary>
        /// Converts state data to a string.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString() => $"time: {time}, position: {position}, rotation: {rotation}, selectActive: {selectInteractionState.active}, activateActive: {activateInteractionState.active}, pressActive: {uiPressInteractionState.active}, isTracked: {isTracked}, inputTrackingState: {inputTrackingState}";
    }
}
