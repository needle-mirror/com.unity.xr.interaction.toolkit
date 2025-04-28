namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
    /// <summary>
    /// Utility methods for matrix operations.
    /// </summary>
    static class MatrixUtility
    {
        /// <summary>
        /// Applies a Matrix4x4 to a Transform by decomposing the matrix into position, rotation, and scale components.
        /// </summary>
        /// <param name="transform">The Transform component to modify.</param>
        /// <param name="worldMatrix">The world space Matrix4x4 to apply.</param>
        /// <remarks>
        /// This utility method extracts the translation (position), rotation, and scale from the provided matrix
        /// and applies these components to the Transform. This is useful when you need to set a Transform's
        /// properties from a computed or manipulated world matrix.
        /// <br />
        /// Note that for skewed or non-uniformly scaled matrices, the decomposed values might not perfectly
        /// recreate the original matrix when recomposed.
        /// </remarks>
        // ReSharper disable once InconsistentNaming -- Match Matrix4x4 type name
        public static void ApplyMatrix4x4(Transform transform, Matrix4x4 worldMatrix)
        {
            // Extract position (translation component of the matrix)
            Vector3 position = worldMatrix.GetPosition();

            // Extract rotation
            Quaternion rotation = worldMatrix.rotation;

            // Extract scale
            Vector3 scale = new Vector3(
                worldMatrix.GetColumn(0).magnitude,
                worldMatrix.GetColumn(1).magnitude,
                worldMatrix.GetColumn(2).magnitude
            );

            // Apply to transform
            transform.SetPositionAndRotation(position, rotation);
            transform.localScale = scale;
        }
    }
}
