using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Diagnostics;

using SilverlightMappingToolBasic.MappingService;

namespace SilverlightMappingToolBasic.MapDepth
{
    public class MapDepthNavigator : NodeNavigator
    {
        protected MapDepthNavigator() : base()
        {
        }

        public MapDepthNavigator(INodeService nodeService, ThemeManager themeManagementObject, Guid domainId)
            : this()
        {
            DomainId = domainId;
            NodeService = nodeService;
            NodeService.GetMapsNodesCompleted += new EventHandler<ReturnedNodesEventArgs>(OnGetMapsNodesCompletedNodeArgs);
            NodeService.AddNodeCompleted += new EventHandler<AddedNodeEventArgs>(OnAddNodeCompletedNodeArgs);
            NodeService.PasteNodeCloneCompleted += new EventHandler<AddedNodeEventArgs>(OnPasteNodeCloneCompletedNodeArgs);
            NodeService.UpdateNodeMetadataCompleted += new EventHandler<ReturnedNodesEventArgs>(OnUpdateNodeMetadataCompletedNodeArgs);
            NodeService.ConnectNodesCompleted += new EventHandler<ConnectedNodesEventArgs>(OnConnectNodesCompletedNodeArgs);
            NodeService.DeleteNodeCompleted += new EventHandler<DeleteEventArgs>(OnDeleteNodeCompletedNodeArgs);
            NodeService.DeleteRelationshipCompleted += new EventHandler<DeleteEventArgs>(OnDeleteRelationshipCompleted);
            ThemeManagementObject = themeManagementObject;
        }

        
        #region INodeNavigator Members

        public override void GetCurrentNodesAsync()
        {
            NodeService.GetMapsNodes(DomainId, FocalNodeId);
        }

        #endregion

        #region New Stuff

        public override event EventHandler<NodesEventArgs> PasteNodeCloneCompleted;
        public override event EventHandler<NodesEventArgs> ConnectNodesCompleted;
        public override event EventHandler<NodesEventArgs> GetCurrentNodesCompleted;
        public override event EventHandler<NodesEventArgs> UpdateNodeMetadataCompleted;
        public override event EventHandler<NodesEventArgs> DeleteNodeCompleted;
        public override event EventHandler<NodesEventArgs> DeleteRelationshipCompleted;
        public override event EventHandler<AddedNodeEventArgs> AddNodeCompleted;

        private void OnAddNodeCompletedNodeArgs(object sender, AddedNodeEventArgs e)
        {
            NodesEventArgs nodesEventArgs = new NodesEventArgs(null, e.Node, (INodeProxy[])null);

            ConnectNodeToMap(e.Node.Id, e.Location); //the event of the connect will add it to the map

            if (AddNodeCompleted != null)
            {
                AddNodeCompleted.Invoke(this, e);
            }
        }

        private void OnPasteNodeCloneCompletedNodeArgs(object sender, AddedNodeEventArgs e)
        {
            NodesEventArgs nodesEventArgs = new NodesEventArgs(null, e.Node, (INodeProxy[])null);

            ConnectNodeToMap(e.Node.Id, e.Location);

            if (PasteNodeCloneCompleted != null)
            {
                PasteNodeCloneCompleted.Invoke(this, nodesEventArgs);
            }
        }

        private void OnConnectNodesCompletedNodeArgs(object sender, ConnectedNodesEventArgs e)
        {
            List<INodeProxy> nodes = new List<INodeProxy>();
            TypeManager typeManager = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>();
            IMetadataTypeProxy metaDataTypeProxy = typeManager.GetMetadataType("double");

            if (e.Relationship.RelationshipType.Name == "TransclusionRelationship")
            {
                foreach (IDescriptorProxy descriptorProxy in e.Relationship.Descriptors.GetByDescriptorTypeName("To"))
                {
                    nodes.Add(descriptorProxy.Node);
                }

                if (e.Nodes.Length == 2) //TODO: Fix this, currently checking if it's just a transcluded node to map relationship
                {
                    IDescriptorTypeProxy transclusionMapDesc = typeManager.GetDescriptorType("TransclusionMap");

                    MetadataContext xPositionKey = new MetadataContext()
                    {
                        NodeUid = e.Nodes[1].Id,
                        MetadataName = "XPosition",
                        RelationshipUid = e.Relationship.Id,
                        DescriptorTypeUid = transclusionMapDesc.Id
                    };

                    MetadataContext yPositionKey = new MetadataContext()
                    {
                        NodeUid = e.Nodes[1].Id,
                        MetadataName = "YPosition",
                        RelationshipUid = e.Relationship.Id,
                        DescriptorTypeUid = transclusionMapDesc.Id
                    };

                    //it'll be the second node that is the added node, the first node is the map - this is unsafe to assume so needs to be fixed
                    UpdateNodeMetadataAsync(e.Nodes[1], e.Relationship.Id, transclusionMapDesc, "XPosition", e.Nodes[1].GetNodeMetadata(xPositionKey).MetadataValue, metaDataTypeProxy);
                    UpdateNodeMetadataAsync(e.Nodes[1], e.Relationship.Id, transclusionMapDesc, "YPosition", e.Nodes[1].GetNodeMetadata(yPositionKey).MetadataValue, metaDataTypeProxy);
                }
            }
            else if (e.Relationship.RelationshipType.Name == "MapContainerRelationship")
            {
                //if it's a MapContainerRelationship update the positioning based on the context of this map it's being added to

                IDescriptorTypeProxy fromDescriptorTypeProxy = null;
                if (e.Relationship != null)
                {
                    fromDescriptorTypeProxy = typeManager.GetDescriptorType("From");

                    MetadataContext xPositionKey = new MetadataContext()
                    {
                        NodeUid = e.Nodes[1].Id,
                        MetadataName = "XPosition",
                        RelationshipUid = e.Relationship.Id,
                        DescriptorTypeUid = fromDescriptorTypeProxy.Id
                    };

                    MetadataContext yPositionKey = new MetadataContext()
                    {
                        NodeUid = e.Nodes[1].Id,
                        MetadataName = "YPosition",
                        RelationshipUid = e.Relationship.Id,
                        DescriptorTypeUid = fromDescriptorTypeProxy.Id
                    };

                    //it'll be the second node that is the added node, the first node is the map - this is unsafe to assume so needs to be fixed
                    UpdateNodeMetadataAsync(e.Nodes[1], e.Relationship.Id, fromDescriptorTypeProxy, "XPosition", e.Nodes[1].GetNodeMetadata(xPositionKey).MetadataValue, metaDataTypeProxy);
                    UpdateNodeMetadataAsync(e.Nodes[1], e.Relationship.Id, fromDescriptorTypeProxy, "YPosition", e.Nodes[1].GetNodeMetadata(yPositionKey).MetadataValue, metaDataTypeProxy);

                    nodes.Add(e.Nodes[1]);
                }
            }
            else
            {
                foreach (INodeProxy nodeProxy in e.Nodes)
                {
                    nodes.Add(nodeProxy);
                }
            }

            NodesEventArgs nodesEventArgs = new NodesEventArgs(null, null, nodes.ToArray());

            if (ConnectNodesCompleted != null)
            {
                ConnectNodesCompleted.Invoke(this, nodesEventArgs);
            }
        }

        private void OnGetMapsNodesCompletedNodeArgs(object sender, ReturnedNodesEventArgs e)
        {
            INodeProxy focalNode = FocalNode;
            List<INodeProxy> filteredNodes = FiltersResults(e.Nodes);
            CompleteRelationshipLinks(filteredNodes);
            List<INodeProxy> nodes = new List<INodeProxy>();

            foreach (INodeProxy nodeProxy in filteredNodes)
            {
                if (nodeProxy.Id == FocalNodeId)
                {
                    //Set the focal node if this is the main node in focus
                    FocalNode = nodeProxy;
                    focalNode = FocalNode;
                }
                else
                {
                    nodes.Add(nodeProxy);
                }
                if (nodeProxy.ParentMapNodeUid != FocalNodeId)
                {
                    nodeProxy.IsTransclusion = true;
                }
                else
                {
                    nodeProxy.IsTransclusion = false;
                }
            }

            NodesEventArgs nodesEventArgs = new NodesEventArgs(null, focalNode, nodes.ToArray());

            if (GetCurrentNodesCompleted != null)
            {
                GetCurrentNodesCompleted.Invoke(this, nodesEventArgs);
            }
        }

        private void CompleteRelationshipLinks(List<INodeProxy> nodes)
        {
            IDictionary<Guid, INodeProxy> nodeDict = new Dictionary<Guid, INodeProxy>();
            foreach (INodeProxy nodeProxy in nodes)
            {
                nodeDict.Add(nodeProxy.Id, nodeProxy);
            }
            foreach (INodeProxy nodeProxy in nodes)
            {
                foreach (IDescriptorProxy descriptor in nodeProxy.Descriptors)
                {
                    foreach (IDescriptorProxy altDescriptor in descriptor.Relationship.Descriptors)
                    {
                        if (altDescriptor.Node == null && altDescriptor.NodeId != Guid.Empty
                            && nodeDict.ContainsKey(altDescriptor.NodeId))
                        {
                            altDescriptor.Node = nodeDict[altDescriptor.NodeId];
                        }
                        else if ((altDescriptor.Node == null && altDescriptor.NodeId != Guid.Empty
                            && FocalNode != null && FocalNode.Id == altDescriptor.NodeId))
                        {
                            altDescriptor.Node = FocalNode;
                        }
                    }
                }
            }
        }

        private void OnUpdateNodeMetadataCompletedNodeArgs(object sender, ReturnedNodesEventArgs e)
        {
            NodesEventArgs nodesEventArgs = new NodesEventArgs(null, e.SearchedNode, e.Nodes);

            if (UpdateNodeMetadataCompleted != null)
            {
                UpdateNodeMetadataCompleted.Invoke(this, nodesEventArgs);
            }
        }

        private void OnDeleteNodeCompletedNodeArgs(object sender, DeleteEventArgs e)
        {
            NodesEventArgs nodesEventArgs = new NodesEventArgs(null, e.DeletedId, (Guid[])null);

            if (DeleteNodeCompleted != null)
            {
                DeleteNodeCompleted.Invoke(this, nodesEventArgs);
            }
        }

        private void OnDeleteRelationshipCompleted(object sender, DeleteEventArgs e)
        {
            NodesEventArgs nodesEventArgs = new NodesEventArgs("DeletedRelationship", e.DeletedId);

            if (DeleteRelationshipCompleted != null)
            {
                DeleteRelationshipCompleted.Invoke(this, nodesEventArgs);
            }
        }

        #endregion

        private void ConnectNodeToMap(Guid nodeId, Point location)
        {
            TypeManager typeManager = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>();
            IDescriptorTypeProxy toDescriptorTypeProxy = typeManager.GetDescriptorType("To");
            IDescriptorTypeProxy fromDescriptorTypeProxy = typeManager.GetDescriptorType("From");

            Dictionary<IDescriptorTypeProxy, Guid> nodes = new Dictionary<IDescriptorTypeProxy, Guid>();
            nodes.Add(toDescriptorTypeProxy, FocalNodeId);
            nodes.Add(fromDescriptorTypeProxy, nodeId);

            IRelationshipTypeProxy relationshipTypeProxy = typeManager.GetRelationshipType("MapContainerRelationship");

            ConnectNodesAsync(nodes, relationshipTypeProxy, location, string.Empty);
        }

    }
}
