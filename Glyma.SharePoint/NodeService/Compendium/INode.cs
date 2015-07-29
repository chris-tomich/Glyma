using System;
using System.Collections.Generic;

namespace NodeService
{
    public interface INode : IStorageElement
    {
        string[] Notes { get; set; }
        string Attachment { get; set; }
        INodeType NodeType { get; set; }
        IDescriptor[] Descriptors { get; set; }
        IDictionary<string, string> Properties { get; }

        void AddNote(string note);
        void AddDescriptor(IDescriptor descriptor);
        void RemoveNote(string note);
        void RemoveDescriptor(IDescriptor descriptor);
    }
}
