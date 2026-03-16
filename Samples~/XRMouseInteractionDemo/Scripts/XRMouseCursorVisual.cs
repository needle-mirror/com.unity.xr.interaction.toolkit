using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.UI;

[Serializable]
internal class ScaleAnimationTarget
{
    public Transform target;
    [Space(5)]
    public Vector3 idleScale = Vector3.one;
    public Vector3 hoverScale = Vector3.one * 1.2f;
    public Vector3 selectScale = Vector3.one * 0.9f;

    [HideInInspector] public Vector3 startScale;
    [HideInInspector] public Vector3 currentScale = Vector3.one;
    [HideInInspector] public Vector3 targetScale = Vector3.one;
    [HideInInspector] public float animationProgress;
}

/// <summary>
/// Manages the visual representation of a mouse cursor in XR, including positioning, rotation, and scale animations.
/// Works in conjunction with a <see cref="NearFarInteractor"/> to position the cursor at raycast hit points
/// and provides visual feedback through scale animations during hover and select interactions.
/// </summary>
/// <remarks>
/// The cursor automatically adjusts its position and orientation based on the type of surface hit (UI, 3D objects, or attach points).
/// For UI elements, the cursor is slightly offset toward the camera to prevent z-fighting.
/// For 3D surfaces, the cursor aligns its rotation with the surface normal.
/// </remarks>
public class XRMouseCursorVisual : MonoBehaviour
{
    [Header("Cursor Settings")]
    [SerializeField]
    GameObject m_CursorOrigin;
    [SerializeField]
    GameObject m_CursorEndpoint;
    [SerializeField]
    NearFarInteractor m_NearFarInteractor;

    [Header("Scale Animation")]
    [SerializeField]
    bool m_EnableScaleAnimation = true;
    [SerializeField]
    ScaleAnimationTarget[] m_ScaleTargets;

    [Header("Animation Settings")]
    [Range(0.01f, 1f)]
    [SerializeField]
    float m_ScaleTransitionSpeed = 0.15f;
    [SerializeField]
    AnimationCurve m_ScaleAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    CurveVisualController m_CurveVisualController;
    bool m_IsHovering;
    bool m_IsSelecting;

    void Start()
    {
        if (m_NearFarInteractor != null)
        {
            m_CurveVisualController = m_NearFarInteractor.gameObject.GetComponentInChildren<CurveVisualController>();

            // Subscribe to interaction events
            m_NearFarInteractor.hoverEntered.AddListener(OnHoverEntered);
            m_NearFarInteractor.hoverExited.AddListener(OnHoverExited);
            m_NearFarInteractor.selectEntered.AddListener(OnSelectEntered);
            m_NearFarInteractor.selectExited.AddListener(OnSelectExited);

            // Subscribe to UI hover events
            m_NearFarInteractor.uiHoverEntered.AddListener(OnUIHoverEntered);
            m_NearFarInteractor.uiHoverExited.AddListener(OnUIHoverExited);
        }

        // Initialize scale targets
        if (m_ScaleTargets != null && m_ScaleTargets.Length > 0)
        {
            foreach (var scaleTarget in m_ScaleTargets)
            {
                if (scaleTarget.target != null)
                {
                    scaleTarget.currentScale = scaleTarget.idleScale;
                    scaleTarget.targetScale = scaleTarget.idleScale;
                    scaleTarget.target.localScale = scaleTarget.idleScale;
                }
            }
        }
    }

    void OnDestroy()
    {
        if (m_NearFarInteractor != null)
        {
            m_NearFarInteractor.hoverEntered.RemoveListener(OnHoverEntered);
            m_NearFarInteractor.hoverExited.RemoveListener(OnHoverExited);
            m_NearFarInteractor.selectEntered.RemoveListener(OnSelectEntered);
            m_NearFarInteractor.selectExited.RemoveListener(OnSelectExited);
            m_NearFarInteractor.uiHoverEntered.RemoveListener(OnUIHoverEntered);
            m_NearFarInteractor.uiHoverExited.RemoveListener(OnUIHoverExited);
        }
    }

    void Update()
    {
        if (m_EnableScaleAnimation && m_ScaleTargets != null && m_ScaleTargets.Length > 0)
        {
            AnimateScales();
        }
    }

    void OnHoverEntered(HoverEnterEventArgs args)
    {
        m_IsHovering = true;
        UpdateTargetScales();
    }

    void OnHoverExited(HoverExitEventArgs args)
    {
        m_IsHovering = false;
        UpdateTargetScales();
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        m_IsSelecting = true;
        UpdateTargetScales();
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        m_IsSelecting = false;
        UpdateTargetScales();
    }

    void OnUIHoverEntered(UIHoverEventArgs args)
    {
        m_IsHovering = true;
        UpdateTargetScales();
    }

    void OnUIHoverExited(UIHoverEventArgs args)
    {
        m_IsHovering = false;
        UpdateTargetScales();
    }

    void UpdateTargetScales()
    {
        if (!m_EnableScaleAnimation || m_ScaleTargets == null)
            return;

        // Priority: Select > Hover > Idle
        foreach (var scaleTarget in m_ScaleTargets)
        {
            if (scaleTarget.target == null)
                continue;

            Vector3 newTargetScale;

            if (m_IsSelecting)
            {
                newTargetScale = scaleTarget.selectScale;
            }
            else if (m_IsHovering)
            {
                newTargetScale = scaleTarget.hoverScale;
            }
            else
            {
                newTargetScale = scaleTarget.idleScale;
            }

            if (scaleTarget.targetScale != newTargetScale)
            {
                scaleTarget.startScale = scaleTarget.currentScale;
                scaleTarget.targetScale = newTargetScale;
                scaleTarget.animationProgress = 0f;
            }
        }
    }

    void AnimateScales()
    {
        foreach (var scaleTarget in m_ScaleTargets)
        {
            if (scaleTarget.target == null)
                continue;

            if ((scaleTarget.currentScale - scaleTarget.targetScale).sqrMagnitude > 0.00001f)
            {
                scaleTarget.animationProgress += Time.deltaTime / m_ScaleTransitionSpeed;
                scaleTarget.animationProgress = Mathf.Clamp01(scaleTarget.animationProgress);

                float curveValue = m_ScaleAnimationCurve.Evaluate(scaleTarget.animationProgress);
                scaleTarget.currentScale = Vector3.Lerp(scaleTarget.startScale, scaleTarget.targetScale, curveValue);

                scaleTarget.target.localScale = scaleTarget.currentScale;
            }
            else
            {
                scaleTarget.currentScale = scaleTarget.targetScale;
            }
        }
    }

    public void UpdateCursorPositionAndRotation(Vector3 center, Vector3 direction, Quaternion aimRotation, float currentRadius)
    {
        if (m_CursorEndpoint == null || m_NearFarInteractor == null)
            return;

        // Use local space transformations to keep cursor relative to XR Origin.
        // This is critical when teleporting - using world space transforms would cause
        // the cursor to remain at its absolute world position when the XR Origin moves,
        // breaking the visual relationship between the cursor and the player's perspective.
        m_CursorOrigin.transform.localPosition = center + direction * currentRadius;
        m_CursorOrigin.transform.localRotation = aimRotation;

        ICurveInteractionDataProvider curveProvider = m_NearFarInteractor;
        EndPointType endPointType = curveProvider.TryGetCurveEndPoint(out Vector3 endPoint, snapToSelectedAttachIfAvailable: true);

        Vector3 cursorPosition;

        // Determine cursor position based on endpoint type
        if (endPointType != EndPointType.None)
        {
            cursorPosition = endPoint;

            // If hovering UI, offset the cursor slightly toward the camera to ensure it renders in front
            if (endPointType is EndPointType.UI)
            {
                Vector3 rayOrigin = curveProvider.curveOrigin.position;
                Vector3 rayDirection = (cursorPosition - rayOrigin).normalized;
                cursorPosition -= rayDirection * 0.0025f; // Pull cursor 2.5mm toward camera
                m_CursorEndpoint.transform.position = cursorPosition;
            }
            else if (endPointType is EndPointType.AttachPoint or EndPointType.ValidCastHit)
            {
                // For attach points and valid hits, rotate the cursor to align with the surface normal
                // Use the curve end normal directly from the interactor (avoids redundant raycast)
                if (curveProvider.TryGetCurveEndNormal(out Vector3 hitNormal) != EndPointType.None)
                {
                    // Align cursor rotation with the surface normal
                    // The cursor's forward direction (-Z) will point along the normal
                    Quaternion normalRotation = Quaternion.LookRotation(-hitNormal);
                    m_CursorEndpoint.transform.rotation = normalRotation;

                    // Optional: slightly offset position along normal to prevent z-fighting
                    cursorPosition += hitNormal * 0.002f;
                    m_CursorEndpoint.transform.position = cursorPosition;
                }
            }
        }
        else
        {
            // When there's no endpoint, use the last sample point
            cursorPosition = center + direction * (currentRadius + m_CurveVisualController.restingVisualLineLength);
            // Use local space transformations to maintain correct positioning relative to XR Origin during teleportation
            m_CursorEndpoint.transform.localPosition = cursorPosition;
            m_CursorEndpoint.transform.localRotation = aimRotation;
        }
    }
}
