#if BURST_PRESENT
using Unity.Burst;
#endif
using Unity.Mathematics;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
    /// <summary>
    /// Provides utility methods for physics calculations in Burst-compiled code.
    /// </summary>
#if BURST_PRESENT
    [BurstCompile]
#endif
    public static class BurstPhysicsUtils
    {
        /// <summary>
        /// Computes sphere overlap parameters given the start and end positions of the overlap.
        /// </summary>
        /// <param name="overlapStart">The starting position of the sphere overlap.</param>
        /// <param name="overlapEnd">The ending position of the sphere overlap.</param>
        /// <param name="normalizedOverlapVector">Output parameter containing the normalized overlap direction vector.</param>
        /// <param name="overlapSqrMagnitude">Output parameter containing the square of the magnitude of the overlap vector.</param>
        /// <param name="overlapDistance">Output parameter containing the distance of the overlap.</param>
#if BURST_PRESENT
        [BurstCompile]
#endif
        public static void GetSphereOverlapParameters(in Vector3 overlapStart, in Vector3 overlapEnd, out Vector3 normalizedOverlapVector, out float overlapSqrMagnitude, out float overlapDistance)
        {
            Vector3 overlapDirectionVector = overlapEnd - overlapStart;
            overlapSqrMagnitude = math.distancesq(overlapStart, overlapEnd);
            overlapDistance = math.sqrt(overlapSqrMagnitude);
            normalizedOverlapVector = overlapDirectionVector / overlapDistance;
        }
    }
}