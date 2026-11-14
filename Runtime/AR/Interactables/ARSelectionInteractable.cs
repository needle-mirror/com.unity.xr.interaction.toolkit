//-----------------------------------------------------------------------
// <copyright file="SelectionManipulator.cs" company="Google">
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
using UnityEngine.XR.Interaction.Toolkit.Interactors;

#if !AR_FOUNDATION_PRESENT && !PACKAGE_DOCS_GENERATION

// Stub class definition used to fool version defines that this MonoScript exists (fixed in 19.3)
namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Controls the selection of an object via a Tap gesture.
    /// </summary>
    [Obsolete("ARSelectionInteractable has been deprecated. To achieve the same results use the interactable's focus state instead.", true)]
    public class ARSelectionInteractable { }
}

#else

namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Controls the selection of an object via a Tap gesture.
    /// </summary>
    [AddComponentMenu("XR/AR/Deprecated/AR Selection Interactable", 22)]
    [HelpURL(XRHelpURLConstants.k_ARSelectionInteractable)]
    [Obsolete(k_ARSelectionInteractableDeprecated, true)]
    public class ARSelectionInteractable : ARBaseGestureInteractable
    {
        const string k_ARSelectionInteractableDeprecated = "ARSelectionInteractable has been deprecated. To achieve the same results use the interactable's focus state instead.";

        /// <summary>
        /// The visualization <see cref="GameObject"/> that will become active when the object is selected.
        /// </summary>
        public GameObject selectionVisualization
        {
            get => default;
            set => _ = value;
        }

        /// <inheritdoc />
        public override bool IsSelectableBy(IXRSelectInteractor interactor)
        {
            throw new NotSupportedException(k_ARSelectionInteractableDeprecated);
        }

        /// <inheritdoc />
        protected override bool CanStartManipulationForGesture(TapGesture gesture)
        {
            throw new NotSupportedException(k_ARSelectionInteractableDeprecated);
        }

        /// <inheritdoc />
        protected override void OnEndManipulation(TapGesture gesture)
        {
            throw new NotSupportedException(k_ARSelectionInteractableDeprecated);
        }

        /// <inheritdoc />
        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            throw new NotSupportedException(k_ARSelectionInteractableDeprecated);
        }

        /// <inheritdoc />
        protected override void OnSelectExiting(SelectExitEventArgs args)
        {
            throw new NotSupportedException(k_ARSelectionInteractableDeprecated);
        }
    }
}

#endif
