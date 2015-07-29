using System.Collections.Generic;
using System.Windows;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class AddIdeaNodeOperation : AddNodeOperationBase
    {
        private NodeType _ideaNodeType;

        public AddIdeaNodeOperation(IMapManager mapManager, INode map, Point location, Dictionary<string, string> defaultMetadata = null)
            : base(mapManager, map, location, defaultMetadata)
        {
        }

        protected override NodeType NodeType
        {
            get
            {
                if (_ideaNodeType == null)
                {
                    _ideaNodeType = MapManager.NodeTypes["CompendiumIdeaNode"];
                }

                return _ideaNodeType;
            }
        }
    }
}
