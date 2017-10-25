using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Bithumb.LIB.Serialize;
using RestSharp;

namespace Bithumb.LIB
{
    /// <summary>
    /// XApiClient 객체의 부모객체 역할
    /// RestClient, RestRequest 객체를 생성하여 반환하는 것이 주 기능
    /// </summary>
    public class OWebRequest
    {
        private const string __content_type = "application/json";
        private const string __user_agent = "btc-trading/5.2.2017.01";

        /// <summary>
        /// bypasses certificate validation  (.NET 4.0)
        /// </summary>
        public OWebRequest()
        { 
            if (ServicePointManager.ServerCertificateValidationCallback == null)
                ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; }
        }

        private static char[] __to_digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        /// <summary>
        /// convert 1 byte to 2 bytes of hexa digit
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected byte[] EncodeHex(byte[] data)
        { // 255 -> 'ff'
            int l = data.Length;
            byte[] _result = new byte[l << 1]; // 원래 길이의 2배 공간 확보

            // two characters form the hex value.
            for (int i = 0, j = 0; i < l; i++)
            {
                _result[j++] = (byte)__to_digits[(0xF0 & data[i]) >> 4]; // 상위 4 Bits
                _result[j++] = (byte)__to_digits[0x0F & data[i]]; //  하위 4 Bits
            }

            return _result;
        }

        /// <summary>
        /// 딕셔너리 내의 원소들로 Key=Value&Key=Value&Key=Value.... 형태의 문자열을 만든다
        /// </summary>
        /// <param name="rgData"></param>
        /// <returns></returns>
        protected string EncodeURIComponent(Dictionary<string, object> rgData)
        { 
            string _result = String.Join("&", rgData.Select((x) => String.Format("{0}={1}", x.Key, x.Value)));

            _result = System.Net.WebUtility.UrlEncode(_result)
                        .Replace("+", "%20").Replace("%21", "!")
                        .Replace("%27", "'").Replace("%28", "(")
                        .Replace("%29", ")").Replace("%26", "&")
                        .Replace("%3D", "=").Replace("%7E", "~");

            return _result;
        }

        /// <summary>
        /// Return IRestClient (Interface)
        /// </summary>
        /// <param name="baseurl"></param>
        /// <returns></returns>
        protected IRestClient CreateJsonClient(string baseurl)
        {
            var _client = new RestClient(baseurl);
            {
                _client.RemoveHandler(__content_type);
                _client.AddHandler(__content_type, new RestSharpJsonNetDeserializer());
                _client.Timeout = 10 * 1000; // 수신대기 10초
                _client.UserAgent = __user_agent;
            }

            return _client;
        }

        /// <summary>
        /// Return IRestRequest (Interface)
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected IRestRequest CreateJsonRequest(string resource, Method method = Method.GET)
        { 
            var _request = new RestRequest(resource, method)
            {
                RequestFormat = DataFormat.Json,
                JsonSerializer = new RestSharpJsonNetSerializer()
            };

            return _request;
        }
    }
}