using System;

namespace UnityEngine.XR.Interaction.Toolkit
{
    public partial class XRControllerRecorder
    {
        [SerializeField, Tooltip("(Deprecated) XR Controller whose output will be recorded and played back.")]
        [Obsolete("m_XRController has been deprecated in version 3.0.0.")]
        XRBaseController m_XRController;

        /// <summary>
        /// (Deprecated) The controller that this recording uses for recording and playback.
        /// </summary>
        [Obsolete("xrController has been deprecated in version 3.0.0. Use interactor to allow the recorder to read and playback button input instead.")]
        public XRBaseController xrController
        {
            get => m_XRController;
            set => m_XRController = value;
        }
    }
}
