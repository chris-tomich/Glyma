using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Realign.Interface;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Realign
{
    /// <summary>
    /// Any problems with this Class must fix by Sam Zhou
    /// </summary>
    public abstract class RealignFactoryBase
    {
        private readonly RealignMap _map;
        private List<RealignNode> _nodes;
        

        protected RealignFactoryBase(IRealignControl mainControl, RealignStyle realignStyle, bool isPartialRealign)
        {
            Ref = mainControl;
            RealignStyle = realignStyle;
            if (Ref.HasVisibleNodes())
            {
                _map = new RealignMap(Ref.GetVisibleNodeControls().ToList(), Ref.GetRelationships().ToList(), realignStyle, isPartialRealign);
            }
            if (_map != null && _map.ChildNodes.Count > 0)
            {
                Nodes = _map.GetAllChildNodes(realignStyle).ToList();
            }
        }

        public static RealignStyle RealignStyle
        {
            get;
            set;
        }

        protected IRealignControl Ref
        {
            get;
            private set;
        }

        protected List<RealignNode> Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    _nodes = new List<RealignNode>();
                }
                return _nodes;
            }
            private set
            {
                _nodes = value;
            }
        }

        
    }
}
