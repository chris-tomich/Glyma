using System;

namespace SilverlightMappingToolBasic
{
    public interface IStorageElement
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string CreatedBy { get; set; }
        string LastModifiedBy { get; set; }
        DateTime Created { get; set; }
        DateTime LastModified { get; set; }

        bool Equals(IStorageElement secondElement);
    }
}
