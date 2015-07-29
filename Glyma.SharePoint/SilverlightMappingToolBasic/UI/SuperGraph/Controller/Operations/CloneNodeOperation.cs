using System;
using System.Windows;
using TransactionalNodeService.Proxy;
using TransactionalNodeService.Service;
using TransactionalNodeService.Soap.TransactionFramework;
using Node = SilverlightMappingToolBasic.UI.SuperGraph.ViewModel.Node;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class CloneNodeOperation : CommonOperationBase, ISuperGraphOperation
    {
        public event EventHandler<NodeOperationCompletedArgs> OperationCompleted;

        public CloneNodeOperation(IMapManager mapManager, INode map, Node node)
            : base(mapManager)
        {
            Node = node;
            Map = map;
        }

        public INode Map
        {
            get;
            private set;
        }

        public Node Node
        {
            get;
            private set;
        }

        public void ExecuteOperation()
        {
            var chain = new TransactionChain();
            chain.TransactionCompleted += ChainOnTransactionCompleted;
            INode newNode = MapManager.CreateNode(Map.DomainId, Map.RootMapId.Value, Node.Proxy.NodeType, string.Empty, ref chain);
            IRelationship newMapContainerRelationship = MapManager.CreateRelationship(Map.DomainId, Map.RootMapId.Value, MapContainerRelationshipType, string.Empty, ref chain);

            newMapContainerRelationship.ConnectNode(FromConnectionType, newNode, ref chain);
            newMapContainerRelationship.ConnectNode(ToConnectionType, Map, ref chain);

            newNode.Metadata.Add(newMapContainerRelationship, FromConnectionType, "XPosition", (Node.Location.X + 100).ToString(), ref chain);
            newNode.Metadata.Add(newMapContainerRelationship, FromConnectionType, "YPosition", (Node.Location.Y + 100).ToString(), ref chain);
            newNode.Metadata.Add(newMapContainerRelationship, FromConnectionType, "CollapseState", "None", ref chain);
            newNode.Metadata.Add(newMapContainerRelationship, FromConnectionType, "Visibility", "Visible", ref chain);


            foreach (var metadata in Node.Metadata)
            {
                if (!string.IsNullOrEmpty(metadata.Value.Value))
                {
                    newNode.Metadata.Add(null, null, metadata.Key, metadata.Value.Value, ref chain);
                }
            }
            Response = new InProcessTransactionResponse();
            Response.Nodes.Add(newNode);
            Response.Relationships.Add(newMapContainerRelationship);

            MapManager.ExecuteTransaction(chain);
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
