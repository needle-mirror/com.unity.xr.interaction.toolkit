using System;

namespace UnityEngine.XR.Interaction.Toolkit
{
    public partial class XRControllerRecorder
    {
        /// <summary>
        /// (Deprecated) The controller that this recording uses for recording and playback.
        /// </summary>
        [Obsolete("xrController has been deprecated in version 3.0.0. Use interactor to allow the recorder to read and playback button input instead.", true)]
        public XRBaseController xrController
        {
            get => default;
            set => _ = value;
        }
    }
}
