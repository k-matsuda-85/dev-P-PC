using LogController;
using OrderTool_Serv;
using OrderTool_Serv.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ServiceIF serv = new ServiceIF();
            OrderTool_Serv.Class.Login logData = null;

            ResultData inRet = serv.Login_Serv("sga", "sga", out logData);


            if (inRet.Result)
            {
            }
        }
    }
}
