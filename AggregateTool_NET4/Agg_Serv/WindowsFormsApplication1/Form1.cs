using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Agg_Serv;
using Agg_Serv.Class;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IF_Service serv = new IF_Service();

            string set = "server=157.2.1.100;database=ProRadRSWebDB;uid=sa;pwd=tryfor;MultipleActiveResultSets=true";
            var reports = serv.GetReport_Org("20170831", "20170831", set, "KHM");
            serv.GetHospital();
        }
    }
}
