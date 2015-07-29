using System;
using System.Collections.Generic;
using System.Linq;
using TransactionalNodeService.Proxy;
using TransactionalNodeService.Service;
using TransactionalNodeService.Soap.TransactionFramework;
using Node = SilverlightMappingToolBasic.UI.SuperGraph.ViewModel.Node;
using Relationship = SilverlightMappingToolBasic.UI.SuperGraph.ViewModel.Relationship;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class BatchCloneNodesOperation : CommonOperationBase, ISuperGraphOperation
    {
        public BatchCloneNodesOperation(IMapManager mapManager, INode map, IEnumerable<Node> nodes, IEnumerable<Relationship> relationships)
            : base(mapManager)
        {
            Map = map;
            Nodes = nodes;
            Relationships = relationships;
        }

        public Dictionary<Node, INode> CloneMap
        {
            get;
            set;
        }

        public IEnumerable<Node> Nodes
        {
            get;
            set;
        }

        public IEnumerable<Relationship> Relationships { 
            get; 
            set; 
        }

        public INode Map
        {
            get;
            set;
        }

        public event EventHandler<NodeOperationCompletedArgs> OperationCompleted;
        public void ExecuteOperation()
        {
            CloneMap = new Dictionary<Node, INode>();
            var chain = new TransactionChain();
            chain.TransactionCompleted += ChainOnTransactionCompleted;
            Response = new InProcessTransactionResponse();
            foreach (var node in Nodes)
            {
                CloneMap.Add(node, BuildTransactionForNode(node, ref chain));
            }

            foreach (var relationship in Relationships)
            {
                BuildTransactionForRelationship(relationship, ref chain);
            }

            MapManager.ExecuteTransaction(chain);
        }

        private INode BuildTransactionForNode(Node node, ref TransactionChain chain)
        {
            INode newNode = MapManager.CreateNode(Map.DomainId, Map.RootMapId.Value, node.Proxy.NodeType, string.Empty, ref chain);
            IRelationship newMapContainerRelationship = MapManager.CreateRelationship(Map.DomainId, Map.RootMapId.Value, MapContainerRelationshipType, string.Empty, ref chain);

            newMapContainerRelationship.ConnectNode(FromConnectionType, newNode, ref chain);
            newMapContainerRelationship.ConnectNode(ToConnectionType, Map, ref chain);

            newNode.Metadata.Add(newMapContainerRelationship, FromConnectionType, "XPosition", (node.Location.X + 100).ToString(), ref chain);
            newNode.Metadata.Add(newMapContainerRelationship, FromConnectionType, "YPosition", (node.Location.Y + 100).ToString(), ref chain);
            newNode.Metadata.Add(newMapContainerRelationship, FromConnectionType, "CollapseState", "None", ref chain);
            newNode.Metadata.Add(newMapContainerRelationship, FromConnectionType, "Visibility", "Visible", ref chain);

            foreach (var metadata in node.Metadata)
            {
                if (!string.IsNullOrEmpty(metadata.Value.Value))
                {
                    newNode.Metadata.Add(null, null, metadata.Key, metadata.Value.Value, ref chain);
                }
            }

            Response.Nodes.Add(newNode);
            return newNode;
        }

        private void BuildTransactionForRelationship(Relationship relationship, ref TransactionChain chain)
        {
            var inProcessRelationship = MapManager.CreateRelationship(Map.DomainId, Map.RootMapId.Value, FromToRelationshipType, string.Empty, ref chain);
            var from = Nodes.FirstOrDefault(q => q.Id == relationship.From);
            var to = Nodes.FirstOrDefault(q => q.Id == relationship.To);

            if (from != null && to != null && CloneMap.ContainsKey(from) && CloneMap.ContainsKey(to))
            {
                inProcessRelationship.ConnectNode(FromConnectionType, CloneMap[from], ref chain);
                inProcessRelationship.ConnectNode(ToConnectionType, CloneMap[to], ref chain);
            }

            Response.Relationships.Add(inProcessRelationship);
        }

        private void ChainOnTransactionCompleted(object sender, CompleteTransactionCompletedEventArgs e)
        {
            if (OperationCompleted != null)
            {
                OperationCompleted(sender, new NodeOperationCompletedArgs { Response = Response });
            }
        }

        
    }
}
