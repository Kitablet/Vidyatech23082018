
using System;
using System.Text;

namespace Kitablet
{
    public class ComputeHash
    {
        public static ComputeHash _ComputeHash { set; get; }
        static ComputeHash()
        {
            _ComputeHash = new ComputeHash();
        }
        public string GetHashCode(byte[] content, string url, string requestHttpMethod)
        {
            string response = string.Empty;
            string requestContentBase64String = string.Empty;
            string requestUri = System.Net.WebUtility.UrlEncode(url.ToLower());


            //Calculate UNIX time
            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            string requestTimeStamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

            //create random nonce for each request
            string nonce = Guid.NewGuid().ToString("N");

            //Checking if the request contains body, usually will be null wiht HTTP GET and DELETE
            if (content != null)
            {
                //MD5Digest md5 = new MD5Digest();
                //md5.Reset();
                //md5.BlockUpdate(content, 0, content.Length);
                //int length = md5.GetDigestSize();
                //byte[] md5data = new byte[length];
                //md5.DoFinal(md5data, 0);
                //requestContentBase64String = Convert.ToBase64String(md5data);
                requestContentBase64String = AppData.FileService.ComputeMD5(content);
            }

            //Creating the raw signature string
            string signatureRawData = String.Format("{0}{1}{2}{3}{4}{5}", Constant.APPId, requestHttpMethod, requestUri, requestTimeStamp, nonce, requestContentBase64String);

            var secretKeyByteArray = Convert.FromBase64String(Constant.APIKey);

            byte[] signature = Encoding.UTF8.GetBytes(signatureRawData.ToLower());

            //HmacSha256 hmac = new HmacSha256(secretKeyByteArray);
            //byte[] signatureBytes = hmac.ComputeHash(signature);
            //string requestSignatureBase64String = Convert.ToBase64String(signatureBytes);
            string requestSignatureBase64String = AppData.FileService.ComputeHmacSha256(secretKeyByteArray, signature);

            //Setting the values in the Authorization header using custom scheme (amx)                                
            response = string.Format("{0}:{1}:{2}:{3}", Constant.APPId, requestSignatureBase64String, nonce, requestTimeStamp);
            
            return response;

        }
    }
    //public class HmacSha256
    //{
    //    private readonly HMac _hmac;

    //    public HmacSha256(byte[] key)
    //    {
    //        _hmac = new HMac(new Sha256Digest());
    //        _hmac.Init(new KeyParameter(key));
    //    }

    //    public byte[] ComputeHash(byte[] value)
    //    {
    //        if (value == null) throw new ArgumentNullException("value");

    //        byte[] resBuf = new byte[_hmac.GetMacSize()];
    //        _hmac.BlockUpdate(value, 0, value.Length);
    //        _hmac.DoFinal(resBuf, 0);

    //        return resBuf;
    //    }
    //}
}
