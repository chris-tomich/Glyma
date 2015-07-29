using System;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Selector
{
    public class MoveTransformEventArgs : EventArgs
    {
        public double X;
        public double Y;

        public MoveTransformEventArgs(double x = 0, double y = 0)
        {
            X = x;
            Y = y;
        }
    }
}
