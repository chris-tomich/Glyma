using System;
using System.Net;

namespace SilverlightMappingToolBasic.Compendium
{
    public class CompendiumNodeRelationshipBaseType : IRelationshipType
    {
        public CompendiumNodeRelationshipBaseType()
        {
        }

        #region INodeRelationshipType Members

        public Guid Id
        {
            get
            {
                return new Guid("{BF3E7DD5-CC5D-43c1-BCE5-2234508BF64B}");
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
                return "Basic Relationship";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
