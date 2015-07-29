using System;
using System.Windows.Messaging;
using System.ComponentModel;
using System.Collections.Generic;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MessageBox;
using TransactionalNodeService.Proxy;
using TransactionalNodeService.Service;
using TransactionalNodeService.Soap.TransactionFramework;
using VideoPlayerSharedLib;
using System.Windows.Browser;

namespace SilverlightMappingToolBasic.UI.Extensions.VideoWebPart
{
    public class VideoController
    {
        private Dictionary<Guid, VideoInfo> _viewModels;
        private bool _isChangedByController;
        private bool _playerInitialised;
        private Command _cachedCommand;

        public VideoController()
        {
            MessageSender = new JSMessageSender(MessageRecipient.VideoPlayer);
        }

        private Dictionary<Guid, VideoInfo> ViewModels
        {
            get
            {
                if (_viewModels == null)
                {
                    _viewModels = new Dictionary<Guid, VideoInfo>();
                }

                return _viewModels;
            }
        }

        public string DefaultVideoSource
        {
            get;
            set;
        }

        public bool HasVideo
        {
            get; set;
        }

        public JSMessageSender MessageSender
        {
            get;
            set;
        }

        public string VideoSource
        {
            get;
            set;
        }

        public void Clear()
        {
            if (_viewModels != null)
            {
                _viewModels.Clear();
            }
        }

        public VideoInfo CreateVideoInfoViewModel(ViewModel.INode node)
        {
            var viewModelVideoInfo = new VideoInfo(node);
            viewModelVideoInfo.PropertyChanged += OnVideoInfoChanged;

            ViewModels[viewModelVideoInfo.Id] = viewModelVideoInfo;

            viewModelVideoInfo.SetStartPositionRequested += OnSetStartPositionRequested;
            viewModelVideoInfo.SetStopPositionRequested += OnSetStopPositionRequested;

            return viewModelVideoInfo;
        }

        private void OnSetStartPositionRequested(object sender, EventArgs e)
        {
            var viewModelVideoInfo = sender as VideoInfo;

            if (viewModelVideoInfo != null)
            {
                SendGetStartPositionCommand(viewModelVideoInfo);
            }
        }

        private void OnSetStopPositionRequested(object sender, EventArgs e)
        {
            var viewModelVideoInfo = sender as VideoInfo;

            if (viewModelVideoInfo != null)
            {
                SendGetStopPositionCommand(viewModelVideoInfo);
            }
        }

        

        private void OnVideoInfoChanged(object sender, PropertyChangedEventArgs e)
        {
                var viewModelVideoInfo = sender as VideoInfo;

                if (viewModelVideoInfo != null)
                {
                    switch (e.PropertyName)
                    {
                        case "Status":
                            if (!_isChangedByController)
                            {
                                if (viewModelVideoInfo.Status == VideoState.Playing)
                                {
                                    SendPlayCommand(viewModelVideoInfo);
                                }
                                else if (viewModelVideoInfo.Status == VideoState.Pause)
                                {
                                    SendPauseCommand(viewModelVideoInfo);
                                }
                            }
                            else
                            {
                                _isChangedByController = false;
                            }
                            break;
                    }
                }
        }

        public void ReceiveMessage(string message)
        {
            var receivedEvent = Utilities.Deserialize<Event>(message);
            if (receivedEvent != null)
            {
                switch (receivedEvent.Name)
                {
                    case "GetSourceAndPositionCallback":
                        HandleGetSourceAndPositionCallback(receivedEvent);
                        break;
                    case "CurrentStateChanged":
                        HandledCurrentStateChanged(receivedEvent);
                        break;
                    case "GetPlayingStateCallback":
                        HandleGetPlayingStateCallback(receivedEvent);
                        break;
                    case "Seek":
                        HandleSeekEvent(receivedEvent);
                        break;
                    case "ReplayingLastNode":
                        HandleReplayingLastNodeEvent(receivedEvent);
                        break;
                    case "Initialised":
                        HandleInitialisedEvent();
                        break;
                    case "PlayerDisposed":
                        HandlePlayerDisposedEvent(receivedEvent);
                        break;
                }
            }
        }

        private void HandleGetSourceAndPositionCallback(Event callback)
        {
            if (callback.EventArgs != null && callback.ContainsEventArg("CallbackId")
                && callback.ContainsEventArg("Position") && callback.ContainsEventArg("Source")
                && callback.ContainsEventArg("NodeId"))
            {
                string callbackId = callback.GetEventArgValue("CallbackId");
                string source = callback.GetEventArgValue("Source");
                source = HttpUtility.UrlDecode(source); 
                string viewModelIdAsString = callback.GetEventArgValue("NodeId");

                if (!string.IsNullOrEmpty(viewModelIdAsString) && !string.IsNullOrEmpty(source))
                {
                    var chain = new TransactionChain();
                    var viewModelId = new Guid(viewModelIdAsString);
                    var viewModelVideoInfo = ViewModels[viewModelId];

                    switch (callbackId)
                    {
                        case "StartPosition":
                            {
                                TimeSpan startPosition = TimeSpan.Parse(callback.GetEventArgValue("Position"));
                                // Just add a temporary end position so that the entry doesn't end up in a bad state.
                                ViewModel.IMetadata startPositionMetadata = viewModelVideoInfo.Context.Metadata["Video.StartPosition"];

                                if (startPositionMetadata != null)
                                {
                                    startPositionMetadata.SetValue(startPosition.ToString(), ref chain); 
                                    ((SuperGraph.ViewModel.Node)viewModelVideoInfo.Context).NodeProperties.UIMetadata["Video.StartPosition"] = startPosition.ToString();
                                }
                                else
                                {
                                    viewModelVideoInfo.Context.Metadata.Add("Video.StartPosition", startPosition.ToString(), ref chain);
                                    ((SuperGraph.ViewModel.Node)viewModelVideoInfo.Context).NodeProperties.UIMetadata.Add("Video.StartPosition", startPosition.ToString());
                                }

                                ViewModel.IMetadata endPositionMetadata = viewModelVideoInfo.Context.Metadata["Video.EndPosition"];

                                if (endPositionMetadata != null)
                                {
                                    endPositionMetadata.SetValue("", ref chain);
                                }
                                else
                                {
                                    viewModelVideoInfo.Context.Metadata.Add("Video.EndPosition", "", ref chain);
                                    ((SuperGraph.ViewModel.Node)viewModelVideoInfo.Context).NodeProperties.UIMetadata.Add("Video.EndPosition", "");
                                }

                                viewModelVideoInfo.HasVideo = true;
                                viewModelVideoInfo.StartPosition = startPosition;
                                viewModelVideoInfo.StopPosition = null;
                                break;
                            }
                        case "EndPosition":
                            {
                                TimeSpan endPosition = TimeSpan.Parse(callback.GetEventArgValue("Position"));

                                ViewModel.IMetadata endPositionMetadata = viewModelVideoInfo.Context.Metadata["Video.EndPosition"];

                                if (endPositionMetadata != null)
                                {
                                    endPositionMetadata.SetValue("", ref chain);
                                    ((SuperGraph.ViewModel.Node)viewModelVideoInfo.Context).NodeProperties.UIMetadata["Video.EndPosition"] = endPosition.ToString();
                                }
                                else
                                {
                                    viewModelVideoInfo.Context.Metadata.Add("Video.EndPosition", "", ref chain);
                                    ((SuperGraph.ViewModel.Node)viewModelVideoInfo.Context).NodeProperties.UIMetadata.Add("Video.EndPosition", endPosition.ToString());
                                }

                                viewModelVideoInfo.Context.Metadata.Add(null, null, "Video.EndPosition", endPosition.ToString(), ref chain);
                                viewModelVideoInfo.StopPosition = endPosition;
                                viewModelVideoInfo.HasVideo = true;
                                break;
                            }
                    }

                    ViewModel.IMetadata videoSourceMetadata = viewModelVideoInfo.Context.Metadata["Video.Source"];

                    if (videoSourceMetadata != null)
                    {
                        videoSourceMetadata.SetValue(source, ref chain);
                        ((SuperGraph.ViewModel.Node) viewModelVideoInfo.Context).NodeProperties.UIMetadata[
                            "Video.Source"] = source;
                    }
                    else
                    {
                        viewModelVideoInfo.Context.Metadata.Add(null, null, "Video.Source", source, ref chain);
                        ((SuperGraph.ViewModel.Node)viewModelVideoInfo.Context).NodeProperties.UIMetadata.Add("Video.Source", source);
                    }
                    viewModelVideoInfo.VideoSource = source;
                    viewModelVideoInfo.Context.Proxy.MapManager.ExecuteTransaction(chain);
                }
            }
        }

        private void HandleInitialisedEvent()
        {
            if (!_playerInitialised)
            {
                _playerInitialised = true;

                if (_cachedCommand != null)
                {
                    Utilities.SendMessage(MessageSender, _cachedCommand);
                    _cachedCommand = null;
                }
            }
        }

        //Sent to the Video Player so that if Gylma starts after the Video Player the Video Player will ping back that it's initialised
        public void Initialised()
        {
            var initialisedEvent = new Command()
            {
                Name = "Initialised"
            };
            Utilities.SendMessage(MessageSender, initialisedEvent);
        }

        private void HandlePlayerDisposedEvent(Event disposedEvent)
        {
            _cachedCommand = null;
            _playerInitialised = false;
            if (disposedEvent.EventArgs != null && disposedEvent.ContainsEventArg("NodeId"))
            {
                string viewModelIdAsString = disposedEvent.GetEventArgValue("NodeId");
                var viewModelId = new Guid(viewModelIdAsString);
                if (ViewModels.ContainsKey(viewModelId))
                {
                    VideoInfo viewModelVideoInfo = ViewModels[viewModelId];
                    _isChangedByController = true;
                    viewModelVideoInfo.Status = VideoState.Pause;
                }
            }
        }

        private void HandleGetPlayingStateCallback(Event callback)
        {
            if (callback.EventArgs != null && callback.ContainsEventArg("CallbackId")
                && callback.ContainsEventArg("State") && callback.ContainsEventArg("NodeId"))
            {
                var callbackId = callback.GetEventArgValue("CallbackId");
                var state = callback.GetEventArgValue("State");
                var viewModelIdAsString = callback.GetEventArgValue("NodeId");

                if (!string.IsNullOrEmpty(viewModelIdAsString))
                {
                    var viewModelId = new Guid(viewModelIdAsString);

                    foreach (var viewModelVideoInfo in ViewModels.Values)
                    {
                        if (viewModelVideoInfo.Id == viewModelId)
                        {
                            if (state == "Playing" || state == "Buffering")
                            {
                                _isChangedByController = true;
                                viewModelVideoInfo.Status = VideoState.Playing;
                            }
                            else
                            {
                                _isChangedByController = true;
                                viewModelVideoInfo.Status = VideoState.Pause;
                            }
                        }
                        else
                        {
                            if (viewModelVideoInfo.HasVideo)
                            {
                                _isChangedByController = true;
                                viewModelVideoInfo.Status = VideoState.Pause;
                            }
                        }
                    }
                }
            }
        }

        private void HandledCurrentStateChanged(Event receivedEvent)
        {
            if (!HasVideo)
            {
                HasVideo = true;
            }

            if (receivedEvent.EventArgs != null && receivedEvent.ContainsEventArg("State"))
            {
                string state = receivedEvent.GetEventArgValue("State");

                if (receivedEvent.ContainsEventArg("NodeId"))
                {
                    string viewModelIdAsString = receivedEvent.GetEventArgValue("NodeId");

                    if (!string.IsNullOrEmpty(state) && !string.IsNullOrEmpty(viewModelIdAsString))
                    {
                        var neutralisedState = state.ToLower();
                        var viewModelId = new Guid(viewModelIdAsString);

                        if (ViewModels.ContainsKey(viewModelId))
                        {
                            VideoInfo viewModelVideoInfo = ViewModels[viewModelId];

                            switch (neutralisedState)
                            {
                                case "idle":
                                    _isChangedByController = true;
                                    viewModelVideoInfo.Status = VideoState.Pause;
                                    break;
                                case "playing":
                                    _isChangedByController = true;
                                    viewModelVideoInfo.Status = VideoState.Playing;
                                    break;
                                case "buffering":
                                    _isChangedByController = true;
                                    viewModelVideoInfo.Status = VideoState.Playing;
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void HandleSeekEvent(Event receivedEvent)
        {
            //if (receivedEvent.EventArgs != null && receivedEvent.EventArgs.ContainsKey("NewPosition"))
            //{
            //    if (ViewManager != null && ViewManager.CurrentView != null && ViewManager.CurrentView.NodeRenderers != null)
            //    {
            //        TimeSpan newPosition = TimeSpan.MinValue;
            //        if (TimeSpan.TryParse(receivedEvent.EventArgs["NewPosition"], out newPosition))
            //        {
            //            foreach (NodeRenderer nodeRenderer in ViewManager.CurrentView.NodeRenderers.Values)
            //            {
            //                if (nodeRenderer.ResetMediaIcon(newPosition))
            //                {
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //}
        }

        private void ResetNodeRendererMediaIcon(Guid nodeId, bool playing)
        {
            //if (ViewManager != null && ViewManager.CurrentView != null && ViewManager.CurrentView.NodeRenderers != null && nodeId != Guid.Empty)
            //{
            //    foreach (NodeRenderer nodeRenderer in ViewManager.CurrentView.NodeRenderers.Values)
            //    {
            //        if (nodeRenderer.Node.Id == nodeId)
            //        {
            //            nodeRenderer.ResetMediaIcon(playing);
            //        }
            //        else
            //        {
            //            //all other nodes are not playing
            //            nodeRenderer.ResetMediaIcon(false);
            //        }
            //    }
            //}
        }

        private void HandleReplayingLastNodeEvent(Event receivedEvent)
        {
            //if (ViewManager != null && ViewManager.CurrentView != null && ViewManager.CurrentView.NodeRenderers != null
            //    && receivedEvent.EventArgs != null && receivedEvent.EventArgs.ContainsKey("NodeId"))
            //{
            //    Guid nodeId = Guid.Empty;
            //    if (Guid.TryParse(receivedEvent.EventArgs["NodeId"], out nodeId))
            //    {
            //        NodeRenderer playingNode = ViewManager.CurrentView.NodeRenderers[nodeId];
            //        {
            //            playingNode.SetIsPlaying(true);
            //            playingNode.ResetMediaIcon(true);
            //        }
            //    }
            //}
        }

        public void SendGetStartPositionCommand(VideoInfo viewModelVideoInfo)
        {
            var getStartPositionCommand = new Command();
            getStartPositionCommand.Name = "GetSourceAndPosition";
            getStartPositionCommand.Params = new List<Param>();
            getStartPositionCommand.Params.Add(new Param(){Name="CallbackId",Value="StartPosition"});
            getStartPositionCommand.Params.Add(new Param(){Name="NodeId",Value=viewModelVideoInfo.Id.ToString()});
            Utilities.SendMessage(MessageSender, getStartPositionCommand);
        }

        public void SendGetStopPositionCommand(VideoInfo viewModelVideoInfo)
        {
            var getEndPositionCommand = new Command();
            getEndPositionCommand.Name = "GetSourceAndPosition";
            getEndPositionCommand.Params = new List<Param>();
            getEndPositionCommand.Params.Add(new Param() { Name = "CallbackId", Value = "EndPosition" });
            getEndPositionCommand.Params.Add(new Param() { Name = "NodeId", Value = viewModelVideoInfo.Id.ToString() });
            Utilities.SendMessage(MessageSender, getEndPositionCommand);
        }

        public void SendPauseCommand(VideoInfo viewModelVideoInfo)
        {
            var pauseCommand = new Command();
            pauseCommand.Name = "Pause";
            Utilities.SendMessage<Command>(MessageSender, pauseCommand);
        }

        public void SendSetSourceCommand(INode node)
        {
            VideoSizeHelper sizeHelper = new VideoSizeHelper(node);
            RelatedContentPanelUtil.Instance.LoadRelatedVideoContent(VideoSource, sizeHelper.Size);

            var setSourceCommand = new Command();
            setSourceCommand.Name = "Play";
            setSourceCommand.Params = new List<Param>();
            setSourceCommand.Params.Add(new Param() { Name="Source", Value=VideoSource});
            setSourceCommand.Params.Add(new Param() { Name="StartTimeCode", Value="00:00:00"});
            setSourceCommand.Params.Add(new Param() { Name="AutoPlay", Value="false"});
            setSourceCommand.Params.Add(new Param() { Name="NodeId", Value=node.Id.ToString()});

            if (_playerInitialised)
            {
                Utilities.SendMessage(MessageSender, setSourceCommand);
            }
            else
            {
                _cachedCommand = setSourceCommand;
            }
        }

        public void SendPlayCommand(VideoInfo viewModelVideoInfo, bool isAutoPlay = true)
        {
            RelatedContentPanelUtil.Instance.LoadRelatedVideoContent(viewModelVideoInfo.VideoSource, viewModelVideoInfo.Size);

            var playCommand = new Command();
            playCommand.Name = "Play";
            playCommand.Params = new List<Param>();
            playCommand.Params.Add(new Param() { Name = "Source", Value = viewModelVideoInfo.VideoSource });
            playCommand.Params.Add(new Param() { Name="StartTimeCode", Value=viewModelVideoInfo.StartPosition.ToString()});

            if (viewModelVideoInfo.StopPosition != null)
            {
                playCommand.Params.Add(new Param() { Name="EndTimeCode", Value=viewModelVideoInfo.StopPosition.ToString()});
            }

            playCommand.Params.Add(new Param() { Name="NodeId", Value=viewModelVideoInfo.Id.ToString()});
            playCommand.Params.Add(new Param() { Name = "AutoPlay", Value = isAutoPlay.ToString()});

            if (_playerInitialised)
            {
                Utilities.SendMessage(MessageSender, playCommand);
            }
            else
            {
                _cachedCommand = playCommand;
            }
        }

        public void SendGetPlayStateCommand(VideoInfo viewModelVideoInfo)
        {
            var getPlayingStateCommand = new Command();
            getPlayingStateCommand.Name = "GetPlayingState";
            getPlayingStateCommand.Params = new List<Param>();
            getPlayingStateCommand.Params.Add(new Param() { Name="NodeId", Value=viewModelVideoInfo.Id.ToString()});
            Utilities.SendMessage(MessageSender, getPlayingStateCommand);
        }
    }
}
