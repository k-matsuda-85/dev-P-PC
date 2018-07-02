using CSV_ColumEdit.Class;
using LogController;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace CSV_ColumEdit
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string msg = "";
                msg = Config.InitConfig();
                if (!string.IsNullOrEmpty(msg))
                {
                    LogControl.WriteLog(LogType.ERR, "初期設定", msg);
                    return;
                }

                var files = Directory.GetFiles(Config.inputDir, "*.xml");

                foreach (var file in files)
                {
                    if (!CheckFileAcc(file))
                        continue;

                    EditFile(file);
                }
            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "メイン処理", ex.Message);
                LogControl.WriteLog(LogType.ERR, "メイン処理", ex.StackTrace);
            }
        }

        private static int EditFile(string file)
        {
            int ret = -1;

            string fileName = Path.GetFileName(file);
            string fileNameWE = Path.GetFileNameWithoutExtension(file);
            string workFile = Path.Combine(Config.workDir, fileName);
            string outFile = Path.Combine(Config.outputDir, Config.hospCd + fileNameWE + ".csv");
            string errFile = Path.Combine(Config.errDir, fileName);
            string pastFile = Path.Combine(Config.pastOutputDir, Config.hospCd + fileNameWE + ".csv");
            string triFile = "";

            if (!string.IsNullOrEmpty(Config.Trigger))
            {
                var triFiles = Directory.GetFiles(Config.inputDir, fileNameWE + "." + Config.Trigger);
                foreach (var fi in triFiles)
                {
                    triFile = fi;
                    break;
                }

                if (string.IsNullOrEmpty(triFile))
                {
                    // トリガーファイルがない場合読み飛ばし
                    ret = 1;
                    return ret;
                }
            }

            LogControl.WriteLog(LogType.ORE, "処理ファイル", "----------------------------------------------------");
            LogControl.WriteLog(LogType.ORE, "処理ファイル", file);

            MoveFile(file, workFile);

            bool isPast = false;

            try
            {
                Dictionary<string,string> val = new Dictionary<string, string>();
                string writeVal = "";
                string masterFile = Config.MasterFile;
                string pMastFile = Config.PastMasterFile;
                string rMastFile = Config.EditMasterFile;
                List<string> imgFiles = new List<string>();

                string tmpKey = "";

                using (var reader = new XmlTextReader(workFile))
                    while (reader.Read())
                        if (reader.NodeType == XmlNodeType.Element)
                            if (reader.LocalName == "name")
                            {
                                if (!string.IsNullOrEmpty(tmpKey))
                                {
                                    val[tmpKey] = "";
                                    tmpKey = "";
                                }

                                tmpKey = reader.ReadString();

                                if (string.IsNullOrEmpty(tmpKey))
                                    continue;
                            }
                            else if (reader.LocalName == "value")
                            {
                                string dat = reader.ReadString(); // 改行コードのため
                                val[tmpKey] = dat;
                                tmpKey = "";
                            }
                            else if (reader.LocalName == "filename")
                            {
                                if (imgFiles.Count < 6)
                                    imgFiles.Add(reader.ReadString());
                            }

                if (!string.IsNullOrEmpty(tmpKey))
                {
                    val[tmpKey] = "";
                    tmpKey = "";
                }

                foreach (var dic in Config.MasterVals)
                {
                    if (!val.ContainsKey(dic.Value[0]))
                        continue;

                    if(val[dic.Value[0]] == dic.Value[1])
                    {
                        masterFile = dic.Value[2];
                        break;
                    }
                }

                val["OrderNo"] = Config.hospCd + val["Order_No"];
                string reportFile = Path.Combine(Config.reportDir, val["Order_No"] + ".xml");

                val["PatientID"] = Config.hospCd + val["患者ID"];

                switch(val["患者性別"])
                {
                    case "1":
                        val["Sex"] = "M";
                        break;
                    case "3":
                        val["Sex"] = "F";
                        break;
                    case "2":
                        val["Sex"] = "O";
                        break;
                }

                if (val["緊急"] == "緊急")
                    val["Eフラグ"] = "1";
                else
                    val["Eフラグ"] = "0";

                if (val["依頼キャンセル"] == "1")
                    val["UdtCd"] = "3";
                else
                    val["UdtCd"] = "1";

                val["OrderCd"] = "3";

                var dateA = Convert.ToUInt32( DateTime.Now.ToString("yyyyMMdd"));
                var dateB = Convert.ToUInt32(val["患者生年月日"]);
                var age = (dateA - dateB) / 10000;
                val["Age"] = age.ToString();

                val["Date"] = val["検査日"].Substring(0, 8);
                val["Time"] = val["検査時間"].Substring(8, 6);

                foreach (var dic in Config.EditVals)
                {
                    if (!val.ContainsKey(dic.Key))
                        continue;

                    val[dic.Key] = val[dic.Key].Replace(dic.Value[0], dic.Value[1]);
                }

                foreach (var log in Config.LogVals)
                    if (val.ContainsKey(log.Value))
                        LogControl.WriteLog(LogType.ORE, "読込処理", log.Key + "：" + val[log.Value]);


                foreach (var ext in Config.PastPassVals)
                    if (val.ContainsKey(ext.Key) && val[ext.Key] == ext.Value)
                    {
                        isPast = true;
                        break;
                    }

                if(isPast)
                {
                    LogControl.WriteLog(LogType.ORE, "読込処理", "過去所見出力");

                    val["OrderCd"] = "4";

                    if (File.Exists(reportFile))
                    {
                        LogControl.WriteLog(LogType.ORE, "過去所見", "ProMed出力所見のため、関連ファイル削除");
                        foreach (var im in imgFiles)
                        {
                            string inImgPath = Path.Combine(Config.inputDir, im);

                            if (File.Exists(inImgPath))
                            {
                                LogControl.WriteLog(LogType.ORE, "キー画像", "削除：" + inImgPath);

                                File.Delete(inImgPath);
                            }
                        }

                        MoveBackFile(Config.backDir, workFile);

                        if (!string.IsNullOrEmpty(triFile) && File.Exists(triFile))
                            File.Delete(triFile);

                        return 1;
                    }

                    LogControl.WriteLog(LogType.ORE, "キー画像", "枚数：" + imgFiles.Count);
                    val["ImgCnt"] = imgFiles.Count.ToString();
                    for (var i = 0; i < 6; i++)
                    {
                        if (imgFiles.Count <= i)
                        {
                            val["Img" + (i + 1)] = "";
                            continue;
                        }
                        else
                        {
                            val["Img" + (i + 1)] = (i + 1).ToString();
                            string imgPath = Path.Combine(Config.imgOutputDir, Config.hospCd + fileNameWE + ".csv" + val["OrderNo"] + "_" + i + ".jpg");
                            string inImgPath = Path.Combine(Config.inputDir, imgFiles[i]);

                            if (File.Exists(inImgPath))
                            {
                                LogControl.WriteLog(LogType.ORE, "キー画像", inImgPath + "→" + imgPath);
                                if (File.Exists(imgPath))
                                    File.Delete(imgPath);
                                File.Move(inImgPath, imgPath);
                            }
                        }
                    }

                    if(imgFiles.Count > 6)
                    {
                        LogControl.WriteLog(LogType.ORE, "キー画像", "枚数超過のため削除");
                        foreach (var im in imgFiles)
                        {
                            string inImgPath = Path.Combine(Config.inputDir, im);

                            if (File.Exists(inImgPath))
                            {
                                LogControl.WriteLog(LogType.ORE, "キー画像", "削除：" + inImgPath);

                                File.Delete(inImgPath);
                            }
                        }
                    }

                    using (var sr = new StreamReader(Config.PastMasterFile, Encoding.GetEncoding("Shift-JIS")))
                        writeVal = sr.ReadToEnd();

                    foreach (var vald in val)
                        writeVal = writeVal.Replace("@" + vald.Key, vald.Value);

                    using (var sw = new StreamWriter(pastFile, false, Encoding.GetEncoding("Shift-JIS")))
                        sw.Write(writeVal);
                }
                else if(val["UdtCd"] != "3")
                {
                    if(imgFiles.Count > 0)
                    {
                        string imgPath = Path.Combine(Config.outputDir, Config.hospCd + fileNameWE + ".jpg");
                        string checkPath = Path.Combine(Config.reportDir, val["Order_No"] + ".jpg");
                        string inImgPath = Path.Combine(Config.inputDir, imgFiles[0]);

                        LogControl.WriteLog(LogType.ORE, "キー画像", inImgPath + "→" + imgPath);

                        if (File.Exists(checkPath))
                            File.Delete(checkPath);
                        if (File.Exists(imgPath))
                            File.Delete(imgPath);

                        File.Copy(inImgPath, checkPath);
                        File.Move(inImgPath, imgPath);

                        val["JPEG_TEXT"] = Config.JPEG_TEXT;
                    }else
                    {
                        val["JPEG_TEXT"] = "";
                    }

                    val["OrderCd"] = "3";
                    if (File.Exists(reportFile))
                    {
                        val["UdtCd"] = "2";
                    }

                    using (var sr = new StreamReader(Config.EditMasterFile, Encoding.GetEncoding("UTF-8")))
                        writeVal = sr.ReadToEnd();

                    foreach (var vald in val)
                        writeVal = writeVal.Replace("@" + vald.Key, vald.Value);

                    using (var sw = new StreamWriter(reportFile, false, Encoding.GetEncoding("UTF-8")))
                        sw.Write(writeVal);
                    LogControl.WriteLog(LogType.ORE, "読込処理", "所見連携用ファイル：" + reportFile);

                }

                using (var sr = new StreamReader(masterFile, Encoding.GetEncoding("Shift-JIS")))
                writeVal = sr.ReadToEnd();

                foreach (var vald in val)
                    writeVal = writeVal.Replace("@" + vald.Key, vald.Value);

                if (File.Exists(outFile))
                    File.Delete(outFile);

                using (var sw = new StreamWriter(outFile, false, Encoding.GetEncoding("Shift-JIS")))
                    sw.Write(writeVal);

                if (!isPast && Config.ExtPaths.Count > 0 && val["UdtCd"] == "1")
                {
                    foreach(var dic in Config.ExtPaths)
                    {
                        string extPath = Path.Combine(dic.Key, Config.hospCd + fileNameWE + ".csv");
                        using (var sw = new StreamWriter(extPath, false, Encoding.GetEncoding("Shift-JIS")))
                            sw.Write(writeVal);

                        if(!string.IsNullOrEmpty(dic.Value))
                        {
                            string extTriPath = Path.Combine(dic.Key, Config.hospCd + fileNameWE + "." + dic.Value);
                            using (var sw = new StreamWriter(extTriPath, false, Encoding.GetEncoding("Shift-JIS")))
                                sw.Close();
                        }
                        LogControl.WriteLog(LogType.ORE, "連携出力", extPath);
                    }
                }

                MoveBackFile(Config.backDir, workFile);

                if (!string.IsNullOrEmpty(triFile) && File.Exists(triFile))
                    File.Delete(triFile);
                LogControl.WriteLog(LogType.ORE, "変換出力完了", outFile);

            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ORE, "読込処理", "読み込みエラー");
                LogControl.WriteLog(LogType.ERR, "読込処理", e.Message);
                LogControl.WriteLog(LogType.ERR, "読込処理", e.StackTrace);
                MoveFile(workFile, errFile);
                if (!string.IsNullOrEmpty(triFile) && File.Exists(triFile))
                    File.Delete(triFile);
            }

            return ret;
        }

        private static int PastImageMove(string file)
        {
            int ret = -1;


            return ret;
        }

        private static void MoveFile(string srcFile, string distFile)
        {

            if (File.Exists(srcFile))
            {
                if (File.Exists(distFile))
                    File.Delete(distFile);

                File.Move(srcFile, distFile);
            }

            return;
        }

        private static void CopyFile(string srcFile, string distFile)
        {

            if (File.Exists(srcFile))
            {
                if (File.Exists(distFile))
                    File.Delete(distFile);

                File.Copy(srcFile, distFile);
            }

            return;
        }

        private static void MoveBackFile(string dir, string distFile)
        {
            if (string.IsNullOrEmpty(dir))
                File.Delete(distFile);

            DateTime nowTime = DateTime.Now;

            string path = Path.Combine(dir, nowTime.ToString("yyyyMMdd"));

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string imgName = Path.GetFileName(distFile);
            string imgPath = Path.Combine(path, imgName);

            if (File.Exists(imgPath))
                File.Delete(imgPath);

            File.Move(distFile, imgPath);
            return;
        }


        private static bool CheckFileAcc(string path)
        {
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

    }
}
