namespace UnityEngine.XR.Interaction.Toolkit.Attachment
{
    /// <summary>
    /// Enum representing the stabilization mode for the anchor.
    /// </summary>
    /// <seealso cref="InteractionAttachController.motionStabilizationMode"/>
    public enum MotionStabilizationMode
    {
        /// <summary>
        /// Indicates that stabilization should never be applied.
        /// </summary>
        Never,

        /// <summary>
        /// Stabilization is applied only when there is a position offset.
        /// </summary>
        WithPositionOffset,

        /// <summary>
        /// Stabilization is always applied regardless of position offset.
        /// </summary>
        Always,
    }
    
    /// <summary>
    /// Interface defining the control and behavior of an interaction anchor.
    /// It includes methods for creating and updating anchors, managing motion stabilization, 
    /// and applying position and rotation offsets.
    /// </summary>
    public interface IInteractionAttachController
    {
        /// <summary>
        /// Gets or creates the transform used as the anchor, optionally updating its position and rotation.
        /// </summary>
        /// <param name="updateTransform">Whether to update the transform's position and rotation.</param>
        /// <returns>The transform used as the anchor.</returns>
        Transform GetOrCreateAnchorTransform(bool updateTransform = false);
        
        /// <summary>
        /// The transform that the anchor will follow.
        /// </summary>
        Transform transformToFollow { get; set; }
        
        /// <summary>
        /// The mode determining how motion stabilization is applied.
        /// </summary>
        MotionStabilizationMode motionStabilizationMode { get; set; }
        
        /// <summary>
        /// Indicates whether the anchor currently has an offset applied.
        /// </summary>
        bool hasOffset { get; }

        /// <summary>
        /// Moves the anchor child to a specified world position. Adjusts the position if it results in a negative z-value in local space.
        /// </summary>
        /// <param name="targetWorldPosition">The target world position to move the anchor child to.</param>
        void MoveTo(Vector3 targetWorldPosition);

        /// <summary>
        /// Applies a local position offset to the anchor child. Ensures the z-value of the new position is non-negative.
        /// </summary>
        /// <param name="offset">The local position offset to apply.</param>
        void ApplyLocalPositionOffset(Vector3 offset);

        /// <summary>
        /// Applies a local rotation offset to the anchor child.
        /// </summary>
        /// <param name="localRotation">The local rotation offset to apply.</param>
        void ApplyLocalRotationOffset(Quaternion localRotation);

        /// <summary>
        /// Resets the anchor child's position and rotation to the origin, removing any applied offsets.
        /// </summary>
        void ResetOffset();

        /// <summary>
        /// Updates the anchor based on motion stabilization settings and applies velocity-based offset calculations.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update.</param>
        void DoUpdate(float deltaTime);
    }
}