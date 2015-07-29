using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Glyma.Security
{
    [DataContract]
    public class GetPermissionNameResponse : ResponseObject
    {
        [DataMember]
        public string Result
        {
            get;
            set;
        }
    }
}
