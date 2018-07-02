using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Agg_Serv;
using Agg_Serv.Class;

namespace AggregateTool
{
    public partial class Form_Setting : Form
    {
        private static List<Hospital> RetHospList = new List<Hospital>();
        private static List<Hospital> RetHospList_Admin = new List<Hospital>();
        private static List<User> RetUserList = new List<User>();
        private static List<Doctor> RetdocList = new List<Doctor>();
        private static List<Config> SystemConfig = new List<Config>();
        private static List<Config> HospConfig = new List<Config>();
        private static List<string> ModalityList = new List<string>();
        private static List<string> ModalityDocList = new List<string>();
        private static string HospCd = "";
        public Form_Menu form_menu;
        public User UserRet = new User();

        public Form_Setting()
        {
            InitializeComponent();

            InitDoctor();

            GetSystemSetting();
            LoadSetting_Hosp();
            LoadSetting_User();
            LoadSetting_Doctor();
            LoadSetting_Mod();
            LoadSetting_Mod_Doc();
            LoadSetting();

            if(UserRet.IsAdmin != "1")
            {
                tabPage5.Hide();
            }
        }

        private void GetSystemSetting()
        {
            try
            {
                IF_Service service = new IF_Service();

                SystemConfig = service.GetSystemConfig().ToList();
            }
            catch (Exception e)
            {

            }

        }

        #region 出力設定
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButton2.Checked)
            {
                radioButton1.Enabled = false;
                radioButton3.Enabled = false;
            }
            else
            {
                radioButton1.Enabled = true;
                radioButton3.Enabled = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                dataGridView1.Enabled = true;
            else
                dataGridView1.Enabled = false;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
                dataGridView2.Enabled = true;
            else
                dataGridView2.Enabled = false;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked){
                dataGridView7.Enabled = true;
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
            }
            else
            {
                dataGridView7.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
            }
        }

        private void cmb_Hosp_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 施設変更
            HospCd = cmb_Hosp.SelectedValue.ToString();

            LoadSetting_Disp();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 出力設定更新
            if (!Check_Hosp())
                return;

            var ret = MessageBox.Show("入力した内容で更新しますが\nよろしいですか？", "更新", MessageBoxButtons.YesNo);

            if (ret == System.Windows.Forms.DialogResult.Yes)
            {
                SetSetting_Disp();

                MessageBox.Show("施設情報を更新しました。", "完了", MessageBoxButtons.OK);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 出力設定キャンセル
            var ret = MessageBox.Show("入力を取り消しますが\nよろしいですか？", "キャンセル", MessageBoxButtons.YesNo);

            if (ret == System.Windows.Forms.DialogResult.Yes)
                LoadSetting_Disp();
        }

        private bool Check_Disp()
        {
            bool isRet = true;

            if(rdb_Export.Checked == false && radioButton2.Checked == false)
            {
                MessageBox.Show("出力設定を選択してください。", "エラー", MessageBoxButtons.OK);
                rdb_Export.Focus();
                return false;
            }
            if (radioButton2.Checked && (!radioButton1.Checked && !radioButton3.Checked))
            {
                MessageBox.Show("読影医集計の金額有無を選択してください。", "エラー", MessageBoxButtons.OK);
                radioButton1.Focus();
                return false;
            }

            if (checkBox1.Checked)
            {
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    if (dataGridView1.Rows[i].Cells[0].Value == null
                        || dataGridView1.Rows[i].Cells[1].Value == null
                        || dataGridView1.Rows[i].Cells[2].Value == null
                        || dataGridView1.Rows[i].Cells[3].Value == null)
                    {
                        MessageBox.Show("空白の項目があります。\n全て項目に値を入力してください。", "エラー", MessageBoxButtons.OK);

                        dataGridView1.Rows[i].Selected = true;
                        return false;
                    }
                }
            }
            if(checkBox2.Checked)
            {
                for (int i = 0; i < dataGridView2.Rows.Count - 1; i++)
                {
                    if (dataGridView2.Rows[i].Cells[0].Value == null
                        || dataGridView2.Rows[i].Cells[1].Value == null
                        || dataGridView2.Rows[i].Cells[2].Value == null
                        || dataGridView2.Rows[i].Cells[3].Value == null)
                    {
                        MessageBox.Show("空白の項目があります。\n全て項目に値を入力してください。", "エラー", MessageBoxButtons.OK);

                        dataGridView2.Rows[i].Selected = true;
                        return false;
                    }
                }
            }
            if(checkBox3.Checked)
            {
                if(String.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("空白の項目があります。\n全て項目に値を入力してください。", "エラー", MessageBoxButtons.OK);
                    textBox1.Focus();
                    return false;
                }
                if (String.IsNullOrEmpty(textBox2.Text))
                {
                    MessageBox.Show("空白の項目があります。\n全て項目に値を入力してください。", "エラー", MessageBoxButtons.OK);
                    textBox2.Focus();
                    return false;
                }
                if (String.IsNullOrEmpty(textBox3.Text))
                {
                    MessageBox.Show("空白の項目があります。\n全て項目に値を入力してください。", "エラー", MessageBoxButtons.OK);
                    textBox3.Focus();
                    return false;
                }

                if (cmbAddCost.SelectedValue == null)
                {
                    MessageBox.Show("空白の項目があります。\n全て項目に値を入力してください。", "エラー", MessageBoxButtons.OK);
                    cmbAddCost.Focus();
                    return false;
                }

                for (int i = 0; i < dataGridView7.Rows.Count - 1; i++)
                {
                    if (dataGridView7.Rows[i].Cells[0].Value == null
                        || dataGridView7.Rows[i].Cells[1].Value == null
                        || dataGridView7.Rows[i].Cells[2].Value == null
                        || dataGridView7.Rows[i].Cells[3].Value == null)
                    {
                        MessageBox.Show("空白の項目があります。\n全て項目に値を入力してください。", "エラー", MessageBoxButtons.OK);

                        dataGridView7.Rows[i].Selected = true;
                        return false;
                    }
                }
            }

            return isRet;
        }

        private void LoadSetting_Disp()
        {
            try
            {
                rdb_Export.Checked = false;
                radioButton2.Checked = false;
                radioButton1.Checked = false;
                radioButton3.Checked = false;
                radioButton4.Checked = false;
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                radioButton1.Enabled = false;
                radioButton3.Enabled = false;
                dataGridView1.Enabled = false;
                dataGridView2.Enabled = false;
                dataGridView7.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                dataGridView1.Rows.Clear();
                dataGridView2.Rows.Clear();
                dataGridView7.Rows.Clear();
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                cmbAddCost.SelectedValue = "12";


                IF_Service service = new IF_Service();

                HospConfig = service.GetHospitalConfig(HospCd).ToList();
                if (HospConfig.Count == 0)
                    return;

                foreach (var conf in HospConfig)
                {
                    switch(conf.Key)
                    {
                        case "Dist":
                            if (conf.Value == "0")
                            {
                                rdb_Export.Checked = true;
                                radioButton2.Checked = false;
                                radioButton4.Checked = false;
                            }
                            else if (conf.Value == "1")
                            {
                                rdb_Export.Checked = false;
                                radioButton2.Checked = true;
                                radioButton4.Checked = false;
                            }
                            else if (conf.Value == "2")
                            {
                                rdb_Export.Checked = false;
                                radioButton2.Checked = false;
                                radioButton4.Checked = true;
                            }

                            break;
                        case "Cost":
                            if (conf.Value == "1")
                            {
                                radioButton1.Checked = true;
                                radioButton3.Checked = false;
                            }
                            else if (conf.Value == "2")
                            {
                                radioButton1.Checked = false;
                                radioButton3.Checked = true;
                            }
                            break;
                        case "Hosp":
                            if (!String.IsNullOrEmpty(conf.Value))
                            {
                                checkBox1.Checked = true;
                                var tmpList = conf.Value.Split('@');

                                for (int i = 0; i < tmpList.Length; i++)
                                {
                                    var tmp = tmpList[i].Split(':');
                                    var hos = tmp[0];
                                    var values = tmp[1].Split(',');

                                    dataGridView1.Rows.Add();

                                    dataGridView1.Rows[i].Cells[0].Value = values[0];
                                    dataGridView1.Rows[i].Cells[1].Value = hos;
                                    dataGridView1.Rows[i].Cells[2].Value = values[1];
                                    dataGridView1.Rows[i].Cells[3].Value = values[2].Replace("%", "");
                               }
                            }

                            break;
                        case "Sheet":
                            if (!String.IsNullOrEmpty(conf.Value))
                            {
                                checkBox2.Checked = true;
                                var tmpList = conf.Value.Split('@');

                                for (int i = 0; i < tmpList.Length; i++)
                                {
                                    var tmp = tmpList[i].Split(':');
                                    var mod1 = tmp[0];
                                    var mod2 = tmp[1];
                                    var values = tmp[2].Split(',');

                                    dataGridView2.Rows.Add();

                                    dataGridView2.Rows[i].Cells[0].Value = mod1;
                                    dataGridView2.Rows[i].Cells[1].Value = mod2;
                                    dataGridView2.Rows[i].Cells[2].Value = values[0];
                                    dataGridView2.Rows[i].Cells[3].Value = values[1];
                                    dataGridView2.Rows[i].Cells[4].Value = values[2].Replace("%", "");
                                    if (tmp.Length > 3)
                                        dataGridView2.Rows[i].Cells[5].Value = tmp[3];
                                }
                            }

                            break;
                        case "Kaikei":
                            if (!String.IsNullOrEmpty(conf.Value))
                            {
                                checkBox3.Checked = true;
                                textBox1.Enabled = true;
                                textBox2.Enabled = true;
                                textBox3.Enabled = true;
                                var tmpList = conf.Value.Split('@');

                                textBox1.Text = tmpList[0];
                                textBox2.Text = tmpList[1];
                                textBox3.Text = tmpList[2];
                                cmbAddCost.SelectedValue = tmpList[3];

                                dataGridView7.Rows.Clear();

                                if (tmpList.Length > 4)
                                {
                                    for (int i = 4; i < tmpList.Length; i++)
                                    {
                                        var tmp = tmpList[i].Split(':');

                                        if (tmp.Length < 7)
                                            break;

                                        dataGridView7.Rows.Add();

                                        dataGridView7.Rows[i - 4].Cells[0].Value = Convert.ToInt32(tmp[0]);
                                        dataGridView7.Rows[i - 4].Cells[1].Value = tmp[1];
                                        dataGridView7.Rows[i - 4].Cells[2].Value = tmp[2];
                                        dataGridView7.Rows[i - 4].Cells[3].Value = tmp[3];
                                        dataGridView7.Rows[i - 4].Cells[4].Value = tmp[4];
                                        dataGridView7.Rows[i - 4].Cells[5].Value = tmp[5];
                                        dataGridView7.Rows[i - 4].Cells[6].Value = tmp[6];
                                        dataGridView7.Rows[i - 4].Cells[7].Value = tmp[7];
                                    }
                                }
                            }
                            break;
                        case "Memo":
                            textBox4.Text = conf.Value;
                            break;
                    }
                }

            }
            catch(Exception e)
            {

            }

        }

        private void SetSetting_Disp()
        {
            try
            {
                IF_Service service = new IF_Service();

                if(HospConfig.Count == 0)
                {
                    Config tmp = new Config();
                    tmp.Key = "Dist";
                    tmp.Remarks = "出力設定";
                    HospConfig.Add(tmp);
                    tmp = new Config();
                    tmp.Key = "Cost";
                    tmp.Remarks = "金額有無";
                    HospConfig.Add(tmp);
                    tmp = new Config();
                    tmp.Key = "Hosp";
                    tmp.Remarks = "仕訳病院";
                    HospConfig.Add(tmp);
                    tmp = new Config();
                    tmp.Key = "Sheet";
                    tmp.Remarks = "シート分け設定";
                    HospConfig.Add(tmp);
                    tmp = new Config();
                    tmp.Key = "Kaikei";
                    tmp.Remarks = "弥生会計連携設定";
                    HospConfig.Add(tmp);
                    tmp = new Config();
                    tmp.Key = "Memo";
                    tmp.Remarks = "メモ";
                    HospConfig.Add(tmp);
                }
                if(HospConfig.Count == 5)
                {
                    Config tmp = new Config();
                    tmp = new Config();
                    tmp.Key = "Memo";
                    tmp.Remarks = "メモ";
                    HospConfig.Add(tmp);
                }

                var dist = "";
                var cost = "";
                var hosp = "";
                var sheet = "";
                var kaikei = "";

                if (rdb_Export.Checked)
                    dist = "0";
                else if(radioButton2.Checked)
                {
                    dist = "1";

                    if (radioButton1.Checked)
                        cost = "1";
                    else
                        cost = "2";
                }
                else if (radioButton4.Checked)
                {
                    dist = "2";
                }

                if (checkBox1.Checked)
                {
                    for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                    {
                        if (i > 0)
                            hosp += "@";

                        hosp += dataGridView1.Rows[i].Cells[1].Value.ToString();
                        hosp += ":";
                        hosp += dataGridView1.Rows[i].Cells[0].Value.ToString();
                        hosp += ",";
                        hosp += dataGridView1.Rows[i].Cells[2].Value.ToString();
                        hosp += ",";
                        if (dataGridView1.Rows[i].Cells[2].Value.ToString() == "LIKE"
                            || dataGridView1.Rows[i].Cells[2].Value.ToString() == "NOT LIKE")
                        {
                            hosp += "%";
                            hosp += dataGridView1.Rows[i].Cells[3].Value.ToString();
                            hosp += "%";
                        }
                        else
                        {
                            hosp += dataGridView1.Rows[i].Cells[3].Value.ToString();
                        }
                    }
                }
                else
                {
                    hosp = "";
                }

                if (checkBox2.Checked)
                {
                    for (int i = 0; i < dataGridView2.Rows.Count - 1; i++)
                    {
                        if (i > 0)
                            sheet += "@";

                        sheet += dataGridView2.Rows[i].Cells[0].Value.ToString();
                        sheet += ":";
                        sheet += dataGridView2.Rows[i].Cells[1].Value.ToString();
                        sheet += ":";
                        sheet += dataGridView2.Rows[i].Cells[2].Value.ToString();
                        sheet += ",";
                        sheet += dataGridView2.Rows[i].Cells[3].Value.ToString();
                        sheet += ",";
                        if (dataGridView2.Rows[i].Cells[3].Value.ToString() == "LIKE"
                            || dataGridView2.Rows[i].Cells[3].Value.ToString() == "NOT LIKE")
                        {
                            sheet += "%";
                            sheet += dataGridView2.Rows[i].Cells[4].Value.ToString();
                            sheet += "%";
                        }
                        else
                        {
                            sheet += dataGridView2.Rows[i].Cells[4].Value.ToString();
                        }
                        sheet += ":";
                        if (dataGridView2.Rows[i].Cells[5].Value != null)
                            sheet += dataGridView2.Rows[i].Cells[5].Value.ToString();
                    }
                }
                else
                {
                    sheet = "";
                }
                if (checkBox3.Checked)
                {
                    kaikei += textBox1.Text;
                    kaikei += "@";
                    kaikei += textBox2.Text;
                    kaikei += "@";
                    kaikei += textBox3.Text;
                    kaikei += "@";
                    kaikei += cmbAddCost.SelectedValue;
                    kaikei += "@";

                    for (int i = 0; i < dataGridView7.Rows.Count - 1; i++)
                    {
                        if (i > 0)
                            kaikei += "@";

                        kaikei += dataGridView7.Rows[i].Cells[0].Value.ToString();
                        kaikei += ":";
                        kaikei += dataGridView7.Rows[i].Cells[1].Value.ToString();
                        kaikei += ":";
                        kaikei += dataGridView7.Rows[i].Cells[2].Value.ToString();
                        kaikei += ":";
                        kaikei += dataGridView7.Rows[i].Cells[3].Value.ToString();
                        kaikei += ":";
                        if (dataGridView7.Rows[i].Cells[4].Value != null)
                            kaikei += dataGridView7.Rows[i].Cells[4].Value.ToString();
                        kaikei += ":";
                        if (dataGridView7.Rows[i].Cells[5].Value != null)
                            kaikei += dataGridView7.Rows[i].Cells[5].Value.ToString();
                        kaikei += ":";
                        if (dataGridView7.Rows[i].Cells[6].Value != null)
                            kaikei += dataGridView7.Rows[i].Cells[6].Value.ToString();
                        kaikei += ":";
                        if (dataGridView7.Rows[i].Cells[7].Value != null)
                            kaikei += dataGridView7.Rows[i].Cells[7].Value.ToString();

                    }
                }
                else
                {
                    kaikei = "";
                }
                foreach (var conf in HospConfig)
                {
                    if (conf.Key == "Dist")
                        conf.Value = dist;
                    else if (conf.Key == "Cost")
                        conf.Value = cost;
                    else if (conf.Key == "Hosp")
                        conf.Value = hosp;
                    else if (conf.Key == "Sheet")
                        conf.Value = sheet;
                    else if (conf.Key == "Kaikei")
                        conf.Value = kaikei;
                    else if (conf.Key == "Memo")
                        conf.Value = textBox4.Text;

                }
                if (!service.SetHospitalConfig(HospCd, HospConfig.ToArray()))
                {
                    MessageBox.Show("出力設定の更新に失敗しました。", "更新失敗", MessageBoxButtons.OK);
                }


                LoadSetting_Hosp();
            }
            catch (Exception e)
            {

            }
        }

        private void LoadSetting()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Display", typeof(string));
            table.Columns.Add("Value", typeof(string));

            table.Rows.Add("", "");
            foreach (var mod in ModalityList)
            {
                table.Rows.Add(mod, mod);
            }
            
            Modality.DataSource = table;
            Modality.ValueMember = "Value";
            Modality.DisplayMember = "Display";
            SendModality.DataSource = table;
            SendModality.ValueMember = "Value";
            SendModality.DisplayMember = "Display";
            Search3.DataSource = table;
            Search3.ValueMember = "Value";
            Search3.DisplayMember = "Display";

            table = new DataTable();

            table.Columns.Add("Display", typeof(string));
            table.Columns.Add("Value", typeof(string));

            table.Rows.Add("", "");
            foreach (var mod in ModalityDocList)
            {
                table.Rows.Add(mod, mod);
            }

            Modality_Doc.DataSource = table;
            Modality_Doc.ValueMember = "Value";
            Modality_Doc.DisplayMember = "Display";

            table = new DataTable();

            table.Columns.Add("Display", typeof(string));
            table.Columns.Add("Value", typeof(string));

            table.Rows.Add("病院CD", "HCd");
            table.Rows.Add("患者ID", "PatID");
            table.Rows.Add("検査日", "StudyDate");
            table.Rows.Add("モダリティ", "Modality");
            table.Rows.Add("検査部位", "BodyPart");
            table.Rows.Add("画像枚数", "ImageCnt");
            table.Rows.Add("依頼科", "Department");
            table.Rows.Add("主治医", "PhysicianName");
            table.Rows.Add("受付専用", "Accept");

            Target.DataSource = table;
            Target.ValueMember = "Value";
            Target.DisplayMember = "Display";
            Type.DataSource = table;
            Type.ValueMember = "Value";
            Type.DisplayMember = "Display";

            table = new DataTable();

            table.Columns.Add("Display", typeof(string));
            table.Columns.Add("Value", typeof(string));

            table.Rows.Add("", "");
            table.Rows.Add("手入力", "0");
            table.Rows.Add("モダリティ", "1");
            table.Rows.Add("画像加算", "2");
            table.Rows.Add("緊急", "3");
            table.Rows.Add("FAX/ﾒｰﾙ", "4");
            table.Rows.Add("部位追加", "5");
            
            YType.DataSource = table;
            YType.ValueMember = "Value";
            YType.DisplayMember = "Display";

            table = new DataTable();

            table.Columns.Add("Display", typeof(string));
            table.Columns.Add("Value", typeof(string));

            table.Rows.Add("等しい", "=");
            table.Rows.Add("含む", "LIKE");
            table.Rows.Add("含まない", "NOT LIKE");
            table.Rows.Add("～以上", ">=");
            table.Rows.Add("～以下", "<=");

            Search.DataSource = table;
            Search.ValueMember = "Value";
            Search.DisplayMember = "Display";
            Search2.DataSource = table;
            Search2.ValueMember = "Value";
            Search2.DisplayMember = "Display";

            table = new DataTable();

            table.Columns.Add("Display", typeof(string));
            table.Columns.Add("Value", typeof(string));

            table.Rows.Add("", "");
            table.Rows.Add("外税/伝票", "1");
            table.Rows.Add("外税/請求", "2");
            table.Rows.Add("内税", "3");
            table.Rows.Add("輸出", "4");
            table.Rows.Add("内税/総額", "5");
            table.Rows.Add("内税/請求", "6");
            table.Rows.Add("内税/請求調整", "7");
            table.Rows.Add("外税/手入力", "9");

            AddCost.DataSource = table;
            AddCost.ValueMember = "Value";
            AddCost.DisplayMember = "Display";

            table = new DataTable();

            table.Columns.Add("Display", typeof(string));
            table.Columns.Add("Value", typeof(string));

            table.Rows.Add("3.0%", "10");
            table.Rows.Add("5.0%", "11");
            table.Rows.Add("8.0%", "12");
            table.Rows.Add("10.0%", "13");
            table.Rows.Add("(自)6.0%", "20");
            table.Rows.Add("(自)4.5%", "21");
            table.Rows.Add("(自)3.0%", "22");
            table.Rows.Add("免税(輸)", "70");
            table.Rows.Add("非課税", "80");
            table.Rows.Add("対象外", "90");

            cmbAddCost.DataSource = table;
            cmbAddCost.ValueMember = "Value";
            cmbAddCost.DisplayMember = "Display";

            table = new DataTable();

            table.Columns.Add("Display", typeof(string));
            table.Columns.Add("Value", typeof(string));

            table.Rows.Add("0001-ＣＴ", "0001");
            table.Rows.Add("0002-ＣＴ（部位追加）", "0002");
            table.Rows.Add("0003-ＭＲ", "0003");
            table.Rows.Add("0004-ＭＲ（部位追加）", "0004");
            table.Rows.Add("0005-ＣＲ", "0005");
            table.Rows.Add("0006-ＣＲ（頚椎等）", "0006");
            table.Rows.Add("0007-ＲＦ（胃透視）", "0007");
            table.Rows.Add("0008-ＲＦ（注腸）", "0008");
            table.Rows.Add("0009-マンモグラフィ", "0009");
            table.Rows.Add("0010-ＮＭ（ＲＩ）", "0010");
            table.Rows.Add("0011-ＮＭ（ＰＥＴ－ＣＴ）", "0011");
            table.Rows.Add("0012-脳ＰＥＴ", "0012");
            table.Rows.Add("0013-緊急読影加算", "0013");
            table.Rows.Add("0014-緊急使用加算", "0014");
            table.Rows.Add("0015-画像加算", "0015");
            table.Rows.Add("0016-メール・ＦＡＸ送信サービス", "0016");
            table.Rows.Add("0017-ＸＡ（アンギオ）", "0017");
            table.Rows.Add("0101-システム基本料", "0101");
            table.Rows.Add("0102-保守関係費用", "0102");
            table.Rows.Add("0103-回線費用", "0103");
            table.Rows.Add("0104-ＧＷレンタル費用", "0104");
            table.Rows.Add("0105-医用画像用システム利用料", "0105");
            table.Rows.Add("0108-アカウント設定費用（ＰｒｏＭｅｄＣＬ）", "0108");
            table.Rows.Add("0109-初期費用", "0109");
            table.Rows.Add("1000-その他スポット", "1000");
            table.Rows.Add("1001-ＪＯＩＮ取次手数料", "1001");

            YCd.DataSource = table;
            YCd.ValueMember = "Value";
            YCd.DisplayMember = "Display";



            table = new DataTable();
            table.Columns.Add("Display", typeof(string));
            table.Columns.Add("Value", typeof(string));

            DataTable table2 = new DataTable();
            table2.Columns.Add("Display", typeof(string));
            table2.Columns.Add("Value", typeof(string));
            foreach (var hosp in RetHospList)
            {
                table.Rows.Add(hosp.Name, hosp.Cd);

                if(hosp.IsCopy == "1")
                    table2.Rows.Add(hosp.Name, hosp.Cd);
            }

            cmb_Hosp.DataSource = table;
            cmb_Hosp.ValueMember = "Value";
            cmb_Hosp.DisplayMember = "Display";
            Hospital.DataSource = table2;
            Hospital.ValueMember = "Value";
            Hospital.DisplayMember = "Display";

            radioButton1.Enabled = false;
            radioButton3.Enabled = false;
            dataGridView1.Enabled = false;
            dataGridView2.Enabled = false;

        }

        private void button13_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView7.Rows.Count - 1; i++)
            {
                if (dataGridView7.Rows[i].Cells[5].Value != null
                    && dataGridView7.Rows[i].Cells[5].Value.ToString() == "5")
                {
                    MessageBox.Show("既に部位追加の設定が登録されてあります。", "警告", MessageBoxButtons.OK);
                    dataGridView7.Rows[i].Selected = true;
                    return;
                }
            }

            int no = dataGridView7.Rows.Count - 1;

            dataGridView7.Rows.Add();

            dataGridView7.Rows[no].Cells[0].Value = Convert.ToInt32(no + 1);
            dataGridView7.Rows[no].Cells[1].Value = "0001";
            dataGridView7.Rows[no].Cells[5].Value = "5";
            dataGridView7.Rows[no].Cells[6].Value = "CT";
            dataGridView7.Rows[no].Cells[7].Value = "";

            dataGridView7.Rows.Add();
            no++;

            dataGridView7.Rows[no].Cells[0].Value = Convert.ToInt32(no + 1);
            dataGridView7.Rows[no].Cells[1].Value = "0002";
            dataGridView7.Rows[no].Cells[5].Value = "5";
            dataGridView7.Rows[no].Cells[6].Value = "CT";
            dataGridView7.Rows[no].Cells[7].Value = "部位追加";

            dataGridView7.Rows.Add();
            no++;

            dataGridView7.Rows[no].Cells[0].Value = Convert.ToInt32(no + 1);
            dataGridView7.Rows[no].Cells[1].Value = "0003";
            dataGridView7.Rows[no].Cells[5].Value = "5";
            dataGridView7.Rows[no].Cells[6].Value = "MR";
            dataGridView7.Rows[no].Cells[7].Value = "";

            dataGridView7.Rows.Add();
            no++;

            dataGridView7.Rows[no].Cells[0].Value = Convert.ToInt32(no + 1);
            dataGridView7.Rows[no].Cells[1].Value = "0004";
            dataGridView7.Rows[no].Cells[5].Value = "5";
            dataGridView7.Rows[no].Cells[6].Value = "MR";
            dataGridView7.Rows[no].Cells[7].Value = "部位追加";


            MessageBox.Show("部位追加のレコードを作成いたしました。\nNo・コード・名称・単価・税転嫁のみ変更してください。", "完了", MessageBoxButtons.OK);

        }


        #endregion

        #region 病院マスタ

        private void button6_Click(object sender, EventArgs e)
        {
            if (!Check_Hosp())
                return;

            var ret = MessageBox.Show("入力した内容で更新しますが\nよろしいですか？", "更新", MessageBoxButtons.YesNo);

            if (ret == System.Windows.Forms.DialogResult.Yes)
            {
                SetSetting_Hosp();

                MessageBox.Show("施設情報を更新しました。", "完了", MessageBoxButtons.OK);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var ret = MessageBox.Show("入力を取り消しますが\nよろしいですか？", "キャンセル", MessageBoxButtons.YesNo);

            if (ret == System.Windows.Forms.DialogResult.Yes)
                LoadSetting_Hosp();
        }

        private void LoadSetting_Hosp()
        {
            try
            {
                IF_Service service = new IF_Service();

                Hospital[] hosp = service.GetHospital_Admin();
                RetHospList = new List<Agg_Serv.Class.Hospital>();
                RetHospList_Admin = new List<Agg_Serv.Class.Hospital>();
                dataGridView4.AutoGenerateColumns = false;
                //dataGridView4.Rows.Clear();

                DataTable table = new DataTable();
                table.Columns.Add("Col1", typeof(int));
                table.Columns.Add("Col2", typeof(int));
                table.Columns.Add("Col3", typeof(string));
                table.Columns.Add("Col4", typeof(string));
                table.Columns.Add("Col5", typeof(string));
                table.Columns.Add("Col6", typeof(bool));
                table.Columns.Add("Col7", typeof(bool));

                for (int i = 0; i < hosp.Length; i++ )
                {
                    table.Rows.Add(hosp[i].SortNo, Convert.ToInt32(hosp[i].Cd), hosp[i].Name, hosp[i].Name_DB, hosp[i].Name_Disp, Convert.ToBoolean(Convert.ToInt32(hosp[i].IsCopy)), Convert.ToBoolean(Convert.ToInt32(hosp[i].Status)));

                    //dataGridView4.Rows.Add();

                    //dataGridView4.Rows[i].Cells[0].Value = Convert.ToInt32(hosp[i].Cd);
                    //dataGridView4.Rows[i].Cells[1].Value = hosp[i].Name;
                    //dataGridView4.Rows[i].Cells[2].Value = hosp[i].Name_DB;
                    //dataGridView4.Rows[i].Cells[3].Value = hosp[i].Name_Disp;
                    //dataGridView4.Rows[i].Cells[4].Value = Convert.ToBoolean(Convert.ToInt32(hosp[i].IsCopy));
                }
                dataGridView4.DataSource = table;

                dataGridView4.Columns[0].DataPropertyName = "Col1";
                dataGridView4.Columns[1].DataPropertyName = "Col2";
                dataGridView4.Columns[2].DataPropertyName = "Col3";
                dataGridView4.Columns[3].DataPropertyName = "Col4";
                dataGridView4.Columns[4].DataPropertyName = "Col5";
                dataGridView4.Columns[5].DataPropertyName = "Col6";
                dataGridView4.Columns[6].DataPropertyName = "Col7";

                //dataGridView4.Sort(dataGridView4.Columns[0], ListSortDirection.Ascending);

                foreach(var tmp in hosp)
                {
                    if(tmp.Status == "0")
                        RetHospList.Add(tmp);

                    RetHospList_Admin.Add(tmp);
                }
            }
            catch (Exception e)
            {

            }
        }

        private bool Check_Hosp()
        {
            bool isRet = true;

            for (int i = 0; i < dataGridView4.Rows.Count - 1; i++)
            {
                if (dataGridView4.Rows[i].Cells[2].Value == null
                    || dataGridView4.Rows[i].Cells[3].Value == null
                    || dataGridView4.Rows[i].Cells[4].Value == null)
                {
                    MessageBox.Show("空白の項目があります。\n全て項目に値を入力してください。", "エラー", MessageBoxButtons.OK);

                    dataGridView4.Rows[i].Selected = true;
                    isRet = false;
                    break;
                }

                if (dataGridView4.Rows[i].Cells[5].Value == null)
                    dataGridView4.Rows[i].Cells[5].Value = false;
            }

            return isRet;
        }

        private void SetSetting_Hosp()
        {
            try
            {
                IF_Service service = new IF_Service();

                //foreach (var hosp in RetHospList)
                //{
                //    hosp.Status = "1";
                //}


                for (int i = 0; i < dataGridView4.Rows.Count - 1; i++)
                {
                    var isData = false;
                    foreach (var hosp in RetHospList_Admin)
                    {
                        if (dataGridView4.Rows[i].Cells[1].Value != null && dataGridView4.Rows[i].Cells[1].Value.ToString() == hosp.Cd)
                        {
                            isData = true;
                            hosp.SortNo = i + 1;
                            hosp.Name = dataGridView4.Rows[i].Cells[2].Value.ToString();
                            hosp.Name_DB = dataGridView4.Rows[i].Cells[3].Value.ToString();
                            hosp.Name_Disp = dataGridView4.Rows[i].Cells[4].Value.ToString();
                            hosp.IsCopy = Convert.ToInt32(dataGridView4.Rows[i].Cells[5].Value).ToString();
                            hosp.Status = Convert.ToInt32(dataGridView4.Rows[i].Cells[6].Value).ToString();
                            break;
                        }
                    }

                    if(!isData)
                    {
                        Hospital hosp = new Agg_Serv.Class.Hospital();

                        hosp.SortNo = i + 1;
                        hosp.Name = dataGridView4.Rows[i].Cells[2].Value.ToString();
                        hosp.Name_DB = dataGridView4.Rows[i].Cells[3].Value.ToString();
                        hosp.Name_Disp = dataGridView4.Rows[i].Cells[4].Value.ToString();
                        if (dataGridView4.Rows[i].Cells[5].Value != DBNull.Value)
                            hosp.IsCopy = Convert.ToInt32(dataGridView4.Rows[i].Cells[5].Value).ToString();
                        else
                            hosp.IsCopy = "0";
                        hosp.Status = "0";

                        RetHospList_Admin.Add(hosp);
                    }
                }

                foreach (var hosp in RetHospList_Admin)
                {
                    if (!service.SetHospital(hosp))
                    {
                        MessageBox.Show(hosp.Name + "の情報更新に失敗しました。", "更新失敗", MessageBoxButtons.OK);
                    }
                }


                LoadSetting_Hosp();
            }
            catch (Exception e)
            {

            }
        }

        private int _OwnBeginGrabRowIndex = -1;



        private void dataGridView4_MouseDown(object sender, MouseEventArgs e)
        {
            _OwnBeginGrabRowIndex = -1;

            if ((e.Button & MouseButtons.Left) != MouseButtons.Left) return;

            DataGridView.HitTestInfo hit = dataGridView4.HitTest(e.X, e.Y);
            if (hit.Type != DataGridViewHitTestType.Cell) return;

            // クリック時などは -1 に戻らないが問題なし
            _OwnBeginGrabRowIndex = hit.RowIndex;
        }



        private void dataGridView4_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != MouseButtons.Left) return;
            if (_OwnBeginGrabRowIndex == -1) return;

            // ドラッグ＆ドロップの開始
            dataGridView4.DoDragDrop(_OwnBeginGrabRowIndex, DragDropEffects.Move);
        }



        private bool _DropDestinationIsValid;
        private int _DropDestinationRowIndex;
        private bool _DropDestinationIsNextRow;



        private void dataGridView4_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;



            int from, to; bool next;
            bool valid = DecideDropDestinationRowIndex(
                dataGridView4, e, out from, out to, out next);



            // ドロップ先マーカーの表示・非表示の制御
            bool needRedraw = (valid != _DropDestinationIsValid);
            if (valid)
            {
                needRedraw = needRedraw
                    || (to != _DropDestinationRowIndex)
                    || (next != _DropDestinationIsNextRow);
            }

            if (needRedraw)
            {
                if (_DropDestinationIsValid)
                    dataGridView4.InvalidateRow(_DropDestinationRowIndex);
                if (valid)
                    dataGridView4.InvalidateRow(to);
            }

            _DropDestinationIsValid = valid;
            _DropDestinationRowIndex = to;
            _DropDestinationIsNextRow = next;
        }



        private void dataGridView4_DragLeave(object sender, EventArgs e)
        {
            if (_DropDestinationIsValid)
            {
                _DropDestinationIsValid = false;
                dataGridView4.InvalidateRow(_DropDestinationRowIndex);
            }
        }



        private void dataGridView4_DragDrop(object sender, DragEventArgs e)
        {
            int from, to; bool next;
            if (!DecideDropDestinationRowIndex(
                    dataGridView4, e, out from, out to, out next))
                return;



            _DropDestinationIsValid = false;



            // データの移動
            to = MoveDataValue(from, to, next);

            dataGridView4.CurrentCell =
                dataGridView4[dataGridView4.CurrentCell.ColumnIndex, to];

            dataGridView4.Invalidate();
        }



        private void dataGridView4_RowPostPaint(
            object sender, DataGridViewRowPostPaintEventArgs e)
        {
            // ドロップ先のマーカーを描画
            if (_DropDestinationIsValid
                && e.RowIndex == _DropDestinationRowIndex)
            {
                using (Pen pen = new Pen(Color.Red, 4))
                {
                    int y =
                        !_DropDestinationIsNextRow
                        ? e.RowBounds.Y + 2 : e.RowBounds.Bottom - 2;

                    e.Graphics.DrawLine(
                        pen, e.RowBounds.X, y, e.RowBounds.X + 50, y);
                }
            }
        }



        // ドロップ先の行の決定
        private bool DecideDropDestinationRowIndex(
            DataGridView grid, DragEventArgs e,
            out int from, out int to, out bool next)
        {
            from = (int)e.Data.GetData(typeof(int));
            // 元の行が追加用の行であれば、常に false
            if (grid.NewRowIndex != -1 && grid.NewRowIndex == from)
            {
                to = 0; next = false;
                return false;
            }



            Point clientPoint = grid.PointToClient(new Point(e.X, e.Y));
            // 上下のみに着目するため、横方向は無視する
            clientPoint.X = 1;
            DataGridView.HitTestInfo hit =
                grid.HitTest(clientPoint.X, clientPoint.Y);



            to = hit.RowIndex;
            if (to == -1)
            {
                int top = grid.ColumnHeadersVisible ? grid.ColumnHeadersHeight : 0;
                top += 1; // ...

                if (top > clientPoint.Y)
                    // ヘッダへのドロップ時は表示中の先頭行とする
                    to = grid.FirstDisplayedCell.RowIndex;
                else
                    // 最終行へ
                    to = grid.Rows.Count - 1;
            }

            // 追加用の行は無視
            if (to == grid.NewRowIndex) to--;



            next = (to > from);
            return (from != to);
        }



        // データの移動
        private int MoveDataValue(int from, int to, bool next)
        {
            DataTable table = (DataTable)dataGridView4.DataSource;

            // 移動するデータの退避（計算列があればたぶんダメ）
            object[] rowData = table.Rows[from].ItemArray;
            DataRow row = table.NewRow();
            row.ItemArray = rowData;

            // 移動元から削除
            table.Rows.RemoveAt(from);
            if (to > from) to--;

            // 移動先へ追加
            if (next) to++;
            if (to <= table.Rows.Count)
                table.Rows.InsertAt(row, to);
            else
                table.Rows.Add(row);

            return table.Rows.IndexOf(row);
        }

        #endregion

        #region 読影医マスタ

        private void button4_Click(object sender, EventArgs e)
        {
            if (!Check_Doctor())
                return;

            var ret = MessageBox.Show("入力した内容で更新しますが\nよろしいですか？", "更新", MessageBoxButtons.YesNo);

            if (ret == System.Windows.Forms.DialogResult.Yes)
            {
                SetSetting_Doctor();

                MessageBox.Show("読影医情報を更新しました。", "完了", MessageBoxButtons.OK);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            var ret = MessageBox.Show("入力を取り消しますが\nよろしいですか？", "キャンセル", MessageBoxButtons.YesNo);

            if (ret == System.Windows.Forms.DialogResult.Yes)
                LoadSetting_Doctor();

        }

        private void InitDoctor()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Display", typeof(string));
            table.Columns.Add("Value", typeof(int));
            table.Rows.Add("報酬", 0);
            table.Rows.Add("給与", 1);
            table.Rows.Add("対象外", 2);

            PayType.DataSource = table;
            PayType.ValueMember = "Value";
            PayType.DisplayMember = "Display";

            //PayType.DataSource = Enum.GetValues(typeof(PayType));
        }

        private void LoadSetting_Doctor()
        {
            try
            {
                IF_Service service = new IF_Service();

                Doctor[] doc = service.GetDoctor();

                dataGridView3.Rows.Clear();

                for (int i = 0; i < doc.Length; i++)
                {
                    DataGridViewRow row = dataGridView3.RowTemplate;

                    dataGridView3.Rows.Add();

                    dataGridView3.Rows[i].Cells[0].Value = Convert.ToInt32(doc[i].Cd);
                    dataGridView3.Rows[i].Cells[1].Value = doc[i].Name;
                    dataGridView3.Rows[i].Cells[2].Value = doc[i].Name_Disp;
                    dataGridView3.Rows[i].Cells[3].Value = Convert.ToBoolean(Convert.ToInt32(doc[i].IsVisible));
                    dataGridView3.Rows[i].Cells[4].Value = Convert.ToBoolean(Convert.ToInt32(doc[i].IsCost));
                    dataGridView3.Rows[i].Cells[5].Value = Convert.ToBoolean(Convert.ToInt32(doc[i].IsLisence));
                    dataGridView3.Rows[i].Cells[6].Value = Convert.ToInt32(doc[i].PayType);
                }

                dataGridView3.Sort(dataGridView3.Columns[0], ListSortDirection.Ascending);

                RetdocList = doc.ToList();
            }
            catch (Exception e)
            {

            }
        }

        private bool Check_Doctor()
        {
            bool isRet = true;

            for (int i = 0; i < dataGridView3.Rows.Count - 1; i++)
            {
                if (dataGridView3.Rows[i].Cells[1].Value == null
                    || dataGridView3.Rows[i].Cells[2].Value == null
                    || dataGridView3.Rows[i].Cells[6].Value == null)
                {
                    MessageBox.Show("空白の項目があります。\n全て項目に値を入力してください。", "エラー", MessageBoxButtons.OK);

                    dataGridView3.Rows[i].Selected = true;
                    isRet = false;
                    break;
                }

                if (dataGridView3.Rows[i].Cells[3].Value == null)
                    dataGridView3.Rows[i].Cells[3].Value = false;
                if (dataGridView3.Rows[i].Cells[4].Value == null)
                    dataGridView3.Rows[i].Cells[4].Value = false;
                if (dataGridView3.Rows[i].Cells[5].Value == null)
                    dataGridView3.Rows[i].Cells[5].Value = false;
            }

            return isRet;
        }

        private void SetSetting_Doctor()
        {
            try
            {
                IF_Service service = new IF_Service();

                foreach (var doc in RetdocList)
                {
                    doc.Status = "1";
                }


                for (int i = 0; i < dataGridView3.Rows.Count - 1; i++)
                {
                    var isData = false;
                    foreach (var doc in RetdocList)
                    {
                        if (dataGridView3.Rows[i].Cells[0].Value != null && dataGridView3.Rows[i].Cells[0].Value.ToString() == doc.Cd)
                        {
                            isData = true;
                            doc.Name = dataGridView3.Rows[i].Cells[1].Value.ToString();
                            doc.Name_Disp = dataGridView3.Rows[i].Cells[2].Value.ToString();
                            doc.IsVisible = Convert.ToInt32(dataGridView3.Rows[i].Cells[3].Value).ToString();
                            doc.IsCost = Convert.ToInt32(dataGridView3.Rows[i].Cells[4].Value).ToString();
                            doc.IsLisence = Convert.ToInt32(dataGridView3.Rows[i].Cells[5].Value).ToString();
                            doc.PayType = Convert.ToInt32(dataGridView3.Rows[i].Cells[6].Value).ToString();
                            doc.Status = "0";
                            break;
                        }
                    }

                    if (!isData)
                    {
                        Doctor doc = new Agg_Serv.Class.Doctor();

                        doc.Name = dataGridView3.Rows[i].Cells[1].Value.ToString();
                        doc.Name_Disp = dataGridView3.Rows[i].Cells[2].Value.ToString();
                        doc.IsVisible = Convert.ToInt32(dataGridView3.Rows[i].Cells[3].Value).ToString();
                        doc.IsCost = Convert.ToInt32(dataGridView3.Rows[i].Cells[4].Value).ToString();
                        doc.IsLisence = Convert.ToInt32(dataGridView3.Rows[i].Cells[5].Value).ToString();
                        doc.PayType = Convert.ToInt32(dataGridView3.Rows[i].Cells[6].Value).ToString();
                        doc.Status = "0";

                        RetdocList.Add(doc);
                    }
                }

                foreach (var doc in RetdocList)
                {
                    if (!service.SetDoctor(doc))
                    {
                        MessageBox.Show(doc.Name + "の情報更新に失敗しました。", "更新失敗", MessageBoxButtons.OK);
                    }
                }


                LoadSetting_Doctor();
            }
            catch (Exception e)
            {

            }
        }

        #endregion

        #region モダリティマスタ


        private void button8_Click(object sender, EventArgs e)
        {
            if (!Check_Mod())
                return;

            var ret = MessageBox.Show("入力した内容で更新しますが\nよろしいですか？", "更新", MessageBoxButtons.YesNo);

            if (ret == System.Windows.Forms.DialogResult.Yes)
            {
                SetSetting_Mod();

                MessageBox.Show("モダリティを更新しました。", "完了", MessageBoxButtons.OK);
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            var ret = MessageBox.Show("入力を取り消しますが\nよろしいですか？", "キャンセル", MessageBoxButtons.YesNo);

            if (ret == System.Windows.Forms.DialogResult.Yes)
                LoadSetting_Mod();

        }

        private void LoadSetting_Mod()
        {
            try
            {
                dataGridView5.Rows.Clear();
                ModalityList = new List<string>();

                string[] modList = null;

                foreach(var conf in SystemConfig)
                {
                    if(conf.Key == "Modality")
                    {
                        modList = conf.Value.Split(',');
                        break;
                    }
                }

                for (int i = 0; i < modList.Length; i++)
                {
                    string[] modVal = modList[i].Split(':');

                    dataGridView5.Rows.Add();

                    dataGridView5.Rows[i].Cells[0].Value = Convert.ToInt32(modVal[0]);
                    dataGridView5.Rows[i].Cells[1].Value = modVal[1];
                    dataGridView5.Rows[i].Cells[2].Value = modVal[2];

                    ModalityList.Add(modVal[1]);
                }
            }
            catch (Exception e)
            {

            }
        }

        private bool Check_Mod()
        {
            bool isRet = true;

            for (int i = 0; i < dataGridView5.Rows.Count - 1; i++)
            {
                if (dataGridView5.Rows[i].Cells[0].Value == null
                    || dataGridView5.Rows[i].Cells[1].Value == null
                    || dataGridView5.Rows[i].Cells[2].Value == null)
                {
                    MessageBox.Show("空白の項目があります。\n全て項目に値を入力してください。", "エラー", MessageBoxButtons.OK);

                    dataGridView5.Rows[i].Selected = true;
                    isRet = false;
                    break;
                }
            }

            return isRet;
        }

        private void SetSetting_Mod()
        {
            try
            {
                IF_Service service = new IF_Service();

                var modVal = "";

//                dataGridView5.Sort(dataGridView5.Columns[0], ListSortDirection.Ascending);

                List<Modality> modList = new List<Modality>();

                for (int i = 0; i < dataGridView5.Rows.Count - 1; i++)
                {
                    Modality mod = new AggregateTool.Modality();

                    mod.No = Convert.ToInt32(dataGridView5.Rows[i].Cells[0].Value);
                    mod.Cd = dataGridView5.Rows[i].Cells[1].Value.ToString();
                    mod.Disp = dataGridView5.Rows[i].Cells[2].Value.ToString();

                    modList.Add(mod);
                }

                modList.Sort((a, b) => a.No - b.No);

                for (int i = 0; i < modList.Count; i++)
                {
                    if (i > 0)
                        modVal += ",";

                    modVal += modList[i].No;
                    modVal += ":";
                    modVal += modList[i].Cd;
                    modVal += ":";
                    modVal += modList[i].Disp;
                }

                foreach (var conf in SystemConfig)
                {
                    if (conf.Key == "Modality")
                    {
                        conf.Value = modVal;
                        break;
                    }
                }

                if (!service.SetSystemConfig(SystemConfig.ToArray()))
                {
                    MessageBox.Show("モダリティの情報更新に失敗しました。", "更新失敗", MessageBoxButtons.OK);
                }

                GetSystemSetting();
                LoadSetting_Mod();
            }
            catch (Exception e)
            {

            }
        }

        #endregion

        #region 読影医モダリティマスタ
        private void button12_Click(object sender, EventArgs e)
        {
            if (!Check_Mod_Doc())
                return;

            var ret = MessageBox.Show("入力した内容で更新しますが\nよろしいですか？", "更新", MessageBoxButtons.YesNo);

            if (ret == System.Windows.Forms.DialogResult.Yes)
            {
                SetSetting_Mod_Doc();

                MessageBox.Show("モダリティを更新しました。", "完了", MessageBoxButtons.OK);
            }
        }


        private void button11_Click(object sender, EventArgs e)
        {
            var ret = MessageBox.Show("入力を取り消しますが\nよろしいですか？", "キャンセル", MessageBoxButtons.YesNo);

            if (ret == System.Windows.Forms.DialogResult.Yes)
                LoadSetting_Mod_Doc();

        }


        private void LoadSetting_Mod_Doc()
        {
            try
            {
                dataGridView8.Rows.Clear();
                ModalityDocList = new List<string>();
                string[] modList = null;

                foreach (var conf in SystemConfig)
                {
                    if (conf.Key == "Modality_Doc")
                    {
                        modList = conf.Value.Split(',');
                        break;
                    }
                }

                for (int i = 0; i < modList.Length; i++)
                {
                    string[] modVal = modList[i].Split(':');

                    dataGridView8.Rows.Add();

                    dataGridView8.Rows[i].Cells[0].Value = Convert.ToInt32(modVal[0]);
                    dataGridView8.Rows[i].Cells[1].Value = modVal[1];
                    dataGridView8.Rows[i].Cells[2].Value = modVal[2];

                    ModalityDocList.Add(modVal[1]);
                }
            }
            catch (Exception e)
            {

            }
        }

        private bool Check_Mod_Doc()
        {
            bool isRet = true;

            for (int i = 0; i < dataGridView8.Rows.Count - 1; i++)
            {
                if (dataGridView8.Rows[i].Cells[0].Value == null
                    || dataGridView8.Rows[i].Cells[1].Value == null
                    || dataGridView8.Rows[i].Cells[2].Value == null)
                {
                    MessageBox.Show("空白の項目があります。\n全て項目に値を入力してください。", "エラー", MessageBoxButtons.OK);

                    dataGridView8.Rows[i].Selected = true;
                    isRet = false;
                    break;
                }
            }

            return isRet;
        }

        private void SetSetting_Mod_Doc()
        {
            try
            {
                IF_Service service = new IF_Service();

                var modVal = "";

                //                dataGridView5.Sort(dataGridView5.Columns[0], ListSortDirection.Ascending);

                List<Modality> modList = new List<Modality>();

                for (int i = 0; i < dataGridView8.Rows.Count - 1; i++)
                {
                    Modality mod = new AggregateTool.Modality();

                    mod.No = Convert.ToInt32(dataGridView8.Rows[i].Cells[0].Value);
                    mod.Cd = dataGridView8.Rows[i].Cells[1].Value.ToString();
                    mod.Disp = dataGridView8.Rows[i].Cells[2].Value.ToString();

                    modList.Add(mod);
                }

                modList.Sort((a, b) => a.No - b.No);

                for (int i = 0; i < modList.Count; i++)
                {
                    if (i > 0)
                        modVal += ",";

                    modVal += modList[i].No;
                    modVal += ":";
                    modVal += modList[i].Cd;
                    modVal += ":";
                    modVal += modList[i].Disp;
                }

                foreach (var conf in SystemConfig)
                {
                    if (conf.Key == "Modality_Doc")
                    {
                        conf.Value = modVal;
                        break;
                    }
                }

                if (!service.SetSystemConfig(SystemConfig.ToArray()))
                {
                    MessageBox.Show("モダリティの情報更新に失敗しました。", "更新失敗", MessageBoxButtons.OK);
                }

                GetSystemSetting();
                LoadSetting_Mod_Doc();
            }
            catch (Exception e)
            {

            }
        }

        #endregion

        #region ユーザマスタ


        private void button10_Click(object sender, EventArgs e)
        {
            if (!Check_User())
                return;

            var ret = MessageBox.Show("入力した内容で更新しますが\nよろしいですか？", "更新", MessageBoxButtons.YesNo);

            if (ret == System.Windows.Forms.DialogResult.Yes)
            {
                SetSetting_User();

                MessageBox.Show("ユーザ情報を更新しました。", "完了", MessageBoxButtons.OK);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            var ret = MessageBox.Show("入力を取り消しますが\nよろしいですか？", "キャンセル", MessageBoxButtons.YesNo);

            if (ret == System.Windows.Forms.DialogResult.Yes)
                LoadSetting_User();
        }

        private void LoadSetting_User()
        {
            try
            {
                dataGridView6.Rows.Clear();

                IF_Service service = new IF_Service();

                var userList = service.GetUser();

                for (int i = 0; i < userList.Length; i++)
                {
                    dataGridView6.Rows.Add();

                    dataGridView6.Rows[i].Cells[0].Value = Convert.ToInt32(userList[i].Cd);
                    dataGridView6.Rows[i].Cells[1].Value = userList[i].Name;
                    dataGridView6.Rows[i].Cells[2].Value = userList[i].LoginID;
                    dataGridView6.Rows[i].Cells[3].Value = userList[i].LoginPW;
                    dataGridView6.Rows[i].Cells[4].Value = Convert.ToBoolean(Convert.ToInt32(userList[i].IsAdmin));
                }

                RetUserList = userList.ToList();
                dataGridView6.Sort(dataGridView6.Columns[0], ListSortDirection.Ascending);
            }
            catch (Exception e)
            {

            }
        }

        private bool Check_User()
        {
            bool isRet = true;

            for (int i = 0; i < dataGridView6.Rows.Count - 1; i++)
            {
                if (dataGridView6.Rows[i].Cells[1].Value == null
                    || dataGridView6.Rows[i].Cells[2].Value == null
                    || dataGridView6.Rows[i].Cells[3].Value == null)
                {
                    MessageBox.Show("空白の項目があります。\n全て項目に値を入力してください。", "エラー", MessageBoxButtons.OK);

                    dataGridView6.Rows[i].Selected = true;
                    isRet = false;
                    break;
                }

                if (dataGridView6.Rows[i].Cells[4].Value == null)
                    dataGridView6.Rows[i].Cells[4].Value = false;

            }

            return isRet;
        }

        private void SetSetting_User()
        {
            try
            {
                IF_Service service = new IF_Service();
                foreach (var user in RetUserList)
                {
                    user.Status = "1";
                }

                for (int i = 0; i < dataGridView6.Rows.Count - 1; i++)
                {
                    var isData = false;
                    foreach (var user in RetUserList)
                    {
                        if (dataGridView6.Rows[i].Cells[0].Value != null && dataGridView6.Rows[i].Cells[0].Value.ToString() == user.Cd)
                        {
                            isData = true;
                            user.Cd = dataGridView6.Rows[i].Cells[0].Value.ToString();
                            user.Name = dataGridView6.Rows[i].Cells[1].Value.ToString();
                            user.LoginID = dataGridView6.Rows[i].Cells[2].Value.ToString();
                            user.LoginPW = dataGridView6.Rows[i].Cells[3].Value.ToString();
                            user.IsAdmin = Convert.ToInt32(dataGridView6.Rows[i].Cells[4].Value).ToString();
                            user.Status = "0";
                            break;
                        }
                    }

                    if (!isData)
                    {
                        User user = new Agg_Serv.Class.User();

                        user.Name = dataGridView6.Rows[i].Cells[1].Value.ToString();
                        user.LoginID = dataGridView6.Rows[i].Cells[2].Value.ToString();
                        user.LoginPW = dataGridView6.Rows[i].Cells[3].Value.ToString();
                        user.IsAdmin = Convert.ToInt32(dataGridView6.Rows[i].Cells[4].Value).ToString();
                        user.Status = "0";

                        RetUserList.Add(user);
                    }
                }

                foreach (var user in RetUserList)
                {
                    if (!service.SetUser(user))
                    {
                        MessageBox.Show(user.Name + "の情報更新に失敗しました。", "更新失敗", MessageBoxButtons.OK);
                    }
                }

                LoadSetting_User();
            }
            catch (Exception e)
            {

            }
        }

        #endregion


        private void Form_Setting_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ShowResultDialog("一覧画面を閉じてもよろしいですか？"))
                e.Cancel = true;
            else
            {
                //メニューを開く
                form_menu.Visible = true;
                //フォーカスをなくす
                form_menu.ActiveControl = null;

                form_menu.LoadMaster();
            }
        }

        /// <summary>
        /// 確認ダイアログ表示メソッド
        /// </summary>
        public bool ShowResultDialog(string msg)
        {
            bool decision = false;
            DialogResult result = MessageBox.Show(msg, "注意", MessageBoxButtons.YesNo,
            MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

            //何が選択されたか調べる
            if (result == DialogResult.Yes)
            {
                decision = true;
            }
            return decision;
        }

        private void dataGridView6_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            ////指定されたセルの値を文字列として取得する
            //string str1 = (e.CellValue1 == null ? "" : e.CellValue1.ToString());
            //string str2 = (e.CellValue2 == null ? "" : e.CellValue2.ToString());

            ////結果を代入
            //e.SortResult = str1.Length - str2.Length;
            ////処理したことを知らせる
            //e.Handled = true;
        }

        private void dataGridView7_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            if (YCd.DataSource == null)
                return;
            if(e.ColumnIndex == 1)
            {
                var code = dataGridView7.Rows[e.RowIndex].Cells[1].Value;
                var val = "";
                var data = (DataTable)YCd.DataSource;

                for(int i = 0; i < data.Rows.Count; i++)
                {
                    if(data.Rows[i]["Value"] == code)
                    {
                        var str = (string)data.Rows[i]["Display"];
                        val = str.Split('-')[1];
                        break;
                    }
                }

                dataGridView7.Rows[e.RowIndex].Cells[2].Value = val;
            }
        }





    }

    public class Modality
    {
        public int No;
        public string Cd;
        public string Disp;
    }
}
