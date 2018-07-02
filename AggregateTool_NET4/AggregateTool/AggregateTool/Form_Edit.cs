using Agg_Serv;
using Agg_Serv.Class;
using CommonLib.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AggregateTool
{
    public partial class Form_Edit : Form
    {
        public Form_Menu form_menu;
        private Class_Setting setting = new Class_Setting();
        private Class_Textwork textwork = new Class_Textwork();
        private Class_SearchParam targerResult = new Class_SearchParam();
        public bool confirmationExit = true;
        private bool getUpdate = true;
        private static List<Hospital> RetHospList = new List<Hospital>();
        private static List<Doctor> RetdocList = new List<Doctor>();
        private static List<Config> HospConfig = new List<Config>();
        private static List<string> ModalityList = new List<string>();
        private static Dictionary<string, HospMst[]> HospMst = new Dictionary<string, HospMst[]>();
        List<Report> listRep = new List<Report>();
        List<Report> listHisRep = new List<Report>();
        private static string HospCd = "";

        private static List<string> SysList = new List<string>();

        public Form_Edit()
        {
            InitializeComponent();
        }

        //ここから================================================イベント=========================================================
        
        /// <summary>
        /// ロードイベント
        /// </summary>
        private void Form_Edit_Load(object sender, EventArgs e)
        {
            IF_Service service = new IF_Service();

            RetHospList = service.GetHospital().ToList();

            AddCmbFacility();

            RetdocList = service.GetDoctor().ToList();

            DataTable table = new DataTable();
            table.Columns.Add("Display", typeof(string));
            table.Columns.Add("Value", typeof(string));

            table.Rows.Add("", "");
            foreach (var doc in RetdocList)
            {
                if(doc.IsVisible == "0")
                    table.Rows.Add(doc.Name, doc.Name);
            }

            WorkDoctor.DataSource = table;
            WorkDoctor.ValueMember = "Value";
            WorkDoctor.DisplayMember = "Display";
            SearchDoc.DataSource = table;
            SearchDoc.ValueMember = "Value";
            SearchDoc.DisplayMember = "Display";

            DataTable table2 = new DataTable();
            table2.Columns.Add("Display", typeof(string));
            table2.Columns.Add("Value", typeof(string));
            DataTable table3 = new DataTable();
            table3.Columns.Add("Display", typeof(string));
            table3.Columns.Add("Value", typeof(string));

            table3.Rows.Add("", "");

            string[] modList = null;

            foreach (var conf in form_menu.SystemConfig)
            {
                if (conf.Key == "Modality")
                {
                    modList = conf.Value.Split(',');
                    break;
                }
            }

            for (int i = 0; i < modList.Length; i++)
            {
                string[] modVal = modList[i].Split(':');

                table2.Rows.Add(modVal[1], modVal[2]);
                table3.Rows.Add(modVal[1], modVal[2]);
            }

            Modality.DataSource = table2;
            Modality.ValueMember = "Value";
            Modality.DisplayMember = "Display";
            SearchMod.DataSource = table3;
            SearchMod.ValueMember = "Value";
            SearchMod.DisplayMember = "Display";

        }

        private string GetDBCd(string cd)
        {
            string ret = "";

            foreach (var hos in RetHospList)
            {
                if (hos.Cd == cd)
                {
                    ret = hos.Name_DB;
                    break;
                }
            }

            return ret;
        }

        private string GetHospCd(string dName)
        {
            string ret = "";

            foreach (var hos in RetHospList)
            {
                if(hos.Name_DB == dName)
                {
                    ret = hos.Cd;
                    break;
                }
            }

            return ret;
        }
        
        //ここから------------------------------------------------ボタンクリック処理-----------------------------------------------
        /// <summary>
        /// 検索ボタン
        /// </summary>
        private void Btn_Search_Click(object sender, EventArgs e)
        {
            confirmationExit = true;
            Dgv_InspectionResult.Rows.Clear();
            lblCnt.Text = "";

            IF_Service service = new IF_Service();
            var start = Dtp_SearchDate.Value.ToString("yyyyMMdd");
            var end = dateTimePicker1.Value.ToString("yyyyMMdd");
            var cd = Cmb_Facility.SelectedValue.ToString();

            var dispCd = "";

            foreach(var hosp in RetHospList)
            {
                if(hosp.Cd == cd)
                {
                    dispCd = hosp.Name_DB;
                    break;
                }
            }

            HospCd = dispCd;

            Report[] retRep = null;
                //service.GetReport(dispCd, start, end, "");
            Report[] retHisRep = null;
            //= service.GetReportHist(dispCd, start, end, "");
            listRep = new List<Report>();
            listHisRep = new List<Report>();
            //if (retRep != null)
            //    listRep.AddRange(retRep);
            //if (listHisRep != null)
            //    listHisRep.AddRange(retRep);

            var where = "";
            var mywhere = "";

            if (cd == "SYSTEM")
            {
                mywhere += " AND hcd IN (";

                var wh = "";

                foreach (var hosp in RetHospList)
                {
                    if(SysList.Contains(hosp.Cd))
                    {
                        if (!string.IsNullOrEmpty(wh))
                            wh += ",";

                        wh += "'";
                        wh += hosp.Name_DB;
                        wh += "'";
                    }
                }

                mywhere += wh;
                mywhere += ") ";
                dispCd = "";
            }
            else if (cd == "OTHER")
            {
                mywhere += " AND hcd IN (";

                var wh = "";

                foreach (var hosp in RetHospList)
                {
                    if (!SysList.Contains(hosp.Cd))
                    {
                        if (!string.IsNullOrEmpty(wh))
                            wh += ",";

                        wh += "'";
                        wh += hosp.Name_DB;
                        wh += "'";
                    }
                }

                mywhere += wh;

                mywhere += ") ";
                dispCd = "";
            }
            else
            {
                foreach (var hosp in RetHospList)
                {

                    if (HospMst.Keys.Contains(hosp.Name_DB))
                    {
                        foreach (var mst in HospMst[hosp.Name_DB])
                        {
                            if (mst.Target == cd)
                                where += " AND " + mst.Where;
                            else if (hosp.Name_DB == dispCd)
                                mywhere += " AND NOT " + mst.Where;
                        }

                        if (!string.IsNullOrEmpty(where))
                        {
                            if (!string.IsNullOrEmpty(SearchMod.SelectedValue.ToString()))
                            {
                                where += " AND Modality='" + SearchMod.SelectedValue.ToString() + "'";
                            }
                            if (!string.IsNullOrEmpty(SearchDoc.SelectedValue.ToString()))
                            {
                                where += " AND ReadCd='" + SearchDoc.SelectedValue.ToString() + "'";
                            }
                            Report[] tmp = service.GetReport(hosp.Name_DB, start, end, where);
                            if (tmp != null)
                            {
                                foreach (var t in tmp)
                                {
                                    if (string.IsNullOrEmpty(GetHospCd(t.HCd)))
                                        continue;
                                    listRep.Add(t);
                                }
                            }
                            tmp = service.GetReportHist(hosp.Name_DB, start, end, where);
                            if (tmp != null)
                            {
                                foreach (var t in tmp)
                                {
                                    if (string.IsNullOrEmpty(GetHospCd(t.HCd)))
                                        continue;
                                    listHisRep.Add(t);
                                }
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(SearchMod.SelectedValue.ToString()))
            {
                mywhere += " AND Modality='" + SearchMod.SelectedValue.ToString() + "'";
            }
            if (!string.IsNullOrEmpty(SearchDoc.SelectedValue.ToString()))
            {
                mywhere += " AND ReadCd='" + SearchDoc.SelectedValue.ToString() + "'";
            }

            retRep = service.GetReport(dispCd, start, end, mywhere);
            retHisRep = service.GetReportHist(dispCd, start, end, mywhere);
            if (retRep != null)
            {
                foreach (var t in retRep)
                {
                    if (string.IsNullOrEmpty(GetHospCd(t.HCd)))
                        continue;
                    listRep.Add(t);
                }
            }
            if (retHisRep != null)
            {
                foreach (var t in retHisRep)
                {
                    if (string.IsNullOrEmpty(GetHospCd(t.HCd)))
                        continue;
                    listHisRep.Add(t);
                }
            }

            for(int i = 0; i < listRep.Count; i++)
            {
                Dgv_InspectionResult.Rows.Add();
                Dgv_InspectionResult.Rows[i].Cells[0].Value = listRep[i].PayFlg;
                Dgv_InspectionResult.Rows[i].Cells[1].Value = listRep[i].ClaimFlg;
                Dgv_InspectionResult.Rows[i].Cells[4].Value = listRep[i].PatID;
                Dgv_InspectionResult.Rows[i].Cells[5].Value = listRep[i].PatName;
                Dgv_InspectionResult.Rows[i].Cells[6].Value = listRep[i].StudyDate;
                Dgv_InspectionResult.Rows[i].Cells[7].Value = listRep[i].Modality;
                Dgv_InspectionResult.Rows[i].Cells[8].Value = listRep[i].Accept;
                Dgv_InspectionResult.Rows[i].Cells[9].Value = listRep[i].BodyPart;
                Dgv_InspectionResult.Rows[i].Cells[10].Value = listRep[i].AddMGFlg;
                Dgv_InspectionResult.Rows[i].Cells[11].Value = listRep[i].ReadDate;

                var isDoc = false;
                foreach (var doc in RetdocList)
                {
                    if (doc.Name == listRep[i].ReadCd)
                    {
                        Dgv_InspectionResult.Rows[i].Cells[12].Value = listRep[i].ReadCd;
                        isDoc = true;
                        break;
                    }
                }

                if(!isDoc)
                {
                    Dgv_InspectionResult.Rows[i].Cells[0].Value = false;
                    Dgv_InspectionResult.Rows[i].Cells[1].Value = false;
                }

                Dgv_InspectionResult.Rows[i].Cells[13].Value = listRep[i].BodyPartFlg;
                Dgv_InspectionResult.Rows[i].Cells[14].Value = listRep[i].PriorityFlg;
                Dgv_InspectionResult.Rows[i].Cells[15].Value = listRep[i].ImageCnt;
                Dgv_InspectionResult.Rows[i].Cells[16].Value = listRep[i].AddImageFlg;
                Dgv_InspectionResult.Rows[i].Cells[17].Value = listRep[i].MailFlg;
                Dgv_InspectionResult.Rows[i].Cells[18].Value = listRep[i].Contact;
                Dgv_InspectionResult.Rows[i].Cells[19].Value = listRep[i].OrderDetail;
                Dgv_InspectionResult.Rows[i].Cells[20].Value = listRep[i].Memo;
                Dgv_InspectionResult.Rows[i].Cells[21].Value = listRep[i].OrderNo;
                Dgv_InspectionResult.Rows[i].Cells[22].Value = GetHospCd(listRep[i].HCd);
                Dgv_InspectionResult.Rows[i].Cells[23].Value = listRep[i].PhysicianName;
                Dgv_InspectionResult.Rows[i].Cells[26].Value = listRep[i].Department;

                if (listRep[i].OrderNo.IndexOf("_C") > 0)
                {
                    Dgv_InspectionResult.Rows[i].DefaultCellStyle.BackColor = Color.LimeGreen;
                }

            }

            CheckData();

            lblCnt.Text = listRep.Count + " 件";
        }

        private void CheckData()
        {
            var addDatas = new List<Report>();

            for (int j = 0; j < listHisRep.Count;j++ )
            {
                var isData = false;

                for (int i = 0; i < listRep.Count; i++)
                {
                    if(listRep[i].OrderNo == listHisRep[j].OrderNo)
                    {
                        isData = true;

                        if (listRep[i].PayFlg != listHisRep[j].PayFlg)
                            Dgv_InspectionResult.Rows[i].Cells[0].Style.BackColor = Color.CornflowerBlue;
                        if (listRep[i].ClaimFlg != listHisRep[j].ClaimFlg)
                            Dgv_InspectionResult.Rows[i].Cells[1].Style.BackColor = Color.CornflowerBlue;
                        if (listRep[i].PatID != listHisRep[j].PatID)
                            Dgv_InspectionResult.Rows[i].Cells[4].Style.BackColor = Color.CornflowerBlue;
                        if (listRep[i].PatName != listHisRep[j].PatName)
                            Dgv_InspectionResult.Rows[i].Cells[5].Style.BackColor = Color.CornflowerBlue;
                        if (listRep[i].StudyDate != listHisRep[j].StudyDate)
                            Dgv_InspectionResult.Rows[i].Cells[6].Style.BackColor = Color.CornflowerBlue;
                        if (listRep[i].Modality != listHisRep[j].Modality)
                            Dgv_InspectionResult.Rows[i].Cells[7].Style.BackColor = Color.CornflowerBlue;
                        if (listRep[i].Accept != listHisRep[j].Accept)
                            Dgv_InspectionResult.Rows[i].Cells[8].Style.BackColor = Color.CornflowerBlue;
                        if (listRep[i].BodyPart != listHisRep[j].BodyPart)
                            Dgv_InspectionResult.Rows[i].Cells[9].Style.BackColor = Color.CornflowerBlue;
                        if (listRep[i].AddMGFlg != listHisRep[j].AddMGFlg)
                            Dgv_InspectionResult.Rows[i].Cells[10].Style.BackColor = Color.CornflowerBlue;
                        if (listRep[i].ReadDate != listHisRep[j].ReadDate)
                            Dgv_InspectionResult.Rows[i].Cells[11].Style.BackColor = Color.CornflowerBlue;
                        if (listRep[i].ReadCd != listHisRep[j].ReadCd)
                            Dgv_InspectionResult.Rows[i].Cells[12].Style.BackColor = Color.CornflowerBlue;
                        if (listRep[i].BodyPartFlg != listHisRep[j].BodyPartFlg)
                            Dgv_InspectionResult.Rows[i].Cells[13].Style.BackColor = Color.CornflowerBlue;
                        if (listRep[i].PriorityFlg != listHisRep[j].PriorityFlg)
                            Dgv_InspectionResult.Rows[i].Cells[14].Style.BackColor = Color.CornflowerBlue;
                        if (listRep[i].ImageCnt != listHisRep[j].ImageCnt)
                            Dgv_InspectionResult.Rows[i].Cells[15].Style.BackColor = Color.CornflowerBlue;
                        if (listRep[i].AddImageFlg != listHisRep[j].AddImageFlg)
                            Dgv_InspectionResult.Rows[i].Cells[16].Style.BackColor = Color.CornflowerBlue;
                        if (listRep[i].MailFlg != listHisRep[j].MailFlg)
                            Dgv_InspectionResult.Rows[i].Cells[17].Style.BackColor = Color.CornflowerBlue;
                        if (listRep[i].Contact != listHisRep[j].Contact)
                            Dgv_InspectionResult.Rows[i].Cells[18].Style.BackColor = Color.CornflowerBlue;
                        if (listRep[i].OrderDetail != listHisRep[j].OrderDetail)
                            Dgv_InspectionResult.Rows[i].Cells[19].Style.BackColor = Color.CornflowerBlue;
                        if (listRep[i].Memo != listHisRep[j].Memo)
                            Dgv_InspectionResult.Rows[i].Cells[20].Style.BackColor = Color.CornflowerBlue;

                        break;
                    }
                }


                if (!isData)
                    addDatas.Add(listHisRep[j]);
            }

            for (int i = listRep.Count; i < listRep.Count + addDatas.Count; i++)
            {
                Dgv_InspectionResult.Rows.Add();
                Dgv_InspectionResult.Rows[i].Cells[0].Value = addDatas[i - listRep.Count].PayFlg;
                Dgv_InspectionResult.Rows[i].Cells[1].Value = addDatas[i - listRep.Count].ClaimFlg;
                Dgv_InspectionResult.Rows[i].Cells[4].Value = addDatas[i - listRep.Count].PatID;
                Dgv_InspectionResult.Rows[i].Cells[5].Value = addDatas[i - listRep.Count].PatName;
                Dgv_InspectionResult.Rows[i].Cells[6].Value = addDatas[i - listRep.Count].StudyDate;
                Dgv_InspectionResult.Rows[i].Cells[7].Value = addDatas[i - listRep.Count].Modality;
                Dgv_InspectionResult.Rows[i].Cells[8].Value = addDatas[i - listRep.Count].Accept;
                Dgv_InspectionResult.Rows[i].Cells[9].Value = addDatas[i - listRep.Count].BodyPart;
                Dgv_InspectionResult.Rows[i].Cells[10].Value = addDatas[i - listRep.Count].AddMGFlg;
                Dgv_InspectionResult.Rows[i].Cells[11].Value = addDatas[i - listRep.Count].ReadDate;

                var isDoc = false;
                foreach (var doc in RetdocList)
                {
                    if (doc.Name == addDatas[i - listRep.Count].ReadCd)
                    {
                        Dgv_InspectionResult.Rows[i].Cells[12].Value = addDatas[i - listRep.Count].ReadCd;
                        isDoc = true;
                        break;
                    }
                }

                if (!isDoc)
                {
                    Dgv_InspectionResult.Rows[i].Cells[0].Value = false;
                    Dgv_InspectionResult.Rows[i].Cells[1].Value = false;
                } 

                Dgv_InspectionResult.Rows[i].Cells[13].Value = addDatas[i - listRep.Count].BodyPartFlg;
                Dgv_InspectionResult.Rows[i].Cells[14].Value = addDatas[i - listRep.Count].PriorityFlg;
                Dgv_InspectionResult.Rows[i].Cells[15].Value = addDatas[i - listRep.Count].ImageCnt;
                Dgv_InspectionResult.Rows[i].Cells[16].Value = addDatas[i - listRep.Count].AddImageFlg;
                Dgv_InspectionResult.Rows[i].Cells[17].Value = addDatas[i - listRep.Count].MailFlg;
                Dgv_InspectionResult.Rows[i].Cells[18].Value = addDatas[i - listRep.Count].Contact;
                Dgv_InspectionResult.Rows[i].Cells[19].Value = addDatas[i - listRep.Count].OrderDetail;
                Dgv_InspectionResult.Rows[i].Cells[20].Value = addDatas[i - listRep.Count].Memo;
                Dgv_InspectionResult.Rows[i].Cells[21].Value = addDatas[i - listRep.Count].OrderNo;
                Dgv_InspectionResult.Rows[i].Cells[22].Value = GetHospCd(addDatas[i - listRep.Count].HCd);
                Dgv_InspectionResult.Rows[i].Cells[23].Value = addDatas[i - listRep.Count].PhysicianName;
                Dgv_InspectionResult.Rows[i].Cells[24].Value = "D";
                Dgv_InspectionResult.Rows[i].DefaultCellStyle.BackColor = Color.DarkGray;
                Dgv_InspectionResult.Rows[i].Cells[26].Value = addDatas[i - listRep.Count].Department;
            }

        }

        /// <summary>
        /// 更新ボタン
        /// </summary>
        private void Btn_DgvUpdate_Click(object sender, EventArgs e)
        {
            if(ShowResultDialog("更新を行ってよろしいですか？"))
            {
                List<Report> tmplistRep = new List<Report>();

                for (int i = 0; i < Dgv_InspectionResult.Rows.Count; i++)
                {
                    if (Dgv_InspectionResult.Rows[i].Cells[24].Value == "D")
                        continue;

                    Report rep = new Report();

                    if (Dgv_InspectionResult.Rows[i].Cells[0].Value != null)
                        rep.PayFlg = Convert.ToInt32(Dgv_InspectionResult.Rows[i].Cells[0].Value);
                    if (Dgv_InspectionResult.Rows[i].Cells[1].Value != null)
                        rep.ClaimFlg = Convert.ToInt32(Dgv_InspectionResult.Rows[i].Cells[1].Value);
                    if (Dgv_InspectionResult.Rows[i].Cells[4].Value != null)
                        rep.PatID = Dgv_InspectionResult.Rows[i].Cells[4].Value.ToString();
                    if (Dgv_InspectionResult.Rows[i].Cells[5].Value != null)
                        rep.PatName = Dgv_InspectionResult.Rows[i].Cells[5].Value.ToString();
                    if (Dgv_InspectionResult.Rows[i].Cells[6].Value != null)
                        rep.StudyDate = Dgv_InspectionResult.Rows[i].Cells[6].Value.ToString();
                    if (Dgv_InspectionResult.Rows[i].Cells[7].Value != null)
                        rep.Modality = Dgv_InspectionResult.Rows[i].Cells[7].Value.ToString();
                    if (Dgv_InspectionResult.Rows[i].Cells[8].Value != null)
                        rep.Accept = Dgv_InspectionResult.Rows[i].Cells[8].Value.ToString();
                    if (Dgv_InspectionResult.Rows[i].Cells[9].Value != null)
                        rep.BodyPart = Dgv_InspectionResult.Rows[i].Cells[9].Value.ToString();
                    if (Dgv_InspectionResult.Rows[i].Cells[10].Value != null)
                        rep.AddMGFlg = Convert.ToInt32(Dgv_InspectionResult.Rows[i].Cells[10].Value);
                    if (Dgv_InspectionResult.Rows[i].Cells[11].Value != null)
                        rep.ReadDate = Dgv_InspectionResult.Rows[i].Cells[11].Value.ToString();
                    if (Dgv_InspectionResult.Rows[i].Cells[12].Value != null)
                        rep.ReadCd = Dgv_InspectionResult.Rows[i].Cells[12].Value.ToString();
                    if (Dgv_InspectionResult.Rows[i].Cells[13].Value != null)
                        rep.BodyPartFlg = Convert.ToInt32(Dgv_InspectionResult.Rows[i].Cells[13].Value);
                    if (Dgv_InspectionResult.Rows[i].Cells[14].Value != null)
                        rep.PriorityFlg = Convert.ToInt32(Dgv_InspectionResult.Rows[i].Cells[14].Value);
                    if (Dgv_InspectionResult.Rows[i].Cells[15].Value != null)
                        rep.ImageCnt = Convert.ToInt32(Dgv_InspectionResult.Rows[i].Cells[15].Value);
                    if (Dgv_InspectionResult.Rows[i].Cells[16].Value != null)
                        rep.AddImageFlg = Convert.ToInt32(Dgv_InspectionResult.Rows[i].Cells[16].Value);
                    if (Dgv_InspectionResult.Rows[i].Cells[17].Value != null)
                        rep.MailFlg = Convert.ToInt32(Dgv_InspectionResult.Rows[i].Cells[17].Value);
                    if (Dgv_InspectionResult.Rows[i].Cells[18].Value != null)
                        rep.Contact = Dgv_InspectionResult.Rows[i].Cells[18].Value.ToString();
                    if (Dgv_InspectionResult.Rows[i].Cells[19].Value != null)
                        rep.OrderDetail = Dgv_InspectionResult.Rows[i].Cells[19].Value.ToString();
                    if (Dgv_InspectionResult.Rows[i].Cells[20].Value != null)
                        rep.Memo = Dgv_InspectionResult.Rows[i].Cells[20].Value.ToString();
                    rep.OrderNo = Dgv_InspectionResult.Rows[i].Cells[21].Value.ToString();
                    rep.HCd = GetDBCd(Dgv_InspectionResult.Rows[i].Cells[22].Value.ToString());
                    if (Dgv_InspectionResult.Rows[i].Cells[23].Value != null)
                        rep.PhysicianName = Dgv_InspectionResult.Rows[i].Cells[23].Value.ToString();
                    if (Dgv_InspectionResult.Rows[i].Cells[26].Value != null)
                        rep.Department = Dgv_InspectionResult.Rows[i].Cells[26].Value.ToString();

                    if(rep.ReadCd == "ﾃｽﾄｵｰﾀﾞｰ")
                    {
                        rep.PayFlg = 0;
                        rep.ClaimFlg = 0;
                    }

                    tmplistRep.Add(rep);
                }
                IF_Service service = new IF_Service();

                service.DelReport(HospCd, listRep.ToArray());

                if (service.SetReport(HospCd, tmplistRep.ToArray()))
                {
                    var end = dateTimePicker1.Value.ToString("yyyyMMdd");

                    LogUtil.Info(form_menu.form_login.UserRet.Name + " " + HospCd + " " + end + "  レポートデータ更新");
                    MessageBox.Show("レポートデータの更新が完了いたしました。", "完了", MessageBoxButtons.OK);
                    confirmationExit = true;
                    Btn_Search_Click(sender, e);
                }
                else
                    MessageBox.Show("レポートデータの更新に失敗いたしました。", "エラー", MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// リスト表示画面を初期表示(検索時に保管した検索条件を使って再検索)
        /// </summary>
        private void Btn_DgvClear_Click(object sender, EventArgs e)
        {
            if (ShowResultDialog("初期値の戻してよろしいですか？"))
            {
                //初期状態に戻す為、退出確認フラグをtrueにする。
                confirmationExit = true;
                //グリッドをクリア
                Dgv_InspectionResult.Rows.Clear();
                for (int i = 0; i < listRep.Count; i++)
                {
                    Dgv_InspectionResult.Rows.Add();
                    Dgv_InspectionResult.Rows[i].Cells[0].Value = listRep[i].PayFlg;
                    Dgv_InspectionResult.Rows[i].Cells[1].Value = listRep[i].ClaimFlg;
                    Dgv_InspectionResult.Rows[i].Cells[4].Value = listRep[i].PatID;
                    Dgv_InspectionResult.Rows[i].Cells[5].Value = listRep[i].PatName;
                    Dgv_InspectionResult.Rows[i].Cells[6].Value = listRep[i].StudyDate;
                    Dgv_InspectionResult.Rows[i].Cells[7].Value = listRep[i].Modality;
                    Dgv_InspectionResult.Rows[i].Cells[8].Value = listRep[i].Accept;
                    Dgv_InspectionResult.Rows[i].Cells[9].Value = listRep[i].BodyPart;
                    Dgv_InspectionResult.Rows[i].Cells[10].Value = listRep[i].AddMGFlg;
                    Dgv_InspectionResult.Rows[i].Cells[11].Value = listRep[i].ReadDate;

                    var isDoc = false;
                    foreach (var doc in RetdocList)
                    {
                        if (doc.Name == listRep[i].ReadCd)
                        {
                            Dgv_InspectionResult.Rows[i].Cells[12].Value = listRep[i].ReadCd;
                            isDoc = true;
                            break;
                        }
                    }

                    if (!isDoc)
                    {
                        Dgv_InspectionResult.Rows[i].Cells[0].Value = false;
                        Dgv_InspectionResult.Rows[i].Cells[1].Value = false;
                    }

                    Dgv_InspectionResult.Rows[i].Cells[13].Value = listRep[i].BodyPartFlg;
                    Dgv_InspectionResult.Rows[i].Cells[14].Value = listRep[i].PriorityFlg;
                    Dgv_InspectionResult.Rows[i].Cells[15].Value = listRep[i].ImageCnt;
                    Dgv_InspectionResult.Rows[i].Cells[16].Value = listRep[i].AddImageFlg;
                    Dgv_InspectionResult.Rows[i].Cells[17].Value = listRep[i].MailFlg;
                    Dgv_InspectionResult.Rows[i].Cells[18].Value = listRep[i].Contact;
                    Dgv_InspectionResult.Rows[i].Cells[19].Value = listRep[i].OrderDetail;
                    Dgv_InspectionResult.Rows[i].Cells[20].Value = listRep[i].Memo;
                    Dgv_InspectionResult.Rows[i].Cells[21].Value = listRep[i].OrderNo;
                    Dgv_InspectionResult.Rows[i].Cells[22].Value = GetHospCd(listRep[i].HCd);
                    Dgv_InspectionResult.Rows[i].Cells[23].Value = listRep[i].PhysicianName;


                    if (listRep[i].OrderNo.IndexOf("_C") > 0)
                    {
                        Dgv_InspectionResult.Rows[i].DefaultCellStyle.BackColor = Color.LimeGreen;
                    }
                }
                confirmationExit = true;
            }
            CheckData();

            lblCnt.Text = listRep.Count + " 件";
        }

        //ここまで------------------------------------------------ボタンクリック処理-----------------------------------------------

        //ここから------------------------------------------------グリッドビュー内処理---------------------------------------------
        /// <summary>
        /// ボタンカラムがクリックされた際の処理
        /// </summary>
        private void Dgv_InspectionResult_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex != -1)
            {
                confirmationExit = false;
                //編集画面の消費
                if (e.ColumnIndex == 25)
                {
                    var orderNo = Dgv_InspectionResult.Rows[e.RowIndex].Cells[21].Value;
                    var URL = ConfigurationManager.AppSettings["LinkURL"] + orderNo;
                    System.Diagnostics.Process.Start(URL);
                }
                //コピーを実行
                else if (e.ColumnIndex == 2)
                {
                    confirmationExit = false;
                    CopyRow(e.RowIndex);
                }
                //削除を実行
                else if (e.ColumnIndex == 3)
                {
                    if (Dgv_InspectionResult.Rows[e.RowIndex].Cells[24].Value == "D")
                    {
                        if (ShowResultDialog("この行を集計に含みますがよろしいですか？"))
                        {
                            confirmationExit = false;
                            Dgv_InspectionResult.Rows[e.RowIndex].Cells[24].Value = "";
                            Dgv_InspectionResult.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Lime;
                        }
                    }
                    else
                    {
                        if (ShowResultDialog("この行を集計から削除してもよろしいですか？"))
                        {
                            confirmationExit = false;
                            Dgv_InspectionResult.Rows[e.RowIndex].Cells[24].Value = "D";
                            Dgv_InspectionResult.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                        }
                    }

                }
            }
        }

        //ここまで------------------------------------------------グリッドビュー内処理----------------------------------------------
        /// <summary>
        /// このフォームをクローズする。
        /// </summary>
        private void Btn_ReturnMenu_Click(object sender, EventArgs e)
        {
              this.Close();
        }

        /// <summary>
        /// クローズされている際に、メニュー画面を呼ぶ
        /// </summary>
        private void Form_Edit_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!confirmationExit)
            {
                if (!ShowResultDialog("一覧画面を閉じてもよろしいですか？"))
                    e.Cancel = true;
                else
                {
                    //メニューを開く
                    form_menu.Visible = true;
                    //フォーカスをなくす
                    form_menu.ActiveControl = null;
                }
            }
            else
            {
                //メニューを開く
                form_menu.Visible = true;
                //フォーカスをなくす
                form_menu.ActiveControl = null;
            }
        }

        //ここまで================================================イベント=============================================================

        //ここから================================================内部メソッド=========================================================
        
        /// <summary>
        /// 選択行の値をカラム名をkey、値をValueにしたDictionaryを作成
        /// </summary>
        private Dictionary<string, string> getRowsValue(int rowIndex)
        {
            Dictionary<string, string> editValue = new Dictionary<string, string>();
            for (int i = 3; i < Dgv_InspectionResult.ColumnCount; i++ )
            {
                if (!Dgv_InspectionResult.Columns[i].Visible)
                    continue;
                string ky = Dgv_InspectionResult.Columns[i].Name;
                string vl = Dgv_InspectionResult.Rows[rowIndex].Cells[i].Value.ToString();
                editValue.Add(ky,vl);
            }
            return editValue;
        }

        /// <summary>
        /// 編集画面から戻る際に値を挿入するメソッド
        /// </summary>
        public void SetNewRowsValue(int index, Dictionary<string, string> value)
        {
            for (int i = 3; i < Dgv_InspectionResult.Columns.Count; i++)
            {
                if (!value.Keys.Contains(Dgv_InspectionResult.Columns[i].Name)&&
                    !Dgv_InspectionResult.Columns[i].Visible || !value.Keys.Contains(Dgv_InspectionResult.Columns[i].Name))
                    continue;
                
                Dgv_InspectionResult.Rows[index].Cells[i].Value = value[Dgv_InspectionResult.Columns[i].Name];
            }
        }

        /// <summary>
        /// コンボボックスに施設名称を追加するメソッド
        /// </summary>
        private void AddCmbFacility()
        {
            IF_Service service = new IF_Service();

            //コンボボックスに追加
            DataTable table = new DataTable();
            DataTable table2 = new DataTable();
            table.Columns.Add("Display", typeof(string));
            table.Columns.Add("Value", typeof(string));
            table2.Columns.Add("Display", typeof(string));
            table2.Columns.Add("Value", typeof(string));

            table.Rows.Add("", "");

            foreach (var hosp in RetHospList)
            {
                table.Rows.Add(hosp.Name, hosp.Cd);
                table2.Rows.Add(hosp.Name, hosp.Cd);
                var dat = service.GetHospitalConfig(hosp.Cd);
                string[] confs = null;
                for(int i = 0; i < dat.Length; i++)
                {
                    if(dat[i].Key == "Hosp" && !String.IsNullOrEmpty(dat[i].Value))
                    {
                        confs = dat[i].Value.Split('@');
                    }
                    else if (dat[i].Key == "Dist" && !String.IsNullOrEmpty(dat[i].Value) && dat[i].Value == "1")
                    {
                        SysList.Add(hosp.Cd);
                    }
                }
                if(confs == null)
                    continue;

                List<HospMst> list = new List<HospMst>();
                foreach(var conf in confs)
                {
                    var datas = conf.Split(':');
                    HospMst mst = new AggregateTool.HospMst();
                    mst.Target = datas[0];
                    var wheres = datas[1].Split(',');
                    mst.Where = wheres[0] + " " + wheres[1] + " '" + wheres[2] + "'";

                    list.Add(mst);
                }
                HospMst[hosp.Name_DB] = list.ToArray();
            }

            table.Rows.Add("システム連携施設", "SYSTEM");
            table.Rows.Add("システム連携施設 以外", "OTHER");


            Cmb_Facility.DataSource = table;
            Cmb_Facility.ValueMember = "Value";
            Cmb_Facility.DisplayMember = "Display";
            Office.DataSource = table2;
            Office.ValueMember = "Value";
            Office.DisplayMember = "Display";

//            Cmb_Facility.SelectedIndex = -1;
        }
        /// <summary>
        /// 確認ダイアログ表示メソッド
        /// </summary>
        public bool ShowResultDialog(string msg)
        {
            bool decision = false;
            DialogResult result = MessageBox.Show(msg,"注意",MessageBoxButtons.YesNo,
            MessageBoxIcon.Exclamation,MessageBoxDefaultButton.Button2);

            //何が選択されたか調べる
            if (result == DialogResult.Yes)
            {
                decision = true;
            }
            return decision;
        }

        /// <summary>
        /// 検索結果の列設定
        /// </summary>
        private void SetResultColumn(Class_SearchResult se)
        {
            for(int i = 3 ; i < Dgv_InspectionResult.ColumnCount ; i++)
            {
                string header = Dgv_InspectionResult.Columns[i].Name;
                if (!se.columnValue.Contains(header))
                    Dgv_InspectionResult.Columns[i].Visible = false;
            }
        }

        /// <summary>
        /// 更新作業
        /// </summary>
        private List<string> GetRowsValue()
        {
            List<string> upDateList = new List<string>();
            for(int i = 0 ; i < Dgv_InspectionResult.Rows.Count;i++)
            {
                string line = "";
                for(int j = 3 ; j < Dgv_InspectionResult.Columns.Count ; j++)
                {
                    if (line.Length != 0)
                        line += ',';
                    if (!Dgv_InspectionResult.Columns[j].Visible)
                        continue;
                    line += makeString(Dgv_InspectionResult.Rows[i].Cells[j].Value.ToString(),
                                                       Dgv_InspectionResult.Columns[j].Name);
                }
                upDateList.Add(line);
            }
            return upDateList;
        }

        /// <summary>
        /// 書き込み文字列を作成
        /// </summary>
        private string makeString(string cellValue,string columnName)
        {
            return columnName + ":" + cellValue;
        }

        /// <summary>
        /// 検索結果の表示メインメソッド
        /// </summary>
        private void AddSearchValue(List<Dictionary<string, string>> list, Class_SearchResult seachResult)
        {
            for (int i = 0; i < list.Count;i++ )
            {
                SetColumnValue(seachResult, list[i], i);
            }
        }

        /// <summary>
        /// 行の作成と値の追加を行う
        /// </summary>
        private void SetColumnValue(Class_SearchResult se,Dictionary<string, string> dc , int rowPos)
        {
            Dgv_InspectionResult.Rows.Add();
            foreach(string columnName in se.columnValue)
            {
                Dgv_InspectionResult.Rows[rowPos].Cells[GetColumnIndex(columnName)].Value = dc[columnName];
            }
            if (!se.columnValue.Contains("Payment"))
                Dgv_InspectionResult.Rows[rowPos].Cells[GetColumnIndex("Payment")].Value = 0;
            if (!se.columnValue.Contains("Claim"))
                Dgv_InspectionResult.Rows[rowPos].Cells[GetColumnIndex("Claim")].Value = 0;
        }

        /// <summary>
        /// 値を追加するカラムのインデックスを取得する
        /// </summary>
        private int GetColumnIndex(string name)
        {
            int index = 0;
            for(int i = 0 ; i < Dgv_InspectionResult.Columns.Count;i++)
            {
                if (Dgv_InspectionResult.Columns[i].Name.Equals(name))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        /// <summary>
        /// コピーを直下追加するメソッド
        /// </summary>
        private void CopyRow(int rowIndex)
        {
            //元の行を取得
            DataGridViewRow SourceRow = Dgv_InspectionResult.Rows[rowIndex];
            //あたらしい行の値を作成
            DataGridViewRow DestinationRow = new DataGridViewRow(); 
            //行を作成
            DestinationRow.CreateCells(Dgv_InspectionResult);
            DestinationRow.DefaultCellStyle.BackColor = Color.Lime;
            //値をコピー
            for (int i = 0; i < SourceRow.Cells.Count; i++)
            {
                if (i == 21)
                    DestinationRow.Cells[i].Value = SourceRow.Cells[i].Value + "_C";
                else
                    DestinationRow.Cells[i].Value = SourceRow.Cells[i].Value;
            }
            //作成した行を挿入する
            Dgv_InspectionResult.Rows.Insert(rowIndex + 1, DestinationRow);
        }

        /// <summary>
        /// 検索項目を各コントロールから取得して変換する
        /// </summary>
        private Class_SearchParam SetSeachParam(string datetime)
        {
            //割り当て
            Class_SearchParam se = new Class_SearchParam();
            //DateTimePickerから"****/**/**"この形でstring型に変換
            se.searchDate = datetime;
            //先頭を選択していた場合空で設定する
            if (this.Cmb_Facility.SelectedIndex == -1)
                se.searchName = "";
            else
                //選択したアイテムを設定
                se.searchName = this.Cmb_Facility.SelectedValue.ToString();
            return se;
        }

        /// <summary>
        /// 支払い有無、請求有無を決定
        /// </summary>
        private void HadPaymentColumn()
        {
            //設定ファイルで表示非表示を決定する。
            if(setting.positionGridBtn.Contains("支払有無"))
                Dgv_InspectionResult.Columns["Payment"].Visible = true;
            if(setting.positionGridBtn.Contains("請求有無"))
                Dgv_InspectionResult.Columns["Claim"].Visible = true;
        }

        /// <summary>
        /// テスト用
        /// </summary>
        private void TestFacility()
        {
            Cmb_Facility.Items.Add("テスト病院A");
            Cmb_Facility.Items.Add("テスト病院B");
            Cmb_Facility.Items.Add("テスト病院C");
            Cmb_Facility.SelectedIndex = 0;
        }

        private void Dgv_InspectionResult_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (!confirmationExit)
                Dgv_InspectionResult.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Yellow;

        }
        //ここまで================================================内部メソッド=========================================================
    }
}
 