using System;
using System.Windows.Media;

namespace SilverlightMappingToolBasic
{
    public interface IDescriptorTypeProxy : ITypeElement
    {
        Brush DescriptorImage { get; set; }
    }
}
