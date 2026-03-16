using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;
#if OPEN_XR_1_17_OR_NEWER
using UnityEngine.XR.OpenXR.Features.Interactions;
#endif
#if UNITY_EDITOR
#endif

/// <summary>
/// Component that manages mouse input from Android XR devices for UI interaction.
/// Handles mouse aim position, rotation, scroll input, and click actions,
/// dynamically adjusting cursor radius based on scroll input while preventing
/// scroll interaction when hovering over scrollable UI elements.
/// </summary>
/// <remarks>
/// <para>
/// This component requires Unity 6 or newer and OpenXR Plugin 1.17.0 or newer with
/// the Android Mouse Interaction Profile enabled in OpenXR settings.
/// </para>
/// <para>
/// The component automatically deactivates itself if the Android Mouse Interaction Profile
/// is not available or if running on Unity versions prior to 6.0.
/// </para>
/// </remarks>
public class XRMousePointer : MonoBehaviour
{

    // On Unity versions before 6000.0 these warnings will show up in the console if we don't suppress them.
#pragma warning disable CS0168
#pragma warning disable CS0414
    [SerializeField]
    [Tooltip("Default distance from the aim position center where the cursor is displayed.")]
    float m_InitialRadius = 0.5f;
#pragma warning disable CS0414
#pragma warning restore CS0168

    [SerializeField]
    [Tooltip("Reference to the NearFarInteractor component used for UI interaction detection.")]
    NearFarInteractor m_NearFarInteractor;

    [SerializeField]
    [Tooltip("Reference to the XRMouseCursorVisual component that handles the visual cursor display.")]
    XRMouseCursorVisual m_CursorVisual;

#pragma warning disable CS0168
#pragma warning disable CS0414
    [SerializeField]
    [Range(0.0f, 1.0f)]
    [Tooltip("Speed multiplier for scroll input applied to the cursor radius adjustment. Range is 0.0 to 1.0.")]
    float m_ScrollSpeed = 1.0f;
#pragma warning disable CS0414
#pragma warning restore CS0168

    [SerializeField]
    [Tooltip("XRInputValueReader for Vector2 scroll input from the Android Mouse Interaction Profile.")]
    XRInputValueReader<Vector2> m_ScrollAction;

    [SerializeField]
    [Tooltip("XRInputButtonReader for primary click input from the Android Mouse Interaction Profile.")]
    XRInputButtonReader m_ClickAction;

    [SerializeField]
    [Tooltip("XRInputButtonReader for secondary (right) click input from the Android Mouse Interaction Profile.")]
    XRInputButtonReader m_SecondaryClickAction;

    [SerializeField]
    [Tooltip("XRInputButtonReader for tertiary (middle) click input from the Android Mouse Interaction Profile.")]
    XRInputButtonReader m_TertiaryClickAction;

    [SerializeField]
    [Tooltip("XRInputValueReader for Vector3 aim position input from the Android Mouse Interaction Profile.")]
    XRInputValueReader<Vector3> m_AimPositionAction;

    [SerializeField]
    [Tooltip("XRInputValueReader for Quaternion aim rotation input from the Android Mouse Interaction Profile.")]
    XRInputValueReader<Quaternion> m_AimRotationAction;
#if UNITY_6000_0_OR_NEWER
    float m_CurrentRadius;
    bool m_HoveringScrollableUI;

    void Start()
    {
#if OPEN_XR_1_17_OR_NEWER
        // Check if the Android Mouse Interaction Profile is available in the Input System.
        // If not available or enabled, deactivate the GameObject as the component cannot function.
        var androidMouseProfile = InputSystem.GetDevice<AndroidMouseInteractionProfile.AndroidMouseInteraction>();
        if (androidMouseProfile == null)
        {
            gameObject.SetActive(false);
            Debug.LogWarning("Android Mouse Interaction Profile is missing. Please add it through the XR Plugin-in Management panel in Project Settings.", this);
        }
#endif // OPEN_XR_1_17_OR_NEWER
        m_CurrentRadius = m_InitialRadius;
        m_NearFarInteractor.uiHoverEntered.AddListener(HandleHoverEntered);
        m_NearFarInteractor.uiHoverExited.AddListener(HandleHoverExited);
    }

    void Destroy()
    {
        m_NearFarInteractor.uiHoverEntered.RemoveListener(HandleHoverEntered);
        m_NearFarInteractor.uiHoverExited.RemoveListener(HandleHoverExited);
    }

    void HandleHoverEntered(UIHoverEventArgs arg0)
    {
        if (m_NearFarInteractor.TryGetUIModel(out var hoveringScrollableUI))
        {
            m_HoveringScrollableUI = hoveringScrollableUI.isScrollable;
        }
    }

    void HandleHoverExited(UIHoverEventArgs hoverArgs)
    {
        m_HoveringScrollableUI = false;
    }

    void Update()
    {
        // Read the aim position from the Android Mouse Interaction Profile
        Vector3 center = m_AimPositionAction.ReadValue();

        // Read the aim orientation from the Android Mouse Interaction Profile
        Quaternion aimRotation = m_AimRotationAction.ReadValue();

        // Calculate the forward direction from the aim rotation (OpenXR uses -Z as forward)
        Vector3 direction = aimRotation * Vector3.forward;

        // Apply scroll input to adjust the cursor radius only when NOT hovering over scrollable UI.
        // When hovering over scrollable UI, the scroll input should be passed to the UI element instead
        // of adjusting the cursor radius, allowing natural UI scrolling behavior.
        if (!m_HoveringScrollableUI)
        {
            Vector2 scrollValue = m_ScrollAction.ReadValue();
            m_CurrentRadius += -scrollValue.y * m_ScrollSpeed;
            // Prevent radius from going negative or too small (minimum 0.1 units)
            m_CurrentRadius = Mathf.Max(0.1f, m_CurrentRadius);
        }

        // Update the cursor visual with the current position, direction, rotation, and radius
        m_CursorVisual.UpdateCursorPositionAndRotation(center, direction, aimRotation, m_CurrentRadius);
    }
#else // UNITY_6000_0_OR_NEWER
    void Awake()
    {
        gameObject.SetActive(false);
    }
#endif // UNITY_6000_0_OR_NEWER
}
