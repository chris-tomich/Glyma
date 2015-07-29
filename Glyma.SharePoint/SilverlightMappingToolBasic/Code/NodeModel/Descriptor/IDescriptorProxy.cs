using System;

namespace SilverlightMappingToolBasic
{
    public interface IDescriptorProxy
    {
        Guid NodeId { get; }
        INodeProxy Node { get; set; }
        IRelationshipProxy Relationship { get; set; }
        IDescriptorTypeProxy DescriptorType { get; set; }
    }
}
