using System;
using System.Collections.Generic;
using System.Linq;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Realign.Interface;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Realign
{
    public abstract class RealignObject : IRealignObject
    {
        private List<RealignNode> _childNodes;

        public List<RealignNode> ChildNodes
        {
            get
            {
                if (_childNodes == null)
                    _childNodes = new List<RealignNode>();
                return _childNodes;
            }
            set
            {
                _childNodes = value;
            }
        }

        public IOrderedEnumerable<RealignNode> GetOrderedChildNodes(RealignStyle realignStyle = RealignStyle.Horizontal)
        {
            if (realignStyle == RealignStyle.Horizontal)
            {
                return ChildNodes.OrderBy(q => q.OriginTop).ThenBy(q => q.OriginLeft);
            }
            else
            {
                return ChildNodes.OrderBy(q => q.OriginLeft).ThenBy(q => q.OriginTop);
            }
        }

        public void AddChild(RealignNode node)
        {
            ChildNodes.Add(node);
        }

        // retrieve a list of nodes that exist in the map with out the caller itself.
        public IEnumerable<RealignNode> GetAllChildNodes(RealignStyle realignStyle = RealignStyle.Horizontal)
        {
            var output = new List<RealignNode>();
            foreach (var node in GetOrderedChildNodes(realignStyle))
            {
                if (node.HasChild)
                    output.AddRange(node.GetAllNodes(realignStyle));
                else
                    output.Add(node);
            }
            return output;
        }
    }
}
