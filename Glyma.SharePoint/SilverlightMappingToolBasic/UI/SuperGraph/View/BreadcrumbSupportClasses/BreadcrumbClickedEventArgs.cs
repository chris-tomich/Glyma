using System;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.BreadcrumbSupportClasses
{
    public class BreadcrumbClickedEventArgs : EventArgs
    {
        public bool IsParentRemoved { get; set; }
        public int ParentIndex { get; set; }
    }
}
