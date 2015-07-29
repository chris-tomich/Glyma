using System;
using VideoPlayer.Model.Node.State;

namespace VideoPlayer.Model.Node
{
    public class VideoNode : IVideoNode
    {
        public Guid NodeId { get; private set; }

        public VideoNodeState CurrentState { get; set; }

        public bool AutoPlay { get; set; }
        public Uri Source { get; set; }

        public TimeSpan EndTime { get; set; }
        public bool EndTimeCodeProvided { get; set; }
        public TimeSpan StartTime { get; set; }
        public bool StartTimeCodeProvided { get; set; }
        //public TimeSpan PlayingTime { get; set; }

        public VideoNode(VideoPlayerSharedLib.Command command)
        {
            InitializeVideoNode(command);
            //PlayingTime = TimeSpan.Parse("00:00:00");
            SetSourceFromCommand(command);
        }

        public void UpdatePlayingTime(TimeSpan sp)
        {
            //PlayingTime = sp;
        }

        #region Initialize Video Node
        private void InitializeVideoNode(VideoPlayerSharedLib.Command command)
        {
            InitializeState();
            if (command.ContainsParam("NodeId"))
            {
                Guid nodeId;
                if (Guid.TryParse(command.GetParamValue("NodeId"), out nodeId))
                {
                    NodeId = nodeId;
                }
            }
            SetStartTimeAndEndTimeFromCommand(command);

        }

        public void SetSourceFromCommand(VideoPlayerSharedLib.Command command)
        {
            if (command.ContainsParam("Source"))
            {
                Uri source;
                Uri.TryCreate(command.GetParamValue("Source"), UriKind.Absolute, out source);
                Source = source;
            }
        }

        public void SetStartTimeAndEndTimeFromCommand(VideoPlayerSharedLib.Command command)
        {


            if (command.ContainsParam("StartTimeCode"))
            {
                TimeSpan startTimeCode;
                StartTimeCodeProvided = TimeSpan.TryParse(command.GetParamValue("StartTimeCode"), out startTimeCode);
                StartTime = startTimeCode;
            }

            if (command.ContainsParam("EndTimeCode"))
            {
                TimeSpan endTimeCode;
                EndTimeCodeProvided = TimeSpan.TryParse(command.GetParamValue("EndTimeCode"), out endTimeCode);
                EndTime = endTimeCode;
            }

            if (command.ContainsParam("AutoPlay"))
            {
                bool autoPlay;
                Boolean.TryParse(command.GetParamValue("AutoPlay"), out autoPlay);
                AutoPlay = autoPlay;
            }
            else
            {
                AutoPlay = true;
            }
        }

        private void InitializeState()
        {
            var factory = new VideoNodeStateFactory();
            var first = new OpeningState(factory, this);
            CurrentState = factory.GetOrCreate(delegate { return first; });
        }
        #endregion

    }
}
