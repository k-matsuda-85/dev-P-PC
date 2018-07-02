using CSV_ColumEdit.Class;
using LogController;
using System;
using System.IO;
using System.Text;

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

                var files_jpg = Directory.GetFiles(Config.inputDir, "*.jpg");
                foreach (var file in files_jpg)
                {
                    if (!CheckFileAcc(file))
                        continue;

                    string fileName = Path.GetFileName(file);
                    string outJpg = Path.Combine(Config.outputDir, fileName);

                    MoveFile(file, outJpg);
                }

                var files = Directory.GetFiles(Config.inputDir, "*.csv");

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
            string outFile = Path.Combine(Config.outputDir, fileName);
            string errFile = Path.Combine(Config.errDir, fileName);
            string pastFile = Path.Combine(Config.pastInDir, fileName);
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

            if (File.Exists(pastFile))
            {
                isPast = true;
                ret = PastCSVMove(pastFile);
                if(ret == 1)
                {
                    //除外対象のため、退避（過去所見・キー画像は内部で退避）
                    MoveBackFile(Config.backDir, workFile);
                    if (!string.IsNullOrEmpty(triFile) && File.Exists(triFile))
                        File.Delete(triFile);
                    LogControl.WriteLog(LogType.ORE, "読込処理", "過去所見 除外対象のため退避");
                    ret = 0;
                    return ret;
                }
                else if(ret != 0)
                {
                    MoveFile(workFile, errFile);
                    if (!string.IsNullOrEmpty(triFile) && File.Exists(triFile))
                        File.Delete(triFile);

                    return ret;
                }
            }

            try
            {
                string val = "";
                string writeVal = "";
                string masterFile = Config.MasterFile;

                using (var sr = new StreamReader(workFile, Encoding.GetEncoding("Shift-JIS")))
                    val = sr.ReadToEnd();

                string[] vals = val.Replace("\",\"", "@").Replace("\"\r\n", "").Replace("\"", "").Split('@');

                foreach (var dic in Config.MasterVals)
                {
                    if (vals.Length < Convert.ToInt32(dic.Value[0]))
                        continue;

                    if(vals[Convert.ToInt32(dic.Value[0])] == dic.Value[1])
                    {
                        var isDat = true;
                        foreach (var dic2 in Config.ExtMasterVals)
                        {
                            string key = dic.Key + "_" + dic2.Value[0];

                            if(dic2.Key.IndexOf(dic.Key + "_") > -1)
                            {
                                if (vals[Convert.ToInt32(dic2.Value[0])] != dic2.Value[1])
                                {
                                    isDat = false;
                                    break;
                                }
                            }
                        }

                        if (isDat)
                        {
                            masterFile = dic.Value[2];
                            break;
                        }
                    }
                }

                foreach (var dic in Config.EditVals)
                {
                    if (vals.Length < dic.Key)
                        continue;

                    vals[(int)dic.Key] = vals[(int)dic.Key].Replace(dic.Value[0], dic.Value[1]);
                }

                using (var sr = new StreamReader(masterFile, Encoding.GetEncoding("Shift-JIS")))
                    writeVal = sr.ReadToEnd();

                foreach (var log in Config.LogVals)
                    if (vals.Length >= log.Value + 1)
                        LogControl.WriteLog(LogType.ORE, "読込処理", log.Key + "：" + vals[log.Value]);

                for (uint i = 0; i < vals.Length; i++)
                    writeVal = writeVal.Replace("@" + i.ToString().PadLeft(2, '0'), vals[i]);

                if (File.Exists(outFile))
                    File.Delete(outFile);

                using (var sw = new StreamWriter(outFile, false, Encoding.GetEncoding("Shift-JIS")))
                    sw.Write(writeVal);

                foreach (var ext in Config.ExtPassVals)
                    if (vals.Length >= ext.Key + 1
                        && vals[ext.Key] == ext.Value)
                    {
                        isPast = true;
                        break;
                    }

                if (!isPast && Config.ExtPaths.Count > 0)
                {
                    foreach(var dic in Config.ExtPaths)
                    {
                        string extPath = Path.Combine(dic.Key, fileName);
                        using (var sw = new StreamWriter(extPath, false, Encoding.GetEncoding("Shift-JIS")))
                            sw.Write(writeVal);

                        if(!string.IsNullOrEmpty(dic.Value))
                        {
                            string extTriPath = Path.Combine(dic.Key, fileNameWE + "." + dic.Value);
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

        private static int PastCSVMove(string file)
        {
            int ret = -1;

            string fileNameWE = Path.GetFileNameWithoutExtension(file);
            string fileName = fileNameWE + "_P.csv";

            string workFile = Path.Combine(Config.workDir, fileName);
            string outFile = Path.Combine(Config.pastOutputDir, fileNameWE + ".csv");
            string errFile = Path.Combine(Config.errDir, fileName);
            string[] imgFiles = null;

            LogControl.WriteLog(LogType.ORE, "過去所見読込", file);

            MoveFile(file, workFile);

            try
            {
                if(Config.PastPassVals != null && Config.PastPassVals.Count > 0)
                {
                    string val = "";

                    using (var sr = new StreamReader(workFile, Encoding.GetEncoding("Shift-JIS")))
                        val = sr.ReadToEnd();

                    string[] vals = val.Replace("\",\"", "@").Replace("\"\r\n", "").Replace("\"", "").Split('@');

                    for(uint i = 0; i< vals.Length; i++)
                        if(Config.PastPassVals.ContainsKey(i))
                            if (vals[i].IndexOf(Config.PastPassVals[i]) >= 0)
                            {
                                ret = 1;
                                break;
                            }

                    if(ret == 1)
                    {
                        File.Delete(workFile);
                        return ret;
                    }
                }

                imgFiles = Directory.GetFiles(Config.imgInDir, fileNameWE + "*.*");

                foreach (var img in imgFiles)
                {
                    string distImg = Path.Combine(Config.imgOutputDir, Path.GetFileName(img));

                    MoveFile(img, distImg);
                    LogControl.WriteLog(LogType.ORE, "過去キー画像", distImg);
                }

                MoveFile(workFile, outFile);
                ret = 0;
            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ORE, "過去所見読込", "読み込みエラー");
                LogControl.WriteLog(LogType.ERR, "過去所見読込", ex.Message);
                LogControl.WriteLog(LogType.ERR, "過去所見読込", ex.StackTrace);
                MoveFile(workFile, errFile);
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
