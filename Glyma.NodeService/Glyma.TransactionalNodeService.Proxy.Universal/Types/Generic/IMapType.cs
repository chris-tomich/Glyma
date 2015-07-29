namespace TransactionalNodeService.Proxy.Universal.Types.Generic
{
    public interface IMapType<MapTypeInfo> : IMapType
    {
        void LoadTypeInfo(MapTypeInfo mapTypeInfo);
    }
}
