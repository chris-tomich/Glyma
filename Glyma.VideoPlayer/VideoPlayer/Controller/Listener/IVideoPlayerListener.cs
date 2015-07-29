using System;
using System.Windows.Messaging;

namespace VideoPlayer.Controller.Listener
{
    internal interface IVideoPlayerListener
    {
        void ReceiveMessage(string message);
    }
}
