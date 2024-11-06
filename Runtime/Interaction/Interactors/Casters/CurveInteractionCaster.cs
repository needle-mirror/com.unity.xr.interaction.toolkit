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

        [SerializeField]
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

        float m_CachedConeCastAngle;
        float m_CachedConeCastRadius;

        float coneCastAngleRadius
        {
            get
            {
                if (!Mathf.Approximately(m_CachedConeCastAngle, m_ConeCastAngle))
                {
                    m_CachedConeCastAngle = m_ConeCastAngle;
                    m_CachedConeCastRadius = math.tan(math.radians(m_CachedConeCastAngle) * 0.5f);
                }

                return m_CachedConeCastRadius;
            }
        }

        [SerializeField]
        bool m_LiveConeCastDebugVisuals;

        /// <summary>
        /// If enabled, more detailed cone cast gizmos will be displayed in the editor.
        /// </summary>
        internal bool liveConeCastDebugVisuals
        {
            get => m_LiveConeCastDebugVisuals;
            set => m_LiveConeCastDebugVisuals = value;
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

        /// <summary>
        /// List containing cone cast debug information including the position of the sphere casts and the radius.
        /// </summary>
        readonly List<Tuple<Vector3, float>> m_ConeCastDebugInfo = new List<Tuple<Vector3, float>>();

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();

            // Turn off live debug visuals if not running in the editor
            if (!Application.isEditor)
                m_LiveConeCastDebugVisuals = false;
        }

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
                {
                    targets.Add(m_RaycastHits[i].collider);
                }
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

            float distancePercentBetweenPoints = totalDistance / (numPoints - 1);
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
            m_ConeCastDebugInfo.Clear();

            float totalCastLength = 0f;
            for (var i = 1; i < samplePoints.Length; ++i)
            {
                float3 origin = samplePoints[0];
                float3 fromPoint = samplePoints[i - 1];
                float3 toPoint = samplePoints[i];

                m_RaycastHitsCount = CheckCollidersBetweenPoints(interactionManager, fromPoint, toPoint, origin, m_RaycastHits);
                if (m_RaycastHitsCount > 0)
                {
                    // Add the total cast distance to the raycast hit distance
                    for (var j = 0; j < m_RaycastHitsCount; ++j)
                    {
                        m_RaycastHits[j].distance += totalCastLength;
                    }

                    break;
                }

                // Add the segment distance to the total cast length for the next segment that is processed
                var distanceBetweenPoints = math.length(toPoint - fromPoint);
                totalCastLength += distanceBetweenPoints;
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
                    raycastHitCount = FilteredConecast(interactionManager, from, direction, origin,
                        raycastHits, distanceBetweenPoints, m_RaycastMask, queryTriggerInteraction);
                    break;
            }

            if (raycastHitCount > 0)
            {
                if (m_HitDetectionType != HitDetectionType.ConeCast)
                    raycastHitCount = FilterOutTriggerColliders(interactionManager, raycastHits, raycastHitCount);

                // Sort all the hits by distance along the curve since the results of the 3D ray cast are not ordered.
                // Sorting is done after filtering above for performance.
                SortingHelpers.Sort(raycastHits, m_RaycastHitComparer, raycastHitCount);
            }

            return raycastHitCount;
        }

        int FilteredConecast(XRInteractionManager interactionManager, in Vector3 from, in Vector3 direction, in Vector3 origin, RaycastHit[] results, float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            s_OptimalHits.Clear();

            var obstructionDistance = math.min(maxDistance, 1000f);
            var hitCounter = 0;

            // Raycast looking for obstructions and any optimal targets
            var optimalHits = m_LocalPhysicsScene.Raycast(from, direction, s_SpherecastScratch, maxDistance, layerMask, queryTriggerInteraction);
            if (optimalHits > 0)
            {
                optimalHits = FilterOutTriggerColliders(interactionManager, s_SpherecastScratch, optimalHits);

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
            var distanceOriginToStartSegment = (origin - from).magnitude;
            var currentSegmentCastMax = obstructionDistance;
            var currentSegmentCastDistance = 0f;

            while (currentSegmentCastDistance < obstructionDistance && !Mathf.Approximately(currentSegmentCastDistance, obstructionDistance))
            {
                var currentOffsetFromOrigin = distanceOriginToStartSegment + currentSegmentCastDistance;

                BurstPhysicsUtils.GetMultiSegmentConecastParameters(coneCastAngleRadius, currentSegmentCastDistance, currentOffsetFromOrigin, currentSegmentCastMax, direction, out var originRadiusOffset, out var endRadius, out var sphereCastDistance);

                if (m_LiveConeCastDebugVisuals)
                {
                    m_ConeCastDebugInfo.Add(new Tuple<Vector3, float>(from + originRadiusOffset, endRadius));
                    m_ConeCastDebugInfo.Add(new Tuple<Vector3, float>(from + originRadiusOffset + (sphereCastDistance * direction), endRadius));
                }

                // Spherecast
                var initialResults = m_LocalPhysicsScene.SphereCast(from + originRadiusOffset, endRadius, direction, s_SpherecastScratch, sphereCastDistance, layerMask, queryTriggerInteraction);
                if (initialResults > 0)
                {
                    initialResults = FilterOutTriggerColliders(interactionManager, s_SpherecastScratch, initialResults);

                    for (var i = 0; (i < initialResults && hitCounter < results.Length); i++)
                    {
                        var hit = s_SpherecastScratch[i];
                        var totalHitDistance = currentSegmentCastDistance + hit.distance;

                        // Range check
                        if (totalHitDistance > obstructionDistance)
                            continue;

                        // If it's an optimal hit, then skip it
                        if (s_OptimalHits.Contains(hit.collider))
                            continue;

                        // It must have an interactable
                        if (!interactionManager.IsColliderRegisteredToInteractable(hit.collider))
                            continue;

                        if (Mathf.Approximately(hit.distance, 0f) && BurstMathUtility.FastVectorEquals(hit.point, Vector3.zero))
                            // Sphere cast can return hits where point is (0, 0, 0) in error.
                            continue;

                        // Adjust distance by distance from ray center for default sorting
                        BurstPhysicsUtils.GetConecastOffset(from, hit.point, direction, out var hitToRayDist);

                        // We penalize these off-center hits by a meter + whatever horizontal offset they have
                        // this should be distance from segment start + penalty
                        hit.distance += currentSegmentCastDistance + 1f + (hitToRayDist);
                        results[hitCounter] = hit;
                        hitCounter++;
                    }
                }

                currentSegmentCastDistance += sphereCastDistance;
            }

            s_OptimalHits.Clear();
            Array.Clear(s_SpherecastScratch, 0, k_MaxRaycastHits);
            return hitCounter;
        }

        /// <summary>
        /// Filters out trigger colliders based on caster trigger collider settings.
        /// </summary>
        /// <param name="interactionManager">The XR interaction manager used to help filter colliders.</param>
        /// <param name="raycastHits">Array to store the results of the raycast hits.</param>
        /// <param name="raycastHitCount">Number of existing raycast hits.</param>
        /// <returns>Returns the number of raycast hits after filtering out trigger colliders.</returns>
        int FilterOutTriggerColliders(XRInteractionManager interactionManager, RaycastHit[] raycastHits, int raycastHitCount)
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

            return raycastHitCount;
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

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        internal virtual void OnDrawGizmosSelected()
        {
            var transformData = castOrigin != null ? castOrigin : transform;
            var gizmoStart = transformData.position;
            var gizmoEnd = gizmoStart + (transformData.forward * castDistance);
            Gizmos.color = new Color(58 / 255f, 122 / 255f, 248 / 255f, 237 / 255f);

            switch (m_HitDetectionType)
            {
                case HitDetectionType.Raycast:
                    // Draw the raycast line
                    Gizmos.DrawLine(gizmoStart, gizmoEnd);
                    break;

                case HitDetectionType.SphereCast:
                    {
                        var gizmoUp = transformData.up * m_SphereCastRadius;
                        var gizmoSide = transformData.right * m_SphereCastRadius;
                        Gizmos.DrawWireSphere(gizmoStart, m_SphereCastRadius);
                        Gizmos.DrawLine(gizmoStart + gizmoSide, gizmoEnd + gizmoSide);
                        Gizmos.DrawLine(gizmoStart - gizmoSide, gizmoEnd - gizmoSide);
                        Gizmos.DrawLine(gizmoStart + gizmoUp, gizmoEnd + gizmoUp);
                        Gizmos.DrawLine(gizmoStart - gizmoUp, gizmoEnd - gizmoUp);
                        Gizmos.DrawWireSphere(gizmoEnd, m_SphereCastRadius);
                        break;
                    }

                case HitDetectionType.ConeCast:
                    {
                        var coneRadius = Mathf.Tan(m_ConeCastAngle * Mathf.Deg2Rad * 0.5f) * castDistance;
                        gizmoEnd = gizmoStart + (transformData.forward * (castDistance - coneRadius));
                        var gizmoUp = transformData.up * coneRadius;
                        var gizmoSide = transformData.right * coneRadius;
                        Gizmos.DrawLine(gizmoStart, gizmoEnd);
                        Gizmos.DrawLine(gizmoStart, gizmoEnd + gizmoSide);
                        Gizmos.DrawLine(gizmoStart, gizmoEnd - gizmoSide);
                        Gizmos.DrawLine(gizmoStart, gizmoEnd + gizmoUp);
                        Gizmos.DrawLine(gizmoStart, gizmoEnd - gizmoUp);
                        Gizmos.DrawWireSphere(gizmoEnd, coneRadius);
                        break;
                    }
            }

            // Draw sample points where the ray cast line segments took place
            for (var i = 0; i < m_SamplePoints.Length; ++i)
            {
                var samplePoint = m_SamplePoints[i];

                // Change the color of the points after the segment where a hit happened
                var radius = m_HitDetectionType == HitDetectionType.SphereCast ? m_SphereCastRadius : 0.025f;
                Gizmos.color = new Color(163 / 255f, 73 / 255f, 164 / 255f, 0.75f);
                Gizmos.DrawSphere(samplePoint, radius);
                if (i < m_SamplePoints.Length - 1)
                {
                    var nextPoint = m_SamplePoints[i + 1];
                    Gizmos.DrawLine(samplePoint, nextPoint);
                }
            }

            if (m_LiveConeCastDebugVisuals)
            {
                const float numDisplaySpheresInConeCast = 4f;
                for (var i = 0; i < m_ConeCastDebugInfo.Count; i += 2)
                {
                    Gizmos.color = Color.yellow;
                    for (var j = 0f; j <= numDisplaySpheresInConeCast; ++j)
                    {
                        var percent = j / numDisplaySpheresInConeCast;
                        var lerpVector = (m_ConeCastDebugInfo[i].Item1 + percent * (m_ConeCastDebugInfo[i + 1].Item1 - m_ConeCastDebugInfo[i].Item1));
                        Gizmos.DrawWireSphere(lerpVector, m_ConeCastDebugInfo[i].Item2);
                    }
                }
            }
        }
    }
}
