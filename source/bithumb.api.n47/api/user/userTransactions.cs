using System.Collections.Generic;

namespace Bithumb.API.Info
{
    /// <summary>
    /// 회원 거래 내역
    /// </summary>
    public class UserTransactionData
    {
        /// <summary>
        /// 검색 구분 (0 : 전체, 1 : 구매완료, 2 : 판매완료, 3 : 출금중, 4 : 입금, 5 : 출금, 9 : KRW입금중)
        /// </summary>
        public int search
        {
            get;
            set;
        }

        /// <summary>
        /// 거래 일시 Timestamp (완료일자)
        /// </summary>
        public long transfer_date
        {
            get;
            set;
        }

        /// <summary>
        /// 거래 Currency 수량 (BTC or ETH)
        /// </summary>
        public string units
        {
            get;
            set;
        }

        /// <summary>
        /// 거래금액
        /// </summary>
        public decimal price
        {
            get;
            set;
        }

        /// <summary>
        /// 1Currency당 거래금액 (KRW)
        /// </summary>
        public decimal btc1krw
        {
            get;
            set;
        }

        /// <summary>
        /// 1Currency당 거래금액 (ETH)
        /// </summary>
        public decimal eth1krw
        {
            get;
            set;
        }

        /// <summary>
        /// 거래수수료
        /// </summary>
        public string fee
        {
            get;
            set;
        }

        /// <summary>
        /// 거래 후 BTC 잔액
        /// </summary>
        public decimal btc_remain
        {
            get;
            set;
        }

        /// <summary>
        /// 거래 후 ETH 잔액
        /// </summary>
        public decimal eth_remain
        {
            get;
            set;
        }

        /// <summary>
        /// 거래 후 KRW 잔액
        /// </summary>
        public decimal krw_remain
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 회원 거래 내역
    /// </summary>
    public class UserTransactions : ApiResult<List<UserTransactionData>>
    {
        public UserTransactions()
        {
            this.data = new List<UserTransactionData>();
        }
    }
}