using System;
using SilverlightMappingToolBasic.UI.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.BreadcrumbSupportClasses
{
    public class BreadcrumbChangedEventArgs : EventArgs
    {
        public IBreadcrumbItem NewItem { get; set; }
        public IBreadcrumbItem OldItem { get; set; }
        public int ParentIndex { get; set; }
    }
}
