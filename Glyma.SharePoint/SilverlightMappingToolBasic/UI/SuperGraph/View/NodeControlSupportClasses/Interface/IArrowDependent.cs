using System;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses.Interface
{
    public interface IArrowDependent
    {
        event EventHandler<VisibilityChangedEventArgs> VisibilityChanged;
        event EventHandler Deleted;
    }
}
