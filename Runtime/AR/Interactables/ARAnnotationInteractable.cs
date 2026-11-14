#if !AR_FOUNDATION_PRESENT && !PACKAGE_DOCS_GENERATION

// Stub class definition used to fool version defines that this MonoScript exists (fixed in 19.3)
namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Controls displaying one or more annotations when hovering over the <see cref="GameObject"/> this component is attached to.
    /// </summary>
    public class ARAnnotationInteractable { }
}

#else

using System;
using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// An annotation that appears when the user hovers over the <see cref="GameObject"/>
    /// that the <see cref="ARAnnotationInteractable"/> component governing this annotation is attached to.
    /// </summary>
    [Serializable]
    [Obsolete(k_ARAnnotationDeprecated, true)]
    public class ARAnnotation
    {
        const string k_ARAnnotationDeprecated = "ARAnnotation has been deprecated. Instead, it is suggested to use LazyFollow and the Hover and Select events from the current interaction system to set the visibility of UGUI canvas objects.";

        /// <summary>
        /// The visualization <see cref="GameObject"/> that will become active when the user hovers over this object.
        /// </summary>
        public GameObject annotationVisualization
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Maximum angle (in radians) off of FOV horizontal center to show annotation.
        /// </summary>
        public float maxFOVCenterOffsetAngle
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Minimum range to show annotation at.
        /// </summary>
        public float minAnnotationRange
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Maximum range to show annotation at.
        /// </summary>
        public float maxAnnotationRange
        {
            get => default;
            set => _ = value;
        }
#pragma warning restore 618
    }

    /// <summary>5
    /// Controls displaying one or more annotations when hovering over the <see cref="GameObject"/> this component is attached to.
    /// </summary>
    [AddComponentMenu("XR/AR/Deprecated/AR Annotation Interactable", 22)]
    [HelpURL(XRHelpURLConstants.k_ARAnnotationInteractable)]
    [Obsolete(k_ARAnnotationInteractableDeprecated, true)]
    public class ARAnnotationInteractable : ARBaseGestureInteractable
    {
        const string k_ARAnnotationInteractableDeprecated = "ARAnnotationInteractable has been deprecated. Instead, it is suggested to use LazyFollow and the Hover and Select events from the current interaction system to set the visibility of UGUI canvas objects.";

        /// <summary>
        /// The list of annotations.
        /// </summary>
        public List<ARAnnotation> annotations
        {
            get => default;
            set => _ = value;
        }

        /// <inheritdoc />
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            throw new NotSupportedException(k_ARAnnotationInteractableDeprecated);
        }
#pragma warning restore 618
    }
}

#endif
