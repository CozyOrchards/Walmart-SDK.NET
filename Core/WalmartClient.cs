using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Walmart.SDK.Entities;

namespace Walmart.SDK.Core
{
    public class WalmartClient : IWalmartClient
    {
        private enum HttpMethod
        {
            GET,
            POST
        }

        private readonly string ApiUrl = "http://api.walmartlabs.com/v1";

        private string ApiKey = null;

        public WalmartClient(string apiKey)
        {
            ApiKey = apiKey;
        }

        public void GetCategories(Func<Category, bool> callback)
        {
            GetMultiple<Category>("/taxonomy", "categories", callback);
        }

        public void GetProducts(string categoryId, Func<Product, bool> callback)
        {
            var parms = new Dictionary<string, string>();
            parms.Add("categoryId", categoryId);
            GetMultiple<Product>("/feeds/items", "items", parms, callback);
        }

        public bool GetProduct(string itemId, Action<Product> callback)
        {
            return GetSingle<Product>(string.Format("/items/{0}", itemId), callback);
        }

        private void Request<T>(string url, string rootProperty, HttpMethod httpMethod, Func<T, bool> callback) where T : WalmartEntity
        {
            Request<T>(url, rootProperty, httpMethod, null, callback);
        }

        private void Request<T>(string url, string rootProperty, HttpMethod httpMethod, string data, Func<T, bool> callback) where T : WalmartEntity
        {
            string result = string.Empty;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = httpMethod.ToString();
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            httpWebRequest.AllowReadStreamBuffering = false;

            if (data != null)
            {
                byte[] bytes = UTF8Encoding.UTF8.GetBytes(data.ToString());
                httpWebRequest.ContentLength = bytes.Length;
                using(Stream stream = httpWebRequest.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }

            // Create a temp file
            string filename = string.Format("{0}", Guid.NewGuid());

            // Save it to our file
            using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (FileStream file = File.Create(filename))
                {
                    httpWebResponse.GetResponseStream().CopyTo(file);
                }
            }
            try
            {
                using (StreamReader streamReader = new StreamReader(filename))
                {
                    using (JsonTextReader reader = new JsonTextReader(streamReader))
                    {
                        JsonSerializer ser = new JsonSerializer();
                        bool foundStart = false;
                        T obj = null;
                        bool shouldContinue = true;
                        while (shouldContinue && reader.Read())
                        {
                            if(!foundStart)
                            {
                                foundStart = reader.TokenType == JsonToken.PropertyName && rootProperty.Equals(reader.Value);
                            }
                            else
                            {
                                switch(reader.TokenType)
                                {
                                    case JsonToken.StartObject:
                                        {
                                            obj = ser.Deserialize<T>(reader);
                                            shouldContinue = callback(obj);
                                            break;
                                        }
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                // Always delete our temp file
                File.Delete(filename);
            }
        }

        private bool RequestSingle<T>(string url, HttpMethod httpMethod, Action<T> callback) where T : WalmartEntity
        {
            return RequestSingle<T>(url, httpMethod, null, callback);
        }

        private bool RequestSingle<T>(string url, HttpMethod httpMethod, string data, Action<T> callback) where T : WalmartEntity
        {
            string result = string.Empty;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = httpMethod.ToString();
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            httpWebRequest.AllowReadStreamBuffering = false;

            if (data != null)
            {
                byte[] bytes = UTF8Encoding.UTF8.GetBytes(data.ToString());
                httpWebRequest.ContentLength = bytes.Length;
                using (Stream stream = httpWebRequest.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
            }

            using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using(StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
            }

            // Convert result to object
            T singleObj = JsonConvert.DeserializeObject<T>(result);
            if(singleObj != null)
            {
                callback(singleObj);
            }

            return singleObj != null;
        }

        private void GetMultiple<T>(string endpoint, string rootProperty, Func<T, bool> callback) where T : WalmartEntity
        {
            GetMultiple<T>(endpoint, rootProperty, null, false, callback);
        }

        private void GetMultiple<T>(string endpoint, string rootProperty, bool unauthenticated, Func<T, bool> callback) where T : WalmartEntity
        {
            GetMultiple<T>(endpoint, rootProperty, null, unauthenticated, callback);
        }

        private void GetMultiple<T>(string endpoint, string rootProperty, Dictionary<string, string> parameters, Func<T, bool> callback) where T : WalmartEntity
        {
            GetMultiple<T>(endpoint, rootProperty, parameters, false, callback);
        }

        private void GetMultiple<T>(string endpoint, string rootProperty, Dictionary<string, string> parameters, bool unauthenticated, Func<T, bool> callback) where T : WalmartEntity
        {
            string serializedParameters = "";
            if (parameters != null)
            {
                serializedParameters = "&" + SerializeDictionary(parameters);
            }

            Request<T>(string.Format("{0}{1}?apiKey={2}{3}", ApiUrl, endpoint, ApiKey, serializedParameters), rootProperty, HttpMethod.GET, callback);
        }

        private bool GetSingle<T>(string endpoint, Action<T> callback) where T : WalmartEntity
        {
            return GetSingle<T>(endpoint, null, false, callback);
        }

        private bool GetSingle<T>(string endpoint, Dictionary<string, string> parameters, bool unauthenticated, Action<T> callback) where T : WalmartEntity
        {
            string serializedParameters = "";
            if (parameters != null)
            {
                serializedParameters = "&" + SerializeDictionary(parameters);
            }

            return RequestSingle<T>(string.Format("{0}{1}?apiKey={2}{3}", ApiUrl, endpoint, ApiKey, serializedParameters), HttpMethod.GET, callback);
        }


        private string SerializeDictionary(Dictionary<string, string> dictionary)
        {
            StringBuilder parameters = new StringBuilder();
            foreach (KeyValuePair<string, string> keyValuePair in dictionary)
            {
                parameters.Append(keyValuePair.Key + "=" + keyValuePair.Value + "&");
            }
            return parameters.Remove(parameters.Length - 1, 1).ToString();
        }
    }
}
