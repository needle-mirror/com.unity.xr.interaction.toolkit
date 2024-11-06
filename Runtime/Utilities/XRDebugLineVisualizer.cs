using System.Collections.Generic;
using System;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
    /// <summary>
    /// A utility class for visualizing debug lines in VR environments.
    /// </summary>
    [AddComponentMenu("")] // Hide in menu
    [HelpURL(XRHelpURLConstants.k_XRDebugLineVisualizer)]
    internal class XRDebugLineVisualizer : MonoBehaviour
    {
        /// <summary>
        /// Represents a debug line with its properties.
        /// </summary>
        [Serializable]
        class DebugLine
        {
            /// <summary>
            /// The name identifier for the debug line.
            /// </summary>
            public string name;

            /// <summary>
            /// The color of the debug line.
            /// </summary>
            public Color color;

            /// <summary>
            /// The LineRenderer component used to render the debug line.
            /// </summary>
            public LineRenderer lineRenderer;

            /// <summary>
            /// The remaining time before the line decays and is removed.
            /// </summary>
            public float decayTime;
        }

        /// <summary>
        /// List of active debug lines.
        /// </summary>
        List<DebugLine> m_DebugLines = new List<DebugLine>();

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Update()
        {
            for (int i = m_DebugLines.Count - 1; i >= 0; i--)
            {
                m_DebugLines[i].decayTime -= Time.deltaTime;
                if (m_DebugLines[i].decayTime <= 0f)
                {
                    Destroy(m_DebugLines[i].lineRenderer.gameObject);
                    m_DebugLines.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnDestroy()
        {
            ClearLines();
        }

        /// <summary>
        /// Updates an existing debug line or creates a new one if it doesn't exist.
        /// </summary>
        /// <param name="lineName">The name identifier for the line.</param>
        /// <param name="start">The start position of the line.</param>
        /// <param name="end">The end position of the line.</param>
        /// <param name="color">The color of the line.</param>
        /// <param name="decayTime">The time in seconds before the line decays and is removed. Defaults to 0.2 seconds.</param>
        public void UpdateOrCreateLine(string lineName, Vector3 start, Vector3 end, Color color, float decayTime = 0.2f)
        {
            DebugLine line = m_DebugLines.Find(l => l.name == lineName);
            if (line == null)
            {
                GameObject lineObj = new GameObject(lineName + "Line");
                lineObj.transform.SetParent(transform, false);
                LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
                lineRenderer.startWidth = 0.01f;
                lineRenderer.endWidth = 0.01f;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                lineRenderer.startColor = color;
                lineRenderer.endColor = color;

                line = new DebugLine { name = lineName, color = color, lineRenderer = lineRenderer, };
                m_DebugLines.Add(line);
            }

            line.lineRenderer.SetPosition(0, start);
            line.lineRenderer.SetPosition(1, end);
            line.decayTime = decayTime;
        }

        /// <summary>
        /// Clears all active debug lines.
        /// </summary>
        public void ClearLines()
        {
            foreach (var line in m_DebugLines)
            {
                Destroy(line.lineRenderer.gameObject);
            }
            m_DebugLines.Clear();
        }
    }
}
