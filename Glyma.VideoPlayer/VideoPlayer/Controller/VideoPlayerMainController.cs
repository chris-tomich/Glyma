using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VideoPlayer.Controller.Interface;
using VideoPlayer.Controller.Listener;
using VideoPlayer.Controller.Sender;
using VideoPlayer.UI;
using VideoPlayerSharedLib;

namespace VideoPlayer.Controller
{
    public class VideoPlayerMainController : IDisposable, ICommandController
    {
        private readonly VideoPlayerCommandListener _commandListener;
        private readonly VideoPlayerMsgSender _videoPlayerMsgSender;
        
        private readonly VideoPlayerMainContainer _ref;

        private MediaElement MediaPlayer {
            get { return _ref.MediaElementControl.MediaPlayer; }
        }

        public VideoPlayerMainController(VideoPlayerMainContainer videoPlayerMainContainer)
        {
            _ref = videoPlayerMainContainer;
            try
            {
                _commandListener = new VideoPlayerCommandListener(this);
                _videoPlayerMsgSender = new VideoPlayerMsgSender();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Cannot Register Video Player:\r\n{0}", ex.Message));
            }
        }

        public void ReceiveMessage(string message)
        {
            _commandListener.ReceiveMessage(message);
        }

        //Sent to Glyma so that any cached command is sent now the player is ready to process it.
        public void Initialised()
        {
            var initialisedEvent = new Event()
            {
                Name = "Initialised"
            };
            _videoPlayerMsgSender.Send(initialisedEvent);
        }

        //Sent to Glyma whenever the player is disposed so that it waits for the initialised event before sending commands
        public void PlayerDisposed()
        {
            var playerDisposedEvent = new Event()
            {
                Name = "PlayerDisposed",
                EventArgs = new List<EventArg>()
            };
            if (_ref.MediaElementControl.CurrentNode != null)
            {
                playerDisposedEvent.EventArgs.Add(new EventArg() { Name = "NodeId", Value = _ref.MediaElementControl.CurrentNode.NodeId.ToString() });
            }
            _videoPlayerMsgSender.Send(playerDisposedEvent);
        }

        public void Mute(Command muteCommand)
        {
            if (muteCommand != null && muteCommand.Params != null)
            {
                if (muteCommand.ContainsParam("IsMuted"))
                {
                    bool isMuted = Boolean.Parse(muteCommand.GetParamValue("IsMuted"));
                    MediaPlayer.IsMuted = isMuted;
                }
            }
        }

        public void Volume(Command volumeCommand)
        {
            if (volumeCommand != null && volumeCommand.Params != null)
            {
                if (volumeCommand.ContainsParam("Volume"))
                {
                    double volume = Double.Parse(volumeCommand.GetParamValue("Volume"));
                    MediaPlayer.Volume = volume;
                }
            }
        }

        public void PositionRequest(Command getPosCommand)
        {
            if (getPosCommand != null && getPosCommand.Params != null)
            {
                var positionReturnEvent = new Event
                {
                    Name = "MediaPositionCallback",
                    EventArgs =
                        new List<EventArg>(Utilities.ConvertParamsToEventArgs(getPosCommand.Params))
                        {
                            {new EventArg() { Name="Position", Value = MediaPlayer.Position.ToString()}}
                        }
                };

                //copy over the original params which will contain some callback identifier info
                _videoPlayerMsgSender.Send(positionReturnEvent);
            }
        }

        public void SourceRequest(Command getSourceCommand)
        {
            if (getSourceCommand != null && getSourceCommand.Params != null)
            {
                var getSourceReturnEvent = new Event
                {
                    Name = "GetSourceCallback",
                    EventArgs = new List<EventArg>(Utilities.ConvertParamsToEventArgs(getSourceCommand.Params))
                };

                //copy over the original params which will contain some callback identifier info
                string source = null;
                if (MediaPlayer.Source != null)
                {
                    source = MediaPlayer.Source.ToString();
                }
                getSourceReturnEvent.EventArgs.Add(new EventArg() { Name = "Source", Value = source });
                _videoPlayerMsgSender.Send(getSourceReturnEvent);
            }
        }

        public void SourceAndPositionRequest(Command getSourceAndPositionCommand)
        {
            if (getSourceAndPositionCommand != null && getSourceAndPositionCommand.Params != null)
            {
                var getSourceAndPosReturnEvent = new Event
                {
                    Name = "GetSourceAndPositionCallback",
                    EventArgs = new List<EventArg>(Utilities.ConvertParamsToEventArgs(getSourceAndPositionCommand.Params))
                };

                //copy over the original params which will contain some callback identifier info
                string source = null;
                if (MediaPlayer.Source != null)
                {
                    source = MediaPlayer.Source.ToString();
                }
                getSourceAndPosReturnEvent.EventArgs.Add(new EventArg() { Name="Source", Value = source });
                getSourceAndPosReturnEvent.EventArgs.Add(new EventArg() { Name="Position", Value=MediaPlayer.Position.ToString() });
                _videoPlayerMsgSender.Send(getSourceAndPosReturnEvent);
            }
        }

        public void StateRequest(Command getPlayingStateCommand)
        {
            if (getPlayingStateCommand != null && getPlayingStateCommand.Params != null)
            {
                var playingStateReturnEvent = new Event
                {
                    Name = "GetPlayingStateCallback",
                    EventArgs = new List<EventArg>(Utilities.ConvertParamsToEventArgs(getPlayingStateCommand.Params))
                };

                //copy over the original params which will contain some callback identifier info
                switch (MediaPlayer.CurrentState)
                {
                    case MediaElementState.Playing:
                        playingStateReturnEvent.EventArgs.Add(new EventArg() { Name = "State", Value = "Playing" });
                        break;
                    case MediaElementState.Paused:
                        playingStateReturnEvent.EventArgs.Add(new EventArg() { Name = "State", Value = "Idle" });
                        break;
                    case MediaElementState.Buffering:
                        playingStateReturnEvent.EventArgs.Add(new EventArg() { Name = "State", Value = "Buffering"});
                        break;
                    case MediaElementState.Stopped:
                        playingStateReturnEvent.EventArgs.Add(new EventArg() { Name = "State", Value = "Idle"});
                        break;
                    case MediaElementState.Opening:
                        playingStateReturnEvent.EventArgs.Add(new EventArg() { Name = "State", Value = "Buffering"});
                        break;
                    default:
                        //report all other states as stopped/not playing yet
                        playingStateReturnEvent.EventArgs.Add(new EventArg() { Name = "State", Value = "Idle"});
                        break;
                }
                if (_ref.MediaElementControl.CurrentNode != null)
                {
                    playingStateReturnEvent.EventArgs.Add(new EventArg() { Name = "NodeId", Value = _ref.MediaElementControl.CurrentNode.NodeId.ToString()});
                }

                _videoPlayerMsgSender.Send(playingStateReturnEvent);
            }
        }


        public void VolumeRequest()
        {
            var volumeReturnEvent = new Event
            {
                Name = "VolumeCallback",
                EventArgs = new List<EventArg> { { new EventArg() { Name="Volume", Value=MediaPlayer.Volume.ToString(CultureInfo.InvariantCulture)} } }
            };
            _videoPlayerMsgSender.Send(volumeReturnEvent);
        }

        public void Dispose()
        {
            PlayerDisposed(); //tell Glyma that it was unloaded so it waits for the initialised signal again
        }

        public void SendEvent(Event e)
        {
            _videoPlayerMsgSender.Send(e);
        }

        public void Play(Command command)
        {
            _ref.MediaElementControl.Play(command);
        }


        public void Pause()
        {
            _ref.MediaElementControl.Pause();
        }

        public void Stop()
        {
            _ref.MediaElementControl.Stop();
        }

        public void Seek(Command command)
        {
            _ref.MediaElementControl.Seek(command);
        }

        public void IsMuted()
        {
            _ref.VolumeControl.IsMuted();
        }
    }
}
