using System.Windows;
using System.Collections.Generic;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Realign.Interface;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Realign
{
    /// <summary>
    /// Any problems with this Class must fix by Sam Zhou
    /// </summary>
    public abstract class RealignFactory : RealignFactoryBase, IRealignFactory
    {
        // Construction
        protected RealignFactory(IRealignControl mainControl, RealignStyle realignStyle, bool isPartialRealign)
            : base(mainControl, realignStyle, isPartialRealign)
        {
            CalculateNodes();
        }

        private void CalculateNodes()
        {
            if (Nodes.Count > 0)
            {
                CalculateCoordinates();
            }
        }

        protected abstract void CalculateCoordinates();

        public List<ViewModel.Node> Draw()
        {
            var nodesToBeCommitted = new List<ViewModel.Node>();
            foreach (var node in Nodes)
            {
                node.NodeControl.Centre = new Point(node.Left, node.Top);
                node.NodeControl.ViewModelNode.Location = node.NodeControl.Centre;
                nodesToBeCommitted.Add(node.NodeControl.ViewModelNode);
            }
            return nodesToBeCommitted;
        }
    }
}

