using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse;
using Telerik.Windows.Controls.ColorEditor;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.BreadcrumbSupportClasses
{
    public partial class BreadcrumbBarControl : UserControl
    {
        public IBreadcrumbOperationHandler Handler { get; set; }

        public event EventHandler<BreadcrumbChangedEventArgs> BreadcrumbChanged;
        public event EventHandler<BreadcrumbClickedEventArgs> BreadcrumbClicked;

        private BreadcrumbTrail _breadcrumbTrail;
        public BreadcrumbTrail BreadcrumbTrail
        {
            get
            {
                return _breadcrumbTrail;
            }
            set
            {
                _breadcrumbTrail = value;
                _breadcrumbTrail.BreadcrumbChanged += OnBreadcrumbChanged;
                _breadcrumbTrail.BreadcrumbClicked += OnBreadcrumbClicked;
                _breadcrumbTrail.PropertiesClicked += OnPropertiesClicked;
            }
        }

        
        public BreadcrumbBarControl()
        {
            InitializeComponent();
            BreadcrumbTrail = new BreadcrumbTrail(BreadcrumbContainer, BreadcrumbEllipsis);
        }

        private void OnBreadcrumbChanged(object sender, BreadcrumbChangedEventArgs e)
        {
            if (BreadcrumbChanged != null)
            {
                BreadcrumbChanged(sender, e);
            }
        }


        private void OnPropertiesClicked(object sender, EventArgs e)
        {
            var breadcrumbControl = sender as BreadcrumbControl;

            if (breadcrumbControl != null)
            {
                Handler.NodePropertiesDialog.DataContext = breadcrumbControl.BreadcrumbData.Node.NodeProperties;
                Handler.NodePropertiesDialog.Show();
            }
        }

        private void OnBreadcrumbClicked(object sender, BreadcrumbClickedEventArgs e)
        {
            if (BreadcrumbClicked != null)
            {
                BreadcrumbClicked(sender, e);
            }
        }


        private void Home_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Handler.Home();
        }

        public void ChangeMouseStyle()
        {
            if (CurrentBreadcrumbControl != null)
            {
                CurrentBreadcrumbControl.BreadcrumbData.HasVisibleProperties = App.UserStyle == UserStyle.Author;
            }
        }

        public BreadcrumbControl CurrentBreadcrumbControl
        {
            get
            {
                if (BreadcrumbContainer.Count > 0)
                {
                    var currentBreadcrumb = BreadcrumbContainer.BreadcrumbControls.Last();
                    if (currentBreadcrumb != null)
                    {
                        return currentBreadcrumb;
                    }
                }
                return null;
            }
        }

        private void Home_MouseEnter(object sender, MouseEventArgs e)
        {
            Home.Background = new SolidColorBrush(ColorConverter.ColorFromString("#FF296C99"));
        }

        private void Home_MouseLeave(object sender, MouseEventArgs e)
        {
            Home.Background = new SolidColorBrush(ColorConverter.ColorFromString("#FF003f69"));
        }
    }
}
