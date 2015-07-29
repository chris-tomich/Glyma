using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NodeService
{
    public class SoapMetadata
    {
        public SoapMetadata()
        {
        }

        public SoapMetadataType MetadataType
        {
            get;
            set;
        }

        public string MetadataName
        {
            get;
            set;
        }

        public string MetadataValue
        {
            get;
            set;
        }
    }
}