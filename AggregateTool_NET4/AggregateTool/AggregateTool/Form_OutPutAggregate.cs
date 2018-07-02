using Agg_Serv;
using Agg_Serv.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AggregateTool
{
    public partial class Form_OutPutAggregate : Form
    {
        //設定クラス
        private Class_Setting setting = new Class_Setting();
        //メニュークラス
        public Form_Menu form_menu = new Form_Menu();

        private static List<Config> SystemConfig = new List<Config>();
        private static List<Hospital> RetHospList = new List<Hospital>();
        private static List<Doctor> RetdocList = new List<Doctor>();
        private static List<Config> HospConfig = new List<Config>();
        private static Dictionary<string, string> ModalityList = new Dictionary<string, string>();
        private static Dictionary<string, string> ModalityDocList = new Dictionary<string, string>();
        private static List<HospDistMst> HospDistMsts = new List<HospDistMst>();
        private static List<ExtReports> HospReports = new List<ExtReports>();

        public List<string> YayoiHosps = new List<string>();

        private static string start;
        private static string end;

        public Form_OutPutAggregate()
        {
            InitializeComponent();

            IF_Service service = new IF_Service();

            RetHospList = service.GetHospital().ToList();
            RetdocList = service.GetDoctor().ToList();
            SystemConfig = service.GetSystemConfig().ToList();

            ModalityList = new Dictionary<string, string>();
            ModalityDocList = new Dictionary<string, string>();
            HospDistMsts = new List<HospDistMst>();

            string[] modList = null;
            string[] modDocList = null;

            foreach (var conf in SystemConfig)
            {
                if (conf.Key == "Modality")
                {
                    modList = conf.Value.Split(',');
                }
                else if (conf.Key == "Modality_Doc")
                {
                    modDocList = conf.Value.Split(',');
                }
            }
            ModalityList = new Dictionary<string, string>();
            for (int i = 0; i < modList.Length; i++)
            {
                string[] modVal = modList[i].Split(':');

                ModalityList.Add(modVal[1], modVal[2]);
            }

            ModalityDocList = new Dictionary<string,string>();
            for (int i = 0; i < modDocList.Length; i++)
            {
                string[] modVal = modDocList[i].Split(':');

                ModalityDocList.Add(modVal[1], modVal[2]);
            }

            foreach (var hosp in RetHospList)
            {
                var dat = service.GetHospitalConfig(hosp.Cd);
                string[] confs = null;
                HospDistMst tmpData = new HospDistMst();
                tmpData.Hospital = hosp;

                for (int i = 0; i < dat.Length; i++)
                {
                    confs = null;

                    if (!String.IsNullOrEmpty(dat[i].Value))
                    {
                        switch (dat[i].Key)
                        {
                            case "Cost":
                                tmpData.Cost = dat[i].Value;
                                break;
                            case "Dist":
                                tmpData.Dist = dat[i].Value;
                                break;
                            default:
                                confs = dat[i].Value.Split('@');
                                break;
                        }
                    }

                    if (confs == null)
                        continue;

                    switch(dat[i].Key)
                    {
                        case "Hosp":
                            List<HospMst> list = new List<HospMst>();
                            foreach (var conf in confs)
                            {
                                var datas = conf.Split(':');
                                HospMst mst = new AggregateTool.HospMst();
                                mst.Target = datas[0];
                                var wheres = datas[1].Split(',');
                                mst.Where = wheres[0] + " " + wheres[1] + " '" + wheres[2] + "'";

                                list.Add(mst);
                            }
                            tmpData.Hosp = list.ToArray();
                            break;
                        case "Sheet":
                            List<ModarityMst> listMod = new List<ModarityMst>();
                            foreach (var conf in confs)
                            {
                                var datas = conf.Split(':');
                                ModarityMst mst = new ModarityMst();
                                mst.Sum = datas[0];
                                mst.Target = datas[1];
                                var wheres = datas[2].Split(',');
                                mst.Where = wheres[0] + " " + wheres[1] + " '" + wheres[2] + "'";
                                if (datas.Length > 3)
                                    mst.DocMod = datas[3];
                                listMod.Add(mst);
                            }
                            tmpData.Sheet = listMod.ToArray();
                            break;
                        case "Kaikei":
                            tmpData.Yayoi = confs;
                            break;
                        default:
                            continue;
                    }
                }

                HospDistMsts.Add(tmpData);
            }
        }

        //ここから================================================イベント=========================================================
        
        private void Btn_OutPutAggregate_Click(object sender, EventArgs e)
        {
            var ret = MessageBox.Show("集計には時間がかかりますが、よろしいですか？", "警告", MessageBoxButtons.OKCancel);
            if (ret == System.Windows.Forms.DialogResult.Cancel)
                return;

            start = this.Dtp_AggregateMonth.Value.ToString("yyyyMM") + "01";
            end = this.Dtp_AggregateMonth.Value.ToString("yyyyMM") + DateTime.DaysInMonth(this.Dtp_AggregateMonth.Value.Year, this.Dtp_AggregateMonth.Value.Month).ToString().PadLeft(2, '0');

            HospReports = new List<ExtReports>();

            foreach(var hosp in HospDistMsts)
            {
                if (hosp.Hospital.IsCopy == "0")
                    GetTargetReport(hosp);
                else
                {
                    hosp.ParentCd = GetTargetReport_Child(hosp);

                }
            }

            Class_ExcelWork ew = new Class_ExcelWork();
            ew.HospReports = HospReports;
            ew.HospDistMsts = HospDistMsts;
            ew.RetdocList = RetdocList;
            ew.ModalityList = ModalityList;
            ew.ModalityDocList = ModalityDocList;
            ew.WriteExcel_Sheet_1(this.Dtp_AggregateMonth.Value, 0);
            ew.WriteExcel_Sheet_2(this.Dtp_AggregateMonth.Value, 0);
            ew.WriteExcel_Sheet_3(this.Dtp_AggregateMonth.Value, 0);
            ew.WriteExcel_Sheet_4(this.Dtp_AggregateMonth.Value, 0);
            ew.WriteExcel_Sheet_6(this.Dtp_AggregateMonth.Value, 0);

            MessageBox.Show("集計出力が完了いたしました。");
        }

        /// <summary>
        ///   レポートデータ取得（親施設）
        /// </summary>
        /// <param name="Hosp"></param>
        private void GetTargetReport(HospDistMst Hosp)
        {
            IF_Service service = new IF_Service();

            ExtReports hospRep = new ExtReports();
            hospRep.Cd = Hosp.Hospital.Cd;

            List<ModReports> modLists = new List<ModReports>();

            foreach (var mod in ModalityList)
            {
                ModReports tmpReps = new ModReports();
                tmpReps.Mod = mod.Value;


                string modWhere = "";
                modWhere += " Modality = '" + mod.Key + "'";

                if (Hosp.Sheet != null)
                {
                    var isData = false;
                    foreach (var sheet in Hosp.Sheet)
                    {
                        if (sheet.Sum == mod.Key)
                        {
                            modWhere += " OR ( Modality = '" + sheet.Target + "'";
                            modWhere += " AND " + sheet.Where + ")";
                            tmpReps.DocMod = sheet.DocMod;
                                    
                            isData = true;
                        }
                    }
                    if (isData)
                        modWhere = " AND (" + modWhere + ")";
                    else
                        modWhere = " AND " + modWhere;

                    foreach (var sheet in Hosp.Sheet)
                    {
                        if (sheet.Target == mod.Key)
                        {
                            modWhere += " AND NOT " + sheet.Where;
                        }
                    }
                }
                else
                {
                    modWhere = " AND " + modWhere;
                }

                if (Hosp.Hosp != null)
                    foreach (var mst in Hosp.Hosp)
                    {
                        modWhere += " AND NOT " + mst.Where;
                    }

                tmpReps.Reports = service.GetReport(Hosp.Hospital.Name_DB, start, end, modWhere);

                if (tmpReps.Reports != null)
                {
                    tmpReps.Count = tmpReps.Reports.Length;
                    hospRep.Count += tmpReps.Reports.Length;
                    modLists.Add(tmpReps);
                }
            }

            hospRep.ModReports = modLists.ToArray();

            HospReports.Add(hospRep);
        }

        private string GetTargetReport_Child(HospDistMst Hosp)
        {
            IF_Service service = new IF_Service();

            ExtReports hospRep = new ExtReports();
            hospRep.Cd = Hosp.Hospital.Cd;

            List<ModReports> modLists = new List<ModReports>();

            var dispCd = "";
            var childWhere = "";

            foreach (var mst in HospDistMsts)
            {
                if (mst.Hosp == null)
                {
                    continue;
                }

                foreach (var hos in mst.Hosp)
                {
                    if (hos.Target == Hosp.Hospital.Cd)
                    {
                        dispCd = mst.Hospital.Name_DB;
                        childWhere += " AND " + hos.Where;
                    }
                }
            }

            foreach (var mod in ModalityList)
            {
                ModReports tmpReps = new ModReports();
                tmpReps.Mod = mod.Value;

                string modWhere = "";
                modWhere += " Modality = '" + mod.Key + "'";

                if (Hosp.Sheet != null)
                {
                    var isData = false;
                    foreach (var sheet in Hosp.Sheet)
                    {
                        if (sheet.Sum == mod.Key)
                        {
                            modWhere += " OR ( Modality = '" + sheet.Target + "'";
                            modWhere += " AND " + sheet.Where + ")";
                            isData = true;
                        }
                    }
                    if (isData)
                        modWhere = " AND (" + modWhere + ")";
                    else
                        modWhere = " AND " + modWhere;

                    foreach (var sheet in Hosp.Sheet)
                    {
                        if (sheet.Target == mod.Key)
                        {
                            modWhere += " AND NOT " + sheet.Where;
                        }
                    }
                }
                else
                {
                    modWhere = " AND " + modWhere;
                }

                if (Hosp.Hosp != null)
                    foreach (var mst in Hosp.Hosp)
                    {
                        modWhere += " AND NOT " + mst.Where;
                    }

                tmpReps.Reports = service.GetReport(dispCd, start, end, childWhere + modWhere);

                if (tmpReps.Reports != null)
                {
                    tmpReps.Count = tmpReps.Reports.Length;
                    hospRep.Count += tmpReps.Reports.Length;
                    modLists.Add(tmpReps);
                }
            }

            hospRep.ModReports = modLists.ToArray();

            HospReports.Add(hospRep);

            return dispCd;
        }

        
        private void Btn_Accounting_Click(object sender, EventArgs e)
        {
            var ret = MessageBox.Show("集計には時間がかかりますが、よろしいですか？", "会計連携", MessageBoxButtons.OKCancel);
            if (ret == System.Windows.Forms.DialogResult.Cancel)
                return;

            start = this.Dtp_AggregateMonth.Value.ToString("yyyyMM") + "01";
            end = this.Dtp_AggregateMonth.Value.ToString("yyyyMM") + DateTime.DaysInMonth(this.Dtp_AggregateMonth.Value.Year, this.Dtp_AggregateMonth.Value.Month).ToString().PadLeft(2, '0');

            Form_EditHosp edit = new Form_EditHosp();
            edit.form_menu = this;
            YayoiHosps = new List<string>();
            edit.ShowDialog();

            if(YayoiHosps.Count == 0)
            {
                MessageBox.Show("出力施設が選択されていません。");
                return;
            }

            HospReports = new List<ExtReports>();

            foreach (var hosp in HospDistMsts)
            {
                if (!YayoiHosps.Contains(hosp.Hospital.Name_DB))
                    continue;

                if (hosp.Hospital.IsCopy == "0")
                    GetTargetReport(hosp);
                else
                {
                    hosp.ParentCd = GetTargetReport_Child(hosp);

                }
            }


            Class_ExcelWork ew = new Class_ExcelWork();
            ew.HospReports = HospReports;
            ew.HospDistMsts = HospDistMsts;
            ew.RetdocList = RetdocList;
            ew.ModalityList = ModalityList;
            ew.ModalityDocList = ModalityDocList;
            ew.YayoiHosps = YayoiHosps;

            string msg_1;
            string msg_2;

            if (!CheckMod(out msg_1, out msg_2))
            {
                if(!string.IsNullOrEmpty(msg_1))
                {
                    ret = MessageBox.Show("設定されていないモダリティが集計されています。\n処理を継続してもよろしいですか？\n\n" + msg_1, "会計連携", MessageBoxButtons.OKCancel);
                    if (ret == System.Windows.Forms.DialogResult.Cancel)
                        return;
                }
                //if (!string.IsNullOrEmpty(msg_2))
                //{
                //    ret = MessageBox.Show("設定されているモダリティの中に、件数が0件のものが存在します。\n処理を継続してもよろしいですか？\n\n" + msg_2, "会計連携", MessageBoxButtons.OKCancel);
                //    if (ret == System.Windows.Forms.DialogResult.Cancel)
                //        return;
                //}
            }

            ew.WriteExcel_Sheet_5(this.Dtp_AggregateMonth.Value, 0);

            MessageBox.Show("出力が完了いたしました。");
        }

        private bool CheckMod(out string msg_1, out string msg_2)
        {
            bool ret = true;

            msg_1 = "";
            msg_2 = "";


            foreach (var hosp in HospDistMsts)
            {
                List<string> cntMods = new List<string>();
                List<string> setMods = new List<string>();

                if (hosp.Yayoi == null || hosp.Yayoi.Length < 5)
                    continue;


                ExtReports val = new ExtReports();
                val.ModReports = new List<ModReports>().ToArray();

                foreach (var tmp in HospReports)
                {
                    if (tmp.Cd == hosp.Hospital.Cd)
                    {
                        val = tmp;
                        break;
                    }
                }


                string hoscd = hosp.Yayoi[0];
                string hosname = hosp.Yayoi[1];
                string hosname_S = hosp.Yayoi[2];
                string cost = hosp.Yayoi[3];

                List<KaikeiMst> YList = new List<KaikeiMst>();

                for (int i = 4; i < hosp.Yayoi.Length; i++)
                {
                    KaikeiMst tmp = new KaikeiMst();

                    List<string> vals = hosp.Yayoi[i].Split(':').ToList();

                    if (String.IsNullOrEmpty(vals[0]))
                        continue;

                    tmp.No = Convert.ToInt32(vals[0]);

                    vals.RemoveAt(0);

                    tmp.Values = vals.ToArray();

                    if (tmp.Values.Length < 5)
                        continue;
                    if (tmp.Values[4] != "1" && tmp.Values[4] != "5")
                        continue;

                    setMods.Add(tmp.Values[5]);
                }

                for (var i = 0; i < val.ModReports.Length; i++)
                {
                    if (val.ModReports[i].Count > 0)
                    {
                        cntMods.Add(val.ModReports[i].Mod);
                    }
                }

                string tmpstr = "";

                if(setMods.Count > cntMods.Count)
                {
                    foreach(var mod in setMods)
                    {
                        if(!cntMods.Contains(mod))
                        {
                            if (!string.IsNullOrEmpty(tmpstr))
                                tmpstr += ",";
                            tmpstr += mod;
                        }
                    }

                    msg_2 += "【" + hosp.Hospital.Name_DB + "】  " + tmpstr + "\n";

                    ret = false;
                }
                else if(cntMods.Count > setMods.Count)
                {
                    foreach (var mod in cntMods)
                    {
                        if (!setMods.Contains(mod))
                        {
                            if (!string.IsNullOrEmpty(tmpstr))
                                tmpstr += ",";
                            tmpstr += mod;
                        }
                    }

                    msg_1 += "【" + hosp.Hospital.Name_DB + "】  " + tmpstr + "\n";

                    ret = false;
                }
            }

            return ret;
        }

        private void Btn_ReturnMenu_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form_OutPutAggregate_FormClosing(object sender, FormClosingEventArgs e)
        {
            form_menu.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var ret = MessageBox.Show("集計には時間がかかりますが、よろしいですか？", "月次データ", MessageBoxButtons.OKCancel);
            if (ret == System.Windows.Forms.DialogResult.Cancel)
                return;

            start = this.Dtp_AggregateMonth.Value.ToString("yyyyMM") + "01";
            end = this.Dtp_AggregateMonth.Value.ToString("yyyyMM") + DateTime.DaysInMonth(this.Dtp_AggregateMonth.Value.Year, this.Dtp_AggregateMonth.Value.Month).ToString().PadLeft(2, '0');

            HospReports = new List<ExtReports>();

            foreach (var hosp in HospDistMsts)
            {
                if(hosp.Hospital.Name_DB =="KHM"){
                    int xx =0;
                    xx++;
                }


                if (hosp.Hospital.IsCopy == "0")
                    GetTargetReport(hosp);
                else
                {
                    hosp.ParentCd = GetTargetReport_Child(hosp);

                }
            }

            Class_ExcelWork ew = new Class_ExcelWork();
            ew.HospReports = HospReports;
            ew.HospDistMsts = HospDistMsts;
            ew.RetdocList = RetdocList;
            ew.ModalityList = ModalityList;
            ew.ModalityDocList = ModalityDocList; 
            ew.WriteExcel_Sheet_1(this.Dtp_AggregateMonth.Value, 0);

            MessageBox.Show("集計出力が完了いたしました。");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var ret = MessageBox.Show("集計には時間がかかりますが、よろしいですか？", "依頼推移表", MessageBoxButtons.OKCancel);
            if (ret == System.Windows.Forms.DialogResult.Cancel)
                return;

            start = this.Dtp_AggregateMonth.Value.ToString("yyyyMM") + "01";
            end = this.Dtp_AggregateMonth.Value.ToString("yyyyMM") + DateTime.DaysInMonth(this.Dtp_AggregateMonth.Value.Year, this.Dtp_AggregateMonth.Value.Month).ToString().PadLeft(2, '0');

            HospReports = new List<ExtReports>();

            foreach (var hosp in HospDistMsts)
            {
                if (hosp.Hospital.IsCopy == "0")
                    GetTargetReport(hosp);
                else
                {
                    hosp.ParentCd = GetTargetReport_Child(hosp);

                }
            }

            Class_ExcelWork ew = new Class_ExcelWork();
            ew.HospReports = HospReports;
            ew.HospDistMsts = HospDistMsts;
            ew.RetdocList = RetdocList;
            ew.ModalityList = ModalityList;
            ew.ModalityDocList = ModalityDocList;
            ew.WriteExcel_Sheet_2(this.Dtp_AggregateMonth.Value, 0);

            MessageBox.Show("集計出力が完了いたしました。");

        }

        private void button3_Click(object sender, EventArgs e)
        {
            var ret = MessageBox.Show("集計には時間がかかりますが、よろしいですか？", "依頼推移表", MessageBoxButtons.OKCancel);
            if (ret == System.Windows.Forms.DialogResult.Cancel)
                return;

            start = this.Dtp_AggregateMonth.Value.ToString("yyyyMM") + "01";
            end = this.Dtp_AggregateMonth.Value.ToString("yyyyMM") + DateTime.DaysInMonth(this.Dtp_AggregateMonth.Value.Year, this.Dtp_AggregateMonth.Value.Month).ToString().PadLeft(2, '0');

            HospReports = new List<ExtReports>();

            foreach (var hosp in HospDistMsts)
            {
                if (hosp.Hospital.IsCopy == "0")
                    GetTargetReport(hosp);
                else
                {
                    hosp.ParentCd = GetTargetReport_Child(hosp);

                }
            }

            Class_ExcelWork ew = new Class_ExcelWork();
            ew.HospReports = HospReports;
            ew.HospDistMsts = HospDistMsts;
            ew.RetdocList = RetdocList;
            ew.ModalityList = ModalityList;
            ew.ModalityDocList = ModalityDocList;
            ew.WriteExcel_Sheet_3(this.Dtp_AggregateMonth.Value, 0);

            MessageBox.Show("集計出力が完了いたしました。");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var ret = MessageBox.Show("集計には時間がかかりますが、よろしいですか？", "依頼推移表", MessageBoxButtons.OKCancel);
            if (ret == System.Windows.Forms.DialogResult.Cancel)
                return;

            start = this.Dtp_AggregateMonth.Value.ToString("yyyyMM") + "01";
            end = this.Dtp_AggregateMonth.Value.ToString("yyyyMM") + DateTime.DaysInMonth(this.Dtp_AggregateMonth.Value.Year, this.Dtp_AggregateMonth.Value.Month).ToString().PadLeft(2, '0');

            HospReports = new List<ExtReports>();

            foreach (var hosp in HospDistMsts)
            {
                if (hosp.Hospital.IsCopy == "0")
                    GetTargetReport(hosp);
                else
                {
                    hosp.ParentCd = GetTargetReport_Child(hosp);

                }
            }

            Class_ExcelWork ew = new Class_ExcelWork();
            ew.HospReports = HospReports;
            ew.HospDistMsts = HospDistMsts;
            ew.RetdocList = RetdocList;
            ew.ModalityList = ModalityList;
            ew.ModalityDocList = ModalityDocList;
            ew.WriteExcel_Sheet_4(this.Dtp_AggregateMonth.Value, 0);

            MessageBox.Show("集計出力が完了いたしました。");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var ret = MessageBox.Show("集計には時間がかかりますが、よろしいですか？", "平均", MessageBoxButtons.OKCancel);
            if (ret == System.Windows.Forms.DialogResult.Cancel)
                return;

            start = this.Dtp_AggregateMonth.Value.ToString("yyyyMM") + "01";
            end = this.Dtp_AggregateMonth.Value.ToString("yyyyMM") + DateTime.DaysInMonth(this.Dtp_AggregateMonth.Value.Year, this.Dtp_AggregateMonth.Value.Month).ToString().PadLeft(2, '0');

            HospReports = new List<ExtReports>();

            foreach (var hosp in HospDistMsts)
            {
                if (hosp.Hospital.IsCopy == "0")
                    GetTargetReport(hosp);
                else
                {
                    hosp.ParentCd = GetTargetReport_Child(hosp);
                }
            }

            Class_ExcelWork ew = new Class_ExcelWork();
            ew.HospReports = HospReports;
            ew.HospDistMsts = HospDistMsts;
            ew.RetdocList = RetdocList;
            ew.ModalityList = ModalityList;
            ew.ModalityDocList = ModalityDocList;
            ew.WriteExcel_Sheet_6(this.Dtp_AggregateMonth.Value, 0);

            MessageBox.Show("集計出力が完了いたしました。");
        }

        //ここまで================================================イベント=========================================================
    }
}
