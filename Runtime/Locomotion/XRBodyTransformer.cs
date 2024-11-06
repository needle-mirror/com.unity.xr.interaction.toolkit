using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion
{
    /// <summary>
    /// Behavior that manages user locomotion via transformation of an <see cref="XROrigin.Origin"/>. This behavior
    /// applies queued <see cref="IXRBodyTransformation"/>s every <see cref="Update"/>.
    /// </summary>
    [AddComponentMenu("XR/Locomotion/XR Body Transformer", 11)]
    [HelpURL(XRHelpURLConstants.k_XRBodyTransformer)]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_XRBodyTransformer)]
    public class XRBodyTransformer : MonoBehaviour
    {
        struct OrderedTransformation
        {
            public IXRBodyTransformation transformation;
            public int priority;
        }

        [SerializeField]
        [Tooltip("The XR Origin to transform (will find one if None).")]
        XROrigin m_XROrigin;

        /// <summary>
        /// The XR Origin whose <see cref="XROrigin.Origin"/> to transform (will find one if <see langword="null"/>).
        /// </summary>
        /// <remarks>
        /// Setting this property at runtime also re-links the <see cref="constrainedBodyManipulator"/> to the new origin.
        /// </remarks>
        public XROrigin xrOrigin
        {
            get => m_XROrigin;
            set
            {
                m_XROrigin = value;
                if (Application.isPlaying)
                    InitializeMovableBody();
            }
        }

        [SerializeField]
        [RequireInterface(typeof(IXRBodyPositionEvaluator))]
        [Tooltip("Object that determines the position of the user's body. If set to None, this behavior will estimate " +
            "the position to be the camera position projected onto the XZ plane of the XR Origin.")]
        Object m_BodyPositionEvaluatorObject;

        IXRBodyPositionEvaluator m_BodyPositionEvaluator;

        /// <summary>
        /// Object supplied to transformations that determines the position of the user's body. If <see langword="null"/>
        /// on <see cref="OnEnable"/>, this will be set to a shared instance of <see cref="UnderCameraBodyPositionEvaluator"/>.
        /// </summary>
        /// <remarks>
        /// Setting this property at runtime also re-links the <see cref="constrainedBodyManipulator"/> to the new evaluator.
        /// </remarks>
        public IXRBodyPositionEvaluator bodyPositionEvaluator
        {
            get => m_BodyPositionEvaluator;
            set
            {
                m_BodyPositionEvaluator = value;
                if (Application.isPlaying)
                    InitializeMovableBody();
            }
        }

        [SerializeField]
        [RequireInterface(typeof(IConstrainedXRBodyManipulator))]
        [Tooltip("Object used to perform movement that is constrained by collision (optional, may be None).")]
        Object m_ConstrainedBodyManipulatorObject;

        IConstrainedXRBodyManipulator m_ConstrainedBodyManipulator;

        /// <summary>
        /// Object supplied to transformations that can be used to perform movement that is constrained by collision
        /// (optional, may be <see langword="null"/>).
        /// </summary>
        /// <remarks>
        /// Setting this property at runtime unlinks the previous manipulator from the body and links the new manipulator
        /// to the body.
        /// </remarks>
        public IConstrainedXRBodyManipulator constrainedBodyManipulator
        {
            get => m_ConstrainedBodyManipulator;
            set
            {
                m_ConstrainedBodyManipulator = value;
                if (m_MovableBody != null)
                {
                    m_MovableBody.UnlinkConstrainedManipulator();
                    if (m_ConstrainedBodyManipulator != null)
                        m_MovableBody.LinkConstrainedManipulator(m_ConstrainedBodyManipulator);
                }
            }
        }

        [SerializeField]
        [Tooltip("When enabled and if a Constrained Manipulator is not already assigned, this behavior will use the XR " +
            "Origin's Character Controller to perform constrained movement, if one exists on the XR Origin's base GameObject.")]
        bool m_UseCharacterControllerIfExists = true;

        /// <summary>
        /// If <see langword="true"/> and if a <see cref="constrainedBodyManipulator"/> is not already assigned, this
        /// behavior will check in <see cref="OnEnable"/> if the <see cref="XROrigin.Origin"/> has a
        /// <see cref="CharacterController"/>. If so, it will set <see cref="constrainedBodyManipulator"/> to a shared
        /// instance of <see cref="CharacterControllerBodyManipulator"/>, so that the Character Controller is used
        /// to perform constrained movement.
        /// </summary>
        public bool useCharacterControllerIfExists
        {
            get => m_UseCharacterControllerIfExists;
            set => m_UseCharacterControllerIfExists = value;
        }

        /// <summary>
        /// Calls the methods in its invocation list every <see cref="Update"/> before transformations are applied.
        /// </summary>
        public event Action<XRBodyTransformer> beforeApplyTransformations;

        bool m_UsingDynamicBodyPositionEvaluator;
        bool m_UsingDynamicConstrainedBodyManipulator;

        XRMovableBody m_MovableBody;

        readonly LinkedList<OrderedTransformation> m_TransformationsQueue = new LinkedList<OrderedTransformation>();

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Reset()
        {
            m_XROrigin = ComponentLocatorUtility<XROrigin>.FindComponent();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
            if (m_XROrigin == null)
            {
                if (!ComponentLocatorUtility<XROrigin>.TryFindComponent(out m_XROrigin))
                {
                    Debug.LogError("XR Body Transformer requires an XR Origin in the scene.", this);
                    enabled = false;
                    return;
                }
            }

            m_BodyPositionEvaluator = m_BodyPositionEvaluatorObject as IXRBodyPositionEvaluator;
            if (m_BodyPositionEvaluator == null)
            {
                m_UsingDynamicBodyPositionEvaluator = true;
                m_BodyPositionEvaluator = ScriptableSingletonCache<UnderCameraBodyPositionEvaluator>.GetInstance(this);
            }

            m_ConstrainedBodyManipulator = m_ConstrainedBodyManipulatorObject as IConstrainedXRBodyManipulator;
            if (m_ConstrainedBodyManipulator == null && m_UseCharacterControllerIfExists)
            {
                if (m_XROrigin.Origin.TryGetComponent<CharacterController>(out _))
                {
                    m_UsingDynamicConstrainedBodyManipulator = true;
                    m_ConstrainedBodyManipulator =
                        ScriptableSingletonCache<CharacterControllerBodyManipulator>.GetInstance(this);
                }
            }

            InitializeMovableBody();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDisable()
        {
            m_MovableBody?.UnlinkConstrainedManipulator();
            m_MovableBody = null;

            if (m_UsingDynamicBodyPositionEvaluator)
            {
                ScriptableSingletonCache<UnderCameraBodyPositionEvaluator>.ReleaseInstance(this);
                m_UsingDynamicBodyPositionEvaluator = false;
            }

            if (m_UsingDynamicConstrainedBodyManipulator)
            {
                ScriptableSingletonCache<CharacterControllerBodyManipulator>.ReleaseInstance(this);
                m_UsingDynamicConstrainedBodyManipulator = false;
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Update()
        {
            beforeApplyTransformations?.Invoke(this);
            while (m_TransformationsQueue.Count > 0)
            {
                m_TransformationsQueue.First.Value.transformation.Apply(m_MovableBody);
                m_TransformationsQueue.RemoveFirst();
            }
        }

        void InitializeMovableBody()
        {
            m_MovableBody = new XRMovableBody(m_XROrigin, m_BodyPositionEvaluator);
            if (m_ConstrainedBodyManipulator != null)
                m_MovableBody.LinkConstrainedManipulator(m_ConstrainedBodyManipulator);
        }

        /// <summary>
        /// Queues a transformation to be applied during the next <see cref="Update"/>. Transformations are applied
        /// sequentially based on ascending <paramref name="priority"/>. Transformations with the same priority are
        /// applied in the order they were queued. Each transformation is removed from the queue after it is applied.
        /// </summary>
        /// <param name="transformation">The transformation that will receive a call to
        /// <see cref="IXRBodyTransformation.Apply"/> in the next <see cref="Update"/>.</param>
        /// <param name="priority">Value that determines when to apply the transformation. Transformations with lower
        /// priority values are applied before those with higher priority values.</param>
        public void QueueTransformation(IXRBodyTransformation transformation, int priority = 0)
        {
            var orderedTransformation = new OrderedTransformation
            {
                transformation = transformation,
                priority = priority,
            };

            var node = m_TransformationsQueue.First;
            if (node == null || node.Value.priority > priority)
            {
                m_TransformationsQueue.AddFirst(orderedTransformation);
                return;
            }

            while (node.Next != null && node.Next.Value.priority <= priority)
            {
                node = node.Next;
            }

            m_TransformationsQueue.AddAfter(node, orderedTransformation);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        internal void OnDrawGizmosSelected()
        {
            if (m_UseCharacterControllerIfExists && m_ConstrainedBodyManipulator != null &&
                m_ConstrainedBodyManipulator is CharacterControllerBodyManipulator characterControllerManipulator &&
                characterControllerManipulator.characterController != null)
            {
                var characterController = characterControllerManipulator.characterController;
                var center = characterController.center + characterController.transform.position + (Vector3.up * ((characterController.stepOffset - characterController.skinWidth) * 0.5f));
                var height = characterController.height + characterController.stepOffset + characterController.skinWidth;
                var radius = characterController.radius + characterController.skinWidth;

                GizmoHelpers.DrawCapsule(center, height, radius, Vector3.up, new Color(1.0f, 0.92f, 0.016f, 0.5f));
            }
        }
    }
}
