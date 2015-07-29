using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SilverlightMappingToolBasic.UI.SuperGraph.Controller;
using SilverlightMappingToolBasic.UI.SuperGraph.View.CollapseSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Selector;
using SimpleIoC;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow
{
    public class ArrowCreationManager
    {
        private readonly List<bool> _added = new List<bool>();
        private IIoCContainer _ioc;
        private List<ArrowCreationSet> _arrowCreationSets;

        public ArrowCreationManager(IArrowContainerControl parentSurface)
        {
            InMotion = false;
            ParentSurface = parentSurface;
            parentSurface.MapMoved += OnMapMoved;
            ParentSurface.MouseMove += ParentSurface_MouseMove;
        }


        public IIoCContainer IoC
        {
            get
            {
                if (_ioc == null)
                {
                    _ioc = IoCContainer.GetInjectionInstance();
                }

                return _ioc;
            }
        }

        private void OnMapMoved(object sender, MoveTransformEventArgs e)
        {
            if (InMotion)
            {
                foreach (var arrowCreationSet in ArrowCreationSets)
                {
                    var from = arrowCreationSet.From.Centre;
                    from.X += e.X;
                    from.Y += e.Y;
                    arrowCreationSet.From.Centre = from;
                }
                
            }
        }

        private void ParentSurface_MouseMove(object sender, MouseEventArgs e)
        {
            if (InMotion)
            {
                foreach (var arrowCreationSet in ArrowCreationSets)
                {
                    var location = e.GetPosition((UIElement)ParentSurface);
                    location.Y = location.Y / ParentSurface.Zoom;
                    location.X = location.X / ParentSurface.Zoom;

                    if (!arrowCreationSet.IsAdded)
                    {
                        ParentSurface.Add(arrowCreationSet.ArrowControl);
                        arrowCreationSet.IsAdded = true;
                    }
                    Canvas.SetZIndex(arrowCreationSet.ArrowControl,1000);
                    arrowCreationSet.To.Centre = location;
                }
                
            }
        }

        private IArrowContainerControl ParentSurface
        {
            get;
            set;
        }

        private List<ArrowCreationSet> ArrowCreationSets
        {
            get
            {
                if (_arrowCreationSets == null)
                {
                    _arrowCreationSets = new List<ArrowCreationSet>();
                }
                return _arrowCreationSets;
            }
        }

        public bool InMotion
        {
            get;
            set;
        }

        public bool InTransfrom
        {
            get; 
            set;
        }

        public Transform RenderTransform
        {
            get;
            set;
        }

        public void SetFrom(NodeControl nodeControl)
        {
            InMotion = true;
            var location = RenderTransform.Transform(nodeControl.Centre);
            ArrowCreationSets.Add(new ArrowCreationSet(nodeControl,location));
        }

        public void SetTo(NodeControl nodeControl)
        {
            if (nodeControl != null)
            {
                foreach (var arrowCreationSet in ArrowCreationSets)
                {
                    if (arrowCreationSet.NodeControl != null && arrowCreationSet.NodeControl != nodeControl)
                    {
                        if (!ParentSurface.IsArrowExist(arrowCreationSet.NodeControl, nodeControl))
                        {
                            if (nodeControl.CollapseControl.State == CollapseState.None)
                            {
                                nodeControl.CollapseControl.State = CollapseState.Expanded;
                            }
                            else if (nodeControl.CollapseControl.State == CollapseState.Collapsed)
                            {
                                nodeControl.CollapseControl.State = CollapseState.SemiCollapsed;
                            }

                            if (arrowCreationSet.NodeControl.ViewModelNode.IsTranscluded || nodeControl.ViewModelNode.IsTranscluded)
                            {
                                IoC.GetInstance<ISuperGraphRelationshipFactory>().ConnectTranscludedNodes(arrowCreationSet.NodeControl.ViewModelNode, nodeControl.ViewModelNode);
                            }
                            else
                            {
                                IoC.GetInstance<ISuperGraphRelationshipFactory>().ConnectNodes(arrowCreationSet.NodeControl.ViewModelNode, nodeControl.ViewModelNode);
                            }
                        }
                    }
                }
            }
            Reset();
        }

        private void Reset()
        {
            foreach (var arrowCreationSet in ArrowCreationSets)
            {
                ParentSurface.Remove(arrowCreationSet.ArrowControl);
            }
            ArrowCreationSets.Clear();
            InMotion = false;
            _added.Clear();
        }
    }
}
