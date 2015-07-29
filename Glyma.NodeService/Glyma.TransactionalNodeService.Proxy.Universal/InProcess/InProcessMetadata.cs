using TransactionalNodeService.Proxy.Universal.Collections.MetadataCollection;
using TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.InProcess
{
    internal class InProcessMetadata : MetadataSet, IInProcess
    {
        private DelayedMetadataActions _delayedActions = null;

        public InProcessMetadata(IMapManager mapManager)
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

        public override void Update(string name, string value, INode node, IRelationship relationship, ConnectionType connectionType, ref TransactionChain chain)
        {
            base.Update(name, value, node, relationship, connectionType, ref chain);

            DelayedMetadataAction delayedAction = new DelayedMetadataAction();
            delayedAction.Action = TransactionActionType.Updated;
            delayedAction.Name = name;
            delayedAction.Value = value;
            delayedAction.Node = node;
            delayedAction.Relationship = relationship;
            delayedAction.ConnectionType = connectionType;

            DelayedActions.Enqueue(delayedAction);
        }

        public override void Delete(ref TransactionChain chain)
        {
            base.Delete(ref chain);

            DelayedMetadataAction delayedAction = new DelayedMetadataAction();
            delayedAction.Action = TransactionActionType.Deleted;

            DelayedActions.Enqueue(delayedAction);
        }

        public ISoapTransactionLink OriginLink
        {
            get;
            set;
        }
    }
}
