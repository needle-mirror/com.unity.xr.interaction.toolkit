using System;

namespace UnityEngine.XR.Interaction.Toolkit
{
    public abstract partial class XRBaseController
    {
#pragma warning disable 618
        /// <summary>
        /// (Deprecated) Gets the state of the controller.
        /// </summary>
        /// <param name="controllerState">When this method returns, contains the <see cref="XRControllerState"/> object representing the state of the controller.</param>
        /// <returns>Returns <see langword="false"/>.</returns>
        /// <seealso cref="currentControllerState"/>
        [Obsolete("GetControllerState has been deprecated. Use currentControllerState instead.")]
        public virtual bool GetControllerState(out XRControllerState controllerState)
        {
            controllerState = currentControllerState;
            return false;
        }

        /// <summary>
        /// (Deprecated) Sets the state of the controller.
        /// </summary>
        /// <param name="controllerState">The state of the controller to set.</param>
        /// <seealso cref="currentControllerState"/>
        [Obsolete("SetControllerState has been deprecated. Use currentControllerState instead.")]
        public virtual void SetControllerState(XRControllerState controllerState)
        {
            currentControllerState = controllerState;
        }
#pragma warning restore 618
    }
}
