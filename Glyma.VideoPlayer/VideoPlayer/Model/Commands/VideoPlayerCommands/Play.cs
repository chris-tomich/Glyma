using System;
using System.Collections.Generic;

namespace VideoPlayer.Model.Commands.VideoPlayerCommands
{
    internal class Play : VideoPlayerCommand
    {
        public Play(IDictionary<string, string> para) : base(para) { }

        public Guid NodeId { get; set; }
        public Uri Source { get; set; }
        public TimeSpan StartTime { get; set; }
        public bool StartTimeCodeProvided { get; set; }
        public bool EndTimeCodeProvided { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool AutoPlay { get; set; }

        public void Execute()
        {
            if(Parameters == null) throw new ArgumentNullException(string.Format("Play Command Parameter Lost"));
            if (Parameters.ContainsKey("NodeId"))
            {
                Guid nodeId;
                if (Guid.TryParse(Parameters["NodeId"], out nodeId))
                {
                    NodeId = nodeId;
                }
            }
            if (Parameters.ContainsKey("Source"))
            {
                Uri source;
                Uri.TryCreate(Parameters["Source"], UriKind.Absolute, out source);
                Source = source;
            }
            if (Parameters.ContainsKey("StartTimeCode"))
            {
                TimeSpan startTimeCode;
                StartTimeCodeProvided = TimeSpan.TryParse(Parameters["StartTimeCode"], out startTimeCode);
                StartTime = startTimeCode;
            }

            if (Parameters.ContainsKey("EndTimeCode"))
            {
                TimeSpan endTimeCode;
                EndTimeCodeProvided = TimeSpan.TryParse(Parameters["EndTimeCode"], out endTimeCode);
                EndTime = endTimeCode;
            }

            if (Parameters.ContainsKey("AutoPlay"))
            {
                bool autoPlay;
                Boolean.TryParse(Parameters["AutoPlay"], out autoPlay);
                AutoPlay = autoPlay;
            }
            else
            {
                AutoPlay = true;
            }
        }

    }
}
