using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEditor.XR.Interaction.Toolkit
{
    /// <summary>
    /// Multi-column <see cref="TreeView"/> that shows Interactors.
    /// </summary>
    class XRInteractorsTreeView : TreeView
    {
        public static XRInteractorsTreeView Create(List<XRInteractionManager> interactionManagers, ref TreeViewState treeState, ref MultiColumnHeaderState headerState)
        {
            if (treeState == null)
                treeState = new TreeViewState();

            var newHeaderState = CreateHeaderState();
            if (headerState != null)
                MultiColumnHeaderState.OverwriteSerializedFields(headerState, newHeaderState);
            headerState = newHeaderState;

            var header = new MultiColumnHeader(headerState);
            return new XRInteractorsTreeView(interactionManagers, treeState, header);
        }

        const float k_RowHeight = 20f;

        class Item : TreeViewItem
        {
            public XRBaseInteractor interactor;
        }

        enum ColumnId
        {
            Name,
            Type,
            HoverActive,
            SelectActive,
            HoverInteractable,
            SelectInteractable,
            ValidTargets,

            COUNT
        }

        readonly List<XRInteractionManager> m_InteractionManagers = new List<XRInteractionManager>();

        readonly List<XRBaseInteractable> m_Targets = new List<XRBaseInteractable>();

        static MultiColumnHeaderState CreateHeaderState()
        {
            var columns = new MultiColumnHeaderState.Column[(int)ColumnId.COUNT];

            columns[(int)ColumnId.Name] = new MultiColumnHeaderState.Column
            {
                width = 180f,
                minWidth = 60f,
                headerContent = EditorGUIUtility.TrTextContent("Name"),
            };
            columns[(int)ColumnId.Type] = new MultiColumnHeaderState.Column
            {
                width = 120f,
                minWidth = 60f,
                headerContent = EditorGUIUtility.TrTextContent("Type"),
            };
            columns[(int)ColumnId.HoverActive] = new MultiColumnHeaderState.Column
            {
                width = 120f,
                headerContent = EditorGUIUtility.TrTextContent("Hover Active"),
            };
            columns[(int)ColumnId.SelectActive] = new MultiColumnHeaderState.Column
            {
                width = 120f,
                headerContent = EditorGUIUtility.TrTextContent("Select Active"),
            };
            columns[(int)ColumnId.HoverInteractable] = new MultiColumnHeaderState.Column
            {
                width = 140f,
                headerContent = EditorGUIUtility.TrTextContent("Hover Interactable"),
            };
            columns[(int)ColumnId.SelectInteractable] = new MultiColumnHeaderState.Column
            {
                width = 140f,
                headerContent = EditorGUIUtility.TrTextContent("Select Interactable"),
            };
            columns[(int)ColumnId.ValidTargets] = new MultiColumnHeaderState.Column
            {
                width = 140f,
                headerContent = EditorGUIUtility.TrTextContent("Valid Targets"),
            };

            return new MultiColumnHeaderState(columns);
        }

        XRInteractorsTreeView(List<XRInteractionManager> managers, TreeViewState state, MultiColumnHeader header)
            : base(state, header)
        {
            foreach(var manager in managers)
                AddManager(manager);
            showBorder = false;
            rowHeight = k_RowHeight;
            Reload();
        }

        public void UpdateManagersList(List<XRInteractionManager> currentManagers)
        {
            bool managerListChanged = false;

            // Check for Removal
            for (int i = 0; i < m_InteractionManagers.Count; i++)
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

            if(managerListChanged)
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

        void OnInteractorRegistered(InteractorRegisteredEventArgs eventArgs) { Reload(); }

        void OnInteractorUnregistered(InteractorUnregisteredEventArgs eventArgs) { Reload(); }

        /// <inheritdoc />
        protected override TreeViewItem BuildRoot()
        {
            // Wrap root control in invisible item required by TreeView.
            return new Item
            {
                id = 0,
                children = BuildInteractableTree(),
                depth = -1,
            };
        }

        List<TreeViewItem> BuildInteractableTree()
        {
            var items = new List<TreeViewItem>();
            var interactors = new List<XRBaseInteractor>();

            foreach (var interactionManager in m_InteractionManagers)
            {
                if (interactionManager == null)
                    continue;

                var rootTreeItem = new Item
                {
                    id = interactionManager.GetInstanceID(),
                    displayName = interactionManager.name,
                    depth = 0,
                };

                // Build children.
                interactionManager.GetRegisteredInteractors(interactors);
                if (interactors.Count > 0)
                {
                    var children = new List<TreeViewItem>();
                    foreach (var interactor in interactors)
                    {
                        var childItem = new Item
                        {
                            id = interactor.GetInstanceID(),
                            displayName = interactor.name,
                            interactor = interactor,
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

            if (item.interactor != null)
            {
                switch (column)
                {
                    case (int)ColumnId.Type:
                        GUI.Label(cellRect, item.interactor.GetType().Name);
                        break;
                    case (int)ColumnId.HoverActive:
                        GUI.Label(cellRect, item.interactor.isHoverActive.ToString());
                        break;
                    case (int)ColumnId.SelectActive:
                        GUI.Label(cellRect, item.interactor.isSelectActive.ToString());
                        break;
                    case (int)ColumnId.HoverInteractable:
                        item.interactor.GetHoverTargets(m_Targets);
                        if (m_Targets.Count > 0)
                            GUI.Label(cellRect, XRInteractionDebuggerWindow.JoinNames(",", m_Targets));
                        break;
                    case (int)ColumnId.SelectInteractable:
                        if (item.interactor.selectTarget != null)
                            GUI.Label(cellRect, item.interactor.selectTarget.name);
                        break;
                    case (int)ColumnId.ValidTargets:
                        item.interactor.GetValidTargets(m_Targets);
                        if (m_Targets.Count > 0)
                            GUI.Label(cellRect, XRInteractionDebuggerWindow.JoinNames(",", m_Targets));
                        break;
                }
            }
        }
    }
}
