using System;

namespace SilverlightMappingToolBasic.Compendium
{
    public class ListNode : INodeType
    {
        public const string ListNodeTypeId = "{2561235F-7344-4eaa-BB1B-3B181D9414DD}";

        public ListNode()
        {
        }

        #region INodeType Members

        public Guid Id
        {
            get
            {
                return new Guid(ListNodeTypeId);
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
                return "List";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
