using System;
using System.Collections.Generic;
using System.Windows;
using SilverlightMappingToolBasic.UI.Extensions.Security;
using SilverlightMappingToolBasic.UI.SuperGraph.View.CollapseSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using TransactionalNodeService.Service;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;
using Proxy = TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class BatchSaveCollapseStatesOperation : CommonOperationBase, ISuperGraphOperation
    {
        private readonly string _visibilityKey;
        private readonly string _collapseStateKey;

        public event EventHandler<NodeOperationCompletedArgs> OperationCompleted;

        public BatchSaveCollapseStatesOperation(Proxy.IMapManager mapManager, PermissionLevel group)
            : base(mapManager)
        {
            if (group < PermissionLevel.Author)
            {
                _visibilityKey = "Visibility";
                _collapseStateKey = "CollapseState";
            }
            else
            {
                _visibilityKey = "AuthorVisibility";
                _collapseStateKey = "AuthorCollapseState";
            }
        }

        public IEnumerable<Node> PendingVisibilityNodes
        {
            get;
            set;
        }

        public IEnumerable<Node> PendingCollapseNodes
        {
            get;
            set;
        }

        public Proxy.INode Map
        {
            get;
            set;
        }

        private void CommitVisibility(Node node, Visibility visibility, ref TransactionFramework.TransactionChain chain)
        {
            var relationships = node.Proxy.Relationships.FindRelationships(FromConnectionType, MapContainerRelationshipType);

            var isComplete = false;

            foreach (Proxy.IRelationship relationship in relationships)
            {
                var potentialMapNodes = relationship.Nodes.FindNodes(ToConnectionType);

                foreach (Proxy.INode mapNode in potentialMapNodes)
                {
                    if (mapNode.Id == Map.Id)
                    {
                        isComplete = true;

                        node.Proxy.Metadata.Add(relationship, FromConnectionType, _visibilityKey, visibility.ToString(), ref chain);

                        break;
                    }
                }

                if (isComplete)
                {
                    break;
                }
            }
        }

        private void CommitCollapseState(Node node, CollapseState state, ref TransactionFramework.TransactionChain chain)
        {
            var relationships = node.Proxy.Relationships.FindRelationships(FromConnectionType, MapContainerRelationshipType);

            var isComplete = false;

            foreach (Proxy.IRelationship relationship in relationships)
            {
                var potentialMapNodes = relationship.Nodes.FindNodes(ToConnectionType);

                foreach (Proxy.INode mapNode in potentialMapNodes)
                {
                    if (mapNode.Id == Map.Id)
                    {
                        isComplete = true;

                        node.Proxy.Metadata.Add(relationship, FromConnectionType, _collapseStateKey, state.ToString(), ref chain);

                        break;
                    }
                }

                if (isComplete)
                {
                    break;
                }
            }
        }

        public void ExecuteOperation()
        {
            var chain = new TransactionFramework.TransactionChain();
            foreach (var node in PendingVisibilityNodes)
            {
                CommitVisibility(node, node.Visibility, ref chain);
                if (chain.NumOfTransactions > 50)
                {
                    MapManager.ExecuteTransaction(chain);
                    chain = new TransactionFramework.TransactionChain();
                }
            }
            foreach (var node in PendingCollapseNodes)
            {
                CommitCollapseState(node, node.State, ref chain);
                if (chain.NumOfTransactions > 50)
                {
                    MapManager.ExecuteTransaction(chain);
                    chain = new TransactionFramework.TransactionChain();
                }
            }
            MapManager.ExecuteTransaction(chain);
        }
    }
}
