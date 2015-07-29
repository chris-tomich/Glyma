using System;

namespace SilverlightMappingToolBasic.Compendium
{
    public class DecisionNode : INodeType
    {
        public const string DecisionNodeTypeId = "{6EED2DFC-0C14-4ac5-BD56-2C5F3C1796F9}";

        public DecisionNode()
        {
        }

        #region INodeType Members

        public Guid Id
        {
            get
            {
                return new Guid(DecisionNodeTypeId);
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
                return "Decision";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
