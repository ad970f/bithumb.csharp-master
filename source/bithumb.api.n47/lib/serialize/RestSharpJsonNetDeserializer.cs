using Newtonsoft.Json;
// REpresentational State Transfer API
// HTTP URI 로 표현된 리소스에 대한 행위를 HTTP Method 로 정의한다.
// 리소스의 내용은 json, xml, yaml 등의 다양한 언어로 정의된다.
// HTTP Method = POST GET PUT PATCH DELETE
using RestSharp.Deserializers;
using RestSharp;

namespace Bithumb.LIB.Serialize
{
    /// <summary>
    /// 
    /// RestClient 객체의 AddHandler 메소드의 인자로 넘겨질 객체
    /// 
    /// 
    /// 
    /// Default JSON serializer for request bodies
    /// Doesn't currently use the SerializeAs attribute, defers to Newtonsoft's attributes
    /// </summary>
    public class RestSharpJsonNetDeserializer : IDeserializer
    {
        /// <summary>
        /// Default deserializer
        /// </summary>
        public RestSharpJsonNetDeserializer()
        {
            DateFormat = "yyyy-MM-dd HH:mm:ss";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        public T Deserialize<T>(IRestResponse response)
        {
            var _js = new JsonSerializerSettings()
            {
                DateFormatString = DateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            return JsonConvert.DeserializeObject<T>(response.Content, _js);
        }

        public string RootElement
        {
            get;
            set;
        }

        public string Namespace
        {
            get;
            set;
        }

        public string DateFormat
        {
            get;
            set;
        }
    }
}