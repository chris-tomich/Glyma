using System.Windows;
using System.Windows.Controls;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Sidebar
{
    [TemplateVisualState(Name = "Over", GroupName = "ButtonStates")]
    [TemplateVisualState(Name = "Clicked", GroupName = "ButtonStates")]
    [TemplateVisualState(Name = "Out", GroupName = "ButtonStates")]
    public class SidebarButtonBase : UserControl, ISidebarButton
    {
        public SidebarButtonBase()
        {
            
            DataContext = this;
            
        }

        public bool IsClicked { set; get; }

        

        public string Text
        {
            get { return GetValue(TextProperty).ToString(); }
            set { SetValue(TextProperty,value); }
        }

        public string Data
        {
            get { return GetValue(DataProperty).ToString(); }
            set { SetValue(DataProperty, value); }
        }

        public string BackgroundColor
        {
            get { return GetValue(BackgroundColorProperty).ToString(); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public string Color
        {
            get { return GetValue(ColorProperty).ToString(); }
            set { SetValue(ColorProperty, value); }
        }

        public string HoverColor
        {
            get { return GetValue(HoverColorProperty).ToString(); }
            set { SetValue(HoverColorProperty, value); }
        }

        public string ClickColor
        {
            get { return GetValue(ClickColorProperty).ToString(); }
            set { SetValue(ClickColorProperty, value); }
        }

        public string HoverBackgroundColor
        {
            get { return GetValue(HoverColorProperty).ToString(); }
            set { SetValue(HoverColorProperty, value); }
        }

        public string ClickBackgroundColor
        {
            get { return GetValue(ClickColorProperty).ToString(); }
            set { SetValue(ClickColorProperty, value); }
        }



        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SidebarButtonBase), new PropertyMetadata(""));
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(string), typeof(SidebarButtonBase), new PropertyMetadata(""));
        public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register("BackgroundColor", typeof(string), typeof(SidebarButtonBase), new PropertyMetadata(""));
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(string), typeof(SidebarButtonBase), new PropertyMetadata(""));
        public static readonly DependencyProperty HoverColorProperty = DependencyProperty.Register("HoverColor", typeof(string), typeof(SidebarButtonBase), new PropertyMetadata(""));
        public static readonly DependencyProperty ClickColorProperty = DependencyProperty.Register("ClickColor", typeof(string), typeof(SidebarButtonBase), new PropertyMetadata(""));
        public static readonly DependencyProperty HoverBackgroundColorProperty = DependencyProperty.Register("HoverBackgroundColor", typeof(string), typeof(SidebarButtonBase), new PropertyMetadata(""));
        public static readonly DependencyProperty ClickBackgroundColorProperty = DependencyProperty.Register("ClickBackgroundColor", typeof(string), typeof(SidebarButtonBase), new PropertyMetadata(""));

        public void SetToUnClicked()
        {
            VisualStateManager.GoToState(this, "Out", true);
        }

        public void SetToClicked()
        {
            VisualStateManager.GoToState(this, "Clicked", true);
        }

        
    }
}
