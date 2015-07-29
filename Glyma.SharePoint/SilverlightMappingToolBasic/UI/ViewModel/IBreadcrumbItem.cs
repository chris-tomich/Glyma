using System.ComponentModel;
using System.Windows;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;

namespace SilverlightMappingToolBasic.UI.ViewModel
{
    public interface IBreadcrumbItem : INotifyPropertyChanged
    {
        Node Node { get; set; }
        bool HasVisibleProperties { get; set; }
        Visibility PropertiesVisibility { get; }
        bool IsFirst { get; set; }
        Visibility ArrowVisibility { get; }
        string Name { get;}
    }
}
