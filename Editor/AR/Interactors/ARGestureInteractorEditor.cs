using UnityEngine.XR.Interaction.Toolkit.AR;

namespace UnityEditor.XR.Interaction.Toolkit.AR
{
    /// <summary>
    /// Custom editor for an <see cref="ARGestureInteractor"/>.
    /// </summary>
    [CustomEditor(typeof(ARGestureInteractor), true), CanEditMultipleObjects]
    public class ARGestureInteractorEditor : XRBaseInteractorEditor
    {
    }
}
