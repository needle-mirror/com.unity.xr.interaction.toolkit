using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
    /// <summary>
    /// Utility to facilitate an efficient interpolation between two gradients.
    /// </summary>
    public class GradientUtility
    {
        // Maximum number of gradient color keys allowed.
        const int k_MaxGradientColorKeys = 8;

        // Precision level for truncating the key times, useful for handling very close values.
        const int k_Precision = 100;

        // Lists to store the times of color and alpha keys.
        static readonly List<float> s_ColorKeyTimes = new List<float>();
        static readonly HashSet<int> s_TruncatedColorKeyTimes = new HashSet<int>();
        static readonly List<float> s_AlphaKeyTimes = new List<float>();
        static readonly HashSet<int> s_TruncatedAlphaKeyTimes = new HashSet<int>();

        // Static arrays for various key counts to reduce allocations.
        static readonly GradientColorKey[][] s_ColorKeyArrays = new GradientColorKey[7][];
        static readonly GradientAlphaKey[][] s_AlphaKeyArrays = new GradientAlphaKey[7][];

        // Static constructor to initialize the key arrays.
        static GradientUtility()
        {
            for (int i = 2; i <= 8; i++)
            {
                s_ColorKeyArrays[i - 2] = new GradientColorKey[i];
                s_AlphaKeyArrays[i - 2] = new GradientAlphaKey[i];
            }
        }

        /// <summary>
        /// Linearly interpolates between two gradients with options to independently skip color or alpha interpolation.
        /// </summary>
        /// <param name="a">The first gradient.</param>
        /// <param name="b">The second gradient.</param>
        /// <param name="t">Interpolation factor (0 to 1).</param>
        /// <param name="lerpColors">If true, interpolate colors; otherwise, use colors from gradient b.</param>
        /// <param name="lerpAlphas">If true, interpolate alphas; otherwise, use alphas from gradient b.</param>
        /// <returns>Returns the interpolated gradient as a new gradient instance.</returns>
        /// <remarks>
        /// This method allocates a new Gradient instance for the interpolated result. Consider using the other
        /// <c>Lerp</c> method to provide a pre-allocated Gradient instance to avoid GC allocations.
        /// </remarks>
        public static Gradient Lerp(Gradient a, Gradient b, float t, bool lerpColors = true, bool lerpAlphas = true)
        {
            // Create and return the interpolated gradient.
            Gradient g = new Gradient();
            Lerp(a, b, g, t, lerpColors, lerpAlphas);
            return g;
        }

        /// <summary>
        /// Linearly interpolates between two gradients with options to independently skip color or alpha interpolation.
        /// </summary>
        /// <param name="a">The first gradient.</param>
        /// <param name="b">The second gradient.</param>
        /// <param name="output">The interpolated gradient.</param>
        /// <param name="t">Interpolation factor (0 to 1).</param>
        /// <param name="lerpColors">If true, interpolate colors; otherwise, use colors from gradient b.</param>
        /// <param name="lerpAlphas">If true, interpolate alphas; otherwise, use alphas from gradient b.</param>
        public static void Lerp(Gradient a, Gradient b, Gradient output, float t, bool lerpColors = true, bool lerpAlphas = true)
        {
            // Clear previous keys to prepare for new interpolation.
            s_ColorKeyTimes.Clear();
            s_TruncatedColorKeyTimes.Clear();
            s_AlphaKeyTimes.Clear();
            s_TruncatedAlphaKeyTimes.Clear();

            // Add unique keys from both gradients based on the specified interpolation flags.
            if (lerpColors)
            {
                AddUniqueColorKeys(a.colorKeys);
                AddUniqueColorKeys(b.colorKeys);
            }

            if (lerpAlphas)
            {
                AddUniqueAlphaKeys(a.alphaKeys);
                AddUniqueAlphaKeys(b.alphaKeys);
            }

            // Reduce the number of keys to meet the maximum limit.
            ReduceKeysIfNeeded(s_ColorKeyTimes, k_MaxGradientColorKeys);
            ReduceKeysIfNeeded(s_AlphaKeyTimes, k_MaxGradientColorKeys);

            // Prepare the final color and alpha keys for the gradient.
            var colorKeys = lerpColors ? PrepareColorKeys(s_ColorKeyTimes, a, b, t) : b.colorKeys;
            var alphaKeys = lerpAlphas ? PrepareAlphaKeys(s_AlphaKeyTimes, a, b, t) : b.alphaKeys;

            // Create and return the interpolated gradient.
            output.SetKeys(colorKeys, alphaKeys);
        }

        /// <summary>
        /// Copies the color and alpha keys from a source Gradient to a destination Gradient.
        /// </summary>
        /// <param name="source">The Gradient to copy from.</param>
        /// <param name="destination">The Gradient to copy to. This Gradient is modified by this method.</param>
        public static void CopyGradient(Gradient source, Gradient destination)
        {
            destination.SetKeys(source.colorKeys, source.alphaKeys);
        }

        // Adds unique color keys to the key times list, avoiding duplicates.
        static void AddUniqueColorKeys(GradientColorKey[] keys)
        {
            foreach (var key in keys)
                AddColorKeyIfUnique(key.time);
        }

        // Adds unique alpha keys to the key times list, avoiding duplicates.
        static void AddUniqueAlphaKeys(GradientAlphaKey[] keys)
        {
            foreach (var key in keys)
                AddAlphaKeyIfUnique(key.time);
        }

        // Adds a color key to the list if it is unique, determined by truncated precision.
        static void AddColorKeyIfUnique(float keyTime)
        {
            int truncatedKey = TruncatePrecision(keyTime);
            if (s_TruncatedColorKeyTimes.Add(truncatedKey))
                s_ColorKeyTimes.Add(keyTime);
        }

        // Adds an alpha key to the list if it is unique, determined by truncated precision.
        static void AddAlphaKeyIfUnique(float keyTime)
        {
            int truncatedKey = TruncatePrecision(keyTime);
            if (s_TruncatedAlphaKeyTimes.Add(truncatedKey))
                s_AlphaKeyTimes.Add(keyTime);
        }

        // Prepares color keys for the final gradient based on the interpolation settings.
        static GradientColorKey[] PrepareColorKeys(List<float> keyTimes, Gradient a, Gradient b, float t)
        {
            int keyTimesCount = keyTimes.Count;
            GradientColorKey[] clrs = GetColorKeyArray(keyTimesCount);
            for (int i = 0; i < keyTimesCount; i++)
            {
                float key = keyTimes[i];
                Color clr = Color.Lerp(a.Evaluate(key), b.Evaluate(key), t);
                clrs[i] = new GradientColorKey(clr, key);
            }

            return clrs;
        }

        // Prepares alpha keys for the final gradient based on the interpolation settings.
        static GradientAlphaKey[] PrepareAlphaKeys(List<float> keyTimes, Gradient a, Gradient b, float t)
        {
            int keyTimesCount = keyTimes.Count;
            GradientAlphaKey[] alphas = GetAlphaKeyArray(keyTimesCount);
            for (int i = 0; i < keyTimesCount; i++)
            {
                float key = keyTimes[i];
                float alpha = Mathf.Lerp(a.Evaluate(key).a, b.Evaluate(key).a, t);
                alphas[i] = new GradientAlphaKey(alpha, key);
            }

            return alphas;
        }

        // Truncates the precision of a float value for determining uniqueness of gradient keys.
        static int TruncatePrecision(float value) => (int)(value * k_Precision);

        // Reduces the number of keys if they exceed the maximum limit by removing the middle key.
        static void ReduceKeysIfNeeded(List<float> keyTimes, int maxKeys)
        {
            while (keyTimes.Count > maxKeys)
            {
                // Remove the middle key to maintain the gradient's shape as much as possible.
                int midPointIndex = keyTimes.Count / 2;
                keyTimes.RemoveAt(midPointIndex);
            }
        }

        // Retrieves a pre-allocated array of GradientColorKey of the specified size.
        static GradientColorKey[] GetColorKeyArray(int size)
        {
            return size >= 2 && size <= 8 ? s_ColorKeyArrays[size - 2] : new GradientColorKey[size];
        }

        // Retrieves a pre-allocated array of GradientAlphaKey of the specified size.
        static GradientAlphaKey[] GetAlphaKeyArray(int size)
        {
            return size >= 2 && size <= 8 ? s_AlphaKeyArrays[size - 2] : new GradientAlphaKey[size];
        }
    }
}
