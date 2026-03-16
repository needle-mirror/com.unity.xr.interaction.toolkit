using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using static UnityEngine.XR.Interaction.Toolkit.Tests.UIPointerTests;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    [TestFixture]
    class UIInputModuleTests
    {
        /// <summary>
        /// Runtime behavior types that can appear in the Inspector that implement <see cref="IUIInteractor"/>.
        /// </summary>
        static readonly Type[] s_RuntimeBehaviorTypes;

        static readonly XRInteractionRuntimeSettings.ManagerCreationMode[] s_CreationModes =
        {
            XRInteractionRuntimeSettings.ManagerCreationMode.CreateAutomatically,
            XRInteractionRuntimeSettings.ManagerCreationMode.Manual,
        };

        static readonly XRInteractionRuntimeSettings.ManagerRegistrationMode[] s_RegistrationModes =
        {
            XRInteractionRuntimeSettings.ManagerRegistrationMode.FindAutomatically,
            XRInteractionRuntimeSettings.ManagerRegistrationMode.Manual,
        };

        static UIInputModuleTests()
        {
            var assembly = Assembly.Load("Unity.XR.Interaction.Toolkit");
            if (assembly == null)
                return;

            s_RuntimeBehaviorTypes = assembly.GetExportedTypes()
                .Where(type => type.IsSubclassOf(typeof(MonoBehaviour)) && !type.IsAbstract && typeof(IUIInteractor).IsAssignableFrom(type))
                .OrderBy(type => type.FullName)
                .ToArray();
        }

        [TearDown]
        public void TearDown()
        {
            TestUtilities.ResetXRIRuntimeSettings();
            TestUtilities.DestroyAllSceneObjects();
        }

        [Test]
        public void InteractorRegisteredOnEnable([ValueSource(nameof(s_RuntimeBehaviorTypes))] Type type)
        {
            Assume.That(ComponentLocatorUtility<XRUIInputModule>.componentCache, Is.Null);

            var eventSystemGo = new GameObject("EventSystem", typeof(TestEventSystem), typeof(XRUIInputModule));
            var inputModule = eventSystemGo.GetComponent<XRUIInputModule>();

            IUIInteractor registeredInteractor = null;
            inputModule.interactorRegistered += args => registeredInteractor = args.interactor;

            var interactor = CreateUIInteractor(type);

            var interactors = new List<IUIInteractor>();
            inputModule.GetRegisteredInteractors(interactors);
            Assert.That(interactors, Is.EqualTo(new[] { interactor }));
            Assert.That(registeredInteractor, Is.SameAs(interactor));
        }

        [Test]
        public void InteractorUnregisteredOnDisable([ValueSource(nameof(s_RuntimeBehaviorTypes))] Type type)
        {
            Assume.That(ComponentLocatorUtility<XRUIInputModule>.componentCache, Is.Null);

            var eventSystemGo = new GameObject("EventSystem", typeof(TestEventSystem), typeof(XRUIInputModule));
            var inputModule = eventSystemGo.GetComponent<XRUIInputModule>();

            var interactor = CreateUIInteractor(type);

            var interactors = new List<IUIInteractor>();
            inputModule.GetRegisteredInteractors(interactors);
            Assert.That(interactors, Is.EqualTo(new[] { interactor }));

            IUIInteractor unregisteredInteractor = null;
            inputModule.interactorUnregistered += args => unregisteredInteractor = args.interactor;

            Assert.That(interactor, Is.InstanceOf<Behaviour>());
            ((Behaviour)interactor).enabled = false;

            inputModule.GetRegisteredInteractors(interactors);
            Assert.That(interactors, Is.Empty);
            Assert.That(unregisteredInteractor, Is.SameAs(interactor));
        }

        [UnityTest]
        public IEnumerator InteractorUnregisteredOnModuleDestroyed()
        {
            Assume.That(ComponentLocatorUtility<XRUIInputModule>.componentCache, Is.Null);

            var eventSystemGo = new GameObject("EventSystem", typeof(TestEventSystem), typeof(XRUIInputModule));
            var inputModule = eventSystemGo.GetComponent<XRUIInputModule>();

            var interactor = TestUtilities.CreateMockClassUIInteractor();
            inputModule.RegisterInteractor(interactor);

            var interactors = new List<IUIInteractor>();
            inputModule.GetRegisteredInteractors(interactors);
            Assert.That(interactors, Is.EqualTo(new[] { interactor }));
            Assert.That(interactor.registeredInputModule, Is.SameAs(inputModule));

            Object.Destroy(inputModule);

            yield return null;

            Assert.That(inputModule == null, Is.True);

            inputModule.GetRegisteredInteractors(interactors);
            Assert.That(interactors, Is.Empty);
            Assert.That(interactor.registeredInputModule, Is.Null);
        }

        [Test]
        public void InteractorAddsMissingModuleToEventSystemOnEnable([ValueSource(nameof(s_RuntimeBehaviorTypes))] Type type)
        {
            Assume.That(ComponentLocatorUtility<XRUIInputModule>.componentCache, Is.Null);

            var eventSystemGo = new GameObject("EventSystem", typeof(TestEventSystem));
            var eventSystem = eventSystemGo.GetComponent<EventSystem>();

            var interactor = CreateUIInteractor(type);

            Assert.That(ComponentLocatorUtility<XRUIInputModule>.componentCache, Is.Not.Null);
            Assert.That(EventSystem.current, Is.SameAs(eventSystem));
            var inputModule = EventSystem.current.GetComponent<XRUIInputModule>();
            Assert.That(inputModule != null, Is.True);
            Assert.That(ComponentLocatorUtility<XRUIInputModule>.componentCache, Is.SameAs(inputModule));

            Assert.That(Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None), Has.Length.EqualTo(1));
            Assert.That(Object.FindObjectsByType<XRUIInputModule>(FindObjectsSortMode.None), Has.Length.EqualTo(1));

            var interactors = new List<IUIInteractor>();
            inputModule.GetRegisteredInteractors(interactors);
            Assert.That(interactors, Is.EqualTo(new[] { interactor }));
        }

        [Test]
        public void InteractorCreatesEventSystemAndModuleOnEnable([ValueSource(nameof(s_RuntimeBehaviorTypes))] Type type)
        {
            Assume.That(ComponentLocatorUtility<XRUIInputModule>.componentCache, Is.Null);
            Assume.That(EventSystem.current, Is.Null);

            var interactor = CreateUIInteractor(type);

            Assert.That(ComponentLocatorUtility<XRUIInputModule>.componentCache, Is.Not.Null);
            Assert.That(EventSystem.current != null, Is.True);
            var inputModule = EventSystem.current.GetComponent<XRUIInputModule>();
            Assert.That(inputModule != null, Is.True);
            Assert.That(ComponentLocatorUtility<XRUIInputModule>.componentCache, Is.SameAs(inputModule));

            Assert.That(Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None), Has.Length.EqualTo(1));
            Assert.That(Object.FindObjectsByType<XRUIInputModule>(FindObjectsSortMode.None), Has.Length.EqualTo(1));

            var interactors = new List<IUIInteractor>();
            inputModule.GetRegisteredInteractors(interactors);
            Assert.That(interactors, Is.EqualTo(new[] { interactor }));
        }

        [TestCase(XRInteractionRuntimeSettings.ManagerRegistrationMode.FindAutomatically)]
        [TestCase(XRInteractionRuntimeSettings.ManagerRegistrationMode.Manual)]
        public void CustomInteractorRegisteredFromWaitlist(XRInteractionRuntimeSettings.ManagerRegistrationMode registrationMode)
        {
            // Unlike the test below, this test does not rely on the interactor itself registering with the
            // module during its own OnEnable. This test verifies that an interactor manually
            // added to the waitlist through API is actually registered with a new module.

            Assume.That(XRInteractionRuntimeSettings.InstanceInternal != null, Is.True);
            //Assume.That(XRInteractionManager.activeInteractionManagers, Is.Empty);
            Assume.That(ComponentLocatorUtility<XRUIInputModule>.componentCache, Is.Null);

            XRInteractionRuntimeSettings.InstanceInternal.managerCreationMode = XRInteractionRuntimeSettings.ManagerCreationMode.Manual;
            XRInteractionRuntimeSettings.InstanceInternal.uiModuleRegistrationMode = registrationMode;

            var interactor = TestUtilities.CreateMockClassUIInteractor();

            // The module should not be automatically created from the mock plain C# class-based interactor
            //Assert.That(XRInteractionManager.activeInteractionManagers, Is.Empty);
            //Assert.That(interactor.isRegistered, Is.False);
            Assert.That(EventSystem.current == null, Is.True);
            Assert.That(Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None), Has.Length.EqualTo(0));
            Assert.That(Object.FindObjectsByType<XRUIInputModule>(FindObjectsSortMode.None), Has.Length.EqualTo(0));
            Assert.That(interactor.registeredInputModule, Is.Null);

            // Use API to add the custom interactor to the waitlist
            XRUIInputModule.RegisterWithWaitlist(interactor);

            var interactors = new List<IUIInteractor>();
            var eventSystemGo = new GameObject("EventSystem", typeof(TestEventSystem), typeof(XRUIInputModule));
            var inputModule = eventSystemGo.GetComponent<XRUIInputModule>();

            if (registrationMode == XRInteractionRuntimeSettings.ManagerRegistrationMode.Manual)
            {
                // The manager should not automatically register waitlist items when set to Manual
                inputModule.GetRegisteredInteractors(interactors);

                Assert.That(interactors, Is.Empty);
                Assert.That(interactor.registeredInputModule, Is.Null);

                // Explicitly register waitlist items
                inputModule.RegisterWaitlistInteractors();
            }

            inputModule.GetRegisteredInteractors(interactors);

            Assert.That(interactors, Is.EqualTo(new[] { interactor }));
            Assert.That(interactor.registeredInputModule, Is.SameAs(inputModule));
        }

        [Test]
        public void InteractorRegisteredFromWaitlist(
            [ValueSource(nameof(s_RegistrationModes))] XRInteractionRuntimeSettings.ManagerRegistrationMode registrationMode,
            [ValueSource(nameof(s_RuntimeBehaviorTypes))] Type type)
        {
            Assume.That(XRInteractionRuntimeSettings.InstanceInternal != null, Is.True);
            Assume.That(ComponentLocatorUtility<XRUIInputModule>.componentCache, Is.Null);

            XRInteractionRuntimeSettings.InstanceInternal.managerCreationMode = XRInteractionRuntimeSettings.ManagerCreationMode.Manual;
            XRInteractionRuntimeSettings.InstanceInternal.uiModuleRegistrationMode = registrationMode;

            var interactor = CreateUIInteractor(type);

            // The module should not be automatically created when set to Manual
            Assert.That(EventSystem.current == null, Is.True);
            Assert.That(Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None), Has.Length.EqualTo(0));
            Assert.That(Object.FindObjectsByType<XRUIInputModule>(FindObjectsSortMode.None), Has.Length.EqualTo(0));

            var interactors = new List<IUIInteractor>();
            var eventSystemGo = new GameObject("EventSystem", typeof(TestEventSystem), typeof(XRUIInputModule));
            var inputModule = eventSystemGo.GetComponent<XRUIInputModule>();

            if (registrationMode == XRInteractionRuntimeSettings.ManagerRegistrationMode.Manual)
            {
                // The module should not automatically register waitlist items when set to Manual
                inputModule.GetRegisteredInteractors(interactors);

                Assert.That(interactors, Is.Empty);

                // Explicitly register waitlist items
                inputModule.RegisterWaitlistInteractors();
            }

            inputModule.GetRegisteredInteractors(interactors);

            Assert.That(interactors, Is.EqualTo(new[] { interactor }));
        }

        [Test]
        public void InteractorCanCreateModuleButOnlyRegisterWithWaitlist(
            [ValueSource(nameof(s_CreationModes))] XRInteractionRuntimeSettings.ManagerCreationMode creationMode,
            [ValueSource(nameof(s_RuntimeBehaviorTypes))] Type type)
        {
            Assume.That(XRInteractionRuntimeSettings.InstanceInternal != null, Is.True);
            Assume.That(ComponentLocatorUtility<XRUIInputModule>.componentCache, Is.Null);

            XRInteractionRuntimeSettings.InstanceInternal.managerCreationMode = creationMode;
            XRInteractionRuntimeSettings.InstanceInternal.uiModuleRegistrationMode = XRInteractionRuntimeSettings.ManagerRegistrationMode.Manual;

            var interactor = CreateUIInteractor(type);

            // The module should not be automatically created when set to Manual
            XRUIInputModule inputModule;
            if (creationMode == XRInteractionRuntimeSettings.ManagerCreationMode.Manual)
            {
                Assert.That(EventSystem.current == null, Is.True);
                Assert.That(Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None), Has.Length.EqualTo(0));
                Assert.That(Object.FindObjectsByType<XRUIInputModule>(FindObjectsSortMode.None), Has.Length.EqualTo(0));

                var eventSystemGo = new GameObject("EventSystem", typeof(TestEventSystem), typeof(XRUIInputModule));
                inputModule = eventSystemGo.GetComponent<XRUIInputModule>();
            }
            else
            {
                Assert.That(EventSystem.current != null, Is.True);
                inputModule = EventSystem.current.GetComponent<XRUIInputModule>();
                Assert.That(inputModule != null, Is.True);
                Assert.That(ComponentLocatorUtility<XRUIInputModule>.componentCache, Is.SameAs(inputModule));
            }

            // The module should not automatically register waitlist items when set to Manual
            var interactors = new List<IUIInteractor>();
            inputModule.GetRegisteredInteractors(interactors);

            Assert.That(interactors, Is.Empty);

            // Explicitly register waitlist items
            inputModule.RegisterWaitlistInteractors();
            inputModule.GetRegisteredInteractors(interactors);

            Assert.That(interactors, Is.EqualTo(new[] { interactor }));
        }

        /// <summary>
        /// Create and return an instance of the given type of interactor which supports registering with the XR UI Input Module.
        /// </summary>
        /// <param name="type">The type of interactor <see cref="IUIInteractor"/>.</param>
        /// <returns>Returns the component instance. Returns <see langword="null"/> only if the test has not been updated for the newly introduced type.</returns>
        static IUIInteractor CreateUIInteractor(Type type)
        {
            if (type == typeof(NearFarInteractor))
            {
                var interactor = TestUtilities.CreateNearFarInteractor();
                interactor.enableUIInteraction = true;
                return interactor;
            }

            if (type == typeof(XRGazeInteractor))
            {
                var interactor = TestUtilities.CreateGazeInteractor();
                interactor.enableUIInteraction = true;
                return interactor;
            }

            if (type == typeof(XRPokeInteractor))
            {
                var interactor = TestUtilities.CreatePokeInteractor();
                interactor.enableUIInteraction = true;
                return interactor;
            }

            if (type == typeof(XRRayInteractor))
            {
                var interactor = TestUtilities.CreateRayInteractor();
                interactor.enableUIInteraction = true;
                return interactor;
            }

            Assert.Fail($"Unhandled type of interactor that implements IUIInteractor. Please update test to add code for creating {type}.");
            return null;
        }
    }
}
