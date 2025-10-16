using NUnit.Framework;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    /// <summary>
    /// Global one-time setup and teardown for all the test fixtures in this namespace.
    /// See https://docs.nunit.org/articles/nunit/writing-tests/attributes/setupfixture.html.
    /// </summary>
    [SetUpFixture]
    class GlobalTestSetup
    {
        XRInteractionRuntimeSettings m_OldRuntimeSettings;
        XRInteractionRuntimeSettings m_TestRuntimeSettings;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            m_OldRuntimeSettings = XRInteractionRuntimeSettings.InstanceInternal;

            // Ensure consistent behavior by using the default project settings for XRI.
            m_TestRuntimeSettings = ScriptableObject.CreateInstance<XRInteractionRuntimeSettings>();
            m_TestRuntimeSettings.hideFlags |= HideFlags.DontSave;
            XRInteractionRuntimeSettings.InstanceInternal = m_TestRuntimeSettings;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (m_TestRuntimeSettings != null)
            {
                Object.Destroy(m_TestRuntimeSettings);
                m_TestRuntimeSettings = null;
            }

            XRInteractionRuntimeSettings.InstanceInternal = m_OldRuntimeSettings;
        }
    }
}
