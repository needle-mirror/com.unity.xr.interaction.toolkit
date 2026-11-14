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

using System;
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

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
    [Obsolete("ARGestureInteractor has been replaced by the XRScreenspaceController and XRRayInteractor.", true)]
    public class ARGestureInteractor { }
}

#else

using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

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
#if UNITY_6000_5_OR_NEWER
    [NoAutoStaticsCleanup]
#endif
    [AddComponentMenu("XR/AR/Deprecated/AR Gesture Interactor", 22)]
    [HelpURL(XRHelpURLConstants.k_ARGestureInteractor)]
    [Obsolete(k_ARGestureInteractorDeprecated, true)]
    public class ARGestureInteractor : XRBaseInteractor
    {
        const string k_ARGestureInteractorDeprecated = "ARGestureInteractor has been replaced by the XRScreenspaceController and XRRayInteractor.";
        const string k_PushARSessionOriginDeprecated = "PushARSessionOrigin has been deprecated. Use PushXROrigin instead for similar functionality.";

#pragma warning restore CS0618

        /// <summary>
        /// The <a href="https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/api/UnityEngine.XR.ARFoundation.ARSessionOrigin.html">ARSessionOrigin</a>
        /// that this Interactor will use (such as to get the <see cref="Camera"/>
        /// or to transform from Session space). Will find one if <see langword="null"/>.
        /// </summary>
        [Obsolete("arSessionOrigin is marked for deprecation and will be removed in a future version. Please use xrOrigin instead.", true)]
        public ARSessionOrigin arSessionOrigin
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// (Read Only) The <see cref="ARGestureInteractor"/> instance.
        /// </summary>
        /// <remarks>
        /// <c>instance</c> has been deprecated. Use <see cref="ARBaseGestureInteractable.gestureInteractor"/> instead of singleton.
        /// </remarks>
        [Obsolete("instance has been deprecated. Use ARBaseGestureInteractable.gestureInteractor instead of singleton.", true)]
        public static ARGestureInteractor instance => default;

        /// <inheritdoc cref="instance"/>
        /// <remarks>
        /// <c>Instance</c> has been deprecated. Use <see cref="instance"/> instead.
        /// </remarks>
        [Obsolete("Instance has been deprecated. Use instance instead. (UnityUpgradable) -> instance", true)]
        public static ARGestureInteractor Instance => default;

        /// <inheritdoc cref="dragGestureRecognizer"/>
        /// <remarks><c>DragGestureRecognizer</c> has been deprecated. Use <see cref="dragGestureRecognizer"/> instead.</remarks>
        [Obsolete("DragGestureRecognizer has been deprecated. Use dragGestureRecognizer instead. (UnityUpgradable) -> dragGestureRecognizer", true)]
        public DragGestureRecognizer DragGestureRecognizer => default;

        /// <inheritdoc cref="pinchGestureRecognizer"/>
        /// <remarks><c>PinchGestureRecognizer</c> has been deprecated. Use <see cref="pinchGestureRecognizer"/> instead.</remarks>
        [Obsolete("PinchGestureRecognizer has been deprecated. Use pinchGestureRecognizer instead. (UnityUpgradable) -> pinchGestureRecognizer", true)]
        public PinchGestureRecognizer PinchGestureRecognizer => default;

        /// <inheritdoc cref="twoFingerDragGestureRecognizer"/>
        /// <remarks><c>TwoFingerDragGestureRecognizer</c> has been deprecated. Use <see cref="twoFingerDragGestureRecognizer"/> instead.</remarks>
        [Obsolete("TwoFingerDragGestureRecognizer has been deprecated. Use twoFingerDragGestureRecognizer instead. (UnityUpgradable) -> twoFingerDragGestureRecognizer", true)]
        public TwoFingerDragGestureRecognizer TwoFingerDragGestureRecognizer => default;

        /// <inheritdoc cref="tapGestureRecognizer"/>
        /// <remarks><c>TapGestureRecognizer</c> has been deprecated. Use <see cref="tapGestureRecognizer"/> instead.</remarks>
        [Obsolete("TapGestureRecognizer has been deprecated. Use tapGestureRecognizer instead. (UnityUpgradable) -> tapGestureRecognizer", true)]
        public TapGestureRecognizer TapGestureRecognizer => default;

        /// <inheritdoc cref="twistGestureRecognizer"/>
        /// <remarks><c>TwistGestureRecognizer</c> has been deprecated. Use <see cref="twistGestureRecognizer"/> instead.</remarks>
        [Obsolete("TwistGestureRecognizer has been deprecated. Use twistGestureRecognizer instead. (UnityUpgradable) -> twistGestureRecognizer", true)]
        public TwistGestureRecognizer TwistGestureRecognizer => default;

        /// <summary>
        /// The <see cref="XROrigin"/> that this Interactor will use
        /// (such as to get the <see cref="Camera"/> or to transform from Session space).
        /// Will find one if <see langword="null"/>.
        /// </summary>
        public XROrigin xrOrigin
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Gets or sets layer mask used for limiting ray cast targets.
        /// </summary>
        public LayerMask raycastMask
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Gets or sets type of interaction with trigger colliders via ray cast.
        /// </summary>
        /// <remarks>
        /// When set to <see cref="QueryTriggerInteraction.UseGlobal"/>, the value of Queries Hit Triggers (<see cref="Physics.queriesHitTriggers"/>)
        /// in Edit &gt; Project Settings &gt; Physics will be used.
        /// </remarks>
        public QueryTriggerInteraction raycastTriggerInteraction
        {
            get => default;
            set => _ = value;
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
        /// (Read Only) The two-finger Drag gesture recognizer.
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

        /// <inheritdoc />
        protected override void Awake()
        {
            throw new NotSupportedException(k_ARGestureInteractorDeprecated);
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
        }

        /// <inheritdoc />
        protected override void OnDisable()
        {
        }

        /// <inheritdoc />
        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            throw new NotSupportedException(k_ARGestureInteractorDeprecated);
        }

        /// <inheritdoc />
        public override void GetValidTargets(List<IXRInteractable> validTargets)
        {
            throw new NotSupportedException(k_ARGestureInteractorDeprecated);
        }

        /// <summary>
        /// Update all Gesture Recognizers.
        /// </summary>
        /// <seealso cref="GestureRecognizer{T}.Update"/>
        protected virtual void UpdateGestureRecognizers()
        {
            throw new NotSupportedException(k_ARGestureInteractorDeprecated);
        }

        /// <summary>
        /// Passes the <see cref="arSessionOrigin"/> to the Gesture Recognizers.
        /// </summary>
        /// <seealso cref="GestureRecognizer{T}.arSessionOrigin"/>
        [Obsolete(k_PushARSessionOriginDeprecated, true)]
        protected virtual void PushARSessionOrigin()
        {
            throw new NotSupportedException(k_PushARSessionOriginDeprecated);
        }

        /// <summary>
        /// Passes the <see cref="xrOrigin"/> to the Gesture Recognizers.
        /// </summary>
        /// <seealso cref="GestureRecognizer{T}.xrOrigin"/>
        protected virtual void PushXROrigin()
        {
            throw new NotSupportedException(k_ARGestureInteractorDeprecated);
        }

        /// <summary>
        /// Passes raycast layer mask properties to the Gesture Recognizers.
        /// </summary>
        /// <seealso cref="GestureRecognizer{T}.raycastMask"/>
        protected virtual void PushRaycastLayerMask()
        {
            throw new NotSupportedException(k_ARGestureInteractorDeprecated);
        }

         /// <summary>
        /// Passes raycast trigger interaction properties to the Gesture Recognizers.
        /// </summary>
        /// <seealso cref="GestureRecognizer{T}.raycastTriggerInteraction"/>
        protected virtual void PushRaycastTriggerInteraction()
        {
            throw new NotSupportedException(k_ARGestureInteractorDeprecated);
        }

        /// <inheritdoc />
        protected override void OnRegistered(InteractorRegisteredEventArgs args)
        {
            throw new NotSupportedException(k_ARGestureInteractorDeprecated);
        }

        /// <inheritdoc />
        protected override void OnUnregistered(InteractorUnregisteredEventArgs args)
        {
            throw new NotSupportedException(k_ARGestureInteractorDeprecated);
        }
    }
}
#endif
