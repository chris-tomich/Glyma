using System;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel.NodeTypes
{
    public class DecisionNode : INodeType
    {
        public const string DecisionNodeTypeId = "{53EC78E3-F189-4340-B251-AAF9D78CF56D}";

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
                return "CompendiumDecisionNode";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
