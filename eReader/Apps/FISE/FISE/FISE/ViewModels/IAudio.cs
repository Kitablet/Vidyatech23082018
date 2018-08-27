namespace Kitablet.ViewModels
{
    public interface IAudio
    {
        void Load(string fileName, bool AutoPlay);
        void Play();
        void Pause();
        void Stop();
        string GetMediaState();
    }
}
