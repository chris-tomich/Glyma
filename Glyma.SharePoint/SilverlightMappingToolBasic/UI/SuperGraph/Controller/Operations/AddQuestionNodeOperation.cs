using System.Collections.Generic;
using System.Windows;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class AddQuestionNodeOperation : AddNodeOperationBase
    {
        private NodeType _questionNodeType;

        public AddQuestionNodeOperation(IMapManager mapManager, INode map, Point location, Dictionary<string, string> defaultMetadata = null)
            : base(mapManager, map, location, defaultMetadata)
        {
        }

        protected override NodeType NodeType
        {
            get
            {
                if (_questionNodeType == null)
                {
                    _questionNodeType = MapManager.NodeTypes["CompendiumQuestionNode"];
                }

                return _questionNodeType;
            }
        }
    }
}
