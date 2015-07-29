using System.Collections.Generic;
using System.Linq;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Realign.Interface;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Realign
{
    /// <summary>
    /// Any problems with this Class must fix by Sam Zhou
    /// </summary>
    public class VerticalRealignFactory : RealignFactory
    {
        private const double Width = GlymaParameters.NodeTextWidth;
        private const double LeftMargin = GlymaParameters.Margin;
        private Dictionary<int, double> _textHeights;
        private Dictionary<int, double> _imageHeights;
        private Dictionary<int, List<RealignNode>> _nodesOnDepth;

        public VerticalRealignFactory(IRealignControl mainControl, bool isPartialRealign = false)
            : base(mainControl, RealignStyle.Vertical, isPartialRealign)
        {
            mainControl.MapInformation.AutoRealignStyle = RealignStyle.Vertical;
        }

        private Dictionary<int, double> TextHeights
        {
            get
            {
                return _textHeights ?? (_textHeights = new Dictionary<int, double>());
            }
        }

        private Dictionary<int, double> ImageHeights
        {
            get
            {
                return _imageHeights ?? (_imageHeights = new Dictionary<int, double>());
            }
        }

        private void CalculateCoordinatesByDepth(int depth, ref double topOffset)
        {
            topOffset -= TextHeights[depth];
            var leftOffset = LeftMargin;
            var stackNumber = 0;
            foreach (var reDrawNode in _nodesOnDepth[depth])
            {
                leftOffset += Width/2;
                reDrawNode.StackNumber = stackNumber;
                var centerLeftOfChildNodes = reDrawNode.GetCenterLeftOfChildNode();
                if (reDrawNode.HasChild)
                {
                    if (leftOffset <= centerLeftOfChildNodes)
                    {
                        leftOffset = centerLeftOfChildNodes;
                    }
                    else
                    {
                        //get the minimum statck number of childs
                        var deeperStackNumber = reDrawNode.ChildNodes.Min(q => q.StackNumber);

                        //Get all the nodes that deeper then current node
                        var deeperNodes = Nodes.Where(q => q.RootDepth == depth + 1 && q.StackNumber >= deeperStackNumber);

                        var offset = leftOffset - centerLeftOfChildNodes;
                        foreach (var redrawNode in deeperNodes)
                        {
                            redrawNode.AddLeftOffset(offset);
                        }
                    }

                }
                else
                {
                    if (leftOffset <= reDrawNode.StackNumber * Width + Width / 2 + LeftMargin)
                        leftOffset = reDrawNode.StackNumber * Width + Width / 2 + LeftMargin;
                }
                reDrawNode.Top = topOffset;
                reDrawNode.Left = leftOffset;
                leftOffset += Width/2;
                stackNumber ++;
            }
            topOffset -= ImageHeights[depth];
        }

        protected override void CalculateCoordinates()
        {
            _nodesOnDepth = new Dictionary<int, List<RealignNode>>();
            var maxDepth = Nodes.Max(q => q.RootDepth);
            if (maxDepth >= 0)
            {
                for (var i = 0; i <= maxDepth; i++)
                {
                    _nodesOnDepth.Add(i, Nodes.Where(q => q.RootDepth == i).ToList());
                    //Get all nodes on depth i
                    if (!_nodesOnDepth[i].Any())
                    {
                        TextHeights.Add(i, GlymaParameters.Margin * 2);
                        ImageHeights.Add(i, GlymaParameters.Margin * 2);
                    }
                    else
                    {


                        //Found the maximum height
                        var maxHeight = _nodesOnDepth[i].Max(q => q.TextHeight);

                        //Add margin to height, and store it in height array
                        TextHeights.Add(i, GlymaParameters.Margin * 2 + maxHeight);
                        ImageHeights.Add(i, GlymaParameters.NodeImageHeight);
                    }

                }

                var topOffset = GlymaParameters.Margin + TextHeights.Sum(q => q.Value) + ImageHeights.Sum(q => q.Value);
                for (var i = maxDepth; i >= 0; i--)
                {
                    CalculateCoordinatesByDepth(i, ref topOffset);
                }
            }
        }
    }
}
