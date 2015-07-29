using System.Collections.Generic;
using VideoPlayer.Controller.Interface;

namespace VideoPlayer.Model.Commands
{
    internal abstract class VideoPlayerCommand : IVideoPlayerCommand
    {
        public IDictionary<string, string> Parameters;

        protected VideoPlayerCommand(IDictionary<string,string> para)
        {
            Parameters = para;
        }

        public void Excute(IMediaController controller)
        {
            
        }
    }
}
