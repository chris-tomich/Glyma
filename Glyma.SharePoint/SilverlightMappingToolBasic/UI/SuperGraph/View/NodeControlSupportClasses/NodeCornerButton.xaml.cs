using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SilverlightMappingToolBasic.Code.ColorsManagement;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses
{
    public enum NodeCornerButtonType
    {
        Play,
        Pause,
        Feed,
        Map,
        Content,
    }

    public class NodeCornerButtonClickedEventArgs : EventArgs
    {
        public NodeCornerButtonType Type { get; set; }
    }

    public partial class NodeCornerButton : Button, INodeCornerButton
    {
        public event EventHandler<NodeCornerButtonClickedEventArgs> ButtonClicked;
        private readonly static DependencyProperty IconColourProperty = DependencyProperty.Register("IconColour", typeof(SolidColorBrush), typeof(NodeCornerButton), new PropertyMetadata(new SolidColorBrush(ColorConverter.FromHex("#FFa7a7a7"))));
        private readonly static DependencyProperty IconHoverColourProperty = DependencyProperty.Register("IconHoverColour", typeof(SolidColorBrush), typeof(NodeCornerButton), new PropertyMetadata(new SolidColorBrush(ColorConverter.FromHex("#FFe64e65"))));
        
        public Brush IconHoverColour
        {
            get { return GetValue(IconHoverColourProperty) as SolidColorBrush; }
            set { SetValue(IconHoverColourProperty, value); }
        }

        public Brush IconColour
        {
            get { return GetValue(IconColourProperty) as SolidColorBrush; }
            set { SetValue(IconColourProperty, value); }
        }



        public NodeCornerButton()
        {
            InitializeComponent();
        }

        private void NodeCornerButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ButtonClicked != null)
            {
                ButtonClicked(this, new NodeCornerButtonClickedEventArgs{Type = ((NodeCornerButtonViewModel)DataContext).ButtonType});
            }
        }
    }
}
