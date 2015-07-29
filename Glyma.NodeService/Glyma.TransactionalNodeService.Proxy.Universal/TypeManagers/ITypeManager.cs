using System;

namespace TransactionalNodeService.Proxy.Universal.TypeManagers
{
    public interface ITypeManager<ServiceType, TypeManagerType>
    {
        TypeManagerType this[Guid typeId] { get; }
        TypeManagerType this[string typeName] { get; }

        void Add(ServiceType serviceType, TypeManagerType mapType);
        ServiceType ConvertProxyToService(TypeManagerType mapType);
        bool ContainsKey(Guid typeId);
        bool ContainsKey(string typeName);
    }
}
