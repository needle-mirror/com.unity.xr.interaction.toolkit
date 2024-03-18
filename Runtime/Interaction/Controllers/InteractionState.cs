using System;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Serializable struct to hold logical input state for a given interaction, such as for select.
    /// </summary>
    /// <remarks>
    /// Use of this class should be avoided outside of the context of playback and recording.
    /// Instead, use <see cref="XRBaseInputInteractor.LogicalInputState"/> when possible.
    /// </remarks>
    /// <seealso cref="XRControllerState"/>
    [Serializable]
    public partial struct InteractionState
    {
        [Range(0f, 1f)]
        [SerializeField]
        float m_Value;

        /// <summary>
        /// The value of the interaction in this frame.
        /// </summary>
        public float value
        {
            get => m_Value;
            set => m_Value = value;
        }

        [SerializeField]
        bool m_Active;

        /// <summary>
        /// Whether it is currently on.
        /// </summary>
        public bool active
        {
            get => m_Active;
            set => m_Active = value;
        }

        bool m_ActivatedThisFrame;

        /// <summary>
        /// Whether the interaction state activated this frame.
        /// </summary>
        public bool activatedThisFrame
        {
            get => m_ActivatedThisFrame;
            set => m_ActivatedThisFrame = value;
        }

        bool m_DeactivatedThisFrame;

        /// <summary>
        /// Whether the interaction state deactivated this frame.
        /// </summary>
        public bool deactivatedThisFrame
        {
            get => m_DeactivatedThisFrame;
            set => m_DeactivatedThisFrame = value;
        }

        /// <summary>
        /// Sets the interaction state for this frame. This method should only be called once per frame.
        /// </summary>
        /// <param name="isActive">Whether the state is active (in other words, pressed).</param>
        public void SetFrameState(bool isActive)
        {
            SetFrameState(isActive, isActive ? 1f : 0f);
        }

        /// <summary>
        /// Sets the interaction state for this frame. This method should only be called once per frame.
        /// </summary>
        /// <param name="isActive">Whether the state is active (in other words, pressed).</param>
        /// <param name="newValue">The interaction value.</param>
        public void SetFrameState(bool isActive, float newValue)
        {
            value = newValue;
            activatedThisFrame = !active && isActive;
            deactivatedThisFrame = active && !isActive;
            active = isActive;
        }

        /// <summary>
        /// Sets the interaction state that are based on whether they occurred "this frame".
        /// </summary>
        /// <param name="wasActive">Whether the previous state is active (in other words, pressed).</param>
        public void SetFrameDependent(bool wasActive)
        {
            activatedThisFrame = !wasActive && active;
            deactivatedThisFrame = wasActive && !active;
        }

        /// <summary>
        /// Resets the interaction states that are based on whether they occurred "this frame".
        /// </summary>
        /// <seealso cref="activatedThisFrame"/>
        /// <seealso cref="deactivatedThisFrame"/>
        public void ResetFrameDependent()
        {
            activatedThisFrame = false;
            deactivatedThisFrame = false;
        }
    }
}
