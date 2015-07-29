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

namespace SilverlightMappingToolBasic.UI.ViewModel
{
    public interface IRelationship : IMapObject
    {
        Proxy.IRelationship Proxy { get; }

        void LoadRelationship(Proxy.IRelationship proxy);
    }
}
