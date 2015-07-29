using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Glyma.Security
{
    [DataContract]
    public class GetCurrentUserAccessToObjectResponse : ResponseObject
    {
        [DataMember]
        public GlymaPermissionLevel HighestPermissionLevel
        {
            get;
            set;
        }

        [DataMember]
        public bool HasAccess
        {
            get;
            set;
        }
    }
}
