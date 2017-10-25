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

            PublicTickerBroker ptb = new PublicTickerBroker((CoinType)comboBox1.SelectedItem);

            ptb.OnPublicTickerTableChanged += new EventHandler<PublicTickerEventArgs>(NewPublicTickerHandler);

            ptb.SqlDependencyStart();
        }


        void NewPublicTickerHandler(object source, PublicTickerEventArgs e)
        {

            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { UpdateChart(e); });
            }
            else
            {
                UpdateChart(e);
            }
        }


        void UpdateChart(PublicTickerEventArgs e)
        {
            decimal highestMax = decimal.MinValue;
            decimal lowestMin = decimal.MaxValue;

            for ( int i=0; i < e.publicTickers.Count; i++)
            {
                if (e.publicTickers[i].data.max_price > highestMax)
                    highestMax = e.publicTickers[i].data.max_price;

                if (e.publicTickers[i].data.min_price < lowestMin)
                    lowestMin = e.publicTickers[i].data.min_price;
            }

            siseChart.Series["closing_price"].Points.Clear();
            siseChart.Series["max_price"].Points.Clear();
            siseChart.Series["min_price"].Points.Clear();
            siseChart.Series["average_price"].Points.Clear();

            siseChart.ChartAreas[0].AxisY.Maximum = (double)highestMax * 1.01;
            siseChart.ChartAreas[0].AxisY.Minimum = (double)lowestMin * 0.99;

            for (int i = 0; i < e.publicTickers.Count - 1; i++)
            {
                siseChart.Series["closing_price"].Points.AddY(e.publicTickers[i].data.closing_price);
                siseChart.Series["max_price"].Points.AddY(e.publicTickers[i].data.max_price);
                siseChart.Series["min_price"].Points.AddY(e.publicTickers[i].data.min_price);
                siseChart.Series["average_price"].Points.AddY(e.publicTickers[i].data.average_price);
            }

            label1.Text = "24H Range% = " +
                ((e.publicTickers[e.publicTickers.Count - 1].data.max_price
                - e.publicTickers[e.publicTickers.Count - 1].data.min_price)
                / e.publicTickers[e.publicTickers.Count - 1].data.closing_price).ToString("P2")

                + ", Current = " + 
                e.publicTickers[e.publicTickers.Count - 1].data.closing_price.ToString("#,#")

                + ", Stochastic = " +

               ((e.publicTickers[e.publicTickers.Count - 1].data.closing_price
                - e.publicTickers[e.publicTickers.Count - 1].data.min_price)
               / (e.publicTickers[e.publicTickers.Count - 1].data.max_price
                - e.publicTickers[e.publicTickers.Count - 1].data.min_price)).ToString("P2")
            ;

            textBox1.AppendText(UnixTime.ToLocalTimeMilliString(e.publicTickers[e.publicTickers.Count - 1].data.date) + "   "
                + e.publicTickers[e.publicTickers.Count - 1].data.closing_price.ToString("#,###.####") + "   "
                + e.publicTickers[e.publicTickers.Count - 1].data.max_price.ToString("#,###.####") + "   "
                + e.publicTickers[e.publicTickers.Count - 1].data.min_price.ToString("#,###.####") + "   "
                + e.publicTickers[e.publicTickers.Count - 1].data.average_price.ToString("#,###.###0") + "   "
                + "\n");

        }

    }

}



