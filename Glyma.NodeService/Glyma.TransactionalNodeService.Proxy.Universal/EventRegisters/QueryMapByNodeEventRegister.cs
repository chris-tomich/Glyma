using TransactionalNodeService.Proxy.Universal.EventArgs;

namespace TransactionalNodeService.Proxy.Universal.EventRegisters
{
    public class QueryMapByNodeEventRegister : EventRegister<INode, NodesEventArgs>
    {
        public QueryMapByNodeEventRegister()
            : base()
        {
        }
    }
}
