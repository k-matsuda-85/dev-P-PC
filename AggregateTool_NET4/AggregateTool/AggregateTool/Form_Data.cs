using Agg_Serv;
using Agg_Serv.Class;
using CommonLib.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AggregateTool
{
    public partial class Form_Data : Form
    {
        public Form_Menu form_menu;
        private static List<Hospital> RetHospList = new List<Hospital>();
        private static List<Doctor> RetdocList = new List<Doctor>();

        public Form_Data()
        {
            InitializeComponent();
        }

        private string GetHospCd(string dName)
        {
            string ret = "";

            foreach (var hos in RetHospList)
            {
                if (hos.Name_DB == dName)
                {
                    ret = hos.Cd;
                    break;
                }
            }

            return ret;
        }

        private string GetDocCd(string dName)
        {
            string ret = "";

            foreach (var hos in RetdocList)
            {
                if (hos.Name == dName)
                {
                    ret = hos.Cd;
                    break;
                }
            }

            return ret;
        }

        private void btn_Get_Click(object sender, EventArgs e)
        {

            string levelCode = ConfigurationManager.AppSettings["LevelCode"]; /*n次事業所コード*/

            try
            {
                IF_Service service = new IF_Service();

                var start = dateTimePicker1.Value.ToString("yyyyMMdd");
                var end = dateTimePicker2.Value.ToString("yyyyMMdd");
                var imgstart = dateTimePicker1.Value.AddMonths(-1).ToString("yyyy/MM/dd");
                var imgend = dateTimePicker2.Value.ToString("yyyy/MM/dd");

                var setting = "";

                RetHospList = service.GetHospital().ToList();
                RetdocList = service.GetDoctor().ToList();

                foreach (var conf in form_menu.SystemConfig){
                    if("RefDB_RS" == conf.Key)
                    {
                        setting = conf.Value;
                        break;
                    }
                }

                /*☆★↓↓↓2016/06/08 処理分岐用に事業所コード追加↓↓↓★☆*/
                //var reports = service.GetReport_Org(start, end, setting);
                var reports = service.GetReport_Org(start,end,setting,levelCode);
                /*☆★↑↑↑2016/06/08↑↑↑★☆*/

                var pro = "";
                foreach (var conf in form_menu.SystemConfig)
                {
                    if ("RefDB_PACS" == conf.Key)
                    {
                        setting = conf.Value;
                    }
                    if("RefPro_PACS" == conf.Key)
                    {
                        pro = conf.Value;
                    }
                }

                var images = service.GetImageCnt_Org(imgstart, imgend, setting, pro);

                var noHos = "";
                var noDoc = "";

                List<Report> addData = new List<Report>();
                string sHosp = ConfigurationManager.AppSettings["ImageSpecial"];
                foreach (var rep in reports)
                {
                    if (sHosp.IndexOf(rep.HCd) < 0)
                    {
                        if (images != null)
                        {
                            foreach (var img in images)
                            {
                                if (rep.Modality == img.Modality
                                    && img.PatID == rep.PatID
                                    && img.StudyDate == rep.StudyDate)
                                {
                                    rep.ImageCnt = img.ImageCnt;
                                    break;
                                }
                            }
                        }
                    }

                    if(rep.ImageCnt > 0)
                    {
                        switch (rep.HCd)
                        {
                            case "ASE":
                                rep.AddImageFlg = rep.ImageCnt / 1000;

                                if (rep.ImageCnt - rep.AddImageFlg * 1000 == 0)
                                    rep.AddImageFlg--;
                                break;
                            case "HKR":
                                if (rep.ImageCnt > 500)
                                    rep.AddImageFlg = 1;
                                break;
                            default:
                                rep.AddImageFlg = rep.ImageCnt / 500;

                                if (rep.ImageCnt - rep.AddImageFlg * 500 == 0)
                                    rep.AddImageFlg--;
                                break;
                        }
                    }

                    if (string.IsNullOrEmpty(GetHospCd(rep.HCd)))
                    {
                        if (noHos != ""){
                            if (noHos.IndexOf(rep.HCd) < 0)
                                noHos += "," + rep.HCd;
                        }
                        else
                            noHos = rep.HCd;
                    }
                    if (string.IsNullOrEmpty(GetDocCd(rep.ReadCd)))
                    {
                        if (noDoc != "")
                        {
                            if (noDoc.IndexOf(rep.HCd) < 0)
                                noDoc += "," + rep.HCd;
                        }
                        else
                            noDoc = rep.HCd;

                        rep.PayFlg = 0;
                        rep.ClaimFlg = 0;
                    }

                    var datas = service.GetReportOrderNos(rep.HCd, start, end).ToList();

                    if (!datas.Contains(rep.OrderNo))
                        addData.Add(rep);
                    //else
                    //    service.SetImageCnt(rep.HCd, rep);
                }

                if(!string.IsNullOrEmpty(noHos))
                {
                    MessageBox.Show("登録されていない施設が取り込まれています。\n\n" + noHos, "警告");
                }
                if (!string.IsNullOrEmpty(noDoc))
                {
                    MessageBox.Show("登録されていない読影医が記録されたオーダーが取り込まれています。\n\n" + noHos, "警告");
                }

                if (!service.SetReport("", addData.ToArray()))
                    MessageBox.Show("取り込みに失敗いたしました。", "エラー", MessageBoxButtons.OK);
                if (!service.SetReportHist("", addData.ToArray()))
                    MessageBox.Show("取り込みに失敗いたしました。", "エラー", MessageBoxButtons.OK);
                else
                {
                    LogUtil.Info(form_menu.form_login.UserRet.Name + " " + start + " " + end + "  サーバから取り込み");
                    MessageBox.Show("取り込みが完了いたしました。", "取り込み完了", MessageBoxButtons.OK);
                }

            }
            catch(Exception ex)
            {

            }
        }

        private void Form_Data_FormClosing(object sender, FormClosingEventArgs e)
        {
            //メニューを開く
            form_menu.Visible = true;
            //フォーカスをなくす
            form_menu.ActiveControl = null;
        }
    }
}
