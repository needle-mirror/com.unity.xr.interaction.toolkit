using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors.Casters
{
    /// <summary>
    /// A specialized interaction caster that extends <see cref="InteractionCasterBase"/> to support curved ray casting. 
    /// This class implements the <see cref="IUIModelUpdater"/> interface to facilitate world-space UI interactions based on the generated sample points.
    /// It handles complex ray casting scenarios using curves, allowing for a more flexible interaction approach.
    /// </summary>
    /// <remarks>
    /// The caster supports different types of hit detection, including sphere and cone casting, alongside the standard ray casting.
    /// It allows customization of parameters like the number of curve segments, casting distance, and the size of the casting radius.
    /// </remarks>
    [DisallowMultipleComponent]
    [AddComponentMenu("XR/Interactors/Curve Interaction Caster", 22)]
    [HelpURL(XRHelpURLConstants.k_CurveInteractionCaster)]
    public class CurveInteractionCaster : InteractionCasterBase, ICurveInteractionCaster, IUIModelUpdater
    {
        /// <summary>
        /// Sets which shape of physics cast to use for the cast when detecting collisions.
        /// </summary>
        /// <seealso cref="hitDetectionType"/>
        public enum HitDetectionType
        {
            /// <summary>
            /// Uses <see cref="Physics"/> Ray cast to detect collisions.
            /// </summary>
            Raycast,

            /// <summary>
            /// Uses <see cref="Physics"/> Sphere Cast to detect collisions.
            /// </summary>
            SphereCast,

            /// <summary>
            /// Uses cone casting to detect collisions.
            /// </summary>
            ConeCast,
        }

        /// <summary>
        /// Sets whether ray cast queries hit Trigger colliders and include or ignore snap volume trigger colliders.
        /// </summary>
        /// <seealso cref="raycastSnapVolumeInteraction"/>
        public enum QuerySnapVolumeInteraction
        {
            /// <summary>
            /// Queries never report Trigger hits that are registered with a snap volume.
            /// </summary>
            Ignore,

            /// <summary>
            /// Queries always report Trigger hits that are registered with a snap volume.
            /// </summary>
            Collide,
        }
        
        const int k_MaxRaycastHits = 10;
        const int k_MinNumCurveSegments = 1;
        const int k_MaxNumCurveSegments = 100;

        NativeArray<Vector3> m_SamplePoints;

        /// <inheritdoc />
        public NativeArray<Vector3> samplePoints
        {
            get => m_SamplePoints;
            protected set => m_SamplePoints = value;
        }

        /// <inheritdoc />
        public Vector3 lastSamplePoint
        {
            get
            {
                if (isInitialized && m_SamplePoints.Length == targetNumCurveSegments + 1)
                    return m_SamplePoints[targetNumCurveSegments];
                return castOrigin.position;
            }
        }
        
        [Header("Filtering Settings")]
        [SerializeField]
        [Tooltip("Gets or sets layer mask used for limiting ray cast targets.")]
        LayerMask m_RaycastMask = -1;

        /// <summary>
        /// Gets or sets layer mask used for limiting ray cast targets.
        /// </summary>
        public LayerMask raycastMask
        {
            get => m_RaycastMask;
            set => m_RaycastMask = value;
        }

        [SerializeField]
        [Tooltip("Gets or sets type of interaction with trigger colliders via ray cast.")]
        QueryTriggerInteraction m_RaycastTriggerInteraction = QueryTriggerInteraction.Ignore;

        /// <summary>
        /// Gets or sets type of interaction with trigger colliders via ray cast.
        /// </summary>
        public QueryTriggerInteraction raycastTriggerInteraction
        {
            get => m_RaycastTriggerInteraction;
            set => m_RaycastTriggerInteraction = value;
        }

        [SerializeField]
        [Tooltip("Determines if ray casts include snap volume triggers: 'Collide' to include, 'Ignore' for performance optimization when not using specific XR components.")]
        QuerySnapVolumeInteraction m_RaycastSnapVolumeInteraction = QuerySnapVolumeInteraction.Collide;

        /// <summary>
        /// Whether ray cast should include or ignore hits on trigger colliders that are snap volume colliders,
        /// even if the ray cast is set to ignore triggers.
        /// If you are not using gaze assistance or XR Interactable Snap Volume components, you should set this property
        /// to <see cref="QuerySnapVolumeInteraction.Ignore"/> to avoid the performance cost.
        /// </summary>
        /// <remarks>
        /// When set to <see cref="QuerySnapVolumeInteraction.Collide"/> when <see cref="raycastTriggerInteraction"/> is set to ignore trigger colliders
        /// (when set to <see cref="QueryTriggerInteraction.Ignore"/> or when set to <see cref="QueryTriggerInteraction.UseGlobal"/>
        /// while <see cref="Physics.queriesHitTriggers"/> is <see langword="false"/>),
        /// the ray cast query will be modified to include trigger colliders, but then this behavior will ignore any trigger collider
        /// hits that are not snap volumes.
        /// <br />
        /// When set to <see cref="XRRayInteractor.QuerySnapVolumeInteraction.Ignore"/> when <see cref="raycastTriggerInteraction"/> is set to hit trigger colliders
        /// (when set to <see cref="QueryTriggerInteraction.Collide"/> or when set to <see cref="QueryTriggerInteraction.UseGlobal"/>
        /// while <see cref="Physics.queriesHitTriggers"/> is <see langword="true"/>),
        /// this behavior will ignore any trigger collider hits that are snap volumes.
        /// </remarks>
        /// <seealso cref="raycastTriggerInteraction"/>
        /// <seealso cref="XRInteractableSnapVolume.snapCollider"/>
        public QuerySnapVolumeInteraction raycastSnapVolumeInteraction
        {
            get => m_RaycastSnapVolumeInteraction;
            set => m_RaycastSnapVolumeInteraction = value;
        }
        
        [Header("Curve Casting settings")]
        
        [SerializeField]
        [Range(k_MinNumCurveSegments, k_MaxNumCurveSegments)]
        int m_TargetNumCurveSegments = 1;

        /// <summary>
        /// Gets or sets the number of segments to sample along the curve. This also determines the length of the <see cref="samplePoints"/> array.
        /// </summary>
        public int targetNumCurveSegments
        {
            get => m_TargetNumCurveSegments;
            set
            {
                m_TargetNumCurveSegments = Mathf.Clamp(value, k_MinNumCurveSegments, k_MaxNumCurveSegments);
                isInitialized = false;
            }
        }
        
        [SerializeField]
        HitDetectionType m_HitDetectionType = HitDetectionType.ConeCast;

        /// <summary>
        /// Gets or sets which type of hit detection to use for the ray cast.
        /// </summary>
        public HitDetectionType hitDetectionType
        {
            get => m_HitDetectionType;
            set => m_HitDetectionType = value;
        }

        [SerializeField]
        float m_CastDistance = 10f;

        /// <summary>
        /// Maximum distance for all physics casts.
        /// </summary>
        public float castDistance
        {
            get => m_CastDistance;
            set => m_CastDistance = value;
        }

        [SerializeField]
        [Range(0.01f, 0.25f)]
        float m_SphereCastRadius = 0.1f;

        /// <summary>
        /// Gets or sets radius used for sphere casting.
        /// </summary>
        /// <seealso cref="HitDetectionType.SphereCast"/>
        /// <seealso cref="hitDetectionType"/>
        public float sphereCastRadius
        {
            get => m_SphereCastRadius;
            set => m_SphereCastRadius = Mathf.Clamp(value, 0.01f, 0.25f);
        }

        [SerializeField]
        float m_ConeCastAngle = 3f;

        /// <summary>
        /// Gets or sets the angle in degrees of the cone used for cone casting. Will use regular ray casting if set to 0.
        /// </summary>
        public float coneCastAngle
        {
            get => m_ConeCastAngle;
            set => m_ConeCastAngle = value;
        }

        /// <summary>
        /// Indicates whether OnDestroy has been called on this object.
        /// </summary>
        protected bool isDestroyed { get; private set; } = false;

        PhysicsScene m_LocalPhysicsScene;

        int m_RaycastHitsCount;
        readonly RaycastHit[] m_RaycastHits = new RaycastHit[k_MaxRaycastHits];
        readonly RaycastHitComparer m_RaycastHitComparer = new RaycastHitComparer();

        /// <summary>
        /// Reusable list of raycast hits, used to avoid allocations during sphere casting.
        /// </summary>
        static readonly RaycastHit[] s_SpherecastScratch = new RaycastHit[k_MaxRaycastHits];

        /// <summary>
        /// Reusable list of optimal raycast hits, for lookup during sphere casting.
        /// </summary>
        static readonly HashSet<Collider> s_OptimalHits = new HashSet<Collider>();

        // ReSharper disable once Unity.RedundantEventFunction -- OnEnable is required to ensure that the checkbox appears in the inspector
        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
        }

        // ReSharper disable once Unity.RedundantEventFunction -- For consistent method override signature in derived classes
        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDisable()
        {
        }

        /// <inheritdoc />
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (samplePoints.IsCreated)
                samplePoints.Dispose();
            isDestroyed = true;
        }

        /// <inheritdoc />
        protected override bool InitializeCaster()
        {
            if (!isDestroyed && !isInitialized)
            {
                if (samplePoints.IsCreated)
                    samplePoints.Dispose();
                m_SamplePoints = new NativeArray<Vector3>(targetNumCurveSegments + 1, Allocator.Persistent);

                m_LocalPhysicsScene = gameObject.scene.GetPhysicsScene();
                isInitialized = true;
            }

            return isInitialized;
        }

        /// <inheritdoc />
        public override bool TryGetColliderTargets(XRInteractionManager interactionManager, List<Collider> targets)
        {
            if (!base.TryGetColliderTargets(interactionManager, targets))
                return false;

            if (UpdatePhysicscastHits(interactionManager))
            {
                for (int i = 0; i < m_RaycastHitsCount; i++)
                    targets.Add(m_RaycastHits[i].collider);
                return true;
            }
            return false;
        }
        
        /// <inheritdoc />
        public bool TryGetColliderTargets(XRInteractionManager interactionManager, List<Collider> targets, List<RaycastHit> raycastHits)
        {
            raycastHits.Clear();
            if (!base.TryGetColliderTargets(interactionManager, targets))
                return false;

            if (UpdatePhysicscastHits(interactionManager))
            {
                for (int i = 0; i < m_RaycastHitsCount; i++)
                {
                    raycastHits.Add(m_RaycastHits[i]);
                    targets.Add(m_RaycastHits[i].collider);
                }
                return true;
            }
            return false;
        }
        
        /// <inheritdoc />
        protected override void UpdateInternalData()
        {
            base.UpdateInternalData();
            UpdateSamplePoints();
        }

        /// <summary>
        /// Updates sample points for casting, based on the current, potentially stabilized cast origin and direction.
        /// </summary>
        protected virtual void UpdateSamplePoints()
        {
            if (!isInitialized)
                return;
            var sampleOrigin = effectiveCastOrigin;
            UpdateSamplePoints(sampleOrigin.position, sampleOrigin.forward, castDistance, m_SamplePoints);
        }

        /// <summary>
        /// Updates a set of sample points for casting, given the origin, direction, total distance, and an array of points.
        /// Base implementation does a simple linear interpolation between the origin and the end point.
        /// </summary>
        /// <param name="origin">The starting point of the cast.</param>
        /// <param name="direction">The direction in which the cast is performed.</param>
        /// <param name="totalDistance">The total distance of the cast.</param>
        /// <param name="points">The array of points to be updated for the cast.</param>
        protected virtual void UpdateSamplePoints(in Vector3 origin, in Vector3 direction, float totalDistance, NativeArray<Vector3> points)
        {
            int numPoints = points.Length;
            if (numPoints < 2)
            {
                return;
            }

            if (numPoints == 2)
            {
                points[0] = origin;
                points[1] = origin + direction * totalDistance;
                return;
            }

            float distancePercentBetweenPoints = totalDistance / numPoints;
            Vector3 scaledDirection = direction * distancePercentBetweenPoints;

            points[0] = origin;
            for (int i = 1; i < numPoints; i++)
            {
                points[i] = points[i - 1] + scaledDirection;
            }
        }

        /// <summary>
        /// Performs physics casting to update hits based on the current sample points. Returns a boolean indicating whether any hits were recorded.
        /// </summary>
        /// <param name="interactionManager">The XR interaction manager used to help filter colliders</param>
        /// <returns>True if any raycast hits were found; otherwise, false.</returns>
        protected virtual bool UpdatePhysicscastHits(in XRInteractionManager interactionManager)
        {
            m_RaycastHitsCount = 0;

            for (var i = 1; i < samplePoints.Length; ++i)
            {
                float3 origin = samplePoints[0];
                float3 fromPoint = samplePoints[i - 1];
                float3 toPoint = samplePoints[i];

                m_RaycastHitsCount = CheckCollidersBetweenPoints(interactionManager, fromPoint, toPoint, origin, m_RaycastHits);
                if (m_RaycastHitsCount > 0)
                    break;
            }

            return m_RaycastHitsCount > 0;
        }

        /// <summary>
        /// Checks for colliders between specified points using a raycast, sphere cast, or cone cast based on the current configuration. Returns the count of hits detected.
        /// </summary>
        /// <param name="interactionManager">The XR interaction manager used to help filter colliders</param>
        /// <param name="from">The starting point of the check.</param>
        /// <param name="to">The ending point of the check.</param>
        /// <param name="origin">The origin point for the casting.</param>
        /// <param name="raycastHits">Array to store the results of the raycast hits.</param>
        /// <returns>The number of colliders detected between the given points.</returns>
        protected virtual int CheckCollidersBetweenPoints(in XRInteractionManager interactionManager, Vector3 from, Vector3 to, Vector3 origin, RaycastHit[] raycastHits)
        {
            int raycastHitCount = 0;

            float3 fromToVector = to - from;
            var distanceBetweenPoints = math.length(fromToVector);
            var direction = math.normalize(fromToVector);

            var queryTriggerInteraction = m_RaycastSnapVolumeInteraction == QuerySnapVolumeInteraction.Collide
                ? QueryTriggerInteraction.Collide
                : m_RaycastTriggerInteraction;

            switch (m_HitDetectionType)
            {
                case HitDetectionType.Raycast:
                    raycastHitCount = m_LocalPhysicsScene.Raycast(from, direction,
                        raycastHits, distanceBetweenPoints, m_RaycastMask, queryTriggerInteraction);
                    break;

                case HitDetectionType.SphereCast:
                    raycastHitCount = m_LocalPhysicsScene.SphereCast(from, m_SphereCastRadius, direction,
                        raycastHits, distanceBetweenPoints, m_RaycastMask, queryTriggerInteraction);
                    break;

                case HitDetectionType.ConeCast:
                    raycastHitCount = FilteredConecast(interactionManager, from, m_ConeCastAngle, direction, origin,
                        raycastHits, distanceBetweenPoints, m_RaycastMask, queryTriggerInteraction);
                    break;
            }

            if (raycastHitCount > 0)
            {
                var baseQueryHitsTriggers = m_RaycastTriggerInteraction == QueryTriggerInteraction.Collide ||
                                            (m_RaycastTriggerInteraction == QueryTriggerInteraction.UseGlobal && Physics.queriesHitTriggers);

                if (m_RaycastSnapVolumeInteraction == QuerySnapVolumeInteraction.Ignore && baseQueryHitsTriggers)
                {
                    // Filter out Snap Volume trigger collider hits
                    raycastHitCount = FilterOutSnapTriggerColliders(interactionManager, raycastHits, raycastHitCount);
                }
                else if (m_RaycastSnapVolumeInteraction == QuerySnapVolumeInteraction.Collide && !baseQueryHitsTriggers)
                {
                    // Filter out trigger collider hits that are not Snap Volume snap colliders
                    raycastHitCount = FilterOutNonSnapTriggerColliders(interactionManager, raycastHits, raycastHitCount);
                }

                // Sort all the hits by distance along the curve since the results of the 3D ray cast are not ordered.
                // Sorting is done after filtering above for performance.
                SortingHelpers.Sort(raycastHits, m_RaycastHitComparer, raycastHitCount);
            }

            return raycastHitCount;
        }

        int FilteredConecast(XRInteractionManager interactionManager, in Vector3 from, float coneCastAngleDegrees, in Vector3 direction, in Vector3 origin, RaycastHit[] results, float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            s_OptimalHits.Clear();

            // Set up all the sphere casts
            var obstructionDistance = math.min(maxDistance, 1000f);

            // Raycast looking for obstructions and any optimal targets
            var hitCounter = 0;
            var optimalHits = m_LocalPhysicsScene.Raycast(origin, direction, s_SpherecastScratch, obstructionDistance, layerMask, queryTriggerInteraction);
            if (optimalHits > 0)
            {
                for (var i = 0; i < optimalHits; ++i)
                {
                    var hitInfo = s_SpherecastScratch[i];
                    if (hitInfo.distance > obstructionDistance)
                        continue;

                    // If an obstruction is found, then reject anything behind it
                    if (!interactionManager.IsColliderRegisteredToInteractable(hitInfo.collider))
                    {
                        obstructionDistance = math.min(hitInfo.distance, obstructionDistance);

                        // Since we are rejecting anything past the obstruction, we push its distance back to allow for objects in the periphery to be selected first
                        hitInfo.distance += 1.5f;
                    }

                    results[hitCounter] = hitInfo;
                    s_OptimalHits.Add(hitInfo.collider);
                    hitCounter++;
                }
            }

            // Now do a series of sphere casts that increase in size.
            // We don't process obstructions here
            // We don't do ultra-fine cone rejection instead add horizontal distance to the spherecast depth
            var angleRadius = math.tan(math.radians(coneCastAngleDegrees) * 0.5f);
            var currentOffset = (origin - from).magnitude;
            while (currentOffset < obstructionDistance)
            {
                BurstPhysicsUtils.GetConecastParameters(angleRadius, currentOffset, obstructionDistance, direction, out var originOffset, out var endRadius, out var castMax);

                // Spherecast
                var initialResults = m_LocalPhysicsScene.SphereCast(origin + originOffset, endRadius, direction, s_SpherecastScratch, castMax, layerMask, queryTriggerInteraction);

                // Go through each result
                for (var i = 0; (i < initialResults && hitCounter < results.Length); i++)
                {
                    var hit = s_SpherecastScratch[i];

                    // Range check
                    if (hit.distance > obstructionDistance)
                        continue;

                    // If it's an optimal hit, then skip it
                    if (s_OptimalHits.Contains(hit.collider))
                        continue;

                    // It must have an interactable
                    if (!interactionManager.IsColliderRegisteredToInteractable(hit.collider))
                        continue;

                    if (Mathf.Approximately(hit.distance, 0f) && BurstMathUtility.FastVectorEquals(hit.point, Vector3.zero))
                    {
                        // Sphere cast can return hits where point is (0, 0, 0) in error.
                        continue;
                    }

                    // Adjust distance by distance from ray center for default sorting
                    BurstPhysicsUtils.GetConecastOffset(origin, hit.point, direction, out var hitToRayDist);

                    // We penalize these off-center hits by a meter + whatever horizontal offset they have
                    hit.distance += currentOffset + 1f + (hitToRayDist);
                    results[hitCounter] = hit;
                    hitCounter++;
                }

                currentOffset += castMax;
            }

            s_OptimalHits.Clear();
            Array.Clear(s_SpherecastScratch, 0, k_MaxRaycastHits);
            return hitCounter;
        }

        static int FilterOutSnapTriggerColliders(in XRInteractionManager interactionManager, RaycastHit[] raycastHits, int count)
        {
            int remainingCount = count;
            for (var index = 0; index < remainingCount; ++index)
            {
                var hitCollider = raycastHits[index].collider;
                if (hitCollider == null || (hitCollider.isTrigger && interactionManager.IsColliderRegisteredSnapVolume(hitCollider)))
                {
                    // Replace item at current index with item at the end of the list.
                    raycastHits[index--] = raycastHits[--remainingCount];
                }
            }

            return remainingCount;
        }

        static int FilterOutNonSnapTriggerColliders(in XRInteractionManager interactionManager, RaycastHit[] raycastHits, int count)
        {
            int remainingCount = count;
            for (var index = 0; index < remainingCount; ++index)
            {
                var hitCollider = raycastHits[index].collider;
                if (hitCollider == null || (hitCollider.isTrigger && !interactionManager.IsColliderRegisteredSnapVolume(hitCollider)))
                {
                    // Replace item at current index with item at the end of the list.
                    raycastHits[index--] = raycastHits[--remainingCount];
                }
            }

            return remainingCount;
        }

        /// <inheritdoc />
        public bool UpdateUIModel(ref TrackedDeviceModel uiModel, bool isSelectActive, in Vector2 scrollDelta)
        {
            if (!isInitialized)
                return false;

            var sampleOrigin = effectiveCastOrigin;
            uiModel.position = sampleOrigin.position;
            uiModel.orientation = sampleOrigin.rotation;
            uiModel.select = isSelectActive;
            uiModel.scrollDelta = scrollDelta;
            uiModel.raycastLayerMask = m_RaycastMask;
            uiModel.interactionType = UIInteractionType.Ray;

            var raycastPoints = uiModel.raycastPoints;
            raycastPoints.Clear();

            UpdateInternalData();
            var numPoints = m_SamplePoints.Length;
            if (numPoints <= 0)
            {
                return false;
            }

            if (raycastPoints.Capacity < numPoints)
                raycastPoints.Capacity = numPoints;

            for (var i = 0; i < numPoints; ++i)
            {
                raycastPoints.Add(m_SamplePoints[i]);
            }

            return true;
        }

        /// <summary>
        /// Compares ray cast hits by distance, to sort in ascending order.
        /// </summary>
        protected sealed class RaycastHitComparer : IComparer<RaycastHit>
        {
            /// <summary>
            /// Compares ray cast hits by distance in ascending order.
            /// </summary>
            /// <param name="a">The first ray cast hit to compare.</param>
            /// <param name="b">The second ray cast hit to compare.</param>
            /// <returns>Returns less than 0 if a is closer than b. 0 if a and b are equal. Greater than 0 if b is closer than a.</returns>
            public int Compare(RaycastHit a, RaycastHit b)
            {
                return a.distance.CompareTo(b.distance);
            }
        }
    }
}