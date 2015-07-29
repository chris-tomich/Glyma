using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Glyma.UtilityService.Common
{
    [DataContract]
    public class ExportError
    {
        [DataMember]
        public string ErrorMessage
        {
            get;
            set;
        }
    }
}
