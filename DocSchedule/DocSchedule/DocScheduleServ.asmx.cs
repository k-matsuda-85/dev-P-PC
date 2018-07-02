using LogController;
using MyAccDB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Web.Services;

namespace DocSchedule
{
    /// <summary>
    /// DocScheduleServ の概要の説明です
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // この Web サービスを、スクリプトから ASP.NET AJAX を使用して呼び出せるようにするには、次の行のコメントを解除します。
    [System.Web.Script.Services.ScriptService]
    public class DocScheduleServ : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        public WebUserList WebGetUserList()
        {
            WebUserList ret = new WebUserList();

            try
            {
                var file = ConfigurationManager.AppSettings["UserFile"];
                List<UserDat> fileVals = new List<UserDat>();

                using (var sr = new StreamReader(file, Encoding.GetEncoding("Shift-JIS")))
                    while (!sr.EndOfStream)
                    {
                        UserDat tmp = new UserDat();

                        var val = sr.ReadLine();
                        var vals = val.Split(',');

                        tmp.Name = vals[0];
                        tmp.ID = vals[1];

                        fileVals.Add(tmp);
                    }

                ret.Items = fileVals.ToArray();
                ret.Result = true;
            }
            catch (Exception e)
            {
                ret.Result = false;
                ret.Message = "処理中に障害が発生いたしました。\nシステム管理者にお問い合わせください。";
            }

            return ret;
        }




        [WebMethod]
        public string[] GetColorList()
        {
            List<string> ret = new List<string>();

            try
            {
                string dir = ConfigurationManager.AppSettings["ColorFile"];

                using (var sr = new StreamReader(dir))
                {
                    while (!sr.EndOfStream)
                        ret.Add(sr.ReadLine());
                }

            }
            catch(Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "GetColorList", ex.Message);
                LogControl.WriteLog(LogType.ERR, "GetColorList", ex.StackTrace);
            }
            return ret.ToArray();
        }

        [WebMethod]
        public void SetColor(string color)
        {
            List<string> ret = new List<string>();

            try
            {
                string dir = ConfigurationManager.AppSettings["ColorFile"];

                using (var sw = new StreamWriter(dir, true))
                {
                    sw.WriteLine(color);
                }

            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "SetColorList", ex.Message);
                LogControl.WriteLog(LogType.ERR, "SetColorList", ex.StackTrace);
            }
            return;
        }

        [WebMethod]
        public Doctor[] GetDoctors()
        {
            List<Doctor> ret = new List<Doctor>();

            try
            {
                string dir = ConfigurationManager.AppSettings["DocDir"];

                var files = Directory.GetFiles(dir);

                foreach(var fi in files)
                {
                    var dat = "";

                    using (var sr = new StreamReader(fi,Encoding.GetEncoding("Shift-JIS")))
                    {
                        dat = sr.ReadToEnd();
                    }

                    var values = dat.Split('$');

                    Doctor doc = new Doctor();
                    doc.DocID = values[0];
                    doc.DocName = values[1];
                    doc.DocName_R = values[2];
                    doc.DocName_H = values[3];
                    doc.Comment = values[4];
                    doc.Count = values[5];
                    doc.Speed = values[6];
                    doc.Main = values[7];
                    doc.Element = values[8];
                    doc.Memo = values[9];
                    doc.Color = values[10];
                    doc.Color2 = values[11];

                    var dats = values[12].Replace("\r\n", "").Split('@');
                    List<BodyPart> dp = new List<BodyPart>();

                    var dt = dats[0];

                    var d = dt.Split(':');
                    BodyPart tmp = new BodyPart();
                    List<Data> tmpDataList = new List<Data>();

                    foreach(var da in d)
                    {
                        if (string.IsNullOrEmpty(da))
                            continue;
                        var tmpdat = da.Split(',');
                        Data tmpdata = new Data();
                        tmpdata.Id = Convert.ToInt32(tmpdat[0]);
                        tmpdata.Text = tmpdat[1];

                        tmpDataList.Add(tmpdata);
                    }

                    tmp.OK = tmpDataList.ToArray();
                    dt = dats[1];

                    d = dt.Split(':');
                    tmpDataList = new List<Data>();

                    foreach (var da in d)
                    {
                        if (string.IsNullOrEmpty(da))
                            continue;

                        var tmpdat = da.Split(',');
                        Data tmpdata = new Data();
                        tmpdata.Id = Convert.ToInt32(tmpdat[0]);
                        tmpdata.Text = tmpdat[1];

                        tmpDataList.Add(tmpdata);
                    }

                    tmp.NG = tmpDataList.ToArray();

                    dt = dats[2];

                    d = dt.Split(':');
                    tmpDataList = new List<Data>();

                    foreach (var da in d)
                    {
                        if (string.IsNullOrEmpty(da))
                            continue;
                        var tmpdat = da.Split(',');
                        Data tmpdata = new Data();
                        tmpdata.Id = Convert.ToInt32(tmpdat[0]);
                        tmpdata.Text = tmpdat[1];

                        tmpDataList.Add(tmpdata);
                    }

                    tmp.Other = tmpDataList.ToArray();

                    doc.Body = tmp;

                    var hospdat = values[13].Replace("\r\n","").Split('@');

                    doc.Hosp = new Hospital();
                    doc.Hosp.OK = hospdat[0].Split(',');
                    doc.Hosp.NG = hospdat[1].Split(',');
                    doc.Hosp.Other = hospdat[2].Split(',');

                    ret.Add(doc);
                }

                ret.Sort(delegate (Doctor a, Doctor b) { return string.Compare(a.DocName_H, b.DocName_H); });
            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "GetDoctors", ex.Message);
                LogControl.WriteLog(LogType.ERR, "GetDoctors", ex.StackTrace);
            }

            return ret.ToArray();
        }

        [WebMethod]
        public string[] GetSPDays()
        {
            string mstFile = ConfigurationManager.AppSettings["DayFile"];
            List<string> spDays = new List<string>();

            using (var sr = new StreamReader(mstFile))
                while (!sr.EndOfStream)
                    spDays.Add(sr.ReadLine());

            return spDays.ToArray();
        }

        [WebMethod]
        public DayDat[] GetScheduleList(string day)
        {
            List<DayDat> ret = new List<DayDat>();

            try
            {
                string dir = ConfigurationManager.AppSettings["SchDir"];
                string mstFile = ConfigurationManager.AppSettings["DayFile"];

                var files = Directory.GetFiles(dir);

                if (string.IsNullOrEmpty(day))
                    return ret.ToArray();

                DateTime start = DateTime.Parse(day);
                Dictionary<int, string> day_format = new Dictionary<int, string>();
                Dictionary<int, string> allday_format = new Dictionary<int, string>();

                List<string> spDays = new List<string>();

                using (var sr = new StreamReader(mstFile))
                    while(!sr.EndOfStream)
                        spDays.Add(sr.ReadLine());

                for (int i = 0; i < 7; i++)
                {
                    DateTime dt = start.AddDays(i);
                    string sdt = dt.ToString("yyyyMMdd");

                    if(!spDays.Contains(sdt))
                        day_format[(int)dt.DayOfWeek] = sdt;

                    allday_format[(int)dt.DayOfWeek] = sdt;
                }

                var editDays = getEditSchedule();
                List<string> editIDs = new List<string>();

                var holiDays = getHoliList(day);

                List<Day> days = new List<Day>();

                DayTimes conDt = new DocScheduleServ.DayTimes();

                foreach (var fi in files)
                {
                    var dat = "";

                    var id = Path.GetFileNameWithoutExtension(fi);

                    using (var sr = new StreamReader(fi, Encoding.GetEncoding("Shift-JIS")))
                        dat = sr.ReadLine();

                    var values = dat.Split('$');

                    //var sdate = values[0];
                    //var edate = values[1];

                    //if (!string.IsNullOrEmpty(sdate))
                    //{
                    //    if (sdate.CompareTo(start.ToString("yyyyMMdd")) > 0)
                    //        continue;
                    //    else if(!string.IsNullOrEmpty(edate))
                    //    {
                    //        if (edate.CompareTo(start.AddDays(6).ToString("yyyyMMdd")) < 0)
                    //            continue;
                    //    }
                    //}
                    //else if (!string.IsNullOrEmpty(edate))
                    //{
                    //    if (edate.CompareTo(start.AddDays(6).ToString("yyyyMMdd")) < 0)
                    //        continue;
                    //}

                    var vals = values[2].Split('@');

                    Week[] weeks = getSchedule(vals);
                    var id_list = new List<string>();
                    int id_index = 0;

                    foreach(var wk in weeks)
                    {
                        Day tmpDay = new Day();

                        tmpDay.DocID = id;

                        if (!day_format.Keys.Contains(wk.Day))
                            continue;
                        else
                        {
                            var ttDay = day_format[wk.Day];

                            if (ttDay.CompareTo(wk.Sdate.Replace("/", "")) >= 0)
                            {
                                if (!string.IsNullOrEmpty(wk.Edate) &&
                                    ttDay.CompareTo(wk.Edate.Replace("/", "")) > 0)
                                    continue;
                            }
                            else
                                continue;

                        }


                        tmpDay.Date = day_format[wk.Day];
                        tmpDay.ID = id.ToString() + "_" + wk.Day.ToString() + "_" + wk.Type.ToString() + "_" + wk.SubType.ToString();
                        if (id_list.Contains(tmpDay.ID))
                        {
                            tmpDay.ID += "_" + id_index.ToString();
                            id_index++;
                        }

                        id_list.Add(tmpDay.ID);

                        tmpDay.Type = wk.Type;
                        tmpDay.SubType = wk.SubType;

                        if(editDays.Keys.Contains(tmpDay.ID + "@" + tmpDay.Date))
                        {
                            editIDs.Add(tmpDay.ID + "@" + tmpDay.Date);

                            tmpDay.Count = editDays[tmpDay.ID + "@" + tmpDay.Date].Count;
                            if (editDays[tmpDay.ID + "@" + tmpDay.Date].iStime != -2)
                                wk.Stime = editDays[tmpDay.ID + "@" + tmpDay.Date].iStime;
                            if (editDays[tmpDay.ID + "@" + tmpDay.Date].iEtime != -2)
                                wk.Etime = editDays[tmpDay.ID + "@" + tmpDay.Date].iEtime;

                            tmpDay.IsAll = editDays[tmpDay.ID + "@" + tmpDay.Date].IsAll;
                            tmpDay.IsChange = editDays[tmpDay.ID + "@" + tmpDay.Date].IsChange;
                            tmpDay.IsDelete = editDays[tmpDay.ID + "@" + tmpDay.Date].IsDelete;
                            tmpDay.Update = editDays[tmpDay.ID + "@" + tmpDay.Date].Update;
                        }
                        else
                        {
                            tmpDay.Count = wk.Count;

                            tmpDay.Update = "";
                        }

                        tmpDay.iStime = wk.Stime;
                        tmpDay.iEtime = wk.Etime;

                        if (tmpDay.IsChange == 1)
                        {
                            tmpDay.SubDays = getSubSchedule(tmpDay.ID, tmpDay.Date);

                            var itime = wk.Stime;
                            var otime = wk.Etime;

                            for (int sub = 0; sub < tmpDay.SubDays.Length; sub++)
                            {
                                tmpDay.SubDays[sub].Style = getLineData(tmpDay.SubDays[sub].iStime, tmpDay.SubDays[sub].iEtime);
                                tmpDay.SubDays[sub].Stime = conDt.ConstData[tmpDay.SubDays[sub].iStime];
                                tmpDay.SubDays[sub].Etime = conDt.ConstData[tmpDay.SubDays[sub].iEtime];

                                if(tmpDay.SubDays[sub].iStime == itime && tmpDay.SubDays[sub].iEtime < otime)
                                    itime = tmpDay.SubDays[sub].iEtime;
                                if (tmpDay.SubDays[sub].iEtime == otime && tmpDay.SubDays[sub].iStime > itime)
                                    otime = tmpDay.SubDays[sub].iStime;

                            }

                            wk.Stime = itime;
                            wk.Etime = otime;

                            tmpDay.IsDelete = 1;
                        }

                        if (wk.Stime == -1 && wk.Etime == -1)
                        {
                            tmpDay.Stime = "";
                            tmpDay.Etime = "";
                            tmpDay.Style = "";
                        }
                        else
                        {
                            tmpDay.Style = getLineData(wk.Stime, wk.Etime);
                            if(wk.Stime == -1)
                                tmpDay.Stime = conDt.ConstData[0];
                            else
                                tmpDay.Stime = conDt.ConstData[wk.Stime];

                            if (wk.Etime == -1)
                                tmpDay.Etime = conDt.ConstData[46];
                            else
                                tmpDay.Etime = conDt.ConstData[wk.Etime];
                        }


                        foreach(var holi in holiDays)
                        {
                            if(tmpDay.DocID==holi.DocID && holi.Type == 0)
                            {
                                if (holi.SDay.CompareTo(tmpDay.Date) > 0)
                                    break;
                                if (holi.EDay.CompareTo(tmpDay.Date) < 0)
                                    break;

                                tmpDay.IsDelete = 1;
                            }
                        }

                        days.Add(tmpDay);
                    }
                }

                foreach (var dy in allday_format)
                {
                    foreach(var newDay in editDays)
                    {
                        if (newDay.Key.IndexOf("@" + dy.Value) < 0)
                            continue;

                        if (editIDs.Contains(newDay.Key))
                            continue;

                        Day tmpDay = new Day();

                        tmpDay = newDay.Value;

                        tmpDay.Style = getLineData(tmpDay.iStime, tmpDay.iEtime);
                        if (tmpDay.iStime <= -1)
                            tmpDay.Stime = conDt.ConstData[0];
                        else
                            tmpDay.Stime = conDt.ConstData[tmpDay.iStime];

                        if (tmpDay.iEtime <= -1)
                            tmpDay.Etime = conDt.ConstData[46];
                        else
                            tmpDay.Etime = conDt.ConstData[tmpDay.iEtime];


                        foreach (var holi in holiDays)
                        {
                            if (tmpDay.DocID == holi.DocID)
                            {
                                if (holi.SDay.CompareTo(tmpDay.Date) > 0)
                                    break;
                                if (holi.EDay.CompareTo(tmpDay.Date) < 0)
                                    break;

                                tmpDay.IsDelete = 1;
                            }
                        }

                        days.Add(tmpDay);
                    }
                }

                IOrderedEnumerable<Day> sortList = days.OrderBy(rec => rec.Date).ThenBy(rec => rec.Type).ThenBy(rec => rec.iStime).ThenBy(rec => rec.iEtime);

                Dictionary<int, int> preETime = new Dictionary<int, int>();

                Dictionary<int, List<Day>> preData = new Dictionary<int, List<Day>>();
                List<Day> subDays = new List<Day>();
                List<Day> subDays_o = new List<Day>();

                string preDate = "";
                int preType = -1;
                int subCnt = 0;

                preETime[0] = -1;

                preData[0] = new List<Day>();

                ret = new List<DayDat>();

                foreach (var dy in sortList)
                {
                    var row = 0;

                    if (preDate != dy.Date || preType != dy.Type)
                    {
                        preETime = new Dictionary<int, int>();
                        preETime[0] = -1;

                        if(!string.IsNullOrEmpty(preDate))
                        {
                            foreach (var pd in preData)
                            {
                                DayDat tmpDD = new DayDat();

                                tmpDD.Row = pd.Key;
                                tmpDD.Data = pd.Value.ToArray();

                                if(subDays.Count > subCnt)
                                {
                                    List<Day> tmplist = new List<Day>();
                                    tmplist = tmpDD.Data.ToList();
                                    tmplist.Add(subDays[subCnt]);
                                    subCnt++;
                                    if (subDays.Count > subCnt)
                                    {
                                        tmplist.Add(subDays[subCnt]);
                                        subCnt++;
                                    }
                                    tmpDD.Data = tmplist.ToArray();
                                }

                                ret.Add(tmpDD);
                            }
                            if (subDays.Count > subCnt)
                            {
                                for(int i = subCnt; i < subDays.Count; i++)
                                {
                                    DayDat tmpDD = new DayDat();

                                    tmpDD.Row = subCnt;

                                    List<Day> tmplist = new List<Day>();
                                    tmplist.Add(subDays[i]);
                                    i++;
                                    if (subDays.Count > i)
                                    {
                                        tmplist.Add(subDays[i]);
                                        subCnt++;
                                    }
                                    tmpDD.Data = tmplist.ToArray();

                                    ret.Add(tmpDD);
                                }
                            }
                        }


                        preData = new Dictionary<int, List<Day>>();
                        preData[0] = new List<Day>();

                        preDate = dy.Date;
                        preType = dy.Type;
                        subCnt = 0;
                        subDays = new List<Day>();
                    }

                    if (dy.SubType != -1)
                    {
                        if (dy.SubType == 1)
                            subDays.Add(dy);
                        if (dy.SubType == 0)
                            preData[row].Add(dy);

                        continue;
                    }

                    while (preETime[row] > dy.iStime)
                    {
                        row++;
                        if (!preData.Keys.Contains(row))
                        {
                            preData[row] = new List<Day>();
                            break;
                        }
                        if (!preETime.Keys.Contains(row))
                            break;
                    }

                    preETime[row] = dy.iEtime;
                    preData[row].Add(dy);
                }


                if (subDays.Count > subCnt)
                {
                    for (int i = subCnt; i < subDays.Count; i++)
                    {
                        DayDat tmpDD = new DayDat();

                        tmpDD.Row = subCnt;

                        List<Day> tmplist = new List<Day>();
                        tmplist.Add(subDays[i]);
                        i++;
                        if (subDays.Count > i)
                        {
                            tmplist.Add(subDays[i]);
                            subCnt++;
                        }
                        tmpDD.Data = tmplist.ToArray();

                        ret.Add(tmpDD);
                    }
                }

                foreach (var pd in preData)
                {
                    DayDat tmpDD = new DayDat();

                    tmpDD.Row = pd.Key;
                    tmpDD.Data = pd.Value.ToArray();

                    ret.Add(tmpDD);
                }

            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "GetDoctors", ex.Message);
                LogControl.WriteLog(LogType.ERR, "GetDoctors", ex.StackTrace);
            }

            return ret.ToArray();
        }

        [WebMethod]
        public void SetDoctor(string[] values)
        {
            try
            {
                Doctor doc = new Doctor();
                string dir = ConfigurationManager.AppSettings["DocDir"];
                string dir2 = ConfigurationManager.AppSettings["SchDir"];

                doc.DocID = values[0];
                doc.DocName = values[1];
                doc.DocName_R = values[2];
                doc.DocName_H = values[3];
                doc.Comment = values[4];
                doc.Count = values[5];
                doc.Speed = values[6];
                doc.Main = values[7];
                doc.Element = values[8];
                doc.Memo = values[9];
                doc.Color = values[10];
                doc.Color2 = values[11];

                string writeVal = "";

                if(string.IsNullOrEmpty(doc.DocID))
                {
                    var dinfo = new DirectoryInfo(dir);

                    var files = dinfo.GetFiles();

                    int maxInt = 0;

                    foreach(var fi in files)
                    {
                        int tmpInt = Convert.ToInt32(Path.GetFileNameWithoutExtension(fi.FullName));

                        if (maxInt < tmpInt)
                            maxInt = tmpInt;
                    }

                    doc.DocID = (maxInt + 1).ToString();
                }

                writeVal += doc.DocID;
                writeVal += "$";
                writeVal += doc.DocName;
                writeVal += "$";
                writeVal += doc.DocName_R;
                writeVal += "$";
                writeVal += doc.DocName_H;
                writeVal += "$";
                writeVal += doc.Comment;
                writeVal += "$";
                writeVal += doc.Count;
                writeVal += "$";
                writeVal += doc.Speed;
                writeVal += "$";
                writeVal += doc.Main;
                writeVal += "$";
                writeVal += doc.Element;
                writeVal += "$";
                writeVal += doc.Memo;
                writeVal += "$";
                writeVal += doc.Color;
                writeVal += "$";
                writeVal += doc.Color2;
                writeVal += "$";
                writeVal += values[12];
                writeVal += "$";
                writeVal += values[13];

                using (var sr = new StreamWriter(Path.Combine(dir, doc.DocID + ".txt"), false, Encoding.GetEncoding("Shift-JIS")))
                {
                    sr.WriteLine(writeVal);
                }

                writeVal = "";

                writeVal += values[14];
                writeVal += "$";
                writeVal += values[15];
                writeVal += "$";
                writeVal += values[16];

                using (var sr = new StreamWriter(Path.Combine(dir2, doc.DocID + ".txt"), false, Encoding.GetEncoding("Shift-JIS")))
                {
                    sr.WriteLine(writeVal);
                }

            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "SetDoctor", ex.Message);
                LogControl.WriteLog(LogType.ERR, "SetDoctor", ex.StackTrace);
            }
        }

        [WebMethod]
        public DocSchedule GetSchedule(string id)
        {
            DocSchedule sch = new DocSchedule();
            try
            {
                var dat = "";
                string dir = ConfigurationManager.AppSettings["SchDir"];

                var file = Directory.GetFiles(dir, id + ".txt");

                if(file.Length ==0)
                {
                    sch.Schedule = new List<Week>().ToArray();
                    return sch;
                }

                using (var sr = new StreamReader(file[0], Encoding.GetEncoding("Shift-JIS")))
                {
                    dat = sr.ReadLine();
                }

                var values = dat.Split('$');

                //sch.Sdate = values[0];
                //sch.Edate = values[1];

                var vals = values[2].Split('@');

                sch.Schedule = getSchedule(vals);
            }
            catch(Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "GetSchedule", ex.Message);
                LogControl.WriteLog(LogType.ERR, "GetSchedule", ex.StackTrace);
            }

            return sch;
        }

        [WebMethod]
        public Holiday[] GetHoliDay(string day)
        {
            Holiday[] ret = null;
            try
            {
                ret = getHoliList(day);

                IOrderedEnumerable<Holiday> sortList = ret.OrderBy(rec => rec.SDay).ThenByDescending(rec => rec.EDay);

                ret = sortList.ToArray();
            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "GetSchedule", ex.Message);
                LogControl.WriteLog(LogType.ERR, "GetSchedule", ex.StackTrace);
            }

            return ret;
        }

        [WebMethod]
        public void SetDocUserMemo(string id, string userid, string memo)
        {
            try
            {
                string dir = ConfigurationManager.AppSettings["MemoDir"];
                string mfile = Path.Combine(dir, userid + "_" + id + ".txt");

                using (var sw = new StreamWriter(mfile, false, Encoding.GetEncoding("Shift-JIS")))
                    sw.WriteLine(memo);
            }
            catch(Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "SetDocUserMemo", ex.Message);
                LogControl.WriteLog(LogType.ERR, "SetDocUserMemo", ex.StackTrace);
            }
        }

        [WebMethod]
        public void SetUserMemo(string userid, string memo)
        {
            try
            {
                string dir = ConfigurationManager.AppSettings["UMemoDir"];
                string mfile = Path.Combine(dir, userid + ".txt");

                using (var sw = new StreamWriter(mfile, false, Encoding.GetEncoding("Shift-JIS")))
                    sw.WriteLine(memo);
            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "SetUserMemo", ex.Message);
                LogControl.WriteLog(LogType.ERR, "SetUserMemo", ex.StackTrace);
            }
        }

        [WebMethod]
        public string[] GetMailList(string docid)
        {
            List<string> ret = new List<string>();

            try
            {
                var dir = ConfigurationManager.AppSettings["MailDir"];

                var Afiles = Directory.GetFiles(dir);

                if (!string.IsNullOrEmpty(docid))
                {
                    dir = Path.Combine(dir, docid);

                    if(Directory.Exists(dir))
                    {
                        var Bfiles = Directory.GetFiles(dir);

                        foreach (var fi in Bfiles)
                            ret.Add(Path.GetFileNameWithoutExtension(fi));
                    }
                }

                foreach (var fi in Afiles)
                    ret.Add(Path.GetFileNameWithoutExtension(fi));

            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "GetMailList", ex.Message);
                LogControl.WriteLog(LogType.ERR, "GetMailList", ex.StackTrace);
            }

            return ret.ToArray();
        }

        [WebMethod]
        public string[] GetMailMaster(string docid)
        {
            List<string> ret = new List<string>();

            try
            {
                var dir = ConfigurationManager.AppSettings["MailMstDir"];

                var files = Directory.GetFiles(dir, docid + ".txt");

                if(files.Length > 0)
                    using (var sr = new StreamReader(files[0]))
                        ret.AddRange(sr.ReadLine().Split(','));

            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "GetMailMaster", ex.Message);
                LogControl.WriteLog(LogType.ERR, "GetMailMaster", ex.StackTrace);
            }

            return ret.ToArray();
        }

        [WebMethod]
        public Mail GetMailData(string title, string cd, string docid)
        {
            Mail ret = new Mail();
            try
            {
                string dir = ConfigurationManager.AppSettings["MailDir"];
                string mfile = Path.Combine(dir, title + ".txt");

                if(!File.Exists(mfile))
                {
                    dir = Path.Combine(dir, docid);
                    mfile = Path.Combine(dir, title + ".txt");
                }

                using (var sr = new StreamReader(mfile, Encoding.GetEncoding("Shift-JIS")))
                {
                    ret.Title = sr.ReadLine();
                    ret.Bcc = sr.ReadLine();

                    while (!sr.EndOfStream)
                    {
                        ret.Main += sr.ReadLine();
                        ret.Main += "\r\n";
                    }
                }

                string cnt = getReportData(cd);

                ret.Main = ret.Main.Replace("@2", cd);
                ret.Main = ret.Main.Replace("@3", cnt);

                if(ret.Main.IndexOf("@4") >= 0)
                {
                    cnt = getReportData_ModCountA(cd);
                    ret.Main = ret.Main.Replace("@4", cnt);
                    cnt = getReportData_ModCountB(cd);
                    ret.Main = ret.Main.Replace("@5", cnt);
                }
                if (ret.Main.IndexOf("@6") >= 0)
                {
                    cnt = getReportData_ModCountC(cd);
                    ret.Main = ret.Main.Replace("@6", cnt);
                }
            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "GetMailData", ex.Message);
                LogControl.WriteLog(LogType.ERR, "GetMailData", ex.StackTrace);
            }

            return ret;
        }


        [WebMethod]
        public void SendMail(string[] values)
        {
            try
            {
                string dir = ConfigurationManager.AppSettings["MailOutDir"];
                string mast = ConfigurationManager.AppSettings["MailFile"];
                string main = "";

                using (var sr = new StreamReader(mast, Encoding.GetEncoding("UTF-8")))
                    main = sr.ReadToEnd();

                string[] tos = values[0].Split(',');
                string[] ccs = values[1].Split(',');
                string[] bccs = values[2].Split(',');

                var add = "";

                foreach (var to in tos)
                    add += "<to>" + to + "</to>";
                foreach (var cc in ccs)
                    add += "<cc>" + cc + "</cc>";
                foreach (var bcc in bccs)
                    add += "<bcc>" + bcc + "</bcc>";

                main = main.Replace("@0", values[3]);
                main = main.Replace("@1", values[4]);
                main = main.Replace("@2", add);

                var fName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xml";

                using (var sw = new StreamWriter(Path.Combine(dir, fName), false, Encoding.GetEncoding("UTF-8")))
                    sw.WriteLine(main);

            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "GetMailData", ex.Message);
                LogControl.WriteLog(LogType.ERR, "GetMailData", ex.StackTrace);
            }
        }


        [WebMethod]
        public void SetEditSchedule(string userid, string[] vals)
        {
            try
            {
                string dir = ConfigurationManager.AppSettings["UserSchDir"];
                string id = vals[0];

                if(string.IsNullOrEmpty(id))
                {
                    id = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    vals[0] = id;
                }

                string[] preVals = null;

                string mfile = Path.Combine(dir, id + "@" + vals[2].Replace("/","") + ".txt");
                if (File.Exists(mfile))
                    using (var sr = new StreamReader(mfile, Encoding.GetEncoding("Shift-JIS")))
                        preVals = sr.ReadLine().Split('@');

                string val = "";

                val += userid;
                val += "@";
                val += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                for (int i = 0; i < vals.Length; i++)
                {
                    val += "@";
                    val += vals[i];
                }
                if(preVals != null && (vals.Length + 2) < preVals.Length)
                {
                    for (int i = vals.Length + 2; i < preVals.Length; i++)
                    {
                        val += "@";
                        val += preVals[i];
                    }
                }

                using (var sw = new StreamWriter(mfile, false, Encoding.GetEncoding("Shift-JIS")))
                    sw.WriteLine(val);

            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "SetEditSchedule", ex.Message);
                LogControl.WriteLog(LogType.ERR, "SetEditSchedule", ex.StackTrace);
            }
        }

        [WebMethod]
        public void DeleteHoliday(string sid)
        {
            try
            {
                string maindir = ConfigurationManager.AppSettings["HoliSchDir"];
                string mfile = Path.Combine(maindir, sid + ".txt");
                if (File.Exists(mfile))
                    File.Delete(mfile);
            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "DeleteHoliday", ex.Message);
                LogControl.WriteLog(LogType.ERR, "DeleteHoliday", ex.StackTrace);
            }
        }

        [WebMethod]
        public void DeleteSchedule(string sid, string subid, string date)
        {
            try
            {
                if(!string.IsNullOrEmpty(subid))
                {
                    string dir = ConfigurationManager.AppSettings["SubSchDir"];
                    dir = Path.Combine(dir, sid);

                    string file = Path.Combine(dir, subid + "@" + date + ".txt");

                    if(File.Exists(file))
                        File.Delete(file);
                }
                else
                {
                    string maindir = ConfigurationManager.AppSettings["UserSchDir"];
                    string mfile = Path.Combine(maindir, sid + "@" + date + ".txt");
                    if (File.Exists(mfile))
                        File.Delete(mfile);


                    string dir = ConfigurationManager.AppSettings["SubSchDir"];
                    dir = Path.Combine(dir, sid);

                    string[] files = Directory.GetFiles(dir, "*@" + date + ".txt");

                    foreach(var file in files){
                        if (File.Exists(file))
                            File.Delete(file);
                    }
                }
            }
            catch(Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "DeleteSchedule", ex.Message);
                LogControl.WriteLog(LogType.ERR, "DeleteSchedule", ex.StackTrace);
            }
        }

        [WebMethod]
        public void SetSubSchedule(string userid, string[] vals)
        {
            try
            {
                string dir = ConfigurationManager.AppSettings["SubSchDir"];
                string baseId = vals[0];

                string id = vals[vals.Length - 1];

                if (string.IsNullOrEmpty(id))
                {
                    id = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    vals[vals.Length - 1] = id;
                }

                dir = Path.Combine(dir, baseId);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                string mfile = Path.Combine(dir, id + "@" + vals[2].Replace("/", "") + ".txt");
                string val = "";

                val += userid;
                val += "@";
                val += DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                for (int i = 0; i < vals.Length; i++)
                {
                    val += "@";
                    val += vals[i];
                }

                using (var sw = new StreamWriter(mfile, false, Encoding.GetEncoding("Shift-JIS")))
                    sw.WriteLine(val);

            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "SetEditSchedule", ex.Message);
                LogControl.WriteLog(LogType.ERR, "SetEditSchedule", ex.StackTrace);
            }
        }

        [WebMethod]
        public void SetHoliSchedule(string[] vals)
        {
            try
            {
                string dir = ConfigurationManager.AppSettings["HoliSchDir"];
                string id = vals[0];

                if (string.IsNullOrEmpty(id))
                {
                    id = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    vals[0] = id;
                }

                string mfile = Path.Combine(dir, id + ".txt");
                string val = "";

                for (int i = 0; i < vals.Length; i++)
                {
                    if(i != 0)
                        val += "@";
                    val += vals[i];
                }

                using (var sw = new StreamWriter(mfile, false, Encoding.GetEncoding("Shift-JIS")))
                    sw.WriteLine(val);

            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "SetEditSchedule", ex.Message);
                LogControl.WriteLog(LogType.ERR, "SetEditSchedule", ex.StackTrace);
            }
        }

        [WebMethod]
        public string GetDocUserMemo(string id, string userid)
        {
            string ret = "";

            try
            {
                string dir = ConfigurationManager.AppSettings["MemoDir"];
                string mfile = Path.Combine(dir, userid + "_" + id + ".txt");

                if(File.Exists(mfile))
                    using (var sr = new StreamReader(mfile, Encoding.GetEncoding("Shift-JIS")))
                        ret = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "GetDocUserMemo", ex.Message);
                LogControl.WriteLog(LogType.ERR, "GetDocUserMemo", ex.StackTrace);

            }

            return ret;
        }

        [WebMethod]
        public string GetUserMemo(string userid)
        {
            string ret = "";

            try
            {
                string dir = ConfigurationManager.AppSettings["UMemoDir"];
                string mfile = Path.Combine(dir, userid + ".txt");

                if (File.Exists(mfile))
                    using (var sr = new StreamReader(mfile, Encoding.GetEncoding("Shift-JIS")))
                        ret = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "GetUserMemo", ex.Message);
                LogControl.WriteLog(LogType.ERR, "GetUserMemo", ex.StackTrace);

            }

            return ret;
        }

        private Day[] getSubSchedule(string id, string date)
        {
            List<Day> ret = new List<Day>();

            string dir = ConfigurationManager.AppSettings["SubSchDir"];

            dir = Path.Combine(dir, id);

            if(Directory.Exists(dir))
            {
                var files = Directory.GetFiles(dir, "*@" + date + ".txt");

                foreach(var fi in files)
                {
                    var val = "";
                    using (var sr = new StreamReader(fi, Encoding.GetEncoding("Shift-JIS")))
                        val = sr.ReadLine();

                    var vals = val.Split('@');

                    Day tmpDay = new Day();

                    tmpDay.Update = vals[1] + " " + vals[0];
                    tmpDay.SubDocID = vals[2];
                    tmpDay.Count = vals[3];
                    tmpDay.Date = vals[4].Replace("/","");

                    var tmpint = 0;
                    if (!int.TryParse(vals[5], out tmpint))
                        tmpDay.iEtime = 0;
                    else
                        tmpDay.iStime = Convert.ToInt32(vals[5]);

                    if (!int.TryParse(vals[7], out tmpint))
                        tmpDay.iEtime = 46;
                    else
                        tmpDay.iEtime = Convert.ToInt32(vals[7]);
                    tmpDay.DocID = vals[8];
                    tmpDay.ID = vals[9];

                    ret.Add(tmpDay);
                }

                IOrderedEnumerable<Day> sortList = ret.OrderBy(rec => rec.iStime);
                ret = sortList.ToList();
            }

            return ret.ToArray();
        }
        private Holiday[] getHoliList(string sDay)
        {
            List<Holiday> ret = new List<Holiday>();

            DateTime start = DateTime.Parse(sDay);
            DateTime end = start.AddDays(7);

            string dir = ConfigurationManager.AppSettings["HoliSchDir"];

            var files = Directory.GetFiles(dir, "*.txt");

            if (files.Length == 0)
                return ret.ToArray();

            foreach (var fi in files)
            {
                var vals = "";

                using (var sr = new StreamReader(fi, Encoding.GetEncoding("Shift-JIS")))
                    vals = sr.ReadLine();

                var dats = vals.Split('@');
                Holiday tmpDat = new Holiday();

                tmpDat.ID = dats[0];

                tmpDat.SDay = dats[1].Replace("/", "");
                tmpDat.EDay = dats[2].Replace("/", "");

                if (tmpDat.SDay.CompareTo(end.ToString("yyyyMMdd")) > 0)
                    continue;
                if (tmpDat.EDay.CompareTo(start.ToString("yyyyMMdd")) < 0)
                    continue;

                tmpDat.Comment = dats[3];
                tmpDat.Type = Convert.ToInt32(dats[4]);
                tmpDat.DocID = dats[5];

                ret.Add(tmpDat);
            }

            return ret.ToArray();
        }

        private Week[] getSchedule(string[] vals)
        {
            var weeks = new List<Week>();

            foreach (var val in vals)
            {
                var we = new Week();
                var data = val.Split(',');

                var tmpint = 0;
                if (!int.TryParse(data[0], out tmpint))
                    continue;

                we.Day = Convert.ToInt32(data[0]);

                if (int.TryParse(data[1], out tmpint))
                    we.Type = Convert.ToInt32(data[1]);
                else
                    we.Type = -1;

                if(we.Type == 2)
                {
                    weeks[weeks.Count - 1].SubType = 0;
                    weeks[weeks.Count - 1].Count += data[4];
                    continue;
                }
                else if(we.Type == 3)
                {
                    weeks[weeks.Count - 1].SubType = 1;
                    weeks[weeks.Count - 1].Count += data[4];
                    continue;
                }
                else
                {
                    we.SubType = -1;
                }

                if (int.TryParse(data[2], out tmpint))
                    we.Stime = Convert.ToInt32(data[2]);
                else
                    we.Stime = -1;

                if (int.TryParse(data[3], out tmpint))
                    we.Etime = Convert.ToInt32(data[3]);
                else
                    we.Etime = -1;

                we.Count = data[4];

                if(data.Length > 5)
                {
                    if (int.TryParse(data[5], out tmpint))
                        we.SubType = Convert.ToInt32(data[5]);
                    else
                        we.SubType = -1;
                }
                if (data.Length > 7)
                {
                    we.Sdate = data[6];
                    we.Edate = data[7];
                }else
                {
                    we.Sdate = "2018/05/01";
                    we.Edate = "";
                }

                weeks.Add(we);
            }

            IOrderedEnumerable<Week> sortList = weeks.OrderBy(rec => rec.Day).ThenBy(rec => rec.Sdate).ThenBy(rec => rec.Edate);

            return sortList.ToArray();
        }

        private string getLineData(int stime, int etime)
        {
            string ret = "";

            var st = 0;
            var et = 0;

            if (stime == 0)
                st = 0;
            else if (stime == -1)
                st = 0;
            else
            {
                st += 30;
                st += (stime - 1) * 15;
            }

            if (etime == 46)
                et = 780 - st;
            else
            {
                et = (etime - stime) * 15;
                if (stime == 0 || stime == -1)
                    et += 15;

            }

            ret = "left:" + st.ToString() + "px;top:0px;width:" + et.ToString() + "px";

            return ret;
        }

        private Dictionary<string, Day> getEditSchedule()
        {
            Dictionary<string, Day> ret = new Dictionary<string, Day>();

            var dir = ConfigurationManager.AppSettings["UserSchDir"];

            var files = Directory.GetFiles(dir , "*.txt");

            foreach(var file in files)
            {
                var vals = "";
                using (var sr = new StreamReader(file, Encoding.GetEncoding("Shift-JIS")))
                    vals = sr.ReadLine();

                var dats = vals.Split('@');
                Day tmpDat = new Day();

                tmpDat.Update = dats[1] + " " + dats[0];
                tmpDat.ID = dats[2];
                tmpDat.Count = dats[3];
                tmpDat.Date = dats[4].Replace("/", "");
                if (!string.IsNullOrEmpty(dats[5]))
                    tmpDat.iStime = Convert.ToInt32(dats[5]);
                else
                    tmpDat.iStime = -2;

                if (!string.IsNullOrEmpty(dats[7]))
                    tmpDat.iEtime = Convert.ToInt32(dats[7]);
                else
                    tmpDat.iEtime = -2;

                tmpDat.IsChange = Convert.ToInt32(dats[8]);
                tmpDat.IsAll = Convert.ToInt32(dats[9]);
                tmpDat.IsDelete = Convert.ToInt32(dats[10].Replace("\r\n", ""));

                if(dats.Length > 11)
                {
                    tmpDat.Type = Convert.ToInt32(dats[11]);
                    tmpDat.SubType = Convert.ToInt32(dats[12].Replace("\r\n", ""));
                    tmpDat.DocID = dats[13];
                }

                ret[tmpDat.ID + "@" + tmpDat.Date] = tmpDat;

            }

            return ret;
        }

        private string getReportData(string usercd)
        {
            string ret = "";

            using (var con = new AccDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
            {
                string userName = "";

                using (var cmd = con.CreateCommand())
                {
                    // SQL生成
                    StringBuilder selSQL = new StringBuilder();
                    selSQL.Append("SELECT");
                    selSQL.Append(" UserName");
                    selSQL.Append(" FROM");
                    selSQL.Append(" UserMst");

                    selSQL.Append(" WHERE");
                    selSQL.Append(" UserCD");
                    selSQL.Append(" =");
                    selSQL.Append(cmd.Add(usercd).ParameterName);

                    cmd.CommandText = selSQL.ToString();

                    // SQL実行
                    using (var dr = cmd.ExecuteReader())
                        // 該当データがある場合、返却値を設定
                        while (dr.Read())
                            userName = dr[0].ToString();
                }

                using (var cmd = con.CreateCommand())
                {
                    // SQL生成
                    StringBuilder selSQL = new StringBuilder();
                    selSQL.Append("SELECT");
                    selSQL.Append(" COUNT(SerialNo)");
                    selSQL.Append(" FROM");
                    selSQL.Append(" ReportTbl");

                    selSQL.Append(" WHERE");
                    selSQL.Append(" ReportStatus");
                    selSQL.Append(" =");
                    selSQL.Append(" 1");
                    selSQL.Append(" AND");
                    selSQL.Append(" RequestedPhysicianName");
                    selSQL.Append(" =");
                    selSQL.Append(cmd.Add(userName).ParameterName);

                    cmd.CommandText = selSQL.ToString();

                    // SQL実行
                    using (var dr = cmd.ExecuteReader())
                        // 該当データがある場合、返却値を設定
                        while (dr.Read())
                        {
                            ret = dr[0].ToString();
                        }
                }
            }

            return ret;
        }

        private string getReportData_ModCountA(string usercd)
        {
            string ret = "";

            using (var con = new AccDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
            {
                string userName = "";

                using (var cmd = con.CreateCommand())
                {
                    // SQL生成
                    StringBuilder selSQL = new StringBuilder();
                    selSQL.Append("SELECT");
                    selSQL.Append(" UserName");
                    selSQL.Append(" FROM");
                    selSQL.Append(" UserMst");

                    selSQL.Append(" WHERE");
                    selSQL.Append(" UserCD");
                    selSQL.Append(" =");
                    selSQL.Append(cmd.Add(usercd).ParameterName);

                    cmd.CommandText = selSQL.ToString();

                    // SQL実行
                    using (var dr = cmd.ExecuteReader())
                        // 該当データがある場合、返却値を設定
                        while (dr.Read())
                            userName = dr[0].ToString();
                }

                using (var cmd = con.CreateCommand())
                {
                    // SQL生成
                    StringBuilder selSQL = new StringBuilder();
                    selSQL.Append("SELECT");
                    selSQL.Append(" COUNT(SerialNo)");
                    selSQL.Append(" FROM");
                    selSQL.Append(" ReportTbl");

                    selSQL.Append(" WHERE");
                    selSQL.Append(" (");
                    selSQL.Append(" ReportStatus");
                    selSQL.Append(" =");
                    selSQL.Append(" 1");
                    selSQL.Append(" OR");
                    selSQL.Append(" SubStatus");
                    selSQL.Append(" =");
                    selSQL.Append(" 1");
                    selSQL.Append(" )");
                    selSQL.Append(" AND");
                    selSQL.Append(" RequestedPhysicianName");
                    selSQL.Append(" =");
                    selSQL.Append(cmd.Add(userName).ParameterName);
                    selSQL.Append(" AND");
                    selSQL.Append(" (");
                    selSQL.Append(" Modality");
                    selSQL.Append(" =");
                    selSQL.Append(cmd.Add("PT").ParameterName);
                    selSQL.Append(" OR");

                    selSQL.Append(" (");
                    selSQL.Append(" Modality");
                    selSQL.Append(" =");
                    selSQL.Append(cmd.Add("NM").ParameterName);
                    selSQL.Append(" AND");
                    selSQL.Append(" ReportReserve8");
                    selSQL.Append(" > 600");
                    selSQL.Append(" )");

                    selSQL.Append(" )");

                    cmd.CommandText = selSQL.ToString();

                    // SQL実行
                    using (var dr = cmd.ExecuteReader())
                        // 該当データがある場合、返却値を設定
                        while (dr.Read())
                        {
                            ret = dr[0].ToString();
                        }
                }
            }

            return ret;
        }

        private string getReportData_ModCountB(string usercd)
        {
            string ret = "";

            using (var con = new AccDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
            {
                string userName = "";

                using (var cmd = con.CreateCommand())
                {
                    // SQL生成
                    StringBuilder selSQL = new StringBuilder();
                    selSQL.Append("SELECT");
                    selSQL.Append(" UserName");
                    selSQL.Append(" FROM");
                    selSQL.Append(" UserMst");

                    selSQL.Append(" WHERE");
                    selSQL.Append(" UserCD");
                    selSQL.Append(" =");
                    selSQL.Append(cmd.Add(usercd).ParameterName);

                    cmd.CommandText = selSQL.ToString();

                    // SQL実行
                    using (var dr = cmd.ExecuteReader())
                        // 該当データがある場合、返却値を設定
                        while (dr.Read())
                            userName = dr[0].ToString();
                }

                using (var cmd = con.CreateCommand())
                {
                    // SQL生成
                    StringBuilder selSQL = new StringBuilder();
                    selSQL.Append("SELECT");
                    selSQL.Append(" COUNT(SerialNo)");
                    selSQL.Append(" FROM");
                    selSQL.Append(" ReportTbl");

                    selSQL.Append(" WHERE");
                    selSQL.Append(" ReportStatus");
                    selSQL.Append(" =");
                    selSQL.Append(" 1");
                    selSQL.Append(" AND");
                    selSQL.Append(" RequestedPhysicianName");
                    selSQL.Append(" =");
                    selSQL.Append(cmd.Add(userName).ParameterName);
                    selSQL.Append(" AND");
                    selSQL.Append(" Modality");
                    selSQL.Append(" =");
                    selSQL.Append(cmd.Add("NM").ParameterName);
                    selSQL.Append(" AND");
                    selSQL.Append(" ReportReserve8");
                    selSQL.Append(" < 600");

                    cmd.CommandText = selSQL.ToString();

                    // SQL実行
                    using (var dr = cmd.ExecuteReader())
                        // 該当データがある場合、返却値を設定
                        while (dr.Read())
                        {
                            ret = dr[0].ToString();
                        }
                }
            }

            return ret;
        }

        private string getReportData_ModCountC(string usercd)
        {
            string ret = "";

            using (var con = new AccDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
            {
                string userName = "";

                using (var cmd = con.CreateCommand())
                {
                    // SQL生成
                    StringBuilder selSQL = new StringBuilder();
                    selSQL.Append("SELECT");
                    selSQL.Append(" UserName");
                    selSQL.Append(" FROM");
                    selSQL.Append(" UserMst");

                    selSQL.Append(" WHERE");
                    selSQL.Append(" UserCD");
                    selSQL.Append(" =");
                    selSQL.Append(cmd.Add(usercd).ParameterName);

                    cmd.CommandText = selSQL.ToString();

                    // SQL実行
                    using (var dr = cmd.ExecuteReader())
                        // 該当データがある場合、返却値を設定
                        while (dr.Read())
                            userName = dr[0].ToString();
                }

                using (var cmd = con.CreateCommand())
                {
                    // SQL生成
                    StringBuilder selSQL = new StringBuilder();
                    selSQL.Append("SELECT");
                    selSQL.Append(" COUNT(SerialNo)");
                    selSQL.Append(" FROM");
                    selSQL.Append(" ReportTbl");

                    selSQL.Append(" WHERE");
                    selSQL.Append(" ReportStatus");
                    selSQL.Append(" =");
                    selSQL.Append(" 1");
                    selSQL.Append(" AND");
                    selSQL.Append(" RequestedPhysicianName");
                    selSQL.Append(" =");
                    selSQL.Append(cmd.Add(userName).ParameterName);
                    selSQL.Append(" AND");
                    selSQL.Append(" Modality");
                    selSQL.Append(" =");
                    selSQL.Append(cmd.Add("MR").ParameterName);
                    selSQL.Append(" AND");

                    selSQL.Append(" (");
                    selSQL.Append(" StudyBodyPart");
                    selSQL.Append(" LIKE ");
                    selSQL.Append(cmd.Add("%頭%").ParameterName);
                    selSQL.Append(" OR");
                    selSQL.Append(" StudyBodyPart");
                    selSQL.Append(" LIKE ");
                    selSQL.Append(cmd.Add("%脳%").ParameterName);
                    selSQL.Append(" )");

                    cmd.CommandText = selSQL.ToString();

                    // SQL実行
                    using (var dr = cmd.ExecuteReader())
                        // 該当データがある場合、返却値を設定
                        while (dr.Read())
                        {
                            ret = dr[0].ToString();
                        }
                }
            }

            return ret;
        }


        public class WebResult
        {
            public bool Result { get; set; }
            public string Message { get; set; }
        }
        public class WebUserList : WebResult
        {
            public UserDat[] Items { get; set; }
        }
        [DataContract]
        public class UserDat
        {
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public string ID { get; set; }
        }

        [DataContract]
        public class Mail
        {
            [DataMember]
            public string Title { get; set; }
            [DataMember]
            public string Bcc { get; set; }
            [DataMember]
            public string Main { get; set; }
        }

        [DataContract]
        public class Doctor
        {
            [DataMember]
            public string DocID { get; set; }
            [DataMember]
            public string DocName { get; set; }
            [DataMember]
            public string DocName_R { get; set; }
            [DataMember]
            public string DocName_H { get; set; }
            [DataMember]
            public string Comment { get; set; }
            [DataMember]
            public string Count { get; set; }
            [DataMember]
            public string Speed { get; set; }
            [DataMember]
            public string Main { get; set; }
            [DataMember]
            public BodyPart Body { get; set; }
            [DataMember]
            public Hospital Hosp { get; set; }
            [DataMember]
            public string Element { get; set; }
            [DataMember]
            public string Memo { get; set; }
            [DataMember]
            public string Color { get; set; }
            [DataMember]
            public string Color2 { get; set; }
        }

        [DataContract]
        public class DocSchedule
        {
            [DataMember]
            public Week[] Schedule { get; set; }
        }

        [DataContract]
        public class DayDat
        {
            [DataMember]
            public int Row { get; set; }
            [DataMember]
            public Day[] Data { get; set; }
        }


        [DataContract]
        public class Week
        {
            [DataMember]
            public int Day { get; set; }
            [DataMember]
            public int Type { get; set; }
            [DataMember]
            public string Count { get; set; }
            [DataMember]
            public int Stime { get; set; }
            [DataMember]
            public int Etime { get; set; }

            [DataMember]
            public int SubType { get; set; }
            [DataMember]
            public string Sdate { get; set; }
            [DataMember]
            public string Edate { get; set; }

        }

        [DataContract]
        public class Holiday
        {
            [DataMember]
            public string ID { get; set; }
            [DataMember]
            public string SDay { get; set; }
            [DataMember]
            public string EDay { get; set; }
            [DataMember]
            public int Type { get; set; }
            [DataMember]
            public string Comment { get; set; }
            [DataMember]
            public string DocID { get; set; }
        }

        [DataContract]
        public class Day
        {
            [DataMember]
            public string Serial { get; set; }
            [DataMember]
            public string ID { get; set; }
            [DataMember]
            public string DocID { get; set; }
            [DataMember]
            public string Date { get; set; }
            [DataMember]
            public string Count { get; set; }
            [DataMember]
            public string Style { get; set; }
            [DataMember]
            public string Stime { get; set; }
            [DataMember]
            public string Etime { get; set; }
            [DataMember]
            public int iStime { get; set; }
            [DataMember]
            public int iEtime { get; set; }
            [DataMember]
            public string Update { get; set; }
            [DataMember]
            public int Type { get; set; }
            [DataMember]
            public int SubType { get; set; }
            [DataMember]
            public Day[] SubDays { get; set; }
            [DataMember]
            public int IsDelete { get; set; }
            [DataMember]
            public int IsChange { get; set; }
            [DataMember]
            public int IsAll { get; set; }
            [DataMember]
            public string SubDocID { get; set; }

        }


        [DataContract]
        public class BodyPart
        {
            [DataMember]
            public Data[] OK { get; set; }
            [DataMember]
            public Data[] NG { get; set; }
            [DataMember]
            public Data[] Other { get; set; }

        }

        [DataContract]
        public class Hospital
        {
            [DataMember]
            public string[] OK { get; set; }
            [DataMember]
            public string[] NG { get; set; }
            [DataMember]
            public string[] Other { get; set; }
        }

        [DataContract]
        public class Data
        {
            [DataMember]
            public int Id { get; set; }
            [DataMember]
            public string Text { get; set; }
        }

        public class DayTimes
        {
            public Dictionary<int, string> ConstData { get; set; }

            public DayTimes()
            {
                ConstData = new Dictionary<int, string>();

                ConstData[0] = "8:30";
                ConstData[1] = "9:00";
                ConstData[2] = "9:15";
                ConstData[3] = "9:30";
                ConstData[4] = "9:45";
                ConstData[5] = "10:00";
                ConstData[6] = "10:15";
                ConstData[7] = "10:30";
                ConstData[8] = "10:45";
                ConstData[9] = "11:00";
                ConstData[10] = "11:15";
                ConstData[11] = "11:30";
                ConstData[12] = "11:45";
                ConstData[13] = "12:00";
                ConstData[14] = "12:15";
                ConstData[15] = "12:30";
                ConstData[16] = "12:45";
                ConstData[17] = "13:00";
                ConstData[18] = "13:15";
                ConstData[19] = "13:30";
                ConstData[20] = "13:45";
                ConstData[21] = "14:00";
                ConstData[22] = "14:15";
                ConstData[23] = "14:30";
                ConstData[24] = "14:45";
                ConstData[25] = "15:00";
                ConstData[26] = "15:15";
                ConstData[27] = "15:30";
                ConstData[28] = "15:45";
                ConstData[29] = "16:00";
                ConstData[30] = "16:15";
                ConstData[31] = "16:30";
                ConstData[32] = "16:45";
                ConstData[33] = "17:00";
                ConstData[34] = "17:15";
                ConstData[35] = "17:30";
                ConstData[36] = "17:45";
                ConstData[37] = "18:00";
                ConstData[38] = "18:15";
                ConstData[39] = "18:30";
                ConstData[40] = "18:45";
                ConstData[41] = "19:00";
                ConstData[42] = "19:15";
                ConstData[43] = "19:30";
                ConstData[44] = "19:45";
                ConstData[45] = "20:00";
                ConstData[46] = "20:00-";
            }
        }

    }

}
