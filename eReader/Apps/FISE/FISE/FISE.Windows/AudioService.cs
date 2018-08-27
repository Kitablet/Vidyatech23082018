using Kitablet.ViewModels;
using Kitablet.Windows;
using System;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioService))]
namespace Kitablet.Windows
{
    public class AudioService : IAudio
    {
        #region Audio Function
        private MediaElement _mediaElement = null;

        public async void Load(string fileName, bool AutoPlay)
        {
            global::Windows.Storage.StorageFolder folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(Path.GetDirectoryName(fileName));
            global::Windows.Storage.StorageFile file = await folder.GetFileAsync(Path.GetFileName(fileName));
            global::Windows.Storage.Streams.IRandomAccessStream stream = await file.OpenAsync(global::Windows.Storage.FileAccessMode.Read);

            if (_mediaElement != null)
            {
                _mediaElement.Stop();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(_mediaElement);
                _mediaElement = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            _mediaElement = new MediaElement
            {
                Name = fileName,
                IsLooping = false,
                IsMuted = false,
                AutoPlay = AutoPlay,
                Position = new TimeSpan(0, 0, 0),
                Volume = 1
            };

            _mediaElement.Name = Path.GetFileName(fileName);
            _mediaElement.SetSource(stream, file.ContentType);
        }

        public void Play()
        {
            _mediaElement?.Play();          
        }

        public void Pause()
        {
            _mediaElement?.Pause();
        }

        public void Stop()
        {
            if (_mediaElement != null)
            {
                _mediaElement.Stop();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(_mediaElement);
                _mediaElement = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public string GetMediaState()
        {
            if (_mediaElement != null)
            {
                return _mediaElement.CurrentState.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion
    }
}
