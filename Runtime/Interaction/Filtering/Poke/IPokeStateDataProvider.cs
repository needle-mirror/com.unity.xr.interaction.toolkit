using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Filtering
{
    /// <summary>
    /// This provider interface allows a source component to populate <see cref="PokeStateData"/> upon request to
    /// a component that is bound to the <see cref="pokeStateData"/> bindable variable that provides
    /// state data about a poke interaction. Typically this is needed by an affordance listener for poke.
    /// </summary>
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State")]
    public interface IPokeStateDataProvider
    {
        /// <summary>
        /// <see cref="IReadOnlyBindableVariable{T}"/> that updates whenever the poke logic state is evaluated.
        /// </summary>
        /// <seealso cref="PokeStateData"/>
        IReadOnlyBindableVariable<PokeStateData> pokeStateData { get; }
    }
}
