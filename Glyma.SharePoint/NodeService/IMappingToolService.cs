using System;
using System.Collections.Generic;
using System.Drawing;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace NodeService
{
    [ServiceContract(Namespace = "http://sevensigma.com.au/NodeService")]
    public interface IMappingToolService
    {
        [OperationContract]
        SoapNode CreateNewDomain(string domainName);

        [OperationContract]
        Guid CreateNewMap(Guid domainId, string mapName);

        [OperationContract]
        void InsertNodes(Guid domainId, Guid mapId, string siteUrl, byte[] userToken, Guid webId, Guid listId, int itemId);

        [OperationContract]
        SoapNode AddNode(Guid domainId, SoapNodeType nodeType, string originalId);

        [OperationContract]
        bool RenameNodeMetadata(Guid domainId, Guid soapNodeId, Guid soapRelationshipId, SoapDescriptorType soapDescriptorType, string originalMetadataName, string newMetadataName);
        
        [OperationContract]
        SoapNode PasteNodeClone(Guid domainId, Guid copiedNodeId);

        [OperationContract]
        DeleteResult DeleteNode(Guid domainId, Guid nodeId);

        [OperationContract]
        DeleteResult DeleteNodePromoteTransclusion(Guid domainId, Guid mapIdToDeleteFrom, Guid nodeIdToDelete);

        [OperationContract]
        DeleteResult DeleteNodeTransclusion(Guid domainId, Guid mapIdToDeleteFrom, Guid nodeIdToDelete);

        [OperationContract]
        DeleteResult DeleteRelationship(Guid domainId, Guid relationshipId);

        [OperationContract]
        DeleteResult DeleteMetadata(MetadataContext key);

        [OperationContract]
        [FaultContract(typeof(ConnectNodesFailureFault))]
        ConnectedNodesResult ConnectNodes(Guid domainId, Dictionary<SoapDescriptorType, Guid> nodes, SoapRelationshipType relationshipType, string originalId);

        [OperationContract]
        SoapNode UpdateNodeMetadata(Guid domainId, Guid soapNodeId, Guid soapRelationshipId, SoapDescriptorType soapDescriptorType, string metadataName, string metadataValue, SoapMetadataType soapMetadataType);

        [OperationContract]
        Dictionary<string, Guid> GetDomainNodeIds();

        [OperationContract]
        Guid GetDomainNodeId(Guid domainId);

        [OperationContract]
        RelatedNodesSearchResult GetDomainMapNodes(Guid domainId);

        [OperationContract(Name="GetDomainMapNodesPaged")]
        RelatedNodesSearchResult GetDomainMapNodes(Guid domainId, int page, int pageSize);

        [OperationContract]
        List<SoapTypeElement> GetAllSoapTypes();

        [OperationContract]
        Dictionary<Guid, SoapNode> GetAllNodes(Guid domainId);

        [OperationContract]
        Dictionary<Guid, SoapNode> GetNodes(Dictionary<Guid, Guid> nodeIds);

        [OperationContract]
        RelatedNodesSearchResult GetRelatedNodes(Guid domainId, Guid nodeId, int depth);

        [OperationContract]
        RelatedNodesSearchResult GetMapsNodes(Guid domainId, Guid mapNodeId);

        [OperationContract]
        Guid GetContainerMapNodeId(Guid domainId, Guid nodeId);

        //[OperationContract]
        //RelatedNodesSearchResult GetNodesWithConditions(Guid domainId, Guid nodeId, int maxDepth, List<Condition> conditions);

        [OperationContract]
        RelatedNodesSearchResult GetNodesXmlQuery(string graphQueryXml);
    }

    [DataContract]
    public class RelatedNodesSearchResult
    {
        public RelatedNodesSearchResult()
        {
        }

        public RelatedNodesSearchResult(Guid nodeId, IEnumerable<Node> nodes)
        {
            Nodes = new Dictionary<Guid, SoapNode>();

            foreach (Node relatedNode in nodes)
            {
                SoapNode soapNode = relatedNode.ToSoapObject();

                if (soapNode.NodeType.Name != "DomainNode")
                {
                    Nodes.Add(soapNode.Id, soapNode);
                }
            }
        }

        [DataMember]
        public Guid SearchedNode
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<Guid, SoapNode> Nodes
        {
            get;
            set;
        }
    }

    [DataContract]
    public class ConnectedNodesResult
    {
        [DataMember]
        public SoapRelationship Relationship
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<Guid, SoapNode> Nodes
        {
            get;
            set;
        }
    }

    [DataContract]
    public class DeleteResult
    {
        [DataMember]
        public bool DeleteSuccessful
        {
            get;
            set;
        }

        [DataMember]
        public Guid DeletedId
        {
            get;
            set;
        }
    }

    [DataContract]
    public class ConnectNodesFailureFault { }

    [DataContract]
    public class MetadataContext
    {
        [DataMember]
        public string MetadataName
        {
            get;
            set;
        }

        [DataMember]
        public Guid? NodeUid
        {
            get;
            set;
        }

        [DataMember()]
        public Guid? RelationshipUid
        {
            get;
            set;
        }

        [DataMember]
        public Guid? DescriptorTypeUid
        {
            get;
            set;
        }
    }
}
