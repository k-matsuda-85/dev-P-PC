using System;
using System.Web.Services;

using OrderTool_Serv;
using OrderTool_Serv.Class;
using LogController;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Serialization;
using System.Linq;

namespace OrderTool
{
    /// <summary>
    /// OrderTool_WS の概要の説明です
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // この Web サービスを、スクリプトから ASP.NET AJAX を使用して呼び出せるようにするには、次の行のコメントを解除します。
    [System.Web.Script.Services.ScriptService]
    public class OrderTool_WS : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        public WebLogin WebLogin(string id, string pw)
        {
            WebLogin ret = new WebLogin();

            try
            {
                ServiceIF serv = new ServiceIF();
                OrderTool_Serv.Class.Login logData = null;

                ResultData inRet = serv.Login_Serv(id, pw, out logData);

                if (inRet.Result)
                {
                    LogControl.WriteLog(LogType.ORE, "WebLogin", "【ログイン】 ユーザID：" + id);
                    ret.Items = logData;
                }
                else
                    ret.Message = inRet.Message;

                ret.Result = inRet.Result;
            }
            catch (Exception e)
            {
                ret.Result = false;
                ret.Message = "処理中に障害が発生いたしました。\nシステム管理者にお問い合わせください。";
                LogControl.WriteLog(LogType.ERR, "WebLogin", e.Message);
            }

            return ret;
        }

        [WebMethod(EnableSession = true)]
        public WebHosp WebGetHosp(int userid)
        {
            WebHosp ret = new WebHosp();

            try
            {
                ServiceIF serv = new ServiceIF();
                OrderTool_Serv.Class.HospMst hospData = null;

                ResultData inRet = serv.GetHosp_Serv(userid, out hospData);

                if (inRet.Result)
                {
                    LogControl.WriteLog(LogType.ORE, "WebGetHosp", "【施設情報取得】 施設：" + hospData.CD + " 施設ID：" + hospData.HospID);
                    ret.Items = hospData;
                }
                else
                    ret.Message = inRet.Message;

                ret.Result = inRet.Result;
            }
            catch (Exception e)
            {
                ret.Result = false;
                ret.Message = "処理中に障害が発生いたしました。\nシステム管理者にお問い合わせください。";
                LogControl.WriteLog(LogType.ERR, "WebGetHosp", e.Message);
            }

            return ret;
        }

        [WebMethod(EnableSession = true)]
        public WebHospConf WebGetHospConf(int hospid)
        {
            WebHospConf ret = new WebHospConf();

            try
            {
                ServiceIF serv = new ServiceIF();
                OrderTool_Serv.Class.HospitalConfig hospData = null;

                ResultData inRet = serv.GetHospConf_Serv(hospid, out hospData);

                if (inRet.Result)
                {
                    LogControl.WriteLog(LogType.ORE, "WebGetHospConf", "【施設マスタ取得】 施設ID：" + hospData);
                    ret.Items = hospData;
                }
                else
                    ret.Message = inRet.Message;

                ret.Result = inRet.Result;
            }
            catch (Exception e)
            {
                ret.Result = false;
                ret.Message = "処理中に障害が発生いたしました。\nシステム管理者にお問い合わせください。";
                LogControl.WriteLog(LogType.ERR, "WebGetHospConf", e.Message);
            }

            return ret;
        }

        [WebMethod(EnableSession = true)]
        public WebPatientList WebGetPatList(int hospid)
        {
            WebPatientList ret = new WebPatientList();

            try
            {
                ServiceIF serv = new ServiceIF();
                OrderTool_Serv.Class.Search search = new OrderTool_Serv.Class.Search();

                search.HospID = hospid;

                Patient[] tmpList = null;

                ResultData inRet = serv.GetPatient_Serv(search, out tmpList);

                if (inRet.Result)
                {
                    LogControl.WriteLog(LogType.ORE, "WebGetPatList", "【患者一覧取得】 施設ID:" + hospid);
                    ret.Items = tmpList;
                }
                else
                    ret.Message = inRet.Message;

                ret.Result = inRet.Result;
            }
            catch (Exception e)
            {
                ret.Result = false;
                ret.Message = "処理中に障害が発生いたしました。\nシステム管理者にお問い合わせください。";
                LogControl.WriteLog(LogType.ERR, "WebGetList", e.Message);
            }

            return ret;
        }
        [WebMethod(EnableSession = true)]
        public ResultData SetEditOrder(string[] values)
        {
            ResultData ret = new ResultData();

            try
            {
                ServiceIF serv = new ServiceIF();

                HospMst[] hospList;
                HospMst hospMst = new HospMst();
                serv.GetHospList_Serv(out hospList);

                foreach (var hosp in hospList)
                {
                    if (values[0] == hosp.HospID.ToString())
                    {
                        hospMst = hosp;
                        break;
                    }
                }
                LogControl.WriteLog(LogType.ORE, "SetEditOrder", "【変更依頼】 施設ID：" + hospMst.Name + " オーダーID：" + values[1] + " 患者ID：" + values[2] + " ﾓﾀﾞﾘﾃｨ：" + values[5] + " 検査日：" + values[6]);
                var fileName = "【変更】" + hospMst.Name + "_" + values[1] + ".txt";

                string backDef = System.IO.Path.Combine(ConfigurationManager.AppSettings["BackDir"], hospMst.CD);
                backDef = System.IO.Path.Combine(backDef, DateTime.Now.ToString("yyyyMMdd"));

                if (!System.IO.Directory.Exists(backDef))
                    System.IO.Directory.CreateDirectory(backDef);

                using (var sw = new System.IO.StreamWriter(System.IO.Path.Combine(ConfigurationManager.AppSettings["OrderDir"], fileName)))
                {
                    sw.WriteLine(values[2]);
                    sw.WriteLine("");
                    sw.WriteLine("【" + values[3] + "(" + values[4] + ")】");
                    sw.WriteLine("");
                    sw.WriteLine(values[5]);
                    sw.WriteLine("");
                    sw.WriteLine(values[6]);
                }

                using (var sw = new System.IO.StreamWriter(System.IO.Path.Combine(backDef, fileName)))
                {
                    sw.WriteLine(values[2]);
                    sw.WriteLine("");
                    sw.WriteLine("【" + values[3] + "(" + values[4] + ")】");
                    sw.WriteLine("");
                    sw.WriteLine(values[5]);
                    sw.WriteLine("");
                    sw.WriteLine(values[6]);
                }
                ret.Result = true;
            }
            catch (Exception e)
            {
                ret.Result = false;
                ret.Message = "処理中に障害が発生いたしました。\nシステム管理者にお問い合わせください。";
                LogControl.WriteLog(LogType.ERR, "SetEditOrder", e.Message);
            }
            return ret;
        }

        [WebMethod(EnableSession = true)]
        public ResultData DelPreOrder(string[] values)
        {
            ResultData ret = new ResultData();

            try
            {
                ServiceIF serv = new ServiceIF();

                ret = serv.DelPreOrder_Serv(Convert.ToInt32(values[1]));
                if(ret.Result)
                    LogControl.WriteLog(LogType.ORE, "DelPreOrder", "【オーダー候補削除】 施設ID：" + values[0] + " オーダーID：" + values[1] + " 患者ID：" + values[2] + " ﾓﾀﾞﾘﾃｨ：" + values[3] + " 検査日：" + values[4]);
            }
            catch (Exception e)
            {
                ret.Result = false;
                ret.Message = "処理中に障害が発生いたしました。\nシステム管理者にお問い合わせください。";
                LogControl.WriteLog(LogType.ERR, "DelPreOrder", e.Message);
            }
            return ret;
        }


        [WebMethod(EnableSession = true)]
        public WebPreOrderList WebGetPreList(int hospid, string patid)
        {
            WebPreOrderList ret = new WebPreOrderList();

            try
            {
                ServiceIF serv = new ServiceIF();
                OrderTool_Serv.Class.Search search = new OrderTool_Serv.Class.Search();

                search.HospID = hospid;
                search.PatID = patid;

                PreOrder[] tmpList = null;

                ResultData inRet = serv.GetPreOrder_Serv(search, out tmpList);

                if (inRet.Result)
                {
                    if (string.IsNullOrEmpty(patid))
                        LogControl.WriteLog(LogType.ORE, "WebGetPreList", "【オーダー候補一覧取得】 施設ID：" + hospid + " 件数：" + tmpList.Length);
                    else
                        LogControl.WriteLog(LogType.ORE, "WebGetPreList", "【オーダー候補取得】 施設ID：" + hospid + " 患者ID：" + patid + " 件数(過去含む)：" + tmpList.Length);

                    ret.Items = tmpList;
                }
                else
                    ret.Message = inRet.Message;

                ret.Result = inRet.Result;
            }
            catch (Exception e)
            {
                ret.Result = false;
                ret.Message = "処理中に障害が発生いたしました。\nシステム管理者にお問い合わせください。";
                LogControl.WriteLog(LogType.ERR, "WebGetPreList", e.Message);
            }

            return ret;
        }

        [WebMethod(EnableSession = true)]
        public WebResult SetPreOrder(string[] values)
        {
            WebResult ret = new WebResult();

            try
            {
                ServiceIF serv = new ServiceIF();
                PreOrder order = new PreOrder();
                string msg = "";
                string msg2 = "";
                string msg3 = "";

                List<string> pastIds = new List<string>();

                order.HospID = Convert.ToInt32(values[0]);
                if (!string.IsNullOrEmpty(values[1]))
                    order.OrderID = Convert.ToInt32(values[1]);
                else
                    order.OrderID = 0;
                order.PatID = values[2];
                order.PatName = values[3];
                order.PatName_H = values[4];
                order.PatAge = Convert.ToInt32(values[5]);
                order.BirthDay = values[6];
                order.Modality = values[7];
                order.Date = values[8];
                order.Time = values[9];
                order.BodyPart = values[10];
                order.Type = values[11];
                order.ImgCnt = Convert.ToInt32(values[12]);
                order.IsEmergency = Convert.ToInt32(values[13]);
                order.IsMail = Convert.ToInt32(values[14]);
                order.Comment = values[15];
                order.Sex = Convert.ToInt32(values[16]);
                order.Status = 1;
                if (values[17] != "")
                    msg2 = "別途依頼票あり";

                if (order.IsMail == 1)
                    msg3 = values[19] + " " + values[18];

                if (values.Length > 20)
                {
                    for (int i = 20; i < values.Length; i++)
                    {
                        pastIds.Add(values[i]);
                    }
                }

                int key = 0;

                serv.SetPatient_Serv(order, out key);
                order.Key = key;

                ResultData inRet = serv.SetPreOrder_Serv(order);

                if (inRet.Result)
                {
                    LogControl.WriteLog(LogType.ORE, "SetPreOrder", "【オーダー登録】 施設ID：" + order.HospID + "　患者ID：" + order.PatID + " ﾓﾀﾞﾘﾃｨ：" + order.Modality + " 検査日：" + order.Date);

                    OrderTool_Serv.Class.Search search = new OrderTool_Serv.Class.Search();
                    search.PatID = order.PatID;

                    PreOrder[] tmpOrders;

                    serv.GetPreOrder_Serv(search, out tmpOrders);
                    var msg4 = "";
                    foreach (var odr in tmpOrders)
                    {
                        if (pastIds.Contains(odr.OrderID.ToString()))
                        {
                            odr.Status = 2;
                            serv.SetPreOrder_Serv(odr);

                            if (!string.IsNullOrEmpty(msg))
                            {
                                msg += "、";
                                msg4 += "、";
                            }
                            msg += odr.Date.Substring(4, 2) + "/" + odr.Date.Substring(6, 2);
                            msg4 += odr.Date.Substring(0, 4) + "/" + odr.Date.Substring(4, 2) + "/" + odr.Date.Substring(6, 2);
                            if(order.Modality != odr.Modality)
                            {
                                msg4 += " ";
                                msg4 += odr.Modality;
                                msg += " ";
                                msg += odr.Modality;
                            }
                            msg += " ";
                            msg += odr.ImgCnt.ToString();
                        }
                        else if(order.Date == odr.Date
                             && order.Modality == odr.Modality
                             && order.PatID == odr.PatID
                             && order.Time == odr.Time)
                        {
                            if(order.OrderID == 0)
                                order.OrderID = odr.OrderID;
                        }
                    }

                    if (!string.IsNullOrEmpty(msg4))
                        msg4 = "比較参照お願いします。（" + msg4 + "）" + msg2;
                    else
                        msg4 = msg2;

                    string cA = order.ImgCnt.ToString();
                    string cB = msg;

                    if (string.IsNullOrEmpty(msg))
                        msg = order.ImgCnt.ToString();
                    else
                        msg = order.ImgCnt.ToString() + "(" + msg + ")";

                    WriteCSV(order, msg, msg2, msg3, msg4, cA, cB);

                    ret.Result = inRet.Result;
                }
                else
                    ret.Message = inRet.Message;

                ret.Result = inRet.Result;
            }
            catch (Exception e)
            {
                ret.Result = false;
                ret.Message = "処理中に障害が発生いたしました。\nシステム管理者にお問い合わせください。";
                LogControl.WriteLog(LogType.ERR, "WebGetList", e.Message);
            }

            return ret;
        }

        [WebMethod(EnableSession = true)]
        public WebResult SetViewOrder(string[] values)
        {
            WebResult ret = new WebResult();

            try
            {
                ServiceIF serv = new ServiceIF();
                OrderTool_Serv.Class.Search search = new OrderTool_Serv.Class.Search();

                search.HospID = Convert.ToInt32(values[0]);
                search.PatID = values[1];
                PreOrder[] orderList = null;

                serv.GetPreOrder_Serv(search, out orderList);

                foreach(var order in orderList)
                {
                    if(order.Date == values[2] && order.Modality == values[3])
                    {
                        order.Status = 5;
                        serv.SetPreOrder_Serv(order);

                        break;
                    }
                }
            }
            catch (Exception e)
            {
                ret.Result = false;
                ret.Message = "処理中に障害が発生いたしました。\nシステム管理者にお問い合わせください。";
                LogControl.WriteLog(LogType.ERR, "SetViewOrder", e.Message);
            }

            return ret;
        }

        private void WriteCSV(PreOrder order, string msg1, string msg2, string msg3, string msg4, string cntA, string cntB)
        {
            string mstPath = "";
            string outPath = "";
            string orderPath = "";
            string outPath_back = "";
            string orderPath_back = "";

            ServiceIF serv = new ServiceIF();

            HospMst[] hospList;
            HospMst hospMst = new HospMst();
            serv.GetHospList_Serv(out hospList);

            foreach (var hosp in hospList)
            {
                if (order.HospID == hosp.HospID)
                {
                    hospMst = hosp;
                    break;
                }
            }

            mstPath = System.IO.Path.Combine(ConfigurationManager.AppSettings["MastDir"], hospMst.CD + ".csv");
            outPath = System.IO.Path.Combine(ConfigurationManager.AppSettings["CsvDir"], hospMst.CD);

            string backDef = System.IO.Path.Combine(ConfigurationManager.AppSettings["BackDir"], hospMst.CD);
            backDef = System.IO.Path.Combine(backDef, DateTime.Now.ToString("yyyyMMdd"));

            if (!System.IO.Directory.Exists(backDef))
                System.IO.Directory.CreateDirectory(backDef);

            outPath_back = System.IO.Path.Combine(backDef, "CSV");
            if (!System.IO.Directory.Exists(outPath_back))
                System.IO.Directory.CreateDirectory(outPath_back);


            if (order.IsEmergency == 1)
                orderPath = System.IO.Path.Combine(ConfigurationManager.AppSettings["OrderDir"], "☆緊急☆" + hospMst.Name + "_" + order.OrderID + ".txt");
            else
                orderPath = System.IO.Path.Combine(ConfigurationManager.AppSettings["OrderDir"], "【依頼】" + hospMst.Name + "_" + order.OrderID + ".txt");

            orderPath_back = System.IO.Path.Combine(backDef, System.IO.Path.GetFileName(orderPath));

            var csvValue = "";

            using (var sr = new System.IO.StreamReader(mstPath, System.Text.Encoding.GetEncoding("Shift-JIS")))
            {
                csvValue = sr.ReadLine();
            }

            csvValue = csvValue.Replace("@00", order.PatID);
            csvValue = csvValue.Replace("@01", order.PatName);
            csvValue = csvValue.Replace("@02", order.PatName_H);
            switch (order.Sex)
            {
                case 0:
                    csvValue = csvValue.Replace("@03", "O");
                    break;
                case 1:
                    csvValue = csvValue.Replace("@03", "M");
                    break;
                case 2:
                    csvValue = csvValue.Replace("@03", "F");
                    break;
            }

            csvValue = csvValue.Replace("@04", order.BirthDay);
            csvValue = csvValue.Replace("@05", order.PatAge.ToString());
            csvValue = csvValue.Replace("@06", order.Date);
            csvValue = csvValue.Replace("@07", order.Time);
            csvValue = csvValue.Replace("@08", order.Modality);
            csvValue = csvValue.Replace("@09", order.IsEmergency.ToString());
            csvValue = csvValue.Replace("@10", order.Comment);
//            csvValue = csvValue.Replace("@11", msg + " " + msg3);
            csvValue = csvValue.Replace("@11", msg3);
            csvValue = csvValue.Replace("@12", msg4);
            csvValue = csvValue.Replace("@13", order.IsMail.ToString());
            csvValue = csvValue.Replace("@14", hospMst.CD);
            csvValue = csvValue.Replace("@15", hospMst.Name);
            csvValue = csvValue.Replace("@16", order.BodyPart);
            csvValue = csvValue.Replace("@17", order.Type);

            csvValue = csvValue.Replace("@18", order.OrderID.ToString());

            csvValue = csvValue.Replace("@19", cntA);

            if (string.IsNullOrEmpty(cntB) || string.IsNullOrEmpty(cntB.Trim()))
                csvValue = csvValue.Replace("@20", "none");
            else
                csvValue = csvValue.Replace("@20", cntB);


            var fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            using (var sw = new System.IO.StreamWriter(System.IO.Path.Combine(outPath, fileName) + ".csv", false, System.Text.Encoding.GetEncoding("Shift-JIS")))
            {
                sw.WriteLine(csvValue);
            }

            var dat = System.IO.File.Create(System.IO.Path.Combine(outPath, fileName) + ".txt");
            dat.Close();

            if(!string.IsNullOrEmpty(msg3))
            {
                var checkDir = ConfigurationManager.AppSettings["CheckDir"];
                var idxFile = System.IO.Path.Combine(checkDir, hospMst.CD + ".txt");
                if (!System.IO.File.Exists(idxFile))
                {
                    msg3 = ConfigurationManager.AppSettings["CheckVal"] + " " + msg3;
                    using (var sw = new System.IO.StreamWriter(idxFile, false, System.Text.Encoding.GetEncoding("Shift-JIS")))
                        sw.Close();
                }
            }

            using (var sw = new System.IO.StreamWriter(orderPath, false, System.Text.Encoding.GetEncoding("Shift-JIS")))
            {
                sw.WriteLine(order.PatID);
                sw.WriteLine("");
                sw.WriteLine("" + order.PatName + "(" + order.PatName_H + ")");
                sw.WriteLine("");
                sw.WriteLine(order.Modality);
                sw.WriteLine("");
                sw.WriteLine(order.Date);
                sw.WriteLine("");
                sw.WriteLine(msg2);
                sw.WriteLine("");
                sw.WriteLine(msg3);
            }

            using (var sw = new System.IO.StreamWriter(System.IO.Path.Combine(outPath_back, fileName) + ".csv", false, System.Text.Encoding.GetEncoding("Shift-JIS")))
            {
                sw.WriteLine(csvValue);
            }
            using (var sw = new System.IO.StreamWriter(orderPath_back, false, System.Text.Encoding.GetEncoding("Shift-JIS")))
            {
                sw.WriteLine(order.PatID);
                sw.WriteLine("");
                sw.WriteLine("" + order.PatName + "(" + order.PatName_H + ")");
                sw.WriteLine("");
                sw.WriteLine(order.Modality);
                sw.WriteLine("");
                sw.WriteLine(order.Date);
                sw.WriteLine("");
                sw.WriteLine(msg2);
                sw.WriteLine("");
                sw.WriteLine(msg3);
            }
        }

        [WebMethod(EnableSession = true)]
        public WebViewList WebGetViewList(string hospcd)
        {
            WebViewList ret = new WebViewList();

            try
            {
                string path = System.IO.Path.Combine(ConfigurationManager.AppSettings["RetPath"], hospcd);

                if (!System.IO.Directory.Exists(path))
                {
                    LogControl.WriteLog(LogType.ERR, "WebGetViewList", "【所見取得】 フォルダなし：" + path);
                    return ret;
                }

                string[] filePaths = System.IO.Directory.GetFiles(path);
                List<View> tmpList = new List<View>();

                ServiceIF serv = new ServiceIF();
                HospMst[] hospList = null;

                int hospid = 0;

                serv.GetHospList_Serv(out hospList);
                foreach (var hosp in hospList)
                {
                    if (hosp.CD == hospcd)
                    {
                        hospid = hosp.HospID;
                        break;
                    }
                }

                filePaths = filePaths.OrderByDescending(f => System.IO.File.GetLastWriteTime(f)).ToArray();
                int maxCnt = Convert.ToInt32(ConfigurationManager.AppSettings["MaxCnt"]);
                int cnt = 0;
                foreach (var file in filePaths)
                {
                    if (cnt == maxCnt)
                        break;
                    cnt++;

                    string name = System.IO.Path.GetFileNameWithoutExtension(file);
                    string[] vals = name.Split('_');

                    if (vals.Length < 4)
                        continue;

                    View tmp = new View();

                    tmp.PatID = vals[0];
                    tmp.Date = vals[1];
                    tmp.Modality = vals[2];
                    tmp.OrderID = vals[3];
                    if (!string.IsNullOrEmpty(vals[4]))
                    {
                        tmp.ReadDate = vals[4].Substring(0, 8);
                        tmp.ReadDateTime = vals[4];
                    }


                    OrderTool_Serv.Class.Search search = new OrderTool_Serv.Class.Search();
                    search.PatID = tmp.PatID;
                    search.HospID = hospid;

                    Patient[] patList = null;

                    serv.GetPatient_Serv(search, out patList);
                    if (patList.Length > 0)
                        tmp.PatName = patList[0].PatName;
                    else
                        tmp.PatName = "";

                    string dir = System.IO.Path.Combine(ConfigurationManager.AppSettings["PDFPath"], hospcd);
                    string[] tmpFiles = System.IO.Directory.GetFiles(dir, name + ".*");

                    if (tmpFiles == null || tmpFiles.Length == 0)
                        tmp.Status = 0;
                    else
                        tmp.Status = 1;

                    tmpList.Add(tmp);
                }

                ret.Items = tmpList.ToArray();
                ret.Result = true;
            }
            catch (Exception e)
            {
                ret.Result = false;
                ret.Message = "処理中に障害が発生いたしました。\nシステム管理者にお問い合わせください。";
                LogControl.WriteLog(LogType.ERR, "WebGetViewList", e.Message);
            }

            return ret;
        }

    }


    // アシスタントクラス群
    public class WebResult
    {
        public bool Result { get; set; }
        public string Message { get; set; }
    }
    public class WebUser : WebResult
    {
        public UserMst Items { get; set; }
    }
    public class WebLogin : WebResult
    {
        public OrderTool_Serv.Class.Login Items { get; set; }
    }

    public class WebHosp : WebResult
    {
        public OrderTool_Serv.Class.HospMst Items { get; set; }
    }

    public class WebHospConf : WebResult
    {
        public OrderTool_Serv.Class.HospitalConfig Items { get; set; }
    }

    public class WebPreOrderList : WebResult
    {
        public OrderTool_Serv.Class.PreOrder[] Items { get; set; }
    }

    public class WebPatientList : WebResult
    {
        public OrderTool_Serv.Class.Patient[] Items { get; set; }
    }
    public class WebViewList : WebResult
    {
        public View[] Items { get; set; }
    }

    [DataContract]
    public class View
    {
        [DataMember]
        public string PatID { get; set; }
        [DataMember]
        public string Modality { get; set; }
        [DataMember]
        public string Date { get; set; }
        [DataMember]
        public string OrderID { get; set; }
        [DataMember]
        public string ReadDate { get; set; }
        [DataMember]
        public string ReadDateTime { get; set; }
        [DataMember]
        public int Status { get; set; }
        [DataMember]
        public string PatName { get; set; }
    }
}
