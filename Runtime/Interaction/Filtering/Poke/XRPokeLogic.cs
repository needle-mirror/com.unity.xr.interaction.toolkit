using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Bindings.Variables;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
#if BURST_PRESENT
using Unity.Burst;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.Filtering
{
    /// <summary>
    /// Class that encapsulates the logic for evaluating whether an interactable has been poked or not
    /// through enter and exit thresholds.
    /// </summary>
#if BURST_PRESENT
    [BurstCompile]
#endif
    class XRPokeLogic : IDisposable
    {
        /// <summary>
        /// Length of interaction axis computed from the attached collider bounds and configured interaction direction.
        /// </summary>
        float interactionAxisLength { get; set; } = 1f;

        readonly BindableVariable<PokeStateData> m_PokeStateData = new BindableVariable<PokeStateData>();

        /// <summary>
        /// Bindable variable that updates whenever the poke logic state is evaluated.
        /// </summary>
        public IReadOnlyBindableVariable<PokeStateData> pokeStateData => m_PokeStateData;

        Transform m_InitialTransform;
        PokeThresholdData m_PokeThresholdData;
        float m_SelectEntranceVectorDotThreshold;

        readonly Dictionary<object, Transform> m_LastHoveredTransform = new Dictionary<object, Transform>();
        readonly Dictionary<object, bool> m_HoldingHoverCheck = new Dictionary<object, bool>();
        readonly Dictionary<Transform, HashSetList<object>> m_HoveredInteractorsOnThisTransform = new Dictionary<Transform, HashSetList<object>>();
        readonly Dictionary<object, float> m_LastInteractorPressDepth = new Dictionary<object, float>();
        readonly Dictionary<object, bool> m_LastRequirementsMet = new Dictionary<object, bool>();

        /// <summary>
        /// Threshold value where the poke interaction is considered to be selecting the interactable.
        /// We normally checked 1% as the activation point, but setting it to 2.5 % makes things feel a bit more responsive.
        /// </summary>
        const float k_DepthPercentActivationThreshold = 0.025f;

        /// <summary>
        /// We require a minimum velocity for poke hover conditions to be met, and avoid the noise of tracking jitter.
        /// </summary>
        const float k_SquareVelocityHoverThreshold = 0.0001f;

        /// <summary>
        /// Initializes <see cref="XRPokeLogic"/> with properties calculated from the collider of the associated interactable.
        /// </summary>
        /// <param name="associatedTransform"><see cref="Transform"/> object used for poke calculations.</param>
        /// <param name="pokeThresholdData"><see cref="PokeThresholdData"/> object containing the specific poke parameters used for calculating
        /// whether or not the current interaction meets the requirements for poke hover or select.</param>
        /// <param name="collider"><see cref="Collider"/> for computing the interaction axis length used to detect if poke depth requirements are met.</param>
        public void Initialize(Transform associatedTransform, PokeThresholdData pokeThresholdData, Collider collider)
        {
            m_InitialTransform = associatedTransform;
            m_PokeThresholdData = pokeThresholdData;
            m_SelectEntranceVectorDotThreshold = pokeThresholdData.GetSelectEntranceVectorDotThreshold();

            if (collider != null)
            {
                interactionAxisLength = ComputeInteractionAxisLength(ComputeBounds(collider));
            }
            ResetPokeStateData(m_InitialTransform);
        }

        /// <summary>
        /// This method will reset the underlying interaction length used to determine if the current poke depth has been reached. This is typically
        /// used on UI objects, or objects where poke depth is not appropriately defined by the collider bounds of the object.
        /// </summary>
        /// <param name="pokeDepth">A value representing the poke depth required to meet requirements for select.</param>
        public void SetPokeDepth(float pokeDepth)
        {
            interactionAxisLength = pokeDepth;
        }

        /// <summary>
        /// Clears cached data hover enter pose data.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Logic to check if the attempted poke interaction meets the requirements for a select action.
        /// </summary>
        /// <param name="interactor">The interactor that is a candidate for selection.</param>
        /// <param name="pokableAttachPosition">The attach transform position of the pokable object, typically an interactable object.</param>
        /// <param name="pokerAttachPosition">The attach transform position for the interactor.</param>
        /// <param name="pokeInteractionOffset">An additional offset that will be applied to the calculation for the depth required to meet requirements for selection.</param>
        /// <param name="pokedTransform">The target Transform that is being poked.</param>
        /// <returns>
        /// Returns <see langword="true"/> if interaction meets requirements for select action.
        /// Otherwise, returns <see langword="false"/>.
        /// </returns>
        public bool MeetsRequirementsForSelectAction(object interactor, Vector3 pokableAttachPosition, Vector3 pokerAttachPosition, float pokeInteractionOffset, Transform pokedTransform)
        {
            if (!IsPokeDataValid(pokedTransform))
                return false;

            Vector3 axisNormal = ComputeRotatedDepthEvaluationAxis(pokedTransform);
            Vector3 interactionPoint = CalculateInteractionPoint(pokerAttachPosition, axisNormal, pokeInteractionOffset);

            CalculatePokeParams(interactionPoint, pokableAttachPosition, axisNormal, out float interactionDepth, out float entranceVectorDot);

            bool isOverObject = entranceVectorDot > 0f;
            float depthPercent = CalculateDepthPercent(interactionDepth, entranceVectorDot, interactionAxisLength);

            bool meetsHoverRequirements = CalculateHoverRequirements(interactor, isOverObject, axisNormal);

            float clampedDepthPercent = !meetsHoverRequirements ? 1f : Mathf.Clamp01(depthPercent);

            bool meetsRequirements = CalculateRequirements(ref meetsHoverRequirements, clampedDepthPercent, interactor);

            UpdatePokeStateData(meetsRequirements, meetsHoverRequirements, clampedDepthPercent, interactor, pokerAttachPosition, pokableAttachPosition, axisNormal, pokedTransform);

            return meetsRequirements;
        }

        bool IsPokeDataValid(Transform pokedTransform)
        {
            return m_PokeThresholdData != null && pokedTransform != null;
        }

        Vector3 CalculateInteractionPoint(Vector3 pokerAttachPosition, Vector3 axisNormal, float pokeInteractionOffset)
        {
            float combinedOffset = pokeInteractionOffset + m_PokeThresholdData.interactionDepthOffset;
            CalculateInteractionPoint(pokerAttachPosition, axisNormal, combinedOffset, out var interactionPoint);
            return interactionPoint;
        }

        bool CalculateHoverRequirements(object interactor, bool isOverObject, float3 axisNormal)
        {
            if (!m_PokeThresholdData.enablePokeAngleThreshold)
                return true;

            bool meetsHoverRequirements = true;
            if (!m_HoldingHoverCheck.TryGetValue(interactor, out bool holdingHoverCheck) || !holdingHoverCheck)
                meetsHoverRequirements = CheckVelocity(interactor, isOverObject, axisNormal);
            return meetsHoverRequirements;
        }

        bool CheckVelocity(object interactor, bool isOverObject, Vector3 axisNormal)
        {
            if (!isOverObject)
                return false;

            if (interactor is not IAttachPointVelocityProvider velocityProvider)
                return true;

            var interactorVelocity = velocityProvider.GetAttachPointVelocity();
            if (!IsVelocitySufficient(interactorVelocity, k_SquareVelocityHoverThreshold))
                return false;

            float velocityAxisDotProduct = Vector3.Dot(-interactorVelocity.normalized, axisNormal);
            return velocityAxisDotProduct > m_SelectEntranceVectorDotThreshold;
        }

#if BURST_PRESENT
        [BurstCompile]
#endif
        static void CalculatePokeParams(in float3 interactionPoint, in float3 pokableAttachPosition, in float3 axisNormal, out float interactionDepth, out float entranceVectorDot)
        {
            var interactionPointOffset = interactionPoint - pokableAttachPosition;
            var axisAlignedInteractionPointOffset = math.project(interactionPointOffset, axisNormal);
            interactionDepth = math.length(axisAlignedInteractionPointOffset);
            entranceVectorDot = math.dot(axisNormal, math.normalizesafe(interactionPointOffset));
        }

#if BURST_PRESENT
        [BurstCompile]
#endif
        static void CalculateInteractionPoint(in float3 pokerAttachPosition, in float3 axisNormal, float combinedPokeOffset, out float3 interactionPoint)
        {
            float3 toleranceOffset = axisNormal * combinedPokeOffset;
            interactionPoint = pokerAttachPosition - toleranceOffset;
        }

#if BURST_PRESENT
        [BurstCompile]
#endif
        float CalculateDepthPercent(float interactionDepth, float entranceVectorDot, float axisLength)
        {
            float entranceVectorDotSign = math.sign(entranceVectorDot);
            return entranceVectorDotSign * interactionDepth / axisLength;
        }

#if BURST_PRESENT
        [BurstCompile]
#endif
        static bool IsVelocitySufficient(in float3 velocity, float threshold) => math.lengthsq(velocity) > threshold;

        bool CalculateRequirements(ref bool meetsHoverRequirements, float clampedDepthPercent, object interactor)
        {
            bool meetsRequirements = meetsHoverRequirements && clampedDepthPercent < k_DepthPercentActivationThreshold;
            if (m_LastRequirementsMet.TryGetValue(interactor, out bool lastRequirementsMet) && lastRequirementsMet && !meetsRequirements)
                meetsHoverRequirements = false;
            return meetsRequirements;
        }

        void UpdatePokeStateData(bool meetsRequirements, bool meetsHoverRequirements, float clampedDepthPercent, object interactor, Vector3 pokerAttachPosition, Vector3 pokableAttachPosition, Vector3 axisNormal, Transform pokedTransform)
        {
            m_HoldingHoverCheck[interactor] = meetsHoverRequirements;
            m_LastRequirementsMet[interactor] = meetsRequirements;
            m_LastInteractorPressDepth[interactor] = clampedDepthPercent;

            if (!meetsRequirements && m_HoveredInteractorsOnThisTransform.TryGetValue(pokedTransform, out var hoveringInteractors))
            {
                var hoveringInteractorsCount = hoveringInteractors.Count;
                if (hoveringInteractorsCount > 1)
                {
                    var hoveringInteractorsList = hoveringInteractors.AsList();
                    for (int i = 0; i < hoveringInteractorsCount; i++)
                    {
                        var hoveringInteractor = hoveringInteractorsList[i];
                        if (hoveringInteractor == interactor)
                            continue;

                        var otherInteractorPressDepth = m_LastInteractorPressDepth[hoveringInteractor];
                        if (otherInteractorPressDepth < clampedDepthPercent)
                            return;
                    }
                }
            }

            var offsetRemoval = clampedDepthPercent < 1f && !meetsRequirements ? m_PokeThresholdData.interactionDepthOffset : 0f;
            var clampedPokeDepth = Mathf.Clamp(clampedDepthPercent * interactionAxisLength + offsetRemoval, 0f, interactionAxisLength);

            m_PokeStateData.Value = new PokeStateData
            {
                meetsRequirements = meetsRequirements,
                pokeInteractionPoint = pokerAttachPosition,
                axisAlignedPokeInteractionPoint = pokableAttachPosition + clampedPokeDepth * axisNormal,
                interactionStrength = 1f - clampedDepthPercent,
                axisNormal = axisNormal,
                target = pokedTransform,
            };
        }

        /// <summary>
        /// Computes the direction of the interaction axis, as configured with the poke threshold data.
        /// </summary>
        /// <param name="associatedTransform">This represents the Transform used to determine the evaluation axis along the specified poke axis.</param>
        /// <param name="isWorldSpace">World space uses the current interactable rotation, local space takes basic vector directions.</param>
        /// <returns>Normalized vector along the axis of interaction.</returns>
        Vector3 ComputeRotatedDepthEvaluationAxis(Transform associatedTransform, bool isWorldSpace = true)
        {
            if (m_PokeThresholdData == null || associatedTransform == null)
                return Vector3.zero;

            Vector3 rotatedDepthEvaluationAxis = Vector3.zero;
            switch (m_PokeThresholdData.pokeDirection)
            {
                case PokeAxis.X:
                case PokeAxis.NegativeX:
                    rotatedDepthEvaluationAxis = isWorldSpace ? associatedTransform.right : Vector3.right;
                    break;
                case PokeAxis.Y:
                case PokeAxis.NegativeY:
                    rotatedDepthEvaluationAxis = isWorldSpace ? associatedTransform.up : Vector3.up;
                    break;
                case PokeAxis.Z:
                case PokeAxis.NegativeZ:
                    rotatedDepthEvaluationAxis = isWorldSpace ? associatedTransform.forward : Vector3.forward;
                    break;
            }

            switch (m_PokeThresholdData.pokeDirection)
            {
                case PokeAxis.X:
                case PokeAxis.Y:
                case PokeAxis.Z:
                    rotatedDepthEvaluationAxis = -rotatedDepthEvaluationAxis;
                    break;
            }

            return rotatedDepthEvaluationAxis;
        }

        float ComputeInteractionAxisLength(Bounds bounds)
        {
            if (m_PokeThresholdData == null || m_InitialTransform == null)
                return 0f;

            Vector3 boundsSize = bounds.size;

            Vector3 center = m_InitialTransform.position;

            float lengthOfInteractionAxis = 0f;
            float centerOffsetLength;

            switch (m_PokeThresholdData.pokeDirection)
            {
                case PokeAxis.X:
                case PokeAxis.NegativeX:
                    centerOffsetLength = bounds.center.x - center.x;
                    lengthOfInteractionAxis = boundsSize.x / 2f + centerOffsetLength;
                    break;
                case PokeAxis.Y:
                case PokeAxis.NegativeY:
                    centerOffsetLength = bounds.center.y - center.y;
                    lengthOfInteractionAxis = boundsSize.y / 2f + centerOffsetLength;
                    break;
                case PokeAxis.Z:
                case PokeAxis.NegativeZ:
                    centerOffsetLength = bounds.center.z - center.z;
                    lengthOfInteractionAxis = boundsSize.z / 2f + centerOffsetLength;
                    break;
            }

            return lengthOfInteractionAxis;
        }

        /// <summary>
        /// Logic for caching pose when an <see cref="IXRInteractor"/> enters a hover state.
        /// </summary>
        /// <param name="interactor">The XR Interactor associated with the hover enter event interaction.</param>
        /// <param name="updatedPose">The pose of the interactor's attach transform, in world space.</param>
        /// <param name="pokedTransform">The transform of the poked object. Mainly considered for UGUI work where poke logic is shared between multiple transforms.</param>
        public void OnHoverEntered(object interactor, Pose updatedPose, Transform pokedTransform)
        {
            m_LastHoveredTransform[interactor] = pokedTransform;
            m_LastInteractorPressDepth[interactor] = 1f;
            m_HoldingHoverCheck[interactor] = false;
            m_LastRequirementsMet[interactor] = false;

            if (!m_HoveredInteractorsOnThisTransform.TryGetValue(pokedTransform, out var hoveringInteractors))
            {
                hoveringInteractors = new HashSetList<object>();
                m_HoveredInteractorsOnThisTransform[pokedTransform] = hoveringInteractors;
            }

            hoveringInteractors.Add(interactor);
        }

        /// <summary>
        /// Logic to update poke state data when interaction terminates.
        /// </summary>
        /// <param name="interactor">The XR Interactor associated with the hover exit event interaction.</param>
        public void OnHoverExited(object interactor)
        {
            m_HoldingHoverCheck[interactor] = false;
            m_LastInteractorPressDepth[interactor] = 1f;
            m_LastRequirementsMet[interactor] = false;

            if (m_LastHoveredTransform.TryGetValue(interactor, out var lastTransform))
            {
                if (m_HoveredInteractorsOnThisTransform.TryGetValue(lastTransform, out var hoveringInteractors))
                    hoveringInteractors.Remove(interactor);

                ResetPokeStateData(lastTransform);
                m_LastHoveredTransform.Remove(interactor);
            }
            else if (m_LastHoveredTransform.Count == 0)
            {
                ResetPokeStateData(m_InitialTransform);
            }
        }

        void ResetPokeStateData(Transform transform)
        {
            if (transform == null)
                return;

            var startPos = transform.position;
            var axisNormal = ComputeRotatedDepthEvaluationAxis(transform);
            var axisExtent = startPos + axisNormal * interactionAxisLength;

            m_PokeStateData.Value = new PokeStateData
            {
                meetsRequirements = false,
                pokeInteractionPoint = axisExtent,
                axisAlignedPokeInteractionPoint = axisExtent,
                interactionStrength = 0f,
                axisNormal = Vector3.zero,
                target = null,
            };
        }

        static Bounds ComputeBounds(Collider targetCollider, bool rotateBoundsScale = false, Space targetSpace = Space.World)
        {
            Bounds newBounds = default;
            if (targetCollider is BoxCollider boxCollider)
            {
                newBounds = new Bounds(boxCollider.center, boxCollider.size);
            }
            else if (targetCollider is SphereCollider sphereCollider)
            {
                newBounds = new Bounds(sphereCollider.center, Vector3.one * (sphereCollider.radius * 2));
            }
            else if (targetCollider is CapsuleCollider capsuleCollider)
            {
                Vector3 targetSize = Vector3.zero;
                float diameter = capsuleCollider.radius * 2f;
                float fullHeight = capsuleCollider.height;
                switch (capsuleCollider.direction)
                {
                    // X
                    case 0:
                        targetSize = new Vector3(fullHeight, diameter, diameter);
                        break;
                    // Y
                    case 1:
                        targetSize = new Vector3(diameter, fullHeight, diameter);
                        break;
                    // Z
                    case 2:
                        targetSize = new Vector3(diameter, diameter, fullHeight);
                        break;
                }

                newBounds = new Bounds(capsuleCollider.center, targetSize);
            }

            if (targetSpace == Space.Self)
                return newBounds;

            return BoundsLocalToWorld(newBounds, targetCollider.transform, rotateBoundsScale);
        }

        static Bounds BoundsLocalToWorld(Bounds targetBounds, Transform targetTransform, bool rotateBoundsScale = false)
        {
            Vector3 objectScale = targetTransform.lossyScale;
            Vector3 adjustedSize = objectScale.Multiply(targetBounds.size);
            Vector3 rotatedSize = rotateBoundsScale ? targetTransform.rotation * adjustedSize : adjustedSize;
            return new Bounds(targetTransform.position + objectScale.Multiply(targetBounds.center), rotatedSize);
        }

        /// <summary>
        /// Logic for drawing gizmos in the editor that visualize the collider bounds and vector through which a poke
        /// interaction will be evaluated for interactables that support poke.
        /// </summary>
        public void DrawGizmos()
        {
            if (m_PokeThresholdData == null || m_InitialTransform == null)
                return;

            Vector3 interactionOrigin = m_InitialTransform.position;
            var interactionNormal = ComputeRotatedDepthEvaluationAxis(m_InitialTransform);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(interactionOrigin, interactionOrigin + interactionNormal * interactionAxisLength);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(interactionOrigin, interactionOrigin + interactionNormal * m_PokeThresholdData.interactionDepthOffset);

            if (m_PokeStateData != null && m_PokeStateData.Value.interactionStrength > 0f)
            {
                Gizmos.color = m_PokeStateData.Value.meetsRequirements ? Color.green : Color.yellow;
                Gizmos.DrawWireSphere(m_PokeStateData.Value.pokeInteractionPoint, 0.01f);
                Gizmos.DrawWireSphere(m_PokeStateData.Value.axisAlignedPokeInteractionPoint, 0.01f);
            }
        }
    }
}
