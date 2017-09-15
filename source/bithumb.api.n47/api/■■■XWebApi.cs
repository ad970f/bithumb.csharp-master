//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Bithumb.LIB;
//using Bithumb.LIB.Configuration;
//using Bithumb.LIB.Models;

//namespace Bithumb.API
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public partial class XWebApi
//    {
//        private OWebClient __xcoin_web_client = null;

//        private OWebClient XCoinWebClient
//        {
//            get
//            {
//                if (__xcoin_web_client == null)
//                    __xcoin_web_client = new OWebClient("https://www.bithumb.com/");
//                return __xcoin_web_client;
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <returns></returns>
//        public async Task<Periods> GetXCoinTradeDays()
//        {
//            var _result = new Periods();

//            var _params = new Dictionary<string, object>();
//            {
//                _params.Add("key", UnixTime.NowMilli.ToString());
//            }

//            var _values = await XCoinWebClient.CallApiAsync("/resources/csv/xcoinTrade_minute.json", _params);
//            _result.AddRange(
//                _values
//                    .Select(x => new PeriodData()
//                    {
//                        time_value = Convert.ToInt64(x[0]) / 1000,

//                        open_price = Convert.ToDecimal(x[1]),
//                        close_price = Convert.ToDecimal(x[2]),

//                        high_price = Convert.ToDecimal(x[3]),
//                        low_price = Convert.ToDecimal(x[4]),

//                        sell_volume = Convert.ToDecimal(x[5])
//                    })
//            );

//            return _result;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <returns></returns>
//        public async Task<Periods> GetXCoinTradeMonths()
//        {
//            var _result = new Periods();

//            var _params = new Dictionary<string, object>();
//            {
//                _params.Add("key", DateTime.Now.ToString("yyyyMMdd"));
//            }

//            var _values = await XCoinWebClient.CallApiAsync("/resources/csv/xcoinTrade.json", _params);
//            _result.AddRange(
//                _values
//                    .Select(x => new PeriodData()
//                    {
//                        time_value = Convert.ToInt64(x[0]) / 1000,

//                        open_price = Convert.ToDecimal(x[1]),
//                        close_price = Convert.ToDecimal(x[2]),

//                        high_price = Convert.ToDecimal(x[3]),
//                        low_price = Convert.ToDecimal(x[4]),

//                        sell_volume = Convert.ToDecimal(x[5])
//                    })
//            );

//            return _result;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <returns></returns>
//        public async Task<CurrencyRates> GetXCoinCurrencyRate()
//        {
//            var _params = new Dictionary<string, object>();
//            {
//                _params.Add("_", UnixTime.NowMilli);
//            }

//            return await XCoinWebClient.CallApiAsync<CurrencyRates>("/resources/csv/CurrencyRate.json", _params);
//        }
//    }
//}