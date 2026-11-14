//-----------------------------------------------------------------------
// <copyright file="RotationManipulator.cs" company="Google">
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

#if !AR_FOUNDATION_PRESENT && !PACKAGE_DOCS_GENERATION

// Stub class definition used to fool version defines that this MonoScript exists (fixed in 19.3)
namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Manipulates the rotation of an object via a drag or a twist gesture.
    /// If an object is selected, then dragging along the horizontal axis
    /// or performing a twist gesture will rotate along the y-axis of the item.
    /// </summary>
    [Obsolete("ARRotationInteractable has been replaced by the ARTransformer. Use the ARTransformer instead.", true)]
    public class ARRotationInteractable { }
}

#else

namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Manipulates the rotation of an object via a drag or a twist gesture.
    /// If an object is selected, then dragging along the horizontal axis
    /// or performing a twist gesture will rotate along the y-axis of the item.
    /// </summary>
    [AddComponentMenu("XR/AR/Deprecated/AR Rotation Interactable", 22)]
    [HelpURL(XRHelpURLConstants.k_ARRotationInteractable)]
    [Obsolete(k_ARRotationInteractableDeprecated, true)]
    public class ARRotationInteractable : ARBaseGestureInteractable
    {
        const string k_ARRotationInteractableDeprecated = "ARRotationInteractable has been replaced by the ARTransformer. Use the ARTransformer instead.";

        /// <summary>
        /// The rate at which Unity rotates the attached object with a drag gesture.
        /// </summary>
        public float rotationRateDegreesDrag
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The rate at which Unity rotates the attached object with a twist gesture.
        /// </summary>
        public float rotationRateDegreesTwist
        {
            get => default;
            set => _ = value;
        }

        /// <inheritdoc />
        protected override bool CanStartManipulationForGesture(DragGesture gesture)
        {
            throw new NotSupportedException(k_ARRotationInteractableDeprecated);
        }

        /// <inheritdoc />
        protected override bool CanStartManipulationForGesture(TwistGesture gesture)
        {
            throw new NotSupportedException(k_ARRotationInteractableDeprecated);
        }

        /// <summary>
        /// Rotates the object around the y-axis via a Drag gesture.
        /// </summary>
        /// <param name="gesture">The current drag gesture.</param>
        /// <inheritdoc />
        protected override void OnContinueManipulation(DragGesture gesture)
        {
            throw new NotSupportedException(k_ARRotationInteractableDeprecated);
        }

        /// <summary>
        /// Rotates the object around the y-axis via a Twist gesture.
        /// </summary>
        /// <param name="gesture">The current twist gesture.</param>
        /// <inheritdoc />
        protected override void OnContinueManipulation(TwistGesture gesture)
        {
            throw new NotSupportedException(k_ARRotationInteractableDeprecated);
        }
    }
}

#endif
