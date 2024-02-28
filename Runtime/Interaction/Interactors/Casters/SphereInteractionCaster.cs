using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors.Casters
{
    /// <summary>
    /// Implementation of <see cref="InteractionCasterBase"/> that performs a sphere cast with a set radius to find valid targets.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("XR/Interactors/Sphere Interaction Caster", 22)]
    [HelpURL(XRHelpURLConstants.k_SphereInteractionCaster)]
    public class SphereInteractionCaster : InteractionCasterBase
    {
        const int k_MaxRaycastHits = 10;
        readonly RaycastHit[] m_OverlapSphereHits = new RaycastHit[k_MaxRaycastHits];
        readonly Collider[] m_OverlapSphereColliderHits = new Collider[k_MaxRaycastHits];

        [Header("Filtering Settings")]
        [SerializeField]
        [Tooltip("Layer mask used for limiting sphere cast and sphere overlap targets.")]
        LayerMask m_PhysicsLayerMask = -1;

        /// <summary>
        /// Gets or sets layer mask used for limiting sphere cast and sphere overlap targets.
        /// </summary>
        public LayerMask physicsLayerMask
        {
            get => m_PhysicsLayerMask;
            set => m_PhysicsLayerMask = value;
        }

        [SerializeField]
        QueryTriggerInteraction m_PhysicsTriggerInteraction = QueryTriggerInteraction.Ignore;

        /// <summary>
        /// Determines whether the cast sphere overlap will hit triggers.
        /// </summary>
        public QueryTriggerInteraction physicsTriggerInteraction
        {
            get => m_PhysicsTriggerInteraction;
            set => m_PhysicsTriggerInteraction = value;
        }

        [Header("Sphere Casting Settings")]
        [SerializeField]
        [Tooltip("Radius of the sphere cast.")]
        float m_CastRadius = 0.1f;
        
        /// <summary>
        /// Radius of the sphere cast.
        /// </summary>
        public float castRadius
        {
            get => m_CastRadius;
            set => m_CastRadius = value;
        }

        bool m_FirstFrame = true;
        Vector3 m_LastSphereCastOrigin = Vector3.zero;
        PhysicsScene m_LocalPhysicsScene;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
            m_FirstFrame = true;
            m_LastSphereCastOrigin = Vector3.zero;
        }

        // ReSharper disable once Unity.RedundantEventFunction -- For consistent method override signature in derived classes
        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDisable()
        {
        }

        /// <inheritdoc />
        public override bool TryGetColliderTargets(XRInteractionManager interactionManager, List<Collider> targets)
        {
            if (!base.TryGetColliderTargets(interactionManager, targets))
                return false;
            
            Vector3 interactorPosition = effectiveCastOrigin.position;
            Vector3 overlapStart = m_LastSphereCastOrigin;
            Vector3 interFrameEnd = interactorPosition;
            float grabRadius = m_CastRadius * transform.lossyScale.x;
            bool hasHit;

            BurstPhysicsUtils.GetSphereOverlapParameters(overlapStart, interFrameEnd, out var normalizedOverlapVector, out var overlapSqrMagnitude, out var overlapDistance);

            // If no movement is recorded.
            // Check if sphere cast size is sufficient for proper cast, or if first frame since last frame poke position will be invalid.
            if (m_FirstFrame || overlapSqrMagnitude < 0.001f)
            {
                var numberOfOverlaps = m_LocalPhysicsScene.OverlapSphere(interFrameEnd, grabRadius, m_OverlapSphereColliderHits,
                    m_PhysicsLayerMask, m_PhysicsTriggerInteraction);

                for (var i = 0; i < numberOfOverlaps; ++i)
                    targets.Add(m_OverlapSphereColliderHits[i]);

                hasHit = numberOfOverlaps > 0;
            }
            else
            {
                var numberOfOverlaps = m_LocalPhysicsScene.SphereCast(
                    overlapStart,
                    grabRadius,
                    normalizedOverlapVector,
                    m_OverlapSphereHits,
                    overlapDistance,
                    m_PhysicsLayerMask,
                    m_PhysicsTriggerInteraction);

                for (var i = 0; i < numberOfOverlaps; ++i)
                    targets.Add(m_OverlapSphereHits[i].collider);
                
                hasHit = numberOfOverlaps > 0;
            }

            m_LastSphereCastOrigin = interactorPosition;
            m_FirstFrame = false;
            return hasHit;
        }

        /// <inheritdoc />
        protected override bool InitializeCaster()
        {
            if (!isInitialized)
            {
                m_LocalPhysicsScene = gameObject.scene.GetPhysicsScene();
                isInitialized = true;
            }

            return isInitialized;
        }
    }
}