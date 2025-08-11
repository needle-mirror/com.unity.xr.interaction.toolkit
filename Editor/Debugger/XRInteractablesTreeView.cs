using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace UnityEditor.XR.Interaction.Toolkit
{
    /// <summary>
    /// Multi-column <see cref="TreeView"/> that shows Interactables.
    /// </summary>
#if UNITY_6000_2_OR_NEWER
    class XRInteractablesTreeView : TreeView<int>
#else
    class XRInteractablesTreeView : TreeView
#endif
    {
        public static XRInteractablesTreeView Create(List<XRInteractionManager> interactionManagers, ref State treeState, ref MultiColumnHeaderState headerState)
        {
            treeState ??= new State();

            var newHeaderState = CreateHeaderState();
            if (headerState != null)
                MultiColumnHeaderState.OverwriteSerializedFields(headerState, newHeaderState);
            headerState = newHeaderState;

            var header = new MultiColumnHeader(headerState);
            return new XRInteractablesTreeView(interactionManagers, treeState, header);
        }

        const float k_RowHeight = 20f;
        const int k_LayerSize = 32;
        const string k_LayerMaskOn = "\u25A0";
        const string k_LayerMaskOff = "\u25A1";

#if UNITY_6000_2_OR_NEWER
        class Item : TreeViewItem<int>
#else
        class Item : TreeViewItem
#endif
        {
            public IXRInteractable interactable;
        }

        [Serializable]
#if UNITY_6000_2_OR_NEWER
        public class State : TreeViewState<int>
#else
        public class State : TreeViewState
#endif
        {
        }

        enum ColumnId
        {
            Name,
            Type,
            LayerMask,
            LayerMaskList,
            Colliders,
            Hovered,
            Selected,

            Count,
        }

        static bool exitingPlayMode => EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode;

        readonly List<XRInteractionManager> m_InteractionManagers = new List<XRInteractionManager>();

        static MultiColumnHeaderState CreateHeaderState()
        {
            var columns = new MultiColumnHeaderState.Column[(int)ColumnId.Count];

            columns[(int)ColumnId.Name] = new MultiColumnHeaderState.Column { width = 180f, minWidth = 80f, headerContent = EditorGUIUtility.TrTextContent("Name") };
            columns[(int)ColumnId.Type] = new MultiColumnHeaderState.Column { width = 120f, minWidth = 80f, headerContent = EditorGUIUtility.TrTextContent("Type") };
            columns[(int)ColumnId.LayerMask] = new MultiColumnHeaderState.Column { width = 240f, minWidth = 80f, headerContent = EditorGUIUtility.TrTextContent("Layer Mask") };
            columns[(int)ColumnId.LayerMaskList] = new MultiColumnHeaderState.Column { width = 120f, minWidth = 80f, headerContent = EditorGUIUtility.TrTextContent("Layer Mask List") };
            columns[(int)ColumnId.Colliders] = new MultiColumnHeaderState.Column { width = 120f, minWidth = 80f, headerContent = EditorGUIUtility.TrTextContent("Colliders") };
            columns[(int)ColumnId.Hovered] = new MultiColumnHeaderState.Column { width = 80f, minWidth = 80f, headerContent = EditorGUIUtility.TrTextContent("Hovered") };
            columns[(int)ColumnId.Selected] = new MultiColumnHeaderState.Column { width = 80f, minWidth = 80f, headerContent = EditorGUIUtility.TrTextContent("Selected") };

            return new MultiColumnHeaderState(columns);
        }

        XRInteractablesTreeView(List<XRInteractionManager> managers, State state, MultiColumnHeader header)
            : base(state, header)
        {
            foreach (var manager in managers)
                AddManager(manager);
            showBorder = false;
            rowHeight = k_RowHeight;
            Reload();
        }

        public void UpdateManagersList(List<XRInteractionManager> currentManagers)
        {
            var managerListChanged = false;

            // Check for Removal
            for (var i = 0; i < m_InteractionManagers.Count; i++)
            {
                var manager = m_InteractionManagers[i];
                if (!currentManagers.Contains(manager))
                {
                    RemoveManager(manager);
                    managerListChanged = true;
                    --i;
                }
            }

            // Check for Add
            foreach (var manager in currentManagers)
            {
                if (!m_InteractionManagers.Contains(manager))
                {
                    AddManager(manager);
                    managerListChanged = true;
                }
            }

            if (managerListChanged)
                Reload();
        }

        void AddManager(XRInteractionManager manager)
        {
            if (m_InteractionManagers.Contains(manager))
                return;

            manager.interactableRegistered += OnInteractableRegistered;
            manager.interactableUnregistered += OnInteractableUnregistered;

            m_InteractionManagers.Add(manager);
            Reload();
        }

        void RemoveManager(XRInteractionManager manager)
        {
            if (!m_InteractionManagers.Contains(manager))
                return;

            if (manager != null)
            {
                manager.interactableRegistered -= OnInteractableRegistered;
                manager.interactableUnregistered -= OnInteractableUnregistered;
            }

            m_InteractionManagers.Remove(manager);
            Reload();
        }

        void OnInteractableRegistered(InteractableRegisteredEventArgs eventArgs)
        {
            Reload();
        }

        void OnInteractableUnregistered(InteractableUnregisteredEventArgs eventArgs)
        {
            // Skip reloading as each interactable is being destroyed when exiting Play mode
            if (!exitingPlayMode)
                Reload();
        }

        /// <inheritdoc />
#if UNITY_6000_2_OR_NEWER
        protected override TreeViewItem<int> BuildRoot()
#else
        protected override TreeViewItem BuildRoot()
#endif
        {
            // Wrap root control in invisible item required by TreeView.
            return new Item
            {
                id = 0,
                children = BuildInteractableTree(),
                depth = -1,
            };
        }

#if UNITY_6000_2_OR_NEWER
        static List<TreeViewItem<int>> CreateItemsList() => new List<TreeViewItem<int>>();
#else
        static List<TreeViewItem> CreateItemsList() => new List<TreeViewItem>();
#endif

#if UNITY_6000_2_OR_NEWER
        List<TreeViewItem<int>> BuildInteractableTree()
#else
        List<TreeViewItem> BuildInteractableTree()
#endif
        {
            var items = CreateItemsList();
            var interactables = new List<IXRInteractable>();

            foreach (var interactionManager in m_InteractionManagers)
            {
                if (interactionManager == null)
                    continue;

                var rootTreeItem = new Item
                {
                    id = XRInteractionDebuggerWindow.GetUniqueTreeViewId(interactionManager),
                    displayName = XRInteractionDebuggerWindow.GetDisplayName(interactionManager),
                    depth = 0,
                };

                // Build children.
                interactionManager.GetRegisteredInteractables(interactables);
                if (interactables.Count > 0)
                {
                    var children = CreateItemsList();
                    foreach (var interactable in interactables)
                    {
                        var childItem = new Item
                        {
                            id = XRInteractionDebuggerWindow.GetUniqueTreeViewId(interactable),
                            displayName = XRInteractionDebuggerWindow.GetDisplayName(interactable),
                            interactable = interactable,
                            depth = 1,
                            parent = rootTreeItem,
                        };
                        children.Add(childItem);
                    }

                    // Sort children by name.
                    children.Sort((a, b) => string.Compare(a.displayName, b.displayName));
                    rootTreeItem.children = children;
                }

                items.Add(rootTreeItem);
            }

            return items;
        }

        /// <inheritdoc />
        protected override void RowGUI(RowGUIArgs args)
        {
            if (!Application.isPlaying || exitingPlayMode)
                return;

            var item = (Item)args.item;

            var columnCount = args.GetNumVisibleColumns();
            for (var i = 0; i < columnCount; ++i)
            {
                ColumnGUI(args.GetCellRect(i), item, args.GetColumn(i), ref args);
            }
        }

        void ColumnGUI(Rect cellRect, Item item, int column, ref RowGUIArgs args)
        {
            CenterRectUsingSingleLineHeight(ref cellRect);

            if (column == (int)ColumnId.Name)
            {
                args.rowRect = cellRect;
                base.RowGUI(args);
            }

            if (item.interactable != null)
            {
                switch (column)
                {
                    case (int)ColumnId.Type:
                        GUI.Label(cellRect, item.interactable.GetType().Name);
                        break;
                    case (int)ColumnId.LayerMask:
                        GUI.Label(cellRect, XRInteractionDebuggerWindow.GetLayerMaskDisplay(k_LayerSize, item.interactable.interactionLayers.value, k_LayerMaskOn, k_LayerMaskOff));
                        break;
                    case (int)ColumnId.LayerMaskList:
                        var activeLayers = XRInteractionDebuggerWindow.GetActiveLayers(k_LayerSize, item.interactable.interactionLayers.value);
                        GUI.Label(cellRect, string.Join(", ", activeLayers));
                        break;
                    case (int)ColumnId.Colliders:
                        GUI.Label(cellRect, XRInteractionDebuggerWindow.JoinNames(",", item.interactable.colliders));
                        break;
                    case (int)ColumnId.Hovered:
                        if (item.interactable is IXRHoverInteractable hoverable)
                            GUI.Label(cellRect, hoverable.isHovered ? "True" : "False");
                        break;
                    case (int)ColumnId.Selected:
                        if (item.interactable is IXRSelectInteractable selectable)
                            GUI.Label(cellRect, selectable.isSelected ? "True" : "False");
                        break;
                }
            }
        }

        /// <inheritdoc />
        protected override void DoubleClickedItem(int id)
        {
            base.DoubleClickedItem(id);

            EditorGUIUtility.PingObject(id);
            Selection.activeInstanceID = id;
        }
    }
}
