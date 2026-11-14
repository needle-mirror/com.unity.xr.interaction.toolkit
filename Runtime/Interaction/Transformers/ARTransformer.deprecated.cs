#if AR_FOUNDATION_PRESENT || PACKAGE_DOCS_GENERATION
using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Transformers
{
    public partial class ARTransformer : XRBaseGrabTransformer, IXRDropTransformer
    {
        [SerializeField]
        [Obsolete("m_PlaneClassificationsList is marked for deprecation in XRI 7.0.0 and will be removed in a future version. Use m_PlaneClassifications instead.")]
        List<PlaneClassification> m_PlaneClassificationsList;

        /// <summary>
        /// The classifications a plane needs to match one of to allow interactable manipulation with.
        /// Only used if <see cref="enablePlaneClassificationFilter"/> is enabled.
        /// </summary>
        /// <seealso cref="enablePlaneClassificationFilter"/>
        [Obsolete("planeClassificationsList is marked for deprecation in XRI 7.0.0 and will be removed in a future version. Use planeClassifications instead.")]
        public List<PlaneClassification> planeClassificationsList
        {
            get => m_PlaneClassificationsList;
            set => m_PlaneClassificationsList = value;
        }

#if AR_FOUNDATION_6_0_OR_NEWER
        [Obsolete("MigratePlaneClassifications is marked for deprecation in XRI 7.0.0 and will be removed in a future version. It is only used for migrating PlaneClassification enum changes in AR Foundation 6.0.")]
        internal void MigratePlaneClassifications()
        {
            if (m_PlaneClassificationsList.Count == 0)
                return;

            foreach (var classification in m_PlaneClassificationsList)
            {
                m_PlaneClassifications |= classification.ConvertToPlaneClassifications();
            }

            m_PlaneClassificationsList.Clear();
        }
#endif
    }
}
#endif
