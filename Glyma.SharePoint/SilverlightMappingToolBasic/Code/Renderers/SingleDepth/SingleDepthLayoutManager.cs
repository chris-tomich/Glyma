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

namespace SilverlightMappingToolBasic.SingleDepth
{
    public class SingleDepthLayoutManager
    {
        private CompendiumNodeSettings _nodeSettings;

        public SingleDepthLayoutManager()
        {
        }

        public void LoadSettings(CompendiumNodeSettings settings)
        {
            _nodeSettings = settings;
        }

        public void AddNode(ICompendiumNode node)
        {
            /*NodeLabelUIElement = new TextBlock();
            NodeLabelUIElement.Text = _nodeSettings.NodeDetails.Name;
            NodeLabelUIElement.FontSize = NodeFontSize;
            NodeLabelUIElement.TextWrapping = TextWrapping.Wrap;
            NodeLabelUIElement.SetValue(Canvas.LeftProperty, TextLocation.X);
            NodeLabelUIElement.SetValue(Canvas.TopProperty, TextLocation.Y);
            NodeLabelUIElement.Width = TextLocation.Width;
            NodeLabelUIElement.Height = TextLocation.Height;

            RectangleGeometry nodeClippingRect = new RectangleGeometry();
            nodeClippingRect.Rect = new Rect(0, 0, TextLocation.Width, TextLocation.Height);
            NodeClippingGeometry = nodeClippingRect;

            Rectangle nodeUIElement = new Rectangle();
            nodeUIElement.Tag = this;
            nodeUIElement.Fill = NodeImage;
            nodeUIElement.SetValue(Canvas.LeftProperty, NodeUIElementLocation.X);
            nodeUIElement.SetValue(Canvas.TopProperty, NodeUIElementLocation.Y);
            nodeUIElement.Width = NodeUIElementLocation.Width;
            nodeUIElement.Height = NodeUIElementLocation.Height;
            NodeUIElement = nodeUIElement;*/
        }
    }
}
