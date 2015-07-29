using System;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse
{
    public class MouseManagerEventArgs : EventArgs
    {
        public MouseStateProperties Start
        {
            get;
            set;
        }

        public MouseStateProperties End
        {
            get;
            set;
        }
    }
}
