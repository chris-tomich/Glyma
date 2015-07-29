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
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace TransactionalNodeService.Proxy
{
    public class FacadeMetadataSet : IMetadataSet, IFacade, IMetadataSetManager, IUpdatableMetadataSet
    {
        private Proxy.MetadataSet _baseMetadata = null;

        public event EventHandler BaseCured;

        public FacadeMetadataSet()
        {
        }

        private Proxy.MetadataSet PreviousBaseMetadata
        {
            get;
            set;
        }

        public bool IsConcrete
        {
            get;
            private set;
        }

        public Proxy.MetadataSet BaseMetadata
        {
            get
            {
                return _baseMetadata;
            }
            set
            {
                PreviousBaseMetadata = _baseMetadata;
                _baseMetadata = value;

                if (_baseMetadata is Soap.SoapMetadataSet)
                {
                    IsConcrete = true;

                    if (BaseCured != null)
                    {
                        BaseCured(this, new EventArgs());
                    }
                }
                else
                {
                    IsConcrete = false;
                }
            }
        }

        public TransactionFramework.ISoapTransactionLink TransactionOrigin
        {
            get;
            set;
        }

        public Guid Id
        {
            get
            {
                return BaseMetadata.Id;
            }
            set
            {
                BaseMetadata.Id = value;
            }
        }

        public Guid DomainId
        {
            get
            {
                return BaseMetadata.DomainId;
            }
            set
            {
                BaseMetadata.DomainId = value;
            }
        }

        public Guid? RootMapId
        {
            get
            {
                return BaseMetadata.RootMapId;
            }
            set
            {
                BaseMetadata.RootMapId = value;
            }
        }

        public string Name
        {
            get
            {
                return BaseMetadata.Name;
            }
            set
            {
                BaseMetadata.Name = value;
            }
        }

        public string Value
        {
            get
            {
                return BaseMetadata.Value;
            }
            set
            {
                BaseMetadata.Value = value;
            }
        }

        public INode Node
        {
            get
            {
                return BaseMetadata.Node;
            }
            set
            {
                BaseMetadata.Node = value;
            }
        }

        public IRelationship Relationship
        {
            get
            {
                return BaseMetadata.Relationship;
            }
            set
            {
                BaseMetadata.Relationship = value;
            }
        }

        public ConnectionType ConnectionType
        {
            get
            {
                return BaseMetadata.ConnectionType;
            }
            set
            {
                BaseMetadata.ConnectionType = value;
            }
        }

        public override bool Equals(object obj)
        {
            IMetadataSet comparisonObj = obj as IMetadataSet;

            if (comparisonObj != null)
            {
                return comparisonObj.Id.Equals(Id);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as IMetadataSet);
        }

        public int CompareTo(IMetadataSet other)
        {
            if (other == null)
            {
                return 1;
            }

            int idHashCode = Id.GetHashCode();
            int otherHashCode = other.GetHashCode();

            if (idHashCode > otherHashCode)
            {
                return 1;
            }
            else if (idHashCode == otherHashCode)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        public void ResetToFacade()
        {
            if (!(_baseMetadata is InProcess.InProcessMetadata))
            {
                _baseMetadata = PreviousBaseMetadata;
                PreviousBaseMetadata = null;

                IsConcrete = false;
            }
        }

        public void Update(string name, string value, Proxy.INode node, Proxy.IRelationship relationship, Proxy.ConnectionType connectionType, ref TransactionFramework.TransactionChain chain)
        {
            BaseMetadata.Update(name, value, node, relationship, connectionType, ref chain);
        }

        public void Delete(ref TransactionFramework.TransactionChain chain)
        {
            BaseMetadata.Delete(ref chain);
        }

        MetadataCollection IMetadataSetManager.Container
        {
            get
            {
                IMetadataSetManager metadataSetManager = BaseMetadata as IMetadataSetManager;

                if (metadataSetManager != null)
                {
                    return metadataSetManager.Container;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                IMetadataSetManager metadataSetManager = BaseMetadata as IMetadataSetManager;

                if (metadataSetManager != null)
                {
                    metadataSetManager.Container = value;
                }
            }
        }

        void IUpdatableMetadataSet.UpdateMetadataSet(ServerObjects.Metadata metadataSet)
        {
            if (BaseMetadata is IUpdatableMetadataSet)
            {
                IUpdatableMetadataSet updatableMetadataSet = BaseMetadata as IUpdatableMetadataSet;

                updatableMetadataSet.UpdateMetadataSet(metadataSet);
            }
        }
    }
}
