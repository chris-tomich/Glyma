using System;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel.DescriptorTypes
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
                return new Guid("{07C91D35-4DAC-431B-966B-64C924B8CDAB}");
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
