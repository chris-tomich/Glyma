using TransactionalNodeService.Proxy.Universal.Collections;
using TransactionalNodeService.Proxy.Universal.Collections.NodeContainers;

namespace TransactionalNodeService.Proxy.Universal
{
    internal interface INodeManager
    {
        void Load(ConnectionSet connectionSet);
        void Clear();
        void UnionWith(NodeSet nodes);
    }
}
