using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse
{
    public class MouseButtonManager
    {
        private readonly DispatcherTimer _timerHeldClick;
        private readonly DispatcherTimer _timerDoubleClick;
        private MouseStateManager _buttonState;

        public MouseButtonManager()
        {
            _timerHeldClick = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 1, 250)};
            _timerHeldClick.Tick += OnHeldClick;

            _timerDoubleClick = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 0, 400)};
            _timerDoubleClick.Tick += DoubleClickTick;
        }

        public event MouseEventHandler ButtonDragMove;

        public event EventHandler<MouseManagerEventArgs> ButtonDragStart;
        public event EventHandler<MouseManagerEventArgs> ButtonDragReleased;
        public event EventHandler<MouseManagerEventArgs> ButtonHeldClick;
        public event EventHandler<MouseManagerEventArgs> ButtonSingleClick;
        public event EventHandler<MouseManagerEventArgs> ButtonDoubleClick;

        private MouseStateManager ButtonState
        {
            get
            {
                if (_buttonState == null)
                {
                    _buttonState = new MouseStateManager();
                }

                return _buttonState;
            }
        }

        public bool IsInactive
        {
            get
            {
                return ButtonState.IsInactive;
            }
        }

        public UIElement Control
        {
            get;
            set;
        }

        private void DoubleClickTick(object sender, EventArgs e)
        {
            _timerDoubleClick.Stop();

            if (ButtonState.IsSingleClick)
            {
                OnSingleClick();
            }
            else if (ButtonState.IsDoubleClick)
            {
                OnDoubleClick();
            }
        }

        private void OnSingleClick()
        {
            _timerHeldClick.Stop();

            if (ButtonSingleClick != null)
            {
                ButtonSingleClick(this, ButtonState.BuildEventArgs());
            }

            ButtonState.Reset();
        }

        private void OnDoubleClick()
        {
            _timerHeldClick.Stop();

            if (ButtonDoubleClick != null)
            {
                ButtonDoubleClick(this, ButtonState.BuildEventArgs());
            }

            ButtonState.Reset();
        }

        private void OnHeldClick(object sender, EventArgs e)
        {
            _timerHeldClick.Stop();

            if (ButtonState.IsMouseDown || ButtonState.IsSingleClick)
            {
                if (ButtonHeldClick != null)
                {
                    MouseManagerEventArgs eventArgs = ButtonState.BuildEventArgs();
                    eventArgs.End = eventArgs.Start;

                    ButtonHeldClick(this, eventArgs);
                }
            }

            ButtonState.Reset();
        }

        private bool ShouldIgnoreMovement(MouseEventArgs movedToEventArgs)
        {
            if (ButtonState.Start != null)
            {
                Point startPosition = ButtonState.Start.PositionInformation.GetPosition(Control);
                Point movedToPosition = movedToEventArgs.GetPosition(Control);

                double distanceMoved = startPosition.Distance(movedToPosition);

                if (distanceMoved < 5)
                {
                    return true;
                }
            }

            return false;
        }

        public void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (ButtonState.IsDrag)
            {
                _timerDoubleClick.Stop();
                _timerHeldClick.Stop();

                ButtonState.Reset();
            }

            if (ButtonState.IsInactive)
            {
                _timerHeldClick.Start();
                _timerDoubleClick.Start();

                ButtonState.SetMouseDown();
            }

            ButtonState.Start.PositionInformation = e;
            ButtonState.Start.EventOrigin = sender;
        }

        public void OnMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            ButtonState.End.PositionInformation = e;
            ButtonState.End.EventOrigin = sender;

            if (ButtonState.IsMouseDown)
            {
                ButtonState.SetSingleClick();
            }
            else if (ButtonState.IsSingleClick)
            {
                ButtonState.SetDoubleClick();
            }
            else if (ButtonState.IsDrag)
            {
                _timerDoubleClick.Stop();
                _timerHeldClick.Stop();

                if (ButtonDragReleased != null)
                {
                    ButtonDragReleased(this, ButtonState.BuildEventArgs());
                }

                ButtonState.Reset();
            }
            else if (ButtonState.IsInactive)
            {
                _timerDoubleClick.Stop();
                _timerHeldClick.Stop();

                if (ButtonDragReleased != null)
                {
                    MouseManagerEventArgs eventArgs = ButtonState.BuildEventArgs();
                    eventArgs.Start = eventArgs.End;

                    ButtonDragReleased(this, eventArgs);
                }

                ButtonState.Reset();
            }
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (ButtonState.IsDrag)
            {
                if (ButtonDragMove != null)
                {
                    ButtonDragMove(sender, e);
                }
            }
            else if (!ButtonState.IsInactive && !ButtonState.IsSingleClick)
            {
                if (ShouldIgnoreMovement(e))
                {
                    return;
                }

                ButtonState.SetDrag();

                _timerDoubleClick.Stop();
                _timerHeldClick.Stop();

                if (ButtonDragStart != null)
                {
                    MouseManagerEventArgs eventArgs = ButtonState.BuildEventArgs();
                    eventArgs.End = eventArgs.Start;

                    ButtonDragStart(this, eventArgs);
                }
            }
        }

        public void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (!ButtonState.IsDrag && !ButtonState.IsInactive && !ButtonState.IsSingleClick)
            {
                ButtonState.SetDrag();

                _timerDoubleClick.Stop();
                _timerHeldClick.Stop();

                if (ButtonDragStart != null)
                {
                    MouseManagerEventArgs eventArgs = ButtonState.BuildEventArgs();
                    eventArgs.End = eventArgs.Start;

                    ButtonDragStart(this, eventArgs);
                }
            }
        }
    }
}
