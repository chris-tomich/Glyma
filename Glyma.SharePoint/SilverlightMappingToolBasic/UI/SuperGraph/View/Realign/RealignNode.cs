using System;
using System.Collections.Generic;
using System.Linq;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;
using TransactionalNodeService.Service;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Realign
{
    public class RealignNode : RealignObject
    {
        public RealignNode(NodeControl nodeControl)
        {
            RootDepth = 0;
            StackNumber = 0;
            NodeControl = nodeControl;
            Left = nodeControl.Centre.X;
            Top = nodeControl.Centre.Y;
        }

        public double ImageHeight
        {
            get { return GlymaParameters.NodeImageHeight + GlymaParameters.Margin; }
        }


        public int RootDepth
        {
            get;
            set;
        }

        public int StackNumber
        {
            get;
            set;
        }

        public double Top
        {
            get;
            set;
        }

        public double Left
        {
            get;
            set;
        }

        public NodeControl NodeControl
        {
            get; 
            private set;
        }

        public double TextHeight {
            get
            {
                if (NodeControl.NodeText.ActualHeight < 25)
                {
                    NodeControl.NodeText.UpdateLayout();
                    return NodeControl.NodeText.ActualHeight < 25 ? 25 : NodeControl.NodeText.ActualHeight;
                }
                return NodeControl.NodeText.ActualHeight;
            }
        }

        public Guid Id
        {
            get
            {
                return NodeControl.ViewModelNode.Id;
            }
        }

        public double OriginTop
        {
            get
            {
                return NodeControl.Centre.Y;
            }
        }

        public double OriginLeft
        {
            get
            {
                return NodeControl.Centre.X;
            }
        }

        public double Offset
        {
            get; 
            set;
        }

        public double Height
        {
            get
            {
                return TextHeight + ImageHeight;
            }
        }

        public bool HasChild
        {
            get
            {
                return ChildNodes.Count > 0;
            }
        }

        private int ParentCount()
        {
            return NodeControl.ParentNodes.Count;
        }

        // retrieve a list of nodes that exist in the map.
        public IEnumerable<RealignNode> GetAllNodes(RealignStyle realignStyle = RealignStyle.Horizontal)
        {
            var output = new List<RealignNode> { this };
            foreach (var node in GetOrderedChildNodes(realignStyle))
            {
                if (node.HasChild)
                    output.AddRange(node.GetAllNodes(realignStyle));
                else
                    output.Add(node);
            }
            return output;
        }

        public void AddTopOffset(double offset)
        {
            if (!HasChild) 
            { 
                Top += offset;
                Offset += offset;
            }
            else
            {
                Top += offset;
                Offset += offset;
                foreach (var node in ChildNodes)
                {
                    node.AddTopOffset(offset);
                }
            }
        }

        public void AddLeftOffset(double offset)
        {
            if (!HasChild)
            {
                Left += offset; 
                Offset += offset;
            }
            else
            {
                Left += offset;
                Offset += offset;
                foreach (var node in ChildNodes)
                {
                    node.AddLeftOffset(offset);
                }
            }
        }        

        public bool HasMultiParents()
        {
            return ParentCount() > 1;
        }

        public bool GetAnyChildHasMultipleParents()
        {
            return ChildNodes.Any(q => q.HasMultiParents());
        }

        public double GetCenterTopOfChildNode()
        {
            if (ChildNodes.Count > 0)
                return ChildNodes.Average(q => q.Top);
            return 0;
        }

        public double GetCenterLeftOfChildNode()
        {
            if (ChildNodes.Count > 0)
                return ChildNodes.Average(q => q.Left);
            return 0;
        }
    }
}
