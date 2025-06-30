using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Pooling;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
    /// <summary>
    /// Use this class to maintain a list of Interactors that are potentially influenced by discontinuous locomotion
    /// of the rig, such as teleportation and snap turn. Uses the events invoked by <see cref="LocomotionProvider"/>
    /// components to detect locomotion.
    /// </summary>
    /// <remarks>
    /// Used by the XR Grab Interactable to cancel out the effect of the teleportation from its tracked velocity
    /// so it does not release at unintentionally high energy. Snap turning is treated similar to a teleport
    /// where the turn velocity should also be canceled out.
    /// </remarks>
    /// <seealso cref="XRGrabInteractable"/>
    class TeleportationMonitor
    {
        class PoseContainer
        {
            public Pose beforePose;
            public Pose afterPose;
            public Pose deltaPose;

            // Used to avoid repeated work capturing or calculating the poses
            int m_BeforeFrame = -1;
            int m_AfterFrame = -1;
            int m_DeltaFrame = -1;

            public void CaptureBeforePose(XRBodyTransformer bodyTransformer)
            {
                // If the origin pose has already been captured this frame, we don't need to do it again
                // since locomotion application is only done once per frame.
                var currentFrame = Time.frameCount;
                if (m_BeforeFrame == currentFrame)
                    return;

                if (!LocomotionUtility.TryGetOriginTransform(bodyTransformer, out var originTransform))
                    return;

                m_BeforeFrame = currentFrame;
                beforePose = originTransform.GetWorldPose();
            }

            public void CaptureAfterPose(XRBodyTransformer bodyTransformer)
            {
                // If the origin pose has already been captured this frame, we don't need to do it again
                // since locomotion application is only done once per frame.
                var currentFrame = Time.frameCount;
                if (m_AfterFrame == currentFrame)
                    return;

                if (!LocomotionUtility.TryGetOriginTransform(bodyTransformer, out var originTransform))
                    return;

                m_AfterFrame = currentFrame;
                afterPose = originTransform.GetWorldPose();
            }

            public void CalculateDeltaPose()
            {
                var currentFrame = Time.frameCount;
                if (m_DeltaFrame == currentFrame)
                    return;

                var translated = afterPose.position - beforePose.position;
                var rotated = afterPose.rotation * Quaternion.Inverse(beforePose.rotation);

                m_DeltaFrame = currentFrame;
                deltaPose = new Pose(translated, rotated);
            }
        }

        abstract class ProviderMonitor
        {
            public abstract void AddInteractor(IXRInteractor interactor);

            public abstract void RemoveInteractor(IXRInteractor interactor);

            /// <summary>
            /// The <see cref="Pose"/> of the <see cref="XROrigin"/> rig before and after locomotion.
            /// Used to calculate the locomotion delta.
            /// </summary>
            protected static Dictionary<XRBodyTransformer, PoseContainer> s_OriginPoses;
        }

        class ProviderMonitor<T> : ProviderMonitor
            where T : LocomotionProvider
        {
            public event Action<PoseContainer> providerStepped;

            /// <summary>
            /// The list of interactors monitored that are influenced by locomotion.
            /// Consists of those that are a child GameObject of the <see cref="XROrigin"/> rig.
            /// </summary>
            /// <remarks>
            /// There will typically only ever be one <see cref="T"/> in the scene.
            /// </remarks>
            Dictionary<T, List<IXRInteractor>> m_ProviderInteractors;

            /// <summary>
            /// References to provider instances found.
            /// </summary>
            static List<T> s_Providers;

            static readonly LinkedPool<Dictionary<T, List<IXRInteractor>>> s_ProviderInteractorsPool =
                new LinkedPool<Dictionary<T, List<IXRInteractor>>>(() => new Dictionary<T, List<IXRInteractor>>());

            public static void InitializeProvidersList()
            {
                if (s_Providers != null)
                    return;

                s_Providers = new List<T>();

                foreach (var provider in LocomotionProvider.locomotionProviders)
                {
                    if (provider == null)
                        continue;

                    if (provider is T providerT)
                        s_Providers.Add(providerT);
                }

                LocomotionProvider.locomotionProvidersChanged += OnLocomotionProvidersChanged;
                return;

                void OnLocomotionProvidersChanged(LocomotionProvider provider)
                {
                    if (provider is T providerT)
                        s_Providers.Add(providerT);

                    // Prune the list as new locomotion providers are added so that it doesn't infinitely grow in size.
                    // It's likely if a new locomotion provider is added, the old rig with providers may have been destroyed.
                    s_Providers.RemoveAll(p => p == null);
                }
            }

            public override void AddInteractor(IXRInteractor interactor)
            {
                if (interactor == null)
                    throw new ArgumentNullException(nameof(interactor));

                var interactorTransform = interactor.transform;
                if (interactorTransform == null)
                    return;

                if (s_Providers == null)
                {
                    InitializeProvidersList();
                    Debug.Assert(s_Providers != null);
                }

                foreach (var provider in s_Providers)
                {
                    if (provider == null)
                        continue;

                    if (!LocomotionUtility.TryGetOriginTransform(provider, out var originTransform))
                        continue;

                    if (!interactorTransform.IsChildOf(originTransform))
                        continue;

                    m_ProviderInteractors ??= s_ProviderInteractorsPool.Get();

                    if (!m_ProviderInteractors.TryGetValue(provider, out var interactors))
                    {
                        interactors = new List<IXRInteractor>();
                        m_ProviderInteractors.Add(provider, interactors);
                    }

                    Debug.Assert(!interactors.Contains(interactor));
                    interactors.Add(interactor);

                    if (interactors.Count == 1)
                    {
                        provider.beforeStepLocomotion += OnBeforeStepLocomotion;
                        provider.afterStepLocomotion += OnAfterStepLocomotion;
                    }
                }
            }

            public override void RemoveInteractor(IXRInteractor interactor)
            {
                if (interactor == null)
                    throw new ArgumentNullException(nameof(interactor));

                var totalInteractors = 0;
                if (m_ProviderInteractors != null)
                {
                    foreach (var kvp in m_ProviderInteractors)
                    {
                        var provider = kvp.Key;
                        var interactors = kvp.Value;

                        if (provider == null)
                            continue;

                        if (interactors.Remove(interactor) && interactors.Count == 0)
                        {
                            provider.beforeStepLocomotion -= OnBeforeStepLocomotion;
                            provider.afterStepLocomotion -= OnAfterStepLocomotion;
                        }

                        totalInteractors += interactors.Count;
                    }
                }

                // Release back to the pool
                if (totalInteractors == 0 && m_ProviderInteractors != null)
                {
                    s_ProviderInteractorsPool.Release(m_ProviderInteractors);
                    m_ProviderInteractors = null;
                }
            }

            static void CaptureOriginPoseBefore(XRBodyTransformer bodyTransformer)
            {
                s_OriginPoses ??= new Dictionary<XRBodyTransformer, PoseContainer>();

                if (!s_OriginPoses.TryGetValue(bodyTransformer, out var poseContainer))
                {
                    poseContainer = new PoseContainer();
                    s_OriginPoses[bodyTransformer] = poseContainer;
                }

                poseContainer.CaptureBeforePose(bodyTransformer);
            }

            static PoseContainer CaptureOriginPoseAfter(XRBodyTransformer bodyTransformer)
            {
                s_OriginPoses ??= new Dictionary<XRBodyTransformer, PoseContainer>();

                if (!s_OriginPoses.TryGetValue(bodyTransformer, out var poseContainer))
                {
                    poseContainer = new PoseContainer();
                    s_OriginPoses[bodyTransformer] = poseContainer;
                }

                poseContainer.CaptureAfterPose(bodyTransformer);

                return poseContainer;
            }

            static void OnBeforeStepLocomotion(LocomotionProvider provider)
            {
                if (provider.mediator == null)
                    return;

                CaptureOriginPoseBefore(provider.mediator.bodyTransformer);
            }

            void OnAfterStepLocomotion(LocomotionProvider provider)
            {
                if (provider.mediator == null)
                    return;

                var poseContainer = CaptureOriginPoseAfter(provider.mediator.bodyTransformer);
                providerStepped?.Invoke(poseContainer);
            }
        }

        /// <summary>
        /// Calls the methods in its invocation list when one of the Interactors monitored has been influenced by teleportation.
        /// The <see cref="Pose"/> event args represents the amount the <see cref="XROrigin"/> rig was translated and rotated.
        /// </summary>
        public event Action<Pose, Pose, Pose> teleported;

        int m_TeleportedFrame = -1;

        ProviderMonitor[] m_Monitors;

        void Initialize()
        {
            var teleportMonitor = new ProviderMonitor<TeleportationProvider>();
            teleportMonitor.providerStepped += OnTeleportedAlways;

            var snapMonitor = new ProviderMonitor<SnapTurnProvider>();
            snapMonitor.providerStepped += OnTeleportedAlways;

            var turnMonitor = new ProviderMonitor<ContinuousTurnProvider>();
            turnMonitor.providerStepped += OnTeleportedTurnAround;

            ProviderMonitor<TeleportationProvider>.InitializeProvidersList();
            ProviderMonitor<SnapTurnProvider>.InitializeProvidersList();
            ProviderMonitor<ContinuousTurnProvider>.InitializeProvidersList();

            m_Monitors = new ProviderMonitor[] { teleportMonitor, snapMonitor, turnMonitor, };
        }

        /// <summary>
        /// Adds <paramref name="interactor"/> to monitor. If it is a child of the XR Origin, <see cref="teleported"/>
        /// will be invoked when the player teleports (or snap turns).
        /// </summary>
        /// <param name="interactor">The Interactor to add.</param>
        /// <seealso cref="RemoveInteractor"/>
        public void AddInteractor(IXRInteractor interactor)
        {
            if (m_Monitors == null)
            {
                Initialize();
                Debug.Assert(m_Monitors != null);
            }

            foreach (var monitor in m_Monitors)
            {
                monitor.AddInteractor(interactor);
            }
        }

        /// <summary>
        /// Removes <paramref name="interactor"/> from monitor.
        /// </summary>
        /// <param name="interactor">The Interactor to remove.</param>
        /// <seealso cref="AddInteractor"/>
        public void RemoveInteractor(IXRInteractor interactor)
        {
            foreach (var monitor in m_Monitors)
            {
                monitor.RemoveInteractor(interactor);
            }
        }

        void OnTeleportedAlways(PoseContainer poseContainer)
        {
            // Ensure that the public event is only invoked once across all monitors
            var currentFrame = Time.frameCount;
            if (m_TeleportedFrame == currentFrame)
                return;

            m_TeleportedFrame = currentFrame;
            poseContainer.CalculateDeltaPose();
            teleported?.Invoke(poseContainer.beforePose, poseContainer.afterPose, poseContainer.deltaPose);
        }

        void OnTeleportedTurnAround(PoseContainer poseContainer)
        {
            // Ensure that the public event is only invoked once across all monitors
            var currentFrame = Time.frameCount;
            if (m_TeleportedFrame == currentFrame)
                return;

            // Only consider a Turn Around as a teleport
            if (Vector3.Dot(poseContainer.beforePose.forward, poseContainer.afterPose.forward) >= 0)
                return;

            m_TeleportedFrame = currentFrame;
            poseContainer.CalculateDeltaPose();
            teleported?.Invoke(poseContainer.beforePose, poseContainer.afterPose, poseContainer.deltaPose);
        }
    }
}
