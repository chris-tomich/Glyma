using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Data;

namespace TransactionalNodeService.Model
{
    [DataContract(Namespace = "http://sevensigma.com.au/TransactionalNodeService/OpObjects", Name = "DE")]
    public class Descriptor : IMapElementLoader
    {
        public Descriptor()
        {
        }

        [DataMember(Name = "D")]
        public Guid DescriptorUid
        {
            get;
            set;
        }

        [DataMember(Name = "T")]
        public Guid DescriptorTypeUid
        {
            get;
            set;
        }

        [DataMember(Name = "N")]
        public Guid NodeUid
        {
            get;
            set;
        }

        [DataMember(Name = "R")]
        public Guid RelationshipUid
        {
            get;
            set;
        }

        public void LoadElement(IDataRecord record)
        {
            DescriptorUid = (Guid)record["DescriptorUid"];
            DescriptorTypeUid = (Guid)record["DescriptorTypeUid"];
            NodeUid = (Guid)record["NodeUid"];
            RelationshipUid = (Guid)record["RelationshipUid"];
        }
    }
}