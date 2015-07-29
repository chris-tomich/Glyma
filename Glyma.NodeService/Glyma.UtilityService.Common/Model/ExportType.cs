using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Glyma.UtilityService.Common.Model
{
    [DataContract(Namespace = "http://sevensigma.com.au/GlymaUtilityService/ExportObjects")]
    public enum ExportType
    {
        [EnumMember]
        Compendium,
        [EnumMember]
        PDF,
        [EnumMember]
        Word,
        [EnumMember]
        GlymaXml
    }
}
