using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Printing;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using SilverlightMappingToolBasic.UI.ViewModel;
using TransactionalNodeService.Service;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;
using Proxy = TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class BatchMoveNodesOperation : CommonOperationBase, ISuperGraphOperation
    {
        public event EventHandler<NodeOperationCompletedArgs> OperationCompleted;

        public BatchMoveNodesOperation(Proxy.IMapManager mapManager)
            : base(mapManager)
        {
        }

        public IEnumerable<Node> Nodes
        {
            get;
            set;
        }

        public Proxy.INode Map
        {
            get;
            set;
        }

        private Guid GetMapId()
        {
            return Map.Id;
        }

        private Proxy.INode GetProxy(Node node)
        {
            return node.Proxy;
        }

        private Proxy.RelationshipSet GetRelationships(Proxy.INode proxy)
        {
            return proxy.Relationships;
        }

        private Proxy.NodeSet GetNodesFromRelationship(Proxy.IRelationship relationship)
        {
            return relationship.Nodes;
        }

        private Proxy.NodeMetadataCollection GetMetadataCollection(Proxy.INode proxy)
        {
            return proxy.Metadata;
        }

        private double GetXPosition(Point p)
        {
            return p.X;
        }

        private double GetYPosition(Point p)
        {
            return p.Y;
        }

        private void CommitLocation(Node node, Point location, ref TransactionFramework.TransactionChain chain)
        {
            //Replaced properties with Get methods in order to trace which property throws Null reference exception.
            var relationships = GetRelationships(GetProxy(node)).FindRelationships(FromConnectionType, MapContainerRelationshipType);
            var isComplete = false;

            foreach (var relationship in relationships)
            {
                var potentialMapNodes = GetNodesFromRelationship(relationship).FindNodes(ToConnectionType);

                foreach (Proxy.INode mapNode in potentialMapNodes)
                {
                    if (mapNode.Id == GetMapId())
                    {
                        isComplete = true;

                        GetMetadataCollection(GetProxy(node)).Add(relationship, FromConnectionType, "XPosition", GetXPosition(location).ToString(), ref chain);
                        GetMetadataCollection(GetProxy(node)).Add(relationship, FromConnectionType, "YPosition", GetYPosition(location).ToString(), ref chain);

                        break;
                    }
                }

                if (isComplete)
                {
                    break;
                }
            }
        }

        private void CommitLocationsCompleted(object sender, CompleteTransactionCompletedEventArgs e)
        {
            if (OperationCompleted != null)
            {
                var nodes = new List<KeyValuePair<Node, ModelOperationType>>();
                foreach (var node in Nodes)
                {
                    nodes.Add(new KeyValuePair<Node, ModelOperationType>(node, ModelOperationType.Modified));
                }
                OperationCompleted(sender, new NodeOperationCompletedArgs { Nodes = nodes });
            }
        }

        public void ExecuteOperation()
        {
            var chain = new TransactionFramework.TransactionChain();
            chain.TransactionCompleted += CommitLocationsCompleted;
            var isUpdated = false;
            foreach (var node in Nodes)
            {
                if (node.IsLocationDirty)
                {
                    node.IsLocationDirty = false;
                    isUpdated = true;
                    CommitLocation(node, node.Location, ref chain);
                }
            }
            MapManager.ExecuteTransaction(chain);
            if (!isUpdated)
            {
                 CommitLocationsCompleted(this, null);
            }
        }
    }
}
