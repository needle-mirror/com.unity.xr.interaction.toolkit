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
using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    public abstract partial class GestureRecognizer<T> where T : Gesture<T>
    {
        const string k_TryCreateOneFingerGestureOnTouchBeganDeprecated = "TryCreateOneFingerGestureOnTouchBegan(Func<Touch, T>, Action<T, Touch>) is marked for deprecation in XRI 3.2.0 and will be removed in a future version. Use TryCreateOneFingerGestureOnTouchBegan(Func<InputSystem.EnhancedTouch.Touch, T>, Action<T, InputSystem.EnhancedTouch.Touch>) instead.";
        const string k_TryCreateTwoFingerGestureOnTouchBeganDeprecated = "TryCreateTwoFingerGestureOnTouchBegan(Func<Touch, Touch, T>, Action<T, Touch, Touch>) is marked for deprecation in XRI 3.2.0 and will be removed in a future version. Use TryCreateTwoFingerGestureOnTouchBegan(Func<InputSystem.EnhancedTouch.Touch, InputSystem.EnhancedTouch.Touch, T>, Action<T, InputSystem.EnhancedTouch.Touch>, InputSystem.EnhancedTouch.Touch) instead.";

        /// <summary>
        /// The deprecated <a href="https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/api/UnityEngine.XR.ARFoundation.ARSessionOrigin.html">ARSessionOrigin</a>
        /// that was used by gestures (such as to get the [Camera](xref:UnityEngine.Camera)
        /// or to transform from Session space). Now obsolete.
        /// </summary>
        [Obsolete("arSessionOrigin has been deprecated. Use xrOrigin instead for similar functionality.", true)]
        public ARSessionOrigin arSessionOrigin { get; set; }

#if !XRI_LEGACY_INPUT_DISABLED

        /// <summary>
        /// (Deprecated) Helper function for creating or re-initializing one-finger gestures when a touch begins.
        /// </summary>
        /// <param name="createGestureFunction">Function to be executed to create the gesture if no dead gesture was available to re-initialize.</param>
        /// <param name="reinitializeGestureFunction">Function to be executed to re-initialize the gesture if a dead gesture was available to re-initialize.</param>
        /// <remarks>
        /// This is deprecated for its reference to Input Manager Touch. Set active input handling to New Input System, and use InputSystem.EnhancedTouch.Touch instead.
        /// </remarks>
        [Obsolete(k_TryCreateOneFingerGestureOnTouchBeganDeprecated, true)]
        protected void TryCreateOneFingerGestureOnTouchBegan(
            Func<Touch, T> createGestureFunction,
            Action<T, Touch> reinitializeGestureFunction)
        {
            throw new NotSupportedException(k_TryCreateOneFingerGestureOnTouchBeganDeprecated);
        }

        /// <summary>
        /// (Deprecated) Helper function for creating or re-initializing two-finger gestures when a touch begins.
        /// </summary>
        /// <param name="createGestureFunction">Function to be executed to create the gesture if no dead gesture was available to re-initialize.</param>
        /// <param name="reinitializeGestureFunction">Function to be executed to re-initialize the gesture if a dead gesture was available to re-initialize.</param>
        /// <remarks>
        /// This is deprecated for its reference to Input Manager Touch. Set active input handling to New Input System, and use InputSystem.EnhancedTouch.Touch instead.
        /// </remarks>
        [Obsolete(k_TryCreateTwoFingerGestureOnTouchBeganDeprecated, true)]
        protected void TryCreateTwoFingerGestureOnTouchBegan(
            Func<Touch, Touch, T> createGestureFunction,
            Action<T, Touch, Touch> reinitializeGestureFunction)
        {
            throw new NotSupportedException(k_TryCreateTwoFingerGestureOnTouchBeganDeprecated);
        }
#endif
    }
}

#endif
