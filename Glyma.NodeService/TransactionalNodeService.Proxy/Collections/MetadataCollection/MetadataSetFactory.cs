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
using System.Linq;
using ServerObjects = TransactionalNodeService.Proxy.ServerObjects;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace TransactionalNodeService.Proxy
{
    internal class MetadataSetFactory
    {
        private static Dictionary<IMapManager, MetadataSetFactory> instances = null;
        private static readonly object padlock = new object();

        private Dictionary<Guid, IMetadataSet> _metadata = null;
        private Dictionary<TransactionFramework.ISoapTransactionLink, FacadeMetadataSet> _inProcessMetadata = null;

        private MetadataSetFactory()
        {
        }

        private MetadataSetFactory(IMapManager mapManager)
        {
            MapManager = mapManager;
        }

        public static MetadataSetFactory GetInstance(IMapManager mapManager)
        {
            lock (padlock)
            {
                if (instances == null)
                {
                    instances = new Dictionary<IMapManager, MetadataSetFactory>();
                }

                if (!instances.ContainsKey(mapManager))
                {
                    instances[mapManager] = new MetadataSetFactory(mapManager);
                }

                return instances[mapManager];
            }
        }

        private IMapManager MapManager
        {
            get;
            set;
        }

        private Dictionary<TransactionFramework.ISoapTransactionLink, FacadeMetadataSet> InProcessMetadata
        {
            get
            {
                _inProcessMetadata = _inProcessMetadata ?? new Dictionary<TransactionFramework.ISoapTransactionLink, FacadeMetadataSet>();

                return _inProcessMetadata;
            }
        }

        private Dictionary<Guid, IMetadataSet> Metadata
        {
            get
            {
                _metadata = _metadata ?? new Dictionary<Guid, IMetadataSet>();

                return _metadata;
            }
        }

        //public IMetadataSet GetMetadata(Guid metadataId)
        //{
        //    if (Metadata.ContainsKey(metadataId))
        //    {
        //        return Metadata[metadataId];
        //    }

        //    return null;
        //}

        public IMetadataSet GetMetadata(ServerObjects.Metadata serviceMetadata, INode node, IRelationship relationship)
        {
            IMetadataSet metadataSet = null;

            if (Metadata.ContainsKey(serviceMetadata.MetadataId))
            {
                metadataSet = Metadata[serviceMetadata.MetadataId];

                if (metadataSet is IUpdatableMetadataSet)
                {
                    IUpdatableMetadataSet updatableMetadataSet = metadataSet as IUpdatableMetadataSet;

                    updatableMetadataSet.UpdateMetadataSet(serviceMetadata);
                }
            }
            else
            {
                metadataSet = new Soap.SoapMetadataSet(MapManager, serviceMetadata, node, relationship);

                Metadata[metadataSet.Id] = metadataSet;
            }

            return metadataSet;
        }

        public IMetadataSet GetMetadata(TransactionFramework.ISoapTransactionLink link, Guid domainId, Guid rootMapId, string name, string value, INode node, IRelationship relationship, ConnectionType connectionType)
        {
            /// Return a FacadeNode containing an InProcessMetadataSet
            InProcess.InProcessMetadata newMetadataSet = new InProcess.InProcessMetadata(MapManager);
            newMetadataSet.OriginLink = link;
            newMetadataSet.Id = Guid.Empty;
            newMetadataSet.DomainId = domainId;
            newMetadataSet.RootMapId = rootMapId;
            newMetadataSet.Name = name;
            newMetadataSet.Value = value;
            newMetadataSet.Node = node;
            newMetadataSet.Relationship = relationship;
            newMetadataSet.ConnectionType = connectionType;

            FacadeMetadataSet facadeMetadataSet = new FacadeMetadataSet();
            facadeMetadataSet.BaseMetadata = newMetadataSet;

            newMetadataSet.Facade = facadeMetadataSet;
            InProcessMetadata[link] = facadeMetadataSet;

            return facadeMetadataSet;
        }

        //public IMetadataSet GetMetadata(Guid metadataId, string name, string value, INode node, IRelationship relationship, ConnectionType connectionType)
        //{
        //    Soap.SoapMetadataSet newMetadatSet = new Soap.SoapMetadataSet(MapManager);
        //    newMetadatSet.Id = metadataId;
        //    newMetadatSet.Name = name;
        //    newMetadatSet.Value = value;
        //    newMetadatSet.Node = node;
        //    newMetadatSet.Relationship = relationship;
        //    newMetadatSet.ConnectionType = connectionType;

        //    Metadata[metadataId] = newMetadatSet;

        //    return newMetadatSet;
        //}

        public void UpgradeFacade(TransactionFramework.ISoapTransactionLink link, ServerObjects.Metadata serviceMetadata)
        {
            if (!InProcessMetadata.ContainsKey(link))
            {
                return;
            }

            FacadeMetadataSet facadeMetadataSet = InProcessMetadata[link];
            InProcess.InProcessMetadata inProcessMetadata = facadeMetadataSet.BaseMetadata as InProcess.InProcessMetadata;

            if (inProcessMetadata == null)
            {
                return;
            }

            Soap.SoapMetadataSet soapMetadataSet = new Soap.SoapMetadataSet(inProcessMetadata, serviceMetadata);

            IMetadataSetManager soapMetadataSetManager = soapMetadataSet as IMetadataSetManager;
            IMetadataSetManager inProcessMetadataSetManager = inProcessMetadata as IMetadataSetManager;

            if (soapMetadataSetManager != null && inProcessMetadataSetManager != null)
            {
                soapMetadataSetManager.Container = inProcessMetadataSetManager.Container;
            }

            facadeMetadataSet.BaseMetadata = soapMetadataSet;
            Metadata[soapMetadataSet.Id] = soapMetadataSet;
            InProcessMetadata.Remove(link);
        }

        public void Remove(IMetadataSet metadataSet)
        {
            List<TransactionFramework.ISoapTransactionLink> linksToRemove = new List<TransactionFramework.ISoapTransactionLink>();

            foreach (KeyValuePair<TransactionFramework.ISoapTransactionLink, FacadeMetadataSet> metadataSetPair in InProcessMetadata)
            {
                if (metadataSetPair.Value == metadataSet)
                {
                    linksToRemove.Add(metadataSetPair.Key);
                }
            }

            foreach (TransactionFramework.ISoapTransactionLink keyToRemove in linksToRemove)
            {
                InProcessMetadata.Remove(keyToRemove);
            }

            List<Guid> idsToRemove = new List<Guid>();

            foreach (KeyValuePair<Guid, IMetadataSet> metadataSetPair in Metadata)
            {
                if (metadataSetPair.Value == metadataSet)
                {
                    idsToRemove.Add(metadataSetPair.Key);
                }
            }

            foreach (Guid keyToRemove in idsToRemove)
            {
                Metadata.Remove(keyToRemove);
            }
        }
    }
}
