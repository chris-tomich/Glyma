using System.Linq;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Realign.Interface;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Realign
{
    /// <summary>
    /// Any problems with this Class must fix by Sam Zhou
    /// </summary>
    public class HorizontalRealignFactory : RealignFactory
    {
        private const double TopMargin = GlymaParameters.Margin;
        private const double Width = GlymaParameters.NodeTextWidth + GlymaParameters.Margin * 2;


        public HorizontalRealignFactory(IRealignControl mainControl, bool isPartialRealign = false)
            : base(mainControl, RealignStyle.Horizontal, isPartialRealign)
        {
            mainControl.MapInformation.AutoRealignStyle = RealignStyle.Horizontal;
        }

        private void CalculateCoordinatesByDepth(int depth)
        {
            var topOffset = TopMargin;
            var leftOffset = (depth + 0.5)  * Width;
            var stackNumber = 0;
            foreach (var reDrawNode in Nodes.Where(q => q.RootDepth == depth))
            {
                reDrawNode.StackNumber = stackNumber;
                topOffset += reDrawNode.ImageHeight;
                var centerTopOfChildNodes = reDrawNode.GetCenterTopOfChildNode();
                if (reDrawNode.HasChild)
                {
                    if (centerTopOfChildNodes >= topOffset)
                    {
                        topOffset = centerTopOfChildNodes;
                    }
                    else
                    {
                        if (topOffset <= centerTopOfChildNodes && !reDrawNode.GetAnyChildHasMultipleParents())
                        {
                            topOffset = centerTopOfChildNodes;
                        }
                        else
                        {
                            //get the minimum statck number of childs
                            var deeperStackNumber = reDrawNode.ChildNodes.Min(q => q.StackNumber);

                            //Get all the nodes that deeper then current node
                            var deeperNodes = Nodes.Where(q => q.RootDepth == depth + 1 && q.StackNumber >= deeperStackNumber).Distinct();
                            var offset = topOffset - centerTopOfChildNodes;
                            foreach (var redrawNode in deeperNodes)
                            {
                                redrawNode.AddTopOffset(offset);
                            }
                        }
                    }
                    
                }
                reDrawNode.Top = topOffset;
                reDrawNode.Left = leftOffset;
                topOffset += reDrawNode.TextHeight;
                stackNumber++;
            }
        }

        protected override void CalculateCoordinates()
        {
            var maxDepth = Nodes.Max(q => q.RootDepth);
            if (maxDepth >= 0)
            {
                for (var i = maxDepth; i >= 0; i--)
                {
                    CalculateCoordinatesByDepth(i);
                }
            }
        }
    }
}
