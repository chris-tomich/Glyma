using System.Collections.Generic;
using System.Windows;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class AddProNodeOperation : AddNodeOperationBase
    {
        private NodeType _proNodeType;

        public AddProNodeOperation(IMapManager mapManager, INode map, Point location, Dictionary<string, string> defaultMetadata = null)
            : base(mapManager, map, location, defaultMetadata)
        {
        }

        protected override NodeType NodeType
        {
            get
            {
                if (_proNodeType == null)
                {
                    _proNodeType = MapManager.NodeTypes["CompendiumProNode"];
                }

                return _proNodeType;
            }
        }
    }
}
