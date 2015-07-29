using System;

namespace Glyma.UtilityService.Export.Common.Model
{
    public class NodeVideo
    {
        public bool HasVideo { get; private set; }

        public string Source { get; private set; }

        public TimeSpan? StartPosition { get; private set; }


        public TimeSpan? EndPosition { get; private set; }


        public string Link { get; set; }


        public NodeVideo(string source, string start, string end)
        {
            if (!string.IsNullOrEmpty(source))
            {
                Source = source;
                HasVideo = true;
                TimeSpan startPosition, endPosition;
                if (TimeSpan.TryParse(start, out startPosition))
                {
                    StartPosition = startPosition;
                }

                if (TimeSpan.TryParse(end, out endPosition))
                {
                    EndPosition = endPosition;
                }

                Link = CreateLink();
            }
        }

        private string CreateLink()
        {
            var link = Source;
            if (HasVideo)
            {
                if (!Source.Contains("?"))
                {
                    link = link + "?";
                }
                if (StartPosition.HasValue)
                {
                    link = link + "&start=" + StartPosition.Value.TotalSeconds;
                }

                if (EndPosition.HasValue)
                {
                    link = link + "&end=" + EndPosition.Value.TotalSeconds;
                }
            }
            return link;
        }
    }
}
