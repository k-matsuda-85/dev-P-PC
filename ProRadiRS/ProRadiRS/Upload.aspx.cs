// ProRadiRS.Upload
using ProRadiRS;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;

public class Upload : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            base.Response.ClearHeaders();
            base.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            base.Response.Cache.SetValidUntilExpires(true);
            int num = 0;
            if (HttpContext.Current.Session["usercd"] != null)
            {
                num = (int)HttpContext.Current.Session["usercd"];
            }
            else
            {
                LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "clipboard_Upload", "セッションからユ\u30fcザ\u30fcコ\u30fcドが取得出来ません。");
            }
            LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "clipboard_Upload", "START [" + num.ToString() + "]");
            string[] allKeys = base.Request.Files.AllKeys;
            foreach (string text in allKeys)
            {
                HttpPostedFile httpPostedFile = base.Request.Files[text];
                string text2 = base.Server.MapPath("~/") + text;
                string path = Path.Combine(ConfigUtil._ImageViewPath, num.ToString()) + "\\" + text;
                if (httpPostedFile.ContentType == "image/png")
                {
                    path = Path.ChangeExtension(path, ".jpg");
                    using (Bitmap bitmap = new Bitmap(httpPostedFile.InputStream))
                    {
                        using (EncoderParameters encoderParameters = new EncoderParameters(1))
                        {
                            EncoderParameter encoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                            encoderParameters.Param[0] = encoderParameter;
                            ImageCodecInfo encoderInfo = Upload.GetEncoderInfo(ImageFormat.Jpeg);
                            bitmap.Save(path, encoderInfo, encoderParameters);
                            LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "clipboard_Upload", "test]");
                        }
                    }
                }
            }
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string s = javaScriptSerializer.Serialize(true);
            base.Response.ClearContent();
            base.Response.ContentType = "application/json; charset=utf-8";
            base.Response.ContentEncoding = Encoding.GetEncoding("utf-8");
            base.Response.Write(s);
        }
        catch
        {
            base.Response.StatusCode = 404;
            base.Response.SuppressContent = true;
        }
    }

    private static ImageCodecInfo GetEncoderInfo(string mineType)
    {
        ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
        ImageCodecInfo[] array = imageEncoders;
        foreach (ImageCodecInfo imageCodecInfo in array)
        {
            if (imageCodecInfo.MimeType == mineType)
            {
                return imageCodecInfo;
            }
        }
        return null;
    }

    private static ImageCodecInfo GetEncoderInfo(ImageFormat f)
    {
        ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
        ImageCodecInfo[] array = imageEncoders;
        foreach (ImageCodecInfo imageCodecInfo in array)
        {
            if (imageCodecInfo.FormatID == f.Guid)
            {
                return imageCodecInfo;
            }
        }
        return null;
    }
}
