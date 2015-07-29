using System;

namespace SilverlightMappingToolBasic.Compendium
{
    public class ConNode : INodeType
    {
        public const string ConNodeTypeId = "{68281F7C-037E-4032-98A3-303180B92141}";

        public ConNode()
        {
        }

        #region INodeType Members

        public Guid Id
        {
            get
            {
                return new Guid(ConNodeTypeId);
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
                return "Con";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
