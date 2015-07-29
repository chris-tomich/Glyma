using System;
using System.Collections.Generic;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.Soap
{
    internal class SoapNodeManager : INodeFactory
    {
        private Dictionary<Guid, INode> _proxyNodes = null;
        private Dictionary<Guid, ServerObjects.Node> _serviceNodes = null;
        private Dictionary<TransactionFramework.ISoapTransactionLink, FacadeNode> _inProcessNodes = null;

        public SoapNodeManager(SoapMapManager mapManager)
        {
            MapManager = mapManager;
        }

        private SoapMapManager MapManager
        {
            get;
            set;
        }

        private IDictionary<TransactionFramework.ISoapTransactionLink, FacadeNode> InProcessNodes
        {
            get
            {
                if (_inProcessNodes == null)
                {
                    _inProcessNodes = new Dictionary<TransactionFramework.ISoapTransactionLink, FacadeNode>();
                }

                return _inProcessNodes;
            }
        }

        private IDictionary<Guid, INode> ProxyNodes
        {
            get
            {
                if (_proxyNodes == null)
                {
                    _proxyNodes = new Dictionary<Guid, INode>();
                }

                return _proxyNodes;
            }
        }

        private IDictionary<Guid, ServerObjects.Node> ServiceNodes
        {
            get
            {
                if (_serviceNodes == null)
                {
                    _serviceNodes = new Dictionary<Guid, ServerObjects.Node>();
                }

                return _serviceNodes;
            }
        }

        public INode CreateNode(TransactionFramework.ISoapTransactionLink transactionLink, Guid domainId, Guid rootMapId, NodeType nodeType, string originalId)
        {
            FacadeNode facadeNode = new FacadeNode();

            InProcess.InProcessNode node = new InProcess.InProcessNode(MapManager);
            node.OriginLink = transactionLink;
            node.Facade = facadeNode;
            node.Id = Guid.NewGuid();
            node.DomainId = domainId;
            node.RootMapId = rootMapId;
            node.NodeType = nodeType;
            node.OriginalId = originalId;
            node.Status = LoadState.Full;

            facadeNode.BaseNode = node;

            InProcessNodes.Add(transactionLink, facadeNode);

            return facadeNode;
        }

        public void CreateNode(Guid nodeId)
        {
            FindNode(nodeId);
        }

        public INode FindNode(Guid nodeId)
        {
            INode proxyNode;

            if (ProxyNodes.ContainsKey(nodeId))
            {
                proxyNode = ProxyNodes[nodeId];
            }
            else
            {
                SoapNode soapNode = new SoapNode(MapManager);
                soapNode.PreInitialiseNode(nodeId);

                ProxyNodes.Add(soapNode.Id, soapNode);
                proxyNode = soapNode;
            }

            return proxyNode;
        }

        public void CreateNode(ServerObjects.Node serviceNode)
        {
            FindNode(serviceNode);
        }

        public INode FindNode(ServerObjects.Node serviceNode)
        {
            INode node;

            if (ProxyNodes.ContainsKey(serviceNode.NodeUid))
            {
                node = ProxyNodes[serviceNode.NodeUid];

                SoapNode soapNode = node as SoapNode;

                /// Not all the nodes that are stored in the NodeManager are SoapNodes, some are FacadeNodes. In this scenario we want to check if they have an inner SoapNode and use that instead.
                if (soapNode == null)
                {
                    if (node is FacadeNode)
                    {
                        FacadeNode facadeNode = node as FacadeNode;
                        soapNode = facadeNode.BaseNode as SoapNode;
                    }
                }

                if (soapNode != null)
                {
                    soapNode.UpdateNode(serviceNode);
                }
            }
            else
            {
                SoapNode soapNode = new SoapNode(MapManager);
                soapNode.UpdateNode(serviceNode);

                ProxyNodes.Add(soapNode.Id, soapNode);

                node = soapNode;
            }

            if (!ServiceNodes.ContainsKey(serviceNode.NodeUid))
            {
                ServiceNodes.Add(serviceNode.NodeUid, serviceNode);
            }

            return node;
        }

        public void UpgradeFacade(TransactionFramework.ISoapTransactionLink transactionLink, ServerObjects.Node serviceNode)
        {
            if (InProcessNodes.ContainsKey(transactionLink))
            {
                FacadeNode facadeNode = InProcessNodes[transactionLink];
                InProcess.InProcessNode inProcessNode = facadeNode.BaseNode as InProcess.InProcessNode;

                if (inProcessNode != null)
                {
                    SoapNode soapNode = new SoapNode(inProcessNode, serviceNode);

                    facadeNode.BaseNode = soapNode;

                    InProcessNodes.Remove(transactionLink);
                    ProxyNodes[facadeNode.Id] = facadeNode;

                    /// TODO: Need to consider a better way to do this. I don't like that there is a need to call this afterwards and maybe it should be done when creating the SoapNode. I don't like it because it doesn't have to be done everytime a new SoapNode is created e.g. if the SoapNode is created from a Service.NO as opposed to a returned call like here.
                    soapNode.ProcessDelayedActions();
                }
            }
        }

        public void DeleteNode(ServerObjects.Node serviceNode)
        {
            DeleteNode(serviceNode.NodeUid);
        }

        public void DeleteNode(INode proxyNode)
        {
            DeleteNode(proxyNode.Id);
        }

        private void DeleteNode(Guid nodeUid)
        {
            if (ProxyNodes.ContainsKey(nodeUid))
            {
                ProxyNodes.Remove(nodeUid);
            }

            if (ServiceNodes.ContainsKey(nodeUid))
            {
                ServiceNodes.Remove(nodeUid);
            }
        }
    }
}
