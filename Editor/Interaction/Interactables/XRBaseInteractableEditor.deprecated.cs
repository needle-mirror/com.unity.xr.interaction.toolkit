using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Object = UnityEngine.Object;

namespace UnityEditor.XR.Interaction.Toolkit.Interactables
{
    public partial class XRBaseInteractableEditor
    {       
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractable.interactionLayerMask"/>.</summary>
        [Obsolete("m_InteractionLayerMask is marked for deprecation and will be removed in a future version. It is only used for migrating layers.", true)]
        protected SerializedProperty m_InteractionLayerMask;
        
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractable.onFirstHoverEntered"/>.</summary>
        [Obsolete("m_OnFirstHoverEntered is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        protected SerializedProperty m_OnFirstHoverEntered;
        
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractable.onLastHoverExited"/>.</summary>
        [Obsolete("m_OnLastHoverExited is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        protected SerializedProperty m_OnLastHoverExited;
        
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractable.onHoverEntered"/>.</summary>
        [Obsolete("m_OnHoverEntered is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        protected SerializedProperty m_OnHoverEntered;
        
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractable.onHoverExited"/>.</summary>
        [Obsolete("m_OnHoverExited is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        protected SerializedProperty m_OnHoverExited;
        
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractable.onSelectEntered"/>.</summary>
        [Obsolete("m_OnSelectEntered is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        protected SerializedProperty m_OnSelectEntered;
        
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractable.onSelectExited"/>.</summary>
        [Obsolete("m_OnSelectExited is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        protected SerializedProperty m_OnSelectExited;
        
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractable.onSelectCanceled"/>.</summary>
        [Obsolete("m_OnSelectCanceled is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        protected SerializedProperty m_OnSelectCanceled;
        
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractable.onActivate"/>.</summary>
        [Obsolete("m_OnActivate is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        protected SerializedProperty m_OnActivate;
        
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="XRBaseInteractable.onDeactivate"/>.</summary>
        [Obsolete("m_OnDeactivate is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        protected SerializedProperty m_OnDeactivate;

        /// <summary><see cref="SerializedProperty"/> of the persistent calls backing <see cref="XRBaseInteractable.onFirstHoverEntered"/>.</summary>
        [Obsolete("m_OnFirstHoverEnteredCalls is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        protected SerializedProperty m_OnFirstHoverEnteredCalls;
        
        /// <summary><see cref="SerializedProperty"/> of the persistent calls backing <see cref="XRBaseInteractable.onLastHoverExited"/>.</summary>
        [Obsolete("m_OnLastHoverExitedCalls is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        protected SerializedProperty m_OnLastHoverExitedCalls;
        
        /// <summary><see cref="SerializedProperty"/> of the persistent calls backing <see cref="XRBaseInteractable.onHoverEntered"/>.</summary>
        [Obsolete("m_OnHoverEnteredCalls is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        protected SerializedProperty m_OnHoverEnteredCalls;
        
        /// <summary><see cref="SerializedProperty"/> of the persistent calls backing <see cref="XRBaseInteractable.onHoverExited"/>.</summary>
        [Obsolete("m_OnHoverExitedCalls is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        protected SerializedProperty m_OnHoverExitedCalls;
        
        /// <summary><see cref="SerializedProperty"/> of the persistent calls backing <see cref="XRBaseInteractable.onSelectEntered"/>.</summary>
        [Obsolete("m_OnSelectEnteredCalls is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        protected SerializedProperty m_OnSelectEnteredCalls;
        
        /// <summary><see cref="SerializedProperty"/> of the persistent calls backing <see cref="XRBaseInteractable.onSelectExited"/>.</summary>
        [Obsolete("m_OnSelectExitedCalls is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        protected SerializedProperty m_OnSelectExitedCalls;
        
        /// <summary><see cref="SerializedProperty"/> of the persistent calls backing <see cref="XRBaseInteractable.onSelectCanceled"/>.</summary>
        [Obsolete("m_OnSelectCanceledCalls is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        protected SerializedProperty m_OnSelectCanceledCalls;
        
        /// <summary><see cref="SerializedProperty"/> of the persistent calls backing <see cref="XRBaseInteractable.onActivate"/>.</summary>
        [Obsolete("m_OnActivateCalls is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        protected SerializedProperty m_OnActivateCalls;
        
        /// <summary><see cref="SerializedProperty"/> of the persistent calls backing <see cref="XRBaseInteractable.onDeactivate"/>.</summary>
        [Obsolete("m_OnDeactivateCalls is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        protected SerializedProperty m_OnDeactivateCalls;

        /// <summary>
        /// Contents of GUI elements used by this editor.
        /// </summary>
        protected static partial class BaseContents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractable.interactionLayerMask"/>.</summary>
            [Obsolete("interactionLayerMask is marked for deprecation and will be removed in a future version. It is only used for migrating layers in XRI 2.x.", true)]
            public static readonly GUIContent interactionLayerMask = EditorGUIUtility.TrTextContent("Deprecated Interaction Layer Mask", "Deprecated Interaction Layer Mask that uses the Unity physics Layers. Hide this property by disabling \'Show Old Interaction Layer Mask In Inspector\' in the XR Interaction Toolkit project settings.");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractable.onFirstHoverEntered"/>.</summary>
            [Obsolete("onFirstHoverEntered is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
            public static readonly GUIContent onFirstHoverEntered = EditorGUIUtility.TrTextContent("(Deprecated) On First Hover Entered");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractable.onLastHoverExited"/>.</summary>
            [Obsolete("onLastHoverExited is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
            public static readonly GUIContent onLastHoverExited = EditorGUIUtility.TrTextContent("(Deprecated) On Last Hover Exited");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractable.onHoverEntered"/>.</summary>
            [Obsolete("onHoverEntered is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
            public static readonly GUIContent onHoverEntered = EditorGUIUtility.TrTextContent("(Deprecated) On Hover Entered");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractable.onHoverExited"/>.</summary>
            [Obsolete("onHoverExited is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
            public static readonly GUIContent onHoverExited = EditorGUIUtility.TrTextContent("(Deprecated) On Hover Exited");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractable.onSelectEntered"/>.</summary>
            [Obsolete("onSelectEntered is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
            public static readonly GUIContent onSelectEntered = EditorGUIUtility.TrTextContent("(Deprecated) On Select Entered");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractable.onSelectExited"/>.</summary>
            [Obsolete("onSelectExited is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
            public static readonly GUIContent onSelectExited = EditorGUIUtility.TrTextContent("(Deprecated) On Select Exited");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractable.onSelectCanceled"/>.</summary>
            [Obsolete("onSelectCanceled is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
            public static readonly GUIContent onSelectCanceled = EditorGUIUtility.TrTextContent("(Deprecated) On Select Canceled");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractable.onActivate"/>.</summary>
            [Obsolete("onActivate is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
            public static readonly GUIContent onActivate = EditorGUIUtility.TrTextContent("(Deprecated) On Activate");
            /// <summary><see cref="GUIContent"/> for <see cref="XRBaseInteractable.onDeactivate"/>.</summary>
            [Obsolete("onDeactivate is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
            public static readonly GUIContent onDeactivate = EditorGUIUtility.TrTextContent("(Deprecated) On Deactivate");
            
            /// <summary>The help box message when deprecated Interactable Events are being used.</summary>
            [Obsolete("deprecatedEventsInUse is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
            public static readonly GUIContent deprecatedEventsInUse = EditorGUIUtility.TrTextContent("Some deprecated Interactable Events are being used. These deprecated events will be removed in a future version. Please convert these to use the newer events, and update script method signatures for Dynamic listeners.");
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
        /// properties to the new events on an <see cref="XRBaseInteractable"/>.
        /// </summary>
        /// <param name="serializedObject">The object to upgrade.</param>
        /// <remarks>
        /// Assumes On Select Exited should be migrated to Select Exited even though
        /// it will now be invoked even when canceled.
        /// On Select Canceled is skipped since it can't be migrated.
        /// </remarks>
        [Obsolete("MigrateEvents is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        protected virtual void MigrateEvents(SerializedObject serializedObject)
        {
        }

        /// <summary>
        /// Migrate the persistent listeners from the deprecated <see cref="UnityEvent"/>
        /// properties to the new events on an <see cref="XRBaseInteractable"/>.
        /// </summary>
        /// <param name="targets">An array of all the objects to upgrade.</param>
        /// <remarks>
        /// Assumes On Select Exited should be migrated to Select Exited even though
        /// it will now be invoked even when canceled.
        /// On Select Canceled is skipped since it can't be migrated.
        /// </remarks>
        [Obsolete("MigrateEvents is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        public void MigrateEvents(Object[] targets)
        {
        }
    }
}
