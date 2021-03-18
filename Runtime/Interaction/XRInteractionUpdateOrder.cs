using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// The update order for <see cref="MonoBehaviour"/>s in XR Interaction.
    /// </summary>
    /// <remarks>
    /// This is primarily used to control initialization order as the update of interactors / interaction manager / interactables is handled by the
    /// Interaction managers themselves.
    /// </remarks>
    public static class XRInteractionUpdateOrder
    {
        /// <summary>
        /// Order when instances of type <see cref="XRControllerRecorder"/> are updated.
        /// </summary>
        public const int k_ControllerRecorder = -30000;

        /// <summary>
        /// Order when instances of type <see cref="XRDeviceSimulator"/> are updated.
        /// </summary>
        public const int k_DeviceSimulator = k_Controllers - 1;

        /// <summary>
        /// Order when instances of type <see cref="XRBaseController"/> are updated.
        /// </summary>
        public const int k_Controllers = k_ControllerRecorder + 10;

        /// <summary>
        /// Order when instances of type <see cref="LocomotionProvider"/> are updated.
        /// </summary>
        public const int k_LocomotionProviders = k_UIInputModule - 10;

        /// <summary>
        /// Order when instances of type <see cref="UIInputModule"/> are updated.
        /// </summary>
        public const int k_UIInputModule = -200;

        /// <summary>
        /// Order when <see cref="XRInteractionManager"/> is updated.
        /// </summary>
        public const int k_InteractionManager = -100;

        /// <summary>
        /// Order when instances of type <see cref="XRBaseInteractor"/> are updated.
        /// </summary>
        public const int k_Interactors = k_InteractionManager + 1;

        /// <summary>
        /// Order when instances of type <see cref="XRBaseInteractable"/> are updated.
        /// </summary>
        public const int k_Interactables = k_Interactors + 1;

        /// <summary>
        /// Order when instances of type <see cref="XRInteractorLineVisual"/> are updated.
        /// </summary>
        public const int k_LineVisual = 100;

        /// <summary>
        /// Order when <see cref="XRInteractionManager.OnBeforeRender"/> is called.
        /// </summary>
        public const int k_BeforeRenderOrder = 100;

        /// <summary>
        /// Order when <see cref="XRInteractorLineVisual.OnBeforeRenderLineVisual"/> is called.
        /// </summary>
        public const int k_BeforeRenderLineVisual = k_BeforeRenderOrder + 1;

        /// <summary>
        /// The phase in which updates happen.
        /// </summary>
        public enum UpdatePhase
        {
            /// <summary>
            /// Frame-rate independent.
            /// </summary>
            Fixed,

            /// <summary>
            /// Called every frame.
            /// </summary>
            Dynamic,

            /// <summary>
            /// Called at the end of every frame.
            /// </summary>
            Late,

             /// <summary>
            /// Called just before render.
            /// </summary>
            OnBeforeRender,
        }
    }
}
