using System;

namespace VideoPlayer.Controller.Interface
{
    public interface ITimeLineSliderController
    {
        void SeekPosition(TimeSpan sp);
        TimeSpan GetPosition();
        void ResetToDefaultNode();
    }
}
