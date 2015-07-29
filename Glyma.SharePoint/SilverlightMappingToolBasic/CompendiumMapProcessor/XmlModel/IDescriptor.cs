using System;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel
{
    public interface IDescriptor
    {
        INode Node { get; set; }
        IRelationship Relationship { get; set; }
        IDescriptorType DescriptorType { get; set; }
    }
}
