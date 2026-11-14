#if AR_FOUNDATION_PRESENT || PACKAGE_DOCS_GENERATION

using System;

namespace UnityEngine.XR.Interaction.Toolkit.AR
{
    public partial class TapGesture
    {
        const string k_ReinitializeDeprecated = "Reinitialize(Touch) is marked for deprecation in XRI 3.2.0 and will be removed in a future version. Use Reinitialize(InputSystem.EnhancedTouch.Touch) instead.";

#if !XRI_LEGACY_INPUT_DISABLED
        /// <summary>
        /// (Deprecated) Initializes and returns an instance of <see cref="TapGesture"/>.
        /// </summary>
        /// <param name="recognizer">The gesture recognizer.</param>
        /// <param name="touch">The touch that started this gesture.</param>
        /// <remarks>
        /// This is deprecated for its reference to Input Manager Touch. Set active input handling to New Input System, and use InputSystem.EnhancedTouch.Touch instead.
        /// </remarks>
        [Obsolete("TapGesture(TapGestureRecognizer, Touch) is marked for deprecation in XRI 3.2.0 and will be removed in a future version. Use TapGesture(TapGestureRecognizer, InputSystem.EnhancedTouch.Touch) instead.", true)]
        public TapGesture(TapGestureRecognizer recognizer, Touch touch)
            : this(recognizer, new CommonTouch(touch))
        {
        }

        [Obsolete(k_ReinitializeDeprecated, true)]
        internal void Reinitialize(Touch touch)
        {
            throw new NotSupportedException(k_ReinitializeDeprecated);
        }
#endif
    }
}
#endif
