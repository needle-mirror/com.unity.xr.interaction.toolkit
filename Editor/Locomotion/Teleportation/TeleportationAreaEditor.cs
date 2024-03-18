using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace UnityEditor.XR.Interaction.Toolkit.Locomotion.Teleportation
{
    /// <summary>
    /// Custom editor for an <see cref="TeleportationArea"/>.
    /// </summary>
    [CustomEditor(typeof(TeleportationArea), true), CanEditMultipleObjects]
    [MovedFrom("UnityEditor.XR.Interaction.Toolkit")]
    public class TeleportationAreaEditor : BaseTeleportationInteractableEditor
    {
    }
}
