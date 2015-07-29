using System.Collections.Generic;
using System.Windows;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class AddDecisionNodeOperation : AddNodeOperationBase
    {
        private NodeType _conNodeType;

        public AddDecisionNodeOperation(IMapManager mapManager, INode map, Point location, Dictionary<string, string> defaultMetadata = null)
            : base(mapManager, map, location, defaultMetadata)
        {
        }

        protected override NodeType NodeType
        {
            get
            {
                if (_conNodeType == null)
                {
                    _conNodeType = MapManager.NodeTypes["CompendiumDecisionNode"];
                }

                return _conNodeType;
            }
        }
    }
}
