using System;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Interprets feature values on a tracked input controller device into XR Interaction states, such as Select.
    /// Additionally, it applies the current pose value of a tracked device to the transform of the GameObject.
    /// </summary>
    /// <seealso cref="ActionBasedController"/>
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_Controllers)]
    [DisallowMultipleComponent]
    [Obsolete(k_XRBaseControllerDeprecated, true)]
    public abstract class XRBaseController : MonoBehaviour, IXRHapticImpulseProvider
    {
        const string k_XRBaseControllerDeprecated = "XRBaseController has been deprecated in version 3.0.0. Its functionality has been distributed into different components.";

        /// <summary>
        /// The time within the frame that controller pose will be sampled.
        /// </summary>
        /// <seealso cref="updateTrackingType"/>
        public enum UpdateType
        {
            /// <summary>
            /// Sample input at both update and directly before rendering. For smooth controller pose tracking,
            /// we recommend using this value as it will provide the lowest input latency for the device.
            /// This is the default value for the UpdateType option.
            /// </summary>
            UpdateAndBeforeRender,

            /// <summary>
            /// Only sample input during the update phase of the frame.
            /// </summary>
            Update,

            /// <summary>
            /// Only sample input directly before rendering.
            /// </summary>
            BeforeRender,

            /// <summary>
            /// Sample input corresponding to <see cref="FixedUpdate"/>.
            /// </summary>
            Fixed,
        }

        /// <summary>
        /// The time within the frame that the controller samples tracking input.
        /// </summary>
        /// <seealso cref="UpdateType"/>
        public UpdateType updateTrackingType
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Whether input pose tracking is enabled for this controller.
        /// When enabled, Unity reads the current tracking pose input of the controller device each frame.
        /// </summary>
        /// <remarks>
        /// You can disable this in order to drive the controller state manually instead of from reading current inputs,
        /// such as when playing back recorded pose inputs.
        /// </remarks>
        /// <seealso cref="enableInputActions"/>
        public bool enableInputTracking
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Whether input for XR Interaction events is enabled for this controller.
        /// When enabled, Unity reads the current input of the controller device each frame.
        /// </summary>
        /// <remarks>
        /// You can disable this in order to drive the controller state manually instead of from reading current inputs,
        /// such as when playing back recorded inputs.
        /// </remarks>
        /// <seealso cref="enableInputTracking"/>
        public bool enableInputActions
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The prefab of a controller model to show for this controller that this behavior automatically instantiates.
        /// </summary>
        /// <remarks>
        /// This behavior automatically instantiates an instance of the prefab as a child
        /// of <see cref="modelParent"/> upon startup unless <see cref="model"/> is already set,
        /// in which case this value is ignored.
        /// </remarks>
        /// <seealso cref="model"/>
        public Transform modelPrefab
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The transform that this behavior uses as the parent for the model prefab when it is instantiated.
        /// </summary>
        /// <remarks>
        /// Automatically instantiated and set in <see cref="Awake"/> if not already set.
        /// Setting this will not automatically destroy the previous object.
        /// </remarks>
        public Transform modelParent
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The instance of the controller model in the scene. You can set this to an existing object instead of using <see cref="modelPrefab"/>.
        /// </summary>
        /// <remarks>
        /// If set, it should reference a child GameObject of this behavior so it will update with the controller pose.
        /// </remarks>
        public Transform model
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Whether to animate the model in response to interaction events. When enabled, the animation trigger will be set for the corresponding
        /// animator component on the controller model when a select or deselect interaction events occurs.
        /// </summary>
        /// <seealso cref="modelSelectTransition"/>
        /// <seealso cref="modelDeSelectTransition"/>
        public bool animateModel
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The animation trigger name to activate upon selecting.
        /// </summary>
        /// <seealso cref="Animator.SetTrigger(string)"/>
        public string modelSelectTransition
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The animation trigger name to activate upon deselecting.
        /// </summary>
        /// <seealso cref="Animator.SetTrigger(string)"/>
        public string modelDeSelectTransition
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Whether to hide the controller model.
        /// </summary>
        /// <seealso cref="model"/>
        /// <seealso cref="UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor.hideControllerOnSelect"/>
        public bool hideControllerModel
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Read Only) The current select interaction state.
        /// </summary>
        public InteractionState selectInteractionState => default;

        /// <summary>
        /// (Read Only) The current activate interaction state.
        /// </summary>
        public InteractionState activateInteractionState => default;

        /// <summary>
        /// (Read Only) The current UI press interaction state.
        /// </summary>
        public InteractionState uiPressInteractionState => default;

        /// <summary>
        /// (Read Only) The current UI scroll value.
        /// </summary>
        public Vector2 uiScrollValue => default;

        /// <summary>
        /// The current state of the controller.
        /// </summary>
        public XRControllerState currentControllerState
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Awake()
        {
            throw new NotSupportedException(k_XRBaseControllerDeprecated);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDisable()
        {
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Update()
        {
        }

        /// <summary>
        /// Gets the prefab that should be instantiated upon startup.
        /// </summary>
        /// <returns>Returns the prefab that should be instantiated upon startup.</returns>
        protected virtual GameObject GetModelPrefab()
        {
            throw new NotSupportedException(k_XRBaseControllerDeprecated);
        }

        /// <summary>
        /// Updates the controller every frame.
        /// This is called automatically from <see cref="Update"/>.
        /// </summary>
        protected virtual void UpdateController()
        {
            throw new NotSupportedException(k_XRBaseControllerDeprecated);
        }

        /// <summary>
        /// This method is automatically called for "Just Before Render" input updates for VR devices.
        /// </summary>
        /// <seealso cref="Application.onBeforeRender"/>
        protected virtual void OnBeforeRender()
        {
            throw new NotSupportedException(k_XRBaseControllerDeprecated);
        }

        /// <summary>
        /// Corresponds to <see cref="FixedUpdate"/>. It has the frequency of the physics system and is called
        /// every fixed framerate frame.
        /// </summary>
        protected virtual void FixedUpdate()
        {
        }

        /// <summary>
        /// Applies the given controller state to this <see cref="XRBaseController"/>.
        /// Depending on the update phase, the XR Interaction states may be copied
        /// and/or the pose value may be applied to the transform of the GameObject.
        /// Unity calls this automatically from <see cref="FixedUpdate"/>, <see cref="OnBeforeRender"/>, and <see cref="UpdateController"/>.
        /// </summary>
        /// <param name="updatePhase">The update phase during this call.</param>
        /// <param name="controllerState">The state of the controller to apply.</param>
        protected virtual void ApplyControllerState(XRInteractionUpdateOrder.UpdatePhase updatePhase, XRControllerState controllerState)
        {
            throw new NotSupportedException(k_XRBaseControllerDeprecated);
        }

        /// <summary>
        /// Updates the pose values in the given controller state based on the current tracking input of the controller device.
        /// Unity calls this automatically from <see cref="FixedUpdate"/>, <see cref="OnBeforeRender"/>, and <see cref="UpdateController"/> so explicit calls
        /// to this function are not required.
        /// </summary>
        /// <param name="controllerState">The state of the controller.</param>
        protected virtual void UpdateTrackingInput(XRControllerState controllerState)
        {
            throw new NotSupportedException(k_XRBaseControllerDeprecated);
        }

        /// <summary>
        /// Updates the XR Interaction states in the given controller state based on the current inputs of the controller device.
        /// Unity calls this automatically during <see cref="UpdateController"/> so explicit calls to this function are not required.
        /// </summary>
        /// <param name="controllerState">The state of the controller.</param>
        protected virtual void UpdateInput(XRControllerState controllerState)
        {
            throw new NotSupportedException(k_XRBaseControllerDeprecated);
        }

        /// <summary>
        /// Updates the animation on the model instance (if the model contains an <see cref="Animator"/>).
        /// Unity calls this automatically from <see cref="UpdateController"/>.
        /// </summary>
        /// <seealso cref="animateModel"/>
        /// <seealso cref="modelSelectTransition"/>
        /// <seealso cref="modelDeSelectTransition"/>
        protected virtual void UpdateControllerModelAnimation()
        {
            throw new NotSupportedException(k_XRBaseControllerDeprecated);
        }

        /// <summary>
        /// Play a haptic impulse on the controller if one is available.
        /// </summary>
        /// <param name="amplitude">Amplitude (from 0.0 to 1.0) to play impulse at.</param>
        /// <param name="duration">Duration (in seconds) to play haptic impulse.</param>
        /// <returns>Returns <see langword="true"/> if successful. Otherwise, returns <see langword="false"/>.</returns>
        public virtual bool SendHapticImpulse(float amplitude, float duration)
        {
            throw new NotSupportedException(k_XRBaseControllerDeprecated);
        }

        /// <inheritdoc />
        IXRHapticImpulseChannelGroup IXRHapticImpulseProvider.GetChannelGroup()
        {
            throw new NotSupportedException(k_XRBaseControllerDeprecated);
        }
    }
}
