using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeService
{
    public class CompendiumMapRelationshipBaseType : IRelationshipType
    {
        #region ITypeElement Members

        public Guid Id
        {
            get
            {
                return new Guid("{2AB5A77B-8EDD-48E9-8FFB-E07397ECD0D7}");
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
                return "Map Container Relationship";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
