using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Gaze;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.Hands
{
    /// <summary>
    /// Aims the far ray of a hand or controller's <see cref="NearFarInteractor"/> with gaze, so users target objects and UI by looking at them.
    /// While the interactor selects an object or presses UI, the ray starts at the hand or controller and moves with it.
    /// When gaze is not available, the ray falls back to the aim pose.
    /// </summary>
    /// <remarks>
    /// Only the far ray's origin changes, so grabbing and moving objects work as they do without gaze.
    /// The ray only switches between gaze and the aim pose between selections, so losing gaze never drops a held object.
    /// </remarks>
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_GazeAssistance)]
    public class GazePinchRayDriver : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The Near-Far Interactor whose far ray this component drives.")]
        NearFarInteractor m_NearFarInteractor;

        /// <summary>
        /// The Near-Far Interactor whose far ray this component drives.
        /// </summary>
        public NearFarInteractor nearFarInteractor
        {
            get => m_NearFarInteractor;
            set => m_NearFarInteractor = value;
        }

        [SerializeField]
        [Tooltip("The aim pose of the hand or controller, used while gaze is not available or while selecting. If not set, uses the far caster's cast origin.")]
        Transform m_AimPose;

        /// <summary>
        /// The aim pose of the hand or controller, used while gaze is not available or while selecting.
        /// If not set, uses the far caster's cast origin.
        /// </summary>
        public Transform aimPose
        {
            get => m_AimPose;
            set => m_AimPose = value;
        }

        [SerializeField]
        [Tooltip("The gaze pose the far ray casts from while gaze is available, such as the Gaze Stabilized object of the XR Origin.")]
        Transform m_GazePose;

        /// <summary>
        /// The gaze pose the far ray casts from while gaze is available, such as the Gaze Stabilized object of the XR Origin.
        /// </summary>
        public Transform gazePose
        {
            get => m_GazePose;
            set => m_GazePose = value;
        }

        [SerializeField]
        [Tooltip("The Gaze Input Manager that reports whether gaze is available.")]
        GazeInputManager m_GazeInputManager;

        /// <summary>
        /// The Gaze Input Manager that reports whether gaze is available.
        /// </summary>
        public GazeInputManager gazeInputManager
        {
            get => m_GazeInputManager;
            set => m_GazeInputManager = value;
        }

        [SerializeField]
        [Tooltip("Seconds that gaze must stay unavailable before the far ray falls back to the aim pose.")]
        float m_GazeLostDelay = 0.5f;

        /// <summary>
        /// Seconds that gaze must stay unavailable before the far ray falls back to the aim pose.
        /// </summary>
        public float gazeLostDelay
        {
            get => m_GazeLostDelay;
            set => m_GazeLostDelay = value;
        }

        [SerializeField]
        [Tooltip("Seconds that gaze must stay available before the far ray returns from the aim pose to gaze.")]
        float m_GazeRegainedDelay = 0.2f;

        /// <summary>
        /// Seconds that gaze must stay available before the far ray returns from the aim pose to gaze.
        /// </summary>
        public float gazeRegainedDelay
        {
            get => m_GazeRegainedDelay;
            set => m_GazeRegainedDelay = value;
        }

        [SerializeField]
        [Tooltip("Objects to hide while the far ray casts from gaze, such as the ray's line visual.")]
        GameObject[] m_ObjectsToHideWhileUsingGaze;

        /// <summary>
        /// Objects to hide while the far ray casts from gaze, such as the ray's line visual.
        /// </summary>
        public GameObject[] objectsToHideWhileUsingGaze
        {
            get => m_ObjectsToHideWhileUsingGaze;
            set => m_ObjectsToHideWhileUsingGaze = value;
        }

        /// <summary>
        /// Whether the far ray currently casts from gaze rather than from the aim pose.
        /// </summary>
        public bool isUsingGaze => m_UsingGaze;

        // This interactor's own cast origin in gaze mode. The gaze pose is shared by every driver and driven by
        // tracking, so it can't be moved to follow the hand or controller while selecting.
        Transform m_GazeRayOrigin;
        Transform m_OriginalCastOrigin;
        bool m_UsingGaze;
        bool m_Selecting;
        bool m_GazeAvailable;
        bool m_HasCheckedGazeAssistance;
        float m_NextGazeAvailableUpdateTime = -1f;
        Quaternion m_RayOffsetFromAim;
        readonly Dictionary<GameObject, bool> m_ObjectsActiveOnEnable = new Dictionary<GameObject, bool>();

        void OnEnable()
        {
            RecordObjectsActive();

            if (m_NearFarInteractor == null)
                m_NearFarInteractor = GetComponent<NearFarInteractor>();

            var farCaster = m_NearFarInteractor != null ? m_NearFarInteractor.farInteractionCaster : null;
            if (farCaster == null)
            {
                Debug.LogError($"{nameof(GazePinchRayDriver)} requires a {nameof(NearFarInteractor)} with a far caster. Disabling component.", this);
                enabled = false;
                return;
            }

            m_OriginalCastOrigin = farCaster.castOrigin;
            if (m_AimPose == null)
                m_AimPose = m_OriginalCastOrigin;

            if (m_GazePose == null || m_GazeInputManager == null)
                Debug.LogWarning($"{nameof(GazePinchRayDriver)} needs a Gaze Pose and a {nameof(GazeInputManager)} to use gaze, so the far ray casts from the aim pose.", this);

            WarnIfGazeAssistanceDrivesInteractor();

            var xrOrigin = GetComponentInParent<XROrigin>();
            if (xrOrigin == null || xrOrigin.CameraFloorOffsetObject == null)
            {
                Debug.LogError($"{nameof(GazePinchRayDriver)} must be under an {nameof(XROrigin)} with a Camera Floor Offset Object. Disabling component.", this);
                enabled = false;
                return;
            }

            if (m_GazeRayOrigin == null)
            {
                m_GazeRayOrigin = new GameObject($"[{m_NearFarInteractor.handedness}] Gaze Ray Origin").transform;
                // Its world pose is set every frame while the ray uses gaze. The parent only makes it move with the rig
                // during locomotion and share the rig's lifetime.
                m_GazeRayOrigin.SetParent(xrOrigin.CameraFloorOffsetObject.transform, false);
            }

            m_NextGazeAvailableUpdateTime = -1f;
            m_Selecting = false;
            m_GazeAvailable = IsGazeAvailable();
            SwitchRayOrigin(m_GazeAvailable);
        }

        void OnDisable()
        {
            if (m_NearFarInteractor != null && m_NearFarInteractor.farInteractionCaster != null && m_OriginalCastOrigin != null)
                m_NearFarInteractor.farInteractionCaster.castOrigin = m_OriginalCastOrigin;

            m_UsingGaze = false;
            SetObjectsHidden(false);
        }

        void OnDestroy()
        {
            if (m_GazeRayOrigin != null)
                Destroy(m_GazeRayOrigin.gameObject);
        }

        void Update()
        {
            UpdateGazeAvailable(IsGazeAvailable());

            // Only switch between gaze and the aim pose while nothing is selected, so a held object
            // or pressed UI element never has its ray swapped out from under it.
            var selecting = m_NearFarInteractor.hasSelection || (m_NearFarInteractor.TryGetUIModel(out var uiModel) && uiModel.select);

            if (!selecting)
            {
                m_Selecting = false;
                if (m_UsingGaze != m_GazeAvailable)
                    SwitchRayOrigin(m_GazeAvailable);
            }
            else if (!m_Selecting)
            {
                // On select, record the angle from the aim pose to the gaze target,
                // so the ray starts on it and UI drags follow the hand or controller.
                m_Selecting = true;
                if (m_UsingGaze)
                    m_RayOffsetFromAim = Quaternion.Inverse(m_AimPose.rotation) * GetRotationTowardEndPoint();
            }

            // Without gaze the caster uses the aim pose directly, so there is nothing to drive.
            if (!m_UsingGaze)
                return;

            // While selecting, the ray starts at the aim pose and turns with it. Otherwise, it casts from the gaze.
            if (m_Selecting)
                m_GazeRayOrigin.SetPositionAndRotation(m_AimPose.position, m_AimPose.rotation * m_RayOffsetFromAim);
            else
                m_GazeRayOrigin.SetWorldPose(m_GazePose.GetWorldPose());
        }

        Quaternion GetRotationTowardEndPoint()
        {
            // Without a hit, keep the gaze ray's direction.
            var direction = m_GazeRayOrigin.forward;
            if (m_NearFarInteractor.TryGetCurveEndPoint(out var endPoint) != EndPointType.None)
            {
                var toEndPoint = endPoint - m_AimPose.position;
                if (toEndPoint.sqrMagnitude > Mathf.Epsilon)
                    direction = toEndPoint;
            }

            // Get the rotation that points from the aim pose at the hit point.
            return Quaternion.LookRotation(direction, m_AimPose.up);
        }

        bool IsGazeAvailable()
        {
            if (m_GazePose == null || m_GazeInputManager == null)
                return false;

            // The gaze pose usually stops updating while its object is inactive, because the components driving it don't run.
            if (!m_GazePose.gameObject.activeInHierarchy)
                return false;

            return m_GazeInputManager.IsGazeTracked();
        }

        void UpdateGazeAvailable(bool gazeAvailable)
        {
            if (gazeAvailable == m_GazeAvailable)
            {
                m_NextGazeAvailableUpdateTime = -1f;
                return;
            }

            // Only accept the change once it has lasted for the lost or regained delay.
            if (m_NextGazeAvailableUpdateTime < 0f)
            {
                var delay = gazeAvailable ? m_GazeRegainedDelay : m_GazeLostDelay;
                m_NextGazeAvailableUpdateTime = Time.unscaledTime + delay;
            }

            if (Time.unscaledTime < m_NextGazeAvailableUpdateTime)
                return;

            m_GazeAvailable = gazeAvailable;
            m_NextGazeAvailableUpdateTime = -1f;
        }

        void SwitchRayOrigin(bool useGaze)
        {
            m_UsingGaze = useGaze;

            // Place the ray at the eyes now, so the caster's first gaze-mode cast doesn't use a stale pose.
            if (useGaze)
                m_GazeRayOrigin.SetWorldPose(m_GazePose.GetWorldPose());

            m_NearFarInteractor.farInteractionCaster.castOrigin = useGaze ? m_GazeRayOrigin : m_AimPose;
            SetObjectsHidden(useGaze);
        }

        void WarnIfGazeAssistanceDrivesInteractor()
        {
            if (m_HasCheckedGazeAssistance)
                return;

            m_HasCheckedGazeAssistance = true;
            var gazeAssistance = FindAnyObjectByType<XRGazeAssistance>();
            if (gazeAssistance == null || !gazeAssistance.isActiveAndEnabled)
                return;

            foreach (var interactorData in gazeAssistance.rayInteractors)
            {
                if (interactorData.interactor != m_NearFarInteractor)
                    continue;

                Debug.LogWarning($"{nameof(XRGazeAssistance)} on {gazeAssistance.name} also drives the far ray of {m_NearFarInteractor.name}. " +
                    $"Both components set its cast origin, so remove {m_NearFarInteractor.name} from the Ray Interactors list of {nameof(XRGazeAssistance)}.", this);
                return;
            }
        }

        void RecordObjectsActive()
        {
            m_ObjectsActiveOnEnable.Clear();
            if (m_ObjectsToHideWhileUsingGaze == null)
                return;

            foreach (var objectToHide in m_ObjectsToHideWhileUsingGaze)
            {
                if (objectToHide != null)
                    m_ObjectsActiveOnEnable[objectToHide] = objectToHide.activeSelf;
            }
        }

        void SetObjectsHidden(bool hidden)
        {
            foreach (var (objectToHide, wasActive) in m_ObjectsActiveOnEnable)
            {
                if (objectToHide == null)
                    continue;

                // Showing restores the state recorded on enable, so an object that started inactive stays inactive.
                objectToHide.SetActive(!hidden && wasActive);
            }
        }
    }
}
