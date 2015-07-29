using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Glyma.Security
{
    [DataContract]
    public class ResponseObject
    {
        [DataMember(EmitDefaultValue = true)]
        public bool HasError
        {
            get;
            set;
        }

        [DataMember]
        public string ErrorMessage
        {
            get;
            set;
        }
    }
}
