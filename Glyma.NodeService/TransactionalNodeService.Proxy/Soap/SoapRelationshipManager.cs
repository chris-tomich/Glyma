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
using ServerObjects = TransactionalNodeService.Proxy.ServerObjects;
using Service = TransactionalNodeService.Service;
using InProcess = TransactionalNodeService.InProcess;

namespace TransactionalNodeService.Soap
{
    internal class SoapRelationshipManager : Proxy.IRelationshipFactory
    {
        private Dictionary<Guid, Proxy.IRelationship> _proxyRelationships = null;
        private Dictionary<Guid, ServerObjects.Relationship> _serviceRelationships = null;
        private Dictionary<TransactionFramework.ISoapTransactionLink, FacadeRelationship> _inProcessRelationships = null;

        public SoapRelationshipManager(SoapMapManager mapManager)
        {
            MapManager = mapManager;
        }

        private SoapMapManager MapManager
        {
            get;
            set;
        }

        public IDictionary<TransactionFramework.ISoapTransactionLink, FacadeRelationship> InProcessRelationships
        {
            get
            {
                if (_inProcessRelationships == null)
                {
                    _inProcessRelationships = new Dictionary<TransactionFramework.ISoapTransactionLink, FacadeRelationship>();
                }

                return _inProcessRelationships;
            }
        }

        public IDictionary<Guid, Proxy.IRelationship> ProxyRelationships
        {
            get
            {
                if (_proxyRelationships == null)
                {
                    _proxyRelationships = new Dictionary<Guid, Proxy.IRelationship>();
                }

                return _proxyRelationships;
            }
        }

        public IDictionary<Guid, ServerObjects.Relationship> ServiceRelationships
        {
            get
            {
                if (_serviceRelationships == null)
                {
                    _serviceRelationships = new Dictionary<Guid, ServerObjects.Relationship>();
                }

                return _serviceRelationships;
            }
        }

        public Proxy.IRelationship CreateRelationship(TransactionFramework.ISoapTransactionLink transactionLink, Guid domainId, Guid rootMapId, Dictionary<Proxy.ConnectionType, Proxy.INode> nodes, Proxy.RelationshipType relationshipType, string originalId)
        {
            FacadeRelationship facadeRelationship = new FacadeRelationship();

            InProcess.InProcessRelationship relationship = new InProcess.InProcessRelationship(MapManager);
            relationship.OriginLink = transactionLink;
            relationship.Facade = facadeRelationship;
            relationship.Id = Guid.NewGuid();
            relationship.DomainId = domainId;
            relationship.RootMapId = rootMapId;
            relationship.RelationshipType = relationshipType;
            relationship.OriginalId = originalId;
            relationship.Status = Proxy.LoadState.Full;

            facadeRelationship.BaseRelationship = relationship;

            foreach (KeyValuePair<Proxy.ConnectionType, Proxy.INode> relationshipContext in nodes)
            {
                Proxy.ConnectionSet connection = Proxy.ConnectionSetFactory.Instance.GetConnection(relationshipContext.Value, facadeRelationship, relationshipContext.Key);

                Proxy.INodeManager newRelationshipNodes = relationship.Nodes;
                newRelationshipNodes.Load(connection);

                Proxy.IRelationshipManager nodeRelationships = relationshipContext.Value.Relationships;
                nodeRelationships.Load(connection);
            }

            InProcessRelationships.Add(transactionLink, facadeRelationship);

            return facadeRelationship;
        }

        public void CreateRelationship(ServerObjects.Relationship serviceRelationship)
        {
            FindRelationship(serviceRelationship);
        }

        public Proxy.IRelationship FindRelationship(Guid relationshipId)
        {
            if (ProxyRelationships.ContainsKey(relationshipId))
            {
                return ProxyRelationships[relationshipId];
            }
            else
            {
                return null;
            }
        }

        public Proxy.IRelationship FindRelationship(ServerObjects.Relationship serviceRelationship)
        {
            Proxy.IRelationship relationship;

            if (ProxyRelationships.ContainsKey(serviceRelationship.RelationshipUid))
            {
                relationship = ProxyRelationships[serviceRelationship.RelationshipUid];

                SoapRelationship soapRelationship = relationship as SoapRelationship;

                /// Not all the relationships that are stored in the RelationshipManager are SoapRelationships, some are FacadeRelationships. In this scenario we want to check if they have an inner SoapRelationship and use that instead.
                if (soapRelationship == null)
                {
                    if (relationship is FacadeRelationship)
                    {
                        FacadeRelationship facadeRelationship = relationship as FacadeRelationship;
                        soapRelationship = facadeRelationship.BaseRelationship as SoapRelationship;
                    }
                }

                if (soapRelationship != null)
                {
                    soapRelationship.UpdateRelationship(serviceRelationship);
                }
            }
            else
            {
                SoapRelationship soapRelationship = new SoapRelationship(MapManager);
                soapRelationship.UpdateRelationship(serviceRelationship);

                ProxyRelationships.Add(soapRelationship.Id, soapRelationship);

                relationship = soapRelationship;
            }

            if (!ServiceRelationships.ContainsKey(serviceRelationship.RelationshipUid))
            {
                ServiceRelationships.Add(serviceRelationship.RelationshipUid, serviceRelationship);
            }

            return relationship;
        }

        public void UpgradeFacade(TransactionFramework.ISoapTransactionLink transactionLink, ServerObjects.Relationship serviceRelationship)
        {
            if (InProcessRelationships.ContainsKey(transactionLink))
            {
                FacadeRelationship facadeRelationship = InProcessRelationships[transactionLink];
                InProcess.InProcessRelationship inProcessRelationship = facadeRelationship.BaseRelationship as InProcess.InProcessRelationship;

                if (inProcessRelationship != null)
                {
                    SoapRelationship soapRelationship = new SoapRelationship(inProcessRelationship, serviceRelationship);

                    facadeRelationship.BaseRelationship = soapRelationship;

                    InProcessRelationships.Remove(transactionLink);
                    ProxyRelationships[facadeRelationship.Id] = facadeRelationship;

                    /// TODO: Need to consider a better way to do this. I don't like that there is a need to call this afterwards and maybe it should be done when creating the SoapRelationship. I don't like it because it doesn't have to be done everytime a new SoapRelationship is created e.g. if the SoapNode is created from a Service.RE as opposed to a returned call like here.
                    soapRelationship.ProcessDelayedActions();
                }
            }
        }

        public void DeleteRelationship(ServerObjects.Relationship serviceRelationship)
        {
            DeleteRelationship(serviceRelationship.RelationshipUid);
        }

        public void DeleteRelationship(Proxy.IRelationship proxyRelationship)
        {
            DeleteRelationship(proxyRelationship.Id);
        }

        private void DeleteRelationship(Guid relationshipUid)
        {
            if (ProxyRelationships.ContainsKey(relationshipUid))
            {
                Proxy.IRelationship relationship = ProxyRelationships[relationshipUid];
                IEnumerable<Proxy.NodeTuple> nodeTuples = relationship.Nodes.FindNodes();

                foreach (Proxy.NodeTuple nodeTuple in nodeTuples)
                {
                    Proxy.IRelationshipManager relationships = nodeTuple.Node.Relationships;
                    relationships.Remove(relationship);
                }

                Proxy.INodeManager nodes = relationship.Nodes;
                nodes.Clear();

                ProxyRelationships.Remove(relationshipUid);
            }

            if (ServiceRelationships.ContainsKey(relationshipUid))
            {
                ServiceRelationships.Remove(relationshipUid);
            }
        }
    }
}
