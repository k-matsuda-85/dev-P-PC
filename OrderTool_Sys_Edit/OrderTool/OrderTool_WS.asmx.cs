using System;
using System.Web.Services;

using LogController;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Serialization;
using OrderTool.cls;
using OrderTool.QRService;
using System.IO;
using System.Text;
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
        public WebResult DelPreOrder(string orderNo)
        {
            WebResult ret = new WebResult();

            try
            {
                string orderPath = "";
                string imagePath = "";
                orderPath = ConfigurationManager.AppSettings["OrderDir"];
                orderPath = Path.Combine(orderPath, orderNo.Trim() + ".csv");

                imagePath = ConfigurationManager.AppSettings["ImagePath"];
                imagePath = Path.Combine(imagePath, orderNo.Trim());

                PreOrder tmp = ReadCSV(orderPath);

                tmp.Status = 3;

                WriteCSV(tmp, "", "", "", tmp.ImgCnt.ToString(), "");

                if (File.Exists(orderPath))
                    File.Delete(orderPath);

                if(Directory.Exists(imagePath))
                {
                    string[] files = Directory.GetFiles(imagePath);

                    foreach (var file in files)
                        File.Delete(file);
                }

                LogControl.WriteLog(LogType.ORE, "DelPreOrder", "【オーダー削除】 オーダーNo：" + orderNo);

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
        public WebResult WebDelScanImage(string file, string orderno, string patid, string date, string mod)
        {
            WebResult ret = new WebResult();

            try
            {
                string imageDir = ConfigurationManager.AppSettings["ImagePath"];

                if (string.IsNullOrEmpty(orderno))
                    imageDir = Path.Combine(imageDir, patid + "_" + date + "_" + mod);
                else
                    imageDir = Path.Combine(imageDir, orderno);

                string[] files = null;

                if(string.IsNullOrEmpty(file))
                    files = Directory.GetFiles(imageDir);
                else
                    files = Directory.GetFiles(imageDir, file);

                foreach (var fi in files)
                    File.Delete(fi);

            }
            catch (Exception e)
            {
                ret.Result = false;
                ret.Message = "スキャン画像削除中にエラーが発生しました。\n再度削除しなおしてください。";
                LogControl.WriteLog(LogType.ERR, "WebDelScanImage", e.Message);
                LogControl.WriteLog(LogType.ERR, "WebDelScanImage", e.StackTrace);
            }

            return ret;
        }

        [WebMethod(EnableSession = true)]
        public WebImageList WebGetScanImage(string orderno, string patid, string date, string mod)
        {
            WebImageList ret = new WebImageList();

            try
            {
                string scanDir = ConfigurationManager.AppSettings["ScanPath"];
                string imageDir = ConfigurationManager.AppSettings["ImagePath"];

                string imageURL = ConfigurationManager.AppSettings["ImageURL"];

                if(string.IsNullOrEmpty(orderno))
                {
                    imageURL = imageURL + patid + "_" + date + "_" + mod;
                    imageDir = Path.Combine(imageDir, patid + "_" + date + "_" + mod);
                }
                else
                {
                    imageURL = imageURL + orderno.Trim();
                    imageDir = Path.Combine(imageDir, orderno.Trim());
                }

                if (!Directory.Exists(imageDir))
                    Directory.CreateDirectory(imageDir);

                string[] files = Directory.GetFiles(scanDir);

                foreach(var fi in files)
                {
                    System.Drawing.Image tmp = null;
                    try
                    {
                        tmp = System.Drawing.Image.FromFile(fi);
                    }
                    catch
                    {
                        continue;
                    }
                    finally
                    {
                        if (tmp != null)
                            tmp.Dispose();
                    }

                    string ext = Path.GetExtension(fi);

                    string mvFile = Path.Combine(imageDir, DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext);
                    File.Move(fi, mvFile);
                }

                files = Directory.GetFiles(imageDir);

                List<ImageURL> tmpList = new List<ImageURL>();

                foreach (var fi in files)
                {
                    ImageURL tmp = new ImageURL();

                    tmp.Name = Path.GetFileName(fi);
                    tmp.URL = imageURL + "/" + tmp.Name;

                    tmpList.Add(tmp);
                }

                ret.Items = tmpList.ToArray();
                ret.Result = true;
            }
            catch (Exception e)
            {
                ret.Result = false;
                ret.Message = "スキャン画像取込中にエラーが発生しました。\n再度取り込みなおしてください。";
                LogControl.WriteLog(LogType.ERR, "WebGetScanImage", e.Message);
                LogControl.WriteLog(LogType.ERR, "WebGetScanImage", e.StackTrace);
            }

            return ret;
        }

        [WebMethod(EnableSession = true)]
        public WebImageList WebGetScanImage_First(string orderno, string patid, string date, string mod)
        {
            WebImageList ret = new WebImageList();

            try
            {
                string scanDir = ConfigurationManager.AppSettings["ScanPath"];
                string imageDir = ConfigurationManager.AppSettings["ImagePath"];

                string imageURL = ConfigurationManager.AppSettings["ImageURL"];

                if (string.IsNullOrEmpty(orderno))
                {
                    imageURL = imageURL + patid + "_" + date + "_" + mod;
                    imageDir = Path.Combine(imageDir, patid + "_" + date + "_" + mod);
                }
                else
                {
                    imageURL = imageURL + orderno.Trim();
                    imageDir = Path.Combine(imageDir, orderno.Trim());
                }

                List<ImageURL> tmpList = new List<ImageURL>();

                if (Directory.Exists(imageDir))
                {
                    //string[] files = Directory.GetFiles(scanDir);

                    //foreach (var fi in files)
                    //{
                    //    System.Drawing.Image tmp = null;
                    //    try
                    //    {
                    //        tmp = System.Drawing.Image.FromFile(fi);
                    //    }
                    //    catch
                    //    {
                    //        continue;
                    //    }
                    //    finally
                    //    {
                    //        if (tmp != null)
                    //            tmp.Dispose();
                    //    }

                    //    string ext = Path.GetExtension(fi);

                    //    string mvFile = Path.Combine(imageDir, DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext);
                    //    File.Move(fi, mvFile);
                    //}

                    string[] files = Directory.GetFiles(imageDir);


                    foreach (var fi in files)
                    {
                        ImageURL tmp = new ImageURL();

                        tmp.Name = Path.GetFileName(fi);
                        tmp.URL = imageURL + "/" + tmp.Name;

                        tmpList.Add(tmp);
                    }
                }

                ret.Items = tmpList.ToArray();
                ret.Result = true;
            }
            catch (Exception e)
            {
                ret.Result = false;
                ret.Message = "スキャン画像取込中にエラーが発生しました。\n再度取り込みなおしてください。";
                LogControl.WriteLog(LogType.ERR, "WebGetScanImage", e.Message);
                LogControl.WriteLog(LogType.ERR, "WebGetScanImage", e.StackTrace);
            }

            return ret;
        }


        [WebMethod(EnableSession = true)]
        public WebPreOrderList WebGetPreList(string patid, string date, string mod, int isCnt)
        {
            WebPreOrderList ret = new WebPreOrderList();

            try
            {
                DicomQRItem[] studyDatas = null;

                List<PreOrder> tmpList = new List<PreOrder>();
                string orderDir = ConfigurationManager.AppSettings["OrderDir"];
                C_Find.Setting();

                studyDatas = C_Find.C_FindFromStudy(patid, date, "", mod, "");

                for (int i = 0; i < studyDatas.Length; i++)
                {
                    StudyData tmpStudy = new StudyData();
                    tmpStudy.SetStudy(studyDatas[i].Tags);

                    DicomQRItem[] tmpDat = C_Find.C_FindFromSeries(tmpStudy.UID);
                    tmpStudy.SetSeries(tmpDat);
                    if(isCnt == 1)
                    {
                        foreach (var dat in tmpDat)
                            tmpStudy.SetImages(
                                C_Find.C_FindFromImages(
                                    tmpStudy.UID
                                  , dat.Tags[(uint)Tags.SERIES_INSTANCE_UID]
                                )
                            );
                    }

                    if (tmpStudy.Modality.IndexOf(mod) < 0)
                        continue;

                    PreOrder tmp = new PreOrder();

                    if (tmpStudy.Tags.ContainsKey((uint)Tags.ACCSESSIONNO))
                    {
                        tmp.OrderNo = tmpStudy.Tags[(uint)Tags.ACCSESSIONNO].Trim();
                    }
                    else
                    {
                        tmp.OrderNo = tmpStudy.Tags[(uint)Tags.PATIENTID].Trim() + tmpStudy.Tags[(uint)Tags.STUDYDATE].Trim().Substring(2, 6) + tmpStudy.Modality;
                    }

                    if (File.Exists(Path.Combine(orderDir, tmp.OrderNo + ".csv")))
                    {
                        PreOrder past = ReadCSV(Path.Combine(orderDir, tmp.OrderNo + ".csv"));
                        tmp = past;
                    }
                    else
                        tmp.Status = 0;

                    tmp.PatID = tmpStudy.Tags[(uint)Tags.PATIENTID];
                    tmp.PatName = tmpStudy.Tags[(uint)Tags.PATIENTNAME];


                    if (tmpStudy.Tags.ContainsKey((uint)Tags.SEX))
                    {
                        string dat = tmpStudy.Tags[(uint)Tags.SEX];
                        switch (dat.ToLower().Trim())
                        {
                            case "m":
                            case "male":
                                tmp.Sex = 1;
                                break;
                            case "f":
                            case "femail":
                                tmp.Sex = 2;
                                break;
                            default:
                                tmp.Sex = 0;
                                break;
                        }
                    }
                    else
                    {
                        tmp.Sex = 2;
                    }

                    if (tmpStudy.Tags.ContainsKey((uint)Tags.BIRTHDAY))
                        tmp.BirthDay = tmpStudy.Tags[(uint)Tags.BIRTHDAY];

                    tmp.Date = tmpStudy.Tags[(uint)Tags.STUDYDATE];
                    tmp.Time = tmpStudy.Tags[(uint)Tags.STUDYTIME];

                    if (tmpStudy.Tags.ContainsKey((uint)Tags.BODYPART))
                        tmp.BodyPart = tmpStudy.Tags[(uint)Tags.BODYPART];
                    if (tmpStudy.Tags.ContainsKey((uint)Tags.DESCRIPTION))
                        tmp.Desc = tmpStudy.Tags[(uint)Tags.DESCRIPTION];
                    tmp.Modality = tmpStudy.Modality;
                    tmp.StudyInstanceUID = tmpStudy.UID;
                    tmp.ImgCnt = tmpStudy.Cnt;
                    tmpList.Add(tmp);
                }

                tmpList.Sort((a, b) => { return -((a.Date + a.Time).CompareTo((b.Date + b.Time))); });

                ret.Items = tmpList.ToArray();
                ret.Result = true;
                C_Find.Client.Close();
                C_Find.Client = new DicomQRClient();
            }
            catch (Exception e)
            {
                ret.Result = false;
                ret.Message = "処理中に障害が発生いたしました。\nシステム管理者にお問い合わせください。";
                LogControl.WriteLog(LogType.ERR, "WebGetPreList", e.Message);
                LogControl.WriteLog(LogType.ERR, "WebGetPreList", e.StackTrace);
                C_Find.Client.Close();
                C_Find.Client = new DicomQRClient();
            }

            return ret;
        }

        [WebMethod(EnableSession = true)]
        public WebPreOrder WebGetPreOrder(string orderno, string uid)
        {
            WebPreOrder ret = new WebPreOrder();

            try
            {
                DicomQRItem[] studyDatas = null;

                string orderDir = ConfigurationManager.AppSettings["OrderDir"];
                C_Find.Setting();

                studyDatas = C_Find.C_FindFromStudy("", "", "", "", uid);

                PreOrder tmp = new PreOrder();

                for (int i = 0; i < studyDatas.Length; i++)
                {
                    StudyData tmpStudy = new StudyData();
                    tmpStudy.SetStudy(studyDatas[i].Tags);

                    DicomQRItem[] tmpDat = C_Find.C_FindFromSeries(tmpStudy.UID);
                    tmpStudy.SetSeries(tmpDat);


                    if (tmpStudy.Tags.ContainsKey((uint)Tags.ACCSESSIONNO))
                    {
                        tmp.OrderNo = tmpStudy.Tags[(uint)Tags.ACCSESSIONNO].Trim();
                    }
                    else
                    {
                        tmp.OrderNo = tmpStudy.Tags[(uint)Tags.PATIENTID].Trim() + tmpStudy.Tags[(uint)Tags.STUDYDATE].Trim().Substring(2, 6) + tmpStudy.Modality;
                    }

                    if (File.Exists(Path.Combine(orderDir, tmp.OrderNo + ".csv")))
                    {
                        PreOrder past = ReadCSV(Path.Combine(orderDir, tmp.OrderNo + ".csv"));

                        tmp = past;
                        tmp.Status = 2;
                        tmp.StudyInstanceUID = tmpStudy.UID;
                        break;
                    }
                    else
                    {
                        foreach (var dat in tmpDat)
                            tmpStudy.SetImages(
                                C_Find.C_FindFromImages(
                                    tmpStudy.UID
                                  , dat.Tags[(uint)Tags.SERIES_INSTANCE_UID]
                                )
                            );

                        tmp.Status = 0;
                    }


                    tmp.PatID = tmpStudy.Tags[(uint)Tags.PATIENTID].Trim();

                    var nameSep = ConfigurationManager.AppSettings["NameSep"];
                    if (!string.IsNullOrEmpty(nameSep))
                    {
                        var vals = tmpStudy.Tags[(uint)Tags.PATIENTNAME].Trim().Split(nameSep[0]);
                        int nix = -1;
                        int kix = -1;

                        if (int.TryParse(ConfigurationManager.AppSettings["NameSep_Kanji"], out nix))
                            tmp.PatName = vals[nix];

                        if (int.TryParse(ConfigurationManager.AppSettings["NameSep_Kana"], out kix))
                            tmp.PatName_H = vals[kix];
                        else
                            tmp.PatName_H = tmp.PatName;
                    }
                    else
                    {
                        tmp.PatName = tmpStudy.Tags[(uint)Tags.PATIENTNAME].Trim();
                        tmp.PatName_H = tmpStudy.Tags[(uint)Tags.PATIENTNAME].Trim();
                    }

                    if (tmpStudy.Tags.ContainsKey((uint)Tags.SEX))
                    {
                        string dat = tmpStudy.Tags[(uint)Tags.SEX];
                        switch (dat.ToLower().Trim())
                        {
                            case "m":
                            case "male":
                                tmp.Sex = 1;
                                break;
                            case "f":
                            case "femail":
                                tmp.Sex = 2;
                                break;
                            default:
                                tmp.Sex = 0;
                                break;
                        }
                    }
                    else
                    {
                        tmp.Sex = 2;
                    }

                    if (tmpStudy.Tags.ContainsKey((uint)Tags.BIRTHDAY))
                        tmp.BirthDay = tmpStudy.Tags[(uint)Tags.BIRTHDAY];

                    tmp.Date = tmpStudy.Tags[(uint)Tags.STUDYDATE];
                    tmp.Time = tmpStudy.Tags[(uint)Tags.STUDYTIME];

                    if (tmpStudy.Tags.ContainsKey((uint)Tags.BODYPART))
                        tmp.BodyPart = tmpStudy.Tags[(uint)Tags.BODYPART];
                    tmp.Modality = tmpStudy.Modality;
                    tmp.StudyInstanceUID = tmpStudy.UID;
                    tmp.ImgCnt = tmpStudy.Cnt;
                }

                ret.Items = tmp;
                ret.Result = true;
                C_Find.Client.Close();
                C_Find.Client = new DicomQRClient();
            }
            catch (Exception e)
            {
                ret.Result = false;
                ret.Message = "処理中に障害が発生いたしました。\nシステム管理者にお問い合わせください。";
                LogControl.WriteLog(LogType.ERR, "WebGetPreOrder", e.Message);
                LogControl.WriteLog(LogType.ERR, "WebGetPreOrder", e.StackTrace);
                C_Find.Client.Close();
                C_Find.Client = new DicomQRClient();
            }

            return ret;
        }

        [WebMethod(EnableSession = true)]
        public string[] WebGetComboList(int type, string val)
        {
            string[] ret = null;

            try
            {
                string listDir = ConfigurationManager.AppSettings["ListDir"];

                switch(type)
                {
                    case 0:
                        listDir = Path.Combine(listDir, "Modality");
                        break;
                }

                string file = Path.Combine(listDir, val + ".txt");
                List<string> tmpVals = new List<string>();

                if(File.Exists(file))
                    using (var sr = new StreamReader(file, Encoding.GetEncoding("Shift-JIS")))
                    {
                        while(!sr.EndOfStream)
                            tmpVals.Add(sr.ReadLine());
                    }

                ret = tmpVals.ToArray();
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "WebGetComboList", e.Message);
                LogControl.WriteLog(LogType.ERR, "WebGetComboList", e.StackTrace);
            }

            return ret;
        }

        [WebMethod(EnableSession = true)]
        public WebResult SetPreOrder(string[] values)
        {
            WebResult ret = new WebResult();

            try
            {
                PreOrder order = new PreOrder();
                string msg = "";
                string msg3 = "";

                List<string> pastIds = new List<string>();

                order.StudyInstanceUID = values[0];
                order.OrderNo = values[1].Trim();
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
                order.Comment = values[14];
                order.Sex = Convert.ToInt32(values[15]);
                order.Department = values[16];
                order.Doctor = values[17];
                msg = values[18];

                string orderPath = "";
                orderPath = ConfigurationManager.AppSettings["OrderDir"];
                orderPath = Path.Combine(orderPath, order.OrderNo + ".csv");

                if (!File.Exists(orderPath))
                    order.Status = 1;
                else
                    order.Status = 2;

                if (values.Length > 19)
                {
                    for (int i = 19; i < values.Length; i++)
                    {
                        pastIds.Add(values[i]);
                    }
                }

                LogControl.WriteLog(LogType.ORE, "SetPreOrder", "【オーダー登録】オーダー番号：" + order.OrderNo + "患者ID：" + order.PatID + " ﾓﾀﾞﾘﾃｨ：" + order.Modality + " 検査日：" + order.Date);

                Search search = new Search();

                var msg4 = "";

                if (!string.IsNullOrEmpty(msg))
                    msg4 = "比較参照お願いします。（" + msg + "）" ;

                string cA = order.ImgCnt.ToString();
                string cB = msg;

                if (string.IsNullOrEmpty(msg))
                    msg = order.ImgCnt.ToString();
                else
                    msg = order.ImgCnt.ToString() + "(" + msg + ")";

                WriteCSV(order, msg, msg3, msg4, cA, cB);

                if(order.Status == 1 || order.Status == 2)
                {
                    C_Find.Setting();
                    foreach (var past in pastIds)
                    {
                        DicomQRItem[] studyDatas = null;

                        studyDatas = C_Find.C_FindFromStudy("", "", "", "", past);

                        PreOrder tmp = new PreOrder();

                        tmp = order;

                        for (int i = 0; i < studyDatas.Length; i++)
                        {
                            StudyData tmpStudy = new StudyData();
                            tmpStudy.SetStudy(studyDatas[i].Tags);

                            DicomQRItem[] tmpDat = C_Find.C_FindFromSeries(tmpStudy.UID);
                            tmpStudy.SetSeries(tmpDat);

                            tmp.Date = tmpStudy.Tags[(uint)Tags.STUDYDATE];
                            tmp.Modality = tmpStudy.Modality;
                        }

                        WriteCSV_Past(tmp, msg, msg3, msg4, cA, cB);
                    }

                    string UIDfile = Path.Combine(ConfigurationManager.AppSettings["UIDDir"], order.OrderNo + ".txt");

                    if (File.Exists(UIDfile))
                        File.Delete(UIDfile);

                    using (var sw = new StreamWriter(UIDfile))
                    {
                        sw.WriteLine(order.StudyInstanceUID);
                    }
                }
                ret.Result = true;
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


        [WebMethod(EnableSession = true)]
        public WebViewList WebGetViewList(string hospcd)
        {
            WebViewList ret = new WebViewList();

            try
            {
                string path = ConfigurationManager.AppSettings["RetPath"];

                if (!Directory.Exists(path))
                {
                    LogControl.WriteLog(LogType.ERR, "WebGetViewList", "【所見取得】 フォルダなし：" + path);
                    return ret;
                }

                string[] filePaths = Directory.GetFiles(path);
                List<View> tmpList = new List<View>();

                filePaths = filePaths.OrderByDescending(f => File.GetLastWriteTime(f)).ToArray();
                int maxCnt = Convert.ToInt32(ConfigurationManager.AppSettings["MaxCnt"]);
                int cnt = 0;
                foreach (var file in filePaths)
                {
                    if (cnt == maxCnt)
                        break;
                    cnt++;

                    string name = Path.GetFileNameWithoutExtension(file);
                    string[] vals = name.Split('_');

                    if (vals.Length < 4)
                        continue;

                    View tmp = new View();

                    tmp.PatID = vals[0];
                    tmp.PatName = vals[1];
                    tmp.Modality = vals[2];
                    tmp.Date = vals[3];
                    tmp.OrderID = vals[4] + "_" + vals[5];

                    string dir = ConfigurationManager.AppSettings["PDFPath"];
                    string[] tmpFiles = Directory.GetFiles(dir, name + ".*");

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

        [WebMethod(EnableSession = true)]
        public string[] WebGetDoctor()
        {
            List<string> ret = new List<string>();

            try
            {
                string path = ConfigurationManager.AppSettings["DocFile"];

                if (!File.Exists(path))
                    return ret.ToArray();

                using (var sr = new StreamReader(path, Encoding.GetEncoding("Shift-JIS")))
                    while(!sr.EndOfStream)
                        ret.Add(sr.ReadLine());


            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "WebGetDoctor", e.Message);
            }

            return ret.ToArray();
        }

        [WebMethod(EnableSession = true)]
        public bool WebSetDoctor(string[] doc)
        {
            bool ret = false;

            try
            {
                string path = ConfigurationManager.AppSettings["DocFile"];

                if (!File.Exists(path))
                    return false;

                using (var sw = new StreamWriter(path, false, Encoding.GetEncoding("Shift-JIS")))
                    foreach(var dr in doc)
                        sw.WriteLine(dr);

                ret = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "WebGetDoctor", e.Message);
            }

            return ret;
        }


        private PreOrder ReadCSV(string file)
        {
            PreOrder order = new PreOrder();

            string mstPath = "";
            mstPath = System.IO.Path.Combine(ConfigurationManager.AppSettings["MastDir"], "Mast.csv");

            var csvValue = "";
            using (var sr = new System.IO.StreamReader(mstPath, System.Text.Encoding.GetEncoding("Shift-JIS")))
                csvValue = sr.ReadLine();

            var csvCols = csvValue.Replace("\"", "").Split(',');

            using (var sr = new System.IO.StreamReader(file, System.Text.Encoding.GetEncoding("Shift-JIS")))
                csvValue = sr.ReadToEnd();

            var datCols = csvValue.Replace("\",\"", "$").Replace("\"", "").Replace("\r\n", "").Replace("\n", "").Split('$');

            for (int i = 0; i <= 23; i++)
            {
                var key = "@" + i.ToString().PadLeft(2, '0');
                var index = -1;

                for(int j = 0; j < csvCols.Length; j++)
                {
                    if(csvCols[j] == key)
                    {
                        index = j;
                        break;
                    }
                }

                if (index < 0)
                    continue;

                switch(key)
                {
                    case "@00":
                        order.Status = Convert.ToInt32(datCols[index]);
                        break;
                    case "@01":
                        order.PatID = datCols[index].Replace(ConfigurationManager.AppSettings["HospCD"], "");
                        break;
                    case "@02":
                        order.PatName = datCols[index];
                        break;
                    case "@03":
                        order.PatName_H = datCols[index];
                        break;
                    case "@04":
                        order.PatName_R = datCols[index];
                        break;
                    case "@05":
                        switch (datCols[index])
                        {
                            case "M":
                                order.Sex = 1;
                                break;
                            case "F":
                                order.Sex = 2;
                                break;
                            default:
                                order.Sex = 0;
                                break;
                        }
                        break;
                    case "@06":
                        order.BirthDay = datCols[index];
                        break;
                    case "@07":
                        order.PatAge = Convert.ToInt32(datCols[index]);
                        break;
                    case "@08":
                        order.Date = datCols[index];
                        break;
                    case "@09":
                        order.Time = datCols[index];
                        break;
                    case "@10":
                        order.Modality = datCols[index];
                        break;
                    case "@11":
                        order.IsEmergency = Convert.ToInt32(datCols[index]);
                        break;
                    case "@12":
                        order.Comment = datCols[index];
                        break;
                    case "@13":
                        order.Recept = datCols[index];
                        break;
                    case "@14":
                        order.Contact = datCols[index];
                        break;
                    case "@15":
                        break;
                    case "@16":
                        break;
                    case "@17":
                        order.BodyPart = datCols[index];
                        break;
                    case "@18":
                        order.Type = datCols[index];
                        break;
                    case "@19":
                        order.ImgCnt = Convert.ToInt32(datCols[index]);
                        break;
                    case "@20":
                        order.PastCnt = datCols[index];
                        break;
                    case "@21":
                        order.OrderNo = datCols[index].Replace(ConfigurationManager.AppSettings["HospCD"], "");
                        break;
                    case "@22":
                        order.Doctor = datCols[index];
                        break;
                    case "@23":
                        order.Department = datCols[index];
                        break;
                }
            }


            return order;
        }

        private void WriteCSV(PreOrder order, string msg1, string msg3, string msg4, string cntA, string cntB)
        {
            string hospCd = "";
            string hospName = "";

            string mstPath = "";
            string outPath = "";
            string orderPath = "";
            string outPath_back = "";

            hospCd = ConfigurationManager.AppSettings["HospCD"];
            hospName = ConfigurationManager.AppSettings["HospName"];

            mstPath = System.IO.Path.Combine(ConfigurationManager.AppSettings["MastDir"], "Mast.csv");
            outPath = ConfigurationManager.AppSettings["CsvDir"];

            string backDef = ConfigurationManager.AppSettings["BackDir"];
            backDef = System.IO.Path.Combine(backDef, DateTime.Now.ToString("yyyyMMdd"));

            if (!System.IO.Directory.Exists(backDef))
                System.IO.Directory.CreateDirectory(backDef);

            outPath_back = System.IO.Path.Combine(backDef, "CSV");
            if (!System.IO.Directory.Exists(outPath_back))
                System.IO.Directory.CreateDirectory(outPath_back);

            orderPath = ConfigurationManager.AppSettings["OrderDir"];

            var csvValue = "";

            using (var sr = new System.IO.StreamReader(mstPath, System.Text.Encoding.GetEncoding("Shift-JIS")))
            {
                csvValue = sr.ReadLine();
            }

            csvValue = csvValue.Replace("@00", order.Status.ToString());
            csvValue = csvValue.Replace("@01", hospCd + order.PatID);
            csvValue = csvValue.Replace("@02", order.PatName);
            csvValue = csvValue.Replace("@03", order.PatName_H);
            csvValue = csvValue.Replace("@04", order.PatName_R);
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
            csvValue = csvValue.Replace("@09", order.Time.Trim());
            csvValue = csvValue.Replace("@10", order.Modality);
            csvValue = csvValue.Replace("@11", order.IsEmergency.ToString());
            csvValue = csvValue.Replace("@12", order.Comment);
            csvValue = csvValue.Replace("@13", msg3);
            csvValue = csvValue.Replace("@14", msg4);
            csvValue = csvValue.Replace("@15", hospCd);
            csvValue = csvValue.Replace("@16", hospName);
            csvValue = csvValue.Replace("@17", order.BodyPart);
            csvValue = csvValue.Replace("@18", order.Type);

            csvValue = csvValue.Replace("@19", cntA);

            if (string.IsNullOrEmpty(cntB) || string.IsNullOrEmpty(cntB.Trim()))
                csvValue = csvValue.Replace("@20", "none");
            else
                csvValue = csvValue.Replace("@20", cntB);

            csvValue = csvValue.Replace("@21", hospCd + order.OrderNo);

            csvValue = csvValue.Replace("@22", order.Doctor);
            csvValue = csvValue.Replace("@23", order.Department);

            var fileName = order.OrderNo;

            using (var sw = new System.IO.StreamWriter(System.IO.Path.Combine(orderPath, fileName) + ".csv", false, System.Text.Encoding.GetEncoding("Shift-JIS")))
            {
                sw.WriteLine(csvValue);
            }

            fileName = hospCd + fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");

            using (var sw = new System.IO.StreamWriter(System.IO.Path.Combine(outPath, fileName) + ".csv", false, System.Text.Encoding.GetEncoding("Shift-JIS")))
            {
                sw.WriteLine(csvValue);
            }

            if(order.Status == 1)
            {
                string triPath = ConfigurationManager.AppSettings["ExtraDir"];
                string triExt = ConfigurationManager.AppSettings["ExtraExt"];

                using (var sw = new System.IO.StreamWriter(System.IO.Path.Combine(triPath, fileName) + ".csv", false, System.Text.Encoding.GetEncoding("Shift-JIS")))
                    sw.WriteLine(csvValue);

                if(!string.IsNullOrEmpty(triExt))
                {
                    var dat = System.IO.File.Create(System.IO.Path.Combine(triPath, fileName) + "." + triExt);
                    dat.Close();
                }
            }

            using (var sw = new System.IO.StreamWriter(System.IO.Path.Combine(outPath_back, fileName) + ".csv", false, System.Text.Encoding.GetEncoding("Shift-JIS")))
            {
                sw.WriteLine(csvValue);
            }
        }


        private void WriteCSV_Past(PreOrder order, string msg1, string msg3, string msg4, string cntA, string cntB)
        {
            string hospCd = "";
            string hospName = "";

            string mstPath = "";
            string outPath = "";

            hospCd = ConfigurationManager.AppSettings["HospCD"];
            hospName = ConfigurationManager.AppSettings["HospName"];

            mstPath = System.IO.Path.Combine(ConfigurationManager.AppSettings["MastDir"], "Mast.csv");
            outPath = ConfigurationManager.AppSettings["CsvDir"];

            var csvValue = "";

            using (var sr = new System.IO.StreamReader(mstPath, System.Text.Encoding.GetEncoding("Shift-JIS")))
            {
                csvValue = sr.ReadLine();
            }

            csvValue = csvValue.Replace("@00", order.Status.ToString());
            csvValue = csvValue.Replace("@01", hospCd + order.PatID);
            csvValue = csvValue.Replace("@02", order.PatName);
            csvValue = csvValue.Replace("@03", order.PatName_H);
            csvValue = csvValue.Replace("@04", order.PatName_R);
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
            csvValue = csvValue.Replace("@09", order.Time);
            csvValue = csvValue.Replace("@10", order.Modality);
            csvValue = csvValue.Replace("@11", order.IsEmergency.ToString());
            csvValue = csvValue.Replace("@12", order.Comment);
            csvValue = csvValue.Replace("@13", msg3);
            csvValue = csvValue.Replace("@14", msg4);
            csvValue = csvValue.Replace("@15", hospCd);
            csvValue = csvValue.Replace("@16", hospName);
            csvValue = csvValue.Replace("@17", order.BodyPart);
            csvValue = csvValue.Replace("@18", order.Type);

            csvValue = csvValue.Replace("@19", cntA);

            if (string.IsNullOrEmpty(cntB) || string.IsNullOrEmpty(cntB.Trim()))
                csvValue = csvValue.Replace("@20", "none");
            else
                csvValue = csvValue.Replace("@20", cntB);

            csvValue = csvValue.Replace("@21", hospCd + order.OrderNo);


            var fileName = hospCd + order.OrderNo + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string triPath = ConfigurationManager.AppSettings["ExtraDir_P"];
            string triExt = ConfigurationManager.AppSettings["ExtraExt_P"];

            using (var sw = new System.IO.StreamWriter(System.IO.Path.Combine(triPath, fileName) + ".csv", false, System.Text.Encoding.GetEncoding("Shift-JIS")))
                sw.WriteLine(csvValue);

            if (!string.IsNullOrEmpty(triExt))
            {
                var dat = System.IO.File.Create(System.IO.Path.Combine(triPath, fileName) + "." + triExt);
                dat.Close();
            }
        }

    }


    // アシスタントクラス群
    public class WebResult
    {
        public bool Result { get; set; }
        public string Message { get; set; }
    }

    public class WebPreOrderList : WebResult
    {
        public PreOrder[] Items { get; set; }
    }

    public class WebPreOrder : WebResult
    {
        public PreOrder Items { get; set; }
    }

    public class WebPatientList : WebResult
    {
        public Patient[] Items { get; set; }
    }
    public class WebImageList : WebResult
    {
        public ImageURL[] Items { get; set; }
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

    [DataContract]
    public class ImageURL
    {
        /// <summary>ファイルURL</summary>
        [DataMember]
        public string URL { get; set; }

        /// <summary>ファイル名</summary>
        [DataMember]
        public string Name { get; set; }
    }
    /// <summary>
    /// 患者情報クラス
    /// </summary>
    [DataContract]
    public class Patient
    {
        /// <summary>患者ID</summary>
        [DataMember]
        public string PatID { get; set; }

        /// <summary>患者名</summary>
        [DataMember]
        public string PatName { get; set; }

        /// <summary>患者名カナ</summary>
        [DataMember]
        public string PatName_H { get; set; }

        /// <summary>患者名ローマ</summary>
        [DataMember]
        public string PatName_R { get; set; }

        /// <summary>性別</summary>
        [DataMember]
        public int Sex { get; set; }

        /// <summary>生年月日</summary>
        [DataMember]
        public string BirthDay { get; set; }

        /// <summary>コンストラクタ</summary>
        public Patient()
        {
            this.PatID = "";
            this.PatName = "";
            this.PatName_H = "";
            this.PatName_R = "";
            this.Sex = 0;
            this.BirthDay = "";
        }
    }

    /// <summary>
    /// 事前登録用オーダークラス
    /// </summary>
    [DataContract]
    public class PreOrder
    {
        /// <summary>オーダー管理ID</summary>
        [DataMember]
        public int OrderID { get; set; }

        /// <summary>患者ID</summary>
        [DataMember]
        public string PatID { get; set; }

        /// <summary>患者名</summary>
        [DataMember]
        public string PatName { get; set; }

        /// <summary>患者名カナ</summary>
        [DataMember]
        public string PatName_H { get; set; }

        /// <summary>患者名ローマ</summary>
        [DataMember]
        public string PatName_R { get; set; }

        /// <summary>性別</summary>
        [DataMember]
        public int Sex { get; set; }

        /// <summary>生年月日</summary>
        [DataMember]
        public string BirthDay { get; set; }

        /// <summary>オーダー番号</summary>
        [DataMember]
        public string OrderNo { get; set; }

        /// <summary>患者年齢</summary>
        [DataMember]
        public int PatAge { get; set; }

        /// <summary>モダリティ</summary>
        [DataMember]
        public string Modality { get; set; }

        /// <summary>検査日</summary>
        [DataMember]
        public string Date { get; set; }

        /// <summary>検査時刻</summary>
        [DataMember]
        public string Time { get; set; }

        /// <summary>検査部位</summary>
        [DataMember]
        public string BodyPart { get; set; }

        /// <summary>単純/造影</summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>画像枚数</summary>
        [DataMember]
        public int ImgCnt { get; set; }

        /// <summary>過去画像枚数</summary>
        [DataMember]
        public string PastCnt { get; set; }

        /// <summary>緊急フラグ</summary>
        [DataMember]
        public int IsEmergency { get; set; }

        /// <summary>依頼内容</summary>
        [DataMember]
        public string Comment { get; set; }

        /// <summary>受付専用</summary>
        [DataMember]
        public string Recept { get; set; }

        /// <summary>連絡事項j</summary>
        [DataMember]
        public string Contact { get; set; }

        /// <summary>状態</summary>
        [DataMember]
        public int Status { get; set; }

        /// <summary>依頼内容</summary>
        [DataMember]
        public string StudyInstanceUID { get; set; }

        /// <summary>依頼医</summary>
        [DataMember]
        public string Doctor { get; set; }

        /// <summary>依頼科</summary>
        [DataMember]
        public string Department { get; set; }
        /// <summary>依頼科</summary>
        [DataMember]
        public string Desc { get; set; }


        /// <summary>コンストラクタ</summary>
        public PreOrder()
        {
            this.OrderNo = "";
            this.PatID = "";
            this.PatName = "";
            this.PatName_H = "";
            this.PatName_R = "";
            this.Sex = 0;
            this.BirthDay = "";
            this.PatAge = 0;
            this.Modality = "";
            this.Date = "";
            this.Time = "";
            this.BodyPart = "";
            this.Type = "";
            this.ImgCnt = 0;
            this.IsEmergency = 0;
            this.Comment = "";
            this.Status = -1;
            this.StudyInstanceUID = "";

            this.Doctor = "";
            this.Department = "";

            this.Desc = "";

            this.Status = -1;
        }

    }

}
