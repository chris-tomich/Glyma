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

namespace SilverlightMappingToolBasic.UI.ViewModel
{
    public interface IMapObject
    {
        Guid Id { get; }
        Guid DomainId { get; }
        string OriginalId { get; }
        Guid MapObjectType { get; }
    }
}
