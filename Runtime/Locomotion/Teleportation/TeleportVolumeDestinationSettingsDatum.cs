using Unity.XR.CoreUtils.Datums;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation
{
    /// <summary>
    /// <see cref="ScriptableObject"/> container class that holds a <see cref="TeleportVolumeDestinationSettings"/> value.
    /// </summary>
    [CreateAssetMenu(fileName = "TeleportVolumeDestinationSettings", menuName = "XR/Locomotion/Teleport Volume Destination Settings")]
    [HelpURL(XRHelpURLConstants.k_TeleportVolumeDestinationSettingsDatum)]
    public class TeleportVolumeDestinationSettingsDatum : Datum<TeleportVolumeDestinationSettings>
    {
        
    }
}