using System.Windows;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu
{
    public partial class SubContextMenu
    {
        public SubContextMenu()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LocationProperty = DependencyProperty.Register(
            "Location", typeof(Thickness),
            typeof(SubContextMenu), new PropertyMetadata(new Thickness(0))
            );

        public Thickness Location
        {
            get { return (Thickness)GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }
    }
}
