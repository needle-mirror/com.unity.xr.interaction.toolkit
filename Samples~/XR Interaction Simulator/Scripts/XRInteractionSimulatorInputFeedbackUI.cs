using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.InteractionSimulator
{
    class XRInteractionSimulatorInputFeedbackUI : MonoBehaviour
    {
        [Header("Input Device Modes")]

        [SerializeField]
        GameObject m_HMDPanel;

        [SerializeField]
        GameObject m_RightHandPanel;

        [SerializeField]
        GameObject m_LeftHandPanel;

        [SerializeField]
        GameObject m_RightControllerPanel;

        [SerializeField]
        GameObject m_LeftControllerPanel;

        [SerializeField]
        GameObject m_RightOtherHandPanel;

        [SerializeField]
        GameObject m_LeftOtherHandPanel;

        [SerializeField]
        GameObject m_RightOtherControllerPanel;

        [SerializeField]
        GameObject m_LeftOtherControllerPanel;

        [SerializeField]
        GameObject m_BothControllersPanel;

        [SerializeField]
        GameObject m_BothHandsPanel;

        [Header("Controller Input Modes")]

        [SerializeField]
        GameObject m_TriggerPanel;
        [SerializeField]
        Image m_TriggerBg;

        [SerializeField]
        GameObject m_GripPanel;
        [SerializeField]
        Image m_GripBg;

        [SerializeField]
        GameObject m_PrimaryPanel;
        [SerializeField]
        Image m_PrimaryBg;

        [SerializeField]
        GameObject m_SecondaryPanel;
        [SerializeField]
        Image m_SecondaryBg;

        [SerializeField]
        GameObject m_MenuPanel;
        [SerializeField]
        Image m_MenuBg;

        [SerializeField]
        GameObject m_Primary2DAxisClickPanel;
        [SerializeField]
        Image m_Primary2DAxisClickBg;

        [SerializeField]
        GameObject m_Secondary2DAxisClickPanel;
        [SerializeField]
        Image m_Secondary2DAxisClickBg;

        [SerializeField]
        GameObject m_Primary2DAxisTouchPanel;
        [SerializeField]
        Image m_Primary2DAxisTouchBg;

        [SerializeField]
        GameObject m_Secondary2DAxisTouchPanel;
        [SerializeField]
        Image m_Secondary2DAxisTouchBg;

        [SerializeField]
        GameObject m_PrimaryTouchPanel;
        [SerializeField]
        Image m_PrimaryTouchBg;

        [SerializeField]
        GameObject m_SecondaryTouchPanel;
        [SerializeField]
        Image m_SecondaryTouchBg;

        [SerializeField]
        GameObject m_ControllerHotkeyPanel;
        [SerializeField]
        Image m_ControllerHotkeyBg;
        [SerializeField]
        Image m_ControllerHotkeyIcon;
        [SerializeField]
        Text m_ControllerHotkeyText;
        [SerializeField]
        Sprite m_LeftControllerSprite;
        [SerializeField]
        Sprite m_RightControllerSprite;

        [SerializeField]
        GameObject m_ControllerInputRow;

        [Header("Other Controller Input Modes")]

        [SerializeField]
        GameObject m_OtherTriggerPanel;
        [SerializeField]
        Image m_OtherTriggerBg;

        [SerializeField]
        GameObject m_OtherGripPanel;
        [SerializeField]
        Image m_OtherGripBg;

        [SerializeField]
        GameObject m_OtherPrimaryPanel;
        [SerializeField]
        Image m_OtherPrimaryBg;

        [SerializeField]
        GameObject m_OtherSecondaryPanel;
        [SerializeField]
        Image m_OtherSecondaryBg;

        [SerializeField]
        GameObject m_OtherMenuPanel;
        [SerializeField]
        Image m_OtherMenuBg;

        [SerializeField]
        GameObject m_OtherPrimary2DAxisClickPanel;
        [SerializeField]
        Image m_OtherPrimary2DAxisClickBg;

        [SerializeField]
        GameObject m_OtherSecondary2DAxisClickPanel;
        [SerializeField]
        Image m_OtherSecondary2DAxisClickBg;

        [SerializeField]
        GameObject m_OtherPrimary2DAxisTouchPanel;
        [SerializeField]
        Image m_OtherPrimary2DAxisTouchBg;

        [SerializeField]
        GameObject m_OtherSecondary2DAxisTouchPanel;
        [SerializeField]
        Image m_OtherSecondary2DAxisTouchBg;

        [SerializeField]
        GameObject m_OtherPrimaryTouchPanel;
        [SerializeField]
        Image m_OtherPrimaryTouchBg;

        [SerializeField]
        GameObject m_OtherSecondaryTouchPanel;
        [SerializeField]
        Image m_OtherSecondaryTouchBg;

        [Header("Hand Input Modes")]

        [SerializeField]
        GameObject m_PokePanel;
        [SerializeField]
        Image m_PokePanelBg;

        [SerializeField]
        GameObject m_PinchPanel;
        [SerializeField]
        Image m_PinchPanelBg;

        [SerializeField]
        GameObject m_GrabPanel;
        [SerializeField]
        Image m_GrabPanelBg;

        [SerializeField]
        GameObject m_CustomPanel;

        [SerializeField]
        GameObject m_HandHotkeyPanel;
        [SerializeField]
        Image m_HandHotkeyBg;
        [SerializeField]
        Image m_HandHotkeyIcon;
        [SerializeField]
        Text m_HandHotkeyText;
        [SerializeField]
        Sprite m_LeftHandSprite;
        [SerializeField]
        Sprite m_RightHandSprite;

        [SerializeField]
        GameObject m_HandInputRow;

        [Header("Other Hand Input Modes")]

        [SerializeField]
        GameObject m_OtherIdlePanel;
        [SerializeField]
        Image m_OtherIdlePanelBg;

        [SerializeField]
        GameObject m_OtherPokePanel;
        [SerializeField]
        Image m_OtherPokePanelBg;

        [SerializeField]
        GameObject m_OtherPinchPanel;
        [SerializeField]
        Image m_OtherPinchPanelBg;

        [SerializeField]
        GameObject m_OtherGrabPanel;
        [SerializeField]
        Image m_OtherGrabPanelBg;

        [SerializeField]
        GameObject m_OtherCustomPanel;

        [Header("General Input")]

        [SerializeField]
        GameObject m_TranslateForwardPanel;

        [SerializeField]
        GameObject m_TranslateBackwardPanel;

        [SerializeField]
        GameObject m_TranslateUpPanel;

        [SerializeField]
        GameObject m_TranslateDownPanel;

        [SerializeField]
        GameObject m_TranslateLeftPanel;

        [SerializeField]
        GameObject m_TranslateRightPanel;

        [SerializeField]
        GameObject m_RotateUpPanel;

        [SerializeField]
        GameObject m_RotateDownPanel;

        [SerializeField]
        GameObject m_RotateLeftPanel;

        [SerializeField]
        GameObject m_RotateRightPanel;

        [SerializeField]
        GameObject m_OtherDeviceInputRow;

        XRInteractionSimulator m_Simulator;

        Dictionary<ControllerInputMode, GameObject> m_ControllerInputPanels;
        Dictionary<ControllerInputMode, Image> m_ControllerInputBgs;
        Dictionary<ControllerInputMode, GameObject> m_OtherControllerInputPanels;
        Dictionary<ControllerInputMode, Image> m_OtherControllerInputBgs;
        Dictionary<string, GameObject> m_HandExpressionPanels;
        Dictionary<string, Image> m_HandExpressionBgs;
        Dictionary<string, GameObject> m_OtherHandExpressionPanels;
        Dictionary<string, Image> m_OtherHandExpressionBgs;
        Dictionary<HeldHotkeyButtons, XRInputButtonReader> m_HotkeyInputReaders;
        List<GameObject> m_ActiveHotkeyPanelInstances = new List<GameObject>();

        SimulatedHandPlaybackManager m_HandPlaybackManager;

        // ReSharper disable InconsistentNaming -- Treat as constants
        static readonly Color k_DefaultPanelColor = new Color(0x55 / 255f, 0x55 / 255f, 0x55 / 255f);
        static readonly Color k_SelectedColor = new Color(0x4F / 255f, 0x65 / 255f, 0x7F / 255f);
        static readonly Color k_EnabledColor = new Color(0x88 / 255f, 0x88 / 255f, 0x88 / 255f);
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Start()
        {
            if (!ComponentLocatorUtility<XRInteractionSimulator>.TryFindComponent(out m_Simulator))
            {
                Debug.LogError("Could not find the XRInteractionSimulator component, disabling simulator UI.", this);
                gameObject.SetActive(false);
                return;
            }

            if (!ComponentLocatorUtility<SimulatedHandPlaybackManager>.TryFindComponent(out m_HandPlaybackManager))
            {
                Debug.LogError("Could not find SimulatedHandPlaybackManager component in the scene, disabling simulator UI.", this);
                gameObject.SetActive(false);
                return;
            }

            InitializeUIDictionaries();
            ActivateControllerPanels();
            ActivateHandPanels();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Update()
        {
            HandleActiveDeviceModePanels();
            HandleOtherDeviceModePanels();
            HandleGeneralInputFeedback();
            HandleActiveInputModePanels();
            HandleOtherActiveInputModePanels();

            if (m_Simulator.currentState.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Controller)
                HandleDeviceHotkeyPanels();
            else if (m_Simulator.currentState.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Hand)
                HandleHandHotkeyPanels();
        }

        void HandleGeneralInputFeedback()
        {
            var state = m_Simulator.currentState;

            m_TranslateRightPanel.SetActive(state.isTranslatingRight);
            m_TranslateLeftPanel.SetActive(state.isTranslatingLeft);
            m_TranslateForwardPanel.SetActive(state.isTranslatingForward);
            m_TranslateBackwardPanel.SetActive(state.isTranslatingBackward);
            m_TranslateUpPanel.SetActive(state.isTranslatingUp);
            m_TranslateDownPanel.SetActive(state.isTranslatingDown);
            m_RotateRightPanel.SetActive(state.isRotatingRight);
            m_RotateLeftPanel.SetActive(state.isRotatingLeft);
            m_RotateUpPanel.SetActive(state.isRotatingUp);
            m_RotateDownPanel.SetActive(state.isRotatingDown);
        }

        void HandleActiveDeviceModePanels()
        {
            var current = m_Simulator.currentState;
            var previous = m_Simulator.previousState;

            if (current.deviceMode == previous.deviceMode &&
                current.targetedDeviceInput == previous.targetedDeviceInput)
                return;

            ClearActiveInputModePanels();

            if (current.manipulatingFPS || current.manipulatingHMD)
                m_HMDPanel.SetActive(true);
            else if (current.manipulatingLeftController && current.manipulatingRightController)
                m_BothControllersPanel.SetActive(true);
            else if (current.manipulatingLeftController)
                m_LeftControllerPanel.SetActive(true);
            else if (current.manipulatingRightController)
                m_RightControllerPanel.SetActive(true);
            else if (current.manipulatingLeftHand && current.manipulatingRightHand)
                m_BothHandsPanel.SetActive(true);
            else if (current.manipulatingLeftHand)
                m_LeftHandPanel.SetActive(true);
            else if (current.manipulatingRightHand)
                m_RightHandPanel.SetActive(true);
        }

        void HandleOtherDeviceModePanels()
        {
            var current = m_Simulator.currentState;
            var previous = m_Simulator.previousState;

            if (current.deviceMode == previous.deviceMode &&
                current.targetedDeviceInput == previous.targetedDeviceInput)
                return;

            ClearOtherInputModePanels();

            if (current.manipulatingLeftController && !current.manipulatingRightController)
                m_RightOtherControllerPanel.SetActive(true);
            else if (current.manipulatingRightController && !current.manipulatingLeftController)
                m_LeftOtherControllerPanel.SetActive(true);
            else if (current.manipulatingLeftHand && !current.manipulatingRightHand)
                m_RightOtherHandPanel.SetActive(true);
            else if (current.manipulatingRightHand && !current.manipulatingLeftHand)
                m_LeftOtherHandPanel.SetActive(true);
        }

        void HandleActiveInputModePanels()
        {
            var current = m_Simulator.currentState;
            var previous = m_Simulator.previousState;

            bool deviceModeChanged = current.deviceMode != previous.deviceMode;
            bool targetedDeviceChanged = current.targetedDeviceInput != previous.targetedDeviceInput;
            bool controllerInputModeChanged = current.currentControllerInputMode != previous.currentControllerInputMode;
            bool handExpressionChanged = current.currentHandExpression != previous.currentHandExpression;
            bool quickActionChanged = current.performingLeftQuickAction != previous.performingLeftQuickAction ||
                current.performingRightQuickAction != previous.performingRightQuickAction;

            if (current.manipulatingFPS || current.manipulatingHMD)
            {
                m_ControllerInputRow.SetActive(false);
                m_HandInputRow.SetActive(false);
                m_OtherDeviceInputRow.SetActive(false);
                return;
            }

            bool controllerModeActive = current.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Controller;
            bool handModeActive = current.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Hand;
            bool updateRows = deviceModeChanged || m_ControllerInputRow.activeSelf != controllerModeActive ||
                m_HandInputRow.activeSelf != handModeActive;

            if (updateRows)
            {
                m_ControllerInputRow.SetActive(controllerModeActive);
                m_HandInputRow.SetActive(handModeActive);
                m_OtherDeviceInputRow.SetActive(true);
            }

            if ((current.manipulatingLeftDevice && !current.performingLeftQuickAction) ||
                (current.manipulatingRightDevice && !current.performingRightQuickAction))
            {
                if (current.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Controller && controllerInputModeChanged)
                    HighlightActiveControllerInputMode(k_SelectedColor, current);
                else if (current.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Hand && handExpressionChanged)
                    HighlightActiveHandInputMode(k_SelectedColor, current);
            }

            bool updateHighlight = quickActionChanged ||
                (targetedDeviceChanged &&
                    (current.targetedDeviceInput == TargetedDevices.LeftDevice ||
                        current.targetedDeviceInput == TargetedDevices.RightDevice));

            if (updateHighlight)
                UpdateActiveInputModeHighlight(current);
        }

        void UpdateActiveInputModeHighlight(XRInteractionSimulatorState current)
        {
            bool isPerformingActive = (current.manipulatingLeftDevice && current.performingLeftQuickAction)
                || (current.manipulatingRightDevice && current.performingRightQuickAction);

            if (current.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Controller)
            {
                HighlightActiveControllerInputMode(isPerformingActive ? k_EnabledColor : k_SelectedColor, current);
            }
            else if (current.deviceMode == SimulatedDeviceLifecycleManager.DeviceMode.Hand)
            {
                if (isPerformingActive && current.currentHandExpression.sequenceType == SimulatedHandExpression.SequenceType.MultiFrame)
                    HighlightActiveHandInputMode(k_EnabledColor, current);
                else
                    HighlightActiveHandInputMode(k_SelectedColor, current);
            }
        }

        void HandleOtherActiveInputModePanels()
        {
            var current = m_Simulator.currentState;
            var previous = m_Simulator.previousState;
            var stateChanged =
                current.deviceMode != previous.deviceMode ||
                current.targetedDeviceInput != previous.targetedDeviceInput ||
                current.leftControllerInputMode != previous.leftControllerInputMode ||
                current.rightControllerInputMode != previous.rightControllerInputMode ||
                current.leftHandExpression != previous.leftHandExpression ||
                current.rightHandExpression != previous.rightHandExpression ||
                current.performingLeftQuickAction != previous.performingLeftQuickAction ||
                current.performingRightQuickAction != previous.performingRightQuickAction;

            if (!stateChanged)
                return;

            ClearOtherActiveInputPanels();

            if (current.manipulatingLeftController && !current.manipulatingRightController)
            {
                m_OtherControllerInputPanels[current.rightControllerInputMode].SetActive(true);
                HighlightOtherControllerInputMode(current.rightControllerInputMode,
                    current.performingRightQuickAction ? k_EnabledColor : k_DefaultPanelColor);
            }
            else if (current.manipulatingRightController && !current.manipulatingLeftController)
            {
                m_OtherControllerInputPanels[current.leftControllerInputMode].SetActive(true);
                HighlightOtherControllerInputMode(current.leftControllerInputMode,
                    current.performingLeftQuickAction ? k_EnabledColor : k_DefaultPanelColor);
            }
            else if (current.manipulatingLeftHand && !current.manipulatingRightHand)
            {
                m_OtherHandExpressionPanels[current.rightHandExpression.name].SetActive(true);
                HighlightOtherHandInputMode(current.rightHandExpression.name, current.performingRightQuickAction &&
                    current.rightHandExpression.sequenceType == SimulatedHandExpression.SequenceType.MultiFrame ? k_EnabledColor : k_DefaultPanelColor);
            }
            else if (current.manipulatingRightHand && !current.manipulatingLeftHand)
            {
                m_OtherHandExpressionPanels[current.leftHandExpression.name].SetActive(true);
                HighlightOtherHandInputMode(current.leftHandExpression.name, current.performingLeftQuickAction &&
                    current.leftHandExpression.sequenceType == SimulatedHandExpression.SequenceType.MultiFrame ? k_EnabledColor : k_DefaultPanelColor);
            }
        }

        void HandleDeviceHotkeyPanels()
        {
            var current = m_Simulator.currentState;
            var previous = m_Simulator.previousState;

            if (current.activeControllerHotkeyButtons == previous.activeControllerHotkeyButtons)
                return;

            foreach (var instance in m_ActiveHotkeyPanelInstances)
            {
                Destroy(instance);
            }

            m_ActiveHotkeyPanelInstances.Clear();

            foreach (var (flag, reader) in m_HotkeyInputReaders)
            {
                if ((current.activeControllerHotkeyButtons & flag) == 0)
                    continue;

                ApplyControllerHotkeyText(reader, current);
                var panel = Instantiate(m_ControllerHotkeyPanel, m_ControllerHotkeyPanel.transform.parent);
                panel.SetActive(true);
                m_ActiveHotkeyPanelInstances.Add(panel);
            }
        }

        void HandleHandHotkeyPanels()
        {
            var current = m_Simulator.currentState;
            var previous = m_Simulator.previousState;

            if (current.handExpressionToggleHeld == previous.handExpressionToggleHeld)
                return;

            if (current.handExpressionToggleHeld)
            {
                ApplyHandHotkeyText(current);
                m_HandHotkeyPanel.SetActive(true);
            }
            else
            {
                m_HandHotkeyPanel.SetActive(false);
            }
        }

        void ApplyHandHotkeyText(XRInteractionSimulatorState current)
        {
            string bindingText = current.leftDeviceHotkeyModifierPressed
                ? current.leftHandExpression.toggleInput.inputActionReferencePerformed.action.GetBindingDisplayString(0)
                : current.rightHandExpression.toggleInput.inputActionReferencePerformed.action.GetBindingDisplayString(0);
            m_HandHotkeyIcon.sprite = current.leftDeviceHotkeyModifierPressed ? m_LeftHandSprite : m_RightHandSprite;
            m_HandHotkeyText.text = bindingText;
        }

        void ApplyControllerHotkeyText(XRInputButtonReader inputReader, XRInteractionSimulatorState current)
        {
            string bindingText = inputReader.inputActionReferencePerformed.action.GetBindingDisplayString(0);
            m_ControllerHotkeyIcon.sprite = current.leftDeviceHotkeyModifierPressed ? m_LeftControllerSprite : m_RightControllerSprite;
            m_ControllerHotkeyText.text = bindingText;
        }

        void ActivateControllerPanels()
        {
            for (var i = 0; i < m_Simulator.quickActionControllerInputModes.Count; i++)
            {
                if (!m_ControllerInputPanels.TryGetValue(m_Simulator.quickActionControllerInputModes[i], out var panel))
                {
                    string inputModeName = m_Simulator.quickActionControllerInputModes[i].ToString();
                    Debug.LogError($"Panel for the {inputModeName} controller input mode does not exist.", this);
                    continue;
                }

                panel.SetActive(true);
                panel.transform.SetSiblingIndex(i);
            }
        }

        void ActivateHandPanels()
        {
            for (var i = 0; i < m_HandPlaybackManager.simulatedHandExpressions.Count; i++)
            {
                if (m_HandPlaybackManager.simulatedHandExpressions[i].isQuickAction)
                {
                    string handExpressionName = m_HandPlaybackManager.simulatedHandExpressions[i].name;

                    if (!m_HandExpressionPanels.TryGetValue(handExpressionName, out var panel))
                    {
                        Debug.LogError($"Panel for the {handExpressionName} hand expression does not exist.", this);
                        continue;
                    }

                    panel.SetActive(true);
                    panel.transform.SetSiblingIndex(i);
                }
            }
        }

        void InitializeUIDictionaries()
        {
            m_ControllerInputPanels = new Dictionary<ControllerInputMode, GameObject>
            {
                {ControllerInputMode.Trigger, m_TriggerPanel},
                {ControllerInputMode.Grip, m_GripPanel},
                {ControllerInputMode.PrimaryButton, m_PrimaryPanel},
                {ControllerInputMode.SecondaryButton, m_SecondaryPanel},
                {ControllerInputMode.Menu, m_MenuPanel},
                {ControllerInputMode.Primary2DAxisClick, m_Primary2DAxisClickPanel},
                {ControllerInputMode.Secondary2DAxisClick, m_Secondary2DAxisClickPanel},
                {ControllerInputMode.Primary2DAxisTouch, m_Primary2DAxisTouchPanel},
                {ControllerInputMode.Secondary2DAxisTouch, m_Secondary2DAxisTouchPanel},
                {ControllerInputMode.PrimaryTouch, m_PrimaryTouchPanel},
                {ControllerInputMode.SecondaryTouch, m_SecondaryTouchPanel},
            };

            m_OtherControllerInputPanels = new Dictionary<ControllerInputMode, GameObject>
            {
                {ControllerInputMode.Trigger, m_OtherTriggerPanel},
                {ControllerInputMode.Grip, m_OtherGripPanel},
                {ControllerInputMode.PrimaryButton, m_OtherPrimaryPanel},
                {ControllerInputMode.SecondaryButton, m_OtherSecondaryPanel},
                {ControllerInputMode.Menu, m_OtherMenuPanel},
                {ControllerInputMode.Primary2DAxisClick, m_OtherPrimary2DAxisClickPanel},
                {ControllerInputMode.Secondary2DAxisClick, m_OtherSecondary2DAxisClickPanel},
                {ControllerInputMode.Primary2DAxisTouch, m_OtherPrimary2DAxisTouchPanel},
                {ControllerInputMode.Secondary2DAxisTouch, m_OtherSecondary2DAxisTouchPanel},
                {ControllerInputMode.PrimaryTouch, m_OtherPrimaryTouchPanel},
                {ControllerInputMode.SecondaryTouch, m_OtherSecondaryTouchPanel},
            };

            m_ControllerInputBgs = new Dictionary<ControllerInputMode, Image>
            {
                {ControllerInputMode.Trigger, m_TriggerBg},
                {ControllerInputMode.Grip, m_GripBg},
                {ControllerInputMode.PrimaryButton, m_PrimaryBg},
                {ControllerInputMode.SecondaryButton, m_SecondaryBg},
                {ControllerInputMode.Menu, m_MenuBg},
                {ControllerInputMode.Primary2DAxisClick, m_Primary2DAxisClickBg},
                {ControllerInputMode.Secondary2DAxisClick, m_Secondary2DAxisClickBg},
                {ControllerInputMode.Primary2DAxisTouch, m_Primary2DAxisTouchBg},
                {ControllerInputMode.Secondary2DAxisTouch, m_Secondary2DAxisTouchBg},
                {ControllerInputMode.PrimaryTouch, m_PrimaryTouchBg},
                {ControllerInputMode.SecondaryTouch, m_SecondaryTouchBg},
            };

            m_OtherControllerInputBgs = new Dictionary<ControllerInputMode, Image>
            {
                {ControllerInputMode.Trigger, m_OtherTriggerBg},
                {ControllerInputMode.Grip, m_OtherGripBg},
                {ControllerInputMode.PrimaryButton, m_OtherPrimaryBg},
                {ControllerInputMode.SecondaryButton, m_OtherSecondaryBg},
                {ControllerInputMode.Menu, m_OtherMenuBg},
                {ControllerInputMode.Primary2DAxisClick, m_OtherPrimary2DAxisClickBg},
                {ControllerInputMode.Secondary2DAxisClick, m_OtherSecondary2DAxisClickBg},
                {ControllerInputMode.Primary2DAxisTouch, m_OtherPrimary2DAxisTouchBg},
                {ControllerInputMode.Secondary2DAxisTouch, m_OtherSecondary2DAxisTouchBg},
                {ControllerInputMode.PrimaryTouch, m_OtherPrimaryTouchBg},
                {ControllerInputMode.SecondaryTouch, m_OtherSecondaryTouchBg},
            };

            m_HotkeyInputReaders = new Dictionary<HeldHotkeyButtons, XRInputButtonReader>
            {
                {HeldHotkeyButtons.Trigger, m_Simulator.triggerInput},
                {HeldHotkeyButtons.Grip, m_Simulator.gripInput},
                {HeldHotkeyButtons.PrimaryButton, m_Simulator.primaryButtonInput},
                {HeldHotkeyButtons.SecondaryButton, m_Simulator.secondaryButtonInput},
                {HeldHotkeyButtons.Menu, m_Simulator.menuInput},
                {HeldHotkeyButtons.Primary2DAxisClick, m_Simulator.primary2DAxisClickInput},
                {HeldHotkeyButtons.Secondary2DAxisClick, m_Simulator.secondary2DAxisClickInput},
                {HeldHotkeyButtons.Primary2DAxisTouch, m_Simulator.primary2DAxisTouchInput},
                {HeldHotkeyButtons.Secondary2DAxisTouch, m_Simulator.secondary2DAxisTouchInput},
                {HeldHotkeyButtons.PrimaryTouch, m_Simulator.primaryTouchInput},
                {HeldHotkeyButtons.SecondaryTouch, m_Simulator.secondaryTouchInput},
            };

            m_HandExpressionPanels = new Dictionary<string, GameObject>
            {
                {"Poke", m_PokePanel},
                {"Pinch", m_PinchPanel},
                {"Grab", m_GrabPanel},
            };

            m_OtherHandExpressionPanels = new Dictionary<string, GameObject>
            {
                {"Idle", m_OtherIdlePanel},
                {"Poke", m_OtherPokePanel},
                {"Pinch", m_OtherPinchPanel},
                {"Grab", m_OtherGrabPanel},
            };

            m_HandExpressionBgs = new Dictionary<string, Image>
            {
                {"Poke", m_PokePanelBg},
                {"Pinch", m_PinchPanelBg},
                {"Grab", m_GrabPanelBg},
            };

            m_OtherHandExpressionBgs = new Dictionary<string, Image>
            {
                {"Poke", m_OtherPokePanelBg},
                {"Pinch", m_OtherPinchPanelBg},
                {"Grab", m_OtherGrabPanelBg},
            };

            InitializeCustomHandExpressionPanels();
        }

        void InitializeCustomHandExpressionPanels()
        {
            foreach (var handExpression in m_HandPlaybackManager.simulatedHandExpressions)
            {
                InstantiateHandPanelObjects(handExpression, m_CustomPanel, m_HandExpressionPanels, m_HandExpressionBgs);
                InstantiateHandPanelObjects(handExpression, m_OtherCustomPanel, m_OtherHandExpressionPanels, m_OtherHandExpressionBgs);
            }
        }

        static void InstantiateHandPanelObjects(SimulatedHandExpression handExpression, GameObject customPanel, Dictionary<string, GameObject> panels, Dictionary<string, Image> bgs)
        {
            if (!panels.ContainsKey(handExpression.name) && handExpression.isQuickAction)
            {
                var panel = Instantiate(customPanel, customPanel.transform.parent);
                panel.name = $"{handExpression.name}Panel";

                var bgImage = panel.GetComponentInChildren<Image>();
                if (bgImage == null)
                {
                    var bgImageGO = Instantiate(new GameObject(), panel.transform);
                    bgImageGO.name = "Bg";
                    bgImage = bgImageGO.AddComponent<Image>();
                }

                var textUI = panel.GetComponentInChildren<Text>();
                if (textUI == null)
                {
                    var textGO = Instantiate(new GameObject(), panel.transform);
                    textGO.name = "Text";
                    textUI = textGO.AddComponent<Text>();
                    textUI.fontStyle = FontStyle.Bold;
                }
                textUI.text = handExpression.name;

                panels[handExpression.name] = panel;
                bgs[handExpression.name] = bgImage;
            }
        }

        void HighlightActiveControllerInputMode(Color highlightColor, XRInteractionSimulatorState current)
        {
            ClearHighlightedControllerPanels(m_ControllerInputBgs);

            if (!m_ControllerInputBgs.TryGetValue(current.currentControllerInputMode, out var bg))
            {
                string inputModeName = current.currentControllerInputMode.ToString();
                Debug.LogError($"Background for the {inputModeName} controller input mode panel does not exist.", this);
                return;
            }

            bg.color = highlightColor;
        }

        void HighlightOtherControllerInputMode(ControllerInputMode inputMode, Color highlightColor)
        {
            ClearHighlightedControllerPanels(m_OtherControllerInputBgs);

            if (!m_OtherControllerInputBgs.TryGetValue(inputMode, out var bg))
            {
                string inputModeName = inputMode.ToString();
                Debug.LogError($"Background for the {inputModeName} controller input mode panel does not exist.", this);
                return;
            }

            bg.color = highlightColor;
        }

        void HighlightActiveHandInputMode(Color highlightColor, XRInteractionSimulatorState current)
        {
            ClearHighlightedHandPanels(m_HandExpressionBgs);

            if (current.currentHandExpression.name == m_HandPlaybackManager.restingHandExpression.name)
                return;

            var handExpressionName = current.currentHandExpression.name;
            if (string.IsNullOrEmpty(handExpressionName))
                return;

            if (!m_HandExpressionBgs.TryGetValue(handExpressionName, out var bg))
            {
                Debug.LogError($"Background for the {handExpressionName} hand expression panel does not exist.", this);
                return;
            }

            bg.color = highlightColor;
        }

        void HighlightOtherHandInputMode(string handExpressionName, Color highlightColor)
        {
            ClearHighlightedHandPanels(m_OtherHandExpressionBgs);

            if (string.IsNullOrEmpty(handExpressionName))
                return;

            if (handExpressionName == m_HandPlaybackManager.restingHandExpression.name)
                return;

            if (!m_OtherHandExpressionBgs.TryGetValue(handExpressionName, out var bg))
            {
                Debug.LogError($"Background for the {handExpressionName} hand expression panel does not exist.", this);
                return;
            }

            bg.color = highlightColor;
        }

        void ClearActiveInputModePanels()
        {
            m_BothControllersPanel.SetActive(false);
            m_LeftControllerPanel.SetActive(false);
            m_RightControllerPanel.SetActive(false);
            m_BothHandsPanel.SetActive(false);
            m_LeftHandPanel.SetActive(false);
            m_RightHandPanel.SetActive(false);
            m_HMDPanel.SetActive(false);
        }

        void ClearOtherInputModePanels()
        {
            m_LeftOtherControllerPanel.SetActive(false);
            m_RightOtherControllerPanel.SetActive(false);
            m_LeftOtherHandPanel.SetActive(false);
            m_RightOtherHandPanel.SetActive(false);
        }

        void ClearOtherActiveInputPanels()
        {
            m_OtherTriggerPanel.SetActive(false);
            m_OtherGripPanel.SetActive(false);
            m_OtherPrimaryPanel.SetActive(false);
            m_OtherSecondaryPanel.SetActive(false);
            m_OtherMenuPanel.SetActive(false);
            m_OtherPrimary2DAxisClickPanel.SetActive(false);
            m_OtherSecondary2DAxisClickPanel.SetActive(false);
            m_OtherPrimary2DAxisTouchPanel.SetActive(false);
            m_OtherSecondary2DAxisTouchPanel.SetActive(false);
            m_OtherPrimaryTouchPanel.SetActive(false);
            m_OtherSecondaryTouchPanel.SetActive(false);

            foreach (var panel in m_OtherHandExpressionPanels.Values)
            {
                panel.SetActive(false);
            }
        }

        static void ClearHighlightedHandPanels(Dictionary<string, Image> bgs)
        {
            foreach (var bg in bgs.Values)
            {
                bg.color = k_DefaultPanelColor;
            }
        }

        static void ClearHighlightedControllerPanels(Dictionary<ControllerInputMode, Image> bgs)
        {
            foreach (var bg in bgs.Values)
            {
                bg.color = k_DefaultPanelColor;
            }
        }
    }
}
