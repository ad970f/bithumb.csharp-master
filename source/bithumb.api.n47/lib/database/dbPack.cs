using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;

using Bithumb.LIB;
using Bithumb.LIB.Configuration;
using Bithumb.API.Public;
using Bithumb.LIB.Types;

namespace Bithumb.API.lib.database
{
    /// <summary>
    /// 
    /// </summary>
    public class dbConnector : IDisposable
    {
        protected readonly string __connection_string;

        protected dbConnector()
        {
            __connection_string = "Data Source=.;Initial Catalog=Bithumb;Integrated Security=True";
        }

        private SqlConnection __conn = null;
        protected SqlConnection Connection
        {
            get
            {
                if (__conn == null)
                {
                    try
                    {
                        __conn = new SqlConnection();
                        __conn.ConnectionString = __connection_string;
                        __conn.Open();
                    }
                    catch (Exception e)
                    {
                        new CLogger().WriteLog(e);
                    }

                }

                return __conn;
            }
        }

        public void Dispose()
        {
            if (__conn != null)
                __conn.Close();
        }
    }

    /// <summary>
    /// Caller 이벤트핸들러에게 전달할 매개변수
    /// </summary>
    public class PublicTickerEventArgs : EventArgs
    {
        public List<decimal> opening_prices = new List<decimal>();
        public List<decimal> closing_prices = new List<decimal>();
        public List<decimal> min_prices = new List<decimal>();
        public List<decimal> max_prices = new List<decimal>();
        public List<decimal> average_prices = new List<decimal>();
        public List<decimal> units_tradeds = new List<decimal>();
        public List<decimal> volume_1days = new List<decimal>();
        public List<decimal> volume_7days = new List<decimal>();
        public List<decimal> buy_prices = new List<decimal>();
        public List<decimal> sell_prices = new List<decimal>();
        public List<Int64> time_stamps = new List<Int64>();

        public int Count { get; }

        public PublicTickerEventArgs(List<PublicTicker> publicTickers)
        {

            Count = publicTickers.Count;

            foreach (PublicTicker pt in publicTickers)
            {
                opening_prices.Add(pt.data.opening_price);
                closing_prices.Add(pt.data.closing_price);
                min_prices.Add(pt.data.min_price);
                max_prices.Add(pt.data.max_price);
                average_prices.Add(pt.data.average_price);
                units_tradeds.Add(pt.data.units_traded);
                volume_1days.Add(pt.data.volume_1day);
                volume_7days.Add(pt.data.volume_7day);
                buy_prices.Add(pt.data.buy_price);
                sell_prices.Add(pt.data.sell_price);
                time_stamps.Add(pt.data.date);
            }

        }

    }

    /// <summary>
    /// PublicTicker 테이블의 CRUD 를 위한 타잎
    /// </summary>
    public class PublicTickerBroker : dbConnector
    {
        public event EventHandler<PublicTickerEventArgs> OnPublicTickerTableChanged;

        private readonly string __table_name;
        private readonly CoinType __coin_type;

        // 단위 : Hours
        private int __lookbackHours;

        public PublicTickerBroker(CoinType ct)
        {
            __table_name = "PublicTicker" + ct.enumToString();
            __coin_type = ct;
 
        }

        /// <summary>
        /// Crawler 가 빗썸 사이트에 Request 한 시세 정보를 DB 에 저장
        /// </summary>
        /// <param name="pt"></param>
        public void Insert(PublicTicker pt)
        {
            string insertQuery = string.Format("INSERT INTO {0} ", __table_name);

            insertQuery += "(opening_price, closing_price, min_price, max_price, average_price, units_traded, volume_1day, volume_7day, buy_price, sell_price, ts) VALUES ";
            insertQuery += "(@opening_price, @closing_price, @min_price, @max_price, @average_price, @units_traded, @volume_1day, @volume_7day, @buy_price, @sell_price, @ts)";

            SqlCommand insertCommand = new SqlCommand(insertQuery, Connection);

            insertCommand.Parameters.Add("@opening_price", SqlDbType.Decimal);
            insertCommand.Parameters.Add("@closing_price", SqlDbType.Decimal);
            insertCommand.Parameters.Add("@min_price", SqlDbType.Decimal);
            insertCommand.Parameters.Add("@max_price", SqlDbType.Decimal);
            insertCommand.Parameters.Add("@average_price", SqlDbType.Decimal);
            insertCommand.Parameters.Add("@units_traded", SqlDbType.Decimal);
            insertCommand.Parameters.Add("@volume_1day", SqlDbType.Decimal);
            insertCommand.Parameters.Add("@volume_7day", SqlDbType.Decimal);
            insertCommand.Parameters.Add("@buy_price", SqlDbType.Decimal);
            insertCommand.Parameters.Add("@sell_price", SqlDbType.Decimal);
            insertCommand.Parameters.Add("@ts", SqlDbType.BigInt);

            insertCommand.Parameters["@opening_price"].Value = pt.data.opening_price;
            insertCommand.Parameters["@closing_price"].Value = pt.data.closing_price;
            insertCommand.Parameters["@min_price"].Value = pt.data.min_price;
            insertCommand.Parameters["@max_price"].Value = pt.data.max_price;
            insertCommand.Parameters["@average_price"].Value = pt.data.average_price;
            insertCommand.Parameters["@units_traded"].Value = pt.data.units_traded;
            insertCommand.Parameters["@volume_1day"].Value = pt.data.volume_1day;
            insertCommand.Parameters["@volume_7day"].Value = pt.data.volume_7day;
            insertCommand.Parameters["@buy_price"].Value = pt.data.buy_price;
            insertCommand.Parameters["@sell_price"].Value = pt.data.sell_price;
            insertCommand.Parameters["@ts"].Value = pt.data.date;

            try
            {
                insertCommand.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                new CLogger().WriteLog(e);
            }
        }

        /// <summary>
        /// 시세 테이블에 신규 데이터 발생 시 Select 한 행집합을 매개변수로 Caller 이벤트핸들러 호출
        /// </summary>
        public void OnChangeFromDB()
        {
            List<PublicTicker> publicTickers = new List<PublicTicker>();

            string sql = "SELECT [opening_price],[closing_price],[min_price],[max_price],[average_price],[units_traded],[volume_1day],[volume_7day],[buy_price],[sell_price],[ts]"
                 + " FROM dbo." + __table_name
                 + " WHERE ts > " + (long)(DateTime.UtcNow.AddHours(-1*__lookbackHours) - new DateTime(1970, 1, 1)).TotalMilliseconds
                 + " ORDER BY ts ASC";

            using (SqlCommand cmd = new SqlCommand(sql, Connection))
            {
                // (optional) 이전 Notification 삭제
                cmd.Notification = null;

                var sqlDependency = new SqlDependency(cmd);

                sqlDependency.OnChange += (o, e) => // object sender, SqlNotificationEventArgs e
                {
                    if (e.Info == SqlNotificationInfo.Invalid)
                    {
                        new CLogger().WriteLog(e.ToString());
                    }
                    else
                    {
                        // SQL Server 로 부터 이벤트가 발생하면 본 함수가 다시 호출될 수 있도록 델리게이트를 등록한다.
                        OnChangeFromDB();
                    }
                };


                DataTable dt = new DataTable();

                dt.Load(cmd.ExecuteReader());

                foreach (DataRow row in dt.Rows)
                {
                    PublicTicker pt = new PublicTicker();

                    pt.data.opening_price = (decimal)row["opening_price"];
                    pt.data.closing_price = (decimal)row["closing_price"];
                    pt.data.min_price = (decimal)row["min_price"];
                    pt.data.max_price = (decimal)row["max_price"];
                    pt.data.average_price = (decimal)row["average_price"];
                    pt.data.units_traded = (decimal)row["units_traded"];
                    pt.data.volume_1day = (decimal)row["volume_1day"];
                    pt.data.volume_7day = (decimal)row["volume_7day"];
                    pt.data.buy_price = (decimal)row["buy_price"];
                    pt.data.sell_price = (decimal)row["sell_price"];
                    pt.data.date = Convert.ToInt64(row["ts"]);// (long)row["ts"];

                    publicTickers.Add(pt);
                }
            }

            //To make sure we only trigger the event if a handler is present
            //we check the event to make sure it's not null.
            if (OnPublicTickerTableChanged != null)
            { // callee 에서 함수가 지정된 경우에 지정된 함수를 실행한다 
                // 매개변수로 수취한 정보를 전달한다.
                OnPublicTickerTableChanged(this, new PublicTickerEventArgs(publicTickers));
            }
        }

        /// <summary>
        //
        /// </summary>
        /// <param name="lookbackHours">Default: 1 Hour</param>
        public void SqlDependencyStart(int lookbackHours = 1)
        {
            __lookbackHours = lookbackHours;

            SqlDependencyStop();

            try
            {
                SqlDependency.Start(__connection_string);

                // 처음 한번은 DB 쿼리를 해야 이벤트가 동작을 시작한다
                OnChangeFromDB();
            }
            catch (Exception e)
            {
                new CLogger().WriteLog(e);
            }
        }

        public void SqlDependencyStop()
        {
            SqlDependency.Stop(__connection_string);
        }

    }


}
