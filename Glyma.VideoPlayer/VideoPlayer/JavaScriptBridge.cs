using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;
using VideoPlayer.Controller;
using VideoPlayerSharedLib;

namespace VideoPlayer
{
    public class JavaScriptBridge
    {
        private VideoPlayerMainController _controller;

        public JavaScriptBridge(VideoPlayerMainController controller)
        {
            _controller = controller;
        }

        [ScriptableMember]
        public void ReceiveVideoPlayerMessage(string message)
        {
            _controller.ReceiveMessage(message);
        }

        [ScriptableMember]
        public void StopVideoPlayer()
        {
            _controller.Stop();
        }
    }
}
