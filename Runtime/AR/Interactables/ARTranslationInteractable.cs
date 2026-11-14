//-----------------------------------------------------------------------
// <copyright file="TranslationManipulator.cs" company="Google">
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
    /// Manipulates the position of an object via a drag gesture.
    /// If not selected, the object will be selected when the drag gesture starts.
    /// </summary>
    [Obsolete("ARTranslationInteractable has been replaced by the ARTransformer. Use the ARTransformer instead.", true)]
    public class ARTranslationInteractable { }
}

#else

namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Manipulates the position of an object via a drag gesture.
    /// If not selected, the object will be selected when the drag gesture starts.
    /// </summary>
    [AddComponentMenu("XR/AR/Deprecated/AR Translation Interactable", 22)]
    [RequireComponent(typeof(ARSelectionInteractable))]
    [HelpURL(XRHelpURLConstants.k_ARTranslationInteractable)]
    [Obsolete(k_ARTranslationInteractableDeprecated, true)]
    public class ARTranslationInteractable : ARBaseGestureInteractable
    {
        const string k_ARTranslationInteractableDeprecated = "ARTranslationInteractable has been replaced by the ARTransformer. Use the ARTransformer instead.";

        /// <summary>
        /// Controls whether the object will be constrained vertically, horizontally, or free to move in all axis.
        /// </summary>
        public GestureTransformationUtility.GestureTranslationMode objectGestureTranslationMode
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The maximum translation distance of this object.
        /// </summary>
        public float maxTranslationDistance
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// The <see cref="LayerMask"/> that Unity uses during an additional ray cast
        /// when a user touch does not hit any AR trackable planes.
        /// </summary>
        public LayerMask fallbackLayerMask
        {
            get => default;
            set => _ = value;
        }

        const float k_PositionSpeed = 12f;
        const float k_DiffThreshold = 0.0001f;

        /// <inheritdoc />
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            throw new NotSupportedException(k_ARTranslationInteractableDeprecated);
        }

        /// <inheritdoc />
        protected override bool CanStartManipulationForGesture(DragGesture gesture)
        {
            throw new NotSupportedException(k_ARTranslationInteractableDeprecated);
        }

        /// <inheritdoc />
        protected override void OnStartManipulation(DragGesture gesture)
        {
            throw new NotSupportedException(k_ARTranslationInteractableDeprecated);
        }

        /// <inheritdoc />
        protected override void OnContinueManipulation(DragGesture gesture)
        {
            throw new NotSupportedException(k_ARTranslationInteractableDeprecated);
        }

        /// <inheritdoc />
        protected override void OnEndManipulation(DragGesture gesture)
        {
            throw new NotSupportedException(k_ARTranslationInteractableDeprecated);
        }
    }
}

#endif
