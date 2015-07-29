using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.BreadcrumbSupportClasses
{
    public partial class BreadcrumbEllipsis : UserControl
    {
        private bool _isMouseOver;
        private readonly DispatcherTimer _timer;

        public IEnumerable<BreadcrumbControl> BreadcrumbControls
        {
            get
            {
                return BreadcrumbContainer.BreadcrumbControls;
            }
        }

        public BreadcrumbEllipsis()
        {
            InitializeComponent();
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.5) };
            _timer.Tick += DelayedMouseLeave;
        }

        private void DelayedMouseLeave(object sender, EventArgs eventArgs)
        {
            if (!_isMouseOver)
            {
                Ellipsis.Visibility = Visibility.Visible;
                BreadcrumbContainer.Visibility = Visibility.Collapsed;
            }
            _timer.Stop();
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseOver = false;
            _timer.Start();
        }

        private void OnEllipsisMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseOver = true;
            Ellipsis.Visibility = Visibility.Collapsed;
            BreadcrumbContainer.Visibility = Visibility.Visible;
        }

        public void Clear()
        {
            BreadcrumbContainer.Clear();
            BreadcrumbContainer.Visibility = Visibility.Collapsed;
            Ellipsis.Visibility = Visibility.Visible;
        }
    }
}
