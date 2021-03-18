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

#if !AR_FOUNDATION_PRESENT && !PACKAGE_DOCS_GENERATION

// Stub class definition used to fool version defines that this MonoScript exists (fixed in 19.3)
namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Base class that manipulates an object via a gesture.
    /// </summary>
    public class ARBaseGestureInteractable {}
}

#else

using UnityEngine.Assertions;
using UnityEngine.XR.ARFoundation;

#if UNITY_EDITOR
using UnityEditor.XR.Interaction.Toolkit.Utilities;
#endif

namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Base class that manipulates an object via a gesture.
    /// </summary>
    public abstract class ARBaseGestureInteractable : XRBaseInteractable
    {
        [SerializeField]
        ARSessionOrigin m_ARSessionOrigin;

        /// <summary>
        /// The <see cref="ARSessionOrigin"/> that this Interactable will use
        /// (such as to get the <see cref="Camera"/> or to transform from Session space).
        /// Will find one if <see langword="null"/>.
        /// </summary>
        public ARSessionOrigin arSessionOrigin
        {
            get => m_ARSessionOrigin;
            set => m_ARSessionOrigin = value;
        }

        /// <summary>
        /// The <see cref="ARGestureInteractor"/> whose gestures are listened to by this Interactable
        /// when connected.
        /// </summary>
        /// <seealso cref="ConnectGestureInteractor"/>
        /// <seealso cref="DisconnectGestureInteractor"/>
        protected ARGestureInteractor gestureInteractor { get; private set; }

        bool m_IsManipulating;

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
            FindARSessionOrigin();
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            FindARSessionOrigin();
        }

        /// <inheritdoc />
        public override bool IsHoverableBy(XRBaseInteractor interactor) => interactor is ARGestureInteractor;

        /// <inheritdoc />
        public override bool IsSelectableBy(XRBaseInteractor interactor) => false;

        /// <summary>
        /// Determines if the manipulation can be started for the given gesture.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <returns>Returns <see langword="true"/> if the manipulation can be started. Otherwise, returns <see langword="false"/>.</returns>
        protected virtual bool CanStartManipulationForGesture(DragGesture gesture) => false;

        /// <summary>
        /// Determines if the manipulation can be started for the given gesture.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <returns>Returns <see langword="true"/> if the manipulation can be started. Otherwise, returns <see langword="false"/>.</returns>
        protected virtual bool CanStartManipulationForGesture(PinchGesture gesture) => false;

        /// <summary>
        /// Determines if the manipulation can be started for the given gesture.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <returns>Returns <see langword="true"/> if the manipulation can be started. Otherwise, returns <see langword="false"/>.</returns>
        protected virtual bool CanStartManipulationForGesture(TapGesture gesture) => false;

        /// <summary>
        /// Determines if the manipulation can be started for the given gesture.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <returns>Returns <see langword="true"/> if the manipulation can be started. Otherwise, returns <see langword="false"/>.</returns>
        protected virtual bool CanStartManipulationForGesture(TwistGesture gesture) => false;

        /// <summary>
        /// Determines if the manipulation can be started for the given gesture.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <returns>Returns <see langword="true"/> if the manipulation can be started. Otherwise, returns <see langword="false"/>.</returns>
        protected virtual bool CanStartManipulationForGesture(TwoFingerDragGesture gesture) => false;

        /// <summary>
        /// Unity calls this method automatically when the manipulation is started.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="GestureRecognizer{T}.onGestureStarted"/>
        protected virtual void OnStartManipulation(DragGesture gesture)
        {
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation is started.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="GestureRecognizer{T}.onGestureStarted"/>
        protected virtual void OnStartManipulation(PinchGesture gesture)
        {
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation is started.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="GestureRecognizer{T}.onGestureStarted"/>
        protected virtual void OnStartManipulation(TapGesture gesture)
        {
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation is started.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="GestureRecognizer{T}.onGestureStarted"/>
        protected virtual void OnStartManipulation(TwistGesture gesture)
        {
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation is started.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="GestureRecognizer{T}.onGestureStarted"/>
        protected virtual void OnStartManipulation(TwoFingerDragGesture gesture)
        {
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation is continued.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="Gesture{T}.onUpdated"/>
        protected virtual void OnContinueManipulation(DragGesture gesture)
        {
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation is continued.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="Gesture{T}.onUpdated"/>
        protected virtual void OnContinueManipulation(PinchGesture gesture)
        {
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation is continued.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="Gesture{T}.onUpdated"/>
        protected virtual void OnContinueManipulation(TapGesture gesture)
        {
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation is continued.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="Gesture{T}.onUpdated"/>
        protected virtual void OnContinueManipulation(TwistGesture gesture)
        {
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation is continued.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="Gesture{T}.onUpdated"/>
        protected virtual void OnContinueManipulation(TwoFingerDragGesture gesture)
        {
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation is ended.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="Gesture{T}.onFinished"/>
        protected virtual void OnEndManipulation(DragGesture gesture)
        {
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation is ended.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="Gesture{T}.onFinished"/>
        protected virtual void OnEndManipulation(PinchGesture gesture)
        {
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation is ended.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="Gesture{T}.onFinished"/>
        protected virtual void OnEndManipulation(TapGesture gesture)
        {
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation is ended.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="Gesture{T}.onFinished"/>
        protected virtual void OnEndManipulation(TwistGesture gesture)
        {
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation is ended.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="Gesture{T}.onFinished"/>
        protected virtual void OnEndManipulation(TwoFingerDragGesture gesture)
        {
        }

        /// <summary>
        /// Determines if the Gesture Interactor is selecting the <see cref="GameObject"/> this Interactable is attached to.
        /// </summary>
        /// <returns>Returns <seealso langword="true"/> if the Gesture Interactor is selecting the <see cref="GameObject"/> this Interactable is attached to.
        /// Otherwise, returns <seealso langword="false"/>.</returns>
        protected virtual bool IsGameObjectSelected()
        {
            if (gestureInteractor == null)
                return false;

            var selectedInteractable = gestureInteractor.selectTarget;
            if (selectedInteractable == null)
                return false;

            return selectedInteractable.gameObject == gameObject;
        }

        /// <summary>
        /// Connect an interactor's gestures to this interactable.
        /// </summary>
        protected virtual void ConnectGestureInteractor()
        {
            if (gestureInteractor == null)
                return;

            if (gestureInteractor.dragGestureRecognizer != null)
                gestureInteractor.dragGestureRecognizer.onGestureStarted += OnGestureStarted;

            if (gestureInteractor.pinchGestureRecognizer != null)
                gestureInteractor.pinchGestureRecognizer.onGestureStarted += OnGestureStarted;

            if (gestureInteractor.tapGestureRecognizer != null)
                gestureInteractor.tapGestureRecognizer.onGestureStarted += OnGestureStarted;

            if (gestureInteractor.twistGestureRecognizer != null)
                gestureInteractor.twistGestureRecognizer.onGestureStarted += OnGestureStarted;

            if (gestureInteractor.twoFingerDragGestureRecognizer != null)
                gestureInteractor.twoFingerDragGestureRecognizer.onGestureStarted += OnGestureStarted;
        }

        /// <summary>
        /// Disconnect an interactor's gestures from this interactable.
        /// </summary>
        protected virtual void DisconnectGestureInteractor()
        {
            if (gestureInteractor == null)
                return;

            if (gestureInteractor.dragGestureRecognizer != null)
                gestureInteractor.dragGestureRecognizer.onGestureStarted -= OnGestureStarted;

            if (gestureInteractor.pinchGestureRecognizer != null)
                gestureInteractor.pinchGestureRecognizer.onGestureStarted -= OnGestureStarted;

            if (gestureInteractor.tapGestureRecognizer != null)
                gestureInteractor.tapGestureRecognizer.onGestureStarted -= OnGestureStarted;

            if (gestureInteractor.twistGestureRecognizer != null)
                gestureInteractor.twistGestureRecognizer.onGestureStarted -= OnGestureStarted;

            if (gestureInteractor.twoFingerDragGestureRecognizer != null)
                gestureInteractor.twoFingerDragGestureRecognizer.onGestureStarted -= OnGestureStarted;
        }

        /// <inheritdoc />
        protected internal override void OnRegistered(InteractableRegisteredEventArgs args)
        {
            base.OnRegistered(args);

            FindAndConnectGestureInteractor(args.manager);
        }

        /// <inheritdoc />
        protected internal override void OnUnregistered(InteractableUnregisteredEventArgs args)
        {
            base.OnUnregistered(args);

            if (gestureInteractor != null)
            {
                DisconnectGestureInteractor();
                gestureInteractor.unregistered -= OnGestureInteractorUnregistered;
                gestureInteractor = null;
            }
            else
            {
                args.manager.interactorRegistered -= OnInteractorRegistered;
            }
        }

        void FindAndConnectGestureInteractor(XRInteractionManager manager)
        {
            // Find the Gesture Interactor registered to the same Interaction Manager,
            // warning if there is more than one. To simplify the API, this Interactable
            // can only listen to one at a time.
            // If the Gesture Interactor exists, need to handle it being unregistered.
            // Otherwise, listen on the Interaction Manager until one is registered.
            gestureInteractor = GetGestureInteractor(manager);
            if (gestureInteractor != null)
            {
                ConnectGestureInteractor();
                gestureInteractor.unregistered += OnGestureInteractorUnregistered;
            }
            else
            {
                manager.interactorRegistered += OnInteractorRegistered;
            }
        }

        void OnGestureInteractorUnregistered(InteractorUnregisteredEventArgs args)
        {
            Assert.AreEqual(gestureInteractor, args.interactor as ARGestureInteractor);

            DisconnectGestureInteractor();
            gestureInteractor.unregistered -= OnGestureInteractorUnregistered;
            gestureInteractor = null;

            // Try to find another or start listening until one is registered
            FindAndConnectGestureInteractor(args.manager);
        }

        void OnInteractorRegistered(InteractorRegisteredEventArgs args)
        {
            Assert.IsNull(gestureInteractor);

            if (args.interactor is ARGestureInteractor registeredGestureInteractor)
            {
                gestureInteractor = registeredGestureInteractor;
                ConnectGestureInteractor();
                gestureInteractor.unregistered += OnGestureInteractorUnregistered;
                args.manager.interactorRegistered -= OnInteractorRegistered;
            }
        }

        ARGestureInteractor GetGestureInteractor(XRInteractionManager manager)
        {
            ARGestureInteractor result = null;
            var numRegisteredGestureInteractors = 0;

            foreach (var interactor in manager.interactors)
            {
                if (interactor is ARGestureInteractor registeredGestureInteractor)
                {
                    result = registeredGestureInteractor;
                    ++numRegisteredGestureInteractors;
                }
            }

            if (numRegisteredGestureInteractors > 1)
            {
                Debug.LogWarning($"More than one {nameof(ARGestureInteractor)} is registered with {manager}," +
                    $" but only one can be listened to at a time. Choosing to listen to {result}.", this);
            }

            return result;
        }

        void FindARSessionOrigin()
        {
            if (m_ARSessionOrigin != null)
                return;

            if (s_ARSessionOriginCache == null)
                s_ARSessionOriginCache = FindObjectOfType<ARSessionOrigin>();

            m_ARSessionOrigin = s_ARSessionOriginCache;
        }

        /// <summary>
        /// Determines if the manipulation can be started for the given gesture.
        /// </summary>
        /// <typeparam name="T">The gesture type.</typeparam>
        /// <param name="gesture">The current gesture.</param>
        /// <returns>Returns <see langword="true"/> if the manipulation can be started. Otherwise, returns <see langword="false"/>.</returns>
        // TODO Consider making this protected virtual and deprecating non-generic methods
        bool CanStartManipulationForGesture<T>(Gesture<T> gesture) where T : Gesture<T>
        {
            switch (gesture)
            {
                case DragGesture dragGesture:
                    return CanStartManipulationForGesture(dragGesture);
                case PinchGesture pinchGesture:
                    return CanStartManipulationForGesture(pinchGesture);
                case TapGesture tapGesture:
                    return CanStartManipulationForGesture(tapGesture);
                case TwistGesture twistGesture:
                    return CanStartManipulationForGesture(twistGesture);
                case TwoFingerDragGesture twoFingerDragGesture:
                    return CanStartManipulationForGesture(twoFingerDragGesture);
            }

            return false;
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation is started.
        /// </summary>
        /// <typeparam name="T">The gesture type.</typeparam>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="GestureRecognizer{T}.onGestureStarted"/>
        // TODO Consider making this protected virtual and deprecating non-generic methods
        void OnStartManipulation<T>(Gesture<T> gesture) where T : Gesture<T>
        {
            switch (gesture)
            {
                case DragGesture dragGesture:
                    OnStartManipulation(dragGesture);
                    break;
                case PinchGesture pinchGesture:
                    OnStartManipulation(pinchGesture);
                    break;
                case TapGesture tapGesture:
                    OnStartManipulation(tapGesture);
                    break;
                case TwistGesture twistGesture:
                    OnStartManipulation(twistGesture);
                    break;
                case TwoFingerDragGesture twoFingerDragGesture:
                    OnStartManipulation(twoFingerDragGesture);
                    break;
            }
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation is continued.
        /// </summary>
        /// <typeparam name="T">The gesture type.</typeparam>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="Gesture{T}.onUpdated"/>
        // TODO Consider making this protected virtual and deprecating non-generic methods
        void OnContinueManipulation<T>(Gesture<T> gesture) where T : Gesture<T>
        {
            switch (gesture)
            {
                case DragGesture dragGesture:
                    OnContinueManipulation(dragGesture);
                    break;
                case PinchGesture pinchGesture:
                    OnContinueManipulation(pinchGesture);
                    break;
                case TapGesture tapGesture:
                    OnContinueManipulation(tapGesture);
                    break;
                case TwistGesture twistGesture:
                    OnContinueManipulation(twistGesture);
                    break;
                case TwoFingerDragGesture twoFingerDragGesture:
                    OnContinueManipulation(twoFingerDragGesture);
                    break;
            }
        }

        /// <summary>
        /// Unity calls this method automatically when the manipulation is ended.
        /// </summary>
        /// <typeparam name="T">The gesture type.</typeparam>
        /// <param name="gesture">The current gesture.</param>
        /// <seealso cref="Gesture{T}.onFinished"/>
        // TODO Consider making this protected virtual and deprecating non-generic methods
        void OnEndManipulation<T>(Gesture<T> gesture) where T : Gesture<T>
        {
            switch (gesture)
            {
                case DragGesture dragGesture:
                    OnEndManipulation(dragGesture);
                    break;
                case PinchGesture pinchGesture:
                    OnEndManipulation(pinchGesture);
                    break;
                case TapGesture tapGesture:
                    OnEndManipulation(tapGesture);
                    break;
                case TwistGesture twistGesture:
                    OnEndManipulation(twistGesture);
                    break;
                case TwoFingerDragGesture twoFingerDragGesture:
                    OnEndManipulation(twoFingerDragGesture);
                    break;
            }
        }

        void OnGestureStarted<T>(Gesture<T> gesture) where T : Gesture<T>
        {
            if (m_IsManipulating)
                return;

            if (CanStartManipulationForGesture(gesture))
            {
                m_IsManipulating = true;
                gesture.onUpdated += OnUpdated;
                gesture.onFinished += OnFinished;
                OnStartManipulation(gesture);
            }
        }

        void OnUpdated<T>(Gesture<T> gesture) where T : Gesture<T>
        {
            if (!m_IsManipulating)
                return;

            // Can only transform selected Items.
            if (!IsGameObjectSelected())
            {
                m_IsManipulating = false;
                OnEndManipulation(gesture);
                return;
            }

            OnContinueManipulation(gesture);
        }

        void OnFinished<T>(Gesture<T> gesture) where T : Gesture<T>
        {
            m_IsManipulating = false;
            OnEndManipulation(gesture);
        }
    }
}

#endif
