using System;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel.NodeTypes
{
    public class ArgumentNode : INodeType
    {
        public const string ArgumentNodeTypeId = "{042E7E3B-8A5F-4A52-B1DD-3361A3ACD72A}";

        public ArgumentNode()
        {
        }

        #region INodeType Members

        public Guid Id
        {
            get
            {
                return new Guid(ArgumentNodeTypeId);
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
                return "CompendiumArgumentNode";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
