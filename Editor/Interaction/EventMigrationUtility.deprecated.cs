using System;
using UnityEngine.Events;

namespace UnityEditor.XR.Interaction.Toolkit
{
    /// <summary>
    /// Utility functions related to migrating deprecated <see cref="UnityEvent"/> properties.
    /// </summary>
    [Obsolete("The EventMigrationUtility has been deprecated and will be removed in a future version of XRI.", true)]
    public static class EventMigrationUtility
    {
        /// <summary>
        /// Migrate the persistent listeners from one <see cref="UnityEvent"/> to another.
        /// The listeners will be removed from the source event, and appended to the destination event.
        /// The scripts of the target of Dynamic listeners still need to be manually updated to match the new event signature.
        /// </summary>
        /// <param name="srcUnityEvent">The source <see cref="SerializedProperty"/> of the <see cref="UnityEvent"/> to move from.</param>
        /// <param name="dstUnityEvent">The destination <see cref="SerializedProperty"/> of the <see cref="UnityEvent"/> to move to.</param>
        [Obsolete("MigrateEvent is marked for deprecation and will be removed in a future version. It is only used for migrating deprecated events.", true)]
        public static void MigrateEvent(SerializedProperty srcUnityEvent, SerializedProperty dstUnityEvent)
        {
        }
    }
}
