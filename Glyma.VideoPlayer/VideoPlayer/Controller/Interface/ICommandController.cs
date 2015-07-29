using VideoPlayerSharedLib;

namespace VideoPlayer.Controller.Interface
{
    internal interface ICommandController
    {
        void Play(Command command);
        void Pause();
        void Stop();
        void Seek(Command command);
        void Mute(Command command );
        void Volume(Command command);
        void PositionRequest(Command command);
        void VolumeRequest();
        void IsMuted();
        void SourceRequest(Command command);
        void SourceAndPositionRequest(Command command);
        void StateRequest(Command command);
    }
}
