using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using Object = UnityEngine.Object;

namespace UnityEditor.XR.Interaction.Toolkit
{
    /// <summary>
    /// Multi-column <see cref="TreeView"/> that shows Interactors.
    /// </summary>
#if UNITY_6000_2_OR_NEWER
    class XRInteractorsTreeView : TreeView<int>
#else
    class XRInteractorsTreeView : TreeView
#endif
    {
        public static XRInteractorsTreeView Create(List<XRInteractionManager> interactionManagers, ref State treeState, ref MultiColumnHeaderState headerState)
        {
            treeState ??= new State();

            var newHeaderState = CreateHeaderState();
            if (headerState != null)
                MultiColumnHeaderState.OverwriteSerializedFields(headerState, newHeaderState);
            headerState = newHeaderState;

            var header = new MultiColumnHeader(headerState);
            return new XRInteractorsTreeView(interactionManagers, treeState, header);
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
            public IXRInteractor interactor;
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
            LayerMask,
            LayerMaskList,
            HoverActive,
            SelectActive,
            HoverInteractable,
            SelectInteractable,
            ValidTargets,
            Group,
            Parents,

            // Initially hidden column kept as last column
            HierarchyPath,

            Count,
        }

        static bool exitingPlayMode => EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode;

        readonly List<XRInteractionManager> m_InteractionManagers = new List<XRInteractionManager>();

        readonly HashSet<XRInteractionManager> m_InteractionManagersInitialized = new HashSet<XRInteractionManager>();

        readonly List<IXRInteractable> m_Targets = new List<IXRInteractable>();

        readonly List<IXRInteractable> m_ExplicitParents = new List<IXRInteractable>();

        static MultiColumnHeaderState CreateHeaderState()
        {
            var columns = new MultiColumnHeaderState.Column[(int)ColumnId.Count];

            columns[(int)ColumnId.Name] = new MultiColumnHeaderState.Column
            {
                width = 180f,
                minWidth = 60f,
                canSort = false,
                headerContent = EditorGUIUtility.TrTextContent("Name"),
            };
            columns[(int)ColumnId.Type] = new MultiColumnHeaderState.Column
            {
                width = 120f,
                minWidth = 60f,
                canSort = false,
                headerContent = EditorGUIUtility.TrTextContent("Type"),
            };
            columns[(int)ColumnId.LayerMask] = new MultiColumnHeaderState.Column
            {
                width = 240f,
                minWidth = 80f,
                canSort = false,
                headerContent = EditorGUIUtility.TrTextContent("Layer Mask"),
            };
            columns[(int)ColumnId.LayerMaskList] = new MultiColumnHeaderState.Column
            {
                width = 120f,
                minWidth = 80f,
                canSort = false,
                headerContent = EditorGUIUtility.TrTextContent("Layer Mask List"),
            };
            columns[(int)ColumnId.HoverActive] = new MultiColumnHeaderState.Column
            {
                width = 90f,
                canSort = false,
                headerContent = EditorGUIUtility.TrTextContent("Hover Active"),
            };
            columns[(int)ColumnId.SelectActive] = new MultiColumnHeaderState.Column
            {
                width = 90f,
                canSort = false,
                headerContent = EditorGUIUtility.TrTextContent("Select Active"),
            };
            columns[(int)ColumnId.HoverInteractable] = new MultiColumnHeaderState.Column
            {
                width = 140f,
                canSort = false,
                headerContent = EditorGUIUtility.TrTextContent("Hover Interactable"),
            };
            columns[(int)ColumnId.SelectInteractable] = new MultiColumnHeaderState.Column
            {
                width = 140f,
                canSort = false,
                headerContent = EditorGUIUtility.TrTextContent("Select Interactable"),
            };
            columns[(int)ColumnId.ValidTargets] = new MultiColumnHeaderState.Column
            {
                width = 140f,
                canSort = false,
                headerContent = EditorGUIUtility.TrTextContent("Valid Targets"),
            };
            columns[(int)ColumnId.Group] = new MultiColumnHeaderState.Column
            {
                width = 60f,
                canSort = false,
                headerContent = EditorGUIUtility.TrTextContent("Group"),
            };
            columns[(int)ColumnId.Parents] = new MultiColumnHeaderState.Column
            {
                width = 140f,
                canSort = false,
                headerContent = EditorGUIUtility.TrTextContent("Parents"),
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

        XRInteractorsTreeView(List<XRInteractionManager> managers, State state, MultiColumnHeader header)
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

            manager.interactorRegistered += OnInteractorRegistered;
            manager.interactorUnregistered += OnInteractorUnregistered;

            m_InteractionManagers.Add(manager);
            Reload();
        }

        void RemoveManager(XRInteractionManager manager)
        {
            if (!m_InteractionManagers.Contains(manager))
                return;

            if (manager != null)
            {
                manager.interactorRegistered -= OnInteractorRegistered;
                manager.interactorUnregistered -= OnInteractorUnregistered;
            }

            m_InteractionManagers.Remove(manager);
            Reload();
        }

        void OnInteractorRegistered(InteractorRegisteredEventArgs eventArgs)
        {
            Reload();
        }

        void OnInteractorUnregistered(InteractorUnregisteredEventArgs eventArgs)
        {
            // Skip reloading as each interactor is being destroyed when exiting Play mode
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
            var interactors = new List<IXRInteractor>();

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
                interactionManager.GetRegisteredInteractors(interactors);
                if (interactors.Count > 0)
                {
                    var children = CreateItemsList();
                    foreach (var interactor in interactors)
                    {
                        var childItem = new Item
                        {
                            id = XRInteractionDebuggerWindow.GetUniqueTreeViewId(interactor),
                            displayName = XRInteractionDebuggerWindow.GetDisplayName(interactor),
                            interactor = interactor,
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

            if (item.interactor != null)
            {
                var selectInteractor = item.interactor as IXRSelectInteractor;
                var hoverInteractor = item.interactor as IXRHoverInteractor;

                switch (column)
                {
                    case (int)ColumnId.Name:
                        args.rowRect = cellRect;
                        base.RowGUI(args);
                        break;
                    case (int)ColumnId.Type:
                        GUI.Label(cellRect, item.interactor.GetType().Name);
                        break;
                    case (int)ColumnId.LayerMask:
                        GUI.Label(cellRect, XRInteractionDebuggerWindow.GetLayerMaskDisplay(k_LayerSize, item.interactor.interactionLayers.value, k_LayerMaskOn, k_LayerMaskOff));
                        break;
                    case (int)ColumnId.LayerMaskList:
                        var activeLayers = XRInteractionDebuggerWindow.GetActiveLayers(k_LayerSize, item.interactor.interactionLayers.value);
                        GUI.Label(cellRect, string.Join(", ", activeLayers));
                        break;
                    case (int)ColumnId.HoverActive:
                        if (hoverInteractor != null && hoverInteractor.isHoverActive)
                            GUI.Label(cellRect, "True");
                        break;
                    case (int)ColumnId.SelectActive:
                        if (selectInteractor != null && selectInteractor.isSelectActive)
                            GUI.Label(cellRect, "True");
                        break;
                    case (int)ColumnId.HoverInteractable:
                        if (hoverInteractor?.interactablesHovered.Count > 0)
                            GUI.Label(cellRect, XRInteractionDebuggerWindow.JoinNames(",", hoverInteractor.interactablesHovered));
                        break;
                    case (int)ColumnId.SelectInteractable:
                        if (selectInteractor?.interactablesSelected.Count > 0)
                            GUI.Label(cellRect, XRInteractionDebuggerWindow.JoinNames(",", selectInteractor.interactablesSelected));
                        break;
                    case (int)ColumnId.ValidTargets:
                        item.interactor.GetValidTargets(m_Targets);
                        if (m_Targets.Count > 0)
                            GUI.Label(cellRect, XRInteractionDebuggerWindow.JoinNames(",", m_Targets));
                        break;
                    case (int)ColumnId.Group:
                        if (item.interactor is IXRGroupMember groupMember && groupMember.containingGroup != null)
                        {
                            var groupName = groupMember.containingGroup.groupName;
                            if (string.IsNullOrWhiteSpace(groupName) && groupMember.containingGroup is Object unityObject)
                                groupName = unityObject.name;

                            if (!string.IsNullOrEmpty(groupName))
                                GUI.Label(cellRect, groupName);
                        }

                        break;
                    case (int)ColumnId.Parents:
                        item.interactionManager.GetParentRelationships(item.interactor, m_ExplicitParents);
                        if (m_ExplicitParents.Count > 0)
                            GUI.Label(cellRect, XRInteractionDebuggerWindow.JoinNames(",", m_ExplicitParents));
                        break;
                    case (int)ColumnId.HierarchyPath:
                        GUI.Label(cellRect, SearchUtils.GetHierarchyPath(item.interactor.transform.gameObject), XRInteractionDebuggerWindow.Styles.richLabel);
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
