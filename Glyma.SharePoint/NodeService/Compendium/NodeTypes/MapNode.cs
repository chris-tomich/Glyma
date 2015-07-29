using System;

namespace NodeService
{
    public class MapNode : INodeType
    {
        public const string MapNodeTypeId = "{B8C354CB-C7D0-4982-9A0F-6C4368FAB749}";

        public MapNode()
        {
        }

        #region INodeType Members

        public Guid Id
        {
            get
            {
                return new Guid(MapNodeTypeId);
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
                return "CompendiumMapNode";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
