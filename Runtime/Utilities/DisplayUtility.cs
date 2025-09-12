namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
    static class DisplayUtility
    {
        static float s_ScreenDpi = 100f;
        static float s_OneOverScreenDpi = 1f / s_ScreenDpi;
        static bool s_ScreenDpiChecked;

        /// <summary>
        /// This wrapper property checks whether the Screen.dpi value has been set by the
        /// underlying platform. If it has not, it defaults to a 100 dpi default value to
        /// use for dpi-based calculations.
        /// </summary>
        public static float screenDpi
        {
            get
            {
                CacheScreenDpi();
                return s_ScreenDpi;
            }
        }

        /// <summary>
        /// Convenience property to get the value of (1 / <see cref="screenDpi"/>) that
        /// gets cached when the screenDpi is checked.
        /// </summary>
        public static float screenDpiRatio
        {
            get
            {
                CacheScreenDpi();
                return s_OneOverScreenDpi;
            }
        }

        static void CacheScreenDpi()
        {
            if (!s_ScreenDpiChecked)
            {
                if (Screen.dpi > 0)
                    s_ScreenDpi = Screen.dpi;
                else
                    Debug.LogWarning("Platform has reported a screen DPI of 0. Using default value of 100.");

                s_OneOverScreenDpi = 1f / s_ScreenDpi;
                s_ScreenDpiChecked = true;
            }
        }

        /// <summary>
        /// Converts Pixels to Inches.
        /// </summary>
        /// <param name="pixels">The amount to convert in pixels.</param>
        /// <returns>The converted amount in inches.</returns>
        public static float PixelsToInches(float pixels) => pixels * screenDpiRatio;

        /// <summary>
        /// Converts Inches to Pixels.
        /// </summary>
        /// <param name="inches">The amount to convert in inches.</param>
        /// <returns>The converted amount in pixels.</returns>
        public static float InchesToPixels(float inches) => inches * screenDpi;
    }
}
