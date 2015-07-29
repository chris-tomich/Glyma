using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NodeService
{
    public partial class GraphQueryFilter
    {
        private IEnumerable<Node> CheckNodeOperator(IEnumerable<Node> nodes, GraphQuerySchema schemaTest)
        {
            List<Node> filteredNodes = new List<Node>();

            foreach (Node node in nodes)
            {
                if (schemaTest.Operator.Equals("Equals", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (node.NodeUid == schemaTest.Id)
                    {
                        filteredNodes.Add(node);
                    }
                }
                else if (schemaTest.Operator.Equals("NotEquals", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (node.NodeUid != schemaTest.Id)
                    {
                        filteredNodes.Add(node);
                    }
                }
            }

            return filteredNodes;
        }

        private IEnumerable<Node> CheckNodeTypeOperator(IEnumerable<Node> nodes, GraphQueryType typeTest)
        {
            List<Node> filteredNodes = new List<Node>();

            foreach (Node node in nodes)
            {
                if (typeTest.Operator.Equals("Equals", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (node.NodeType.NodeTypeName == typeTest.Name)
                    {
                        filteredNodes.Add(node);
                    }
                }
                else if (typeTest.Operator.Equals("NotEquals", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (node.NodeType.NodeTypeName != typeTest.Name)
                    {
                        filteredNodes.Add(node);
                    }
                }
            }

            return filteredNodes;
        }

        private IEnumerable<Node> CheckMetadataStringOperator(MappingToolDatabaseDataContext dbContext, IEnumerable<Node> nodes, GraphQueryMetadata metadataTest)
        {
            List<Node> filteredNodes = null;

            if (metadataTest.Operator.Equals("Contains", StringComparison.InvariantCultureIgnoreCase))
            {
                IQueryable<Node> queryableNodes = nodes.AsQueryable();
                var filteredMetadata = from dbMetadata in dbContext.udf_SearchMetadataContains(metadataTest.Name, metadataTest.Value) select dbMetadata;
                int counter = filteredMetadata.Count();
                filteredNodes = nodes.Join(filteredMetadata, x => x.NodeUid, y => y.NodeUid, (a, b) => a).ToList();
            }
            else if (metadataTest.Operator.Equals("FreeText", StringComparison.InvariantCultureIgnoreCase))
            {
                IQueryable<Node> queryableNodes = nodes.AsQueryable();
                var filteredMetadata = from dbMetadata in dbContext.udf_SearchMetadataFreeText(metadataTest.Name, metadataTest.Value) select dbMetadata;
                int counter = filteredMetadata.Count();
                filteredNodes = nodes.Join(filteredMetadata, x => x.NodeUid, y => y.NodeUid, (a, b) => a).ToList();
            }
            else
            {
                filteredNodes = new List<Node>();

                foreach (Node node in nodes)
                {
                    if (metadataTest.Operator.Equals("Equals", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && metadata.MetadataValue.Contains(metadataTest.Value)).Count() > 0)
                        {
                            filteredNodes.Add(node);
                        }
                    }
                    else if (metadataTest.Operator.Equals("NotEquals", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && metadata.MetadataValue != metadataTest.Value).Count() > 0)
                        {
                            filteredNodes.Add(node);
                        }
                    }
                }
            }

            return filteredNodes;
        }

        private IEnumerable<Node> CheckMetadataIntOperator(IEnumerable<Node> nodes, GraphQueryMetadata metadataTest)
        {
            List<Node> filteredNodes = new List<Node>();

            foreach (Node node in nodes)
            {
                if (metadataTest.Operator.Equals("Equals", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && int.Parse(metadata.MetadataValue) == int.Parse(metadataTest.Value)).Count() > 0)
                    {
                        filteredNodes.Add(node);
                    }
                }
                else if (metadataTest.Operator.Equals("NotEquals", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && int.Parse(metadata.MetadataValue) != int.Parse(metadataTest.Value)).Count() > 0)
                    {
                        filteredNodes.Add(node);
                    }
                }
                else if (metadataTest.Operator.Equals("GreaterThan", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && int.Parse(metadata.MetadataValue) > int.Parse(metadataTest.Value)).Count() > 0)
                    {
                        filteredNodes.Add(node);
                    }
                }
                else if (metadataTest.Operator.Equals("LessThan", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && int.Parse(metadata.MetadataValue) < int.Parse(metadataTest.Value)).Count() > 0)
                    {
                        filteredNodes.Add(node);
                    }
                }
                else if (metadataTest.Operator.Equals("GreaterThanOrEqual", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && int.Parse(metadata.MetadataValue) >= int.Parse(metadataTest.Value)).Count() > 0)
                    {
                        filteredNodes.Add(node);
                    }
                }
                else if (metadataTest.Operator.Equals("LessThanOrEqual", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && int.Parse(metadata.MetadataValue) <= int.Parse(metadataTest.Value)).Count() > 0)
                    {
                        filteredNodes.Add(node);
                    }
                }
            }

            return filteredNodes;
        }

        private IEnumerable<Node> CheckMetadataDoubleOperator(IEnumerable<Node> nodes, GraphQueryMetadata metadataTest)
        {
            List<Node> filteredNodes = new List<Node>();

            foreach (Node node in nodes)
            {
                if (metadataTest.Operator.Equals("Equals", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && double.Parse(metadata.MetadataValue) == double.Parse(metadataTest.Value)).Count() > 0)
                    {
                        filteredNodes.Add(node);
                    }
                }
                else if (metadataTest.Operator.Equals("NotEquals", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && double.Parse(metadata.MetadataValue) != double.Parse(metadataTest.Value)).Count() > 0)
                    {
                        filteredNodes.Add(node);
                    }
                }
                else if (metadataTest.Operator.Equals("GreaterThan", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && double.Parse(metadata.MetadataValue) > double.Parse(metadataTest.Value)).Count() > 0)
                    {
                        filteredNodes.Add(node);
                    }
                }
                else if (metadataTest.Operator.Equals("LessThan", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && double.Parse(metadata.MetadataValue) < double.Parse(metadataTest.Value)).Count() > 0)
                    {
                        filteredNodes.Add(node);
                    }
                }
                else if (metadataTest.Operator.Equals("GreaterThanOrEqual", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && double.Parse(metadata.MetadataValue) >= double.Parse(metadataTest.Value)).Count() > 0)
                    {
                        filteredNodes.Add(node);
                    }
                }
                else if (metadataTest.Operator.Equals("LessThanOrEqual", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && double.Parse(metadata.MetadataValue) <= double.Parse(metadataTest.Value)).Count() > 0)
                    {
                        filteredNodes.Add(node);
                    }
                }
            }

            return filteredNodes;
        }

        private IEnumerable<Node> CheckMetadataDateTimeOperator(IEnumerable<Node> nodes, GraphQueryMetadata metadataTest)
        {
            List<Node> filteredNodes = new List<Node>();

            foreach (Node node in nodes)
            {
                if (metadataTest.Operator.Equals("Equals", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && DateTime.Parse(metadata.MetadataValue) == DateTime.Parse(metadataTest.Value)).Count() > 0)
                    {
                        filteredNodes.Add(node);
                    }
                }
                else if (metadataTest.Operator.Equals("NotEquals", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && DateTime.Parse(metadata.MetadataValue) != DateTime.Parse(metadataTest.Value)).Count() > 0)
                    {
                        filteredNodes.Add(node);
                    }
                }
                else if (metadataTest.Operator.Equals("GreaterThan", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && DateTime.Parse(metadata.MetadataValue) > DateTime.Parse(metadataTest.Value)).Count() > 0)
                    {
                        filteredNodes.Add(node);
                    }
                }
                else if (metadataTest.Operator.Equals("LessThan", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && DateTime.Parse(metadata.MetadataValue) < DateTime.Parse(metadataTest.Value)).Count() > 0)
                    {
                        filteredNodes.Add(node);
                    }
                }
                else if (metadataTest.Operator.Equals("GreaterThanOrEqual", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && DateTime.Parse(metadata.MetadataValue) >= DateTime.Parse(metadataTest.Value)).Count() > 0)
                    {
                        filteredNodes.Add(node);
                    }
                }
                else if (metadataTest.Operator.Equals("LessThanOrEqual", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (node.Metadatas.Where(metadata => metadata.MetadataName == metadataTest.Name && DateTime.Parse(metadata.MetadataValue) <= DateTime.Parse(metadataTest.Value)).Count() > 0)
                    {
                        filteredNodes.Add(node);
                    }
                }
            }

            return filteredNodes;
        }

        public IEnumerable<Node> ApplySearchConditions(MappingToolDatabaseDataContext dbContext, IEnumerable<Node> nodes)
        {
            //Dictionary<Guid, Node> nodes = node.GetConnectedNodes().ToDictionary(connectedNode => connectedNode.NodeUid);
            Dictionary<Guid, int> nodeUniquenessCount = new Dictionary<Guid, int>();
            Dictionary<Guid, Node> nodesById = nodes.Distinct().ToDictionary<Node, Guid>(node => node.NodeUid);
            Dictionary<Guid, int> capturedNodes = new Dictionary<Guid, int>();
            int numOfTests = 0;

            if (Match.SchemaSet != null)
            {
                foreach (GraphQuerySchema schemaTest in Match.SchemaSet)
                {
                    numOfTests++;
                    IEnumerable<Node> filteredNodes = null;

                    if (schemaTest.BaseType.Equals("Node", StringComparison.InvariantCultureIgnoreCase))
                    {
                        filteredNodes = CheckNodeOperator(nodes, schemaTest);
                    }

                    if (filteredNodes != null)
                    {
                        foreach (Node filteredNode in filteredNodes)
                        {
                            if (capturedNodes.ContainsKey(filteredNode.NodeUid))
                            {
                                capturedNodes[filteredNode.NodeUid] += 1;
                            }
                            else
                            {
                                capturedNodes[filteredNode.NodeUid] = 1;
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
                    IEnumerable<Node> filteredNodes = null;

                    if (typeTest.BaseType.Equals("NodeType", StringComparison.InvariantCultureIgnoreCase))
                    {
                        filteredNodes = CheckNodeTypeOperator(nodes, typeTest);
                    }

                    if (filteredNodes != null)
                    {
                        foreach (Node filteredNode in filteredNodes)
                        {
                            if (capturedNodes.ContainsKey(filteredNode.NodeUid))
                            {
                                capturedNodes[filteredNode.NodeUid] += 1;
                            }
                            else
                            {
                                capturedNodes[filteredNode.NodeUid] = 1;
                            }
                        }
                    }
                }
            }

            if (Match.MetadataSet != null)
            {
                foreach (GraphQueryMetadata metadataTest in Match.MetadataSet)
                {
                    numOfTests++;
                    IEnumerable<Node> filteredNodes = null;
                    if (!string.IsNullOrEmpty(metadataTest.BaseType))
                    {
                        if (metadataTest.BaseType.Equals("String", StringComparison.InvariantCultureIgnoreCase))
                        {
                            filteredNodes = CheckMetadataStringOperator(dbContext, nodes, metadataTest);
                        }
                        else if (metadataTest.BaseType.Equals("Int", StringComparison.InvariantCultureIgnoreCase))
                        {
                            filteredNodes = CheckMetadataIntOperator(nodes, metadataTest);
                        }
                        else if (metadataTest.BaseType.Equals("Double", StringComparison.InvariantCultureIgnoreCase))
                        {
                            filteredNodes = CheckMetadataDoubleOperator(nodes, metadataTest);
                        }
                        else if (metadataTest.BaseType.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase))
                        {
                            filteredNodes = CheckMetadataDateTimeOperator(nodes, metadataTest);
                        }

                        if (filteredNodes != null)
                        {
                            foreach (Node filteredNode in filteredNodes)
                            {
                                if (capturedNodes.ContainsKey(filteredNode.NodeUid))
                                {
                                    capturedNodes[filteredNode.NodeUid] += 1;
                                }
                                else
                                {
                                    capturedNodes[filteredNode.NodeUid] = 1;
                                }
                            }
                        }
                    }
                }
            }

            List<Node> allFilteredNodes = new List<Node>();

            foreach (KeyValuePair<Guid, int> capturedNode in capturedNodes)
            {
                if (capturedNode.Value == numOfTests)
                {
                    allFilteredNodes.Add(nodesById[capturedNode.Key]);
                }
            }

            return allFilteredNodes;
        }
    }
}