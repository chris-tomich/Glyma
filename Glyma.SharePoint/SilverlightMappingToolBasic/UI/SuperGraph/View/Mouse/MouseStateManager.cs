namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse
{
    public class MouseStateManager
    {
        private enum MouseState
        {
            Inactive,
            MouseDown,
            SingleClick,
            DoubleClick,
            Drag
        }

        private MouseState _state;

        public MouseStateManager()
        {
            Reset();
        }

        public MouseStateProperties Start
        {
            get;
            private set;
        }

        public MouseStateProperties End
        {
            get;
            private set;
        }

        public bool IsInactive
        {
            get
            {
                return _state == MouseState.Inactive;
            }
        }

        public bool IsMouseDown
        {
            get
            {
                return _state == MouseState.MouseDown;
            }
        }

        public bool IsSingleClick
        {
            get
            {
                return _state == MouseState.SingleClick;
            }
        }

        public bool IsDoubleClick
        {
            get
            {
                return _state == MouseState.DoubleClick;
            }
        }

        public bool IsDrag
        {
            get
            {
                return _state == MouseState.Drag;
            }
        }

        public void SetInactive()
        {
            _state = MouseState.Inactive;
        }

        public void SetMouseDown()
        {
            _state = MouseState.MouseDown;
        }

        public void SetSingleClick()
        {
            _state = MouseState.SingleClick;
        }

        public void SetDoubleClick()
        {
            _state = MouseState.DoubleClick;
        }

        public void SetDrag()
        {
            _state = MouseState.Drag;
        }

        public MouseManagerEventArgs BuildEventArgs()
        {
            return new MouseManagerEventArgs { Start = Start, End = End };
        }

        public void Reset()
        {
            _state = MouseState.Inactive;
            Start = new MouseStateProperties();
            End = new MouseStateProperties();
        }
    }
}
