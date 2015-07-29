using System.Windows.Media;

namespace VideoPlayer.Model.Node.State
{
    internal class BufferingState : VideoNodeState
    {
        public BufferingState(VideoNodeStateFactory factory, IVideoNode context) : base(factory, context)
        {
        }

        public override MediaElementState GetState()
        {
            return MediaElementState.Buffering;
        }
    }
}
