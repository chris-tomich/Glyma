using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Glyma.Security
{
    [DataContract]
    public class GetSecurityGroupsResponse : ResponseObject
    {
        [DataMember]
        public IList<GlymaSecurityGroup> Result
        {
            get;
            set;
        }
    }
}
