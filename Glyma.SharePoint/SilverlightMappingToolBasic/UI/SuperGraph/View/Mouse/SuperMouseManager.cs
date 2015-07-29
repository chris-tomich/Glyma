using System;
using System.Windows;
using System.Windows.Input;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Interface;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse
{
    public class SuperMouseManager
    {
        private IMouseManager _pointerMouseManager;
        private IMouseManager _explorerMouseManager;
        private MouseButtonManager _leftButtonManager;
        private MouseButtonManager _rightButtonManager;
        


        public SuperMouseManager(UIElement control)
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

                    _rightButtonManager.Control = Control;
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

        

        private UIElement Control
        {
            get; 
            set;
        }

        public IMouseManager MouseManager
        {
            get
            {
                return App.UserStyle == UserStyle.Author ? PointerMouseManager : ExplorerMouseManager;
            }
        }

        public IMouseManager PointerMouseManager
        {
            get
            {
                if (_pointerMouseManager == null)
                {
                    _pointerMouseManager = new MouseManager(Control);

                }
                return _pointerMouseManager;
            }
        }

        public IMouseManager ExplorerMouseManager
        {
            get
            {
                return _explorerMouseManager ?? (_explorerMouseManager = new MouseManager(Control));
            }
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            MouseManager.OnMouseMove(sender, e);
        }

        #region Left Mouse Button Event Callers
        public void OnLeftButtonDragStart(object sender, MouseManagerEventArgs e)
        {
            MouseManager.OnLeftButtonDragStart(this, e);
        }

        public void OnLeftButtonDragMove(object sender, MouseEventArgs e)
        {
            MouseManager.OnLeftButtonDragMove(sender, e);
        }

        public void OnLeftButtonDragReleased(object sender, MouseManagerEventArgs e)
        {
            MouseManager.OnLeftButtonDragReleased(this, e);
        }

        public void OnLeftButtonSingleClick(object sender, MouseManagerEventArgs e)
        {
            MouseManager.OnLeftButtonSingleClick(this, e);
        }

        public void OnLeftButtonDoubleClick(object sender, MouseManagerEventArgs e)
        {
            MouseManager.OnLeftButtonDoubleClick(this, e);
        }

        public void OnLeftButtonHeldClick(object sender, MouseManagerEventArgs e)
        {
            MouseManager.OnLeftButtonHeldClick(this, e);
        }
        #endregion

        #region Right Mouse Button Event Callers
        public void OnRightButtonDragStart(object sender, MouseManagerEventArgs e)
        {
            MouseManager.OnRightButtonDragStart(this, e);
        }

        public void OnRightButtonDragMove(object sender, MouseEventArgs e)
        {
            MouseManager.OnRightButtonDragMove(sender, e);
        }

        public void OnRightButtonDragReleased(object sender, MouseManagerEventArgs e)
        {
            MouseManager.OnRightButtonDragReleased(this, e);
        }

        public void OnRightButtonSingleClick(object sender, MouseManagerEventArgs e)
        {
            MouseManager.OnRightButtonSingleClick(this, e);
        }

        public void OnRightButtonDoubleClick(object sender, MouseManagerEventArgs e)
        {
            MouseManager.OnRightButtonDoubleClick(this, e);
        }

        public void OnRightButtonHeldClick(object sender, MouseManagerEventArgs e)
        {
            MouseManager.OnRightButtonHeldClick(this, e);
        }
        #endregion
    }
}
