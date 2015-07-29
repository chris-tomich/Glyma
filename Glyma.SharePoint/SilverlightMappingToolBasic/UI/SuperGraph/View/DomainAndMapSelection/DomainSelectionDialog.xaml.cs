using System;
using System.Windows;
using System.Windows.Controls;
using SilverlightMappingToolBasic.GlymaSecurityService;
using SilverlightMappingToolBasic.UI.Extensions.Security;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.UI;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.ViewModel;
using Telerik.Windows.Controls;
using TransactionalNodeService.Proxy;
using Proxy = TransactionalNodeService.Proxy;
using SelectionChangedEventArgs = Telerik.Windows.Controls.SelectionChangedEventArgs;
using System.Collections.Generic;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.DomainAndMapSelection
{
    public partial class DomainSelectionDialog : ChildWindow
    {
        private NodeType _mapNodeType;
        private ManagementConsoleDialog _managementConsoleDialog;

        public class ContextInfoTuple : EventArgs
        {
            public INode Node;
            public string Name;
        }

        public DomainSelectionDialog(SecurityManager securityManager)
        {
            InitializeComponent();
            SecurityManager = securityManager;
            if (App.PermissionLevel >= PermissionLevel.Author)
            {
                ManagementConsoleButton.Visibility = Visibility.Visible;
            }
            else
            {
                ManagementConsoleButton.Visibility = Visibility.Collapsed;
            }
        }

        public IMapManager MapManager
        {
            get;
            set;
        }

        public SecurityManager SecurityManager
        {
            get; 
            private set;
        }

        private INode Domain
        {
            get;
            set;
        }

        public INode Map
        {
            get;
            private set;
        }

        public bool IsAuthorMode
        {
            get; 
            set;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            RelatedContentPanelUtil.Instance.IsDomainAndMapSelectionDialogShown = false;
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RelatedContentPanelUtil.Instance.IsDomainAndMapSelectionDialogShown = true;

            DomainSelector.Items.Clear();

            MapManager.QueryDomainsCompleted.RegisterEvent(OnAcquireDomainsCompleted);
            MapManager.QueryDomainsAsync();
        }

        private void DomainSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e != null && e.AddedItems != null && e.AddedItems.Count > 0)
            {
                MapSelector.Items.Clear();
                var selectedItem = e.AddedItems[0] as RadComboBoxItem;
                if (selectedItem != null)
                {
                    Domain = selectedItem.DataContext as INode;

                    if (Domain != null)
                    {
                        MapManager.QueryMapByDomainCompleted.RegisterEvent(Domain.DomainId, OnAcquireNodeByDomainCompleted);
                        MapManager.QueryMapByDomainAsync(Domain.DomainId);
                    }
                }
                
                
                MapSelector.IsEnabled = false;
            }
        }

        #region Load Domains
        private void OnAcquireDomainsCompleted(object sender, Proxy.NodesEventArgs e)
        {
            DomainSelector.Items.Clear();

            List<INode> nodes = new List<INode>(e.Nodes.Values);
            nodes.Sort(new Comparison<INode>(NodeComparison));

            foreach (INode node in nodes)
            {
                var item = new RadComboBoxItem { DataContext = node, Content = node.Metadata.FindMetadata("Name").Value };

                DomainSelector.Items.Add(item);
            }
        }

        private int NodeComparison(INode node1, INode node2)
        {
            int result = 0;
            if (node1 != null && node2 != null)
            {
                IMetadataSet node1Metadata = node1.Metadata.FindMetadata("Name");
                IMetadataSet node2Metadata = node2.Metadata.FindMetadata("Name");
                if (node1Metadata != null && node2Metadata != null)
                {
                    return node1Metadata.Value.CompareTo(node2Metadata.Value);
                }
            }
            return result;
        }

        #endregion

        #region Load Maps
        private void OnAcquireNodeByDomainCompleted(object sender, Proxy.NodesEventArgs e)
        {
            List<INode> nodes = new List<INode>(e.Nodes.Values);
            nodes.Sort(new Comparison<INode>(NodeComparison));

            foreach (var node in nodes)
            {
                if (Equals(node.NodeType, MapNodeType))
                {
                    var item = new RadComboBoxItem { DataContext = node, Content = node.Metadata.FindMetadata("Name").Value };

                    MapSelector.Items.Add(item);

                    MapSelector.IsEnabled = true;
                }
            }
        }
        #endregion

        private void MapSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e != null && e.AddedItems != null && e.AddedItems.Count > 0)
            {
                var selectedItem = e.AddedItems[0] as RadComboBoxItem;
                if (selectedItem != null)
                {
                    Map = selectedItem.DataContext as INode;
                    LoadButton.IsEnabled = true;
                }
            }
            else
            {
                LoadButton.IsEnabled = false;
            }
        }

        #region Common Super Graph Types

        protected NodeType MapNodeType
        {
            get
            {
                if (_mapNodeType == null)
                {
                    _mapNodeType = MapManager.NodeTypes["CompendiumMapNode"];
                }

                return _mapNodeType;
            }
        }

        #endregion

        private void ManagementConsoleButtonOnClick(object sender, RoutedEventArgs e)
        {
            _managementConsoleDialog = new ManagementConsoleDialog(MapManager, SecurityManager);
            _managementConsoleDialog.Closed += ManagementConsoleOnClosed;
            _managementConsoleDialog.LoadMapClicked += ManagementDialogOnLoadMapClicked;
            _managementConsoleDialog.Ref = this;
            _managementConsoleDialog.Show();
            Visibility = Visibility.Collapsed;
        }

        private void ManagementDialogOnLoadMapClicked(object sender, EventArgs e)
        {
            _managementConsoleDialog.Closed -= ManagementConsoleOnClosed;
            _managementConsoleDialog.Close();
            var rootMap = sender as RootMap;
            if (rootMap != null)
            {
                Map = rootMap.Node;
                DialogResult = true;
                IsAuthorMode = rootMap.IsNewMap;
            }
            _managementConsoleDialog = null;
            Close();
        }

        private void ManagementConsoleOnClosed(object sender, EventArgs eventArgs)
        {
            Visibility = Visibility.Visible;
            DomainSelector.Items.Clear();
            MapSelector.Items.Clear();
            MapSelector.IsEnabled = false;
            MapManager.QueryDomainsCompleted.RegisterEvent(OnAcquireDomainsCompleted);
            MapManager.QueryDomainsAsync();
            _managementConsoleDialog = null;
        }
    }
}

