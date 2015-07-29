using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Glyma.UtilityService.Common.Model
{
    [DataContract(Namespace = "http://sevensigma.com.au/GlymaUtilityService/ExportObjects")]
    public enum ExportStatus
    {
        [EnumMember]
        Scheduled,
        [EnumMember]
        Processing,
        [EnumMember]
        Completed,
        [EnumMember]
        Error
    }
}
