using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.InteractionSimulator
{
    public class XRInteractionSimulatorCursor : MonoBehaviour
    {
        enum CursorIcon
        {
            None,
            HMD,
            LeftController,
            RightController,
            Both
        }

        [SerializeField]
        [Tooltip("The cursor texture used to represent left controller point-and-click on Mac devices.")]
        Texture2D m_MacLeftControllerCursor;

        [SerializeField]
        [Tooltip("The cursor texture used to represent right controller point-and-click on Mac devices.")]
        Texture2D m_MacRightControllerCursor;

        [SerializeField]
        [Tooltip("The cursor texture used to represent multi-controller point-and-click on Mac devices.")]
        Texture2D m_MacBothControllerCursor;

        [SerializeField]
        [Tooltip("The cursor texture used to represent point-and-click in controller mode on Mac devices.")]
        Texture2D m_MacHMDCursor;

        [SerializeField]
        [Tooltip("The cursor texture used to represent left controller point-and-click on Windows devices.")]
        Texture2D m_WinLeftControllerCursor;

        [SerializeField]
        [Tooltip("The cursor texture used to represent right controller point-and-click on Windows devices.")]
        Texture2D m_WinRightControllerCursor;

        [SerializeField]
        [Tooltip("The cursor texture used to represent multi-controller point-and-click on Windows devices.")]
        Texture2D m_WinBothControllerCursor;

        [SerializeField]
        [Tooltip("The cursor texture used to represent point-and-click in controller mode on Windows devices.")]
        Texture2D m_WinHMDCursor;

        Texture2D m_LeftControllerCursor;
        Texture2D m_RightControllerCursor;
        Texture2D m_BothControllerCursor;
        Texture2D m_HMDCursor;

        CursorIcon m_CursorIcon;
        Vector2 m_LeftControllerCursorOffset;

        XRInteractionSimulator m_Simulator;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Awake()
        {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            m_LeftControllerCursor = m_MacLeftControllerCursor;
            m_RightControllerCursor = m_MacRightControllerCursor;
            m_BothControllerCursor = m_MacBothControllerCursor;
            m_HMDCursor = m_MacHMDCursor;
#else
            m_LeftControllerCursor = m_WinLeftControllerCursor;
            m_RightControllerCursor = m_WinRightControllerCursor;
            m_BothControllerCursor = m_WinBothControllerCursor;
            m_HMDCursor = m_WinHMDCursor;
#endif

            if (m_LeftControllerCursor != null)
            {
                if (!m_LeftControllerCursor.isReadable)
                    Debug.LogError("Left Controller Cursor for simulator UI must be a readable texture.", this);
                else
                    m_LeftControllerCursorOffset = new Vector2(m_LeftControllerCursor.width, 0f);
            }

            if (m_RightControllerCursor != null && !m_RightControllerCursor.isReadable)
                Debug.LogError("Right Controller Cursor for simulator UI must be a readable texture.", this);

            if (m_BothControllerCursor != null && !m_BothControllerCursor.isReadable)
                Debug.LogError("Both Controller Cursor for simulator UI must be a readable texture.", this);

            if (m_HMDCursor != null && !m_HMDCursor.isReadable)
                Debug.LogError("HMD Cursor for simulator UI must be a readable texture.", this);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnDisable()
        {
            SetCursor(CursorIcon.None);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Start()
        {
            if (!ComponentLocatorUtility<XRInteractionSimulator>.TryFindComponent(out m_Simulator))
            {
                Debug.LogError("Could not find the XRInteractionSimulator component, disabling simulator cursor UI.", this);
                enabled = false;
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Update()
        {
            if (!m_Simulator.pointAndClickActive)
            {
                SetCursor(CursorIcon.None);
            }
            else
            {
                if (RotationInputIsPerformed())
                    SetCursor(CursorIcon.HMD);
                else if (m_Simulator.manipulatingLeftController && m_Simulator.manipulatingRightController)
                    SetCursor(CursorIcon.Both);
                else if (m_Simulator.manipulatingLeftController)
                    SetCursor(CursorIcon.LeftController);
                else if (m_Simulator.manipulatingRightController)
                    SetCursor(CursorIcon.RightController);
            }
        }

        bool RotationInputIsPerformed() => m_Simulator.RotationInputIsPerformed();

        void SetCursor(CursorIcon cursorIcon)
        {
            if (m_CursorIcon == cursorIcon)
                return;

            switch (cursorIcon)
            {
                case CursorIcon.None:
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    break;

                case CursorIcon.HMD:
                    if (m_HMDCursor != null && m_HMDCursor.isReadable)
                        Cursor.SetCursor(m_HMDCursor, Vector2.zero, CursorMode.Auto);
                    break;

                case CursorIcon.LeftController:
                    if (m_LeftControllerCursor != null && m_LeftControllerCursor.isReadable)
                        Cursor.SetCursor(m_LeftControllerCursor, m_LeftControllerCursorOffset, CursorMode.Auto);
                    break;

                case CursorIcon.RightController:
                    if (m_RightControllerCursor != null && m_RightControllerCursor.isReadable)
                        Cursor.SetCursor(m_RightControllerCursor, Vector2.zero, CursorMode.Auto);
                    break;

                case CursorIcon.Both:
                    if (m_BothControllerCursor != null && m_BothControllerCursor.isReadable)
                        Cursor.SetCursor(m_BothControllerCursor, Vector2.zero, CursorMode.Auto);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(cursorIcon), cursorIcon, "Unhandled Cursor type for simulator.");
            }

            m_CursorIcon = cursorIcon;
        }
    }
}
