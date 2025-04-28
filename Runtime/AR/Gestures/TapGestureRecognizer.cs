//-----------------------------------------------------------------------
// <copyright file="TapGestureRecognizer.cs" company="Google">
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
using UnityEngine;

namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Gesture Recognizer for when the user performs a tap on the touch screen.
    /// </summary>
    /// <inheritdoc />
    public class TapGestureRecognizer : GestureRecognizer<TapGesture>
    {
        /// <summary>
        /// Distance in inches a user's touch can drift from the start position
        /// before the tap gesture is canceled.
        /// </summary>
        public float slopInches { get; set; } = 0.1f;

        /// <summary>
        /// Time (in seconds) within (≤) which a touch and release has to occur for it
        /// to be registered as a tap.
        /// </summary>
        /// <remarks>
        /// A touch and release that takes > this value causes the tap gesture to be canceled.
        /// </remarks>
        public float durationSeconds { get; set; } = 0.3f;

        // Preallocate delegates to avoid GC Alloc that would happen in TryCreateGestures
        readonly Func<InputSystem.EnhancedTouch.Touch, TapGesture> m_CreateEnhancedGesture;
        readonly Action<TapGesture, InputSystem.EnhancedTouch.Touch> m_ReinitializeEnhancedGesture;

#if !XRI_LEGACY_INPUT_DISABLED
        readonly Func<Touch, TapGesture> m_CreateGestureFunction;
        readonly Action<TapGesture, Touch> m_ReinitializeGestureFunction;
#endif

        /// <summary>
        /// Initializes and returns an instance of <see cref="TapGestureRecognizer"/>.
        /// </summary>
        public TapGestureRecognizer()
        {
            m_CreateEnhancedGesture = CreateEnhancedGesture;
            m_ReinitializeEnhancedGesture = ReinitializeEnhancedGesture;

#if !XRI_LEGACY_INPUT_DISABLED
            m_CreateGestureFunction = CreateGesture;
            m_ReinitializeGestureFunction = ReinitializeGesture;
#endif
        }

#if !XRI_LEGACY_INPUT_DISABLED
        /// <summary>
        /// Creates a Tap gesture with the given touch.
        /// </summary>
        /// <param name="touch">The touch that started this gesture.</param>
        /// <returns>The created Tap gesture.</returns>
        TapGesture CreateGesture(Touch touch)
        {
#pragma warning disable CS0618 // Type or member is obsolete -- For backwards compatibility with existing projects
            return new TapGesture(this, touch);
#pragma warning restore CS0618
        }

        static void ReinitializeGesture(TapGesture gesture, Touch touch)
        {
#pragma warning disable CS0618 // Type or member is obsolete -- For backwards compatibility with existing projects
            gesture.Reinitialize(touch);
#pragma warning restore CS0618
        }
#endif

        /// <summary>
        /// Creates a Tap gesture with the given touch.
        /// </summary>
        /// <param name="touch">The touch that started this gesture.</param>
        /// <returns>The created Tap gesture.</returns>
        TapGesture CreateEnhancedGesture(InputSystem.EnhancedTouch.Touch touch)
        {
            return new TapGesture(this, touch);
        }

        static void ReinitializeEnhancedGesture(TapGesture gesture, InputSystem.EnhancedTouch.Touch touch)
        {
            gesture.Reinitialize(touch);
        }

        /// <inheritdoc />
        protected override void TryCreateGestures()
        {
            if (GestureTouchesUtility.touchInputSource == GestureTouchesUtility.TouchInputSource.Enhanced)
                TryCreateOneFingerGestureOnTouchBegan(m_CreateEnhancedGesture, m_ReinitializeEnhancedGesture);
#if !XRI_LEGACY_INPUT_DISABLED
            else
#pragma warning disable CS0618 // Type or member is obsolete -- For backwards compatibility with existing projects
                TryCreateOneFingerGestureOnTouchBegan(m_CreateGestureFunction, m_ReinitializeGestureFunction);
#pragma warning restore CS0618
#endif
        }
    }
}

#endif
