#if AR_FOUNDATION_PRESENT || PACKAGE_DOCS_GENERATION

using System;

namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    public partial class PinchGesture
    {
#if !XRI_LEGACY_INPUT_DISABLED
        /// <summary>
        /// (Deprecated) Initializes and returns an instance of <see cref="PinchGesture"/>.
        /// </summary>
        /// <param name="recognizer">The gesture recognizer.</param>
        /// <param name="touch1">The first touch that started this gesture.</param>
        /// <param name="touch2">The second touch that started this gesture.</param>
        /// <remarks>
        /// This is deprecated for its reference to Input Manager Touch. Set active input handling to New Input System, and use InputSystem.EnhancedTouch.Touch instead.
        /// </remarks>
        [Obsolete("PinchGesture(PinchGestureRecognizer, Touch, Touch) is marked for deprecation in XRI 3.2.0 and will be removed in a future version. Use PinchGesture(PinchGestureRecognizer, InputSystem.EnhancedTouch.Touch, InputSystem.EnhancedTouch.Touch) instead.")]
        public PinchGesture(PinchGestureRecognizer recognizer, Touch touch1, Touch touch2)
            : this(recognizer, new CommonTouch(touch1), new CommonTouch(touch2))
        {
        }

        [Obsolete("Reinitialize(Touch, Touch) is marked for deprecation in XRI 3.2.0 and will be removed in a future version. Use Reinitialize(InputSystem.EnhancedTouch.Touch, InputSystem.EnhancedTouch.Touch) instead.")]
        internal void Reinitialize(Touch touch1, Touch touch2) => Reinitialize(new CommonTouch(touch1), new CommonTouch(touch2));
#endif
    }
}
#endif
