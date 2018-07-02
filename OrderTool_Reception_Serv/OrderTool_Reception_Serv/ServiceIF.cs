using LogController;
using OrderTool_Reception_Serv.Class;
using OrderTool_Reception_Serv.Util;
using System;

namespace OrderTool_Reception_Serv
{
    public class ServiceIF
    {
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
        /// 施設毎の候補値取得
        /// </summary>
        /// <param name="id">施設ID</param>
        /// <param name="hostemp">o:施設設定クラス</param>
        /// <returns>処理結果クラス</returns>
        public ResultData GetHospTemplate_Serv(int id, out HospitalTemplate[] hostemp)
        {
            var ret = new ResultData();
            hostemp = null;

            try
            {
                // 施設設定情報取得メソッド呼び出し（内部関数）
                hostemp = Hosp.GetHospitalTemplate(id);

                ret.Result = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "GetHospTemplate_Serv", e.Message);
                ret.Message = e.Message;
            }

            return ret;
        }

        /// <summary>
        /// 施設毎の設定取得
        /// </summary>
        /// <param name="id">施設ID</param>
        /// <param name="hosconf">o:施設設定クラス</param>
        /// <returns>処理結果クラス</returns>
        public ResultData GetHospConfig_Serv(int id, out HospitalConfig hosconf)
        {
            var ret = new ResultData();
            hosconf = null;

            try
            {
                // 施設設定情報取得メソッド呼び出し（内部関数）
                hosconf = Hosp.GetHospConfig(id);

                ret.Result = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "GetHospConfig_Serv", e.Message);
                ret.Message = e.Message;
            }

            return ret;
        }

        /// <summary>
        /// 施設毎の設定更新
        /// </summary>
        /// <param name="hosconf">施設設定クラス</param>
        /// <returns>処理結果クラス</returns>
        public ResultData SetHospConfig_Serv(HospitalConfig hosconf)
        {
            var ret = new ResultData();

            try
            {
                // 施設設定更新メソッド呼び出し（内部関数）
                ret.Result = Hosp.SetHospConfig(hosconf);
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "SetHospConfig_Serv", e.Message);
                ret.Message = e.Message;
            }

            return ret;
        }

        /// <summary>
        /// 施設毎の候補値設定
        /// </summary>
        /// <param name="id">施設ID</param>
        /// <param name="hosCon">施設設定クラス</param>
        /// <returns>処理結果クラス</returns>
        public ResultData SetHospTemplate_Serv(int id, string key, string[] hosTempList)
        {
            var ret = new ResultData();

            try
            {
                // 施設設定情報取得メソッド呼び出し（内部関数）
                ret.Result = Hosp.SetHospitalTemplate(id, key, hosTempList);

                ret.Result = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "SetHospTemplate_Serv", e.Message);
                ret.Message = e.Message;
            }

            return ret;
        }

        /// <summary>
        /// ユーザ情報取得
        /// </summary>
        /// <param name="id">ユーザID</param>
        /// <param name="userMst">o:ユーザ情報クラス</param>
        /// <returns>処理結果クラス</returns>
        public ResultData GetUser_Serv(int id, out UserMst userMst)
        {
            var ret = new ResultData();
            userMst = new UserMst();

            try
            {
                // 施設情報取得メソッド呼び出し（内部関数）
                userMst = User.GetUserInfo(id);

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
        /// ユーザ情報一覧取得
        /// </summary>
        /// <param name="userList">o:ユーザ情報クラス配列</param>
        /// <returns>処理結果クラス</returns>
        public ResultData GetUserList_Serv(out UserMst[] userList)
        {
            var ret = new ResultData();
            userList = null;

            try
            {
                // 施設情報取得メソッド呼び出し（内部関数）
                userList = User.GetUserList();

                ret.Result = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "GetUserList_Serv", e.Message);
                ret.Message = e.Message;
            }

            return ret;
        }

        /// <summary>
        /// 施設毎の設定取得
        /// </summary>
        /// <param name="id">施設ID</param>
        /// <param name="hosconf">o:施設設定クラス</param>
        /// <returns>処理結果クラス</returns>
        public ResultData GetUserConfig_Serv(int id, out UserConfig userconf)
        {
            var ret = new ResultData();
            userconf = null;

            try
            {
                // 施設設定情報取得メソッド呼び出し（内部関数）
                userconf = User.GetUserConfig(id);

                ret.Result = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "GetUserConfig_Serv", e.Message);
                ret.Message = e.Message;
            }

            return ret;
        }

        /// <summary>
        /// オーダーリスト取得
        /// </summary>
        /// <param name="search">検索項目クラス</param>
        /// <param name="orderList">o:事前登録オーダー情報クラス一覧</param>
        /// <returns></returns>
        public ResultData GetOrder_Serv(Search search, out Order[] orderList)
        {
            var ret = new ResultData();
            orderList = null;

            try
            {
                // オーダーリスト取得メソッド呼び出し（内部関数）
                orderList = Orders.GetOrderList(search);

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
        public ResultData SetOrder_Serv(Order order)
        {
            var ret = new ResultData();

            try
            {
                if (order.Key == 0)
                {
                    Search search = new Search();

                    search.PatID = order.PatID;

                    Patient[] tmpList = Pat.GetPatientList(search);

                    if (tmpList == null || tmpList.Length == 0)
                        order.Key = Pat.SetPatient(order);
                }
                else
                    Pat.SetPatient(order);


                // 事前オーダー登録メソッド呼び出し（内部関数）
                if (!Orders.SetOrder(order))
                    ret.Message = "オーダー登録失敗:" + order.PatID + ":" + order.Modality + ":" + order.Date;
                else
                    ret.Result = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "SetOrder_Serv", e.Message);
                ret.Message = e.Message;
            }

            return ret;
        }

        /// <summary>
        /// 事前オーダー登録(オーダーID返却)
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderid"></param>
        /// <returns>処理結果クラス</returns>
        public ResultData SetOrder_Serv(Order order, out int orderid)
        {
            var ret = new ResultData();
            orderid = 0;
            try
            {
                if (order.Key == 0)
                {
                    Search search = new Search();

                    search.PatID = order.PatID;

                    Patient[] tmpList = Pat.GetPatientList(search);

                    if (tmpList == null || tmpList.Length == 0)
                        order.Key = Pat.SetPatient(order);
                }
                else
                    Pat.SetPatient(order);

                orderid = Orders.SetOrder_RetId(order);

                // 事前オーダー登録メソッド呼び出し（内部関数）
                if (orderid == 0)
                    ret.Message = "オーダー登録失敗:" + order.PatID + ":" + order.Modality + ":" + order.Date;
                else
                    ret.Result = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "SetOrder_Serv", e.Message);
                ret.Message = e.Message;
            }

            return ret;
        }

        /// <summary>
        /// オーダー削除
        /// </summary>
        /// <param name="orderid">削除オーダーID</param>
        /// <returns>処理結果クラス</returns>
        public ResultData DelOrder_Serv(int orderid)
        {
            var ret = new ResultData();

            try
            {
                // 事前オーダー登録メソッド呼び出し（内部関数）
                if (!Orders.DelOrder(orderid))
                    ret.Message = "オーダー削除失敗 オーダーID:" + orderid;
                else
                    ret.Result = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "DelOrder_Serv", e.Message);
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
        /// オーダーリスト取得
        /// </summary>
        /// <param name="search">検索項目クラス</param>
        /// <param name="fileList">o:事前登録オーダー情報クラス一覧</param>
        /// <returns></returns>
        public ResultData GetFile_Serv(int id, out Files[] fileList)
        {
            var ret = new ResultData();
            fileList = null;

            try
            {
                // オーダーリスト取得メソッド呼び出し（内部関数）
                fileList = FileData.GetFileList(id);

                ret.Result = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "GetPreFile_Serv", e.Message);
                ret.Message = e.Message;
            }

            return ret;
        }

        /// <summary>
        /// 事前オーダー登録
        /// </summary>
        /// <param name="file">事前登録オーダー情報クラス</param>
        /// <returns>処理結果クラス</returns>
        public ResultData SetFile_Serv(Files file)
        {
            var ret = new ResultData();

            try
            {

                // 事前オーダー登録メソッド呼び出し（内部関数）
                if (!FileData.SetFile(file))
                    ret.Message = "ファイル登録失敗:" + file.Name + ":" + file.OrderID;
                else
                    ret.Result = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "SetFile_Serv", e.Message);
                ret.Message = e.Message;
            }

            return ret;
        }

        /// <summary>
        /// オーダー削除
        /// </summary>
        /// <param name="fileid">削除オーダーID</param>
        /// <returns>処理結果クラス</returns>
        public ResultData DelFile_Serv(int orderid)
        {
            var ret = new ResultData();

            try
            {
                // 事前オーダー登録メソッド呼び出し（内部関数）
                if (!FileData.DelFile(orderid))
                    ret.Message = "ファイル削除失敗 オーダーID:" + orderid;
                else
                    ret.Result = true;
            }
            catch (Exception e)
            {
                LogControl.WriteLog(LogType.ERR, "DelFile_Serv", e.Message);
                ret.Message = e.Message;
            }

            return ret;
        }
    }
}
