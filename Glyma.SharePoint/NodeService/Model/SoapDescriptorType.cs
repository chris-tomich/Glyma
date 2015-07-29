using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace NodeService
{
    [DataContract]
    public class SoapDescriptorType : SoapTypeElement
    {
        public SoapDescriptorType()
        {
        }

        [DataMember]
        public override Guid Id
        {
            get;
            set;
        }

        [DataMember]
        public override string Name
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<string, SoapMetadata> Metadata
        {
            get;
            set;
        }
    }
}