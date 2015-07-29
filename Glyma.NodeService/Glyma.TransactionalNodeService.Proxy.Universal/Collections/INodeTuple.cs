using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.Collections
{
    public interface INodeTuple
    {
        INode Node { get; }
        ConnectionType ConnectionType { get; }
    }
}
