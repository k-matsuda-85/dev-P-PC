using MyAccDB;
using OrderTool_Reception_Serv.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderTool_Reception_Serv.Class
{
    static class User
    {
        public static UserMst[] GetUserList()
        {
            UserMst[] ret = null;

            // DB接続
            using (var con = new AccDbConnection(Conf._settings))
            {
                ret = getUserList(con);
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

        public static UserConfig GetUserConfig(int id)
        {
            UserConfig ret = new UserConfig();

            // DB接続
            using (var con = new AccDbConnection(Conf._settings))
            {
                ret = getUserConfig(con, id);

                if (ret.Conf.Length == 0)
                    ret = getUserConfig(con, 0);

            }

            return ret;
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
                selSQL.Append(",name");
                selSQL.Append(",cd");
                selSQL.Append(",seq");
                selSQL.Append(",permission");
                selSQL.Append(" FROM");
                selSQL.Append(" M_User");
                selSQL.Append(" ORDER BY");
                selSQL.Append(" seq");

                cmd.CommandText = selSQL.ToString();

                // SQL実行
                using (var dr = cmd.ExecuteReader())
                    // 該当データがある場合、返却値を設定
                    while (dr.Read())
                    {
                        UserMst tmpUser = new UserMst();
                        tmpUser.UserID = Convert.ToInt32(dr["id"]);
                        tmpUser.CD = dr["cd"].ToString();
                        if (dr["name"] != DBNull.Value)
                            tmpUser.Name = dr["name"].ToString();
                        tmpUser.Seq = Convert.ToInt32(dr["seq"]);
                        tmpUser.Permission = Convert.ToInt32(dr["permission"]);

                        ret.Add(tmpUser);
                    }
            }

            return ret.ToArray();
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
                selSQL.Append(",cd");
                selSQL.Append(",seq");
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
                        ret.CD = dr["cd"].ToString();
                        if (dr["name"] != DBNull.Value)
                            ret.Name = dr["name"].ToString();
                        ret.Seq = Convert.ToInt32(dr["seq"]);
                        ret.Permission = Convert.ToInt32(dr["permission"]);
                    }
            }

            return ret;
        }

        private static UserConfig getUserConfig(AccDbConnection con, int id)
        {
            UserConfig ret = new UserConfig();
            List<Config> confs = new List<Config>();

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                // SQL生成
                // ----------------------------
                // SELECT key,value
                // FROM UserConfig
                // WHERE user_id=id
                // ----------------------------
                StringBuilder selSQL = new StringBuilder();
                selSQL.Append("SELECT");
                selSQL.Append(" key");
                selSQL.Append(",value");
                selSQL.Append(" FROM");
                selSQL.Append(" UserConfig");
                selSQL.Append(" WHERE");
                selSQL.Append(" user_id=");
                selSQL.Append(cmd.Add(id).ParameterName);

                cmd.CommandText = selSQL.ToString();

                // SQL実行
                using (var dr = cmd.ExecuteReader())
                    // 該当データがある場合、返却値を設定
                    while (dr.Read())
                    {
                        Config tmpConf = new Config();
                        tmpConf.Key = dr["key"].ToString();
                        if (dr["value"] != DBNull.Value)
                            tmpConf.Value = dr["value"].ToString();

                        confs.Add(tmpConf);
                    }
            }

            ret.Conf = confs.ToArray();

            return ret;
        }
    }


}
