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
using Proxy = TransactionalNodeService.Proxy;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;
using System.Collections.Generic;

namespace TransactionalNodeService.InProcess
{
    internal class InProcessRelationship : Proxy.Relationship, IInProcess
    {
        private DelayedRelationshipActions _delayedActions = null;

        protected InProcessRelationship()
        {
        }

        public InProcessRelationship(Proxy.IMapManager mapManager)
            : base(mapManager)
        {
        }

        public Soap.FacadeRelationship Facade
        {
            get;
            set;
        }

        public override Proxy.RelationshipMetadataCollection Metadata
        {
            get
            {
                if (Status == Proxy.LoadState.None)
                {
                    return null;
                }

                if (_metadataCollection == null)
                {
                    _metadataCollection = new Proxy.RelationshipMetadataCollection(Facade);
                }

                return _metadataCollection;
            }
            protected set
            {
                _metadataCollection = value;
            }
        }

        public DelayedRelationshipActions DelayedActions
        {
            get
            {
                _delayedActions = _delayedActions ?? new DelayedRelationshipActions(this);

                return _delayedActions;
            }
        }

        public override void ConnectNode(Proxy.ConnectionType connectionType, Proxy.INode node, ref Soap.TransactionFramework.TransactionChain chain)
        {
            base.ConnectNode(connectionType, node, ref chain);

            if (Facade != null)
            {
                TransactionFramework.AddRelationshipTransactionLink addRelationshipTransaction = Facade.TransactionOrigin as TransactionFramework.AddRelationshipTransactionLink;

                if (addRelationshipTransaction != null)
                {
                    if (addRelationshipTransaction.TransactionStatus == TransactionFramework.ServerStatus.ProcessingClient)
                    {
                        addRelationshipTransaction.AddNode(connectionType, node);

                        return;
                    }
                }
            }
            
            DelayedActions.Enqueue(new DelayedRelationshipAction() { Action = Proxy.TransactionActionType.Updated, ConnectionType = connectionType, Node = node });
        }

        public override void Update(Proxy.RelationshipType relationshipType, ref Soap.TransactionFramework.TransactionChain chain)
        {
            if (Facade != null)
            {
                TransactionFramework.AddRelationshipTransactionLink addRelationshipTransaction = Facade.TransactionOrigin as TransactionFramework.AddRelationshipTransactionLink;

                if (addRelationshipTransaction != null)
                {
                    if (addRelationshipTransaction.TransactionStatus == TransactionFramework.ServerStatus.ProcessingClient)
                    {
                        addRelationshipTransaction.RelationshipType = relationshipType;

                        return;
                    }
                }
            }

            DelayedActions.Enqueue(new DelayedRelationshipAction() { Action = Proxy.TransactionActionType.TypeUpdated, RelationshipType = relationshipType });
        }

        public override void Delete(ref Soap.TransactionFramework.TransactionChain chain)
        {
            if (Facade != null)
            {
                TransactionFramework.AddRelationshipTransactionLink addRelationshipTransaction = Facade.TransactionOrigin as TransactionFramework.AddRelationshipTransactionLink;

                if (addRelationshipTransaction != null)
                {
                    if (addRelationshipTransaction.TransactionStatus == TransactionFramework.ServerStatus.ProcessingClient)
                    {
                        TransactionFramework.DeleteRelationshipTransactionLink deleteTransaction = new TransactionFramework.DeleteRelationshipTransactionLink();

                        deleteTransaction.DomainId = DomainId;
                        deleteTransaction.MapManager = MapManager;
                        deleteTransaction.Relationship = Facade;

                        chain.AddTransaction(deleteTransaction);

                        return;
                    }
                }
            }

            DelayedActions.Enqueue(new DelayedRelationshipAction() { Action = Proxy.TransactionActionType.Deleted });
        }

        public TransactionFramework.ISoapTransactionLink OriginLink
        {
            get;
            set;
        }
    }
}
