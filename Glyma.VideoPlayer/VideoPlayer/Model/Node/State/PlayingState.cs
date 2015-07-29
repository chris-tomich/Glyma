using System.Windows.Media;

namespace VideoPlayer.Model.Node.State
{
    internal class PlayingState : VideoNodeState
    {
        public PlayingState(VideoNodeStateFactory factory, IVideoNode context)
            : base(factory, context)
        {
        }

        public override MediaElementState GetState()
        {
            return MediaElementState.Playing;
        }
    }
}
