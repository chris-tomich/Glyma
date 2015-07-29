using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TransactionalNodeService.Model
{
    [DataContract]
    public class GlymaVersion
    {
        public GlymaVersion()
        {
        }

        [DataMember]
        public int Major
        {
            get;
            set;
        }

        [DataMember]
        public int Minor
        {
            get;
            set;
        }

        [DataMember]
        public int Build
        {
            get;
            set;
        }

        [DataMember]
        public int Revision
        {
            get;
            set;
        }
    }
}