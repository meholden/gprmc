using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gprmc
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            gprmc.nmea nmeastuff = new gprmc.nmea(); // declare the class


            if (nmeastuff.processString("$GPRMC,230046,A,3759.8006,N,12"))
            {
                if (nmeastuff.gpsgood)
                {
                    button1.Text = "+++" + nmeastuff.lat.ToString();
                    button1.Text += "\r\n";
                    button1.Text += nmeastuff.lon.ToString();
                    button1.Text += "\r\n";
                    button1.Text += nmeastuff.sog.ToString();
                    button1.Text += "\r\n";
                    button1.Text += nmeastuff.cog.ToString();

                    nmeastuff.gotsentence = false; // reset now that we've used the data
                }
            }
            if (nmeastuff.processString("205.4429,W,0.0,190.3,260702,15.1,E,A*3C"))
            {
                if (nmeastuff.gpsgood)
                {
                    button1.Text = nmeastuff.lat.ToString();
                    button1.Text += "\r\n";
                    button1.Text += nmeastuff.lon.ToString();
                    button1.Text += "\r\n";
                    button1.Text += nmeastuff.sog.ToString();
                    button1.Text += "\r\n";
                    button1.Text += nmeastuff.cog.ToString();

                    nmeastuff.gotsentence = false; // reset now that we've used the data
                }
                else
                {
                    button1.Text = "bad gps";
                }
            }

        }
    }
}
