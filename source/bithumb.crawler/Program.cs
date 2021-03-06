﻿using System;
using System.Threading;

using Bithumb.API.Public;
using Bithumb.LIB;
using Bithumb.LIB.Types;
using Bithumb.API.lib.database;
using Bithumb.LIB.Configuration;

namespace bithumb.crawler
{
    class Program
    {
        // 프로그램 기동 시 현 시스템 시각으로 초기화
        static DateTime prevTime = DateTime.Now;

        static void Main(string[] args)
        {
            Console.WriteLine("Bithumb Public Ticker Data Request Start. Press Any Key To Stop.\n\n\n");

            Timer t = new Timer(callback, null, 0, 1000 * 2);

            Console.ReadLine();

            t.Dispose();

        }

        /// <summary>
        /// 타이머 이벤트 콜백
        /// </summary>
        /// <param name="state"></param>
        static void callback(Object state)
        {
            DateTime currentTime = DateTime.Now;

            //if (currentTime.Minute == prevTime.Minute)
            //    return; // 같은 분 안에서는 동작 안함

            prevTime = currentTime; // 분이 바뀌었음 

            // \r 스위치로 콘솔의 동일 행에 겹쳐쓰기
            Console.Write("\rCrawler requests public ticker data at {0} ", DateTime.Now);

            foreach (CoinType ct in Enum.GetValues(typeof(CoinType)))
            {
                XPublicTicker(ct);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        static async void XPublicTicker(CoinType ct)
        {
            var _public_api = new XPublicApi();

            try
            {
                var _ticker = await _public_api.Ticker(ct.enumToString());

                if (_ticker != null)
                {
                    if (_ticker.status == 0)
                    {
                        // 정상 수신 시 DB Insert
                        PublicTickerBroker ptt = new PublicTickerBroker(ct);


                        ptt.Insert(_ticker);
                    }
                    else
                    {
                        Console.WriteLine("_ticker.status = {0}, _ticker.message = {1}", _ticker.status, _ticker.message);
                    }
                }

            }
            catch(Exception e)
            {
                new CLogger().WriteLog(e);
            }


        }
    }
}

