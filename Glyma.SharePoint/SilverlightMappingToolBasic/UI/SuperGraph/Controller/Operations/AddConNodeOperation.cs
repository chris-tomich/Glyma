using System.Collections.Generic;
using System.Windows;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class AddConNodeOperation : AddNodeOperationBase
    {
        private NodeType _conNodeType;

        public AddConNodeOperation(IMapManager mapManager, INode map, Point location, Dictionary<string, string> defaultMetadata = null)
            : base(mapManager, map, location, defaultMetadata)
        {
        }

        protected override NodeType NodeType
        {
            get
            {
                if (_conNodeType == null)
                {
                    _conNodeType = MapManager.NodeTypes["CompendiumConNode"];
                }

                return _conNodeType;
            }
        }
    }
}
