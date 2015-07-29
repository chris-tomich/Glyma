using System;
using TransactionalNodeService.Proxy.Universal.Collections.MetadataCollection;
using TransactionalNodeService.Proxy.Universal.Collections.NodeContainers;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.Soap
{
    public class FacadeRelationship : IRelationship, IFacade
    {
        private IRelationship _baseRelationship = null;

        public event EventHandler BaseCured;

        public FacadeRelationship()
        {
            ClientId = Guid.NewGuid();
        }

        private IRelationship PreviousBaseRelationship
        {
            get;
            set;
        }

        public bool IsConcrete
        {
            get;
            private set;
        }

        public IRelationship BaseRelationship
        {
            get
            {
                return _baseRelationship;
            }
            set
            {
                PreviousBaseRelationship = _baseRelationship;
                _baseRelationship = value;

                if (_baseRelationship is SoapRelationship)
                {
                    IsConcrete = true;

                    if (BaseCured != null)
                    {
                        BaseCured(this, new System.EventArgs());
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

        public Guid ClientId
        {
            get;
            set;
        }

        public Guid Id
        {
            get
            {
                return BaseRelationship.Id;
            }
            set
            {
                BaseRelationship.Id = value;
            }
        }

        public Guid DomainId
        {
            get
            {
                return BaseRelationship.DomainId;
            }
            set
            {
                BaseRelationship.DomainId = value;
            }
        }

        public Guid? RootMapId
        {
            get
            {
                return BaseRelationship.RootMapId;
            }
            set
            {
                BaseRelationship.RootMapId = value;
            }
        }

        public string OriginalId
        {
            get
            {
                return BaseRelationship.OriginalId;
            }
            set
            {
                BaseRelationship.OriginalId = value;
            }
        }

        public LoadState Status
        {
            get
            {
                return BaseRelationship.Status;
            }
            set
            {
                BaseRelationship.Status = value;
            }
        }

        public RelationshipType RelationshipType
        {
            get
            {
                return BaseRelationship.RelationshipType;
            }
            set
            {
                BaseRelationship.RelationshipType = value;
            }
        }

        public IMapManager MapManager
        {
            get
            {
                return BaseRelationship.MapManager;
            }
        }

        public NodeSet Nodes
        {
            get
            {
                return BaseRelationship.Nodes;
            }
        }

        public RelationshipMetadataCollection Metadata
        {
            get
            {
                return BaseRelationship.Metadata;
            }
        }

        public void ResetToFacade()
        {
            if (!(_baseRelationship is InProcess.InProcessRelationship))
            {
                _baseRelationship = PreviousBaseRelationship;
                PreviousBaseRelationship = null;

                IsConcrete = false;
            }
        }

        public void ConnectNode(ConnectionType connectionType, INode node, ref TransactionFramework.TransactionChain chain)
        {
            BaseRelationship.ConnectNode(connectionType, node, ref chain);
        }

        public void Update(RelationshipType relationshipType, ref TransactionFramework.TransactionChain chain)
        {
            BaseRelationship.Update(relationshipType, ref chain);
        }

        public void Delete(ref TransactionFramework.TransactionChain chain)
        {
            BaseRelationship.Delete(ref chain);
        }
    }
}
