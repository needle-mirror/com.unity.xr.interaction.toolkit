using System;
using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit
{
    public partial class XRRayInteractorEditor
    {
        protected static partial class Contents
        {
            /// <summary>The help box message when the hit detection type is cone cast but the line type is not straight line.</summary>
            [Obsolete("coneCastRequiresStraightLineWarning has been deprecated in version 2.6.4. The warning message is no longer necessary since the configuration is now supported.")]
            public static readonly GUIContent coneCastRequiresStraightLineWarning = EditorGUIUtility.TrTextContent("Cone Cast requires Straight Line.");
        }
    }
}
