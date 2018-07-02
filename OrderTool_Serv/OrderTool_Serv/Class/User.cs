using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using MyAccDB;

namespace OrderTool_Serv.Class
{
    /// <summary>
    /// ユーザー情報操作クラス
    /// </summary>
    public static class User
    {
        public static Login GetSessionKey(int id)
        {
            Login ret = new Login();

            // DB接続
            using (var con = new AccDbConnection(Conf._settings))
            {
                // セッションキー取得
                ret.Key = getSessionKey(con, id);

                ret.UserID = id;
            }

            return ret;
        }

        public static Login LoginAcc(string id, string pw)
        {
            Login ret = new Login();

            // DB接続
            using (var con = new AccDbConnection(Conf._settings))
            {
                // ユーザ情報確認
                int userid = getUserID(con, id, pw);

                if (userid == 0)
                    return ret;

                Random _r = new Random();

                char[] list = new char[16];
                for (int i = 0; i < 16; i++)
                {
                    int num = _r.Next(0, Conf._randStr.Length); // ランダムに０～ｎの数値を返す
                    list[i] = Conf._randStr[num];
                }

                string random = new string(list);

                // ログインセッション登録
                if (setSessionKey(con, userid, random))
                {
                    ret.UserID = userid;
                    ret.Key = random;
                }
            }

            return ret;
        }

        public static UserMst GetUserInfo(int id)
        {
            UserMst ret = new UserMst();

            // DB接続
            using (var con = new AccDbConnection(Conf._settings))
            {
                ret = getUser(con, id);
            }

            return ret;
        }

        public static UserMst[] GetUserList(int hosp_id)
        {
            UserMst[] ret = null;

            // DB接続
            using (var con = new AccDbConnection(Conf._settings))
            {
                if(hosp_id == 0)
                    ret = getUserList(con);
                else
                    ret = getUserList(con, hosp_id);
            }

            return ret;
        }

        public static int SetUser(UserMst user)
        {
            int ret = 0;

            // DB接続
            using (var con = new AccDbConnection(Conf._settings))
            {
                con.BeginTransaction();

                if (user.UserID == 0)
                    setUser(con, user);
                else
                    udtUser(con, user);

                con.Commit();

                ret = getUserID(con, user.LoginID, user.LoginPW);
            }

            return ret;
        }

        private static string getSessionKey(AccDbConnection con, int id)
        {
            string ret = "";

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                // SQL生成
                // ----------------------------
                // SELECT id
                // FROM M_User
                // WHERE login_id=id
                // AND login_pw=pw
                // ----------------------------
                StringBuilder selSQL = new StringBuilder();
                selSQL.Append("SELECT");
                selSQL.Append(" key");
                selSQL.Append(" FROM");
                selSQL.Append(" Login");
                selSQL.Append(" WHERE");
                selSQL.Append(" user_id=");
                selSQL.Append(cmd.Add(id).ParameterName);

                cmd.CommandText = selSQL.ToString();

                // SQL実行
                using (var dr = cmd.ExecuteReader())
                    // 該当データがある場合、返却値を設定
                    if (dr.Read())
                        ret = dr["key"].ToString();
            }

            return ret;
        }

        private static int getUserID(AccDbConnection con, string id, string pw)
        {
            int ret = 0;

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                // SQL生成
                // ----------------------------
                // SELECT id
                // FROM M_User
                // WHERE login_id=id
                // AND login_pw=pw
                // ----------------------------
                StringBuilder selSQL = new StringBuilder();
                selSQL.Append("SELECT");
                selSQL.Append(" id");
                selSQL.Append(" FROM");
                selSQL.Append(" M_User");
                selSQL.Append(" WHERE");
                selSQL.Append(" login_id=");
                selSQL.Append(cmd.Add(id).ParameterName);
                selSQL.Append(" AND");
                selSQL.Append(" login_pw=");
                selSQL.Append(cmd.Add(pw).ParameterName);

                cmd.CommandText = selSQL.ToString();

                // SQL実行
                using (var dr = cmd.ExecuteReader())
                    // 該当データがある場合、返却値を設定
                    if (dr.Read())
                        ret = Convert.ToInt32(dr["id"]);
            }

            return ret;
        }

        private static UserMst getUser(AccDbConnection con, int id)
        {
            UserMst ret = new UserMst();

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                // SQL生成
                // ----------------------------
                // SELECT id,name,permission
                // FROM M_User
                // WHERE login_id=id
                // AND id=id
                // ----------------------------
                StringBuilder selSQL = new StringBuilder();
                selSQL.Append("SELECT");
                selSQL.Append(" id");
                selSQL.Append(",name");
                selSQL.Append(",permission");
                selSQL.Append(" FROM");
                selSQL.Append(" M_User");
                selSQL.Append(" WHERE");
                selSQL.Append(" id=");
                selSQL.Append(cmd.Add(id).ParameterName);

                cmd.CommandText = selSQL.ToString();

                // SQL実行
                using (var dr = cmd.ExecuteReader())
                    // 該当データがある場合、返却値を設定
                    if (dr.Read())
                    {
                        ret.UserID = id;
                        if (dr["name"] != DBNull.Value)
                            ret.UserName = dr["name"].ToString();
                        ret.Permission = Convert.ToInt32(dr["permission"]);
                    }
            }

            return ret;
        }

        private static UserMst[] getUserList(AccDbConnection con, int hosp_id)
        {
            List<UserMst> ret = new List<UserMst>();

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                // SQL生成
                // ----------------------------
                // SELECT U.id, U.login_id, U.name, U.permission
                // FROM M_User As U
                // INNER JOIN HospUser HU
                // ON U.id = HU.user_id
                // WHERE hosp_id=hosp_id
                // ----------------------------
                StringBuilder selSQL = new StringBuilder();
                selSQL.Append("SELECT");
                selSQL.Append(" U.id");
                selSQL.Append(",U.login_id");
                selSQL.Append(",U.name");
                selSQL.Append(",U.permission");
                selSQL.Append(" FROM");
                selSQL.Append(" M_User As U");
                selSQL.Append(" INNER JOIN");
                selSQL.Append(" HospUser HU");
                selSQL.Append(" ON");
                selSQL.Append(" U.id = HU.user_id");
                selSQL.Append(" WHERE");
                selSQL.Append(" hosp_id=");
                selSQL.Append(cmd.Add(hosp_id).ParameterName);

                cmd.CommandText = selSQL.ToString();

                // SQL実行
                using (var dr = cmd.ExecuteReader())
                    // 該当データがある場合、返却値を設定
                    while (dr.Read())
                    {
                        UserMst tmpUser = new UserMst();
                        tmpUser.UserID = Convert.ToInt32(dr["user_id"]);
                        tmpUser.LoginID = dr["login_id"].ToString();
                        if (dr["name"] != DBNull.Value)
                            tmpUser.UserName = dr["name"].ToString();
                        tmpUser.Permission = Convert.ToInt32(dr["permission"]);

                        ret.Add(tmpUser);
                    }
            }

            return ret.ToArray();
        }

        private static UserMst[] getUserList(AccDbConnection con)
        {
            List<UserMst> ret = new List<UserMst>();

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                // SQL生成
                // ----------------------------
                // SELECT id,login_id,name,permission
                // FROM M_User
                // ----------------------------
                StringBuilder selSQL = new StringBuilder();
                selSQL.Append("SELECT");
                selSQL.Append(" id");
                selSQL.Append(",login_id");
                selSQL.Append(",name");
                selSQL.Append(",permission");
                selSQL.Append(" FROM");
                selSQL.Append(" M_User");

                cmd.CommandText = selSQL.ToString();

                // SQL実行
                using (var dr = cmd.ExecuteReader())
                    // 該当データがある場合、返却値を設定
                    while (dr.Read())
                    {
                        UserMst tmpUser = new UserMst();
                        tmpUser.UserID = Convert.ToInt32(dr["user_id"]);
                        tmpUser.LoginID = dr["login_id"].ToString();
                        if (dr["name"] != DBNull.Value)
                            tmpUser.UserName = dr["name"].ToString();
                        tmpUser.Permission = Convert.ToInt32(dr["permission"]);

                        ret.Add(tmpUser);
                    }
            }

            return ret.ToArray();
        }


        private static bool setSessionKey(AccDbConnection con, int id, string key)
        {
            bool ret = false;

            // コマンドオブジェクト生成（DELETE用）
            using (var cmd = con.CreateCommand())
            {
                // SQL生成
                // ----------------------------
                // DELETE
                // FROM Login
                // WHERE
                // user_id=id
                // ----------------------------
                StringBuilder delSQL = new StringBuilder();
                delSQL.Append("DELETE");
                delSQL.Append(" FROM");
                delSQL.Append(" Login");
                delSQL.Append(" WHERE");
                delSQL.Append(" user_id=");
                delSQL.Append(cmd.Add(id).ParameterName);

                cmd.CommandText = delSQL.ToString();

                cmd.ExecuteNonQuery();
            }

            // コマンドオブジェクト生成（INSERT用）
            using (var cmd = con.CreateCommand())
            {
                // SQL生成
                // ----------------------------
                // INSERT
                // INTO Login
                // (user_id, key)
                // VALUES
                // (id, key)
                // ----------------------------
                StringBuilder insSQL = new StringBuilder();
                insSQL.Append("INSERT");
                insSQL.Append(" INTO");
                insSQL.Append(" Login");
                insSQL.Append(" (");
                insSQL.Append(" user_id");
                insSQL.Append(",key");
                insSQL.Append(" )");
                insSQL.Append(" VALUES");
                insSQL.Append(" (");
                insSQL.Append(cmd.Add(id).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(key).ParameterName);
                insSQL.Append(" )");

                cmd.CommandText = insSQL.ToString();

                var retcnt = cmd.ExecuteNonQuery();
                if (retcnt > 0)
                    ret = true;
            }

            return ret;
        }

        private static bool setUser(AccDbConnection con, UserMst user)
        {
            bool ret = false;

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                // SQL生成
                // ----------------------------
                // INSERT
                // INTO M_User
                // (login_id, login_pw, name, permission)
                // VALUES
                // (user.LoginID, user.LoginPW, user.UserName, user.Permission)
                // ----------------------------
                StringBuilder insSQL = new StringBuilder();
                insSQL.Append("INSERT");
                insSQL.Append(" INTO");
                insSQL.Append(" M_User");
                insSQL.Append(" (");
                insSQL.Append(" login_id");
                insSQL.Append(",login_pw");
                insSQL.Append(",name");
                insSQL.Append(",permission");
                insSQL.Append(" )");
                insSQL.Append(" VALUES");
                insSQL.Append(" (");
                insSQL.Append(cmd.Add(user.LoginID).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(user.LoginPW).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(user.UserName).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(user.Permission).ParameterName);
                insSQL.Append(" )");

                cmd.CommandText = insSQL.ToString();

                var retcnt = cmd.ExecuteNonQuery();
                if (retcnt > 0)
                    ret = true;
            }

            return ret;
        }

        private static bool udtUser(AccDbConnection con, UserMst user)
        {
            bool ret = false;

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                // SQL生成
                // ----------------------------
                // UPDATE
                // M_User
                // SET
                //  login_id=user.LoginID
                // ,login_pw=user.LoginPW
                // ,name=user.UserName
                // ,permission=user.Permission
                // WHERE
                // id=user.UserID
                // ----------------------------
                StringBuilder udtSQL = new StringBuilder();
                udtSQL.Append("UPDATE");
                udtSQL.Append(" M_User");
                udtSQL.Append(" SET");
                udtSQL.Append(" login_id=" + cmd.Add(user.LoginID).ParameterName);
                udtSQL.Append(",login_pw=" + cmd.Add(user.LoginPW).ParameterName);
                udtSQL.Append(",name=" + cmd.Add(user.UserName).ParameterName);
                udtSQL.Append(",permission=" + cmd.Add(user.Permission).ParameterName);
                udtSQL.Append(" WHERE");
                udtSQL.Append(" id=");
                udtSQL.Append(cmd.Add(user.UserID).ParameterName);

                cmd.CommandText = udtSQL.ToString();

                var retcnt = cmd.ExecuteNonQuery();
                if (retcnt > 0)
                    ret = true;
            }

            return ret;
        }

    }

}
