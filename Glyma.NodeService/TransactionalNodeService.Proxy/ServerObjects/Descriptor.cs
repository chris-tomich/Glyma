using System;

namespace TransactionalNodeService.Proxy.ServerObjects
{
    public class Descriptor
    {
        public Descriptor()
        {
        }

        ///[DataMember(Name = "D")]
        public Guid DescriptorUid
        {
            get;
            set;
        }

        //[DataMember(Name = "T")]
        public Guid DescriptorTypeUid
        {
            get;
            set;
        }

        //[DataMember(Name = "N")]
        public Guid NodeUid
        {
            get;
            set;
        }

        //[DataMember(Name = "R")]
        public Guid RelationshipUid
        {
            get;
            set;
        }
    }
}