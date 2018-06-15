using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProRadiRS
{
    public partial class webLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string usercd = Page.Request.QueryString.Get("usercd");
                if (!string.IsNullOrEmpty(usercd))
                {
                    System.Diagnostics.Debug.WriteLine(usercd);
                    // ログインチェック
                    if (CommonWebServiceProc.AutoLogin(usercd) == 0)
                    {
                        // 検索画面にリダイレクト
                        Response.Redirect("./webSearch.aspx");
                        HttpContext.Current.Session["usercd"] = usercd;
                    }
                }
            }
            catch { }
        }
    }
}