using System;
using System.Configuration;
using System.IO;

namespace OrderTool
{
    public partial class View1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string cd = Request.Form["cd"];
            string patid = Request.Form["patid"];
            string mod = Request.Form["mod"];
            string date = Request.Form["date"];
            string orderid = Request.Form["orderid"];
            string read = Request.Form["read"];

            string path = Path.Combine(ConfigurationManager.AppSettings["RetPath"], cd);
            string file = patid + "_" + date + "_" + mod + "_" + orderid + "_" + read;

            if (!Directory.Exists(path))
                return;

            string[] files = Directory.GetFiles(path, file + "*");

            if (files == null || files.Length == 0)
                return;


            string checkPath = Path.Combine(ConfigurationManager.AppSettings["PDFPath"], cd);
            using (var sw = new StreamWriter(Path.Combine(checkPath, Path.GetFileName(files[0]))))
            {
                sw.Close();
            }

            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", "inline");
            Response.AppendHeader("Content-Transfer-Encoding", "base64");
            Response.AppendHeader("attachment", "filename=" + Path.GetFileName(files[0]));

            Response.WriteFile(files[0]);
            Response.Flush();
            Response.End();
        }
    }
}