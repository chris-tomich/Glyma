using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace NodeService
{
    public partial class GraphQueryCondition
    {
        private IEnumerable<Node> CheckNodeOperator(Node node, GraphQuerySchema schemaTest)
        {
            IEnumerable<Node> stoppingPointNodes = null;

            if (schemaTest.Operator == "Equals")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.NodeUid == schemaTest.Id);
            }
            else if (schemaTest.Operator == "NotEquals")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.NodeUid != schemaTest.Id);
            }

            return stoppingPointNodes;
        }

        private IEnumerable<Node> CheckRelationshipOperator(Node node, GraphQuerySchema schemaTest)
        {
            IEnumerable<Node> stoppingPointNodes = null;

            if (schemaTest.Operator == "Equals")
            {
                stoppingPointNodes = node.GetConnectedNodes((Relationship relationship) => relationship.RelationshipUid == schemaTest.Id);
            }
            else if (schemaTest.Operator == "NotEquals")
            {
                stoppingPointNodes = node.GetConnectedNodes((Relationship relationship) => relationship.RelationshipUid != schemaTest.Id);
            }

            return stoppingPointNodes;
        }

        private IEnumerable<Node> CheckDescriptorOperator(Node node, GraphQuerySchema schemaTest)
        {
            IEnumerable<Node> stoppingPointNodes = null;

            if (schemaTest.Operator == "Equals")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.DescriptorUid == schemaTest.Id);
            }
            else if (schemaTest.Operator == "NotEquals")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.DescriptorUid != schemaTest.Id);
            }

            return stoppingPointNodes;
        }

        private IEnumerable<Node> CheckNodeTypeOperator(Node node, GraphQueryType typeTest)
        {
            IEnumerable<Node> stoppingPointNodes = null;

            if (typeTest.Operator == "Equals")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.NodeType.NodeTypeName == typeTest.Name);
            }
            else if (typeTest.Operator == "NotEquals")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.NodeType.NodeTypeName != typeTest.Name);
            }

            return stoppingPointNodes;
        }

        private IEnumerable<Node> CheckRelationshipTypeOperator(Node node, GraphQueryType typeTest)
        {
            IEnumerable<Node> stoppingPointNodes = null;

            if (typeTest.Operator == "Equals")
            {
                stoppingPointNodes = node.GetConnectedNodes((Relationship relationship) => relationship.RelationshipType.RelationshipTypeName == typeTest.Name);
            }
            else if (typeTest.Operator == "NotEquals")
            {
                stoppingPointNodes = node.GetConnectedNodes((Relationship relationship) => relationship.RelationshipType.RelationshipTypeName != typeTest.Name);
            }

            return stoppingPointNodes;
        }

        private IEnumerable<Node> CheckDescriptorTypeOperator(Node node, GraphQueryType typeTest)
        {
            IEnumerable<Node> stoppingPointNodes = null;

            if (typeTest.Operator == "Equals")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.DescriptorType.DescriptorTypeName == typeTest.Name);
            }
            else if (typeTest.Operator == "NotEquals")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.DescriptorType.DescriptorTypeName != typeTest.Name);
            }

            return stoppingPointNodes;
        }

        private IEnumerable<Node> CheckMetadataStringOperator(Node node, GraphQueryMetadata metadataTest)
        {
            IEnumerable<Node> stoppingPointNodes = null;

            if (metadataTest.Operator == "Equals")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && metadata.MetadataValue == metadataTest.Value).Count() > 0);
            }
            else if (metadataTest.Operator == "NotEquals")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && metadata.MetadataValue != metadataTest.Value).Count() > 0);
            }

            return stoppingPointNodes;
        }

        private IEnumerable<Node> CheckMetadataIntOperator(Node node, GraphQueryMetadata metadataTest)
        {
            IEnumerable<Node> stoppingPointNodes = null;

            if (metadataTest.Operator == "Equals")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && int.Parse(metadata.MetadataValue) == int.Parse(metadataTest.Value)).Count() > 0);
            }
            else if (metadataTest.Operator == "NotEquals")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && int.Parse(metadata.MetadataValue) != int.Parse(metadataTest.Value)).Count() > 0);
            }
            else if (metadataTest.Operator == "GreaterThan")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && int.Parse(metadata.MetadataValue) > int.Parse(metadataTest.Value)).Count() > 0);
            }
            else if (metadataTest.Operator == "LessThan")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && int.Parse(metadata.MetadataValue) < int.Parse(metadataTest.Value)).Count() > 0);
            }
            else if (metadataTest.Operator == "GreaterThanOrEqual")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && int.Parse(metadata.MetadataValue) >= int.Parse(metadataTest.Value)).Count() > 0);
            }
            else if (metadataTest.Operator == "LessThanOrEqual")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && int.Parse(metadata.MetadataValue) <= int.Parse(metadataTest.Value)).Count() > 0);
            }

            return stoppingPointNodes;
        }

        private IEnumerable<Node> CheckMetadataDoubleOperator(Node node, GraphQueryMetadata metadataTest)
        {
            IEnumerable<Node> stoppingPointNodes = null;

            if (metadataTest.Operator == "Equals")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && double.Parse(metadata.MetadataValue) == double.Parse(metadataTest.Value)).Count() > 0);
            }
            else if (metadataTest.Operator == "NotEquals")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && double.Parse(metadata.MetadataValue) != double.Parse(metadataTest.Value)).Count() > 0);
            }
            else if (metadataTest.Operator == "GreaterThan")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && double.Parse(metadata.MetadataValue) > double.Parse(metadataTest.Value)).Count() > 0);
            }
            else if (metadataTest.Operator == "LessThan")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && double.Parse(metadata.MetadataValue) < double.Parse(metadataTest.Value)).Count() > 0);
            }
            else if (metadataTest.Operator == "GreaterThanOrEqual")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && double.Parse(metadata.MetadataValue) >= double.Parse(metadataTest.Value)).Count() > 0);
            }
            else if (metadataTest.Operator == "LessThanOrEqual")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && double.Parse(metadata.MetadataValue) <= double.Parse(metadataTest.Value)).Count() > 0);
            }

            return stoppingPointNodes;
        }

        private IEnumerable<Node> CheckMetadataDateTimeOperator(Node node, GraphQueryMetadata metadataTest)
        {
            IEnumerable<Node> stoppingPointNodes = null;

            if (metadataTest.Operator == "Equals")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && DateTime.Parse(metadata.MetadataValue) == DateTime.Parse(metadataTest.Value)).Count() > 0);
            }
            else if (metadataTest.Operator == "NotEquals")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && DateTime.Parse(metadata.MetadataValue) != DateTime.Parse(metadataTest.Value)).Count() > 0);
            }
            else if (metadataTest.Operator == "GreaterThan")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && DateTime.Parse(metadata.MetadataValue) > DateTime.Parse(metadataTest.Value)).Count() > 0);
            }
            else if (metadataTest.Operator == "LessThan")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && DateTime.Parse(metadata.MetadataValue) < DateTime.Parse(metadataTest.Value)).Count() > 0);
            }
            else if (metadataTest.Operator == "GreaterThanOrEqual")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && DateTime.Parse(metadata.MetadataValue) >= DateTime.Parse(metadataTest.Value)).Count() > 0);
            }
            else if (metadataTest.Operator == "LessThanOrEqual")
            {
                stoppingPointNodes = node.GetConnectedNodes((Descriptor localDescriptor) => true, (Descriptor foreignDescriptor) => foreignDescriptor.Node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && DateTime.Parse(metadata.MetadataValue) <= DateTime.Parse(metadataTest.Value)).Count() > 0);
            }

            return stoppingPointNodes;
        }

        public IEnumerable<Node> ApplySearchConditions(Node node)
        {
            Dictionary<Guid, Node> allowedNodes = node.GetConnectedNodes().ToDictionary(connectedNode => connectedNode.NodeUid);
            Dictionary<Guid, int> stopNodes = new Dictionary<Guid, int>();
            int numOfTests = 0;

            if (Match.SchemaSet != null)
            {
                foreach (GraphQuerySchema schemaTest in Match.SchemaSet)
                {
                    numOfTests++;
                    IEnumerable<Node> stoppingPointNodes = null;

                    if (schemaTest.BaseType == "Node")
                    {
                        stoppingPointNodes = CheckNodeOperator(node, schemaTest);
                    }
                    else if (schemaTest.BaseType == "Relationship")
                    {
                        stoppingPointNodes = CheckRelationshipOperator(node, schemaTest);
                    }
                    else if (schemaTest.BaseType == "Descriptor")
                    {
                        stoppingPointNodes = CheckDescriptorOperator(node, schemaTest);
                    }

                    if (stoppingPointNodes != null)
                    {
                        foreach (Node stoppingPointNode in stoppingPointNodes)
                        {
                            if (stopNodes.ContainsKey(stoppingPointNode.NodeUid))
                            {
                                stopNodes[stoppingPointNode.NodeUid] += 1;
                            }
                            else
                            {
                                stopNodes[stoppingPointNode.NodeUid] = 1;
                            }
                        }
                    }
                }
            }

            if (Match.TypeSet != null)
            {
                foreach (GraphQueryType typeTest in Match.TypeSet)
                {
                    numOfTests++;
                    IEnumerable<Node> stoppingPointNodes = null;

                    if (typeTest.BaseType == "NodeType")
                    {
                        stoppingPointNodes = CheckNodeTypeOperator(node, typeTest);
                    }
                    else if (typeTest.BaseType == "RelationshipType")
                    {
                        stoppingPointNodes = CheckRelationshipTypeOperator(node, typeTest);
                    }
                    else if (typeTest.BaseType == "DescriptorType")
                    {
                        stoppingPointNodes = CheckDescriptorTypeOperator(node, typeTest);
                    }

                    if (stoppingPointNodes != null)
                    {
                        foreach (Node stoppingPointNode in stoppingPointNodes)
                        {
                            if (stopNodes.ContainsKey(stoppingPointNode.NodeUid))
                            {
                                stopNodes[stoppingPointNode.NodeUid] += 1;
                            }
                            else
                            {
                                stopNodes[stoppingPointNode.NodeUid] = 1;
                            }
                        }
                    }
                }
            }

            if (Match.MetadataSet != null)
            {
                foreach (GraphQueryMetadata metadataTest in Match.MetadataSet)
                {
                    IEnumerable<Node> stoppingPointNodes = null;

                    if (metadataTest.BaseType == "String")
                    {
                        stoppingPointNodes = CheckMetadataStringOperator(node, metadataTest);
                    }
                    else if (metadataTest.BaseType == "Int")
                    {
                        stoppingPointNodes = CheckMetadataIntOperator(node, metadataTest);
                    }
                    else if (metadataTest.BaseType == "Double")
                    {
                        stoppingPointNodes = CheckMetadataDoubleOperator(node, metadataTest);
                    }
                    else if (metadataTest.BaseType == "DateTime")
                    {
                        stoppingPointNodes = CheckMetadataDateTimeOperator(node, metadataTest);
                    }

                    if (stoppingPointNodes != null)
                    {
                        foreach (Node stoppingPointNode in stoppingPointNodes)
                        {
                            if (stopNodes.ContainsKey(stoppingPointNode.NodeUid))
                            {
                                stopNodes[stoppingPointNode.NodeUid] += 1;
                            }
                            else
                            {
                                stopNodes[stoppingPointNode.NodeUid] = 1;
                            }
                        }
                    }
                }
            }

            foreach (KeyValuePair<Guid, int> stopNode in stopNodes)
            {
                if (stopNode.Value == numOfTests)
                {
                    allowedNodes.Remove(stopNode.Key);
                }
            }

            return allowedNodes.Values;
        }
    }
}