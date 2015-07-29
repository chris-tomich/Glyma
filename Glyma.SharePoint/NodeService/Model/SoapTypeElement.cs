using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NodeService
{
    [DataContract]
    [KnownType(typeof(SoapDescriptorType))]
    [KnownType(typeof(SoapMetadataType))]
    [KnownType(typeof(SoapNodeType))]
    [KnownType(typeof(SoapRelationshipType))]
    public abstract class SoapTypeElement : ISoapTypeElement
    {
        #region ISoapTypeElement Members

        [DataMember]
        public abstract Guid Id
        {
            get;
            set;
        }

        [DataMember]
        public abstract string Name
        {
            get;
            set;
        }

        #endregion
    }
}