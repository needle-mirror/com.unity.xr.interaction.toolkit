using System.Collections.Generic;
using Unity.Collections;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors.Casters
{
    /// <summary>
    /// Interface containing necessary information to perform various types of casts along a curve.
    /// The caster is responsible for updating the sample points, performing the cast and returning a sorted list of colliders.
    /// </summary>
    /// <seealso cref="CurveInteractionCaster"/>
    public interface ICurveInteractionCaster : IInteractionCaster
    {
        /// <summary>
        /// Gets or sets the layer mask used for limiting ray cast targets.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Implemented as a default interface member to avoid a breaking change
        /// since this property was introduced after the interface was public.
        /// Implementers of this interface should typically implement this property as:
        /// </para>
        /// <code>
        /// [SerializeField]
        /// LayerMask m_RaycastMask = -1;
        ///
        /// public LayerMask raycastMask
        /// {
        ///     get => m_RaycastMask;
        ///     set => m_RaycastMask = value;
        /// }
        /// </code>
        /// </remarks>
        LayerMask raycastMask
        {
            get => -1;
            set
            {
            }
        }

        /// <summary>
        /// Gets the curve sample points used to determine interaction results.
        /// </summary>
        NativeArray<Vector3> samplePoints { get; }

        /// <summary>
        /// Gets the sample point at the last index of <see cref="samplePoints"/>.
        /// </summary>
        Vector3 lastSamplePoint { get; }

        /// <summary>
        /// Tries to get a list of collider targets based on the interaction caster's current state, sorted by their distance from the cast origin.
        /// </summary>
        /// <param name="interactionManager">The XR interaction manager.</param>
        /// <param name="colliders">List of colliders to populate with detected targets.</param>
        /// <param name="raycastHits">List of raycast hits lined up to the list of colliders.</param>
        /// <returns>True if targets are successfully detected.</returns>
        bool TryGetColliderTargets(XRInteractionManager interactionManager, List<Collider> colliders, List<RaycastHit> raycastHits);
    }
}
