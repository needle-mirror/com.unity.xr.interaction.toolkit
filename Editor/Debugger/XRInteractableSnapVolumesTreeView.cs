using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEditor.XR.Interaction.Toolkit
{
    /// <summary>
    /// Multi-column <see cref="TreeView"/> that shows snap volumes.
    /// </summary>
#if UNITY_6000_2_OR_NEWER
    class XRInteractableSnapVolumesTreeView : TreeView<int>
#else
    class XRInteractableSnapVolumesTreeView : TreeView
#endif
    {
        public static XRInteractableSnapVolumesTreeView Create(List<XRInteractionManager> interactionManagers, ref State treeState, ref MultiColumnHeaderState headerState)
        {
            treeState ??= new State();

            var newHeaderState = CreateHeaderState();
            if (headerState != null)
                MultiColumnHeaderState.OverwriteSerializedFields(headerState, newHeaderState);
            headerState = newHeaderState;

            var header = new MultiColumnHeader(headerState);
            return new XRInteractableSnapVolumesTreeView(interactionManagers, treeState, header);
        }

        const float k_RowHeight = 20f;

#if UNITY_6000_2_OR_NEWER
        class Item : TreeViewItem<int>
#else
        class Item : TreeViewItem
#endif
        {
            public XRInteractableSnapVolume snapVolume;
            public XRInteractionManager interactionManager;
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
            SnapCollider,
            SnapColliderLayer,
            Interactable,
            SnapToCollider,

            // Initially hidden column kept as last column
            HierarchyPath,

            Count,
        }

        static bool exitingPlayMode => EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode;

        readonly List<XRInteractionManager> m_InteractionManagers = new List<XRInteractionManager>();

        readonly HashSet<XRInteractionManager> m_InteractionManagersInitialized = new HashSet<XRInteractionManager>();

        static MultiColumnHeaderState CreateHeaderState()
        {
            var columns = new MultiColumnHeaderState.Column[(int)ColumnId.Count];

            columns[(int)ColumnId.Name] = new MultiColumnHeaderState.Column
            {
                width = 180f,
                minWidth = 80f,
                canSort = false,
                headerContent = EditorGUIUtility.TrTextContent("Name"),
            };
            columns[(int)ColumnId.Type] = new MultiColumnHeaderState.Column
            {
                width = 120f,
                minWidth = 80f,
                canSort = false,
                headerContent = EditorGUIUtility.TrTextContent("Type"),
            };
            columns[(int)ColumnId.SnapCollider] = new MultiColumnHeaderState.Column
            {
                width = 180f,
                minWidth = 80f,
                canSort = false,
                headerContent = EditorGUIUtility.TrTextContent("Snap Collider"),
            };
            columns[(int)ColumnId.SnapColliderLayer] = new MultiColumnHeaderState.Column
            {
                width = 120f,
                minWidth = 80f,
                canSort = false,
                headerContent = EditorGUIUtility.TrTextContent("Snap Collider Layer"),
            };
            columns[(int)ColumnId.Interactable] = new MultiColumnHeaderState.Column
            {
                width = 180f,
                minWidth = 80f,
                canSort = false,
                headerContent = EditorGUIUtility.TrTextContent("Interactable"),
            };
            columns[(int)ColumnId.SnapToCollider] = new MultiColumnHeaderState.Column
            {
                width = 180f,
                minWidth = 80f,
                canSort = false,
                headerContent = EditorGUIUtility.TrTextContent("Snap To Collider"),
            };
            columns[(int)ColumnId.HierarchyPath] = new MultiColumnHeaderState.Column
            {
                width = 540f,
                minWidth = 80f,
                canSort = false,
                headerContent = EditorGUIUtility.TrTextContent("Hierarchy Path"),
            };

            var headerState = new MultiColumnHeaderState(columns)
            {
                // Replace the default visible columns to hide the last Hierarchy Path column by default
                // since it can be somewhat expensive to create the string as it uses recursion.
                visibleColumns = XRInteractionDebuggerWindow.CreateVisibleColumns((int)ColumnId.HierarchyPath),
            };

            return headerState;
        }

        XRInteractableSnapVolumesTreeView(List<XRInteractionManager> managers, State state, MultiColumnHeader header)
            : base(state, header)
        {
            foreach (var manager in managers)
            {
                AddManager(manager);
            }

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

            manager.snapVolumeRegistered += OnSnapVolumeRegistered;
            manager.snapVolumeUnregistered += OnSnapVolumeUnregistered;

            m_InteractionManagers.Add(manager);
            Reload();
        }

        void RemoveManager(XRInteractionManager manager)
        {
            if (!m_InteractionManagers.Contains(manager))
                return;

            if (manager != null)
            {
                manager.snapVolumeRegistered -= OnSnapVolumeRegistered;
                manager.snapVolumeUnregistered -= OnSnapVolumeUnregistered;
            }

            m_InteractionManagers.Remove(manager);
            Reload();
        }

        void OnSnapVolumeRegistered(InteractableSnapVolumeRegisteredEventArgs eventArgs)
        {
            Reload();
        }

        void OnSnapVolumeUnregistered(InteractableSnapVolumeUnregisteredEventArgs eventArgs)
        {
            // Skip reloading as each snap volume is being destroyed when exiting Play mode
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
            var snapVolumes = new List<XRInteractableSnapVolume>();

            foreach (var interactionManager in m_InteractionManagers)
            {
                if (interactionManager == null)
                    continue;

                var rootTreeItem = new Item
                {
                    id = XRInteractionDebuggerWindow.GetUniqueTreeViewId(interactionManager),
                    displayName = m_InteractionManagers.Count > 1 && ComponentLocatorUtility<XRInteractionManager>.componentCache == interactionManager
                        ? $"{XRInteractionDebuggerWindow.GetDisplayName(interactionManager)} <Default>"
                        : XRInteractionDebuggerWindow.GetDisplayName(interactionManager),
                    interactionManager = interactionManager,
                    depth = 0,
                };

                // If this is the first time we've added this manager, expand it.
                if (m_InteractionManagersInitialized.Add(interactionManager))
                    SetExpanded(rootTreeItem.id, true);

                // Build children.
                interactionManager.GetRegisteredSnapVolumes(snapVolumes);
                if (snapVolumes.Count > 0)
                {
                    var children = CreateItemsList();
                    foreach (var snapVolume in snapVolumes)
                    {
                        var childItem = new Item
                        {
                            id = XRInteractionDebuggerWindow.GetUniqueTreeViewId(snapVolume),
                            displayName = XRInteractionDebuggerWindow.GetDisplayName(snapVolume),
                            snapVolume = snapVolume,
                            interactionManager = interactionManager,
                            depth = 1,
                            parent = rootTreeItem,
                        };
                        children.Add(childItem);
                    }

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

            // depth 0 is the manager itself
            if (item.depth == 0 && item.interactionManager != null)
            {
                switch (column)
                {
                    case (int)ColumnId.Name:
                        args.rowRect = cellRect;
                        using (new EditorGUI.DisabledScope(!item.interactionManager.isActiveAndEnabled))
                            base.RowGUI(args);
                        break;
                    case (int)ColumnId.HierarchyPath:
                        GUI.Label(cellRect, SearchUtils.GetHierarchyPath(item.interactionManager.gameObject), XRInteractionDebuggerWindow.Styles.richLabel);
                        break;
                }

                return;
            }

            if (item.snapVolume != null)
            {
                switch (column)
                {
                    case (int)ColumnId.Name:
                        args.rowRect = cellRect;
                        base.RowGUI(args);
                        break;
                    case (int)ColumnId.Type:
                        GUI.Label(cellRect, item.snapVolume.GetType().Name);
                        break;
                    case (int)ColumnId.SnapCollider:
                        if (item.snapVolume.snapCollider != null)
                        {
                            using (new EditorGUI.DisabledScope(!item.snapVolume.snapCollider.enabled))
                                GUI.Label(cellRect, XRInteractionDebuggerWindow.GetDisplayName(item.snapVolume.snapCollider));
                        }
                        break;
                    case (int)ColumnId.SnapColliderLayer:
                        if (item.snapVolume.snapCollider != null)
                            GUI.Label(cellRect, $"{item.snapVolume.snapCollider.gameObject.layer}: {LayerMask.LayerToName(item.snapVolume.snapCollider.gameObject.layer)}");
                        break;
                    case (int)ColumnId.Interactable:
                        if (item.snapVolume.interactable != null)
                        {
                            using (new EditorGUI.DisabledScope(item.snapVolume.interactable is Behaviour unityObject && unityObject != null && !unityObject.isActiveAndEnabled))
                                GUI.Label(cellRect, XRInteractionDebuggerWindow.GetDisplayName(item.snapVolume.interactable));
                        }
                        break;
                    case (int)ColumnId.SnapToCollider:
                        if (item.snapVolume.snapToCollider != null)
                        {
                            using (new EditorGUI.DisabledScope(!item.snapVolume.snapToCollider.enabled))
                                GUI.Label(cellRect, XRInteractionDebuggerWindow.GetDisplayName(item.snapVolume.snapToCollider));
                        }
                        break;
                    case (int)ColumnId.HierarchyPath:
                        GUI.Label(cellRect, SearchUtils.GetHierarchyPath(item.snapVolume.gameObject), XRInteractionDebuggerWindow.Styles.richLabel);
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
