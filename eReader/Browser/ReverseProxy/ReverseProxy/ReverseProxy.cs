using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using PCLCrypto;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;

namespace ReverseProxy
{

    public class ReverseProxy : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        string LocalUrl = ConfigurationManager.AppSettings["LocalWebSite"];

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                if (context.Request.Url.AbsoluteUri.Contains(".html") || context.Request.Url.AbsoluteUri.Contains(".xhtml"))
                {
                    try
                    {
                        var encArray = ReadBlob(context.Request.Url.GetLeftPart(UriPartial.Path).Replace(LocalUrl, ""));
                        byte[] responseData;
                        if (context.Request.Url.AbsoluteUri.Contains("BookActivity.html"))
                            responseData = encArray;
                        else
                            responseData = DecryptFile(Encoding.UTF8.GetString(encArray, 0, encArray.Length), ConfigurationManager.AppSettings["decryptkey"]);
                        context.Response.OutputStream.Write(responseData, 0, responseData.Length);
                    }
                    catch (Exception ex)
                    { context.Response.Write("Something went wrong"); }
                }
                else
                {
                    if (context.Request.Url.AbsoluteUri.Contains(".opf"))
                    {
                        context.Response.ContentType = "text/xml";
                        byte[] responseData = ReadBlob(context.Request.Url.GetLeftPart(UriPartial.Path).Replace(LocalUrl, ""));
                        context.Response.Write(System.Text.Encoding.Default.GetString(responseData));
                    }
                    else
                    {
                        if (context.Request.Url.AbsoluteUri.Contains(".css"))
                            context.Response.ContentType = "text/css";
                        else if (context.Request.Url.AbsoluteUri.Contains(".js"))
                            context.Response.ContentType = "application/x-javascript";
                        else if (context.Request.Url.AbsoluteUri.Contains(".ttf"))
                            context.Response.ContentType = "font/opentype";

                        byte[] responseData = ReadBlob(context.Request.Url.GetLeftPart(UriPartial.Path).Replace(LocalUrl, ""));
                        context.Response.OutputStream.Write(responseData, 0, responseData.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~\\App_Data\\Log.txt"),ex.Message+" : "+  DateTime.Now.ToString() + Environment.NewLine);
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }

        byte[] ReadBlob(string blobPath = "")
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference(ConfigurationManager.AppSettings["container"]);

            // Retrieve reference to a blob named "myblob.txt"
            CloudBlockBlob blockBlob2 = container.GetBlockBlobReference(blobPath);

            string ext = blobPath.Substring(blobPath.LastIndexOf('.'), blobPath.Length - blobPath.LastIndexOf('.'));
            using (var memoryStream = new MemoryStream())
            {
                string tempPath = HttpContext.Current.Server.MapPath("\\Content\\" + Guid.NewGuid().ToString("N") + ext);
                blockBlob2.DownloadToStream(memoryStream);
                var sr = new StreamReader(memoryStream);
                var myStr = sr.ReadToEnd();
                return memoryStream.ToArray();
            }
        }

        public byte[] DecryptFile(string text, string key)
        {
            byte[] buffer;
            string data;
            try
            {
                buffer = Convert.FromBase64String(text);
                data = DecryptAes(buffer, key);
            }
            catch (Exception ex) { data = ""; }
            return Encoding.UTF8.GetBytes(data);
        }

        public string DecryptAes(byte[] data, string password)
        {
            byte[] key = Encoding.UTF8.GetBytes(password);

            ISymmetricKeyAlgorithmProvider aes = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(PCLCrypto.SymmetricAlgorithm.AesCbcPkcs7);
            ICryptographicKey symetricKey = aes.CreateSymmetricKey(key);
            var bytes = WinRTCrypto.CryptographicEngine.Decrypt(symetricKey, data);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }
    }
}