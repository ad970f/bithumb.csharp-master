using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Bithumb.LIB;
using Bithumb.LIB.Types;
using Bithumb.API.Info;
using Bithumb.API.Public;
using Bithumb.API.lib.database;
using Bithumb.LIB.Configuration;

namespace bithumb.chart
{
    public class CoinInfoData
    {
        /// <summary>
        /// 1회 최소주문수량
        /// </summary>
        public decimal MinQty { get; set; }

        /// <summary>
        /// 1회 최대주문수량
        /// </summary>
        public decimal MaxQty { get; set; }

        /// <summary>
        /// 주문수량 문자열포맷
        /// </summary>
        public string Format { get; set; }

        public CoinInfoData(decimal MinQty, decimal MaxQty, string Format)
        {
            this.MinQty = MinQty;
            this.MaxQty = MaxQty;
            this.Format = Format;
        }
    }

    public class CoinInfo : Dictionary<CoinType, CoinInfoData>
    {
    }


    public class PublicTickerSeries
    {
        public CoinType ct { get; set; }

        public int length { get; set; }

        public decimal[] closeSeries;
        public decimal[] maxSeries;
        public decimal[] minSeries;
        public decimal[] avgSeries;
        public decimal[] bidSeries;
        public decimal[] askSeries;
        public long[] ts;

        public PublicTickerSeries(CoinType ct, List<PublicTicker> ptList)
        {
            this.ct = ct;

            this.length = ptList.Count;

            closeSeries = new decimal[length];
            minSeries = new decimal[length];
            maxSeries = new decimal[length];
            avgSeries = new decimal[length];
            bidSeries = new decimal[length];
            askSeries = new decimal[length];
            ts = new long[length];

            for (int i = 0; i < ptList.Count; i++)
            {
                closeSeries[i] = ptList[i].data.closing_price;
                minSeries[i] = ptList[i].data.min_price;
                maxSeries[i] = ptList[i].data.max_price;
                avgSeries[i] = ptList[i].data.average_price;
                bidSeries[i] = ptList[i].data.buy_price;
                askSeries[i] = ptList[i].data.sell_price;
                ts[i] = ptList[i].data.date;
            }
        }



        public decimal DayVolatility
        {
            get
            {
                return ((maxSeries[length - 1] - minSeries[length - 1]) / closeSeries[length - 1]);
            }
        }

        public decimal Stochastic
        {
            get
            {
                return ((closeSeries[length - 1] - minSeries[length - 1]) 
                    / (maxSeries[length - 1] - minSeries[length - 1]));
            }
        }
    }






    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            foreach (CoinType ct in Enum.GetValues(typeof(CoinType)))
            {
                comboBox1.Items.Add(ct);
            }
        }

        void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;

            PublicTickerTbl pt_tbl = new PublicTickerTbl((CoinType)comboBox1.SelectedItem);

            pt_tbl.OnPublicTickerTblChanged += new PublicTickerEventHandler(PublicTickerTblChanged);
            //pt_tbl.OnPublicTickerTblChanged += new EventHandler(PublicTickerTblChanged);

            pt_tbl.SqlDependencyStart();
        }

        //This is the actual method that will be assigned to the event handler
        //within the above class. This is where we perform an action once the
        //event has been triggered.
        void PublicTickerTblChanged(object source, PublicTickerArgs e)
        //void PublicTickerTblChanged(object source, EventArgs ex)
        {
            //PublicTickerArgs e = (PublicTickerArgs)ex;

            PublicTickerSeries pts = new PublicTickerSeries((CoinType)comboBox1.SelectedItem, e.ptList);

            //--------------------------------------------------------------
            // Strategy (테이블에 PublicTicker 신규 발생 시 마다 호출 )
            //--------------------------------------------------------------
            //Strategy myStrategy = new Strategy(pts);

            //myStrategy.Run();

            //--------------------------------------------------------------



            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { UpdateChart(pts); });
            }
            else
            {
                UpdateChart(pts);
            }
        }


        void UpdateChart(PublicTickerSeries pts)
        {
            siseChart.Series["closing_price"].Points.Clear();
            siseChart.Series["max_price"].Points.Clear();
            siseChart.Series["min_price"].Points.Clear();
            siseChart.Series["average_price"].Points.Clear();

            siseChart.ChartAreas[0].AxisY.Maximum = (double)pts.maxSeries.Max() * 1.01;
            siseChart.ChartAreas[0].AxisY.Minimum = (double)pts.minSeries.Min() * 0.99;

            for (int i = 0; i < pts.length - 1; i++)
            {
                siseChart.Series["closing_price"].Points.AddY(pts.closeSeries[i]);
                siseChart.Series["max_price"].Points.AddY(pts.maxSeries[i]);
                siseChart.Series["min_price"].Points.AddY(pts.minSeries[i]);
                siseChart.Series["average_price"].Points.AddY(pts.avgSeries[i]);
            }

            label1.Text = "24H Range% = " + pts.DayVolatility.ToString("P2")
                + ", Current = " + pts.closeSeries[pts.length - 1].ToString("#,#")
                + ", Stochastic = " + pts.Stochastic.ToString("P2")                
                ;

            textBox1.AppendText(UnixTime.ToLocalTimeMilliString(pts.ts[pts.length - 1]) + "   "
                + pts.closeSeries[pts.length - 1].ToString("#,###.####") + "   "
                + pts.maxSeries[pts.length - 1].ToString("#,###.####") + "   "
                + pts.minSeries[pts.length - 1].ToString("#,###.####") + "   "
                + pts.avgSeries[pts.length - 1].ToString("#,###.###0") + "   "
                + "\n");

        }

    }








 //   //abstract class Strategy
 //   class Strategy
 //   {
 //       const string __connect_key = "379657e537c0af8d59be111443a1de76";
 //       const string __secret_key = "bb40d4de9427f7dbb35f08e6198a20d0";

 //       CoinInfoData cid;

 //       protected PublicTickerSeries __pts;

 //       public Strategy(PublicTickerSeries pts)
 //       {
 //           __pts = pts;

 //           CoinInfo ci = new CoinInfo()
 //           {
 //               { CoinType.BTC, new CoinInfoData(0.001m,300m,"N4") },
 //               { CoinType.ETH, new CoinInfoData(0.01m,2500m,"N1") },
 //               { CoinType.DASH, new CoinInfoData(0.01m,4000m,"N1") },
 //               { CoinType.LTC, new CoinInfoData(0.1m,15000m,"N1") },
 //               { CoinType.ETC, new CoinInfoData(0.1m,30000m,"N1") },
 //               { CoinType.XRP, new CoinInfoData(10m,2500000m,"N0") },
 //               { CoinType.BCH, new CoinInfoData(0.01m,1200m,"N1") }
 //           };

 //           cid = ci[__pts.ct];
 //       }

 //       //decimal balanceCashAmt; // 신규 매수를 위한 가용한 현금
 //       //decimal balanceCoinQty; // 청산을 위한 코인 잔고

 //       async void GetBalance()
 //       {
 //           var __info_api = new XUserApi(__connect_key, __secret_key);

 //           var _balance = await __info_api.Balance(__pts.ct.enumToString());

 //           if (_balance.status == 0)
 //           {

 //           }

 //       }







 //       public void Buy(decimal losscutPercent = 0.01m)
 //       {
 //           //decimal losscutUnit = __pts.closeSeries[__pts.Size - 1] - __pts.minSeries[__pts.Size - 1];
 //           //decimal losscutSize = (coinAmount + cashAmount) * losscutPercent;

 //           //decimal theoreticalBuyableQty = losscutSize / losscutUnit;
 //           //decimal theoreticalCashAmt = theoreticalBuyableQty * __pts.closeSeries[__pts.Size - 1];

 //           //decimal realBettingAmount = Math.Min(theoreticalCashAmt, cashAmount);
 //           //decimal realBettingQty = realBettingAmount / __pts.closeSeries[__pts.Size - 1];


 //       }


 //       public void Sell(decimal liquidationPercent = 1.0m)
 //       {
 ////           decimal realBettingQty = coinAmount * liquidationPercent;



 //       }

 //       public async void Run()
 //       {



 //       }

 //   }

    //class HalfPullbackStrategy : Strategy
    //{
    //    public HalfPullbackStrategy(PublicTickerSeries pts) : base(pts)
    //    {

    //    }

    //    public override void Run()
    //    {

    //    }

        
    //}

}
