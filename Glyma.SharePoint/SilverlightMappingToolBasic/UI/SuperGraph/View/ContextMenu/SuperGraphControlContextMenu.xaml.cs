using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SilverlightMappingToolBasic.UI.Extensions.Security;
using SilverlightMappingToolBasic.UI.SuperGraph.Controller;
using SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu.Base;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MessageBox;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;
using SimpleIoC;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu
{
    public partial class SuperGraphControlContextMenu : ContextMenuBase
    {
        private IIoCContainer _ioc;
        private NodeOperationDetails _nodeOperationDetails;
        

        public event EventHandler<CommitCollapseStatesEventArgs> CommitCollapseStatesClicked;

        public event EventHandler<CreateNewNodeClickedEventArgs> CreateNewNodeClicked;

        public event EventHandler ImportClicked;

        public SuperGraphControlContextMenu()
        {
            InitializeComponent();
        }

        public IContextMenuParent ContextMenuParent { get; set; }

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

        public NodeOperationDetails OperationDetails
        {
            get
            {
                return _nodeOperationDetails;
            }
            set
            {
                _nodeOperationDetails = value;
                if (_nodeOperationDetails.Nodes!= null)
                {
                    ContextMenuPaste.Disabled = false;
                }
                else
                {
                    ContextMenuPaste.Disabled = true;
                }
            }
        }

        public Point Location
        {
            get;
            set;
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            object focussedElement = FocusManager.GetFocusedElement();

            if (focussedElement != this && focussedElement != PopupMenu && !PopupMenu.Items.Contains(focussedElement)
                && focussedElement != CreateNewSubMenu && !CreateNewSubMenu.Items.Contains(focussedElement)
                && focussedElement != SaveCollapseStateSubMenu && !SaveCollapseStateSubMenu.Items.Contains(focussedElement))
            {
                Visibility = Visibility.Collapsed;
                CreateNewSubMenu.Visibility = Visibility.Collapsed;
                SaveCollapseStateSubMenu.Visibility = Visibility.Collapsed;
            }

            var item = focussedElement as SuperContextMenuItem;
            if (item != null)
            {
                if (!item.HasSubMenu && PopupMenu.Items.Contains(item))
                {
                    CreateNewSubMenu.Visibility = Visibility.Collapsed;
                    SaveCollapseStateSubMenu.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ContextMenuMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Visibility = Visibility.Collapsed;

            if (CreateNewNodeClicked != null)
            {
                CreateNewNodeClicked(sender, new CreateNewNodeClickedEventArgs { NodeType = "Map", Location = Location });
            }
        }

        private void ContextMenuQuestion_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Visibility = Visibility.Collapsed;

            if (CreateNewNodeClicked != null)
            {
                CreateNewNodeClicked(sender, new CreateNewNodeClickedEventArgs { NodeType = "Question", Location = Location });
            }
        }

        private void ContextMenuIdea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Visibility = Visibility.Collapsed;

            if (CreateNewNodeClicked != null)
            {
                CreateNewNodeClicked(sender, new CreateNewNodeClickedEventArgs { NodeType = "Idea", Location = Location });
            }
        }

        private void ContextMenuPro_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Visibility = Visibility.Collapsed;

            if (CreateNewNodeClicked != null)
            {
                CreateNewNodeClicked(sender, new CreateNewNodeClickedEventArgs { NodeType = "Pro", Location = Location });
            }
        }

        private void ContextMenuCon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Visibility = Visibility.Collapsed;

            if (CreateNewNodeClicked != null)
            {
                CreateNewNodeClicked(sender, new CreateNewNodeClickedEventArgs { NodeType = "Con", Location = Location });
            }
        }

        private void ContextDecisionMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Visibility = Visibility.Collapsed;

            if (CreateNewNodeClicked != null)
            {
                CreateNewNodeClicked(sender, new CreateNewNodeClickedEventArgs { NodeType = "Decision", Location = Location });
            }
        }

        private void ContextNoteMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Visibility = Visibility.Collapsed;

            if (CreateNewNodeClicked != null)
            {
                CreateNewNodeClicked(sender, new CreateNewNodeClickedEventArgs { NodeType = "Note", Location = Location });
            }
        }

        public void Paste(Point location)
        {
            Location = location;
            ContextMenuPaste_MouseLeftButtonUp(this, null);
        }

        private void ContextMenuPaste_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Visibility = Visibility.Collapsed;

            if (OperationDetails.Operation != NodeOperation.None && OperationDetails.Map != null && OperationDetails.Nodes != null )
            {
                if (IoC.GetInstance<ISuperGraphRelationshipFactory>().IsSameDomain)
                {
                    var currentNodeControls = ContextMenuParent.GetNodeControls().ToList();
                    var operateNodes = OperationDetails.Nodes.ToList();
                    foreach (var nodeControl in currentNodeControls)
                    {
                        if (operateNodes.Any(q => q.Id == nodeControl.ViewModelNode.Id))
                        {
                            operateNodes.RemoveAll(q => q.Id == nodeControl.ViewModelNode.Id);
                        }
                    }

                    var currentRelationships = ContextMenuParent.GetRelationships().ToList();
                    var operateRelationships = OperationDetails.Relationships.ToList();
                    foreach (var relationship in currentRelationships)
                    {
                        if (operateRelationships.Any(q => q.From == relationship.From && q.To == relationship.To))
                        {
                            operateRelationships.RemoveAll(q => q.From == relationship.From && q.To == relationship.To);
                        }
                    }

                    switch (OperationDetails.Operation)
                    {
                        case NodeOperation.None:
                            break;
                        case NodeOperation.Cut:
                            if (!OperationDetails.Map.Proxy.Id.Equals(ContextMenuParent.Context.Proxy.Id))
                            {
                                IoC.GetInstance<ISuperGraphRelationshipFactory>().TranscludeNode(operateNodes, OperationDetails.Map, Location);
                                IoC.GetInstance<ISuperGraphRelationshipFactory>().DeleteTranscludedNode(operateNodes, OperationDetails.Map, true);
                            }
                            break;
                        case NodeOperation.Copy:
                            IoC.GetInstance<ISuperGraphRelationshipFactory>().TranscludeNode(operateNodes, OperationDetails.Map, Location);
                            break;
                        case NodeOperation.Clone:
                            break;
                        case NodeOperation.CopyMultiNodes:
                            IoC.GetInstance<ISuperGraphRelationshipFactory>().TranscludeNodes(operateNodes, OperationDetails.Map, Location, OperationDetails.OldLocation);
                            IoC.GetInstance<ISuperGraphRelationshipFactory>().ConnectMultipleTranscludedNodes(operateRelationships, OperationDetails.Nodes.ToList(), OperationDetails.Map);
                            break;
                        case NodeOperation.CutMultiNodes:
                            if (!OperationDetails.Map.Proxy.Id.Equals(ContextMenuParent.Context.Proxy.Id))
                            {
                                IoC.GetInstance<ISuperGraphRelationshipFactory>().TranscludeNodes(operateNodes, OperationDetails.Map, Location, OperationDetails.OldLocation);
                                IoC.GetInstance<ISuperGraphRelationshipFactory>().ConnectMultipleTranscludedNodes(operateRelationships, OperationDetails.Nodes.ToList(), OperationDetails.Map);
                                IoC.GetInstance<ISuperGraphRelationshipFactory>().DeleteNodes(operateNodes, OperationDetails.Map, false);
                            }
                            break;
                    }
                }
                else
                {
                    SuperMessageBoxService.ShowWarning("Paste Node(s)",
                        "You are attempting to copy/cut node(s) from another map/project. \r\n\r\n" +
                        "Press YES to clone the node(s) into this map instead\r\n" +
                        "Press NO to cancel",
                        "Yes","No",
                        () =>
                        {
                            var count = OperationDetails.Nodes.Count();
                            if (count > 1)
                            {
                                IoC.GetInstance<ISuperGraphNodeFactory>().CloneNodes(OperationDetails.Nodes, OperationDetails.Relationships);
                            }
                            else if (count == 1)
                            {
                                IoC.GetInstance<ISuperGraphNodeFactory>().CloneNode(OperationDetails.Nodes.First());
                            }
                        });
                }
            }
        }

        public void Paste()
        {
            ContextMenuPaste_MouseLeftButtonUp(this, null);
        }

        private void ContextMenuSaveCollapseState_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
            //IoC.GetInstance<ISuperGraphNodeBatchOperations>().CommitCollapseStates();
        }

        private void ContextMenuNew_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowSubMenu(CreateNewSubMenu);
            SaveCollapseStateSubMenu.Visibility = Visibility.Collapsed;
        }

        private void CreateNewSubMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Focus();
                CreateNewSubMenu.Visibility = Visibility.Collapsed;
            }
        }


        private void SaveCollapseStateSubMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            Focus();
            SaveCollapseStateSubMenu.Visibility = Visibility.Collapsed;
        }

        private void SaveViewerCollapseState_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            if (CommitCollapseStatesClicked != null)
            {
                CommitCollapseStatesClicked(sender, new CommitCollapseStatesEventArgs { Group = PermissionLevel.Reader });
            }
        }

        private void SaveAuthorCollapseState_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            if (CommitCollapseStatesClicked != null)
            {
                CommitCollapseStatesClicked(sender, new CommitCollapseStatesEventArgs { Group = PermissionLevel.Author });
            }
        }

        private void SaveCollapseState_OnMouseEnter(object sender, MouseEventArgs e)
        {
            ShowSubMenu(SaveCollapseStateSubMenu);
            CreateNewSubMenu.Visibility = Visibility.Collapsed;
        }

        private void ContextMenuBase_GotFocus(object sender, RoutedEventArgs e)
        {
            ContextMenuPaste.Disabled = OperationDetails == null;
        }

        private void ContextMenuImport_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            if (ImportClicked != null)
            {
                ImportClicked(sender, null);
            }
        }
    }
}
