using TransactionalNodeService.Proxy;
using TransactionalNodeService.Soap.TransactionFramework;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public abstract class CommonOperationBase : CommonSuperGraphBase
    {
        protected CommonOperationBase(IMapManager mapManager) : base(mapManager)
        {
        }

        public InProcessTransactionResponse Response { get; protected set; }
    }
}
