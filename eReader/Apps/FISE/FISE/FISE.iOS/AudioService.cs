using AudioToolbox;
using AVFoundation;
using FISE.iOS;
using FISE.ViewModels;
using Foundation;
using System;
using System.Diagnostics;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioService))]
namespace FISE.iOS
{
    public class AudioService : IAudio
    {
        #region Audio Function
        private AVAudioPlayer _mediaElement = null;
        private NSUrl fileURL;
        string fullPath;
        bool _AutoPlay;
        bool mediaFinish = false;
        public void Load(string fileName, bool AutoPlay)
        {

            if (_mediaElement != null)
            {
                _mediaElement.Stop();
                _mediaElement.Dispose();
                fullPath = string.Empty;
                _mediaElement = null;
            }
            mediaFinish = false;

            fullPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/" + fileName;
            this._AutoPlay = AutoPlay;
            if (_mediaElement == null)
            {
                try
                {
                    InitializePlayer(fullPath);
                }
                catch (Exception ex)
                { }
            }
        }
        private void InitializePlayer(string sUrl)
        {
            fileURL = new NSUrl(sUrl);
            var mp3 = AudioFile.Open(fileURL.AbsoluteUrl, AudioFilePermission.Read, AudioFileType.MP3);
            if (mp3 != null)
            {
                _mediaElement = AVAudioPlayer.FromUrl(fileURL);
                _mediaElement.FinishedPlaying += _mediaElement_FinishedPlaying;
                if (_AutoPlay && _mediaElement != null)
                {
                    _mediaElement.Play();
                }
            }
            else
            {
                Debug.WriteLine("File could not be loaded: {0}", fileURL.FilePathUrl);
            }
        }
        private void _mediaElement_FinishedPlaying(object sender, AVStatusEventArgs e)
        {
            try
            {
                if (_mediaElement != null)
                {
                    mediaFinish = true;
                    _mediaElement.Stop();
                    _mediaElement = null;
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void Play()
        {
            if (_mediaElement != null)
            {
                _mediaElement.Play();
            }
        }
        public void Pause()
        {
            if (_mediaElement != null && _mediaElement.Playing)
                _mediaElement.Pause();
        }
        public void Stop()
        {
            if (_mediaElement != null)
            {
                _mediaElement.Stop();
            }
        }
        public string GetMediaState()
        {
            try
            {
                if (_mediaElement != null && !_mediaElement.Playing)
                {
                    return "Paused";
                }
                else if (mediaFinish)
                {
                    return "Stopped";
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex) { return string.Empty; }
        }
        #endregion
    }
}