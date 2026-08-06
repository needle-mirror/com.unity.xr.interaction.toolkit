using System;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation
{
    /// <summary>
    /// Default <see cref="ISimulatorDeviceAimRaycastFilter"/> used by the
    /// <see cref="XRInteractionSimulator"/> to decide which raycast hits should be
    /// discarded when aiming a simulated device at a world point during point-and-click.
    /// A hit is discarded when it is closer to the camera than the configured radius, or
    /// when it belongs to an <see cref="XRGrabInteractable"/> currently held by the device
    /// being manipulated.
    /// </summary>
    internal class XRSimulatorDeviceAimRaycastFilter : ISimulatorDeviceAimRaycastFilter
    {
        readonly Func<SimulatorDeviceAimState> m_StateGetter;

        public XRSimulatorDeviceAimRaycastFilter(Func<SimulatorDeviceAimState> stateGetter)
        {
            m_StateGetter = stateGetter;
        }

        /// <inheritdoc />
        public bool DiscardRaycastHit(RaycastHit raycastHit)
        {
            var state = m_StateGetter();

            // Hit distances are measured from the ray origin (on the near clip plane), while the
            // radius is measured from the camera, so subtract the ray origin's distance from the
            // camera (clamped to zero).
            var effectiveRadius = Mathf.Max(0f, state.cameraRadius - state.rayOriginDistanceFromCamera);

            if (raycastHit.distance < effectiveRadius)
                return true;

            return IsHeldByManipulatedDevice(raycastHit.collider, state);
        }

        static bool IsHeldByManipulatedDevice(Collider hitCollider, in SimulatorDeviceAimState state)
        {
            if (hitCollider == null)
                return false;

            if (!TryGetInteractableForCollider(hitCollider, out var interactable))
                return false;

            if (interactable is not XRGrabInteractable grab)
                return false;

            foreach (var interactor in grab.interactorsSelecting)
            {
                if (interactor.handedness == InteractorHandedness.None)
                    continue;

                if (IsManipulatedDevice(interactor, state))
                    return true;
            }

            return false;
        }

        // A scene can contain multiple interaction managers (Manager Singleton Mode = Allow Multiple),
        // each with its own collider-to-interactable map, so every active manager must be checked.
        static bool TryGetInteractableForCollider(Collider hitCollider, out IXRInteractable interactable)
        {
            var interactionManagers = XRInteractionManager.activeInteractionManagers;
            for (var i = 0; i < interactionManagers.Count; i++)
            {
                if (interactionManagers[i].TryGetInteractableForCollider(hitCollider, out interactable))
                    return true;
            }

            interactable = null;
            return false;
        }

        static bool IsManipulatedDevice(IXRSelectInteractor interactor, in SimulatorDeviceAimState state)
        {
            var interactorTransform = interactor.transform;
            var modalityManager = state.inputModalityManager;

            // NOTE: fallback to strictly a handedness check if there is no modality manager
            // since simulation only modifies one device type at a time for handedness

            if (modalityManager == null)
                return IsManipulatingHandedness(interactor, state);

            switch (interactor.handedness)
            {
                case InteractorHandedness.Left:
                    return (state.manipulatingLeftController && IsLeftController(modalityManager, interactorTransform)) ||
                           (state.manipulatingLeftHand && IsLeftHand(modalityManager, interactorTransform));
                case InteractorHandedness.Right:
                    return (state.manipulatingRightController && IsRightController(modalityManager, interactorTransform)) ||
                           (state.manipulatingRightHand && IsRightHand(modalityManager, interactorTransform));
                default:
                    return false;
            }
        }

        static bool IsManipulatingHandedness(IXRSelectInteractor interactor, in SimulatorDeviceAimState state)
        {
            switch (interactor.handedness)
            {
                case InteractorHandedness.Left:
                    return state.manipulatingLeftController || state.manipulatingLeftHand;
                case InteractorHandedness.Right:
                    return state.manipulatingRightController || state.manipulatingRightHand;
                default:
                    return false;
            }
        }

        static bool IsLeftController(XRInputModalityManager modalityManager, Transform interactorTransform)
        {
            return modalityManager.leftController != null && interactorTransform.IsChildOf(modalityManager.leftController.transform);
        }

        static bool IsRightController(XRInputModalityManager modalityManager, Transform interactorTransform)
        {
            return modalityManager.rightController != null && interactorTransform.IsChildOf(modalityManager.rightController.transform);
        }

        static bool IsLeftHand(XRInputModalityManager modalityManager, Transform interactorTransform)
        {
            return modalityManager.leftHand != null && interactorTransform.IsChildOf(modalityManager.leftHand.transform);
        }

        static bool IsRightHand(XRInputModalityManager modalityManager, Transform interactorTransform)
        {
            return modalityManager.rightHand != null && interactorTransform.IsChildOf(modalityManager.rightHand.transform);
        }
    }
}
