using System;

namespace NodeService
{
    public class IdeaNode : INodeType
    {
        public const string IdeaNodeTypeId = "{3B53600F-39EC-42FB-B08A-325062037130}";

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
                return "CompendiumIdeaNode";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
