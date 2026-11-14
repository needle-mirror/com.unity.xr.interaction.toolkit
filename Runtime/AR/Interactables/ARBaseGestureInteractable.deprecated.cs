//-----------------------------------------------------------------------
// <copyright file="Manipulator.cs" company="Google">
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
    /// Base class that manipulates an object via a gesture.
    /// </summary>
    [Obsolete("ARBaseGestureInteractable has been deprecated.", true)]
    public class ARBaseGestureInteractable { }
}

#else

using Unity.XR.CoreUtils;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Base class that manipulates an object via a gesture.
    /// </summary>
#if UNITY_6000_5_OR_NEWER
    [NoAutoStaticsCleanup]
#endif
    [Obsolete(k_ARBaseGestureInteractableDeprecated, true)]
    public abstract class ARBaseGestureInteractable : XRBaseInteractable
    {
        const string k_ARBaseGestureInteractableDeprecated = "ARBaseGestureInteractable has been deprecated.";

#pragma warning restore CS0618

        /// <summary>
        /// The <a href="https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/api/UnityEngine.XR.ARFoundation.ARSessionOrigin.html">ARSessionOrigin</a>
        /// that this Interactable will use (such as to get the [Camera](xref:UnityEngine.Camera)
        /// or to transform from Session space). Will find one if <see langword="null"/>.
        /// </summary>
        [Obsolete("arSessionOrigin is marked for deprecation and will be removed in a future version. Please use xrOrigin instead.", true)]
        public ARSessionOrigin arSessionOrigin
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The <see cref="XROrigin"/>
        /// that this Interactable will use (such as to get the [Camera](xref:UnityEngine.Camera)
        /// or to transform from Session space). Will find one if <see langword="null"/>.
        /// </summary>
        public XROrigin xrOrigin
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// When <see langword="true"/>, will exclude touches that are over UI.
        /// Used to make screen space canvas elements block touches from hitting planes behind it.
        /// </summary>
        /// <seealso cref="EventSystem.IsPointerOverGameObject(int)"/>
        public bool excludeUITouches
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The <see cref="ARGestureInteractor"/> that this Interactable listens
        /// to for gestures when connected.
        /// </summary>
        /// <seealso cref="ConnectGestureInteractor"/>
        /// <seealso cref="DisconnectGestureInteractor"/>
        protected ARGestureInteractor gestureInteractor { get; private set; }

        /// <inheritdoc />
        protected override void Awake()
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
        }

        /// <inheritdoc />
        public override bool IsHoverableBy(IXRHoverInteractor interactor)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <inheritdoc />
        public override bool IsSelectableBy(IXRSelectInteractor interactor)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Determines if the manipulation can start for the given gesture.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <returns>Returns <see langword="true"/> if the manipulation can start. Otherwise, returns <see langword="false"/>.</returns>
        protected virtual bool CanStartManipulationForGesture(DragGesture gesture)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Determines if the manipulation can start for the given gesture.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <returns>Returns <see langword="true"/> if the manipulation can start. Otherwise, returns <see langword="false"/>.</returns>
        protected virtual bool CanStartManipulationForGesture(PinchGesture gesture)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Determines if the manipulation can start for the given gesture.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <returns>Returns <see langword="true"/> if the manipulation can start. Otherwise, returns <see langword="false"/>.</returns>
        protected virtual bool CanStartManipulationForGesture(TapGesture gesture)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Determines if the manipulation can start for the given gesture.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <returns>Returns <see langword="true"/> if the manipulation can start. Otherwise, returns <see langword="false"/>.</returns>
        protected virtual bool CanStartManipulationForGesture(TwistGesture gesture)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Determines if the manipulation can start for the given gesture.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <returns>Returns <see langword="true"/> if the manipulation can start. Otherwise, returns <see langword="false"/>.</returns>
        protected virtual bool CanStartManipulationForGesture(TwoFingerDragGesture gesture)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation starts.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="GestureRecognizer{T}.onGestureStarted"/>
        protected virtual void OnStartManipulation(DragGesture gesture)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation starts.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="GestureRecognizer{T}.onGestureStarted"/>
        protected virtual void OnStartManipulation(PinchGesture gesture)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation starts.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="GestureRecognizer{T}.onGestureStarted"/>
        protected virtual void OnStartManipulation(TapGesture gesture)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation starts.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="GestureRecognizer{T}.onGestureStarted"/>
        protected virtual void OnStartManipulation(TwistGesture gesture)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation starts.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="GestureRecognizer{T}.onGestureStarted"/>
        protected virtual void OnStartManipulation(TwoFingerDragGesture gesture)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation continues.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="Gesture{T}.onUpdated"/>
        protected virtual void OnContinueManipulation(DragGesture gesture)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation continues.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="Gesture{T}.onUpdated"/>
        protected virtual void OnContinueManipulation(PinchGesture gesture)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation continues.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="Gesture{T}.onUpdated"/>
        protected virtual void OnContinueManipulation(TapGesture gesture)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation continues.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="Gesture{T}.onUpdated"/>
        protected virtual void OnContinueManipulation(TwistGesture gesture)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation continues.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="Gesture{T}.onUpdated"/>
        protected virtual void OnContinueManipulation(TwoFingerDragGesture gesture)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation ends.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="Gesture{T}.onFinished"/>
        protected virtual void OnEndManipulation(DragGesture gesture)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation ends.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="Gesture{T}.onFinished"/>
        protected virtual void OnEndManipulation(PinchGesture gesture)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation ends.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="Gesture{T}.onFinished"/>
        protected virtual void OnEndManipulation(TapGesture gesture)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation ends.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="Gesture{T}.onFinished"/>
        protected virtual void OnEndManipulation(TwistGesture gesture)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation ends.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="Gesture{T}.onFinished"/>
        protected virtual void OnEndManipulation(TwoFingerDragGesture gesture)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Determines if the <see cref="ARGestureInteractor"/> is selecting the
        /// <see cref="GameObject"/> this Interactable is attached to.
        /// </summary>
        /// <returns>Returns <seealso langword="true"/> if the Gesture Interactor
        /// is selecting the <see cref="GameObject"/> this Interactable is attached
        /// to. Otherwise, returns <seealso langword="false"/>.</returns>
        protected virtual bool IsGameObjectSelected()
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Connect an <see cref="ARGestureInteractor"/>'s gestures to this interactable.
        /// </summary>
        protected virtual void ConnectGestureInteractor()
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <summary>
        /// Disconnect an <see cref="ARGestureInteractor"/>'s gestures from this interactable.
        /// </summary>
        protected virtual void DisconnectGestureInteractor()
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <inheritdoc />
        protected override void OnRegistered(InteractableRegisteredEventArgs args)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }

        /// <inheritdoc />
        protected override void OnUnregistered(InteractableUnregisteredEventArgs args)
        {
            throw new NotSupportedException(k_ARBaseGestureInteractableDeprecated);
        }
    }
}

#endif
