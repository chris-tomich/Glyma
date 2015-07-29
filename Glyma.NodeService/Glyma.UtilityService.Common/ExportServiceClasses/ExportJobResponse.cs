using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Glyma.UtilityService.Common.Model;

namespace Glyma.UtilityService.Common.ExportServiceClasses
{
    [DataContract(Namespace = "http://sevensigma.com.au/GlymaUtilityService/ExportObjects")]
    public class ExportJobResponse
    {
        [DataMember]
        public ExportJob ExportJob
        {
            get;
            set;
        }
    }
}
