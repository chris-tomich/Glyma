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
    public class NodeLocationOrganiser
    {
        public NodeLocationOrganiser()
        {
        }

        public int NumberOfNodes
        {
            get;
            set;
        }

        public double Radius
        {
            get;
            set;
        }

        public double CentreX
        {
            get;
            set;
        }

        public double CentreY
        {
            get;
            set;
        }

        private double OriginX
        {
            get
            {
                return CentreX;
            }
        }

        private double OriginY
        {
            get
            {
                return (CentreY - Radius);
            }
        }

        private double NodeSpacing
        {
            get
            {
                double spacing = Radius * 4; // Needs to be 4 as we are finding the spacing required between ALL the nodes and there are two halves to a circle meaning twice the vertical space.

                spacing = spacing / NumberOfNodes; // Find the total spacing between each node;

                return spacing;
            }
        }

        private double CalculateYOffset(ISingleDepthNodeRenderer nodeRenderer)
        {
            double yOffset = nodeRenderer.NodeIndex * NodeSpacing; // Find the distance from the origin.
            double diameter = Radius * 2;

            if (yOffset > diameter)
            {
                yOffset = (2 * diameter) - yOffset; // If the distance from the origin is great then the diameter, then the circle of nodes is coming back around. This is a simplification of (diameter - (distanceFromOrigin - diameter)).
            }

            return yOffset;
        }

        /// <summary>
        /// Method represents the calculation sqrt(R^2 - (R - y)^2) where R is the radius and y is the 'DistanceFromOrigin'.
        /// </summary>
        /// <param name="nodeRenderer"></param>
        /// <returns></returns>
        private double CalculateXOffset(ISingleDepthNodeRenderer nodeRenderer)
        {
            double distanceFromOrigin = CalculateYOffset(nodeRenderer);

            double ySquared = Radius - distanceFromOrigin; // (R - y)
            ySquared = ySquared * ySquared; // (R - y) ^ 2

            double rSquared = Radius * Radius; // R ^ 2

            double xSquared = rSquared - ySquared; // R^2 - (R - y)^2

            double xOffset = Math.Sqrt(xSquared);

            if (nodeRenderer.NodeIndex > (nodeRenderer.NodeCount / 2))
            {
                xOffset = -1 * xOffset;
            }

            return xOffset;
        }

        public Point GetLocation(ISingleDepthNodeRenderer nodeRenderer)
        {
            double xOffset = CalculateXOffset(nodeRenderer);
            double yOffset = CalculateYOffset(nodeRenderer);

            xOffset -= nodeRenderer.Skin.NodeSkinWidth / 2; // Centre the node image.
            yOffset -= nodeRenderer.Skin.NodeSkinHeight / 2; // Centre the node image.

            Point nodeLocation = new Point();
            nodeLocation.X = OriginX + xOffset;
            nodeLocation.Y = OriginY + yOffset;

            return nodeLocation;
        }
    }
}
