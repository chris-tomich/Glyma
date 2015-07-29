using System;

namespace SilverlightMappingToolBasic.Compendium
{
    public class ReferenceNode : INodeType
    {
        public const string ReferenceNodeTypeId = "{026A973B-96BA-4289-B768-38124AAEF320}";

        public ReferenceNode()
        {
        }

        #region INodeType Members

        public Guid Id
        {
            get
            {
                return new Guid(ReferenceNodeTypeId);
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
                return "Reference";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
