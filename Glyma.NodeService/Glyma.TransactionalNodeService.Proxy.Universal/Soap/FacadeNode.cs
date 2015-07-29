using System;
using TransactionalNodeService.Proxy.Universal.Collections.MetadataCollection;
using TransactionalNodeService.Proxy.Universal.Collections.RelationshipContainers;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.Soap
{
    public class FacadeNode : INode, IFacade
    {
        private INode _baseNode = null;

        public event EventHandler BaseCured;

        public FacadeNode()
        {
            ClientId = Guid.NewGuid();
        }

        private INode PreviousBaseNode
        {
            get;
            set;
        }

        public bool IsConcrete
        {
            get;
            private set;
        }

        public INode BaseNode
        {
            get
            {
                return _baseNode;
            }
            set
            {
                PreviousBaseNode = _baseNode;
                _baseNode = value;

                if (_baseNode is SoapNode)
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
                return BaseNode.Id;
            }
            set
            {
                BaseNode.Id = value;
            }
        }

        public Guid DomainId
        {
            get
            {
                return BaseNode.DomainId;
            }
            set
            {
                BaseNode.DomainId = value;
            }
        }

        public Guid? RootMapId
        {
            get
            {
                return BaseNode.RootMapId;
            }
            set
            {
                BaseNode.RootMapId = value;
            }
        }

        public string OriginalId
        {
            get
            {
                return BaseNode.OriginalId;
            }
            set
            {
                BaseNode.OriginalId = value;
            }
        }

        public LoadState Status
        {
            get
            {
                return BaseNode.Status;
            }
            set
            {
                BaseNode.Status = value;
            }
        }

        public NodeType NodeType
        {
            get
            {
                return BaseNode.NodeType;
            }
            set
            {
                BaseNode.NodeType = value;
            }
        }

        public IMapManager MapManager
        {
            get
            {
                return BaseNode.MapManager;
            }
        }

        public RelationshipSet Relationships
        {
            get
            {
                return BaseNode.Relationships;
            }
        }

        public NodeMetadataCollection Metadata
        {
            get
            {
                return BaseNode.Metadata;
            }
        }

        public void ResetToFacade()
        {
            if (!(_baseNode is InProcess.InProcessNode))
            {
                _baseNode = PreviousBaseNode;
                PreviousBaseNode = null;

                IsConcrete = false;
            }
        }

        public void Update(NodeType nodeType, ref TransactionFramework.TransactionChain chain)
        {
            BaseNode.Update(nodeType, ref chain);
        }

        public void Delete(ref TransactionFramework.TransactionChain chain)
        {
            BaseNode.Delete(ref chain);
        }
    }
}
