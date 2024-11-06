using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// Utility functions related to <see cref="Gizmos"/>.
    /// </summary>
    public static class GizmoHelpers
    {
        static readonly Color s_XAxisColor = new Color(219f / 255, 62f / 255, 29f / 255, .93f);
        static readonly Color s_YAxisColor = new Color(154f / 255, 243f / 255, 72f / 255, .93f);
        static readonly Color s_ZAxisColor = new Color(58f / 255, 122f / 255, 248f / 255, .93f);

        // Helper dictionary for easier axis lookup, adapted from lookup mechanism in Editor/Mono/EditorHandles/BoundsHandle/CapsuleBoundsHandle.cs
        static readonly Dictionary<Vector3, (Vector3, Vector3)> s_AxisMapping = new Dictionary<Vector3, (Vector3, Vector3)>()
        {
            {Vector3.up, (Vector3.forward, Vector3.right)},
            {Vector3.forward, (Vector3.right, Vector3.up)},
            {Vector3.right, (Vector3.up, Vector3.forward)},
        };

        /// <summary>
        /// Draws oriented wire plane.
        /// </summary>
        /// <param name="position"> Position of the plane.</param>
        /// <param name="rotation"> Rotation of the plane.</param>
        /// <param name="size"> Size of the plane.</param>
        public static void DrawWirePlaneOriented(Vector3 position, Quaternion rotation, float size)
        {
            var halfSize = size / 2f;
            var tl = new Vector3(halfSize, 0f, -halfSize);
            var tr = new Vector3(halfSize, 0f, halfSize);
            var bl = new Vector3(-halfSize, 0f, -halfSize);
            var br = new Vector3(-halfSize, 0f, halfSize);

            Gizmos.DrawLine((rotation * tl) + position,
                (rotation * tr) + position);

            Gizmos.DrawLine((rotation * tr) + position,
                (rotation * br) + position);

            Gizmos.DrawLine((rotation * br) + position,
                (rotation * bl) + position);

            Gizmos.DrawLine((rotation * bl) + position,
                (rotation * tl) + position);
        }

        /// <summary>
        /// Draws oriented wire cube.
        /// </summary>
        /// <param name="position"> Position of the cube.</param>
        /// <param name="rotation"> Rotation of the cube.</param>
        /// <param name="size"> Size of the cube.</param>
        public static void DrawWireCubeOriented(Vector3 position, Quaternion rotation, float size)
        {
            var halfSize = size / 2f;
            var tl = new Vector3(halfSize, 0f, -halfSize);
            var tr = new Vector3(halfSize, 0f, halfSize);
            var bl = new Vector3(-halfSize, 0f, -halfSize);
            var br = new Vector3(-halfSize, 0f, halfSize);

            var tlt = new Vector3(halfSize, size, -halfSize);
            var trt = new Vector3(halfSize, size, halfSize);
            var blt = new Vector3(-halfSize, size, -halfSize);
            var brt = new Vector3(-halfSize, size, halfSize);

            Gizmos.DrawLine((rotation * tl) + position, (rotation * tr) + position);

            Gizmos.DrawLine((rotation * tr) + position, (rotation * br) + position);

            Gizmos.DrawLine((rotation * br) + position, (rotation * bl) + position);

            Gizmos.DrawLine((rotation * bl) + position, (rotation * tl) + position);

            Gizmos.DrawLine((rotation * tlt) + position, (rotation * trt) + position);

            Gizmos.DrawLine((rotation * trt) + position, (rotation * brt) + position);

            Gizmos.DrawLine((rotation * brt) + position, (rotation * blt) + position);

            Gizmos.DrawLine((rotation * blt) + position, (rotation * tlt) + position);

            Gizmos.DrawLine((rotation * tlt) + position, (rotation * tl) + position);

            Gizmos.DrawLine((rotation * trt) + position, (rotation * tr) + position);

            Gizmos.DrawLine((rotation * brt) + position, (rotation * br) + position);

            Gizmos.DrawLine((rotation * blt) + position, (rotation * bl) + position);
        }

        /// <summary>
        /// Draws world space standard basis vectors at <paramref name="transform"/>.
        /// </summary>
        /// <param name="transform">The <see cref="Transform"/> to represent.</param>
        /// <param name="size">Length of each ray.</param>
        public static void DrawAxisArrows(Transform transform, float size)
        {
            var position = transform.position;

            Gizmos.color = s_ZAxisColor;
            Gizmos.DrawRay(position, transform.forward * size);

            Gizmos.color = s_YAxisColor;
            Gizmos.DrawRay(position, transform.up * size);

            Gizmos.color = s_XAxisColor;
            Gizmos.DrawRay(position, transform.right * size);
        }

        /// <summary>
        /// Draws a capsule gizmo in the Scene view at the center location defined.
        /// Only functional in the Unity Editor.
        /// </summary>
        /// <param name="center">Center of the capsule.</param>
        /// <param name="height">Height of the capsule to be drawn, not including the radius.</param>
        /// <param name="radius">Radius of the arcs of the capsule being drawn.</param>
        /// <param name="axis">Direction the capsule will be drawn.</param>
        /// <param name="color">Color of the capsule.</param>
        /// <remarks>
        /// <paramref name="axis"/> must be set to one of the 3 following: <see cref="Vector3.up"/>, <see cref="Vector3.forward"/>, or <see cref="Vector3.right"/>
        /// </remarks>
        internal static void DrawCapsule(Vector3 center, float height, float radius, Vector3 axis, Color color)
        {
#if UNITY_EDITOR
            if (!s_AxisMapping.TryGetValue(axis, out var mapping))
            {
                axis = Vector3.up;
                mapping = s_AxisMapping[axis];
            }

            Vector3 heightAxis = axis;
            Vector3 forwardAxis = mapping.Item1;
            Vector3 rightAxis = mapping.Item2;

            float halfHeight = height * 0.5f;
            Vector3 top = center + heightAxis * (halfHeight - radius);
            Vector3 bottom = center - heightAxis * (halfHeight - radius);

            Handles.color = color;
            Handles.DrawWireArc(top, forwardAxis, rightAxis, 180f, radius);
            Handles.DrawWireArc(bottom, forwardAxis, rightAxis, -180f, radius);
            Handles.DrawLine(top + rightAxis * radius, bottom + rightAxis * radius);
            Handles.DrawLine(top - rightAxis * radius, bottom - rightAxis * radius);
            Handles.DrawWireArc(top, rightAxis, forwardAxis, -180f, radius);
            Handles.DrawWireArc(bottom, rightAxis, forwardAxis, 180f, radius);
            Handles.DrawLine(top + forwardAxis * radius, bottom + forwardAxis * radius);
            Handles.DrawLine(top - forwardAxis * radius, bottom - forwardAxis * radius);

            Handles.DrawWireArc(top, heightAxis, forwardAxis, 360f, radius);
            Handles.DrawWireArc(bottom, heightAxis, forwardAxis, -360f, radius);
#endif
        }
    }
}
