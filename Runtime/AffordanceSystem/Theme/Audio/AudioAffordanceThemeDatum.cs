using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils.Datums;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Theme.Audio
{
    /// <summary>
    /// Affordance state theme data structure for for Audio Clip affordances.
    /// </summary>
    [Serializable]
    public sealed class AudioAffordanceThemeData
    {
        /// <summary>
        /// Name of the affordance state the theme data is for.
        /// This value is optional and does not serve a functional purpose.
        /// </summary>
        public string stateName;

        /// <summary>
        /// Audio clip to play when the state is entered.
        /// </summary>
        public AudioClip stateEntered;

        /// <summary>
        /// Audio clip to play when the state is exited.
        /// </summary>
        public AudioClip stateExited;
    }

    /// <summary>
    /// Audio clip affordance theme.
    /// </summary>
    /// <remarks>
    /// It does not support state tweening so it is simplified and does not inherit from <see cref="BaseAffordanceTheme{T}"/>.
    /// </remarks>
    [Serializable]
    public class AudioAffordanceTheme
    {
        [SerializeField]
        List<AudioAffordanceThemeData> m_List;

        /// <summary>
        /// Initializes and returns an instance of <see cref="AudioAffordanceTheme"/>.
        /// </summary>
        protected AudioAffordanceTheme()
        {
            // Create initial list with states and names as initial order
            m_List = new List<AudioAffordanceThemeData>
            {
                new AudioAffordanceThemeData { stateName = nameof(AffordanceStateShortcuts.disabled) },
                new AudioAffordanceThemeData { stateName = nameof(AffordanceStateShortcuts.idle) },
                new AudioAffordanceThemeData { stateName = nameof(AffordanceStateShortcuts.hovered) },
                new AudioAffordanceThemeData { stateName = nameof(AffordanceStateShortcuts.hoveredPriority) },
                new AudioAffordanceThemeData { stateName = nameof(AffordanceStateShortcuts.selected) },
                new AudioAffordanceThemeData { stateName = nameof(AffordanceStateShortcuts.activated) },
            };
        }

        /// <summary>
        /// Gets the affordance theme data for the affordance state.
        /// </summary>
        /// <param name="stateIndex">The affordance state index to get the theme data for.</param>
        /// <returns>Returns the affordance theme data for the affordance state,
        /// or <see langword="null"/> if there is no theme data associated with the state.</returns>
        public AudioAffordanceThemeData GetAffordanceThemeDataForIndex(byte stateIndex)
        {
            return stateIndex <= m_List.Count ? m_List[stateIndex] : null;
        }
    }

    /// <summary>
    /// Serializable container class that holds an audio clip affordance theme value or container asset reference.
    /// </summary>
    /// <seealso cref="AudioAffordanceThemeDatum"/>
    [Serializable]
    public class AudioAffordanceThemeDatumProperty : DatumProperty<AudioAffordanceTheme, AudioAffordanceThemeDatum>
    {
        /// <inheritdoc/>
        public AudioAffordanceThemeDatumProperty(AudioAffordanceTheme value) : base(value)
        {
        }

        /// <inheritdoc/>
        public AudioAffordanceThemeDatumProperty(AudioAffordanceThemeDatum datum) : base(datum)
        {
        }
    }

    /// <summary>
    /// <see cref="ScriptableObject"/> container class that holds an audio clip affordance theme value.
    /// </summary>
    [CreateAssetMenu(fileName = "AudioAffordanceTheme", menuName = "Affordance Theme/Audio Affordance Theme", order = 0)]
    [HelpURL(XRHelpURLConstants.k_AudioAffordanceThemeDatum)]
    public class AudioAffordanceThemeDatum : Datum<AudioAffordanceTheme>
    {
    }
}
