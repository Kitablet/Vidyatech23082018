using System.IO;
using System.Threading.Tasks;

namespace Kitablet
{
    public interface ISaveAndLoad
    {
        void SaveText(string filename, string text);
        string LoadText(string filename);
        Task SaveTextAsync(string filename, string text);
        Task<string> LoadTextAsync(string filename);
        bool FileExists(string filename);
        string GetId();
        void saveCacheDetails(string id, string value);
        string getCacheDetails(string id);
        void saveUserDetails(string id, User user);
        User getUserDetails(string id);
        void deleteKey(string id);
        int GetDeviceHeight();
        int GetDeviceWidth();
        float GetDeviceDensity();
        bool DownloadAndSaveFile(string FileUrl, string DirStruc, string FileName);
        bool CheckFileExistence(string FilePath);
        bool CheckDirectoryExistence(string FilePath);
        bool FileUnzip(string FilePath);
        void EncryptFile(string path, string key);
        void DecryptFile(string path, string key);
        bool BookXmlFilesOperation(string bookPath, string type, string key);
        string LoadEncryptedFile(string path, string key);
        string getBasePath();
        string GetLocalLocalFolderPath();
        string GetAshwid();
        void DeleteDirectory(string FilePath);
        void DeleteFile(string FilePath);
        bool SaveBookDownload(string BookID, string fileName, MemoryStream stream);
        bool MergeFile(string BookID);
        string SetAndGetBackCover(string BookID, string file);
        string setPlateform();
        string getDeviceName();
        string PlatformName();
        string ComputeMD5(byte[] content);
        string ComputeHmacSha256(byte[] secretKeyByteArray, byte[] signature);
    }
}


