using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Bithumb.LIB;
using Bithumb.LIB.Configuration;
using Newtonsoft.Json;
using RestSharp;

namespace Bithumb.API
{
    /// <summary>
    /// XApiClient : OWebRequest, IDisposable
    /// </summary>
    public class XApiClient : OWebRequest, IDisposable
    {
        private const string __api_url = "https://api.bithumb.com";

        private string __connect_key;
        private string __secret_key;

        /// <summary>
        /// 
        /// </summary>
        public XApiClient(string connect_key, string secret_key) : base()
        {
            __connect_key = connect_key;
            __secret_key = secret_key;
        }

        /// <summary>
        /// 컨텐츠에 대한 보안 정보 생성하여 Header 설정
        /// 비밀키 기반으로 HTTP 헤더에 서명을 추가하기 위한 작업이 핵심
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="rgData"></param>
        /// <param name="apiKey"></param>
        /// <param name="apiSecret"></param>
        /// <returns></returns>
        protected Dictionary<string, object> GetHttpHeaders(string endpoint, Dictionary<string, object> rgData, string apiKey, string apiSecret)
        { 
            var _nonce = UnixTime.NowMilli.ToString();
            var _data = EncodeURIComponent(rgData);
            var _message = String.Format("{0};{1};{2}", endpoint, _data, _nonce);

            var _secretKey = Encoding.UTF8.GetBytes(apiSecret);
            var _hmac = new HMACSHA512(_secretKey);
            _hmac.Initialize();

            var _bytes = Encoding.UTF8.GetBytes(_message);
            var _rawHmac = _hmac.ComputeHash(_bytes);

            var _encoded = EncodeHex(_rawHmac);
            var _signature = Convert.ToBase64String(_encoded);

            var _headers = new Dictionary<string, object>();
            {
                _headers.Add("Api-Key", apiKey);
                _headers.Add("Api-Sign", _signature);
                _headers.Add("Api-Nonce", _nonce);
            }

            return _headers;
        }

        /// <summary>
        /// POST 방식 비동기 호출
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<T> CallApiPostAsync<T>(string endpoint, Dictionary<string, object> args = null) where T : new()
        {
            var _request = CreateJsonRequest(endpoint, Method.POST);
            {
                var _params = new Dictionary<string, object>();
                { // 매개변수 args 선두에 endpoint 만 삽입한 상태
                    _params.Add("endpoint", endpoint);
                    if (args != null)
                    {
                        foreach (var a in args)
                            _params.Add(a.Key, a.Value);
                    }
                }

                // header
                _request.AddHeader("api-client-type", "2"); // 이건 뭐지...???

                var _headers = GetHttpHeaders(endpoint, _params, __connect_key, __secret_key);
                foreach (var h in _headers)
                    _request.AddHeader(h.Key, h.Value.ToString());

                // parameter
                foreach (var a in _params)
                    _request.AddParameter(a.Key, a.Value);
            }

            // request & process response
            var _client = CreateJsonClient(__api_url);
            {
                var tcs = new TaskCompletionSource<T>();

                try
                {
                    _client.ExecuteAsync(_request, response =>
                    {
                        tcs.SetResult(JsonConvert.DeserializeObject<T>(response.Content));
                    });
                }
                catch (Exception e)
                {
                    new CLogger().WriteLog(e);
                }

                return await tcs.Task;
            }
        }

        /// <summary>
        /// GET 방식 비동기 호출
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<T> CallApiGetAsync<T>(string endpoint, Dictionary<string, object> args = null) where T : new()
        {
            var _request = CreateJsonRequest(endpoint, Method.GET);

            if (args != null)
            { // POST 방식에서는 Header 를 사용했으나, GET 방식은 사용 안함
                //  POST 방식은 endpoint 를 패러미터 선두에 추가했으나, GET 방식은 패러미터에 추가 안함
                foreach (var a in args)
                    _request.AddParameter(a.Key, a.Value);
            }

            var _client = CreateJsonClient(__api_url);
            {
                var tcs = new TaskCompletionSource<T>();

                try
                {
                    _client.ExecuteAsync(_request, response =>
                    {
                        tcs.SetResult(JsonConvert.DeserializeObject<T>(response.Content));
                    });
                }
                catch (Exception e)
                {
                    new CLogger().WriteLog(e);
                }


                return await tcs.Task;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
        }
    }
}