using System;

namespace NodeService
{
    public class ListNode : INodeType
    {
        public const string ListNodeTypeId = "{7D3C9B87-F31D-400F-A375-ABC0D1888625}";

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
                return "CompendiumListNode";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
