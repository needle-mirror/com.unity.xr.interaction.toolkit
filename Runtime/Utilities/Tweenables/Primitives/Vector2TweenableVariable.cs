using System;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Jobs;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives
{
    /// <summary>
    /// Bindable variable that can tween over time towards a target float2 (Vector2) value.
    /// Uses an async implementation to tween using the job system.
    /// </summary>
    [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
    public class Vector2TweenableVariable : TweenableVariableAsyncBase<float2>
    {
        /// <inheritdoc />
        protected override JobHandle ScheduleTweenJob(ref TweenJobData<float2> jobData)
        {
            var job = new Float2TweenJob { jobData = jobData };
            return job.Schedule();
        }
    }
}