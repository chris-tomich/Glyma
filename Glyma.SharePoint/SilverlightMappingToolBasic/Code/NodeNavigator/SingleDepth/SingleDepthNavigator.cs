using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using SilverlightMappingToolBasic.MappingService;
using System.Collections;

namespace SilverlightMappingToolBasic.SingleDepth
{
    public class SingleDepthNavigator : NodeNavigator
    {
        protected SingleDepthNavigator() : base()
        {  
        }

        public SingleDepthNavigator(INodeService nodeService, ThemeManager themeManagementObject, Guid domainId)
            : this()
        {
            DomainId = domainId;
            NodeService = nodeService;
            NodeService.GetRelatedNodesByIdCompleted += new EventHandler<ReturnedNodesEventArgs>(OnGetRelatedNodesByIdCompleted);
            ThemeManagementObject = themeManagementObject;
        }

        private void OnGetRelatedNodesByIdCompleted(object sender, ReturnedNodesEventArgs e)
        {
            int count = 0;
            int length = e.Nodes.Length - 1;

            if (Views.ContainsKey(FocalNodeId))
            {
                CurrentView = Views[FocalNodeId];
            }
            else
            {
                CurrentView = new NavigatorView();

                Views.Add(FocalNodeId, CurrentView);

                SingleDepthNodeRenderer focalNodeRenderer = null;

                foreach (INodeProxy nodeProxy in e.Nodes)
                {
                    if (nodeProxy.Id == FocalNodeId)
                    {
                        if (FocalNode == null)
                        {
                            FocalNode = nodeProxy;

                            focalNodeRenderer = new SingleDepthNodeRenderer(CurrentView, FocalNode, ThemeManagementObject, "Focal");

                            CurrentView.NodeRenderers.Add(focalNodeRenderer.Node.Id, focalNodeRenderer);

                            focalNodeRenderer.NodePositionUpdating += new EventHandler(OnNodePositionUpdating);
                            focalNodeRenderer.NodeDoubleClicked += new EventHandler<NodeClickedArgs>(OnNodeDoubleClicked);
                        }
                    }
                    else
                    {
                        SingleDepthNodeRenderer nodeRenderer = new SingleDepthNodeRenderer(CurrentView, nodeProxy, ThemeManagementObject, "Default");

                            nodeRenderer.NodeIndex = count;
                            nodeRenderer.NodeCount = length;

                            CurrentView.NodeRenderers.Add(nodeRenderer.Node.Id, nodeRenderer);

                            nodeRenderer.NodePositionUpdating += new EventHandler(OnNodePositionUpdating);
                            nodeRenderer.NodeDoubleClicked += new EventHandler<NodeClickedArgs>(OnNodeDoubleClicked);

                            count++;
                    }
                }

                if (focalNodeRenderer == null)
                {
                    return;
                }

                foreach (INodeRenderer nodeRenderer in CurrentView.NodeRenderers.Values)
                {
                    foreach (IDescriptorProxy descriptor in nodeRenderer.Node.Descriptors)
                    {
                        if (!CurrentView.RelationshipRenderers.ContainsKey(descriptor.Relationship.Id))
                        {
                            bool relationshipIsPresent = true;

                            foreach (IDescriptorProxy alternateDescriptor in descriptor.Relationship.Descriptors)
                            {
                                if (!CurrentView.NodeRenderers.ContainsKey(alternateDescriptor.Node.Id))
                                {
                                    relationshipIsPresent = false;
                                    break;
                                }
                            }

                            if (relationshipIsPresent)
                            {
                                ImprovedArrow arrow = new ImprovedArrow(CurrentView, descriptor.Relationship);

                                CurrentView.RelationshipRenderers.Add(descriptor.Relationship.Id, arrow);
                            }
                        }
                    }
                }
            }

            RendererNodesEventArgs rendererNodesEventArgs = new RendererNodesEventArgs(CurrentView);

            if (GetCurrentNodesCompleted != null)
            {
                GetCurrentNodesCompleted.Invoke(sender, rendererNodesEventArgs);
            }
        }

        protected override void OnNodePositionUpdating(object sender, EventArgs e)
        {
            INodeRenderer nodeRenderer = (INodeRenderer)sender;

            if (nodeRenderer != null)
            {
                CurrentView.RelationshipRenderers.UpdateRelationship(nodeRenderer);
            }
        }

        protected override void OnNodePositionUpdated(object sender, EventArgs e)
        {
            //do nothing nodes find their own positions so it's not stored metadata
        }

        protected override void OnNodeDoubleClicked(object sender, NodeClickedArgs e)
        {
            SetCurrentNode(e.Node);
            GetCurrentNodesAsync();
        }

        #region INodeNavigator Members
        public override event EventHandler<RendererNodesEventArgs> GetCurrentNodesCompleted;
        public override event EventHandler<UpdatedRendererNodesEventArgs> AddNodeCompleted;
        public override event EventHandler<UpdatedRendererNodesEventArgs> ConnectNodesCompleted;
        public override event EventHandler<ReturnedNodesEventArgs> UpdateNodeMetadataCompleted;
        public override event EventHandler<DeleteNodeEventArgs> DeleteNodeCompleted;

        public override void GetCurrentNodesAsync()
        {
            NodeService.GetRelatedNodesByIdAsync(DomainId, FocalNodeId, 1);
        }
        #endregion
    }
}
