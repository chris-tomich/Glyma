using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NodeService
{
    public static class NodeServiceExtensionMethods
    {
        public static SoapNode ToSoapObject(this Node node)
        {
            SoapNode soapNode = new SoapNode();

            if (node.DomainUid == null || !node.DomainUid.HasValue)
            {
                throw new NullReferenceException("There is no Domain ID for this node.");
            }

            soapNode.Domain = node.DomainUid.Value;
            soapNode.Id = node.NodeUid;

            if (node.NodeTypeUid == null || !node.NodeTypeUid.HasValue)
            {
                throw new NullReferenceException("There is no Node Type for this node.");
            }

            soapNode.NodeType = (SoapNodeType)SoapTypeFactory.GetSoapType<SoapNodeType>(node.NodeTypeUid.Value);
            soapNode.Node = node;

            var allMetadata = node.Metadatas.OrderBy(x => (x.RelationshipUid != null) ? x.RelationshipUid : Guid.Empty).OrderBy(x => (x.DescriptorTypeUid != null) ? x.DescriptorTypeUid : Guid.Empty);

            foreach (var datum in allMetadata)
            {
                MetadataContext key = new MetadataContext() { 
                    MetadataName = datum.MetadataName,
                    NodeUid = datum.NodeUid, 
                    RelationshipUid = datum.RelationshipUid, 
                    DescriptorTypeUid = datum.DescriptorTypeUid };

                SoapMetadata soapDatum = new SoapMetadata();
                soapDatum.MetadataName = datum.MetadataName;
                soapDatum.MetadataValue = datum.MetadataValue;

                if (datum.MetadataTypeUid.HasValue)
                {
                    soapDatum.MetadataType = (SoapMetadataType)SoapTypeFactory.GetSoapType<SoapMetadataType>(datum.MetadataTypeUid.Value);
                }

                soapNode.Metadata[key] = soapDatum;
            }

            foreach (Descriptor localDescriptor in node.Descriptors)
            {
                SoapRelationship relationship = localDescriptor.Relationship.ToSoapObject();

                if (relationship != null && !soapNode.Relationships.ContainsKey(relationship.Id))
                {
                    soapNode.Relationships.Add(relationship.Id, relationship);
                }
            }

            return soapNode;
        }

        public static IEnumerable<SoapNode> ToSoapObject(this IEnumerable<Node> nodes)
        {
            List<SoapNode> soapNodes = new List<SoapNode>();

            foreach (Node node in nodes)
            {
                SoapNode soapNode = node.ToSoapObject();

                soapNodes.Add(soapNode);
            }

            return soapNodes;
        }

        public static SoapRelationship ToSoapObject(this Relationship relationship)
        {
            if (relationship != null)
            {
                SoapRelationship soapRelationship = new SoapRelationship();

                soapRelationship.Id = relationship.RelationshipUid;

                if (relationship.RelationshipTypeUid == null || !relationship.RelationshipTypeUid.HasValue)
                {
                    throw new NullReferenceException("There is no Relationship Type for this relationship.");
                }

                soapRelationship.RelationshipType = (SoapRelationshipType)SoapTypeFactory.GetSoapType<SoapRelationshipType>(relationship.RelationshipTypeUid.Value);

                soapRelationship.Relationship = relationship;

                var allMetadata = relationship.Metadatas.OrderBy(x => (x.DescriptorTypeUid != null) ? x.DescriptorTypeUid : Guid.Empty);

                foreach (var datum in allMetadata)
                {
                    SoapMetadata soapDatum = new SoapMetadata();
                    soapDatum.MetadataName = datum.MetadataName;
                    soapDatum.MetadataValue = datum.MetadataValue;

                    if (datum.MetadataTypeUid.HasValue)
                    {
                        soapDatum.MetadataType = (SoapMetadataType)SoapTypeFactory.GetSoapType<SoapMetadataType>(datum.MetadataTypeUid.Value);
                    }

                    soapRelationship.Metadata[datum.MetadataName] = soapDatum;
                }

                foreach (var descriptor in relationship.Descriptors)
                {
                    SoapDescriptorType soapDescriptor = (SoapDescriptorType)SoapTypeFactory.GetSoapType<SoapDescriptorType>(descriptor.DescriptorTypeUid.Value);

                    soapRelationship.Nodes.Add(soapDescriptor, descriptor.NodeUid.Value);
                }

                return soapRelationship;
            }
            return null;
        }

        public static IEnumerable<Node> GetConnectedNodes(this Node node)
        {
            Dictionary<Guid, Node> connectedNodes = new Dictionary<Guid, Node>();

            foreach (Descriptor descriptor in node.Descriptors)
            {
                var foreignDescriptors = descriptor.Relationship.Descriptors.Where(x => x.DescriptorUid != descriptor.DescriptorUid);

                foreach (Descriptor foreignDescriptor in foreignDescriptors)
                {
                    if (!connectedNodes.ContainsKey(foreignDescriptor.NodeUid.Value))
                    {
                        connectedNodes.Add(foreignDescriptor.NodeUid.Value, foreignDescriptor.Node);
                    }
                }
            }

            return connectedNodes.Values;
        }

        public static IEnumerable<Node> GetConnectedNodes(this Node node, Func<Descriptor, bool> localDescriptorSelector)
        {
            Dictionary<Guid, Node> connectedNodes = new Dictionary<Guid, Node>();

            var matchingLocalDescriptors = node.Descriptors.Where(localDescriptorSelector);

            foreach (Descriptor matchingLocalDescriptor in matchingLocalDescriptors)
            {
                var foreignDescriptors = matchingLocalDescriptor.Relationship.Descriptors.Where(x => x.DescriptorUid != matchingLocalDescriptor.DescriptorUid);

                foreach (Descriptor foreignDescriptor in foreignDescriptors)
                {
                    if (!connectedNodes.ContainsKey(foreignDescriptor.NodeUid.Value))
                    {
                        connectedNodes.Add(foreignDescriptor.NodeUid.Value, foreignDescriptor.Node);
                    }
                }
            }

            return connectedNodes.Values;
        }

        public static IEnumerable<Node> GetConnectedNodes(this Node node, Func<Descriptor, bool> localDescriptorSelector, Func<Descriptor, bool> foreignDescriptorSelector)
        {
            Dictionary<Guid, Node> connectedNodes = new Dictionary<Guid, Node>();

            var matchingLocalDescriptors = node.Descriptors.Where(localDescriptorSelector);

            foreach (Descriptor matchingLocalDescriptor in matchingLocalDescriptors)
            {
                var matchingForeignDescriptors = matchingLocalDescriptor.Relationship.Descriptors.Where(x => (x.DescriptorUid != matchingLocalDescriptor.DescriptorUid) && foreignDescriptorSelector(x));

                foreach (Descriptor matchingForeignDescriptor in matchingForeignDescriptors)
                {
                    if (!connectedNodes.ContainsKey(matchingForeignDescriptor.NodeUid.Value))
                    {
                        connectedNodes.Add(matchingForeignDescriptor.NodeUid.Value, matchingForeignDescriptor.Node);
                    }
                }
            }

            return connectedNodes.Values;
        }

        public static IEnumerable<Node> GetConnectedNodes(this Node node, Func<Relationship, bool> relationshipSelector)
        {
            Dictionary<Guid, Node> connectedNodes = new Dictionary<Guid, Node>();

            var matchingLocalDescriptors = node.Descriptors.Where(x => relationshipSelector(x.Relationship));

            foreach (Descriptor matchingLocalDescriptor in matchingLocalDescriptors)
            {
                var foreignDescriptors = matchingLocalDescriptor.Relationship.Descriptors.Where(x => x.DescriptorUid != matchingLocalDescriptor.DescriptorUid);

                foreach (Descriptor foreignDescriptor in foreignDescriptors)
                {
                    if (!connectedNodes.ContainsKey(foreignDescriptor.NodeUid.Value))
                    {
                        connectedNodes.Add(foreignDescriptor.NodeUid.Value, foreignDescriptor.Node);
                    }
                }
            }

            return connectedNodes.Values;
        }

        public static IEnumerable<Node> GetConnectedNodes(this Node node, Func<Descriptor, bool> localDescriptorSelector, Func<Relationship, bool> relationshipSelector)
        {
            Dictionary<Guid, Node> connectedNodes = new Dictionary<Guid, Node>();

            var matchingLocalDescriptors = node.Descriptors.Where(x => localDescriptorSelector(x) && relationshipSelector(x.Relationship));

            foreach (Descriptor matchingLocalDescriptor in matchingLocalDescriptors)
            {
                var foreignDescriptors = matchingLocalDescriptor.Relationship.Descriptors.Where(x => x.DescriptorUid != matchingLocalDescriptor.DescriptorUid);

                foreach (Descriptor foreignDescriptor in foreignDescriptors)
                {
                    if (!connectedNodes.ContainsKey(foreignDescriptor.NodeUid.Value))
                    {
                        connectedNodes.Add(foreignDescriptor.NodeUid.Value, foreignDescriptor.Node);
                    }
                }
            }

            return connectedNodes.Values;
        }

        public static IEnumerable<Node> GetConnectedNodes(this Node node, Func<Descriptor, bool> localDescriptorSelector, Func<Relationship, bool> relationshipSelector, Func<Descriptor, bool> foreignDescriptorSelector)
        {
            Dictionary<Guid, Node> connectedNodes = new Dictionary<Guid, Node>();

            var matchingLocalDescriptors = node.Descriptors.Where(x => localDescriptorSelector(x) && relationshipSelector(x.Relationship));

            foreach (Descriptor matchingLocalDescriptor in matchingLocalDescriptors)
            {
                var foreignDescriptors = matchingLocalDescriptor.Relationship.Descriptors.Where(x => (x.DescriptorUid != matchingLocalDescriptor.DescriptorUid) && foreignDescriptorSelector(x));

                foreach (Descriptor foreignDescriptor in foreignDescriptors)
                {
                    if (!connectedNodes.ContainsKey(foreignDescriptor.NodeUid.Value))
                    {
                        connectedNodes.Add(foreignDescriptor.NodeUid.Value, foreignDescriptor.Node);
                    }
                }
            }

            return connectedNodes.Values;
        }
    }
}