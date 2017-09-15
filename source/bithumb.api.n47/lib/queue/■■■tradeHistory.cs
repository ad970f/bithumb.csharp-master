//using Newtonsoft.Json;
//using Newtonsoft.Json.Converters;
//using Bithumb.LIB.Types;

//namespace Bithumb.LIB.Queue
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public class TradeHistory
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        [JsonConverter(typeof(StringEnumConverter))]
//        public DealerType dealer;

//        /// <summary>
//        /// Indicate 'buy' or 'sell' trade.
//        /// </summary>
//        [JsonConverter(typeof(StringEnumConverter))]
//        public OrderType type;

//        /// <summary>
//        /// Trade id.
//        /// </summary>
//        public long tid;

//        /// <summary>
//        /// Unix time in seconds since 1 January 1970.
//        /// </summary>
//        public long timestamp;

//        /// <summary>
//        /// 
//        /// </summary>
//        [JsonConverter(typeof(StringEnumConverter))]
//        public CurrencyType currency;

//        /// <summary>
//        /// 
//        /// </summary>
//        [JsonConverter(typeof(StringEnumConverter))]
//        public CoinType coin;

//        /// <summary>
//        /// Price for 1 BTC.
//        /// </summary>
//        public decimal price;

//        /// <summary>
//        /// Amount of BTC traded.
//        /// </summary>
//        public decimal amount;

//        /// <summary>
//        /// 
//        /// </summary>
//        public decimal total;
//    }
//}