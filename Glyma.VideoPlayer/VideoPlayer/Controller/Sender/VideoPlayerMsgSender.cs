using VideoPlayerSharedLib;
using VideoPlayer.UI;

namespace VideoPlayer.Controller.Sender
{
    internal class VideoPlayerMsgSender
    {
        private readonly JSMessageSender _sender;

        internal VideoPlayerMsgSender()
        {
            _sender = new JSMessageSender(MessageRecipient.Glyma);
        }

        public void Send(Event e)
        {
            Utilities.SendMessage(_sender, e);
        }
    }
}
