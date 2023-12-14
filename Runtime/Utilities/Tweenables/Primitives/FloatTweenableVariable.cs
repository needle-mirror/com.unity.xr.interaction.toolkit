using System;
using Unity.Jobs;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Jobs;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables.Primitives
{
    /// <summary>
    /// Bindable variable that can tween over time towards a target float value.
    /// Uses an async implementation to tween using the job system.
    /// </summary>
    [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
    public class FloatTweenableVariable : TweenableVariableAsyncBase<float>
    {
        /// <inheritdoc />
        protected override JobHandle ScheduleTweenJob(ref TweenJobData<float> jobData)
        {
            var job = new FloatTweenJob { jobData = jobData };
            return job.Schedule();
        }
    }
}