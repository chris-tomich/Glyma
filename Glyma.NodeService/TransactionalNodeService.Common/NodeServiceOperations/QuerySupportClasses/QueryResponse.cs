using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransactionalNodeService.Common.Model;
using System.Runtime.Serialization;
using System.IO;
using Ionic.Zlib;

namespace TransactionalNodeService.Common
{
    [DataContract]
    public class QueryResponse
    {
        private Dictionary<Guid, Node> _nodes = null;
        private Dictionary<Guid, Metadata> _metadata = null;
        private Dictionary<Guid, Descriptor> _descriptors = null;
        private Dictionary<Guid, Relationship> _relationships = null;

        public QueryResponse()
        {
            StartingObjectIndex = 0;
            LastObjectIndex = 0;
            FinalObjectIndex = 0;
            ErrorId = 0;
            ErrorMessage = string.Empty;
        }

        [DataMember]
        public int ErrorId
        {
            get;
            set;
        }

        [DataMember]
        public string ErrorMessage
        {
            get;
            set;
        }

        [DataMember]
        public Domain Domain
        {
            get;
            set;
        }

        [DataMember]
        public Node NodeContext
        {
            get;
            set;
        }

        [DataMember]
        public int StartingObjectIndex
        {
            get;
            set;
        }

        [DataMember]
        public int LastObjectIndex
        {
            get;
            set;
        }

        [DataMember]
        public int FinalObjectIndex
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<Guid, Node> Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    _nodes = new Dictionary<Guid,Node>();
                }

                return _nodes;
            }
            set
            {
                _nodes = value;
            }
        }

        [DataMember]
        public Dictionary<Guid, Relationship> Relationships
        {
            get
            {
                if (_relationships == null)
                {
                    _relationships = new Dictionary<Guid,Relationship>();
                }

                return _relationships;
            }
            set
            {
                _relationships = value;
            }
        }

        [IgnoreDataMember]
        public Dictionary<Guid, Metadata> Metadata
        {
            get
            {
                if (_metadata == null)
                {
                    _metadata = new Dictionary<Guid, Metadata>();
                }

                return _metadata;
            }
        }

        [IgnoreDataMember]
        public Dictionary<Guid, Descriptor> Descriptors
        {
            get
            {
                if (_descriptors == null)
                {
                    _descriptors = new Dictionary<Guid, Descriptor>();
                }

                return _descriptors;
            }
        }

        [DataMember(Name = "ZNO")]
        public string CompressedNodeData
        {
            get;
            set;
        }

        public void CompressResponse()
        {
            if (Nodes == null || Nodes.Count == 0)
            {
                return;
            }

            MemoryStream nodeCollectionXml = null;
            MemoryStream compressedNodeCollectionStream = null;
            ZlibStream zipStream = null;

            try
            {
                nodeCollectionXml = new MemoryStream();

                DataContractSerializer serializer = new DataContractSerializer(typeof(CompressedResponseTuple));

                CompressedResponseTuple responseTuple = new CompressedResponseTuple();
                responseTuple.Nodes = Nodes;
                responseTuple.Relationships = Relationships;

                serializer.WriteObject(nodeCollectionXml, responseTuple);

                byte[] metadataCollectionBytes = nodeCollectionXml.ToArray();
                compressedNodeCollectionStream = new MemoryStream();

                using (zipStream = new ZlibStream(compressedNodeCollectionStream, CompressionMode.Compress))
                {
                    zipStream.Write(metadataCollectionBytes, 0, metadataCollectionBytes.Length);
                }

                byte[] compressedMetadataCollectionBytes = compressedNodeCollectionStream.ToArray();

                CompressedNodeData = Convert.ToBase64String(compressedMetadataCollectionBytes);
                Nodes.Clear();
                Relationships.Clear();
            }
            finally
            {
                if (nodeCollectionXml != null)
                {
                    nodeCollectionXml.Dispose();
                    nodeCollectionXml = null;
                }
            }
        }

        public void AddNode(Node node)
        {
            if (!Nodes.ContainsKey(node.NodeUid))
            {
                Nodes.Add(node.NodeUid, node);
            }
        }

        public void AddMetadata(Metadata metadata)
        {
            Metadata.Add(metadata.MetadataId, metadata);
        }

        public void AddDescriptor(Descriptor descriptor)
        {
            Descriptors.Add(descriptor.DescriptorUid, descriptor);
        }

        public void AddRelationship(Relationship relationship)
        {
            Relationships.Add(relationship.RelationshipUid, relationship);
        }

        public void AttachElements()
        {
            foreach (Descriptor descriptor in Descriptors.Values)
            {
                if (Relationships.ContainsKey(descriptor.RelationshipUid))
                {
                    Relationship relationship = Relationships[descriptor.RelationshipUid];

                    relationship.AddNode(descriptor);
                }
            }

            foreach (Metadata metadata in Metadata.Values)
            {
                MetadataContext metadataContext = new MetadataContext(metadata);

                if (metadataContext.GetContextType() == MetadataContextType.Node && Nodes.ContainsKey(metadata.NodeUid.Value))
                {
                    Node node = Nodes[metadata.NodeUid.Value];

                    node.Metadata.Add(metadataContext, metadata);
                }
                else if (metadataContext.GetContextType() == MetadataContextType.Relationship && Relationships.ContainsKey(metadata.RelationshipUid.Value))
                {
                    Relationship relationship = Relationships[metadata.RelationshipUid.Value];

                    relationship.Metadata.Add(metadataContext, metadata);
                }
            }
        }

        public int CountObjects()
        {
            return Nodes.Count + Relationships.Count + Metadata.Count + Descriptors.Count;
        }

        public QueryResponse GetPage(int startingObjectIndex)
        {
            SortedList<Guid, object> sortedObjects = new SortedList<Guid, object>();

            QueryResponse pagedQueryResponse = new QueryResponse();
            pagedQueryResponse.Nodes = new Dictionary<Guid, Node>();
            pagedQueryResponse.Relationships = new Dictionary<Guid, Relationship>();

            pagedQueryResponse.Domain = Domain;
            pagedQueryResponse.NodeContext = NodeContext;
            pagedQueryResponse.StartingObjectIndex = startingObjectIndex;
            //pagedQueryResponse.LastObjectIndex = startingObjectIndex + 2000;
            pagedQueryResponse.FinalObjectIndex = 0;

            foreach (KeyValuePair<Guid, Node> nodePair in Nodes)
            {
                sortedObjects.Add(nodePair.Key, nodePair.Value);

                pagedQueryResponse.FinalObjectIndex++;

                /// Adding up all the metadata.
                //pagedQueryResponse.FinalObjectIndex += nodePair.Value.Metadata.Count;
            }

            foreach (KeyValuePair<Guid, Relationship> relationshipPair in Relationships)
            {
                sortedObjects.Add(relationshipPair.Key, relationshipPair.Value);

                pagedQueryResponse.FinalObjectIndex++;

                /// Adding up all the descriptors.
                //pagedQueryResponse.FinalObjectIndex += relationshipPair.Value.Nodes.Count;
            }

            int sortedObjectIndex = pagedQueryResponse.StartingObjectIndex - 1;
            int totalObjectCount = 0;

            while (totalObjectCount < TransactionalMappingToolServiceCommonBase.SoftObjectLimit)
            {
                sortedObjectIndex++;

                if (sortedObjectIndex >= sortedObjects.Count)
                {
                    break;
                }

                object sortedObject = sortedObjects.Values[sortedObjectIndex];

                if (sortedObject is Node)
                {
                    Node node = sortedObject as Node;

                    /// Add one for the node;
                    totalObjectCount++;

                    /// Add the number of metadata;
                    totalObjectCount += node.Metadata.Count;

                    pagedQueryResponse.Nodes.Add(node.NodeUid, node);
                }
                else if (sortedObject is Relationship)
                {
                    Relationship relationship = sortedObject as Relationship;

                    /// Add one for the relationship.
                    totalObjectCount++;

                    // Add the number of descriptors.
                    totalObjectCount += relationship.Nodes.Count;

                    pagedQueryResponse.Relationships.Add(relationship.RelationshipUid, relationship);
                }
            }

            pagedQueryResponse.LastObjectIndex = sortedObjectIndex;
            pagedQueryResponse.FinalObjectIndex = sortedObjects.Count;

            return pagedQueryResponse;
        }
    }
}