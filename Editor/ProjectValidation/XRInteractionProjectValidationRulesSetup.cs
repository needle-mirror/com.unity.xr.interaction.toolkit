using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils.Editor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.ProjectValidation
{
    /// <summary>
    /// Shared GUIContent for project validation. 
    /// </summary>
    static class ProjectValidationContents
    {
        public static readonly GUIContent warningIcon = EditorGUIUtility.IconContent("Warning@2x");
        public static readonly GUIContent errorIcon = EditorGUIUtility.IconContent("Error@2x");
        public static readonly GUIContent helpIcon = EditorGUIUtility.IconContent("_Help@2x");

        public static readonly GUIContent validation = new GUIContent("Your project has some settings that are incompatible with XR Interaction Toolkit. Click to open the project validator.");
        public static readonly GUIContent validationErrorIcon = new GUIContent("", errorIcon.image, validation.text);
        public static readonly GUIContent validationWarningIcon = new GUIContent("", warningIcon.image, validation.text);
    }
    
    [InitializeOnLoad]
    class XRInteractionProjectValidationRulesSetup : EditorWindow
    {
        static XRInteractionProjectValidationRulesSetup()
        {
            EditorApplication.playModeStateChanged += (playModeState) =>
            {
                if (playModeState == PlayModeStateChange.EnteredPlayMode)
                {
                    var playmodeIssues = XRInteractionProjectValidation.LogBuildValidationErrors();
                    if (playmodeIssues)
                    {
                        EditorApplication.ExitPlaymode();
                    }
                }
            };
        }

        Vector2 m_ScrollViewPos = Vector2.zero;
        readonly List<BuildValidationRule> m_Failures = new List<BuildValidationRule>();

        // Fix all stack. 
        List<BuildValidationRule> m_FixAllStack = new List<BuildValidationRule>();

        /// <summary>
        /// Last time the issues in the window were updated.
        /// </summary>
        double m_LastUpdate;
        
        /// <summary>
        /// Interval that that issues should be updated.
        /// </summary>
        const double k_UpdateInterval = 1.0f;

        /// <summary>
        /// Interval that that issues should be updated when the window does not have focus.
        /// </summary>
        const double k_BackgroundUpdateInterval = 3.0f;

        static class Content
        {
            public static readonly GUIContent title = new GUIContent("XR Interaction Toolkit Project Validation", ProjectValidationContents.errorIcon.image);
            public static readonly GUIContent fixButton = new GUIContent("Fix", "");
            public static readonly GUIContent editButton = new GUIContent("Edit", "");
            public static readonly GUIContent playMode = new GUIContent("Exit play mode before fixing project validation issues.", EditorGUIUtility.IconContent("console.infoicon").image);
            public static readonly GUIContent helpButton = new GUIContent(ProjectValidationContents.helpIcon.image);
            public static readonly Vector2 iconSize = new Vector2(16.0f, 16.0f);
        }

        static class Styles
        {
            public static GUIStyle selectionStyle = "TV Selection";
            public static GUIStyle issuesBackground = "ScrollViewAlt";
            public static GUIStyle listLabel;
            public static GUIStyle issuesTitleLabel;
            public static GUIStyle wrap;
            public static GUIStyle icon;
            public static GUIStyle infoBanner;
            public static GUIStyle fixAll;
        }
        
        static void ShowWindowIfIssuesExist()
        {
            List<BuildValidationRule> issues = new List<BuildValidationRule>();
            XRInteractionProjectValidation.GetCurrentValidationIssues(issues);

            if (issues.Count > 0)
            {
                ShowWindow();
            }
        }

        [MenuItem("Window/XR/XR Interaction Toolkit/Project Validation")]
        static void MenuItem()
        {
            ShowWindow();
        }

        internal static void ShowWindow()
        {
            var window = (XRInteractionProjectValidationRulesSetup) GetWindow(typeof(XRInteractionProjectValidationRulesSetup));
            window.titleContent = Content.title;
            window.minSize = new Vector2(500.0f, 300.0f);
            window.UpdateIssues();
            window.Show();
        }

        internal static void CloseWindow()
        {
            var window = (XRInteractionProjectValidationRulesSetup) GetWindow(typeof(XRInteractionProjectValidationRulesSetup));
            window.Close();
        }

        static void InitStyles()
        {
            if (Styles.listLabel != null)
                return;

            Styles.listLabel = new GUIStyle(Styles.selectionStyle)
            {
                border = new RectOffset(0, 0, 0, 0),
                padding = new RectOffset(5, 5, 5, 5),
                margin = new RectOffset(5, 5, 5, 5)
            };

            Styles.issuesTitleLabel = new GUIStyle(EditorStyles.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(10, 10, 0, 0)
            };

            Styles.wrap = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(0, 5, 1, 1)
            };

            Styles.icon = new GUIStyle(EditorStyles.label)
            {
                margin = new RectOffset(5, 5, 0, 0),
                fixedWidth = Content.iconSize.x * 2
            };

            Styles.infoBanner = new GUIStyle(EditorStyles.label)
            {
                padding = new RectOffset(10,10,15,5)
            };

            Styles.fixAll = new GUIStyle(EditorStyles.miniButton)
            {
                stretchWidth = false,
                fixedWidth = 80,
                margin = new RectOffset(0,10,2,2)
            };
        }

        protected void OnFocus() => UpdateIssues(true);

        protected void Update() => UpdateIssues();

        void DrawIssuesList()
        {
            var hasFix = m_Failures.Any(f => f.FixIt != null);
            var hasAutoFix = hasFix && m_Failures.Any(f => f.FixIt != null && f.FixItAutomatic);

            // Header
            EditorGUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
            {
                EditorGUILayout.LabelField($"Issues ({m_Failures.Count})", Styles.issuesTitleLabel);
            }

            // FixAll button
            if (hasAutoFix)
            {
                using (new EditorGUI.DisabledScope(EditorApplication.isPlaying || m_FixAllStack.Count > 0))
                {
                    if (GUILayout.Button("Fix All", Styles.fixAll))
                        m_FixAllStack = m_Failures.Where(i => i.FixIt != null && i.FixItAutomatic).ToList();
                }
            }
            EditorGUILayout.EndHorizontal();

            m_ScrollViewPos = EditorGUILayout.BeginScrollView(m_ScrollViewPos, Styles.issuesBackground, GUILayout.ExpandHeight(true));

            using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
            {
                foreach (var result in m_Failures)
                {
                    EditorGUILayout.BeginHorizontal(Styles.listLabel);

                    GUILayout.Label(result.Error ? ProjectValidationContents.errorIcon : ProjectValidationContents.warningIcon, Styles.icon, GUILayout.Width(Content.iconSize.x));

                    var message = result.Message;

                    GUILayout.Label(message, Styles.wrap);
                    GUILayout.FlexibleSpace();

                    if (!string.IsNullOrEmpty(result.HelpText) || !string.IsNullOrEmpty(result.HelpLink))
                    {
                        Content.helpButton.tooltip = result.HelpText;
                        if (GUILayout.Button(Content.helpButton, Styles.icon, GUILayout.Width(Content.iconSize.x * 1.5f)))
                        {
                            if (!string.IsNullOrEmpty(result.HelpLink))
                                Application.OpenURL(result.HelpLink);
                        }
                    }
                    else
                        GUILayout.Label("", GUILayout.Width(Content.iconSize.x * 1.5f));

                    if (result.FixIt != null)
                    {
                        using (new EditorGUI.DisabledScope(m_FixAllStack.Count != 0))
                        {
                            var button = result.FixItAutomatic ? Content.fixButton : Content.editButton;
                            button.tooltip = result.FixItMessage;
                            if (GUILayout.Button(button, GUILayout.Width(80.0f)))
                            {
                                if (result.FixItAutomatic)
                                    m_FixAllStack.Add(result);
                                else
                                    result.FixIt();
                            }
                        }
                    }
                    else if (hasFix)
                    {
                        GUILayout.Label("", GUILayout.Width(80.0f));
                    }


                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        void UpdateIssues(bool force = false)
        {
            var interval = EditorWindow.focusedWindow == this ? k_UpdateInterval : k_BackgroundUpdateInterval;
            if (!force && EditorApplication.timeSinceStartup - m_LastUpdate < interval)
                return;
            
            if (m_FixAllStack.Count > 0)
            {
                m_FixAllStack[0].FixIt?.Invoke();
                m_FixAllStack.RemoveAt(0);
            }

            var failureCount = m_Failures.Count;

            XRInteractionProjectValidation.GetCurrentValidationIssues(m_Failures);

            // Repaint the window if the failure count has changed
            if(m_Failures.Count > 0 || failureCount > 0)
                Repaint();

            m_LastUpdate = EditorApplication.timeSinceStartup;
        }

        public void OnGUI()
        {
            InitStyles();

            EditorGUIUtility.SetIconSize(Content.iconSize);
            EditorGUILayout.BeginVertical();

            if (EditorApplication.isPlaying && m_Failures.Count > 0)
            {
                GUILayout.Label(Content.playMode, Styles.infoBanner);
            }

            EditorGUILayout.Space();

            DrawIssuesList();

            EditorGUILayout.EndVertical();
        }
    }
    
    class XRInteractionProjectValidationBuildStep : IPreprocessBuildWithReport
    {
        [OnOpenAsset(0)]
        static bool ConsoleErrorDoubleClicked(int instanceId, int line)
        {
            var objName = EditorUtility.InstanceIDToObject(instanceId).name;
            if (objName == "XRInteractionProjectValidation")
            {
                XRInteractionProjectValidationRulesSetup.ShowWindow();
                return true;
            }

            return false;
        }

        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)
        {
            if (XRInteractionProjectValidation.LogBuildValidationErrors())
                throw new BuildFailedException("Build Failed - XR Interaction Validation issue.");
        }
    }
}