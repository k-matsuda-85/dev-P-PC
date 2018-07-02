using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace CSV_ColumEdit.Class
{
    public static class Config
    {
        public static string inputDir;
        public static string outputDir;
        public static string excelOutputDir;
        public static string workDir;
        public static string errDir;

        public static string backDir;

        public static string imgInDir;
        public static string pastInDir;

        public static string pastOutputDir;
        public static string imgOutputDir;

        public static string Trigger;

        public static string SD_Pre;


        public static int ReturnDay;
        public static string extraFile;

        public static string MasterFile;

        public static uint PatIDCol;
        public static uint PatSexCol;
        public static uint StudyDateCol;
        public static uint BirthDayCol;
        public static string HospCd;

        public static Dictionary<uint, uint> MasterVals;

        public static Dictionary<uint, string[]> EditVals;

        public static Dictionary<string, uint> LogVals;

        public static Dictionary<string, string> ExtPaths;

        public static Dictionary<uint, string> ExtPassVals;

        public static Dictionary<uint, string> PastPassVals;

        public static string InitConfig()
        {
            string ret = "";

            HospCd = ConfigurationManager.AppSettings["HospCd"];
            inputDir = ConfigurationManager.AppSettings["InputDir"];
            if (string.IsNullOrEmpty(inputDir))
            {
                ret = "取込フォルダを指定してください。";
                return ret;
            }
            else if (!Directory.Exists(inputDir))
            {
                ret = "取込フォルダが存在しません";
                return ret;
            }

            outputDir = ConfigurationManager.AppSettings["OutputDir"];
            if (string.IsNullOrEmpty(outputDir))
            {
                ret = "出力フォルダを指定してください。";
                return ret;
            }
            else if (!Directory.Exists(outputDir))
            {
                ret = "出力フォルダが存在しません";
                return ret;
            }

            excelOutputDir = ConfigurationManager.AppSettings["ExcelOutputDir"];
            if (string.IsNullOrEmpty(excelOutputDir))
            {
                ret = "Excel出力フォルダを指定してください。";
                return ret;
            }
            else if (!Directory.Exists(excelOutputDir))
            {
                ret = "Excel出力フォルダが存在しません";
                return ret;
            }

            MasterFile = ConfigurationManager.AppSettings["MasterFile"];
            if (string.IsNullOrEmpty(MasterFile))
            {
                ret = "マスターファイルを指定してください。";
                return ret;
            }
            else if (!File.Exists(MasterFile))
            {
                ret = "マスターファイルが存在しません";
                return ret;
            }

            uint.TryParse(ConfigurationManager.AppSettings["PatIDCol"], out PatIDCol);
            uint.TryParse(ConfigurationManager.AppSettings["PatSexCol"], out PatSexCol);
            uint.TryParse(ConfigurationManager.AppSettings["StudyDateCol"], out StudyDateCol);
            uint.TryParse(ConfigurationManager.AppSettings["BirthDayCol"], out BirthDayCol);

            pastInDir = ConfigurationManager.AppSettings["PastInputDir"];
            if (!string.IsNullOrEmpty(pastInDir))
                if (!Directory.Exists(pastInDir))
                {
                    ret = "過去所見取込フォルダが存在しません";
                    return ret;
                }

            imgInDir = ConfigurationManager.AppSettings["ImgInputDir"];
            if (!string.IsNullOrEmpty(imgInDir))
                if (!Directory.Exists(imgInDir))
                {
                    ret = "キー画像取込フォルダが存在しません";
                    return ret;
                }

            pastOutputDir = ConfigurationManager.AppSettings["PastOutputDir"];
            if (!string.IsNullOrEmpty(pastOutputDir))
                if (!Directory.Exists(pastOutputDir))
                {
                    ret = "過去所見出力フォルダが存在しません";
                    return ret;
                }

            imgOutputDir = ConfigurationManager.AppSettings["ImgOutputDir"];
            if (!string.IsNullOrEmpty(imgOutputDir))
                if (!Directory.Exists(imgOutputDir))
                {
                    ret = "キー画像出力フォルダが存在しません";
                    return ret;
                }

            backDir = ConfigurationManager.AppSettings["BackDir"];
            if (!string.IsNullOrEmpty(backDir))
                if (!Directory.Exists(backDir))
                {
                    ret = "バックアップフォルダが存在しません";
                    return ret;
                }

            extraFile = ConfigurationManager.AppSettings["ExtraDayList"];
            if (!string.IsNullOrEmpty(extraFile))
                if (!File.Exists(extraFile))
                {
                    ret = "祝日一覧が存在しません。";
                    return ret;
                }

            ReturnDay = 2;
            int.TryParse(ConfigurationManager.AppSettings["ReturnDay"], out ReturnDay);

            LogVals = new Dictionary<string, uint>();
            int index = 0;

            while (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["LogName" + index]))
            {
                uint tmpCol = 0;
                if(!uint.TryParse(ConfigurationManager.AppSettings["LogCol" + index], out tmpCol))
                {
                    index++;
                    continue;
                }

                LogVals[ConfigurationManager.AppSettings["LogName" + index]] = tmpCol;
                index++;
            }

            ExtPaths = new Dictionary<string, string>();
            index = 0;

            while (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ExtOutputDir" + index]))
            {
                ExtPaths[ConfigurationManager.AppSettings["ExtOutputDir" + index]] = ConfigurationManager.AppSettings["ExtOutputTri" + index];
                index++;
            }

            ExtPassVals = new Dictionary<uint, string>();
            index = 0;

            while (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ExtPassCol" + index]))
            {
                uint tmpCol = 0;
                if (!uint.TryParse(ConfigurationManager.AppSettings["ExtPassCol" + index], out tmpCol))
                {
                    index++;
                    continue;
                }

                ExtPassVals[tmpCol] = ConfigurationManager.AppSettings["ExtPassVal" + index];
                index++;
            }

            PastPassVals = new Dictionary<uint, string>();
            index = 0;

            while (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["PastPassCol" + index]))
            {
                uint tmpCol = 0;
                if (!uint.TryParse(ConfigurationManager.AppSettings["PastPassCol" + index], out tmpCol))
                {
                    index++;
                    continue;
                }

                PastPassVals[tmpCol] = ConfigurationManager.AppSettings["PastPassVal" + index];
                index++;
            }

            MasterVals = new Dictionary<uint, uint>();
            index = 0;

            while (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CSVCol" + index]))
            {
                uint tmpCol = 0;
                uint tmpCol2 = 0;
                if (!uint.TryParse(ConfigurationManager.AppSettings["CSVCol" + index], out tmpCol))
                {
                    index++;
                    continue;
                }

                List<string> tmpVals = new List<string>();

                if (!uint.TryParse(ConfigurationManager.AppSettings["ExcelCol" + index], out tmpCol2))
                {
                    index++;
                    continue;
                }

                MasterVals[tmpCol] = tmpCol2;
                index++;
            }

            EditVals = new Dictionary<uint, string[]>();
            index = 0;

            while (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["EditCol" + index]))
            {
                uint tmpCol = 0;
                if (!uint.TryParse(ConfigurationManager.AppSettings["EditCol" + index], out tmpCol))
                {
                    index++;
                    continue;
                }

                List<string> tmpVals = new List<string>();

                tmpVals.Add(ConfigurationManager.AppSettings["EditVal_Before_" + index]);
                if (string.IsNullOrEmpty(tmpVals[0]))
                {
                    index++;
                    continue;
                }

                tmpVals.Add(ConfigurationManager.AppSettings["EditVal_After_" + index]);
                if (string.IsNullOrEmpty(tmpVals[1]))
                {
                    index++;
                    continue;
                }

                EditVals[tmpCol] = tmpVals.ToArray();
                index++;
            }

            SD_Pre = ConfigurationManager.AppSettings["StudyDate_Pre"];

            return ret;
        }
    }

}
