using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Interface
{
    public interface IZoomControl
    {
        event EventHandler<RoutedPropertyChangedEventArgs<double>> ZoomChanged;

        double Zoom { get; }

        void ZoomIn();
        void ZoomOut();
        void DefaultZoom();

        void ResetViewport();

        double ActualWidth { get; }
        double ActualHeight { get; }
        void MoveMap(double x, double y);
        TranslateTransform MoveGraphTransform { get; }
    }
}
