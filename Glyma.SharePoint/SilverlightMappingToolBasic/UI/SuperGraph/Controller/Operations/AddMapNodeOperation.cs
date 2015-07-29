using System.Collections.Generic;
using System.Windows;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class AddMapNodeOperation : AddNodeOperationBase
    {
        private NodeType _mapNodeType;

        public AddMapNodeOperation(IMapManager mapManager, INode map, Point location, Dictionary<string, string> defaultMetadata = null)
            : base(mapManager, map, location, defaultMetadata)
        {
        }

        protected override NodeType NodeType
        {
            get
            {
                if (_mapNodeType == null)
                {
                    _mapNodeType = MapManager.NodeTypes["CompendiumMapNode"];
                }

                return _mapNodeType;
            }
        }
    }
}
