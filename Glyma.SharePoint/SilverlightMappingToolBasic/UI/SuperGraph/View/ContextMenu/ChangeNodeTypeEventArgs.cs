using System;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu
{
    public class ChangeNodeTypeEventArgs : EventArgs
    {
        public ChangeNodeTypeEnum ChangedTo
        {
            get;
            set;
        }
    }
}
