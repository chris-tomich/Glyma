using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightMappingToolBasic.Controls
{
    public class BreadcrumbBar : ListBox
    {
        #region Dependency Properties - NodeNavigator

        public static readonly DependencyProperty MapControlProperty =
            DependencyProperty.Register("MapControl", typeof(IMapControl), typeof(BreadcrumbBar),
                                         new PropertyMetadata(OnMapControlChanged));

        public IMapControl MapControl
        {
            get { return (IMapControl)this.GetValue(BreadcrumbBar.MapControlProperty); }
            set { this.SetValue(BreadcrumbBar.MapControlProperty, value); }
        }

        private static void OnMapControlChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            BreadcrumbBar breadcrumbControl = o as BreadcrumbBar;
            breadcrumbControl.OnMapControlChanged(e.OldValue as IMapControl, e.NewValue as IMapControl);
        }

        #endregion

        public BreadcrumbBar()
        {
            // Set default style key for generic template to be applied.
            this.DefaultStyleKey = typeof(BreadcrumbBar);

            // Navigate on selection changed.
            this.MouseLeftButtonUp += new MouseButtonEventHandler(BreadcrumbBar_MouseLeftButtonUp);
        }

        void BreadcrumbBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NodeHistoryElement nodeHistoryElement = (NodeHistoryElement)this.SelectedItem;
            if (nodeHistoryElement != null && nodeHistoryElement.Node.Id != MapControl.Navigator.FocalNodeId)
            {
                MapControl.Navigator.GoBackTo(nodeHistoryElement.Node.Id);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Ensure single selection mode.
            this.SelectionMode = SelectionMode.Single;
        }

        private void OnMapControlChanged(IMapControl oldMapControl, IMapControl newMapControl)
        {
            this.Items.Clear();

            // Monitor the map control so we can update the selected item.
            if (oldMapControl != null)
            {
                oldMapControl.NavigatorInitialised -= new EventHandler(MapControl_NavigatorInitialised);
            }
            if (newMapControl != null)
            {
                newMapControl.NavigatorInitialised += new EventHandler(MapControl_NavigatorInitialised);
            }
        }

        void MapControl_NavigatorInitialised(object sender, EventArgs e)
        {
            this.Items.Clear();

            if (MapControl.Navigator != null)
            {
                MapControl.Navigator.NavigationCompleted += new EventHandler(OnMapLevelChanged);
                foreach (NodeHistoryElement ele in MapControl.Navigator.History.Stack.Reverse())
                {
                    this.Items.Add(ele);
                }
            }
        }

        private void OnMapLevelChanged( object sender, EventArgs e)
        {
            this.Items.Clear();
            foreach (NodeHistoryElement ele in MapControl.Navigator.History.Stack.Reverse())
            {
                this.Items.Add(ele);
            }
        }
    }
}
