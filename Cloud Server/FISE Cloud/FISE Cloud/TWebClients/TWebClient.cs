using System;
using System.Linq;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace FISE_Cloud.TWebClients
{
    public partial class TWebClient: ITWebClient
    {
        #region Fields        
        private static string _baseApiPath = ConfigurationManager.AppSettings["ApiPath"];
        private static string _APPId = ConfigurationManager.AppSettings["APPId"];
        private static string _APIKey = ConfigurationManager.AppSettings["APIKey"];

        private  WebClient _webClient;

        #endregion

        #region Properties

        public string ApiPath
        {
            get { return _baseApiPath; }
            protected set { }
        }

        #endregion

        public TWebClient(int UserId)
        {
            _webClient = new WebClient();
            _webClient.Headers.Add("Content-Type", "application/json");
            _webClient.Headers.Add("CurrentUserId", UserId.ToString());
        }
        public void Dispose()
        {
 	        this.Dispose();
        }



        public T UploadData<T>(string resourceName, object postData, bool includeContent = true)
        {
            try
            {
                T t = default(T);
                if (_webClient == null)
                    throw new NullReferenceException("webclient");

                if (postData != null && !String.IsNullOrEmpty(resourceName.Trim()))
                {
                    string data = CreatePostData(postData);
                    var address = _baseApiPath + resourceName;
                    _webClient.Headers.Remove(HttpRequestHeader.Authorization);
                    _webClient.Headers.Add(HttpRequestHeader.Authorization, "amx " + GetHashCode(Encoding.UTF8.GetBytes(data), address, "POST", includeContent));
                    string s = Encoding.UTF8.GetString(_webClient.UploadData(address, "POST", Encoding.UTF8.GetBytes(data)));
                    var result = JsonConvert.DeserializeObject<T>(s);
                    t = result;
                }
                return t;
            }
            catch (JsonSerializationException)
            {
                throw new Exception("Serialization error.");
            }
            catch(WebException ex)
            {
                throw new Exception("Api access error.");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            
        }

        public T DownloadData<T>(string resourceName, object param)
        {
            try
            {
                T t = default(T);
                if (_webClient == null)
                    throw new NullReferenceException("webclient");

                if (!String.IsNullOrEmpty(resourceName.Trim()))
                {
                    string address = _baseApiPath+resourceName+CreateGetRequestQueryString(param);
                    _webClient.Headers.Remove(HttpRequestHeader.Authorization);
                    _webClient.Headers.Add(HttpRequestHeader.Authorization, "amx " + GetHashCode(null, address, "GET"));
                    string s = Encoding.UTF8.GetString(_webClient.DownloadData(address));
                    //if(s.Contains("\\\""))
                    //    s = s.Replace("\\\"","\"").Trim('"');

                    var result = JsonConvert.DeserializeObject<T>(s);
                    t=result;
                }
                return t;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string CreatePostData(object postData)
        {
            string str = JsonConvert.SerializeObject(postData);
            if (String.IsNullOrEmpty(str))
            {
                StringBuilder data = new StringBuilder();
                Type t = postData.GetType();
                PropertyInfo[] properties = t.GetProperties();
                int i = 0;
                foreach (PropertyInfo prop in properties)
                {
                    i++;
                    data = data.Append("'" + prop.Name + "':'" + prop.GetValue(postData) + "'");
                    if (i < properties.Count())
                    {
                        data.Append(",");
                    }
                }
                return @"{" + data.ToString() + "}";
            }
            return str;
        }


        private string CreateGetRequestQueryString(object parameters)
        {
            if (parameters == null)
                return "";
            StringBuilder data = new StringBuilder("/?");
            Type t = parameters.GetType();
            PropertyInfo[] properties = t.GetProperties();
            int i = 0;
            foreach (PropertyInfo prop in properties)
            {
                i++;
                string val = prop.GetValue(parameters).ToString();

                if (String.IsNullOrEmpty(val))
                {
                    val = "";
                }               
                //string val = String.IsNullOrEmpty(dd)? "" : prop.GetValue(parameters).ToString();
                data = data.Append(prop.Name + "=" + val);
                if (i < properties.Count())
                {
                    data.Append("&");
                }
            }
            return data.ToString();
        }

        private string GetHashCode(byte[] content, string url, string requestHttpMethod,bool includeContent=true)
        {
            string APPId = _APPId;
            string APIKey = _APIKey;
            string response = string.Empty;
            string requestContentBase64String = string.Empty;
            string requestUri = System.Web.HttpUtility.UrlEncode(url.ToLower());


            //Calculate UNIX time
            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            string requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            //create random nonce for each request
            string nonce = Guid.NewGuid().ToString("N");

            //Checking if the request contains body, usually will be null wiht HTTP GET and DELETE
            if (content != null && includeContent)
            {
                MD5 md5 = MD5.Create();
                //Hashing the request body, any change in request body will result in different hash, we'll incure message integrity
                byte[] requestContentHash = md5.ComputeHash(content);
                requestContentBase64String = Convert.ToBase64String(requestContentHash);
            }

            //Creating the raw signature string
            string signatureRawData = String.Format("{0}{1}{2}{3}{4}{5}", APPId, requestHttpMethod, requestUri, requestTimeStamp, nonce, requestContentBase64String);

            var secretKeyByteArray = Convert.FromBase64String(APIKey);

            byte[] signature = Encoding.UTF8.GetBytes(signatureRawData.ToLower());

            using (HMACSHA256 hmac = new HMACSHA256(secretKeyByteArray))
            {
                byte[] signatureBytes = hmac.ComputeHash(signature);
                string requestSignatureBase64String = Convert.ToBase64String(signatureBytes);
                //Setting the values in the Authorization header using custom scheme (amx)                                
                response = string.Format("{0}:{1}:{2}:{3}", APPId, requestSignatureBase64String, nonce, requestTimeStamp);
            }
            return response;

        }

    }



}