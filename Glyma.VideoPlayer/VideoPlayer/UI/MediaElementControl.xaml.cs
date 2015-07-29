using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VideoPlayer.Controller;
using VideoPlayer.Model.Node;
using VideoPlayerSharedLib;

namespace VideoPlayer.UI
{
    public partial class MediaElementControl : UserControl
    {
        //Video nodes controller
        private readonly VideoNodesController _videoNodesController;
        public VideoPlayerMainContainer MainUI { get; set; }

        public MediaElementControl()
        {
            InitializeComponent();
            MediaPlayer.AutoPlay = true;
            MediaPlayer.BufferingTime = new TimeSpan(0, 0, 5); //5 seconds is supposedly the default, we were seeing much more than that
            _videoNodesController = new VideoNodesController();
            _videoNodesController.NodeChanged += VideoNodesControllerOnNodeChanged;
        }

        public void SetCurrentNodeFromNodeId(Guid nodeId)
        {
            _videoNodesController.SetCurrentNode(_videoNodesController.GetVideoNodeById(nodeId));
        }


        public void SeekBySeconds(int seconds)
        {
            TimeSpan newPosition;
            if (seconds > 0)
            {
                TimeSpan addSeconds = new TimeSpan(0, 0, seconds);
                newPosition = MediaPlayer.Position.Add(addSeconds);
            }
            else
            {
                TimeSpan subtractSeconds = new TimeSpan(0, 0, (seconds * -1)); //make it a positive number again
                newPosition = MediaPlayer.Position.Subtract(subtractSeconds);
            }
            Duration duration = MediaPlayer.NaturalDuration;

            if ((duration.HasTimeSpan && newPosition < duration.TimeSpan) && newPosition > TimeSpan.Zero)
            {
                MediaPlayer.Position = newPosition;

                if (MediaPlayer.Markers.Count > 0)
                {
                    var currentEndTimeMaker = MediaPlayer.Markers[0].Time;
                    if (newPosition > currentEndTimeMaker)
                    {
                        MediaPlayer.Markers.Clear();
                        MainUI.SliderControl.HasSeekedBeyondEnd = true;
                    }
                }
                else
                {
                    MainUI.SliderControl.HasSeekedBeyondEnd = false;
                }

                var seekEvent = new Event
                {
                    Name = "Seek",
                    EventArgs = new List<EventArg>() { { new EventArg() { Name = "NewPosition", Value = newPosition.ToString() } } }
                };
                MainUI.Controller.SendEvent(seekEvent);
            }
        }

        public void SeekToMediaPosition(TimeSpan position)
        {
            MediaPlayer.Position = position;
            //if (CurrentNode != null)
            //{
            //    CurrentNode.PlayingTime = position;
            //}
            if (MediaPlayer.Markers.Count > 0)
            {
                var currentEndTimeMarker = MediaPlayer.Markers[0].Time; //get the end time marker
                if (position > currentEndTimeMarker) //have seeked beyond the marker
                {
                    MediaPlayer.Markers.Clear();
                    MainUI.SliderControl.HasSeekedBeyondEnd = true;
                }
            }
            else
            {
                MainUI.SliderControl.HasSeekedBeyondEnd = false;
            }

            var seekEvent = new Event
            {
                Name = "Seek",
                EventArgs = new List<EventArg>() { { new EventArg() { Name = "NewPosition", Value = position.ToString()} } }
            };
            MainUI.Controller.SendEvent(seekEvent);
        }

        public void Seek(Command seekCommand)
        {
            if (seekCommand != null && seekCommand.Params != null)
            {
                if (seekCommand.ContainsParam("Position"))
                {
                    TimeSpan seekPosition;
                    if (TimeSpan.TryParse(seekCommand.GetParamValue("Position"), out seekPosition))
                    {
                        SeekToMediaPosition(seekPosition);
                    }
                }
            }
        }

        public void ResetToDefaultNode()
        {
            _videoNodesController.ResetToDefaultNode();
        }

        public void Play()
        {
            if (CurrentNode == null) return;
            MediaPlayer.Play();
        }

        public void Pause()
        {
            //CurrentNode.PlayingTime = MediaPlayer.Position;
            MainUI.PlayControl.StopState();
            MediaPlayer.Pause();
        }

        public void Stop()
        {
            MediaPlayer.Stop();
            MainUI.PlayControl.StopState();
            MediaPlayer.Position = new TimeSpan(0);
            MediaPlayer_CurrentStateChanged(this, new RoutedEventArgs());
        }

        public VideoNode CurrentNode
        {
            get
            {
                return _videoNodesController.CurrentNode;
            }

        }

        /*
         * Reset Media Player
         */
        private void ResetMediaPlayer()
        {
            MainUI.SliderControl.HasSeekedBeyondEnd = false;
            MediaPlayer.Stop();
            MediaPlayer.Markers.Clear();
            MediaPlayer.Source = null;
            MainUI.PlayControl.StopState();
        }

        public void Play(Command command)
        {
            if (command.ContainsParam("NodeId"))
            {
                Guid id;
                if (Guid.TryParse(command.GetParamValue("NodeId"), out id))
                {
                    if (CurrentNode != null && CurrentNode.NodeId.Equals(id))
                    {
                        HandleCurrentNodePlay(command); //the current playing node is the same one this play command came from
                    }
                    else
                    {
                        HandleNonCurrentNodePlay(command, id); //it was a different node to the currently playing node that the command came from
                    }
                }
                if (MediaPlayer.Source != null)
                    MainUI.UrlControl.Data.Source = MediaPlayer.Source.ToString();
            }
        }

        private void HandleNonCurrentNodePlay(Command command, Guid id)
        {
            var exist = _videoNodesController.GetVideoNodeById(id);
            if (exist == null)
            {
                var videoNode = new VideoNode(command);
                _videoNodesController.AddNode(videoNode); //adds and sets as current
            }
            else
            {
                exist.SetStartTimeAndEndTimeFromCommand(command);
                exist.SetSourceFromCommand(command);
                _videoNodesController.SetCurrentNode(exist);
            }
            PlayCurrentVideoNode(); //non current node becoming current video node
        }

        private void HandleCurrentNodePlay(Command command)
        {
            CurrentNode.SetStartTimeAndEndTimeFromCommand(command);
            CurrentNode.SetSourceFromCommand(command);
            MediaPlayer.Markers.Clear();
            if (CurrentNode.EndTimeCodeProvided)
            {
                MediaPlayer.Markers.Add(new TimelineMarker
                {
                    Time = CurrentNode.EndTime,
                    Text = "EndTimeCode",
                    Type = "EndTimeCode"
                });
            }
            if (MediaPlayer.Source == null || !MediaPlayer.Source.Equals(CurrentNode.Source))
            {
                //If not currently playing anything or playing a different source
                PlayCurrentVideoNode(); //different source to the current source
            }
            else if (MediaPlayer.Source.Equals(CurrentNode.Source))
            {
                //if it's the same video source (ie it's a resume from paused)
                if (CurrentNode.AutoPlay)
                {
                    if (!CurrentNode.EndTimeCodeProvided)
                    {
                        if (CurrentNode.StartTimeCodeProvided)
                        {
                            if (MediaPlayer.Position < CurrentNode.StartTime)
                            {
                                MediaPlayer.Position = CurrentNode.StartTime;
                            }
                        }
                        MediaPlayer.Play(); //resume
                    }
                    else
                    {
                        if (CurrentNode.StartTimeCodeProvided)
                        {
                            if (MediaPlayer.Position < CurrentNode.EndTime && MediaPlayer.Position > CurrentNode.StartTime)
                            {
                                MediaPlayer.Play();  //resume from paused in this same segment
                            }
                            else
                            {
                                MediaPlayer.Position = CurrentNode.StartTime; //there was a start time code provided so use it
                                MediaPlayer.Play(); //play from start time
                            }
                        }
                        else
                        {
                            if (MediaPlayer.Position < CurrentNode.EndTime)
                            {
                                MediaPlayer.Play(); //resume from paused in this same segment
                            }
                            else
                            {
                                MediaPlayer.Position = TimeSpan.Parse("00:00:00"); //no start time code was provided so assume start of video
                                MediaPlayer.Play(); //play from beginning
                            }
                        }
                    }
                }
                else
                {
                    MediaPlayer.Position = CurrentNode.StartTimeCodeProvided ? CurrentNode.StartTime : TimeSpan.Parse("00:00:00");
                    if (CurrentNode.EndTimeCodeProvided)
                    {
                        MediaPlayer.Markers.Add(new TimelineMarker
                        {
                            Time = CurrentNode.EndTime,
                            Text = "EndTimeCode",
                            Type = "EndTimeCode"
                        });
                    }
                    MediaPlayer.Pause();
                }
            }
            else
            {
                PlayCurrentVideoNode(); //not a possible scenario - TODO: review further and remove if this is the case
            }
        }

        private void PlayCurrentVideoNode()
        {
            MediaPlayer.Markers.Clear();
            
            // If have not set current node or current node has not set source, return
            if (CurrentNode == null || CurrentNode.Source == null)
            {
                return;
            }

            // If MediaPlayer current source does not equal to new source, reset MediaPlayer
            if (!CurrentNode.Source.Equals(MediaPlayer.Source))
            {
                MainUI.LoadingPanel.Show();
                MainUI.LoadingPanel.LeftBottomStatusText = "Connecting...";
                MainUI.LoadingPanel.RightBottomStatusText = string.Empty;
                ResetMediaPlayer();
                MediaPlayer.Source = CurrentNode.Source;
                if (MediaPlayer.AutoPlay)
                {
                    MediaPlayer.Pause();
                }
            }


            if (CurrentNode.StartTimeCodeProvided)
            {
                MediaPlayer.Position = CurrentNode.StartTime;
            }

            if (CurrentNode.EndTimeCodeProvided)
            {
                MediaPlayer.Markers.Add(new TimelineMarker
                {
                    Time = CurrentNode.EndTime,
                    Text = "EndTimeCode",
                    Type = "EndTimeCode"
                });
                if (!CurrentNode.StartTimeCodeProvided && MediaPlayer.Position > CurrentNode.EndTime)
                {
                    MediaPlayer.Position = TimeSpan.Parse("00:00:00");
                }
            }

            if (CurrentNode.AutoPlay)
            {
                MediaPlayer.Play();
            }
            else
            {
                MediaPlayer.Pause();
            }

            // Set UI
            MainUI.ErrorPanel.Visibility = Visibility.Collapsed;
            MediaPlayer.Visibility = Visibility.Visible;
        }

        private void SetMediaPlayToStart()
        {
            if (CurrentNode.StartTimeCodeProvided)
                SeekToMediaPosition(CurrentNode.StartTime);
            else
                MediaPlayer.Position = TimeSpan.Parse("00:00:00");
            //CurrentNode.PlayingTime = MediaPlayer.Position;
        }

        #region Node Changed Event
        private void VideoNodesControllerOnNodeChanged(object sender, NodeChangedEventArgs e)
        {
            var stateChangeEvent = new Event
            {
                Name = "CurrentStateChanged",
                EventArgs = new List<EventArg>()
            };
            stateChangeEvent.EventArgs.Add(new EventArg() { Name = "NodeId", Value = string.Format("{0}", e.Old) });
            stateChangeEvent.EventArgs.Add(new EventArg() { Name = "State", Value = "Idle" });
            MainUI.Controller.SendEvent(stateChangeEvent);
        }

        #endregion

        #region MediaPlayer Events

        private void MediaPlayer_DownloadProgressChanged(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.CurrentState == MediaElementState.Buffering || MediaPlayer.CurrentState == MediaElementState.Playing || MediaPlayer.CurrentState == MediaElementState.Paused)
            {
                MainUI.LoadingPanel.RightBottomStatusText = string.Format("Downloading {0:P0}", MediaPlayer.DownloadProgress);
            }
            else
            {
                MainUI.LoadingPanel.RightBottomStatusText = string.Empty;
            }
            MainUI.SliderControl.BufferValue.Width = MainUI.SliderControl.SliderValue.Width*MediaPlayer.DownloadProgress;
        }


        private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Markers.Clear();
            MainUI.SliderControl.Maximum = MediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
            var mediaOpenedEvent = new Event
            {
                Name = "MediaOpened",
                EventArgs = new List<EventArg> { { new EventArg() { Name = "Source", Value = MediaPlayer.Source.ToString()} } }
            };
            MainUI.Controller.SendEvent(mediaOpenedEvent);
            if (CurrentNode != null && CurrentNode.StartTimeCodeProvided)
            {
                SeekToMediaPosition(CurrentNode.StartTime);
                if (MediaPlayer.Markers.Count == 0 && CurrentNode.EndTimeCodeProvided)
                {
                    MediaPlayer.Markers.Add(new TimelineMarker
                    {
                        Time = CurrentNode.EndTime,
                        Text = "EndTimeCode",
                        Type = "EndTimeCode"
                    });
                }
            }

            if(MediaPlayer.CurrentState == MediaElementState.Playing || MediaPlayer.CurrentState == MediaElementState.Paused)
                MainUI.LoadingPanel.Close();
        }

        private void MediaPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MainUI.LoadingPanel.Close();
            MainUI.ErrorPanel.Visibility = Visibility.Visible;
            MediaPlayer.Visibility = Visibility.Collapsed;
            MediaPlayer.Source = null;
            var mediaFailedEvent = new Event
            {
                Name = "MediaFailedEvent",
                EventArgs = new List<EventArg> { {new EventArg() { Name = "ErrorExplanation", Value = e.ErrorException.Message} } }
            };
            MainUI.Controller.SendEvent(mediaFailedEvent);
        }

        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            var mediaEndedEvent = new Event { Name = "CurrentStateChanged", EventArgs = new List<EventArg>() };
            mediaEndedEvent.EventArgs.Add(new EventArg() { Name = "State", Value = "Idle" });
            if (CurrentNode != null)
            {
                mediaEndedEvent.EventArgs.Add(new EventArg() { Name = "NodeId", Value = CurrentNode.NodeId.ToString() });
            }
            MainUI.Controller.SendEvent(mediaEndedEvent);
            MainUI.PlayControl.StopState();
            SetMediaPlayToStart();
        }

        private void MediaPlayer_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (CurrentNode != null && CurrentNode.CurrentState != null && CurrentNode.CurrentState.GetState() != MediaPlayer.CurrentState)
            {
                var stateChangeEvent = new Event
                {
                    Name = "CurrentStateChanged",
                    EventArgs = new List<EventArg>()
                };
                if (CurrentNode != null)
                {
                    stateChangeEvent.EventArgs.Add(new EventArg() { Name = "NodeId", Value = CurrentNode.NodeId.ToString() });
                }
                if (CurrentNode != null && CurrentNode.CurrentState != null)
                {
                    CurrentNode.CurrentState.SetState(MediaPlayer.CurrentState);
                }
                switch (MediaPlayer.CurrentState)
                {
                    case MediaElementState.Playing:
                        if (MediaPlayer.Markers.Count == 0 && CurrentNode.EndTimeCodeProvided)
                        {
                            MediaPlayer.Markers.Add(new TimelineMarker
                            {
                                Time = CurrentNode.EndTime,
                                Text = "EndTimeCode",
                                Type = "EndTimeCode"
                            });
                        }
                        MainUI.LoadingPanel.Close();
                        MainUI.SliderControl.StartTimer();
                        MainUI.PlayControl.PlayState();
                        stateChangeEvent.EventArgs.Add(new EventArg() { Name = "State", Value = "Playing" });
                        break;
                    case MediaElementState.Buffering:
                        MainUI.LoadingPanel.Show();
                        MainUI.PlayControl.PlayState();
                        stateChangeEvent.EventArgs.Add(new EventArg() { Name = "State", Value ="Buffering"});
                        break;
                    case MediaElementState.Opening:
                        MainUI.LoadingPanel.Show();
                        MainUI.PlayControl.PlayState();
                        stateChangeEvent.EventArgs.Add(new EventArg() { Name = "State", Value ="Playing"});
                        break;
                    case MediaElementState.Closed:
                        MainUI.LoadingPanel.Close();
                        MainUI.PlayControl.StopState();
                        stateChangeEvent.EventArgs.Add(new EventArg() { Name = "State", Value ="Idle"});
                        break;
                    case MediaElementState.Stopped:
                        MainUI.LoadingPanel.Close();
                        MainUI.PlayControl.StopState();
                        stateChangeEvent.EventArgs.Add(new EventArg() { Name = "State", Value ="Idle"});
                        break;
                    case MediaElementState.Paused:
                        MainUI.LoadingPanel.Close();
                        MainUI.PlayControl.StopState();
                        stateChangeEvent.EventArgs.Add(new EventArg() { Name = "State", Value = "Idle"});
                        break;
                    default:
                        MainUI.LoadingPanel.Close();
                        stateChangeEvent.EventArgs.Add(new EventArg() { Name = "State", Value = "Idle" });
                        MainUI.SliderControl.StopTimer();
                        MainUI.PlayControl.StopState();
                        break;
                }
                
                MainUI.Controller.SendEvent(stateChangeEvent);
            }
        }

        private void MediaPlayer_BufferingProgressChanged(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.BufferingProgress.Equals(1))
            {
                MainUI.LoadingPanel.LeftBottomStatusText = string.Format("Buffering {0:P0}", 1);
                MainUI.LoadingPanel.Close();
            }
            else
            {
                MainUI.LoadingPanel.LeftBottomStatusText = string.Format("Buffering {0:P0}", MediaPlayer.BufferingProgress);
                MainUI.LoadingPanel.Show();
            }
        }

        private void MediaPlayer_MarkerReached(object sender, TimelineMarkerRoutedEventArgs e)
        {
            if (e.Marker.Type == "EndTimeCode")
            {
                MediaPlayer.Pause();
                SeekToMediaPosition(CurrentNode.StartTimeCodeProvided
                    ? CurrentNode.StartTime
                    : TimeSpan.Parse("00:00:00"));
                MediaPlayer_CurrentStateChanged(sender, new RoutedEventArgs());
                MainUI.PlayControl.StopState();
            }

            var markerEvent = new Event
            {
                Name = "TimelineMarkerEvent",
                EventArgs = new List<EventArg>
                {
                    {new EventArg() { Name="Text", Value=e.Marker.Text}},
                    {new EventArg() { Name="Time", Value=e.Marker.Time.TotalSeconds.ToString(CultureInfo.InvariantCulture)}},
                    {new EventArg() { Name="Type", Value=e.Marker.Type}}
                }
            };
            MainUI.Controller.SendEvent(markerEvent);
            //CurrentNode.PlayingTime = new TimeSpan();
        }

        #endregion
    }
}
