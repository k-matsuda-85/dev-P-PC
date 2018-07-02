using LogController;
using OrderTool_Serv.Class;
using System;

namespace OrderTool_Serv
{
    public class ServiceIF
    {
        /// <summary>
        /// セッションキー取得
        /// </summary>
        /// <param name="id">ユーザID</param>
        /// <param name="logMst">o:ログイン情報クラス</param>
        /// <returns>処理結果クラス</returns>
        public ResultData GetSessionKey_Serv(int id, out Login logMst)
        {
            var ret = new ResultData();
            logMst = new Login();

            try
            {
                // ログインメソッド呼び出し（内部関数）
                logMst = User.GetSessionKey(id);

                if (string.IsNullOrEmpty(logMst.Key))
                    ret.Message = "IDまたはパスワードが異なります。";
                else
                    ret.Result = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "Login", e.Message);
                ret.Message = "IDまたはパスワードが異なります。";
            }

            return ret;
        }


        /// <summary>
        /// ログイン
        /// </summary>
        /// <param name="id">ログインID</param>
        /// <param name="pass">パスワード</param>
        /// <param name="logMst">o:ログイン管理クラス</param>
        /// <returns>処理結果クラス</returns>
        public ResultData Login_Serv(string id, string pass, out Login logMst)
        {
            var ret = new ResultData();
            logMst = new Login();

            try
            {
                // ログインメソッド呼び出し（内部関数）
                logMst = User.LoginAcc(id, pass);

                if (string.IsNullOrEmpty(logMst.Key))
                    ret.Message = "ID、パスワードをご確認ください。";
                else
                    ret.Result = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "Login", e.Message);
                ret.Message = e.Message;
            }

            return ret;
        }

        /// <summary>
        /// ユーザ情報取得
        /// </summary>
        /// <param name="id">ユーザID</param>
        /// <param name="usrMst">o:ユーザ情報クラス</param>
        /// <returns>処理結果クラス</returns>
        public ResultData GetUser_Serv(int id, out UserMst usrMst)
        {
            var ret = new ResultData();
            usrMst = new UserMst();

            try
            {
                // ユーザ情報取得メソッド呼び出し（内部関数）
                usrMst = User.GetUserInfo(id);

                ret.Result = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "GetUser_Serv", e.Message);
                ret.Message = e.Message;
            }

            return ret;
        }

        /// <summary>
        /// 所属施設取得
        /// </summary>
        /// <param name="id">ユーザID</param>
        /// <param name="usrMst">o:施設情報クラス</param>
        /// <returns>処理結果クラス</returns>
        public ResultData GetHosp_Serv(int id, out HospMst hosMst)
        {
            var ret = new ResultData();
            hosMst = new HospMst();

            try
            {
                // 施設情報取得メソッド呼び出し（内部関数）
                hosMst = Hosp.GetHosp(id);

                ret.Result = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "GetHosp_Serv", e.Message);
                ret.Message = e.Message;
            }

            return ret;
        }

        /// <summary>
        /// 施設情報一覧取得
        /// </summary>
        /// <param name="hosList">o:施設情報クラス配列</param>
        /// <returns>処理結果クラス</returns>
        public ResultData GetHospList_Serv(out HospMst[] hosList)
        {
            var ret = new ResultData();
            hosList = null;

            try
            {
                // 施設情報取得メソッド呼び出し（内部関数）
                hosList = Hosp.GetHospList();

                ret.Result = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "GetHospList_Serv", e.Message);
                ret.Message = e.Message;
            }

            return ret;
        }

        /// <summary>
        /// 施設設定情報取得
        /// </summary>
        /// <param name="id">施設ID</param>
        /// <param name="hosCon">o:施設設定クラス</param>
        /// <returns>処理結果クラス</returns>
        public ResultData GetHospConf_Serv(int id, out HospitalConfig hosCon)
        {
            var ret = new ResultData();
            hosCon = new HospitalConfig();

            try
            {
                // 施設設定情報取得メソッド呼び出し（内部関数）
                hosCon = Hosp.GetHospConfig(id);

                ret.Result = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "GetHospConf_Serv", e.Message);
                ret.Message = e.Message;
            }

            return ret;
        }

        /// <summary>
        /// 事前登録オーダーリスト取得
        /// </summary>
        /// <param name="search">検索項目クラス</param>
        /// <param name="orderList">o:事前登録オーダー情報クラス一覧</param>
        /// <returns></returns>
        public ResultData GetPreOrder_Serv(Search search, out PreOrder[] orderList)
        {
            var ret = new ResultData();
            orderList = null;

            try
            {
                // 事前登録オーダーリスト取得メソッド呼び出し（内部関数）
                orderList = POrder.GetPreOrderList(search);

                ret.Result = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "GetPreOrder_Serv", e.Message);
                ret.Message = e.Message;
            }

            return ret;
        }

        /// <summary>
        /// 事前オーダー登録
        /// </summary>
        /// <param name="order">事前登録オーダー情報クラス</param>
        /// <returns>処理結果クラス</returns>
        public ResultData SetPreOrder_Serv(PreOrder order)
        {
            var ret = new ResultData();

            try
            {
                if(order.Key == 0)
                {
                    Search search = new Search();

                    search.HospID = order.HospID;
                    search.PatID = order.PatID;

                    Patient[] tmpList = Pat.GetPatientList(search);

                    if(tmpList == null || tmpList.Length == 0)
                        order.Key = Pat.SetPatient(order);
                }
                else
                    Pat.SetPatient(order);


                // 事前オーダー登録メソッド呼び出し（内部関数）
                if (!POrder.SetPreOrder(order))
                    ret.Message = "オーダー登録失敗:" + order.PatID + ":" + order.Modality + ":" + order.Date;
                else
                    ret.Result = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "SetPreOrder_Serv", e.Message);
                ret.Message = e.Message;
            }

            return ret;
        }

        /// <summary>
        /// 患者一覧取得
        /// </summary>
        /// <param name="search">検索項目クラス</param>
        /// <param name="patientList">o:患者情報一覧</param>
        /// <returns>処理結果クラス</returns>
        public ResultData GetPatient_Serv(Search search, out Patient[] patientList)
        {
            var ret = new ResultData();
            patientList = null;

            try
            {
                // 事前登録オーダーリスト取得メソッド呼び出し（内部関数）
                patientList = Pat.GetPatientList(search);

                ret.Result = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "GetPatient_Serv", e.Message);
                ret.Message = e.Message;
            }

            return ret;
        }

        public ResultData SetPatient_Serv(Patient patient, out int PatKey)
        {
            var ret = new ResultData();

            PatKey = 0;

            try
            {
                PatKey = Pat.SetPatient(patient);

                ret.Result = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "SetPreOrder_Serv", e.Message);
                ret.Message = e.Message;
            }

            return ret;
        }

        /// <summary>
        /// 事前オーダー削除
        /// </summary>
        /// <param name="orderid">削除事前オーダーID</param>
        /// <returns>処理結果クラス</returns>
        public ResultData DelPreOrder_Serv(int orderid)
        {
            var ret = new ResultData();

            try
            {
                // 事前オーダー登録メソッド呼び出し（内部関数）
                if (!POrder.DelPreOrder(orderid))
                    ret.Message = "オーダー削除失敗 オーダーID:" + orderid;
                else
                    ret.Result = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "SetPreOrder_Serv", e.Message);
                ret.Message = e.Message;
            }

            return ret;
        }

    }
}
