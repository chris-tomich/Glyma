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
using SilverlightMappingToolBasic.MappingService;
using System.Collections.Generic;

namespace SilverlightMappingToolBasic
{
    public class ReturnedNodesEventArgs : EventArgs
    {
        public ReturnedNodesEventArgs()
        {
        }

        public ReturnedNodesEventArgs(Guid searchedNode, INodeProxy[] resultNodes)
        {
            Nodes = resultNodes;
            SearchedNode = searchedNode;
        }

        public INodeProxy[] Nodes
        {
            get;
            set;
        }

        public Guid SearchedNode
        {
            get;
            set;
        }
    }

    public class ReturnedMapNodesEventArgs : EventArgs
    {
        private List<INodeProxy> _nodes = new List<INodeProxy>();

        public ReturnedMapNodesEventArgs()
        {
        }

        public ReturnedMapNodesEventArgs(GetDomainMapNodesCompletedEventArgs results)
        {
            DomainId = results.Result.SearchedNode;

            foreach (SoapNode node in results.Result.Nodes.Values)
            {
                _nodes.Add(new NodeProxy(node));
            }
        }

        public Guid DomainId
        {
            get;
            private set;
        }

        public INodeProxy[] Nodes
        {
            get
            {
                return _nodes.ToArray();
            }
        }
    }

    public class ReturnedTypesEventArgs : EventArgs
    {
        public ReturnedTypesEventArgs()
        {
        }

        public INodeTypeProxy[] NodeTypes
        {
            get;
            set;
        }

        public IDescriptorTypeProxy[] DescriptorTypes
        {
            get;
            set;
        }

        public IRelationshipTypeProxy[] RelationshipTypes
        {
            get;
            set;
        }

        public IMetadataTypeProxy[] MetadataTypes
        {
            get;
            set;
        }
    }

    public class AddedNodeEventArgs : EventArgs 
    {
        public AddedNodeEventArgs() 
        {
        }

        public AddedNodeEventArgs(INodeProxy node, Point location) 
        {
            Node = node;
            Location = location;
        }

        public INodeProxy Node 
        {
            get;
            private set;
        }

        public Point Location
        {
            get;
            private set;
        }
    }

    public class ConnectedNodesEventArgs : EventArgs
    {
        public ConnectedNodesEventArgs()
        {
        }

        public INodeProxy[] Nodes
        {
            get;
            set;
        }

        public RelationshipProxy Relationship
        {
            get;
            set;
        }
    }

    public class ReturnedNodeIdEventArgs : EventArgs
    {
        public ReturnedNodeIdEventArgs()
        {
        }

        public ReturnedNodeIdEventArgs(Guid nodeId)
        {
            NodeId = nodeId;
        }

        public Guid NodeId
        {
            get;
            set;
        }
    }

    public class ReturnedNodeIdsEventArgs : EventArgs
    {
        public ReturnedNodeIdsEventArgs()
        {
        }

        public ReturnedNodeIdsEventArgs(Dictionary<string, Guid> nodeIds)
        {
            NodeIds = nodeIds;
        }

        public Dictionary<string, Guid> NodeIds
        {
            get;
            set;
        }
    }

    public class DeleteEventArgs : EventArgs
    {
        public DeleteEventArgs()
        {
        }

        public DeleteEventArgs(bool deleteSuccessful, Guid deletedId)
        {
            DeleteSuccessful = deleteSuccessful;
            DeletedId = deletedId;
        }

        public bool DeleteSuccessful
        {
            get;
            set;
        }

        public Guid DeletedId
        {
            get;
            set;
        }
    }

    public class ResultEventArgs : EventArgs
    {
        public ResultEventArgs(bool result)
        {
            WasSuccess = result;
        }

        public bool WasSuccess
        {
            get;
            set;
        }
    }

    public interface INodeService
    {
        event EventHandler<ReturnedTypesEventArgs> GetAllSoapTypesCompleted;
        event EventHandler<ReturnedNodesEventArgs> GetNodeByIdCompleted;
        event EventHandler<ReturnedNodesEventArgs> GetRelatedNodesByIdCompleted;
        event EventHandler<ReturnedNodesEventArgs> GetMapsNodesCompleted;
        event EventHandler<AddedNodeEventArgs> PasteNodeCloneCompleted;
        event EventHandler<AddedNodeEventArgs> AddNodeCompleted;
        event EventHandler<ConnectedNodesEventArgs> ConnectNodesCompleted;
        event EventHandler<ReturnedNodesEventArgs> UpdateNodeMetadataCompleted;
        event EventHandler<ReturnedNodeIdsEventArgs> GetDomainNodeIdsCompleted;
        event EventHandler<ReturnedNodeIdEventArgs> GetDomainNodeIdCompleted;
        event EventHandler<ReturnedMapNodesEventArgs> GetDomainMapNodesCompleted;
        event EventHandler<DeleteEventArgs> DeleteNodeCompleted;
        event EventHandler<DeleteEventArgs> DeleteRelationshipCompleted;
        event EventHandler<ReturnedNodesEventArgs> CreateNewDomainCompleted;
        event EventHandler<ReturnedNodeIdEventArgs> CreateNewMapCompleted;
        event EventHandler<ReturnedNodeIdEventArgs> GetContainerMapNodeIdCompleted;
        event EventHandler<DeleteEventArgs> DeleteMetadataCompleted;
        event EventHandler<ResultEventArgs> RenameNodeMetadataCompleted;

        void GetDomainNodeIdsAsync();
        void GetDomainNodeIdAsync(Guid domain);
        void GetDomainMapNodes(Guid domain);

        void CreateNewDomain(string domainName);
        void CreateNewMap(Guid domain, string mapName);

        void GetNodeByIdAsync(Guid domain, Guid nodeId);
        void GetRelatedNodesByIdAsync(Guid domain, Guid nodeId, int depth);
        //void GetNodesWithConditions(Guid domain, Guid nodeId, List<Condition> conditions);
        void GetMapsNodes(Guid domain, Guid mapNodeId);
        void GetContainerMapNodeId(Guid domain, Guid nodeId);
        void GetAllSoapTypesAsync();

        void PasteNodeClone(Guid domainId, Guid copiedNodeId, Point location);

        //void AddNode(Guid domainId, INodeTypeProxy nodeType, string originalId);
        void AddNode(Guid domainId, INodeTypeProxy nodeType, string originalId, Point location);

        void DeleteMetadata(MetadataContext context);

        void DeleteNode(Guid domainId, Guid nodeToDelete);
        void DeleteRelationship(Guid domainId, Guid relationshipId);

        void DeleteNodeTransclusion(Guid domainId, Guid currentMapId, INodeProxy transcludedNode);
        void DeleteNodePromoteTransclusion(Guid domainId, Guid currentMapId, INodeProxy originalNode);

        void ConnectNodesAsync(Guid domainId, Dictionary<IDescriptorTypeProxy, Guid> nodes, IRelationshipTypeProxy relationshipType, string originalId);
        void ConnectNodesAsync(Guid domainId, Dictionary<IDescriptorTypeProxy, Guid> nodes, IRelationshipTypeProxy relationshipType, Point location, string originalId);

        void RenameNodeMetadataAsync(Guid domainId, Guid nodeId, Guid relationshipId, IDescriptorTypeProxy descriptorTypeProxy, string originalMetadataName, string newMetadataName);
        void UpdateNodeMetadataAsync(Guid domainId, Guid nodeId, Guid relationshipId, IDescriptorTypeProxy descriptorTypeProxy, string metadataName, string metadataValue, IMetadataTypeProxy metadataType);
    }

    public class MapLocationState
    {
        public Point Location
        {
            get;
            set;
        }

        public Guid MapRelationshipId
        {
            get;
            set;
        }
    }
}
