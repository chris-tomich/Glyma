using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Glyma.Security
{
    [DataContract]
    public class SecurityAssociations
    {
        [DataMember]
        public bool IsInherited
        {
            get;
            set;
        }

        [DataMember]
        public IDictionary<GlymaSecurityGroup, bool> HasAssociations
        {
            get;
            set;
        }
    }
}
