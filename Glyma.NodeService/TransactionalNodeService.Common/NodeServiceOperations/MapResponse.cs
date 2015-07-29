using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransactionalNodeService.Common.Model;
using System.Runtime.Serialization;

namespace TransactionalNodeService.Common
{
    [DataContract(Namespace = "http://sevensigma.com.au/TransactionalNodeService/OpObjects", Name = "RS")]
    public class MapResponse
    {
        public MapResponse()
        {
            Changes = new List<MapChange>();
        }

        [DataMember(Name = "C")]
        public List<MapChange> Changes
        {
            get;
            set;
        }
    }
}