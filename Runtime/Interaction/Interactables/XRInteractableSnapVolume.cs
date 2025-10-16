using System;
using System.Diagnostics;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Interactables
{
    /// <summary>
    /// Utility component for supporting interactors snapping to and selecting interactables.
    /// Add this component to a child GameObject of the interactable.
    /// </summary>
    /// <remarks>
    /// Currently supports one collider as the snapping volume. To support multiple snap colliders for a single interactable,
    /// add multiple components with each using a different collider.
    /// </remarks>
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
    [AddComponentMenu("XR/XR Interactable Snap Volume", 11)]
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_InteractableSnapVolume)]
    [HelpURL(XRHelpURLConstants.k_XRInteractableSnapVolume)]
    public class XRInteractableSnapVolume : MonoBehaviour
    {
        /// <summary>
        /// Calls the methods in its invocation list when this snap volume is registered with an <see cref="XRInteractionManager"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="InteractableSnapVolumeRegisteredEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="XRInteractionManager.snapVolumeRegistered"/>
        public event Action<InteractableSnapVolumeRegisteredEventArgs> registered;

        /// <summary>
        /// Calls the methods in its invocation list when this snap volume is unregistered from an <see cref="XRInteractionManager"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="InteractableSnapVolumeUnregisteredEventArgs"/> passed to each listener is only valid while the event is invoked,
        /// do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="XRInteractionManager.snapVolumeUnregistered"/>
        public event Action<InteractableSnapVolumeUnregisteredEventArgs> unregistered;

        [SerializeField]
        XRInteractionManager m_InteractionManager;

        /// <summary>
        /// The <see cref="XRInteractionManager"/> that this snap volume will communicate with (will find one if <see langword="null"/>).
        /// </summary>
        public XRInteractionManager interactionManager
        {
            get => m_InteractionManager;
            set
            {
                m_InteractionManager = value;
                if (Application.isPlaying && isActiveAndEnabled)
                    RegisterWithInteractionManager(value);
            }
        }

        [SerializeField]
        [RequireInterface(typeof(IXRInteractable))]
        Object m_InteractableObject;

        /// <summary>
        /// The <see cref="IXRInteractable"/> associated with this <see cref="XRInteractableSnapVolume"/> serialized as a Unity <see cref="Object"/>.
        /// If not set, Unity will find it up the hierarchy.
        /// </summary>
        /// <remarks>
        /// Use this for Unity Editor scripting. Use <see cref="interactable"/> to change the interactable at runtime.
        /// </remarks>
        public Object interactableObject
        {
            get => m_InteractableObject;
            set
            {
                m_InteractableObject = value;
                interactable = value as IXRInteractable;
            }
        }

        [SerializeField]
        Collider m_SnapCollider;

        /// <summary>
        /// The trigger collider to associate with the interactable when it is hit/collided.
        /// Rays will snap from this to the <see cref="snapToCollider"/>.
        /// </summary>
        /// <remarks>
        /// This should be larger than or positioned away from the <see cref="snapToCollider"/>.
        /// Changing this value at runtime does not alter the enabled state of the previous collider.
        /// </remarks>
        /// <seealso cref="snapToCollider"/>
        public Collider snapCollider
        {
            get => m_SnapCollider;
            set
            {
                if (m_SnapCollider == value)
                    return;

                if (Application.isPlaying && isActiveAndEnabled)
                {
                    // Update the collider to snap volume mapping.
                    // Must wait to modify value until after unregistered since the manager currently
                    // requires it to remove the dictionary entry.
                    UnregisterWithInteractionManager();

                    m_SnapCollider = value;
                    ValidateSnapCollider();
                    RefreshSnapColliderEnabled();

                    RegisterWithInteractionManager(m_InteractionManager);
                }
                else
                {
                    m_SnapCollider = value;
                }
            }
        }

        [SerializeField]
        bool m_DisableSnapColliderWhenSelected = true;

        /// <summary>
        /// Automatically disable or enable the Snap Collider when the interactable is selected or deselected.
        /// </summary>
        /// <remarks>
        /// This behavior will always automatically disable the Snap Collider when this behavior is disabled.
        /// </remarks>
        public bool disableSnapColliderWhenSelected
        {
            get => m_DisableSnapColliderWhenSelected;
            set
            {
                m_DisableSnapColliderWhenSelected = value;
                if (Application.isPlaying && isActiveAndEnabled)
                    RefreshSnapColliderEnabled();
            }
        }

        [SerializeField]
        Collider m_SnapToCollider;

        /// <summary>
        /// (Optional) The collider that will be used to find the closest point to snap to. If this is <see langword="null"/>,
        /// then the associated <see cref="IXRInteractable"/> transform's position or this GameObject's transform position
        /// will be used as the snap point.
        /// </summary>
        /// <seealso cref="snapCollider"/>
        public Collider snapToCollider
        {
            get => m_SnapToCollider;
            set => m_SnapToCollider = value;
        }

        IXRInteractable m_Interactable;

        /// <summary>
        /// The runtime <see cref="IXRInteractable"/> associated with this <see cref="XRInteractableSnapVolume"/>.
        /// </summary>
        public IXRInteractable interactable
        {
            get => m_Interactable;
            set
            {
                m_Interactable = value;
                m_InteractableObject = value as Object;
                if (Application.isPlaying && isActiveAndEnabled)
                    SetBoundInteractable(value);
            }
        }

        IXRInteractable m_BoundInteractable;
        IXRSelectInteractable m_BoundSelectInteractable;

        XRInteractionManager m_RegisteredInteractionManager;

        // Used to avoid GC Alloc from delegate object creation if passing the method directly
        Action<XRInteractionManager> m_RegisterWithInteractionManager;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        protected virtual void Reset()
        {
#if UNITY_EDITOR
            m_InteractableObject = GetComponentInParent<IXRInteractable>() as Object;
            m_SnapCollider = FindSnapCollider(gameObject);
            if (m_InteractableObject != null)
            {
                // Initialize with a Collider component on the Interactable
                var col = ((IXRInteractable)m_InteractableObject).transform.GetComponent<Collider>();
                if (col != null && col.enabled && !col.isTrigger)

                    m_SnapToCollider = col;
            }
#endif
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Awake()
        {
            if (m_SnapCollider == null)
                m_SnapCollider = FindSnapCollider(gameObject);

            ValidateSnapCollider();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnEnable()
        {
            if (m_InteractionManager != null)
                RegisterWithInteractionManager(m_InteractionManager);
            else
            {
                // This is an optimization to not search for the manager if both manager modes are Manual
                // since it won't create one and will just add to the waitlist during the callback anyway.
                var runtimeSettings = XRInteractionRuntimeSettings.Instance;
                if (runtimeSettings.managerRegistrationMode == XRInteractionRuntimeSettings.ManagerRegistrationMode.Manual &&
                    runtimeSettings.managerCreationMode == XRInteractionRuntimeSettings.ManagerCreationMode.Manual)
                {
                    XRInteractionManager.RegisterWithWaitlist(this);
                }
                else
                {
                    ComponentLocatorUtility<XRInteractionManager>.FindComponentDeferred(
                        m_RegisterWithInteractionManager ??= RegisterWithInteractionManager,
                        createComponent: runtimeSettings.managerCreationMode == XRInteractionRuntimeSettings.ManagerCreationMode.CreateAutomatically,
                        dontDestroyOnLoad: true);
                }
            }

            // Try to find interactable in parent if necessary
            if (m_InteractableObject != null && m_InteractableObject is IXRInteractable serializedInteractable)
                interactable = serializedInteractable;
            else
                interactable = m_Interactable ??= GetComponentInParent<IXRInteractable>();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDisable()
        {
            XRInteractionManager.UnregisterFromWaitlist(this);
            UnregisterWithInteractionManager();

            SetBoundInteractable(null);
            SetSnapColliderEnabled(false);
        }

        void RegisterWithInteractionManager(XRInteractionManager manager)
        {
            if (!isActiveAndEnabled)
                return;

            // Skip if already registered with this manager to avoid a needless unregister/re-register.
            if (m_RegisteredInteractionManager == manager && m_RegisteredInteractionManager != null)
                return;

            // Skip if this manager differs from the field, which can happen if it was set during the find operation.
            if (m_InteractionManager != manager && m_InteractionManager != null)
                return;

            if (XRInteractionRuntimeSettings.Instance.managerRegistrationMode == XRInteractionRuntimeSettings.ManagerRegistrationMode.Manual &&
                m_InteractionManager == null)
            {
                XRInteractionManager.RegisterWithWaitlist(this);
                return;
            }

            UnregisterWithInteractionManager();

            if (manager != null)
                manager.RegisterSnapVolume(this);
            else
                XRInteractionManager.RegisterWithWaitlist(this);
        }

        void UnregisterWithInteractionManager()
        {
            if (m_RegisteredInteractionManager != null)
                m_RegisteredInteractionManager.UnregisterSnapVolume(this);
        }

        /// <summary>
        /// This method is responsible for finding a valid <see cref="Collider"/> component available.
        /// available. The first valid collider found will be used as the <see cref="snapCollider"/>.
        /// </summary>
        /// <param name="gameObject">The <see cref="GameObject"/> to find the <see cref="Collider"/> component for.</param>
        /// <returns>The best snap collider candidate for the provided <see cref="GameObject"/>.</returns>
        /// <remarks>
        /// The snap collider must be a trigger collider, so the collider type
        /// can only be a <see cref="BoxCollider"/>, <see cref="SphereCollider"/>, <see cref="CapsuleCollider"/>, or convex <see cref="MeshCollider"/>.
        /// </remarks>
        /// <seealso cref="MeshCollider.convex"/>
        /// <seealso cref="snapCollider"/>
        protected static Collider FindSnapCollider(GameObject gameObject)
        {
            Collider bestCandidate = null;

            // If multiple colliders are found on this object, take the first valid one as the snap collider.
            var colliders = gameObject.GetComponents<Collider>();
            for (var index = 0; index < colliders.Length; ++index)
            {
                var colliderCandidate = colliders[index];
                if (SupportsTriggerCollider(colliderCandidate))
                {
                    if (colliderCandidate.isTrigger)
                        return colliderCandidate;

                    if (bestCandidate == null)
                        bestCandidate = colliderCandidate;
                }
            }

            return bestCandidate;
        }

        /// <summary>
        /// Returns whether the given collider supports enabling <see cref="Collider.isTrigger"/>.
        /// </summary>
        /// <param name="col">The collider to check.</param>
        /// <returns>Returns <see langword="true"/> if the collider supports being a trigger collider.</returns>
        internal static bool SupportsTriggerCollider(Collider col)
        {
            return col is BoxCollider ||
                col is SphereCollider ||
                col is CapsuleCollider ||
                col is MeshCollider { convex: true };
        }

        void ValidateSnapCollider()
        {
            if (m_SnapCollider == null)
            {
                Debug.LogWarning("XR Interactable Snap Volume is missing a Snap Collider assignment.", this);
            }
            else if (!SupportsTriggerCollider(m_SnapCollider))
            {
                Debug.LogError("Snap Collider is set to a collider which does not support being a trigger collider." +
                    " Set it to a Box Collider, Sphere Collider, Capsule Collider, or convex Mesh Collider.", this);
            }
            else if (!m_SnapCollider.isTrigger)
            {
                Debug.LogWarning($"Snap Collider must be trigger collider, updating {m_SnapCollider}.", this);
                m_SnapCollider.isTrigger = true;
            }
        }

        /// <summary>
        /// Enables or disables the snap volume collider.
        /// </summary>
        /// <param name="enable">Whether to enable <see cref="snapCollider"/> if currently set.</param>
        void SetSnapColliderEnabled(bool enable)
        {
            if (m_SnapCollider != null)
                m_SnapCollider.enabled = enable;
        }

        /// <summary>
        /// Tries to get the closest point on the associated snapping collider. If <see cref="snapToCollider"/> is null,
        /// it will return the transform position of the associated <see cref="interactable"/>. If both
        /// <see cref="snapToCollider"/> and <see cref="interactable"/> are null, it will return the transform
        /// position of this GameObject.
        /// </summary>
        /// <param name="point">The point for which we are trying to find the the nearest point on the <see cref="snapToCollider"/>.</param>
        /// <returns>The closest point on the <see cref="snapToCollider"/> if possible. Defaults to the <see cref="interactable"/>
        /// transform position if available, or the transform position of this GameObject.</returns>
        public Vector3 GetClosestPoint(Vector3 point)
        {
            if (m_SnapToCollider == null || !m_SnapToCollider.gameObject.activeInHierarchy || !m_SnapToCollider.enabled)
            {
                var interactableValid = m_Interactable != null && (!(m_Interactable is Object unityObject) || unityObject != null);
                return interactableValid ? m_Interactable.transform.position : transform.position;
            }

            return m_SnapToCollider.ClosestPoint(point);
        }

        /// <summary>
        /// Tries to get the closest point on the associated snapping collider based on the attach transform position
        /// of the associated <see cref="interactable"/>. If <see cref="snapToCollider"/> is null, it will return the
        /// attach transform position of the associated <see cref="interactable"/>. If <see cref="interactable"/> is
        /// also null in that case, it will return the transform position of this GameObject.
        /// </summary>
        /// <param name="interactor">The <see cref="IXRInteractor"/> interacting with the <see cref="XRInteractableSnapVolume"/> used to get the attach transform.</param>
        /// <returns>The closest point on the <see cref="snapToCollider"/> if possible. Defaults to the <see cref="interactable"/>
        /// attach transform position of the associated <paramref name="interactor"/> if available, or the transform position of this GameObject.</returns>
        public Vector3 GetClosestPointOfAttachTransform(IXRInteractor interactor)
        {
            var interactableValid = m_Interactable != null && (!(m_Interactable is Object unityObject) || unityObject != null);
            var point = interactableValid ? m_Interactable.GetAttachTransform(interactor).position : transform.position;

            if (m_SnapToCollider == null || !m_SnapToCollider.gameObject.activeInHierarchy || !m_SnapToCollider.enabled)
                return point;

            return m_SnapToCollider.ClosestPoint(point);
        }

        void SetBoundInteractable(IXRInteractable source)
        {
            Debug.Assert(Application.isPlaying);

            if (m_BoundInteractable == source)
                return;

            if (m_BoundSelectInteractable != null)
            {
                m_BoundSelectInteractable.firstSelectEntered.RemoveListener(OnFirstSelectEntered);
                m_BoundSelectInteractable.lastSelectExited.RemoveListener(OnLastSelectExited);
            }

            m_BoundInteractable = source;
            m_BoundSelectInteractable = source as IXRSelectInteractable;

            if (m_BoundSelectInteractable != null)
            {
                m_BoundSelectInteractable.firstSelectEntered.AddListener(OnFirstSelectEntered);
                m_BoundSelectInteractable.lastSelectExited.AddListener(OnLastSelectExited);
            }

            // Refresh the snap collider enabled state (which is what the callbacks do)
            RefreshSnapColliderEnabled();
        }

        void RefreshSnapColliderEnabled()
        {
            var isSelected = m_BoundSelectInteractable != null && m_BoundSelectInteractable.isSelected;
            if (m_DisableSnapColliderWhenSelected)
                SetSnapColliderEnabled(!isSelected);
            else
                SetSnapColliderEnabled(true);
        }

        void OnFirstSelectEntered(SelectEnterEventArgs args)
        {
            if (m_DisableSnapColliderWhenSelected)
                SetSnapColliderEnabled(false);
        }

        void OnLastSelectExited(SelectExitEventArgs args)
        {
            if (m_DisableSnapColliderWhenSelected)
                SetSnapColliderEnabled(true);
        }

        // internal wrapper methods to only allow the manager to call the protected virtual methods.
        // Keeping this as a separate method rather than making the other method `protected internal` to avoid
        // confusion that derived methods cannot be created due to `internal` in the method signature.
        internal void OnRegisteredInternal(InteractableSnapVolumeRegisteredEventArgs args) => OnRegistered(args);
        internal void OnUnregisteredInternal(InteractableSnapVolumeUnregisteredEventArgs args) => OnUnregistered(args);

        /// <summary>
        /// The <see cref="XRInteractionManager"/> calls this method
        /// when this snap volume is registered with it.
        /// </summary>
        /// <param name="args">Event data containing the Interaction Manager that registered this snap volume.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="XRInteractionManager.RegisterSnapVolume"/>
        protected virtual void OnRegistered(InteractableSnapVolumeRegisteredEventArgs args)
        {
            if (args.manager != m_InteractionManager && m_InteractionManager != null)
                Debug.LogWarning($"A snap volume was registered with an unexpected {nameof(XRInteractionManager)}." +
                    $" {this} was expecting to communicate with \"{m_InteractionManager}\" but was registered with \"{args.manager}\".", this);

            if (m_RegisteredInteractionManager != null && args.manager != m_RegisteredInteractionManager)
                Debug.LogWarning($"A snap volume was registered with another {nameof(XRInteractionManager)} while already registered with a manager." +
                    $" {this} was expecting to only communicate with \"{m_RegisteredInteractionManager}\" but was registered with \"{args.manager}\" also." +
                    " This snap volume will not automatically unregister with the original manager when this component is disabled.", this);

            m_RegisteredInteractionManager = args.manager;
            m_InteractionManager = args.manager;

            registered?.Invoke(args);
        }

        /// <summary>
        /// The <see cref="XRInteractionManager"/> calls this method
        /// when this snap volume is unregistered from it.
        /// </summary>
        /// <param name="args">Event data containing the Interaction Manager that unregistered this snap volume.</param>
        /// <remarks>
        /// <paramref name="args"/> is only valid during this method call, do not hold a reference to it.
        /// </remarks>
        /// <seealso cref="XRInteractionManager.UnregisterSnapVolume"/>
        protected virtual void OnUnregistered(InteractableSnapVolumeUnregisteredEventArgs args)
        {
            if (args.manager != m_RegisteredInteractionManager)
                Debug.LogWarning($"A snap volume was unregistered from an unexpected {nameof(XRInteractionManager)}." +
                    $" {this} was expecting to communicate with \"{m_RegisteredInteractionManager}\" but was unregistered from \"{args.manager}\".", this);

            m_RegisteredInteractionManager = null;

            // Clear the reference immediately so it can evaluate to null for the rest of this frame.
            // The manager reference can still be obtained through the event args.
            if (args.managerDestroyed && ReferenceEquals(m_InteractionManager, args.manager))
                m_InteractionManager = null;

            if (m_InteractionManager == null && isActiveAndEnabled)
                XRInteractionManager.RegisterWithWaitlist(this);

            unregistered?.Invoke(args);
        }
    }
}
