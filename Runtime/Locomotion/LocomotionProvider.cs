using System;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion
{
    /// <summary>
    /// Base for a behavior that implements a specific type of user locomotion. This behavior communicates with a
    /// <see cref="LocomotionMediator"/> to gain access to the mediator's <see cref="XRBodyTransformer"/>, which the
    /// provider can use to queue <see cref="IXRBodyTransformation"/>s that move the user.
    /// </summary>
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_LocomotionProviders)]
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
    public abstract partial class LocomotionProvider : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The behavior that this provider communicates with for access to the mediator's XR Body Transformer. " +
            "If one is not provided, this provider will attempt to locate one during its Awake call.")]
        LocomotionMediator m_Mediator;

        /// <summary>
        /// The behavior that this provider communicates with for access to the mediator's <see cref="XRBodyTransformer"/>.
        /// If one is not provided, this provider will attempt to locate one during its <see cref="Awake"/> call.
        /// </summary>
        public LocomotionMediator mediator
        {
            get => m_Mediator;
            set => m_Mediator = value;
        }

        [SerializeField]
        [Tooltip("The queue order of this provider's transformations of the XR Origin. The lower the value, the " +
            "earlier the transformations are applied.")]
        int m_TransformationPriority;

        /// <summary>
        /// The queue order of this provider's transformations of the XR Origin. The lower the value, the earlier the
        /// transformations are applied.
        /// </summary>
        /// <seealso cref="TryQueueTransformation(UnityEngine.XR.Interaction.Toolkit.Locomotion.IXRBodyTransformation)"/>
        public int transformationPriority
        {
            get => m_TransformationPriority;
            set => m_TransformationPriority = value;
        }

        /// <summary>
        /// The current state of locomotion. The <see cref="mediator"/> determines this state based on the provider's
        /// requests for the <see cref="XRBodyTransformer"/>.
        /// </summary>
        /// <seealso cref="TryPrepareLocomotion"/>
        /// <seealso cref="canStartMoving"/>
        /// <seealso cref="TryEndLocomotion"/>
        public LocomotionState locomotionState => m_Mediator != null ? m_Mediator.GetProviderLocomotionState(this) : LocomotionState.Idle;

        /// <summary>
        /// Whether the provider is actively preparing or performing locomotion. This is <see langword="true"/> when
        /// <see cref="locomotionState"/> is <see cref="LocomotionState.Preparing"/> or <see cref="LocomotionState.Moving"/>,
        /// <see langword="false"/> otherwise.
        /// </summary>
        /// <seealso cref="LocomotionStateExtensions.IsActive"/>
        public bool isLocomotionActive => locomotionState.IsActive();

        /// <summary>
        /// Whether the provider has finished preparing for locomotion and is ready to enter the <see cref="LocomotionState.Moving"/> state.
        /// This only applies when <see cref="locomotionState"/> is <see cref="LocomotionState.Preparing"/>, so there is
        /// no need for this implementation to query <see cref="locomotionState"/>.
        /// </summary>
        public virtual bool canStartMoving => true;

        /// <summary>
        /// Calls the methods in its invocation list when the provider has entered the <see cref="LocomotionState.Moving"/> state.
        /// </summary>
        /// <seealso cref="locomotionState"/>
        public event Action<LocomotionProvider> locomotionStarted;

        /// <summary>
        /// Calls the methods in its invocation list when the provider has entered the <see cref="LocomotionState.Ended"/> state.
        /// </summary>
        /// <seealso cref="locomotionState"/>
        public event Action<LocomotionProvider> locomotionEnded;

        /// <summary>
        /// Calls the methods in its invocation list just before the <see cref="XRBodyTransformer"/> applies this
        /// provider's transformation(s). This is invoked at most once per frame while <see cref="locomotionState"/> is
        /// <see cref="LocomotionState.Moving"/>, and only if the provider has queued at least one transformation.
        /// </summary>
        /// <remarks>This is invoked before the <see cref="XRBodyTransformer"/> applies the transformations from other
        /// providers as well.</remarks>
        public event Action<LocomotionProvider> beforeStepLocomotion;

        XRBodyTransformer m_ActiveBodyTransformer;
        bool m_AnyTransformationsThisFrame;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Awake()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            if (m_System == null)
            {
                m_System = GetComponentInParent<LocomotionSystem>();
                if (m_System == null)
                {
                    ComponentLocatorUtility<LocomotionSystem>.TryFindComponent(out m_System);
                }
            }

            if (m_Mediator == null)
            {
                m_Mediator = GetComponentInParent<LocomotionMediator>();
                if (m_Mediator == null)
                {
                    ComponentLocatorUtility<LocomotionMediator>.TryFindComponent(out m_Mediator);
                }
            }

            if (m_Mediator == null && m_System == null)
            {
                Debug.LogError("Locomotion Provider requires a Locomotion Mediator or Locomotion System (legacy) in the scene.", this);
                enabled = false;
            }
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Attempts to transition this provider into the <see cref="LocomotionState.Preparing"/> state. This succeeds
        /// if <see cref="LocomotionProvider.isLocomotionActive"/> was <see langword="false"/> when this was called.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if <see cref="LocomotionProvider.isLocomotionActive"/> was
        /// <see langword="false"/> when this was called, <see langword="false"/> otherwise.</returns>
        /// <remarks>
        /// If this succeeds, then the provider can enter the <see cref="LocomotionState.Moving"/> state either by calling
        /// <see cref="TryStartLocomotionImmediately"/> or by waiting for the <see cref="mediator"/>'s next
        /// <see cref="LocomotionMediator.Update"/> in which the provider's <see cref="LocomotionProvider.canStartMoving"/>
        /// is <see langword="true"/>. When the provider enters the <see cref="LocomotionState.Moving"/> state, it will
        /// invoke <see cref="locomotionStarted"/> and gain access to the <see cref="XRBodyTransformer"/>.
        /// </remarks>
        protected bool TryPrepareLocomotion() { return m_Mediator != null && m_Mediator.TryPrepareLocomotion(this); }

        /// <summary>
        /// Attempts to transition this provider into the <see cref="LocomotionState.Moving"/> state. This succeeds if
        /// <see cref="LocomotionProvider.locomotionState"/> was not already <see cref="LocomotionState.Moving"/> when
        /// this was called.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if <see cref="LocomotionProvider.locomotionState"/> was not already
        /// <see cref="LocomotionState.Moving"/> when this was called, <see langword="false"/> otherwise.</returns>
        /// <remarks>
        /// This method bypasses the check for <see cref="LocomotionProvider.canStartMoving"/>.
        /// After this provider enters the <see cref="LocomotionState.Moving"/> state, it will invoke
        /// <see cref="locomotionStarted"/> and gain access to the <see cref="XRBodyTransformer"/>.
        /// </remarks>
        protected bool TryStartLocomotionImmediately() { return m_Mediator != null && m_Mediator.TryStartLocomotion(this); }

        /// <summary>
        /// Attempts to transition this provider into the <see cref="LocomotionState.Ended"/> state. This succeeds if
        /// <see cref="LocomotionProvider.isLocomotionActive"/> was <see langword="true"/> when this was called.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if <see cref="LocomotionProvider.isLocomotionActive"/> was
        /// <see langword="true"/> when this was called, <see langword="false"/> otherwise.</returns>
        /// <remarks>
        /// After this provider enters the <see cref="LocomotionState.Ended"/> state, it will invoke
        /// <see cref="locomotionEnded"/> and lose access to the <see cref="XRBodyTransformer"/>. Then during the
        /// <see cref="mediator"/>'s <see cref="LocomotionMediator.Update"/> in the next frame, the provider will enter
        /// the <see cref="LocomotionState.Idle"/> state, unless it has called <see cref="TryPrepareLocomotion"/> or
        /// <see cref="TryStartLocomotionImmediately"/> again.
        /// </remarks>
        protected bool TryEndLocomotion() { return m_Mediator != null && m_Mediator.TryEndLocomotion(this); }

        internal void OnLocomotionStart(XRBodyTransformer transformer)
        {
            m_ActiveBodyTransformer = transformer;
            m_ActiveBodyTransformer.beforeApplyTransformations += OnBeforeTransformationsApplied;
            m_AnyTransformationsThisFrame = false;
            OnLocomotionStarting();
            locomotionStarted?.Invoke(this);
        }

        /// <summary>
        /// Called when locomotion enters the <see cref="LocomotionState.Moving"/> state, after the provider gains
        /// access to the <see cref="XRBodyTransformer"/> and before it invokes <see cref="locomotionStarted"/>.
        /// </summary>
        protected virtual void OnLocomotionStarting() { }

        internal void OnLocomotionEnd()
        {
            locomotionEnded?.Invoke(this);
            OnLocomotionEnding();

            if (m_ActiveBodyTransformer != null)
                m_ActiveBodyTransformer.beforeApplyTransformations -= OnBeforeTransformationsApplied;
            m_ActiveBodyTransformer = null;
        }

        /// <summary>
        /// Called when locomotion enters the <see cref="LocomotionState.Ended"/> state, after the provider invokes
        /// <see cref="locomotionEnded"/> and before it loses access to the <see cref="XRBodyTransformer"/>.
        /// </summary>
        protected virtual void OnLocomotionEnding() { }

        /// <summary>
        /// Attempts to queue a transformation to be applied during the active <see cref="XRBodyTransformer"/>'s next
        /// <see cref="XRBodyTransformer.Update"/>. The provider's <see cref="transformationPriority"/> determines when
        /// the transformation is applied in relation to others. The queue attempt only succeeds if the provider is in
        /// the <see cref="LocomotionState.Moving"/> state.
        /// </summary>
        /// <param name="bodyTransformation">The transformation that will receive a call to
        /// <see cref="IXRBodyTransformation.Apply"/> in the next <see cref="XRBodyTransformer.Update"/>.</param>
        /// <returns>Returns <see langword="true"/> if the provider has access to the <see cref="XRBodyTransformer"/>,
        /// <see langword="false"/> otherwise.</returns>
        /// <remarks>This should only be called when <see cref="locomotionState"/> is <see cref="LocomotionState.Moving"/>,
        /// otherwise this method will do nothing and return <see langword="false"/>.</remarks>
        /// <seealso cref="TryQueueTransformation(UnityEngine.XR.Interaction.Toolkit.Locomotion.IXRBodyTransformation, int)"/>
        protected bool TryQueueTransformation(IXRBodyTransformation bodyTransformation)
        {
            if (!CanQueueTransformation())
                return false;

            m_ActiveBodyTransformer.QueueTransformation(bodyTransformation, m_TransformationPriority);
            m_AnyTransformationsThisFrame = true;
            return true;
        }

        /// <summary>
        /// Attempts to queue a transformation to be applied during the active <see cref="XRBodyTransformer"/>'s next
        /// <see cref="XRBodyTransformer.Update"/>. The given <paramref name="priority"/> determines when the
        /// transformation is applied in relation to others. The queue attempt only succeeds if the provider is in the
        /// <see cref="LocomotionState.Moving"/> state.
        /// </summary>
        /// <param name="bodyTransformation">The transformation that will receive a call to
        /// <see cref="IXRBodyTransformation.Apply"/> in the next <see cref="XRBodyTransformer.Update"/>.</param>
        /// <param name="priority">Value that determines when to apply the transformation. Transformations with lower
        /// priority values are applied before those with higher priority values.</param>
        /// <returns>Returns <see langword="true"/> if the provider has access to the <see cref="XRBodyTransformer"/>,
        /// <see langword="false"/> otherwise.</returns>
        /// <remarks>This should only be called when <see cref="locomotionState"/> is <see cref="LocomotionState.Moving"/>,
        /// otherwise this method will do nothing and return <see langword="false"/>.</remarks>
        /// <seealso cref="TryQueueTransformation(UnityEngine.XR.Interaction.Toolkit.Locomotion.IXRBodyTransformation)"/>
        protected bool TryQueueTransformation(IXRBodyTransformation bodyTransformation, int priority)
        {
            if (!CanQueueTransformation())
                return false;

            m_ActiveBodyTransformer.QueueTransformation(bodyTransformation, priority);
            m_AnyTransformationsThisFrame = true;
            return true;
        }

        bool CanQueueTransformation()
        {
            if (m_ActiveBodyTransformer == null)
            {
                if (locomotionState == LocomotionState.Moving)
                {
                    Debug.LogError("Cannot queue transformation because reference to active XR Body Transformer " +
                        "is null, even though Locomotion Provider is in Moving state. This should not happen.", this);
                }

                return false;
            }

            return true;
        }

        void OnBeforeTransformationsApplied(XRBodyTransformer bodyTransformer)
        {
            if (m_AnyTransformationsThisFrame)
                beforeStepLocomotion?.Invoke(this);

            m_AnyTransformationsThisFrame = false;
        }
    }
}
