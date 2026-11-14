//-----------------------------------------------------------------------
// <copyright file="ScaleManipulator.cs" company="Google">
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
    /// Controls the scale of an object via a Pinch gesture.
    /// If an object is selected, then doing a pinch/zoom will modify the scale
    /// of the object.
    /// </summary>
    [Obsolete("ARScaleInteractable has been replaced by the ARTransformer. Use the ARTransformer instead.", true)]
    public class ARScaleInteractable { }
}

#else

namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Controls the scale of an object via a Pinch gesture.
    /// If an object is selected, then doing a pinch/zoom will modify the scale
    /// of the object.
    /// </summary>
    [AddComponentMenu("XR/AR/Deprecated/AR Scale Interactable", 22)]
    [HelpURL(XRHelpURLConstants.k_ARScaleInteractable)]
    [Obsolete(k_ARScaleInteractableDeprecated, true)]
    public class ARScaleInteractable : ARBaseGestureInteractable
    {
        const string k_ARScaleInteractableDeprecated = "ARScaleInteractable has been replaced by the ARTransformer. Use the ARTransformer instead.";

        /// <summary>
        /// The minimum scale of the object.
        /// </summary>
        public float minScale
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The maximum scale of the object.
        /// </summary>
        public float maxScale
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The limit of the elastic ratio.
        /// </summary>
        public float elasticRatioLimit
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Sensitivity to movement being translated into scale.
        /// </summary>
        public float sensitivity
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Amount that the scale bounces back after hitting min/max of range.
        /// </summary>
        public float elasticity
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnValidate()
        {
        }

        /// <inheritdoc />
        protected override void OnEnable()
        {
        }

        /// <inheritdoc />
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            throw new NotSupportedException(k_ARScaleInteractableDeprecated);
        }

        /// <inheritdoc />
        protected override bool CanStartManipulationForGesture(PinchGesture gesture)
        {
            throw new NotSupportedException(k_ARScaleInteractableDeprecated);
        }

        /// <summary>
        /// Recalculates the current scale ratio in case local scale, min or max scale were changed.
        /// </summary>
        /// <param name="gesture">The gesture that started this transformation.</param>
        /// <inheritdoc />
        protected override void OnStartManipulation(PinchGesture gesture)
        {
            throw new NotSupportedException(k_ARScaleInteractableDeprecated);
        }

        /// <summary>
        /// Continues the scaling of the object.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <inheritdoc />
        protected override void OnContinueManipulation(PinchGesture gesture)
        {
            throw new NotSupportedException(k_ARScaleInteractableDeprecated);
        }

        /// <summary>
        /// Finishes the scaling of the object.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <inheritdoc />
        protected override void OnEndManipulation(PinchGesture gesture)
        {
            throw new NotSupportedException(k_ARScaleInteractableDeprecated);
        }
    }
}

#endif
