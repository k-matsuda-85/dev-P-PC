using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using static CommonWebServiceProc;

namespace ProRadiRS
{
    /// <summary>
    /// CommonWebService の概要の説明です
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // この Web サービスを、スクリプトから ASP.NET AJAX を使用して呼び出せるようにするには、次の行のコメントを解除します。
    [System.Web.Script.Services.ScriptService]
    public class CommonWebService : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "HelloWorld";
        }

        [WebMethod(EnableSession = true)]
        public int Login(string loginID, string loginPW)
        {
            return CommonWebServiceProc.Login(loginID, loginPW);
        }

        [WebMethod(EnableSession = true)]
        public WebParams GetParams()
        {
            return CommonWebServiceProc.GetParams();
        }

        [WebMethod(EnableSession = true)]
        public WebResult SetParams(string[] key, string[] val)
        {
            return CommonWebServiceProc.SetParams(key, val);
        }

        [WebMethod(EnableSession = true)]
        public WebViewer GetViewerUrl(int serialno, string orderno, string patientid, string studydate, string modality, int viewReservflg)
        {
            return CommonWebServiceProc.GetViewerUrl(serialno, orderno, patientid, studydate, modality, viewReservflg);
        }

        [WebMethod(EnableSession = true)]
        public WebResult ReadingCheck()
        {
            return CommonWebServiceProc.ReadingCheck();
        }

        [WebMethod(EnableSession = true)]
        public WebResult ReadingStart(int serialNo)
        {
            return CommonWebServiceProc.ReadingStart(serialNo);
        }

        [WebMethod(EnableSession = true)]
        public WebResult CancelReading(int serialNo)
        {
            return CommonWebServiceProc.CancelReading(serialNo);
        }

        [WebMethod(EnableSession = true)]
        public WebReportList GetSearchList(string patientID, string searchDay, string modality)
        {
            return CommonWebServiceProc.GetSearchList(patientID, searchDay, modality);
        }

        [WebMethod(EnableSession = true)]
        public WebResult DeleteTemp(int serialNo)
        {
            return CommonWebServiceProc.DeleteTemp(serialNo);
        }

        [WebMethod(EnableSession = true)]
        public WebReportList GetRequestReportList(string isusernametype, string username)
        {
            return CommonWebServiceProc.GetRequestReportList(isusernametype, username);
        }

        [WebMethod(EnableSession = true)]
        public WebReportList GetEmergencyReportList()
        {
            return CommonWebServiceProc.GetEmergencyReportList();
        }

        [WebMethod(EnableSession = true)]
        public WebReportList GetReadReportList(string isusernametype, string username)
        {
            return CommonWebServiceProc.GetReadReportList(isusernametype, username);
        }

        [WebMethod(EnableSession = true)]
        public WebReport GetReportData(string serialNo, string readTempSave)
        {
            return CommonWebServiceProc.GetReportData(serialNo, readTempSave);
        }

        [WebMethod(EnableSession = true)]
        public WebImageList GetImageList(int serialNo, int imageNum)
        {
            return CommonWebServiceProc.GetImageList(serialNo, imageNum);
        }

        [WebMethod(EnableSession = true)]
        public WebResult ClearImagePath()
        {
            return CommonWebServiceProc.ClearImagePath();
        }

        [WebMethod(EnableSession = true)]
        public WebResult SaveReportData(string[] key, string[] val, string[] image)
        {
            return CommonWebServiceProc.SaveReportData(key, val, image);
        }

        [WebMethod(EnableSession = true)]
        public WebResult TempSaveReportData(string[] key, string[] val, string[] image)
        {
            return CommonWebServiceProc.TempSaveReportData(key, val, image);
        }

        [WebMethod(EnableSession = true)]
        public WebResult DeleteImage(string image)
        {
            return CommonWebServiceProc.DeleteImage(image);
        }

        [WebMethod(EnableSession = true)]
        public WebHistoryReport GetHistoryReportData(int serialNo)
        {
            return CommonWebServiceProc.GetHistoryReportData(serialNo);
        }

        [WebMethod(EnableSession = true)]
        public WebResult RequestViewerImage(string orderno, string patientid, string studydate, string modality)
        {
            return CommonWebServiceProc.RequestViewerImage(orderno, patientid, studydate, modality);
        }

        [WebMethod(EnableSession = true)]
        public WebResult SetViewerImageCheckFile(string checktype, string orderno)
        {
            return CommonWebServiceProc.SetViewerImageCheckFile(checktype, orderno);
        }

        [WebMethod(EnableSession = true)]
        public WebUserList GetUserList()
        {
            return CommonWebServiceProc.GetUserList();
        }

        #region 2018/02/22 新規追加
        [WebMethod(EnableSession = true)]
        public int AutoLogin(string usercd)
        {
            return CommonWebServiceProc.AutoLogin(usercd);
        }

        [WebMethod(EnableSession = true)]
        public WebSentenceList GetSentenceData(int usercd, int groupcd)
        {
            return CommonWebServiceProc.GetSentenceData(usercd, groupcd);
        }
        [WebMethod(EnableSession = true)]
        public bool SetSentenceData(string[] vals)
        {
            return CommonWebServiceProc.SetSentenceData(vals);
        }
        [WebMethod(EnableSession = true)]
        public bool DelSentenceData(string cd)
        {
            return CommonWebServiceProc.DelSentenceData(cd);
        }

        [WebMethod(EnableSession = true)]
        public WebHistoryList GetChangeHistory(int serialno)
        {
            return CommonWebServiceProc.GetChangeHistory(serialno);
        }
        #endregion
    }
}
