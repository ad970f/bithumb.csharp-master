//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using RestSharp;

//namespace Bithumb.LIB
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public class OWebClient : OWebRequest, IDisposable
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        public OWebClient(string web_api_url) 
//            : base()
//        {
//            this.WebApiUrl = web_api_url;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public string WebApiUrl
//        {
//            get;
//            set;
//        }

//        /// <summary>
//        /// GET.......................
//        /// RestRequest 객체에 필요한 패러미터를 세팅한 후
//        /// RestClient 객체에 콜백 함수를 지정한 후 요청을 전송하고 응답을 기다린다
//        /// 이때 작업은 비동기 방식으로 실행된다.
//        /// 비동기 호출은 TaskCompletionSource 타잎을 이용한다 (C# 5.0)
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="endpoint"></param>
//        /// <param name="args"></param>
//        /// <returns></returns>
//        public async Task<List<JArray>> CallApiAsync(string endpoint, Dictionary<string, object> args = null)
//        {
//            var _request = CreateJsonRequest(endpoint, Method.GET);
//            {
//                var _params = new Dictionary<string, object>();
//                {
//                    _params.Add("endpoint", endpoint);
//                    if (args != null)
//                    {
//                        foreach (var a in args)
//                            _params.Add(a.Key, a.Value);
//                    }
//                }

//                foreach (var a in _params)
//                    _request.AddParameter(a.Key, a.Value);
//            }

//            var _client = CreateJsonClient(WebApiUrl);
//            {
//                var tcs = new TaskCompletionSource<List<JArray>>();
//                _client.ExecuteAsync(_request, response =>
//                {
//                    tcs.SetResult(JsonConvert.DeserializeObject<List<JArray>>(response.Content));
//                });

//                return await tcs.Task;
//            }
//        }

//        /// <summary>
//        /// POST..................................................
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="endpoint"></param>
//        /// <param name="args"></param>
//        /// <returns></returns>
//        public async Task<T> CallApiAsync<T>(string endpoint, Dictionary<string, object> args = null) where T : new()
//        {
//            var _request = CreateJsonRequest(endpoint, Method.POST);
//            {
//                var _params = new Dictionary<string, object>();
//                {
//                    _params.Add("endpoint", endpoint);
//                    if (args != null)
//                    {
//                        foreach (var a in args)
//                            _params.Add(a.Key, a.Value);
//                    }
//                }

//                foreach (var a in _params)
//                    _request.AddParameter(a.Key, a.Value);
//            }

//            var _client = CreateJsonClient(WebApiUrl);
//            {
//                var tcs = new TaskCompletionSource<T>();
//                _client.ExecuteAsync(_request, response =>
//                {
//                    tcs.SetResult(JsonConvert.DeserializeObject<T>(response.Content));
//                });

//                return await tcs.Task;
//            }
//        }

//        /// <summary>
//        /// GET...................굳이 왜 만들었을까??
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="endpoint"></param>
//        /// <param name="args"></param>
//        /// <returns></returns>
//        public async Task<T> CallApiGetAsync<T>(string endpoint, Dictionary<string, object> args = null) where T : new()
//        {
//            var _request = CreateJsonRequest(endpoint, Method.GET);
//            {
//                var _params = new Dictionary<string, object>();
//                {
//                    _params.Add("endpoint", endpoint);
//                    if (args != null)
//                    {
//                        foreach (var a in args)
//                            _params.Add(a.Key, a.Value);
//                    }
//                }

//                foreach (var a in _params)
//                    _request.AddParameter(a.Key, a.Value);
//            }

//            var _client = CreateJsonClient(WebApiUrl);
//            {
//                var tcs = new TaskCompletionSource<T>();
//                _client.ExecuteAsync(_request, response =>
//                {
//                    tcs.SetResult(JsonConvert.DeserializeObject<T>(response.Content));
//                });

//                return await tcs.Task;
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public void Dispose()
//        {
//        }
//    }
//}