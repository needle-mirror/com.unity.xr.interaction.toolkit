using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.AR.Inputs
{
    /// <summary>
    /// Applies the pose computed from the touchscreen gestures to the Transform this component is on.
    /// Similar to a Tracked Pose Driver in purpose but uses touchscreen gesture input sources to compute
    /// the pose rather than directly using input actions.
    /// </summary>
    [AddComponentMenu("XR/Input/Screen Space Ray Pose Driver", 11)]
    [HelpURL(XRHelpURLConstants.k_ScreenSpaceRayPoseDriver)]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_ScreenSpaceRayPoseDriver)]
    public class ScreenSpaceRayPoseDriver : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The camera associated with the screen, and through which screen presses/touches will be interpreted.")]
        Camera m_ControllerCamera;

        /// <summary>
        /// The camera associated with the screen, and through which screen presses/touches will be interpreted.
        /// </summary>
        public Camera controllerCamera
        {
            get => m_ControllerCamera;
            set => m_ControllerCamera = value;
        }

        [SerializeField]
        XRInputValueReader<Vector2> m_TapStartPositionInput = new XRInputValueReader<Vector2>("Tap Start Position");

        /// <summary>
        /// Input to use for the screen tap start position.
        /// </summary>
        /// <seealso cref="TouchscreenGestureInputController.tapStartPosition"/>
        public XRInputValueReader<Vector2> tapStartPositionInput
        {
            get => m_TapStartPositionInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_TapStartPositionInput, value, this);
        }

        [SerializeField]
        XRInputValueReader<Vector2> m_DragCurrentPositionInput = new XRInputValueReader<Vector2>("Drag Current Position");

        /// <summary>
        /// Input to use for the screen drag current position.
        /// </summary>
        /// <seealso cref="TouchscreenGestureInputController.dragCurrentPosition"/>
        public XRInputValueReader<Vector2> dragCurrentPositionInput
        {
            get => m_DragCurrentPositionInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_DragCurrentPositionInput, value, this);
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
        /// Holds captured value of the parent transform of this component to avoid doing an Object comparison to null
        /// each frame when it hasn't changed reference.
        /// </summary>
        readonly UnityObjectReferenceCache<Transform> m_ParentTransformCache = new UnityObjectReferenceCache<Transform>();

        Vector2 m_TapStartPosition;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            if (m_ControllerCamera == null)
            {
                m_ControllerCamera = Camera.main;
                if (m_ControllerCamera == null)
                {
                    Debug.LogWarning($"Could not find associated {nameof(Camera)} in scene." +
                        $"This {nameof(ScreenSpaceRayPoseDriver)} will be disabled.", this);
                    enabled = false;
                    return;
                }
            }

            m_TapStartPositionInput.EnableDirectActionIfModeUsed();
            m_DragCurrentPositionInput.EnableDirectActionIfModeUsed();
            m_ScreenTouchCountInput.EnableDirectActionIfModeUsed();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            m_TapStartPositionInput.DisableDirectActionIfModeUsed();
            m_DragCurrentPositionInput.DisableDirectActionIfModeUsed();
            m_ScreenTouchCountInput.DisableDirectActionIfModeUsed();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Update()
        {
            var prevTapStartPosition = m_TapStartPosition;
            var tapPerformedThisFrame = m_TapStartPositionInput.TryReadValue(out m_TapStartPosition) && prevTapStartPosition != m_TapStartPosition;

            if (m_ScreenTouchCountInput.TryReadValue(out var screenTouchCount) && screenTouchCount > 1)
                return;

            if (m_DragCurrentPositionInput.TryReadValue(out var screenPosition))
            {
                ApplyPose(screenPosition);
                return;
            }

            if (tapPerformedThisFrame)
            {
                ApplyPose(m_TapStartPosition);
                return;
            }
        }

        void ApplyPose(Vector2 screenPosition)
        {
            var screenToWorldPoint = m_ControllerCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, m_ControllerCamera.nearClipPlane));
            var directionVector = (screenToWorldPoint - m_ControllerCamera.transform.position).normalized;

            var localPosition = m_ParentTransformCache.TryGet(transform.parent, out var parent)
                ? parent.InverseTransformPoint(screenToWorldPoint)
                : screenToWorldPoint;

            transform.SetLocalPose(new Pose(localPosition, Quaternion.LookRotation(directionVector)));
        }
    }
}
