#if UIELEMENTS_MODULE_PRESENT && UNITY_6000_2_OR_NEWER
#define UITOOLKIT_WORLDSPACE_ENABLED
using System.Collections.Generic;
using UnityEngine.UIElements;
#endif
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    /// <summary>
    /// Class that handles UI Toolkit poke interactions.
    /// This is used by XRPokeInteractor to encapsulate UI interaction logic.
    /// </summary>
    internal class XRUIToolkitPokeHandler
    {
#if UITOOLKIT_WORLDSPACE_ENABLED
        // References to debug visualizers
        GameObject m_VisualizersRoot;
        Transform m_PokePointVisualizer;
        Transform m_ClosestPointVisualizer;
        Transform m_RayOriginVisualizer;
        Transform m_NormalVisualizer;
        bool m_VisualizersCreated;
#endif

        // Reference to the owning interactor
        readonly XRPokeInteractor m_Interactor;

        // Flag to enable/disable depth updates
        bool m_UpdateDepth;

        /// <summary>
        /// Whether to update depth during interactions.
        /// </summary>
        public bool updateDepth
        {
            get => m_UpdateDepth;
            set => m_UpdateDepth = value;
        }

        /// <summary>
        /// Creates a new UI Toolkit poke handler for the given interactor.
        /// </summary>
        /// <param name="interactor">The owning poke interactor.</param>
        public XRUIToolkitPokeHandler(XRPokeInteractor interactor)
        {
            m_Interactor = interactor;
        }

        /// <summary>
        /// Clean up any resources when done
        /// </summary>
        public void Dispose()
        {
#if UITOOLKIT_WORLDSPACE_ENABLED
            DestroyVisualizers();
#endif
        }

        /// <summary>
        /// Processes a poke interaction with a UI Toolkit document.
        /// </summary>
        /// <param name="hitCollider">The <see cref="Collider"/> being interacted with.</param>
        /// <param name="interactableTransform">The <see cref="Transform"/> being interacted with. This requires a UI Document component.</param>
        /// <param name="interactable">The <see cref="IXRInteractable"/> being interacted with.</param>
        /// <param name="useMultiPick">If <see langword="True"/>, UI Toolkit will perform a multi-picking operation.</param>
        /// <param name="pokeFilter">The <see cref="IXRPokeFilter"/> reference for the <paramref name="interactable"/>.</param>
        public void ProcessPokeInteraction(
            Collider hitCollider,
            Transform interactableTransform,
            IXRInteractable interactable,
            bool useMultiPick,
            IXRPokeFilter pokeFilter = null)
        {
#if UITOOLKIT_WORLDSPACE_ENABLED
            if (!interactableTransform.TryGetComponent(out UIDocument document))
                return;

            var documentTransform = document.transform;
            var pokeTransform = m_Interactor.GetAttachTransform(null);
            var pokePoint = pokeTransform.position;

            // Use document's forward as our stable picking axis
            var documentNormal = -documentTransform.forward;

            // Project onto document plane
            var documentPlane = new Plane(documentNormal, documentTransform.position);
            documentPlane.Raycast(new Ray(pokePoint, -documentNormal), out float distanceToPlane);
            var closestPoint = pokePoint - documentNormal * distanceToPlane;

            // Pull back ray origin by our poke width to ensure stable picking
            var rayOrigin = closestPoint + documentNormal;

            // Update debug visuals if enabled
            UpdateVisualizers(pokePoint, closestPoint, rayOrigin, documentNormal, documentTransform);

            // Perform pick to find hit element
            var hitElement = PerformPick(document, rayOrigin, -documentNormal, m_Interactor.pokeWidth, useMultiPick);

            if (hitElement != null)
            {
                // Cache the physical hit data for use by interactables
                XRUIToolkitHandler.UpdateInteractorHitData(m_Interactor, new InteractorHitData
                {
                    closestPoint = closestPoint,
                    interactorOrigin = pokePoint,
                    interactorDirection = documentNormal,
                    hitDocument = document
                });

                // Determine poke depth from filter if available
                float processResult = 1f;
                if (pokeFilter != null && interactable is IXRSelectInteractable selectInteractable)
                {
                    // We invoke the poke filter process to obtain the poke press amount. This call is not expensive because unlike the normal process call, it just returns the cached value for the poke data.
                    // It might be preferable to find a different solution to, assuming user's implement their own poke filter that might behave differently, but the value should be correct in those cases, which is import to maintain.
                    // With Unity provided code it is the least invasive way of getting the poke press amount at this point in the frame.

                    // The reason we call need to get this data here, is because we want to retrieve it before the XRUIToolkitHandler invokes the event system update, which occurs at the end of PreProcess.
                    processResult = pokeFilter.Process(m_Interactor, selectInteractable, 0f);
                }

                bool pokeComplete = processResult > 0.99f;

                // Apply element depth based on poke progress if enabled
                if (m_UpdateDepth)
                {
                    // Add better handling of detection of z depth to handle this
                    // TODO Improve depth support - it's a bit glitchy right now
                    XRUIToolkitHandler.SetZDepthForInteractor(hitElement, m_Interactor, 20f * (1f - processResult));
                }

                // Update XRUIToolkitHandler pointer state
                XRUIToolkitHandler.HandlePointerUpdate(
                    m_Interactor,
                    rayOrigin,
                    Quaternion.LookRotation((closestPoint - rayOrigin).normalized),
                    pokeComplete,
                    false);
            }
            else
            {
                // No hit, reset pointer state
                ResetPointerState();
            }
#endif
        }

        /// <summary>
        /// Reset the pointer state when no longer interacting with UI
        /// </summary>
        public void ResetPointerState()
        {
            XRUIToolkitHandler.HandlePointerUpdate(
                m_Interactor,
                Vector3.zero,
                Quaternion.identity,
                false,
                true);

#if UITOOLKIT_WORLDSPACE_ENABLED
            // Clear depth if enabled
            if (m_UpdateDepth)
            {
                XRUIToolkitHandler.ClearZDepthForInteractor(m_Interactor);
            }
#endif
        }

#if UITOOLKIT_WORLDSPACE_ENABLED
        /// <summary>
        /// Perform a pick operation at the given point
        /// </summary>
        VisualElement PerformPick(UIDocument document, Vector3 center, Vector3 direction, float radius, bool useMultiPick)
        {
            // Center pick always has priority if it hits
            var centerElement = WorldSpaceInput.Pick3D(document, new Ray(center, direction));
            if (centerElement != null)
                return centerElement;

            // If multi-pick is disabled or radius is too small, return null
            if (useMultiPick)
                return PerformMultiPick(document, center, direction, radius);
            else
                return null;


        }

        /// <summary>
        /// Performs multiple ray casts in a pattern around the center point to find the best UI element to interact with.
        /// This helps with hitting small UI elements and provides more reliable picking.
        /// </summary>
        VisualElement PerformMultiPick(UIDocument document, Vector3 center, Vector3 direction, float radius)
        {
            // Store all hits with their distances
            Dictionary<VisualElement, float> elementDistances = new Dictionary<VisualElement, float>();

            // Sample points in a circle around the center
            const int sampleCount = 4;
            const float angleStep = 2f * Mathf.PI / sampleCount; // Using radians directly

            // Extract document's transformed axes for sampling in correct space
            Matrix4x4 documentTransform = document.transform.localToWorldMatrix;
            Vector3 xAxis = documentTransform.GetColumn(0); // Already includes proper scaling
            Vector3 yAxis = documentTransform.GetColumn(1); // Already includes proper scaling

            for (int i = 0; i < sampleCount; i++)
            {
                float angle = i * angleStep;

                // Calculate offset in world space using document's transformed axes
                Vector3 worldOffset = xAxis * (Mathf.Cos(angle) * radius) +
                                    yAxis * (Mathf.Sin(angle) * radius);

                var sampleOrigin = center + worldOffset;
                var element = WorldSpaceInput.Pick3D(document, new Ray(sampleOrigin, direction));

                if (element != null)
                {
                    // Use squared distance from sampling point to center as proximity metric
                    float distanceSquared = worldOffset.sqrMagnitude;

                    // Keep the closest detection point for each element
                    if (!elementDistances.TryGetValue(element, out float existingDistance) ||
                        distanceSquared < existingDistance)
                    {
                        elementDistances[element] = distanceSquared;
                    }
                }
            }

            // Find the closest element
            VisualElement bestMatch = null;
            float closestDistance = float.MaxValue;

            foreach (var pair in elementDistances)
            {
                if (pair.Value < closestDistance)
                {
                    closestDistance = pair.Value;
                    bestMatch = pair.Key;
                }
            }

            return bestMatch;
        }

        /// <summary>
        /// Update visualizer state based on debug settings
        /// </summary>
        public void UpdateVisualizersState()
        {
            if (m_Interactor.debugVisualizationsEnabled && m_Interactor.isActiveAndEnabled)
            {
                CreateVisualizers();
            }
            else
            {
                DestroyVisualizers();
            }
        }

        void UpdateVisualizers(Vector3 pokePoint, Vector3 closestPoint, Vector3 rayOrigin, Vector3 normal, Transform parentTransform)
        {
            if (!m_Interactor.debugVisualizationsEnabled || !m_VisualizersCreated)
                return;

            m_PokePointVisualizer.position = pokePoint;
            m_ClosestPointVisualizer.position = closestPoint;
            m_RayOriginVisualizer.position = rayOrigin;

            m_NormalVisualizer.position = closestPoint + normal * 0.025f;
            m_NormalVisualizer.up = normal;

            // Ensure visualizers are parented to the document for proper visibility
            if (m_VisualizersRoot.transform.parent != parentTransform)
            {
                m_VisualizersRoot.transform.SetParent(parentTransform, false);
            }
        }

        void CreateVisualizers()
        {
            if (m_VisualizersCreated)
                return;

            // Create a parent object to keep visualizers organized
            m_VisualizersRoot = new GameObject("UIPokeVisualizers");

            // Create debug sphere for poke point (blue)
            GameObject pokePointSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pokePointSphere.name = "PokePoint";
            pokePointSphere.transform.SetParent(m_VisualizersRoot.transform);
            pokePointSphere.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            if (pokePointSphere.TryGetComponent<Collider>(out var collider1))
                Object.Destroy(collider1);
            pokePointSphere.GetComponent<Renderer>().material.color = Color.blue;
            m_PokePointVisualizer = pokePointSphere.transform;

            // Create debug sphere for closest point (green)
            GameObject closestPointSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            closestPointSphere.name = "ClosestPoint";
            closestPointSphere.transform.SetParent(m_VisualizersRoot.transform);
            closestPointSphere.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            if (closestPointSphere.TryGetComponent<Collider>(out var collider2))
                Object.Destroy(collider2);
            closestPointSphere.GetComponent<Renderer>().material.color = Color.green;
            m_ClosestPointVisualizer = closestPointSphere.transform;

            // Create debug sphere for ray origin (yellow)
            GameObject rayOriginSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rayOriginSphere.name = "RayOrigin";
            rayOriginSphere.transform.SetParent(m_VisualizersRoot.transform);
            rayOriginSphere.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            if (rayOriginSphere.TryGetComponent<Collider>(out var collider3))
                Object.Destroy(collider3);
            rayOriginSphere.GetComponent<Renderer>().material.color = Color.yellow;
            m_RayOriginVisualizer = rayOriginSphere.transform;

            // Create cylinder for document normal direction (red)
            GameObject normalIndicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            normalIndicator.name = "DocumentNormal";
            normalIndicator.transform.SetParent(m_VisualizersRoot.transform);
            normalIndicator.transform.localScale = new Vector3(0.005f, 0.05f, 0.005f);
            if (normalIndicator.TryGetComponent<Collider>(out var collider4))
                Object.Destroy(collider4);
            normalIndicator.GetComponent<Renderer>().material.color = Color.red;
            m_NormalVisualizer = normalIndicator.transform;

            m_VisualizersCreated = true;
        }

        void DestroyVisualizers()
        {
            if (m_VisualizersRoot != null)
            {
                Object.Destroy(m_VisualizersRoot);
                m_VisualizersCreated = false;
                m_PokePointVisualizer = null;
                m_ClosestPointVisualizer = null;
                m_RayOriginVisualizer = null;
                m_NormalVisualizer = null;
            }
        }
#endif
    }
}
