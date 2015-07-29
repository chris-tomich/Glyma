using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SilverlightMappingToolBasic.Code.ColorsManagement;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Sidebar.Interface;
using TransactionalNodeService.Service;
using System.Windows.Media.Imaging;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Sidebar
{
    public partial class SuperSidebarButton : UserControl, ISidebarButton
    {
        public event RoutedEventHandler ButtonClicked;

        public SuperSidebarButton()
        {
            InitializeComponent();
            DataContext = this;

            this.Loaded += SuperSidebarButton_Loaded;
            App.SidebarModel.PropertyChanged += SidebarModel_PropertyChanged;
        }

        public bool IsClicked { get; set; }

        public ISubMenu SubMenu { get; set;}

        public bool HasSubMenu {
            get
            {
                return SubMenu != null;
            }
        }

        private bool IsMouseOver
        {
            get;
            set;
        }

        public string ReaderImageSource
        {
            get { return GetValue(ReaderImageSourceProperty).ToString(); }
            set { SetValue(ReaderImageSourceProperty, value); }
        }

        public string AuthorImageSource
        {
            get { return GetValue(AuthorImageSourceProperty).ToString(); }
            set { SetValue(AuthorImageSourceProperty, value); }
        }

        public string ReaderHoverImageSource
        {
            get { return GetValue(ReaderHoverImageSourceProperty).ToString(); }
            set { SetValue(ReaderHoverImageSourceProperty, value); }
        }

        public string AuthorHoverImageSource
        {
            get { return GetValue(AuthorHoverImageSourceProperty).ToString(); }
            set { SetValue(AuthorHoverImageSourceProperty, value); }
        }

        public Color AuthorHoverBackgroundColor
        {
            get { return (Color)GetValue(AuthorHoverBackgroundColorProperty); }
            set { SetValue(AuthorHoverBackgroundColorProperty, value); }
        }

        public Color ReaderHoverBackgroundColor
        {
            get { return (Color)GetValue(ReaderHoverBackgroundColorProperty); }
            set { SetValue(ReaderHoverBackgroundColorProperty, value); }
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

        public static readonly DependencyProperty ReaderImageSourceProperty = DependencyProperty.Register("ReaderImageSource", typeof(string), typeof(SuperSidebarButton), new PropertyMetadata(""));
        public static readonly DependencyProperty AuthorImageSourceProperty = DependencyProperty.Register("AuthorImageSource", typeof(string), typeof(SuperSidebarButton), new PropertyMetadata(""));
        public static readonly DependencyProperty ReaderHoverImageSourceProperty = DependencyProperty.Register("ReaderHoverImageSource", typeof(string), typeof(SuperSidebarButton), new PropertyMetadata(""));
        public static readonly DependencyProperty AuthorHoverImageSourceProperty = DependencyProperty.Register("AuthorHoverImageSource", typeof(string), typeof(SuperSidebarButton), new PropertyMetadata(""));
        public static readonly DependencyProperty ReaderHoverBackgroundColorProperty = DependencyProperty.Register("ReaderHoverBackgroundColor", typeof(Color), typeof(SuperSidebarButton), new PropertyMetadata(Color.FromArgb(255, 199, 224, 234)));
        public static readonly DependencyProperty AuthorHoverBackgroundColorProperty = DependencyProperty.Register("AuthorHoverBackgroundColor", typeof(Color), typeof(SuperSidebarButton), new PropertyMetadata(Color.FromArgb(255, 1, 126, 186)));
        public static readonly DependencyProperty ReaderBackgroundColorProperty = DependencyProperty.Register("ReaderBackgroundColor", typeof(Color), typeof(SuperSidebarButton), new PropertyMetadata(Color.FromArgb(255, 88, 174, 209)));
        public static readonly DependencyProperty AuthorBackgroundColorProperty = DependencyProperty.Register("AuthorBackgroundColor", typeof(Color), typeof(SuperSidebarButton), new PropertyMetadata(Color.FromArgb(255, 0, 61, 100)));

        private void SuperSidebarButton_Loaded(object sender, RoutedEventArgs e)
        {
            IsMouseOver = false;
            if (App.UserStyle == Mouse.UserStyle.Reader)
            {
                ButtonBackground.Background = new SolidColorBrush(ReaderBackgroundColor);
                ButtonHoverImage.Source = new BitmapImage(new Uri(ReaderHoverImageSource, UriKind.Relative));
                ButtonImage.Source = new BitmapImage(new Uri(ReaderImageSource, UriKind.Relative));
            }
            else if (App.UserStyle == Mouse.UserStyle.Author)
            {
                ButtonBackground.Background = new SolidColorBrush(AuthorBackgroundColor);
                ButtonHoverImage.Source = new BitmapImage(new Uri(AuthorHoverImageSource, UriKind.Relative));
                ButtonImage.Source = new BitmapImage(new Uri(AuthorImageSource, UriKind.Relative));
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            IsMouseOver = true;
            if (App.UserStyle == Mouse.UserStyle.Reader)
            {
                ButtonBackground.Background = new SolidColorBrush(ReaderHoverBackgroundColor);
                ButtonHoverImage.Source = new BitmapImage(new Uri(ReaderHoverImageSource, UriKind.Relative));
            }
            else if (App.UserStyle == Mouse.UserStyle.Author)
            {
                ButtonBackground.Background = new SolidColorBrush(AuthorHoverBackgroundColor);
                ButtonHoverImage.Source = new BitmapImage(new Uri(AuthorHoverImageSource, UriKind.Relative));
            }
            ButtonHoverImage.Visibility = Visibility.Visible;
            ButtonImage.Visibility = Visibility.Collapsed;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            IsMouseOver = false;
            if (App.UserStyle == Mouse.UserStyle.Reader)
            {
                ButtonBackground.Background = new SolidColorBrush(ReaderBackgroundColor);
                ButtonImage.Source = new BitmapImage(new Uri(ReaderImageSource, UriKind.Relative));
            }
            else if (App.UserStyle == Mouse.UserStyle.Author)
            {
                ButtonBackground.Background = new SolidColorBrush(AuthorBackgroundColor);
                ButtonImage.Source = new BitmapImage(new Uri(AuthorImageSource, UriKind.Relative));
            }
            ButtonHoverImage.Visibility = Visibility.Collapsed;
            ButtonImage.Visibility = Visibility.Visible;
        }

        private void Button_Clicked(object sender, RoutedEventArgs e)
        {
            if (HasSubMenu)
            {
                SubMenu.Show();
            }

            if (ButtonClicked != null)
            {
                ButtonClicked(this, e);
            }
        }

        private void ExpandButton_LostFocus(object sender, RoutedEventArgs e)
        {
            IsMouseOver = false;
            if (App.UserStyle == Mouse.UserStyle.Reader)
            {
                ButtonBackground.Background = new SolidColorBrush(ReaderBackgroundColor);
                ButtonImage.Source = new BitmapImage(new Uri(ReaderImageSource, UriKind.Relative));
            }
            else if (App.UserStyle == Mouse.UserStyle.Author)
            {
                ButtonBackground.Background = new SolidColorBrush(AuthorBackgroundColor);
                ButtonImage.Source = new BitmapImage(new Uri(AuthorImageSource, UriKind.Relative));
            }
            ButtonHoverImage.Visibility = Visibility.Collapsed;
            ButtonImage.Visibility = Visibility.Visible;
        }

        private void SidebarModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UserStyle")
            {
                if (IsMouseOver)
                {
                    if (App.UserStyle == Mouse.UserStyle.Reader)
                    {
                        ButtonBackground.Background = new SolidColorBrush(ReaderHoverBackgroundColor);
                        ButtonHoverImage.Source = new BitmapImage(new Uri(ReaderHoverImageSource, UriKind.Relative));
                        ButtonImage.Source = new BitmapImage(new Uri(ReaderImageSource, UriKind.Relative));
                    }
                    else if (App.UserStyle == Mouse.UserStyle.Author)
                    {
                        ButtonBackground.Background = new SolidColorBrush(AuthorHoverBackgroundColor);
                        ButtonHoverImage.Source = new BitmapImage(new Uri(AuthorHoverImageSource, UriKind.Relative));
                        ButtonImage.Source = new BitmapImage(new Uri(AuthorImageSource, UriKind.Relative));
                    }
                }
                else
                {
                    if (App.UserStyle == Mouse.UserStyle.Reader)
                    {
                        ButtonBackground.Background = new SolidColorBrush(ReaderBackgroundColor);
                        ButtonHoverImage.Source = new BitmapImage(new Uri(ReaderHoverImageSource, UriKind.Relative));
                        ButtonImage.Source = new BitmapImage(new Uri(ReaderImageSource, UriKind.Relative));
                    }
                    else if (App.UserStyle == Mouse.UserStyle.Author)
                    {
                        ButtonBackground.Background = new SolidColorBrush(AuthorBackgroundColor);
                        ButtonHoverImage.Source = new BitmapImage(new Uri(AuthorHoverImageSource, UriKind.Relative));
                        ButtonImage.Source = new BitmapImage(new Uri(AuthorImageSource, UriKind.Relative));
                    }
                }
            }
        }
    }
}
