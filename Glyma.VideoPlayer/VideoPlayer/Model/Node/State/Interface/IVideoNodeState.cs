using System.Windows.Media;

namespace VideoPlayer.Model.Node.State.Interface
{
    internal interface IVideoNodeState
    {
        MediaElementState GetState();
    }
}
