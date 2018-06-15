using DBLib;
using LogController;
using MyAccDB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Xml.Serialization;

namespace ProRadiRSService2
{
    //[ServiceContract(Namespace ="")]
    //[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    // メモ: [リファクター] メニューの [名前の変更] コマンドを使用すると、コード、svc、および config ファイルで同時にクラス名 "Service1" を変更できます。
    // 注意: このサービスをテストするために WCF テスト クライアントを起動するには、ソリューション エクスプローラーで Service1.svc または Service1.svc.cs を選択し、デバッグを開始してください。
    public class ProRadiRSService2 : IProRadiRSService2
    {
        ConnectionStringSettings DBRemote = ConfigurationManager.ConnectionStrings["DBRemote"];

        public bool AutoLoginCheck(string UserCD, out List<string[]> userinfo, out string retErrMsg)
        {
            bool bRet = false;
            userinfo = new List<string[]>();
            retErrMsg = ""; //エラーメッセージ用

            try
            {
                using (DBAcc clsDB = new DBAcc(DBRemote))
                {
                    // SQL
                    StringBuilder sql = new StringBuilder("SELECT mUser.* ");
                    sql.Append(" FROM UserMst AS mUser ");
                    sql.Append(" WHERE mUser.UserCD=@0");

                    DbParamClass sqlDbParam = new DbParamClass();
                    sqlDbParam.Add(int.Parse(UserCD));
                    DbParameter[] dbprms = sqlDbParam.GetDbParameter(clsDB);

                    //string str = sql.ToString();

                    DbDataReader dr;
                    clsDB.SqlQuery(sql.ToString(), dbprms, out dr);
                    while (dr.Read())
                    {
                        List<string> user = new List<string>();
                        user.Add((string)dr["LoginID"]);
                        user.Add(((int)dr["UserCD"]).ToString());
                        user.Add((string)dr["UserName"]);
                        user.Add(((int)dr["GroupCD"]).ToString());

                        userinfo.Add(user.ToArray());
                        bRet = true;
                    }
                    dr.Close();
                }

            }
            catch (Exception ex)
            {
                retErrMsg = ex.Message;
            }

            return bRet;
        }

        //検索
        public bool GetSentenceData(int userCD, int userGroupCD, out List<string[]> SentenceList, out string retErrMsg)
        {
            bool bRet = false;
            retErrMsg = ""; //エラーメッセージ用
            SentenceList = new List<string[]>();

            try
            {
                using (DBAcc clsDB = new DBAcc(DBRemote))
                {
                    // SQL
                    StringBuilder sql = new StringBuilder("SELECT senM.*, userM.UserName  ");
                    sql.Append(" FROM SentenceMst senM, UserMst userM ");
                    sql.Append(" WHERE senM.UserCD = @0 ");
                    //selSql.Append(" WHERE (( PublicCD = 0 AND senM.UserCD = @0 ) ");
                    //selSql.Append(" OR ( PublicCD = @1 ) OR ( PublicCD = -1 )) ");
                    sql.Append(" AND userM.UserCD = senM.UserCD ");
                    sql.Append(" ORDER BY SentenceName ");

                    DbParamClass sqlDbParam = new DbParamClass();
                    sqlDbParam.Add(userCD);
                    //sqlDbParam.Add(userGroupCD);  // userGroupCDは使用しない
                    DbParameter[] dbprms = sqlDbParam.GetDbParameter(clsDB);

                    //string str = sql.ToString();

                    DbDataReader dr;
                    clsDB.SqlQuery(sql.ToString(), dbprms, out dr);
                    while (dr.Read())
                    {
                        List<string> Sentence = new List<string>();
                        Sentence.Add(((int)dr["SentenceCD"]).ToString());
                        Sentence.Add((string)dr["SentenceValue1"]);
                        Sentence.Add((string)dr["SentenceValue2"]);
                        Sentence.Add(((int)dr["UserCD"]).ToString());
                        Sentence.Add((string)dr["SentenceName"]);
                        Sentence.Add((string)dr["UserName"]);
                        Sentence.Add(((int)dr["PublicCD"]).ToString());
                        SentenceList.Add(Sentence.ToArray());
                    }
                    dr.Close();

                    bRet = true;
                }

            }
            catch (Exception ex)
            {
                retErrMsg = ex.Message;
            }

            return bRet;
        }

        //変更履歴の取得
        public bool GetChangeHistory(int userCD, int SerialNo, string imagePath, out List<string[]> HistoryDataList, out List<string[]> ImageDataList, out string retErrMsg)
        {
            bool bRet = false;
            retErrMsg = "";
            HistoryDataList = new List<string[]>(); // 変更履歴リスト
            ImageDataList = new List<string[]>();   // イメージデータリスト

            try
            {
                using (DBAcc clsDB = new DBAcc(DBRemote))
                {
                    // パラメータのセット
                    DbParamClass sqlDbParam = new DbParamClass();
                    sqlDbParam.Add(SerialNo);
                    DbParameter[] dbprms = sqlDbParam.GetDbParameter(clsDB);

                    //読影履歴テーブルからデータ取得
                    StringBuilder sql = new StringBuilder("SELECT * FROM HistoryTbl  ");
                    sql.Append(" WHERE SerialNo = @0 and ");
                    sql.Append(" ( ReportStatus = 2 ");
                    sql.Append("   or ");
                    sql.Append("   historyno in (select max(historyno) as historyno from historytbl where serialno = @0 and ReportStatus = 3 group by edition )");
                    sql.Append(" )");
                    sql.Append(" ORDER BY HistoryNo DESC");

                    //string str = sql.ToString();

                    DbDataReader dr;
                    clsDB.SqlQuery(sql.ToString(), dbprms, out dr);

                    //履歴テーブルにデータが存在する場合、データ取得
                    while (dr.Read())
                    {
                        List<string> HistoryArray = new List<string>();

                        RsHistoryTbl rsHistoryTbl = new RsHistoryTbl(); // 読影テーブル
                        rsHistoryTbl.SerialNo = (int)dr["SerialNo"];    // シリアル番号
                        rsHistoryTbl.HistoryNo = (int)dr["HistoryNo"];  // 読影番号
                        rsHistoryTbl.ReadDate = (string)dr["ReadDate"]; // 読影日
                        rsHistoryTbl.ReadTime = (string)dr["ReadTime"]; // 読影時刻
                        rsHistoryTbl.ReadPhysicianName = (string)dr["ReadPhysicianName"];   // 読影医
                        rsHistoryTbl.Finding = (string)dr["Finding"];       // 画像所見
                        rsHistoryTbl.Diagnosing = (string)dr["Diagnosing"]; // 結論・診断

                        HistoryArray.Add(rsHistoryTbl.SerialNo.ToString());
                        HistoryArray.Add(rsHistoryTbl.HistoryNo.ToString());
                        HistoryArray.Add(rsHistoryTbl.ReadDate);
                        HistoryArray.Add(rsHistoryTbl.ReadTime);
                        HistoryArray.Add(rsHistoryTbl.ReadPhysicianName);
                        HistoryArray.Add(rsHistoryTbl.Finding);
                        HistoryArray.Add(rsHistoryTbl.Diagnosing);
                        HistoryArray.Add("");

                        HistoryDataList.Add(HistoryArray.ToArray());
                    }
                    dr.Close();


                    int nIdx = 0;
                    // 読影データ分繰り返す
                    foreach (string[] HistoryData in HistoryDataList)
                    {
                        List<ReportImage> lstImage = new List<ReportImage>();

                        int HistoryNo = int.Parse(HistoryData[1]);// 読影番号の取得

                        List<string> ImageArray = new List<string>();
                        ImageArray.Add(HistoryNo.ToString());

                        // パラメータのセット
                        sqlDbParam = new DbParamClass();
                        sqlDbParam.Add(SerialNo);
                        sqlDbParam.Add(HistoryNo);
                        dbprms = sqlDbParam.GetDbParameter(clsDB);

                        // 画像テーブルからデータ取得
                        sql = new StringBuilder("SELECT * FROM ImageTbl WHERE SerialNo = @0 and HistoryNo = @1");
                        clsDB.SqlQuery(sql.ToString(), dbprms, out dr);

                        List<RsImageTbl> rsImgList = new List<RsImageTbl>();
                        // 画像テーブルにデータが存在する場合、データ取得
                        while (dr.Read())
                        {
                            ReportImage rImage = new ReportImage();

                            rImage.ImageData = (byte[])dr["ImageData"];
                            rImage.ImageExt = (string)dr["ImageExt"];

                            lstImage.Add(rImage);
/*
                            RsImageTbl rsImageTbl = new RsImageTbl();
                            rsImageTbl.SeqNo = (int)dr["SeqNo"];    // SEQ番号
                            //rsImageTbl.ImageData = (byte[])dr["ImageData"]; // 画像データ
                            rsImageTbl.ImageExt = (string)dr["ImageExt"];   // 画像拡張子
                            rsImageTbl.ImageName = (string)dr["ImageName"];
                            rsImgList.Add(rsImageTbl);

                            List<string> ImageArray = new List<string>();
                            ImageArray.Add(SerialNo.ToString());
                            ImageArray.Add(rsImageTbl.SeqNo.ToString());
                            ImageArray.Add(rsImageTbl.ImageName);
                            ImageArray.Add(rsImageTbl.ImageExt);

                            ImageDataList.Add(ImageArray.ToArray());
*/
                        }
                        // 読影データ/画像テーブルへセット
                        // rsReadingData.ImageTblList = rsImgList;

                        // 画像配列へセット

                        dr.Close();

                        if (lstImage.Count > 0)
                        {
                            List<string> lstImageFileName = new List<string>();
                            outputReportImage(userCD, SerialNo, imagePath, lstImage, ref lstImageFileName);

                            foreach (string ImgName in lstImageFileName)
                            {
                                ImageArray.Add(ImgName);
                            }
                        }
                        ImageDataList.Add(ImageArray.ToArray());
                    }

                    bRet = true;
                }
            }
            catch (Exception ex)
            {
                retErrMsg = ex.Message;
            }
            return bRet;
        }

        public bool SaveReport(int userCd, int serialNo, string finding, string diagnosing, int imgCnt)
        {
            bool ret = false;
            try
            {
                string userName = "";
                int history = 0;
                int status = 0;

                using (var con = new AccDbConnection(DBRemote))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        // SQL
                        StringBuilder sql = new StringBuilder();
                        sql.Append("SELECT ");
                        sql.Append(" UserName");
                        sql.Append(" FROM");
                        sql.Append(" UserMst");
                        sql.Append(" WHERE");
                        sql.Append(" UserCD = " + cmd.Add(userCd).ParameterName);

                        cmd.CommandText = sql.ToString();

                        using (var dr = cmd.ExecuteReader())
                            if (dr.Read())
                                userName = dr["UserName"].ToString();
                    }

                    using (var cmd = con.CreateCommand())
                    {
                        // SQL
                        StringBuilder sql = new StringBuilder();
                        sql.Append("SELECT ");
                        sql.Append(" HistoryNo");
                        sql.Append(",ReadingStatus");
                        sql.Append(" FROM");
                        sql.Append(" ReportTbl");
                        sql.Append(" WHERE");
                        sql.Append(" SerialNo = " + cmd.Add(serialNo).ParameterName);

                        cmd.CommandText = sql.ToString();

                        using (var dr = cmd.ExecuteReader())
                            if (dr.Read())
                            {
                                history = Convert.ToInt32(dr["HistoryNo"]);
                                status = Convert.ToInt32(dr["ReadingStatus"]);
                            }
                    }

                    if (status == 0)
                    {
                        // HistoryNo をインクリメント
                        history++;
                        status = 1;
                    }

                    using (var cmd = con.CreateCommand())
                    {
                        // SQL
                        StringBuilder sql = new StringBuilder();
                        sql.Append("UPDATE ");
                        sql.Append(" ReportTbl");
                        sql.Append(" SET");
                        sql.Append(" ReadingStatus = " + cmd.Add(status).ParameterName);
                        sql.Append(",ReadPhysicianName = " + cmd.Add(userName).ParameterName);
                        sql.Append(",HistoryNo = " + cmd.Add(history).ParameterName);
                        sql.Append(",Finding = " + cmd.Add(finding).ParameterName);
                        sql.Append(",Diagnosing = " + cmd.Add(diagnosing).ParameterName);
                        sql.Append(",ImageUmu = " + cmd.Add(imgCnt).ParameterName);
                        sql.Append(" WHERE");
                        sql.Append(" SerialNo = " + cmd.Add(serialNo).ParameterName);

                        cmd.CommandText = sql.ToString();

                        var retDB = cmd.ExecuteNonQuery();

                        if (retDB > -1)
                            ret = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "S:saveReport", ex.Message);
                LogControl.WriteLog(LogType.ERR, "S:saveReport", ex.StackTrace);
            }

            return ret;
        }

        public bool SaveReportImage(string serialNo, string[] images)
        {
            bool ret = false;

            try
            {
                int history = 0;

                using (var con = new AccDbConnection(DBRemote))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        // SQL
                        StringBuilder sql = new StringBuilder();
                        sql.Append("SELECT ");
                        sql.Append(" HistoryNo");
                        sql.Append(" FROM");
                        sql.Append(" ReportTbl");
                        sql.Append(" WHERE");
                        sql.Append(" SerialNo = " + cmd.Add(serialNo).ParameterName);

                        cmd.CommandText = sql.ToString();

                        using (var dr = cmd.ExecuteReader())
                            if (dr.Read())
                                history = Convert.ToInt32(dr["HistoryNo"]);
                    }

                    using (var cmd = con.CreateCommand())
                    {
                        // SQL
                        StringBuilder sql = new StringBuilder();
                        sql.Append("DELETE ");
                        sql.Append(" FROM");
                        sql.Append(" ImageTbl");
                        sql.Append(" WHERE");
                        sql.Append(" SerialNo = " + cmd.Add(serialNo).ParameterName);
                        sql.Append(" AND");
                        sql.Append(" HistoryNo = " + cmd.Add(history).ParameterName);

                        cmd.CommandText = sql.ToString();

                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = con.CreateCommand())
                    {
                        // SQL
                        StringBuilder sql = new StringBuilder();
                        sql.Append("INSERT ");
                        sql.Append(" INTO");
                        sql.Append(" ImageTbl");
                        sql.Append(" (");
                        sql.Append(" SerialNo");
                        sql.Append(",HistoryNo");
                        sql.Append(",SeqNo");
                        sql.Append(",ImageData");
                        sql.Append(",ImageExt");
                        sql.Append(" )");
                        sql.Append(" VALUES");

                        for (int i = 0; i < images.Length; i++)
                        {
                            if (i != 0)
                                sql.Append(",");

                            byte[] tmpByte;

                            using (Image img = Image.FromFile(images[i]))
                            {
                                //LogControl.WriteLog(LogType.ORE, "S:saveReportImage", images[i]);
                                ImageConverter imgconv = new ImageConverter();
                                tmpByte = (byte[])imgconv.ConvertTo(img, typeof(byte[]));
                            }
                            sql.Append(" (");
                            sql.Append(cmd.Add(serialNo).ParameterName);
                            sql.Append(",");
                            sql.Append(cmd.Add(history).ParameterName);
                            sql.Append(",");
                            sql.Append(cmd.Add(i).ParameterName);
                            sql.Append(",");
                            //LogControl.WriteLog(LogType.ORE, "S:saveReportImage", Convert.ToBase64String(tmpByte));
                            sql.Append(cmd.Add(tmpByte).ParameterName);
                            sql.Append(",");
                            sql.Append(cmd.Add("jpg").ParameterName);
                            sql.Append(" )");
                        }

                        cmd.CommandText = sql.ToString();

                        var retDB = cmd.ExecuteNonQuery();

                        if (retDB > -1)
                            ret = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "S:saveReportImage", ex.Message);
                LogControl.WriteLog(LogType.ERR, "S:saveReportImage", ex.StackTrace);
            }

            return ret;
        }

        public bool UpdateSentence(string scd, string name, string val1, string val2, string pcd, string usercd)
        {
            bool ret = false;

            try
            {
                using (var con = new AccDbConnection(DBRemote))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        if (string.IsNullOrEmpty(scd))
                        {
                            sql.Append("INSERT ");
                            sql.Append(" INTO");
                            sql.Append(" SentenceMst");
                            sql.Append(" (");
                            sql.Append(" SentenceName");
                            sql.Append(",SentenceValue1");
                            sql.Append(",SentenceValue2");
                            sql.Append(",PublicCD");
                            sql.Append(",UserCD");
                            sql.Append(" )");
                            sql.Append(" VALUES");

                            sql.Append(" (");
                            sql.Append(cmd.Add(name).ParameterName);
                            sql.Append(",");
                            sql.Append(cmd.Add(val1).ParameterName);
                            sql.Append(",");
                            sql.Append(cmd.Add(val2).ParameterName);
                            sql.Append(",");
                            sql.Append(cmd.Add(pcd).ParameterName);
                            sql.Append(",");
                            sql.Append(cmd.Add(usercd).ParameterName);
                            sql.Append(" )");
                        }
                        else
                        {
                            sql.Append("UPDATE ");
                            sql.Append(" SentenceMst");
                            sql.Append(" SET");
                            sql.Append(" SentenceName = " + cmd.Add(name).ParameterName);
                            sql.Append(",SentenceValue1 = " + cmd.Add(val1).ParameterName);
                            sql.Append(",SentenceValue2 = " + cmd.Add(val2).ParameterName);
                            sql.Append(",PublicCD = " + cmd.Add(pcd).ParameterName);
                            sql.Append(",UserCD = " + cmd.Add(usercd).ParameterName);
                            sql.Append(" WHERE");
                            sql.Append(" SentenceCD = " + cmd.Add(scd).ParameterName);
                        }

                        cmd.CommandText = sql.ToString();

                        var retDB = cmd.ExecuteNonQuery();

                        if (retDB > -1)
                            ret = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "S:saveReportImage", ex.Message);
                LogControl.WriteLog(LogType.ERR, "S:saveReportImage", ex.StackTrace);
            }

            return ret;
        }

        public bool DeleteSentence(string scd)
        {
            bool ret = false;

            try
            {
                using (var con = new AccDbConnection(DBRemote))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.Append("DELETE ");
                        sql.Append(" FROM");
                        sql.Append(" SentenceMst");
                        sql.Append(" WHERE");
                        sql.Append(" SentenceCD = " + cmd.Add(scd).ParameterName);

                        cmd.CommandText = sql.ToString();

                        var retDB = cmd.ExecuteNonQuery();

                        if (retDB > -1)
                            ret = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "S:saveReportImage", ex.Message);
                LogControl.WriteLog(LogType.ERR, "S:saveReportImage", ex.StackTrace);
            }

            return ret;
        }

        public string[] GetImageExt(int userCd, string serialNo, string imagePath)
        {
            List<string> ret = new List<string>();

            try
            {
                int history = 0;
                List<ReportImage> lstImage = new List<ReportImage>();

                using (var con = new AccDbConnection(DBRemote))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        // SQL
                        StringBuilder sql = new StringBuilder();
                        sql.Append("SELECT ");
                        sql.Append(" HistoryNo");
                        sql.Append(" FROM");
                        sql.Append(" ReportTbl");
                        sql.Append(" WHERE");
                        sql.Append(" SerialNo = " + cmd.Add(serialNo).ParameterName);

                        cmd.CommandText = sql.ToString();

                        using (var dr = cmd.ExecuteReader())
                            if (dr.Read())
                                history = Convert.ToInt32(dr["HistoryNo"]);
                    }

                    using (var cmd = con.CreateCommand())
                    {
                        // SQL
                        StringBuilder sql = new StringBuilder();
                        sql.Append(" HistoryNo");
                        sql.Append(" FROM");
                        sql.Append(" ReportTbl");
                        sql.Append(" WHERE");
                        sql.Append(" SerialNo = " + cmd.Add(serialNo).ParameterName);

                        cmd.CommandText = sql.ToString();

                    }


                    using (var cmd = con.CreateCommand())
                    {
                        // SQL
                        StringBuilder sql = new StringBuilder();
                        sql.Append("SELECT ");
                        sql.Append(" ImageData");
                        sql.Append(",ImageExt");
                        sql.Append(" FROM");
                        sql.Append(" ImageTbl");
                        sql.Append(" WHERE");
                        sql.Append(" SerialNo = " + cmd.Add(serialNo).ParameterName);
                        sql.Append(" AND");
                        sql.Append(" HistoryNo = " + cmd.Add(history).ParameterName);
                        sql.Append(" ORDER BY SeqNo ASC");

                        cmd.CommandText = sql.ToString();

                        using (var dr = cmd.ExecuteReader())
                            while (dr.Read())
                            {
                                ReportImage tmpImage = new ReportImage();
                                tmpImage.ImageData = (byte[])dr["ImageData"];
                                tmpImage.ImageExt = (string)dr["ImageExt"];

                                lstImage.Add(tmpImage);
                            }

                    }
                }

                outputReportImage(userCd, Convert.ToInt32(serialNo), imagePath, lstImage, ref ret);
            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "S:GetImageExt", ex.Message);
                LogControl.WriteLog(LogType.ERR, "S:GetImageExt", ex.StackTrace);
            }

            return ret.ToArray();
        }

        public UserMst2[] GetUserList()
        {
            List<UserMst2> ret = new List<UserMst2>();

            try
            {
                using (var con = new AccDbConnection(DBRemote))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        // SQL
                        StringBuilder sql = new StringBuilder();
                        sql.Append("SELECT ");
                        sql.Append(" UserCD");
                        sql.Append(",UserName");
                        sql.Append(" FROM");
                        sql.Append(" UserMst");
                        sql.Append(" WHERE");
                        sql.Append(" ViewType=1");
                        sql.Append(" AND");
                        sql.Append(" GroupCD NOT IN (999,1000)");
                        sql.Append(" ORDER BY SeqNo");

                        cmd.CommandText = sql.ToString();

                        using (var dr = cmd.ExecuteReader())
                            while (dr.Read())
                            {
                                UserMst2 mst = new UserMst2();
                                mst.UserCD = Convert.ToInt32(dr["UserCD"]);
                                mst.UserName = dr["UserName"].ToString();

                                ret.Add(mst);
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                LogControl.WriteLog(LogType.ERR, "S:GetUserList", ex.Message);
                LogControl.WriteLog(LogType.ERR, "S:GetUserList", ex.StackTrace);
            }

            return ret.ToArray();
        }

        public static void outputReportImage(int userCd, int serialNo, string imagePath, List<ReportImage> lstImage, ref List<string> lstImageFileName)
        {
            if (lstImageFileName == null)
            {
                lstImageFileName = new List<string>();
            }
            if (!Directory.Exists(imagePath))
            {
                LogControl.WriteLog(LogType.ERR, "S:outputReportImage", "画像保管ディレクトリが見つかりません。:" + imagePath);
                return;
            }
            string outputPath = Path.Combine(imagePath, userCd.ToString());
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            int seqNo = 1;
            DateTime dt = DateTime.Now;
            foreach (ReportImage img in lstImage)
            {
                string file = Path.ChangeExtension(string.Concat(new string[]
                {
                    serialNo.ToString(),
                    "_",
                    seqNo.ToString(),
                    "_",
                    dt.ToString("yyMMddHHmmssfff")
                }), img.ImageExt);

                string output = Path.Combine(outputPath, file);
                using (FileStream fs = new FileStream(output, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(img.ImageData, 0, img.ImageData.Length);
                }
                lstImageFileName.Add(Path.ChangeExtension(file, null));
                seqNo++;
            }
        }

    }
}
