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
using System.Collections;
using System.Collections.Generic;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace TransactionalNodeService.Proxy
{
    public class MetadataCollection : IEnumerable<IMetadataSet>
    {
        private List<IMetadataSet> _metadata = null;
        private Proxy.MetadataType _stringType = null;

        #region Common Super Graph Types
        protected Proxy.MetadataType StringMetadataType
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

        protected Proxy.IMapManager MapManager
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

        protected TransactionFramework.AddMetadataTransactionLink AddMetadataTransaction(IMetadataSet metadataSet, Proxy.INode node, Proxy.IRelationship relationship, Proxy.ConnectionType connectionType, string name, string value)
        {
            TransactionFramework.AddMetadataTransactionLink addMetadataTransaction = new TransactionFramework.AddMetadataTransactionLink();
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
