using System;
using System.Collections.Generic;
using Proxy = TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel
{
    public interface INode : IStorageElement
    {
        string[] Notes { get; set; }
        string Attachment { get; set; }
        Proxy.NodeType NodeType { get; set; }
        IDescriptor[] Descriptors { get; set; }
        IDictionary<string, string> Properties { get; }

        void AddNote(string note);
        void AddDescriptor(IDescriptor descriptor);
        void RemoveNote(string note);
        void RemoveDescriptor(IDescriptor descriptor);
    }
}
