using System.Windows.Media;

namespace VideoPlayer.Model.Node.State
{
    internal class OpeningState : VideoNodeState
    {
        public OpeningState(VideoNodeStateFactory factory, IVideoNode context)
            : base(factory, context)
        {
        }

        public override MediaElementState GetState()
        {
            return MediaElementState.Opening;
        }
    }
}
