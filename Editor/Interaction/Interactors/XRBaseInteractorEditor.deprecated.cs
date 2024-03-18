using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using Object = UnityEngine.Object;

namespace UnityEditor.XR.Interaction.Toolkit.Interactors
{
    public partial class XRBaseInteractorEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractor.interactionLayerMask"/>.</summary>
        [Obsolete("m_InteractionLayerMask property was deprecated from XRBaseInteractor and will be removed in a future version of XRI. Use the m_InteractionLayers instead.", true)]
        protected SerializedProperty m_InteractionLayerMask;
     
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractor.onHoverEntered"/>.</summary>
        [Obsolete("m_OnHoverEntered property was deprecated from XRBaseInteractor and will be removed in a future version of XRI. Use the m_HoverEntered instead.", true)]
        protected SerializedProperty m_OnHoverEntered;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractor.onHoverExited"/>.</summary>
        [Obsolete("m_OnHoverExited property was deprecated from XRBaseInteractor and will be removed in a future version of XRI. Use the m_HoverExited instead.", true)]
        protected SerializedProperty m_OnHoverExited;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractor.onSelectEntered"/>.</summary>
        [Obsolete("m_OnSelectEntered property was deprecated from XRBaseInteractor and will be removed in a future version of XRI. Use the m_SelectEntered instead.", true)]
        protected SerializedProperty m_OnSelectEntered;
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractor.onSelectExited"/>.</summary>
        [Obsolete("m_OnSelectExited property was deprecated from XRBaseInteractor and will be removed in a future version of XRI. Use the m_SelectExited instead.", true)]
        protected SerializedProperty m_OnSelectExited;

        /// <summary><see cref="SerializedProperty"/> of the persistent calls backing <see cref="XRBaseInteractor.onHoverEntered"/>.</summary>
        [Obsolete("m_HoverEnteredCalls property was deprecated from XRBaseInteractor and will be removed in a future version of XRI. Use the m_HoverEntered instead.", true)]
        protected SerializedProperty m_OnHoverEnteredCalls;
        /// <summary><see cref="SerializedProperty"/> of the persistent calls backing <see cref="XRBaseInteractor.onHoverExited"/>.</summary>
        [Obsolete("m_OnHoverExitedCalls property was deprecated from XRBaseInteractor and will be removed in a future version of XRI. Use the m_HoverExitedCalls instead.", true)]
        protected SerializedProperty m_OnHoverExitedCalls;
        /// <summary><see cref="SerializedProperty"/> of the persistent calls backing <see cref="XRBaseInteractor.onSelectEntered"/>.</summary>
        [Obsolete("m_OnSelectEnteredCalls property was deprecated from XRBaseInteractor and will be removed in a future version of XRI. Use the m_SelectEnteredCalls instead.", true)]
        protected SerializedProperty m_OnSelectEnteredCalls;
        /// <summary><see cref="SerializedProperty"/> of the persistent calls backing <see cref="XRBaseInteractor.onSelectExited"/>.</summary>
        [Obsolete("m_OnSelectExitedCalls property was deprecated from XRBaseInteractor and will be removed in a future version of XRI. Use the m_SelectExitedCalls instead.", true)]
        protected SerializedProperty m_OnSelectExitedCalls;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static partial class BaseContents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractor.interactionLayerMask"/>.</summary>
            [Obsolete("interactionLayerMask property was deprecated from XRBaseInteractor and will be removed in a future version of XRI.", true)]
            public static readonly GUIContent interactionLayerMask = EditorGUIUtility.TrTextContent("Deprecated Interaction Layer Mask", "Deprecated Interaction Layer Mask that uses the Unity physics Layers. Hide this property by disabling \'Show Old Interaction Layer Mask In Inspector\' in the XR Interaction Toolkit project settings.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractor.onHoverEntered"/>.</summary>
            [Obsolete("onHoverEntered property was deprecated from XRBaseInteractor and will be removed in a future version of XRI.", true)]
            public static readonly GUIContent onHoverEntered = EditorGUIUtility.TrTextContent("(Deprecated) On Hover Entered");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractor.onHoverExited"/>.</summary>
            [Obsolete("onHoverExited property was deprecated from XRBaseInteractor and will be removed in a future version of XRI.", true)]
            public static readonly GUIContent onHoverExited = EditorGUIUtility.TrTextContent("(Deprecated) On Hover Exited");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractor.onSelectEntered"/>.</summary>
            [Obsolete("onSelectEntered property was deprecated from XRBaseInteractor and will be removed in a future version of XRI.", true)]
            public static readonly GUIContent onSelectEntered = EditorGUIUtility.TrTextContent("(Deprecated) On Select Entered");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractor.onSelectExited"/>.</summary>
            [Obsolete("onSelectExited property was deprecated from XRBaseInteractor and will be removed in a future version of XRI.", true)]
            public static readonly GUIContent onSelectExited = EditorGUIUtility.TrTextContent("(Deprecated) On Select Exited");
            
            /// <summary>The help box message when deprecated Interactor Events are being used.</summary>
            [Obsolete("deprecatedEventsInUse property was deprecated from XRBaseInteractor and will be removed in a future version of XRI.", true)]
            public static readonly GUIContent deprecatedEventsInUse = EditorGUIUtility.TrTextContent("Some deprecated Interactor Events are being used. These deprecated events will be removed in a future version. Please convert these to use the newer events, and update script method signatures for Dynamic listeners.");
        }

        /// <summary>
        /// Get whether deprecated events are in use.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if deprecated events are in use. Otherwise, returns <see langword="false"/>.</returns>
        [Obsolete("IsDeprecatedEventsInUse is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        protected virtual bool IsDeprecatedEventsInUse()
        {
            return default;
        }

        /// <summary>
        /// Migrate the persistent listeners from the deprecated <see cref="UnityEvent"/>
        /// properties to the new events on an <see cref="XRBaseInteractor"/>.
        /// </summary>
        /// <param name="serializedObject">The object to upgrade.</param>
        [Obsolete("MigrateEvents is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        protected virtual void MigrateEvents(SerializedObject serializedObject)
        {
        }

        /// <summary>
        /// Migrate the persistent listeners from the deprecated <see cref="UnityEvent"/>
        /// properties to the new events on an <see cref="XRBaseInteractor"/>.
        /// </summary>
        /// <param name="targets">An array of all the objects to upgrade.</param>
        [Obsolete("MigrateEvents is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        public void MigrateEvents(Object[] targets)
        {
        }
    }
}
