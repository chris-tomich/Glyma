using System;

namespace SilverlightMappingToolBasic
{
    public interface INodeAttachment : IStorageElement
    {
        byte[] Data { get; set; }
    }
}
