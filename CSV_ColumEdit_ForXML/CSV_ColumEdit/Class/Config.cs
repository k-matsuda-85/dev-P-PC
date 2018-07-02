using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace CSV_ColumEdit.Class
{
    public static class Config
    {
        public static string hospCd;

        public static string inputDir;
        public static string outputDir;
        public static string workDir;
        public static string errDir;
        public static string reportDir;

        public static string backDir;

        public static string imgInDir;
        public static string pastInDir;

        public static string pastOutputDir;
        public static string imgOutputDir;

        public static string Trigger;

        public static string JPEG_TEXT;

        public static string MasterFile;
        public static string PastMasterFile;
        public static string EditMasterFile;

        public static Dictionary<uint, string[]> MasterVals;

        public static Dictionary<string, string[]> EditVals;

        public static Dictionary<string, string> LogVals;

        public static Dictionary<string, string> ExtPaths;

        public static Dictionary<string, string> ExtPassVals;

        public static Dictionary<string, string> PastPassVals;

        public static string InitConfig()
        {
            string ret = "";

            hospCd = ConfigurationManager.AppSettings["HospCd"];

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

            workDir = ConfigurationManager.AppSettings["WorkDir"];
            if (string.IsNullOrEmpty(workDir))
            {
                ret = "一時フォルダを指定してください。";
                return ret;
            }
            else if (!Directory.Exists(workDir))
            {
                ret = "一時フォルダが存在しません";
                return ret;
            }

            errDir = ConfigurationManager.AppSettings["ErrDir"];
            if (string.IsNullOrEmpty(errDir))
            {
                ret = "エラーフォルダを指定してください。";
                return ret;
            }
            else if (!Directory.Exists(errDir))
            {
                ret = "エラーフォルダが存在しません";
                return ret;
            }

            reportDir = ConfigurationManager.AppSettings["ReportDir"];
            if (string.IsNullOrEmpty(reportDir))
            {
                ret = "所見連携フォルダを指定してください。";
                return ret;
            }
            else if (!Directory.Exists(reportDir))
            {
                ret = "所見連携フォルダが存在しません";
                return ret;
            }

            Trigger = ConfigurationManager.AppSettings["Trigger"];

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
            PastMasterFile = ConfigurationManager.AppSettings["PastMasterFile"];
            if (string.IsNullOrEmpty(PastMasterFile))
            {
                ret = "過去所見マスターファイルを指定してください。";
                return ret;
            }
            else if (!File.Exists(PastMasterFile))
            {
                ret = "過去所見マスターファイルが存在しません";
                return ret;
            }
            EditMasterFile = ConfigurationManager.AppSettings["EditMasterFile"];
            if (string.IsNullOrEmpty(EditMasterFile))
            {
                ret = "所見連携マスターファイルを指定してください。";
                return ret;
            }
            else if (!File.Exists(EditMasterFile))
            {
                ret = "所見連携マスターファイルが存在しません";
                return ret;
            }

            //pastInDir = ConfigurationManager.AppSettings["PastInputDir"];
            //if (!string.IsNullOrEmpty(pastInDir))
            //    if (!Directory.Exists(pastInDir))
            //    {
            //        ret = "過去所見取込フォルダが存在しません";
            //        return ret;
            //    }

            //imgInDir = ConfigurationManager.AppSettings["ImgInputDir"];
            //if (!string.IsNullOrEmpty(imgInDir))
            //    if (!Directory.Exists(imgInDir))
            //    {
            //        ret = "キー画像取込フォルダが存在しません";
            //        return ret;
            //    }

            JPEG_TEXT = ConfigurationManager.AppSettings["JPEG_TEXT"];

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


            LogVals = new Dictionary<string, string>();
            int index = 0;

            while (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["LogName" + index]))
            {
                LogVals[ConfigurationManager.AppSettings["LogName" + index]] = ConfigurationManager.AppSettings["LogCol" + index];
                index++;
            }

            ExtPaths = new Dictionary<string, string>();
            index = 0;

            while (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ExtOutputDir" + index]))
            {
                ExtPaths[ConfigurationManager.AppSettings["ExtOutputDir" + index]] = ConfigurationManager.AppSettings["ExtOutputTri" + index];
                index++;
            }

            ExtPassVals = new Dictionary<string, string>();
            index = 0;

            while (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ExtPassCol" + index]))
            {
                ExtPassVals[ConfigurationManager.AppSettings["ExtPassCol" + index]] = ConfigurationManager.AppSettings["ExtPassVal" + index];
                index++;
            }

            PastPassVals = new Dictionary<string, string>();
            index = 0;

            while (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["PastPassCol" + index]))
            {
                PastPassVals[ConfigurationManager.AppSettings["PastPassCol" + index]] = ConfigurationManager.AppSettings["PastPassVal" + index];
                index++;
            }

            MasterVals = new Dictionary<uint, string[]>();
            index = 0;

            while (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CheckCol" + index]))
            {
                List<string> tmpVals = new List<string>();

                tmpVals.Add(ConfigurationManager.AppSettings["CheckCol" + index]);
                tmpVals.Add(ConfigurationManager.AppSettings["CheckVal" + index]);
                if(string.IsNullOrEmpty(tmpVals[1]))
                {
                    index++;
                    continue;
                }

                tmpVals.Add(ConfigurationManager.AppSettings["CheckMasterFile" + index]);
                if (string.IsNullOrEmpty(tmpVals[2]) || !File.Exists(tmpVals[2]))
                {
                    index++;
                    continue;
                }

                MasterVals[(uint)index] = tmpVals.ToArray();
                index++;
            }

            EditVals = new Dictionary<string, string[]>();
            index = 0;

            while (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["EditCol" + index]))
            {
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

                EditVals[ConfigurationManager.AppSettings["EditCol" + index]] = tmpVals.ToArray();
                index++;
            }


            return ret;
        }
    }

}
