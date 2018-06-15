// ProRadiRS.ConfigUtil
using System.Configuration;

public class ConfigUtil
{
    public static readonly string _Modality = ConfigurationManager.AppSettings["Modality"];

    public static readonly string _Help = ConfigurationManager.AppSettings["HelpURL"];

    public static readonly string _LogPath = ConfigurationManager.AppSettings["LogPath"] ?? "";

    public static readonly string _ViewerUrl = ConfigurationManager.AppSettings["ViewerUrl"] ?? "";

    public static readonly string _ViewerUrlKey = ConfigurationManager.AppSettings["ViewerUrlKey"] ?? "";

    public static readonly string _ViewerUrl_reserve = ConfigurationManager.AppSettings["ViewerUrl2"] ?? "";

    public static readonly string _ImageImportPath = ConfigurationManager.AppSettings["ImageImportPath"] ?? "";

    public static readonly string _ImageViewPath = ConfigurationManager.AppSettings["ImageViewPath"] ?? "";

    public static readonly string _ImageExt = ConfigurationManager.AppSettings["ImageExt"] ?? "";

    public static readonly string _SaveReportPath = ConfigurationManager.AppSettings["SaveReportPath"] ?? "";

    public static readonly string _HistoryImageViewPath = ConfigurationManager.AppSettings["HistoryImageViewPath"] ?? "";

    public static readonly string _iProMedCode = ConfigurationManager.AppSettings["iProMedCode"];

    public static readonly string _ViewerImageRequestPath = ConfigurationManager.AppSettings["ViewerImageRequestPath"];

    public static readonly string _ViewerImageRequestDatExt = ConfigurationManager.AppSettings["ViewerImageRequestDatExt"];

    public static readonly string _ViewerImageRequestTrgExt = ConfigurationManager.AppSettings["ViewerImageRequestTrgExt"];

    public static readonly string _MaxImageImportNum = ConfigurationManager.AppSettings["MaxImageImportNum"] ?? "";

    public static readonly string _TempSaveReportPath = ConfigurationManager.AppSettings["TempSaveReportPath"];

    public static readonly string _ImageCheckPath = ConfigurationManager.AppSettings["ImageCheckPath"] ?? "";

    public static readonly string _ReplaceExamOrderPath = ConfigurationManager.AppSettings["ReplaceExamOrderPath"] ?? "";

    public static readonly string _NETUSE_PATH = ConfigurationManager.AppSettings["NETUSE_PATH"] ?? "";

    public static readonly string _NETUSE_USER = ConfigurationManager.AppSettings["NETUSE_USER"] ?? "";

    public static readonly string _NETUSE_PASS = ConfigurationManager.AppSettings["NETUSE_PASS"] ?? "";

    public static readonly string _IsImageCheck = ConfigurationManager.AppSettings["IsImageCheck"] ?? "";

    public static readonly string _IsRequestSearch = ConfigurationManager.AppSettings["IsRequestSearch"] ?? "";

    public static readonly string _IsInfomation = ConfigurationManager.AppSettings["IsInfomation"] ?? "";
}
