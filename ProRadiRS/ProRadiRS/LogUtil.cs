// ProRadiRS.LogUtil
using CommonLib.Log;
using ProRadiRS;
using System;
using System.Collections.Generic;
using System.Web;

public class LogUtil
{
    public enum LogType
    {
        Debug,
        Error,
        Warning,
        Information
    }

    static LogUtil()
    {
        TraceFileLog.DefaultPath = ConfigUtil._LogPath;
        TraceFileLog.Start();
    }

    public static void Write(LogType type, string className, string methodName, object message)
    {
        string message2 = (message == null) ? "" : message.ToString();
        LogUtil.WriteFile(className, type, methodName, message2);
    }

    public static void Write(LogType type, string className, string methodName, string format, params object[] args)
    {
        string message = string.Format(format, args);
        LogUtil.WriteFile(className, type, methodName, message);
    }

    private static void WriteFile(object sender, LogType type, string source, string message)
    {
        List<object> list = new List<object>();
        list.Add(source);
        list.Add(message);
        HttpContext current = HttpContext.Current;
        if (current != null)
        {
            list.Add((current.User != null) ? current.User.Identity.Name : "");
            list.Add(current.Request.UserHostAddress);
            list.Add(current.Request.UserAgent);
            list.Add((current.Request.UrlReferrer != (Uri)null) ? current.Request.UrlReferrer.Authority : "");
        }
        CommonLib.Log.LogType type2;
        switch (type)
        {
            case LogType.Debug:
                type2 = CommonLib.Log.LogType.DEBUG;
                break;
            case LogType.Error:
                type2 = CommonLib.Log.LogType.ERROR;
                break;
            case LogType.Warning:
                type2 = CommonLib.Log.LogType.WARNING;
                break;
            case LogType.Information:
                type2 = CommonLib.Log.LogType.NORMAL;
                break;
            default:
                type2 = CommonLib.Log.LogType.NORMAL;
                break;
        }
        CustomLog.Write(sender, type2, list.ToArray());
    }
}
