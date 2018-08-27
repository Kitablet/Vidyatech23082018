using System;
using Android.Media;
using Xamarin.Forms;
using Kitablet.Droid;
using Kitablet.ViewModels;

[assembly: Dependency(typeof(AudioService))]
namespace Kitablet.Droid
{
    public class AudioService : IAudio
    {
        #region Audio Function
        private MediaPlayer _mediaElement = null;
        bool AutoPlay;
        string filePath;

        public void Load(string fileName, bool AutoPlay)
        {
            if (_mediaElement != null)
            {
                _mediaElement.Prepared -= _mediaElement_Prepared;
                _mediaElement.Stop();
                _mediaElement.Release();
                filePath = string.Empty;
                _mediaElement = null;
            }

            filePath = "file://" + Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/" + fileName;           

            if (_mediaElement == null)
            {
                _mediaElement = new MediaPlayer();
            }
            this.AutoPlay = AutoPlay;
            _mediaElement.Reset();
            _mediaElement.SetVolume(1.0f, 1.0f);
            _mediaElement.SetDataSource(filePath);
            _mediaElement.PrepareAsync();
            _mediaElement.Prepared += _mediaElement_Prepared;
        }

        private void _mediaElement_Prepared(object sender, EventArgs e)
        {
            if (this.AutoPlay)
            {
                _mediaElement?.Start();
            }           
        }

        public void Play()
        {
            _mediaElement?.Start();
        }

        public void Pause()
        {
            if(_mediaElement.IsPlaying) {
                _mediaElement?.Pause();
            }           
        }

        public void Stop()
        {
            _mediaElement?.Stop();
        }

        public string GetMediaState()
        {
            if (_mediaElement != null && !_mediaElement.IsPlaying)
            {
                return "Paused";
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion
    }
}