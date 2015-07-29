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

namespace TransactionalNodeService.Soap
{
    public class FacadeRelationship : Proxy.IRelationship, Proxy.IFacade
    {
        private Proxy.IRelationship _baseRelationship = null;

        public event EventHandler BaseCured;

        public FacadeRelationship()
        {
            ClientId = Guid.NewGuid();
        }

        private Proxy.IRelationship PreviousBaseRelationship
        {
            get;
            set;
        }

        public bool IsConcrete
        {
            get;
            private set;
        }

        public Proxy.IRelationship BaseRelationship
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

        public Proxy.LoadState Status
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

        public Proxy.RelationshipType RelationshipType
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

        public Proxy.IMapManager MapManager
        {
            get
            {
                return BaseRelationship.MapManager;
            }
        }

        public Proxy.NodeSet Nodes
        {
            get
            {
                return BaseRelationship.Nodes;
            }
        }

        public Proxy.RelationshipMetadataCollection Metadata
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

        public void ConnectNode(Proxy.ConnectionType connectionType, Proxy.INode node, ref TransactionFramework.TransactionChain chain)
        {
            BaseRelationship.ConnectNode(connectionType, node, ref chain);
        }

        public void Update(Proxy.RelationshipType relationshipType, ref TransactionFramework.TransactionChain chain)
        {
            BaseRelationship.Update(relationshipType, ref chain);
        }

        public void Delete(ref TransactionFramework.TransactionChain chain)
        {
            BaseRelationship.Delete(ref chain);
        }
    }
}
