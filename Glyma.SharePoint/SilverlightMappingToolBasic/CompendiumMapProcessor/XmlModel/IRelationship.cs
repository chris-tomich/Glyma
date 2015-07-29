using System;
using System.Collections.Generic;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel
{
    public interface IRelationship : IStorageElement
    {
        string[] Notes { get; set; }
        string[] Attachments { get; set; }
        IDescriptor[] Descriptors { get; set; }
        IRelationshipType RelationshipType { get; set; }

        void AddNote(string note);
        void AddAttachment(string attachment);
        void AddDescriptor(IDescriptor descriptor);
        void RemoveNote(string note);
        void RemoveAttachment(string attachment);
        void RemoveDescriptor(IDescriptor descriptor);
        //IEnumerable<INode> GetNodesByDescriptorType(INode context, IDescriptorType descriptorType);
        //IEnumerable<INode> GetNodesByDescriptorTypeId(INode context, Guid descriptorTypeId);
        //IEnumerable<INode> GetNodesByDescriptorTypeName(INode context, string descriptorTypeName);
    }
}
