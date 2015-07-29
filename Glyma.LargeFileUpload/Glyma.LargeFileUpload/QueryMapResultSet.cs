using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Data;

namespace Glyma.HttpHandlers
{
    public class QueryMapResultSet
    {
        public static readonly Guid FromDescriptorType = new Guid("96DA1782-058C-4F9B-BB1A-31B048F8C75A");
        public static readonly Guid TransclusionMapDescriptorType = new Guid("47107835-A485-4A23-BF0C-3FC631A07777");
        public static readonly Guid ToDescriptorType = new Guid("07C91D35-4DAC-431B-966B-64C924B8CDAB");
        public static readonly Guid TransclusionFromToRelationshipType = new Guid("9B64675F-D27B-42F1-BDAA-38D4697CC27A");
        public static readonly Guid MapContainerRelationshipType = new Guid("4AFF46D7-87BE-48DD-B703-A93E38EF8FFB");
        public static readonly Guid FromToRelationshipType = new Guid("FE31AE41-5501-4B13-8F4A-AFE902A78F40");

        private Dictionary<Guid, QueryMapNode> _nodes = null;
        private Dictionary<Guid, QueryMapRelationship> _relationships = null;
        private List<QueryMapDescriptor> _descriptors = null;
        private Dictionary<Guid, Dictionary<Guid, List<QueryMapDescriptor>>> _descriptorsByNode = null;
        private Dictionary<Guid, Dictionary<Guid, List<QueryMapDescriptor>>> _descriptorsByRelationship = null;
        private List<QueryMapMetadata> _metadata = null;

        public QueryMapResultSet(IMultipleResults queryMapResultSets)
        {
            var queryMapResultSet = queryMapResultSets.GetResult<QueryMapMultiDepthResult>();

            while (queryMapResultSet != null)
            {
                foreach (var queryMapResult in queryMapResultSet)
                {
                    if (queryMapResult.Level != null)
                    {
                        if (queryMapResult.NodeUid.HasValue && queryMapResult.NodeUid != Guid.Empty)
                        {
                            /// Make sure that we aren't displaying a domain node.
                            if (queryMapResult.NodeTypeUid.HasValue && queryMapResult.NodeTypeUid != new Guid("263754C2-2F31-4D21-B9C4-6509E00A5E94"))
                            {
                                /// The QueryMap procedure returns ALL nodes by following relationships to max depth meaning some nodes are repeated in some of the levels multiple nodes may connect to them.
                                if (!Nodes.ContainsKey(queryMapResult.NodeUid.Value))
                                {
                                    /// TODO: Need to consider if we require the NodeOriginalId.
                                    QueryMapNode node = new QueryMapNode(queryMapResult);

                                    Nodes[queryMapResult.NodeUid.Value] = node;
                                }
                            }
                        }
                    }
                    else if (queryMapResult.MetadataId != null)
                    {
                        if (queryMapResult.MetadataId.HasValue && queryMapResult.MetadataId != Guid.Empty)
                        {
                            QueryMapMetadata metadatum = new QueryMapMetadata(queryMapResult);

                            if (metadatum.NodeUid.HasValue)
                            {
                                if (Nodes.ContainsKey(metadatum.NodeUid.Value))
                                {
                                    Nodes[metadatum.NodeUid.Value].AddMetadata(metadatum);
                                }
                            }

                            Metadata.Add(metadatum);
                        }
                    }
                    else if (queryMapResult.DescriptorUid != null)
                    {
                        if (queryMapResult.DescriptorUid.HasValue && queryMapResult.DescriptorUid != Guid.Empty)
                        {
                            QueryMapDescriptor descriptor = new QueryMapDescriptor(queryMapResult);

                            Descriptors.Add(descriptor);
                            AddDescriptorByNode(descriptor);
                            AddDescriptorByRelationship(descriptor);
                        }
                    }
                    else
                    {
                        if (queryMapResult.RelationshipUid.HasValue && queryMapResult.RelationshipUid != Guid.Empty)
                        {
                            /// TODO: Need to consider if we require the RelationshipOriginalId.
                            QueryMapRelationship relationship = new QueryMapRelationship(queryMapResult);

                            Relationships[relationship.RelationshipUid] = relationship;
                        }
                    }
                }

                queryMapResultSet = queryMapResultSets.GetResult<QueryMapMultiDepthResult>();
            }
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

        private List<QueryMapDescriptor> Descriptors
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

        private Dictionary<Guid, Dictionary<Guid, List<QueryMapDescriptor>>> DescriptorsByNode
        {
            get
            {
                if (_descriptorsByNode == null)
                {
                    _descriptorsByNode = new Dictionary<Guid, Dictionary<Guid, List<QueryMapDescriptor>>>();
                }

                return _descriptorsByNode;
            }
        }

        private Dictionary<Guid, Dictionary<Guid, List<QueryMapDescriptor>>> DescriptorsByRelationship
        {
            get
            {
                if (_descriptorsByRelationship == null)
                {
                    _descriptorsByRelationship = new Dictionary<Guid, Dictionary<Guid, List<QueryMapDescriptor>>>();
                }

                return _descriptorsByRelationship;
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

        private void AddDescriptorByNode(QueryMapDescriptor descriptor)
        {
            if (descriptor.NodeUid.HasValue && descriptor.DescriptorTypeUid.HasValue)
            {
                Dictionary<Guid, List<QueryMapDescriptor>> descriptorsForNodeByDescriptorType;

                if (DescriptorsByNode.ContainsKey(descriptor.NodeUid.Value))
                {
                    descriptorsForNodeByDescriptorType = DescriptorsByNode[descriptor.NodeUid.Value];
                }
                else
                {
                    descriptorsForNodeByDescriptorType = new Dictionary<Guid, List<QueryMapDescriptor>>();
                    DescriptorsByNode[descriptor.NodeUid.Value] = descriptorsForNodeByDescriptorType;
                }

                List<QueryMapDescriptor> descriptorsForNode;

                if (descriptorsForNodeByDescriptorType.ContainsKey(descriptor.DescriptorTypeUid.Value))
                {
                    descriptorsForNode = descriptorsForNodeByDescriptorType[descriptor.DescriptorTypeUid.Value];
                }
                else
                {
                    descriptorsForNode = new List<QueryMapDescriptor>();
                    descriptorsForNodeByDescriptorType[descriptor.DescriptorTypeUid.Value] = descriptorsForNode;
                }

                descriptorsForNode.Add(descriptor);
            }
        }

        private void AddDescriptorByRelationship(QueryMapDescriptor descriptor)
        {
            if (descriptor.RelationshipUid.HasValue)
            {
                Dictionary<Guid, List<QueryMapDescriptor>> descriptorsForRelationshipByDescriptorType;

                if (DescriptorsByRelationship.ContainsKey(descriptor.RelationshipUid.Value))
                {
                    descriptorsForRelationshipByDescriptorType = DescriptorsByRelationship[descriptor.RelationshipUid.Value];
                }
                else
                {
                    descriptorsForRelationshipByDescriptorType = new Dictionary<Guid, List<QueryMapDescriptor>>();
                    DescriptorsByRelationship[descriptor.RelationshipUid.Value] = descriptorsForRelationshipByDescriptorType;
                }

                List<QueryMapDescriptor> descriptorsForRelationship;

                if (descriptorsForRelationshipByDescriptorType.ContainsKey(descriptor.DescriptorTypeUid.Value))
                {
                    descriptorsForRelationship = descriptorsForRelationshipByDescriptorType[descriptor.DescriptorTypeUid.Value];
                }
                else
                {
                    descriptorsForRelationship = new List<QueryMapDescriptor>();
                    descriptorsForRelationshipByDescriptorType[descriptor.DescriptorTypeUid.Value] = descriptorsForRelationship;
                }

                descriptorsForRelationship.Add(descriptor);
            }
        }

        public IEnumerable<QueryMapRelationship> GetRelationshipsByDescriptorType(Guid nodeId, Guid descriptorType)
        {
            if (DescriptorsByNode.ContainsKey(nodeId))
            {
                if (DescriptorsByNode[nodeId].ContainsKey(descriptorType))
                {
                    foreach (var descriptor in DescriptorsByNode[nodeId][descriptorType])
                    {
                        if (Relationships.ContainsKey(descriptor.RelationshipUid.Value))
                        {
                            yield return Relationships[descriptor.RelationshipUid.Value];
                        }
                    }
                }
            }
        }

        public IEnumerable<QueryMapRelationship> GetRelationshipsByRelationshipType(Guid nodeId, params Guid[] relationshipTypes)
        {
            if (DescriptorsByNode.ContainsKey(nodeId))
            {
                Dictionary<Guid, List<QueryMapDescriptor>> descriptorsByType = DescriptorsByNode[nodeId];

                foreach (List<QueryMapDescriptor> descriptorList in descriptorsByType.Values)
                {
                    foreach (QueryMapDescriptor descriptor in descriptorList)
                    {
                        if (Relationships.ContainsKey(descriptor.RelationshipUid.Value))
                        {
                            QueryMapRelationship relationship = Relationships[descriptor.RelationshipUid.Value];

                            foreach (Guid relationshipType in relationshipTypes)
                            {
                                if (relationship.RelationshipTypeUid == relationshipType)
                                {
                                    yield return relationship;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void FindNodeIdsByRelationshipType(Guid nodeId, ref HashSet<Guid> relatedMapsNodeList, Guid ignoreNodeId, Guid descriptorTypeId, params Guid[] relationshipTypes)
        {
            if (DescriptorsByNode.ContainsKey(nodeId))
            {
                Dictionary<Guid, List<QueryMapDescriptor>> descriptorsByType = DescriptorsByNode[nodeId];

                foreach (List<QueryMapDescriptor> descriptorList in descriptorsByType.Values)
                {
                    foreach (QueryMapDescriptor descriptor in descriptorList)
                    {
                        if (Relationships.ContainsKey(descriptor.RelationshipUid.Value))
                        {
                            QueryMapRelationship relationship = Relationships[descriptor.RelationshipUid.Value];

                            foreach (Guid relationshipType in relationshipTypes)
                            {
                                if (relationship.RelationshipTypeUid == relationshipType)
                                {
                                    var descriptors = DescriptorsByRelationship[relationship.RelationshipUid][descriptorTypeId];

                                    foreach (QueryMapDescriptor opposingDescriptor in descriptors)
                                    {
                                        if (opposingDescriptor.NodeUid.Value != null && (ignoreNodeId == Guid.Empty || opposingDescriptor.NodeUid != ignoreNodeId))
                                        {
                                            relatedMapsNodeList.Add(opposingDescriptor.NodeUid.Value);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public QueryMapNode GetNode(Guid relationshipId, Guid descriptorType)
        {
            if (DescriptorsByRelationship.ContainsKey(relationshipId))
            {
                if (DescriptorsByRelationship[relationshipId].ContainsKey(descriptorType))
                {
                    foreach (var descriptor in DescriptorsByRelationship[relationshipId][descriptorType])
                    {
                        if (Nodes.ContainsKey(descriptor.NodeUid.Value))
                        {
                            return Nodes[descriptor.NodeUid.Value];
                        }
                    }
                }
            }

            return null;
        }

        public Dictionary<Guid, QueryMapDescriptor> GetNodes(Guid relationshipId)
        {
            Dictionary<Guid, QueryMapDescriptor> descriptors = new Dictionary<Guid, QueryMapDescriptor>();

            if (DescriptorsByRelationship.ContainsKey(relationshipId))
            {
                foreach (KeyValuePair<Guid, List<QueryMapDescriptor>> nodeDescriptorPairs in DescriptorsByRelationship[relationshipId])
                {
                    descriptors[nodeDescriptorPairs.Key] = nodeDescriptorPairs.Value.FirstOrDefault();
                }
            }

            return descriptors;
        }
    }
}
