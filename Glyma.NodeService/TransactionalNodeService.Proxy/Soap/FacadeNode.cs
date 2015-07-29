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
    public class FacadeNode : Proxy.INode, Proxy.IFacade
    {
        private Proxy.INode _baseNode = null;

        public event EventHandler BaseCured;

        public FacadeNode()
        {
            ClientId = Guid.NewGuid();
        }

        private Proxy.INode PreviousBaseNode
        {
            get;
            set;
        }

        public bool IsConcrete
        {
            get;
            private set;
        }

        public Proxy.INode BaseNode
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

        public Proxy.LoadState Status
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

        public Proxy.NodeType NodeType
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

        public Proxy.IMapManager MapManager
        {
            get
            {
                return BaseNode.MapManager;
            }
        }

        public Proxy.RelationshipSet Relationships
        {
            get
            {
                return BaseNode.Relationships;
            }
        }

        public Proxy.NodeMetadataCollection Metadata
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

        public void Update(Proxy.NodeType nodeType, ref TransactionFramework.TransactionChain chain)
        {
            BaseNode.Update(nodeType, ref chain);
        }

        public void Delete(ref TransactionFramework.TransactionChain chain)
        {
            BaseNode.Delete(ref chain);
        }
    }
}
