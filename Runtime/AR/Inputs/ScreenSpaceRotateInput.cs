using System.Diagnostics;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.AR.Inputs
{
    /// <summary>
    /// Component that provides the twist rotation or two finger drag value from a touchscreen for mobile AR.
    /// Intended to be used with an <see cref="XRInputValueReader"/> as its object reference
    /// to provide a rotation value.
    /// </summary>
    /// <seealso cref="XRRayInteractor.rotateManipulationInput"/>
    /// <seealso cref="XRRayInteractor.RotateMode.RotateOverTime"/>
    [AddComponentMenu("XR/Input/Screen Space Rotate Input", 11)]
    [HelpURL(XRHelpURLConstants.k_ScreenSpaceRotateInput)]
    public class ScreenSpaceRotateInput : MonoBehaviour, IXRInputValueReader<Vector2>
    {
        [SerializeField]
        [Tooltip("The ray interactor to get the attach transform from.")]
        XRRayInteractor m_RayInteractor;

        /// <summary>
        /// The ray interactor to get the attach transform from.
        /// </summary>
        public XRRayInteractor rayInteractor
        {
            get => m_RayInteractor;
            set => m_RayInteractor = value;
        }

        [SerializeField]
        [Tooltip("The input used to read the twist delta rotation value.")]
        XRInputValueReader<float> m_TwistDeltaRotationInput = new XRInputValueReader<float>("Twist Delta Rotation");

        /// <summary>
        /// Input to use for the screen twist delta relative to the previous frame.
        /// </summary>
        /// <seealso cref="TouchscreenGestureInputController.twistDeltaRotation"/>
        public XRInputValueReader<float> twistDeltaRotationInput
        {
            get => m_TwistDeltaRotationInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_TwistDeltaRotationInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to read the drag delta value.")]
        XRInputValueReader<Vector2> m_DragDeltaInput = new XRInputValueReader<Vector2>("Drag Delta");

        /// <summary>
        /// Input to use for the screen drag delta relative to the previous frame.
        /// </summary>
        /// <seealso cref="TouchscreenGestureInputController.dragDelta"/>
        public XRInputValueReader<Vector2> dragDeltaInput
        {
            get => m_DragDeltaInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_DragDeltaInput, value, this);
        }

        [SerializeField]
        [Tooltip("The input used to read the screen touch count value.")]
        XRInputValueReader<int> m_ScreenTouchCountInput = new XRInputValueReader<int>("Screen Touch Count");

        /// <summary>
        /// The input used to read the screen touch count value.
        /// </summary>
        /// <seealso cref="TouchscreenGestureInputController.fingerCount"/>
        public XRInputValueReader<int> screenTouchCountInput
        {
            get => m_ScreenTouchCountInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_ScreenTouchCountInput, value, this);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        protected void Reset()
        {
#if UNITY_EDITOR
            m_RayInteractor = GetComponentInParent<XRRayInteractor>(true);
#endif
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Awake()
        {
            if (m_RayInteractor == null)
                m_RayInteractor = GetComponentInParent<XRRayInteractor>(true);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            m_TwistDeltaRotationInput.EnableDirectActionIfModeUsed();
            m_DragDeltaInput.EnableDirectActionIfModeUsed();
            m_ScreenTouchCountInput.EnableDirectActionIfModeUsed();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            m_TwistDeltaRotationInput.DisableDirectActionIfModeUsed();
            m_DragDeltaInput.DisableDirectActionIfModeUsed();
            m_ScreenTouchCountInput.DisableDirectActionIfModeUsed();
        }

        /// <inheritdoc />
        public Vector2 ReadValue()
        {
            TryReadValue(out var value);
            return value;
        }

        /// <inheritdoc />
        public bool TryReadValue(out Vector2 value)
        {
            if (m_TwistDeltaRotationInput.TryReadValue(out var twistDeltaRotation))
            {
                value = new Vector2(-twistDeltaRotation, 0f);
                return true;
            }

            if (m_ScreenTouchCountInput.ReadValue() > 1 &&
                m_DragDeltaInput.TryReadValue(out var dragDeltaRotation))
            {
                var attachTransform = m_RayInteractor.attachTransform;
                var worldToVerticalOrientedDevice = Quaternion.Inverse(Quaternion.LookRotation(attachTransform.forward, Vector3.up));
                var rotatedDelta = worldToVerticalOrientedDevice * attachTransform.rotation * dragDeltaRotation;

                value = new Vector2((rotatedDelta.x / Screen.dpi) * -50f, 0f);
                return true;
            }

            value = Vector2.zero;
            return false;
        }
    }
}
