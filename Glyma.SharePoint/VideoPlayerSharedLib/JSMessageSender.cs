using System;
using System.Windows.Browser;

namespace VideoPlayerSharedLib
{
    public class JSMessageSender
    {
        private ScriptObject _mappingToolController = null;
        private ScriptObject _videoControllerObject = null;

        public MessageRecipient Recipient
        {
            get;
            set;
        }

        public JSMessageSender(MessageRecipient recipient)
        {
            Recipient = recipient;

            var mappingToolControllerObject = HtmlPage.Window.Eval("Glyma.MappingTool.MappingToolController") as ScriptObject;
            if (mappingToolControllerObject != null)
            {
                MappingToolController = mappingToolControllerObject.Invoke("getInstance") as ScriptObject;
            }

            var videoControllerObject = HtmlPage.Window.Eval("Glyma.RelatedContentPanels.VideoController") as ScriptObject;
            if (videoControllerObject != null)
            {
                VideoControllerObject = videoControllerObject;
            }
        }

        private ScriptObject MappingToolController 
        {
            get 
            {
                return _mappingToolController;
            }
            set 
            {
                _mappingToolController = value;
            }
        }

        private ScriptObject VideoControllerObject
        {
            get 
            {
                return _videoControllerObject;
            }
            set 
            {
                _videoControllerObject = value;
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                if (Recipient == MessageRecipient.Glyma)
                {
                    if (MappingToolController != null)
                    {
                        MappingToolController.Invoke("SendGlymaMessage", message);
                    }
                }
                else if (Recipient == MessageRecipient.VideoPlayer)
                {
                    if (VideoControllerObject != null)
                    {
                        VideoControllerObject.Invoke("SendVideoPlayerMessage", message);
                    }
                }
            }
            catch (Exception)
            {
                // There is a chance of an exception when the page is refreshed, the video player exits which 
                // causes an event that will always fail as the JS no longer exists.
                // Other exceptions could occur if the JavaScript methods had errors in them, TODO: determine if this is a problem that can be mitigated other ways
            }
        }
    }
}
