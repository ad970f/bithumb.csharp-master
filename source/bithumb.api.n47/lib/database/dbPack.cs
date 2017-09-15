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
    public delegate void PublicTickerEventHandler(object source, PublicTickerArgs e);

    public class PublicTickerArgs : EventArgs
    {
        List<PublicTicker> __ptList;

        public PublicTickerArgs(List<PublicTicker> ptList)
        {
            __ptList = ptList;
        }


        public List<PublicTicker> ptList
        {
            get
            {
                return __ptList;
            }
            
        }
    }

    public class dbPack : IDisposable
    {
        protected CLogger __logger;

        protected readonly string __connection_string;

        protected dbPack()
        {
            __logger = new CLogger();

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
                    catch(Exception e)
                    {
                        __logger.WriteLog(e);
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

    public class PublicTickerTbl : dbPack
    {
        /*
         * public event EventHandler OnPublicTickerTblChanged;
         *  상기 형태로 미리 정의된 델리게이트 타잎 EventHandler 를 사용할 수 있음
         *  EventHandler 는 아래와 같이 미리 정의 되어 있음
         *  delegate void System.EventHandler(object sender, System.EventArgs e)
         *  따라서, 외부 클래스에서 결과 값을 처리하려면 EventArgs 을 타잎 캐스팅해야 함
         * 
         *  반환 타잎 캐스팅을 하는 번거로움을 피하기 위해 커스텀 델리게이트 타잎을 사용함
         * 
         */
        public event PublicTickerEventHandler OnPublicTickerTblChanged;

        /*
         * event 키워드 사용에 대해서
         *  event 키워드를 사용하면 클래스 외부에서 OnPublicTickerTblChanged 를 직접 호출할 수 없음
         *  객체.OnPublicTickerTblChanged [+=|-=] PublicTickerEventHandler(콜백함수) 형태로만 접근 가능
         * 
         *  event 키워드가 생략되면 클래스 외부에서 일반 멤버함수처럼 OnPublicTickerTblChanged 를 직접 호출 가능함
         */


        private readonly string __table_name;
        private readonly CoinType __coin_type;

        private const int __lookbackPeriod = 1;


        public PublicTickerTbl(CoinType ct)
        {
            __table_name = "PublicTicker" + ct.enumToString();
            __coin_type = ct;
        }

        public void SqlDependencyStart()
        {
            SqlDependencyStop();

            try
            {
                SqlDependency.Start(__connection_string);

                // 처음 한번은 DB 쿼리를 해야 이벤트가 동작을 시작한다
                SelectOnChange();
            }
            catch (Exception e)
            {
                __logger.WriteLog(e);
            }
        }

        public void SqlDependencyStop()
        {
            SqlDependency.Stop(__connection_string);
        }

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
                __logger.WriteLog(e);
            }

            //Disconnect();
        }

        public void SelectOnChange()
        {
            List<PublicTicker> ptList = new List<PublicTicker>();

            string sql = "SELECT [opening_price],[closing_price],[min_price],[max_price],[average_price],[units_traded],[volume_1day],[volume_7day],[buy_price],[sell_price],[ts]"
                 + " FROM dbo." + __table_name
                 + " WHERE ts > " + (long)(DateTime.UtcNow.AddDays(-1*__lookbackPeriod) - new DateTime(1970, 1, 1)).TotalMilliseconds
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
                        __logger.WriteLog(e.ToString());
                    }
                    else
                    {
                        // SQL Server 로 부터 이벤트가 발생하면 본 함수가 다시 호출될 수 있도록 델리게이트를 등록한다.
                        SelectOnChange();
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

                    ptList.Add(pt);
                }
            }

            //To make sure we only trigger the event if a handler is present
            //we check the event to make sure it's not null.
            if (OnPublicTickerTblChanged != null)
            { // callee 에서 함수가 지정된 경우에 지정된 함수를 실행한다 
                // 매개변수로 수취한 정보를 전달한다.
                OnPublicTickerTblChanged(this, new PublicTickerArgs(ptList));
            }
        }

    }


}
