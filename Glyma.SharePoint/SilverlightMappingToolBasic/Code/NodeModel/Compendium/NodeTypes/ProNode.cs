using System;

namespace SilverlightMappingToolBasic.Compendium
{
    public class ProNode : INodeType
    {
        public const string ProNodeTypeId = "{013EAEA2-A2DE-4304-858C-EA9EF91B7AF4}";

        public ProNode()
        {
        }

        #region INodeType Members

        public Guid Id
        {
            get
            {
                return new Guid(ProNodeTypeId);
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
                return "Pro";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
