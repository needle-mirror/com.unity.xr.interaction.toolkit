using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Unity.XR.CoreUtils;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Interactor used for interacting with interactables at a distance. This is handled via ray casts
    /// that update the current set of valid targets for this interactor.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("XR/XR Ray Interactor", 11)]
    [HelpURL(XRHelpURLConstants.k_XRRayInteractor)]
    public partial class XRRayInteractor : XRBaseControllerInteractor, ILineRenderable, IUIInteractor
    {
        /// <summary>
        /// Reusable list of interactables (used to process the valid targets when this interactor has a filter).
        /// </summary>
        static readonly List<IXRInteractable> s_Results = new List<IXRInteractable>();

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
                var aDistance = a.collider != null ? a.distance : float.MaxValue;
                var bDistance = b.collider != null ? b.distance : float.MaxValue;
                return aDistance.CompareTo(bDistance);
            }
        }

        const int k_MaxRaycastHits = 10;

        const int k_MinSampleFrequency = 2;
        const int k_MaxSampleFrequency = 100;

        /// <summary>
        /// Sets which trajectory path Unity uses for the cast when detecting collisions.
        /// </summary>
        /// <seealso cref="lineType"/>
        public enum LineType
        {
            /// <summary>
            /// Performs a single ray cast into the Scene with a set ray length.
            /// </summary>
            StraightLine,

            /// <summary>
            /// Samples the trajectory of a projectile to generate a projectile curve.
            /// </summary>
            ProjectileCurve,

            /// <summary>
            /// Uses a control point and an end point to create a quadratic Bézier curve.
            /// </summary>
            BezierCurve,
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
        }

        /// <summary>
        /// Sets how anchor rotation is controlled.
        /// </summary>
        /// <seealso cref="anchorRotationMode"/>
        public enum AnchorRotationMode
        {
            /// <summary>
            /// The anchor rotates over time while rotation input is active.
            /// </summary>
            RotateOverTime,

            /// <summary>
            /// The anchor rotates to match the direction of the 2-dimensional rotation input.
            /// </summary>
            MatchDirection,
        }

        [SerializeField]
        LineType m_LineType = LineType.StraightLine;
        /// <summary>
        /// Gets or sets the type of ray cast.
        /// </summary>
        public LineType lineType
        {
            get => m_LineType;
            set => m_LineType = value;
        }

        [SerializeField]
        bool m_BlendVisualLinePoints = true;
        /// <summary>
        /// Blend the line sample points Unity uses for ray casting with the current pose of the controller.
        /// Use this to make the line visual stay connected with the controller instead of lagging behind.
        /// </summary>
        /// <remarks>
        /// When the controller is configured to sample tracking input directly before rendering to reduce
        /// input latency, the controller may be in a new position or rotation relative to the starting point
        /// of the sample curve used for ray casting.
        /// <br/>
        /// A value of <see langword="false"/> will make the line visual stay at a fixed reference frame rather than bending
        /// or curving towards the end of the ray cast line.
        /// </remarks>
        public bool blendVisualLinePoints
        {
            get => m_BlendVisualLinePoints;
            set => m_BlendVisualLinePoints = value;
        }

        [SerializeField]
        float m_MaxRaycastDistance = 30f;
        /// <summary>
        /// Gets or sets the max distance of ray cast when the line type is a straight line.
        /// Increasing this value will make the line reach further.
        /// </summary>
        /// <seealso cref="LineType.StraightLine"/>
        public float maxRaycastDistance
        {
            get => m_MaxRaycastDistance;
            set => m_MaxRaycastDistance = value;
        }

        [SerializeField]
        Transform m_RayOriginTransform;
        /// <summary>
        /// The starting position and direction of any ray casts.
        /// </summary>
        /// <remarks>
        /// Automatically instantiated and set in <see cref="Awake"/> if <see langword="null"/>
        /// and initialized with the pose of the <see cref="XRBaseInteractor.attachTransform"/>.
        /// Setting this will not automatically destroy the previous object.
        /// </remarks>
        public Transform rayOriginTransform
        {
            get => m_RayOriginTransform;
            set => m_RayOriginTransform = value;
        }

        [SerializeField]
        Transform m_ReferenceFrame;
        /// <summary>
        /// The reference frame of the curve to define the ground plane and up.
        /// If not set at startup it will try to find the <see cref="XROrigin.Origin"/> GameObject,
        /// and if that does not exist it will use global up and origin by default.
        /// </summary>
        /// <seealso cref="LineType.ProjectileCurve"/>
        /// <seealso cref="LineType.BezierCurve"/>
        public Transform referenceFrame
        {
            get => m_ReferenceFrame;
            set => m_ReferenceFrame = value;
        }

        [SerializeField]
        float m_Velocity = 16f;
        /// <summary>
        /// Initial velocity of the projectile. Increasing this value will make the curve reach further.
        /// </summary>
        /// <seealso cref="LineType.ProjectileCurve"/>
        public float velocity
        {
            get => m_Velocity;
            set => m_Velocity = value;
        }

        [SerializeField]
        float m_Acceleration = 9.8f;
        /// <summary>
        /// Gravity of the projectile in the reference frame.
        /// </summary>
        /// <seealso cref="LineType.ProjectileCurve"/>
        public float acceleration
        {
            get => m_Acceleration;
            set => m_Acceleration = value;
        }

        [SerializeField]
        float m_AdditionalGroundHeight = 0.1f;
        /// <summary>
        /// Additional height below ground level that the projectile will continue to.
        /// Increasing this value will make the end point drop lower in height.
        /// </summary>
        /// <seealso cref="LineType.ProjectileCurve"/>
        public float additionalGroundHeight
        {
            get => m_AdditionalGroundHeight;
            set => m_AdditionalGroundHeight = value;
        }

        [SerializeField]
        float m_AdditionalFlightTime = 0.5f;
        /// <summary>
        /// Additional flight time after the projectile lands at the adjusted ground level.
        /// Increasing this value will make the end point drop lower in height.
        /// </summary>
        /// <seealso cref="LineType.ProjectileCurve"/>
        public float additionalFlightTime
        {
            get => m_AdditionalFlightTime;
            set => m_AdditionalFlightTime = value;
        }

        [SerializeField]
        float m_EndPointDistance = 30f;
        /// <summary>
        /// Increase this value distance to make the end of the curve further from the start point.
        /// </summary>
        /// <seealso cref="LineType.BezierCurve"/>
        public float endPointDistance
        {
            get => m_EndPointDistance;
            set => m_EndPointDistance = value;
        }

        [SerializeField]
        float m_EndPointHeight = -10f;
        /// <summary>
        /// Decrease this value to make the end of the curve drop lower relative to the start point.
        /// </summary>
        /// <seealso cref="LineType.BezierCurve"/>
        public float endPointHeight
        {
            get => m_EndPointHeight;
            set => m_EndPointHeight = value;
        }

        [SerializeField]
        float m_ControlPointDistance = 10f;
        /// <summary>
        /// Increase this value to make the peak of the curve further from the start point.
        /// </summary>
        /// <seealso cref="LineType.BezierCurve"/>
        public float controlPointDistance
        {
            get => m_ControlPointDistance;
            set => m_ControlPointDistance = value;
        }

        [SerializeField]
        float m_ControlPointHeight = 5f;
        /// <summary>
        /// Increase this value to make the peak of the curve higher relative to the start point.
        /// </summary>
        /// <seealso cref="LineType.BezierCurve"/>
        public float controlPointHeight
        {
            get => m_ControlPointHeight;
            set => m_ControlPointHeight = value;
        }

        [SerializeField]
        [Range(k_MinSampleFrequency, k_MaxSampleFrequency)]
        int m_SampleFrequency = 20;
        /// <summary>
        /// The number of sample points Unity uses to approximate curved paths.
        /// Larger values produce a better quality approximate at the cost of reduced performance
        /// due to the number of ray casts.
        /// </summary>
        /// <remarks>
        /// A value of <i>n</i> will result in <i>n - 1</i> line segments for ray cast.
        /// This property is not used when using <see cref="LineType.StraightLine"/> since the value would always be 2.
        /// </remarks>
        /// <seealso cref="LineType.ProjectileCurve"/>
        /// <seealso cref="LineType.BezierCurve"/>
        public int sampleFrequency
        {
            get => m_SampleFrequency;
            set => m_SampleFrequency = SanitizeSampleFrequency(value);
        }

        [SerializeField]
        HitDetectionType m_HitDetectionType = HitDetectionType.Raycast;
        /// <summary>
        /// Gets or sets which type of hit detection to use for the ray cast.
        /// </summary>
        public HitDetectionType hitDetectionType
        {
            get => m_HitDetectionType;
            set => m_HitDetectionType = value;
        }

        [SerializeField]
        [Range(0.01f, 0.25f)]
        float m_SphereCastRadius = 0.1f;
        /// <summary>
        /// Gets or sets radius used for sphere casting. Will use regular ray casting if set to 0 or less.
        /// </summary>
        /// <seealso cref="HitDetectionType.SphereCast"/>
        /// <seealso cref="hitDetectionType"/>
        public float sphereCastRadius
        {
            get => m_SphereCastRadius;
            set => m_SphereCastRadius = value;
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
        /// When set to <see cref="QuerySnapVolumeInteraction.Ignore"/> when <see cref="raycastTriggerInteraction"/> is set to hit trigger colliders
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
        bool m_HitClosestOnly;
        /// <summary>
        /// Whether Unity considers only the closest Interactable as a valid target for interaction.
        /// </summary>
        /// <remarks>
        /// Enable this to make only the closest Interactable receive hover events.
        /// Otherwise, all hit Interactables will be considered valid and this Interactor will multi-hover.
        /// </remarks>
        /// <seealso cref="GetValidTargets"/>
        public bool hitClosestOnly
        {
            get => m_HitClosestOnly;
            set => m_HitClosestOnly = value;
        }

        [SerializeField]
        bool m_HoverToSelect;
        /// <summary>
        /// Whether this Interactor will automatically select an Interactable after hovering over it for a period of time.
        /// </summary>
        /// <seealso cref="hoverTimeToSelect"/>
        public bool hoverToSelect
        {
            get => m_HoverToSelect;
            set => m_HoverToSelect = value;
        }

        [SerializeField]
        float m_HoverTimeToSelect = 0.5f;
        /// <summary>
        /// Number of seconds for which this Interactor must hover over an Interactable to select it if Hover To Select is enabled.
        /// </summary>
        /// <seealso cref="hoverToSelect"/>
        public float hoverTimeToSelect
        {
            get => m_HoverTimeToSelect;
            set => m_HoverTimeToSelect = value;
        }

        [SerializeField]
        bool m_AutoDeselect;
        /// <summary>
        /// Whether this Interactor will automatically deselect an Interactable after selecting it via hover for a period of time.
        /// </summary>
        /// <remarks>
        /// This only applies when an interactable is selected due to <see cref="hoverToSelect"/>.
        /// </remarks>
        /// <seealso cref="timeToAutoDeselect"/>
        public bool autoDeselect
        {
            get => m_AutoDeselect;
            set => m_AutoDeselect = value;
        }

        [SerializeField]
        float m_TimeToAutoDeselect = 3f;
        /// <summary>
        /// Number of seconds for which this Interactor will keep an Interactable selected before automatically deselecting it.
        /// </summary>
        /// <remarks>
        /// This only applies when an interactable is selected due to <see cref="hoverToSelect"/>.
        /// </remarks>
        /// <seealso cref="hoverToSelect"/>
        public float timeToAutoDeselect
        {
            get => m_TimeToAutoDeselect;
            set => m_TimeToAutoDeselect = value;
        }

        [SerializeField]
        bool m_EnableUIInteraction = true;
        /// <summary>
        /// Gets or sets whether this Interactor is able to affect UI.
        /// </summary>
        public bool enableUIInteraction
        {
            get => m_EnableUIInteraction;
            set
            {
                if (m_EnableUIInteraction != value)
                {
                    m_EnableUIInteraction = value;
                    m_RegisteredUIInteractorCache?.RegisterOrUnregisterXRUIInputModule(m_EnableUIInteraction);
                }
            }
        }

        [SerializeField]
        bool m_AllowAnchorControl = true;
        /// <summary>
        /// Allows the user to move the attach anchor point using the joystick.
        /// </summary>
        /// <seealso cref="rotateSpeed"/>
        /// <seealso cref="translateSpeed"/>
        /// <seealso cref="anchorRotateReferenceFrame"/>
        /// <seealso cref="anchorRotationMode"/>
        public bool allowAnchorControl
        {
            get => m_AllowAnchorControl;
            set => m_AllowAnchorControl = value;
        }

        [SerializeField]
        bool m_UseForceGrab = true;
        /// <summary>
        /// Force grab moves the object to your hand rather than interacting with it at a distance.
        /// </summary>
        public bool useForceGrab
        {
            get => m_UseForceGrab;
            set => m_UseForceGrab = value;
        }

        [SerializeField]
        float m_RotateSpeed = 180f;
        /// <summary>
        /// Speed that the anchor is rotated when <see cref="anchorRotationMode"/> is set to <see cref="AnchorRotationMode.RotateOverTime"/>.
        /// </summary>
        /// <seealso cref="allowAnchorControl"/>
        /// <seealso cref="translateSpeed"/>
        /// <seealso cref="anchorRotationMode"/>
        public float rotateSpeed
        {
            get => m_RotateSpeed;
            set => m_RotateSpeed = value;
        }

        [SerializeField]
        float m_TranslateSpeed = 1f;
        /// <summary>
        /// Speed that the anchor is translated.
        /// </summary>
        /// <seealso cref="allowAnchorControl"/>
        /// <seealso cref="rotateSpeed"/>
        public float translateSpeed
        {
            get => m_TranslateSpeed;
            set => m_TranslateSpeed = value;
        }

        [SerializeField]
        Transform m_AnchorRotateReferenceFrame;
        /// <summary>
        /// The optional reference frame to define the up axis when rotating the attach anchor point.
        /// When not set, rotates about the local up axis of the attach transform.
        /// </summary>
        /// <seealso cref="allowAnchorControl"/>
        /// <seealso cref="RotateAnchor(Transform, float)"/>
        /// <seealso cref="RotateAnchor(Transform, Vector2, Quaternion)"/>
        public Transform anchorRotateReferenceFrame
        {
            get => m_AnchorRotateReferenceFrame;
            set => m_AnchorRotateReferenceFrame = value;
        }

        [SerializeField]
        AnchorRotationMode m_AnchorRotationMode;
        /// <summary>
        /// Gets or sets how the anchor rotation is controlled.
        /// </summary>
        /// <seealso cref="allowAnchorControl"/>
        /// <seealso cref="AnchorRotationMode"/>
        public AnchorRotationMode anchorRotationMode
        {
            get => m_AnchorRotationMode;
            set => m_AnchorRotationMode = value;
        }

        /// <summary>
        /// The launch angle of the Projectile Curve.
        /// More specifically, this is the signed angle in degrees between the original attach forward
        /// direction and the plane of the reference frame, with positive angles when pointing upward.
        /// </summary>
        public float angle
        {
            get
            {
                var castForward = effectiveRayOrigin.forward;
                var up = m_ReferenceFrame != null ? m_ReferenceFrame.up : Vector3.up;
                var projectedForward = Vector3.ProjectOnPlane(castForward, up);
                return Mathf.Approximately(Vector3.Angle(castForward, projectedForward), 0f)
                    ? 0f
                    : Vector3.SignedAngle(castForward, projectedForward, Vector3.Cross(up, castForward));
            }
        }

        /// <summary>
        /// The nearest <see cref="IXRInteractable"/> object hit by the ray that was inserted into the valid targets
        /// list when not selecting anything.
        /// </summary>
        /// <remarks>
        /// Updated during <see cref="PreprocessInteractor"/>.
        /// </remarks>
        protected IXRInteractable currentNearestValidTarget { get; private set; }

        readonly List<IXRInteractable> m_ValidTargets = new List<IXRInteractable>();

        /// <summary>
        /// The starting position and direction of any ray casts.
        /// Safe version of <see cref="rayOriginTransform"/>, falls back to this Transform if not set.
        /// </summary>
        Transform effectiveRayOrigin => m_RayOriginTransform != null ? m_RayOriginTransform : transform;

        /// <summary>
        /// The closest index of the sample endpoint where a 3D or UI hit occurred.
        /// </summary>
        int closestAnyHitIndex => (m_RaycastHitEndpointIndex > 0 && m_UIRaycastHitEndpointIndex > 0) // Are both valid?
            ? Mathf.Min(m_RaycastHitEndpointIndex, m_UIRaycastHitEndpointIndex) // When both are valid, return the closer one
            : (m_RaycastHitEndpointIndex > 0 ? m_RaycastHitEndpointIndex : m_UIRaycastHitEndpointIndex); // Otherwise return the valid one

        float m_LastTimeHoveredObjectChanged;
        bool m_PassedHoverTimeToSelect;
        float m_LastTimeAutoSelected;
        bool m_PassedTimeToAutoDeselect;

        readonly RaycastHit[] m_RaycastHits = new RaycastHit[k_MaxRaycastHits];
        int m_RaycastHitsCount;
        readonly RaycastHitComparer m_RaycastHitComparer = new RaycastHitComparer();

        /// <summary>
        /// A polygonal chain represented by a list of endpoints which form line segments
        /// to approximate the curve. Each line segment is where the ray cast starts and ends.
        /// World space coordinates.
        /// </summary>
        List<SamplePoint> m_SamplePoints;

        /// <summary>
        /// The <see cref="Time.frameCount"/> when Unity last updated the sample points.
        /// Used as an optimization to avoid recomputing the points during <see cref="PreprocessInteractor"/>
        /// when it was already computed and used for an input module in <see cref="UpdateUIModel"/>.
        /// </summary>
        int m_SamplePointsFrameUpdated = -1;

        /// <summary>
        /// The index of the sample endpoint if a 3D hit occurred. Otherwise, a value of <c>0</c> if no hit occurred.
        /// </summary>
        int m_RaycastHitEndpointIndex;

        /// <summary>
        /// The index of the sample endpoint if a UI hit occurred. Otherwise, a value of <c>0</c> if no hit occurred.
        /// </summary>
        int m_UIRaycastHitEndpointIndex;

        /// <summary>
        /// Control points to calculate the quadratic Bezier curve used for aiming.
        /// </summary>
        /// <seealso cref="LineType.BezierCurve"/>
        /// <seealso cref="endPointDistance"/>
        /// <seealso cref="endPointHeight"/>
        /// <seealso cref="controlPointDistance"/>
        /// <seealso cref="controlPointHeight"/>
        readonly Vector3[] m_ControlPoints = new Vector3[3];

        /// <summary>
        /// Control points to calculate the equivalent quadratic Bezier curve to the endpoint where a hit occurred.
        /// </summary>
        readonly Vector3[] m_HitChordControlPoints = new Vector3[3];

        /// <summary>
        /// Reusable list to hold the current sample points.
        /// </summary>
        static List<SamplePoint> s_ScratchSamplePoints;

        /// <summary>
        /// Reusable array to hold the current control points for a quadratic Bezier curve.
        /// </summary>
        static readonly Vector3[] s_ScratchControlPoints = new Vector3[3];

        PhysicsScene m_LocalPhysicsScene;

        RegisteredUIInteractorCache m_RegisteredUIInteractorCache;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnValidate()
        {
            m_SampleFrequency = SanitizeSampleFrequency(m_SampleFrequency);
            m_RegisteredUIInteractorCache?.RegisterOrUnregisterXRUIInputModule(m_EnableUIInteraction);
        }

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();

            m_LocalPhysicsScene = gameObject.scene.GetPhysicsScene();
            m_RegisteredUIInteractorCache = new RegisteredUIInteractorCache(this);

            CreateSamplePointsListsIfNecessary();

            FindReferenceFrame();
            CreateRayOrigin();
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            if (m_EnableUIInteraction)
                m_RegisteredUIInteractorCache.RegisterWithXRUIInputModule();
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
            base.OnDisable();

            // Clear lines
            m_SamplePoints?.Clear();

            if (m_EnableUIInteraction)
                m_RegisteredUIInteractorCache.UnregisterFromXRUIInputModule();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying || m_SamplePoints == null || m_SamplePoints.Count < 2)
            {
                return;
            }

            if (TryGetCurrent3DRaycastHit(out var raycastHit))
            {
                // Draw the normal of the surface at the hit point
                Gizmos.color = new Color(58 / 255f, 122 / 255f, 248 / 255f, 237 / 255f);
                const float length = 0.075f;
                Gizmos.DrawLine(raycastHit.point, raycastHit.point + raycastHit.normal.normalized * length);
            }

            if (TryGetCurrentUIRaycastResult(out var uiRaycastResult))
            {
                // Draw the normal of the surface at the hit point
                Gizmos.color = new Color(58 / 255f, 122 / 255f, 248 / 255f, 237 / 255f);
                const float length = 0.075f;
                Gizmos.DrawLine(uiRaycastResult.worldPosition, uiRaycastResult.worldPosition + uiRaycastResult.worldNormal.normalized * length);
            }

            var hitIndex = closestAnyHitIndex;

            // Draw sample points where the ray cast line segments took place
            for (var i = 0; i < m_SamplePoints.Count; ++i)
            {
                var samplePoint = m_SamplePoints[i];

                // Change the color of the points after the segment where a hit happened
                var radius = m_HitDetectionType == HitDetectionType.SphereCast ? m_SphereCastRadius : 0.025f;
                var color = hitIndex == 0 || i < hitIndex
                    ? new Color(163 / 255f, 73 / 255f, 164 / 255f, 0.75f)
                    : new Color(205 / 255f, 143 / 255f, 205 / 255f, 0.5f);
                Gizmos.color = color;
                Gizmos.DrawSphere(samplePoint.position, radius);
                if (i < m_SamplePoints.Count - 1)
                {
                    var nextPoint = m_SamplePoints[i + 1];
                    Gizmos.DrawLine(samplePoint.position, nextPoint.position);
                }
            }

            switch (m_LineType)
            {
                case LineType.ProjectileCurve:
                    DrawQuadraticBezierGizmo(m_HitChordControlPoints[0], m_HitChordControlPoints[1], m_HitChordControlPoints[2]);
                    break;
                case LineType.BezierCurve:
                    DrawQuadraticBezierGizmo(m_ControlPoints[0], m_ControlPoints[1], m_ControlPoints[2]);
                    break;
            }
        }

        static void DrawQuadraticBezierGizmo(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            // Draw the control points of the quadratic Bezier curve
            // (P₀ = start point, P₁ = control point, P₂ = end point)
            const float radius = 0.025f;
            Gizmos.color = new Color(1f, 0f, 0f, 0.75f);
            Gizmos.DrawSphere(p0, radius);
            Gizmos.DrawSphere(p1, radius);
            Gizmos.DrawSphere(p2, radius);

            // Draw lines between the control points
            Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.75f);
            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawLine(p1, p2);

            // Draw tangent lines along the curve like string art
            // (Q₀ = intermediate start point, Q₁ = intermediate end point, and the linear interpolation between them is the curve)
            Gizmos.color = new Color(0f, 0f, 205 / 25f, 0.75f);
            for (var t = 0.1f; t <= 0.9f; t += 0.1f)
            {
                var q0 = Vector3.Lerp(p0, p1, t);
                var q1 = Vector3.Lerp(p1, p2, t);
                Gizmos.DrawLine(q0, q1);
            }
        }

        /// <summary>
        /// Attempts to locate a reference frame for the curve (if necessary).
        /// </summary>
        /// <seealso cref="referenceFrame"/>
        void FindReferenceFrame()
        {
            if (m_ReferenceFrame != null)
                return;

            var xrOrigin = FindObjectOfType<XROrigin>();
            if (xrOrigin != null)
            {
                var origin = xrOrigin.Origin;
                if (origin != null)
                {
                    m_ReferenceFrame = origin.transform;
                }
                else
                {
                    Debug.Log($"Reference frame of the curve not set and {nameof(XROrigin)}.{nameof(XROrigin.Origin)} is not set, using global up as default.", this);
                }
            }
            else
            {
                Debug.Log($"Reference frame of the curve not set and {nameof(XROrigin)} is not found, using global up as default.", this);
            }
        }

        void CreateRayOrigin()
        {
            if (m_RayOriginTransform == null)
            {
                m_RayOriginTransform = new GameObject($"[{gameObject.name}] Ray Origin").transform;
                m_RayOriginTransform.SetParent(transform, false);
                if (attachTransform != null)
                {
                    m_RayOriginTransform.position = attachTransform.position;
                    m_RayOriginTransform.rotation = attachTransform.rotation;
                }
                else
                {
                    m_RayOriginTransform.localPosition = Vector3.zero;
                    m_RayOriginTransform.localRotation = Quaternion.identity;
                }
            }
        }

        /// <summary>
        /// Use this to determine if the ray is currently hovering over a UI GameObject.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if hovering over a UI element. Otherwise, returns <see langword="false"/>.</returns>
        /// <remarks>
        /// <see cref="enableUIInteraction"/> must be enabled, otherwise the function will always return <see langword="false"/>.
        /// </remarks>
        /// <seealso cref="UIInputModule.IsPointerOverGameObject(int)"/>
        /// <seealso cref="EventSystem.IsPointerOverGameObject(int)"/>
        public bool IsOverUIGameObject()
        {
            return m_EnableUIInteraction && m_RegisteredUIInteractorCache != null && m_RegisteredUIInteractorCache.IsOverUIGameObject();
        }

        /// <inheritdoc />
        public bool GetLinePoints(ref Vector3[] linePoints, out int numPoints)
        {
            if (m_SamplePoints == null || m_SamplePoints.Count < 2)
            {
                numPoints = default;
                return false;
            }

            if (!m_BlendVisualLinePoints)
            {
                numPoints = m_SamplePoints.Count;
                EnsureCapacity(ref linePoints, numPoints);

                for (var i = 0; i < numPoints; ++i)
                    linePoints[i] = m_SamplePoints[i].position;

                return true;
            }

            // Because this method may be invoked during OnBeforeRender, the current positions
            // of sample points may be different as the controller moves. Recompute the current
            // positions of sample points.
            CreateSamplePointsListsIfNecessary();
            UpdateSamplePoints(m_SamplePoints.Count, s_ScratchSamplePoints);

            if (m_LineType == LineType.StraightLine)
            {
                numPoints = 2;
                EnsureCapacity(ref linePoints, numPoints);

                linePoints[0] = s_ScratchSamplePoints[0].position;
                linePoints[1] = m_SamplePoints[m_SamplePoints.Count - 1].position;

                return true;
            }

            // Recompute the equivalent Bezier curve.
            var hitIndex = closestAnyHitIndex;
            CreateBezierCurve(s_ScratchSamplePoints, hitIndex, s_ScratchControlPoints);

            // Blend between the current curve and the sample curve,
            // using the beginning of the current curve and the end of the sample curve.
            // Together it forms a new cubic Bezier curve with control points P₀, P₁, P₂, P₃.
            ElevateQuadraticToCubicBezier(s_ScratchControlPoints[0], s_ScratchControlPoints[1], s_ScratchControlPoints[2],
                out var p0, out var p1, out _, out _);
            ElevateQuadraticToCubicBezier(m_HitChordControlPoints[0], m_HitChordControlPoints[1], m_HitChordControlPoints[2],
                out _, out _, out var p2, out var p3);

            if (hitIndex > 0 && hitIndex != m_SamplePoints.Count - 1 && m_LineType == LineType.ProjectileCurve)
            {
                numPoints = m_SamplePoints.Count;
                EnsureCapacity(ref linePoints, numPoints);

                linePoints[0] = p0;

                // Sample from the blended cubic Bezier curve
                // until the line segment endpoint where the hit occurred.
                var interval = 1f / hitIndex;
                for (var i = 1; i <= hitIndex; ++i)
                {
                    // Parametric parameter t where 0 ≤ t ≤ 1
                    var percent = i * interval;
                    linePoints[i] = SampleCubicBezierPoint(p0, p1, p2, p3, percent);
                }

                // Use the original sample curve beyond that point.
                for (var i = hitIndex + 1; i < m_SamplePoints.Count; ++i)
                {
                    linePoints[i] = m_SamplePoints[i].position;
                }
            }
            else
            {
                numPoints = m_SampleFrequency;
                EnsureCapacity(ref linePoints, numPoints);

                linePoints[0] = p0;

                // Sample from the blended cubic Bezier curve
                var interval = 1f / (m_SampleFrequency - 1);
                for (var i = 1; i < m_SampleFrequency; ++i)
                {
                    // Parametric parameter t where 0 ≤ t ≤ 1
                    var percent = i * interval;
                    linePoints[i] = SampleCubicBezierPoint(p0, p1, p2, p3, percent);
                }
            }

            return true;
        }

        static void EnsureCapacity(ref Vector3[] linePoints, int numPoints)
        {
            if (linePoints == null || linePoints.Length < numPoints)
                linePoints = new Vector3[numPoints];
        }

        /// <inheritdoc />
        public bool TryGetHitInfo(out Vector3 position, out Vector3 normal, out int positionInLine, out bool isValidTarget)
        {
            position = default;
            normal = default;
            positionInLine = default;
            isValidTarget = default;

            if (!TryGetCurrentRaycast(
                out var raycastHit,
                out var raycastHitIndex,
                out var raycastResult,
                out var raycastResultIndex,
                out var isUIHitClosest))
            {
                return false;
            }

            if (raycastResult.HasValue && isUIHitClosest)
            {
                position = raycastResult.Value.worldPosition;
                normal = raycastResult.Value.worldNormal;
                positionInLine = raycastResultIndex;

                isValidTarget = raycastResult.Value.gameObject != null;
            }
            else if (raycastHit.HasValue)
            {
                position = raycastHit.Value.point;
                normal = raycastHit.Value.normal;
                positionInLine = raycastHitIndex;

                // Determine if the collider is registered as an interactable and the interactable is being hovered
                isValidTarget = interactionManager.TryGetInteractableForCollider(raycastHit.Value.collider, out var interactable) &&
                    IsHovering(interactable);
            }

            return true;
        }

        /// <inheritdoc />
        public virtual void UpdateUIModel(ref TrackedDeviceModel model)
        {
            if (!isActiveAndEnabled || m_SamplePoints == null || this.IsBlockedByInteractionWithinGroup())
            {
                model.Reset(false);
                return;
            }

            model.position = effectiveRayOrigin.position;
            model.orientation = effectiveRayOrigin.rotation;
            model.select = isUISelectActive;
            model.raycastLayerMask = m_RaycastMask;
            model.interactionType = UIInteractionType.Ray;

            var raycastPoints = model.raycastPoints;
            raycastPoints.Clear();

            UpdateSamplePointsIfNecessary();
            var numPoints = m_SamplePoints.Count;
            if (numPoints > 0)
            {
                if (raycastPoints.Capacity < numPoints)
                    raycastPoints.Capacity = numPoints;

                for (var i = 0; i < numPoints; ++i)
                    raycastPoints.Add(m_SamplePoints[i].position);
            }
        }

        /// <inheritdoc />
        public bool TryGetUIModel(out TrackedDeviceModel model)
        {
            return m_RegisteredUIInteractorCache.TryGetUIModel(out model);
        }

        /// <inheritdoc cref="TryGetCurrent3DRaycastHit(out RaycastHit, out int)"/>
        public bool TryGetCurrent3DRaycastHit(out RaycastHit raycastHit)
        {
            return TryGetCurrent3DRaycastHit(out raycastHit, out _);
        }

        /// <summary>
        /// Gets the first 3D ray cast hit, if any ray cast hits are available.
        /// </summary>
        /// <param name="raycastHit">When this method returns, contains the ray cast hit if available; otherwise, the default value.</param>
        /// <param name="raycastEndpointIndex">When this method returns, contains the index of the sample endpoint if a hit occurred.
        /// Otherwise, a value of <c>0</c> if no hit occurred.</param>
        /// <returns>Returns <see langword="true"/> if a hit occurred, implying the ray cast hit information is valid.
        /// Otherwise, returns <see langword="false"/>.</returns>
        public bool TryGetCurrent3DRaycastHit(out RaycastHit raycastHit, out int raycastEndpointIndex)
        {
            if (m_RaycastHitsCount > 0)
            {
                Assert.IsTrue(m_RaycastHits.Length >= m_RaycastHitsCount);
                raycastHit = m_RaycastHits[0];
                raycastEndpointIndex = m_RaycastHitEndpointIndex;
                return true;
            }

            raycastHit = default;
            raycastEndpointIndex = default;
            return false;
        }

        /// <inheritdoc cref="TryGetCurrentUIRaycastResult(out RaycastResult, out int)"/>
        public bool TryGetCurrentUIRaycastResult(out RaycastResult raycastResult)
        {
            return TryGetCurrentUIRaycastResult(out raycastResult, out _);
        }

        /// <summary>
        /// Gets the first UI ray cast result, if any ray cast results are available.
        /// </summary>
        /// <param name="raycastResult">When this method returns, contains the UI ray cast result if available; otherwise, the default value.</param>
        /// <param name="raycastEndpointIndex">When this method returns, contains the index of the sample endpoint if a hit occurred.
        /// Otherwise, a value of <c>0</c> if no hit occurred.</param>
        /// <returns>Returns <see langword="true"/> if a hit occurred, implying the ray cast hit information is valid.
        /// Otherwise, returns <see langword="false"/>.</returns>
        public bool TryGetCurrentUIRaycastResult(out RaycastResult raycastResult, out int raycastEndpointIndex)
        {
            if (TryGetUIModel(out var model) && model.currentRaycast.isValid)
            {
                raycastResult = model.currentRaycast;
                raycastEndpointIndex = model.currentRaycastEndpointIndex;
                return true;
            }

            raycastResult = default;
            raycastEndpointIndex = default;
            return false;
        }

        /// <summary>
        /// Gets the first 3D and UI ray cast hits, if any ray cast hits are available.
        /// </summary>
        /// <param name="raycastHit">When this method returns, contains the ray cast hit if available; otherwise, the default value.</param>
        /// <param name="raycastHitIndex">When this method returns, contains the index of the sample endpoint if a hit occurred.
        /// Otherwise, a value of <c>0</c> if no hit occurred.</param>
        /// <param name="uiRaycastHit">When this method returns, contains the UI ray cast result if available; otherwise, the default value.</param>
        /// <param name="uiRaycastHitIndex">When this method returns, contains the index of the sample endpoint if a hit occurred.
        /// Otherwise, a value of <c>0</c> if no hit occurred.</param>
        /// <param name="isUIHitClosest">When this method returns, contains whether the UI ray cast result was the closest hit.</param>
        /// <returns>Returns <see langword="true"/> if either hit occurred, implying the ray cast hit information is valid.
        /// Otherwise, returns <see langword="false"/>.</returns>
        public bool TryGetCurrentRaycast(
            out RaycastHit? raycastHit,
            out int raycastHitIndex,
            out RaycastResult? uiRaycastHit,
            out int uiRaycastHitIndex,
            out bool isUIHitClosest)
        {
            raycastHit = default;
            uiRaycastHit = default;
            isUIHitClosest = default;

            var hitOccurred = false;

            var hitIndex = int.MaxValue;
            var distance = float.MaxValue;
            if (TryGetCurrent3DRaycastHit(out var raycastHitValue, out raycastHitIndex))
            {
                raycastHit = raycastHitValue;
                hitIndex = raycastHitIndex;
                distance = raycastHitValue.distance;

                hitOccurred = true;
            }

            if (TryGetCurrentUIRaycastResult(out var raycastResultValue, out uiRaycastHitIndex))
            {
                uiRaycastHit = raycastResultValue;

                // Determine if the UI hit is closer than the 3D hit.
                // The ray cast segments are sourced from a polygonal chain of endpoints.
                // Within each segment, this Interactor could have hit either a 3D object or a UI object.
                // The distance is just from the segment start position, not from the origin of the whole curve.
                isUIHitClosest = uiRaycastHitIndex > 0 &&
                    (uiRaycastHitIndex < hitIndex || (uiRaycastHitIndex == hitIndex && raycastResultValue.distance <= distance));

                hitOccurred = true;
            }

            return hitOccurred;
        }

        /// <summary>
        /// Calculates the quadratic Bezier control points used for <see cref="LineType.BezierCurve"/>.
        /// </summary>
        void UpdateBezierControlPoints()
        {
            var forward = effectiveRayOrigin.forward;
            var up = m_ReferenceFrame != null ? m_ReferenceFrame.up : Vector3.up;
            m_ControlPoints[0] = effectiveRayOrigin.position;
            m_ControlPoints[1] = m_ControlPoints[0] + forward * m_ControlPointDistance + up * m_ControlPointHeight;
            m_ControlPoints[2] = m_ControlPoints[0] + forward * m_EndPointDistance + up * m_EndPointHeight;
        }

        static Vector3 SampleQuadraticBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            var u = 1f - t;   // (1 - t)
            var uu = u * u;   // (1 - t)²
            var tt = t * t;   // t²

            // (1 - t)²P₀ + 2(1 - t)tP₁ + t²P₂ where 0 ≤ t ≤ 1
            // u²P₀ + 2utP₁ + t²P₂
            return (uu * p0) +
                (2f * u * t * p1) +
                (tt * p2);
        }

        static Vector3 SampleCubicBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            var u = 1f - t;   // (1 - t)
            var uu = u * u;   // (1 - t)²
            var uuu = uu * u; // (1 - t)³
            var tt = t * t;   // t²
            var ttt = tt * t; // t³

            // (1 - t)³P₀ + 3(1 - t)²tP₁ + 3(1 - t)t²P₂ + t³P₃ where 0 ≤ t ≤ 1
            // u³P₀ + 3u²tP₁ + 3ut²P₂ + t³P₃
            return (uuu * p0) +
                (3f * uu * t * p1) +
                (3f * u * tt * p2) +
                (ttt * p3);
        }

        static void ElevateQuadraticToCubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, out Vector3 c0, out Vector3 c1, out Vector3 c2, out Vector3 c3)
        {
            // A Bezier curve of one degree can be reproduced by one of higher degree.
            // Convert quadratic Bezier curve with control points P₀, P₁, P₂
            // into a cubic Bezier curve with control points C₀, C₁, C₂, C₃.
            // The end points remain the same.
            c0 = p0;
            c1 = p0 + (2f / 3f) * (p1 - p0);
            c2 = p2 + (2f / 3f) * (p1 - p2);
            c3 = p2;
        }

        static Vector3 SampleProjectilePoint(Vector3 initialPosition, Vector3 initialVelocity, Vector3 constantAcceleration, float time)
        {
            // Position of object in constant acceleration is:
            // x(t) = x₀ + v₀t + 0.5at²
            // where x₀ is the position at time 0,
            // v₀ is the velocity vector at time 0,
            // a is the constant acceleration vector
            return initialPosition + initialVelocity * time + constantAcceleration * (0.5f * time * time);
        }

        void CalculateProjectileParameters(out Vector3 initialPosition, out Vector3 initialVelocity, out Vector3 constantAcceleration, out float flightTime)
        {
            initialPosition = effectiveRayOrigin.position;
            initialVelocity = effectiveRayOrigin.forward * m_Velocity;
            var up = m_ReferenceFrame != null ? m_ReferenceFrame.up : Vector3.up;
            var referencePosition = m_ReferenceFrame != null ? m_ReferenceFrame.position : Vector3.zero;
            constantAcceleration = up * -m_Acceleration;

            // Vertical velocity component Vy = v₀sinθ
            // When initial height = 0,
            // Time of flight = 2(initial velocity)(sine of launch angle) / (acceleration) = 2v₀sinθ/g
            // When initial height > 0,
            // Time of flight = [Vy + √(Vy² + 2gh)] / g
            // The additional flight time property is added.
            var vy = m_Velocity * Mathf.Sin(angle * Mathf.Deg2Rad);
            var height = Vector3.Project(referencePosition - initialPosition, up).magnitude + m_AdditionalGroundHeight;
            if (height < 0f)
                flightTime = m_AdditionalFlightTime;
            else if (Mathf.Approximately(height, 0f))
                flightTime = 2f * vy / m_Acceleration + m_AdditionalFlightTime;
            else
                flightTime = (vy + Mathf.Sqrt(vy * vy + 2f * m_Acceleration * height)) / m_Acceleration + m_AdditionalFlightTime;

            flightTime = Mathf.Max(flightTime, 0f);
        }

        static bool TryRead2DAxis(InputAction action, out Vector2 output)
        {
            if (action != null)
            {
                output = action.ReadValue<Vector2>();
                return true;
            }
            output = default;
            return false;
        }

        /// <summary>
        /// Rotates the attach anchor for this interactor. This can be useful to rotate a held object.
        /// </summary>
        /// <param name="anchor">The attach transform of the interactor.</param>
        /// <param name="directionAmount">The rotation amount.</param>
        protected virtual void RotateAnchor(Transform anchor, float directionAmount)
        {
            if (Mathf.Approximately(directionAmount, 0f))
                return;

            var rotateAngle = directionAmount * (m_RotateSpeed * Time.deltaTime);

            if (m_AnchorRotateReferenceFrame != null)
                anchor.Rotate(m_AnchorRotateReferenceFrame.up, rotateAngle, Space.World);
            else
                anchor.Rotate(Vector3.up, rotateAngle);
        }

        /// <summary>
        /// Rotates the attach anchor for this interactor to match a given direction. This can be useful to compute a direction angle for teleportation.
        /// </summary>
        /// <param name="anchor">The attach transform of the interactor.</param>
        /// <param name="direction">The directional input.</param>
        /// <param name="referenceRotation">The reference rotation to define the up axis for rotation.</param>
        protected virtual void RotateAnchor(Transform anchor, Vector2 direction, Quaternion referenceRotation)
        {
            if (Mathf.Approximately(direction.sqrMagnitude, 0f))
                return;

            var rotateAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            var directionalQuaternion = Quaternion.AngleAxis(rotateAngle, Vector3.up);
            anchor.rotation = referenceRotation * directionalQuaternion;
        }

        /// <summary>
        /// Translates the attach anchor for this interactor. This can be useful to move a held object closer or further away from the interactor.
        /// </summary>
        /// <param name="rayOrigin">The starting position and direction of any ray casts.</param>
        /// <param name="anchor">The attach transform of the interactor.</param>
        /// <param name="directionAmount">The translation amount.</param>
        protected virtual void TranslateAnchor(Transform rayOrigin, Transform anchor, float directionAmount)
        {
            if (Mathf.Approximately(directionAmount, 0f))
                return;

            var originPosition = rayOrigin.position;
            var originForward = rayOrigin.forward;

            var resultingPosition = anchor.position + originForward * (directionAmount * m_TranslateSpeed * Time.deltaTime);

            // Check the delta between the origin position and the calculated position.
            // Clamp so it doesn't go further back than the origin position.
            var posInAttachSpace = resultingPosition - originPosition;
            var dotResult = Vector3.Dot(posInAttachSpace, originForward);

            anchor.position = dotResult > 0f ? resultingPosition : originPosition;
        }

        /// <inheritdoc />
        public override void PreprocessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.PreprocessInteractor(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                UpdateSamplePointsIfNecessary();
                if (m_SamplePoints != null && m_SamplePoints.Count >= 2)
                {
                    // Perform ray casts and store the equivalent Bezier curve to the endpoint where a hit occurred (used for blending)
                    UpdateRaycastHits();
                    UpdateUIHitIndex();
                    CreateBezierCurve(m_SamplePoints, closestAnyHitIndex, m_HitChordControlPoints);
                }

                // Determine the Interactables that this Interactor could possibly interact with this frame
                GetValidTargets(m_ValidTargets);

                // Check to see if we have a new hover object.
                // This handles auto select and deselect.
                var nearestObject = (m_ValidTargets.Count > 0) ? m_ValidTargets[0] : null;
                if (nearestObject != currentNearestValidTarget && !hasSelection)
                {
                    currentNearestValidTarget = nearestObject;
                    m_LastTimeHoveredObjectChanged = Time.time;
                    m_PassedHoverTimeToSelect = false;
                }
                else if (!m_PassedHoverTimeToSelect && nearestObject != null)
                {
                    var progressToHoverSelect = Mathf.Clamp01((Time.time - m_LastTimeHoveredObjectChanged) / GetHoverTimeToSelect(currentNearestValidTarget));

                    // If we have a selection and we're processing hover to select, don't allow hover to pass
                    // Selection likely came from non-hover method and we don't want to auto-deselect
                    if (progressToHoverSelect >= 1f && !hasSelection)
                        m_PassedHoverTimeToSelect = true;
                }

                // If we have a selection and interactable is set to auto deselect, process the select time
                if (m_AutoDeselect && hasSelection && !m_PassedTimeToAutoDeselect)
                {
                    var progressToDeselect = Mathf.Clamp01((Time.time - m_LastTimeAutoSelected) / GetTimeToAutoDeselect(currentNearestValidTarget));
                    if (progressToDeselect >= 1f)
                        m_PassedTimeToAutoDeselect = true;
                }
            }
        }

        /// <inheritdoc />
        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractor(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                // Update the pose of the attach point
                if (m_AllowAnchorControl && hasSelection)
                {
                    var ctrl = xrController as XRController;
                    if (ctrl != null && ctrl.inputDevice.isValid)
                    {
                        ctrl.inputDevice.IsPressed(ctrl.moveObjectIn, out var inPressed, ctrl.axisToPressThreshold);
                        ctrl.inputDevice.IsPressed(ctrl.moveObjectOut, out var outPressed, ctrl.axisToPressThreshold);

                        if (inPressed || outPressed)
                        {
                            var directionAmount = inPressed ? 1f : -1f;
                            TranslateAnchor(effectiveRayOrigin, attachTransform, directionAmount);
                        }

                        switch (m_AnchorRotationMode)
                        {
                            case AnchorRotationMode.RotateOverTime:
                                ctrl.inputDevice.IsPressed(ctrl.rotateObjectLeft, out var leftPressed, ctrl.axisToPressThreshold);
                                ctrl.inputDevice.IsPressed(ctrl.rotateObjectRight, out var rightPressed, ctrl.axisToPressThreshold);
                                if (leftPressed || rightPressed)
                                {
                                    var directionAmount = leftPressed ? -1f : 1f;
                                    RotateAnchor(attachTransform, directionAmount);
                                }
                                break;

                            case AnchorRotationMode.MatchDirection:
                                if (ctrl.inputDevice.TryReadAxis2DValue(ctrl.directionalAnchorRotation, out var directionalValue))
                                {
                                    var referenceRotation = m_AnchorRotateReferenceFrame != null
                                        ? m_AnchorRotateReferenceFrame.rotation
                                        : GetAttachPoseOnSelect(interactablesSelected[0]).rotation;

                                    RotateAnchor(attachTransform, directionalValue, referenceRotation);
                                }
                                break;

                            default:
                                Assert.IsTrue(false, $"Unhandled {nameof(AnchorRotationMode)}={m_AnchorRotationMode}.");
                                break;
                        }
                    }

                    var actionBasedController = xrController as ActionBasedController;
                    if (actionBasedController != null)
                    {
                        switch (m_AnchorRotationMode)
                        {
                            case AnchorRotationMode.RotateOverTime:
                                if (TryRead2DAxis(actionBasedController.rotateAnchorAction.action, out var rotateAmt))
                                    RotateAnchor(attachTransform, rotateAmt.x);
                                break;

                            case AnchorRotationMode.MatchDirection:
                                if (TryRead2DAxis(actionBasedController.directionalAnchorRotationAction.action, out var directionAmt))
                                {
                                    var referenceRotation = m_AnchorRotateReferenceFrame != null
                                        ? m_AnchorRotateReferenceFrame.rotation
                                        : GetAttachPoseOnSelect(interactablesSelected[0]).rotation;

                                    RotateAnchor(attachTransform, directionAmt, referenceRotation);
                                }
                                break;

                            default:
                                Assert.IsTrue(false, $"Unhandled {nameof(AnchorRotationMode)}={m_AnchorRotationMode}.");
                                break;
                        }

                        if (TryRead2DAxis(actionBasedController.translateAnchorAction.action, out var translateAmt))
                        {
                            TranslateAnchor(effectiveRayOrigin, attachTransform, translateAmt.y);
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        public override void GetValidTargets(List<IXRInteractable> targets)
        {
            targets.Clear();

            if (!isActiveAndEnabled)
                return;

            if (m_RaycastHitsCount > 0)
            {
                var hasUIHit = TryGetCurrentUIRaycastResult(out var uiRaycastResult, out var uiHitIndex);
                for (var i = 0; i < m_RaycastHitsCount; ++i)
                {
                    var raycastHit = m_RaycastHits[i];

                    // A hit on UI should block Interactables behind it from being a valid target
                    if (hasUIHit && uiHitIndex > 0 && (uiHitIndex < m_RaycastHitEndpointIndex || (uiHitIndex == m_RaycastHitEndpointIndex && uiRaycastResult.distance <= raycastHit.distance)))
                        break;

                    // A hit on geometry not associated with Interactables should block Interactables behind it from being a valid target
                    if (!interactionManager.TryGetInteractableForCollider(raycastHit.collider, out var interactable))
                        break;

                    if (!targets.Contains(interactable))
                    {
                        targets.Add(interactable);

                        // Stop after the first if enabled
                        if (m_HitClosestOnly)
                            break;
                    }
                }
            }

            var filter = targetFilter;
            if (filter != null && filter.canProcess)
            {
                filter.Process(this, targets, s_Results);

                // Copy results elements to targets
                targets.Clear();
                targets.AddRange(s_Results);
            }
        }

        void CreateSamplePointsListsIfNecessary()
        {
            if (m_SamplePoints != null && s_ScratchSamplePoints != null)
                return;

            var capacity = m_LineType == LineType.StraightLine ? 2 : m_SampleFrequency;

            m_SamplePoints ??= new List<SamplePoint>(capacity);
            s_ScratchSamplePoints ??= new List<SamplePoint>(capacity);
        }

        /// <summary>
        /// Update curve approximation used for ray casts for this frame.
        /// </summary>
        /// <remarks>
        /// This method is called first by <see cref="UpdateUIModel"/> due to the UI Input Module
        /// before <see cref="PreprocessInteractor"/> gets called later in the frame, so this
        /// method is a performance optimization so it only gets done once each frame.
        /// </remarks>
        void UpdateSamplePointsIfNecessary()
        {
            CreateSamplePointsListsIfNecessary();
            if (m_SamplePointsFrameUpdated != Time.frameCount)
            {
                UpdateSamplePoints(m_SampleFrequency, m_SamplePoints);
                m_SamplePointsFrameUpdated = Time.frameCount;
            }
        }

        /// <summary>
        /// Approximates the curve into a polygonal chain of endpoints, whose line segments can be used as
        /// the rays for doing Physics ray casts.
        /// </summary>
        /// <param name="count">The number of sample points to calculate.</param>
        /// <param name="samplePoints">The result list of sample points to populate.</param>
        void UpdateSamplePoints(int count, List<SamplePoint> samplePoints)
        {
            Assert.IsTrue(count >= 2);
            Assert.IsNotNull(samplePoints);

            samplePoints.Clear();
            var samplePoint = new SamplePoint
            {
                position = effectiveRayOrigin.position,
                parameter = 0f,
            };
            samplePoints.Add(samplePoint);

            switch (m_LineType)
            {
                case LineType.StraightLine:
                    samplePoint.position = samplePoints[0].position + effectiveRayOrigin.forward * m_MaxRaycastDistance;
                    samplePoint.parameter = 1f;
                    samplePoints.Add(samplePoint);
                    break;
                case LineType.ProjectileCurve:
                {
                    CalculateProjectileParameters(out var initialPosition, out var initialVelocity, out var constantAcceleration, out var flightTime);

                    var interval = flightTime / (count - 1);
                    for (var i = 1; i < count; ++i)
                    {
                        var time = i * interval;
                        samplePoint.position = SampleProjectilePoint(initialPosition, initialVelocity, constantAcceleration, time);
                        samplePoint.parameter = time;
                        samplePoints.Add(samplePoint);
                    }
                }
                    break;
                case LineType.BezierCurve:
                {
                    // Update control points for Bezier curve
                    UpdateBezierControlPoints();
                    var p0 = m_ControlPoints[0];
                    var p1 = m_ControlPoints[1];
                    var p2 = m_ControlPoints[2];

                    var interval = 1f / (count - 1);
                    for (var i = 1; i < count; ++i)
                    {
                        // Parametric parameter t where 0 ≤ t ≤ 1
                        var percent = i * interval;
                        samplePoint.position = SampleQuadraticBezierPoint(p0, p1, p2, percent);
                        samplePoint.parameter = percent;
                        samplePoints.Add(samplePoint);
                    }
                }
                    break;
            }
        }

        /// <summary>
        /// Walks the line segments from the approximated curve, casting from one endpoint to the next.
        /// </summary>
        void UpdateRaycastHits()
        {
            m_RaycastHitsCount = 0;
            m_RaycastHitEndpointIndex = 0;

            for (var i = 1; i < m_SamplePoints.Count; ++i)
            {
                var fromPoint = m_SamplePoints[i - 1].position;
                var toPoint = m_SamplePoints[i].position;

                CheckCollidersBetweenPoints(fromPoint, toPoint);
                if (m_RaycastHitsCount > 0)
                {
                    m_RaycastHitEndpointIndex = i;
                    break;
                }
            }
        }

        void CheckCollidersBetweenPoints(Vector3 from, Vector3 to)
        {
            Array.Clear(m_RaycastHits, 0, k_MaxRaycastHits);

            var direction = (to - from).normalized;
            var maxDistance = Vector3.Distance(to, from);
            var queryTriggerInteraction = m_RaycastSnapVolumeInteraction == QuerySnapVolumeInteraction.Collide
                ? QueryTriggerInteraction.Collide
                : m_RaycastTriggerInteraction;

            if (m_HitDetectionType == HitDetectionType.SphereCast && m_SphereCastRadius > 0f)
            {
                m_RaycastHitsCount = m_LocalPhysicsScene.SphereCast(from, m_SphereCastRadius, direction,
                    m_RaycastHits, maxDistance, m_RaycastMask, queryTriggerInteraction);
            }
            else
            {
                m_RaycastHitsCount = m_LocalPhysicsScene.Raycast(from, direction,
                    m_RaycastHits, maxDistance, m_RaycastMask, queryTriggerInteraction);
            }

            if (m_RaycastHitsCount > 0)
            {
                var baseQueryHitsTriggers = m_RaycastTriggerInteraction == QueryTriggerInteraction.Collide ||
                    (m_RaycastTriggerInteraction == QueryTriggerInteraction.UseGlobal && Physics.queriesHitTriggers);

                if (m_RaycastSnapVolumeInteraction == QuerySnapVolumeInteraction.Ignore && baseQueryHitsTriggers)
                {
                    // Filter out Snap Volume trigger collider hits
                    m_RaycastHitsCount = FilterTriggerColliders(interactionManager, m_RaycastHits, m_RaycastHitsCount, snapVolume => snapVolume != null);
                }
                else if (m_RaycastSnapVolumeInteraction == QuerySnapVolumeInteraction.Collide && !baseQueryHitsTriggers)
                {
                    // Filter out trigger collider hits that are not Snap Volume snap colliders
                    m_RaycastHitsCount = FilterTriggerColliders(interactionManager, m_RaycastHits, m_RaycastHitsCount, snapVolume => snapVolume == null);
                }

                // Sort all the hits by distance along the curve since the results of the 3D ray cast are not ordered.
                // Sorting is done after filtering above for performance.
                SortingHelpers.Sort(m_RaycastHits, m_RaycastHitComparer, m_RaycastHitsCount);
            }
        }

        static int FilterTriggerColliders(XRInteractionManager interactionManager, RaycastHit[] raycastHits, int count, Func<XRInteractableSnapVolume, bool> removeRule)
        {
            for (var index = 0; index < count; ++index)
            {
                var hitCollider = raycastHits[index].collider;
                if (hitCollider.isTrigger)
                {
                    interactionManager.TryGetInteractableForCollider(hitCollider, out _, out var snapVolume);
                    if (removeRule(snapVolume))
                    {
                        RemoveAt(raycastHits, index, count);
                        --count;
                        --index;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Remove the array element by shifting the remaining elements down by one index.
        /// This does not resize the length of the array.
        /// </summary>
        /// <typeparam name="T">The struct type.</typeparam>
        /// <param name="array">The array to modify.</param>
        /// <param name="index">The index of the array element to effectively remove.</param>
        /// <param name="count">The number of elements contained in the array, which may be less than the array length.</param>
        static void RemoveAt<T>(T[] array, int index, int count) where T : struct
        {
            Array.Copy(array, index + 1, array, index, count - index - 1);
            Array.Clear(array, count - 1, 1);
        }

        void UpdateUIHitIndex()
        {
            TryGetCurrentUIRaycastResult(out _, out m_UIRaycastHitEndpointIndex);
        }

        void CreateBezierCurve(List<SamplePoint> samplePoints, int endSamplePointIndex, Vector3[] quadraticControlPoints)
        {
            // Convert the ray cast curve ranging from the controller to the sample endpoint
            // where the hit occurred into a quadratic Bezier curve
            // with control points P₀, P₁, P₂.
            var endSamplePoint = endSamplePointIndex > 0 && endSamplePointIndex < samplePoints.Count
                ? samplePoints[endSamplePointIndex]
                : samplePoints[samplePoints.Count - 1];
            var p2 = endSamplePoint.position;
            var p0 = samplePoints[0].position;

            var midpoint = 0.5f * (p0 + p2);

            switch (m_LineType)
            {
                case LineType.StraightLine:
                    quadraticControlPoints[0] = p0;
                    quadraticControlPoints[1] = midpoint;
                    quadraticControlPoints[2] = p2;
                    break;
                case LineType.ProjectileCurve:
                    CalculateProjectileParameters(out _, out var initialVelocity, out var constantAcceleration, out _);

                    var midTime = 0.5f * endSamplePoint.parameter;
                    var sampleMidTime = SampleProjectilePoint(p0, initialVelocity, constantAcceleration, midTime);
                    var p1 = midpoint + 2f * (sampleMidTime - midpoint);

                    quadraticControlPoints[0] = p0;
                    quadraticControlPoints[1] = p1;
                    quadraticControlPoints[2] = p2;
                    break;
                case LineType.BezierCurve:
                    Assert.IsTrue(m_ControlPoints[0] == p0);
                    quadraticControlPoints[0] = m_ControlPoints[0];
                    quadraticControlPoints[1] = m_ControlPoints[1];
                    quadraticControlPoints[2] = m_ControlPoints[2];
                    break;
            }
        }

        /// <inheritdoc />
        public override bool isSelectActive
        {
            get
            {
                if (m_HoverToSelect && m_PassedHoverTimeToSelect)
                    return allowSelect;

                return base.isSelectActive;
            }
        }

        /// <inheritdoc />
        public override bool CanHover(IXRHoverInteractable interactable)
        {
            return base.CanHover(interactable) && (!hasSelection || IsSelecting(interactable));
        }

        /// <inheritdoc />
        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            if (currentNearestValidTarget == interactable && m_AutoDeselect && hasSelection && m_PassedHoverTimeToSelect && m_PassedTimeToAutoDeselect)
                return false;

            if (m_HoverToSelect && m_PassedHoverTimeToSelect && currentNearestValidTarget != interactable)
                return false;

            return base.CanSelect(interactable) && (!hasSelection || IsSelecting(interactable));
        }

        /// <summary>
        /// Gets the number of seconds for which this interactor must hover over the interactable to select it if <see cref="hoverToSelect"/> is enabled.
        /// </summary>
        /// <param name="interactable">The interactable to get the duration for.</param>
        /// <returns>Returns the number of seconds for which this Interactor must hover over an Interactable to select it.</returns>
        /// <seealso cref="hoverTimeToSelect"/>
        protected virtual float GetHoverTimeToSelect(IXRInteractable interactable) => m_HoverTimeToSelect;

        /// <summary>
        /// Gets the number of seconds for which this interactor will keep the interactable selected before automatically deselecting it.
        /// </summary>
        /// <param name="interactable">The interactable to get the duration for.</param>
        /// <returns>Returns the number of seconds for which this Interactor will keep an Interactable selected before automatically deselecting it.</returns>
        /// <seealso cref="timeToAutoDeselect"/>
        protected virtual float GetTimeToAutoDeselect(IXRInteractable interactable) => m_TimeToAutoDeselect;

        /// <inheritdoc />
        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            base.OnSelectEntering(args);

            // Update when selecting via hover to select
            if (m_AutoDeselect && m_PassedHoverTimeToSelect)
            {
                m_LastTimeAutoSelected = Time.time;
                m_PassedTimeToAutoDeselect = false;
            }

            if (!m_UseForceGrab && interactablesSelected.Count == 1 && TryGetCurrent3DRaycastHit(out var raycastHit))
                attachTransform.position = raycastHit.point;
        }

        /// <inheritdoc />
        protected override void OnSelectExiting(SelectExitEventArgs args)
        {
            base.OnSelectExiting(args);

            // Reset to allow stop hover from automatically selecting again after auto deselect
            m_PassedHoverTimeToSelect = false;

            // Reset the auto select/deselect properties to allow this Interactor to select again after select exit
            m_LastTimeHoveredObjectChanged = Time.time;
            m_PassedTimeToAutoDeselect = false;

            if (!hasSelection)
                RestoreAttachTransform();
        }

        void RestoreAttachTransform()
        {
            var pose = GetLocalAttachPoseOnSelect(firstInteractableSelected);
            attachTransform.localPosition = pose.position;
            attachTransform.localRotation = pose.rotation;
        }

        static int SanitizeSampleFrequency(int value)
        {
            // Upper range does not need to be enforced, just the minimum.
            // The max const just provides a reasonable slider range.
            return Mathf.Max(value, k_MinSampleFrequency);
        }

        /// <summary>
        /// A point within a polygonal chain of endpoints which form line segments
        /// to approximate the curve. Each line segment is where the ray cast starts and ends.
        /// </summary>
        struct SamplePoint
        {
            /// <summary>
            /// The world space position of the sample.
            /// </summary>
            public Vector3 position { get; set; }

            /// <summary>
            /// For <see cref="LineType.ProjectileCurve"/>, this represents flight time at the sample.
            /// For <see cref="LineType.BezierCurve"/> and <see cref="LineType.StraightLine"/>, this represents
            /// the parametric parameter <i>t</i> of the curve at the sample (with range [0, 1]).
            /// </summary>
            public float parameter { get; set; }
        }
    }
}