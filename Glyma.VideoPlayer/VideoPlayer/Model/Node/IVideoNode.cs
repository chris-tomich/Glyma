using System.Net.Sockets;
using VideoPlayer.Model.Node.State;

namespace VideoPlayer.Model.Node
{
    public interface IVideoNode
    {
        VideoNodeState CurrentState { get; set; }
    }

}
