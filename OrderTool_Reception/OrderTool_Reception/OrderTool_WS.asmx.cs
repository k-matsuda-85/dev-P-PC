using System;
using System.Web.Services;

using OrderTool_Reception_Serv;
using OrderTool_Reception_Serv.Util;
using LogController;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Serialization;
using System.Linq;
using System.IO;
using System.Text;

namespace OrderTool_Reception
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
        public string WebCheckString(string val)
        {
            string ret = "";
            try
            {
                var fname = "check_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";
                var path = Path.Combine(ConfigurationManager.AppSettings["WorkDir"], fname);

                using (var sw = new StreamWriter(path, false, Encoding.GetEncoding("Shift-JIS")))
                    sw.Write(val);

                using (var sr = new StreamReader(path, Encoding.GetEncoding("Shift-JIS")))
                    ret = sr.ReadToEnd();

                File.Delete(path);
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "WebGetHospList", e.Message);
            }

            return ret;
        }
        [WebMethod(EnableSession = true)]
        public WebHospList WebGetHospList()
        {
            WebHospList ret = new WebHospList();

            try
            {
                ServiceIF serv = new ServiceIF();
                HospMst[] hospList = null;

                ResultData inRet = serv.GetHospList_Serv(out hospList);

                if (inRet.Result)
                    ret.Items = hospList;
                else
                    ret.Message = inRet.Message;

                ret.Result = inRet.Result;
            }
            catch (Exception e)
            {
                ret.Result = false;
                ret.Message = "処理中に障害が発生いたしました。\nシステム管理者にお問い合わせください。";
                LogControl.WriteLog(LogType.ERR, "WebGetHospList", e.Message);
            }

            return ret;
        }
        [WebMethod(EnableSession = true)]
        public WebHospTempList WebGetHospTempList(int id)
        {
            WebHospTempList ret = new WebHospTempList();

            try
            {
                ServiceIF serv = new ServiceIF();
                HospitalTemplate[] tempList = null;

                ResultData inRet = serv.GetHospTemplate_Serv(id, out tempList);

                if (inRet.Result)
                    ret.Items = tempList;
                else
                    ret.Message = inRet.Message;

                ret.Result = inRet.Result;
            }
            catch (Exception e)
            {
                ret.Result = false;
                ret.Message = "処理中に障害が発生いたしました。\nシステム管理者にお問い合わせください。";
                LogControl.WriteLog(LogType.ERR, "WebGetHospTempList", e.Message);
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
                HospitalConfig hospData = null;

                ResultData inRet = serv.GetHospConfig_Serv(hospid, out hospData);

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
        public void WebSetHospTempList(int id, string key, string[] values)
        {

            try
            {
                ServiceIF serv = new ServiceIF();

                serv.SetHospTemplate_Serv(id, key, values);
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "WebGetHospTempList", e.Message);
            }

            return ;
        }

        [WebMethod(EnableSession = true)]
        public void WebSetHospConfig(int id, string key, string values)
        {

            try
            {
                ServiceIF serv = new ServiceIF();

                HospitalConfig conf = new HospitalConfig();
                conf.HospID = id;
                Config tmpconf = new Config();
                tmpconf.Key = key;
                tmpconf.Value = values;

                List<Config> tmpList = new List<Config>();
                tmpList.Add(tmpconf);

                conf.Conf = tmpList.ToArray();

                serv.SetHospConfig_Serv(conf);

            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "WebSetHospConfig", e.Message);
            }

            return;
        }

        [WebMethod(EnableSession = true)]
        public void WebGetOrderCSV()
        {

            try
            {
                var path = ConfigurationManager.AppSettings["InputCSVDir"];

                path = Path.Combine(path, DateTime.Now.ToString("yyyyMMddHHmmssfff"));

                using (var sw = new StreamWriter(path))
                {

                }

            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "WebGetOrderCSV", e.Message);
            }

            return;
        }

        [WebMethod(EnableSession = true)]
        public WebUserList WebGetUserList()
        {
            WebUserList ret = new WebUserList();

            try
            {
                ServiceIF serv = new ServiceIF();
                UserMst[] tempList = null;

                ResultData inRet = serv.GetUserList_Serv(out tempList);

                if (inRet.Result)
                    ret.Items = tempList;
                else
                    ret.Message = inRet.Message;

                ret.Result = inRet.Result;
            }
            catch (Exception e)
            {
                ret.Result = false;
                ret.Message = "処理中に障害が発生いたしました。\nシステム管理者にお問い合わせください。";
                LogControl.WriteLog(LogType.ERR, "WebGetUserList", e.Message);
            }

            return ret;
        }

        [WebMethod(EnableSession = true)]
        public WebOrderList WebGetOrderList(string[] values)
        {

            WebOrderList ret = new WebOrderList();

            try
            {
                ServiceIF serv = new ServiceIF();

                OrderTool_Reception_Serv.Util.Search param = new OrderTool_Reception_Serv.Util.Search();

                if(!string.IsNullOrEmpty(values[0]))
                    param.HospID = Convert.ToInt32(values[0]);
                if (!string.IsNullOrEmpty(values[1]))
                    param.Status = Convert.ToInt32(values[1]);
                if (!string.IsNullOrEmpty(values[2]))
                    param.OrderID = Convert.ToInt32(values[2]);

                if(param.Status > 3)
                {
                    param.AreaStatus = param.Status - 3;
                    param.Status = -2;
                }

                param.PatID = values[3];
                param.Modality = values[4];
                param.Date_F = values[5];
                param.Date_T = values[6];

                OrderTool_Reception_Serv.Util.Order[] retData = null;

                ResultData inRet = serv.GetOrder_Serv(param, out retData);

                if (inRet.Result)
                {
                    if(values.Length > 7)
                    {
                        List<OrderTool_Reception_Serv.Util.Order> tmpList;
                        tmpList = retData.ToList();

                        tmpList.Sort((a, b) => { return -((a.Date + a.Time).CompareTo((b.Date + b.Time))); });

                        retData = tmpList.ToArray();
                    }

                    ret.Items = retData;
                }
                else
                    ret.Message = inRet.Message;

                ret.Result = inRet.Result;
            }
            catch (Exception e)
            {
                ret.Result = false;
                ret.Message = "処理中に障害が発生いたしました。\nシステム管理者にお問い合わせください。";
                LogControl.WriteLog(LogType.ERR, "WebGetUserList", e.Message);
            }

            return ret;
        }

        [WebMethod(EnableSession = true)]
        public WebFaxList WebGetFaxSender(string hospCd)
        {
            WebFaxList ret = new WebFaxList();

            try
            {
                string iniFile = ConfigurationManager.AppSettings["FAXFile_Intro"];

                if(!File.Exists(iniFile))
                {
                    ret.Result = false;
                    ret.Message = "";
                    LogControl.WriteLog(LogType.ERR, "WebGetFaxSender", "設定ファイルが存在しない。");
                    return ret;
                }

                List<string> datVals = new List<string>();

                using (var sr = new StreamReader(iniFile, Encoding.GetEncoding("Shift-JIS")))
                {
                    while (!sr.EndOfStream)
                    {
                        if (string.IsNullOrEmpty(hospCd))
                            break;

                        string tmpDat = "";

                        tmpDat = sr.ReadLine();

                        if(tmpDat.IndexOf(hospCd) >= 0)
                            datVals.Add(tmpDat);
                    }
                }

                ret.Items = datVals.ToArray();
                ret.Result = true;

            }
            catch(Exception ex)
            {
                ret.Result = false;
                ret.Message = "FAX一覧の取得に失敗しました。";
                LogControl.WriteLog(LogType.ERR, "WebGetFaxSender", ex.Message);
            }

            return ret;
        }

        [WebMethod(EnableSession = true)]
        public WebFaxList WebGetMailSender(string hospCd)
        {
            WebFaxList ret = new WebFaxList();

            try
            {
                string iniFile = ConfigurationManager.AppSettings["MailFile_Intro"];

                if (!File.Exists(iniFile))
                {
                    ret.Result = false;
                    ret.Message = "";
                    LogControl.WriteLog(LogType.ERR, "WebGetMailSender", "設定ファイルが存在しない。");
                    return ret;
                }

                List<string> datVals = new List<string>();

                using (var sr = new StreamReader(iniFile, Encoding.GetEncoding("Shift-JIS")))
                {
                    while (!sr.EndOfStream)
                    {
                        if (string.IsNullOrEmpty(hospCd))
                            break;

                        string tmpDat = "";

                        tmpDat = sr.ReadLine();

                        if (tmpDat.IndexOf(hospCd) >= 0)
                            datVals.Add(tmpDat);
                    }
                }

                ret.Items = datVals.ToArray();
                ret.Result = true;

            }
            catch (Exception ex)
            {
                ret.Result = false;
                ret.Message = "FAX一覧の取得に失敗しました。";
                LogControl.WriteLog(LogType.ERR, "WebGetMailSender", ex.Message);
            }

            return ret;
        }

        [WebMethod(EnableSession = true)]
        public string WebGetSender(string orderNo)
        {
            string ret = "";

            try
            {
                string iniFile = Path.Combine(ConfigurationManager.AppSettings["ContactPath"], orderNo + ".csv");

                if (!File.Exists(iniFile))
                    return ret;

                using (var sr = new StreamReader(iniFile, Encoding.GetEncoding("Shift-JIS")))
                {
                    ret = sr.ReadLine();
                }

                ret = ret.Replace("\"", "");

            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "WebGetMailSender", ex.Message);
            }

            return ret;
        }

        [WebMethod(EnableSession = true)]
        public string WebSetOriginal(int userid, int isEm, int isNew, string value)
        {
            string ret = "";

            try
            {
                //if(isNew == 1)
                //{
                ServiceIF serv = new ServiceIF();

                UserMst userMst = new UserMst();
                serv.GetUser_Serv(userid, out userMst);

                string prefix = "";

                if (isEm == 1)
                    prefix = "★緊" + userMst.CD + "_";
                else
                    prefix = "★" + userMst.CD + "_";

                string[] vals = value.Split(',');

                foreach (var val in vals)
                {
                    string imgPath = Path.Combine(ConfigurationManager.AppSettings["Image00"], val);
                    string newFile = Path.Combine(ConfigurationManager.AppSettings["Image01"], prefix + val);
                    string storageFile = Path.Combine(ConfigurationManager.AppSettings["ImageDir"], "User", userid.ToString(), val);

                    if (!File.Exists(newFile))
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(storageFile)))
                            Directory.CreateDirectory(Path.GetDirectoryName(storageFile));

                        if (File.Exists(storageFile))
                            File.Delete(storageFile);

                        File.Copy(imgPath, storageFile);

                        if (File.Exists(newFile))
                            File.Delete(newFile);

                        File.Move(imgPath, newFile);
                    }

                    if (File.Exists(newFile) && File.Exists(storageFile))
                    {
                        if (!string.IsNullOrEmpty(ret))
                            ret += ",";

                        ret += value;

                    }
                }

                //}
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "WebSetOriginal", e.Message);
                ret = "";
            }

            return ret;
        }

        [WebMethod(EnableSession = true)]
        public string WebGetImage(int status, int orderid, int userid, int isOrigin, string value)
        {
            string url = "";

            try
            {
                url = ConfigurationManager.AppSettings["ImageURL"];

                if (isOrigin == 0)
                    value = "C_" + value;

                string path;
                string defPath;
                defPath = ConfigurationManager.AppSettings["ImageDir"];

                path = Path.Combine(defPath, "User", userid.ToString(), value);
                if(!File.Exists(path))
                {
                    path = Path.Combine(defPath, "Order", orderid.ToString(), value);
                    if(File.Exists(path))
                        url += "/Order/" + orderid;
                }
                else
                {
                    url += "/User/" + userid;
                }

                url += "/" + value;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "WebGetUserList", e.Message);
            }

            return url;
        }

        [WebMethod(EnableSession = true)]
        public bool WebResetImage(int status, int orderid, int userid, int isOrigin, string value)
        {
            bool ret= false;

            string path;
            string defPath;

            try
            {
                defPath = ConfigurationManager.AppSettings["ImageDir"];

                switch (status)
                {
                    case -1:
                    case 0:
                        if (isOrigin == 1)
                        {
                            path = Path.Combine(defPath, "User", userid.ToString(), value);

                            if (File.Exists(path))
                            {
                                string[] files = Directory.GetFiles(ConfigurationManager.AppSettings["Image01"], "★*" + value);
                                if (files.Length != 0)
                                    File.Delete(files[0]);

                                string image = Path.Combine(ConfigurationManager.AppSettings["Image00"], value);
                                if (File.Exists(image))
                                    File.Delete(image);

                                File.Copy(path, image);
                            }
                        }
                        else
                            path = Path.Combine(defPath, "User", userid.ToString(), "C_" + value);

                        if(File.Exists(path))
                            File.Delete(path);
                        break;
                    case 1:
                    case 2:
                        if (isOrigin == 1)
                        {
                            path = Path.Combine(defPath, "Order", orderid.ToString(), value);
                            if (!File.Exists(path))
                                path = Path.Combine(defPath, "User", userid.ToString(), value);

                            if (File.Exists(path))
                            {
                                string[] files = Directory.GetFiles(ConfigurationManager.AppSettings["Image02"], "*" + value);
                                if(files.Length != 0)
                                    File.Delete(files[0]);
                                else
                                {
                                    files = Directory.GetFiles(ConfigurationManager.AppSettings["Image00"], "★*" + value);
                                    if (files.Length != 0)
                                        File.Delete(files[0]);
                                }

                                string image = Path.Combine(ConfigurationManager.AppSettings["Image00"], value);
                                if (File.Exists(image))
                                    File.Delete(image);

                                File.Copy(path, image);
                            }
                        }
                        else
                        {
                            path = Path.Combine(defPath, "User", userid.ToString(), "C_" + value);
                            if (!File.Exists(path))
                                path = Path.Combine(defPath, "Order", orderid.ToString(), "C_" + value);

                            if (File.Exists(path))
                                File.Delete(path);
                        }

                        break;
                }


                ret = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "WebResetImage", e.Message);
            }

            return ret;
        }

        [WebMethod(EnableSession = true)]
        public bool WebSetCutImg(int userid, string value)
        {
            bool ret = false;

            try
            {
                var ipAdd = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                if (ipAdd == "::1")
                    ipAdd = "localhost";

                string imgPath = Path.Combine(ConfigurationManager.AppSettings["CutDir"], ipAdd, "IMG.jpg");

                if (!File.Exists(imgPath))
                    return ret;

                ServiceIF serv = new ServiceIF();

                UserMst userMst = new UserMst();
                serv.GetUser_Serv(userid, out userMst);

                //string[] dat = value.Split('_');

                //string newFile = "";
                //if(dat.Length > 2)
                //    newFile = Path.Combine(ConfigurationManager.AppSettings["ImageDir"], "User", userid.ToString(), "C_" + value.Replace(dat[0] + "_", ""));
                //else
                //    newFile = Path.Combine(ConfigurationManager.AppSettings["ImageDir"], "User", userid.ToString(), "C_" + value);

                string newFile = "";
                newFile = Path.Combine(ConfigurationManager.AppSettings["ImageDir"], "User", userid.ToString(), "C_" + value);

                if (!Directory.Exists(Path.GetDirectoryName(newFile)))
                    Directory.CreateDirectory(newFile);

                if (File.Exists(newFile))
                    File.Delete(newFile);

                File.Move(imgPath, newFile);

                ret = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "WebGetUserList", e.Message);
            }

            return ret;
        }

        [WebMethod(EnableSession = true)]
        public WebPatientList WebGetPatList(int hospid, string patid)
        {
            WebPatientList ret = new WebPatientList();

            try
            {
                ServiceIF serv = new ServiceIF();
                OrderTool_Reception_Serv.Util.Search search = new OrderTool_Reception_Serv.Util.Search();

                search.HospID = hospid;
                search.PatID = patid;

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
        public WebFileList WebGetFileList(int orderid)
        {
            WebFileList ret = new WebFileList();

            try
            {
                ServiceIF serv = new ServiceIF();

                Files[] tmpList = null;

                ResultData inRet = serv.GetFile_Serv(orderid, out tmpList);

                if (inRet.Result)
                {
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
                LogControl.WriteLog(LogType.ERR, "WebGetFileList", e.Message);
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
                OrderTool_Reception_Serv.Util.Order order = new OrderTool_Reception_Serv.Util.Order();
                string oriFile = "";
                string thumbFile = "";

                int oldStatus = Convert.ToInt32(values[0]);

                List<string> pastIds = new List<string>();

                order.Status = Convert.ToInt32(values[values.Length -1]);
                order.HospID = Convert.ToInt32(values[1]);
                order.OrderID = Convert.ToInt32(values[2]);
                order.OrderNo = values[3];
                order.PatID = values[4].Trim();
                order.PatName = values[5].Trim();
                order.PatName_H = values[6].Trim();
                order.Sex = Convert.ToInt32(values[7]);
                order.PatAge = Convert.ToInt32(values[8]);
                order.BirthDay = values[9].Replace("/", "");
                order.Modality = values[10];
                order.Date = values[11].Replace("/", "");
                order.Time = values[12].Replace(":", "");
                if (string.IsNullOrEmpty(order.Time))
                    order.Time = "120000";

                order.BodyPart = values[13];
                order.Type = values[14];
                order.IsVisit = values[15];
                order.Department = values[16];
                order.Doctor = values[17];
                order.IsEmergency = Convert.ToInt32(values[18]);
                order.IsMail = Convert.ToInt32(values[19]);
                order.Comment = values[20];
                order.Contact = values[21];
                order.Recept = values[22];
                oriFile = values[23];
                thumbFile = values[24];

                int key = 0;

                serv.SetPatient_Serv(order, out key);
                order.Key = key;

                if(string.IsNullOrEmpty(order.OrderNo))
                {
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
                    order.OrderNo = hospMst.CD + DateTime.Now.ToString("yyMMddHHmmssf");
                }

                if(oldStatus < 2 && order.OrderID > 0)
                {
                    OrderTool_Reception_Serv.Util.Order[] orders;
                    OrderTool_Reception_Serv.Util.Search search;
                    search = new OrderTool_Reception_Serv.Util.Search();
                    search.OrderID = order.OrderID;

                    serv.GetOrder_Serv(search, out orders);

                    if(orders.Length > 0)
                    {
                        if (orders[0].Status > oldStatus)
                        {
                            ret.Result = false;
                            ret.Message = "この依頼は他のユーザに更新されています。\n一覧に戻り、確認してください。";
                            return ret;
                        }
                    }
                }

                WriteTXT(order.HospID, order.OrderID.ToString(), order.OrderNo, values[25], values[26], values[27], values[28]);

                int tmpOdr = 0;
                ResultData inRet = serv.SetOrder_Serv(order, out tmpOdr);

                if (inRet.Result)
                {
                    ret.Message = tmpOdr.ToString();

                    LogControl.WriteLog(LogType.ORE, "SetPreOrder", "【オーダー登録】 施設ID：" + order.HospID + "　患者ID：" + order.PatID + " ﾓﾀﾞﾘﾃｨ：" + order.Modality + " 検査日：" + order.Date);

                    OrderTool_Reception_Serv.Util.Order[] orders;
                    OrderTool_Reception_Serv.Util.Search search;
                    search = new OrderTool_Reception_Serv.Util.Search();
                    search.OrderNo = order.OrderNo;

                    serv.GetOrder_Serv(search, out orders);

                    order = orders[0];

                    string thumpath;
                    thumpath = Path.Combine(ConfigurationManager.AppSettings["ImageDir"], "Order", orders[0].OrderID.ToString(), "C_" + thumbFile);



                    if (!String.IsNullOrEmpty(oriFile))
                    {
                        Files[] files = null;

                        serv.GetFile_Serv(orders[0].OrderID, out files);
                        serv.DelFile_Serv(orders[0].OrderID);

                        if (files == null || files.Length == 0)
                        {
                            Files file = new Files();
                            file.OrderID = orders[0].OrderID;
                            file.IsOrigin = 1;
                            file.Name = oriFile;
                            file.Seq = 0;
                            serv.SetFile_Serv(file);

                            if (!String.IsNullOrEmpty(thumbFile))
                            {
                                file = new Files();
                                file.OrderID = orders[0].OrderID;
                                file.IsOrigin = 0;
                                file.Name = "C_" + thumbFile;
                                file.Seq = 1;
                                serv.SetFile_Serv(file);
                            }

                            if (Convert.ToInt32(values[values.Length -1]) >= 1)
                            {
                                string[] orifiles = oriFile.Split(',');

                                foreach(var ori in orifiles)
                                {
                                    if (string.IsNullOrEmpty(ori))
                                        continue;

                                    string oripath;
                                    oripath = Path.Combine(ConfigurationManager.AppSettings["ImageDir"], "Order", orders[0].OrderID.ToString(), ori);
                                    string srcpath = Path.Combine(ConfigurationManager.AppSettings["ImageDir"], "User", Convert.ToInt32(values[values.Length -2]).ToString(), ori);
                                    if (!Directory.Exists(Path.GetDirectoryName(oripath)))
                                        Directory.CreateDirectory(Path.GetDirectoryName(oripath));

                                    if (File.Exists(oripath))
                                        File.Delete(oripath);
                                    File.Move(srcpath, oripath);
                                }

                                if (!String.IsNullOrEmpty(thumbFile))
                                {
                                    string srcpath = Path.Combine(ConfigurationManager.AppSettings["ImageDir"], "User", Convert.ToInt32(values[values.Length -2]).ToString(), "C_" + thumbFile);
                                    if (!Directory.Exists(Path.GetDirectoryName(thumpath)))
                                        Directory.CreateDirectory(Path.GetDirectoryName(thumpath));

                                    if (File.Exists(thumpath))
                                        File.Delete(thumpath);
                                    File.Move(srcpath, thumpath);
                                }
                            }
                        }
                        else
                        {
                            files[0].Name = oriFile;
                            files[0].FileID = 0;
                            serv.SetFile_Serv(files[0]);

                            if (!String.IsNullOrEmpty(thumbFile))
                            {
                                bool isThumb = false;
                                foreach(var file in files)
                                {
                                    if(file.IsOrigin == 0)
                                    {
                                        files[1].FileID = 0;
                                        files[1].Name = "C_" + thumbFile;
                                        serv.SetFile_Serv(files[1]);
                                        isThumb = true;
                                    }
                                }
                                if(!isThumb)
                                {
                                    Files file = new Files();
                                    file.OrderID = orders[0].OrderID;
                                    file.IsOrigin = 0;
                                    file.Name = "C_" + thumbFile;
                                    file.Seq = 1;
                                    serv.SetFile_Serv(file);
                                }
                            }

                            if (Convert.ToInt32(values[values.Length -1]) >= 1)
                            {
                                string[] orifiles = oriFile.Split(',');

                                foreach (var ori in orifiles)
                                {
                                    if (string.IsNullOrEmpty(ori))
                                        continue;

                                    string oripath;
                                    oripath = Path.Combine(ConfigurationManager.AppSettings["ImageDir"], "Order", orders[0].OrderID.ToString(), ori);
                                    string srcpath = Path.Combine(ConfigurationManager.AppSettings["ImageDir"], "User", Convert.ToInt32(values[values.Length -2]).ToString(), ori);
                                    if (File.Exists(srcpath))
                                    {
                                        if (!Directory.Exists(Path.GetDirectoryName(oripath)))
                                            Directory.CreateDirectory(Path.GetDirectoryName(oripath));

                                        if (File.Exists(oripath))
                                            File.Delete(oripath);

                                        File.Move(srcpath, oripath);
                                    }
                                }

                                if (!String.IsNullOrEmpty(thumbFile))
                                {
                                    string srcpath = Path.Combine(ConfigurationManager.AppSettings["ImageDir"], "User", Convert.ToInt32(values[values.Length -2]).ToString(), "C_" + thumbFile);
                                    if (File.Exists(srcpath))
                                    {
                                        if (!Directory.Exists(Path.GetDirectoryName(thumpath)))
                                            Directory.CreateDirectory(Path.GetDirectoryName(thumpath));

                                        if (File.Exists(thumpath))
                                            File.Delete(thumpath);

                                        File.Move(srcpath, thumpath);
                                    }
                                }
                            }
                        }
                    }

                    if (Convert.ToInt32(values[values.Length -1]) == 1)
                    {
                        string[] orifiles = oriFile.Split(',');

                        foreach (var ori in orifiles)
                        {
                            if (string.IsNullOrEmpty(ori))
                                continue;

                            string[] dfiles = Directory.GetFiles(ConfigurationManager.AppSettings["Image01"], "*" + ori);
                            if (dfiles.Length != 0)
                            {

                                UserMst userMst = new UserMst();
                                serv.GetUser_Serv(Convert.ToInt32(values[values.Length -2]), out userMst);

                                string prefix = "";

                                if (order.IsEmergency == 1)
                                    prefix = "緊" + userMst.CD;
                                else
                                    prefix = userMst.CD;

                                var newfile = Path.Combine(ConfigurationManager.AppSettings["Image02"], prefix + "_" + ori);

                                if (File.Exists(newfile))
                                    File.Delete(newfile);
                                File.Move(dfiles[0], newfile);
                            }

                        }

                        if (order.PreID > 0)
                        {
                            string[] dfiles = Directory.GetFiles(ConfigurationManager.AppSettings["Image01"], "*" + order.PreID + "*");
                            if (dfiles.Length != 0)
                            {

                                UserMst userMst = new UserMst();
                                serv.GetUser_Serv(Convert.ToInt32(values[values.Length -2]), out userMst);

                                string prefix = "";

                                if (order.IsEmergency == 1)
                                    prefix = "緊" + userMst.CD;
                                else
                                    prefix = userMst.CD;

                                var newfile = Path.Combine(ConfigurationManager.AppSettings["Image02"], prefix + "_" + Path.GetFileName(dfiles[0]));

                                if (File.Exists(newfile))
                                    File.Delete(newfile);
                                File.Move(dfiles[0], newfile);
                            }
                        }

                    }
                    else if (Convert.ToInt32(values[values.Length -1]) == 2 && oldStatus != 2)
                    {
                        WriteCSV(order, thumpath, 1);

                        string[] orifiles = oriFile.Split(',');

                        foreach (var ori in orifiles)
                        {
                            if (string.IsNullOrEmpty(ori))
                                continue;

                            string[] dfiles = Directory.GetFiles(ConfigurationManager.AppSettings["Image02"], "*" + ori);
                            if (!string.IsNullOrEmpty(oriFile) && dfiles.Length != 0)
                                File.Delete(dfiles[0]);
                            dfiles = Directory.GetFiles(ConfigurationManager.AppSettings["Image01"], "*" + ori);
                            if (!string.IsNullOrEmpty(oriFile) && dfiles.Length != 0)
                                File.Delete(dfiles[0]);
                        }

                        if (order.PreID > 0)
                        {
                            string[] dfiles = Directory.GetFiles(ConfigurationManager.AppSettings["Image01"], "*_" + order.PreID + ".txt");
                            if (dfiles.Length != 0)
                                File.Delete(dfiles[0]);
                            dfiles = Directory.GetFiles(ConfigurationManager.AppSettings["Image02"], "*_" + order.PreID + ".txt");
                            if (dfiles.Length != 0)
                                File.Delete(dfiles[0]);
                        }

                    }
                    else if (Convert.ToInt32(values[values.Length -1]) == 2 && oldStatus == 2)
                    {
                        WriteCSV(order, thumpath, 2);

                        string[] orifiles = oriFile.Split(',');

                        foreach (var ori in orifiles)
                        {
                            string[] dfiles = Directory.GetFiles(ConfigurationManager.AppSettings["Image02"], "*" + ori);
                            if (!string.IsNullOrEmpty(oriFile) && dfiles.Length != 0)
                                File.Delete(dfiles[0]);
                            dfiles = Directory.GetFiles(ConfigurationManager.AppSettings["Image01"], "*" + ori);
                            if (!string.IsNullOrEmpty(oriFile) && dfiles.Length != 0)
                                File.Delete(dfiles[0]);
                        }

                        if (order.PreID > 0)
                        {
                            string[] dfiles = Directory.GetFiles(ConfigurationManager.AppSettings["Image01"], "*_" + order.PreID + ".txt");
                            if (dfiles.Length != 0)
                                File.Delete(dfiles[0]);
                            dfiles = Directory.GetFiles(ConfigurationManager.AppSettings["Image02"], "*_" + order.PreID + ".txt");
                            if (dfiles.Length != 0)
                                File.Delete(dfiles[0]);
                        }

                    }
                    else if (Convert.ToInt32(values[values.Length -1]) == 3 && oldStatus == 2)
                        WriteCSV(order, thumpath, 3);
                    else if (Convert.ToInt32(values[values.Length -1]) == 4 && oldStatus == 2)
                        WriteCSV(order, thumpath, 3);

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
                LogControl.WriteLog(LogType.ERR, "WebGetList", e.StackTrace);
            }

            return ret;
        }

        private void WriteCSV(OrderTool_Reception_Serv.Util.Order order, string jpgFile, int status)
        {
            string mstPath = "";
            string outPath = "";
            string outImgPath = "";
            string outTriggar = "";
            string outPath_back = "";
            string orderPath_back = "";

            DateTime now = DateTime.Now;

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

            HospitalConfig hospConf = null;
            serv.GetHospConfig_Serv(hospMst.HospID, out hospConf);

            mstPath = System.IO.Path.Combine(ConfigurationManager.AppSettings["MastDir"], hospMst.CD + ".csv");

            var defPath = "";
            var bacPath = "";

            if (!File.Exists(mstPath))
                mstPath = System.IO.Path.Combine(ConfigurationManager.AppSettings["MastDir"], "MST.csv");

            foreach(var conf in hospConf.Conf)
            {
                if(conf.Key == "OutDir")
                {
                    defPath = conf.Value;
                }else if(conf.Key == "OutBackPath")
                {
                    bacPath = conf.Value;
                }
            }

            outPath = System.IO.Path.Combine(defPath, order.OrderNo + "_" + now.ToString("yyyyMMddHHmmssfff") + ".csv");
            if(!string.IsNullOrEmpty(bacPath))
                orderPath_back = System.IO.Path.Combine(bacPath, order.OrderNo + "_" + now.ToString("yyyyMMddHHmmssfff") + ".csv");
            outImgPath = System.IO.Path.Combine(defPath, order.OrderNo + "_" + now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(jpgFile));
            outTriggar = System.IO.Path.Combine(defPath, order.OrderNo + "_" + now.ToString("yyyyMMddHHmmssfff") + ".txt");

            string backDef = System.IO.Path.Combine(ConfigurationManager.AppSettings["BackDir"], hospMst.CD);
            backDef = System.IO.Path.Combine(backDef, DateTime.Now.ToString("yyyyMMdd"));

            if (!System.IO.Directory.Exists(backDef))
                System.IO.Directory.CreateDirectory(backDef);

            outPath_back = System.IO.Path.Combine(backDef, "CSV");
            if (!System.IO.Directory.Exists(outPath_back))
                System.IO.Directory.CreateDirectory(outPath_back);

            var csvValue = "";

            using (var sr = new System.IO.StreamReader(mstPath, System.Text.Encoding.GetEncoding("Shift-JIS")))
            {
                csvValue = sr.ReadLine();
            }

            csvValue = csvValue.Replace("@00", status.ToString());
            csvValue = csvValue.Replace("@01", order.OrderNo);
            csvValue = csvValue.Replace("@02", hospMst.CD + order.PatID);
            csvValue = csvValue.Replace("@03", order.PatName);
            csvValue = csvValue.Replace("@04", order.PatName_H);
            switch (order.Sex)
            {
                case 0:
                    csvValue = csvValue.Replace("@05", "O");
                    break;
                case 1:
                    csvValue = csvValue.Replace("@05", "M");
                    break;
                case 2:
                    csvValue = csvValue.Replace("@05", "F");
                    break;
            }

            csvValue = csvValue.Replace("@06", order.BirthDay);
            csvValue = csvValue.Replace("@07", order.PatAge.ToString());
            csvValue = csvValue.Replace("@08", order.Date);

            string time = "";

            if(!string.IsNullOrEmpty(order.Time))
            {
                time = order.Time.Replace("：", ":").Replace(":", "");
                if (time.Length < 6)
                    time = time.PadLeft(4, '0').PadRight(6, '0');
                else if (time.Length > 6)
                    time = time.Substring(0, 6);
            }

            csvValue = csvValue.Replace("@09", time);
            csvValue = csvValue.Replace("@10", order.Modality);
            csvValue = csvValue.Replace("@11", order.IsEmergency.ToString());
            csvValue = csvValue.Replace("@12", order.Comment.Replace("\r\n", " ").Replace("\n", " "));
            csvValue = csvValue.Replace("@13", order.Contact.Replace("\r\n", " ").Replace("\n", " "));
            csvValue = csvValue.Replace("@14", order.Recept.Replace("\r\n", " ").Replace("\n", " "));
            csvValue = csvValue.Replace("@15", order.IsMail.ToString());
            csvValue = csvValue.Replace("@16", hospMst.CD);
            csvValue = csvValue.Replace("@17", hospMst.Name);
            csvValue = csvValue.Replace("@18", order.BodyPart).Replace("\r\n", " ").Replace("\n", " ");
            csvValue = csvValue.Replace("@19", order.Type);
            csvValue = csvValue.Replace("@20", order.Department);
            csvValue = csvValue.Replace("@21", order.Doctor);
            csvValue = csvValue.Replace("@22", order.IsVisit);
            csvValue = csvValue.Replace("@23", order.IsMail.ToString());

            csvValue = csvValue.Replace("@24", Microsoft.VisualBasic.Strings.StrConv(order.PatName_H, Microsoft.VisualBasic.VbStrConv.Wide, 0x411));

            csvValue = csvValue.Replace("@25", order.ImgCnt.ToString());
            csvValue = csvValue.Replace("@26", order.PastCnt.ToString());


            if (File.Exists(jpgFile))
                File.Copy(jpgFile, outImgPath);

            using (var sw = new System.IO.StreamWriter(outPath, false, Encoding.GetEncoding("Shift-JIS")))
            {
                sw.WriteLine(csvValue);
            }

            if (!string.IsNullOrEmpty(orderPath_back))
            {
                using (var sw = new System.IO.StreamWriter(orderPath_back, false, Encoding.GetEncoding("Shift-JIS")))
                {
                    sw.WriteLine(csvValue);
                }
            }

            using (var sw = new System.IO.StreamWriter(outTriggar))
            {
            }

            using (var sw = new System.IO.StreamWriter(System.IO.Path.Combine(outPath_back, Path.GetFileName(outPath)), false, System.Text.Encoding.GetEncoding("Shift-JIS")))
            {
                sw.WriteLine(csvValue);
            }

            if(status == 1)
            {
                string checkDir = Path.Combine(ConfigurationManager.AppSettings["CheckCutDir"], Path.GetFileName(outPath));
                using (var sw = new System.IO.StreamWriter(checkDir, false, Encoding.GetEncoding("Shift-JIS")))
                {
                    sw.WriteLine(csvValue);
                }
            }
        }

        private void WriteTXT(int hospID, string orderid, string orderno, string isOwn, string isIntro, string name, string fax)
        {
            string outPath = "";
            string oldPath = "";

            if(!string.IsNullOrEmpty(orderno))
            {
                outPath = Path.Combine(ConfigurationManager.AppSettings["ContactPath"], orderno + ".csv");
                oldPath = Path.Combine(ConfigurationManager.AppSettings["ContactPath"], orderid + ".csv");
            }
            else
            {
                outPath = Path.Combine(ConfigurationManager.AppSettings["ContactPath"], orderid + ".csv");
            }

            if (!string.IsNullOrEmpty(oldPath) && File.Exists(oldPath))
                File.Delete(oldPath);

            //if (string.IsNullOrEmpty(isOwn) && string.IsNullOrEmpty(isIntro))
            //    return;

            string ownPath = ConfigurationManager.AppSettings["OwnFile"];
            string writeVal = "";

            string ownName = "";
            string ownNo = "";

            ServiceIF serv = new ServiceIF();

            HospMst[] hospList;
            HospMst hospMst = new HospMst();
            serv.GetHospList_Serv(out hospList);

            foreach (var hosp in hospList)
            {
                if (hospID == hosp.HospID)
                {
                    hospMst = hosp;
                    break;
                }
            }

            using (var sr = new StreamReader(ownPath, Encoding.GetEncoding("Shift-JIS")))
            {
                while (!sr.EndOfStream)
                {
                    string tmpDat = "";

                    tmpDat = sr.ReadLine();

                    if (tmpDat.IndexOf(hospMst.CD) >= 0)
                    {
                        string[] tmpList = tmpDat.Split(',');
                        ownName = tmpList[1];
                        ownNo = tmpList[2];
                        break;
                    }
                }
            }

            writeVal += "\"" + isOwn + "\"";
            writeVal += ",\"" + isIntro + "\"";
            writeVal += ",\"" + ownName + "\"";

            if (!string.IsNullOrEmpty(isIntro))
                writeVal += ",\"" + name + "\"";
            else
                writeVal += ",\"\"";

            if (isOwn == "0" || isOwn == "2")
                writeVal += ",\"" + ownNo + "\"";
            else
                writeVal += ",\"\"";

            if (isIntro == "0")
                writeVal += ",\"" + fax + "\"";
            else
                writeVal += ",\"\"";

            using (var sw = new System.IO.StreamWriter(outPath, false, Encoding.GetEncoding("Shift-JIS")))
            {
                sw.WriteLine(writeVal);
            }


            if (isIntro == "0")
            {
                string introFaxPath = ConfigurationManager.AppSettings["FAXFile_Intro"];
                bool isNew = false;

                using (var sr = new StreamReader(introFaxPath, Encoding.GetEncoding("Shift-JIS")))
                {
                    while (!sr.EndOfStream)
                    {
                        string tmpDat = "";

                        tmpDat = sr.ReadLine();

                        if (tmpDat.IndexOf(fax) >= 0)
                        {
                            isNew = true;
                            break;
                        }
                    }
                }

                if(!isNew)
                {
                    string newTxt = hospMst.CD + "," + name + "," + fax;
                    using (var sw = new StreamWriter(introFaxPath, true, Encoding.GetEncoding("Shift-JIS")))
                    {
                        sw.WriteLine(newTxt);
                    }
                }
            }
            if (isIntro == "1")
            {
                string introMailPath = ConfigurationManager.AppSettings["MailFile_Intro"];
                bool isNew = false;

                using (var sr = new StreamReader(introMailPath, Encoding.GetEncoding("Shift-JIS")))
                {
                    while (!sr.EndOfStream)
                    {
                        string tmpDat = "";

                        tmpDat = sr.ReadLine();

                        if (tmpDat.IndexOf(name) >= 0)
                        {
                            isNew = true;
                            break;
                        }
                    }
                }

                if (!isNew)
                {
                    string newTxt = hospMst.CD + "," + name;
                    using (var sw = new StreamWriter(introMailPath, true, Encoding.GetEncoding("Shift-JIS")))
                    {
                        sw.WriteLine(newTxt);
                    }
                }
            }
        }

    }
    // アシスタントクラス群
    public class WebResult
    {
        public bool Result { get; set; }
        public string Message { get; set; }
    }
    public class WebHosp : WebResult
    {
        public HospMst Items { get; set; }
    }
    public class WebHospList : WebResult
    {
        public HospMst[] Items { get; set; }
    }

    public class WebHospConf : WebResult
    {
        public HospitalConfig Items { get; set; }
    }

    public class WebHospTempList : WebResult
    {
        public HospitalTemplate[] Items { get; set; }
    }

    public class WebUserList : WebResult
    {
        public UserMst[] Items { get; set; }
    }

    public class WebOrderList : WebResult
    {
        public OrderTool_Reception_Serv.Util.Order[] Items { get; set; }
    }

    public class WebPatientList : WebResult
    {
        public Patient[] Items { get; set; }
    }

    public class WebFileList : WebResult
    {
        public Files[] Items { get; set; }
    }

    public class WebFaxList : WebResult
    {
        public string[] Items { get; set; }
    }

}
