using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeService
{
    public class CompendiumTransclusionMapDescriptor : IDescriptorType
    {
        public CompendiumTransclusionMapDescriptor()
        {
        }

        #region ITypeElement Members

        public Guid Id
        {
            get
            {
                return new Guid("{47107835-A485-4A23-BF0C-3FC631A07777}");
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
                return "TransclusionMap";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
