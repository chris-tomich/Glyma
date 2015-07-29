using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Glyma.Security
{
    [DataContract]
    public class GlymaSecurityAssociation
    {
        [DataMember]
        public GlymaSecurityGroup GlymaSecurityGroup
        {
            get;
            set;
        }

        [DataMember]
        public GlymaSecurableObject SecurableObject
        {
            get;
            set;
        }

        [DataMember]
        public bool BreakInheritance
        {
            get;
            set;
        }

        [DataMember]
        public bool Value
        {
            get;
            set;
        }
    }
}
