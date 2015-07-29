using System.Collections.Generic;
using System.Linq;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.CollapseSupportClasses
{
    public class ChildrenControlCollection
    {
        private readonly NodeControl _nodeControl;
        private readonly bool _isParentIncluded;
        private readonly bool _checkOneLevel;

        public ChildrenControlCollection(NodeControl nodeControl, bool isParentIncluded = false, bool checkOneLevel = false)
        {
            _nodeControl = nodeControl;
            _isParentIncluded = isParentIncluded;
            _checkOneLevel = checkOneLevel;
            FindChildrenNodeControls();
            FindChildrenArrowControls();
        }

        public IEnumerable<NodeControl> NodeControls
        {
            get;
            private set;
        }

        public IEnumerable<ArrowControl> ArrowControls
        {
            get;
            private set;
        }

        private void FindChildrenNodeControls()
        {
            if (_isParentIncluded)
            {
                if (_checkOneLevel)
                {
                    var nodes = new List<NodeControl>{_nodeControl};
                    nodes.AddRange(_nodeControl.ChildNodes);
                    NodeControls = nodes;
                }
                else
                {
                    NodeControls = _nodeControl.GetAllNodeControls();
                }
                
            }
            else
            {
                if (_checkOneLevel)
                {
                    NodeControls = _nodeControl.ChildNodes;
                }
                else
                {
                    NodeControls = _nodeControl.GetAllChildNodeControls();
                }
            }
        }

        private void FindChildrenArrowControls()
        {
            var arrowControls = new List<ArrowControl>();
            foreach (var arrowControl in _nodeControl.ParentSurface.GetArrowControls())
            {
                if (arrowControl.DataContext is ArrowViewModel)
                {
                    var arrowViewModel = arrowControl.DataContext as ArrowViewModel;

                    foreach (var nodeControl in NodeControls)
                    {
                        if (arrowViewModel.ViewModelRelationship.From == nodeControl.ViewModelNode.Id)
                        {
                            var visibleNodeControls = _nodeControl.ParentSurface.GetVisibleNodeControls().ToList();
                            visibleNodeControls.AddRange(NodeControls);
                            if (visibleNodeControls.Distinct().Any(q => q.ViewModelNode.Id == arrowViewModel.ViewModelRelationship.To))
                            {
                                arrowControls.Add(arrowControl);
                            }
                        }
                        else if ((arrowViewModel.ViewModelRelationship.To == nodeControl.ViewModelNode.Id && !(_isParentIncluded && nodeControl.CollapseControl.State == CollapseState.Collapsed)))
                        {
                            arrowControls.Add(arrowControl);
                        }
                    }
                }
            }
            ArrowControls = arrowControls;
        }

        public bool ContainsNodeControl(NodeControl nodeControl)
        {
            return NodeControls.Contains(nodeControl);
        }
    }
}
