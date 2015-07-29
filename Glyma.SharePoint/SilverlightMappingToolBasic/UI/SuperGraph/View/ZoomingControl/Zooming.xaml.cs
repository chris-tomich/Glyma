using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Interface;
using Telerik.Windows.Controls;
using TransactionalNodeService.Service;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ZoomingControl
{
    public partial class Zooming : UserControl
    {
        private IZoomControl _handler;
        public IZoomControl Handler {
            get { return _handler; }
            set
            {
                _handler = value;
                _handler.ZoomChanged += HandlerOnZoomChanged;
            }
        }

        private void HandlerOnZoomChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var count = 0;
            if (e.NewValue > 1)
            {
                var value = e.NewValue;
                while (value > 1)
                {
                    value *= 0.9;
                    count ++;
                }
            }
            else
            {
                var value = e.NewValue;
                while (value < 1)
                {
                    value /= 0.9;
                    count--;
                }
            }
            SetValue(count);
        }

        public void SetValue(int value)
        {
            ZoomSlider.ValueChanged -= ZoomingChanged;

            ZoomSlider.Value = value;

            if (ZoomSlider.Value.Equals(ZoomSlider.Maximum))
            {
                ZoomIn.IsEnabled = false;
                ZoomOut.IsEnabled = true;
            }
            else if (ZoomSlider.Value.Equals(ZoomSlider.Minimum))
            {
                ZoomIn.IsEnabled = true;
                ZoomOut.IsEnabled = false;
            }
            else
            {
                ZoomIn.IsEnabled = true;
                ZoomOut.IsEnabled = true;
            }

            ZoomSlider.ValueChanged += ZoomingChanged;
        }

        public Zooming()
        {
            InitializeComponent();
        }

        private void ZoomingChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Handler != null)
            {
                var oldlocation = new Point(Handler.ActualWidth / 2 / Handler.Zoom, Handler.ActualHeight / 2 / Handler.Zoom);
                var newLocation = oldlocation;

                if (!e.NewValue.Equals(e.OldValue))
                {
                    if (e.NewValue > e.OldValue)
                    {
                        for (var i = e.NewValue; i > e.OldValue; i--)
                        {
                            Handler.ZoomIn();
                            newLocation.Y = newLocation.Y * 0.9;
                            newLocation.X = newLocation.X * 0.9;
                        }
                    }
                    else if (e.NewValue < e.OldValue)
                    {
                        for (var i = e.NewValue; i < e.OldValue; i++)
                        {
                            Handler.ZoomOut();
                            newLocation.Y = newLocation.Y / 0.9;
                            newLocation.X = newLocation.X / 0.9;
                        }
                    }
                    var y = newLocation.Y - oldlocation.Y;
                    var x = newLocation.X - oldlocation.X;

                    Handler.MoveMap(x, y);
                }
                
            }        
        }

        private void ZoomIn_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ZoomSlider.Value < ZoomSlider.Maximum)
            {
                var oldlocation = new Point(Handler.ActualWidth / 2 / Handler.Zoom, Handler.ActualHeight / 2 / Handler.Zoom);
                var newLocation = oldlocation;

                Handler.ZoomIn();
                newLocation.Y = newLocation.Y * 0.9;
                newLocation.X = newLocation.X * 0.9;

                var y = newLocation.Y - oldlocation.Y;
                var x = newLocation.X - oldlocation.X;

                Handler.MoveMap(x, y);
            }
        }

        private void ZoomOut_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ZoomSlider.Value > ZoomSlider.Minimum)
            {
                var oldlocation = new Point(Handler.ActualWidth / 2 / Handler.Zoom, Handler.ActualHeight / 2 / Handler.Zoom);
                var newLocation = oldlocation;

                Handler.ZoomOut();
                newLocation.Y = newLocation.Y / 0.9;
                newLocation.X = newLocation.X / 0.9;

                var y = newLocation.Y - oldlocation.Y;
                var x = newLocation.X - oldlocation.X;

                Handler.MoveMap(x, y);
            }
            
        }

        private void Reset_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Handler.ResetViewport();
        }

        private void DefaultZoom_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var oldlocation = new Point(Handler.ActualWidth / 2 / Handler.Zoom, Handler.ActualHeight / 2 / Handler.Zoom);
            var newLocation = oldlocation;
            var old = Handler.Zoom;
            Handler.DefaultZoom();
            newLocation.Y = newLocation.Y * old;
            newLocation.X = newLocation.X * old;

            var y = newLocation.Y - oldlocation.Y;
            var x = newLocation.X - oldlocation.X;

            Handler.MoveMap(x, y);
        }
    }
}
