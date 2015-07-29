using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SilverlightMappingToolBasic.UI.Extensions;
using SilverlightMappingToolBasic.UI.Extensions.CookieManagement;
using SilverlightMappingToolBasic.UI.SuperGraph.Controller;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Sidebar.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Sidebar
{
    [TemplateVisualState(Name = "Opened", GroupName = "ToolbarStates")]
    [TemplateVisualState(Name = "Closed", GroupName = "ToolbarStates")]
    public partial class SidebarControl : UserControl, IMarginAnimationControl
    {
        private bool _isOpened;
        private readonly MarginAnimation _marginAnimation;

        public ISidebarOperationHandler Handler
        {
            get; 
            set;
        }

        public bool IsOpened
        {
            get
            {
                return _isOpened;
            }
            set
            {
                if (_isOpened != value)
                {
                    _isOpened = value;
                    ToolBarPanelControl.SetValue(ToolTipService.ToolTipProperty,
                        _isOpened ? "Collapse Sidebar" : "Expand Sidebar");
                    _marginAnimation.Start(_isOpened);
                }
            }
        }

        public SidebarControl()
        {
            InitializeComponent();
            Realign.SubMenu = RealignSubMenu;
            Zoom.SubMenu = ZoomSubMenu;
            MapFilter.SubMenu = MapFilterSubMenu;
            MapFilterSubMenu.OnOpen += OnSubMenuOpen;
            RealignSubMenu.OnOpen += OnSubMenuOpen;
            ZoomSubMenu.OnOpen += OnSubMenuOpen;
            RealignSubMenu.VerticalRealign.MouseLeftButtonUp += VerticalRealignOnMouseLeftButtonUp;
            RealignSubMenu.HorizontalRealign.MouseLeftButtonUp += HorizontalRealignOnMouseLeftButtonUp;
            ZoomSubMenu.ZoomIn.MouseLeftButtonUp += ZoomInOnMouseLeftButtonUp;
            ZoomSubMenu.ZoomOut.MouseLeftButtonUp += ZoomOutOnMouseLeftButtonUp;
            ZoomSubMenu.Default.MouseLeftButtonUp += DefaultOnMouseLeftButtonUp;
            Application.Current.Host.Content.FullScreenChanged += ContentFullScreenChanged;

            App.SidebarModel.PropertyChanged += SidebarModel_PropertyChanged;

            _marginAnimation = new MarginAnimation(new Thickness(-70, 0, 0, 0), new Thickness(0, 0, 0, 0), 0.2, ToolBarPanel);
            IsOpened = true;
        }

        public Color AuthorBackgroundColor
        {
            get { return (Color)GetValue(AuthorBackgroundColorProperty); }
            set { SetValue(AuthorBackgroundColorProperty, value); }
        }

        public Color ReaderBackgroundColor
        {
            get { return (Color)GetValue(ReaderBackgroundColorProperty); }
            set { SetValue(ReaderBackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty ReaderBackgroundColorProperty = DependencyProperty.Register("ReaderBackgroundColor", typeof(Color), typeof(SuperSidebarButton), new PropertyMetadata(Color.FromArgb(255, 88, 174, 209)));
        public static readonly DependencyProperty AuthorBackgroundColorProperty = DependencyProperty.Register("AuthorBackgroundColor", typeof(Color), typeof(SuperSidebarButton), new PropertyMetadata(Color.FromArgb(255, 0, 61, 100)));

        private void SidebarModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UserStyle")
            {
                if (App.UserStyle == UserStyle.Author)
                {
                    ToolBarPanel.Background = new SolidColorBrush(AuthorBackgroundColor);
                    ToolBarPanelControl.Background = new SolidColorBrush(AuthorBackgroundColor);
                    Logo.Background = new SolidColorBrush(AuthorBackgroundColor);
                    LogoImage.Source = new BitmapImage(new Uri("/SilverlightMappingToolBasic;component/Images/SidebarIcon/glymalogo-author.png", UriKind.Relative));
                }
                else if (App.UserStyle == UserStyle.Reader)
                {
                    ToolBarPanel.Background = new SolidColorBrush(ReaderBackgroundColor);
                    ToolBarPanelControl.Background = new SolidColorBrush(ReaderBackgroundColor);
                    Logo.Background = new SolidColorBrush(ReaderBackgroundColor);
                    LogoImage.Source = new BitmapImage(new Uri("/SilverlightMappingToolBasic;component/Images/SidebarIcon/glymalogo.png", UriKind.Relative));
                }
            }
        }

        private void OnSubMenuOpen(object sender, EventArgs e)
        {
            var submenu = sender as ISubMenu;
            if (submenu != null)
            {
                if (submenu != RealignSubMenu)
                {
                    RealignSubMenu.Hide();
                }
                if (submenu != ZoomSubMenu)
                {
                    ZoomSubMenu.Hide();
                }
                if (submenu != MapFilterSubMenu)
                {
                    MapFilterSubMenu.Hide();
                }
            }
        }

        private void DefaultOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Handler.DefaultZoom();
        }

        private void ZoomOutOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Handler.ZoomOut();
        }

        private void ZoomInOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Handler.ZoomIn();
        }

        private void HorizontalRealignOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Handler.HorizontalRealign();
            Realign.SubMenu.Hide();
        }

        private void VerticalRealignOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Handler.VerticalRealign();
            Realign.SubMenu.Hide();
        }

        public void OpenOrCloseToolBox()
        {
            IsOpened = !IsOpened;
        }

        private void ExpandButton_Clicked(object sender, RoutedEventArgs e)
        {
            OpenOrCloseToolBox();
        }

        private void ContentFullScreenChanged(object sender, EventArgs e)
        {
            if (Application.Current.Host.Content.IsFullScreen)
            {
                ExitFullScreen.Visibility = Visibility.Visible;
                FullScreen.Visibility = Visibility.Collapsed;
            }
            else
            {
                ExitFullScreen.Visibility = Visibility.Collapsed;
                FullScreen.Visibility = Visibility.Visible;
            }
        }

        private void Pointer_OnButtonClicked(object sender, RoutedEventArgs e)
        {
            Handler.AuthorMode();
        }

        private void Pan_OnButtonClicked(object sender, RoutedEventArgs e)
        {
            Handler.ReaderMode();
        }

        private void Refresh_OnButtonClicked(object sender, RoutedEventArgs e)
        {
            Handler.Refresh();
        }

        private void FullScreen_OnButtonClicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Host.Content.IsFullScreen = true;
        }

        private void ExitFullScreen_OnButtonClicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Host.Content.IsFullScreen = false;
        }

        private void UserControl_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
