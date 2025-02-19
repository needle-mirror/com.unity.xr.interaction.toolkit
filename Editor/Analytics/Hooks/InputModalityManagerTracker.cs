#if ENABLE_CLOUD_SERVICES_ANALYTICS || UNITY_2023_2_OR_NEWER

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace UnityEditor.XR.Interaction.Toolkit.Analytics.Hooks
{
    /// <summary>
    /// Runtime tracker for XR Input Modality Manager modality events.
    /// This class is used to track the number of enabled modality managers along with
    /// the duration that it's in controller or hand mode during a play mode session.
    /// </summary>
    /// <seealso cref="UpdateEventPayload"/>
    /// <seealso cref="XRInputModalityManager"/>
    class InputModalityManagerTracker
    {
        class InputModeTracker
        {
            public enum Handedness
            {
                Left,
                Right,
            }

            public double durationNone { get; private set; }

            public double durationTrackedHand { get; private set; }

            public double durationMotionController { get; private set; }

            public bool trackedHandAssigned { get; private set; }

            public bool motionControllerAssigned { get; private set; }

            readonly Handedness m_Handedness;

            int m_NoneCount;
            int m_TrackedHandCount;
            int m_MotionControllerCount;

            double m_NoneTimestamp;
            double m_TrackedHandTimestamp;
            double m_MotionControllerTimestamp;

            Dictionary<XRInputModalityManager, XRInputModalityManager.InputMode> m_InputModes =
                new Dictionary<XRInputModalityManager, XRInputModalityManager.InputMode>();

            public InputModeTracker(Handedness handedness)
            {
                m_Handedness = handedness;
            }

            public void Reset()
            {
                durationNone = 0d;
                durationTrackedHand = 0d;
                durationMotionController = 0d;
                trackedHandAssigned = false;
                motionControllerAssigned = false;
                m_NoneCount = 0;
                m_TrackedHandCount = 0;
                m_MotionControllerCount = 0;
                m_NoneTimestamp = 0d;
                m_TrackedHandTimestamp = 0d;
                m_MotionControllerTimestamp = 0d;

                m_InputModes.Clear();
            }

            public void StartSession(XRInputModalityManager manager, double now)
            {
                QueryAssignedState(manager);

                var inputMode = GetInputMode(manager);
                Increment(inputMode, now);
                m_InputModes[manager] = inputMode;
            }

            public void UpdateMode(XRInputModalityManager manager, double now)
            {
                QueryAssignedState(manager);

                if (!m_InputModes.TryGetValue(manager, out var previousMode))
                    return;

                var inputMode = GetInputMode(manager);
                if (previousMode == inputMode)
                    return;

                Decrement(previousMode, now);
                Increment(inputMode, now);
                m_InputModes[manager] = inputMode;
            }

            public void EndSession(XRInputModalityManager manager, double now)
            {
                QueryAssignedState(manager);

                if (m_InputModes.TryGetValue(manager, out var previousMode))
                {
                    Decrement(previousMode, now);
                    m_InputModes.Remove(manager);
                }
            }

            void Increment(XRInputModalityManager.InputMode inputMode, double now)
            {
                switch (inputMode)
                {
                    case XRInputModalityManager.InputMode.None:
                        m_NoneCount++;
                        if (m_NoneCount == 1)
                            m_NoneTimestamp = now;
                        break;
                    case XRInputModalityManager.InputMode.TrackedHand:
                        m_TrackedHandCount++;
                        if (m_TrackedHandCount == 1)
                            m_TrackedHandTimestamp = now;
                        break;
                    case XRInputModalityManager.InputMode.MotionController:
                        m_MotionControllerCount++;
                        if (m_MotionControllerCount == 1)
                            m_MotionControllerTimestamp = now;
                        break;
                }
            }

            void Decrement(XRInputModalityManager.InputMode previousMode, double now)
            {
                switch (previousMode)
                {
                    case XRInputModalityManager.InputMode.None:
                        m_NoneCount--;
                        if (m_NoneCount == 0)
                            durationNone += now - m_NoneTimestamp;
                        break;
                    case XRInputModalityManager.InputMode.TrackedHand:
                        m_TrackedHandCount--;
                        if (m_TrackedHandCount == 0)
                            durationTrackedHand += now - m_TrackedHandTimestamp;
                        break;
                    case XRInputModalityManager.InputMode.MotionController:
                        m_MotionControllerCount--;
                        if (m_MotionControllerCount == 0)
                            durationMotionController += now - m_MotionControllerTimestamp;
                        break;
                }
            }

            void QueryAssignedState(XRInputModalityManager manager)
            {
                trackedHandAssigned |= GetHandGameObject(manager) != null;
                motionControllerAssigned |= GetControllerGameObject(manager) != null;
            }

            XRInputModalityManager.InputMode GetInputMode(XRInputModalityManager manager)
            {
                switch (m_Handedness)
                {
                    case Handedness.Left:
                        return manager.leftInputMode;
                    case Handedness.Right:
                        return manager.rightInputMode;
                    default:
                        return XRInputModalityManager.InputMode.None;
                }
            }

            GameObject GetHandGameObject(XRInputModalityManager manager)
            {
                switch (m_Handedness)
                {
                    case Handedness.Left:
                        return manager.leftHand;
                    case Handedness.Right:
                        return manager.rightHand;
                    default:
                        return null;
                }
            }

            GameObject GetControllerGameObject(XRInputModalityManager manager)
            {
                switch (m_Handedness)
                {
                    case Handedness.Left:
                        return manager.leftController;
                    case Handedness.Right:
                        return manager.rightController;
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// Duration in seconds of at least one enabled input modality manager during a play mode session.
        /// </summary>
        /// <remarks>
        /// This duration can range from 0 seconds to the total length of the play mode session,
        /// and does not differ based on multiple managers being enabled simultaneously.
        /// </remarks>
        public double sessionDuration { get; private set; }

        /// <summary>
        /// Peak number of enabled input modality managers during a play mode session.
        /// </summary>
        public int modalityManagersPeakCount { get; private set; }

        /// <summary>
        /// Total count of different input modality managers that were enabled during a play mode session.
        /// </summary>
        public int modalityManagersObjectCount => m_AllManagers.Count;

        List<XRInputModalityManager> m_EnabledManagers = new List<XRInputModalityManager>();

        List<XRInputModalityManager> m_AllManagers = new List<XRInputModalityManager>();

        InputModeTracker m_LeftModeTracker = new InputModeTracker(InputModeTracker.Handedness.Left);
        InputModeTracker m_RightModeTracker = new InputModeTracker(InputModeTracker.Handedness.Right);

        double m_StartTimestamp;

        /// <summary>
        /// Reset the tracker to its initial state.
        /// Call this after generating the analytics payload to avoid accumulating data across
        /// multiple play mode sessions when domain reload is disabled.
        /// </summary>
        public void Cleanup()
        {
            foreach (var manager in m_AllManagers)
            {
                if (manager != null)
                {
                    manager.leftInputModeChanged -= OnLeftInputModeChanged;
                    manager.rightInputModeChanged -= OnRightInputModeChanged;
                }
            }

            sessionDuration = 0d;
            modalityManagersPeakCount = 0;

            m_EnabledManagers.Clear();
            m_AllManagers.Clear();

            m_LeftModeTracker.Reset();
            m_RightModeTracker.Reset();

            m_StartTimestamp = 0d;
        }

        /// <summary>
        /// Start tracking the manager component, invoked when its <c>OnEnable</c> is called.
        /// </summary>
        /// <param name="manager">The manager component to track.</param>
        /// <param name="now">Current timestamp.</param>
        public void StartSession(XRInputModalityManager manager, double now)
        {
            if (m_EnabledManagers.Contains(manager))
                return;

            m_EnabledManagers.Add(manager);
            if (m_EnabledManagers.Count > modalityManagersPeakCount)
                modalityManagersPeakCount = m_EnabledManagers.Count;

            if (!m_AllManagers.Contains(manager))
                m_AllManagers.Add(manager);

            manager.leftInputModeChanged += OnLeftInputModeChanged;
            manager.rightInputModeChanged += OnRightInputModeChanged;

            m_LeftModeTracker.StartSession(manager, now);
            m_RightModeTracker.StartSession(manager, now);

            if (m_EnabledManagers.Count == 1)
                m_StartTimestamp = now;
        }

        /// <summary>
        /// End tracking the manager component, invoked when its <c>OnDisable</c> is called.
        /// </summary>
        /// <param name="manager">The manager component to track.</param>
        /// <param name="now">Current timestamp.</param>
        public void EndSession(XRInputModalityManager manager, double now)
        {
            if (m_EnabledManagers.Remove(manager))
            {
                if (m_EnabledManagers.Count == 0)
                    sessionDuration += now - m_StartTimestamp;

                m_LeftModeTracker.EndSession(manager, now);
                m_RightModeTracker.EndSession(manager, now);

                manager.leftInputModeChanged -= OnLeftInputModeChanged;
                manager.rightInputModeChanged -= OnRightInputModeChanged;
            }
        }

        /// <summary>
        /// Update the analytics payload struct with the data from this tracker.
        /// </summary>
        /// <param name="payload">The analytics payload to write into.</param>
        public void UpdateEventPayload(ref XRIPlayModeEvent.Payload payload)
        {
            payload.modalityManagersPeakCount = modalityManagersPeakCount;
            payload.modalityManagersObjectCount = modalityManagersObjectCount;
            payload.modalityManagerDurationSeconds = (float)sessionDuration;

            payload.leftModalityInfo = new ModalityRuntimeData
            {
                handAssigned = m_LeftModeTracker.trackedHandAssigned,
                controllerAssigned = m_LeftModeTracker.motionControllerAssigned,
                noneDurationSeconds = (float)m_LeftModeTracker.durationNone,
                trackedHandDurationSeconds = (float)m_LeftModeTracker.durationTrackedHand,
                motionControllerDurationSeconds = (float)m_LeftModeTracker.durationMotionController,
            };

            payload.rightModalityInfo = new ModalityRuntimeData
            {
                handAssigned = m_RightModeTracker.trackedHandAssigned,
                controllerAssigned = m_RightModeTracker.motionControllerAssigned,
                noneDurationSeconds = (float)m_RightModeTracker.durationNone,
                trackedHandDurationSeconds = (float)m_RightModeTracker.durationTrackedHand,
                motionControllerDurationSeconds = (float)m_RightModeTracker.durationMotionController,
            };
        }

        void OnLeftInputModeChanged(XRInputModalityManager manager, XRInputModalityManager.InputMode inputMode)
        {
            m_LeftModeTracker.UpdateMode(manager, Time.realtimeSinceStartupAsDouble);
        }

        void OnRightInputModeChanged(XRInputModalityManager manager, XRInputModalityManager.InputMode inputMode)
        {
            m_RightModeTracker.UpdateMode(manager, Time.realtimeSinceStartupAsDouble);
        }
    }
}

#endif
