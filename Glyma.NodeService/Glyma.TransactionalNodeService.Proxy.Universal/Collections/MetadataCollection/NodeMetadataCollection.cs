using System;
using System.Collections.Generic;
using TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.Collections.MetadataCollection
{
    public class NodeMetadataCollection : MetadataCollection
    {
        private HashSet<Guid> _containedMetadata = null;

        public NodeMetadataCollection(INode parent, IMapManager mapManager)
        {
            Parent = parent;
            MapManager = mapManager;
        }

        private INode Parent
        {
            get;
            set;
        }

        /// <summary>
        /// This holds the IDs of all the server MetadataSet's contained within this collection. This helps with improving performance as we don't need to use the FindMetadata function.
        /// TODO: Might need to consider
        /// </summary>
        private HashSet<Guid> ContainedMetadata
        {
            get
            {
                if (_containedMetadata == null)
                {
                    _containedMetadata = new HashSet<Guid>();
                }

                return _containedMetadata;
            }
        }

        private bool AreRelationshipsEqual(IRelationship relationship, IMetadataSet metadataSet)
        {
            if (metadataSet == null)
            {
                return false;
            }

            if (relationship == null && metadataSet.Relationship == null)
            {
                return true;
            }

            if (relationship != null && metadataSet.Relationship != null && relationship.Id == metadataSet.Relationship.Id)
            {
                return true;
            }

            return false;
        }

        private bool AreConnectionTypesEqual(ConnectionType connectionType, IMetadataSet metadataSet)
        {
            if (metadataSet == null)
            {
                return false;
            }

            if (connectionType == null && metadataSet.ConnectionType == null)
            {
                return true;
            }

            if (connectionType != null && metadataSet.ConnectionType != null && connectionType.Id == metadataSet.ConnectionType.Id)
            {
                return true;
            }

            return false;
        }

        public IMetadataSet Load(ServerObjects.Metadata serviceMetadata, IRelationship relationship)
        {
            IMetadataSet metadataSet = MetadataSetFactory.GetInstance(MapManager).GetMetadata(serviceMetadata, Parent, relationship);

            if (!ContainedMetadata.Contains(metadataSet.Id))
            {
                Metadata.Add(metadataSet);
                ContainedMetadata.Add(metadataSet.Id);
            }

            IMetadataSetManager metadataSetManager = metadataSet as IMetadataSetManager;

            if (metadataSetManager != null)
            {
                metadataSetManager.Container = this;
            }

            return metadataSet;
        }

        public IMetadataSet Add(IRelationship relationship, ConnectionType connectionType, string name, string value, ref TransactionChain chain)
        {
            IMetadataSet metadataSet = null;

            if (connectionType != null && relationship != null)
            {
                foreach (IMetadataSet metadata in Metadata)
                {
                    if (metadata.Name == name && metadata.Relationship.Id == relationship.Id && metadata.ConnectionType.Id == connectionType.Id)
                    {
                        metadataSet = metadata;
                        break;
                    }
                }
            }
            else if (connectionType == null && relationship != null)
            {
                foreach (IMetadataSet metadata in Metadata)
                {
                    if (metadata.Name == name && metadata.Relationship.Id == relationship.Id)
                    {
                        metadataSet = metadata;
                        break;
                    }
                }
            }
            else
            {
                foreach (IMetadataSet metadata in Metadata)
                {
                    if (metadata.Name == name)
                    {
                        metadataSet = metadata;
                        break;
                    }
                }
            }

            if (metadataSet != null)
            {
                //TransactionFramework.UpdateMetadataTransactionLink updateMetadataTransaction = UpdateMetadataTransaction(metadataSet, Parent, name, value);
                metadataSet.Update(null, value, null, null, null, ref chain);
            }
            else
            {
                AddMetadataTransactionLink addMetadataTransaction = AddMetadataTransaction(metadataSet, Parent, relationship, connectionType, name, value);

                metadataSet = MetadataSetFactory.GetInstance(MapManager).GetMetadata(addMetadataTransaction, addMetadataTransaction.DomainId, addMetadataTransaction.RootMapId.Value, name, value, Parent, relationship, connectionType);

                chain.AddTransaction(addMetadataTransaction);

                Metadata.Add(metadataSet);

                IMetadataSetManager metadataSetManager = metadataSet as IMetadataSetManager;

                if (metadataSetManager != null)
                {
                    metadataSetManager.Container = this;
                }
            }

            return metadataSet;
        }

        public IMetadataSet FindMetadata(string name)
        {
            foreach (IMetadataSet metadataTriple in Metadata)
            {
                if (metadataTriple.Name == name)
                {
                    return metadataTriple;
                }
            }

            return null;
        }

        public IMetadataSet FindMetadata(IRelationship relationship, string name)
        {
            foreach (IMetadataSet metadataTriple in Metadata)
            {
                if (relationship != null)
                {
                    if (metadataTriple.Name == name && metadataTriple.Relationship.Id == relationship.Id)
                    {
                        return metadataTriple;
                    }
                }
                else
                {
                    return FindMetadata(name);
                }
            }

            return null;
        }

        public IMetadataSet FindMetadata(IRelationship relationship, ConnectionType connectionType, string name)
        {
            foreach (IMetadataSet metadataTriple in Metadata)
            {
                if (connectionType != null)
                {
                    if (metadataTriple.Name == name && metadataTriple.Relationship.Id == relationship.Id && metadataTriple.ConnectionType.Id == connectionType.Id)
                    {
                        return metadataTriple;
                    }
                }
                else
                {
                    return FindMetadata(relationship, name);
                }
            }

            return null;
        }

        public IDictionary<string, IMetadataSet> FindMetadata()
        {
            Dictionary<string, IMetadataSet> metadata = new Dictionary<string,IMetadataSet>();

            foreach (IMetadataSet metadataTriple in Metadata)
            {
                metadata[metadataTriple.Name] = metadataTriple;
            }

            return metadata;
        }

        public IDictionary<string, IMetadataSet> FindMetadata(IRelationship relationship)
        {
            Dictionary<string, IMetadataSet> metadata = new Dictionary<string, IMetadataSet>();

            foreach (IMetadataSet metadataTriple in Metadata)
            {
                if (AreRelationshipsEqual(relationship, metadataTriple))
                {
                    metadata[metadataTriple.Name] = metadataTriple;
                }
            }

            return metadata;
        }

        public IDictionary<string, IMetadataSet> FindMetadata(IRelationship relationship, ConnectionType connectionType)
        {
            Dictionary<string, IMetadataSet> metadata = new Dictionary<string, IMetadataSet>();

            foreach (IMetadataSet metadataTriple in Metadata)
            {
                if (AreRelationshipsEqual(relationship, metadataTriple) && AreConnectionTypesEqual(connectionType, metadataTriple))
                {
                    metadata[metadataTriple.Name] = metadataTriple;
                }
            }

            return metadata;
        }
    }
}
