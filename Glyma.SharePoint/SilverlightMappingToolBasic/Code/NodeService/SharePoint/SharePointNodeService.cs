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
using System.ServiceModel;
using SilverlightMappingToolBasic.Service;
using System.Collections.Generic;
using System.Threading;
using SilverlightMappingToolBasic.SingleDepth;

namespace SilverlightMappingToolBasic.SharePoint
{
    public class SharePointNodeService : INodeService
    {
        private Dictionary<string, INodeProxy> _cachedNodes;
        private MappingToolServiceClient _client = null;
        private Dictionary<string, INodeProxy> _localNewNodes;

        public SharePointNodeService()
        {
            _cachedNodes = new Dictionary<string, INodeProxy>();
            _localNewNodes = new Dictionary<string, INodeProxy>();
        }

        private void GetNodeByIdAsyncCompleted(object sender, GetNodesCompletedEventArgs e)
        {
            List<INodeProxy> nodes = new List<INodeProxy>();
            
            foreach (SoapNode soapNode in e.Result.Values)
            {
                NodeProxy node = new NodeProxy(soapNode);

                nodes.Add(node);
                _cachedNodes.Add(node.Id, node);
            }

            ReturnedNodesEventArgs eventArgs = new ReturnedNodesEventArgs(null, nodes.ToArray());

            if (GetNodeByIdCompleted != null)
            {
                GetNodeByIdCompleted.Invoke(this, eventArgs);
            }
        }

        private void GetRelatedNodesByIdAsyncCompleted(object sender, GetRelatedNodesCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                List<INodeProxy> nodes = new List<INodeProxy>();
                
                foreach (SoapNode soapNode in e.Result.RelatedNodes.Values)
                {
                    if (!_cachedNodes.ContainsKey(soapNode.Id))
                    {
                        NodeProxy node = new NodeProxy(soapNode);
                        _cachedNodes.Add(soapNode.Id, node);
                    }
                }

                foreach (SoapNode soapNode in e.Result.RelatedNodes.Values)
                {
                    INodeProxy node = _cachedNodes[soapNode.Id];

                    foreach (SoapRelationship relationship in soapNode.Relationships)
                    {
                        RelationshipProxy relationshipProxy = new RelationshipProxy(relationship);

                        foreach (DescriptorProxy descriptor in relationshipProxy.Descriptors)
                        {
                            if (_cachedNodes.ContainsKey(descriptor.NodeId))
                            {
                                INodeProxy connectedNode = _cachedNodes[descriptor.NodeId];

                                if (!connectedNode.Descriptors.Contains(descriptor))
                                {
                                    descriptor.Node = connectedNode;

                                    connectedNode.Descriptors.Add(descriptor);
                                }
                            }
                        }
                    }
                }

                INodeProxy nodeProxy = _cachedNodes[e.Result.SearchedNode.Id];
                nodes.Add(nodeProxy);

                foreach (INodeProxy np in _localNewNodes.Values)
                {
                    nodes.Add(np);
                }

                foreach (IDescriptorProxy descriptor in nodeProxy.Descriptors)
                {
                    foreach (IDescriptorProxy alternateDescriptor in descriptor.Relationship.Descriptors)
                    {
                        if (!nodes.Contains(alternateDescriptor.Node))
                        {
                            nodes.Add(alternateDescriptor.Node);
                        }
                    }
                }

                INodeProxy searchedNode = new NodeProxy(e.Result.SearchedNode);

                ReturnedNodesEventArgs eventArgs = new ReturnedNodesEventArgs(searchedNode, nodes.ToArray());

                if (GetRelatedNodesByIdCompleted != null)
                {
                    GetRelatedNodesByIdCompleted.Invoke(this, eventArgs);
                }
            }
        }

        private void GetAllNodeTypeResourcesCompleted(object sender, GetAllNodeTypeResourcesCompletedEventArgs e)
        {
            List<INodeTypeProxy> nodeTypeProxies = new List<INodeTypeProxy>();

            foreach (SoapNodeType nodeType in e.Result.Values)
            {
                INodeTypeProxy nodeTypeProxy = new NodeTypeProxy(nodeType);

                nodeTypeProxies.Add(nodeTypeProxy);
            }

            ReturnedNodeTypesEventArgs eventArgs = new ReturnedNodeTypesEventArgs(nodeTypeProxies.ToArray());

            GetAllNodeTypesCompleted.Invoke(this, eventArgs);
        }

        #region INodeService Members

        public event EventHandler<ReturnedNodesEventArgs> GetNodeByIdCompleted;

        public event EventHandler<ReturnedNodesEventArgs> GetRelatedNodesByIdCompleted;

        public event EventHandler<ReturnedNodeTypesEventArgs> GetAllNodeTypesCompleted;

        public void GetNodeByIdAsync(string nodeId, string mapId)
        {
            if (_cachedNodes.ContainsKey(nodeId))
            {
                ReturnedNodesEventArgs returnedNodesEventArgs = new ReturnedNodesEventArgs();
                List<INodeProxy> returnedNodes = new List<INodeProxy>();
                returnedNodes.Add(_cachedNodes[nodeId]);
                returnedNodesEventArgs.Nodes = returnedNodes.ToArray();

                if (GetNodeByIdCompleted != null)
                {
                    GetNodeByIdCompleted.Invoke(this, returnedNodesEventArgs);
                }
            }
            else
            {
                Dictionary<string, string> nodes = new Dictionary<string, string>();
                nodes.Add(nodeId, mapId);

                _client = Client;
                _client.GetNodesAsync(nodes);
            }
        }

        public void GetRelatedNodesByIdAsync(string mapId, string nodeId, int depth)
        {
            if (_localNewNodes.ContainsKey(nodeId))
            {
                ReturnedNodesEventArgs returnedNodesEventArgs = new ReturnedNodesEventArgs();
                List<INodeProxy> returnedNodes = new List<INodeProxy>();
                returnedNodes.Add(_localNewNodes[nodeId]);
                returnedNodes.AddRange(_localNewNodes[nodeId].ParentNodes);
                returnedNodes.AddRange(_localNewNodes[nodeId].ChildNodes);
                returnedNodesEventArgs.SearchedNode = _localNewNodes[nodeId];
                returnedNodesEventArgs.Nodes = returnedNodes.ToArray();

                if (GetRelatedNodesByIdCompleted != null)
                {
                    GetRelatedNodesByIdCompleted.Invoke(this, returnedNodesEventArgs);
                }
            }
            else
            {
                _client = Client;
                _client.GetRelatedNodesAsync(mapId, nodeId, depth);
            }
        }

        public void GetAllNodeTypesAsync()
        {
            _client = Client;
            _client.GetAllNodeTypeResourcesAsync();
        }

        public void AddNode(string mapNodeId, string nodeTypeName, double x, double y)
        {
            INodeProxy nodeProxy = new NodeProxy();
            nodeProxy.Id = Guid.NewGuid().ToString();
            INodeProxy parentNode = _cachedNodes[mapNodeId];
            nodeProxy.ParentNodes = new INodeProxy[1] { parentNode };
            nodeProxy.Properties.Add("XPosition", x.ToString());
            nodeProxy.Properties.Add("YPosition", y.ToString());
            nodeProxy.NodeType = IoC.IoCContainer.GetInjectionInstance().GetInstance<NodeTypeManager>().GetNodeType(nodeTypeName);
            _localNewNodes.Add(nodeProxy.Id, nodeProxy);
            _cachedNodes.Add(nodeProxy.Id, nodeProxy);
        }

        private object _clientLock = new object();

        private MappingToolServiceClient Client
        {
            get
            {
                if (_client == null)
                {
                    lock (_clientLock)
                    {
                        if (_client == null)
                        {
                            _client = new MappingToolServiceClient();
                            _client.GetAllNodeTypeResourcesCompleted += new EventHandler<GetAllNodeTypeResourcesCompletedEventArgs>(GetAllNodeTypeResourcesCompleted);
                            _client.GetRelatedNodesCompleted += new EventHandler<GetRelatedNodesCompletedEventArgs>(GetRelatedNodesByIdAsyncCompleted);
                            _client.GetNodesCompleted += new EventHandler<GetNodesCompletedEventArgs>(GetNodeByIdAsyncCompleted);
                        }
                    }
                }
                return _client;
            }
        }

        #endregion
    }
}
