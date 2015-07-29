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
using System.Diagnostics;

using SilverlightMappingToolBasic.Controls;
using SilverlightMappingToolBasic;

namespace SilverlightMappingToolBasic.SingleDepth
{
    public class SingleDepthNodeRenderer : NodeRenderer, ISingleDepthNodeRenderer
    {
        protected SingleDepthNodeRenderer() : base()
        {
          
        }

        public SingleDepthNodeRenderer(NavigatorView parentNavigatorView, INodeProxy nodeProxy, ThemeManager themeManager, string skinName)
            : base(parentNavigatorView, nodeProxy, themeManager, skinName)
        {

        }

        #region Rendering Helper Methods

        private double GetRadius(RenderingContextInfo info)
        {
            double radius = 0;

            if (info.SurfaceWidth <= info.SurfaceHeight)
            {
                radius = info.SurfaceWidth / 2;
            }
            else
            {
                radius = info.SurfaceHeight / 2;
            }

            if (Skin.NodeSkinWidth >= Skin.NodeSkinHeight)
            {
                radius = radius - (Skin.NodeSkinWidth / 2);
            }
            else
            {
                radius = radius - (Skin.NodeSkinHeight / 2);
            }

            return radius;
        }

        private double GetSurfaceCentreX(RenderingContextInfo info)
        {
            return (info.SurfaceWidth / 2);
        }

        private double GetSurfaceCentreY(RenderingContextInfo info)
        {
            return (info.SurfaceHeight / 2);
        }
        #endregion

        public override NodeControl RenderChildren(RenderingContextInfo info)
        {
            if (_nodeControl == null)
            {
                double centreX = GetSurfaceCentreX(info);
                double centreY = GetSurfaceCentreY(info);

                if (SkinName != "Focal")
                {
                    NodeLocationOrganiser locationOrganiser = new NodeLocationOrganiser();
                    locationOrganiser.CentreX = centreX;
                    locationOrganiser.CentreY = centreY;
                    locationOrganiser.NumberOfNodes = NodeCount;
                    locationOrganiser.Radius = GetRadius(info);

                    Location = locationOrganiser.GetLocation(this);
                }
                else
                {
                    double focalXLocation = centreX - (Skin.NodeSkinWidth / 2); // Centre the focal node image along the x-axis.
                    double focalYLocation = centreY - (Skin.NodeSkinHeight / 2); // Centre the focal node image along the y-axis.

                    Location = new Point(focalXLocation, focalYLocation);
                }

                _nodeControl = Skin.RenderSkinElements(Node, SkinName, SkinProperties);
            }

            return _nodeControl;
        }

        #region ISingleDepthNodeRenderer Members

        public int NodeIndex
        {
            get;
            set;
        }

        public int NodeCount
        {
            get;
            set;
        }

        #endregion
    }
}
