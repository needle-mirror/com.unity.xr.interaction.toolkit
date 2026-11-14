//-----------------------------------------------------------------------
// <copyright file="DragGesture.cs" company="Google">
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

namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    public partial class DragGesture
    {
        const string k_ReinitializeDeprecated = "Reinitialize(Touch) is marked for deprecation in XRI 3.2.0 and will be removed in a future version. Use Reinitialize(InputSystem.EnhancedTouch.Touch) instead.";

#pragma warning disable IDE1006 // Naming Styles
        /// <inheritdoc cref="fingerId"/>
        /// <remarks>
        /// <c>FingerId</c> has been deprecated. Use <see cref="fingerId"/> instead.
        /// </remarks>
        [Obsolete("FingerId has been deprecated. Use fingerId instead. (UnityUpgradable) -> fingerId", true)]
        public int FingerId => default;

        /// <inheritdoc cref="startPosition"/>
        /// <remarks>
        /// <c>StartPosition</c> has been deprecated. Use <see cref="startPosition"/> instead.
        /// </remarks>
        [Obsolete("StartPosition has been deprecated. Use startPosition instead. (UnityUpgradable) -> startPosition", true)]
        public Vector2 StartPosition => default;

        /// <inheritdoc cref="position"/>
        /// <remarks>
        /// <c>Position</c> has been deprecated. Use <see cref="position"/> instead.
        /// </remarks>
        [Obsolete("Position has been deprecated. Use position instead. (UnityUpgradable) -> position", true)]
        public Vector2 Position => default;

        /// <inheritdoc cref="delta"/>
        /// <remarks>
        /// <c>Delta</c> has been deprecated. Use <see cref="delta"/> instead.
        /// </remarks>
        [Obsolete("Delta has been deprecated. Use delta instead. (UnityUpgradable) -> delta", true)]
        public Vector2 Delta => default;

#if !XRI_LEGACY_INPUT_DISABLED
        /// <summary>
        /// (Deprecated) Initializes and returns an instance of <see cref="DragGesture"/>.
        /// </summary>
        /// <param name="recognizer">The gesture recognizer.</param>
        /// <param name="touch">The touch that started this gesture.</param>
        /// <remarks>
        /// <c>DragGesture(DragGestureRecognizer, Touch)</c> has been deprecated for its reference to Input Manager Touch. Use
        /// <see cref="DragGesture(DragGestureRecognizer, InputSystem.EnhancedTouch.Touch)"/> instead.
        /// </remarks>
        [Obsolete("DragGesture(DragGestureRecognizer, Touch) is marked for deprecation in XRI 3.2.0 and will be removed in a future version. Use DragGesture(DragGestureRecognizer, InputSystem.EnhancedTouch.Touch) instead.", true)]
        public DragGesture(DragGestureRecognizer recognizer, Touch touch)
            : this(recognizer, new CommonTouch(touch))
        {
        }

        [Obsolete(k_ReinitializeDeprecated, true)]
        internal void Reinitialize(Touch touch)
        {
            throw new NotSupportedException(k_ReinitializeDeprecated);
        }
#endif
#pragma warning restore IDE1006
    }
}

#endif
