#if BURST_PRESENT
using Unity.Burst;
#else
using System.Runtime.CompilerServices;
#endif
using Unity.Mathematics;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
    /// <summary>
    /// Provides utility functions related to vector and quaternion calculations,
    /// optimized for use with the Burst compiler when available.
    /// </summary>
#if BURST_PRESENT
    [BurstCompile]
#endif
    public static class BurstMathUtility
    {
        /// <summary>
        /// Calculates the orthogonal up vector for a given forward vector and a reference up vector.
        /// </summary>
        /// <param name="forward">The forward vector.</param>
        /// <param name="referenceUp">The reference up vector.</param>
        /// <param name="orthogonalUp">The calculated orthogonal up vector.</param>
        /// <remarks>
        /// Convenience method signature to cast output from <see cref="float3"/> to <see cref="Vector3"/>.
        /// </remarks>
#if BURST_PRESENT
        [BurstCompile]
#endif
        public static void OrthogonalUpVector(in Vector3 forward, in Vector3 referenceUp, out Vector3 orthogonalUp)
        {
            OrthogonalUpVector(forward, referenceUp, out float3 float3OrthogonalUp);
            orthogonalUp = float3OrthogonalUp;
        }

        /// <summary>
        /// Calculates the orthogonal up vector for a given forward vector and a reference up vector.
        /// </summary>
        /// <param name="forward">The forward vector.</param>
        /// <param name="referenceUp">The reference up vector.</param>
        /// <param name="orthogonalUp">The calculated orthogonal up vector.</param>
#if BURST_PRESENT
        [BurstCompile]
#endif
        public static void OrthogonalUpVector(in float3 forward, in float3 referenceUp, out float3 orthogonalUp)
        {
            var right = -math.cross(forward, referenceUp);
            orthogonalUp = math.cross(forward, right);
        }

        /// <summary>
        /// Calculates a look rotation quaternion given a forward vector and a reference up vector.
        /// </summary>
        /// <param name="forward">The forward vector.</param>
        /// <param name="referenceUp">The reference up vector.</param>
        /// <param name="lookRotation">The calculated look rotation quaternion.</param>
        /// <remarks>
        /// Convenience method signature to cast output from <see cref="quaternion"/> to <see cref="Quaternion"/>.
        /// </remarks>
#if BURST_PRESENT
        [BurstCompile]
#endif
        public static void OrthogonalLookRotation(in Vector3 forward, in Vector3 referenceUp, out Quaternion lookRotation)
        {
            OrthogonalLookRotation(forward, referenceUp, out quaternion lookRot);
            lookRotation = lookRot;
        }

        /// <summary>
        /// Calculates a look rotation quaternion given a forward vector and a reference up vector.
        /// </summary>
        /// <param name="forward">The forward vector.</param>
        /// <param name="referenceUp">The reference up vector.</param>
        /// <param name="lookRotation">The calculated look rotation quaternion.</param>
#if BURST_PRESENT
        [BurstCompile]
#endif
        public static void OrthogonalLookRotation(in float3 forward, in float3 referenceUp, out quaternion lookRotation)
        {
            OrthogonalUpVector(forward, referenceUp, out float3 orthogonalUp);
            lookRotation = quaternion.LookRotation(forward, orthogonalUp);
        }

        /// <summary>
        /// Projects a vector onto a plane defined by a normal orthogonal to the plane.
        /// </summary>
        /// <param name="vector">The vector to be projected.</param>
        /// <param name="planeNormal">The normal vector orthogonal to the plane.</param>
        /// <param name="projectedVector">The projected vector on the plane.</param>
#if BURST_PRESENT
        [BurstCompile]
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void ProjectOnPlane(in float3 vector, in float3 planeNormal, out float3 projectedVector)
        {
            var sqrMag = math.dot(planeNormal, planeNormal);
            if (sqrMag < math.EPSILON)
            {
                projectedVector = vector;
                return;
            }

            var dot = math.dot(vector, planeNormal);
            projectedVector = new float3(vector.x - planeNormal.x * dot / sqrMag,
                vector.y - planeNormal.y * dot / sqrMag,
                vector.z - planeNormal.z * dot / sqrMag);
        }

        /// <summary>
        /// Projects a vector onto a plane defined by a normal orthogonal to the plane.
        /// </summary>
        /// <param name="vector">The vector to be projected.</param>
        /// <param name="planeNormal">The normal vector orthogonal to the plane.</param>
        /// <param name="projectedVector">The projected vector on the plane.</param>
        /// <remarks>
        /// Convenience method signature to cast output from <see cref="float3"/> to <see cref="Vector3"/>.
        /// </remarks>
#if BURST_PRESENT
        [BurstCompile]
#endif
        public static void ProjectOnPlane(in Vector3 vector, in Vector3 planeNormal, out Vector3 projectedVector)
        {
            ProjectOnPlane(vector, planeNormal, out float3 float3ProjectedVector);
            projectedVector = float3ProjectedVector;
        }

        /// <summary>
        /// Computes the look rotation with the forward vector projected on a plane defined by a normal orthogonal to the plane.
        /// </summary>
        /// <param name="forward">The forward vector to be projected onto the plane.</param>
        /// <param name="planeNormal">The normal vector orthogonal to the plane.</param>
        /// <param name="lookRotation">The resulting look rotation with the projected forward vector and plane normal as up direction.</param>
#if BURST_PRESENT
        [BurstCompile]
#endif
        public static void LookRotationWithForwardProjectedOnPlane(in float3 forward, in float3 planeNormal, out quaternion lookRotation)
        {
            ProjectOnPlane(forward, planeNormal, out float3 projectedForward);
            lookRotation = quaternion.LookRotation(projectedForward, planeNormal);
        }

        /// <summary>
        /// Computes the look rotation with the forward vector projected on a plane defined by a normal orthogonal to the plane.
        /// </summary>
        /// <param name="forward">The forward vector to be projected onto the plane.</param>
        /// <param name="planeNormal">The normal vector orthogonal to the plane.</param>
        /// <param name="lookRotation">The resulting look rotation with the projected forward vector and plane normal as up direction.</param>
        /// <remarks>
        /// Convenience method signature to cast output from <see cref="quaternion"/> to <see cref="Quaternion"/>.
        /// </remarks>
#if BURST_PRESENT
        [BurstCompile]
#endif
        public static void LookRotationWithForwardProjectedOnPlane(in Vector3 forward, in Vector3 planeNormal, out Quaternion lookRotation)
        {
            LookRotationWithForwardProjectedOnPlane(forward, planeNormal, out quaternion lookRot);
            lookRotation = lookRot;
        }
    }
}