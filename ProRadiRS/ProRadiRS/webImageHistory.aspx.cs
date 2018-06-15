using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProRadiRS
{
    public partial class webImageHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session["usercd"] != null)
            {
                int userCd = (int)HttpContext.Current.Session["usercd"];
                LogUtil.Write(LogUtil.LogType.Debug, "webImageHistory", "Page_Load", "START [" + userCd.ToString() + "]");
                CommonWebServiceProc.GetImage(userCd, ConfigUtil._HistoryImageViewPath);
            }
            else
            {
                LogUtil.Write(LogUtil.LogType.Error, "webImageHistory", "Page_Load", "セッションからのユ\u30fcザ\u30fcコ\u30fcドが取得出来ません。");
            }
        }
    }

}