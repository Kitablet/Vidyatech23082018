using System;
using System.Threading.Tasks;
using Windows.Storage;
using Kitablet.Windows;
using Xamarin.Forms;
using System.IO;
using System.Xml.Linq;
using System.Text;
using System.Net.Http;
using Windows.UI.Xaml;
using PCLStorage;
using Windows.System.Profile;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Linq;
using Security = Windows.Security;
using Windows.UI.ViewManagement;
using Windows.UI.ViewManagement;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using System.Runtime.InteropServices.WindowsRuntime;

[assembly: Dependency(typeof(SaveAndLoad))]
namespace Kitablet.Windows
{

    public class SaveAndLoad : ISaveAndLoad
    {
        #region ISaveAndLoad implementation
        public string GetId()
        {
            try
            {
                //NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                //String sMacAddress = string.Empty;
                //foreach (NetworkInterface adapter in nics)
                //{
                //    if (sMacAddress == String.Empty)// only return MAC Address from first card  
                //    {
                //        IPInterfaceProperties properties = adapter.GetIPProperties();
                //        sMacAddress = adapter.GetPhysicalAddress().ToString();
                //    }
                //}
                //return sMacAddress;
                return "";
            }
            catch (Exception ex)
            {
                return "Error";
            }
        }
        public async Task SaveTextAsync(string filename, string text)
        {
            filename = filename.Replace("/", "\\");
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            filename = filename.Replace(rootFolder.Path.ToString() + "\\", "");
            IFile file = await rootFolder.CreateFileAsync(filename, PCLStorage.CreationCollisionOption.ReplaceExisting);
            await file.WriteAllTextAsync(text);
        }
        public async Task<string> LoadTextAsync(string filename)
        {
            string text = null;
            try
            {
                filename = filename.Replace("/", "\\");
                IFolder rootFolder = FileSystem.Current.LocalStorage;
                filename = filename.Replace(rootFolder.Path.ToString() + "\\", "");
                IFile file = await rootFolder.GetFileAsync(filename);
                text = await file.ReadAllTextAsync();
            }
            catch (Exception ex)
            {

            }
            return text;
        }
        public void SaveText(string filename, string text)
        {
            if (text == null)
            {
                text = "";
            }
            Task.Run(async () =>
            {
                await SaveTextAsync(filename, text);
            }).Wait();
        }
        public string LoadText(string filename)
        {
            try
            {
                Task<string> task = Task.Run(async () =>
                {
                    return await LoadTextAsync(filename);
                });
                task.Wait();
                var Result = task.Result;
                return Result;
            }
            catch (Exception ex) { return null; }

        }
        public async Task<bool> FileExistsAsync(string filename)
        {
            filename = filename.Replace("/", "\\");
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            filename = filename.Replace(rootFolder.Path.ToString() + "\\", "");
            try
            {
                IFile file = await rootFolder.GetFileAsync(filename);
                return true;
            }
            catch (Exception ex) { return false; }
        }
        public async Task<bool> DirectoryExistsAsync(string dirname)
        {
            dirname = dirname.Replace("/", "\\");
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            try
            {
                IFolder dir = await rootFolder.GetFolderAsync(dirname);
                return true;
            }
            catch (Exception ex) { return false; }
        }
        public bool FileExists(string filename)
        {
            return CheckFileExistence(filename);
        }
        public bool CheckFileExistence(string filename)
        {
            Task<bool> task = Task.Run(async () =>
            {
                return await FileExistsAsync(filename);
            });
            task.Wait();
            var Result = task.Result;
            return Result;
        }
        public bool CheckDirectoryExistence(string DirectoryPath)
        {
            Task<bool> task = Task.Run(async () =>
            {
                return await DirectoryExistsAsync(DirectoryPath);
            });
            task.Wait();
            var Result = task.Result;
            return Result;
        }
        public async Task<IFolder> CreateFolder(string directoryPath)
        {
            IFolder localFolder = FileSystem.Current.LocalStorage;
            IFolder dir = await localFolder.CreateFolderAsync(directoryPath, PCLStorage.CreationCollisionOption.OpenIfExists);
            return dir;
        }
        public String getContentPath()
        {
            return "Content";
        }
        public String getBasePath()
        {
            return "Content";
        }
        public async Task<Stream> getStreamAsync(string file)
        {
            Stream retstream = null;
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile sampleFile = await storageFolder.GetFileAsync(file);


                var stream = await sampleFile.OpenReadAsync();
                retstream = stream.AsStreamForRead();
            }
            catch (Exception ex) { }
            return retstream;
        }
        public Stream getStream(string file)
        {
            var task = Task.Run(async () =>
            {
                return await getStreamAsync(file);
            });

            var Result = task.Result;
            return Result;
        }
        #endregion
        #region For Cache
        public void saveCacheDetails(string id, string value)
        {
            try
            {
                var roamingSettings = ApplicationData.Current.LocalSettings;
                if (roamingSettings.Values[id] == null)
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
                ApplicationDataContainer localSettings =
                    ApplicationData.Current.LocalSettings;
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
            ApplicationDataContainer localSettings =
                 ApplicationData.Current.LocalSettings;
            ApplicationDataCompositeValue composite =
               (ApplicationDataCompositeValue)localSettings.Values[id];
            try
            {
                if (composite == null)
                {
                    // No data
                }
                else
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
            catch (Exception ex)
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
            catch (Exception ex)
            {

            }
        }
        #endregion

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
        //public void setDeviceVariable()
        //{
        //    TargetPlatform DeviceOs = Device.OS;

        //    EasClientDeviceInformation eas = new EasClientDeviceInformation();
        //    Constant.DeviceModel = eas.SystemProductName;
        //    Constant.DevicePlateform = DeviceOs.ToString();
        //    Constant.DeviceType = Device.Idiom.ToString();            
        //}
        public float GetDeviceDensity()
        {
            return global::Windows.Graphics.Display.DisplayProperties.LogicalDpi;
        }

        public double GetDeviceScale()
        {
            double scale = 100;
            try
            {
                scale = Convert.ToDouble(global::Windows.Graphics.Display.DisplayProperties.ResolutionScale);
            }
            catch (Exception)
            {

            }
            return scale;
        }

        public bool DownloadAndSaveFile(string FileUrl, string DirStruc, string FileName)
        {
            return false;
            //try
            //{
            //    var client = new HttpClient();
            //    var url = new System.Uri(FileUrl);
            //    byte[] result = client.GetByteArrayAsync(url).Result;
            //    Task.Run(async () =>
            //    {
            //        await DownloadAndSaveFileAsync(result, DirStruc, FileName);
            //    }).Wait();
            //}
            //catch (Exception e)
            //{ }
        }
        public async Task DownloadAndSaveFileAsync(byte[] result, string DirStruc, string FileName)
        {
            IFolder localFolder = FileSystem.Current.LocalStorage;
            string dirname = DirStruc + FileName.Split('.')[0];
            if (!CheckDirectoryExistence(dirname))
            {
                IFolder dir = await CreateFolder(DirStruc);

                IFile file = await dir.CreateFileAsync(FileName, PCLStorage.CreationCollisionOption.ReplaceExisting);
                try
                {
                    using (System.IO.Stream fstream = await file.OpenAsync(FileAccess.ReadAndWrite))
                    {
                        fstream.Write(result, 0, result.Length);
                    }
                }
                catch (Exception ex)
                {
                }

                //await file.WriteAllTextAsync(Encoding.UTF8.GetString(result, 0, result.Length));
            }
        }
        public bool FileUnzip(string FilePath)
        {
            //return;
            try
            {
                Task.Run(async () =>
                {
                    await FileUnzipAsync(FilePath);
                }).Wait();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public async Task FileUnzipAsync(string FilePath)
        {
            FilePath = FilePath.Replace("/", "\\");
            IFolder localFolder = FileSystem.Current.LocalStorage;
            FilePath = FilePath.Replace(localFolder.Path.ToString() + "\\", "");
            if (CheckFileExistence(FilePath))
            {
                IFile file = await localFolder.GetFileAsync(FilePath);
                Stream stream = await file.OpenAsync(FileAccess.Read);
                string extractPath = Regex.Replace(FilePath, ".zip", "");
                IFolder extarctFolder;
                if (CheckDirectoryExistence(extractPath))
                    DeleteDirectory(extractPath);
                if (!CheckDirectoryExistence(extractPath))
                {
                    extarctFolder = await CreateFolder(extractPath);
                }
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {

                        var fname = entry.Name;
                        var fpath = entry.FullName.Replace("/", "\\");
                        string opfile = PortablePath.Combine(extractPath, fpath);
                        string dirName = System.IO.Path.GetDirectoryName(opfile);
                        if (!CheckDirectoryExistence(dirName))
                        {
                            IFolder fld = await CreateFolder(dirName);
                        }
                        if (fname != "")
                        {
                            if (!CheckFileExistence(opfile))
                            {
                                using (Stream entryStream = entry.Open())
                                {
                                    byte[] buffer = new byte[entry.Length];
                                    entryStream.Read(buffer, 0, buffer.Length);
                                    try
                                    {
                                        IFile uncompressedFile = await localFolder.CreateFileAsync(opfile, PCLStorage.CreationCollisionOption.ReplaceExisting);
                                        //await uncompressedFile.WriteAllTextAsync(Encoding.UTF8.GetString(buffer, 0, buffer.Length));
                                        using (System.IO.Stream fstream = await uncompressedFile.OpenAsync(FileAccess.ReadAndWrite))
                                        {
                                            fstream.Write(buffer, 0, buffer.Length);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
                await file.DeleteAsync();
            }
        }
        public void EncryptFile(string path, string key)
        {
            try
            {
                Task.Run(async () =>
                {
                    await EncryptFileAsync(path, key);
                }).Wait();
            }
            catch (Exception e)
            { }
        }
        public async Task EncryptFileAsync(string path, string key)
        {
            path = path.Replace("/", "\\");
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            path = path.Replace(rootFolder.Path.ToString() + "\\", "");
            IFile file = await rootFolder.GetFileAsync(path);
            string text = await file.ReadAllTextAsync();
            //byte[] bytes = Crypto.EncryptAes(text, key, Constant.CryptoSalt);            
            //await file.WriteAllTextAsync(Convert.ToBase64String(bytes, 0, bytes.Length));
            string data = Encrypt(text, key);
            await file.WriteAllTextAsync(data);
        }
        public void DecryptFile(string path, string key)
        {
            try
            {
                Task.Run(async () =>
                {
                    await DecryptFileAsync(path, key);
                }).Wait();
            }
            catch (Exception e)
            { }
        }
        public async Task DecryptFileAsync(string path, string key)
        {
            try
            {
                path = path.Replace("/", "\\");
                IFolder rootFolder = FileSystem.Current.LocalStorage;
                path = path.Replace(rootFolder.Path.ToString() + "\\", "");
                IFile file = await rootFolder.GetFileAsync(path);
                string text = await file.ReadAllTextAsync();
                //byte[] buffer = Convert.FromBase64String(text);
                //string data = Crypto.DecryptAes(buffer, key, Constant.CryptoSalt);
                //await file.WriteAllTextAsync(data);
                string data = Decrypt(text, key);
                await file.WriteAllTextAsync(data);
            }
            catch (Exception ex) { }
        }
        public bool BookXmlFilesOperation(string bookPath, string type, string key)
        {
            try
            {
                string xmlpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, bookPath, "EPUB", "package.opf");
                if (CheckFileExistence(xmlpath))
                {
                    XDocument xldoc = XDocument.Parse(LoadText(xmlpath));

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
                    return false;
            }
            catch (Exception ex) { return false; }
        }
        public string LoadEncryptedFile(string path, string key)
        {
            try
            {
                Task<string> task = Task.Run(async () =>
                {
                    return await LoadEncryptedFileAsync(path, key);
                });
                task.Wait();
                var Result = task.Result;
                return Result;
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public static byte[] StreamToByteArray(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
        public async Task<string> LoadEncryptedFileAsync(string path, string key)
        {
            string data = string.Empty;
            try
            {
                path = path.Replace("/", "\\");
                IFolder rootFolder = FileSystem.Current.LocalStorage;
                path = path.Replace(rootFolder.Path.ToString() + "\\", "");
                IFile file = await rootFolder.GetFileAsync(path);
                string text = await file.ReadAllTextAsync();
                //byte[] buffer = Convert.FromBase64String(text);
                //data = Crypto.DecryptAes(buffer, key, Constant.CryptoSalt);
                data = Decrypt(text, key);
                return data;
            }
            catch (Exception ex) { }
            return data;
        }
        public string GetLocalLocalFolderPath()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            return rootFolder.Path.ToString();
        }

        public void DeleteFile(string FilePath)
        {
            try
            {
                Task.Run(async () =>
                {
                    await DeleteFileAsync(FilePath);
                }).Wait();
            }
            catch (Exception e)
            {

            }
        }
        public async Task DeleteFileAsync(string FilePath)
        {
            FilePath = FilePath.Replace("/", "\\");
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            FilePath = FilePath.Replace(rootFolder.Path.ToString() + "\\", "");
            IFile file = await rootFolder.GetFileAsync(FilePath);
            await file.DeleteAsync();
        }
        public void DeleteDirectory(string FilePath)
        {
            try
            {
                Task.Run(async () =>
                {
                    await DeleteDirectoryAsync(FilePath);
                }).Wait();
            }
            catch (Exception e)
            {

            }
        }
        public async Task DeleteDirectoryAsync(string FilePath)
        {
            FilePath = FilePath.Replace("/", "\\");
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            FilePath = FilePath.Replace(rootFolder.Path.ToString() + "\\", "");
            IFolder dir = await rootFolder.GetFolderAsync(FilePath);
            await dir.DeleteAsync();
        }
        public void CreateFiles(string filename)
        {
            try
            {
                Task.Run(async () =>
                {
                    await CreateFilesAsync(filename);
                }).Wait();
            }
            catch (Exception e)
            {

            }
        }
        public async Task CreateFilesAsync(string filename)
        {
            filename = filename.Replace("/", "\\");
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            filename = filename.Replace(rootFolder.Path.ToString() + "\\", "");
            if (!CheckFileExistence(filename))
            {
                IFile file = await rootFolder.CreateFileAsync(filename, PCLStorage.CreationCollisionOption.ReplaceExisting);
            }
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
            return "Win8.1";
        }

        public bool SaveBookDownload(string BookID, string fileName, MemoryStream stream)
        {
            try
            {
                Task<bool> task = Task.Run(async () =>
                {
                    return await SaveBookDownloadAsync(BookID, fileName, stream);
                });
                task.Wait();
                return task.Result;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public async Task<bool> SaveBookDownloadAsync(string BookID, string fileName, MemoryStream stream)
        {
            try
            {
                //string localFolderPath = ApplicationData.Current.LocalFolder.Path;
                IFolder localFolderPath = FileSystem.Current.LocalStorage;
                //string filePath = Path.Combine(localFolderPath, Constant.FileDirectoryStructure, Constant.FileNameInitials + BookID);
                string filePath = Path.Combine(Constant.FileDirectoryStructure);
                filePath = filePath.Replace("/", "\\").Replace(localFolderPath.Path.ToString() + "\\", "");
                if (!CheckDirectoryExistence(filePath))
                {
                    await CreateFolder(filePath);
                }
                if (CheckFileExistence(Path.Combine(filePath, fileName)))
                {
                    DeleteFile(Path.Combine(filePath, fileName));
                }

                IFile File = await localFolderPath.CreateFileAsync(Path.Combine(filePath, fileName), PCLStorage.CreationCollisionOption.ReplaceExisting);

                using (System.IO.Stream fstream = await File.OpenAsync(FileAccess.ReadAndWrite))
                {
                    stream.WriteTo(fstream);
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
                //string[] tmpfiles = Directory.GetFiles(inputfoldername, "*.tmp");
                //using (FileStream outPutFile = new FileStream(Path.Combine(inputfoldername, Constant.FileNameInitials + BookID + Constant.FileExtension), FileMode.OpenOrCreate, FileAccess.Write))
                //{
                //    foreach (string tempFile in tmpfiles)
                //    {
                //        using (FileStream inputTempFile = new FileStream(tempFile, FileMode.OpenOrCreate, FileAccess.Read))
                //        {
                //            inputTempFile.CopyTo(outPutFile);
                //        }
                //        File.Delete(tempFile);
                //    }
                //    Output = true;
                //}
            }
            catch
            {
                Output = false;
            }
            return Output;
        }

        public string SetAndGetBackCover(string BookID, string file)
        {
            try
            {
                Task<string> task = Task.Run(async () =>
                {
                    return await SetAndGetBackCoverAsync(BookID, file);
                });
                task.Wait();
                var Result = task.Result;
                Result = "file://" + Result.Replace("\\", "/");
                return Result;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
        public async Task<string> SetAndGetBackCoverAsync(string BookID, string file)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            string FileName = Path.GetFileNameWithoutExtension(file) + "_" + BookID + Path.GetExtension(file);
            string CoverFolder = Path.Combine(ApplicationData.Current.LocalFolder.Path, "BackCovers");
            string CoverFolderName = "BackCovers";
            if (!CheckDirectoryExistence(CoverFolderName))
            {
                IFolder fld = await CreateFolder(CoverFolderName);
            }
            try
            {
                if (CheckFileExistence(file))
                {
                    IFile srcFile = await rootFolder.GetFileAsync(file);
                    Stream srcStream = await srcFile.OpenAsync(FileAccess.Read);
                    byte[] buffer = StreamToByteArray(srcStream);
                    //IFile destFile = await rootFolder.CreateFileAsync(Path.Combine(CoverFolderName, FileName), PCLStorage.CreationCollisionOption.ReplaceExisting);
                    try
                    {
                        IFile destFile = await rootFolder.CreateFileAsync(Path.Combine(CoverFolderName, FileName), PCLStorage.CreationCollisionOption.ReplaceExisting);
                        using (System.IO.Stream fstream = await destFile.OpenAsync(FileAccess.ReadAndWrite))
                        {
                            fstream.Write(buffer, 0, buffer.Length);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    return Path.Combine(CoverFolder, FileName);
                }
                else
                {
                    if (CheckFileExistence(Path.Combine(CoverFolderName, FileName)))
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
                var deviceInfo = new Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();
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
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(Encoding.UTF8.GetString(content,0, content.Length), BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToBase64String(hashed);
            return res;
        }
        public string ComputeHmacSha256(byte[] secretKeyByteArray, byte[] signature)
        {
            var objMacProv = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha256);
            var hash = objMacProv.CreateHash(secretKeyByteArray.AsBuffer());
            hash.Append(CryptographicBuffer.ConvertStringToBinary(Encoding.UTF8.GetString(signature, 0, signature.Length), BinaryStringEncoding.Utf8));
            return CryptographicBuffer.EncodeToBase64String(hash.GetValueAndReset());
        }
        #endregion
    }
}

