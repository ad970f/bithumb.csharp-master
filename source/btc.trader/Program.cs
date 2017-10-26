using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bithumb.LIB;
using Bithumb.LIB.Types;
using Bithumb.API.Info;
using Bithumb.API.Public;
using Bithumb.API.lib.database;
using Bithumb.LIB.Configuration;

namespace btc.trader
{
    class Program
    {
        static CoinType ct = CoinType.BTC;

        static void Main(string[] args)
        {
            Console.WriteLine("Strategy Started. Press Any Key To Stop.\n\n\n");

            PublicTickerBroker ptb = new PublicTickerBroker(ct);

            ptb.OnPublicTickerTableChanged += new EventHandler<PublicTickerEventArgs>(Strategy);

            ptb.SqlDependencyStart(1);

            Console.ReadLine();
        }

        /// <summary>
        /// 티커 테이블에 시계열 데이터 한 행이 추가될 때 마다 전략 코드를 실행한다.
        /// 예스 트레이더 혹은 스팟의 스크립트가 호출되는 구조와 같다.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        static void Strategy(object source, PublicTickerEventArgs e)
        {
            Console.Write("\rStrategy Called at {0} ", DateTime.Now);

        }
    }
}
