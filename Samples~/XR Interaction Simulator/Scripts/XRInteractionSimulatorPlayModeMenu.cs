using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
#if XR_HANDS_1_1_OR_NEWER
using UnityEngine.XR.Hands;
#endif
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.InteractionSimulator
{
    class XRInteractionSimulatorPlayModeMenu : MonoBehaviour
    {
        [Header("Menus")]

        [SerializeField]
        GameObject m_InputSelectionMenu;

        [SerializeField]
        GameObject m_ClosedInputSelectionMenu;

        [SerializeField]
        GameObject m_ControllerActionsMenu;

        [SerializeField]
        GameObject m_ClosedControllerActionsMenu;

        [SerializeField]
        GameObject m_HandActionsMenu;

        [SerializeField]
        GameObject m_ClosedHandActionsMenu;

        [Header("Input Readers")]

        [SerializeField]
        XRInputButtonReader m_ToggleActionMenu;

        [SerializeField]
        XRInputButtonReader m_ToggleInputSelectionMenu;

        [Header("Device Highlight Panels")]

        [SerializeField]
        GameObject m_HighlightFullBodyPanel;

        [SerializeField]
        GameObject m_HighlightLeftControllerPanel;

        [SerializeField]
        GameObject m_HighlightRightControllerPanel;

        [SerializeField]
        GameObject m_HighlightLeftHandPanel;

        [SerializeField]
        GameObject m_HighlightRightHandPanel;

        [SerializeField]
        GameObject m_HighlightHeadPanel;

        [Header("Controller Action Panels")]

        [SerializeField]
        GameObject m_ControllerActionHighlightPanel;

        [SerializeField]
        Text m_FirstControllerActionText;

        [SerializeField]
        Text m_SecondControllerActionText;

        [SerializeField]
        Text m_ThirdControllerActionText;

        [SerializeField]
        Text m_FourthControllerActionText;

        [SerializeField]
        Text m_FirstControllerBindingText;

        [SerializeField]
        Text m_SecondControllerBindingText;

        [SerializeField]
        Text m_ThirdControllerBindingText;

        [SerializeField]
        Text m_FourthControllerBindingText;

        [SerializeField]
        GameObject m_FirstControllerBindingGO;

        [SerializeField]
        GameObject m_SecondControllerBindingGO;

        [SerializeField]
        GameObject m_ThirdControllerBindingGO;

        [SerializeField]
        GameObject m_FourthControllerBindingGO;

        [Header("Hand Action Panels")]

        [SerializeField]
        GameObject m_LeftHandHighlightPanel;

        [SerializeField]
        GameObject m_RightHandHighlightPanel;

        [SerializeField]
        GameObject m_LeftHandActionHighlightPanel;

        [SerializeField]
        GameObject m_RightHandActionHighlightPanel;

        [SerializeField]
        Text m_FirstHandActionText;

        [SerializeField]
        Text m_SecondHandActionText;

        [SerializeField]
        Text m_ThirdHandActionText;

        [SerializeField]
        Text m_FirstHandBindingText;

        [SerializeField]
        Text m_SecondHandBindingText;

        [SerializeField]
        Text m_ThirdHandBindingText;

        [SerializeField]
        Text m_LeftFirstHandActionText;

        [SerializeField]
        Text m_LeftSecondHandActionText;

        [SerializeField]
        Text m_LeftThirdHandActionText;

        [SerializeField]
        Text m_LeftFirstHandBindingText;

        [SerializeField]
        Text m_LeftSecondHandBindingText;

        [SerializeField]
        Text m_LeftThirdHandBindingText;

        [SerializeField]
        GameObject m_FirstHandBindingGO;

        [SerializeField]
        GameObject m_SecondHandBindingGO;

        [SerializeField]
        GameObject m_ThirdHandBindingGO;

        [SerializeField]
        GameObject m_LeftFirstHandBindingGO;

        [SerializeField]
        GameObject m_LeftSecondHandBindingGO;

        [SerializeField]
        GameObject m_LeftThirdHandBindingGO;

        [Header("Hand UI")]
        [SerializeField]
        Image m_LeftHandIcon;

        [SerializeField]
        Image m_RightHandIcon;

        [SerializeField]
        GameObject m_HandPackageWarningPanel;

        [SerializeField]
        GameObject m_InputModalityManagerWarningPanel;

        [SerializeField]
        GameObject m_InputMenuHandVisualizerWarningPanel;

        XRInteractionSimulator m_Simulator;
        SimulatedHandPlaybackManager m_HandPlaybackManager;

        Dictionary<ControllerInputMode, GameObject> m_ControllerInputRow = new Dictionary<ControllerInputMode, GameObject>();
        Dictionary<SimulatedHandExpression, GameObject> m_LeftHandExpressionRow = new Dictionary<SimulatedHandExpression, GameObject>();
        Dictionary<SimulatedHandExpression, GameObject> m_RightHandExpressionRow = new Dictionary<SimulatedHandExpression, GameObject>();

        bool m_PreviousControllerMenuState;
        bool m_PreviousHandMenuState;

        static readonly Color k_DisabledColor = new Color(0x70 / 255f, 0x70 / 255f, 0x70 / 255f);

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Start()
        {
            if (!ComponentLocatorUtility<XRInteractionSimulator>.TryFindComponent(out m_Simulator))
            {
                Debug.LogError($"Could not find the XRInteractionSimulator component, disabling simulator UI.", this);
                gameObject.SetActive(false);
                return;
            }

            if (!ComponentLocatorUtility<SimulatedHandPlaybackManager>.TryFindComponent(out m_HandPlaybackManager))
            {
                Debug.LogError($"Could not find SimulatedHandPlaybackManager component in the scene, disabling simulator UI.", this);
                gameObject.SetActive(false);
                return;
            }

            InitializeQuickActionPanels();

#if XR_HANDS_1_1_OR_NEWER
            CheckInputModalityManager();
#else
            m_HandPackageWarningPanel.SetActive(true);
            m_LeftHandIcon.color = k_DisabledColor;
            m_RightHandIcon.color = k_DisabledColor;
#endif

#if XR_HANDS_1_2_OR_NEWER
            if (!m_HandPackageWarningPanel.activeSelf && !m_InputModalityManagerWarningPanel.activeSelf)
                CheckHandVisualizer();
#endif
        }

        void CheckInputModalityManager()
        {
            if (ComponentLocatorUtility<XRInputModalityManager>.TryFindComponent(out var inputModalityManager) &&
                inputModalityManager.leftHand == null && inputModalityManager.rightHand == null)
            {
                m_InputModalityManagerWarningPanel.SetActive(true);
                m_LeftHandIcon.color = k_DisabledColor;
                m_RightHandIcon.color = k_DisabledColor;
            }
        }

#if XR_HANDS_1_2_OR_NEWER
        void CheckHandVisualizer()
        {
            if (ComponentLocatorUtility<XRInputModalityManager>.TryFindComponent(out var inputModalityManager))
            {
                if (inputModalityManager.leftHand == null && inputModalityManager.rightHand == null)
                    return;

                if ((inputModalityManager.leftHand != null &&
                        inputModalityManager.leftHand.GetComponentInChildren<XRHandMeshController>(true) != null) ||
                    (inputModalityManager.rightHand != null &&
                        inputModalityManager.rightHand.GetComponentInChildren<XRHandMeshController>(true) != null))
                {
                    return;
                }

                m_InputMenuHandVisualizerWarningPanel.SetActive(true);
            }
        }
#endif

        void InitializeQuickActionPanels()
        {
            InitializeControllerQuickActionPanels();
            InitializeHandQuickActionPanels();
        }

        void InitializeControllerQuickActionPanels()
        {
            Text[] actionTexts = { m_FirstControllerActionText, m_SecondControllerActionText, m_ThirdControllerActionText, m_FourthControllerActionText };
            Text[] bindingTexts = { m_FirstControllerBindingText, m_SecondControllerBindingText, m_ThirdControllerBindingText, m_FourthControllerBindingText };
            GameObject[] bindingGOs = { m_FirstControllerBindingGO, m_SecondControllerBindingGO, m_ThirdControllerBindingGO, m_FourthControllerBindingGO };

            var inputModes = m_Simulator.quickActionControllerInputModes;
            for (int i = 0; i < actionTexts.Length; i++)
            {
                if (i < inputModes.Count)
                {
                    m_ControllerInputRow[inputModes[i]] = actionTexts[i].gameObject;
                    GetControllerQuickActionNames(inputModes[i], actionTexts[i], bindingTexts[i]);
                }
                else
                {
                    actionTexts[i].gameObject.SetActive(false);
                    bindingGOs[i].SetActive(false);
                }
            }
        }

        void InitializeHandQuickActionPanels()
        {
            Text[] rightActionTexts = { m_FirstHandActionText, m_SecondHandActionText, m_ThirdHandActionText };
            Text[] rightBindingTexts = { m_FirstHandBindingText, m_SecondHandBindingText, m_ThirdHandBindingText };
            GameObject[] rightBindingGOs = { m_FirstHandBindingGO, m_SecondHandBindingGO, m_ThirdHandBindingGO };
            Text[] leftActionTexts = { m_LeftFirstHandActionText, m_LeftSecondHandActionText, m_LeftThirdHandActionText };
            Text[] leftBindingTexts = { m_LeftFirstHandBindingText, m_LeftSecondHandBindingText, m_LeftThirdHandBindingText };
            GameObject[] leftBindingGOs = { m_LeftFirstHandBindingGO, m_LeftSecondHandBindingGO, m_LeftThirdHandBindingGO };

            var quickActions = new List<SimulatedHandExpression>();
            foreach (var expression in m_HandPlaybackManager.simulatedHandExpressions)
            {
                if (expression.isQuickAction)
                    quickActions.Add(expression);
            }

            for (int i = 0; i < rightActionTexts.Length; i++)
            {
                if (i < quickActions.Count)
                {
                    var expression = quickActions[i];
                    m_RightHandExpressionRow[expression] = rightActionTexts[i].gameObject;
                    m_LeftHandExpressionRow[expression] = leftActionTexts[i].gameObject;

                    rightActionTexts[i].text = expression.name;
                    rightBindingTexts[i].text = GetBindingString(expression.toggleInput);
                    leftActionTexts[i].text = expression.name;
                    leftBindingTexts[i].text = GetBindingString(expression.toggleInput);
                }
                else
                {
                    rightActionTexts[i].gameObject.SetActive(false);
                    rightBindingGOs[i].SetActive(false);
                    leftActionTexts[i].gameObject.SetActive(false);
                    leftBindingGOs[i].SetActive(false);
                }
            }
        }

        void GetControllerQuickActionNames(ControllerInputMode inputMode, Text actionText, Text bindingText)
        {
            switch (inputMode)
            {
                case ControllerInputMode.None:
                    actionText.text = "None";
                    bindingText.text = "?";
                    break;
                case ControllerInputMode.Trigger:
                    actionText.text = "Trigger";
                    bindingText.text = GetBindingString(m_Simulator.triggerInput);
                    break;
                case ControllerInputMode.Grip:
                    actionText.text = "Grip";
                    bindingText.text = GetBindingString(m_Simulator.gripInput);
                    break;
                case ControllerInputMode.PrimaryButton:
                    actionText.text = "Primary";
                    bindingText.text = GetBindingString(m_Simulator.primaryButtonInput);
                    break;
                case ControllerInputMode.SecondaryButton:
                    actionText.text = "Secondary";
                    bindingText.text = GetBindingString(m_Simulator.secondaryButtonInput);
                    break;
                case ControllerInputMode.Menu:
                    actionText.text = "Menu";
                    bindingText.text = GetBindingString(m_Simulator.menuInput);
                    break;
                case ControllerInputMode.Primary2DAxisClick:
                    actionText.text = "Prim2DClick";
                    bindingText.text = GetBindingString(m_Simulator.primary2DAxisClickInput);
                    break;
                case ControllerInputMode.Secondary2DAxisClick:
                    actionText.text = "Sec2DClick";
                    bindingText.text = GetBindingString(m_Simulator.secondary2DAxisClickInput);
                    break;
                case ControllerInputMode.Primary2DAxisTouch:
                    actionText.text = "Prim2DTouch";
                    bindingText.text = GetBindingString(m_Simulator.primary2DAxisTouchInput);
                    break;
                case ControllerInputMode.Secondary2DAxisTouch:
                    actionText.text = "Sec2DTouch";
                    bindingText.text = GetBindingString(m_Simulator.secondary2DAxisTouchInput);
                    break;
                case ControllerInputMode.PrimaryTouch:
                    actionText.text = "PrimTouch";
                    bindingText.text = GetBindingString(m_Simulator.primaryTouchInput);
                    break;
                case ControllerInputMode.SecondaryTouch:
                    actionText.text = "SecTouch";
                    bindingText.text = GetBindingString(m_Simulator.secondaryTouchInput);
                    break;
                default:
                    Assert.IsTrue(false, $"Unhandled {nameof(inputMode)}={inputMode}.");
                    break;
            }
        }

        static string GetBindingString(XRInputButtonReader reader)
        {
            if (reader == null)
                return string.Empty;

            InputAction action;
            switch (reader.inputSourceMode)
            {
                case XRInputButtonReader.InputSourceMode.InputActionReference:
                    action = reader.inputActionReferencePerformed != null ? reader.inputActionReferencePerformed.action : null;
                    break;
                case XRInputButtonReader.InputSourceMode.InputAction:
                    action = reader.inputActionPerformed;
                    break;
                default:
                    action = null;
                    break;
            }

            return action != null ? action.GetBindingDisplayString(0) : string.Empty;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Update()
        {
            HandleHighlightedDevicePanels();
            HandleHighlightedControllerActionPanels();
            HandleHighlightedHandActionPanels();
            HandleActiveMenus();
        }

        /// <summary>
        /// Toggles the visibility of the input selection menu.
        /// </summary>
        public void OpenCloseInputSelectionMenu()
        {
            bool isOpen = m_InputSelectionMenu.activeSelf;
            m_InputSelectionMenu.SetActive(!isOpen);
            m_ClosedInputSelectionMenu.SetActive(isOpen);
        }

        /// <summary>
        /// Toggles the visibility of for the controller actions menu.
        /// </summary>
        public void OpenCloseControllerActionsMenu()
        {
            bool isOpen = m_ControllerActionsMenu.activeSelf;
            m_ControllerActionsMenu.SetActive(!isOpen);
            m_ClosedControllerActionsMenu.SetActive(isOpen);
        }

        /// <summary>
        /// Toggles the visibility of for the hand actions menu.
        /// </summary>
        public void OpenCloseHandActionsMenu()
        {
            bool isOpen = m_HandActionsMenu.activeSelf;
            m_HandActionsMenu.SetActive(!isOpen);
            m_ClosedHandActionsMenu.SetActive(isOpen);
        }

        void HandleActiveMenus()
        {
            var current = m_Simulator.currentState;
            var previous = m_Simulator.previousState;

            bool deviceModeChanged = current.deviceMode != previous.deviceMode;
            bool exitedFPS = previous.manipulatingFPS && !current.manipulatingFPS;

            if ((deviceModeChanged || exitedFPS) && !current.manipulatingFPS)
            {
                if (current.manipulatingLeftController || current.manipulatingRightController)
                {
                    m_PreviousHandMenuState = m_HandActionsMenu.activeSelf;
                    m_HandActionsMenu.SetActive(false);
                    m_ClosedHandActionsMenu.SetActive(false);

                    if (m_PreviousControllerMenuState)
                        m_ControllerActionsMenu.SetActive(true);
                    else
                        m_ClosedControllerActionsMenu.SetActive(true);
                }
                else if (current.manipulatingLeftHand || current.manipulatingRightHand)
                {
                    m_PreviousControllerMenuState = m_ControllerActionsMenu.activeSelf;
                    m_ControllerActionsMenu.SetActive(false);
                    m_ClosedControllerActionsMenu.SetActive(false);

                    if (m_PreviousHandMenuState)
                        m_HandActionsMenu.SetActive(true);
                    else
                        m_ClosedHandActionsMenu.SetActive(true);
                }

                if (current.leftHandExpression == m_HandPlaybackManager.restingHandExpression)
                    m_LeftHandActionHighlightPanel.SetActive(false);
                if (current.rightHandExpression == m_HandPlaybackManager.restingHandExpression)
                    m_RightHandActionHighlightPanel.SetActive(false);
            }

            if (current.manipulatingFPS && !previous.manipulatingFPS)
            {
                m_PreviousControllerMenuState = m_ControllerActionsMenu.activeSelf;
                m_PreviousHandMenuState = m_HandActionsMenu.activeSelf;

                m_HandActionsMenu.SetActive(false);
                m_ClosedHandActionsMenu.SetActive(false);
                m_ControllerActionsMenu.SetActive(false);
                m_ClosedControllerActionsMenu.SetActive(false);
            }

            if (m_ToggleActionMenu.ReadWasPerformedThisFrame() && !current.manipulatingFPS)
            {
                if (current.manipulatingLeftController || current.manipulatingRightController)
                    OpenCloseControllerActionsMenu();
                else if (current.manipulatingLeftHand || current.manipulatingRightHand)
                    OpenCloseHandActionsMenu();
            }

            if (m_ToggleInputSelectionMenu.ReadWasPerformedThisFrame())
                OpenCloseInputSelectionMenu();
        }

        void HandleHighlightedDevicePanels()
        {
            var current = m_Simulator.currentState;
            var previous = m_Simulator.previousState;

            if (current.targetedDeviceInput == previous.targetedDeviceInput && current.deviceMode == previous.deviceMode)
                return;

            ClearHighlightedDevicePanels();

            if (current.manipulatingFPS)
            {
                m_HighlightFullBodyPanel.SetActive(true);
                return;
            }

            if (current.manipulatingLeftController)
                m_HighlightLeftControllerPanel.SetActive(true);

            if (current.manipulatingRightController)
                m_HighlightRightControllerPanel.SetActive(true);

            if (current.manipulatingLeftHand)
            {
                m_HighlightLeftHandPanel.SetActive(true);
                m_LeftHandHighlightPanel.SetActive(true);
            }

            if (current.manipulatingRightHand)
            {
                m_HighlightRightHandPanel.SetActive(true);
                m_RightHandHighlightPanel.SetActive(true);
            }

            if (current.manipulatingHMD)
                m_HighlightHeadPanel.SetActive(true);
        }

        void HandleHighlightedControllerActionPanels()
        {
            var current = m_Simulator.currentState;
            var previous = m_Simulator.previousState;

            if (current.manipulatingFPS || current.currentControllerInputMode == previous.currentControllerInputMode)
                return;

            bool hasPanel = m_ControllerInputRow.ContainsKey(current.currentControllerInputMode);
            m_ControllerActionHighlightPanel.SetActive(hasPanel);

            if (hasPanel)
                m_ControllerActionHighlightPanel.transform.position = m_ControllerInputRow[current.currentControllerInputMode].transform.position;
        }

        void HandleHighlightedHandActionPanels()
        {
            var current = m_Simulator.currentState;
            var previous = m_Simulator.previousState;

            if (current.leftHandExpression != previous.leftHandExpression)
                UpdateHandActionHighlight(current.leftHandExpression, m_LeftHandActionHighlightPanel, m_LeftHandExpressionRow);

            if (current.rightHandExpression != previous.rightHandExpression)
                UpdateHandActionHighlight(current.rightHandExpression, m_RightHandActionHighlightPanel, m_RightHandExpressionRow);
        }

        void UpdateHandActionHighlight(SimulatedHandExpression expression, GameObject highlightPanel, Dictionary<SimulatedHandExpression, GameObject> rowDict)
        {
            bool hasPanel = expression != m_HandPlaybackManager.restingHandExpression && rowDict.ContainsKey(expression);
            highlightPanel.SetActive(hasPanel);
            if (hasPanel)
                highlightPanel.transform.position = rowDict[expression].transform.position;
        }

        void ClearHighlightedDevicePanels()
        {
            m_HighlightFullBodyPanel.SetActive(false);
            m_HighlightLeftControllerPanel.SetActive(false);
            m_HighlightRightControllerPanel.SetActive(false);
            m_HighlightLeftHandPanel.SetActive(false);
            m_HighlightRightHandPanel.SetActive(false);
            m_HighlightHeadPanel.SetActive(false);
            m_LeftHandHighlightPanel.SetActive(false);
            m_RightHandHighlightPanel.SetActive(false);
        }
    }
}
