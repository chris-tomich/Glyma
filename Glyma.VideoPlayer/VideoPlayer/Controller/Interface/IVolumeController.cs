namespace VideoPlayer.Controller.Interface
{
    public interface IVolumeController
    {
        void SetVolumeTo(double volume);
        void Mute();
        bool IsMuted();
    }
}
