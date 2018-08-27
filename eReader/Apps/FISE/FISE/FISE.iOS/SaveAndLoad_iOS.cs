using System;
using Xamarin.Forms;
using FISE.iOS;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Text;
using Newtonsoft.Json;
using System.Xml.Linq;
using UIKit;
using PCLCrypto;
using Foundation;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;

[assembly: Dependency(typeof(SaveAndLoad_iOS))]

namespace FISE.iOS
{
    public class SaveAndLoad_iOS : ISaveAndLoad
    {
        #region ISaveAndLoad implementation

        public string GetId()
        {
            string output = string.Empty;
            try
            {
                var nsUid =  UIDevice.CurrentDevice.IdentifierForVendor;
                var guidElements = nsUid.AsString();
                output = guidElements;
            }
            catch (Exception exd)
            {
               
            }
            return output;
        }

        public async Task SaveTextAsync(string filename, string text)
        {
            var path = CreatePathToFile(filename);
            using (StreamWriter sw = File.CreateText(path))
                await sw.WriteAsync(text);
        }

        public async Task<string> LoadTextAsync(string filename)
        {
            var path = CreatePathToFile(filename);
            using (StreamReader sr = File.OpenText(path))
                return await sr.ReadToEndAsync();
        }

        public void SaveText(string filename, string text)
        {
            try
            {
                var path = CreatePathToFile(filename);
                File.WriteAllBytes(path, Encoding.UTF8.GetBytes(text));
            }
            catch(Exception ex)
            { }
        }

        public string LoadText(string filename)
        {
            var path = CreatePathToFile(filename);
            string textFromFile = File.ReadAllText(path, Encoding.UTF8);
            return textFromFile;
        }

        public bool FileExists(string filename)
        {
            return File.Exists(CreatePathToFile(filename));
        }

        #endregion

        string CreatePathToFile(string filename)
        {
            var docsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(docsPath, filename);
        }
        public void saveCacheDetails(string id, string value)
        {
        }

        public bool SaveBookDownload(string BookID, string fileName, MemoryStream stream)
        {
            try
            {
                string localFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string filePath = Path.Combine(localFolderPath, Constant.FileDirectoryStructure);
                //string filePath = Path.Combine(localFolderPath, Constant.FileDirectoryStructure, Constant.FileNameInitials + BookID);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                if (File.Exists(Path.Combine(filePath, fileName)))
                {
                    File.Delete(Path.Combine(filePath, fileName));
                }

                using (FileStream fileStream = File.Create(Path.Combine(filePath, fileName)))
                {
                    stream.WriteTo(fileStream);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public void CreateFiles(string id)
        {

        }
        public string getDeviceName()
        {
            string devicename = string.Empty;
            try
            {
                var deviceInfo = UIDevice.CurrentDevice.Name;
                devicename = deviceInfo;
            }
            catch (Exception ex) { }
            return devicename;
        }
        public void saveUserDetails(string id, User user)
        {
            try
            {
                string userString = JsonConvert.SerializeObject(user);
                FISE.Helpers.CacheSettings.SettingsKey = id;
                FISE.Helpers.CacheSettings.GeneralSettings = userString;
            }
            catch(Exception ex)
            { }
        }

        public string getCacheDetails(string id)
        {
            string value = string.Empty;

            return value;
        }

        public User getUserDetails(string id)
        {
            FISE.Helpers.CacheSettings.SettingsKey = "UserDetails";
            User user = new User();
            if (FISE.Helpers.CacheSettings.GeneralSettings != "")
            {
                user = JsonConvert.DeserializeObject<User>(FISE.Helpers.CacheSettings.GeneralSettings);
            }
            return user;
        }

        public void deleteKey(string id)
        {

        }
        public String getContentPath()
        {
            return "Content";
        }

        public String getBasePath()
        {
            string path = Path.Combine( NSBundle.MainBundle.BundlePath, "Content");
            bool re = File.Exists(path + "/PrivacyPolicy/privacy.html");
            return path;
        }

        public Stream getStream(string file)
        {
            System.IO.Stream stream = new System.IO.MemoryStream();
            return stream;
        }

        public void SaveFile(XDocument doc)
        {
            doc.Save(Path.Combine("Content", "Books.xml"));
        }

        public bool DownloadAndSaveFile(string FileUrl, string DirStruc, string FileName)
        {
            try
            {
                var webClient = new WebClient();
                var url = new System.Uri(FileUrl);
                var bytes = webClient.DownloadData(url);
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var directoryPath = Path.Combine(documentsPath, DirStruc);
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
                string localPath = Path.Combine(directoryPath, FileName);
                File.WriteAllBytes(localPath, bytes);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public int GetDeviceHeight()
        {
            int h = (int)UIScreen.MainScreen.Bounds.Height;
            return h;            
        }
        public int GetDeviceWidth()
        {
            int w = (int)UIScreen.MainScreen.Bounds.Width;
            return w;
        }
        public float GetDeviceDensity()
        {
           double density1 = UIScreen.MainScreen.Scale;
           float density = (float)UIScreen.MainScreen.Scale;
           return density;
        }

        public bool CheckFileExistence(string FilePath)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var directoryPath = Path.Combine(documentsPath, FilePath);
            bool isExist = File.Exists(directoryPath);
            return isExist;
        }

        public bool CheckDirectoryExistence(string FilePath)
        {
            var directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), FilePath);
            bool isExist = Directory.Exists(directoryPath);
            return isExist;
        }

        public void DeleteDirectory(string FilePath)
        {
            var directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), FilePath);
            Directory.Delete(directoryPath, true);
        }

        public void DeleteFile(string FilePath)
        {
            if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), FilePath)))
            {
                File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), FilePath));
            }
        }

        public bool FileUnzip(string FilePath)
        {
            try
            {
                string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal); ;
                var directoryPath = Path.Combine(localFolder, FilePath);
                bool IsExist = File.Exists(directoryPath);
                if (IsExist)
                {
                    string extractPath = Regex.Replace(directoryPath, ".zip", "");

                    if (Directory.Exists(extractPath))
                    {
                        Directory.Delete(extractPath, true);
                    }
                    Decompress decompress = new Decompress(directoryPath, extractPath);
                    decompress.UnZip();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool BookXmlFilesOperation(string bookPath, string type, string key)
        {
            try
            {
                string xmlpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), bookPath, "EPUB", "package.opf");

                if (File.Exists(xmlpath))
                {
                    XDocument xldoc = XDocument.Parse(File.ReadAllText(xmlpath));

                    XElement spineElement = xldoc.Root.Descendants().Where(x => x.Name.LocalName == "spine").ToList().FirstOrDefault();
                    foreach (XElement element in spineElement.Elements())
                    {
                        string idrefValue = element.Attribute("idref").Value.ToString();

                        XElement itemElement = xldoc.Root.Descendants().Where(x => x.Name.LocalName == "manifest").FirstOrDefault().Descendants().Where(e => e.Attribute("id").Value == idrefValue).FirstOrDefault();

                        string folPath = Path.GetDirectoryName(xmlpath);

                        string filePath = Path.Combine(folPath, itemElement.Attribute("href").Value.ToString());

                        if (type == "encrypt")
                        {
                            EncryptFile(filePath, key);
                        }
                        else if (type == "decrypt")
                        {
                            DecryptFile(filePath, key);
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void EncryptFile(string path, string key)
        {
            byte[] bytes = EncryptAes(File.ReadAllText(path), key);
            File.WriteAllText(path, Convert.ToBase64String(bytes, 0, bytes.Length));
        }

        public void DecryptFile(string path, string key)
        {
            string text = File.ReadAllText(path);
            byte[] buffer = Convert.FromBase64String(text);
            string data = DecryptAes(buffer, key);
            File.WriteAllText(path, data);
        }

        public string LoadEncryptedFile(string path, string key)
        {
            string text = File.ReadAllText(path);
            byte[] buffer = Convert.FromBase64String(text);
            string data = DecryptAes(buffer, key);
            return data;
        }

        public string GetLocalLocalFolderPath()
        {
            string LocalFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return LocalFolderPath;
        }

        public string GetAshwid()
        {
            string output = string.Empty;
            try
            {
                var nsUid = UIDevice.CurrentDevice.IdentifierForVendor;
                var guidElements = nsUid.AsString();
                output = guidElements;
            }
            catch (Exception exd)
            {}
            return output;
        }
        public string setPlateform()
        {
            return "iOS";
        }

        public bool MergeFile(string BookID)
        {
            return false;
        }

        public string SetAndGetBackCover(string BookID, string file)
        {
            string localPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = localPath + "/" + file.Replace("\\", "/");
            string backcoverName = file.Substring(file.LastIndexOf("\\") + 1);
            string FileName = Path.GetFileNameWithoutExtension(backcoverName) + "_" + BookID + Path.GetExtension(backcoverName);
            string CoverFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "BackCovers");
            string alternateCover = CoverFolder + "/" + FileName;

            if (!Directory.Exists(CoverFolder))
            {
                Directory.CreateDirectory(CoverFolder);
            }
            try
            {
                if (File.Exists(filePath))
                {
                    File.Copy(filePath, Path.Combine(CoverFolder, FileName), true);
                    return Path.Combine(CoverFolder, FileName);
                }
                else
                {
                    if (File.Exists(alternateCover))
                    {
                        return alternateCover;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public string PlatformName()
        {
            return "IOS";
        }

        #region encryption/decryption provider
        public string DeriveKey(string key)
        {
            string derivedKey = string.Empty;
            if (key.Length > 16)
            {
                // Cut of the end if it exceeds 16 characters
                key = key.Substring(0, 16);
            }
            else
            {
                // Append zero to make it 16 characters if the provided key is less
                while (key.Length < 16)
                {
                    key += "0";
                }
            }
            derivedKey = key;
            return derivedKey;
        }
        public byte[] EncryptAes(string data, string password)
        {
            byte[] key = Encoding.UTF8.GetBytes(DeriveKey(password));

            ISymmetricKeyAlgorithmProvider aes = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
            ICryptographicKey symetricKey = aes.CreateSymmetricKey(key);
            var bytes = WinRTCrypto.CryptographicEngine.Encrypt(symetricKey, Encoding.UTF8.GetBytes(data));
            return bytes;
        }

        public string DecryptAes(byte[] data, string password)
        {
            byte[] key = Encoding.UTF8.GetBytes(DeriveKey(password));

            ISymmetricKeyAlgorithmProvider aes = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithm.AesCbcPkcs7);
            ICryptographicKey symetricKey = aes.CreateSymmetricKey(key);
            var bytes = WinRTCrypto.CryptographicEngine.Decrypt(symetricKey, data);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }
        #endregion encryption/decryption provider

        #region for message digest and sha256
        public string ComputeMD5(byte[] content)
        {
            MD5Digest md5 = new MD5Digest();
            md5.Reset();
            md5.BlockUpdate(content, 0, content.Length);
            int length = md5.GetDigestSize();
            byte[] md5data = new byte[length];
            md5.DoFinal(md5data, 0);
            return Convert.ToBase64String(md5data);
        }
        public string ComputeHmacSha256(byte[] secretKeyByteArray, byte[] signature)
        {
            HmacSha256 hmac = new HmacSha256(secretKeyByteArray);
            byte[] signatureBytes = hmac.ComputeHash(signature);
            return Convert.ToBase64String(signatureBytes);
        }

        public class HmacSha256
        {
            private readonly HMac _hmac;

            public HmacSha256(byte[] key)
            {
                _hmac = new HMac(new Sha256Digest());
                _hmac.Init(new KeyParameter(key));
            }

            public byte[] ComputeHash(byte[] value)
            {
                if (value == null) throw new ArgumentNullException("value");

                byte[] resBuf = new byte[_hmac.GetMacSize()];
                _hmac.BlockUpdate(value, 0, value.Length);
                _hmac.DoFinal(resBuf, 0);

                return resBuf;
            }
        }
        #endregion
    }
}