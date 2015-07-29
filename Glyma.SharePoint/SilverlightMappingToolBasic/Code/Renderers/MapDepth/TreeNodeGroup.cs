using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightMappingToolBasic.MapDepth
{
    public class TreeNodeGroup
    {
        public List<NodeRenderer> ChildNodes
        {
            get;
            set;
        }

        public List<TreeNodeGroup> ParentChildGroups
        {
            get;
            set;
        }

        public NodeRenderer ParentNode
        {
            get;
            set;
        }

        public double GetYPosition() 
        {
            double position = 0;
            if (ChildNodes.Count > 1)
            {
                double nodeGroupHeight = GetHeight();
                position = (0.5 * nodeGroupHeight) + ChildNodes[0].Location.Y;
            }
            else if (ChildNodes.Count == 1)
            {
                position = ChildNodes[0].Location.Y;
            }
            return position;
        }

        private double GetHeight()
        {
            double result = 0;
            if (ChildNodes != null && ChildNodes.Count > 1)
            {
                result = ChildNodes[ChildNodes.Count - 1].Location.Y - ChildNodes[0].Location.Y;
            }
            return result;
        }
    }
}
