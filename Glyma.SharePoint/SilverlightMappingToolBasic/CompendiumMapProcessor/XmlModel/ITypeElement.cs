using System;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel
{
    public interface ITypeElement
    {
        Guid Id { get; set; }
        string Name { get; set; }
    }
}
