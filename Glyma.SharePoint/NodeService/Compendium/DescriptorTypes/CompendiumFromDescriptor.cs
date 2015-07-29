using System;

namespace NodeService
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
                return new Guid("{96DA1782-058C-4F9B-BB1A-31B048F8C75A}");
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
