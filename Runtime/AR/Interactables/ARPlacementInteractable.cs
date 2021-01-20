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

#if !AR_FOUNDATION_PRESENT && !PACKAGE_DOCS_GENERATION

// Stub class definition used to fool version defines that this MonoScript exists (fixed in 19.3)
namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// <see cref="UnityEvent"/> that responds to changes of hover and selection by this interactor.
    /// </summary>
    public class ARPlacementInteractable {}
}

#else

using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// <see cref="UnityEvent"/> that is invoked when an object is placed.
    /// </summary>
    [Serializable, Obsolete("ARObjectPlacedEvent has been deprecated. Use ARObjectPlacementEvent instead.")]
    public class ARObjectPlacedEvent : UnityEvent<ARPlacementInteractable, GameObject>
    {
    }

    /// <summary>
    /// <see cref="UnityEvent"/> that is invoked when an object is placed.
    /// </summary>
    [Serializable]
    public class ARObjectPlacementEvent : UnityEvent<ARObjectPlacementEventArgs>
    {
    }

    /// <summary>
    /// Event data associated with the event when an object is placed.
    /// </summary>
    public class ARObjectPlacementEventArgs
    {
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
    /// Controls the placement of Andy objects via a tap gesture.
    /// </summary>
    [HelpURL(XRHelpURLConstants.k_ARPlacementInteractable)]
    public class ARPlacementInteractable : ARBaseGestureInteractable
    {
        [SerializeField]
        [Tooltip("A GameObject to place when a raycast from a user touch hits a plane.")]
        GameObject m_PlacementPrefab;

        /// <summary>
        /// A <see cref="GameObject"/> to place when a raycast from a user touch hits a plane.
        /// </summary>
        public GameObject placementPrefab
        {
            get => m_PlacementPrefab;
            set => m_PlacementPrefab = value;
        }

        [SerializeField]
        ARObjectPlacementEvent m_ObjectPlaced = new ARObjectPlacementEvent();

        /// <summary>
        /// Gets or sets the event that is called when this Interactable places a new <see cref="GameObject"/> in the world.
        /// </summary>
        public ARObjectPlacementEvent objectPlaced
        {
            get => m_ObjectPlaced;
            set => m_ObjectPlaced = value;
        }

#pragma warning disable 618
        [SerializeField]
        ARObjectPlacedEvent m_OnObjectPlaced = new ARObjectPlacedEvent();

        /// <summary>
        /// Gets or sets the event that is called when this Interactable places a new <see cref="GameObject"/> in the world.
        /// </summary>
        [Obsolete("onObjectPlaced has been deprecated. Use objectPlaced with updated signature instead.")]
        public ARObjectPlacedEvent onObjectPlaced
        {
            get => m_OnObjectPlaced;
            set => m_OnObjectPlaced = value;
        }
#pragma warning restore 618

        readonly ARObjectPlacementEventArgs m_ObjectPlacementEventArgs = new ARObjectPlacementEventArgs();

        static readonly List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
        static GameObject s_TrackablesObject;

        /// <inheritdoc />
        protected override bool CanStartManipulationForGesture(TapGesture gesture)
        {
            // Allow for test planes
            if (gesture.TargetObject == null || gesture.TargetObject.layer == 9) // TODO Placement gesture layer check should be configurable
                return true;

            return false;
        }

        /// <inheritdoc />
        protected override void OnEndManipulation(TapGesture gesture)
        {
            base.OnEndManipulation(gesture);

            if (gesture.WasCancelled)
                return;

            // If gesture is targeting an existing object we are done.
            // Allow for test planes
            if (gesture.TargetObject != null && gesture.TargetObject.layer != 9) // TODO Placement gesture layer check should be configurable
                return;

            // Raycast against the location the player touched to search for planes.
            if (GestureTransformationUtility.Raycast(gesture.startPosition, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                var hit = s_Hits[0];

                // Use hit pose and camera pose to check if hittest is from the
                // back of the plane, if it is, no need to create the anchor.
                if (Vector3.Dot(Camera.main.transform.position - hit.pose.position,
                        hit.pose.rotation * Vector3.up) < 0)
                    return;

                // Instantiate placement prefab at the hit pose.
                var placementObject = Instantiate(placementPrefab, hit.pose.position, hit.pose.rotation);

                // Create anchor to track reference point and set it as the parent of placementObject.
                // TODO This should update with a reference point for better tracking.
                var anchorObject = new GameObject("PlacementAnchor");
                anchorObject.transform.position = hit.pose.position;
                anchorObject.transform.rotation = hit.pose.rotation;
                placementObject.transform.parent = anchorObject.transform;

                // Find trackables object in scene and use that as parent
                if (s_TrackablesObject == null)
                    s_TrackablesObject = GameObject.Find("Trackables");
                if (s_TrackablesObject != null)
                    anchorObject.transform.parent = s_TrackablesObject.transform;

                m_ObjectPlacementEventArgs.placementInteractable = this;
                m_ObjectPlacementEventArgs.placementObject = placementObject;
                m_ObjectPlaced?.Invoke(m_ObjectPlacementEventArgs);
                m_OnObjectPlaced?.Invoke(this, placementObject);
            }
        }
    }
}

#endif
