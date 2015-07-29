using System;
using System.Collections.Generic;
using System.Windows;
using TransactionalNodeService.Service;
using TransactionalNodeService.Proxy;
using TransactionalNodeService.Soap.TransactionFramework;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public abstract class AddNodeOperationBase : CommonOperationBase, ISuperGraphOperation
    {
        public event EventHandler<NodeOperationCompletedArgs> OperationCompleted;

        protected AddNodeOperationBase(IMapManager mapManager, INode map, Point location, Dictionary<string, string> defaultMetadata = null)
            : base(mapManager)
        {
            X = location.X;
            Y = location.Y;
            Map = map;
            DefaultMetadata = defaultMetadata;
        }

        protected abstract NodeType NodeType { get; }

        public INode Map
        {
            get;
            private set;
        }

        public Dictionary<string, string> DefaultMetadata
        {
            get; 
            private set;
        }

        public double X
        {
            get;
            private set;
        }

        public double Y
        {
            get;
            private set;
        }

        private void ChainOnTransactionCompleted(object sender, CompleteTransactionCompletedEventArgs completeTransactionCompletedEventArgs)
        {
            if (OperationCompleted != null)
            {
                OperationCompleted(sender, new NodeOperationCompletedArgs { Response = Response });
            }
        }

        public void ExecuteOperation()
        {
            var chain = new TransactionChain();
            chain.TransactionCompleted += ChainOnTransactionCompleted;
            INode newNode = MapManager.CreateNode(Map.DomainId, Map.RootMapId.Value, NodeType, string.Empty, ref chain);
            IRelationship newMapContainerRelationship = MapManager.CreateRelationship(Map.DomainId, Map.RootMapId.Value, MapContainerRelationshipType, string.Empty, ref chain);

            newMapContainerRelationship.ConnectNode(FromConnectionType, newNode, ref chain);
            newMapContainerRelationship.ConnectNode(ToConnectionType, Map, ref chain);

            newNode.Metadata.Add(newMapContainerRelationship, FromConnectionType, "XPosition", X.ToString(), ref chain);
            newNode.Metadata.Add(newMapContainerRelationship, FromConnectionType, "YPosition", Y.ToString(), ref chain);
            newNode.Metadata.Add(newMapContainerRelationship, FromConnectionType, "CollapseState", "None", ref chain);
            newNode.Metadata.Add(newMapContainerRelationship, FromConnectionType, "Visibility", "Visible", ref chain);

            if (DefaultMetadata != null)
            {
                if (!DefaultMetadata.ContainsKey("Name"))
                {
                    newNode.Metadata.Add(null, null, "Name", string.Empty, ref chain);
                }
                foreach (var valuePair in DefaultMetadata)
                {
                    newNode.Metadata.Add(null, null, valuePair.Key, valuePair.Value, ref chain);
                }
            }
            else
            {
                newNode.Metadata.Add(null, null, "Name", string.Empty, ref chain);
            }

            Response = new InProcessTransactionResponse();
            Response.Nodes.Add(newNode);
            Response.Relationships.Add(newMapContainerRelationship);

            MapManager.ExecuteTransaction(chain);
        }

        
    }
}
