using VideoPlayerSharedLib;

namespace VideoPlayer.Controller.Interface
{
    public interface IMediaControllerBase
    {
        void Play();
        void Pause();
        void Stop();
        void SeekBySeconds(int seconds);
    }
}
