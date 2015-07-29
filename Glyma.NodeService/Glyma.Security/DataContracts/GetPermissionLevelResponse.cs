using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Glyma.Security
{
    [DataContract]
    public class GetPermissionLevelResponse : ResponseObject
    {
        [DataMember]
        public GlymaPermissionLevel Result
        {
            get;
            set;
        }
    }
}
