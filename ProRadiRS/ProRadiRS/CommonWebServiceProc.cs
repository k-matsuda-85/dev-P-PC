// ProRadiRS.CommonWebServiceProc
using ProRadiRS;
using ProRadiRS.NetTcpBinding_ProRadiRSService;
using ProRadiRS.ProRadiRSService2;
using ProRadServiceLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.ServiceModel;
using System.Text;
using System.Web;

public class CommonWebServiceProc
{
	public static int Login(string loginID, string loginPW)
	{
		ProRadiRSServiceClient proRadiRSServiceClient = null;
		int num = 0;
		try
		{
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "Login", loginID + "/" + loginPW);
			int num2 = -1;
			string value = "";
			int num3 = 0;
			proRadiRSServiceClient = new ProRadiRSServiceClient();
			num = proRadiRSServiceClient.Login(loginID, loginPW, out num2, out value, out num3);
			proRadiRSServiceClient.Close();
			LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "Login", "サ\u30fcビス戻り値 [" + num + "]");
			HttpContext.Current.Session.Clear();
			HttpContext.Current.Session["loginid"] = loginID;
			HttpContext.Current.Session["usercd"] = num2;
			HttpContext.Current.Session["username"] = value;
			HttpContext.Current.Session["groupCd"] = num3;
			return num;
		}
		catch (Exception ex)
		{
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "Login", ex.ToString());
			num = -1;
			if (proRadiRSServiceClient != null)
			{
				if (proRadiRSServiceClient.State == CommunicationState.Faulted)
				{
					proRadiRSServiceClient.Abort();
					return num;
				}
				return num;
			}
			return num;
		}
	}

    public static WebParams GetParams()
	{
		ProRadiRSServiceClient proRadiRSServiceClient = null;
		try
		{
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			if (HttpContext.Current.Session["usercd"] != null)
			{
				int num = (int)HttpContext.Current.Session["usercd"];
				LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "GetParams", "START [" + num.ToString() + "]");
				if (HttpContext.Current.Session["groupCd"] != null)
				{
					int num2 = (int)HttpContext.Current.Session["groupCd"];
					LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "GetParams", "START [" + num2.ToString() + "]");
					string value = "";
					string str = "";
					proRadiRSServiceClient = new ProRadiRSServiceClient();
					int[] groupType = new int[2]
					{
						1,
						2
					};
					GroupMst[] array = default(GroupMst[]);
					proRadiRSServiceClient.GetGroupList(groupType, out array);
					proRadiRSServiceClient.Close();
					List<string> list = new List<string>();
					GroupMst[] array2 = array;
					foreach (GroupMst groupMst in array2)
					{
						if (groupMst.GroupType == 1 && groupMst.GroupCD == num2)
						{
							value = groupMst.GroupName;
						}
						if (groupMst.GroupType == 2)
						{
							list.Add(groupMst.GroupName);
						}
					}
					for (int j = 0; j < list.Count - 1; j++)
					{
						str = str + list[j] + ",";
					}
					str += list[list.Count - 1];
					HttpContext.Current.Session["ReadDepartment"] = value;
					string value2 = "";
					proRadiRSServiceClient = new ProRadiRSServiceClient();
					ServerConfigMst[] array3 = default(ServerConfigMst[]);
					proRadiRSServiceClient.GetServerConfigList(out array3);
					proRadiRSServiceClient.Close();
					new List<string>();
					ServerConfigMst[] array4 = array3;
					foreach (ServerConfigMst serverConfigMst in array4)
					{
						if (serverConfigMst.ConfigKey == "ImgCheckModality")
						{
							value2 = serverConfigMst.ConfigValue;
						}
					}
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					for (int l = 0; l < HttpContext.Current.Session.Count; l++)
					{
						if (HttpContext.Current.Session.Keys[l] != "loginid" && HttpContext.Current.Session.Keys[l] != "usercd" && HttpContext.Current.Session.Keys[l] != "username")
						{
							dictionary.Add(HttpContext.Current.Session.Keys[l], HttpContext.Current.Session[HttpContext.Current.Session.Keys[l]].ToString());
						}
					}
					if (!dictionary.ContainsKey("iProMedCode"))
					{
						dictionary.Add("ipromedcode", ConfigUtil._iProMedCode);
					}
					if (!dictionary.ContainsKey("modality"))
					{
						dictionary.Add("modality", str);
					}
					if (!dictionary.ContainsKey("MaxImageImportNum"))
					{
						dictionary.Add("MaxImageImportNum", ConfigUtil._MaxImageImportNum);
					}
					string key = "isimagecheck";
					dictionary.Add(key, "0");
					string[] array5 = ConfigUtil._IsImageCheck.Split(',');
					foreach (string value3 in array5)
					{
						if (num.ToString().Equals(value3))
						{
							dictionary[key] = "1";
							break;
						}
					}
					string key2 = "isrequestsearch";
					dictionary.Add(key2, "0");
					string[] array6 = ConfigUtil._IsRequestSearch.Split(',');
					foreach (string value4 in array6)
					{
						if (num.ToString().Equals(value4))
						{
							dictionary[key2] = "1";
							break;
						}
					}
					string key3 = "isinfomation";
					dictionary.Add(key3, "0");
					string[] array7 = ConfigUtil._IsInfomation.Split(',');
					foreach (string value5 in array7)
					{
						if (num.ToString().Equals(value5))
						{
							dictionary[key3] = "1";
							break;
						}
					}
					if (!dictionary.ContainsKey("helpURL"))
					{
						dictionary.Add("helpURL", ConfigUtil._Help);
					}
					if (!dictionary.ContainsKey("ImgCheckModality"))
					{
						dictionary.Add("ImgCheckModality", value2);
					}

                    #region 2018/02/22 新規追加
                    if (!dictionary.ContainsKey("usercd"))
                    {
                        dictionary.Add("usercd", num.ToString());
                    }
                    #endregion

                    WebParams webParams = new WebParams();
					webParams.Result = "Success";
					webParams.Params = dictionary;
					return webParams;
				}
				LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetParams", "セッションからのグル\u30fcプコ\u30fcドが取得出来ない。");
				WebParams webParams2 = new WebParams();
				webParams2.Result = "Error";
				webParams2.Message = "NoSession";
				return webParams2;
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetParams", "セッションからのユ\u30fcザ\u30fcコ\u30fcドが取得出来ない。");
			WebParams webParams3 = new WebParams();
			webParams3.Result = "Error";
			webParams3.Message = "NoSession";
			return webParams3;
		}
		catch (Exception ex)
		{
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetParams", ex.ToString());
			WebParams webParams4 = new WebParams();
			webParams4.Result = "Error";
			webParams4.Message = "Exception";
			return webParams4;
		}
	}

	public static WebResult SetParams(string[] key, string[] val)
	{
		try
		{
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			if (HttpContext.Current.Session["usercd"] != null)
			{
				LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "SetParams", "START [" + ((int)HttpContext.Current.Session["usercd"]).ToString() + "]");
				for (int i = 0; i < key.Length; i++)
				{
					HttpContext.Current.Session[key[i]] = val[i];
				}
				WebResult webResult = new WebResult();
				webResult.Result = "Success";
				return webResult;
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "SetParams", "セッションからのユ\u30fcザ\u30fcコ\u30fcドが取得出来ない。");
			WebParams webParams = new WebParams();
			webParams.Result = "Error";
			webParams.Message = "NoSession";
			return webParams;
		}
		catch (Exception ex)
		{
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "SetParams", ex.ToString());
			WebResult webResult2 = new WebResult();
			webResult2.Result = "Error";
			return webResult2;
		}
	}

	public static WebReportList GetRequestReportList(string isusernametype, string username)
	{
		ProRadiRSServiceClient proRadiRSServiceClient = null;
		try
		{
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			if (HttpContext.Current.Session["usercd"] != null)
			{
				int userCd = (int)HttpContext.Current.Session["usercd"];
				LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "GetRequestReportList", "START [" + userCd.ToString() + "]");
				int num = default(int);
				if (!int.TryParse(isusernametype, out num))
				{
					num = 0;
				}
				switch (num)
				{
				case 0:
				{
					if (HttpContext.Current.Session["username"] != null)
					{
						username = (string)HttpContext.Current.Session["username"];
						LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "GetRequestReportList", "検索ユ\u30fcザ\u30fc名(セッションから取得) [" + username + "]");
						break;
					}
					LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetRequestReportList", "セッションからユ\u30fcザ\u30fc名(依頼先医師名)が取得出来ません。");
					WebReportList webReportList2 = new WebReportList();
					webReportList2.Result = "Error";
					webReportList2.Message = "NoSession";
					return webReportList2;
				}
				case 1:
					if (username.Length > 0)
					{
						LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "GetRequestReportList", "検索ユ\u30fcザ\u30fc名(引数から取得) [" + username + "]");
					}
					else
					{
						LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "GetRequestReportList", "検索ユ\u30fcザ\u30fc名指定なし");
					}
					break;
				default:
				{
					string text = "検索ユ\u30fcザ\u30fc名取得方法が未定義です。";
					LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetRequestReportList", text);
					WebReportList webReportList = new WebReportList();
					webReportList.Result = "Error";
					webReportList.Message = text;
					return webReportList;
				}
				}
				proRadiRSServiceClient = new ProRadiRSServiceClient();
				ReportView[] getView = default(ReportView[]);
				int count = default(int);
				int reportList = proRadiRSServiceClient.GetReportList(username, out getView, out count);
				proRadiRSServiceClient.Close();
				List<ReportView> list = new List<ReportView>();
				List<string> list2 = new List<string>();
				if (reportList == 0)
				{
					CommonWebServiceProc.getWebReportList(ref list, ref list2, getView);
					List<string> list3 = default(List<string>);
					FileUtil.GetTempSaveList(userCd, out list3);
					WebReportList webReportList3 = new WebReportList();
					webReportList3.Result = "Success";
					webReportList3.Count = count;
					webReportList3.List = list.ToArray();
					webReportList3.TempSaveList = list3.ToArray();
					webReportList3.ImageCheckList = list2.ToArray();
					return webReportList3;
				}
				WebReportList webReportList4 = new WebReportList();
				webReportList4.Result = "Error";
				webReportList4.Message = "サ\u30fcビスでエラ\u30fc発生";
				return webReportList4;
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetRequestReportList", "セッションからのユ\u30fcザ\u30fcコ\u30fcドが取得出来ない。");
			WebReportList webReportList5 = new WebReportList();
			webReportList5.Result = "Error";
			webReportList5.Message = "NoSession";
			return webReportList5;
		}
		catch (Exception ex)
		{
			if (proRadiRSServiceClient != null && proRadiRSServiceClient.State == CommunicationState.Faulted)
			{
				proRadiRSServiceClient.Abort();
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetRequestReportList", ex.ToString());
			WebReportList webReportList6 = new WebReportList();
			webReportList6.Result = "Error";
			webReportList6.Message = "例外発生";
			return webReportList6;
		}
	}

	public static WebReportList GetSearchList(string patientID, string studyDay, string modality)
	{
		ProRadiRSServiceClient proRadiRSServiceClient = null;
		try
		{
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			if (HttpContext.Current.Session["usercd"] != null)
			{
				int userCd = (int)HttpContext.Current.Session["usercd"];
				LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "GetSearchList", "START [" + userCd.ToString() + "]");
				proRadiRSServiceClient = new ProRadiRSServiceClient();
				ReportView[] getView = default(ReportView[]);
				int count = default(int);
				int reportListConditional = proRadiRSServiceClient.GetReportListConditional(patientID, studyDay, modality, out getView, out count);
				proRadiRSServiceClient.Close();
				List<ReportView> list = new List<ReportView>();
				List<string> list2 = new List<string>();
				if (reportListConditional == 0)
				{
					CommonWebServiceProc.getWebReportList(ref list, ref list2, getView);
					List<string> list3 = default(List<string>);
					FileUtil.GetTempSaveList(userCd, out list3);
					WebReportList webReportList = new WebReportList();
					webReportList.Result = "Success";
					webReportList.Count = count;
					webReportList.List = list.ToArray();
					webReportList.TempSaveList = list3.ToArray();
					webReportList.ImageCheckList = list2.ToArray();
					return webReportList;
				}
				WebReportList webReportList2 = new WebReportList();
				webReportList2.Result = "Error";
				webReportList2.Message = "サ\u30fcビスでエラ\u30fc発生";
				return webReportList2;
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetSearchList", "セッションからのユ\u30fcザ\u30fcコ\u30fcドが取得出来ない。");
			WebReportList webReportList3 = new WebReportList();
			webReportList3.Result = "Error";
			webReportList3.Message = "NoSession";
			return webReportList3;
		}
		catch (Exception ex)
		{
			if (proRadiRSServiceClient != null && proRadiRSServiceClient.State == CommunicationState.Faulted)
			{
				proRadiRSServiceClient.Abort();
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetSearchList", ex.ToString());
			WebReportList webReportList4 = new WebReportList();
			webReportList4.Result = "Error";
			webReportList4.Message = "例外発生";
			return webReportList4;
		}
	}

	public static WebReportList GetEmergencyReportList()
	{
		ProRadiRSServiceClient proRadiRSServiceClient = null;
		try
		{
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			if (HttpContext.Current.Session["usercd"] != null)
			{
				int userCd = (int)HttpContext.Current.Session["usercd"];
				LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "GetEmergencyReportList", "START [" + userCd.ToString() + "]");
				if (HttpContext.Current.Session["username"] != null)
				{
					string text = (string)HttpContext.Current.Session["username"];
					proRadiRSServiceClient = new ProRadiRSServiceClient();
					ReportView[] getView = default(ReportView[]);
					int count = default(int);
					int reportListEmergency = proRadiRSServiceClient.GetReportListEmergency(out getView, out count);
					proRadiRSServiceClient.Close();
					List<ReportView> list = new List<ReportView>();
					List<string> list2 = new List<string>();
					if (reportListEmergency == 0)
					{
						CommonWebServiceProc.getWebReportList(ref list, ref list2, getView);
						List<string> list3 = default(List<string>);
						FileUtil.GetTempSaveList(userCd, out list3);
						WebReportList webReportList = new WebReportList();
						webReportList.Result = "Success";
						webReportList.Count = count;
						webReportList.List = list.ToArray();
						webReportList.TempSaveList = list3.ToArray();
						webReportList.ImageCheckList = list2.ToArray();
						return webReportList;
					}
					WebReportList webReportList2 = new WebReportList();
					webReportList2.Result = "Error";
					webReportList2.Message = "サ\u30fcビスでエラ\u30fc発生";
					return webReportList2;
				}
				LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetEmergencyReportList", "セッションからユ\u30fcザ\u30fc名が取得出来ません。");
				WebReportList webReportList3 = new WebReportList();
				webReportList3.Result = "Error";
				webReportList3.Message = "NoSession";
				return webReportList3;
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetEmergencyReportList", "セッションからのユ\u30fcザ\u30fcコ\u30fcドが取得出来ない。");
			WebReportList webReportList4 = new WebReportList();
			webReportList4.Result = "Error";
			webReportList4.Message = "NoSession";
			return webReportList4;
		}
		catch (Exception ex)
		{
			if (proRadiRSServiceClient != null && proRadiRSServiceClient.State == CommunicationState.Faulted)
			{
				proRadiRSServiceClient.Abort();
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetEmergencyReportList", ex.ToString());
			WebReportList webReportList5 = new WebReportList();
			webReportList5.Result = "Error";
			webReportList5.Message = "例外発生";
			return webReportList5;
		}
	}

	public static WebReportList GetReadReportList(string isusernametype, string username)
	{
		ProRadiRSServiceClient proRadiRSServiceClient = null;
		try
		{
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			if (HttpContext.Current.Session["usercd"] != null)
			{
				int userCd = (int)HttpContext.Current.Session["usercd"];
				LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "GetReadReportList", "START [" + userCd.ToString() + "]");
				int num = default(int);
				if (!int.TryParse(isusernametype, out num))
				{
					num = 0;
				}
				switch (num)
				{
				case 0:
				{
					if (HttpContext.Current.Session["username"] != null)
					{
						username = (string)HttpContext.Current.Session["username"];
						LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "GetReadReportList", "検索ユ\u30fcザ\u30fc名(セッションから取得) [" + username + "]");
						break;
					}
					LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetReadReportList", "セッションからユ\u30fcザ\u30fc名(依頼先医師名)が取得出来ません。");
					WebReportList webReportList2 = new WebReportList();
					webReportList2.Result = "Error";
					webReportList2.Message = "NoSession";
					return webReportList2;
				}
				case 1:
					if (username.Length > 0)
					{
						LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "GetReadReportList", "検索ユ\u30fcザ\u30fc名(引数から取得) [" + username + "]");
					}
					else
					{
						LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "GetReadReportList", "検索ユ\u30fcザ\u30fc名指定なし");
					}
					break;
				default:
				{
					string text = "検索ユ\u30fcザ\u30fc名取得方法が未定義です。";
					LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetReadReportList", text);
					WebReportList webReportList = new WebReportList();
					webReportList.Result = "Error";
					webReportList.Message = text;
					return webReportList;
				}
				}
				proRadiRSServiceClient = new ProRadiRSServiceClient();
				ReportView[] getView = default(ReportView[]);
				int count = default(int);
				int reportListRead = proRadiRSServiceClient.GetReportListRead(username, out getView, out count);
				proRadiRSServiceClient.Close();
				List<ReportView> list = new List<ReportView>();
				List<string> list2 = new List<string>();
				if (reportListRead == 0)
				{
					CommonWebServiceProc.getWebReportList(ref list, ref list2, getView);
					List<string> list3 = default(List<string>);
					FileUtil.GetTempSaveList(userCd, out list3);
					WebReportList webReportList3 = new WebReportList();
					webReportList3.Result = "Success";
					webReportList3.Count = count;
					webReportList3.List = list.ToArray();
					webReportList3.TempSaveList = list3.ToArray();
					webReportList3.ImageCheckList = list2.ToArray();
					return webReportList3;
				}
				WebReportList webReportList4 = new WebReportList();
				webReportList4.Result = "Error";
				webReportList4.Message = "サ\u30fcビスでエラ\u30fc発生";
				return webReportList4;
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetReadReportList", "セッションからのユ\u30fcザ\u30fcコ\u30fcドが取得出来ない。");
			WebReportList webReportList5 = new WebReportList();
			webReportList5.Result = "Error";
			webReportList5.Message = "NoSession";
			return webReportList5;
		}
		catch (Exception ex)
		{
			if (proRadiRSServiceClient != null && proRadiRSServiceClient.State == CommunicationState.Faulted)
			{
				proRadiRSServiceClient.Abort();
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetReadReportList", ex.ToString());
			WebReportList webReportList6 = new WebReportList();
			webReportList6.Result = "Error";
			webReportList6.Message = "例外発生";
			return webReportList6;
		}
	}

	public static WebReport GetReportData(string serialNo, string readTempSave)
	{
		ProRadiRSServiceClient proRadiRSServiceClient = null;
		try
		{
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			int serialNo2 = default(int);
			if (!int.TryParse(serialNo, out serialNo2))
			{
				LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetReportData", "SerialNoが数値出ない値です。[" + serialNo + "]");
				WebReport webReport = new WebReport();
				webReport.Result = "Error";
				webReport.Message = "サ\u30fcビスでエラ\u30fc発生";
				return webReport;
			}
			if (HttpContext.Current.Session["usercd"] != null)
			{
				int userCd = (int)HttpContext.Current.Session["usercd"];
				LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "GetReportData", "START [" + userCd.ToString() + "]");
				if (HttpContext.Current.Session["non-edit"] != null)
				{
					string editFlg = (string)HttpContext.Current.Session["non-edit"];
					proRadiRSServiceClient = new ProRadiRSServiceClient();
					ReportView reportView = default(ReportView);
					string[] array = default(string[]);
					ReportHistory[] array2 = default(ReportHistory[]);
					int reportData = proRadiRSServiceClient.GetReportData(serialNo2, userCd, ConfigUtil._ImageViewPath, out reportView, out array, out array2);
					proRadiRSServiceClient.Close();

                    var service2 = new ProRadiRSService2Client();

                    array = service2.GetImageExt(userCd, serialNo, ConfigUtil._ImageViewPath);

                    if (reportData == 0)
					{
						ReportView reportView2 = new ReportView();
						reportView2.SerialNo = reportView.SerialNo;
						reportView2.OrderNo = reportView.OrderNo;
						reportView2.PatientID = reportView.PatientID;
						reportView2.PatientSex = reportView.PatientSex;
						reportView2.PatientBirthDate = reportView.PatientBirthDate;
						reportView2.PatientAge = reportView.PatientAge;
						reportView2.StudyDate = reportView.StudyDate;
						reportView2.StudyTime = reportView.StudyTime;
						reportView2.Modality = reportView.Modality;
						reportView2.StudyBodyPart = reportView.StudyBodyPart;
						reportView2.ReportStatus = reportView.ReportStatus;
						reportView2.SubStatus = reportView.SubStatus;
						reportView2.ReadingStatus = reportView.ReadingStatus;
						reportView2.PriorityFlag = reportView.PriorityFlag;
						reportView2.ReportReserve1 = reportView.ReportReserve1;
						reportView2.ReadFlag = reportView.ReadFlag;
						reportView2.HistoryNo = reportView.HistoryNo;
						reportView2.ImportStatus = reportView.ImportStatus;
						reportView2.PatientName = reportView.PatientName;
						reportView2.PatientNameHankaku = reportView.PatientNameHankaku;
						reportView2.PatientNameZenkaku = reportView.PatientNameZenkaku;
						reportView2.Visit = reportView.Visit;
						reportView2.Department = reportView.Department;
						reportView2.PhysicianName = reportView.PhysicianName;
						reportView2.Ward = reportView.Ward;
						reportView2.StudyType = reportView.StudyType;
						reportView2.StudyMode = reportView.StudyMode;
						reportView2.ContrastAgent = reportView.ContrastAgent;
						reportView2.Comment1 = reportView.Comment1;
						reportView2.Comment2 = reportView.Comment2;
						reportView2.Comment3 = reportView.Comment3;
						reportView2.RequestedPhysicianName = reportView.RequestedPhysicianName;
						reportView2.ReadDate = reportView.ReadDate;
						reportView2.ReadTime = reportView.ReadTime;
						reportView2.ReadDepartment = reportView.ReadDepartment;
						reportView2.ReadPhysicianName = reportView.ReadPhysicianName;
						reportView2.TranscriberName = reportView.TranscriberName;
						reportView2.RequestMessage = reportView.RequestMessage;
						reportView2.RequestHistory = reportView.RequestHistory;
						reportView2.Finding = reportView.Finding;
						reportView2.FindingRTF = reportView.FindingRTF;
						reportView2.Diagnosing = reportView.Diagnosing;
						reportView2.DiagnosingRTF = reportView.DiagnosingRTF;
						reportView2.PdfFile = reportView.PdfFile;
						reportView2.ImageUmu = reportView.ImageUmu;
						reportView2.InfoUmu = reportView.InfoUmu;
						reportView2.InfoList = reportView.InfoList;
						reportView2.HealthCheckup = reportView.HealthCheckup;
						reportView2.ImageAdvice = reportView.ImageAdvice;
						reportView2.UpdateDate = reportView.UpdateDate;
						reportView2.Edition = reportView.Edition;
						reportView2.OperatorName = reportView.OperatorName;
						reportView2.ReportReserve1 = reportView.ReportReserve1;
						reportView2.ReportReserve2 = reportView.ReportReserve2;
						reportView2.ReportReserve3 = reportView.ReportReserve3;
						reportView2.ReportReserve4 = reportView.ReportReserve4;
						reportView2.ReportReserve5 = reportView.ReportReserve5;
						reportView2.DiagnosisPhysicianName = reportView.DiagnosisPhysicianName;
						reportView2.AuthorizationPhysicianName = reportView.AuthorizationPhysicianName;
						reportView2.ReadReserve1 = reportView.ReadReserve1;
						reportView2.ReadReserve2 = reportView.ReadReserve2;
						reportView2.ReadReserve3 = reportView.ReadReserve3;
						reportView2.ReadReserve4 = reportView.ReadReserve4;
						reportView2.ReadReserve5 = reportView.ReadReserve5;
						reportView2.ReferringPhysicianName = reportView.ReferringPhysicianName;
						reportView2.ReferringInstitutionName = reportView.ReferringInstitutionName;
						reportView2.PostscriptPhysicianName = reportView.PostscriptPhysicianName;
						reportView2.ReadOrder = reportView.ReadOrder;
						reportView2.RequestDate = reportView.RequestDate;
						reportView2.RequestTime = reportView.RequestTime;
						reportView2.HospitalOrderNo = reportView.HospitalOrderNo;
						reportView2.HospitalPatientID = reportView.HospitalPatientID;
						reportView2.DecisionRequestFlag = reportView.DecisionRequestFlag;
						reportView2.StudyInstanceUID = reportView.StudyInstanceUID;
						reportView2.ReportReserve6 = reportView.ReportReserve6;
						reportView2.ReportReserve7 = reportView.ReportReserve7;
						reportView2.ReportReserve8 = reportView.ReportReserve8;
						reportView2.ReportReserve9 = reportView.ReportReserve9;
						reportView2.ReportReserve10 = reportView.ReportReserve10;
						List<string> list = new List<string>();
						string[] array3 = array;
						foreach (string item in array3)
						{
							list.Add(item);
						}
						List<ReportHistory> list2 = new List<ReportHistory>();
						ReportHistory[] array4 = array2;
						foreach (ReportHistory reportHistory in array4)
						{
							ReportHistory reportHistory2 = new ReportHistory();
							reportHistory2.SerialNo = reportHistory.SerialNo;
							reportHistory2.StudyDate = reportHistory.StudyDate;
							reportHistory2.StudyTime = reportHistory.StudyTime;
							reportHistory2.Modality = reportHistory.Modality;
							reportHistory2.StudyBodyPart = reportHistory.StudyBodyPart;
							reportHistory2.OrderNo = reportHistory.OrderNo;
							reportHistory2.PatientID = reportHistory.PatientID;
							reportHistory2.Diagnosing = reportHistory.Diagnosing;
							reportHistory2.Finding = "【所見】\r\n" + reportHistory.Finding + "\r\n\r\n【診断】\r\n" + reportHistory.Diagnosing;
							list2.Add(reportHistory2);
						}
						if (readTempSave == "1")
						{
							string text = Path.Combine(ConfigUtil._TempSaveReportPath, userCd.ToString());
							if (Directory.Exists(text))
							{
								FileUtil.SaveXmlRead(text, serialNo, ref reportView2, ref list);
							}
						}
						LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "GetReportData", "依頼票画像パス取得");
						string text2 = default(string);
						if (FileUtil.ExistsExamOrder(out text2, reportView.ReportReserve2))
						{
							HttpContext.Current.Session["examorderpath"] = text2;
						}
						else
						{
							text2 = "";
							HttpContext.Current.Session["examorderpath"] = "";
						}
						WebReport webReport2 = new WebReport();
						webReport2.Result = "Success";
						webReport2.View = reportView2;
						webReport2.Image = list.ToArray();
						webReport2.History = list2.ToArray();
						webReport2.ExamOrder = text2;
						webReport2.editFlg = editFlg;
						return webReport2;
					}
					WebReport webReport3 = new WebReport();
					webReport3.Result = "Error";
					webReport3.Message = "サ\u30fcビスでエラ\u30fc発生";
					return webReport3;
				}
				LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetReportData", "セッションから閲覧フラグが取得出来ません。");
				WebReport webReport4 = new WebReport();
				webReport4.Result = "Error";
				webReport4.Message = "NoSession";
				return webReport4;
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetReportData", "セッションからユ\u30fcザ\u30fcコ\u30fcドが取得出来ません。");
			WebReport webReport5 = new WebReport();
			webReport5.Result = "Error";
			webReport5.Message = "NoSession";
			return webReport5;
		}
		catch (Exception ex)
		{
			if (proRadiRSServiceClient != null && proRadiRSServiceClient.State == CommunicationState.Faulted)
			{
				proRadiRSServiceClient.Abort();
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetReportData", ex.ToString());
			WebReport webReport6 = new WebReport();
			webReport6.Result = "Error";
			webReport6.Message = "例外発生";
			return webReport6;
		}
	}

	public static WebImageList GetImageList(int serialNo, int imageNum, string search)
	{
		try
		{
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			if (HttpContext.Current.Session["usercd"] != null)
			{
				int num = (int)HttpContext.Current.Session["usercd"];
				LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "GetImageList", "START [" + num.ToString() + "]");
				if (!Directory.Exists(ConfigUtil._ImageImportPath))
				{
					LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetImageList", "画像取込パスが見つかりません。[" + ConfigUtil._ImageImportPath + "]");
					WebImageList webImageList = new WebImageList();
					webImageList.Result = "Error";
					webImageList.Message = "NoPath";
					return webImageList;
				}
				string text = Path.Combine(ConfigUtil._ImageImportPath, num.ToString());
				if (!Directory.Exists(text))
				{
					LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetImageList", "画像取込パスが見つかりません。[" + text + "]");
					List<string> list = new List<string>();
					WebImageList webImageList2 = new WebImageList();
					webImageList2.Result = "Success";
					webImageList2.List = list.ToArray();
					return webImageList2;
				}
				string searchPattern = Path.ChangeExtension(serialNo.ToString() + "*", ConfigUtil._ImageExt);
				string[] fileSystemEntries = Directory.GetFileSystemEntries(text, searchPattern, SearchOption.TopDirectoryOnly);
                if(fileSystemEntries.Length == 0)
                    fileSystemEntries = Directory.GetFileSystemEntries(text, "*.png", SearchOption.TopDirectoryOnly);

                List<string> list2 = new List<string>();
				string text2 = Path.Combine(ConfigUtil._ImageViewPath, num.ToString());
				if (!Directory.Exists(text2))
				{
					Directory.CreateDirectory(text2);
				}
				int num2 = default(int);
				if (!int.TryParse(ConfigUtil._MaxImageImportNum, out num2))
				{
					num2 = 6;
				}
				int num3 = imageNum;
				string[] array = fileSystemEntries;
				foreach (string text3 in array)
				{
					if (num3 < num2)
					{
                        FileInfo finfo = new FileInfo(text3);
                        if (finfo.Length == 0)
                            continue;
                        if(Path.GetExtension(text3) == ".png")
                        {
                            using (Bitmap img = new Bitmap(text3))
                            {
                                string text4 = Path.Combine(text2, Path.GetFileNameWithoutExtension(text3));

                                if (text4.IndexOf(search) < 0)
                                    text4 += "_alert";

                                text4 += ".jpg";

                                img.Save(text4, ImageFormat.Jpeg);
                                list2.Add(Path.GetFileNameWithoutExtension(text4));
                            }

                            File.Delete(text3);
                        }
                        else
                        {
                            try
                            {
                                string text4 = Path.Combine(text2, Path.GetFileName(text3));
                                File.Copy(text3, text4, true);
                                if (File.Exists(text4))
                                {
                                    File.Delete(text3);
                                }
                            }
                            catch
                            {
                            }
                            list2.Add(Path.GetFileNameWithoutExtension(text3));
                        }
                        num3++;
					}
				}
				WebImageList webImageList3 = new WebImageList();
				webImageList3.Result = "Success";
				webImageList3.List = list2.ToArray();
				return webImageList3;
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetImageList", "セッションからユ\u30fcザ\u30fcコ\u30fcドが取得出来ません。");
			WebImageList webImageList4 = new WebImageList();
			webImageList4.Result = "Error";
			webImageList4.Message = "NoSession";
			return webImageList4;
		}
		catch (Exception ex)
		{
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetImageList", ex.ToString());
			WebImageList webImageList5 = new WebImageList();
			webImageList5.Result = "Error";
			webImageList5.Message = "Exception";
			return webImageList5;
		}
	}

	public static WebResult ClearImagePath()
	{
		try
		{
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			if (HttpContext.Current.Session["usercd"] != null)
			{
				int num = (int)HttpContext.Current.Session["usercd"];
				LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "ClearImagePath", "START [" + num.ToString() + "]");
				string clearPath = Path.Combine(ConfigUtil._ImageImportPath, num.ToString());
				FileUtil.ImageClear(clearPath);
				clearPath = Path.Combine(ConfigUtil._HistoryImageViewPath, num.ToString());
				FileUtil.ImageClear(clearPath);
				WebResult webResult = new WebResult();
				webResult.Result = "Success";
				return webResult;
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "ClearImagePath", "セッションからのユ\u30fcザ\u30fcコ\u30fcドが取得出来ない。");
			WebResult webResult2 = new WebResult();
			webResult2.Result = "Error";
			webResult2.Message = "NoSession";
			return webResult2;
		}
		catch (Exception ex)
		{
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "ClearImagePath", ex.ToString());
			WebParams webParams = new WebParams();
			webParams.Result = "Error";
			webParams.Message = "Exception";
			return webParams;
		}
	}

	public static void GetImage(int userCd, string viewPath)
	{
		try
		{
			string path = HttpContext.Current.Request.QueryString["key"];
			byte[] array = null;
			string path2 = Path.ChangeExtension(path, ConfigUtil._ImageExt);
			string path3 = Path.Combine(Path.Combine(viewPath, userCd.ToString()), path2);
			using (FileStream fileStream = new FileStream(path3, FileMode.Open, FileAccess.Read))
			{
				array = new byte[fileStream.Length];
				fileStream.Read(array, 0, array.Length);
			}
			HttpContext.Current.Response.ClearContent();
			HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.AddSeconds(3600.0));
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Public);
			HttpContext.Current.Response.Cache.SetValidUntilExpires(true);
			HttpContext.Current.Response.ContentType = "image/jpeg";
			HttpContext.Current.Response.BinaryWrite(array);
			HttpContext.Current.ApplicationInstance.CompleteRequest();
		}
		catch (Exception ex)
		{
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetImage", ex.ToString());
		}
	}

	public static WebResult SaveReportData(string[] key, string[] val, string[] image)
	{
		try
		{
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			if (HttpContext.Current.Session["usercd"] != null)
			{
				int num = (int)HttpContext.Current.Session["usercd"];
				LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "SaveReportData", "START [" + num.ToString() + "]");
				if (key.Length != val.Length)
				{
					LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "SaveReportData", "レポ\u30fcト保存のkeyとvalueの数が一致しません。key[" + key.Length + "] val[" + val.Length + "]");
					WebResult webResult = new WebResult();
					webResult.Result = "Error";
					webResult.Message = "ProRadiRS Error";
					return webResult;
				}
				string text = "";
				string text2 = "";
				string text3 = "";
				string finding = "";
				string diagnosing = "";
				for (int i = 0; i < key.Length; i++)
				{
					if (key[i].ToLower() == "SerialNo".ToLower())
					{
						text = val[i];
					}
					else if (key[i].ToLower() == "OrderNo".ToLower())
					{
						text2 = val[i];
					}
					else if (key[i].ToLower() == "OfficeCD".ToLower())
					{
						text3 = val[i];
					}
					else if (key[i].ToLower() == "Finding".ToLower())
					{
						finding = val[i];
					}
					else if (key[i].ToLower() == "Diagnosing".ToLower())
					{
						diagnosing = val[i];
					}
				}
				if (text.Length == 0)
				{
					LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "SaveReportData", "保存するレポ\u30fcトのシリアル番号が不明です。");
					WebResult webResult2 = new WebResult();
					webResult2.Result = "Error";
					webResult2.Message = "ProRadiRS Error";
					return webResult2;
				}
				if (text2.Length == 0)
				{
					LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "SaveReportData", "保存するレポ\u30fcトのオ\u30fcダ\u30fc番号が不明です。");
					WebResult webResult3 = new WebResult();
					webResult3.Result = "Error";
					webResult3.Message = "ProRadiRS Error";
					return webResult3;
				}
				if (text3.Length == 0)
				{
					LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "SaveReportData", "保存するレポ\u30fcトの事業所コ\u30fcドが不明です。");
					WebResult webResult4 = new WebResult();
					webResult4.Result = "Error";
					webResult4.Message = "ProRadiRS Error";
					return webResult4;
				}
				LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "SaveReportData", "UserCd [" + num + "] SerialNo [" + text + "] OrderNo [" + text2 + "] OfficeCD [" + text3 + "]");
				if (!Directory.Exists(ConfigUtil._SaveReportPath))
				{
					LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "SaveReportData", "レポ\u30fcト保存XML出力PATHが見つかりません。[" + ConfigUtil._SaveReportPath + "]");
					WebResult webResult5 = new WebResult();
					webResult5.Result = "Error";
					webResult5.Message = "ProRadiRS Error";
					return webResult5;
				}
				string path = Path.ChangeExtension(text + "_" + DateTime.Now.ToString("yyMMddHHmmssff"), "xml");
				string text4 = Path.Combine(ConfigUtil._SaveReportPath, path);
				string trgFile = Path.ChangeExtension(text4, "txt");
				FileUtil.SaveXmlOutput(text4, trgFile, num, text, text2, text3, finding, diagnosing, image);
				LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "SaveReportData", "UserCd [" + num + "] FileName [" + text4 + "]");
				string clearPath = Path.Combine(ConfigUtil._TempSaveReportPath, num.ToString());
				FileUtil.TempSaveClear(clearPath, text);
				WebResult webResult6 = new WebResult();
				webResult6.Result = "Success";
				return webResult6;
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "SaveReportData", "セッションからユ\u30fcザ\u30fcコ\u30fcドが取得出来ません。");
			WebResult webResult7 = new WebResult();
			webResult7.Result = "Error";
			webResult7.Message = "NoSession";
			return webResult7;
		}
		catch (Exception ex)
		{
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "SaveReportData", ex.ToString());
			WebResult webResult8 = new WebResult();
			webResult8.Result = "Error";
			webResult8.Message = "Exception";
			return webResult8;
		}
	}

	public static WebResult TempSaveReportData(string[] key, string[] val, string[] image)
	{
		try
		{
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			if (HttpContext.Current.Session["usercd"] != null)
			{
				int num = (int)HttpContext.Current.Session["usercd"];
				LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "TempSaveReportData", "START [" + num.ToString() + "]");
				if (key.Length != val.Length)
				{
					LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "TempSaveReportData", "レポ\u30fcト保存のkeyとvalueの数が一致しません。key[" + key.Length + "] val[" + val.Length + "]");
					WebResult webResult = new WebResult();
					webResult.Result = "Error";
					webResult.Message = "ProRadiRS Error";
					return webResult;
				}
				string text = "";
				string text2 = "";
				string text3 = "";
				string finding = "";
				string diagnosing = "";
				for (int i = 0; i < key.Length; i++)
				{
					if (key[i].ToLower() == "SerialNo".ToLower())
					{
						text = val[i];
					}
					else if (key[i].ToLower() == "OrderNo".ToLower())
					{
						text2 = val[i];
					}
					else if (key[i].ToLower() == "OfficeCD".ToLower())
					{
						text3 = val[i];
					}
					else if (key[i].ToLower() == "Finding".ToLower())
					{
						finding = val[i];
					}
					else if (key[i].ToLower() == "Diagnosing".ToLower())
					{
						diagnosing = val[i];
					}
				}
                //if (text.Length == 0)
                //{
                //	LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "TempSaveReportData", "保存するレポ\u30fcトのシリアル番号が不明です。");
                //	WebResult webResult2 = new WebResult();
                //	webResult2.Result = "Error";
                //	webResult2.Message = "ProRadiRS Error";
                //	return webResult2;
                //}
                //if (text2.Length == 0)
                //{
                //	LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "TempSaveReportData", "保存するレポ\u30fcトのオ\u30fcダ\u30fc番号が不明です。");
                //	WebResult webResult3 = new WebResult();
                //	webResult3.Result = "Error";
                //	webResult3.Message = "ProRadiRS Error";
                //	return webResult3;
                //}
                //if (text3.Length == 0)
                //{
                //	LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "TempSaveReportData", "保存するレポ\u30fcトの事業所コ\u30fcドが不明です。");
                //	WebResult webResult4 = new WebResult();
                //	webResult4.Result = "Error";
                //	webResult4.Message = "ProRadiRS Error";
                //	return webResult4;
                //}
                //LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "TempSaveReportData", "UserCd [" + num + "] SerialNo [" + text + "] OrderNo [" + text2 + "] OfficeCD [" + text3 + "]");
                //string path = Path.ChangeExtension(text + "_" + DateTime.Now.ToString("yyMMddHHmmssff"), "xml");
                //string text4 = Path.Combine(ConfigUtil._TempSaveReportPath, num.ToString());
                //string text5 = Path.Combine(text4, path);
                //if (!Directory.Exists(text4))
                //{
                //	Directory.CreateDirectory(text4);
                //}
                //FileUtil.SaveXmlOutput(text5, "", num, text, text2, text3, finding, diagnosing, image);

                // 20180605 Edit Matsuda SaveReport Change
                WebResult webResult5 = new WebResult();

                ProRadiRSService2Client proRadServicesClient2 = null;
                proRadServicesClient2 = new ProRadiRSService2Client();

                var ret = proRadServicesClient2.SaveReport(num, Convert.ToInt32(text), finding, diagnosing, image.Length);

                if (ret)
                {
                    var imagePath = new List<string>();

                    foreach(var img in image)
                    {
                        string path2 = Path.ChangeExtension(img, ConfigUtil._ImageExt);
                        string path3 = Path.Combine(Path.Combine(ConfigUtil._ImageViewPath, num.ToString()), path2);

                        imagePath.Add(path3);
                    }

                    ret = proRadServicesClient2.SaveReportImage(text, imagePath.ToArray());

                    LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "TempSaveReportData", "UserCd [" + num + "] SerialNo [" + text + "]");
                    webResult5.Result = "Success";
                }else
                {
                    LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "TempSaveReportData", "SaveReport Error");
                    webResult5.Result = "Error";
                }

                return webResult5;
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "TempSaveReportData", "セッションからユ\u30fcザ\u30fcコ\u30fcドが取得出来ません。");
			WebResult webResult6 = new WebResult();
			webResult6.Result = "Error";
			webResult6.Message = "NoSession";
			return webResult6;
		}
		catch (Exception ex)
		{
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "TempSaveReportData", ex.ToString());
			WebResult webResult7 = new WebResult();
			webResult7.Result = "Error";
			webResult7.Message = "Exception";
			return webResult7;
		}
	}

	public static WebResult DeleteImage(string image)
	{
		try
		{
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			int num = 0;
			if (HttpContext.Current.Session["usercd"] != null)
			{
				num = (int)HttpContext.Current.Session["usercd"];
				LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "DeleteImage", "START [" + num.ToString() + "]");
				string text = Path.Combine(Path.Combine(ConfigUtil._ImageViewPath, num.ToString(), Path.ChangeExtension(image, ConfigUtil._ImageExt)));
				if (File.Exists(text))
				{
					FileInfo fileInfo = new FileInfo(text);
					if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
					{
						fileInfo.Attributes = FileAttributes.Normal;
					}
					fileInfo.Delete();
				}
				WebResult webResult = new WebResult();
				webResult.Result = "Success";
				return webResult;
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "DeleteImage", "セッションからユ\u30fcザ\u30fcコ\u30fcドが取得出来ません。");
			WebImageList webImageList = new WebImageList();
			webImageList.Result = "Error";
			webImageList.Message = "NoSession";
			return webImageList;
		}
		catch (Exception ex)
		{
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "DeleteImage", ex.ToString());
			WebImageList webImageList2 = new WebImageList();
			webImageList2.Result = "Error";
			webImageList2.Message = "Exception";
			return webImageList2;
		}
	}

	public static WebHistoryReport GetHistoryReportData(int serialNo)
	{
		ProRadiRSServiceClient proRadiRSServiceClient = null;
		try
		{
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			if (HttpContext.Current.Session["usercd"] != null)
			{
				int userCd = (int)HttpContext.Current.Session["usercd"];
				LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "GetHistoryReportData", "START [" + userCd.ToString() + "]");
				proRadiRSServiceClient = new ProRadiRSServiceClient();
				ReportView reportView = default(ReportView);
				string[] array = default(string[]);
				int historyReportData = proRadiRSServiceClient.GetHistoryReportData(userCd, serialNo, ConfigUtil._HistoryImageViewPath, out reportView, out array);
				proRadiRSServiceClient.Close();
				if (historyReportData == 0)
				{
					ReportView reportView2 = new ReportView();
					reportView2.SerialNo = reportView.SerialNo;
					reportView2.OrderNo = reportView.OrderNo;
					reportView2.PatientID = reportView.PatientID;
					reportView2.PatientSex = reportView.PatientSex;
					reportView2.PatientBirthDate = reportView.PatientBirthDate;
					reportView2.PatientAge = reportView.PatientAge;
					reportView2.StudyDate = reportView.StudyDate;
					reportView2.StudyTime = reportView.StudyTime;
					reportView2.Modality = reportView.Modality;
					reportView2.StudyBodyPart = reportView.StudyBodyPart;
					reportView2.ReportStatus = reportView.ReportStatus;
					reportView2.SubStatus = reportView.SubStatus;
					reportView2.ReadingStatus = reportView.ReadingStatus;
					reportView2.PriorityFlag = reportView.PriorityFlag;
					reportView2.ReportReserve1 = reportView.ReportReserve1;
					reportView2.ReadFlag = reportView.ReadFlag;
					reportView2.HistoryNo = reportView.HistoryNo;
					reportView2.ImportStatus = reportView.ImportStatus;
					reportView2.PatientName = reportView.PatientName;
					reportView2.PatientNameHankaku = reportView.PatientNameHankaku;
					reportView2.PatientNameZenkaku = reportView.PatientNameZenkaku;
					reportView2.Visit = reportView.Visit;
					reportView2.Department = reportView.Department;
					reportView2.PhysicianName = reportView.PhysicianName;
					reportView2.Ward = reportView.Ward;
					reportView2.StudyType = reportView.StudyType;
					reportView2.StudyMode = reportView.StudyMode;
					reportView2.ContrastAgent = reportView.ContrastAgent;
					reportView2.Comment1 = reportView.Comment1;
					reportView2.Comment2 = reportView.Comment2;
					reportView2.Comment3 = reportView.Comment3;
					reportView2.RequestedPhysicianName = reportView.RequestedPhysicianName;
					reportView2.ReadDate = reportView.ReadDate;
					reportView2.ReadTime = reportView.ReadTime;
					reportView2.ReadDepartment = reportView.ReadDepartment;
					reportView2.ReadPhysicianName = reportView.ReadPhysicianName;
					reportView2.TranscriberName = reportView.TranscriberName;
					reportView2.RequestMessage = reportView.RequestMessage;
					reportView2.RequestHistory = reportView.RequestHistory;
					reportView2.Finding = reportView.Finding;
					reportView2.FindingRTF = reportView.FindingRTF;
					reportView2.Diagnosing = reportView.Diagnosing;
					reportView2.DiagnosingRTF = reportView.DiagnosingRTF;
					reportView2.PdfFile = reportView.PdfFile;
					reportView2.ImageUmu = reportView.ImageUmu;
					reportView2.InfoUmu = reportView.InfoUmu;
					reportView2.InfoList = reportView.InfoList;
					reportView2.HealthCheckup = reportView.HealthCheckup;
					reportView2.ImageAdvice = reportView.ImageAdvice;
					reportView2.UpdateDate = reportView.UpdateDate;
					reportView2.Edition = reportView.Edition;
					reportView2.OperatorName = reportView.OperatorName;
					reportView2.ReportReserve1 = reportView.ReportReserve1;
					reportView2.ReportReserve2 = reportView.ReportReserve2;
					reportView2.ReportReserve3 = reportView.ReportReserve3;
					reportView2.ReportReserve4 = reportView.ReportReserve4;
					reportView2.ReportReserve5 = reportView.ReportReserve5;
					reportView2.DiagnosisPhysicianName = reportView.DiagnosisPhysicianName;
					reportView2.AuthorizationPhysicianName = reportView.AuthorizationPhysicianName;
					reportView2.ReadReserve1 = reportView.ReadReserve1;
					reportView2.ReadReserve2 = reportView.ReadReserve2;
					reportView2.ReadReserve3 = reportView.ReadReserve3;
					reportView2.ReadReserve4 = reportView.ReadReserve4;
					reportView2.ReadReserve5 = reportView.ReadReserve5;
					reportView2.ReferringPhysicianName = reportView.ReferringPhysicianName;
					reportView2.ReferringInstitutionName = reportView.ReferringInstitutionName;
					reportView2.PostscriptPhysicianName = reportView.PostscriptPhysicianName;
					reportView2.ReadOrder = reportView.ReadOrder;
					reportView2.RequestDate = reportView.RequestDate;
					reportView2.RequestTime = reportView.RequestTime;
					reportView2.HospitalOrderNo = reportView.HospitalOrderNo;
					reportView2.HospitalPatientID = reportView.HospitalPatientID;
					reportView2.DecisionRequestFlag = reportView.DecisionRequestFlag;
					reportView2.StudyInstanceUID = reportView.StudyInstanceUID;
					reportView2.ReportReserve6 = reportView.ReportReserve6;
					reportView2.ReportReserve7 = reportView.ReportReserve7;
					reportView2.ReportReserve8 = reportView.ReportReserve8;
					reportView2.ReportReserve9 = reportView.ReportReserve9;
					reportView2.ReportReserve10 = reportView.ReportReserve10;
					List<string> list = new List<string>();
					string[] array2 = array;
					foreach (string item in array2)
					{
						list.Add(item);
					}
					WebHistoryReport webHistoryReport = new WebHistoryReport();
					webHistoryReport.Result = "Success";
					webHistoryReport.View = reportView2;
					webHistoryReport.Image = list.ToArray();
					return webHistoryReport;
				}
				WebHistoryReport webHistoryReport2 = new WebHistoryReport();
				webHistoryReport2.Result = "Error";
				webHistoryReport2.Message = "サ\u30fcビスでエラ\u30fc発生";
				return webHistoryReport2;
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetHistoryReportData", "セッションからユ\u30fcザ\u30fcコ\u30fcドが取得出来ません。");
			WebHistoryReport webHistoryReport3 = new WebHistoryReport();
			webHistoryReport3.Result = "Error";
			webHistoryReport3.Message = "NoSession";
			return webHistoryReport3;
		}
		catch (Exception ex)
		{
			if (proRadiRSServiceClient != null && proRadiRSServiceClient.State == CommunicationState.Faulted)
			{
				proRadiRSServiceClient.Abort();
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetReportData", ex.ToString());
			WebHistoryReport webHistoryReport4 = new WebHistoryReport();
			webHistoryReport4.Result = "Error";
			webHistoryReport4.Message = "例外発生";
			return webHistoryReport4;
		}
	}

	public static WebResult SetViewerImageCheckFile(string checktype, string orderno)
	{
		try
		{
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			int num = 0;
			if (HttpContext.Current.Session["usercd"] != null)
			{
				num = (int)HttpContext.Current.Session["usercd"];
				string str = "";
				if (checktype == "0")
				{
					str = "画像確認ファイル出力";
				}
				else if (checktype == "1")
				{
					str = "画像確認ファイル削除";
				}
				LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "SetViewerImageCheckFile", "START [" + num.ToString() + "] " + str);
				if (orderno.Length > 0)
				{
					string path = "";
					if (orderno.Length >= 3)
					{
						path = orderno.Substring(0, 3);
					}
					if (ConfigUtil._ImageCheckPath.Length > 0)
					{
						string path2 = Path.ChangeExtension(orderno, "txt");
						string text = Path.Combine(ConfigUtil._ImageCheckPath, path);
						string text2 = Path.Combine(text, path2);
						if (checktype == "0")
						{
							if (!Directory.Exists(text))
							{
								Directory.CreateDirectory(text);
							}
							using (StreamWriter streamWriter = new StreamWriter(text2, false, Encoding.GetEncoding("shift_jis")))
							{
								streamWriter.Close();
							}
							LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "SetViewerImageCheckFile", "画像確認ファイル出力 [" + text2 + "]");
						}
						else if (checktype == "1")
						{
							if (File.Exists(text2))
							{
								File.Delete(text2);
								LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "SetViewerImageCheckFile", "画像確認ファイル削除 [" + text2 + "]");
							}
							else
							{
								LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "SetViewerImageCheckFile", "画像確認ファイル削除 対象ファイルなし [" + text2 + "]");
							}
						}
					}
					WebResult webResult = new WebResult();
					webResult.Result = "Success";
					return webResult;
				}
				LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "SetViewerImageCheckFile", "オ\u30fcダ\u30fc番号が不明");
				WebResult webResult2 = new WebResult();
				webResult2.Result = "Error";
				webResult2.Message = "オ\u30fcダ\u30fc番号";
				return webResult2;
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "SetViewerImageCheckFile", "セッションからユ\u30fcザ\u30fcコ\u30fcドが取得出来ません。");
			WebImageList webImageList = new WebImageList();
			webImageList.Result = "Error";
			webImageList.Message = "NoSession";
			return webImageList;
		}
		catch (Exception ex)
		{
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "SetViewerImageCheckFile", ex.ToString());
			WebResult webResult3 = new WebResult();
			webResult3.Result = "Error";
			webResult3.Message = "Exception";
			return webResult3;
		}
	}

	public static WebResult RequestViewerImage(string orderno, string patientid, string studydate, string modality)
	{
		try
		{
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			if (HttpContext.Current.Session["usercd"] != null)
			{
				LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "ViewerImageRequest", "START [" + ((int)HttpContext.Current.Session["usercd"]).ToString() + "]");
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("オ\u30fcダ\u30fc番号[{0}] ", orderno);
				stringBuilder.AppendFormat("患者ID[{0}] ", patientid);
				stringBuilder.AppendFormat("検査日[{0}] ", studydate);
				stringBuilder.AppendFormat("モダリティ[{0}] ", modality);
				LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "ViewerImageRequest", stringBuilder.ToString());
				if (orderno.Length == 0)
				{
					LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "ViewerImageRequest", "オ\u30fcダ\u30fc番号が不明。");
					WebResult webResult = new WebResult();
					webResult.Result = "Error";
					webResult.Message = "オ\u30fcダ\u30fc番号";
					return webResult;
				}
				if (patientid.Length == 0)
				{
					LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "ViewerImageRequest", "患者IDが不明。");
					WebResult webResult2 = new WebResult();
					webResult2.Result = "Error";
					webResult2.Message = "患者ID";
					return webResult2;
				}
				if (studydate.Length == 0)
				{
					LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "ViewerImageRequest", "検査日が不明。");
					WebResult webResult3 = new WebResult();
					webResult3.Result = "Error";
					webResult3.Message = "検査日";
					return webResult3;
				}
				if (modality.Length == 0)
				{
					LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "ViewerImageRequest", "モダリティが不明。");
					WebResult webResult4 = new WebResult();
					webResult4.Result = "Error";
					webResult4.Message = "モダリティ";
					return webResult4;
				}
				if (!Directory.Exists(ConfigUtil._ViewerImageRequestPath))
				{
					LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "ViewerImageRequest", "VIEWERへの画像取得要求出力PATHが見つかりません。[" + ConfigUtil._ViewerImageRequestPath + "]");
					WebViewer webViewer = new WebViewer();
					webViewer.Result = "Error";
					webViewer.Message = "ProRadiRS Error";
					return webViewer;
				}
				string text = Path.ChangeExtension(orderno + "_" + DateTime.Now.ToString("yyyyMMddHHmmssff"), ConfigUtil._ViewerImageRequestDatExt);
				string path = Path.ChangeExtension(text, ConfigUtil._ViewerImageRequestTrgExt);
				if (ConfigUtil._ViewerImageRequestDatExt.Length > 0)
				{
					StringBuilder stringBuilder2 = new StringBuilder();
					stringBuilder2.AppendFormat("\"{0}\"", orderno);
					stringBuilder2.Append(",");
					stringBuilder2.AppendFormat("\"{0}\"", patientid);
					stringBuilder2.Append(",");
					stringBuilder2.AppendFormat("\"{0}\"", studydate);
					stringBuilder2.Append(",");
					stringBuilder2.AppendFormat("\"{0}\"", modality);
					string text2 = Path.Combine(ConfigUtil._ViewerImageRequestPath, text);
					using (StreamWriter streamWriter = new StreamWriter(text2, false, Encoding.GetEncoding("shift_jis")))
					{
						streamWriter.Write(stringBuilder2.ToString());
						streamWriter.Close();
					}
					LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "ViewerImageRequest", "出力ファイル [" + text2 + "]");
					if (ConfigUtil._ViewerImageRequestTrgExt.Length > 0)
					{
						string path2 = Path.Combine(ConfigUtil._ViewerImageRequestPath, path);
						using (new StreamWriter(path2, false, Encoding.GetEncoding("shift_jis")))
						{
						}
					}
				}
				WebResult webResult5 = new WebResult();
				webResult5.Result = "Success";
				return webResult5;
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "ViewerImageRequest", "セッションからのユ\u30fcザ\u30fcコ\u30fcドが取得出来ない。");
			WebViewer webViewer2 = new WebViewer();
			webViewer2.Result = "Error";
			webViewer2.Message = "NoSession";
			return webViewer2;
		}
		catch (Exception ex)
		{
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "ViewerImageRequest", ex.ToString());
			WebViewer webViewer3 = new WebViewer();
			webViewer3.Result = "Error";
			webViewer3.Message = "Exception";
			return webViewer3;
		}
	}

	public static WebUserList GetUserList()
	{
//		ProRadiRSServiceClient proRadiRSServiceClient = null;
		try
		{
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			if (HttpContext.Current.Session["usercd"] != null)
			{
				LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "GetUserList", "START [" + ((int)HttpContext.Current.Session["usercd"]).ToString() + "]");
//				proRadiRSServiceClient = new ProRadiRSServiceClient();
				UserMst2[] getUser = default(UserMst2[]);
                //				int userList = proRadiRSServiceClient.GetUserList(out getUser);
                //				proRadiRSServiceClient.Close();
                int userList = 0;

                ProRadiRSService2Client proRadServicesClient2 = null;
                proRadServicesClient2 = new ProRadiRSService2Client();

                getUser = proRadServicesClient2.GetUserList();

                if (userList == 0)
				{
					List<UserMst> list = new List<UserMst>();
					CommonWebServiceProc.getWebUserList(ref list, getUser);
					WebUserList webUserList = new WebUserList();
					webUserList.Result = "Success";
					webUserList.UserList = list.ToArray();
					return webUserList;
				}
				WebUserList webUserList2 = new WebUserList();
				webUserList2.Result = "Error";
				webUserList2.Message = "サ\u30fcビスでエラ\u30fc発生";
				return webUserList2;
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetUserList", "セッションからのユ\u30fcザ\u30fcコ\u30fcドが取得出来ない。");
			WebUserList webUserList3 = new WebUserList();
			webUserList3.Result = "Error";
			webUserList3.Message = "NoSession";
			return webUserList3;
		}
		catch (Exception ex)
		{
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetUserList", ex.ToString());
			WebUserList webUserList4 = new WebUserList();
			webUserList4.Result = "Error";
			webUserList4.Message = "Exception";
			return webUserList4;
		}
	}

	public static void GetExamOrder(int userCd)
	{
		try
		{
			if (HttpContext.Current.Session["examorderpath"] != null)
			{
				string text = (string)HttpContext.Current.Session["examorderpath"];
				if (text.Length > 0)
				{
					if (File.Exists(text))
					{
						byte[] array = null;
						using (FileStream fileStream = new FileStream(text, FileMode.Open, FileAccess.Read))
						{
							array = new byte[fileStream.Length];
							fileStream.Read(array, 0, array.Length);
						}
						HttpContext.Current.Response.ClearContent();
						HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.AddSeconds(3600.0));
						HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Public);
						HttpContext.Current.Response.Cache.SetValidUntilExpires(true);
						HttpContext.Current.Response.ContentType = "image/jpeg";
						HttpContext.Current.Response.BinaryWrite(array);
						HttpContext.Current.ApplicationInstance.CompleteRequest();
					}
					else
					{
						LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetExamOrder", "依頼票画像ファイルにアクセス不可 [" + text + "]");
					}
				}
				else
				{
					LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetExamOrder", "依頼票画像ファイルパス なし");
				}
			}
			else
			{
				LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetExamOrder", "セッションから依頼票画像パス取得出来ません。");
			}
		}
		catch (Exception ex)
		{
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetExamOrder", ex.ToString());
		}
	}

	public static WebResult ReadingCheck()
	{
		ProRadiRSServiceClient proRadiRSServiceClient = null;
		try
		{
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			if (HttpContext.Current.Session["userName"] != null)
			{
				string text = (string)HttpContext.Current.Session["userName"];
				if (HttpContext.Current.Session["ReadDepartment"] != null)
				{
					string text2 = (string)HttpContext.Current.Session["ReadDepartment"];
					if (HttpContext.Current.Session["serialno"] != null)
					{
						string s = (string)HttpContext.Current.Session["serialno"];
						int num = int.Parse(s);
						LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "ReadingCheck", "START [" + num + "," + text2 + "," + text + "]");
						if (num.ToString() == "")
						{
							LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "ReadingCheck", "読影ステ\u30fcタス確認を行うシリアル番号が不明です。");
							WebResult webResult = new WebResult();
							webResult.Result = "Error";
							webResult.Message = "ProRadiRS Error";
							return webResult;
						}
						proRadiRSServiceClient = new ProRadiRSServiceClient();
						ReportView reportView = default(ReportView);
						proRadiRSServiceClient.GetReportView(num, out reportView);
						proRadiRSServiceClient.Close();
						LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "ReadingCheck", "UserName [" + text + "] SerialNo [" + num + "] department [" + text2 + "] ReadPhysicianName [" + reportView.ReadPhysicianName + "]");
						bool flag = text2 != reportView.ReadDepartment;
						if (reportView.ReadPhysicianName != "" && text != reportView.ReadPhysicianName)
						{
							WebResult webResult2 = new WebResult();
							webResult2.Result = "Err";
							webResult2.Message = reportView.ReadPhysicianName + "先生が読影中です。上書きしますか？";
							return webResult2;
						}
						WebResult webResult3 = new WebResult();
						webResult3.Result = "Success";
						return webResult3;
					}
					LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "ReadingCheck", "セッションからserialNoが取得出来ません。");
					WebResult webResult4 = new WebResult();
					webResult4.Result = "Error";
					webResult4.Message = "NoSession";
					return webResult4;
				}
				LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "ReadingCheck", "セッションから診断科が取得出来ません。");
				WebResult webResult5 = new WebResult();
				webResult5.Result = "Error";
				webResult5.Message = "NoSession";
				return webResult5;
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "ReadingCheck", "セッションからユ\u30fcザ\u30fcネ\u30fcムが取得出来ません。");
			WebResult webResult6 = new WebResult();
			webResult6.Result = "Error";
			webResult6.Message = "NoSession";
			return webResult6;
		}
		catch (Exception ex)
		{
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "ReadingCheck", ex.ToString());
			WebResult webResult7 = new WebResult();
			webResult7.Result = "Error";
			webResult7.Message = "Exception";
			return webResult7;
		}
	}

	public static WebResult ReadingStart(int serialNo)
	{
		ProRadiRSServiceClient proRadiRSServiceClient = null;
		try
		{
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			if (HttpContext.Current.Session["userName"] != null)
			{
				string text = (string)HttpContext.Current.Session["userName"];
				if (HttpContext.Current.Session["ReadDepartment"] != null)
				{
					string text2 = (string)HttpContext.Current.Session["ReadDepartment"];
					LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "ReadingStart", "START [" + text2 + "," + text + "]");
					if (serialNo.ToString() == "")
					{
						LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "ReadingStart", "読影取消を行うシリアル番号が不明です。");
						WebResult webResult = new WebResult();
						webResult.Result = "Error";
						webResult.Message = "ProRadiRS Error";
						return webResult;
					}
					proRadiRSServiceClient = new ProRadiRSServiceClient();
					proRadiRSServiceClient.ReadingStart(serialNo, text2, text);
					proRadiRSServiceClient.Close();
					LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "ReadingStart", "UserName [" + text + "] SerialNo [" + serialNo + "] department [" + text2 + "]");
					WebResult webResult2 = new WebResult();
					webResult2.Result = "Success";
					return webResult2;
				}
				LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "ReadingStart", "セッションから診断科が取得出来ません。");
				WebResult webResult3 = new WebResult();
				webResult3.Result = "Error";
				webResult3.Message = "NoSession";
				return webResult3;
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "ReadingStart", "セッションからユ\u30fcザ\u30fcネ\u30fcムが取得出来ません。");
			WebResult webResult4 = new WebResult();
			webResult4.Result = "Error";
			webResult4.Message = "NoSession";
			return webResult4;
		}
		catch (Exception ex)
		{
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "ReadingStart", ex.ToString());
			WebResult webResult5 = new WebResult();
			webResult5.Result = "Error";
			webResult5.Message = "Exception";
			return webResult5;
		}
	}

	public static WebResult CancelReading(int serialNo)
	{
		ProRadiRSServiceClient proRadiRSServiceClient = null;
		try
		{
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			if (serialNo.ToString() == "")
			{
				LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "ReadingStatusCancel", "読影取消を行うシリアル番号が不明です。");
				WebResult webResult = new WebResult();
				webResult.Result = "Error";
				webResult.Message = "ProRadiRS Error";
				return webResult;
			}
			LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "ReadingStatusCancel", "START [" + serialNo.ToString() + "]");
			proRadiRSServiceClient = new ProRadiRSServiceClient();
			proRadiRSServiceClient.CancelReading(serialNo);
			proRadiRSServiceClient.Close();
			LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "ReadingStatusCancel", " SerialNo [" + serialNo + "]");
			WebResult webResult2 = new WebResult();
			webResult2.Result = "Success";
			return webResult2;
		}
		catch (Exception ex)
		{
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "ReadingStatusCancel", ex.ToString());
			WebResult webResult3 = new WebResult();
			webResult3.Result = "Error";
			webResult3.Message = "Exception";
			return webResult3;
		}
	}

	public static WebResult DeleteTemp(int serialNo)
	{
		try
		{
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			if (serialNo.ToString() == "")
			{
				LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "DeleteTemp", "消去する一時保存デ\u30fcタのシリアル番号が不明です。");
				WebResult webResult = new WebResult();
				webResult.Result = "Error";
				webResult.Message = "ProRadiRS Error";
				return webResult;
			}
			if (HttpContext.Current.Session["usercd"] != null)
			{
				int num = (int)HttpContext.Current.Session["usercd"];
				LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "DeleteTemp", "START [" + serialNo.ToString() + "]、[" + num.ToString() + "]");
				string clearPath = Path.Combine(ConfigUtil._TempSaveReportPath, num.ToString());
				FileUtil.TempSaveClear(clearPath, serialNo.ToString());
				LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "DeleteTemp", " SerialNo [" + serialNo + "]");
				WebResult webResult2 = new WebResult();
				webResult2.Result = "Success";
				return webResult2;
			}
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "DeleteTemp", "セッションからユ\u30fcザ\u30fcコ\u30fcドが取得出来ません。");
			WebResult webResult3 = new WebResult();
			webResult3.Result = "Error";
			webResult3.Message = "NoSession";
			return webResult3;
		}
		catch (Exception ex)
		{
			LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "DeleteTemp", ex.ToString());
			WebResult webResult4 = new WebResult();
			webResult4.Result = "Error";
			webResult4.Message = "Exception";
			return webResult4;
		}
	}

	private static void getWebReportList(ref List<ReportView> lstView, ref List<string> lstImageCheck, ReportView[] getView)
	{
		if (lstView == null)
		{
			lstView = new List<ReportView>();
		}
		if (lstImageCheck == null)
		{
			lstImageCheck = new List<string>();
		}
		foreach (ReportView reportView in getView)
		{
			lstView.Add(new ReportView
			{
				SerialNo = reportView.SerialNo,
				OrderNo = reportView.OrderNo,
				PatientID = reportView.PatientID,
				PatientSex = reportView.PatientSex,
				PatientBirthDate = reportView.PatientBirthDate,
				PatientAge = reportView.PatientAge,
				StudyDate = reportView.StudyDate,
				StudyTime = reportView.StudyTime,
				Modality = reportView.Modality,
				StudyBodyPart = reportView.StudyBodyPart,
				ReportStatus = reportView.ReportStatus,
				SubStatus = reportView.SubStatus,
				ReadingStatus = reportView.ReadingStatus,
				PriorityFlag = reportView.PriorityFlag,
				ReportReserve1 = reportView.ReportReserve1,
				Comment3 = reportView.Comment3,
				Comment2 = reportView.Comment2,
				ReadPhysicianName = reportView.ReadPhysicianName,
				Diagnosing = reportView.Diagnosing,
				Finding = "【画像所見】\r\n " + reportView.Finding + "\r\n\r\n【診断】\r\n" + reportView.Diagnosing,
				ReportReserve7 = reportView.ReportReserve7,
				ReportReserve8 = reportView.ReportReserve8,
				ReportReserve9 = reportView.ReportReserve9,
				RequestedPhysicianName = reportView.RequestedPhysicianName,
				AuthorizationPhysicianName = reportView.AuthorizationPhysicianName
			});
			if (FileUtil.ExistsImageCheck(reportView.OrderNo))
			{
				lstImageCheck.Add(reportView.OrderNo);
			}
		}
	}

	private static void getWebUserList(ref List<UserMst> lstUser, UserMst2[] getUser)
	{
		if (lstUser == null)
		{
			lstUser = new List<UserMst>();
		}
		foreach (UserMst2 userMst in getUser)
		{
			lstUser.Add(new UserMst
			{
				UserCD = userMst.UserCD,
				UserName = userMst.UserName
			});
		}
	}

    #region 元のソースをコメントアウト
    //public static WebViewer GetViewerUrl(int serialno, string orderno, string patientid, string studydate, string modality, int ViewReservflg)
    //{
    //	//ProRadServicesClient proRadServicesClient = null;
    //	try
    //	{
    //		HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
    //		string text = ConfigUtil._ViewerUrl;
    //		if (ViewReservflg == 1)
    //		{
    //			text = ConfigUtil._ViewerUrl_reserve;
    //		}
    //		if (text.Length == 0)
    //		{
    //			LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "GetViewerUrl", "Web.config[ViewerUrl]設定なし");
    //			WebViewer webViewer = new WebViewer();
    //			webViewer.Result = "Success";
    //			webViewer.ViewerURL = "";
    //			return webViewer;
    //		}
    //		if (HttpContext.Current.Session["usercd"] != null)
    //		{
    //			int num = (int)HttpContext.Current.Session["usercd"];
    //			LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "GetViewerUrl", "START [" + num.ToString() + "]");
    //			orderno = "";
    //			StringBuilder stringBuilder = new StringBuilder();
    //			stringBuilder.AppendFormat("Nadia SID[{0}] ", ConfigUtil._iProMedCode);
    //			stringBuilder.AppendFormat("ユ\u30fcザ\u30fcCD[{0}] ", num);
    //			stringBuilder.AppendFormat("シリアル番号[{0}] ", serialno);
    //			stringBuilder.AppendFormat("患者ID[{0}] ", patientid);
    //			stringBuilder.AppendFormat("検査日[{0}] ", studydate);
    //			stringBuilder.AppendFormat("モダリティ[{0}] ", modality);
    //			LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "GetViewerUrl", stringBuilder.ToString());
    //			if (serialno == 0)
    //			{
    //				LogUtil.Write(LogUtil.LogType.Warning, "CommonWebServiceProc", "GetViewerUrl", "シリアル番号が0。");
    //			}
    //			if (patientid.Length == 0)
    //			{
    //				LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetViewerUrl", "患者IDが不明。");
    //				WebViewer webViewer2 = new WebViewer();
    //				webViewer2.Result = "Error";
    //				webViewer2.Message = "患者ID";
    //				return webViewer2;
    //			}
    //			//RSKey rSKey = new RSKey();
    //			//rSKey.SerialNo = serialno;
    //			//rSKey.UserCD = num;
    //			//rSKey.OrderNo = orderno;
    //			//rSKey.PatientID = patientid;
    //			//rSKey.StudyDate = studydate;
    //			//rSKey.Modality = modality;
    //			//proRadServicesClient = ((ViewReservflg != 1) ? new ProRadServicesClient("NetTcpBinding_ProRadServices") : new ProRadServicesClient("NetTcpBinding_ProRadServices2"));
    //			string text2 = default(string);
    //			//proRadServicesClient.ToRSKeyString(out text2, ConfigUtil._iProMedCode, rSKey);
    //			if (text2.Length > 0)
    //			{
    //				if (!text.EndsWith("?"))
    //				{
    //					text += "?";
    //				}
    //				text = text + "rskey=" + text2;
    //				LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "GetViewerUrl", "ViewerURL [" + text + "]");
    //				WebViewer webViewer3 = new WebViewer();
    //				webViewer3.Result = "Success";
    //				webViewer3.ViewerURL = text.ToString();
    //				return webViewer3;
    //			}
    //			LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "GetViewerUrl", "RSKeyが取得出来ませんでした。Nadiaサ\u30fcビスのログを確認して下さい。");
    //			WebViewer webViewer4 = new WebViewer();
    //			webViewer4.Result = "Error";
    //			webViewer4.ViewerURL = "";
    //			return webViewer4;
    //		}
    //		LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetViewerUrl", "セッションからのユ\u30fcザ\u30fcコ\u30fcドが取得出来ない。");
    //		WebViewer webViewer5 = new WebViewer();
    //		webViewer5.Result = "Error";
    //		webViewer5.Message = "NoSession";
    //		return webViewer5;
    //	}
    //	catch (Exception ex)
    //	{
    //		//if (proRadServicesClient != null && proRadServicesClient.State == CommunicationState.Faulted)
    //		//{
    //		//	proRadServicesClient.Abort();
    //		//}
    //		LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetViewerUrl", ex.ToString());
    //		WebViewer webViewer6 = new WebViewer();
    //		webViewer6.Result = "Error";
    //		webViewer6.Message = "Exception";
    //		return webViewer6;
    //	}
    //}
    #endregion

    #region 2018/02/22 新規追加
    public static WebViewer GetViewerUrl(int serialno, string orderno, string patientid, string studydate, string modality, int ViewReservflg)
    {
        //ProRadServicesClient proRadServicesClient = null;
        try
        {
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            string text = ConfigUtil._ViewerUrl;
            if (ViewReservflg == 1)
            {
                text = ConfigUtil._ViewerUrl_reserve;
            }
            if (text.Length == 0)
            {
                LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "GetViewerUrl", "Web.config[ViewerUrl]設定なし");
                WebViewer webViewer = new WebViewer();
                webViewer.Result = "Success";
                webViewer.ViewerURL = "";
                return webViewer;
            }

            string urlkey = ConfigUtil._ViewerUrlKey;
            if (urlkey.Length == 0)
            {
                LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "GetViewerUrl", "Web.config[ViewerUrlKey]設定なし");
                WebViewer webViewer = new WebViewer();
                webViewer.Result = "Success";
                webViewer.ViewerURL = "";
                return webViewer;
            }

            if (HttpContext.Current.Session["usercd"] != null)
            {
                int num = (int)HttpContext.Current.Session["usercd"];
                LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "GetViewerUrl", "START [" + num.ToString() + "]");
                orderno = "";
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("Nadia SID[{0}] ", ConfigUtil._iProMedCode);
                stringBuilder.AppendFormat("ユ\u30fcザ\u30fcCD[{0}] ", num);
                stringBuilder.AppendFormat("シリアル番号[{0}] ", serialno);
                stringBuilder.AppendFormat("患者ID[{0}] ", patientid);
                stringBuilder.AppendFormat("検査日[{0}] ", studydate);
                stringBuilder.AppendFormat("モダリティ[{0}] ", modality);
                LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "GetViewerUrl", stringBuilder.ToString());
                if (serialno == 0)
                {
                    LogUtil.Write(LogUtil.LogType.Warning, "CommonWebServiceProc", "GetViewerUrl", "シリアル番号が0。");
                }
                if (patientid.Length == 0)
                {
                    LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetViewerUrl", "患者IDが不明。");
                    WebViewer webViewer2 = new WebViewer();
                    webViewer2.Result = "Error";
                    webViewer2.Message = "患者ID";
                    return webViewer2;
                }

                List<string> param = new List<string>();
                param.Add(serialno.ToString());
                param.Add(orderno);
                param.Add(patientid);
                param.Add(studydate);
                param.Add(modality);

                RSKey rSKey = new RSKey();
                rSKey.SerialNo = serialno;
                rSKey.UserCD = num;
                rSKey.OrderNo = orderno;
                rSKey.PatientID = patientid;
                rSKey.StudyDate = studydate;
                rSKey.Modality = modality;
                param.Add(ConvertUtil.Serialize(rSKey));


                string CallUrl = GetWebUrl(urlkey, text, param);
                if (CallUrl.Length > 0)
                {
                    LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "GetViewerUrl", "ViewerURL [" + CallUrl + "]");
                    WebViewer webViewer3 = new WebViewer();
                    webViewer3.Result = "Success";
                    webViewer3.ViewerURL = CallUrl.ToString();
                    return webViewer3;
                }
                LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "GetViewerUrl", "CallUrlが取得出来ませんでした。Nadiaサ\u30fcビスのログを確認して下さい。");
                WebViewer webViewer4 = new WebViewer();
                webViewer4.Result = "Error";
                webViewer4.ViewerURL = "";
                return webViewer4;
            }
            LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetViewerUrl", "セッションからのユ\u30fcザ\u30fcコ\u30fcドが取得出来ない。");
            WebViewer webViewer5 = new WebViewer();
            webViewer5.Result = "Error";
            webViewer5.Message = "NoSession";
            return webViewer5;
        }
        catch (Exception ex)
        {
            LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetViewerUrl", ex.ToString());
            WebViewer webViewer6 = new WebViewer();
            webViewer6.Result = "Error";
            webViewer6.Message = "Exception";
            return webViewer6;
        }
    }
    public static WebViewer GetViewerUrl2(int serialno, string orderno, string patientid, string studydate, string modality, int ViewReservflg)
    {
        //ProRadServicesClient proRadServicesClient = null;
        try
        {
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            string text = ConfigUtil._ViewerUrlVin;
            if (ViewReservflg == 1)
            {
                text = ConfigUtil._ViewerUrl_reserve;
            }
            if (text.Length == 0)
            {
                LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "GetViewerUrl", "Web.config[ViewerUrl]設定なし");
                WebViewer webViewer = new WebViewer();
                webViewer.Result = "Success";
                webViewer.ViewerURL = "";
                return webViewer;
            }

            string urlkey = ConfigUtil._ViewerUrlKey;
            if (urlkey.Length == 0)
            {
                LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "GetViewerUrl", "Web.config[ViewerUrlKey]設定なし");
                WebViewer webViewer = new WebViewer();
                webViewer.Result = "Success";
                webViewer.ViewerURL = "";
                return webViewer;
            }

            if (HttpContext.Current.Session["usercd"] != null)
            {
                int num = (int)HttpContext.Current.Session["usercd"];
                LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "GetViewerUrl", "START [" + num.ToString() + "]");
                orderno = "";
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("ユ\u30fcザ\u30fcCD[{0}] ", num);
                stringBuilder.AppendFormat("シリアル番号[{0}] ", serialno);
                stringBuilder.AppendFormat("患者ID[{0}] ", patientid);
                stringBuilder.AppendFormat("検査日[{0}] ", studydate);
                stringBuilder.AppendFormat("モダリティ[{0}] ", modality);
                LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "GetViewerUrl", stringBuilder.ToString());
                if (serialno == 0)
                {
                    LogUtil.Write(LogUtil.LogType.Warning, "CommonWebServiceProc", "GetViewerUrl", "シリアル番号が0。");
                }
                if (patientid.Length == 0)
                {
                    LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetViewerUrl", "患者IDが不明。");
                    WebViewer webViewer2 = new WebViewer();
                    webViewer2.Result = "Error";
                    webViewer2.Message = "患者ID";
                    return webViewer2;
                }

                List<string> param = new List<string>();
                param.Add(serialno.ToString());
                param.Add(orderno);
                param.Add(patientid);
                param.Add(studydate);
                param.Add(modality);
                param.Add("");
                param.Add(num.ToString());

                string CallUrl = GetWebUrl(urlkey, text, param);
                if (CallUrl.Length > 0)
                {
                    LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "GetViewerUrl", "ViewerURL [" + CallUrl + "]");
                    WebViewer webViewer3 = new WebViewer();
                    webViewer3.Result = "Success";
                    webViewer3.ViewerURL = CallUrl.ToString();
                    return webViewer3;
                }
                LogUtil.Write(LogUtil.LogType.Information, "CommonWebServiceProc", "GetViewerUrl", "CallUrlが取得出来ませんでした。Nadiaサ\u30fcビスのログを確認して下さい。");
                WebViewer webViewer4 = new WebViewer();
                webViewer4.Result = "Error";
                webViewer4.ViewerURL = "";
                return webViewer4;
            }
            LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetViewerUrl", "セッションからのユ\u30fcザ\u30fcコ\u30fcドが取得出来ない。");
            WebViewer webViewer5 = new WebViewer();
            webViewer5.Result = "Error";
            webViewer5.Message = "NoSession";
            return webViewer5;
        }
        catch (Exception ex)
        {
            LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetViewerUrl", ex.ToString());
            WebViewer webViewer6 = new WebViewer();
            webViewer6.Result = "Error";
            webViewer6.Message = "Exception";
            return webViewer6;
        }
    }

    // 自動ログイン
    public static int AutoLogin(string usercd)
    {
        ProRadiRSService2Client proRadServicesClient2 = null;
        int num = -1;
        try
        {
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "AutoLogin", usercd);

            proRadServicesClient2 = new ProRadiRSService2Client();

            string[][] userinfo = default(string[][]);
            string errmsg = "";

            num = -1;
            HttpContext.Current.Session.Clear();
            if (proRadServicesClient2.AutoLoginCheck(usercd, out userinfo, out errmsg))
            {
                num = 0;
                HttpContext.Current.Session["loginid"] = userinfo[0][0];
                HttpContext.Current.Session["usercd"] = int.Parse(userinfo[0][1]);
                HttpContext.Current.Session["username"] = userinfo[0][2];
                HttpContext.Current.Session["groupCd"] = int.Parse(userinfo[0][3]);
            }
            proRadServicesClient2.Close();

            LogUtil.Write(LogUtil.LogType.Debug, "CommonWebServiceProc", "AutoLogin", "サ\u30fcビス戻り値 [" + num + "]");
            return num;
        }
        catch (Exception ex)
        {
            LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "AutoLogin", ex.ToString());
            num = -1;
            if (proRadServicesClient2 != null)
            {
                if (proRadServicesClient2.State == CommunicationState.Faulted)
                {
                    proRadServicesClient2.Abort();
                    return num;
                }
                return num;
            }
            return num;
        }
    }

    private static string GetWebUrl(string key, string webUrl, List<string> param)
    {
        StringBuilder sbUrl = new StringBuilder();

        string strKey = "";
        string strUrl = webUrl;
        string strUrlCut = "";

        int intCnt = 0;
        while (strUrl.Length > 0)
        {
            intCnt = strUrl.LastIndexOf(key, strUrl.Length);
            if (intCnt > 0)
            {
                strUrlCut = strUrl.Substring(intCnt, strUrl.Length - intCnt);   // Keyが存在する位置より後の文字列
                strUrl = strUrl.Substring(0, intCnt);                           // Keyが存在する位置より前の文字列

                for (int i = param.Count - 1; i > -1; i--)
                {
                    strKey = key + i.ToString();
                    if (strUrlCut.Contains(strKey))
                    {
                        strUrlCut = strUrlCut.Replace(strKey, param[i]);
                        break;
                    }
                }
            }
            else
            {
                strUrlCut = strUrl;
                strUrl = "";
            }
            sbUrl.Insert(0, strUrlCut);
        }

        return sbUrl.ToString();

    }

    public static WebSentenceList GetSentenceData(int usercd, int groupcd)
    {
        ProRadiRSService2Client proRadServicesClient2 = null;
        try
        {
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            proRadServicesClient2 = new ProRadiRSService2Client();

            string[][] SentenceList = default(string[][]);
            string errmsg = "";
            proRadServicesClient2.GetSentenceData(usercd, groupcd, out SentenceList, out errmsg);

            WebSentenceList webSentenceList1 = new WebSentenceList();
            webSentenceList1.List = SentenceList;
            webSentenceList1.Result = "Success";
            webSentenceList1.Message = errmsg;  // 一応セットする
            return webSentenceList1;
        }
        catch (Exception ex)
        {
            LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetSentenceData", ex.ToString());
            WebSentenceList webSentenceList4 = new WebSentenceList();
            webSentenceList4.Result = "Error";
            webSentenceList4.Message = "Exception";
            return webSentenceList4;
        }

    }

    public static bool SetSentenceData(string[] vals)
    {
        ProRadiRSService2Client proRadServicesClient2 = null;
        var ret = false;
        try
        {
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            proRadServicesClient2 = new ProRadiRSService2Client();
            ret = proRadServicesClient2.UpdateSentence(vals[0], vals[1], vals[2], vals[3], vals[4], vals[5]);
        }
        catch (Exception ex)
        {
            LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetSentenceData", ex.ToString());
            WebSentenceList webSentenceList4 = new WebSentenceList();
        }

        return ret;
    }

    public static bool DelSentenceData(string cd)
    {
        ProRadiRSService2Client proRadServicesClient2 = null;
        var ret = false;
        try
        {
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            proRadServicesClient2 = new ProRadiRSService2Client();
            ret = proRadServicesClient2.DeleteSentence(cd);
        }
        catch (Exception ex)
        {
            LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetSentenceData", ex.ToString());
            WebSentenceList webSentenceList4 = new WebSentenceList();
        }

        return ret;
    }

    public static WebHistoryList GetChangeHistory(int serialno)
    {
        ProRadiRSService2Client proRadServicesClient2 = null;
        try
        {
            if (HttpContext.Current.Session["usercd"] != null)
            {
                int num = (int)HttpContext.Current.Session["usercd"];

                string[][] HistoryList = default(string[][]);
                string[][] ImageList = default(string[][]);

                string errmsg = "";
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

                proRadServicesClient2 = new ProRadiRSService2Client();
                proRadServicesClient2.GetChangeHistory(num, serialno, ConfigUtil._HistoryImageViewPath, out HistoryList, out ImageList, out errmsg);

                WebHistoryList webHistoryList1 = new WebHistoryList();
                webHistoryList1.HistList = HistoryList;
                webHistoryList1.ImageList = ImageList;
                webHistoryList1.Result = "Success";
                return webHistoryList1;
            }
            LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetViewerUrl", "セッションからのユ\u30fcザ\u30fcコ\u30fcドが取得出来ない。");
            WebHistoryList webHistoryList2 = new WebHistoryList();
            webHistoryList2.Result = "Error";
            webHistoryList2.Message = "NoSession";
            return webHistoryList2;
        }
        catch (Exception ex)
        {
            LogUtil.Write(LogUtil.LogType.Error, "CommonWebServiceProc", "GetSentenceData", ex.ToString());
            WebHistoryList webHistoryList4 = new WebHistoryList();
            webHistoryList4.Result = "Error";
            webHistoryList4.Message = "Exception";
            return webHistoryList4;
        }
    }
    #endregion


    public class WebResult
    {
        public string Result
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }
    }
    public class WebUserList : WebResult
    {
        public UserMst[] UserList
        {
            get;
            set;
        }
    }

    public class WebViewer : WebResult
    {
        public string ViewerURL
        {
            get;
            set;
        }
    }


    public class WebImageList : WebResult
    {
        public string[] List
        {
            get;
            set;
        }
    }

    public class WebReportList : WebResult
    {
        public int Count
        {
            get;
            set;
        }

        public ReportView[] List
        {
            get;
            set;
        }

        public string[] TempSaveList
        {
            get;
            set;
        }

        public string[] ImageCheckList
        {
            get;
            set;
        }
    }

    public class WebReport : WebResult
    {
        public ReportView View
        {
            get;
            set;
        }

        public string[] Image
        {
            get;
            set;
        }

        public ReportHistory[] History
        {
            get;
            set;
        }

        public string ExamOrder
        {
            get;
            set;
        }

        public string editFlg
        {
            get;
            set;
        }
    }
    public class WebParams : WebResult
    {
        public Dictionary<string, string> Params
        {
            get;
            set;
        }
    }

    public class WebHistoryReport : WebResult
    {
        public ReportView View
        {
            get;
            set;
        }

        public string[] Image
        {
            get;
            set;
        }
    }

    #region 2018/02/22 新規追加
    public class WebSentenceList : WebResult
    {
        public string[][] List
        {
            get;
            set;
        }

    }

    public class WebHistoryList : WebResult
    {
        public string[][] HistList
        {
            get;
            set;
        }

        public string[][] ImageList
        {
            get;
            set;
        }
    }
    #endregion


}
