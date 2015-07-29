using System;

namespace NodeService
{
    public class ReferenceNode : INodeType
    {
        public const string ReferenceNodeTypeId = "{8F3DA942-06C4-4075-AD8B-B51361ABC900}";

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
                return "CompendiumReferenceNode";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
