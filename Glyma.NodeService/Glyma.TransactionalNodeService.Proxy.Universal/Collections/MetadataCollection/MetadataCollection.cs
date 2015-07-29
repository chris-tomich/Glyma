using System.Collections;
using System.Collections.Generic;
using TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.Collections.MetadataCollection
{
    public class MetadataCollection : IEnumerable<IMetadataSet>
    {
        private List<IMetadataSet> _metadata = null;
        private MetadataType _stringType = null;

        #region Common Super Graph Types
        protected MetadataType StringMetadataType
        {
            get
            {
                if (_stringType == null)
                {
                    _stringType = MapManager.MetadataTypes["string"];
                }

                return _stringType;
            }
        }
        #endregion

        protected List<IMetadataSet> Metadata
        {
            get
            {
                if (_metadata == null)
                {
                    _metadata = new List<IMetadataSet>();
                }

                return _metadata;
            }
            set
            {
                _metadata = value;
            }
        }

        protected IMapManager MapManager
        {
            get;
            set;
        }

        public void Clear()
        {
            /// Shouldn't clear all the metadata as some of it may have only just been added.
            /// Only remove the SoapMetadata objects. The InPocessMetadata needs to remain.
            //Metadata.Clear();

            Metadata.RemoveAll(RemoveMetadataSetPredicate);
        }

        private bool RemoveMetadataSetPredicate(IMetadataSet metadataSet)
        {
            FacadeMetadataSet facadeMetadataSet = metadataSet as FacadeMetadataSet;

            if (facadeMetadataSet != null)
            {
                if (facadeMetadataSet.BaseMetadata is Soap.SoapMetadataSet)
                {
                    return true;
                }
            }

            return false;
        }

        public void Remove(IMetadataSet metadataSet)
        {
            Metadata.Remove(metadataSet);
        }

        protected AddMetadataTransactionLink AddMetadataTransaction(IMetadataSet metadataSet, INode node, IRelationship relationship, ConnectionType connectionType, string name, string value)
        {
            AddMetadataTransactionLink addMetadataTransaction = new AddMetadataTransactionLink();
            addMetadataTransaction.DomainId = node.DomainId;
            addMetadataTransaction.RootMapId = node.RootMapId;
            addMetadataTransaction.MapManager = MapManager;
            addMetadataTransaction.MetadataSet = metadataSet;
            addMetadataTransaction.Node = node;
            addMetadataTransaction.Relationship = relationship;
            addMetadataTransaction.ConnectionType = connectionType;
            addMetadataTransaction.MetadataType = StringMetadataType;
            addMetadataTransaction.Name = name;
            addMetadataTransaction.Value = value;

            return addMetadataTransaction;
        }

        IEnumerator<IMetadataSet> IEnumerable<IMetadataSet>.GetEnumerator()
        {
            return Metadata.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Metadata.GetEnumerator();
        }
    }
}
