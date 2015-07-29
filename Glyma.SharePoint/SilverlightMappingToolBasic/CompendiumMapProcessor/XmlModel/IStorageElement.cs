using System;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel
{
    public interface IStorageElement
    {
        string Id { get; set; }
        string Name { get; set; }
        string CreatedBy { get; set; }
        string LastModifiedBy { get; set; }
        DateTime Created { get; set; }
        DateTime LastModified { get; set; }

        bool Equals(IStorageElement secondElement);
    }
}
