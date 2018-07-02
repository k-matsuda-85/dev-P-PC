using Agg_Serv;
using Agg_Serv.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AggregateTool
{
    public partial class Form_EditHosp : Form
    {
        public Form_OutPutAggregate form_menu = new Form_OutPutAggregate();
        private static List<Hospital> RetHospList = new List<Hospital>();
        public Form_EditHosp()
        {
            InitializeComponent();

            IF_Service service = new IF_Service();

            RetHospList = service.GetHospital().ToList();


            foreach(var hosp in RetHospList)
            {
                checkedListBox1.Items.Add(hosp.Name, true);
            }

            checkBox1.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemChecked(i) == true)
                {
                    form_menu.YayoiHosps.Add(RetHospList[i].Name_DB);
                }
            }

            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            bool check = false;

            if(checkBox1.Checked)
                check = true;

            for(int i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, check);
        }
    }
}
