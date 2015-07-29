using System;

namespace NodeService
{
    public interface ITypeElement
    {
        Guid Id { get; set; }
        string Name { get; set; }
    }
}
