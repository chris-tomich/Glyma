using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace SilverlightMappingToolBasic
{
    public class NodeProxyEventArgs : EventArgs
    {
        public NodeProxyEventArgs()
        {
        }

        public INodeProxy NodeProxy
        {
            get;
            set;
        }
    }

    public class RendererNodesEventArgs : EventArgs
    {
        public RendererNodesEventArgs()
        {
        }

        public RendererNodesEventArgs(NavigatorView view)
        {
            View = view;
        }

        public NavigatorView View
        {
            get;
            private set;
        }
    }

    public class UpdatedRendererNodesEventArgs : EventArgs
    {
        public UpdatedRendererNodesEventArgs()
        {
        }

        public UpdatedRendererNodesEventArgs(NavigatorView view)
        {
            View = view;
        }

        public NavigatorView View
        {
            get;
            private set;
        }
    }

    public interface INodeNavigator
    {
        Guid DomainId { get; }
        INodeProxy FocalNode { get; }
        Guid FocalNodeId { get; }
        INodeService NodeService { get; }
        ThemeManager ThemeManagementObject { get; }
        NavigationHistory History { get; }
        List<MetadataFilter> MetadataFilters { get; set; }

        event EventHandler<NodesEventArgs> GetCurrentNodesCompleted;
        event EventHandler<NodesEventArgs> ConnectNodesCompleted;
        event EventHandler<NodesEventArgs> UpdateNodeMetadataCompleted;
        event EventHandler<NodesEventArgs> DeleteNodeCompleted;
        event EventHandler NavigationCompleted;
        
        void SetCurrentNode(INodeProxy node);
        void SetCurrentNode(Guid nodeId);
        void GetCurrentNodesAsync();

        void PasteNodeClone(Guid copiedNode, Point location);
        void AddNode(INodeTypeProxy nodeType, string originalId, Point location);
        void DeleteNodes(INodeProxy[] nodesToDelete);
        void DeleteRelationships(IRelationshipProxy[] relationshipsToDelete);
        void ConnectNodesAsync(Dictionary<IDescriptorTypeProxy, Guid> nodes, IRelationshipTypeProxy relationshipType, string originalId);
        void ConnectNodesAsync(Dictionary<IDescriptorTypeProxy, Guid> nodes, IRelationshipTypeProxy relationshipType, Point location, string originalId);
        void UpdateNodeMetadataAsync(INodeProxy node, Guid relationshipId, IDescriptorTypeProxy descriptorType, string metadataName, string metadataValue, IMetadataTypeProxy metadataType);

        void GoBack();
        void GoBackTo(Guid nodeId);
    }
}
