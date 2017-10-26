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

            ptb.SqlDependencyStart(1);
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
            siseChart.Series["closing_price"].Points.Clear();
            siseChart.Series["max_price"].Points.Clear();
            siseChart.Series["min_price"].Points.Clear();
            siseChart.Series["average_price"].Points.Clear();

            siseChart.ChartAreas[0].AxisY.Maximum = (double)e.max_prices.Max() * 1.01;
            siseChart.ChartAreas[0].AxisY.Minimum = (double)e.min_prices.Min() * 0.99;

            for (int i = 0; i < e.Count - 1; i++)
            {
                siseChart.Series["closing_price"].Points.AddY(e.closing_prices[i]);
                siseChart.Series["max_price"].Points.AddY(e.max_prices[i]);
                siseChart.Series["min_price"].Points.AddY(e.min_prices[i]);
                siseChart.Series["average_price"].Points.AddY(e.average_prices[i]);
            }

            decimal dayVolatility = e.closing_prices[e.Count - 1] == 0 ? 0 : 
                (e.max_prices[e.Count - 1] - e.min_prices[e.Count - 1]) / e.closing_prices[e.Count - 1];

            decimal stochastics = e.max_prices[e.Count - 1] == e.min_prices[e.Count - 1] ? 0 :
                (e.closing_prices[e.Count - 1] - e.min_prices[e.Count - 1]) / (e.max_prices[e.Count - 1] - e.min_prices[e.Count - 1]);


            label1.Text = "24 Hour Range% = " + dayVolatility.ToString("P2")
                + ", Current = " + e.closing_prices[e.Count - 1].ToString("#,#")
                + ", Stochastic = " + stochastics.ToString("P2");

            textBox1.AppendText(UnixTime.ToLocalTimeMilliString(e.time_stamps[e.Count - 1]) + "   "
                + e.closing_prices[e.Count - 1].ToString("#,###.####") + "   "
                + e.max_prices[e.Count - 1].ToString("#,###.####") + "   "
                + e.min_prices[e.Count - 1].ToString("#,###.####") + "   "
                + e.average_prices[e.Count - 1].ToString("#,###.###0") + "   "
                + "\n");

        }

    }

}



