using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using SilverlightMappingToolBasic;

namespace SilverlightMappingToolBasic.Controls
{
    public class RelationshipContextMenu : ContextMenu
    {
        public RelationshipContextMenu(IRelationshipProxy relationshipProxy, IMapControl map, INodeService service)
            : base()
        {
            RelationshipProxy = relationshipProxy;
            NodeService = service;
            Navigator = map.Navigator;

            this.Loaded += new RoutedEventHandler(NodeContextMenu_Loaded);
        }

        private IRelationshipProxy RelationshipProxy
        {
            get;
            set;
        }

        private INodeService NodeService
        {
            get;
            set;
        }

        private INodeNavigator Navigator
        {
            get;
            set;
        }

        private void NodeContextMenu_Loaded(object sender, RoutedEventArgs e)
        {
            MenuItem deleteRelationshipMenuItem = new MenuItem();
            deleteRelationshipMenuItem.Header = "Delete Relationship";
            deleteRelationshipMenuItem.Click += new RoutedEventHandler(deleteRelationshipMenuItem_Click);
            this.Items.Add(deleteRelationshipMenuItem);
        }

        private void deleteRelationshipMenuItem_Click(object sender, RoutedEventArgs e)
        {
            NodeService.DeleteRelationship(Navigator.DomainId, RelationshipProxy.Id);
        }
    }
}
