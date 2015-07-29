using Proxy = TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.ViewModel
{
    public interface INode : IMapObject
    {
        Proxy.INode Proxy { get; }
        MetadataCollection Metadata { get; }
        IViewModelMetadataFactory ViewModelMetadataFactory { get; }

        void LoadNode(Proxy.IRelationship relationship, Proxy.INode proxy);
    }
}
