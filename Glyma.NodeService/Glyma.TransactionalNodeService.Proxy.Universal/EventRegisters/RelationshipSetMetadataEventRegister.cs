using TransactionalNodeService.Proxy.Universal.EventArgs;

namespace TransactionalNodeService.Proxy.Universal.EventRegisters
{
    public class RelationshipSetMetadataEventRegister : EventRegister<Relationship, MetadataChangedEventArgs>
    {
        public RelationshipSetMetadataEventRegister()
            : base()
        {
        }
    }
}
