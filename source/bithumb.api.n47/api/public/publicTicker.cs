namespace Bithumb.API.Public
{
    /// <summary>
    /// 
    /// </summary>
    public class PublicTickerData
    {
        /// <summary>
        /// 최근 24시간 내 시작 거래금액
        /// </summary>
        public decimal opening_price
        {
            get;
            set;
        }

        /// <summary>
        /// 최근 24시간 내 마지막 거래금액
        /// </summary>
        public decimal closing_price
        {
            get;
            set;
        }

        /// <summary>
        /// 최근 24시간 내 최저 거래금액
        /// </summary>
        public decimal min_price
        {
            get;
            set;
        }

        /// <summary>
        /// 최근 24시간 내 최고 거래금액
        /// </summary>
        public decimal max_price
        {
            get;
            set;
        }

        /// <summary>
        /// 최근 24시간 내 평균 거래금액
        /// </summary>
        public decimal average_price
        {
            get;
            set;
        }

        /// <summary>
        /// 최근 24시간 내 BTC 거래량
        /// </summary>
        public decimal units_traded
        {
            get;
            set;
        }

        /// <summary>
        /// 최근 1일간 BTC 거래량
        /// </summary>
        public decimal volume_1day
        {
            get;
            set;
        }

        /// <summary>
        /// 최근 7일간 BTC 거래량
        /// </summary>
        public decimal volume_7day
        {
            get;
            set;
        }

        /// <summary>
        /// 매수 1호가
        /// </summary>
        public decimal buy_price
        {
            get;
            set;
        }

        /// <summary>
        /// 매도 1호가
        /// </summary>
        public decimal sell_price
        {
            get;
            set;
        }

        /// <summary>
        /// 현재 시간 Timestamp
        /// </summary>
        public long date
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PublicTicker : ApiResult<PublicTickerData>
    { // 자기 Property + 공통 상속 Property ( message & status )
        public PublicTicker()
        {
            this.data = new PublicTickerData();
        }
    }
}