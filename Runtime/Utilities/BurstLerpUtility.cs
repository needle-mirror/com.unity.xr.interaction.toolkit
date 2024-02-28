#if BURST_PRESENT
using Unity.Burst;
#endif
using Unity.Mathematics;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
    /// <summary>
    /// Provides utility methods for various types of interpolations.
    /// </summary>
#if BURST_PRESENT
    [BurstCompile]
#endif
    public static class BurstLerpUtility
    {
        /// <summary>
        /// Performs a quadratic Bezier interpolation between two Vector3 values.
        /// This method uses the <c>BezierLerp</c> function for float3 types to perform the interpolation,
        /// allowing the method to be Burst-compiled for performance.
        /// </summary>
        /// <param name="start">Starting Vector3 value.</param>
        /// <param name="end">Ending Vector3 value.</param>
        /// <param name="t">Interpolation factor.</param>
        /// <param name="controlHeightFactor">Offset factor for the control point from the midpoint. Defaults to 0.5f.</param>
        /// <returns>The interpolated Vector3 result.</returns>
        public static Vector3 BezierLerp(in Vector3 start, in Vector3 end, float t, float controlHeightFactor = 0.5f)
        {
            BezierLerp(start, end, t, out float3 result, controlHeightFactor);
            return result;
        }

        /// <summary>
        /// Performs a quadratic Bezier interpolation between two float3 values using a single interpolation factor.
        /// Outputs the result in an out parameter.
        /// </summary>
        /// <param name="start">Starting float3 value.</param>
        /// <param name="end">Ending float3 value.</param>
        /// <param name="t">Interpolation factor.</param>
        /// <param name="controlHeightFactor">Offset factor for the control point from the midpoint. Defaults to 0.5f.</param>
        /// <param name="result">Output interpolated float3 result.</param>
#if BURST_PRESENT
        [BurstCompile]
#endif
        public static void BezierLerp(in float3 start, in float3 end, float t, out float3 result, float controlHeightFactor = 0.5f)
        {
            result = math.lerp(start, end, BezierLerp(0f, 1f, t, controlHeightFactor));
        }

        /// <summary>
        /// Performs a quadratic Bezier interpolation between two float values.
        /// This method calculates a control point as an offset from the midpoint between the start and end values, creating a curved path.
        /// </summary>
        /// <param name="start">Starting float value.</param>
        /// <param name="end">Ending float value.</param>
        /// <param name="t">Interpolation factor.</param>
        /// <param name="controlHeightFactor">Offset factor for the control point from the midpoint. Defaults to 0.5f.</param>
        /// <returns>The interpolated float result.</returns>
#if BURST_PRESENT
        [BurstCompile]
#endif
        public static float BezierLerp(float start, float end, float t, float controlHeightFactor = 0.5f)
        {
            float midPoint = (start + end) / 2;
            float control = midPoint + controlHeightFactor * (end - start);

            float oneMinusT = 1 - t;
            return oneMinusT * (oneMinusT * start + t * control) + t * (oneMinusT * control + t * end);
        }

        /// <summary>
        /// Performs a bounce-out interpolation between two Vector3 values. The interpolation creates a bounce effect towards the end.
        /// </summary>
        /// <param name="start">Starting Vector3 value.</param>
        /// <param name="end">Ending Vector3 value.</param>
        /// <param name="t">Interpolation factor, typically between 0 and 1.</param>
        /// <param name="speed">Speed factor of the bounce effect. Default is 1.</param>
        /// <returns>The interpolated Vector3 result.</returns>
        public static Vector3 BounceOutLerp(Vector3 start, Vector3 end, float t, float speed = 1f)
        {
            BounceOutLerp(start, end, t, out var result, speed);
            return result;
        }

        /// <summary>
        /// Performs a bounce-out interpolation between two float3 values. The interpolation creates a bounce effect towards the end.
        /// </summary>
        /// <param name="start">Starting float3 value.</param>
        /// <param name="end">Ending float3 value.</param>
        /// <param name="t">Interpolation factor, typically between 0 and 1.</param>
        /// <param name="result">The interpolated float3 result.</param>
        /// <param name="speed">Speed factor of the bounce effect. Default is 1.</param>
#if BURST_PRESENT
        [BurstCompile]
#endif
        public static void BounceOutLerp(in float3 start, in float3 end, float t, out float3 result, float speed = 1f)
        {
            result = math.lerp(start, end, EaseOutBounce(t, speed));
        }

        /// <summary>
        /// Performs a bounce-out interpolation between two float values. The interpolation creates a bounce effect towards the end.
        /// </summary>
        /// <param name="start">Starting float value.</param>
        /// <param name="end">Ending float value.</param>
        /// <param name="t">Interpolation factor, typically between 0 and 1.</param>
        /// <param name="speed">Speed factor of the bounce effect. Default is 1.</param>
        /// <returns>The interpolated float result.</returns>
#if BURST_PRESENT
        [BurstCompile]
#endif
        public static float BounceOutLerp(float start, float end, float t, float speed = 1f)
        {
            return math.lerp(start, end, EaseOutBounce(t, speed));
        }

        static float EaseOutBounce(float t, float speed = 1f)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            // Adjust t based on intensity
            t = Mathf.Clamp01(t * speed);

            if (t < 1 / d1)
            {
                return n1 * t * t;
            }
            else if (t < 2 / d1)
            {
                t -= 1.5f / d1;
                return n1 * t * t + 0.75f;
            }
            else if (t < 2.5 / d1)
            {
                t -= 2.25f / d1;
                return n1 * t * t + 0.9375f;
            }
            else
            {
                t -= 2.625f / d1;
                return n1 * t * t + 0.984375f;
            }
        }

        /// <summary>
        /// Performs a single bounce-out interpolation between two Vector3 values. 
        /// The interpolation creates a bounce effect towards the end.
        /// </summary>
        /// <param name="start">Starting Vector3 value.</param>
        /// <param name="end">Ending Vector3 value.</param>
        /// <param name="t">Interpolation factor, typically between 0 and 1.</param>
        /// <param name="speed">Speed factor of the bounce effect. Default is 1.</param>
        /// <returns>The interpolated Vector3 result.</returns>
        public static Vector3 SingleBounceOutLerp(Vector3 start, Vector3 end, float t, float speed = 1f)
        {
            BounceOutLerp(start, end, t, out float3 result, speed);
            return result;
        }

        /// <summary>
        /// Performs a single bounce-out interpolation between two float3 values. 
        /// The interpolation creates a bounce effect towards the end.
        /// </summary>
        /// <param name="start">Starting float3 value.</param>
        /// <param name="end">Ending float3 value.</param>
        /// <param name="t">Interpolation factor, typically between 0 and 1.</param>
        /// <param name="result">The interpolated float3 result.</param>
        /// <param name="speed">Speed factor of the bounce effect. Default is 1.</param>
#if BURST_PRESENT
        [BurstCompile]
#endif
        public static void SingleBounceOutLerp(in float3 start, in float3 end, float t, out float3 result, float speed = 1f)
        {
            result = math.lerp(start, end, EaseOutBounceSingle(t, speed));
        }

        /// <summary>
        /// Performs a single bounce-out interpolation between two float values. 
        /// The interpolation creates a bounce effect towards the end.
        /// </summary>
        /// <param name="start">Starting float value.</param>
        /// <param name="end">Ending float value.</param>
        /// <param name="t">Interpolation factor, typically between 0 and 1.</param>
        /// <param name="speed">Speed factor of the bounce effect. Default is 1.</param>
        /// <returns>The interpolated float result.</returns>
#if BURST_PRESENT
        [BurstCompile]
#endif
        public static float SingleBounceOutLerp(float start, float end, float t, float speed = 1f)
        {
            return math.lerp(start, end, EaseOutBounceSingle(t, speed));
        }

        static float EaseOutBounceSingle(float t, float speed = 1f)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            t = Mathf.Clamp01(t * speed);

            if (t < 1 / d1)
            {
                return n1 * t * t;
            }
            else if (t < 2 / d1)
            {
                t -= 1.5f / d1;
                return n1 * t * t + 0.75f;
            }
            else
            {
                t -= 2.25f / d1;
                return n1 * t * t + 0.9375f;
            }
        }
    }
}