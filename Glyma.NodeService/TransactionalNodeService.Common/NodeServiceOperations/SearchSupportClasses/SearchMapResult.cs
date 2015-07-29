using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TransactionalNodeService.Common
{
    [DataContract]
    public class SearchMapResult
    {
        [DataMember]
        public int Count
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<Guid, SearchedNode> SearchedNodes
        {
            get;
            set;
        }
    }
}