using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Kitablet.ViewModels
{
    public class MyWebRequest
    {
        public static string GetRequest(string API, List<KeyValuePair<string, string>> Headers, MediaTypeWithQualityHeaderValue MediaType)
        {
            string resultContent = null;
            string address = Constant.baseUri + Constant.relativePath + API;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Constant.baseUri);
                    if (MediaType == null)
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    }
                    else
                    {
                        client.DefaultRequestHeaders.Accept.Add(MediaType);
                    }
                    if (Headers != null)
                    {
                        foreach (KeyValuePair<string, string> header in Headers)
                        {
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                    }
                    if (AppData.User != null && AppData.User.UserId != 0)
                    {
                        client.DefaultRequestHeaders.Add("CurrentUserId", AppData.User.UserId.ToString());
                    }
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("amx", ComputeHash._ComputeHash.GetHashCode(null, address, "GET"));
                    var result = client.GetAsync(Constant.relativePath + API).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        resultContent = result.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Exception ex) { }
            return resultContent;
        }

        public static string PostRequest(string API, List<KeyValuePair<string, string>> Headers, object Data, MediaTypeWithQualityHeaderValue MediaType)
        {
            string resultContent = null;
            string address = Constant.baseUri + Constant.relativePath + API;
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Constant.baseUri);
                    if (MediaType == null)
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    }
                    else
                    {
                        client.DefaultRequestHeaders.Accept.Add(MediaType);
                    }
                    if (Headers != null)
                    {
                        foreach (KeyValuePair<string, string> header in Headers)
                        {
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                    }
                    if (AppData.User != null && AppData.User.UserId != 0)
                    {
                        client.DefaultRequestHeaders.Add("CurrentUserId", AppData.User.UserId.ToString());
                    }
                    StringContent content = null;
                    if (Data != null)
                    {
                        var json = JsonConvert.SerializeObject(Data);
                        content = new StringContent(json, Encoding.UTF8, "application/json");
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("amx", ComputeHash._ComputeHash.GetHashCode(Encoding.UTF8.GetBytes(json), address, "POST"));
                    }
                    var result = client.PostAsync(Constant.relativePath + API, content).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        resultContent = result.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Exception ex) { }
            return resultContent;
        }
    }
}
