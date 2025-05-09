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
    public abstract partial class GestureRecognizer<T> where T : Gesture<T>
    {
        /// <summary>
        /// The <a href="https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/api/UnityEngine.XR.ARFoundation.ARSessionOrigin.html">ARSessionOrigin</a>
        /// that will be used by gestures (such as to get the [Camera](xref:UnityEngine.Camera)
        /// or to transform from Session space).
        /// </summary>
        [Obsolete("arSessionOrigin has been deprecated. Use xrOrigin instead for similar functionality.")]
        public ARSessionOrigin arSessionOrigin { get; set; }

#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// (Deprecated) List of current active gestures.
        /// </summary>
        /// <remarks>
        /// <c>m_Gestures</c> has been deprecated. Use <see cref="gestures"/> instead.
        /// </remarks>
        [Obsolete("m_Gestures has been deprecated. Use gestures instead.", true)]
        protected List<T> m_Gestures = new List<T>();
#pragma warning restore IDE1006 // Naming Styles

#if !XRI_LEGACY_INPUT_DISABLED
        /// <summary>
        /// (Deprecated) Helper function for creating one-finger gestures when a touch begins.
        /// </summary>
        /// <param name="createGestureFunction">Function to be executed to create the gesture.</param>
        /// <remarks>
        /// <c>TryCreateOneFingerGestureOnTouchBegan(Func&lt;Touch, T&gt;)</c> has been deprecated. Use
        /// <see cref="TryCreateOneFingerGestureOnTouchBegan(Func{Touch, T}, Action{T, Touch})"/> instead.
        /// </remarks>
        [Obsolete("TryCreateOneFingerGestureOnTouchBegan(Func<Touch, T>) is no longer functional. Use TryCreateOneFingerGestureOnTouchBegan(Func<Touch, T>, Action<T, Touch>) instead.", true)]
        protected void TryCreateOneFingerGestureOnTouchBegan(Func<Touch, T> createGestureFunction)
        {
        }

        /// <summary>
        /// (Deprecated) Helper function for creating two-finger gestures when a touch begins.
        /// </summary>
        /// <param name="createGestureFunction">Function to be executed to create the gesture.</param>
        /// <remarks>
        /// <c>TryCreateTwoFingerGestureOnTouchBegan(Func&lt;Touch, Touch, T&gt;)</c> has been deprecated. Use
        /// <see cref="TryCreateTwoFingerGestureOnTouchBegan(Func{Touch, Touch, T}, Action{T, Touch, Touch})"/> instead.
        /// </remarks>
        [Obsolete("TryCreateTwoFingerGestureOnTouchBegan(Func<Touch, Touch, T>) is no longer functional. Use TryCreateTwoFingerGestureOnTouchBegan(Func<Touch, Touch, T>, Action<T, Touch, Touch>) instead.", true)]
        protected void TryCreateTwoFingerGestureOnTouchBegan(
            Func<Touch, Touch, T> createGestureFunction)
        {
        }

        /// <summary>
        /// (Deprecated) Helper function for creating or re-initializing one-finger gestures when a touch begins.
        /// </summary>
        /// <param name="createGestureFunction">Function to be executed to create the gesture if no dead gesture was available to re-initialize.</param>
        /// <param name="reinitializeGestureFunction">Function to be executed to re-initialize the gesture if a dead gesture was available to re-initialize.</param>
        /// <remarks>
        /// This is deprecated for its reference to Input Manager Touch. Set active input handling to New Input System, and use InputSystem.EnhancedTouch.Touch instead.
        /// </remarks>
        [Obsolete("TryCreateOneFingerGestureOnTouchBegan(Func<Touch, T>, Action<T, Touch>) is marked for deprecation in XRI 3.2.0 and will be removed in a future version. Use TryCreateOneFingerGestureOnTouchBegan(Func<InputSystem.EnhancedTouch.Touch, T>, Action<T, InputSystem.EnhancedTouch.Touch>) instead.")]
        protected void TryCreateOneFingerGestureOnTouchBegan(
            Func<Touch, T> createGestureFunction,
            Action<T, Touch> reinitializeGestureFunction)
        {
            TryCreateOneFingerGestureOnTouchBegan(
                TouchConverterClosureHelper.GetFunc(createGestureFunction),
                TouchActionConverterClosureHelper.GetAction(reinitializeGestureFunction));
        }

        /// <summary>
        /// (Deprecated) Helper function for creating or re-initializing two-finger gestures when a touch begins.
        /// </summary>
        /// <param name="createGestureFunction">Function to be executed to create the gesture if no dead gesture was available to re-initialize.</param>
        /// <param name="reinitializeGestureFunction">Function to be executed to re-initialize the gesture if a dead gesture was available to re-initialize.</param>
        /// <remarks>
        /// This is deprecated for its reference to Input Manager Touch. Set active input handling to New Input System, and use InputSystem.EnhancedTouch.Touch instead.
        /// </remarks>
        [Obsolete("TryCreateTwoFingerGestureOnTouchBegan(Func<Touch, Touch, T>, Action<T, Touch, Touch>) is marked for deprecation in XRI 3.2.0 and will be removed in a future version. Use TryCreateTwoFingerGestureOnTouchBegan(Func<InputSystem.EnhancedTouch.Touch, InputSystem.EnhancedTouch.Touch, T>, Action<T, InputSystem.EnhancedTouch.Touch>, InputSystem.EnhancedTouch.Touch) instead.")]
        protected void TryCreateTwoFingerGestureOnTouchBegan(
            Func<Touch, Touch, T> createGestureFunction,
            Action<T, Touch, Touch> reinitializeGestureFunction)
        {
            TryCreateTwoFingerGestureOnTouchBegan(
                TouchConverterClosureHelper.GetFunc(createGestureFunction),
                TouchActionConverterClosureHelper.GetAction(reinitializeGestureFunction));
        }
#endif

        /// <summary>
        /// (Deprecated) Helper function for creating one-finger gestures when a touch begins.
        /// </summary>
        /// <param name="createGestureFunction">Function to be executed to create the gesture.</param>
        /// <remarks>
        /// <c>TryCreateOneFingerGestureOnTouchBegan(Func&lt;InputSystem.EnhancedTouch.Touch, T&gt;)</c> has been deprecated. Use
        /// <see cref="TryCreateOneFingerGestureOnTouchBegan(Func{InputSystem.EnhancedTouch.Touch, T}, Action{T, InputSystem.EnhancedTouch.Touch})"/> instead.
        /// </remarks>
        [Obsolete("TryCreateOneFingerGestureOnTouchBegan(Func<InputSystem.EnhancedTouch.Touch, T>) is no longer functional. Use TryCreateOneFingerGestureOnTouchBegan(Func<InputSystem.EnhancedTouch.Touch, T>, Action<T, InputSystem.EnhancedTouch.Touch>) instead.", true)]
        protected void TryCreateOneFingerGestureOnTouchBegan(Func<InputSystem.EnhancedTouch.Touch, T> createGestureFunction)
        {
        }

        /// <summary>
        /// (Deprecated) Helper function for creating two-finger gestures when a touch begins.
        /// </summary>
        /// <param name="createGestureFunction">Function to be executed to create the gesture.</param>
        /// <remarks>
        /// <c>TryCreateTwoFingerGestureOnTouchBegan(Func&lt;Touch, Touch, T&gt;)</c> has been deprecated. Use
        /// <see cref="TryCreateTwoFingerGestureOnTouchBegan(Func{Touch, Touch, T}, Action{T, Touch, Touch})"/> instead.
        /// </remarks>
        [Obsolete("TryCreateTwoFingerGestureOnTouchBegan(Func<InputSystem.EnhancedTouch.Touch, InputSystem.EnhancedTouch.Touch, T>) is no longer functional. Use TryCreateTwoFingerGestureOnTouchBegan(Func<InputSystem.EnhancedTouch.Touch, InputSystem.EnhancedTouch.Touch, T>, Action<T, InputSystem.EnhancedTouch.Touch, InputSystem.EnhancedTouch.Touch>) instead.", true)]
        protected void TryCreateTwoFingerGestureOnTouchBegan(
            Func<InputSystem.EnhancedTouch.Touch, InputSystem.EnhancedTouch.Touch, T> createGestureFunction)
        {
        }

        static partial class TouchActionConverterClosureHelper
        {
#if !XRI_LEGACY_INPUT_DISABLED
            [Obsolete("GetAction is marked for deprecation in XRI 3.2.0 and will be removed in a future version. Use GetAction(Action<T, InputSystem.EnhancedTouch.Touch>) instead.", true)]
            public static Action<T, CommonTouch> GetAction(Action<T, Touch> reinitializeGestureFunction)
            {
                s_ReinitializeGestureFromOneTouchFunction = reinitializeGestureFunction;
                return s_ConvertUsingOneTouch;
            }

            [Obsolete("GetAction is marked for deprecation in XRI 3.2.0 and will be removed in a future version. Use GetAction(Action<T, InputSystem.EnhancedTouch.Touch, InputSystem.EnhancedTouch.Touch>) instead.", true)]
            public static Action<T, CommonTouch, CommonTouch> GetAction(Action<T, Touch, Touch> reinitializeGestureFunction)
            {
                s_ReinitializeGestureFromTwoTouchFunction = reinitializeGestureFunction;
                return s_ConvertUsingTwoTouch;
            }
#endif
        }
    }
}

#endif
