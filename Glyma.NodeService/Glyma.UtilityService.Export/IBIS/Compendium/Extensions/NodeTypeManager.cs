using TransactionalNodeService.Proxy.Universal;
using TransactionalNodeService.Proxy.Universal.Types;

namespace Glyma.UtilityService.Export.IBIS.Compendium.Extensions
{
    public class NodeTypeManager
    {
        private static IMapManager _mapManager;

        public NodeTypeManager(IMapManager mapManager)
        {
            _mapManager = mapManager;
        }

        public static NodeType GetNodeType()
        {
            return _mapManager.NodeTypes[""];
        }
    }
}
