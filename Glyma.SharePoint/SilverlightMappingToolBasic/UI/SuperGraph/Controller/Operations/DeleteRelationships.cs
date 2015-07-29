using System;
using System.Collections.Generic;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using TransactionalNodeService.Service;
using Proxy = TransactionalNodeService.Proxy;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class DeleteRelationships : CommonOperationBase, ISuperGraphOperation
    {
        private List<KeyValuePair<Relationship, ModelOperationType>> _relationships;
        public event EventHandler<NodeOperationCompletedArgs> OperationCompleted;

        public DeleteRelationships(Proxy.IMapManager mapManager)
            : base(mapManager)
        {
            
        }

        public List<KeyValuePair<Relationship, ModelOperationType>> Relationships
        {
            get
            {
                if (_relationships == null)
                {
                    _relationships = new List<KeyValuePair<Relationship, ModelOperationType>>();
                }
                return _relationships;
            }
            set
            {
                Relationships = value;
            }
        }


        private void ChainOnTransactionCompleted(object sender, CompleteTransactionCompletedEventArgs completeTransactionCompletedEventArgs)
        {
            if (OperationCompleted != null)
            {
                OperationCompleted(sender, new NodeOperationCompletedArgs { Response = Response, Relationships = Relationships});
            }
        }


        public void ExecuteOperation()
        {
            var chain = new TransactionFramework.TransactionChain();
            chain.TransactionCompleted += ChainOnTransactionCompleted;
            foreach (var keyValuePair in Relationships)
            {
                if (keyValuePair.Key.Proxy.RelationshipType == MapManager.RelationshipTypes["TransclusionFromToRelationship"])
                {
                    ///TODO Chris: Need to check this
                    //throw new NotSupportedException("Transclusions can't be deleted using a standard relationship delete method.");
                }
                keyValuePair.Key.Proxy.Delete(ref chain);                
            }
            MapManager.ExecuteTransaction(chain);
        }
    }
}
