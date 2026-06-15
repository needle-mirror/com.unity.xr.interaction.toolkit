using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
    class TriggerColliderMonitor
    {
        public List<Collider> triggerableColliders { get; } = new List<Collider>();

        Rigidbody m_Rigidbody;

        public Rigidbody rigidbody => m_Rigidbody;

        public bool hasRigidbody { get; private set; }

        readonly List<bool> m_EnabledStates = new List<bool>();

        static readonly List<Collider> s_ScratchColliders = new List<Collider>();

        public void FindColliders(GameObject gameObject)
        {
            triggerableColliders.Clear();

            // If there isn't a Rigidbody on the GameObject, a trigger collider has to be on that GameObject
            // for OnTriggerEnter, OnTriggerStay, and OnTriggerExit to be called by Unity. When there is a Rigidbody,
            // colliders can be on deeply nested child GameObjects.
            // See Collision action matrix https://docs.unity3d.com/Manual/CollidersOverview.html

            hasRigidbody = gameObject.TryGetComponent(out m_Rigidbody);
            if (hasRigidbody)
            {
                gameObject.GetComponentsInChildren(s_ScratchColliders);

                foreach (var collider in s_ScratchColliders)
                {
                    if (ReferenceEquals(collider.attachedRigidbody, rigidbody))
                        triggerableColliders.Add(collider);
                }

                s_ScratchColliders.Clear();
            }
            else
            {
                gameObject.GetComponents(triggerableColliders);
            }
        }

        public void CaptureEnabledState()
        {
            m_EnabledStates.Clear();
            m_EnabledStates.EnsureCapacity(triggerableColliders.Count);

            foreach (var collider in triggerableColliders)
            {
                m_EnabledStates.Add(CanTrigger(collider));
            }
        }

        public bool HasTriggerableColliderBeenDisabled()
        {
            for (int index = 0, count = m_EnabledStates.Count; index < count; index++)
            {
                if (!m_EnabledStates[index])
                    continue;

                var collider = triggerableColliders[index];
                if (!CanTrigger(collider))
                    return true;
            }

            return false;
        }

        static bool CanTrigger(Collider collider) => collider != null && collider.isTrigger && collider.enabled && collider.gameObject.activeInHierarchy;
    }
}
