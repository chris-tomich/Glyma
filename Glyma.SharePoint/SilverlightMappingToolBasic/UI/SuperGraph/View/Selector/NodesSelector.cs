using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SilverlightMappingToolBasic.UI.Extensions;
using SilverlightMappingToolBasic.UI.SuperGraph.Controller;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow;
using SilverlightMappingToolBasic.UI.SuperGraph.View.CollapseSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MessageBox;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using SimpleIoC;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Selector
{
    public class NodesSelector : Canvas, INodesSelector
    {
        private IIoCContainer _ioc;
        private List<ISelectableNode> _nodeControls;
        private List<ArrowControl> _arrowControls; 
        private INodeMotionHandler _multipleNodeMotionHandler;
        private MultipleNodesProperties _multipleNodesProperties;

        public Point StartPoint;
        public Point EndPoint;

        public bool IsDrag { get; private set; }

        public ISelectorControl SelectorControl
        {
            get; 
            private set;
        }

        public INodeMotionHandler MultipleNodeMotionHandler {
            get
            {
                if (_multipleNodeMotionHandler == null)
                {
                    _multipleNodeMotionHandler = new MultipleNodeMotionHandler(this);
                }
                return _multipleNodeMotionHandler;
            }
        }

        public IIoCContainer IoC
        {
            get
            {
                if (_ioc == null)
                {
                    _ioc = IoCContainer.GetInjectionInstance();
                }

                return _ioc;
            }
        }

        public MultiNodesContextMenu MultiNodesContextMenu 
        {
            get
            {
                return SelectorControl.MultiNodesContextMenu;
            }
        }

        public List<ISelectableNode> NodeControls
        {
            get
            {
                if (_nodeControls == null)
                {
                    _nodeControls = new List<ISelectableNode>();
                }
                return _nodeControls;
            }
        }

        public List<ArrowControl> ArrowControls
        {
            get
            {
                if (_arrowControls == null)
                {
                    _arrowControls = new List<ArrowControl>();
                }
                return _arrowControls;
            }
        }

        //Check if it is multiselect (more than 1 node are selected)
        public bool IsMultiSelect
        {
            get
            {
                return NodeControls.Count > 1;
            }
        }

        #region multiselect selector part
        public void MouseManagerOnLeftButtonDragReleased(object sender, MouseManagerEventArgs e)
        {
            if(!IsDrag) return;
            Visibility = Visibility.Collapsed;

            // If nothing is pressed, clean selection
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift && (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
            {
                Clear();
            }
            // If Control is pressed, remove selected nodes from selection 
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                var removedNodeControl = false;
                foreach (var child in SelectorControl.NodeContainer.Children)
                {
                    if (child is NodeControl)
                    {
                        var nodeControl = child as NodeControl;
                        if (nodeControl.Visibility == Visibility.Visible)
                        {
                            if (IsPointInSelection(nodeControl.Centre) && nodeControl.ViewModelNode.IsSelected)
                            {
                                RemoveNode(nodeControl);
                                removedNodeControl = true;
                            }
                        }
                    }
                }

                if (!removedNodeControl)
                {
                    foreach (var arrowControl in SelectorControl.GetArrowControls().Where(q => q.IsCollapsed == false))
                    {
                        var viewModelArrow = arrowControl.DataContext as ArrowViewModel;
                        if (viewModelArrow != null)
                        {
                            if (IsArrowInSelection(viewModelArrow.FirstCoordinate, viewModelArrow.SecondCoordinate))
                            {
                                RemoveArrow(arrowControl);
                            }
                        }

                    }
                }
            }
            // Add selected nodes to selection
            else
            {
                foreach (var child in SelectorControl.GetVisibleNodeControls())
                {
                    if (IsPointInSelection(child.Centre))
                    {
                        AddNode(child);
                    }
                }

                if (NodeControls.Count == 0 || ArrowControls.Count > 0)
                {

                    foreach (var arrowControl in SelectorControl.GetArrowControls().Where(q => q.IsCollapsed == false))
                    {
                        var viewModelArrow = arrowControl.DataContext as ArrowViewModel;
                        if (viewModelArrow != null)
                        {
                            if (IsArrowInSelection(viewModelArrow.FirstCoordinate, viewModelArrow.SecondCoordinate))
                            {
                                AddArrow(arrowControl);
                            }
                        }
                    }
                }
            }

            FindRelatedArrows();
            IsDrag = false;
        }

        private void FindRelatedArrows()
        {
            if (NodeControls.Count > 0)
            {
                foreach (var arrowControl in SelectorControl.GetArrowControls())
                {
                    var viewModelArrow = arrowControl.DataContext as ArrowViewModel;
                    if (viewModelArrow != null)
                    {
                        foreach (var nodeControl in NodeControls)
                        {
                            if (viewModelArrow.ViewModelRelationship.From == nodeControl.ViewModelNode.Id || viewModelArrow.ViewModelRelationship.To == nodeControl.ViewModelNode.Id)
                            {
                                AddArrow(arrowControl);
                            }
                        }
                    }
                }
                NodeControls[0].Focus();
            }
        }

        public void MouseManagerOnLeftButtonDragMove(object sender, MouseEventArgs e)
        {
            if (IsDrag)
            {
                EndPoint = e.GetPosition(SelectorControl.NodeContainer);
                UpdateDragSelectionRect(StartPoint, EndPoint);
                Visibility = Visibility.Visible;
            }
        }

        public void MouseManagerOnLeftButtonDragStart(object sender, MouseManagerEventArgs e)
        {
            if (!IoC.GetInstance<ArrowCreationManager>().InMotion)
            {
                StartPoint = e.Start.PositionInformation.GetPosition(SelectorControl.NodeContainer);
                IsDrag = true;
            }
        }

        public void DeleteArrows(UserControl sender = null)
        {
            foreach (var arrow in ArrowControls)
            {
                arrow.IsCollapsed = true;
                var arrowViewModel = arrow.DataContext as ArrowViewModel;

                if (arrowViewModel == null || arrowViewModel.ViewModelRelationship == null)
                {
                    continue;
                }

                if (arrow.From != null)
                {
                    if (arrow.From.ChildNodes.Count == 0)
                    {
                        arrow.From.CollapseControl.State = CollapseState.None;
                    }
                    else if (arrow.From.ChildNodes.All(q => q.IsCollapsed))
                    {
                        arrow.From.CollapseControl.State = CollapseState.Collapsed;
                    }
                }
            }
            IoC.GetInstance<ISuperGraphRelationshipFactory>().DeleteRelationships(ArrowControls.Select(q => q.DataContext)
                .OfType<ArrowViewModel>()
                .Select(q => q.ViewModelRelationship));
        }

        public void DeleteNodes(UserControl sender = null)
        {
            if (HasInvisibleNodesSelected())
            {
                SuperMessageBoxService.ShowWarning("Delete Node(s)",
                    "Some hidden nodes have been selected, are you sure you wish to delete them? \r\n\r\n" +
                    "Press YES to delete them\r\n" +
                    "Press NO to show all hidden nodes",
                    "Yes",
                    "No",
                    "Cancel",
                    DeleteAllNodes,
                    () =>
                    {
                        foreach (var nodeControl in NodeControls)
                        {
                            if (nodeControl.CollapseState == CollapseState.Collapsed ||
                                nodeControl.CollapseState == CollapseState.SemiCollapsed)
                            {
                                var control = nodeControl as NodeControl;
                                if (control != null && control.GetAllChildNodeControls().Any(q => q.ViewModelNode.IsSelected))
                                {
                                    control.CollapseControl.ExpandNodes(new ChildrenControlCollection(control));
                                }
                            }
                        }
                        if (sender != null)
                        {
                            sender.Focus();
                        }
                    },
                    () =>
                    {
                        if (sender != null)
                        {
                            sender.Focus();
                        }
                    });
            }
            else
            {
                if (NodeControls.Any(q => q.CollapseState == CollapseState.Collapsed || q.CollapseState == CollapseState.SemiCollapsed))
                {
                    SuperMessageBoxService.ShowWarning("Delete Node(s)",
                    "The selected node(s) have hidden related node(s), are you sure you wish to delete your selection? \r\n\r\n" +
                    "Press YES to delete the selected node(s) then show the hidden node(s)\r\n" +
                    "Press NO to just show the hidden node(s)",
                    "Yes",
                    "No",
                    "Cancel",
                        () =>
                        {
                            DeleteAllNodes();
                            SelectorControl.RecheckIncorrectVisibility();
                            SelectorControl.ReScanForCollapseStates();
                        },
                        () =>
                        {
                            foreach (var nodeControl in NodeControls)
                            {
                                if (nodeControl.CollapseState == CollapseState.Collapsed ||
                                    nodeControl.CollapseState == CollapseState.SemiCollapsed)
                                {
                                    var control = nodeControl as NodeControl;
                                    if (control != null)
                                    {
                                        control.CollapseControl.ExpandNodes(new ChildrenControlCollection(control, false, true));
                                    }
                                }
                            }
                            if (sender != null)
                            {
                                sender.Focus();
                            }
                        },
                        () =>
                        {
                            if (sender != null)
                            {
                                sender.Focus();
                            }
                        });
                }
                else
                {
                    DeleteAllNodes();
                }
            }


            
        }

        public void Delete()
        {
            if (NodeControls.Count > 0)
            {
                DeleteNodes();
            }
            else
            {
                DeleteArrows();
            }
        }

        private void DeleteAllNodes()
        {
            if (NodeControls.Any(q => q.ViewModelNode.IsMapNode))
            {
                SuperMessageBoxService.ShowWarning("Delete Node(s)",
                    "You have selected map node(s) to be deleted. All content within the map node(s) will be deleted." +
                    "Press YES to delete",
                    "Yes",
                    "Cancel",
                        () =>
                        {
                            var deleteNodeCollection = GetSelectedNodesCollection();
                            IoC.GetInstance<ISuperGraphRelationshipFactory>().DeleteNodes(deleteNodeCollection.Nodes.ToList(), SelectorControl.Context, true);
                        });
            }
            else
            {
                var deleteNodeCollection = GetSelectedNodesCollection();
                IoC.GetInstance<ISuperGraphRelationshipFactory>().DeleteNodes(deleteNodeCollection.Nodes.ToList(), SelectorControl.Context, true);
            }
        }

        public void Initialise(ISelectorControl parent)
        {
            SelectorControl = parent;
            MultiNodesContextMenu.CopyClicked += OnCopyClicked;
            MultiNodesContextMenu.CutClicked += OnCutClicked;
            MultiNodesContextMenu.DeleteClicked += OnDeleteClicked;
            MultiNodesContextMenu.CloneClicked += OnCloneClicked;
            MultiNodesContextMenu.NodePropertiesClicked += OnNodePropertiesClicked;
            MultiNodesContextMenu.ExportClicked += MultiNodesContextMenuOnExportClicked;
            MultiNodesContextMenu.ChangeNodeTypeClicked += MultiNodesContextMenuOnChangeNodeTypeClicked;
            SelectorControl.MapMoved += OnMapMoved;
        }

        private void MultiNodesContextMenuOnChangeNodeTypeClicked(object sender, ChangeNodeTypeEventArgs e)
        {
            if (NodeControls.Any())
            {
                if (NodeControls.Any(q => q.ViewModelNode.IsMapNode))
                {
                    SuperMessageBoxService.ShowWarning("Change Node Type",
                        "You are changing a map node, you might lose any content within the map. Are you sure you wish to continue? \r\n\r\n" +
                        "Press YES to change node type anyway\r\n" +
                        "Press NO to Cancel",
                        "Yes",
                        "No",
                        () =>
                        {
                            foreach (var selectableNode in NodeControls)
                            {
                                IoC.GetInstance<ISuperGraphNodeFactory>().UpdateNode(selectableNode.ViewModelNode, e.ChangedTo);
                            }
                        });
                }
                else
                {
                    foreach (var selectableNode in NodeControls)
                    {
                        IoC.GetInstance<ISuperGraphNodeFactory>().UpdateNode(selectableNode.ViewModelNode, e.ChangedTo);
                    }
                }
            }
        }

        private void MultiNodesContextMenuOnExportClicked(object sender, ExportClickedEventArgs exportClickedEventArgs)
        {
            
        }

        private void OnMapMoved(object sender, MoveTransformEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                StartPoint.X += e.X;
                StartPoint.Y += e.Y;
                UpdateDragSelectionRect(StartPoint, EndPoint);
            }
            
        }

        private bool IsArrowInSelection(Point p1, Point p2)
        {
            var start = StartPoint;
            var end = EndPoint;
            start.X -= SelectorControl.MoveGraphTransform.X;
            start.Y -= SelectorControl.MoveGraphTransform.Y;
            end.X -= SelectorControl.MoveGraphTransform.X;
            end.Y -= SelectorControl.MoveGraphTransform.Y;
            return MathExtension.IsLineHasIntersectWithRectangle(p1, p2, start, end);
        }

        private bool IsPointInSelection(Point p)
        {
            var start = StartPoint;
            var end = EndPoint;
            start.X -= SelectorControl.MoveGraphTransform.X;
            start.Y -= SelectorControl.MoveGraphTransform.Y;
            end.X -= SelectorControl.MoveGraphTransform.X;
            end.Y -= SelectorControl.MoveGraphTransform.Y;
            return p.IsPointInRectangle(start, end);
        }

        private void UpdateDragSelectionRect(Point pt1, Point pt2)
        {
            var x = pt1.X;
            var y = pt1.Y;
            double width, height;

            if (pt2.X < x)
            {
                x = pt2.X;
                width = pt1.X - pt2.X;
            }
            else
            {
                width = pt2.X - x;
            }

            if (pt2.Y < y)
            {
                y = pt2.Y;
                height = pt1.Y - pt2.Y;
            }
            else
            {
                height = pt2.Y - y;
            }

            SetLeft(SelectorControl.DragSelectionBorder, x * SelectorControl.Zoom);
            SetTop(SelectorControl.DragSelectionBorder, y * SelectorControl.Zoom);
            SelectorControl.DragSelectionBorder.Width = width * SelectorControl.Zoom;
            SelectorControl.DragSelectionBorder.Height = height * SelectorControl.Zoom;
        }

        #endregion

        public void SelectAllChildsOfNode(NodeControl nodeControl)
        {
            foreach (var control in nodeControl.GetAllNodeControls())
            {
                AddNode(control, false);
            }
        }

        public void DeSelectAllChildsOfNode(NodeControl nodeControl)
        {
            foreach (var control in nodeControl.GetAllNodeControls())
            {
                RemoveNode(control);
            }
        }


        

        //Clear selection
        public void Clear()
        {
            foreach (var nodeControl in NodeControls)
            {
                nodeControl.DeSelect();
            }
            NodeControls.Clear();

            foreach (var arrowControl in ArrowControls)
            {
                arrowControl.DeSelect();
            }
            ArrowControls.Clear();
        }

        public bool HasInvisibleNodesSelected()
        {
            return NodeControls.Any(q => q.IsCollapsed);
        }

        //Add node to selection
        public void AddNode(ISelectableNode nodeControl, bool isShowDetails = true)
        {
            if (!HasNode(nodeControl))
            {
                NodeControls.Add(nodeControl);
                nodeControl.Select(isShowDetails);
            }
        }

        //Remove node from selection
        public void RemoveNode(ISelectableNode nodeControl)
        {
            if (HasNode(nodeControl))
            {
                NodeControls.Remove(nodeControl);
                nodeControl.DeSelect();
            }
        }

        //Add node to selection
        public void AddArrow(ArrowControl arrowControl)
        {
            if (!HasArrow(arrowControl))
            {
                ArrowControls.Add(arrowControl);
                arrowControl.Select();
            }
        }

        //Remove node from selection
        public void RemoveArrow(ArrowControl arrowControl)
        {
            if (HasArrow(arrowControl))
            {
                ArrowControls.Remove(arrowControl);
                arrowControl.DeSelect();
            }
        }

        //Check if node has been selected
        public bool HasNode(ISelectableNode nodeControl)
        {
            return NodeControls.Contains(nodeControl);
        }

        public bool HasArrow(ArrowControl arrowControl)
        {
            return ArrowControls.Contains(arrowControl);
        }

        private SelectedNodesCollection GetSelectedNodesCollection()
        {
            var viewModelNodes = NodeControls.Select(q => q.ViewModelNode);
            var relationships = SelectorControl.GetRelationships();
            return new SelectedNodesCollection(viewModelNodes, relationships);
        }

        #region operations on multi nodes
        private void OnCopyClicked(object sender, EventArgs eventArgs)
        {
            //Get all viewmodelnodes of selected nodecontrol
            IoC.GetInstance<ISuperGraphRelationshipFactory>().IsSameDomain = true;
            var copiedNodeCollection = GetSelectedNodesCollection();
            SelectorControl.NodeOperationDetails = new NodeOperationDetails(NodeOperation.CopyMultiNodes,
                SelectorControl.Context, copiedNodeCollection.Nodes, copiedNodeCollection.RelatedRelationships);
        }

        public void Copy()
        {
            OnCopyClicked(this, null);
        }

        public void Cut()
        {
            OnCutClicked(this, null);
        }

        private void OnCutClicked(object sender, EventArgs eventArgs)
        {
            foreach (var child in SelectorControl.NodeContainer.Children)
            {
                if (child is ICutableNode)
                {
                    var nodeControl = child as ICutableNode;
                    if (nodeControl.ViewModelNode.IsSelected)
                    {
                        nodeControl.SetCutState();
                    }
                    else
                    {
                        nodeControl.ReleaseCutState();
                    }
                }
            }

            var cutNodeCollection = GetSelectedNodesCollection();
            IoC.GetInstance<ISuperGraphRelationshipFactory>().IsSameDomain = true;
            SelectorControl.NodeOperationDetails = new NodeOperationDetails(NodeOperation.CutMultiNodes,
                SelectorControl.Context, cutNodeCollection.Nodes, cutNodeCollection.RelatedRelationships); 
        }

        private void OnDeleteClicked(object sender, EventArgs eventArgs)
        {
            DeleteNodes();
        }


        private void OnCloneClicked(object sender, EventArgs eventArgs)
        {
            var nodes = NodeControls.Select(q => q.ViewModelNode).ToList();
            var relationships = new List<Relationship>();
            foreach (var relationship in SelectorControl.GetRelationships())
            {
                if (nodes.Any(n => n.Id == relationship.From) || nodes.Any(n => n.Id == relationship.To))
                {
                    relationships.Add(relationship);
                }
            }
            Clear();
            IoC.GetInstance<ISuperGraphNodeFactory>().CloneNodes(nodes, relationships);
        }

        private void OnNodePropertiesClicked(object sender, EventArgs e)
        {
            _multipleNodesProperties = new MultipleNodesProperties(NodeControls.Select(q => q.ViewModelNode.NodeProperties).ToList());
            SelectorControl.NodePropertiesDialog.DataContext = _multipleNodesProperties;
            SelectorControl.NodePropertiesDialog.Closed += NodesPropertiesDialogOnClosed;
            SelectorControl.NodePropertiesDialog.Show();
        }

        private void NodesPropertiesDialogOnClosed(object sender, EventArgs e)
        {
            SelectorControl.NodePropertiesDialog.Closed -= NodesPropertiesDialogOnClosed;
            foreach (var nodeControl in NodeControls)
            {
                nodeControl.RefreshNodeControl();
            }
        }

        

        #endregion
    }
}