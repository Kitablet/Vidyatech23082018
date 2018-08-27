using Kitablet.UWP;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Xamarin.Forms;

[assembly: Dependency(typeof(SaveAndLoad))]
namespace Kitablet.UWP
{

    public class SaveAndLoad : ISaveAndLoad
    {
        #region ISaveAndLoad implementation
        public string GetId()
        {
            try
            {
                return GetAshwid();
            }
            catch (Exception ex)
            {
                return "Error";
            }
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

        public void SaveText(string filename, string text)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            if (text == null)
                text = "";
            File.WriteAllBytes(Path.Combine(localFolder.Path, filename), Encoding.UTF8.GetBytes(text));
        }

        public string LoadText(string filename)
        {
            string result = null;
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                result = File.ReadAllText(Path.Combine(storageFolder.Path, filename), Encoding.UTF8);
            }
            catch (Exception ex)
            {                                
            }
            return result;
        }

        public bool FileExists(string filename)
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            return File.Exists(Path.Combine(localFolder.Path, filename));
        }

        public String getContentPath()
        {
            return "Content";
        }
        public String getBasePath()
        {
            return "Content";
        }
        public Stream getStream(string file)
        {
            return File.OpenRead(file);
        }
        #endregion
        #region For Cache
        public void saveCacheDetails(string id, string value)
        {
            try
            {
                var roamingSettings = ApplicationData.Current.LocalSettings;
                if(roamingSettings.Values[id] == null)
                {
                    roamingSettings.Values[id] = value;
                }
                else
                {
                    roamingSettings.Values.Remove(id);
                    roamingSettings.Values[id] = value;
                }
            }
            catch (Exception ex)
            {

            }

        }
        public void saveUserDetails(string id, User user)
        {
            try
            {
                Windows.Storage.ApplicationDataContainer localSettings =
                    Windows.Storage.ApplicationData.Current.LocalSettings;
                ApplicationDataCompositeValue composite = new ApplicationDataCompositeValue();
                if (user != null)
                {
                    composite["FirstName"] = user.FirstName;
                    composite["LastName"] = user.LastName;
                    composite["SubSection"] = user.SubSection;
                    composite["AvatarId"] = user.AvatarId;
                    composite["AvatarImage"] = user.AvatarImage;
                    composite["SchoolName"] = user.SchoolName;
                    composite["Grade"] = user.Grade;
                    composite["UserId"] = user.UserId;
                    composite["SubSectionId"] = user.SubSectionId;
                    composite["Role"] = user.Role;
                }
                else
                {
                    if (localSettings.Values[id] != null)
                    {
                        localSettings.Values.Remove(id);
                    }
                }

                if (localSettings.Values[id] == null)
                {
                    localSettings.Values[id] = composite;
                }
                else
                {
                    localSettings.Values.Remove(id);
                    localSettings.Values[id] = composite;
                }
            }
            catch (Exception ex)
            {

            }
        }
        public string getCacheDetails(string id)
        {
            string value = string.Empty;
            try
            {
                var roamingSettings = ApplicationData.Current.LocalSettings;
                value = roamingSettings.Values[id].ToString();
            }
            catch (Exception ex)
            {

            }
            return value;
        }
        public User getUserDetails(string id)
        {
            User user = new User();
            Windows.Storage.ApplicationDataContainer localSettings =
                 Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue composite =
               (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values[id];
            try{
                if (composite != null)
                {
                    user.FirstName = composite["FirstName"].ToString();
                    user.LastName = composite["LastName"].ToString();
                    user.SchoolName = composite["SchoolName"].ToString();
                    user.SubSection = composite["SubSection"].ToString();
                    user.AvatarId = int.Parse(composite["AvatarId"].ToString());
                    user.AvatarImage = composite["AvatarImage"].ToString();
                    user.Grade = composite["Grade"].ToString();
                    user.UserId = int.Parse(composite["UserId"].ToString());
                    user.SubSectionId = int.Parse(composite["SubSectionId"].ToString());
                    user.AvatarImage = composite["AvatarImage"].ToString();
                    user.Role = composite["Role"].ToString();
                }
            }
            catch(Exception  ex)
            {

            }
            return user;
        }
        public void deleteKey(string id)
        {
            try
            {
                var roamingSettings = ApplicationData.Current.LocalSettings;
                roamingSettings.Values.Remove(id);
            }
            catch(Exception ex)
            {
                
            }
        }
        #endregion
        public bool DownloadAndSaveFile(string FileUrl, string DirStruc, string FileName)
        {
            try
            {
                var client = new HttpClient();
                var url = new System.Uri(FileUrl);
                byte[] result = client.GetByteArrayAsync(url).Result;
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                var directoryPath = Path.Combine(localFolder.Path, DirStruc);
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
                string localPath = Path.Combine(directoryPath, FileName);
                File.WriteAllBytes(localPath, result);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public string PlatformName()
        {
            return "Windows";
        }
        public int GetDeviceHeight()
        {
            return (int)Window.Current.Bounds.Height;
        }

        public int GetDeviceWidth()
        {
            return (int)Window.Current.Bounds.Width;
        }

        public float GetDeviceDensity()
        {
            return Windows.Graphics.Display.DisplayProperties.LogicalDpi;
        }

        public double GetDeviceScale()
        {
            double scale = 100;
            try
            {
                scale = Convert.ToDouble(Windows.Graphics.Display.DisplayProperties.ResolutionScale);
            }
            catch (Exception)
            {

            }
            return scale;
        }

        public bool CheckFileExistence(string FilePath)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            var directoryPath = Path.Combine(localFolder.Path, FilePath);
            return File.Exists(directoryPath);
        }

        public bool CheckDirectoryExistence(string FilePath)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            var directoryPath = Path.Combine(localFolder.Path, FilePath);
            return Directory.Exists(directoryPath);
        }

        public void DeleteDirectory(string FilePath)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            var directoryPath = Path.Combine(localFolder.Path, FilePath);
            Directory.Delete(directoryPath, true);
        }

        public void DeleteFile(string FilePath)
        {
            if (File.Exists(Path.Combine(ApplicationData.Current.LocalFolder.Path, FilePath)))
            {
                File.Delete(Path.Combine(ApplicationData.Current.LocalFolder.Path, FilePath));
            }           
        }

        public bool FileUnzip(string FilePath)
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                var directoryPath = Path.Combine(localFolder.Path, FilePath);
                if (File.Exists(directoryPath))
                {
                    string extractPath = Regex.Replace(directoryPath, ".zip", "");

                    if (Directory.Exists(extractPath))
                    {
                        Directory.Delete(extractPath, true);
                    }

                    ZipFile.ExtractToDirectory(directoryPath, extractPath);
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
                string xmlpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, bookPath, "EPUB", "package.opf");
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
            //byte[] bytes = Crypto.EncryptAes(File.ReadAllText(path), key, Constant.CryptoSalt);
            //File.WriteAllText(path, Convert.ToBase64String(bytes, 0, bytes.Length));
            string data = Encrypt(File.ReadAllText(path), key);
            File.WriteAllText(path, data);
        }

        public void DecryptFile(string path, string key)
        {
            //string text = File.ReadAllText(path);
            //byte[] buffer = Convert.FromBase64String(text);
            //string data = Crypto.DecryptAes(buffer, key, Constant.CryptoSalt);
            //File.WriteAllText(path, data);
            string data = Decrypt(File.ReadAllText(path), key);
            File.WriteAllText(path, data);
        }

        public string LoadEncryptedFile(string path, string key)
        {
            //string text = File.ReadAllText(path);
            //byte[] buffer = Convert.FromBase64String(text);
            //string data = Crypto.DecryptAes(buffer, key, Constant.CryptoSalt);
            //return data;
            string data = Decrypt(File.ReadAllText(path), key);
            return data;
        }

        public string GetLocalLocalFolderPath()
        {
            return ApplicationData.Current.LocalFolder.Path;
        }

        public string GetAshwid()
        {
            //setIsScreenCaptureEnabled();

            var token = HardwareIdentification.GetPackageSpecificToken(null);
            var hardwareId = token.Id;
            var dataReader = global::Windows.Storage.Streams.DataReader.FromBuffer(hardwareId);

            byte[] bytes = new byte[hardwareId.Length];
            dataReader.ReadBytes(bytes);

            string strHardwareId = BitConverter.ToString(bytes);


            var hardwareIdString = strHardwareId.Replace("-", "");
            var deviceDic = new Dictionary<string, List<string>>
            {
                {"Invalid", new List<string>()}, // Invalid 
                {"Processor", new List<string>()}, // Processor 
                {"Memory", new List<string>()}, // Memory 
                {"DiskDevice", new List<string>()}, // DiskDevice 
                {"NetworkAdapter", new List<string>()}, // NetworkAdapter 
                {"AudioAdapter", new List<string>()}, // AudioAdapter 
                {"DockingStation", new List<string>()}, // DockingStation 
                {"MobileBroadband", new List<string>()}, // MobileBroadband 
                {"Bluetooth", new List<string>()}, // Bluetooth 
                {"BIOS", new List<string>()}, // SystemBIOS 
            };
            for (var i = 0; i < hardwareIdString.Length / 8; i++)
            {
                switch (hardwareIdString.Substring(i * 8, 4))
                {
                    case "0100": // Processor 
                        deviceDic["Processor"].Add(hardwareIdString.Substring(i * 8 + 4, 4));
                        break;
                    case "0200": // Memory 
                        deviceDic["Memory"].Add(hardwareIdString.Substring(i * 8 + 4, 4));
                        break;
                    case "0300": // Disk Device 
                        deviceDic["DiskDevice"].Add(hardwareIdString.Substring(i * 8 + 4, 4));
                        break;
                    case "0400": // Network Adapter 
                        deviceDic["NetworkAdapter"].Add(hardwareIdString.Substring(i * 8 + 4, 4));
                        break;
                    case "0500": // Audio Adapter 
                        deviceDic["AudioAdapter"].Add(hardwareIdString.Substring(i * 8 + 4, 4));
                        break;
                    case "0600": // Docking Station 
                        deviceDic["DockingStation"].Add(hardwareIdString.Substring(i * 8 + 4, 4));
                        break;
                    case "0700": // Mobile Broadband 
                        deviceDic["MobileBroadband"].Add(hardwareIdString.Substring(i * 8 + 4, 4));
                        break;
                    case "0800": // Bluetooth 
                        deviceDic["Bluetooth"].Add(hardwareIdString.Substring(i * 8 + 4, 4));
                        break;
                    case "0900": // System BIOS 
                        deviceDic["BIOS"].Add(hardwareIdString.Substring(i * 8 + 4, 4));
                        break;
                }
            }

            StringBuilder sb = new StringBuilder();
            string strBIOSId = "";
            foreach (var item in deviceDic["BIOS"])
            {
                strBIOSId = (strBIOSId == "") ? item.ToString() : (strBIOSId + "-" + item.ToString());
            }

            return strBIOSId;
        }

        public string setPlateform()
        {
            return "UWP";
        }

        public bool SaveBookDownload(string BookID, string fileName, MemoryStream stream)
        {
            try
            {
                string localFolderPath = ApplicationData.Current.LocalFolder.Path;
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
            bool Output = false;
            string localFolderPath = ApplicationData.Current.LocalFolder.Path;
            try
            {
                string inputfoldername = Path.Combine(localFolderPath, Constant.FileDirectoryStructure, Constant.FileNameInitials + BookID);
                string[] tmpfiles = Directory.GetFiles(inputfoldername, "*.tmp");
                using (FileStream outPutFile = new FileStream(Path.Combine(inputfoldername, Constant.FileNameInitials + BookID + Constant.FileExtension), FileMode.OpenOrCreate, FileAccess.Write))
                {
                    foreach (string tempFile in tmpfiles)
                    {
                        using (FileStream inputTempFile = new FileStream(tempFile, FileMode.OpenOrCreate, FileAccess.Read))
                        {
                            inputTempFile.CopyTo(outPutFile);
                        }
                        File.Delete(tempFile);
                    }
                    Output = true;
                }               
            }
            catch
            {
                Output = false;
            }
            return Output;
        }

        public string SetAndGetBackCover(string BookID, string file)
        {
            string FileName = Path.GetFileNameWithoutExtension(file) + "_" + BookID + Path.GetExtension(file);
            string CoverFolder = Path.Combine(ApplicationData.Current.LocalFolder.Path, "BackCovers");
            if (!Directory.Exists(CoverFolder))
            {
                Directory.CreateDirectory(CoverFolder);
            }
            try
            {
                if (File.Exists(Path.Combine(ApplicationData.Current.LocalFolder.Path, file)))
                {
                    string coverFilePath = Path.Combine(CoverFolder, FileName);
                    if (!File.Exists(coverFilePath))
                        File.Copy(Path.Combine(ApplicationData.Current.LocalFolder.Path, file), coverFilePath, true);
                    return Path.Combine(CoverFolder, FileName);
                }
                else
                {
                    if (File.Exists(Path.Combine(CoverFolder, FileName)))
                    {
                        return Path.Combine(CoverFolder, FileName);
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
                var deviceInfo = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();
                devicename = deviceInfo.FriendlyName;
            }
            catch (Exception ex) { }
            return devicename;
        }
        private void setIsScreenCaptureEnabled()
        {
            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                Task.Factory.StartNew(() =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ApplicationView.GetForCurrentView().IsScreenCaptureEnabled = false;
                    });
                });
                return false;
            });
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
        public string Encrypt(string dataToEncrypt, string password)
        {
            try
            {
                IBuffer aesKeyMaterial;
                string key = DeriveKey(password);
                aesKeyMaterial = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);

                IBuffer plainText = CryptographicBuffer.ConvertStringToBinary(dataToEncrypt, BinaryStringEncoding.Utf8);

                SymmetricKeyAlgorithmProvider aesProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
                CryptographicKey aesKey = aesProvider.CreateSymmetricKey(aesKeyMaterial);

                IBuffer encrypted = CryptographicEngine.Encrypt(aesKey, plainText, null);
                return CryptographicBuffer.EncodeToBase64String(encrypted);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string Decrypt(string dataToDecrypt, string password)
        {
            try
            {
                IBuffer aesKeyMaterial;
                string key = DeriveKey(password);
                aesKeyMaterial = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);

                SymmetricKeyAlgorithmProvider aesProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
                CryptographicKey aesKey = aesProvider.CreateSymmetricKey(aesKeyMaterial);

                Byte[] bb = Convert.FromBase64String(dataToDecrypt);
                IBuffer ciphertext = bb.AsBuffer(); //CryptographicBuffer.DecodeFromBase64String(dataToDecrypt);

                IBuffer decrypted = CryptographicEngine.Decrypt(aesKey, ciphertext, null);

                byte[] decryptedArray = new byte[decrypted.Length];
                using (var reader = DataReader.FromBuffer(decrypted))
                {
                    reader.ReadBytes(decryptedArray);
                }
                return Encoding.UTF8.GetString(decryptedArray, 0, decryptedArray.Length);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion encryption/decryption provider

        #region for message digest and sha256
        public string ComputeMD5(byte[] content)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(Encoding.UTF8.GetString(content), BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToBase64String(hashed);
            return res;
        }
        public string ComputeHmacSha256(byte[] secretKeyByteArray, byte[] signature)
        {
            var objMacProv = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha256);
            var hash = objMacProv.CreateHash(secretKeyByteArray.AsBuffer());
            hash.Append(CryptographicBuffer.ConvertStringToBinary(Encoding.UTF8.GetString(signature), BinaryStringEncoding.Utf8));
            return CryptographicBuffer.EncodeToBase64String(hash.GetValueAndReset());
        }
        #endregion
    }
}