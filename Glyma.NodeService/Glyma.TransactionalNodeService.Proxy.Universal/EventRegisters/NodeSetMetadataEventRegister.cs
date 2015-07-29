using TransactionalNodeService.Proxy.Universal.EventArgs;

namespace TransactionalNodeService.Proxy.Universal.EventRegisters
{
    public class NodeSetMetadataEventRegister : EventRegister<INode, MetadataChangedEventArgs>
    {
        public NodeSetMetadataEventRegister()
            : base()
        {
        }
    }
}
