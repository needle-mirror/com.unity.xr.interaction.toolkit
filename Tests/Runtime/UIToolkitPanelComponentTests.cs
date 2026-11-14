// Same gate as the PanelRenderer support in the runtime (XRUIToolkitHandler.cs, XRUIToolkitManager.cs),
// so these tests compile in exactly when the code they exercise does. UIELEMENTS_MODULE_PRESENT comes
// from a versionDefine in this assembly's asmdef.
#if UIELEMENTS_MODULE_PRESENT && UNITY_6000_5_OR_NEWER
#define UITOOLKIT_PANEL_RENDERER_ENABLED
#endif

#if UITOOLKIT_PANEL_RENDERER_ENABLED
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Unity.Scripting.LifecycleManagement;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using Object = UnityEngine.Object;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    /// <summary>
    /// End-to-end tests for world space UI Toolkit panels hosted by either a <see cref="UIDocument"/>
    /// or a <see cref="PanelRenderer"/> (Unity 6000.5+): poke and ray clicks, XRI UI hover events,
    /// dynamic collider registration, and trigger collider filtering.
    /// </summary>
    /// <remarks>
    /// Panels, panel settings and visual trees are built in code. Interactors are moved directly and
    /// their UI press input is driven in manual mode, so no XR device or input actions are needed.
    /// </remarks>
    [TestFixture]
    class UIToolkitPanelComponentTests
    {
        public enum PanelType
        {
            UIDocument,
            PanelRenderer,
        }

        /// <summary>
        /// Mirrors <c>UnityEngine.UIElements.ColliderUpdateMode</c>, which is internal to UnityEngine and so
        /// not visible to this test assembly. Values match the engine enum.
        /// </summary>
        public enum PanelColliders
        {
            /// <summary>ColliderUpdateMode.MatchBoundingBox: UI Toolkit creates and resizes the collider.</summary>
            MatchBoundingBox = 0,

            /// <summary>ColliderUpdateMode.Keep: the authored collider is used as is.</summary>
            KeepExisting = 1,
        }

        const string k_ButtonName = "xri-test-button";
        const string k_ScrollViewName = "xri-test-scroll";

        // 300 x 435 px at 100 pixels per unit and 0.2 scale gives a 0.6 m x 0.87 m panel.
        static readonly Vector2 k_PanelSizePixels = new Vector2(300f, 435f);
        const float k_PanelScale = 0.2f;
        static readonly Vector3 k_PanelPosition = new Vector3(0f, 0f, 1.5f);

        class TestPanel
        {
            public GameObject host;
            public IPanelComponent panelComponent;
            public XRSimpleInteractable interactable;
            public PanelSettings panelSettings;
            public VisualElement root;
            public Button button;
            public ScrollView scrollView;
            public int clickCount;
        }

        struct HoverRecord
        {
            public bool entered;
            public IUIHoverInteractor interactor;
            public GameObject uiObject;
            public IPanelComponent panelComponent;
            public UIDocument uiDocument;
            public VisualElement visualElement;
            public bool isScrollable;
            public GameObject selectableObject;
        }

        readonly List<Object> m_CreatedAssets = new List<Object>();
        readonly List<HoverRecord> m_Hovers = new List<HoverRecord>();
        XRInteractionManager m_Manager;
        XRUIToolkitManager m_ToolkitManager;

        [SetUp]
        public void SetUp()
        {
            m_Hovers.Clear();
            TestUtilities.ForceUIToolkitTrackedInput();

            m_Manager = TestUtilities.CreateInteractionManager();
            m_ToolkitManager = new GameObject("XR UI Toolkit Manager").AddComponent<XRUIToolkitManager>();

            var eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<XRUIInputModule>();
            var panelInputConfiguration = new GameObject("Panel Input Configuration").AddComponent<PanelInputConfiguration>();
            panelInputConfiguration.panelInputRedirection = PanelInputConfiguration.PanelInputRedirection.Never;
        }

        [TearDown]
        public void TearDown()
        {
            TestUtilities.ResetXRIRuntimeSettings();
            TestUtilities.DestroyAllSceneObjects();
            XRUIToolkitHandler.Clear();

            foreach (var asset in m_CreatedAssets)
            {
                if (asset != null)
                    Object.Destroy(asset);
            }

            m_CreatedAssets.Clear();
            TestUtilities.RestoreUIToolkitTrackedInput();
        }

        // ---------------------------------------------------------------------------------------------
        // Hover
        // ---------------------------------------------------------------------------------------------

        [UnityTest]
        public IEnumerator RayHoverOverButtonRaisesUIHoverEnteredAndExited([Values] PanelType panelType)
        {
            var panel = CreatePanel(panelType, PanelColliders.MatchBoundingBox);
            var ray = CreateRayInteractor();
            yield return WaitForPanel(panel);

            yield return TestUtilities.AimAtElement(ray.transform, () => ray.rayOriginTransform, panel.host.transform, panel.button);

            var entered = HoversFor(ray, panel, true);
            Assert.That(entered, Is.Not.Empty, $"Ray over a {panelType} button should raise uiHoverEntered.");
            AssertHoverArgs(entered[0], panelType, panel);

            TestUtilities.ParkOutOfReach(ray.transform);
            yield return null;

            var exited = HoversFor(ray, panel, false);
            Assert.That(exited, Is.Not.Empty, $"Moving the ray off a {panelType} panel should raise uiHoverExited.");
            AssertHoverArgs(exited[exited.Count - 1], panelType, panel);
        }

        [UnityTest]
        public IEnumerator NearFarHoverOverButtonRaisesUIHoverEntered([Values] PanelType panelType)
        {
            var panel = CreatePanel(panelType, PanelColliders.MatchBoundingBox);
            var nearFar = CreateNearFarInteractor();
            yield return WaitForPanel(panel);

            yield return TestUtilities.AimAtElement(nearFar.transform, () => nearFar.farInteractionCaster?.effectiveCastOrigin, panel.host.transform, panel.button);

            var entered = HoversFor(nearFar, panel, true);
            Assert.That(entered, Is.Not.Empty, $"Near-Far far cast over a {panelType} button should raise uiHoverEntered.");
            AssertHoverArgs(entered[0], panelType, panel);
        }

        [UnityTest]
        public IEnumerator RayHoverOverScrollViewReportsScrollable([Values] PanelType panelType)
        {
            var panel = CreatePanel(panelType, PanelColliders.MatchBoundingBox);
            var ray = CreateRayInteractor();
            yield return WaitForPanel(panel);

            yield return TestUtilities.AimAtElement(ray.transform, () => ray.rayOriginTransform, panel.host.transform, panel.scrollView);

            var entered = HoversFor(ray, panel, true);
            Assert.That(entered, Is.Not.Empty, $"Ray over a {panelType} ScrollView should raise uiHoverEntered.");
            Assert.That(entered.Any(h => h.isScrollable), Is.True,
                $"Hovering a {panelType} ScrollView should report TrackedDeviceModel.isScrollable.");
        }

        [UnityTest]
        public IEnumerator DisablingHoveredPanelRaisesUIHoverExited([Values] PanelType panelType, [Values] bool deactivateGameObject)
        {
            // An authored collider (Keep mode) survives the panel component being disabled, unlike a generated one.
            var panel = CreatePanel(panelType, PanelColliders.KeepExisting);
            var ray = CreateRayInteractor();
            yield return WaitForPanel(panel);

            yield return TestUtilities.AimAtElement(ray.transform, () => ray.rayOriginTransform, panel.host.transform, panel.button);
            Assert.That(HoversFor(ray, panel, true), Is.Not.Empty, $"Ray over a {panelType} button should raise uiHoverEntered.");

            if (deactivateGameObject)
                panel.host.SetActive(false);
            else
                SetPanelComponentEnabled(panel, false);

            yield return null;

            Assert.That(HoversFor(ray, panel, false), Is.Not.Empty,
                $"{(deactivateGameObject ? "Deactivating the GameObject of" : "Disabling")} a hovered {panelType} should raise uiHoverExited.");
        }

        // ---------------------------------------------------------------------------------------------
        // Clicks
        // ---------------------------------------------------------------------------------------------

        [UnityTest]
        public IEnumerator PokeClicksButton(
            [Values] PanelType panelType,
            [Values] PanelColliders colliderUpdateMode)
        {
            var panel = CreatePanel(panelType, colliderUpdateMode);
            var poke = TestUtilities.CreatePokeInteractor();
            TestUtilities.ParkOutOfReach(poke.transform);
            yield return WaitForPanel(panel);

            yield return TestUtilities.PokeTestElement(poke, panel.host.transform, panel.button);

            Assert.That(panel.clickCount, Is.EqualTo(1), $"Poking a {panelType} button ({colliderUpdateMode}) should click it once.");
        }

        [UnityTest]
        public IEnumerator RayPressClicksButton(
            [Values] PanelType panelType,
            [Values] PanelColliders colliderUpdateMode)
        {
            var panel = CreatePanel(panelType, colliderUpdateMode);
            var ray = CreateRayInteractor();
            yield return WaitForPanel(panel);

            yield return TestUtilities.AimAtElement(ray.transform, () => ray.rayOriginTransform, panel.host.transform, panel.button);
            ray.uiPressInput.QueueManualState(true, 1f);
            yield return null;
            ray.uiPressInput.QueueManualState(false, 0f);
            yield return null;

            Assert.That(panel.clickCount, Is.EqualTo(1), $"Ray press over a {panelType} button ({colliderUpdateMode}) should click it once.");
        }

        // ---------------------------------------------------------------------------------------------
        // Collider registration (XRUIToolkitManager)
        // ---------------------------------------------------------------------------------------------

        [UnityTest]
        public IEnumerator DynamicColliderIsRegisteredWithInteractable([Values] PanelType panelType)
        {
            var panel = CreatePanel(panelType, PanelColliders.MatchBoundingBox);
            yield return WaitForPanel(panel);

            AssertPanelColliderRegistered(panel, "after panel creation");
        }

        [UnityTest]
        public IEnumerator DynamicColliderStaysRegisteredAfterPanelComponentReenabled([Values] PanelType panelType)
        {
            var panel = CreatePanel(panelType, PanelColliders.MatchBoundingBox);
            yield return WaitForPanel(panel);
            AssertPanelColliderRegistered(panel, "before disabling the panel component");

            SetPanelComponentEnabled(panel, false);
            yield return null;
            SetPanelComponentEnabled(panel, true);
            RebuildUIDocumentContent(panel);
            yield return WaitForPanel(panel);

            AssertPanelColliderRegistered(panel, "after re-enabling the panel component");

            // The poke path relies on the registered collider, so check it still works end to end.
            var poke = TestUtilities.CreatePokeInteractor();
            TestUtilities.ParkOutOfReach(poke.transform);
            yield return TestUtilities.PokeTestElement(poke, panel.host.transform, panel.button);
            Assert.That(panel.clickCount, Is.EqualTo(1), $"Poking a {panelType} button after re-enabling the panel component should click it once.");
        }

        [UnityTest]
        public IEnumerator DynamicColliderStaysRegisteredAfterPanelGameObjectReactivated([Values] PanelType panelType)
        {
            var panel = CreatePanel(panelType, PanelColliders.MatchBoundingBox);
            yield return WaitForPanel(panel);
            AssertPanelColliderRegistered(panel, "before deactivating the panel GameObject");

            // Deactivating the GameObject also unregisters and re-registers the interactable, which takes
            // the XRUIToolkitManager re-registration fast path.
            panel.host.SetActive(false);
            yield return null;
            panel.host.SetActive(true);
            RebuildUIDocumentContent(panel);
            yield return WaitForPanel(panel);

            AssertPanelColliderRegistered(panel, "after reactivating the panel GameObject");

            var poke = TestUtilities.CreatePokeInteractor();
            TestUtilities.ParkOutOfReach(poke.transform);
            yield return TestUtilities.PokeTestElement(poke, panel.host.transform, panel.button);
            Assert.That(panel.clickCount, Is.EqualTo(1), $"Poking a {panelType} button after reactivating the panel GameObject should click it once.");
        }

        [UnityTest]
        public IEnumerator ReregisteredPanelIsPolledOnlyWithoutColliderEvents([Values] PanelType panelType, [Values] bool restartManager)
        {
            var panel = CreatePanel(panelType, PanelColliders.MatchBoundingBox);
            yield return WaitForPanel(panel);

            // Reactivating the panel GameObject re-registers the interactable (the OnInteractableRegistered fast path).
            // Restarting the XR UI Toolkit Manager rescans the registered interactables (SubscribeToManager).
            if (restartManager)
            {
                m_ToolkitManager.enabled = false;
                yield return null;
                m_ToolkitManager.enabled = true;
            }
            else
            {
                panel.host.SetActive(false);
                yield return null;
                panel.host.SetActive(true);
                RebuildUIDocumentContent(panel);
            }

            yield return WaitForPanel(panel);

            AssertPanelColliderRegistered(panel, restartManager ? "after restarting the XR UI Toolkit Manager" : "after reactivating the panel GameObject");
#if UNITY_7000_0_a8_OR_NEWER
            Assert.That(m_ToolkitManager.IsMonitored(panel.interactable), Is.False,
                $"A re-registered {panelType} gets collider events, so it should not also be polled every frame.");
#else
            Assert.That(m_ToolkitManager.IsMonitored(panel.interactable), Is.True,
                $"Without collider events, a re-registered {panelType} should be polled every frame.");
#endif
        }

        [UnityTest]
        public IEnumerator KeepExistingCollidersUsesOnlyTheAuthoredCollider([Values] PanelType panelType)
        {
            var panel = CreatePanel(panelType, PanelColliders.KeepExisting);
            yield return WaitForPanel(panel);

            var authored = panel.host.GetComponent<BoxCollider>();
            Assert.That(panel.host.GetComponents<Collider>(), Has.Length.EqualTo(1), "Keep existing colliders should not add a collider.");
            Assert.That(panel.interactable.colliders, Is.EqualTo(new Collider[] { authored }));
            Assert.That(m_Manager.TryGetInteractableForCollider(authored, out var mapped), Is.True);
            Assert.That(mapped, Is.SameAs(panel.interactable));
        }

        // ---------------------------------------------------------------------------------------------
        // Panel component detection and trigger collider filtering
        // ---------------------------------------------------------------------------------------------

        [Test]
        public void HasPanelComponentDetectsUIDocumentAndPanelRenderer()
        {
            var documentTrigger = CreateBarePanelTrigger(PanelType.UIDocument);
            var rendererTrigger = CreateBarePanelTrigger(PanelType.PanelRenderer);
            var plainTrigger = TestUtilities.CreateGOBoxCollider(new GameObject("Plain Trigger"));

            Assert.That(XRUIToolkitHandler.HasPanelComponent(documentTrigger), Is.True);
            Assert.That(XRUIToolkitHandler.HasPanelComponent(rendererTrigger), Is.True);
            Assert.That(XRUIToolkitHandler.HasPanelComponent(plainTrigger), Is.False);

            Assert.That(XRUIToolkitHandler.IsValidUIToolkitInteraction(new List<Collider> { rendererTrigger }), Is.True);
            Assert.That(XRUIToolkitHandler.IsValidUIToolkitInteraction(new List<Collider> { plainTrigger, rendererTrigger }), Is.False,
                "Only the first collider in the list is checked.");
        }

        [Test]
        public void TriggerFilterTreatsPanelRendererLikeUIDocument(
            [Values] bool baseQueryIncludesTriggers,
            [Values] bool allowUIDocumentTriggers)
        {
            var documentTrigger = CreateBarePanelTrigger(PanelType.UIDocument);
            var rendererTrigger = CreateBarePanelTrigger(PanelType.PanelRenderer);
            var plainTrigger = TestUtilities.CreateGOBoxCollider(new GameObject("Plain Trigger"));

            var colliders = new Collider[] { documentTrigger, rendererTrigger, plainTrigger };
            var count = TriggerColliderFilterUtility.FilterTriggerColliders(
                m_Manager, colliders, colliders.Length, baseQueryIncludesTriggers,
                allowSnapVolumeTriggers: false, allowUIDocumentTriggers: allowUIDocumentTriggers);
            var remaining = colliders.Take(count).ToList();

            // A UI Toolkit panel trigger is kept when UI Document triggers are allowed. A plain trigger is only
            // kept when the base query already includes triggers.
            Assert.That(remaining.Contains(documentTrigger), Is.EqualTo(allowUIDocumentTriggers), "UIDocument trigger");
            Assert.That(remaining.Contains(rendererTrigger), Is.EqualTo(allowUIDocumentTriggers), "PanelRenderer trigger");
            Assert.That(remaining.Contains(plainTrigger), Is.EqualTo(baseQueryIncludesTriggers), "Plain trigger");
        }

        [Test]
        public void PointerHitDataResolvesPanelComponentFromCollider([Values] PanelType panelType)
        {
            var trigger = CreateBarePanelTrigger(panelType);
            var hitData = new PointerHitData { hitCollider = trigger };

            Assert.That(hitData.TryGetPanelComponent(out var panelComponent), Is.True);
            Assert.That(panelComponent, Is.SameAs(trigger.GetComponent<IPanelComponent>()));

            // TryGetUIDocument only succeeds for a UIDocument host.
            Assert.That(hitData.TryGetUIDocument(out var document), Is.EqualTo(panelType == PanelType.UIDocument));
            Assert.That(document, panelType == PanelType.UIDocument ? Is.SameAs(trigger.GetComponent<UIDocument>()) : Is.Null);
        }

        // ---------------------------------------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------------------------------------

        TestPanel CreatePanel(PanelType panelType, PanelColliders colliderUpdateMode)
        {
            var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
            panelSettings.renderMode = PanelRenderMode.WorldSpace;
            SetColliderUpdateMode(panelSettings, colliderUpdateMode);
            // A new PanelSettings instance defaults to a trigger collider; the World Space UI sample panels use a
            // solid one, which is what XRBaseInteractable registers by default.
            SetColliderIsTrigger(panelSettings, false);
            m_CreatedAssets.Add(panelSettings);

            // An empty tree asset; the elements are added in code once the panel root exists.
            var visualTree = ScriptableObject.CreateInstance<VisualTreeAsset>();
            m_CreatedAssets.Add(visualTree);

            var panel = new TestPanel { panelSettings = panelSettings };

            // Build the host inactive so the interactable registers after the panel component exists,
            // matching a panel authored in a scene.
            var host = new GameObject($"{panelType} Panel ({colliderUpdateMode})");
            host.SetActive(false);
            host.transform.position = k_PanelPosition;
            host.transform.localScale = Vector3.one * k_PanelScale;
            panel.host = host;

            if (colliderUpdateMode == PanelColliders.KeepExisting)
            {
                // Authored collider covering the panel, as in the World Space UI sample Grab UI prefab.
                // Pivot is top left, so the panel spans +x and -y in local space.
                var box = host.AddComponent<BoxCollider>();
                var sizeUnits = k_PanelSizePixels / 100f;
                box.size = new Vector3(sizeUnits.x, sizeUnits.y, 0.4f);
                box.center = new Vector3(sizeUnits.x * 0.5f, -sizeUnits.y * 0.5f, 0f);
            }

            IPanelComponent panelComponent;
            if (panelType == PanelType.PanelRenderer)
            {
                var panelRenderer = host.AddComponent<PanelRenderer>();
                panelRenderer.RegisterUIReloadCallback(new PanelRenderer.VersionedUIReloadCallback((_, root, _) => BuildContent(panel, root)));
                panelComponent = panelRenderer;
            }
            else
            {
                panelComponent = host.AddComponent<UIDocument>();
            }

            panelComponent.panelSettings = panelSettings;
            panelComponent.visualTreeAsset = visualTree;
            panelComponent.worldSpaceSizeMode = WorldSpaceSizeMode.Fixed;
            panelComponent.worldSpaceSize = k_PanelSizePixels;
            panelComponent.pivot = Pivot.TopLeft;
            panel.panelComponent = panelComponent;

            panel.interactable = host.AddComponent<XRSimpleInteractable>();
            var pokeFilter = host.AddComponent<XRPokeFilter>();
            pokeFilter.pokeInteractable = panel.interactable;
            if (colliderUpdateMode == PanelColliders.KeepExisting)
                pokeFilter.pokeCollider = host.GetComponent<BoxCollider>();

            host.SetActive(true);

            if (panelComponent is UIDocument document)
                BuildContent(panel, document.rootVisualElement);

            return panel;
        }

        static void BuildContent(TestPanel panel, VisualElement root)
        {
            if (root == null)
                return;

            root.Clear();
            root.style.flexGrow = 1f;
            root.style.paddingLeft = 10f;
            root.style.paddingRight = 10f;
            root.style.paddingTop = 10f;
            root.style.paddingBottom = 10f;

            var button = new Button { name = k_ButtonName, text = "Button" };
            button.style.height = 80f;
            button.clicked += () => panel.clickCount++;
            root.Add(button);

            var scrollView = new ScrollView(ScrollViewMode.Vertical) { name = k_ScrollViewName };
            scrollView.style.flexGrow = 1f;
            scrollView.style.marginTop = 20f;
            for (var i = 0; i < 20; ++i)
            {
                var label = new Label($"Item {i + 1:00}");
                label.style.height = 40f;
                scrollView.Add(label);
            }

            root.Add(scrollView);

            panel.root = root;
            panel.button = button;
            panel.scrollView = scrollView;
        }

        /// <summary>
        /// A UIDocument rebuilds its tree from its (empty) tree asset when it is re-enabled, so the elements
        /// added in code have to be added again. A PanelRenderer gets them back from its reload callback.
        /// </summary>
        static void RebuildUIDocumentContent(TestPanel panel)
        {
            if (panel.panelComponent is UIDocument document)
                BuildContent(panel, document.rootVisualElement);
        }

        static IEnumerator WaitForPanel(TestPanel panel)
        {
            // One frame lets the panel build and lay out its content, and create and register its collider.
            yield return null;

            Assert.That(panel.button, Is.Not.Null, $"{panel.host.name}: panel content was not built.");
            Assert.That(panel.button.layout.width, Is.GreaterThan(0f), $"{panel.host.name}: panel was not laid out.");
            var collider = panel.host.GetComponent<Collider>();
            Assert.That(collider, Is.Not.Null, $"{panel.host.name}: panel has no collider.");
            Assert.That(panel.interactable.colliders, Contains.Item(collider), $"{panel.host.name}: panel collider was not registered with the interactable.");
        }

        Collider CreateBarePanelTrigger(PanelType panelType)
        {
            var go = new GameObject($"{panelType} Trigger");
            if (panelType == PanelType.PanelRenderer)
                go.AddComponent<PanelRenderer>();
            else
                go.AddComponent<UIDocument>();

            return TestUtilities.CreateGOBoxCollider(go);
        }

        XRRayInteractor CreateRayInteractor()
        {
            var ray = TestUtilities.CreateRayInteractor();
            ray.enableUIInteraction = true;
            TestUtilities.SetManualInput(ray.uiPressInput, ray.selectInput, ray.activateInput);
            TestUtilities.ParkOutOfReach(ray.transform);
            ray.uiHoverEntered.AddListener(args => Record(true, ray, args));
            ray.uiHoverExited.AddListener(args => Record(false, ray, args));
            return ray;
        }

        NearFarInteractor CreateNearFarInteractor()
        {
            var nearFar = TestUtilities.CreateNearFarInteractor();
            nearFar.enableUIInteraction = true;
            TestUtilities.SetManualInput(nearFar.uiPressInput, nearFar.selectInput, nearFar.activateInput);
            TestUtilities.ParkOutOfReach(nearFar.transform);
            nearFar.uiHoverEntered.AddListener(args => Record(true, nearFar, args));
            nearFar.uiHoverExited.AddListener(args => Record(false, nearFar, args));
            return nearFar;
        }

        void Record(bool entered, IUIHoverInteractor interactor, UIHoverEventArgs args)
        {
            // The args object is pooled, so copy what the asserts need.
#pragma warning disable CS0618 // uiDocument is obsolete on 6000.5+, but XRI still sets it for UIDocument panels and the tests check it.
            var uiDocument = args.uiDocument;
#pragma warning restore CS0618
            m_Hovers.Add(new HoverRecord
            {
                entered = entered,
                interactor = interactor,
                uiObject = args.uiObject,
                panelComponent = args.panelComponent,
                uiDocument = uiDocument,
                visualElement = args.visualElement,
                isScrollable = args.deviceModel.isScrollable,
                selectableObject = args.deviceModel.selectableObject,
            });
        }

        List<HoverRecord> HoversFor(IUIHoverInteractor interactor, TestPanel panel, bool entered) =>
            m_Hovers.Where(h => h.interactor == interactor && h.entered == entered && h.uiObject == panel.host).ToList();

        static void AssertHoverArgs(HoverRecord record, PanelType panelType, TestPanel panel)
        {
            Assert.That(record.uiObject, Is.SameAs(panel.host), "uiObject");
            Assert.That(record.selectableObject, Is.SameAs(panel.host), "deviceModel.selectableObject");
            Assert.That(record.panelComponent, Is.SameAs(panel.panelComponent), "panelComponent");
            if (panelType == PanelType.UIDocument)
                Assert.That(record.uiDocument, Is.SameAs(panel.panelComponent), "uiDocument should be the UIDocument host");
            else
                Assert.That(record.uiDocument, Is.Null, "uiDocument should be null for a PanelRenderer host");
        }

        void AssertPanelColliderRegistered(TestPanel panel, string when)
        {
            var collider = panel.host.GetComponent<Collider>();
            Assert.That(collider, Is.Not.Null, $"{panel.host.name} has no collider {when}.");
            Assert.That(panel.interactable.colliders, Contains.Item(collider), $"Panel collider is not in interactable.colliders {when}.");
            Assert.That(panel.interactable.colliders.Any(c => c == null), Is.False, $"interactable.colliders holds a destroyed collider {when}.");
            Assert.That(m_Manager.TryGetInteractableForCollider(collider, out var mapped), Is.True, $"Manager does not map the panel collider {when}.");
            Assert.That(mapped, Is.SameAs(panel.interactable), $"Manager maps the panel collider to another interactable {when}.");
        }

        /// <summary>
        /// UIDocument is a Behaviour but PanelRenderer is a Renderer, so set enabled by component type.
        /// </summary>
        static void SetPanelComponentEnabled(TestPanel panel, bool enabled)
        {
            if (panel.panelComponent is Behaviour behaviour)
                behaviour.enabled = enabled;
            else if (panel.panelComponent is Renderer renderer)
                renderer.enabled = enabled;
        }

        [NoAutoStaticsCleanup]
        static readonly PropertyInfo s_ColliderUpdateMode =
            typeof(PanelSettings).GetProperty("colliderUpdateMode", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        /// <summary>
        /// PanelSettings.colliderUpdateMode and its enum are internal to UnityEngine (XRI reaches them through
        /// InternalsVisibleTo, this test assembly cannot), so set it through reflection.
        /// </summary>
        static void SetColliderUpdateMode(PanelSettings panelSettings, PanelColliders mode)
        {
            Assert.That(s_ColliderUpdateMode, Is.Not.Null, "PanelSettings.colliderUpdateMode not found.");
            s_ColliderUpdateMode.SetValue(panelSettings, Enum.ToObject(s_ColliderUpdateMode.PropertyType, (int)mode));
            Assert.That(Convert.ToInt32(s_ColliderUpdateMode.GetValue(panelSettings)), Is.EqualTo((int)mode), "colliderUpdateMode was not applied.");
        }

        [NoAutoStaticsCleanup]
        static readonly PropertyInfo s_ColliderIsTrigger =
            typeof(PanelSettings).GetProperty("colliderIsTrigger", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        /// <summary>
        /// PanelSettings.colliderIsTrigger is internal to UnityEngine, so set it through reflection.
        /// </summary>
        static void SetColliderIsTrigger(PanelSettings panelSettings, bool isTrigger)
        {
            Assert.That(s_ColliderIsTrigger, Is.Not.Null, "PanelSettings.colliderIsTrigger not found.");
            s_ColliderIsTrigger.SetValue(panelSettings, isTrigger);
        }
    }
}
#endif // UITOOLKIT_PANEL_RENDERER_ENABLED
