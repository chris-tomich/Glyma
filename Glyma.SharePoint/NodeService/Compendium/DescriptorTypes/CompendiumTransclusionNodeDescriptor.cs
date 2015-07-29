using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeService
{
    public class CompendiumTransclusionNodeDescriptor : IDescriptorType
    {
        public CompendiumTransclusionNodeDescriptor()
        {
        }

        #region ITypeElement Members

        public Guid Id
        {
            get
            {
                return new Guid("{8DDB6352-5A2B-430A-B549-7CE346DD7C3D}");
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
                return "TransclusionNode";
            }
            set
            {
                return;
            }
        }

        #endregion
    }
}
