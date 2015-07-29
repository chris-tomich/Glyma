using System;

namespace SilverlightMappingToolBasic.Compendium
{
    public class IdeaNode : INodeType
    {
        public const string IdeaNodeTypeId = "{FAB0E270-71BE-4721-841B-7E2EE0D7C8E9}";

        public IdeaNode()
        {
        }

        #region INodeType Members

        public Guid Id
        {
            get
            {
                return new Guid(IdeaNodeTypeId);
            }
            set
            {
                return;
            }
        }

        public string Name
        {
            get
            {
                return "Idea";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
