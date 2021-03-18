//-----------------------------------------------------------------------
// <copyright file="ARGestureInteractor.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

// Modifications copyright © 2020 Unity Technologies ApS

#if !AR_FOUNDATION_PRESENT && !PACKAGE_DOCS_GENERATION

// Stub class definition used to fool version defines that this MonoScript exists (fixed in 19.3)
namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// The <see cref="ARGestureInteractor"/> allows the user to manipulate virtual objects (select, translate,
    /// rotate, scale, and elevate) through gestures (tap, drag, twist, and pinch).
    /// </summary>
    /// <remarks>
    /// To make use of this, add an <see cref="ARGestureInteractor"/> to your scene
    /// and an <see cref="ARBaseGestureInteractable"/> to any of your virtual objects.
    /// </remarks>
    public class ARGestureInteractor {}
}

#else

using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.XR.Interaction.Toolkit.Utilities;
#endif
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.EnhancedTouch;
#endif
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// The <see cref="ARGestureInteractor"/> allows the user to manipulate virtual objects (select, translate,
    /// rotate, scale, and elevate) through gestures (tap, drag, twist, and pinch).
    /// </summary>
    /// <remarks>
    /// To make use of this, add an <see cref="ARGestureInteractor"/> to your scene
    /// and an <see cref="ARBaseGestureInteractable"/> to any of your virtual objects.
    /// </remarks>
    [HelpURL(XRHelpURLConstants.k_ARGestureInteractor)]
    public class ARGestureInteractor : XRBaseInteractor
    {
        [SerializeField]
        ARSessionOrigin m_ARSessionOrigin;

        /// <summary>
        /// The <see cref="ARSessionOrigin"/> that this Interactor will use
        /// (such as to get the <see cref="Camera"/> or to transform from Session space).
        /// Will find one if <see langword="null"/>.
        /// </summary>
        public ARSessionOrigin arSessionOrigin
        {
            get => m_ARSessionOrigin;
            set
            {
                m_ARSessionOrigin = value;
                if (Application.isPlaying)
                    PushARSessionOrigin();
            }
        }

        /// <summary>
        /// (Read Only) The Drag gesture recognizer.
        /// </summary>
        public DragGestureRecognizer dragGestureRecognizer { get; private set; }

        /// <summary>
        /// (Read Only) The Pinch gesture recognizer.
        /// </summary>
        public PinchGestureRecognizer pinchGestureRecognizer { get; private set; }

        /// <summary>
        /// (Read Only) The two finger drag gesture recognizer.
        /// </summary>
        public TwoFingerDragGestureRecognizer twoFingerDragGestureRecognizer { get; private set; }

        /// <summary>
        /// (Read Only) The Tap gesture recognizer.
        /// </summary>
        public TapGestureRecognizer tapGestureRecognizer { get; private set; }

        /// <summary>
        /// (Read Only) The Twist gesture recognizer.
        /// </summary>
        public TwistGestureRecognizer twistGestureRecognizer { get; private set; }

#pragma warning disable IDE1006 // Naming Styles
        static ARGestureInteractor s_Instance;
        /// <summary>
        /// (Read Only) The <see cref="ARGestureInteractor"/> instance.
        /// </summary>
        [Obsolete("instance has been deprecated. Use ARBaseGestureInteractable.gestureInteractor instead of singleton.")]
        public static ARGestureInteractor instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindObjectOfType<ARGestureInteractor>();
                    if (s_Instance == null)
                    {
                        Debug.LogError("No instance of ARGestureInteractor exists in the scene.");
                    }
                }

                return s_Instance;
            }
        }

        /// <inheritdoc cref="instance"/>
        [Obsolete("Instance has been deprecated. Use instance instead. (UnityUpgradable) -> instance")]
        public static ARGestureInteractor Instance => instance;

        /// <inheritdoc cref="dragGestureRecognizer"/>
        [Obsolete("DragGestureRecognizer has been deprecated. Use dragGestureRecognizer instead. (UnityUpgradable) -> dragGestureRecognizer")]
        public DragGestureRecognizer DragGestureRecognizer => dragGestureRecognizer;

        /// <inheritdoc cref="pinchGestureRecognizer"/>
        [Obsolete("PinchGestureRecognizer has been deprecated. Use pinchGestureRecognizer instead. (UnityUpgradable) -> pinchGestureRecognizer")]
        public PinchGestureRecognizer PinchGestureRecognizer => pinchGestureRecognizer;

        /// <inheritdoc cref="twoFingerDragGestureRecognizer"/>
        [Obsolete("TwoFingerDragGestureRecognizer has been deprecated. Use twoFingerDragGestureRecognizer instead. (UnityUpgradable) -> twoFingerDragGestureRecognizer")]
        public TwoFingerDragGestureRecognizer TwoFingerDragGestureRecognizer => twoFingerDragGestureRecognizer;

        /// <inheritdoc cref="tapGestureRecognizer"/>
        [Obsolete("TapGestureRecognizer has been deprecated. Use tapGestureRecognizer instead. (UnityUpgradable) -> tapGestureRecognizer")]
        public TapGestureRecognizer TapGestureRecognizer => tapGestureRecognizer;

        /// <inheritdoc cref="twistGestureRecognizer"/>
        [Obsolete("TwistGestureRecognizer has been deprecated. Use twistGestureRecognizer instead. (UnityUpgradable) -> twistGestureRecognizer")]
        public TwistGestureRecognizer TwistGestureRecognizer => twistGestureRecognizer;
#pragma warning restore IDE1006

        /// <summary>
        /// Cached reference to an <see cref="ARSessionOrigin"/> found with <see cref="Object.FindObjectOfType"/>.
        /// </summary>
        static ARSessionOrigin s_ARSessionOriginCache;

        /// <inheritdoc />
        protected override void Reset()
        {
            base.Reset();
#if UNITY_EDITOR
            m_ARSessionOrigin = EditorComponentLocatorUtility.FindSceneComponentOfType<ARSessionOrigin>(gameObject);
#endif
        }

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();

            dragGestureRecognizer = new DragGestureRecognizer();
            pinchGestureRecognizer = new PinchGestureRecognizer();
            twoFingerDragGestureRecognizer = new TwoFingerDragGestureRecognizer();
            tapGestureRecognizer = new TapGestureRecognizer();
            twistGestureRecognizer = new TwistGestureRecognizer();

            FindARSessionOrigin();
            PushARSessionOrigin();
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

#if ENABLE_INPUT_SYSTEM
            EnhancedTouchSupport.Enable();
#endif
            FindARSessionOrigin();
            PushARSessionOrigin();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

#if AR_FOUNDATION_PRESENT && ENABLE_INPUT_SYSTEM
            EnhancedTouchSupport.Disable();
#endif
        }

        /// <inheritdoc />
        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractor(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
                UpdateGestureRecognizers();
        }

        void FindARSessionOrigin()
        {
            if (m_ARSessionOrigin != null)
                return;

            if (s_ARSessionOriginCache == null)
                s_ARSessionOriginCache = FindObjectOfType<ARSessionOrigin>();

            m_ARSessionOrigin = s_ARSessionOriginCache;
        }

        static float GetHorizontalFOV(Camera camera)
        {
            // Calculate the half horizontal FOV in radians
            var vFOV = camera.fieldOfView * Mathf.Deg2Rad;
            var cameraHeight = Mathf.Tan(vFOV * .5f);
            return Mathf.Atan(cameraHeight * camera.aspect);
        }

        /// <inheritdoc />
        public override void GetValidTargets(List<XRBaseInteractable> validTargets)
        {
            validTargets.Clear();

            // ReSharper disable once LocalVariableHidesMember -- hide deprecated camera property
            var camera = m_ARSessionOrigin != null ? m_ARSessionOrigin.camera : Camera.main;
            if (camera == null)
                return;

            var cameraPosition = camera.transform.position;
            var cameraForward = camera.transform.forward;
            var hFOV = GetHorizontalFOV(camera);

            foreach (var interactable in interactionManager.interactables)
            {
                // We can always interact with placement interactables.
                if (interactable is ARPlacementInteractable)
                    validTargets.Add(interactable);
                else if (interactable is ARBaseGestureInteractable)
                {
                    // Check if angle off of camera's forward axis is less than hFOV (more or less in camera frustum).
                    // Note: this does not take size of object into consideration.
                    // Note: this will fall down when directly over/under object (we should also check for dot
                    // product with up/down.
                    var toTarget = Vector3.Normalize(interactable.transform.position - cameraPosition);
                    var dotForwardToTarget = Vector3.Dot(cameraForward, toTarget);
                    if (Mathf.Acos(dotForwardToTarget) < hFOV)
                        validTargets.Add(interactable);
                }
            }
        }

        /// <summary>
        /// Update all Gesture Recognizers.
        /// </summary>
        /// <seealso cref="GestureRecognizer{T}.Update"/>
        protected virtual void UpdateGestureRecognizers()
        {
            dragGestureRecognizer.Update();
            pinchGestureRecognizer.Update();
            twoFingerDragGestureRecognizer.Update();
            tapGestureRecognizer.Update();
            twistGestureRecognizer.Update();
        }

        /// <summary>
        /// Passes the <see cref="arSessionOrigin"/> to the Gesture Recognizers.
        /// </summary>
        /// <seealso cref="GestureRecognizer{T}.arSessionOrigin"/>
        protected virtual void PushARSessionOrigin()
        {
            dragGestureRecognizer.arSessionOrigin = m_ARSessionOrigin;
            pinchGestureRecognizer.arSessionOrigin = m_ARSessionOrigin;
            twoFingerDragGestureRecognizer.arSessionOrigin = m_ARSessionOrigin;
            tapGestureRecognizer.arSessionOrigin = m_ARSessionOrigin;
            twistGestureRecognizer.arSessionOrigin = m_ARSessionOrigin;
        }
    }
}
#endif
