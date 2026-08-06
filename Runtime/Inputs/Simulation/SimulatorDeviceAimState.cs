namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// Snapshot of the <see cref="XRInteractionSimulator"/> state that an
    /// <see cref="ISimulatorDeviceAimRaycastFilter"/> needs to evaluate a raycast hit.
    /// </summary>
    internal readonly struct SimulatorDeviceAimState
    {
        public readonly float cameraRadius;
        public readonly float rayOriginDistanceFromCamera;
        public readonly bool manipulatingLeftController;
        public readonly bool manipulatingRightController;
        public readonly bool manipulatingLeftHand;
        public readonly bool manipulatingRightHand;
        public readonly XRInputModalityManager inputModalityManager;

        public SimulatorDeviceAimState(float cameraRadius, float rayOriginDistanceFromCamera, bool manipulatingLeftController, bool manipulatingRightController, bool manipulatingLeftHand, bool manipulatingRightHand, XRInputModalityManager inputModalityManager)
        {
            this.cameraRadius = cameraRadius;
            this.rayOriginDistanceFromCamera = rayOriginDistanceFromCamera;
            this.manipulatingLeftController = manipulatingLeftController;
            this.manipulatingRightController = manipulatingRightController;
            this.manipulatingLeftHand = manipulatingLeftHand;
            this.manipulatingRightHand = manipulatingRightHand;
            this.inputModalityManager = inputModalityManager;
        }
    }
}
