using CSV_ColumEdit.Class;
using LogController;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
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

                var files = Directory.GetFiles(Config.inputDir, "*.xls");

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
            string fileNameWE = Path.GetFileNameWithoutExtension(file) + "_Read.xls";
            string workFile = Path.Combine(Config.inputDir, fileNameWE);
            string outExFile = Path.Combine(Config.excelOutputDir, fileName);

            int[] Readrows = null;

            Readrows = CheckExcelData(file);

            if(Readrows == null || Readrows.Length == 0)
                return ret;

            LogControl.WriteLog(LogType.ORE, "処理ファイル", "********************************************************************");
            LogControl.WriteLog(LogType.ORE, "処理ファイル", file);

            File.Move(file, workFile);
            LogControl.WriteLog(LogType.ORE, "処理ファイル", "リネーム："+ fileName + " → " + fileNameWE);

            //エクセルの宣言
            Application excelApp = null;
            Workbook masterBook = null;
            Worksheet masterSheet = null;

            LogControl.WriteLog(LogType.ORE, "処理ファイル", "処理オーダー：" + Readrows.Length + "件");
            foreach (var index in Readrows)
            {
                try
                {
                    //Excelのアプリケーションを起動
                    excelApp = new Application();
                    //Excelのブックをオープンする
                    masterBook = excelApp.Workbooks.Open(workFile);
                    masterSheet = (Worksheet)masterBook.Sheets[1];


                    LogControl.WriteLog(LogType.ORE, "オーダー解析", "----------------------------------------------------");
                    string orderNo = Config.HospCd + DateTime.Now.ToString("yyMMddHHmmssfff"); 
                    string writeVal = "";
                    string masterFile = Config.MasterFile;
                    string outFile = Path.Combine(Config.outputDir, orderNo + ".csv");

                    Dictionary<uint, string> vals = new Dictionary<uint, string>();

                    Range range = null;

                    foreach (var dic in Config.MasterVals)
                    {
                        range = (Range)masterSheet.Cells[index, (int)dic.Value];
                        if (range.Value2 == null || range.Value2.ToString() == "")
                            continue;

                        vals[dic.Key] = range.Value2.ToString();
                    }

                    using (var sr = new StreamReader(masterFile, Encoding.GetEncoding("Shift-JIS")))
                        writeVal = sr.ReadToEnd();

                    foreach (var log in Config.LogVals)
                        if (vals.Keys.Count >= log.Value + 1)
                            LogControl.WriteLog(LogType.ORE, "読込処理", log.Key + "：" + vals[log.Value]);

                    foreach(var setVal in vals)
                        writeVal = writeVal.Replace("@" + setVal.Key.ToString().PadLeft(2, '0'), setVal.Value.Trim());

                    range = (Range)masterSheet.Cells[index, (int)Config.PatIDCol];
                    writeVal = writeVal.Replace("@PatID", Config.HospCd + range.Value2.ToString().Trim().PadLeft(8, '0'));
                    //writeVal = writeVal.Replace("@PatID", Config.HospCd + range.Value2.ToString().Trim());

                    range = (Range)masterSheet.Cells[index, (int)Config.PatSexCol];
                    switch(range.Value2.ToString().Trim())
                    {
                        case "男":
                            writeVal = writeVal.Replace("@PatSex", "M");
                            break;
                        case "女":
                            writeVal = writeVal.Replace("@PatSex", "F");
                            break;
                        case "M":
                            writeVal = writeVal.Replace("@PatSex", "M");
                            break;
                        case "F":
                            writeVal = writeVal.Replace("@PatSex", "F");
                            break;
                        default:
                            writeVal = writeVal.Replace("@PatSex", "O");
                            break;
                    }

                    DateTime birth;
                    DateTime date;

                    range = (Range)masterSheet.Cells[index, (int)Config.BirthDayCol];
                    var dat = range.Value.ToString().Trim().Split(' ')[0].Split('/');
                    birth = new DateTime(Convert.ToInt32(dat[0]), Convert.ToInt32(dat[1]), Convert.ToInt32(dat[2]));

                    range = (Range)masterSheet.Cells[index, (int)Config.StudyDateCol];

                    if (string.IsNullOrEmpty(Config.SD_Pre))
                        dat = range.Value.ToString().Trim().Split(' ')[0].Split('/');
                    else
                        dat = (Config.SD_Pre + range.Value.ToString().Trim().Split(' ')[0]).Split('/');

                    date = new DateTime(Convert.ToInt32(dat[0]), Convert.ToInt32(dat[1]), Convert.ToInt32(dat[2]));


                    var age = GetAge(birth, date);

                    writeVal = writeVal.Replace("@Birth", birth.ToString("yyyyMMdd"));
                    writeVal = writeVal.Replace("@StudyDate", date.ToString("yyyyMMdd"));
                    writeVal = writeVal.Replace("@PatAge", age.ToString());

                    writeVal = writeVal.Replace("@OrderNo", orderNo);

                    var receptDate = "";

                    DateTime dt = DateTime.Now;
                    DayOfWeek dow = dt.DayOfWeek;

                    var isDat = 0;

                    while (isDat <= Config.ReturnDay)
                    {
                        switch (dow)
                        {
                            case DayOfWeek.Sunday:
                            case DayOfWeek.Saturday:
                                break;
                            default:
                                using (var sr = new StreamReader(Config.extraFile))
                                {
                                    var isDay = false;
                                    while (!sr.EndOfStream)
                                    {
                                        var tmp = sr.ReadLine();
                                        if (tmp == dt.ToString("yyyy/MM/dd"))
                                        {
                                            isDay = true;
                                            break;
                                        }
                                    }

                                    if (!isDay)
                                        isDat++;
                                }
                                break;
                        }

                        receptDate = dt.ToString("M月d日");
                        dt = dt.AddDays(1);
                        dow = dt.DayOfWeek;
                    }

                    writeVal = writeVal.Replace("@Recept", receptDate + "までに返却");

                    if (File.Exists(outFile))
                        File.Delete(outFile);

                    using (var sw = new StreamWriter(outFile, false, Encoding.GetEncoding("Shift-JIS")))
                        sw.Write(writeVal);

                    if (Config.ExtPaths.Count > 0)
                    {
                        foreach (var dic in Config.ExtPaths)
                        {
                            string extPath = Path.Combine(dic.Key, orderNo + ".csv");
                            using (var sw = new StreamWriter(extPath, false, Encoding.GetEncoding("Shift-JIS")))
                                sw.Write(writeVal);

                            if (!string.IsNullOrEmpty(dic.Value))
                            {
                                string extTriPath = Path.Combine(dic.Key, orderNo + "." + dic.Value);
                                using (var sw = new StreamWriter(extTriPath, false, Encoding.GetEncoding("Shift-JIS")))
                                    sw.Close();
                            }
                            LogControl.WriteLog(LogType.ORE, "連携出力", extPath);
                        }
                    }

                    MoveBackFile(Config.backDir, outFile, writeVal);

                    LogControl.WriteLog(LogType.ORE, "オーダー出力", outFile);
                }
                catch (Exception e)
                {
                    LogControl.WriteLog(LogType.ORE, "オーダー解析", "読み込みエラー");
                    LogControl.WriteLog(LogType.ERR, "オーダー解析", e.Message);
                    LogControl.WriteLog(LogType.ERR, "オーダー解析", e.StackTrace);
                }
                finally
                {
                    masterSheet = null;
                    if (masterBook != null)
                        masterBook.Close(false);
                    if (excelApp != null)
                        excelApp.Quit();
                }
            }

            File.Move(workFile, outExFile);
            LogControl.WriteLog(LogType.ORE, "オーダー解析終了", "----------------------------------------------------");
            LogControl.WriteLog(LogType.ORE, "処理ファイル", "リネーム解除：" + fileNameWE+ " → " + fileName);

            return ret;
        }
        /// <summary>
        /// 生年月日から年齢を計算する
        /// </summary>
        /// <param name="birthDate">生年月日</param>
        /// <param name="today">現在の日付</param>
        /// <returns>年齢</returns>
        private static int GetAge(DateTime birthDate, DateTime today)
        {
            int age = today.Year - birthDate.Year;
            //誕生日がまだ来ていなければ、1引く
            if (today.Month < birthDate.Month ||
                (today.Month == birthDate.Month &&
                today.Day < birthDate.Day))
            {
                age--;
            }

            return age;
        }

        private static int[] CheckExcelData(string path)
        {
            List<int> ret = new List<int>();

            //エクセルの宣言
            Application excelApp = null;
            Workbook masterBook = null;
            Worksheet masterSheet = null;

            try
            {

                //Excelのアプリケーションを起動
                excelApp = new Application();
                //Excelのブックをオープンする
                masterBook = excelApp.Workbooks.Open(path);
                masterSheet = (Worksheet)masterBook.Sheets[1];

                int lastRow = masterSheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Row;

                for (int i = 3; i <= lastRow; i++)
                {
                    Range range = (Range)masterSheet.Cells[i, Config.StudyDateCol];
                    if (range.Value2 == null || range.Value2.ToString() == "")
                        continue;

                    ret.Add(i);
                }
            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ORE, "読込処理", "読み込みエラー");
                LogControl.WriteLog(LogType.ERR, "読込処理", ex.Message);
                LogControl.WriteLog(LogType.ERR, "読込処理", ex.StackTrace);
            }
            finally
            {
                masterSheet = null;
                if (masterBook != null)
                    masterBook.Close(false);
                if (excelApp != null)
                    excelApp.Quit();
            }

            return ret.ToArray();
        }

        private static int GetRowIndex(Worksheet ws, int index, int col, string val)
        {
            Range range = null;
            for (int i = 1; i <= index; i++)
            {
                range = (Range)ws.Cells[i, col];

                if (range.Value2.ToString() == val)
                {
                    return i;
                }
            }

            return 0;
        }

        private static int GetColIndex(Worksheet ws, int row, string val)
        {
            int lastColumn = ws.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Column;

            Range range = null;
            for (int i = 1; i <= lastColumn; i++)
            {
                range = (Range)ws.Cells[row, i];

                if (range.Value2 == null)
                    continue;

                if (range.Value2.ToString() == val)
                {
                    return i;
                }
            }

            return 0;
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

        private static void MoveBackFile(string dir, string outFile, string val)
        {
            string distFile = Path.Combine(dir, Path.GetFileName(outFile));

            DateTime nowTime = DateTime.Now;

            using (var sw = new StreamWriter(distFile, false, Encoding.GetEncoding("Shift-JIS")))
                sw.Write(val);

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
