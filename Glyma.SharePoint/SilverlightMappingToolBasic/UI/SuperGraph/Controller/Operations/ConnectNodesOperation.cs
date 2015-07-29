using System;
using TransactionalNodeService.Service;
using Proxy = TransactionalNodeService.Proxy;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class ConnectNodesOperation : CommonOperationBase, ISuperGraphOperation
    {
        private Proxy.IRelationship _inProcessRelationship;

        public event EventHandler<NodeOperationCompletedArgs> OperationCompleted;

        public ConnectNodesOperation(Proxy.IMapManager mapManager)
            : base(mapManager)
        {
        }

        public Proxy.INode From
        {
            get;
            set;
        }

        public Proxy.INode To
        {
            get;
            set;
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
            var chain = new TransactionFramework.TransactionChain();

            _inProcessRelationship = MapManager.CreateRelationship(From.DomainId, From.RootMapId.Value, FromToRelationshipType, string.Empty, ref chain);

            _inProcessRelationship.ConnectNode(FromConnectionType, From, ref chain);
            _inProcessRelationship.ConnectNode(ToConnectionType, To, ref chain);
            Response = new TransactionFramework.InProcessTransactionResponse();
            Response.Relationships.Add(_inProcessRelationship);
            chain.TransactionCompleted += ChainOnTransactionCompleted;
            MapManager.ExecuteTransaction(chain);
        }
    }
}
