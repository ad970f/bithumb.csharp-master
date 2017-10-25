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

            ptb.SqlDependencyStart();

            Console.ReadLine();
        }

        static void Strategy(object source, PublicTickerEventArgs e)
        {
            Console.Write("\rStrategy Called at {0} ", DateTime.Now);

        }
    }
}
