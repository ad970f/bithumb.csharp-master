using System;
using System.Threading;

using Bithumb.API.Public;
using Bithumb.LIB;
using Bithumb.LIB.Types;
using Bithumb.API.lib.database;

namespace bithumb.crawler
{
    class Program
    {
        // 프로그램 기동 시 현 시스템 시각으로 초기화
        static DateTime prevTime = DateTime.Now;

        static void Main(string[] args)
        {
            Timer t = new Timer(callback, null, 0, 1000*1);

            Console.WriteLine("Bithumb Public Ticker Data Request Start. Press Any Key To Stop.");
            Console.ReadLine();

            t.Dispose();

        }


        static void callback(Object state)
        {
            DateTime currentTime = DateTime.Now;

            if (currentTime.Minute == prevTime.Minute)
                return; // 같은 분 안에서는 동작 안함

            prevTime = currentTime; // 분이 바뀌었음 

            Console.WriteLine("{0} Crawler requests public ticker data", DateTime.Now);

            foreach (CoinType ct in Enum.GetValues(typeof(CoinType)))
            {
                XPublicTicker(ct);
            }

        }

        static async void XPublicTicker(CoinType ct)
        {
            string currency = ct.enumToString();

            var _public_api = new XPublicApi();

            var _ticker = await _public_api.Ticker(currency);

            if (_ticker != null)
            { // null return  이 오는 경우가 있다.
                if( _ticker.status == 0 )
                {
                    // 정상 수신 시 DB Insert
                    PublicTickerTbl ptt = new PublicTickerTbl(ct);

                    
                    ptt.Insert(_ticker);
                }
                else
                {
                    Console.WriteLine("_ticker.status = {0}, _ticker.message = {1}", _ticker.status, _ticker.message);
                }
            }
        }
    }
}

