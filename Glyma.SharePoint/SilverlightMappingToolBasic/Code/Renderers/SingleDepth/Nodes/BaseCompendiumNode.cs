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
using System.Collections.Generic;

namespace SilverlightMappingToolBasic.Compendium
{
    public abstract class BaseCompendiumNode : ICompendiumNode
    {
        public const double DefaultNodeFontSize = 10;

        public BaseCompendiumNode()
        {
        }

        protected abstract Rect TextLocation { get; }

        protected abstract Rect NodeUIElementLocation { get; }

        protected abstract Brush NodeImage { get; }

        protected virtual double NodeFontSize
        {
            get
            {
                return DefaultNodeFontSize;
            }
        }

        protected virtual Geometry NodeClippingGeometry
        {
            get;
            private set;
        }

        protected virtual TextBlock NodeLabelUIElement
        {
            get;
            private set;
        }

        protected virtual FrameworkElement NodeUIElement
        {
            get;
            private set;
        }

        protected virtual CompendiumNodeSettings NodeSettings
        {
            get;
            private set;
        }

        private double CalculateYOffset(CompendiumNodeSettings settings)
        {
            double distanceMultiplier;
            double distanceBetweenNodes;

            if (settings.NodeIndex < (settings.NodeCount / 2))
            {
                distanceMultiplier = settings.NodeIndex;
                distanceBetweenNodes = (4 * settings.Radius) / (settings.NodeCount - 1);
            }
            else
            {
                distanceMultiplier = settings.NodeIndex - (settings.NodeCount / 2);
                distanceBetweenNodes = (4 * settings.Radius) / (settings.NodeCount + 1);
            }

            double heightOffset = distanceMultiplier * distanceBetweenNodes;
            double heightRemainder;

            if (heightOffset < settings.Radius)
            {
                heightRemainder = settings.Radius - heightOffset;
            }
            else
            {
                heightRemainder = heightOffset - settings.Radius;
            }

            return heightRemainder;
        }

        private double CalculateXOffset(CompendiumNodeSettings settings)
        {
            double distanceMultiplier;
            double distanceBetweenNodes;

            if (settings.NodeIndex < (settings.NodeCount / 2))
            {
                distanceMultiplier = settings.NodeIndex;
                distanceBetweenNodes = (4 * settings.Radius) / (settings.NodeCount - 1);
            }
            else
            {
                distanceMultiplier = settings.NodeIndex - (settings.NodeCount / 2);
                distanceBetweenNodes = (4 * settings.Radius) / (settings.NodeCount + 1);
            }

            double heightOffset = distanceMultiplier * distanceBetweenNodes;
            double heightRemainder;

            if (heightOffset < settings.Radius)
            {
                heightRemainder = settings.Radius - heightOffset;
            }
            else
            {
                heightRemainder = heightOffset - settings.Radius;
            }

            double xOffset = Math.Sqrt((settings.Radius * settings.Radius) - (heightRemainder * heightRemainder));

            return xOffset;
        }

        protected Point CalculateLocation()
        {
            Point location = new Point();

            if (NodeSettings != null)
            {
                double xOffset = CalculateXOffset(NodeSettings);
                double yOffset = CalculateYOffset(NodeSettings);

                if (NodeSettings.NodeIndex <= (NodeSettings.NodeCount / 4))
                {
                    location.X = NodeSettings.CentreX + xOffset;
                    location.Y = NodeSettings.CentreY - yOffset;
                }
                else if (NodeSettings.NodeIndex <= (NodeSettings.NodeCount / 2))
                {
                    location.X = NodeSettings.CentreX + xOffset;
                    location.Y = NodeSettings.CentreY + yOffset;
                }
                else if (NodeSettings.NodeIndex <= ((NodeSettings.NodeCount * 3) / 4))
                {
                    location.X = NodeSettings.CentreX - xOffset;
                    location.Y = NodeSettings.CentreY + yOffset;
                }
                else
                {
                    location.X = NodeSettings.CentreX - xOffset;
                    location.Y = NodeSettings.CentreY - yOffset;
                }
            }

            return location;
        }

        public double X
        {
            get
            {
                return NodeUIElementLocation.X;
            }
            set
            {
            }
        }

        public double Y
        {
            get
            {
                return NodeUIElementLocation.Y;
            }
            set
            {
            }
        }

        #region ICompendiumNode Members

        public virtual void LoadSettings(CompendiumNodeSettings settings)
        {
            NodeSettings = settings;

            NodeLabelUIElement = new TextBlock();
            NodeLabelUIElement.Text = NodeSettings.NodeDetails.Name;
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
            NodeUIElement = nodeUIElement;
        }

        public virtual UIElement[] RenderUIElements()
        {
            NodeLabelUIElement.Clip = NodeClippingGeometry;

            List<UIElement> uiElements = new List<UIElement>();
            uiElements.Add(NodeUIElement);
            uiElements.Add(NodeLabelUIElement);

            return uiElements.ToArray();
        }

        #endregion
    }
}
