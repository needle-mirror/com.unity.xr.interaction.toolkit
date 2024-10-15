using System;
#if BURST_PRESENT
using Unity.Burst;
#endif
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Curves;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals
{
    /// <summary>
    /// Specifies the dynamics mode of the line.
    /// </summary>
    /// <seealso cref="CurveVisualController.lineDynamicsMode"/>
    public enum LineDynamicsMode
    {
        /// <summary>
        /// Traditional line mode.
        /// </summary>
        Traditional,

        /// <summary>
        /// Retract the line when a hit is lost.
        /// </summary>
        RetractOnHitLoss,

        /// <summary>
        /// Expand the line from the hit point.
        /// </summary>
        ExpandFromHitPoint,
    }

    /// <summary>
    /// A collection of visual line properties that can be customized for different endpoint type states.
    /// </summary>
    /// <seealso cref="CurveVisualController.noValidHitProperties"/>
    /// <seealso cref="CurveVisualController.uiHitProperties"/>
    /// <seealso cref="CurveVisualController.uiPressHitProperties"/>
    /// <seealso cref="CurveVisualController.hoverHitProperties"/>
    /// <seealso cref="CurveVisualController.selectHitProperties"/>
    [Serializable]
    public class LineProperties
    {
        const float k_DefaultLineWidth = 0.005f;

        [Header("Bend Settings")]
        [SerializeField]
        [Tooltip("Determine if the line should smoothly curve when this state property is active. If false, a straight line will be drawn.")]
        bool m_SmoothlyCurveLine = true;

        /// <summary>
        /// Determine if the line should smoothly curve when this state property is active. If false, a straight line will be drawn.
        /// </summary>
        public bool smoothlyCurveLine
        {
            get => m_SmoothlyCurveLine;
            set => m_SmoothlyCurveLine = value;
        }

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Ratio to control the bend of the line by adjusting the mid-point. A value of 1 defaults to a straight line.")]
        float m_LineBendRatio = 0.5f;

        /// <summary>
        /// Ratio to control the bend of the line by adjusting the mid-point. A value of 1 defaults to a straight line.
        /// </summary>
        public float lineBendRatio
        {
            get => m_LineBendRatio;
            set => m_LineBendRatio = value;
        }

        [Header("Width Settings")]
        [SerializeField]
        [Tooltip("Determine if the line width should be customized from defaults when this state property is active.")]
        bool m_AdjustWidth = true;

        /// <summary>
        /// Determine if the line width should be customized from defaults when this state property is active.
        /// </summary>
        public bool adjustWidth
        {
            get => m_AdjustWidth;
            set => m_AdjustWidth = value;
        }

        [SerializeField]
        [Tooltip("Width of the line at the start.")]
        float m_StarWidth = k_DefaultLineWidth;

        /// <summary>
        /// Width of the line at the start.
        /// </summary>
        public float starWidth
        {
            get => m_StarWidth;
            set => m_StarWidth = value;
        }

        [SerializeField]
        [Tooltip("Width of the line at the end.")]
        float m_EndWidth = k_DefaultLineWidth;

        /// <summary>
        /// Width of the line at the end.
        /// </summary>
        public float endWidth
        {
            get => m_EndWidth;
            set => m_EndWidth = value;
        }

        [SerializeField]
        [Range(0f, 10f)]
        [Tooltip("If greater than 0, the curve end width will be scaled based on the the percentage of the line length to the max visual curve distance, multiplied by the scale factor.")]
        float m_EndWidthScaleDistanceFactor = 2f;

        /// <summary>
        /// If greater than 0, the curve end width will be scaled based on the the percentage of the line length to the max visual curve distance, multiplied by the scale factor.
        /// </summary>
        public float endWidthScaleDistanceFactor
        {
            get => m_EndWidthScaleDistanceFactor;
            set => m_EndWidthScaleDistanceFactor = value;
        }

        [Header("Gradient Settings")]
        [SerializeField]
        [Tooltip("Determine if the line color should change when this state property is active.")]
        bool m_AdjustGradient = true;

        /// <summary>
        /// Determine if the line color should change when this state property is active.
        /// </summary>
        public bool adjustGradient
        {
            get => m_AdjustGradient;
            set => m_AdjustGradient = value;
        }

        [SerializeField]
        [Tooltip("Color gradient to use when this state property is active.")]
        Gradient m_Gradient;

        /// <summary>
        /// Color gradient to use when this state property is active.
        /// </summary>
        public Gradient gradient
        {
            get => m_Gradient;
            set => m_Gradient = value;
        }

        [Header("Expand Settings")]
        [SerializeField]
        [Tooltip("Determine if the line mode expansion should be customized from defaults")]
        bool m_CustomizeExpandLineDrawPercent;

        /// <summary>
        /// Determine if the line mode expansion should be customized from defaults
        /// </summary>
        public bool customizeExpandLineDrawPercent
        {
            get => m_CustomizeExpandLineDrawPercent;
            set => m_CustomizeExpandLineDrawPercent = value;
        }

        [SerializeField]
        [Tooltip("Percent of the line to draw when using the expand from hit point mode when this state property is active.")]
        float m_ExpandModeLineDrawPercent = 1f;

        /// <summary>
        /// Percent of the line to draw when using the expand from hit point mode when this state property is active.
        /// </summary>
        public float expandModeLineDrawPercent
        {
            get => m_ExpandModeLineDrawPercent;
            set => m_ExpandModeLineDrawPercent = value;
        }
    }

    /// <summary>
    /// Behavior designed to provide a versatile and configurable controller for a Line Renderer component, based on
    /// data provided from an interactor implementing <see cref="ICurveInteractionDataProvider"/> to visually represent it.
    /// </summary>
    /// <remarks>
    /// It uses a configurable origin and direction, along with end point data from the Curve Data Provider, to generate
    /// a bezier curve that masks discrepancies between divergent origins, and mask any stabilization applied.
    /// </remarks>
    [DisallowMultipleComponent]
    [AddComponentMenu("XR/Visual/Curve Visual Controller", 11)]
    [HelpURL(XRHelpURLConstants.k_CurveVisualController)]
#if BURST_PRESENT
    [BurstCompile]
#endif
    public class CurveVisualController : MonoBehaviour
    {
        [SerializeField]
        LineRenderer m_LineRenderer;

        /// <summary>
        /// "Line renderer to control."
        /// </summary>
        public LineRenderer lineRenderer
        {
            get => m_LineRenderer;
            set
            {
                m_LineRenderer = value;
                m_LineRenderer.useWorldSpace = false;
            }
        }

        [SerializeField]
        [RequireInterface(typeof(ICurveInteractionDataProvider))]
        Object m_CurveVisualObject;

        /// <summary>
        /// Curve data source used to generate the visual curve.
        /// </summary>
        public ICurveInteractionDataProvider curveInteractionDataProvider
        {
            get => m_CurveDataProviderObjectRef.Get(m_CurveVisualObject);
            set => m_CurveDataProviderObjectRef.Set(ref m_CurveVisualObject, value);
        }

        readonly UnityObjectReferenceCache<ICurveInteractionDataProvider, Object> m_CurveDataProviderObjectRef = new UnityObjectReferenceCache<ICurveInteractionDataProvider, Object>();

        [SerializeField]
        bool m_OverrideLineOrigin = true;

        /// <summary>
        /// Indicates whether to override the line origin with a custom transform. If no <see cref="lineOriginTransform"/> is set and this is true, this transform will be used.
        /// </summary>
        public bool overrideLineOrigin
        {
            get => m_OverrideLineOrigin;
            set => m_OverrideLineOrigin = value;
        }

        [SerializeField]
        Transform m_LineOriginTransform;

        /// <summary>
        /// The transform that determines the origin position and direction of the line when overriding.
        /// </summary>
        public Transform lineOriginTransform
        {
            get => m_LineOriginTransform;
            set
            {
                m_LineOriginTransform = value;
                m_UseCustomOrigin = value != null;
            }
        }

        [SerializeField]
        int m_VisualPointCount = 20;

        /// <summary>
        /// Number of points used to create the visual curve.
        /// </summary>
        public int visualPointCount
        {
            get => m_VisualPointCount;
            set => m_VisualPointCount = value;
        }

        [SerializeField]
        float m_MaxVisualCurveDistance = 10f;

        /// <summary>
        /// Maximum distance the visual curve can extend.
        /// </summary>
        public float maxVisualCurveDistance
        {
            get => m_MaxVisualCurveDistance;
            set => m_MaxVisualCurveDistance = value;
        }

        [SerializeField]
        float m_RestingVisualLineLength = 0.15f;

        /// <summary>
        /// Default length of the line when not extended or retracted.
        /// </summary>
        public float restingVisualLineLength
        {
            get => m_RestingVisualLineLength;
            set => m_RestingVisualLineLength = value;
        }

        [SerializeField]
        LineDynamicsMode m_LineDynamicsMode = LineDynamicsMode.Traditional;

        /// <summary>
        /// Specifies the dynamics mode of the line.
        /// </summary>
        public LineDynamicsMode lineDynamicsMode
        {
            get => m_LineDynamicsMode;
            set => m_LineDynamicsMode = value;
        }

        [SerializeField]
        float m_RetractDelay = 1f;

        /// <summary>
        /// Delay before the line starts retracting after extending.
        /// </summary>
        public float retractDelay
        {
            get => m_RetractDelay;
            set => m_RetractDelay = value;
        }

        [SerializeField]
        float m_RetractDuration = 0.5f;

        /// <summary>
        /// Duration it takes for the line to fully retract.
        /// </summary>
        public float retractDuration
        {
            get => m_RetractDuration;
            set => m_RetractDuration = value;
        }

        [SerializeField]
        bool m_ExtendLineToEmptyHit;

        /// <summary>
        /// Determines if the line should extend out to empty hits, if not, length will be maintained.
        /// </summary>
        public bool extendLineToEmptyHit
        {
            get => m_ExtendLineToEmptyHit;
            set => m_ExtendLineToEmptyHit = value;
        }

        [SerializeField]
        [Range(0f, 30f)]
        float m_ExtensionRate = 10f;

        /// <summary>
        /// Rate at which the line extends. Set to 0 for instant extension.
        /// </summary>
        public float extensionRate
        {
            get => m_ExtensionRate;
            set => m_ExtensionRate = Mathf.Clamp(value, 0f, 30f);
        }

        [SerializeField]
        float m_EndPointExpansionRate = 10f;

        /// <summary>
        /// Rate at which the end point expands and retracts to and from the end point.
        /// </summary>
        public float endPointExpansionRate
        {
            get => m_EndPointExpansionRate;
            set => m_EndPointExpansionRate = value;
        }

        [SerializeField]
        bool m_ComputeMidPointWithComplexCurves;

        /// <summary>
        /// Determines if the mid-point is computed for curves with more than 1 segment. Useful to maintain a good shape with projectile curves. Overwrites any line bend ratio settings when active.
        /// </summary>
        public bool computeMidPointWithComplexCurves
        {
            get => m_ComputeMidPointWithComplexCurves;
            set => m_ComputeMidPointWithComplexCurves = value;
        }

        [SerializeField]
        bool m_SnapToSelectedAttachIfAvailable = true;

        /// <summary>
        /// Snaps the line to a selected attachment point if available.
        /// </summary>
        public bool snapToSelectedAttachIfAvailable
        {
            get => m_SnapToSelectedAttachIfAvailable;
            set => m_SnapToSelectedAttachIfAvailable = value;
        }

        [SerializeField]
        bool m_SnapToSnapVolumeIfAvailable = true;

        /// <summary>
        /// Snaps the line to a snap volume if available.
        /// </summary>
        public bool snapToSnapVolumeIfAvailable
        {
            get => m_SnapToSnapVolumeIfAvailable;
            set => m_SnapToSnapVolumeIfAvailable = value;
        }

        [SerializeField]
        float m_CurveStartOffset;

        /// <summary>
        /// Offset at the start of the curve to avoid overlap with the origin. Set to 0 to disable.
        /// </summary>
        public float curveStartOffset
        {
            get => m_CurveStartOffset;
            set => m_CurveStartOffset = value;
        }

        [SerializeField]
        float m_CurveEndOffset = 0.005f;

        /// <summary>
        /// Offset at the end of the curve in meters to avoid overlap with the target. Set to 0 to disable.
        /// </summary>
        public float curveEndOffset
        {
            get => m_CurveEndOffset;
            set => m_CurveEndOffset = value;
        }

        [SerializeField]
        bool m_CustomizeLinePropertiesForState;

        /// <summary>
        /// Indicates whether to customize line properties for different endpoint type states.
        /// </summary>
        public bool customizeLinePropertiesForState
        {
            get => m_CustomizeLinePropertiesForState;
            set => m_CustomizeLinePropertiesForState = value;
        }

        [SerializeField]
        float m_LinePropertyAnimationSpeed = 8f;

        /// <summary>
        /// Speed at which the line properties animate when transitioning between states. Set to 0 for instant transitions.
        /// </summary>
        public float linePropertyAnimationSpeed
        {
            get => m_LinePropertyAnimationSpeed;
            set => m_LinePropertyAnimationSpeed = value;
        }

        [SerializeField]
        LineProperties m_NoValidHitProperties;

        /// <summary>
        /// Line properties when no hit is detected or when over an object that cannot be interacted with.
        /// </summary>
        public LineProperties noValidHitProperties
        {
            get => m_NoValidHitProperties;
            set => m_NoValidHitProperties = value;
        }

        [SerializeField]
        LineProperties m_UIHitProperties;

        /// <summary>
        /// Line properties when a valid UI hit is detected.
        /// </summary>
        public LineProperties uiHitProperties
        {
            get => m_UIHitProperties;
            set => m_UIHitProperties = value;
        }

        [SerializeField]
        LineProperties m_UIPressHitProperties;

        /// <summary>
        /// Line properties when a valid UI press hit is detected.
        /// </summary>
        public LineProperties uiPressHitProperties
        {
            get => m_UIPressHitProperties;
            set => m_UIPressHitProperties = value;
        }

        [SerializeField]
        LineProperties m_SelectHitProperties;

        /// <summary>
        /// Line properties when a valid selection is detected.
        /// </summary>
        public LineProperties selectHitProperties
        {
            get => m_SelectHitProperties;
            set => m_SelectHitProperties = value;
        }

        [SerializeField]
        LineProperties m_HoverHitProperties;

        /// <summary>
        /// Line properties when a valid non-UI hit is detected.
        /// </summary>
        public LineProperties hoverHitProperties
        {
            get => m_HoverHitProperties;
            set => m_HoverHitProperties = value;
        }

        [SerializeField]
        bool m_RenderLineInWorldSpace = true;

        /// <summary>
        /// If true the line will be rendered in world space, otherwise it will be rendered in local space.
        /// Set this to false in the event that high speed locomotion causes some visual artifacts with the line renderer.
        /// </summary>
        public bool renderLineInWorldSpace
        {
            get => m_RenderLineInWorldSpace;
            set
            {
                m_RenderLineInWorldSpace = value;
                if (m_LineRenderer != null)
                    m_LineRenderer.useWorldSpace = value;
            }
        }

        [SerializeField]
        bool m_SwapMaterials;

        /// <summary>
        /// Indicates whether to swap the Line Renderer component's material for different states.
        /// </summary>
        public bool swapMaterials
        {
            get => m_SwapMaterials;
            set => m_SwapMaterials = value;
        }

        [SerializeField]
        Material m_BaseLineMaterial;

        /// <summary>
        /// Material to use in all cases other than when over 3D geometry that is not a valid interactable target.
        /// </summary>
        public Material baseLineMaterial
        {
            get => m_BaseLineMaterial;
            set => m_BaseLineMaterial = value;
        }

        [SerializeField]
        Material m_EmptyHitMaterial;

        /// <summary>
        /// Material to use when over 3D geometry that is not a valid interactable target.
        /// </summary>
        public Material emptyHitMaterial
        {
            get => m_EmptyHitMaterial;
            set => m_EmptyHitMaterial = value;
        }

        // Fallback length of the curve when the renderer becomes unstable. When the line length falls below this, a straight line is drawn instead.
        const float k_CurveFallbackLength = 0.06f;

        // Squared length when even a straight line fallback is unstable. When the squared line length falls below this, disable the line renderer.
        const float k_DisableSquaredLength = 0.01f * 0.01f;

        // We need 3 points on the fallback line, or the line renderer will become unstable if the alpha on the gradient is set to 0 on both ends.
        const int k_FallBackLinePointCount = 3;

        NativeArray<Vector3> m_InternalSamplePoints;
        NativeArray<Vector3> m_FallBackSamplePoints;

        Transform m_ParentTransform;

        float m_LastHitTime;
        float m_LengthToLastHit;
        float m_LineLength;
        int m_LastPosCount;
        float m_RenderLengthMultiplier;
        bool m_CanSwapMaterials;

        float m_LastLineStartWidth;
        float m_LastLineEndWidth;
        float m_EndPointTypeChangeTime;

        float m_LastBendRatio = 0.5f;

        bool m_UseCustomOrigin;
        EndPointType m_LastEndPointType = EndPointType.None;
        bool m_LastValidSelectState;
        Gradient m_LerpGradient;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Awake()
        {
            if (m_LineRenderer == null)
            {
                m_LineRenderer = GetComponentInChildren<LineRenderer>();
                if (m_LineRenderer == null)
                {
                    Debug.LogError($"Missing Line Renderer component on Curve Caster Visual Controller {this}.", this);
                    enabled = false;
                    return;
                }
            }

            if (curveInteractionDataProvider == null)
            {
                Debug.LogError($"Missing {typeof(ICurveInteractionDataProvider)} Disabling {this}.", this);
                enabled = false;
                return;
            }

            m_LineRenderer.useWorldSpace = m_RenderLineInWorldSpace;
            m_ParentTransform = transform.parent;

            m_FallBackSamplePoints = new NativeArray<Vector3>(k_FallBackLinePointCount, Allocator.Persistent);

            if (m_OverrideLineOrigin && m_LineOriginTransform == null)
                m_LineOriginTransform = transform;

            m_UseCustomOrigin = m_LineOriginTransform != null;
            m_CanSwapMaterials = m_SwapMaterials && m_EmptyHitMaterial != null && m_BaseLineMaterial != null;
            m_LastLineStartWidth = m_LineRenderer.startWidth;
            m_LastLineEndWidth = m_LineRenderer.endWidth;
            m_LerpGradient = m_LineRenderer.colorGradient;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDestroy()
        {
            if (m_FallBackSamplePoints.IsCreated)
                m_FallBackSamplePoints.Dispose();
            if (m_InternalSamplePoints.IsCreated)
                m_InternalSamplePoints.Dispose();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void LateUpdate()
        {
            var curveData = curveInteractionDataProvider;
            if (!curveData.isActive)
            {
                m_LineRenderer.enabled = false;
                return;
            }

            m_LineRenderer.enabled = true;

            ValidatePointCount();

            GetLineOriginAndDirection(out Vector3 worldOrigin, out Vector3 worldDirection);

            float validHitDistance = m_MaxVisualCurveDistance;
            EndPointType endPointType = GetEndpointInformation(worldOrigin, worldDirection, ref validHitDistance, out Vector3 worldEndPoint);

            float newLineDistance = UpdateTargetDistance(endPointType, validHitDistance, m_RestingVisualLineLength, m_MaxVisualCurveDistance,
                m_LineDynamicsMode == LineDynamicsMode.RetractOnHitLoss,
                m_RetractDelay, m_RetractDuration, m_ExtensionRate);

            // If our new line distance is less than the Target/newly found endpoint
            if (newLineDistance < validHitDistance)
            {
                // gets a new endpoint based on a shorter distance that initially determined
                GetAdjustedEndPointForMaxDistance(worldOrigin, worldEndPoint, newLineDistance, out var constrainedEndPoint);
                worldEndPoint = constrainedEndPoint;
            }

            if (CheckIfVisualStateChanged(endPointType, curveData.hasValidSelect))
                SwapMaterials(endPointType);

            DetermineOffsets(endPointType, newLineDistance, out float newOffsetStart, out float targetEndOffset);
            UpdateLineWidth(endPointType, newLineDistance);
            UpdateGradient(endPointType);
            UpdateLinePoints(endPointType, worldOrigin, worldEndPoint, worldDirection, newOffsetStart, targetEndOffset);
        }

        bool CheckIfVisualStateChanged(EndPointType newPointType, bool hasValidSelect)
        {
            if (newPointType == m_LastEndPointType && m_LastValidSelectState == hasValidSelect)
                return false;
            m_EndPointTypeChangeTime = Time.unscaledTime;
            m_LastEndPointType = newPointType;
            m_LastValidSelectState = hasValidSelect;
            return true;
        }

        void GetLineOriginAndDirection(out Vector3 worldOrigin, out Vector3 worldDirection)
        {
            if (m_UseCustomOrigin)
            {
                worldOrigin = m_LineOriginTransform.position;
                worldDirection = m_LineOriginTransform.forward;
            }
            else
            {
                worldOrigin = curveInteractionDataProvider.curveOrigin.position;
                worldDirection = curveInteractionDataProvider.curveOrigin.forward;
            }
        }

        EndPointType GetEndpointInformation(Vector3 worldOrigin, Vector3 worldDirection, ref float validHitDistance, out Vector3 endPoint)
        {
            var endPointType = curveInteractionDataProvider.TryGetCurveEndPoint(out var curveHitPoint, m_SnapToSelectedAttachIfAvailable, m_SnapToSnapVolumeIfAvailable);
            if (endPointType is EndPointType.AttachPoint or EndPointType.UI)
            {
                validHitDistance = math.distance(worldOrigin, curveHitPoint);
                endPoint = curveHitPoint;
            }
            else if (endPointType is EndPointType.EmptyCastHit or EndPointType.ValidCastHit)
            {
                AdjustCastHitEndPoint(worldOrigin, worldDirection, curveHitPoint, curveInteractionDataProvider.lastSamplePoint, out validHitDistance, out var adjustedEndPoint);
                endPoint = adjustedEndPoint;
            }
            else
            {
                endPoint = curveInteractionDataProvider.lastSamplePoint;
            }

            return endPointType;
        }

        void UpdateLinePoints(EndPointType endPointType, Vector3 worldOrigin, Vector3 worldEndPoint, Vector3 worldDirection, float startOffset = 0f, float endOffset = 0f, bool forceStraightLineFallback = false)
        {
            var float3TargetPoints = m_InternalSamplePoints.Reinterpret<float3>();
            float bendRatio = GetLineBendRatio(endPointType);
            bool shouldDrawCurve = forceStraightLineFallback || bendRatio < 1f;
            bool curveGenerated = false;

            Vector3 origin = m_RenderLineInWorldSpace ? worldOrigin : m_ParentTransform.InverseTransformPoint(worldOrigin);
            Vector3 endPoint = m_RenderLineInWorldSpace ? worldEndPoint : m_ParentTransform.InverseTransformPoint(worldEndPoint);

            if (shouldDrawCurve)
            {
                if (m_ComputeMidPointWithComplexCurves && TryGetMidPointFromCurveSamples(curveInteractionDataProvider, out var worldMidPoint))
                {
                    Vector3 midPoint = m_RenderLineInWorldSpace ? worldMidPoint : m_ParentTransform.InverseTransformPoint(worldMidPoint);

                    curveGenerated = CurveUtility.TryGenerateCubicBezierCurve(
                        m_VisualPointCount, origin, midPoint, endPoint, ref float3TargetPoints, k_CurveFallbackLength, startOffset, endOffset);
                }
                else
                {
                    Vector3 direction = m_RenderLineInWorldSpace ? worldDirection : m_ParentTransform.InverseTransformDirection(worldDirection);

                    curveGenerated = CurveUtility.TryGenerateCubicBezierCurve(
                        m_VisualPointCount, bendRatio, origin, direction, endPoint, ref float3TargetPoints, k_CurveFallbackLength, startOffset, endOffset);
                }
            }

            if (!curveGenerated)
            {
                var float3FallBackPoints = m_FallBackSamplePoints.Reinterpret<float3>();
                if (ComputeFallBackLine(origin, endPoint, startOffset, endOffset, ref float3FallBackPoints))
                    SetLinePositions(m_FallBackSamplePoints, k_FallBackLinePointCount);
                else
                    m_LineRenderer.enabled = false;
                return;
            }

            SetLinePositions(m_InternalSamplePoints, m_VisualPointCount);
        }

        static bool TryGetMidPointFromCurveSamples(in ICurveInteractionDataProvider curveInteractionDataProvider, out Vector3 midPoint)
        {
            var length = curveInteractionDataProvider.samplePoints.Length;
            if (length > 2)
            {
                var midPointIndex = length / 2;
                midPoint = curveInteractionDataProvider.samplePoints[midPointIndex];
                return true;
            }

            if (length == 2)
            {
                midPoint = (curveInteractionDataProvider.samplePoints[0] + curveInteractionDataProvider.samplePoints[1]) / 2f;
                return true;
            }

            midPoint = default;
            return false;
        }

        bool TryGetLineProperties(EndPointType endPointType, out LineProperties properties)
        {
            if (!m_CustomizeLinePropertiesForState)
            {
                properties = default;
                return false;
            }

            properties = endPointType switch
            {
                EndPointType.None => m_NoValidHitProperties,
                EndPointType.EmptyCastHit => m_NoValidHitProperties,
                EndPointType.ValidCastHit => curveInteractionDataProvider.hasValidSelect ? m_SelectHitProperties : m_HoverHitProperties,
                EndPointType.AttachPoint => m_SelectHitProperties,
                EndPointType.UI => curveInteractionDataProvider.hasValidSelect ? m_UIPressHitProperties : m_UIHitProperties,
                _ => m_NoValidHitProperties
            };
            return true;
        }

        float GetLineBendRatio(EndPointType endPointType)
        {
            if (!TryGetLineProperties(endPointType, out var properties))
                return 0.5f;
            if (!properties.smoothlyCurveLine)
                return 1f;

            if (m_LinePropertyAnimationSpeed > 0f)
            {
                m_LastBendRatio = Mathf.Lerp(m_LastBendRatio, properties.lineBendRatio, Time.unscaledDeltaTime * m_LinePropertyAnimationSpeed);
                return m_LastBendRatio;
            }

            return properties.lineBendRatio;
        }

        void DetermineOffsets(EndPointType endPointType, float lineDistance, out float startOffset, out float endOffset)
        {
            startOffset = m_CurveStartOffset;
            endOffset = m_CurveEndOffset;

            if (m_LineDynamicsMode != LineDynamicsMode.ExpandFromHitPoint)
                return;

            float targetEndOffset = m_CurveEndOffset;
            float smoothAmt = Time.unscaledDeltaTime * m_EndPointExpansionRate;
            var renderLength = lineDistance;

            float targetRenderLengthMultiplier = m_RenderLengthMultiplier;
            if (TryGetLineProperties(endPointType, out var properties))
            {
                if (properties.customizeExpandLineDrawPercent)
                    targetRenderLengthMultiplier = Mathf.Clamp01(1f - properties.expandModeLineDrawPercent);
            }
            else
            {
                if (endPointType == EndPointType.AttachPoint)
                    targetRenderLengthMultiplier = 0.25f;
                else if (endPointType == EndPointType.ValidCastHit)
                    targetRenderLengthMultiplier = 0.75f;
                else
                    targetRenderLengthMultiplier = 1f;
            }

            m_RenderLengthMultiplier = BurstLerpUtility.BezierLerp(m_RenderLengthMultiplier, targetRenderLengthMultiplier, smoothAmt);
            renderLength *= m_RenderLengthMultiplier;

            startOffset = Mathf.Max(renderLength - (targetEndOffset + 0.001f), startOffset);
            endOffset = targetEndOffset;
        }

        void UpdateLineWidth(EndPointType endPointType, float targetDistance)
        {
            if (!TryGetLineProperties(endPointType, out var properties) || !properties.adjustWidth)
                return;

            if (Mathf.Approximately(m_LastLineStartWidth, properties.starWidth) &&
                Mathf.Approximately(m_LastLineEndWidth, properties.endWidth))
                return;

            var targetStartWidth = properties.starWidth;

            var endWidthScaleFactor = properties.endWidthScaleDistanceFactor > 0f ? (1f + properties.endWidthScaleDistanceFactor * targetDistance / maxVisualCurveDistance) : 1f;
            var targetEndWidth = properties.endWidth * endWidthScaleFactor;

            if (m_LinePropertyAnimationSpeed > 0f)
            {
                if (Mathf.Abs(m_LastLineStartWidth - targetStartWidth) < 0.0001f &&
                    Mathf.Abs(m_LastLineEndWidth - targetEndWidth) < 0.0001f)
                {
                    m_LastLineStartWidth = targetStartWidth;
                    m_LastLineEndWidth = targetEndWidth;
                }
                else
                {
                    float tVal = Time.unscaledDeltaTime * m_LinePropertyAnimationSpeed;
                    m_LastLineStartWidth = Mathf.Lerp(m_LastLineStartWidth, targetStartWidth, tVal);
                    m_LastLineEndWidth = Mathf.Lerp(m_LastLineEndWidth, targetEndWidth, tVal);
                }
            }
            else
            {
                m_LastLineStartWidth = targetStartWidth;
                m_LastLineEndWidth = targetEndWidth;
            }

            m_LineRenderer.startWidth = m_LastLineStartWidth;
            m_LineRenderer.endWidth = m_LastLineEndWidth;
        }

        void UpdateGradient(EndPointType endPointType)
        {
            if (!TryGetLineProperties(endPointType, out var properties) || !properties.adjustGradient)
                return;

            var timeSinceLastChange = Time.unscaledTime - m_EndPointTypeChangeTime;
            if (m_LinePropertyAnimationSpeed > 0 && timeSinceLastChange < 1f)
                GradientUtility.Lerp(m_LerpGradient, properties.gradient, m_LerpGradient, Time.unscaledDeltaTime * m_LinePropertyAnimationSpeed);
            else
                GradientUtility.CopyGradient(properties.gradient, m_LerpGradient);

            m_LineRenderer.colorGradient = m_LerpGradient;
        }

        void SetLinePositions(NativeArray<Vector3> targetPoints, int numPoints)
        {
            if (numPoints != m_LastPosCount)
            {
                m_LineRenderer.positionCount = numPoints;
                m_LastPosCount = numPoints;
            }

            m_LineRenderer.SetPositions(targetPoints);
        }

        /// <summary>
        /// Updates the target line length based on various factors such as hit status, line retraction behavior,
        /// and distance constraints. Uses quadratic Bezier interpolation for non-linear adjustment of line length.
        /// </summary>
        float UpdateTargetDistance(EndPointType endPointType, float validHitDistance, float minLength, float maxLength, bool retractOnHitLoss, float retractionDelay, float retractionDuration, float curveExtensionRate)
        {
            float currentTime = Time.unscaledTime;
            if (endPointType != EndPointType.None)
            {
                if (endPointType != EndPointType.EmptyCastHit)
                    m_LastHitTime = currentTime;

                // If retracting line on empty hit, capture min of new hit distance or the line length from the previous frame.
                // Else, ensure new hit distance does not exceed max line length.
                if (!m_ExtendLineToEmptyHit && endPointType == EndPointType.EmptyCastHit)
                    m_LengthToLastHit = Mathf.Min(validHitDistance, m_LengthToLastHit);
                else
                    m_LengthToLastHit = Mathf.Min(validHitDistance, maxLength);

                // Ensure minimum length is returned and length is not zero
                m_LengthToLastHit = Mathf.Max(m_LengthToLastHit, minLength);

                // If current line length is longer than new target length, snap down to new target length
                if (m_LineLength > m_LengthToLastHit)
                {
                    m_LineLength = m_LengthToLastHit;
                    return m_LineLength;
                }
            }

            float timeSinceLastHit = currentTime - m_LastHitTime;

            // Retract line if time since last hit exceeds retraction delay
            if (retractOnHitLoss && timeSinceLastHit > retractionDelay)
            {
                float lineRetractionTimeElapsed = (timeSinceLastHit - retractionDelay);
                if (lineRetractionTimeElapsed > retractionDuration)
                {
                    m_LineLength = minLength;
                    return m_LineLength;
                }

                m_LineLength = BurstLerpUtility.BezierLerp(m_LengthToLastHit, minLength, Mathf.Clamp01(lineRetractionTimeElapsed / retractionDuration));
            }
            else
            {
                // Extend the line to the target length
                float targetLength = Mathf.Max(m_LengthToLastHit, minLength);
                m_LineLength = curveExtensionRate > 0 ? BurstLerpUtility.BezierLerp(m_LineLength, targetLength, Time.unscaledDeltaTime * curveExtensionRate) : targetLength;
            }

            return m_LineLength;
        }

        void SwapMaterials(EndPointType endPointType)
        {
            if (!m_CanSwapMaterials || !m_SwapMaterials)
                return;
            m_LineRenderer.sharedMaterial = endPointType == EndPointType.EmptyCastHit ? m_EmptyHitMaterial : m_BaseLineMaterial;
        }

        void ValidatePointCount()
        {
            bool isCreated = m_InternalSamplePoints.IsCreated;
            if (isCreated && m_InternalSamplePoints.Length == m_VisualPointCount)
                return;

            if (isCreated)
                m_InternalSamplePoints.Dispose();

            m_InternalSamplePoints = new NativeArray<Vector3>(m_VisualPointCount, Allocator.Persistent);
            m_LineRenderer.positionCount = m_VisualPointCount;
        }

#if BURST_PRESENT
        [BurstCompile]
#endif
        static void GetAdjustedEndPointForMaxDistance(in float3 origin, in float3 endPoint, float maxDistance, out float3 newEndPoint)
        {
            float3 normalizedDirection = math.normalize(endPoint - origin);
            newEndPoint = origin + normalizedDirection * maxDistance;
        }

#if BURST_PRESENT
        [BurstCompile]
#endif
        static void GetClosestPointOnLine(in float3 origin, in float3 direction, in float3 point, out float3 newPoint)
        {
            float3 toPoint = point - origin;

            float dotProduct = math.dot(toPoint, direction);
            float3 projectedVector = dotProduct * direction;

            newPoint = origin + projectedVector;
        }

#if BURST_PRESENT
        [BurstCompile]
#endif
        static void AdjustCastHitEndPoint(in float3 worldOrigin, in float3 worldDirection, in float3 hitEndPoint, in float3 sampleEndPoint, out float validHitDistance, out float3 endPoint)
        {
            GetClosestPointOnLine(worldOrigin, worldDirection, hitEndPoint, out float3 projectedPoint);

            validHitDistance = math.length(projectedPoint - worldOrigin);
            float3 direction = math.normalize(sampleEndPoint - worldOrigin);
            endPoint = worldOrigin + direction * validHitDistance;
        }

#if UNITY_2022_2_OR_NEWER && BURST_PRESENT
        [BurstCompile]
#endif
        static bool ComputeFallBackLine(in float3 curveOrigin, in float3 endPoint, float startOffset, float endOffset, ref NativeArray<float3> fallBackTargetPoints)
        {
            var originToEnd = endPoint - curveOrigin;

            // If the distance is too small or zero, the line is not stable enough to draw.
            // This also avoids a division by zero in the normalize function producing NaN points.
            var squaredLength = math.lengthsq(originToEnd);
            if (squaredLength < k_DisableSquaredLength)
                return false;

            // Normalize the direction vector, equivalent to math.normalize(originToEnd)
            var normalizedDirection = math.rsqrt(squaredLength) * originToEnd;

            // Use linear interpolation between curveOrigin and endPoint to draw a straight line
            float3 startPoint = curveOrigin + (normalizedDirection * startOffset);
            float3 endPointOffset = endPoint - (normalizedDirection * endOffset);

            // Calculate direction vectors
            float3 directionToEnd = math.normalize(endPoint - startPoint);
            float3 directionToEndOffset = math.normalize(endPointOffset - startPoint);

            // Check if the offset end point is behind the start point using dot product, to determine if curve is reversed and invalid.
            if (math.dot(directionToEnd, directionToEndOffset) < 0f)
                return false;

            fallBackTargetPoints[0] = startPoint;
            fallBackTargetPoints[1] = math.lerp(startPoint, endPointOffset, 0.5f);
            fallBackTargetPoints[2] = endPointOffset;
            return true;
        }
    }
}
