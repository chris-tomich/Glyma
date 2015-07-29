using System;
using System.Windows.Input;
using System.Windows.Media;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Selector;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Interface
{
    public interface IMapMoveControl : IZoomControl
    {
        event EventHandler<MoveTransformEventArgs> MapMoved;
        event MouseEventHandler MouseMove;

        TranslateTransform MoveGraphTransform { get; }
    }
}
