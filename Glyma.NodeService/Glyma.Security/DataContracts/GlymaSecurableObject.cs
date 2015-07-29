using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Glyma.Security
{
    [DataContract]
    public class GlymaSecurableObject
    {
        [DataMember]
        public Guid SecurableParentUid
        {
            get;
            set;
        }

        [DataMember]
        public Guid SecurableObjectUid
        {
            get;
            set;
        }
    }
}
