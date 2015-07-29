using System;
using System.Windows.Input;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse
{
    public interface IMouseManager
    {
        event MouseEventHandler MouseMove;
        event MouseEventHandler LeftButtonDragMove;
        event MouseEventHandler RightButtonDragMove;

        event EventHandler<MouseManagerEventArgs> LeftButtonDragStart;
        event EventHandler<MouseManagerEventArgs> LeftButtonDragReleased;
        event EventHandler<MouseManagerEventArgs> LeftButtonHeldClick;
        event EventHandler<MouseManagerEventArgs> LeftButtonSingleClick;
        event EventHandler<MouseManagerEventArgs> LeftButtonDoubleClick;

        event EventHandler<MouseManagerEventArgs> RightButtonDragStart;
        event EventHandler<MouseManagerEventArgs> RightButtonDragReleased;
        event EventHandler<MouseManagerEventArgs> RightButtonHeldClick;
        event EventHandler<MouseManagerEventArgs> RightButtonSingleClick;
        event EventHandler<MouseManagerEventArgs> RightButtonDoubleClick;


        void OnMouseMove(object sender, MouseEventArgs e);

        #region Left

        void OnLeftButtonDragStart(object sender, MouseManagerEventArgs e);
        void OnLeftButtonDragMove(object sender, MouseEventArgs e);
        void OnLeftButtonDragReleased(object sender, MouseManagerEventArgs e);
        void OnLeftButtonSingleClick(object sender, MouseManagerEventArgs e);
        void OnLeftButtonDoubleClick(object sender, MouseManagerEventArgs e);
        void OnLeftButtonHeldClick(object sender, MouseManagerEventArgs e);

        #endregion

        #region Right

        void OnRightButtonDragStart(object sender, MouseManagerEventArgs e);
        void OnRightButtonDragMove(object sender, MouseEventArgs e);
        void OnRightButtonDragReleased(object sender, MouseManagerEventArgs e);
        void OnRightButtonSingleClick(object sender, MouseManagerEventArgs e);
        void OnRightButtonDoubleClick(object sender, MouseManagerEventArgs e);
        void OnRightButtonHeldClick(object sender, MouseManagerEventArgs e);

        #endregion
    }
}
