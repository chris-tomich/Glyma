using System;
using System.Collections.Generic;
using System.Linq;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Realign.Interface
{
    public interface IRealignObject
    {
        List<RealignNode> ChildNodes { get; }

        IOrderedEnumerable<RealignNode> GetOrderedChildNodes(RealignStyle realignStyle = RealignStyle.Horizontal);
        IEnumerable<RealignNode> GetAllChildNodes(RealignStyle realignStyle = RealignStyle.Horizontal);
        void AddChild(RealignNode node);
    }
}
