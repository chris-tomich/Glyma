using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Glyma.UtilityService.Common.Model
{
    [DataContract(Namespace = "http://sevensigma.com.au/GlymaUtilityService/ExportObjects")]
    public class GlymaUser
    {
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
        public string DisplayName
        {
            get;
            set;
        }
    }
}
