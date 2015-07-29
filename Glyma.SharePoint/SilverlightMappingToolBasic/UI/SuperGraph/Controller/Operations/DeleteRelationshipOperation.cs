using System;
using TransactionalNodeService.Service;
using Proxy = TransactionalNodeService.Proxy;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class DeleteRelationshipOperation : CommonOperationBase, ISuperGraphOperation
    {
        public DeleteRelationshipOperation(Proxy.IMapManager mapManager) : base(mapManager)
        {
        }

        public Proxy.IRelationship Relationship
        {
            get
            {
                return ViewModeRelationship.Proxy;
            }
        }

        public ViewModel.Relationship ViewModeRelationship
        {
            get;
            set;
        }

        public event EventHandler<NodeOperationCompletedArgs> OperationCompleted;

        public void ExecuteOperation()
        {
            if (Relationship.RelationshipType == MapManager.RelationshipTypes["TransclusionFromToRelationship"])
            {
                ///TODO Chris: Need to check this
                //throw new NotSupportedException("Transclusions can't be deleted using a standard relationship delete method.");
            }

            TransactionFramework.TransactionChain chain = new TransactionFramework.TransactionChain();
            chain.TransactionCompleted += ChainOnTransactionCompleted;
            Relationship.Delete(ref chain);

            MapManager.ExecuteTransaction(chain);

            
        }

        private void ChainOnTransactionCompleted(object sender, CompleteTransactionCompletedEventArgs e)
        {
            if (OperationCompleted != null)
            {
                Response = new TransactionFramework.InProcessTransactionResponse();
                Response.Relationships.Add(Relationship);
                OperationCompleted(sender, new NodeOperationCompletedArgs{Response = Response, ViewModeRelationship = ViewModeRelationship});
            }
        }
    }
}
