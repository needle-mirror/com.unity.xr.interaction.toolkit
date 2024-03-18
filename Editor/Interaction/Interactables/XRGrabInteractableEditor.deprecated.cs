using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace UnityEditor.XR.Interaction.Toolkit.Interactables
{
    public partial class XRGrabInteractableEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRGrabInteractable.attachPointCompatibilityMode"/>.</summary>
        [Obsolete("m_AttachPointCompatibilityMode has been deprecated due to the XRGrabInteractable.attachPointCompatibilityMode being deprecated.", true)]
        protected SerializedProperty m_AttachPointCompatibilityMode;

        protected static partial class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="XRGrabInteractable.attachPointCompatibilityMode"/>.</summary>
            [Obsolete("attachPointCompatibilityMode has been deprecated due to the XRGrabInteractable.attachPointCompatibilityMode being deprecated.", true)]
            public static readonly GUIContent attachPointCompatibilityMode = EditorGUIUtility.TrTextContent("Attach Point Compatibility Mode", "Use Default for consistent attach points between all Movement Type values. Use Legacy for older projects that want to maintain the incorrect method which was partially based on center of mass.");
        }
    }
}
