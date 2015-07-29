using System;
using System.Windows;
using System.Windows.Input;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse
{
    public abstract class MouseManagerBase: IMouseManager
    {
        private MouseButtonManager _leftButtonManager;
        private MouseButtonManager _rightButtonManager;

        protected MouseManagerBase(UIElement control)
        {
            Control = control;

            Control.MouseMove += OnMouseMove;

            Control.MouseLeftButtonDown += LeftButtonState.OnMouseButtonDown;
            Control.MouseLeftButtonUp += LeftButtonState.OnMouseButtonUp;
            Control.MouseMove += LeftButtonState.OnMouseMove;
            Control.MouseLeave += LeftButtonState.OnMouseLeave;

            Control.MouseRightButtonDown += RightButtonState.OnMouseButtonDown;
            Control.MouseRightButtonUp += RightButtonState.OnMouseButtonUp;
            Control.MouseMove += RightButtonState.OnMouseMove;
            Control.MouseLeave += RightButtonState.OnMouseLeave;
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (LeftButtonState.IsInactive && RightButtonState.IsInactive)
            {
                if (MouseMove != null)
                {
                    MouseMove(sender, e);
                }
            }
        }

        public event MouseEventHandler MouseMove;
        public event MouseEventHandler LeftButtonDragMove;
        public event MouseEventHandler RightButtonDragMove;

        public event EventHandler<MouseManagerEventArgs> LeftButtonDragStart;
        public event EventHandler<MouseManagerEventArgs> LeftButtonDragReleased;
        public event EventHandler<MouseManagerEventArgs> LeftButtonHeldClick;
        public event EventHandler<MouseManagerEventArgs> LeftButtonSingleClick;
        public event EventHandler<MouseManagerEventArgs> LeftButtonDoubleClick;

        public event EventHandler<MouseManagerEventArgs> RightButtonDragStart;
        public event EventHandler<MouseManagerEventArgs> RightButtonDragReleased;
        public event EventHandler<MouseManagerEventArgs> RightButtonHeldClick;
        public event EventHandler<MouseManagerEventArgs> RightButtonSingleClick;
        public event EventHandler<MouseManagerEventArgs> RightButtonDoubleClick;

        private UIElement Control
        {
            get;
            set;
        }

        private MouseButtonManager LeftButtonState
        {
            get
            {
                if (_leftButtonManager == null)
                {
                    _leftButtonManager = new MouseButtonManager();

                    _leftButtonManager.Control = Control;
                    _leftButtonManager.ButtonDragStart += OnLeftButtonDragStart;
                    _leftButtonManager.ButtonDragMove += OnLeftButtonDragMove;
                    _leftButtonManager.ButtonDragReleased += OnLeftButtonDragReleased;
                    _leftButtonManager.ButtonSingleClick += OnLeftButtonSingleClick;
                    _leftButtonManager.ButtonDoubleClick += OnLeftButtonDoubleClick;
                    _leftButtonManager.ButtonHeldClick += OnLeftButtonHeldClick;
                }

                return _leftButtonManager;
            }
        }

        private MouseButtonManager RightButtonState
        {
            get
            {
                if (_rightButtonManager == null)
                {
                    _rightButtonManager = new MouseButtonManager();

                    _leftButtonManager.Control = Control;
                    _rightButtonManager.ButtonDragStart += OnRightButtonDragStart;
                    _rightButtonManager.ButtonDragMove += OnRightButtonDragMove;
                    _rightButtonManager.ButtonDragReleased += OnRightButtonDragReleased;
                    _rightButtonManager.ButtonSingleClick += OnRightButtonSingleClick;
                    _rightButtonManager.ButtonDoubleClick += OnRightButtonDoubleClick;
                    _rightButtonManager.ButtonHeldClick += OnRightButtonHeldClick;
                }

                return _rightButtonManager;
            }
        }

        #region Left Mouse Button Event Callers
        public void OnLeftButtonDragStart(object sender, MouseManagerEventArgs e)
        {
            if (LeftButtonDragStart != null)
            {
                LeftButtonDragStart(this, e);
            }
        }

        public void OnLeftButtonDragMove(object sender, MouseEventArgs e)
        {
            if (LeftButtonDragMove != null)
            {
                LeftButtonDragMove(sender, e);
            }
        }

        public void OnLeftButtonDragReleased(object sender, MouseManagerEventArgs e)
        {
            if (LeftButtonDragReleased != null)
            {
                LeftButtonDragReleased(this, e);
            }
        }

        public void OnLeftButtonSingleClick(object sender, MouseManagerEventArgs e)
        {
            if (LeftButtonSingleClick != null)
            {
                LeftButtonSingleClick(this, e);
            }
        }

        public void OnLeftButtonDoubleClick(object sender, MouseManagerEventArgs e)
        {
            if (LeftButtonDoubleClick != null)
            {
                LeftButtonDoubleClick(this, e);
            }
        }

        public void OnLeftButtonHeldClick(object sender, MouseManagerEventArgs e)
        {
            if (LeftButtonHeldClick != null)
            {
                LeftButtonHeldClick(this, e);
            }
        }
        #endregion

        #region Right Mouse Button Event Callers
        public void OnRightButtonDragStart(object sender, MouseManagerEventArgs e)
        {
            if (RightButtonDragStart != null)
            {
                RightButtonDragStart(this, e);
            }
        }

        public void OnRightButtonDragMove(object sender, MouseEventArgs e)
        {
            if (RightButtonDragMove != null)
            {
                RightButtonDragMove(sender, e);
            }
        }

        public void OnRightButtonDragReleased(object sender, MouseManagerEventArgs e)
        {
            if (RightButtonDragReleased != null)
            {
                RightButtonDragReleased(this, e);
            }
        }

        public void OnRightButtonSingleClick(object sender, MouseManagerEventArgs e)
        {
            if (RightButtonSingleClick != null)
            {
                RightButtonSingleClick(this, e);
            }
        }

        public void OnRightButtonDoubleClick(object sender, MouseManagerEventArgs e)
        {
            if (RightButtonDoubleClick != null)
            {
                RightButtonDoubleClick(this, e);
            }
        }

        public void OnRightButtonHeldClick(object sender, MouseManagerEventArgs e)
        {
            if (RightButtonHeldClick != null)
            {
                RightButtonHeldClick(this, e);
            }
        }
        #endregion
    }
}
