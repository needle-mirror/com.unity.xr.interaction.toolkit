#if AR_FOUNDATION_PRESENT || PACKAGE_DOCS_GENERATION
using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

namespace UnityEditor.XR.Interaction.Toolkit.Transformers
{
    public partial class ARTransformerEditor : BaseInteractionEditor
    {
        /// <summary><see cref="SerializedProperty"/> of the <see cref="SerializeField"/> backing <see cref="ARTransformer.planeClassificationsList"/>.</summary>
        [Obsolete("m_PlaneClassificationsList is marked for deprecation in XRI 7.0.0 and will be removed in a future version. Use m_PlaneClassifications instead.")]
        protected SerializedProperty m_PlaneClassificationsList;

        protected static partial class Contents
        {
            /// <summary><see cref="GUIContent"/> for <see cref="ARTransformer.planeClassificationsList"/>.</summary>
            [Obsolete("planeClassificationsList is marked for deprecation in XRI 7.0.0 and will be removed in a future version. Use planeClassifications instead.")]
            public static readonly GUIContent planeClassificationsList = EditorGUIUtility.TrTextContent("Plane Classifications List", "The classifications a plane needs to match one of to allow interactable manipulation with.");
#if AR_FOUNDATION_6_0_OR_NEWER
            /// <summary>The help box message when a version of AR Foundation 6.0 or higher is present and the old PlaneClassification property was previously set.</summary>
            [Obsolete("planeClassificationWarning is marked for deprecation in XRI 7.0.0 and will be removed in a future version. It is only used for migrating PlaneClassification enum changes in AR Foundation 6.0")]
            public static readonly GUIContent planeClassificationWarning = EditorGUIUtility.TrTextContent("The PlaneClassification enum was deprecated in AR Foundation 6.0. The old plane classification filter property for the ARTransformer contains prior settings. Please update your settings to the new PlaneClassifications enum type.");
#endif
        }
    }
}
#endif
