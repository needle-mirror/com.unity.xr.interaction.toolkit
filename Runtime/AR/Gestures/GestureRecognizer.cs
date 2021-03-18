//-----------------------------------------------------------------------
// <copyright file="GestureRecognizer.cs" company="Google">
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

#if AR_FOUNDATION_PRESENT || PACKAGE_DOCS_GENERATION

using System;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// A Gesture Recognizer processes touch input to determine if a gesture should start
    /// and fires an event when the gesture is started.
    /// </summary>
    /// <typeparam name="T">The actual gesture.</typeparam>
    /// <remarks>
    /// To determine when a gesture is finished/updated, listen to events on the
    /// <see cref="Gesture{T}"/> object.
    /// </remarks>
    public abstract class GestureRecognizer<T> where T : Gesture<T>
    {
        /// <summary>
        /// Calls the methods in its invocation list when a gesture is started.
        /// To receive an event when the gesture is finished/updated, listen to
        /// events on the <see cref="Gesture{T}"/> object.
        /// </summary>
        public event Action<T> onGestureStarted;

        /// <summary>
        /// The <see cref="ARSessionOrigin"/> that will be used by gestures
        /// (such as to get the <see cref="Camera"/> or to transform from Session space).
        /// </summary>
        public ARSessionOrigin arSessionOrigin { get; set; }

        /// <summary>
        /// List of current active gestures.
        /// </summary>
        /// <remarks>
        /// Gestures must be added or removed using <see cref="AddGesture"/> and <see cref="RemoveGesture"/>
        /// rather than by directly modifying this list. This list should be treated as read-only.
        /// </remarks>
        /// <seealso cref="AddGesture"/>
        /// <seealso cref="RemoveGesture"/>
        protected List<T> gestures { get; } = new List<T>();

#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// (Deprecated) List of current active gestures.
        /// </summary>
        [Obsolete("m_Gestures has been deprecated. Use gestures instead. (UnityUpgradable) -> gestures")]
        protected List<T> m_Gestures = new List<T>();
#pragma warning restore IDE1006 // Naming Styles

        readonly List<T> m_PostponedGesturesToRemove = new List<T>();

        bool m_IsUpdatingGestures;

        /// <summary>
        /// Instantiate and update all gestures.
        /// </summary>
        public void Update()
        {
            // Instantiate gestures based on touch input.
            // Just because a gesture was created, doesn't mean that it is started.
            // For example, a DragGesture is created when the user touches down,
            // but doesn't actually start until the touch has moved beyond a threshold.
            TryCreateGestures();

            // Update all gestures
            m_IsUpdatingGestures = true;

            foreach (var gesture in gestures)
            {
                gesture.Update();
            }

            m_IsUpdatingGestures = false;

            // Gestures that finished can now be removed. Removals may have been postponed
            // while the gestures list was being iterated.
            if (m_PostponedGesturesToRemove.Count > 0)
            {
                foreach (var gesture in m_PostponedGesturesToRemove)
                {
                    gestures.Remove(gesture);
                }

                m_PostponedGesturesToRemove.Clear();
            }
        }

        /// <summary>
        /// Try to recognize and create gestures.
        /// </summary>
        protected abstract void TryCreateGestures();

        /// <summary>
        /// Add the given <paramref name="gesture"/> to be managed
        /// so that it can be updated during <see cref="Update"/>.
        /// </summary>
        /// <param name="gesture">The gesture to add.</param>
        /// <seealso cref="RemoveGesture"/>
        protected void AddGesture(T gesture)
        {
            // Should not attempt to add to Gestures list while iterating that list
            if (m_IsUpdatingGestures)
            {
                Debug.LogError($"Cannot add {typeof(T).Name} while updating Gestures." +
                    $" It should be done during {nameof(TryCreateGestures)} instead.");
                return;
            }

            gesture.onStart += OnStart;
            gesture.onFinished += OnFinished;
            gestures.Add(gesture);
        }

        /// <summary>
        /// Remove the given <paramref name="gesture"/> from being managed.
        /// After being removed, it will no longer be updated during <see cref="Update"/>.
        /// </summary>
        /// <param name="gesture">The gesture to remove.</param>
        /// <seealso cref="AddGesture"/>
        protected void RemoveGesture(T gesture)
        {
            // Should not attempt to remove from Gestures list while iterating that list
            if (m_IsUpdatingGestures)
                m_PostponedGesturesToRemove.Add(gesture);
            else
                gestures.Remove(gesture);
        }

        /// <summary>
        /// Helper function for creating one finger gestures when a touch begins.
        /// </summary>
        /// <param name="createGestureFunction">Function to be executed to create the gesture.</param>
        protected void TryCreateOneFingerGestureOnTouchBegan(Func<Touch, T> createGestureFunction)
        {
            TryCreateOneFingerGestureOnTouchBegan(CreateGestureFunction);

            T CreateGestureFunction(CommonTouch touch) => createGestureFunction(touch.GetTouch());
        }

        /// <summary>
        /// Helper function for creating one finger gestures when a touch begins.
        /// </summary>
        /// <param name="createGestureFunction">Function to be executed to create the gesture.</param>
        protected void TryCreateOneFingerGestureOnTouchBegan(Func<InputSystem.EnhancedTouch.Touch, T> createGestureFunction)
        {
            TryCreateOneFingerGestureOnTouchBegan(CreateGestureFunction);

            T CreateGestureFunction(CommonTouch touch) => createGestureFunction(touch.GetEnhancedTouch());
        }

        void TryCreateOneFingerGestureOnTouchBegan(Func<CommonTouch, T> createGestureFunction)
        {
            foreach (var touch in GestureTouchesUtility.touches)
            {
                if (touch.isPhaseBegan &&
                    !GestureTouchesUtility.IsFingerIdRetained(touch.fingerId) &&
                    !GestureTouchesUtility.IsTouchOffScreenEdge(touch))
                {
                    var gesture = createGestureFunction(touch);
                    AddGesture(gesture);
                }
            }
        }

        /// <summary>
        /// Helper function for creating two finger gestures when a touch begins.
        /// </summary>
        /// <param name="createGestureFunction">Function to be executed to create the gesture.</param>
        protected void TryCreateTwoFingerGestureOnTouchBegan(
            Func<Touch, Touch, T> createGestureFunction)
        {
            TryCreateTwoFingerGestureOnTouchBegan(CreateGestureFunction);

            T CreateGestureFunction(CommonTouch touch, CommonTouch otherTouch) =>
                createGestureFunction(touch.GetTouch(), otherTouch.GetTouch());
        }

        /// <summary>
        /// Helper function for creating two finger gestures when a touch begins.
        /// </summary>
        /// <param name="createGestureFunction">Function to be executed to create the gesture.</param>
        protected void TryCreateTwoFingerGestureOnTouchBegan(
            Func<InputSystem.EnhancedTouch.Touch, InputSystem.EnhancedTouch.Touch, T> createGestureFunction)
        {
            TryCreateTwoFingerGestureOnTouchBegan(CreateGestureFunction);

            T CreateGestureFunction(CommonTouch touch, CommonTouch otherTouch) =>
                createGestureFunction(touch.GetEnhancedTouch(), otherTouch.GetEnhancedTouch());
        }

        void TryCreateTwoFingerGestureOnTouchBegan(
            Func<CommonTouch, CommonTouch, T> createGestureFunction)
        {
            var touches = GestureTouchesUtility.touches;
            if (touches.Count < 2)
                return;

            for (var i = 0; i < touches.Count; ++i)
            {
                TryCreateGestureTwoFingerGestureOnTouchBeganForTouchIndex(i, touches, createGestureFunction);
            }
        }

        void TryCreateGestureTwoFingerGestureOnTouchBeganForTouchIndex(
            int touchIndex,
            IReadOnlyList<CommonTouch> touches,
            Func<CommonTouch, CommonTouch, T> createGestureFunction)
        {
            var touch = touches[touchIndex];

            if (!touch.isPhaseBegan ||
                GestureTouchesUtility.IsFingerIdRetained(touch.fingerId) ||
                GestureTouchesUtility.IsTouchOffScreenEdge(touch))
            {
                return;
            }

            for (var i = 0; i < touches.Count; i++)
            {
                if (i == touchIndex)
                    continue;

                var otherTouch = touches[i];

                // Prevents the same two touches from creating two gestures if both touches began on
                // the same frame.
                if (i < touchIndex && otherTouch.isPhaseBegan)
                {
                    continue;
                }

                if (GestureTouchesUtility.IsFingerIdRetained(otherTouch.fingerId) ||
                    GestureTouchesUtility.IsTouchOffScreenEdge(otherTouch))
                {
                    continue;
                }

                var gesture = createGestureFunction(touch, otherTouch);
                AddGesture(gesture);
            }
        }

        void OnStart(T gesture)
        {
            onGestureStarted?.Invoke(gesture);
        }

        void OnFinished(T gesture)
        {
            RemoveGesture(gesture);
        }
    }
}

#endif
