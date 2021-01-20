using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// The <see cref="XRControllerRecording"/> <see cref="ScriptableObject"/> stores position, rotation,
    /// and Interaction state changes from the XR Controller for playback.
    /// </summary>
    [CreateAssetMenu(menuName = "XR/XR Controller Recording")]
    [Serializable, PreferBinarySerialization]
    [HelpURL(XRHelpURLConstants.k_XRControllerRecording)]
    public class XRControllerRecording : ScriptableObject
    {
        [SerializeField, Tooltip("Frames stored in this recording.")]
#pragma warning disable IDE0044 // Add readonly modifier -- readonly fields cannot be serialized by Unity
        List<XRControllerState> m_Frames = new List<XRControllerState>();
#pragma warning restore IDE0044

        /// <summary>
        /// (Read Only) Frames stored in this recording.
        /// </summary>
        public List<XRControllerState> frames => m_Frames;

        /// <summary>
        /// (Read Only) Total playback time for this recording.
        /// </summary>
        public double duration => m_Frames.Count == 0 ? 0d : m_Frames[m_Frames.Count - 1].time;

        /// <summary>
        /// Adds a recording of a frame.
        /// </summary>
        /// <param name="time">The time for this controller frame.</param>
        /// <param name="position">The position for this controller frame.</param>
        /// <param name="rotation">The rotation for this controller frame.</param>
        /// <param name="selectActive">Whether select is active or not.</param>
        /// <param name="activateActive">Whether activate is active or not.</param>
        /// <param name="pressActive">Whether press is active or not.</param>
        public void AddRecordingFrame(
            double time, Vector3 position, Quaternion rotation, bool selectActive, bool activateActive, bool pressActive)
        {
            frames.Add(new XRControllerState(time, position, rotation, selectActive, activateActive, pressActive));
        }

        /// <summary>
        /// Adds a recording of a frame.
        /// </summary>
        /// <param name="state"> The <seealso cref="XRControllerState"/> to be recorded.</param>
        public void AddRecordingFrame(XRControllerState state)
        {
            frames.Add(new XRControllerState(state));
        }

        /// <summary>
        /// Initializes this recording by clearing all frames stored.
        /// </summary>
        public void InitRecording()
        {
            m_Frames.Clear();
#if UNITY_EDITOR
            Undo.RecordObject(this, "Recording XR Controller");
#endif
        }

        /// <summary>
        /// Saves this recording and writes to disk.
        /// </summary>
        public void SaveRecording()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
#endif
        }
    }
}
