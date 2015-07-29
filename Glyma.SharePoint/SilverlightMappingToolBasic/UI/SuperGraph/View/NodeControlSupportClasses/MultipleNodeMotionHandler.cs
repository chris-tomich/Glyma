using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse;
using System.Collections.Generic;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Selector;
using SimpleIoC;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses
{
    public class MultipleNodeMotionHandler : INodeMotionHandler
    {
        private IIoCContainer _ioc;

        public MultipleNodeMotionHandler(INodesSelector selector)
        {
            Selector = selector;
        }

        public INodesSelector Selector { get; private set; }
        public Canvas ParentSurface { get; private set; }
        public UIElement MouseManagingControl { get; private set; }
        public event EventHandler<NodeMotionHandlerEventArgs> NodeInMotionEvent;

        private IIoCContainer IoC
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

        public void OnLeftButtonDragMove(object sender, MouseEventArgs e)
        {
            foreach (var nodeControl in Selector.NodeControls)
            {
                nodeControl.MotionHandler.NodeMotionHandler.OnLeftButtonDragMove(sender, e);
            }
        }

        public void OnLeftButtonDragReleased(object sender, MouseManagerEventArgs e)
        {
            var nodesToBeCommitted = new List<ViewModel.Node>();

            foreach (var nodeControl in Selector.NodeControls)
            {
                var transactionalMouseManagerEventArgs = new TransactionalMouseManagerEventArgs();
                transactionalMouseManagerEventArgs.InnerMouseManagerEventArgs = e;
                transactionalMouseManagerEventArgs.IsTransactional = true;
                transactionalMouseManagerEventArgs.NodesToBeCommitted = nodesToBeCommitted;

                nodeControl.MotionHandler.NodeMotionHandler.OnLeftButtonDragReleased(sender, transactionalMouseManagerEventArgs);
            }

            /// TODO: The following needs to be removed. This should not be in here.
            /// The reason this is bad is because a UI level component is making commands direct into the controller rather than raising it through events.
            /// The issue here is that due to the current architecture, this MupltipleNodeMotionHandler, although it belongs to only one node, it actually performs
            /// operations for EVERY node on the graph. This is very poor encapsulation.
            IoC.GetInstance<Controller.ISuperGraphNodeBatchOperations>().CommitLocations(nodesToBeCommitted);
        }

        public void OnLeftButtonDragStart(object sender, MouseManagerEventArgs e)
        {
            foreach (var nodeControl in Selector.NodeControls)
            {
                nodeControl.MotionHandler.NodeMotionHandler.OnLeftButtonDragStart(sender, e);
            }
        }

        public void OnMapMoved(object sender, MoveTransformEventArgs e)
        {
            
        }
    }
}
