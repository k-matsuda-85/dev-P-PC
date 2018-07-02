using MyAccDB;
using OrderTool_Reception_Serv.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderTool_Reception_Serv.Class
{
    class FileData
    {
        public static Files[] GetFileList(int id)
        {
            Files[] ret = null;

            // DB接続
            using (var con = new AccDbConnection(Conf._settings))
            {
                ret = getFileList(con, id);
            }

            return ret;
        }

        public static bool SetFile(Files file)
        {
            bool ret = false;

            // DB接続
            using (var con = new AccDbConnection(Conf._settings))
            {
                if (file.FileID == 0)
                    ret = setFile(con, file);
                else
                    ret = udtFile(con, file);
            }

            return ret;
        }

        public static bool DelFile(int id)
        {
            bool ret = false;

            // DB接続
            using (var con = new AccDbConnection(Conf._settings))
            {
                ret = delFile(con, id);
            }

            return ret;
        }

        private static Files[] getFileList(AccDbConnection con, int id)
        {
            List<Files> ret = new List<Files>();

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                // SQL生成
                // ----------------------------
                // ----------------------------
                StringBuilder selSQL = new StringBuilder();
                selSQL.Append("SELECT");
                selSQL.Append(" id");
                selSQL.Append(",order_id");
                selSQL.Append(",seq");
                selSQL.Append(",isorigin");
                selSQL.Append(",name");
                selSQL.Append(" FROM");
                selSQL.Append(" Files");
                selSQL.Append(" WHERE");
                selSQL.Append(" order_id=");
                selSQL.Append(cmd.Add(id).ParameterName);
                selSQL.Append(" ORDER BY");
                selSQL.Append(" seq");


                cmd.CommandText = selSQL.ToString();

                // SQL実行
                using (var dr = cmd.ExecuteReader())
                    // 該当データがある場合、返却値を設定
                    while (dr.Read())
                    {
                        Files tmpFile = new Files();

                        tmpFile.FileID = Convert.ToInt32(dr["id"]);

                        tmpFile.OrderID = Convert.ToInt32(dr["order_id"]);

                        tmpFile.IsOrigin = Convert.ToInt32(dr["isorigin"]);

                        tmpFile.Seq = Convert.ToInt32(dr["seq"]);

                        tmpFile.Name = dr["name"].ToString();

                        ret.Add(tmpFile);
                    }
            }

            return ret.ToArray();
        }

        private static bool setFile(AccDbConnection con, Files file)
        {
            bool ret = false;

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                StringBuilder insSQL = new StringBuilder();
                insSQL.Append("INSERT");
                insSQL.Append(" INTO");
                insSQL.Append(" Files");

                insSQL.Append(" (");
                insSQL.Append(" order_id");
                insSQL.Append(",seq");
                insSQL.Append(",isorigin");
                insSQL.Append(",name");
                insSQL.Append(" )");

                insSQL.Append(" VALUES");

                insSQL.Append(" (");
                insSQL.Append(cmd.Add(file.OrderID).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(file.Seq).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(file.IsOrigin).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(file.Name).ParameterName);
                insSQL.Append(" )");

                cmd.CommandText = insSQL.ToString();

                var retcnt = cmd.ExecuteNonQuery();
                if (retcnt > 0)
                    ret = true;
            }

            return ret;
        }

        private static bool udtFile(AccDbConnection con, Files file)
        {
            bool ret = false;

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                StringBuilder udtSQL = new StringBuilder();
                udtSQL.Append("UPDATE");
                udtSQL.Append(" Files");
                udtSQL.Append(" SET");
                udtSQL.Append(" order_id=" + cmd.Add(file.OrderID).ParameterName);
                udtSQL.Append(",seq=" + cmd.Add(file.Seq).ParameterName);
                udtSQL.Append(",isorigin=" + cmd.Add(file.IsOrigin).ParameterName);
                udtSQL.Append(",name=" + cmd.Add(file.Name).ParameterName);
                udtSQL.Append(" WHERE");
                udtSQL.Append(" id=");
                udtSQL.Append(cmd.Add(file.FileID).ParameterName);

                cmd.CommandText = udtSQL.ToString();

                var retcnt = cmd.ExecuteNonQuery();
                if (retcnt > 0)
                    ret = true;
            }

            return ret;
        }

        private static bool delFile(AccDbConnection con, int orderid)
        {
            bool ret = false;

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                StringBuilder delSQL = new StringBuilder();
                delSQL.Append("DELETE");
                delSQL.Append(" FROM");
                delSQL.Append(" Files");
                delSQL.Append(" WHERE");
                delSQL.Append(" order_id=");
                delSQL.Append(cmd.Add(orderid).ParameterName);

                cmd.CommandText = delSQL.ToString();

                cmd.ExecuteNonQuery();
                ret = true;
            }

            return ret;
        }

    }
}
