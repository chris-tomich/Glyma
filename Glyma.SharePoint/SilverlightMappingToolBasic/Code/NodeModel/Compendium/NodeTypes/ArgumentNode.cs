using System;

namespace SilverlightMappingToolBasic.Compendium
{
    public class ArgumentNode : INodeType
    {
        public const string ArgumentNodeTypeId = "{4D565B5B-AD90-41b3-AEEE-5BCBC0A29109}";

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
                return "Argument";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
