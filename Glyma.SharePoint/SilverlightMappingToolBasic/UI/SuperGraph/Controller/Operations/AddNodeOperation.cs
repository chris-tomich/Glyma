using System.Collections.Generic;
using System.Windows;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class AddNodeOperation : AddNodeOperationBase
    {
        private readonly NodeType _nodetype;

        public AddNodeOperation(string nodetype, IMapManager mapManager, INode map, Point location, Dictionary<string, string> defaultMetadata = null)
            : base(mapManager, map, location, defaultMetadata)
        {
            _nodetype = mapManager.NodeTypes[nodetype];
        }

        protected override NodeType NodeType
        {
            get
            {
                return _nodetype;
            }
        }
    }
}
