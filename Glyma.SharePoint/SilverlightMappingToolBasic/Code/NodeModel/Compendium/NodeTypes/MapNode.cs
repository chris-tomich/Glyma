using System;

namespace SilverlightMappingToolBasic.Compendium
{
    public class MapNode : INodeType
    {
        public const string MapNodeTypeId = "{43F7C370-AF9A-40fe-835B-53A437310403}";

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
                return "Map";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
