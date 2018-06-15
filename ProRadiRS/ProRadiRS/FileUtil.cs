// ProRadiRS.FileUtil
using ProRadiRS;
using System;
using ProRadiRS.NetTcpBinding_ProRadiRSService;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Xml;

public class FileUtil
{
    public static void ImageClear(string clearPath)
    {
        if (Directory.Exists(clearPath))
        {
            string[] files = Directory.GetFiles(clearPath);
            if (clearPath.Length > 0)
            {
                string[] array = files;
                foreach (string text in array)
                {
                    try
                    {
                        if (File.Exists(text))
                        {
                            FileInfo fileInfo = new FileInfo(text);
                            if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                            {
                                fileInfo.Attributes = FileAttributes.Normal;
                            }
                            fileInfo.Delete();
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }
    }

    public static void SaveXmlOutput(string datFile, string trgFile, int userCd, string serialNo, string orderNo, string officeCd, string finding, string diagnosing, string[] image)
    {
        using (XmlTextWriter xmlTextWriter = new XmlTextWriter(datFile, Encoding.GetEncoding("UTF-8")))
        {
            xmlTextWriter.WriteStartDocument();
            xmlTextWriter.WriteStartElement("data");
            xmlTextWriter.WriteStartElement("UserInfo");
            xmlTextWriter.WriteElementString("UserCD", userCd.ToString());
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("report");
            xmlTextWriter.WriteElementString("SerialNo", serialNo);
            xmlTextWriter.WriteElementString("OrderNo", orderNo);
            xmlTextWriter.WriteElementString("OfficeCD", officeCd);
            xmlTextWriter.WriteElementString("Finding", finding);
            xmlTextWriter.WriteElementString("Diagnosing", diagnosing);
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteStartElement("image");
            foreach (string path in image)
            {
                xmlTextWriter.WriteElementString("item", Path.ChangeExtension(path, ConfigUtil._ImageExt));
            }
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndDocument();
        }
        if (trgFile.Length > 0 && File.Exists(datFile))
        {
            using (new StreamWriter(trgFile))
            {
            }
        }
    }

    public static void SaveXmlRead(string tempSavePath, string serialNo, ref ReportView report, ref List<string> image)
    {
        string[] files = Directory.GetFiles(tempSavePath, serialNo + "*.xml", SearchOption.TopDirectoryOnly);
        if (files.Length > 0)
        {
            ArrayList arrayList = new ArrayList();
            string[] array = files;
            foreach (string value in array)
            {
                arrayList.Add(value);
            }
            arrayList.Sort();
            string filename = (string)arrayList[arrayList.Count - 1];
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            List<string> list = new List<string>();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filename);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("data/report");
            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                if (childNode.NodeType != XmlNodeType.Comment)
                {
                    string localName = childNode.LocalName;
                    string innerText = childNode.InnerText;
                    if (dictionary.ContainsKey(localName))
                    {
                        Dictionary<string, string> dictionary2;
                        string key;
                        (dictionary2 = dictionary)[key = localName] = dictionary2[key] + "," + innerText;
                    }
                    else
                    {
                        dictionary.Add(localName, innerText);
                    }
                }
            }
            XmlNode xmlNode3 = xmlDocument.SelectSingleNode("data/image");
            foreach (XmlNode childNode2 in xmlNode3.ChildNodes)
            {
                if (childNode2.NodeType != XmlNodeType.Comment)
                {
                    string innerText2 = childNode2.InnerText;
                    list.Add(innerText2);
                }
            }
            if (dictionary.ContainsKey("Finding"))
            {
                report.Finding = dictionary["Finding"];
            }
            if (dictionary.ContainsKey("Diagnosing"))
            {
                report.Diagnosing = dictionary["Diagnosing"];
            }
            image.Clear();
            foreach (string item in list)
            {
                image.Add(item);
            }
        }
    }

    public static void GetTempSaveList(int userCd, out List<string> lstTempSaveXml)
    {
        lstTempSaveXml = new List<string>();
        string path = Path.Combine(ConfigUtil._TempSaveReportPath, userCd.ToString());
        if (Directory.Exists(path))
        {
            string[] files = Directory.GetFiles(path, "*.xml", SearchOption.TopDirectoryOnly);
            string[] array = files;
            foreach (string path2 in array)
            {
                string[] array2 = Path.GetFileNameWithoutExtension(path2).Split('_');
                if (array2.Length > 0)
                {
                    lstTempSaveXml.Add(array2[0]);
                }
            }
        }
    }

    public static void TempSaveClear(string clearPath, string serialNo)
    {
        if (Directory.Exists(clearPath))
        {
            string[] files = Directory.GetFiles(clearPath, serialNo + "*.xml");
            if (clearPath.Length > 0)
            {
                string[] array = files;
                foreach (string text in array)
                {
                    try
                    {
                        if (File.Exists(text))
                        {
                            FileInfo fileInfo = new FileInfo(text);
                            if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                            {
                                fileInfo.Attributes = FileAttributes.Normal;
                            }
                            fileInfo.Delete();
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }
    }

    public static bool ExistsImageCheck(string orderNo)
    {
        bool result = false;
        if (orderNo.Length > 0)
        {
            string path = "";
            if (orderNo.Length >= 3)
            {
                path = orderNo.Substring(0, 3);
            }
            if (ConfigUtil._ImageCheckPath.Length > 0)
            {
                string path2 = Path.ChangeExtension(orderNo, "txt");
                string text = Path.Combine(ConfigUtil._ImageCheckPath, path);
                if (Directory.Exists(text))
                {
                    text = Path.Combine(text, path2);
                    if (File.Exists(text))
                    {
                        result = true;
                    }
                }
            }
        }
        return result;
    }

    public static bool ExistsExamOrder(out string examOrder, string reportReserve2)
    {
        bool result = false;
        examOrder = "";
        if (reportReserve2.Length > 0)
        {
            string text = reportReserve2;
            string[] array = ConfigUtil._ReplaceExamOrderPath.Split('?');
            if (array.Length > 0 && array[0].Length > 0 && array[1].Length > 0)
            {
                text = text.Replace(array[0], array[1]);
            }
            string name = WindowsIdentity.GetCurrent().Name;
            LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "ExistsExamOrder", "NAME [" + name + "]");
            LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "ExistsExamOrder", text);
            if (File.Exists(text))
            {
                examOrder = text;
                result = true;
            }
            else if (FileUtil.execNetUse())
            {
                if (File.Exists(text))
                {
                    examOrder = text;
                    result = true;
                }
                else
                {
                    LogUtil.Write(LogUtil.LogType.Error, "FileUtil", "ExistsExamOrder", "依頼票画像パスにアクセス不可 [" + text + "]");
                    examOrder = "";
                }
            }
        }
        else
        {
            result = true;
        }
        return result;
    }

    private static bool execNetUse()
    {
        bool flag = false;
        try
        {
            if (ConfigUtil._NETUSE_PATH.Length > 0 && ConfigUtil._NETUSE_USER.Length > 0 && ConfigUtil._NETUSE_PASS.Length > 0)
            {
                string text = "/c net use " + ConfigUtil._NETUSE_PATH + " /user:" + ConfigUtil._NETUSE_USER + " " + ConfigUtil._NETUSE_PASS;
                LogUtil.Write(LogUtil.LogType.Debug, "FileUtil", "execNetUse", "[netuseコマンド] " + text);
                using (Process process = new Process())
                {
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.FileName = "cmd";
                    process.StartInfo.Arguments = text;
                    bool flag2 = process.Start();
                    process.WaitForExit();
                    flag = true;
                    LogUtil.Write(LogUtil.LogType.Debug, "FileUtil", "execNetUse", "実行完了 [" + flag2.ToString() + "]");
                    return flag;
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            LogUtil.Write(LogUtil.LogType.Error, "FileUtil", "execNetUse", ex.ToString());
            return false;
        }
    }
}
