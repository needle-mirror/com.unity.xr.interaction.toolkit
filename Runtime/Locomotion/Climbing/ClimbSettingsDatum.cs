using Unity.XR.CoreUtils.Datums;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing
{
    /// <summary>
    /// <see cref="ScriptableObject"/> container class that holds a <see cref="ClimbSettings"/> value.
    /// </summary>
    [CreateAssetMenu(fileName = "ClimbSettings", menuName = "XR/Locomotion/Climb Settings")]
    [HelpURL(XRHelpURLConstants.k_ClimbSettingsDatum)]
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
    public class ClimbSettingsDatum : Datum<ClimbSettings>
    {

    }
}
