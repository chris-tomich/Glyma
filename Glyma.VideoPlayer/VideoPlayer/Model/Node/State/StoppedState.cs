using System.Windows.Media;

namespace VideoPlayer.Model.Node.State
{
    internal class StoppedState : VideoNodeState
    {
        public StoppedState(VideoNodeStateFactory factory, IVideoNode context)
            : base(factory, context)
        {
        }

        public override MediaElementState GetState()
        {
            return MediaElementState.Stopped;
        }
    }
}
