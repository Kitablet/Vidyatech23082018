using System;
using System.Threading.Tasks;
using Windows.Storage;
using FISE;
using FISE;
using Xamarin.Forms;
using Windows.Storage.Streams;
using Windows.System.Profile;

[assembly: Dependency(typeof(SaveAndLoad_WinApp))]

namespace FISE
{
    // https://msdn.microsoft.com/en-us/library/windows/apps/xaml/hh758325.aspx

    public class SaveAndLoad_WinApp : ISaveAndLoad
    {
        #region ISaveAndLoad implementation
        public string GetId()
        {
            
            IBuffer buffer = new Windows.Storage.Streams.Buffer(200);
            var token = HardwareIdentification.GetPackageSpecificToken(buffer);
            var hardwareId = token.Certificate;
            var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(hardwareId);
            byte[] bytes = new byte[hardwareId.Length];
            dataReader.ReadBytes(bytes);

            return BitConverter.ToString(bytes);
            // not working
            //return "";         
        }
        public async Task SaveTextAsync(string filename, string text)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(sampleFile, text);
        }

        public async Task<string> LoadTextAsync(string filename)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await storageFolder.GetFileAsync(filename);
            string text = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
            return text;
        }

        public bool FileExists(string filename)
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            try
            {
                localFolder.GetFileAsync(filename).AsTask().Wait();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
