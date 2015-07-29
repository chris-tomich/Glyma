using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Glyma.Security
{
    [DataContract]
    public class GetAllSecurityGroupsResponse : ResponseObject
    {
        [DataMember]
        public IDictionary<GlymaPermissionLevel, IList<GlymaSecurityGroup>> Result;
    }
}
