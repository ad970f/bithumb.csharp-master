using System;
using System.Collections.Generic;
using System.Globalization;
using Bithumb.LIB.Configuration;
//using Bithumb.LIB.Queue;
//using Bithumb.LIB.Types;
using Newtonsoft.Json;

namespace Bithumb.API.Public
{
    /// <summary>
    /// 
    /// </summary>
    public class PublicTransactionData
    {
        /// <summary>
        /// 멤버변수 이름의 매우 중요하다
        /// Json 객체가 Deseriealize 될 때 해당객체의 변수명과 사용자 클래스 멤버변수명이 일치해야 한다.
        /// </summary>
        /// <param name="transaction_date"></param>
        /// <param name="type"></param>
        /// <param name="units_traded"></param>
        /// <param name="price"></param>
        /// <param name="total"></param>
        [JsonConstructor]
        public PublicTransactionData(string transaction_date, string type, string units_traded, string price, string total)
        {
            if (UnixTime.IsDateTimeFormat(transaction_date) == true)
            {
                var _tdate = UnixTime.ConvertToUtcTime(transaction_date + "+09:00");
                this.transaction_date = UnixTime.ConvertToUnixTimeMilli(_tdate);
            }
            else
                this.transaction_date = Convert.ToInt64(transaction_date);

            this.type = type;
            this.units_traded = Convert.ToDecimal(units_traded);
            this.price = decimal.Parse(price, NumberStyles.Float);
            this.total = decimal.Parse(total, NumberStyles.Float);
        }

        /// <summary>
        /// 거래 채결 시간
        /// </summary>
        public long transaction_date
        {
            get;
            set;
        }

        /// <summary>
        /// 판/구매 (ask, bid)
        /// </summary>
        public string type
        {
            get;
            set;
        }

        /// <summary>
        /// 거래 Currency 수량 (BTC or ETH)
        /// </summary>
        public decimal units_traded
        {
            get;
            set;
        }

        /// <summary>
        /// 1Currency 거래 금액 (BTC or ETH)
        /// </summary>
        public decimal price
        {
            get;
            set;
        }


        /// <summary>
        /// 총 거래금액
        /// </summary>
        public decimal total
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public TradeHistory ToQueue(CoinType coinType = CoinType.btc, CurrencyType currencyType = CurrencyType.krw)
        //{
        //    return new TradeHistory
        //    {
        //        amount = this.units_traded,
        //        coin = coinType,
        //        currency = currencyType,
        //        dealer = DealerType.xcoin,
        //        price = this.price,
        //        tid = this.transaction_date,
        //        timestamp = this.transaction_date * 1000L,
        //        total = this.total,
        //        type = (this.type == "ask") ? OrderType.sell : OrderType.buy
        //    };
        //}
    }

    /// <summary>
    /// 
    /// </summary>
    public class PublicTransactions : ApiResult<List<PublicTransactionData>>
    {
        public PublicTransactions()
        {
            this.data = new List<PublicTransactionData>();
        }
    }
}