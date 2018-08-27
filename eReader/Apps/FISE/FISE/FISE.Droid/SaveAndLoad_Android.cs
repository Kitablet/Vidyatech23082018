using System;
using Xamarin.Forms;
using Kitablet.Droid;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Java.Util;
using System.Text;
using Java.Net;
using System.Xml.Linq;
using System.Net;
using Android.Content.Res;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using PCLCrypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;

[assembly: Dependency(typeof(SaveAndLoad_Android))]
namespace Kitablet.Droid
{
    public class SaveAndLoad_Android : ISaveAndLoad
    {
        #region ISaveAndLoad implementation

        public string GetId()
        {
            try
            {
                string output = string.Empty;

                var all = Collections.List(NetworkInterface.NetworkInterfaces);
                //wlan0
                int cnt = 0;
                foreach (var iinterface in all)
                {

                    var macBytes = (iinterface as NetworkInterface).GetHardwareAddress();

                    if (macBytes == null) continue;

                    if ((iinterface as NetworkInterface).Name != "wlan0") continue;

                    var sb = new StringBuilder();

                    foreach (var b in macBytes)
                    {
                        sb.Append((b & 0xFF).ToString("X2") + ":");
                    }
                    output = sb.ToString().Remove(sb.Length - 1);
                }
                return output;
            }
            catch (Exception exd)
            {
                return "";
            }

        }

        public string PlatformName()
        {
            return "Android";
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
            var path = CreatePathToFile(filename);
            if(text== null)
            {
                text = "";
            }
            File.WriteAllBytes(path, Encoding.UTF8.GetBytes(text));
        }

        public string LoadText(string filename)
        {
            string result = null;
            try
            {
                var path = CreatePathToFile(filename);
                result = File.ReadAllText(path, Encoding.UTF8);
            }
            catch(Exception ex) {
            }
            return result;
        }

        public bool FileExists(string filename)
        {
            return File.Exists(CreatePathToFile(filename));
        }

        #endregion

        string CreatePathToFile(string filename)
        {
            try
            {
                var docsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                return Path.Combine(docsPath, filename);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public void saveCacheDetails(string id, string value)
        {







        }

        public void CreateFiles(string id)
        {

        }
           
        public void saveUserDetails(string id, User user)
        {
            string userString = JsonConvert.SerializeObject(user);
            Helpers.CacheSettings.SettingsKey = id;
            Helpers.CacheSettings.GeneralSettings = userString;
        }

        public string getCacheDetails(string id)
        {
            string value = string.Empty;

            return value;
        }

        public User getUserDetails(string id)
        {
            Helpers.CacheSettings.SettingsKey = "UserDetails";
            User user = new User();
            if (Helpers.CacheSettings.GeneralSettings != "")
            {
                user = JsonConvert.DeserializeObject<User>(Helpers.CacheSettings.GeneralSettings);
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
            return @"file:///android_asset/Content";
        }

        public Stream getStream(string file)
        {
            return Xamarin.Forms.Forms.Context.Assets.Open(file);
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
            return Resources.System.DisplayMetrics.HeightPixels;
        }
        public int GetDeviceWidth()
        {
            return Resources.System.DisplayMetrics.WidthPixels;
        }
        public float GetDeviceDensity()
        {
            return Resources.System.DisplayMetrics.Density;
        }

        public bool CheckFileExistence(string FilePath)
        {
            try
            {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var directoryPath = Path.Combine(documentsPath, FilePath);
                return File.Exists(directoryPath);
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool CheckDirectoryExistence(string FilePath)
        {
            try
            {
                var directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), FilePath);
                return Directory.Exists(directoryPath);
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public void DeleteDirectory(string FilePath)
        {
            var directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), FilePath);
            Directory.Delete(directoryPath, true);
        }

        public void DeleteFile(string FilePath)
        {
            try
            {
                if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), FilePath)))
                {
                    File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), FilePath));
                }
            }
            catch(Exception ex)
            {

            }
        }
        public bool FileUnzip(string FilePath)
        {
            try
            {
                string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal); ;
                var directoryPath = Path.Combine(localFolder, FilePath);
                if (File.Exists(directoryPath))
                {
                    string extractPath = Regex.Replace(directoryPath, ".zip", "/");

                    if (Directory.Exists(extractPath))
                    {
                        Directory.Delete(extractPath, true);
                    }

                    Decompress decompress = new Decompress(directoryPath, extractPath);
                    decompress.UnZip();
                    //ZipFile.ExtractToDirectory(directoryPath, extractPath);
                    //File.Delete(directoryPath);
                    ////FastZip zip = new FastZip();
                    ////zip.ExtractZip(directoryPath, extractPath, null);

                    //File.Delete(directoryPath);
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
            try
            {
                byte[] bytes = EncryptAes(File.ReadAllText(path), key);
                File.WriteAllText(path, Convert.ToBase64String(bytes, 0, bytes.Length));
            }
            catch(Exception ex) { }
            //string data = Crypto.Encrypt(File.ReadAllText(path), key, Constant.CryptoSalt);
            //File.WriteAllText(path, data);
        }

        public void DecryptFile(string path, string key)
        {
            try
            {
                string text = File.ReadAllText(path);
                byte[] buffer = Convert.FromBase64String(text);
                string data = DecryptAes(buffer, key);
                File.WriteAllText(path, data);
            }
            catch(Exception ex) { }          
            //string data = Crypto.Decrypt(File.ReadAllText(path), key, Constant.CryptoSalt);
            //File.WriteAllText(path, data);
        }

        public string LoadEncryptedFile(string path, string key)
        {
            try
            {
                string text = File.ReadAllText(path);
                byte[] buffer = Convert.FromBase64String(text);
                string data = DecryptAes(buffer, key);
                return data;
            }
            catch(Exception ex) { return string.Empty; }            
            //string data = Crypto.Decrypt(File.ReadAllText(path), key, Constant.CryptoSalt);
            //return data;
        }

        public string GetLocalLocalFolderPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        public string GetAshwid()
        {
            return "";
        }
        public string setPlateform()
        {
            return "Android";
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
        public string getDeviceName()
        {
            string devicename = string.Empty;
            try
            {
                //var deviceInfo = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();
                //devicename = deviceInfo.FriendlyName;
            }
            catch (Exception ex) { }
            return devicename;
        }


        #region encryption/decryption provider
        public string DeriveKey(string key)
        {
            try
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
            catch(Exception ex)
            {
                return string.Empty;
            }         
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