// ProRadiRS.webExamOrder
using ProRadiRS;
using System;
using System.Web;
using System.Web.UI;

namespace ProRadiRS
{
    public partial class webExamOrder : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session["usercd"] != null)
            {
                int userCd = (int)HttpContext.Current.Session["usercd"];
                LogUtil.Write(LogUtil.LogType.Debug, "webExamOrder", "Page_Load", "START [" + userCd.ToString() + "]");
                CommonWebServiceProc.GetExamOrder(userCd);
            }
            else
            {
                LogUtil.Write(LogUtil.LogType.Error, "webExamOrder", "Page_Load", "セッションからのユ\u30fcザ\u30fcコ\u30fcドが取得出来ません。");
            }
        }
    }

}
