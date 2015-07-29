using System;

namespace SilverlightMappingToolBasic.Compendium
{
    public class CompendiumToDescriptor : IDescriptorType
    {
        public CompendiumToDescriptor()
        {
        }

        #region ITypeElement Members

        public Guid Id
        {
            get
            {
                return new Guid("{325391EC-CF71-4d7b-868C-C4CB5A69E352}");
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
                return "To";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
