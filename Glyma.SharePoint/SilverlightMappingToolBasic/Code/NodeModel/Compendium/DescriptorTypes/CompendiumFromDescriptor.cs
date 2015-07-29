using System;

namespace SilverlightMappingToolBasic.Compendium
{
    public class CompendiumFromDescriptor : IDescriptorType
    {
        public CompendiumFromDescriptor()
        {
        }

        #region ITypeElement Members

        public Guid Id
        {
            get
            {
                return new Guid("{26E2FE3F-18C8-4b56-BA75-E1309E06D302}");
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
                return "From";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
