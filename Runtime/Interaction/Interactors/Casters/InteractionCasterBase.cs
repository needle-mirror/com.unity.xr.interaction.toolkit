using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors.Casters
{
    /// <summary>
    /// Provides an abstract base for interaction casters used by <see cref="NearFarInteractor"/>.
    /// This class serves as the foundation for casting interactions, managing initialization,
    /// and providing the fundamental mechanics for casting. It implements the <see cref="IInteractionCaster"/> interface.
    /// </summary>
    /// <remarks>
    /// The class maintains a state to track whether it has been initialized and allows for setting
    /// and getting the origin of casting. It also requires implementation of an abstract method
    /// to try and get collider targets based on an <see cref="XRInteractionManager"/>.
    /// </remarks>
    public abstract class InteractionCasterBase : MonoBehaviour, IInteractionCaster
    {
        /// <inheritdoc />
        public bool isInitialized { get; protected set; }

        [SerializeField]
        [Tooltip("Source of origin and direction used when updating sample points.")]
        Transform m_CastOrigin;

        /// <inheritdoc />
        public Transform castOrigin
        {
            get => m_CastOrigin;
            set => m_CastOrigin = value;
        }

        /// <inheritdoc />
        public Transform effectiveCastOrigin => !m_EnableStabilization || !m_InitializedStabilizationOrigin ? castOrigin : m_StabilizationAnchor;

        [Header("Stabilization Parameters")]
        [SerializeField]
        [Tooltip("Determines whether to stabilize the cast origin.")]
        bool m_EnableStabilization;

        /// <summary>
        /// Determines whether to stabilize the cast origin.
        /// </summary>
        public bool enableStabilization
        {
            get => m_EnableStabilization;
            set => m_EnableStabilization = value;
        }

        [SerializeField]
        [Tooltip("Factor for stabilizing position. Larger values increase the range of stabilization, making the effect more pronounced over a greater distance.")]
        float m_PositionStabilization = 0.25f;

        /// <summary>
        /// Factor for stabilizing position. This value represents the maximum distance (in meters) over which position stabilization will be applied. Larger values increase the range of stabilization, making the effect more pronounced over a greater distance.
        /// </summary>
        public float positionStabilization
        {
            get => m_PositionStabilization;
            set => m_PositionStabilization = value;
        }

        [SerializeField]
        [Tooltip("Factor for stabilizing angle. Larger values increase the range of stabilization, making the effect more pronounced over a greater angle.")]
        float m_AngleStabilization = 20f;

        /// <summary>
        /// Factor for stabilizing angle. This value represents the maximum angle (in degrees) over which angle stabilization will be applied. Larger values increase the range of stabilization, making the effect more pronounced over a greater angle.
        /// </summary>
        public float angleStabilization
        {
            get => m_AngleStabilization;
            set => m_AngleStabilization = value;
        }

        [SerializeField]
        [RequireInterface(typeof(IXRRayProvider))]
        [Tooltip("Optional ray provider for calculating stable rotation.")]
        Object m_AimTargetObject;

        /// <summary>
        /// Gets or sets the aim target ray provider for calculating stable rotation.
        /// </summary>
        public IXRRayProvider aimTarget
        {
            get => m_AimTargetObjectRef.Get(m_AimTargetObject);
            set => m_AimTargetObjectRef.Set(ref m_AimTargetObject, value);
        }

        readonly UnityObjectReferenceCache<IXRRayProvider, Object> m_AimTargetObjectRef = new UnityObjectReferenceCache<IXRRayProvider, Object>();

        bool m_InitializedStabilizationOrigin;
        Transform m_StabilizationAnchor;
        float m_LastStabilizationUpdateTime;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnValidate()
        {
            // If no cast origin is set, default to this transform.
            if (m_CastOrigin == null)
                m_CastOrigin = transform;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Awake()
        {
            // If no cast origin is set, default to this transform.
            if (m_CastOrigin == null)
                m_CastOrigin = transform;

            InitializeCaster();
            InitializeStabilization();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDestroy() => isInitialized = false;

        /// <inheritdoc />
        public virtual bool TryGetColliderTargets(XRInteractionManager interactionManager, List<Collider> targets)
        {
            targets.Clear();
            if (!InitializeCaster() && !InitializeStabilization())
                return false;
            UpdateInternalData();
            return true;
        }

        /// <summary>
        /// Tries to initialize the caster.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if successful or already initialized.</returns>
        protected abstract bool InitializeCaster();

        /// <summary>
        /// Updates internal state for the caster.
        /// </summary>
        protected virtual void UpdateInternalData()
        {
            if (!m_EnableStabilization)
                return;

            float deltaTime = Time.unscaledTime - m_LastStabilizationUpdateTime;
            m_LastStabilizationUpdateTime = Time.unscaledTime;

            XRTransformStabilizer.ApplyStabilization(ref m_StabilizationAnchor, m_CastOrigin, aimTarget, m_PositionStabilization, m_AngleStabilization, deltaTime);
        }

        /// <summary>
        /// Creates a stabilization anchor if one does not already exist.
        /// </summary>
        /// <returns>True if anchor exists or stabilization is disabled</returns>
        protected virtual bool InitializeStabilization()
        {
            if (!m_EnableStabilization || m_InitializedStabilizationOrigin)
                return true;

            if (m_StabilizationAnchor == null)
            {
                if (!ComponentLocatorUtility<XROrigin>.TryFindComponent(out var xrOrigin))
                {
                    Debug.LogError($"Failed to find XROrigin component in scene. Cannot stabilize cast origin for {GetType().Name}.", this);
                    m_EnableStabilization = false;
                    return false;
                }

                // Capture hand name
                string handName = "";
                if (TryGetComponent(out IXRInteractor interactor))
                    handName = interactor.handedness.ToString();

                m_StabilizationAnchor = new GameObject($"[{handName} {GetType().Name}] Stabilization Cast Origin").transform;
                m_StabilizationAnchor.SetParent(xrOrigin.Origin.transform, false);
                m_StabilizationAnchor.SetLocalPose(Pose.identity);
                m_InitializedStabilizationOrigin = true;
            }
            return m_InitializedStabilizationOrigin;
        }
    }
}
