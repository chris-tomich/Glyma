using VideoPlayer.UI;
using VideoPlayerSharedLib;

namespace VideoPlayer.Controller.Listener
{
    internal class VideoPlayerCommandListener : IVideoPlayerListener
    {
        private readonly VideoPlayerMainController _mainController;

        public VideoPlayerCommandListener(VideoPlayerMainController mainController)
        {
            _mainController = mainController;
        }

        public void ReceiveMessage(string message)
        {
            var command = Utilities.Deserialize<Command>(message);
            if (command != null)
            {
                switch (command.Name)
                {
                    case "Play":
                        _mainController.Play(command);
                        break;
                    case "Pause":
                        _mainController.Pause();
                        break;
                    case "Stop":
                        _mainController.Stop();
                        break;
                    case "Seek":
                        _mainController.Seek(command);
                        break;
                    case "Mute":
                        _mainController.Mute(command);
                        break;
                    case "Volume":
                        _mainController.Volume(command);
                        break;
                    case "GetPosition":
                        _mainController.PositionRequest(command);
                        break;
                    case "GetVolume":
                        _mainController.VolumeRequest();
                        break;
                    case "GetIsMuted":
                        _mainController.IsMuted();
                        break;
                    case "GetSource":
                        _mainController.SourceRequest(command);
                        break;
                    case "GetSourceAndPosition":
                        _mainController.SourceAndPositionRequest(command);
                        break;
                    case "GetPlayingState":
                        _mainController.StateRequest(command);
                        break;
                    case "Initialised":
                        _mainController.Initialised(); //send the initialised msg again since Glyma didn't receieve it.
                        break;
                }
            }
        }
    }
}
