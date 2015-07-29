using System;
using System.Runtime.Serialization;

namespace TransactionalNodeService.Common
{
    [DataContract]
    public class GlymaSessionConfiguration
    {
        [DataMember]
        public string ParametersDbServer
        {
            get;
            set;
        }

        [DataMember]
        public string ParametersDbName
        {
            get;
            set;
        }

        [DataMember]
        public string SessionDbServer
        {
            get;
            set;
        }

        [DataMember]
        public string SessionDbName
        {
            get;
            set;
        }

        [DataMember]
        public string MapDbServer
        {
            get;
            set;
        }

        [DataMember]
        public string MapDbName
        {
            get;
            set;
        }

        [DataMember]
        public int SecurableContextId
        {
            get;
            set;
        }

        [DataMember]
        public string SecurityDbServer
        {
            get;
            set;
        }

        [DataMember]
        public string SecurityDbName
        {
            get;
            set;
        }
    }
}
