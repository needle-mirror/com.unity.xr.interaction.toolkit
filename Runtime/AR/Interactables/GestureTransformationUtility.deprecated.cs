//-----------------------------------------------------------------------
// <copyright file="TransformationUtility.cs" company="Google">
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

#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif
#if AR_FOUNDATION_PRESENT || PACKAGE_DOCS_GENERATION

using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Provides helper functions for common functionality to transform objects in AR.
    /// </summary>
#if UNITY_6000_5_OR_NEWER
    [NoAutoStaticsCleanup]
#endif
    [Obsolete(k_GestureTransformationUtilityDeprecated, true)]
    public static class GestureTransformationUtility
    {
        const string k_GestureTransformationUtilityDeprecated = "GestureTransformationUtility has been replaced in function by the ARTransformer. Please use the ARTransformer instead.";
        const string k_RaycastDeprecated = "Raycast with the ARSessionOrigin parameter has been deprecated. Use Raycast with the XROrigin parameter instead.";
        const string k_GetBestPlacementPositionDeprecated = "GetBestPlacementPosition with the ARSessionOrigin parameter has been deprecated. Use GetBestPlacementPosition with the XROrigin parameter instead.";
        const string k_RaycastDeprecated2 = "Raycast has been deprecated. Use Raycast with updated signature instead.";
        const string k_GetBestPlacementPositionDeprecated2 = "GetBestPlacementPosition has been deprecated. Use GetBestPlacementPosition with updated signature instead.";

        /// <summary>
        /// Represents the alignment of a plane where translation is allowed.
        /// </summary>
        /// <seealso cref="GetBestPlacementPosition(Vector3, Vector2, float, float, float, GestureTranslationMode, XROrigin, TrackableType, int)"/>
        /// <seealso cref="PlaneAlignment"/>
        public enum GestureTranslationMode
        {
            /// <summary>
            /// Allow translation when the plane is horizontal.
            /// </summary>
            Horizontal,

            /// <summary>
            /// Allow translation when the plane is vertical.
            /// </summary>
            Vertical,

            /// <summary>
            /// Allow translation on any plane.
            /// </summary>
            Any,
        }

        /// <summary>
        /// Max amount (inches) to offset the screen touch in <see cref="GetBestPlacementPosition(Vector3, Vector2, float, float, float, GestureTranslationMode, XROrigin, TrackableType, int)"/>.
        /// The actual amount depends on the angle of the camera relative.
        /// The further downward the camera is angled, the more the screen touch is offset.
        /// </summary>
        const float k_MaxScreenTouchOffset = 0.4f;

        /// <summary>
        /// In <see cref="GetBestPlacementPosition(Vector3, Vector2, float, float, float, GestureTranslationMode, XROrigin, TrackableType, int)"/>, when the camera is closer than this value to the object,
        /// reduce how much the object hovers.
        /// </summary>
        const float k_HoverDistanceThreshold = 1f;

#region Deprecated ARSessionOrigin overloads
#pragma warning restore CS0618

#endregion

        /// <summary>
        /// Cast a ray from a point in screen space against trackables, i.e., detected features such as planes.
        /// Can optionally fallback to hit test against Colliders in the loaded Scenes when no trackables were hit.
        /// </summary>
        /// <remarks>
        /// Unity uses the 3D physics scene of the camera when performing the fallback ray cast.
        /// </remarks>
        /// <param name="screenPoint">The point, in device screen pixels, from which to cast.</param>
        /// <param name="hitResults">Contents are replaced with the ray cast results, if successful.</param>
        /// <param name="sessionOrigin">The <see cref="XROrigin"/> used for ray casting.</param>
        /// <param name="trackableTypes">(Optional) The types of trackables to cast against.</param>
        /// <param name="fallbackLayerMask">(Optional) The <see cref="LayerMask"/> that Unity uses during an additional ray cast when no trackables are hit.
        /// Defaults to Nothing which skips the fallback ray cast.</param>
        /// <returns>Returns <see langword="true"/> if the ray cast hit a trackable in the <paramref name="trackableTypes"/> or if the fallback ray cast hit.
        /// Otherwise, returns <see langword="false"/>.</returns>
        public static bool Raycast(
            Vector2 screenPoint,
            List<ARRaycastHit> hitResults,
            XROrigin sessionOrigin,
#if AR_FOUNDATION_4_2_OR_NEWER
            TrackableType trackableTypes = TrackableType.AllTypes,
#else
            TrackableType trackableTypes = TrackableType.All,
#endif
            int fallbackLayerMask = 0)
        {
            throw new NotSupportedException(k_GestureTransformationUtilityDeprecated);
        }

        /// <summary>
        /// Calculates the best position to place an object in AR based on screen position.
        /// Could be used for tapping a location on the screen, dragging an object, or using a fixed
        /// cursor in the center of the screen for placing and moving objects.
        /// </summary>
        /// <returns>Returns the best placement position.</returns>
        /// <param name="currentAnchorPosition">Position of the parent anchor, i.e., where the
        /// object is before translation starts.</param>
        /// <param name="screenPosition">Location on the screen in pixels to place the object at.</param>
        /// <param name="groundingPlaneHeight">The starting height of the plane to place the object on.</param>
        /// <param name="hoverOffset">How much should the object hover above the groundingPlane
        /// before it has been placed.</param>
        /// <param name="maxTranslationDistance">The maximum distance allowed to translate the object.</param>
        /// <param name="gestureTranslationMode">The translation mode, indicating the plane types allowed.
        /// </param>
        /// <param name="sessionOrigin">The <see cref="XROrigin"/> used for ray casting.</param>
        /// <param name="trackableTypes">(Optional) The types of trackables to cast against.</param>
        /// <param name="fallbackLayerMask">(Optional) The <see cref="LayerMask"/> that Unity uses during
        /// an additional ray cast when no trackables are hit. Defaults to Nothing which skips the fallback ray cast.</param>
        /// <remarks>
        /// Unity places objects along the x/z of the grounding plane. When placed on an AR plane
        /// below the grounding plane, the object will drop straight down onto it in world space.
        /// This prevents the object from being pushed deeper into the scene when moving from a
        /// higher plane to a lower plane. When moving from a lower plane to a higher plane, this
        /// function returns a new groundingPlane to replace the old one.
        /// </remarks>
        public static Placement GetBestPlacementPosition(
            Vector3 currentAnchorPosition,
            Vector2 screenPosition,
            float groundingPlaneHeight,
            float hoverOffset,
            float maxTranslationDistance,
            GestureTranslationMode gestureTranslationMode,
            XROrigin sessionOrigin,
            TrackableType trackableTypes = TrackableType.PlaneWithinPolygon,
            int fallbackLayerMask = 0)
        {
            throw new NotSupportedException(k_GestureTransformationUtilityDeprecated);
        }

        /// <summary>
        /// Result of the function <see cref="GetBestPlacementPosition(Vector3, Vector2, float, float, float, GestureTranslationMode, XROrigin, TrackableType, int)"/>
        /// that indicates if a placement position
        /// was found and information about the placement position.
        /// </summary>
        public partial struct Placement
        {
            /// <summary>
            /// <see langword="true"/> if this Placement has a valid <see cref="hoveringPosition"/> value, otherwise <see langword="false"/>.
            /// </summary>
            /// <seealso cref="hoveringPosition"/>
            public bool hasHoveringPosition { get; set; }

            /// <summary>
            /// The position that the object should be displayed at before the placement has been
            /// confirmed.
            /// </summary>
            /// <seealso cref="hasHoveringPosition"/>
            public Vector3 hoveringPosition { get; set; }

            /// <summary>
            /// <see langword="true"/> if this Placement has a valid <see cref="placementPosition"/> value, otherwise <see langword="false"/>.
            /// </summary>
            /// <seealso cref="placementPosition"/>
            public bool hasPlacementPosition { get; set; }

            /// <summary>
            /// The resulting position that the object should be placed at.
            /// </summary>
            /// <seealso cref="hasPlacementPosition"/>
            public Vector3 placementPosition { get; set; }

            /// <summary>
            /// The resulting rotation that the object should have.
            /// </summary>
            public Quaternion placementRotation { get; set; }

            /// <summary>
            /// <see langword="true"/> if this Placement has a <see cref="placementPlane"/>, otherwise <see langword="false"/>.
            /// </summary>
            /// <seealso cref="placementPlane"/>
            public bool hasPlane { get; set; }

            /// <summary>
            /// The <a href="https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/api/UnityEngine.XR.ARFoundation.ARPlane.html">ARPlane</a>
            /// that the object is being placed on.
            /// </summary>
            /// <seealso cref="hasPlane"/>
            public ARPlane placementPlane { get; set; }

            /// <summary>
            /// The resulting starting height of the plane that the object is being placed along.
            /// </summary>
            public float updatedGroundingPlaneHeight { get; set; }
        }

           /// <summary>
        /// Cast a ray from a point in screen space against trackables, i.e., detected features such as planes.
        /// Can optionally fallback to hit test against Colliders in the loaded Scenes when no trackables were hit.
        /// </summary>
        /// <param name="screenPoint">The point, in device screen pixels, from which to cast.</param>
        /// <param name="hitResults">Contents are replaced with the ray cast results, if successful.</param>
        /// <param name="sessionOrigin">The <see cref="ARSessionOrigin"/> used for ray casting.</param>
        /// <param name="trackableTypes">(Optional) The types of trackables to cast against.</param>
        /// <param name="fallbackLayerMask">(Optional) The <see cref="LayerMask"/> that Unity uses during an additional ray cast when no trackables are hit.
        /// Defaults to Nothing which skips the fallback ray cast.</param>
        /// <returns>Returns <see langword="true"/> if the ray cast hit a trackable in the <paramref name="trackableTypes"/> or if the fallback ray cast hit.
        /// Otherwise, returns <see langword="false"/>.</returns>
        [Obsolete(k_RaycastDeprecated, true)]
        public static bool Raycast(
            Vector2 screenPoint,
            List<ARRaycastHit> hitResults,
            ARSessionOrigin sessionOrigin,
            TrackableType trackableTypes = TrackableType.All,
            int fallbackLayerMask = 0)
        {
            throw new NotSupportedException(k_RaycastDeprecated);
        }

        /// <summary>
        /// Calculates the best position to place an object in AR based on screen position.
        /// Could be used for tapping a location on the screen, dragging an object, or using a fixed
        /// cursor in the center of the screen for placing and moving objects.
        /// </summary>
        /// <returns>Returns the best placement position.</returns>
        /// <param name="currentAnchorPosition">Position of the parent anchor, i.e., where the
        /// object is before translation starts.</param>
        /// <param name="screenPosition">Location on the screen in pixels to place the object at.</param>
        /// <param name="groundingPlaneHeight">The starting height of the plane to place the object on.</param>
        /// <param name="hoverOffset">How much should the object hover above the groundingPlane
        /// before it has been placed.</param>
        /// <param name="maxTranslationDistance">The maximum distance allowed to translate the object.</param>
        /// <param name="gestureTranslationMode">The translation mode, indicating the plane types allowed.
        /// </param>
        /// <param name="sessionOrigin">The <see cref="ARSessionOrigin"/> used for ray casting.</param>
        /// <param name="trackableTypes">(Optional) The types of trackables to cast against.</param>
        /// <param name="fallbackLayerMask">(Optional) The <see cref="LayerMask"/> that Unity uses during
        /// an additional ray cast when no trackables are hit. Defaults to Nothing which skips the fallback ray cast.</param>
        /// <remarks>
        /// Unity places objects along the x/z of the grounding plane. When placed on an AR plane
        /// below the grounding plane, the object will drop straight down onto it in world space.
        /// This prevents the object from being pushed deeper into the scene when moving from a
        /// higher plane to a lower plane. When moving from a lower plane to a higher plane, this
        /// function returns a new groundingPlane to replace the old one.
        /// </remarks>
        [Obsolete(k_GetBestPlacementPositionDeprecated, true)]
        public static Placement GetBestPlacementPosition(
            Vector3 currentAnchorPosition,
            Vector2 screenPosition,
            float groundingPlaneHeight,
            float hoverOffset,
            float maxTranslationDistance,
            GestureTranslationMode gestureTranslationMode,
            ARSessionOrigin sessionOrigin,
            TrackableType trackableTypes = TrackableType.PlaneWithinPolygon,
            int fallbackLayerMask = 0)
        {
            throw new NotSupportedException(k_GetBestPlacementPositionDeprecated);
        }

        /// <summary>
        /// Cast a ray from a point in screen space against trackables, i.e., detected features such as planes.
        /// </summary>
        /// <param name="screenPoint">The point, in device screen pixels, from which to cast.</param>
        /// <param name="hitResults">Contents are replaced with the ray cast results, if successful.</param>
        /// <param name="trackableTypes">(Optional) The types of trackables to cast against.</param>
        /// <returns>Returns <see langword="true"/> if the ray cast hit a trackable in the <paramref name="trackableTypes"/>.
        /// Otherwise, returns <see langword="false"/>.</returns>
        [Obsolete(k_RaycastDeprecated2, true)]
        public static bool Raycast(Vector2 screenPoint, List<ARRaycastHit> hitResults, TrackableType trackableTypes = TrackableType.All)
        {
            throw new NotSupportedException(k_RaycastDeprecated2);
        }

        /// <summary>
        /// Calculates the best position to place an object in AR based on screen position.
        /// Could be used for tapping a location on the screen, dragging an object, or using a fixed
        /// cursor in the center of the screen for placing and moving objects.
        /// </summary>
        /// <returns>Returns the best placement position.</returns>
        /// <param name="currentAnchorPosition">Position of the parent anchor, i.e., where the
        /// object is before translation starts.</param>
        /// <param name="screenPos">Location on the screen in pixels to place the object at.</param>
        /// <param name="groundingPlaneHeight">The starting height of the plane to place the object on.</param>
        /// <param name="hoverOffset">How much should the object hover above the groundingPlane
        /// before it has been placed.</param>
        /// <param name="maxTranslationDistance">The maximum distance allowed to translate the object.</param>
        /// <param name="gestureTranslationMode">The translation mode, indicating the plane types allowed.
        /// </param>
        /// <remarks>
        /// Unity places objects along the x/z of the grounding plane. When placed on an AR plane
        /// below the grounding plane, the object will drop straight down onto it in world space.
        /// This prevents the object from being pushed deeper into the scene when moving from a
        /// higher plane to a lower plane. When moving from a lower plane to a higher plane, this
        /// function returns a new groundingPlane to replace the old one.
        /// </remarks>
        [Obsolete(k_GetBestPlacementPositionDeprecated2, true)]
        public static Placement GetBestPlacementPosition(
            Vector3 currentAnchorPosition,
            Vector2 screenPos,
            float groundingPlaneHeight,
            float hoverOffset,
            float maxTranslationDistance,
            GestureTranslationMode gestureTranslationMode)
        {
            throw new NotSupportedException(k_GetBestPlacementPositionDeprecated2);
        }

        public partial struct Placement
        {
#pragma warning disable IDE1006 // Naming Styles
            /// <inheritdoc cref="hasHoveringPosition"/>
            [Obsolete("HasHoveringPosition has been deprecated. Use hasHoveringPosition instead. (UnityUpgradable) -> hasHoveringPosition", true)]
            public bool HasHoveringPosition
            {
                get => default;
                set => _ = value;
            }

            /// <inheritdoc cref="hoveringPosition"/>
            [Obsolete("HoveringPosition has been deprecated. Use hoveringPosition instead. (UnityUpgradable) -> hoveringPosition", true)]
            public Vector3 HoveringPosition
            {
                get => default;
                set => _ = value;
            }

            /// <inheritdoc cref="hasPlacementPosition"/>
            [Obsolete("HasPlacementPosition has been deprecated. Use hasPlacementPosition instead. (UnityUpgradable) -> hasPlacementPosition", true)]
            public bool HasPlacementPosition
            {
                get => default;
                set => _ = value;
            }

            /// <inheritdoc cref="placementPosition"/>
            [Obsolete("PlacementPosition has been deprecated. Use placementPosition instead. (UnityUpgradable) -> placementPosition", true)]
            public Vector3 PlacementPosition
            {
                get => default;
                set => _ = value;
            }

            /// <inheritdoc cref="placementRotation"/>
            [Obsolete("PlacementRotation has been deprecated. Use placementRotation instead. (UnityUpgradable) -> placementRotation", true)]
            public Quaternion PlacementRotation
            {
                get => default;
                set => _ = value;
            }

            /// <inheritdoc cref="hasPlane"/>
            [Obsolete("HasPlane has been deprecated. Use hasPlane instead. (UnityUpgradable) -> hasPlane", true)]
            public bool HasPlane
            {
                get => default;
                set => _ = value;
            }

            /// <inheritdoc cref="placementPlane"/>
            [Obsolete("PlacementPlane has been deprecated. Use placementPlane instead. (UnityUpgradable) -> placementPlane", true)]
            public ARPlane PlacementPlane
            {
                get => default;
                set => _ = value;
            }

            /// <inheritdoc cref="updatedGroundingPlaneHeight"/>
            [Obsolete("UpdatedGroundingPlaneHeight has been deprecated. Use updatedGroundingPlaneHeight instead. (UnityUpgradable) -> updatedGroundingPlaneHeight", true)]
            public float UpdatedGroundingPlaneHeight
            {
                get => default;
                set => _ = value;
            }
#pragma warning restore IDE1006 // Naming Styles
        }
    }
}

#endif
