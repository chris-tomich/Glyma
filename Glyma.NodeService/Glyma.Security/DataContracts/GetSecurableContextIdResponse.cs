using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Glyma.Security
{
    [DataContract]
    public class GetSecurableContextIdResponse : ResponseObject
    {
        [DataMember]
        public int Result
        {
            get;
            set;
        }
    }
}
