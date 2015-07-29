using System;

namespace NodeService
{
    public interface IDescriptor
    {
        INode Node { get; set; }
        IRelationship Relationship { get; set; }
        IDescriptorType DescriptorType { get; set; }
    }
}
