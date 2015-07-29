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
using TransactionalNodeService.Proxy;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace TransactionalNodeService.InProcess
{
    internal class InProcessMetadata : MetadataSet, IInProcess
    {
        private DelayedMetadataActions _delayedActions = null;

        public InProcessMetadata(Proxy.IMapManager mapManager)
        {
            MapManager = mapManager;
        }

        public FacadeMetadataSet Facade
        {
            get;
            set;
        }

        public DelayedMetadataActions DelayedActions
        {
            get
            {
                _delayedActions = _delayedActions ?? new DelayedMetadataActions(this);

                return _delayedActions;
            }
        }

        public override void Update(string name, string value, Proxy.INode node, Proxy.IRelationship relationship, Proxy.ConnectionType connectionType, ref TransactionFramework.TransactionChain chain)
        {
            base.Update(name, value, node, relationship, connectionType, ref chain);

            DelayedMetadataAction delayedAction = new DelayedMetadataAction();
            delayedAction.Action = TransactionActionType.Updated;
            delayedAction.Name = Name;
            delayedAction.Value = Value;
            delayedAction.Node = Node;
            delayedAction.DomainId = Node.DomainId;
            delayedAction.Relationship = Relationship;
            delayedAction.ConnectionType = ConnectionType;

            DelayedActions.Enqueue(delayedAction);
        }

        public override void Delete(ref TransactionFramework.TransactionChain chain)
        {
            base.Delete(ref chain);

            DelayedMetadataAction delayedAction = new DelayedMetadataAction();
            delayedAction.Action = TransactionActionType.Deleted;

            DelayedActions.Enqueue(delayedAction);
        }

        public TransactionFramework.ISoapTransactionLink OriginLink
        {
            get;
            set;
        }
    }
}
