using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace Glyma.Powershell.Update.v1_5_0_r1
{
    public class QueryMapResultConsumer
    {
        private Dictionary<Guid, QueryMapNode> _nodes = null;
        private Dictionary<Guid, QueryMapRelationship> _relationships = null;
        private List<QueryMapDescriptor> _descriptors = null;
        private List<QueryMapMetadata> _metadata = null;

        public QueryMapResultConsumer()
        {
        }

        public Dictionary<Guid, QueryMapNode> Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    _nodes = new Dictionary<Guid, QueryMapNode>();
                }

                return _nodes;
            }
        }

        public Dictionary<Guid, QueryMapRelationship> Relationships
        {
            get
            {
                if (_relationships == null)
                {
                    _relationships = new Dictionary<Guid, QueryMapRelationship>();
                }

                return _relationships;
            }
        }

        public List<QueryMapDescriptor> Descriptors
        {
            get
            {
                if (_descriptors == null)
                {
                    _descriptors = new List<QueryMapDescriptor>();
                }

                return _descriptors;
            }
        }

        public List<QueryMapMetadata> Metadata
        {
            get
            {
                if (_metadata == null)
                {
                    _metadata = new List<QueryMapMetadata>();
                }

                return _metadata;
            }
        }

        public void Consume(IMultipleResults resultSets)
        {
            IEnumerable<QueryMapMultiDepthResult> resultSet = resultSets.GetResult<QueryMapMultiDepthResult>();

            while (resultSet != null)
            {
                foreach (var queryMapResult in resultSet)
                {
                    if (queryMapResult.Level != null)
                    {
                        if (queryMapResult.NodeUid.HasValue && queryMapResult.NodeUid != Guid.Empty)
                        {
                            /// The QueryMap procedure returns ALL nodes by following relationships to max depth meaning some nodes are repeated in some of the levels multiple nodes may connect to them.
                            if (!Nodes.ContainsKey(queryMapResult.NodeUid.Value))
                            {
                                /// TODO: Need to consider copying the NodeOriginalId.
                                QueryMapNode node = new QueryMapNode();
                                node.NodeUid = queryMapResult.NodeUid.Value;
                                node.DomainUid = queryMapResult.DomainUid.Value;
                                node.NodeTypeUid = queryMapResult.NodeTypeUid;

                                Nodes[queryMapResult.NodeUid.Value] = node;
                            }
                        }
                    }
                    else if (queryMapResult.MetadataId != null)
                    {
                        if (queryMapResult.MetadataId.HasValue && queryMapResult.MetadataId != Guid.Empty)
                        {
                            QueryMapMetadata metadatum = new QueryMapMetadata();
                            metadatum.MetadataId = queryMapResult.MetadataId.Value;
                            metadatum.NodeUid = queryMapResult.NodeUid;
                            metadatum.RelationshipUid = queryMapResult.RelationshipUid;
                            metadatum.DescriptorTypeUid = queryMapResult.DescriptorTypeUid;
                            metadatum.MetadataTypeUid = queryMapResult.MetadataTypeUid;
                            metadatum.MetadataName = queryMapResult.MetadataName;
                            metadatum.MetadataValue = queryMapResult.MetadataValue;

                            Metadata.Add(metadatum);
                        }
                    }
                    else if (queryMapResult.DescriptorUid != null)
                    {
                        if (queryMapResult.DescriptorUid.HasValue && queryMapResult.DescriptorUid != Guid.Empty)
                        {
                            QueryMapDescriptor descriptor = new QueryMapDescriptor();
                            descriptor.DescriptorUid = queryMapResult.DescriptorUid.Value;
                            descriptor.NodeUid = queryMapResult.NodeUid;
                            descriptor.RelationshipUid = queryMapResult.RelationshipUid;
                            descriptor.DescriptorTypeUid = queryMapResult.DescriptorTypeUid;

                            Descriptors.Add(descriptor);
                        }
                    }
                    else
                    {
                        if (queryMapResult.RelationshipUid.HasValue && queryMapResult.RelationshipUid != Guid.Empty)
                        {
                            /// TODO: Need to consider copying the RelationshipOriginalId.
                            QueryMapRelationship relationship = new QueryMapRelationship();
                            relationship.RelationshipUid = queryMapResult.RelationshipUid.Value;
                            relationship.DomainUid = queryMapResult.DomainUid.Value;
                            relationship.RelationshipTypeUid = queryMapResult.RelationshipTypeUid;

                            Relationships[relationship.RelationshipUid] = relationship;
                        }
                    }
                }

                resultSet = resultSets.GetResult<QueryMapMultiDepthResult>();
            }
        }

        public IEnumerable<QueryMapRelationship> GetRelationships(QueryMapNode node, params Guid[] relationshipTypes)
        {
            if (relationshipTypes.Length <= 0)
            {
                foreach (QueryMapDescriptor descriptor in Descriptors)
                {
                    if (descriptor.NodeUid == node.NodeUid)
                    {
                        if (descriptor.RelationshipUid.HasValue)
                        {
                            if (Relationships.ContainsKey(descriptor.RelationshipUid.Value))
                            {
                                yield return Relationships[descriptor.RelationshipUid.Value];
                            }
                        }
                    }
                }
            }
            else
            {
                HashSet<Guid> relationshipTypesHashSet = new HashSet<Guid>(relationshipTypes);

                foreach (QueryMapDescriptor descriptor in Descriptors)
                {
                    if (descriptor.NodeUid == node.NodeUid)
                    {
                        if (descriptor.RelationshipUid.HasValue)
                        {
                            QueryMapRelationship relationship = Relationships[descriptor.RelationshipUid.Value];

                            if (relationship.RelationshipTypeUid.HasValue && relationshipTypesHashSet.Contains(relationship.RelationshipTypeUid.Value))
                            {
                                yield return relationship;
                            }
                        }
                    }
                }
            }
        }

        public IEnumerable<QueryMapNode> GetNodes(QueryMapRelationship relationship, QueryMapNode excludingNode)
        {
            foreach (QueryMapDescriptor descriptor in Descriptors)
            {
                if (descriptor.RelationshipUid == relationship.RelationshipUid && descriptor.NodeUid != excludingNode.NodeUid)
                {
                    if (descriptor.NodeUid.HasValue)
                    {
                        if (Nodes.ContainsKey(descriptor.NodeUid.Value))
                        {
                            yield return Nodes[descriptor.NodeUid.Value];
                        }
                    }
                }
            }
        }
    }
}
