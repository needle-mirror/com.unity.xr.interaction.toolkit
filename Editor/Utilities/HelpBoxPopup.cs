using UnityEngine;

namespace UnityEditor.XR.Interaction.Toolkit.Utilities
{
    /// <summary>
    /// Content for a popup window. Displays a help box with a message to the user.
    /// </summary>
    /// <seealso cref="PopupWindow.Show(Rect,PopupWindowContent)"/>
    class HelpBoxPopup : PopupWindowContent
    {
        /// <summary>
        /// The message text.
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// The type of message: Info, Warning, or Error.
        /// </summary>
        public MessageType messageType { get; set; } = MessageType.Warning;

        /// <inheritdoc />
        public override Vector2 GetWindowSize() => new Vector2(400f, 60f);

        /// <inheritdoc />
        public override void OnGUI(Rect rect)
        {
            EditorGUILayout.HelpBox(message, messageType);
        }
    }
}
