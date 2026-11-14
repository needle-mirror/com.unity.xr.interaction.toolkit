//-----------------------------------------------------------------------
// <copyright originalFile="AndyPlacementManipulator.cs" company="Google">
// <renamed file="ARPlacementInteractable.cs">
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
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

#if !AR_FOUNDATION_PRESENT && !PACKAGE_DOCS_GENERATION

// Stub class definition used to fool version defines that this MonoScript exists (fixed in 19.3)
namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// <see cref="UnityEvent"/> that responds to changes of hover and selection by this interactor.
    /// </summary>
    [Obsolete("ARPlacementInteractable has been replaced by the ObjectSpawner and the ARInteractorSpawnTrigger. These can be found in the general and AR XRI Starter Assets.", true)]
    public class ARPlacementInteractable { }
}

#else

using UnityEngine.Events;

namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// <see cref="UnityEvent"/> that is invoked when the user places an object.
    /// </summary>
    [Serializable]
    [Obsolete(k_ARObjectPlacementEventDeprecated, true)]
    public class ARObjectPlacementEvent : UnityEvent<ARObjectPlacementEventArgs>
    {
        const string k_ARObjectPlacementEventDeprecated = "ARObjectPlacementEvent is deprecated since the ARPlacementInteractable has been replaced by the ObjectSpawner and the ARInteractorSpawnTrigger.";
    }

    /// <summary>
    /// Event data associated with the event when the user places an object.
    /// </summary>
    [Obsolete(k_ARObjectPlacementEventArgsDeprecated, true)]
    public class ARObjectPlacementEventArgs
    {
        const string k_ARObjectPlacementEventArgsDeprecated = "ARObjectPlacementEventArgs is deprecated since the ARPlacementInteractable has been replaced by the ObjectSpawner and the ARInteractorSpawnTrigger.";

        /// <summary>
        /// The Interactable that placed the object.
        /// </summary>
        public ARPlacementInteractable placementInteractable { get; set; }

        /// <summary>
        /// The object that was placed.
        /// </summary>
        public GameObject placementObject { get; set; }
    }

    /// <summary>
    /// (Deprecated) <see cref="UnityEvent"/> that is invoked when an object is placed.
    /// Use <see cref="ARObjectPlacementEvent" /> instead.
    /// </summary>
    [Serializable, Obsolete(k_ARObjectPlacedEventDeprecated, true)]
    public class ARObjectPlacedEvent : UnityEvent<ARPlacementInteractable, GameObject>
    {
        const string k_ARObjectPlacedEventDeprecated = "ARObjectPlacedEvent has been deprecated. Use ARObjectPlacementEvent instead.";
    }

    /// <summary>
    /// Controls the placement of Prefabs via a tap gesture.
    /// </summary>
#if UNITY_6000_5_OR_NEWER
    [NoAutoStaticsCleanup]
#endif
    [AddComponentMenu("XR/AR/Deprecated/AR Placement Interactable", 22)]
    [HelpURL(XRHelpURLConstants.k_ARPlacementInteractable)]
    [Obsolete(k_ARPlacementInteractableDeprecated, true)]
    public class ARPlacementInteractable : ARBaseGestureInteractable
    {
        const string k_ARPlacementInteractableDeprecated = "ARPlacementInteractable has been replaced by the ObjectSpawner and the ARInteractorSpawnTrigger. These can be found in the general and AR XRI Starter Assets.";

        /// <summary>
        /// A <see cref="GameObject"/> to place when a ray cast from a user touch hits a plane.
        /// </summary>
        public GameObject placementPrefab
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

        /// <summary>
        /// Gets or sets the event that is called when this Interactable places a new <see cref="GameObject"/> in the world.
        /// </summary>
        public ARObjectPlacementEvent objectPlaced
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Gets or sets the event that is called when this Interactable places a new <see cref="GameObject"/> in the world.
        /// </summary>
        /// <remarks>
        /// <c>onObjectPlaced</c> has been deprecated. Use <see cref="objectPlaced"/> with updated signature instead.
        /// </remarks>
        [Obsolete("onObjectPlaced has been deprecated. Use objectPlaced with updated signature instead.", true)]
        public ARObjectPlacedEvent onObjectPlaced
        {
            get => default;
            set => _ = value;
        }

        /// <summary>
        /// Gets the pose for the object to be placed from a ray cast hit triggered by a <see cref="TapGesture"/>.
        /// </summary>
        /// <param name="gesture">The tap gesture that triggers the ray cast.</param>
        /// <param name="pose">When this method returns, contains the pose of the placement object based on the ray cast hit.</param>
        /// <returns>Returns <see langword="true"/> if there is a valid ray cast hit that hit the front of a plane.
        /// Otherwise, returns <see langword="false"/>.</returns>
        protected virtual bool TryGetPlacementPose(TapGesture gesture, out Pose pose)
        {
            throw new NotSupportedException(k_ARPlacementInteractableDeprecated);
        }

        /// <summary>
        /// Instantiates the placement object and positions it at the desired pose.
        /// </summary>
        /// <param name="pose">The pose at which the placement object will be instantiated.</param>
        /// <returns>Returns the instantiated placement object at the input pose.</returns>
        /// <seealso cref="placementPrefab"/>
        protected virtual GameObject PlaceObject(Pose pose)
        {
            throw new NotSupportedException(k_ARPlacementInteractableDeprecated);
        }

        /// <summary>
        /// Unity calls this method automatically when the user places an object.
        /// </summary>
        /// <param name="args">Event data containing a reference to the instantiated placement object.</param>
        protected virtual void OnObjectPlaced(ARObjectPlacementEventArgs args)
        {
            throw new NotSupportedException(k_ARPlacementInteractableDeprecated);
        }

        /// <inheritdoc />
        protected override bool CanStartManipulationForGesture(TapGesture gesture)
        {
            throw new NotSupportedException(k_ARPlacementInteractableDeprecated);
        }

        /// <inheritdoc />
        protected override void OnEndManipulation(TapGesture gesture)
        {
            throw new NotSupportedException(k_ARPlacementInteractableDeprecated);
        }
    }
}

#endif
