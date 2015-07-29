using System;
using System.Windows.Media;

namespace SilverlightMappingToolBasic
{
    public interface IRelationshipTypeProxy : ITypeElement
    {
        Color LineColor { get; set; }
        Brush LineStyle { get; set; }
        string LineDescription { get; set; }
    }
}
