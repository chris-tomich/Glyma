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
using SilverlightMappingToolBasic.UI.ViewModel;

namespace SilverlightMappingToolBasic.UI
{
    public interface IMapController<TNodeType, TRelationshipType> where TNodeType : INode where TRelationshipType : IRelationship
    {
        event EventHandler<ModelChangedEventArgs<TNodeType, TRelationshipType>> ModelChanged;
        event EventHandler MapLoadCompleted;
        INode Context { get; }
        Proxy.IMapManager MapManager { get; }
        ThemeManager ThemeManager { get; }
        IMap ActiveModel { get; }

        void RefreshAsync();
        void ChangeContextAsync(IMapObject mapObject);
        void ChangeContextAsync(Proxy.INode mapObject);
    }
}
