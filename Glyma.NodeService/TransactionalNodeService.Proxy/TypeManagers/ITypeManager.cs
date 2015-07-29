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

namespace TransactionalNodeService.Proxy
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
