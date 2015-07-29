using System;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Selector;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses
{
    public interface INodeMotionHandler
    {
        Canvas ParentSurface { get; }
        UIElement MouseManagingControl { get; }

        event EventHandler<NodeMotionHandlerEventArgs> NodeInMotionEvent;

        void OnLeftButtonDragMove(object sender, MouseEventArgs e);
        void OnLeftButtonDragReleased(object sender, MouseManagerEventArgs e);
        void OnLeftButtonDragStart(object sender, MouseManagerEventArgs e);

        void OnMapMoved(object sender, MoveTransformEventArgs e);
    }
}
