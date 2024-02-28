using System.Collections.Generic;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors.Casters
{
    /// <summary>
    /// Basic interface used to define an interaction caster, used by <see cref="NearFarInteractor"/>.
    /// </summary>
    /// <seealso cref="InteractionCasterBase"/>
    /// <seealso cref="ICurveInteractionCaster"/>
    public interface IInteractionCaster
    {
        /// <summary>
        /// Indicates whether the caster has been initialized.
        /// </summary>
        bool isInitialized { get; }
        
        /// <summary>
        /// Source of origin and direction used when updating sample points.
        /// </summary>
        Transform castOrigin { get; set; }
        
        /// <summary>
        /// Gets the effective cast origin, which may be different than the <see cref="castOrigin"/>.
        /// The caster may use a different transform, such as one that is stabilized, to conduct the cast. 
        /// </summary>
        Transform effectiveCastOrigin { get; }

        /// <summary>
        /// Gets an unsorted list of collider targets
        /// </summary>
        /// <param name="interactionManager">XR Interaction manager reference</param>
        /// <param name="targets">List of target colliders to populate.</param>
        /// <returns>Returns true if collider targets were found.</returns>
        bool TryGetColliderTargets(XRInteractionManager interactionManager, List<Collider> targets);
    }
}