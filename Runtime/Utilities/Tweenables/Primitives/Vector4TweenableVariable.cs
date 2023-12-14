using System;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Jobs;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives
{
    /// <summary>
    /// Bindable variable that can tween over time towards a target float4 (Vector4) value.
    /// Uses an async implementation to tween using the job system.
    /// </summary>
    [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
    public class Vector4TweenableVariable : TweenableVariableAsyncBase<float4>
    {
        /// <inheritdoc />
        protected override JobHandle ScheduleTweenJob(ref TweenJobData<float4> jobData)
        {
            var job = new Float4TweenJob { jobData = jobData };
            return job.Schedule();
        }
    }
}