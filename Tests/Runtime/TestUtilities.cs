using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif
using Unity.XR.CoreUtils;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Attachment;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Casters;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Transformers;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    static class TestUtilities
    {
        /// <summary>
        /// Reset the XR Interaction Toolkit runtime project settings back to default.
        /// </summary>
        internal static void ResetXRIRuntimeSettings()
        {
            if (XRInteractionRuntimeSettings.InstanceInternal != null)
            {
                XRInteractionRuntimeSettings.InstanceInternal.managerCreationMode = default;
                XRInteractionRuntimeSettings.InstanceInternal.interactionManagerSingletonMode = default;
                XRInteractionRuntimeSettings.InstanceInternal.interactionManagerRegistrationMode = default;
                XRInteractionRuntimeSettings.InstanceInternal.uiModuleRegistrationMode = default;
            }
        }

        internal static void DestroyAllSceneObjects()
        {
            // Some XR Interaction Toolkit components may have been automatically created with DontDestroyOnLoad set
            // by the ComponentLocatorUtility, so ensure those are destroyed too since the SceneManager does not
            // include that special scene.
            // Note that DontDestroyOnLoad can only be invoked in Play mode, so skip this when running EditMode tests.
            if (Application.isPlaying)
            {
                if (SceneManager.sceneCount > 0)
                {
                    var firstScene = SceneManager.GetSceneAt(0);

                    var helperGO = new GameObject("DestroyAllSceneObjects Helper");
                    Object.DontDestroyOnLoad(helperGO);
                    if (helperGO.scene.name == "DontDestroyOnLoad")
                    {
                        foreach (var go in helperGO.scene.GetRootGameObjects())
                        {
                            // Ignore the DebugUpdater created by com.unity.render-pipelines.core
                            if (go.name == "[Debug Updater]")
                                continue;

                            SceneManager.MoveGameObjectToScene(go, firstScene);
                        }
                    }
                    else
                        Assert.Fail("Helper GameObject was not immediately moved to the DontDestroyOnLoad scene," +
                            "cannot iterate that scene to ensure destruction of all GameObjects.");
                }
                else
                    Assert.Fail("No default scene to move the DontDestroyOnLoad scene GameObjects to." +
                        " Cannot ensure destruction of all GameObjects.");
            }

            // Destroy all GameObjects in all scenes
            for (var index = 0; index < SceneManager.sceneCount; ++index)
            {
                var scene = SceneManager.GetSceneAt(index);
                foreach (var go in scene.GetRootGameObjects())
                {
                    if (go == null || go.name.Contains("tests runner"))
                        continue;

                    // DestroyImmediate should generally only be called during Edit mode only,
                    // but runtime tests need to rely on automatic component location to not find
                    // components that are pending being destroyed from a previous test. So until
                    // the time we update all our tests to wait a frame to ensure the scene objects
                    // from previous tests are destroyed, we use DestroyImmediate.
                    Object.DestroyImmediate(go);
                }
            }
        }

        internal static void DisableDelayProperties(XRGrabInteractable grabInteractable)
        {
            grabInteractable.velocityDamping = 1f;
            grabInteractable.velocityScale = 1f;
            grabInteractable.angularVelocityDamping = 1f;
            grabInteractable.angularVelocityScale = 1f;
            grabInteractable.attachEaseInTime = 0f;
            var rigidbody = grabInteractable.GetComponent<Rigidbody>();
            rigidbody.maxAngularVelocity = float.PositiveInfinity;
        }

        internal static BoxCollider CreateGOBoxCollider(GameObject go, bool isTrigger = true)
        {
            BoxCollider collider = go.AddComponent<BoxCollider>();
            collider.size = new Vector3(2.0f, 2.0f, 2.0f);
            collider.isTrigger = isTrigger;
            return collider;
        }

        internal static SphereCollider CreateGOSphereCollider(GameObject go, bool isTrigger = true)
        {
            SphereCollider collider = go.AddComponent<SphereCollider>();
            collider.radius = 1.0f;
            collider.isTrigger = isTrigger;
            return collider;
        }

        internal static XRInteractionManager CreateInteractionManager()
        {
            GameObject managerGO = new GameObject("Interaction Manager");
            XRInteractionManager manager = managerGO.AddComponent<XRInteractionManager>();
            return manager;
        }

        internal static NearFarInteractor CreateNearFarInteractor()
        {
            GameObject interactorGO = new GameObject("Near-Far Interactor");

            NearFarInteractor interactor = interactorGO.AddComponent<NearFarInteractor>();
            var attachController = interactorGO.GetComponent<InteractionAttachController>();

            // Distance based velocity scaling is disabled because it is not supported without an XR Origin
            attachController.useDistanceBasedVelocityScaling = false;

            return interactor;
        }

        internal static NearFarInteractor CreateNearFarInteractorWithXROrigin(out Camera camera, bool enableStabilization = false)
        {
            var xrOrigin = CreateXROrigin();
            camera = xrOrigin.Camera;

            var leftTesthand = new GameObject("Left Hand Test Controller");
            leftTesthand.transform.SetParent(xrOrigin.CameraFloorOffsetObject.transform, false);

            GameObject interactorGO = new GameObject("Near-Far Interactor");
            interactorGO.transform.SetParent(leftTesthand.transform, false);

            NearFarInteractor interactor = interactorGO.AddComponent<NearFarInteractor>();
            interactor.handedness = InteractorHandedness.Left;

            if (enableStabilization)
            {
                interactor.GetComponent<CurveInteractionCaster>().enableStabilization = true;
                interactor.GetComponent<SphereInteractionCaster>().enableStabilization = true;
            }

            return interactor;
        }

        internal static XRDirectInteractor CreateDirectInteractor()
        {
            GameObject interactorGO = new GameObject("Direct Interactor");
            CreateGOSphereCollider(interactorGO);
            XRDirectInteractor interactor = interactorGO.AddComponent<XRDirectInteractor>();
            return interactor;
        }

        internal static XRPokeInteractor CreatePokeInteractor()
        {
            GameObject interactorGO = new GameObject("Poke Interactor");
            XRPokeInteractor interactor = interactorGO.AddComponent<XRPokeInteractor>();
            return interactor;
        }

        internal static XROrigin CreateXROrigin()
        {
            var xrOriginGO = new GameObject("XR Origin");
            xrOriginGO.SetActive(false);
            var xrOrigin = xrOriginGO.AddComponent<XROrigin>();
            xrOrigin.Origin = xrOriginGO;

            // Add camera offset
            var cameraOffsetGO = new GameObject("CameraOffset");
            cameraOffsetGO.transform.SetParent(xrOrigin.transform, false);
            xrOrigin.CameraFloorOffsetObject = cameraOffsetGO;
            xrOrigin.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            // Add camera
            var cameraGO = new GameObject("Camera");
            var camera = cameraGO.AddComponent<Camera>();

            cameraGO.transform.SetParent(cameraOffsetGO.transform, false);
            xrOrigin.Camera = camera;
            xrOriginGO.SetActive(true);

            return xrOrigin;
        }

        internal static TeleportationArea CreateTeleportAreaPlane()
        {
            var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "plane";
            plane.transform.localScale = new Vector3(10f, 1f, 10f);
            return plane.AddComponent<TeleportationArea>();
        }

        internal static TeleportationAnchor CreateTeleportAnchorPlane()
        {
            var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "plane";
            return plane.AddComponent<TeleportationAnchor>();
        }

        internal static XRRayInteractor CreateRayInteractor()
        {
            GameObject interactorGO = new GameObject("Ray Interactor");
            XRRayInteractor interactor = interactorGO.AddComponent<XRRayInteractor>();
            interactor.enableUIInteraction = false;
            interactorGO.AddComponent<XRInteractorLineVisual>();
            return interactor;
        }

        internal static XRGazeInteractor CreateGazeInteractor()
        {
            GameObject interactorGO = new GameObject("Gaze Interactor");
            XRGazeInteractor interactor = interactorGO.AddComponent<XRGazeInteractor>();
            interactor.enableUIInteraction = false;
            return interactor;
        }

        internal static XRSocketInteractor CreateSocketInteractor()
        {
            GameObject interactorGO = new GameObject("Socket Interactor");
            CreateGOSphereCollider(interactorGO);
            XRSocketInteractor interactor = interactorGO.AddComponent<XRSocketInteractor>();
            return interactor;
        }

        internal static MockInteractor CreateMockInteractor()
        {
            var interactorGO = new GameObject("Mock Interactor");
            interactorGO.transform.localPosition = Vector3.zero;
            interactorGO.transform.localRotation = Quaternion.identity;
            var interactor = interactorGO.AddComponent<MockInteractor>();
            return interactor;
        }

        internal static MockInputInteractor CreateMockInputInteractor()
        {
            var interactorGO = new GameObject("Mock Input Interactor");
            interactorGO.transform.localPosition = Vector3.zero;
            interactorGO.transform.localRotation = Quaternion.identity;
            var interactor = interactorGO.AddComponent<MockInputInteractor>();
            return interactor;
        }

        internal static MockMovementTypeOverrideInteractor CreateMockMovementTypeOverrideInteractor()
        {
            var interactorGO = new GameObject("Mock Interactor");
            interactorGO.transform.localPosition = Vector3.zero;
            interactorGO.transform.localRotation = Quaternion.identity;
            var interactor = interactorGO.AddComponent<MockMovementTypeOverrideInteractor>();
            return interactor;
        }

        internal static MockClassInteractor CreateMockClassInteractor()
        {
            var interactorGO = new GameObject("Mock Interactor");
            interactorGO.transform.localPosition = Vector3.zero;
            interactorGO.transform.localRotation = Quaternion.identity;
            var interactor = new MockClassInteractor(interactorGO.transform);
            return interactor;
        }

        internal static MockClassUIInteractor CreateMockClassUIInteractor()
        {
            var interactorGO = new GameObject("Mock UI Interactor");
            interactorGO.transform.localPosition = Vector3.zero;
            interactorGO.transform.localRotation = Quaternion.identity;
            var interactor = new MockClassUIInteractor(interactorGO.transform);
            return interactor;
        }

        internal static MockClassInteractable CreateMockClassInteractable()
        {
            var interactableGO = new GameObject("Mock Interactable");
            interactableGO.transform.localPosition = Vector3.zero;
            interactableGO.transform.localRotation = Quaternion.identity;
            var interactable = new MockClassInteractable(interactableGO.transform);
            return interactable;
        }

        internal static XRGrabInteractable CreateGrabInteractable()
        {
            GameObject interactableGO = new GameObject("Grab Interactable");
            CreateGOSphereCollider(interactableGO, false);
            XRGrabInteractable interactable = interactableGO.AddComponent<XRGrabInteractable>();
            interactable.throwOnDetach = false;
            var rigidBody = interactableGO.GetComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
            return interactable;
        }

        internal static XRSimpleInteractable CreateSimpleInteractable()
        {
            GameObject interactableGO = new GameObject("Simple Interactable");
            CreateGOSphereCollider(interactableGO, false);
            XRSimpleInteractable interactable = interactableGO.AddComponent<XRSimpleInteractable>();
            Rigidbody rigidBody = interactableGO.AddComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
            return interactable;
        }

        internal static XRSimpleInteractable CreateTriggerInteractable()
        {
            GameObject interactableGO = new GameObject("Trigger Interactable");
            var collider = CreateGOSphereCollider(interactableGO, false);
            XRSimpleInteractable interactable = interactableGO.AddComponent<XRSimpleInteractable>();
            Rigidbody rigidBody = interactableGO.AddComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
            collider.isTrigger = true;      // We set the trigger here, rather than using the function argument so that the collider gets added to the interactable's list of colliders
            return interactable;
        }

        internal static XRInteractableSnapVolume CreateSnapVolume()
        {
            GameObject snapVolumeGO = new GameObject("Snap Volume");
            CreateGOBoxCollider(snapVolumeGO);
            var boxCollider = snapVolumeGO.GetComponent<BoxCollider>();
            XRInteractableSnapVolume snapVolume = snapVolumeGO.AddComponent<XRInteractableSnapVolume>();
            snapVolume.snapCollider = boxCollider;
            return snapVolume;
        }

        internal static GameObject CreateUICanvas(Camera worldCamera)
        {
            var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(TrackedDeviceGraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.worldCamera = worldCamera;
            canvas.renderMode = RenderMode.WorldSpace;

            // Set up a GameObject hierarchy that we send events to. In a real setup,
            // this would be a hierarchy involving UI components.
            var parentGameObject = new GameObject("Parent");
            var parentTransform = parentGameObject.AddComponent<RectTransform>();

            var leftChildGameObject = new GameObject("Left Child");
            var leftChildTransform = leftChildGameObject.AddComponent<RectTransform>();
            leftChildGameObject.AddComponent<Image>();
            leftChildGameObject.AddComponent<Selectable>();

            var rightChildGameObject = new GameObject("Right Child");
            var rightChildTransform = rightChildGameObject.AddComponent<RectTransform>();
            rightChildGameObject.AddComponent<Image>();
            rightChildGameObject.AddComponent<Selectable>();

            parentTransform.SetParent(canvas.transform, false);
            leftChildTransform.SetParent(parentTransform, false);
            rightChildTransform.SetParent(parentTransform, false);

            // Parent occupies full space of canvas.
            parentTransform.sizeDelta = new Vector2(640, 480);

            // Left child occupies left half of parent.
            const int quarterSize = 640 / 4;
            leftChildTransform.anchoredPosition = new Vector2(-quarterSize, 0);
            leftChildTransform.sizeDelta = new Vector2(320, 480);

            // Right child occupies right half of parent.
            rightChildTransform.anchoredPosition = new Vector2(quarterSize, 0);
            rightChildTransform.sizeDelta = new Vector2(320, 480);

            return canvasGo;
        }

        /// <summary>
        /// Creates an EventSystem with an XR UI Input Module that only processes XR input.
        /// Call before creating interactors, otherwise they create their own EventSystem and use that one instead.
        /// </summary>
        internal static XRUIInputModule CreateXRUIInputModule()
        {
            var inputModule = new GameObject("EventSystem", typeof(EventSystem), typeof(XRUIInputModule)).GetComponent<XRUIInputModule>();
            inputModule.enableXRInput = true;
            inputModule.enableMouseInput = false;
            inputModule.enableTouchInput = false;
            inputModule.enableGamepadInput = false;
            inputModule.enableJoystickInput = false;
            return inputModule;
        }

        /// <summary>
        /// Records the uGUI scroll events it receives.
        /// </summary>
        internal class ScrollRecorder : MonoBehaviour, IScrollHandler
        {
            public readonly List<Vector2> deltas = new List<Vector2>();

            public Vector2 total
            {
                get
                {
                    var sum = Vector2.zero;
                    foreach (var delta in deltas)
                    {
                        sum += delta;
                    }

                    return sum;
                }
            }

            public void OnScroll(PointerEventData eventData)
            {
                deltas.Add(eventData.scrollDelta);
            }
        }

        /// <summary>
        /// Creates a world space canvas 2.5 meters in front of the given transform, filled by a <see cref="ScrollRect"/>
        /// that records the scroll events it receives.
        /// </summary>
        internal static ScrollRecorder CreateScrollRectCanvas(Camera worldCamera, Transform facing, float decelerationRate)
        {
            var canvas = CreateUICanvas(worldCamera);
            canvas.transform.position = facing.position + facing.forward * 2.5f;
            var scrollTarget = canvas.transform.Find("Parent").gameObject;
            var scrollRect = scrollTarget.AddComponent<ScrollRect>();
            scrollRect.content = (RectTransform)scrollTarget.transform.Find("Left Child");
            scrollRect.decelerationRate = decelerationRate;
            return scrollTarget.AddComponent<ScrollRecorder>();
        }

        static readonly Vector3 k_OutOfReachPosition = new Vector3(0f, -10f, 0f);

        /// <summary>
        /// Waits <paramref name="count"/> frames.
        /// </summary>
        internal static IEnumerator WaitFrames(int count)
        {
            for (var i = 0; i < count; ++i)
                yield return null;
        }

        /// <summary>
        /// Switches the readers to manual value mode.
        /// </summary>
        internal static void SetManualInput(params XRInputButtonReader[] readers)
        {
            foreach (var reader in readers)
            {
                reader.inputSourceMode = XRInputButtonReader.InputSourceMode.ManualValue;
            }
        }

        /// <summary>
        /// Moves an interactor 10 meters below the origin, pointing down, so it hits nothing in the test scene.
        /// </summary>
        internal static void ParkOutOfReach(Transform interactor)
        {
            interactor.position = k_OutOfReachPosition;
            interactor.rotation = Quaternion.LookRotation(Vector3.down);
        }

#if UIELEMENTS_MODULE_PRESENT && UNITY_6000_2_OR_NEWER
        /// <summary>
        /// Creates the XR UI Toolkit Manager and the Panel Input Configuration that world space UI Toolkit input needs,
        /// as in the World Space UI sample. Create them before interactors so the interactors register with them.
        /// </summary>
        internal static void CreateUIToolkitInputSetup()
        {
            new GameObject("XR UI Toolkit Manager").AddComponent<XRUIToolkitManager>();
            var configuration = new GameObject("Panel Input Configuration").AddComponent<UIElements.PanelInputConfiguration>();
            configuration.panelInputRedirection = UIElements.PanelInputConfiguration.PanelInputRedirection.Never;
        }

        // Restored by RestoreUIToolkitTrackedInput in each test's TearDown, so they do not need automatic cleanup.
#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        static object s_PreviousEditorRemoteConnected;
#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        static bool s_ForcedEditorRemoteConnected;

        /// <summary>
        /// Makes UI Toolkit process the tracked device pointer input that interactors send, without an XR device.
        /// Call <see cref="RestoreUIToolkitTrackedInput"/> in TearDown.
        /// </summary>
        /// <remarks>
        /// In the Editor, UI Toolkit only processes tracked device input while <c>DefaultEventSystem.IsEditorRemoteConnected</c>
        /// returns <see langword="true"/>. XRI sets it only while an XR device is active (see <see cref="XRUIToolkitHandler.UpdateEventSystem"/>),
        /// so tests force it. Without this, batch mode runs never process the input, and the pointer events stay queued.
        /// <c>DefaultEventSystem</c> is internal to UnityEngine, so this goes through reflection.
        /// </remarks>
        internal static void ForceUIToolkitTrackedInput()
        {
            var field = GetEditorRemoteConnectedField();
            Assert.That(field, Is.Not.Null, "Could not find DefaultEventSystem.IsEditorRemoteConnected to route tracked device input to UI Toolkit.");

            if (!s_ForcedEditorRemoteConnected)
                s_PreviousEditorRemoteConnected = field.GetValue(null);

            Func<bool> alwaysConnected = () => true;
            field.SetValue(null, alwaysConnected);
            s_ForcedEditorRemoteConnected = true;
        }

        /// <summary>
        /// Restores the value <see cref="ForceUIToolkitTrackedInput"/> replaced. Does nothing if it was not forced.
        /// </summary>
        internal static void RestoreUIToolkitTrackedInput()
        {
            if (!s_ForcedEditorRemoteConnected)
                return;

            GetEditorRemoteConnectedField()?.SetValue(null, s_PreviousEditorRemoteConnected);
            s_ForcedEditorRemoteConnected = false;
            s_PreviousEditorRemoteConnected = null;
        }

        // A static field in Unity 6000.2 through 7000.0.
        static FieldInfo GetEditorRemoteConnectedField()
        {
            var type = typeof(UIElements.VisualElement).Assembly.GetType("UnityEngine.UIElements.DefaultEventSystem");
            return type?.GetField("IsEditorRemoteConnected", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }

        // UnityEngine.UIElements.WorldSpaceInput is internal to UnityEngine and is not visible to this
        // test assembly (only to the XRI runtime), so it is reached through reflection.
#if UNITY_6000_5_OR_NEWER
        [NoAutoStaticsCleanup]
#endif
        static readonly MethodInfo s_LocalPointToGameObjectWorldSpace =
            typeof(UIElements.VisualElement).Assembly.GetType("UnityEngine.UIElements.WorldSpaceInput")?
                .GetMethod("LocalPointToGameObjectWorldSpace", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                    null, new[] { typeof(UIElements.VisualElement), typeof(Vector3) }, null);

        /// <summary>
        /// Returns the world position of the center of a laid out element on a world space panel.
        /// </summary>
        internal static Vector3 ElementCenterWorld(UIElements.VisualElement element)
        {
            Assert.That(s_LocalPointToGameObjectWorldSpace, Is.Not.Null, "WorldSpaceInput.LocalPointToGameObjectWorldSpace not found.");
            var layout = element.layout;
            return (Vector3)s_LocalPointToGameObjectWorldSpace.Invoke(null, new object[] { element, new Vector3(layout.width * 0.5f, layout.height * 0.5f, 0f) });
        }

        /// <summary>
        /// Places an interactor 0.6 m in front of a world space panel and turns it so its cast origin points at the
        /// center of <paramref name="element"/>, then waits a frame for the interactor to hover it.
        /// </summary>
        /// <param name="interactor">The interactor root to move.</param>
        /// <param name="castOrigin">Returns the transform the interactor casts from, which can be offset from its root.</param>
        /// <param name="panel">The transform of the GameObject that hosts the panel.</param>
        /// <param name="element">The element to aim at.</param>
        internal static IEnumerator AimAtElement(Transform interactor, Func<Transform> castOrigin, Transform panel, UIElements.VisualElement element)
        {
            var target = ElementCenterWorld(element);
            interactor.position = target - panel.forward * 0.6f;
            interactor.rotation = Quaternion.LookRotation(target - interactor.position, Vector3.up);
            yield return null;

            // Correct for a cast origin that is offset from the interactor root.
            var origin = castOrigin();
            if (origin != null && origin != interactor)
                interactor.rotation = Quaternion.FromToRotation(origin.forward, (target - origin.position).normalized) * interactor.rotation;

            yield return null;
        }

        /// <summary>
        /// Pokes the center of <paramref name="element"/> slowly, pulls back, then parks the interactor out of reach.
        /// </summary>
        /// <param name="poke">The poke interactor.</param>
        /// <param name="panel">The transform of the GameObject that hosts the panel.</param>
        /// <param name="element">The element to poke.</param>
        internal static IEnumerator PokeTestElement(XRPokeInteractor poke, Transform panel, UIElements.VisualElement element)
        {
            var target = ElementCenterWorld(element);
            var forward = panel.forward;
            var root = poke.transform;
            var attach = poke.GetAttachTransform(null);

            void PlaceAttach(Vector3 point) => root.position += point - attach.position;

            // Slow press, about 1 mm per frame, that stops just past the surface. A fast push can hover
            // without selecting on the zero depth MatchBoundingBox collider.
            var start = target - forward * 0.05f;
            var end = target + forward * 0.005f;
            PlaceAttach(start);
            yield return WaitFrames(5);

            const int steps = 60;
            for (var i = 1; i <= steps; ++i)
            {
                PlaceAttach(Vector3.Lerp(start, end, i / (float)steps));
                yield return null;
            }

            yield return WaitFrames(10);

            for (var i = 1; i <= steps; ++i)
            {
                PlaceAttach(Vector3.Lerp(end, start, i / (float)steps));
                yield return null;
            }

            yield return WaitFrames(10);
            ParkOutOfReach(root);
            yield return WaitFrames(5);
        }

        // A panel can only be made world space from code in Unity 6.3 or newer.
#if UNITY_6000_3_OR_NEWER
        /// <summary>
        /// Creates a world space UI Toolkit panel 2 meters in front of the origin, filled by a tall ScrollView.
        /// Destroy <paramref name="panelSettings"/> after the test.
        /// </summary>
        internal static UIElements.ScrollView CreateWorldSpaceScrollView(out UIElements.PanelSettings panelSettings)
        {
            const float panelPixels = 400f;
            const float contentHeight = 4000f;

            panelSettings = ScriptableObject.CreateInstance<UIElements.PanelSettings>();
            panelSettings.renderMode = UIElements.PanelRenderMode.WorldSpace;

            var documentObject = new GameObject("UI Document");
            documentObject.transform.position = new Vector3(0f, 0f, 2f);
            var document = documentObject.AddComponent<UIElements.UIDocument>();
            document.panelSettings = panelSettings;
            document.worldSpaceSize = new Vector2(panelPixels, panelPixels);

            var scrollView = new UIElements.ScrollView(UIElements.ScrollViewMode.Vertical);
            var content = new UIElements.VisualElement();
            content.style.height = contentHeight;
            content.style.flexShrink = 0f;
            scrollView.Add(content);
            document.rootVisualElement.Add(scrollView);
            return scrollView;
        }
#endif
#endif

        internal static XRSimpleInteractable CreateMultiSelectableSimpleInteractable()
        {
            var interactable = CreateSimpleInteractable();
            interactable.selectMode = InteractableSelectMode.Multiple;
            return interactable;
        }

        internal static XRSimpleInteractable CreateSimpleInteractableWithColliders()
        {
            GameObject interactableGO = new GameObject("Simple Interactable with Colliders");
            CreateGOSphereCollider(interactableGO, false);
            CreateGOBoxCollider(interactableGO, false);
            XRSimpleInteractable interactable = interactableGO.AddComponent<XRSimpleInteractable>();
            Rigidbody rigidBody = interactableGO.AddComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
            return interactable;
        }

        internal static XRControllerRecorder CreateControllerRecorder(XRBaseInputInteractor interactor, Action<XRControllerRecording> addRecordingFrames)
        {
            var controllerRecorder = interactor.gameObject.AddComponent<XRControllerRecorder>();
            controllerRecorder.SetInteractor(interactor);
            controllerRecorder.recording = ScriptableObject.CreateInstance<XRControllerRecording>();
            addRecordingFrames(controllerRecorder.recording);
            controllerRecorder.recording.SetFrameDependentData();
            return controllerRecorder;
        }

        internal static XRTargetFilter CreateTargetFilter()
        {
            GameObject filterGO = new GameObject("Target Filter");
            return filterGO.AddComponent<XRTargetFilter>();
        }

        internal static XRInteractionGroup CreateInteractionGroup()
        {
            var groupGO = new GameObject("Interaction Group");
            groupGO.transform.localPosition = Vector3.zero;
            groupGO.transform.localRotation = Quaternion.identity;
            var group = groupGO.AddComponent<XRInteractionGroup>();
            return group;
        }

        internal static XRInteractionGroup CreateGroupWithMockInteractors(out MockInteractor memberInteractor1,
            out MockInteractor memberInteractor2, out MockInteractor memberInteractor3)
        {
            var group = CreateInteractionGroup();
            memberInteractor1 = CreateMockInteractor();
            memberInteractor2 = CreateMockInteractor();
            memberInteractor3 = CreateMockInteractor();
            group.AddGroupMember(memberInteractor1);
            group.AddGroupMember(memberInteractor2);
            group.AddGroupMember(memberInteractor3);
            return group;
        }

        internal static XRInteractionGroup CreateGroupWithHoverOnlyMockInteractors(out MockInteractor memberInteractor1,
            out MockInteractor memberInteractor2, out MockInteractor memberInteractor3)
        {
            var group = CreateInteractionGroup();
            memberInteractor1 = CreateMockInteractor();
            memberInteractor2 = CreateMockInteractor();
            memberInteractor3 = CreateMockInteractor();
            memberInteractor1.allowSelect = false;
            memberInteractor2.allowSelect = false;
            memberInteractor3.allowSelect = false;
            group.AddGroupMember(memberInteractor1);
            group.AddGroupMember(memberInteractor2);
            group.AddGroupMember(memberInteractor3);
            return group;
        }

        internal static XRInteractionGroup CreateGroupWithEmptyGroups(out XRInteractionGroup memberGroup1,
            out XRInteractionGroup memberGroup2, out XRInteractionGroup memberGroup3)
        {
            var group = CreateInteractionGroup();
            memberGroup1 = CreateInteractionGroup();
            memberGroup2 = CreateInteractionGroup();
            memberGroup3 = CreateInteractionGroup();
            group.AddGroupMember(memberGroup1);
            group.AddGroupMember(memberGroup2);
            group.AddGroupMember(memberGroup3);
            return group;
        }

        internal static XRBodyTransformer CreateXRBodyTransformer(XROrigin xrOrigin)
        {
            var xrBodyTransformerGO = new GameObject("XR Body Transformer");
            var xrBodyTransformer = xrBodyTransformerGO.AddComponent<XRBodyTransformer>();
            xrBodyTransformer.xrOrigin = xrOrigin;
            return xrBodyTransformer;
        }

        internal static LocomotionMediator CreateLocomotionMediatorWithXROrigin()
        {
            var xrOrigin = CreateXROrigin();
            var mediator = xrOrigin.gameObject.AddComponent<LocomotionMediator>();
            return mediator;
        }

        internal static MockLocomotionProvider CreateMockLocomotionProvider(LocomotionMediator mediator)
        {
            var provider = new GameObject("Mock Locomotion Provider").AddComponent<MockLocomotionProvider>();
            provider.mediator = mediator;
            return provider;
        }

        internal static GameObject CreateCubeGameObject()
        {
            var cube = new GameObject("Cube");
            CreateGOBoxCollider(cube, false);
            return cube;
        }

        internal static void DisableAllInputSystemActions()
        {
            InputSystem.InputSystem.DisableAllEnabledActions();

            if (InputSystem.InputSystem.actions != null)
            {
                foreach (var map in InputSystem.InputSystem.actions.actionMaps)
                {
                    map.Disable();
                }
            }
        }
    }

    class MockInteractor : XRBaseInteractor
    {
        public event Action<XRInteractionUpdateOrder.UpdatePhase> preprocessed;
        public event Action<XRInteractionUpdateOrder.UpdatePhase> processed;
        public List<IXRInteractable> validTargets { get; } = new List<IXRInteractable>();

        /// <inheritdoc />
        public override void PreprocessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.PreprocessInteractor(updatePhase);
            preprocessed?.Invoke(updatePhase);
        }

        /// <inheritdoc />
        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractor(updatePhase);
            processed?.Invoke(updatePhase);
        }

        /// <inheritdoc />
        public override void GetValidTargets(List<IXRInteractable> targets)
        {
            targets.Clear();
            targets.AddRange(validTargets);
        }
    }

    class MockInputInteractor : XRBaseInputInteractor
    {
        public event Action<XRInteractionUpdateOrder.UpdatePhase> preprocessed;
        public event Action<XRInteractionUpdateOrder.UpdatePhase> processed;
        public List<IXRInteractable> validTargets { get; } = new List<IXRInteractable>();

        /// <inheritdoc />
        public override void PreprocessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.PreprocessInteractor(updatePhase);
            preprocessed?.Invoke(updatePhase);
        }

        /// <inheritdoc />
        public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractor(updatePhase);
            processed?.Invoke(updatePhase);
        }

        /// <inheritdoc />
        public override void GetValidTargets(List<IXRInteractable> targets)
        {
            targets.Clear();
            targets.AddRange(validTargets);
        }
    }

    class MockMovementTypeOverrideInteractor : MockInteractor
    {
        public XRBaseInteractable.MovementType? mockMovementTypeOverride { get; set; }

        /// <inheritdoc />
        public override XRBaseInteractable.MovementType? selectedInteractableMovementTypeOverride => mockMovementTypeOverride;
    }

    /// <summary>
    /// An interactor that is a plain C# object that uses a given GameObject.
    /// </summary>
    class MockClassInteractor : IXRInteractor
    {
        public event Action<XRInteractionUpdateOrder.UpdatePhase> preprocessed;
        public event Action<XRInteractionUpdateOrder.UpdatePhase> processed;
        public List<IXRInteractable> validTargets { get; } = new List<IXRInteractable>();

        /// <inheritdoc />
        public event Action<InteractorRegisteredEventArgs> registered;

        /// <inheritdoc />
        public event Action<InteractorUnregisteredEventArgs> unregistered;

        /// <inheritdoc />
        public InteractionLayerMask interactionLayers { get; set; } = -1;

        /// <inheritdoc />
        public InteractorHandedness handedness { get; set; }

        /// <inheritdoc />
        public Transform transform { get; }

        /// <summary>
        /// Whether this interactor is registered;
        /// </summary>
        public bool isRegistered { get; private set; }

        /// <summary>
        /// When this interactor is registered, the interaction manager that this interactor was registered with.
        /// </summary>
        public XRInteractionManager registeredInteractionManager { get; private set; }

        /// <summary>
        /// Constructs a new interactor.
        /// </summary>
        /// <param name="transform">The <see cref="Transform"/> associated with the Interactor.</param>
        public MockClassInteractor(Transform transform)
        {
            this.transform = transform;
        }

        /// <inheritdoc />
        public Transform GetAttachTransform(IXRInteractable interactable)
        {
            return transform;
        }

        /// <inheritdoc />
        public void GetValidTargets(List<IXRInteractable> targets)
        {
            targets.Clear();
            targets.AddRange(validTargets);
        }

        /// <inheritdoc />
        public void OnRegistered(InteractorRegisteredEventArgs args)
        {
            isRegistered = true;
            registeredInteractionManager = args.manager;
            registered?.Invoke(args);
        }

        /// <inheritdoc />
        public void OnUnregistered(InteractorUnregisteredEventArgs args)
        {
            isRegistered = false;
            registeredInteractionManager = null;
            unregistered?.Invoke(args);
        }

        /// <inheritdoc />
        public void PreprocessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            preprocessed?.Invoke(updatePhase);
        }

        /// <inheritdoc />
        public void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            processed?.Invoke(updatePhase);
        }
    }

    class MockClassUIInteractor : MockClassInteractor, IUIInteractorRegistrationHandler
    {
        public delegate void ProcessUIModel(ref TrackedDeviceModel model);

        /// <summary>
        /// External implementation of the <see cref="UpdateUIModel"/> method.
        /// </summary>
        public ProcessUIModel processUIModel { get; set; }

        /// <summary>
        /// When this interactor is registered, the XR UI Input Module that this interactor was registered with.
        /// </summary>
        public XRUIInputModule registeredInputModule { get; private set; }

        /// <inheritdoc />
        public MockClassUIInteractor(Transform transform)
            : base(transform)
        {
        }

        /// <inheritdoc />
        public void UpdateUIModel(ref TrackedDeviceModel model)
        {
            processUIModel?.Invoke(ref model);
        }

        /// <inheritdoc />
        public bool TryGetUIModel(out TrackedDeviceModel model)
        {
            if (registeredInputModule != null)
                return registeredInputModule.GetTrackedDeviceModel(this, out model);

            model = TrackedDeviceModel.invalid;
            return false;
        }

        /// <inheritdoc />
        public void OnRegistered(UIInteractorRegisteredEventArgs args)
        {
            registeredInputModule = args.inputModule;
        }

        /// <inheritdoc />
        public void OnUnregistered(UIInteractorUnregisteredEventArgs args)
        {
            registeredInputModule = null;
        }
    }

    /// <summary>
    /// An interactable that is a plain C# object that uses a given GameObject.
    /// </summary>
    class MockClassInteractable : IXRInteractable
    {
        /// <inheritdoc />
        public event Action<InteractableRegisteredEventArgs> registered;

        /// <inheritdoc />
        public event Action<InteractableUnregisteredEventArgs> unregistered;

        /// <summary>
        /// Invoked when <see cref="ProcessInteractable"/> is called.
        /// </summary>
        public event Action<XRInteractionUpdateOrder.UpdatePhase> processed;

        /// <inheritdoc />
        public InteractionLayerMask interactionLayers { get; set; } = 1;

        /// <inheritdoc />
        public List<Collider> colliders { get; } = new List<Collider>();

        /// <inheritdoc />
        public Transform transform { get; }

        /// <summary>
        /// Whether this interactable is registered;
        /// </summary>
        public bool isRegistered { get; private set; }

        /// <summary>
        /// When this interactable is registered, the interaction manager that this interactable was registered with.
        /// </summary>
        public XRInteractionManager registeredInteractionManager { get; private set; }

        /// <summary>
        /// Constructs a new interactable. Populates <see cref="colliders"/> with non-trigger colliders.
        /// </summary>
        /// <param name="transform">The <see cref="Transform"/> associated with the Interactable.</param>
        public MockClassInteractable(Transform transform)
        {
            this.transform = transform;
            transform.GetComponentsInChildren(colliders);
            colliders.RemoveAll(col => col.isTrigger);
        }

        /// <inheritdoc />
        public Transform GetAttachTransform(IXRInteractor interactor)
        {
            return transform;
        }

        /// <inheritdoc />
        public void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            processed?.Invoke(updatePhase);
        }

        /// <inheritdoc />
        public float GetDistanceSqrToInteractor(IXRInteractor interactor)
        {
            var interactorAttachTransform = interactor?.GetAttachTransform(this);
            if (interactorAttachTransform == null)
                return float.MaxValue;

            return (transform.position - interactorAttachTransform.position).sqrMagnitude;
        }

        /// <inheritdoc />
        public void OnRegistered(InteractableRegisteredEventArgs args)
        {
            isRegistered = true;
            registeredInteractionManager = args.manager;
            registered?.Invoke(args);
        }

        /// <inheritdoc />
        public void OnUnregistered(InteractableUnregisteredEventArgs args)
        {
            isRegistered = false;
            registeredInteractionManager = null;
            unregistered?.Invoke(args);
        }
    }

    enum TargetFilterCallback
    {
        Link,
        Unlink,
        Process,
    }

    class MockTargetFilter : IXRTargetFilter
    {
        public readonly List<TargetFilterCallback> callbackExecution = new List<TargetFilterCallback>();

        public bool canProcess { get; set; } = true;

        public void Link(IXRInteractor interactor)
        {
            callbackExecution.Add(TargetFilterCallback.Link);
        }

        public void Unlink(IXRInteractor interactor)
        {
            callbackExecution.Add(TargetFilterCallback.Unlink);
        }

        public void Process(IXRInteractor interactor, List<IXRInteractable> targets, List<IXRInteractable> results)
        {
            results.Clear();
            callbackExecution.Add(TargetFilterCallback.Process);
            results.AddRange(targets);
        }
    }

    class MockInversionTargetFilter : IXRTargetFilter
    {
        public readonly List<TargetFilterCallback> callbackExecution = new List<TargetFilterCallback>();

        public bool canProcess { get; set; } = true;

        public void Link(IXRInteractor interactor)
        {
            callbackExecution.Add(TargetFilterCallback.Link);
        }

        public void Unlink(IXRInteractor interactor)
        {
            callbackExecution.Add(TargetFilterCallback.Unlink);
        }

        public void Process(IXRInteractor interactor, List<IXRInteractable> targets, List<IXRInteractable> results)
        {
            results.Clear();
            callbackExecution.Add(TargetFilterCallback.Process);
            for (int i = targets.Count - 1; i >= 0; i--)
            {
                results.Add(targets[i]);
            }
        }
    }

    class MockBlockSelectionTargetFilter : IXRTargetFilter
    {
        public readonly List<TargetFilterCallback> callbackExecution = new List<TargetFilterCallback>();

        public bool canProcess { get; set; } = true;

        public void Link(IXRInteractor interactor)
        {
            callbackExecution.Add(TargetFilterCallback.Link);
        }

        public void Unlink(IXRInteractor interactor)
        {
            callbackExecution.Add(TargetFilterCallback.Unlink);
        }

        public void Process(IXRInteractor interactor, List<IXRInteractable> targets, List<IXRInteractable> results)
        {
            results.Clear();
            callbackExecution.Add(TargetFilterCallback.Process);
        }
    }

    class MockGrabTransformer : IXRGrabTransformer
    {
        public enum MethodTrace
        {
            OnLink,
            OnGrab,
            OnGrabCountChanged,
            OnDrop,
            ProcessFixed,
            ProcessDynamic,
            ProcessLate,
            ProcessOnBeforeRender,
            OnUnlink,
        }

        public List<MethodTrace> methodTraces { get; } = new List<MethodTrace>();

        public Dictionary<XRInteractionUpdateOrder.UpdatePhase, bool> phasesTraced { get; } = new Dictionary<XRInteractionUpdateOrder.UpdatePhase, bool>
        {
            { XRInteractionUpdateOrder.UpdatePhase.Fixed, false},
            { XRInteractionUpdateOrder.UpdatePhase.Dynamic, true},
            { XRInteractionUpdateOrder.UpdatePhase.Late, false},
            { XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender, false},
        };

        /// <summary>
        /// The <see cref="Pose"/> value to set in <see cref="Process"/>.
        /// Set to <see langword="null"/> to skip using it.
        /// </summary>
        public Pose? targetPoseValue { get; set; }

        /// <summary>
        /// The <see cref="Vector3"/> local scale value to set in <see cref="Process"/>.
        /// Set to <see langword="null"/> to skip using it.
        /// </summary>
        public Vector3? localScaleValue { get; set; }

        /// <inheritdoc />
        public bool canProcess { get; set; } = true;

        /// <inheritdoc />
        public void OnLink(XRGrabInteractable grabInteractable)
        {
            methodTraces.Add(MethodTrace.OnLink);
        }

        /// <inheritdoc />
        public void OnGrab(XRGrabInteractable grabInteractable)
        {
            methodTraces.Add(MethodTrace.OnGrab);
        }

        /// <inheritdoc />
        public void OnGrabCountChanged(XRGrabInteractable grabInteractable, Pose targetPose, Vector3 localScale)
        {
            methodTraces.Add(MethodTrace.OnGrabCountChanged);
        }

        /// <inheritdoc />
        public void Process(XRGrabInteractable grabInteractable, XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale)
        {
            switch (updatePhase)
            {
                case XRInteractionUpdateOrder.UpdatePhase.Fixed:
                    if (phasesTraced[updatePhase])
                        methodTraces.Add(MethodTrace.ProcessFixed);
                    break;
                case XRInteractionUpdateOrder.UpdatePhase.Dynamic:
                    if (phasesTraced[updatePhase])
                        methodTraces.Add(MethodTrace.ProcessDynamic);
                    break;
                case XRInteractionUpdateOrder.UpdatePhase.Late:
                    if (phasesTraced[updatePhase])
                        methodTraces.Add(MethodTrace.ProcessLate);
                    break;
                case XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender:
                    if (phasesTraced[updatePhase])
                        methodTraces.Add(MethodTrace.ProcessOnBeforeRender);
                    break;
                default:
                    Assert.Fail($"Unhandled {nameof(XRInteractionUpdateOrder.UpdatePhase)}={updatePhase}");
                    break;
            }

            if (targetPoseValue.HasValue)
                targetPose = targetPoseValue.Value;

            if (localScaleValue.HasValue)
                localScale = localScaleValue.Value;
        }

        /// <inheritdoc />
        public void OnUnlink(XRGrabInteractable grabInteractable)
        {
            methodTraces.Add(MethodTrace.OnUnlink);
        }
    }

    class MockDropTransformer : MockGrabTransformer, IXRDropTransformer
    {
        /// <inheritdoc />
        public bool canProcessOnDrop { get; set; } = true;

        /// <inheritdoc />
        public void OnDrop(XRGrabInteractable grabInteractable, DropEventArgs args)
        {
            Assert.That(args, Is.Not.Null);
            Assert.That(args.selectExitEventArgs, Is.Not.Null);
            Assert.That(args.selectExitEventArgs.interactableObject, Is.SameAs(grabInteractable));
            Assert.That(args.selectExitEventArgs.manager, Is.SameAs(grabInteractable.interactionManager));

            methodTraces.Add(MethodTrace.OnDrop);
        }
    }

    // TODO: This is a placeholder until we can get mock touches to work with XRUIInputModule. See ARTests.
    class MockInputModule : BaseInputModule
    {
        public override void Process()
        {
        }

        public override bool IsPointerOverGameObject(int pointerId) => true;
    }

    class MockLocomotionProvider : LocomotionProvider
    {
        /// <inheritdoc />
        public override bool canStartMoving => m_FinishedPreparation;

        public DelegateXRBodyTransformation delegateTransformation { get; } = new DelegateXRBodyTransformation();

        bool m_FinishedPreparation;

        public bool InvokeTryPrepareLocomotion()
        {
            m_FinishedPreparation = false;
            return TryPrepareLocomotion();
        }

        public void FinishPreparation()
        {
            m_FinishedPreparation = true;
        }

        public bool InvokeTryStartLocomotionImmediately()
        {
            return TryStartLocomotionImmediately();
        }

        public bool InvokeTryEndLocomotion()
        {
            return TryEndLocomotion();
        }

        public bool InvokeTryQueueTransformation()
        {
            return TryQueueTransformation(delegateTransformation);
        }
    }
}
