using System;
using System.Collections.Generic;

namespace SilverlightMappingToolBasic
{
    public interface IRelationshipProxy : IStorageElement
    {
        string[] Notes { get; set; }
        string[] Attachments { get; set; }
        //IDescriptorProxy[] Descriptors { get; set; }
        IRelationshipTypeProxy RelationshipType { get; set; }
        DescriptorCollection Descriptors { get; set; }

        void AddNote(string note);
        void AddAttachment(string attachment);
        //void AddDescriptor(IDescriptorProxy descriptor);
        void RemoveNote(string note);
        void RemoveAttachment(string attachment);
        //void RemoveDescriptor(IDescriptorProxy descriptor);
    }
}
