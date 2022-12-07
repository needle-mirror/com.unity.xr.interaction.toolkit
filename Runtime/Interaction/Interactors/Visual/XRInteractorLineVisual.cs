using System;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Get line points and hit point info for rendering.
    /// </summary>
    /// <seealso cref="XRInteractorLineVisual"/>
    /// <seealso cref="XRRayInteractor"/>
    public interface ILineRenderable
    {
        /// <summary>
        /// Gets the polygonal chain represented by a list of endpoints which form line segments to approximate the curve.
        /// Positions are in world space coordinates.
        /// </summary>
        /// <param name="linePoints">When this method returns, contains the sample points if successful.</param>
        /// <param name="numPoints">When this method returns, contains the number of sample points if successful.</param>
        /// <returns>Returns <see langword="true"/> if the sample points form a valid line, such as by having at least two points.
        /// Otherwise, returns <see langword="false"/>.</returns>
        bool GetLinePoints(ref Vector3[] linePoints, out int numPoints);

        /// <summary>
        /// Gets the current ray cast hit information, if a hit occurs. It returns the world position and the normal vector
        /// of the hit point, and its position in linePoints.
        /// </summary>
        /// <param name="position">When this method returns, contains the world position of the ray impact point if a hit occurred.</param>
        /// <param name="normal">When this method returns, contains the world normal of the surface the ray hit if a hit occurred.</param>
        /// <param name="positionInLine">When this method returns, contains the index of the sample endpoint within the list of points returned by <see cref="GetLinePoints"/>
        /// where a hit occurred. Otherwise, a value of <c>0</c> if no hit occurred.</param>
        /// <param name="isValidTarget">When this method returns, contains whether both a hit occurred and it is a valid target for interaction.</param>
        /// <returns>Returns <see langword="true"/> if a hit occurs, implying the ray cast hit information is valid. Otherwise, returns <see langword="false"/>.</returns>
        bool TryGetHitInfo(out Vector3 position, out Vector3 normal, out int positionInLine, out bool isValidTarget);
    }

    /// <summary>
    /// Interactor helper object aligns a <see cref="LineRenderer"/> with the Interactor.
    /// </summary>
    [AddComponentMenu("XR/Visual/XR Interactor Line Visual", 11)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LineRenderer))]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_LineVisual)]
    [HelpURL(XRHelpURLConstants.k_XRInteractorLineVisual)]
    public class XRInteractorLineVisual : MonoBehaviour, IXRCustomReticleProvider
    {
        const float k_MinLineWidth = 0.0001f;
        const float k_MaxLineWidth = 0.05f;

        [SerializeField, Range(k_MinLineWidth, k_MaxLineWidth)]
        float m_LineWidth = 0.02f;
        /// <summary>
        /// Controls the width of the line.
        /// </summary>
        public float lineWidth
        {
            get => m_LineWidth;
            set
            {
                m_LineWidth = value;
                m_PerformSetup = true;
            }
        }

        [SerializeField]
        bool m_OverrideInteractorLineLength = true;
        /// <summary>
        /// A boolean value that controls which source Unity uses to determine the length of the line.
        /// Set to <see langword="true"/> to use the Line Length set by this behavior.
        /// Set to <see langword="false"/> to have the length of the line determined by the Interactor.
        /// </summary>
        /// <seealso cref="lineLength"/>
        public bool overrideInteractorLineLength
        {
            get => m_OverrideInteractorLineLength;
            set => m_OverrideInteractorLineLength = value;
        }

        [SerializeField]
        float m_LineLength = 10f;
        /// <summary>
        /// Controls the length of the line when overriding.
        /// </summary>
        /// <seealso cref="overrideInteractorLineLength"/>
        public float lineLength
        {
            get => m_LineLength;
            set => m_LineLength = value;
        }

        [SerializeField]
        AnimationCurve m_WidthCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        /// <summary>
        /// Controls the relative width of the line from start to end.
        /// </summary>
        public AnimationCurve widthCurve
        {
            get => m_WidthCurve;
            set
            {
                m_WidthCurve = value;
                m_PerformSetup = true;
            }
        }

        [SerializeField]
        Gradient m_ValidColorGradient = new Gradient
        {
            colorKeys = new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) },
        };
        /// <summary>
        /// Controls the color of the line as a gradient from start to end to indicate a valid state.
        /// </summary>
        public Gradient validColorGradient
        {
            get => m_ValidColorGradient;
            set => m_ValidColorGradient = value;
        }

        [SerializeField]
        Gradient m_InvalidColorGradient = new Gradient
        {
            colorKeys = new[] { new GradientColorKey(Color.red, 0f), new GradientColorKey(Color.red, 1f) },
            alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) },
        };
        /// <summary>
        /// Controls the color of the line as a gradient from start to end to indicate an invalid state.
        /// </summary>
        public Gradient invalidColorGradient
        {
            get => m_InvalidColorGradient;
            set => m_InvalidColorGradient = value;
        }

        [SerializeField]
        Gradient m_BlockedColorGradient = new Gradient
        {
            colorKeys = new[] { new GradientColorKey(Color.yellow, 0f), new GradientColorKey(Color.yellow, 1f) },
            alphaKeys = new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) },
        };
        /// <summary>
        /// Controls the color of the line as a gradient from start to end to indicate a state where the interactor has
        /// a valid target but selection is blocked.
        /// </summary>
        public Gradient blockedColorGradient
        {
            get => m_BlockedColorGradient;
            set => m_BlockedColorGradient = value;
        }

        [SerializeField]
        bool m_TreatSelectionAsValidState;
        /// <summary>
        /// Forces the use of valid state visuals while the interactor is selecting an interactable, whether or not the Interactor has any valid targets.
        /// </summary>
        /// <seealso cref="validColorGradient"/>
        public bool treatSelectionAsValidState
        {
            get => m_TreatSelectionAsValidState;
            set => m_TreatSelectionAsValidState = value;
        }

        [SerializeField]
        bool m_SmoothMovement;
        /// <summary>
        /// Controls whether the rendered segments will be delayed from and smoothly follow the target segments.
        /// </summary>
        /// <seealso cref="followTightness"/>
        /// <seealso cref="snapThresholdDistance"/>
        public bool smoothMovement
        {
            get => m_SmoothMovement;
            set => m_SmoothMovement = value;
        }

        [SerializeField]
        float m_FollowTightness = 10f;
        /// <summary>
        /// Controls the speed that the rendered segments follow the target segments when Smooth Movement is enabled.
        /// </summary>
        /// <seealso cref="smoothMovement"/>
        /// <seealso cref="snapThresholdDistance"/>
        public float followTightness
        {
            get => m_FollowTightness;
            set => m_FollowTightness = value;
        }

        [SerializeField]
        float m_SnapThresholdDistance = 10f;
        /// <summary>
        /// Controls the threshold distance between line points at two consecutive frames to snap rendered segments to target segments when Smooth Movement is enabled.
        /// </summary>
        /// <seealso cref="smoothMovement"/>
        /// <seealso cref="followTightness"/>
        public float snapThresholdDistance
        {
            get => m_SnapThresholdDistance;
            set => m_SnapThresholdDistance = value;
        }

        [SerializeField]
        GameObject m_Reticle;
        /// <summary>
        /// Stores the reticle that appears at the end of the line when it is valid.
        /// </summary>
        /// <remarks>
        /// Unity will instantiate it while playing when it is a Prefab asset.
        /// </remarks>
        public GameObject reticle
        {
            get => m_Reticle;
            set
            {
                m_Reticle = value;
                if (Application.isPlaying)
                    SetupReticle();
            }
        }

        [SerializeField]
        GameObject m_BlockedReticle;
        /// <summary>
        /// Stores the reticle that appears at the end of the line when the interactor has a valid target but selection is blocked.
        /// </summary>
        /// <remarks>
        /// Unity will instantiate it while playing when it is a Prefab asset.
        /// </remarks>
        public GameObject blockedReticle
        {
            get => m_BlockedReticle;
            set
            {
                m_BlockedReticle = value;
                if (Application.isPlaying)
                    SetupBlockedReticle();
            }
        }

        [SerializeField]
        bool m_StopLineAtFirstRaycastHit = true;
        /// <summary>
        /// Controls whether this behavior always cuts the line short at the first ray cast hit, even when invalid.
        /// </summary>
        /// <remarks>
        /// The line will always stop short at valid targets, even if this property is set to false.
        /// If you wish this line to pass through valid targets, they must be placed on a different layer.
        /// <see langword="true"/> means to do the same even when pointing at an invalid target.
        /// <see langword="false"/> means the line will continue to the configured line length.
        /// </remarks>
        public bool stopLineAtFirstRaycastHit
        {
            get => m_StopLineAtFirstRaycastHit;
            set => m_StopLineAtFirstRaycastHit = value;
        }

        [SerializeField]
        bool m_StopLineAtSelection;
        /// <summary>
        /// Controls whether the line will stop at the attach point of the closest interactable selected by the interactor, if there is one.
        /// </summary>
        public bool stopLineAtSelection
        {
            get => m_StopLineAtSelection;
            set => m_StopLineAtSelection = value;
        }
        
        [SerializeField]
        bool m_SnapEndpointIfAvailable = true;
        /// <summary>
        /// Controls whether the visualized line will snap endpoint if the ray hits a XRInteractableSnapVolume.
        /// </summary>
        /// <remarks>
        /// Currently snapping only works with an <see cref="XRRayInteractor"/>.
        /// </remarks>
        public bool snapEndpointIfAvailable
        {
            get => m_SnapEndpointIfAvailable;
            set => m_SnapEndpointIfAvailable = value;
        }
        
        Vector3 m_ReticlePos;
        Vector3 m_ReticleNormal;
        int m_EndPositionInLine;

        bool m_SnapCurve = true;
        bool m_PerformSetup;
        GameObject m_ReticleToUse;

        LineRenderer m_LineRenderer;

        // interface to get target point
        ILineRenderable m_LineRenderable;
        IXRSelectInteractor m_LineRenderableAsSelectInteractor;
        XRBaseInteractor m_LineRenderableAsBaseInteractor;
        XRRayInteractor m_LineRenderableAsRayInteractor;

        // reusable lists of target points
        Vector3[] m_TargetPoints;
        int m_NoTargetPoints = -1;

        // reusable lists of rendered points
        Vector3[] m_RenderPoints;
        int m_NoRenderPoints = -1;

        // reusable lists of rendered points to smooth movement
        Vector3[] m_PreviousRenderPoints;
        int m_NoPreviousRenderPoints = -1;

        readonly Vector3[] m_ClearArray = { Vector3.zero, Vector3.zero };

        GameObject m_CustomReticle;
        bool m_CustomReticleAttached;
        
        // Snapping 
        bool m_Snapping;
        XRInteractableSnapVolume m_XRInteractableSnapVolume;
        int m_NumberOfSegmentsForBendableLine = 20;

        // List of raycast points from m_LineRenderable
        Vector3[] m_LineRenderablePoints = Array.Empty<Vector3>();

        // Most recent hit information
        Vector3 m_CurrentHitPoint;
        bool m_HasHitInfo;
        bool m_ValidHit;
        
        // The position at which we want to render the end point (used for bending ray visuals)
        Vector3 m_CurrentRenderEndpoint;
        // Previously hit collider 
        Collider m_PreviousCollider;

        XROrigin m_XROrigin;

        /// <summary>
        /// Cached reference to an <see cref="XROrigin"/> found with <see cref="Object.FindObjectOfType{Type}()"/>.
        /// </summary>
        static XROrigin s_XROriginCache;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Reset()
        {
            // Don't need to do anything; method kept for backwards compatibility.
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnValidate()
        {
            if (Application.isPlaying)
                UpdateSettings();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Awake()
        {
            m_LineRenderable = GetComponent<ILineRenderable>();
            if (m_LineRenderable != null)
            {
                m_LineRenderableAsBaseInteractor = m_LineRenderable as XRBaseInteractor;
                m_LineRenderableAsSelectInteractor = m_LineRenderable as IXRSelectInteractor;
                m_LineRenderableAsRayInteractor = m_LineRenderable as XRRayInteractor;
            }

            FindXROrigin();
            SetupReticle();
            SetupBlockedReticle();
            ClearLineRenderer();
            UpdateSettings();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            m_SnapCurve = true;
            if (m_ReticleToUse != null)
            {
                m_ReticleToUse.SetActive(false);
                m_ReticleToUse = null;
            }

            Application.onBeforeRender += OnBeforeRenderLineVisual;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            if (m_LineRenderer != null)
                m_LineRenderer.enabled = false;

            if (m_ReticleToUse != null)
            {
                m_ReticleToUse.SetActive(false);
                m_ReticleToUse = null;
            }

            Application.onBeforeRender -= OnBeforeRenderLineVisual;
        }

        void ClearLineRenderer()
        {
            if (TryFindLineRenderer())
            {
                m_LineRenderer.SetPositions(m_ClearArray);
                m_LineRenderer.positionCount = 0;
            }
        }

        [BeforeRenderOrder(XRInteractionUpdateOrder.k_BeforeRenderLineVisual)]
        void OnBeforeRenderLineVisual()
        {
            UpdateLineVisual();
        }

        /// <summary>
        /// Tries to get the hit info from the current <see cref="ILineRenderable"/> and checks for any <see cref="XRInteractableSnapVolume"/> collisions.  
        /// </summary>
        /// <returns>Returns whether or not we have a valid hit in this current frame.</returns>
        bool UpdateCurrentHitInfo()
        {
            m_LineRenderable.GetLinePoints(ref m_LineRenderablePoints, out _);

            Collider hitCollider = null;
            m_Snapping = false;
            
            if (m_LineRenderablePoints.Length < 1)
                return false;

            if (m_LineRenderable.TryGetHitInfo(out m_CurrentHitPoint, out m_ReticleNormal, out m_EndPositionInLine, out m_ValidHit))
            {
                m_HasHitInfo = true;
                m_CurrentRenderEndpoint = m_CurrentHitPoint;
                if (m_ValidHit && m_SnapEndpointIfAvailable && m_LineRenderableAsRayInteractor != null && !m_LineRenderableAsRayInteractor.hasSelection)
                {
                    // When hovering a new collider, check if it has a specified snapping volume, if it does then get the closest point on it
                    if (m_LineRenderableAsRayInteractor.TryGetCurrent3DRaycastHit(out var raycastHit, out _))
                        hitCollider = raycastHit.collider;

                    if (hitCollider != m_PreviousCollider && hitCollider != null)
                        m_LineRenderableAsBaseInteractor.interactionManager.TryGetInteractableForCollider(hitCollider, out _, out m_XRInteractableSnapVolume);

                    if (m_XRInteractableSnapVolume != null)
                    {
                        m_CurrentRenderEndpoint = m_XRInteractableSnapVolume.GetClosestPoint(m_CurrentRenderEndpoint);
                        m_EndPositionInLine = m_NumberOfSegmentsForBendableLine - 1; // Override hit index because we're going to use a custom line where the hit point is the end
                        m_Snapping = true;
                    }
                }
            }
            else
            {
                m_CurrentRenderEndpoint = (m_LineRenderablePoints.Length > 0) ? m_LineRenderablePoints[m_LineRenderablePoints.Length - 1] : Vector3.zero;
            }

            if (hitCollider == null)
                m_XRInteractableSnapVolume = null;

            m_PreviousCollider = hitCollider;
            return m_ValidHit;
        }

        /// <summary>
        /// Calculates the target render points based on the targeted snapped endpoint and the actual position of the raycast line.  
        /// </summary>
        void CalculateSnapRenderPoints()
        {
            var startPosition = m_LineRenderablePoints.Length > 0 ? m_LineRenderablePoints[0] : Vector3.zero;
            var forward = Vector3.Normalize(m_CurrentHitPoint - startPosition);
            var straightLineEndPoint = startPosition + forward * Vector3.Distance(m_CurrentRenderEndpoint, startPosition);

            var normalizedPointValue = 0f;
            var increment = 1f / (m_NoTargetPoints - 1);
            
            for (var i = 0; i < m_NoTargetPoints; i++)
            {
                var manipToEndPoint = Vector3.LerpUnclamped(startPosition, m_CurrentRenderEndpoint, normalizedPointValue);
                var manipToAnchor = Vector3.LerpUnclamped(startPosition, straightLineEndPoint, normalizedPointValue);
                m_TargetPoints[i] = Vector3.LerpUnclamped(manipToAnchor, manipToEndPoint, normalizedPointValue);
                normalizedPointValue += increment;
            }
        }
        
        internal void UpdateLineVisual()
        {
            UpdateCurrentHitInfo();

            if (m_PerformSetup)
            {
                UpdateSettings();
                m_PerformSetup = false;
            }

            if (m_LineRenderer == null)
                return;

            if (m_LineRenderer.useWorldSpace && m_XROrigin != null)
            {
                // Update line width with user scale
                var xrOrigin = m_XROrigin.Origin;
                var userScale = xrOrigin != null ? xrOrigin.transform.localScale.x : 1f;
                m_LineRenderer.widthMultiplier = userScale * Mathf.Clamp(m_LineWidth, k_MinLineWidth, k_MaxLineWidth);
            }
            
            if (m_LineRenderable == null)
            {
                m_LineRenderer.enabled = false;
                return;
            }

            if (m_LineRenderableAsBaseInteractor != null &&
                m_LineRenderableAsBaseInteractor.disableVisualsWhenBlockedInGroup &&
                m_LineRenderableAsBaseInteractor.IsBlockedByInteractionWithinGroup())
            {
                m_LineRenderer.enabled = false;
                return;
            }

            m_NoRenderPoints = 0;

            // Get all the line sample points from the ILineRenderable interface
            if (!m_LineRenderable.GetLinePoints(ref m_TargetPoints, out m_NoTargetPoints))
            {
                m_LineRenderer.enabled = false;
                ClearLineRenderer();
                return;
            }

            // If we're snapping, override the target points from the m_LineRenderable and use custom point data
            if (m_Snapping)
            {
                m_NoTargetPoints = m_NumberOfSegmentsForBendableLine;
                if (m_TargetPoints == null || m_TargetPoints.Length < m_NumberOfSegmentsForBendableLine)
                    m_TargetPoints = new Vector3[m_NumberOfSegmentsForBendableLine];
                CalculateSnapRenderPoints();
            }
            
            // Sanity check.
            if (m_TargetPoints == null ||
                m_TargetPoints.Length == 0 ||
                m_NoTargetPoints == 0 ||
                m_NoTargetPoints > m_TargetPoints.Length)
            {
                m_LineRenderer.enabled = false;
                ClearLineRenderer();
                return;
            }

            // Make sure we have the correct sized arrays for everything.
            if (m_RenderPoints == null || m_RenderPoints.Length < m_NoTargetPoints)
            {
                m_RenderPoints = new Vector3[m_NoTargetPoints];
                m_PreviousRenderPoints = new Vector3[m_NoTargetPoints];
                m_NoRenderPoints = 0;
                m_NoPreviousRenderPoints = 0;
            }

            // Unchanged
            // If there is a big movement (snap turn, teleportation), snap the curve
            if (m_NoPreviousRenderPoints != m_NoTargetPoints)
            {
                m_SnapCurve = true;
            }
            else
            {
                // Compare the two endpoints of the curve, as that will have the largest delta.
                if (m_PreviousRenderPoints != null &&
                    m_NoPreviousRenderPoints > 0 &&
                    m_NoPreviousRenderPoints <= m_PreviousRenderPoints.Length &&
                    m_TargetPoints != null &&
                    m_NoTargetPoints > 0 &&
                    m_NoTargetPoints <= m_TargetPoints.Length)
                {
                    var prevPointIndex = m_NoPreviousRenderPoints - 1;
                    var currPointIndex = m_NoTargetPoints - 1;
                    if (Vector3.Distance(m_PreviousRenderPoints[prevPointIndex], m_TargetPoints[currPointIndex]) > m_SnapThresholdDistance)
                    {
                        m_SnapCurve = true;
                    }
                }
            }

            // If the line hits, insert reticle position into the list for smoothing.
            // Remove the last point in the list to keep the number of points consistent.
            if (m_HasHitInfo)
            {
                m_ReticlePos = m_CurrentRenderEndpoint;
            
                // End the line at the current hit point.
                if ((m_ValidHit || m_StopLineAtFirstRaycastHit) && m_EndPositionInLine > 0 && m_EndPositionInLine < m_NoTargetPoints)
                {
                    // The hit position might not lie within the line segment, for example if a sphere cast is used, so use a point projected onto the
                    // segment so that the endpoint is continuous with the rest of the curve.
                    var lastSegmentStartPoint = m_TargetPoints[m_EndPositionInLine - 1];
                    var lastSegmentEndPoint = m_TargetPoints[m_EndPositionInLine];
                    var lastSegment = lastSegmentEndPoint - lastSegmentStartPoint;
                    var projectedHitSegment = Vector3.Project(m_ReticlePos - lastSegmentStartPoint, lastSegment);

                    // Don't bend the line backwards
                    if (Vector3.Dot(projectedHitSegment, lastSegment) < 0)
                        projectedHitSegment = Vector3.zero;

                    m_ReticlePos = lastSegmentStartPoint + projectedHitSegment;
                    m_TargetPoints[m_EndPositionInLine] = m_ReticlePos;
                    m_NoTargetPoints = m_EndPositionInLine + 1;
                }
            }

            // Stop line if there is a selection 
            var hasSelection = m_LineRenderableAsSelectInteractor != null && m_LineRenderableAsSelectInteractor.hasSelection;
            if (m_StopLineAtSelection && hasSelection)
            {
                // Use the selected interactable closest to the start of the line.
                var interactablesSelected = m_LineRenderableAsSelectInteractor.interactablesSelected;
                var firstPoint = m_TargetPoints[0];
                var closestEndPoint = m_LineRenderableAsSelectInteractor.GetAttachTransform(interactablesSelected[0]).position;
                var closestSqDistance = Vector3.SqrMagnitude(closestEndPoint - firstPoint);
                for (var i = 1; i < interactablesSelected.Count; i++)
                {
                    var endPoint = m_LineRenderableAsSelectInteractor.GetAttachTransform(interactablesSelected[i]).position;
                    var sqDistance = Vector3.SqrMagnitude(endPoint - firstPoint);
                    if (sqDistance < closestSqDistance)
                    {
                        closestEndPoint = endPoint;
                        closestSqDistance = sqDistance;
                    }
                }

                // Only stop at selection if it is closer than the current end point.
                var currentEndSqDistance = Vector3.SqrMagnitude(m_TargetPoints[m_EndPositionInLine] - firstPoint);
                if (closestSqDistance < currentEndSqDistance || m_EndPositionInLine == 0)
                {
                    // Find out where the selection point belongs in the line points. Use the closest target point.
                    var endPositionForSelection = 1;
                    var sqDistanceFromEndPoint = Vector3.SqrMagnitude(m_TargetPoints[endPositionForSelection] - closestEndPoint);
                    for (var i = 2; i < m_NoTargetPoints; i++)
                    {
                        var sqDistance = Vector3.SqrMagnitude(m_TargetPoints[i] - closestEndPoint);
                        if (sqDistance < sqDistanceFromEndPoint)
                        {
                            endPositionForSelection = i;
                            sqDistanceFromEndPoint = sqDistance;
                        }
                        else
                        {
                            break;
                        }
                    }

                    m_EndPositionInLine = endPositionForSelection;
                    m_NoTargetPoints = m_EndPositionInLine + 1;
                    m_ReticlePos = closestEndPoint;
                    m_ReticleNormal = Vector3.Normalize(m_TargetPoints[m_EndPositionInLine - 1] - m_ReticlePos);
                    m_TargetPoints[m_EndPositionInLine] = m_ReticlePos;
                }
            }

            if (m_SmoothMovement && (m_NoPreviousRenderPoints == m_NoTargetPoints) && !m_SnapCurve)
            {
                // Smooth movement by having render points follow target points
                var length = 0f;
                var maxRenderPoints = m_RenderPoints.Length;
                for (var i = 0; i < m_NoTargetPoints && m_NoRenderPoints < maxRenderPoints; ++i)
                {
                    var smoothPoint = Vector3.Lerp(m_PreviousRenderPoints[i], m_TargetPoints[i], m_FollowTightness * Time.deltaTime);

                    if (m_OverrideInteractorLineLength)
                    {
                        if (m_NoRenderPoints > 0 && m_RenderPoints.Length > 0)
                        {
                            var segLength = Vector3.Distance(m_RenderPoints[m_NoRenderPoints - 1], smoothPoint);
                            length += segLength;
                            if (length > m_LineLength)
                            {
                                var delta = length - m_LineLength;
                                // Re-project final point to match the desired length
                                smoothPoint = Vector3.Lerp(m_RenderPoints[m_NoRenderPoints - 1], smoothPoint, delta / segLength);
                                m_RenderPoints[m_NoRenderPoints] = smoothPoint;
                                m_NoRenderPoints++;
                                break;
                            }
                        }

                        m_RenderPoints[m_NoRenderPoints] = smoothPoint;
                        m_NoRenderPoints++;
                    }
                    else
                    {
                        m_RenderPoints[m_NoRenderPoints] = smoothPoint;
                        m_NoRenderPoints++;
                    }
                }
            }
            else
            {
                if (m_OverrideInteractorLineLength)
                {
                    var length = 0f;
                    var maxRenderPoints = m_RenderPoints.Length;
                    for (var i = 0; i < m_NoTargetPoints && m_NoRenderPoints < maxRenderPoints; ++i)
                    {
                        var newPoint = m_TargetPoints[i];
                        if (m_NoRenderPoints > 0 && m_RenderPoints.Length > 0)
                        {
                            var segLength = Vector3.Distance(m_RenderPoints[m_NoRenderPoints - 1], newPoint);
                            length += segLength;
                            if (length > m_LineLength)
                            {
                                var delta = length - m_LineLength;
                                // Re-project final point to match the desired length
                                var resolvedPoint = Vector3.Lerp(m_RenderPoints[m_NoRenderPoints - 1], newPoint, 1-(delta / segLength));
                                m_RenderPoints[m_NoRenderPoints] = resolvedPoint;
                                m_NoRenderPoints++;
                                break;
                            }
                        }

                        m_RenderPoints[m_NoRenderPoints] = newPoint;
                        m_NoRenderPoints++;
                    }
                }
                else
                {
                    Array.Copy(m_TargetPoints, m_RenderPoints, m_NoTargetPoints);
                    m_NoRenderPoints = m_NoTargetPoints;
                }
            }

            // When a straight line has only two points and color gradients have more than two keys,
            // interpolate points between the two points to enable better color gradient effects.
            if (m_ValidHit || m_TreatSelectionAsValidState && hasSelection)
            {
                // Use regular valid state visuals unless we are hovering and selection is blocked.
                // We use regular valid state visuals if not hovering because the blocked state does not apply
                // (e.g. we could have a valid target that is UI and therefore not hoverable or selectable as an interactable).
                var useBlockedVisuals = false;
                if (!hasSelection && m_LineRenderableAsBaseInteractor != null && m_LineRenderableAsBaseInteractor.hasHover)
                {
                    var interactionManager = m_LineRenderableAsBaseInteractor.interactionManager;
                    var canSelectSomething = false;
                    foreach (var interactable in m_LineRenderableAsBaseInteractor.interactablesHovered)
                    {
                        if (interactable is IXRSelectInteractable selectInteractable && interactionManager.IsSelectPossible(m_LineRenderableAsBaseInteractor, selectInteractable))
                        {
                            canSelectSomething = true;
                            break;
                        }
                    }

                    useBlockedVisuals = !canSelectSomething;
                }

                m_LineRenderer.colorGradient = useBlockedVisuals ? m_BlockedColorGradient : m_ValidColorGradient;
                // Set reticle position and show reticle
                var previouslyUsedReticle = m_ReticleToUse;
                var validStateReticle = useBlockedVisuals ? m_BlockedReticle : m_Reticle;
                m_ReticleToUse = m_CustomReticleAttached ? m_CustomReticle : validStateReticle;
                if (previouslyUsedReticle != null && previouslyUsedReticle != m_ReticleToUse)
                    previouslyUsedReticle.SetActive(false);

                if (m_ReticleToUse != null)
                {
                    m_ReticleToUse.transform.position = m_ReticlePos;
                    var hoverInteractor = m_LineRenderable as IXRHoverInteractor;
                    if (hoverInteractor?.GetOldestInteractableHovered() is IXRReticleDirectionProvider reticleDirectionProvider)
                    {
                        reticleDirectionProvider.GetReticleDirection(hoverInteractor, m_ReticleNormal, out var reticleUp, out var reticleForward);
                        if (reticleForward.HasValue)
                            m_ReticleToUse.transform.rotation = Quaternion.LookRotation(reticleForward.Value, reticleUp);
                        else
                            m_ReticleToUse.transform.up = reticleUp;
                    }
                    else
                    {
                        m_ReticleToUse.transform.up = m_ReticleNormal;
                    }

                    m_ReticleToUse.SetActive(true);
                }
            }
            else
            {
                m_LineRenderer.colorGradient = m_InvalidColorGradient;
                if (m_ReticleToUse != null)
                {
                    m_ReticleToUse.SetActive(false);
                    m_ReticleToUse = null;
                }
            }

            if (m_NoRenderPoints >= 2)
            {
                m_LineRenderer.enabled = true;
                m_LineRenderer.positionCount = m_NoRenderPoints;
                m_LineRenderer.SetPositions(m_RenderPoints);
            }
            else
            {
                m_LineRenderer.enabled = false;
                ClearLineRenderer();
                return;
            }

            // Update previous points
            Array.Copy(m_RenderPoints, m_PreviousRenderPoints, m_NoRenderPoints);
            m_NoPreviousRenderPoints = m_NoRenderPoints;
            m_SnapCurve = false;
        }

        void UpdateSettings()
        {
            if (TryFindLineRenderer())
            {
                m_LineRenderer.widthMultiplier =  Mathf.Clamp(m_LineWidth, k_MinLineWidth, k_MaxLineWidth);
                m_LineRenderer.widthCurve = m_WidthCurve;
                m_SnapCurve = true;
            }
        }

        bool TryFindLineRenderer()
        {
            m_LineRenderer = GetComponent<LineRenderer>();
            if (m_LineRenderer == null)
            {
                Debug.LogWarning("No Line Renderer found for Interactor Line Visual.", this);
                enabled = false;
                return false;
            }
            return true;
        }

        void FindXROrigin()
        {
            if (m_XROrigin != null)
                return;

            if (s_XROriginCache == null)
                s_XROriginCache = FindObjectOfType<XROrigin>();

            m_XROrigin = s_XROriginCache;
        }

        void SetupReticle()
        {
            if (m_Reticle == null)
                return;

            // Instantiate if the reticle is a Prefab asset rather than a scene GameObject
            if (!m_Reticle.scene.IsValid())
                m_Reticle = Instantiate(m_Reticle);

            m_Reticle.SetActive(false);
        }

        void SetupBlockedReticle()
        {
            if (m_BlockedReticle == null)
                return;

            // Instantiate if the reticle is a Prefab asset rather than a scene GameObject
            if (!m_BlockedReticle.scene.IsValid())
                m_BlockedReticle = Instantiate(m_BlockedReticle);

            m_BlockedReticle.SetActive(false);
        }

        /// <inheritdoc />
        public bool AttachCustomReticle(GameObject reticleInstance)
        {
            m_CustomReticle = reticleInstance;
            m_CustomReticleAttached = true;
            return true;
        }

        /// <inheritdoc />
        public bool RemoveCustomReticle()
        {
            m_CustomReticle = null;
            m_CustomReticleAttached = false;
            return true;
        }
    }
}
