using System.Windows.Media;

namespace VideoPlayer.Model.Node.State
{
    internal class PausedState : VideoNodeState
    {
        public PausedState(VideoNodeStateFactory factory, IVideoNode context)
            : base(factory, context)
        {
        }

        public override MediaElementState GetState()
        {
            return MediaElementState.Paused;
        }
    }
}
