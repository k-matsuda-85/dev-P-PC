using MyAccDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderTool_Serv.Class
{
    public static class Hosp
    {
        public static HospMst[] GetHospList()
        {
            HospMst[] ret = null;

            // DB接続
            using (var con = new AccDbConnection(Conf._settings))
            {
                ret = getHospList(con);
            }

            return ret;
        }

        public static HospMst GetHosp(int id)
        {
            HospMst ret = new HospMst();

            // DB接続
            using (var con = new AccDbConnection(Conf._settings))
            {
                ret = getHosp(con, id);
            }

            return ret;
        }

        public static HospitalConfig GetHospConfig(int id)
        {
            HospitalConfig ret = new HospitalConfig();

            // DB接続
            using (var con = new AccDbConnection(Conf._settings))
            {
                ret = getHospConfig(con, id);

                if (ret.Conf.Length == 0)
                    ret = getHospConfig(con, 0);

            }

            return ret;
        }

        private static HospMst[] getHospList(AccDbConnection con)
        {
            List<HospMst> ret = new List<HospMst>();

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                // SQL生成
                // ----------------------------
                // SELECT H.id, H.name, H.cd, H.seq
                // FROM M_Hosp As H
                // INNER JOIN HospUser HU
                // ON H.id = HU.hosp_id
                // WHERE user_id=id
                // ORDER BY H.seq ASC
                // ----------------------------
                StringBuilder selSQL = new StringBuilder();
                selSQL.Append("SELECT");
                selSQL.Append(" H.id");
                selSQL.Append(",H.name");
                selSQL.Append(",H.cd");
                selSQL.Append(",H.seq");
                selSQL.Append(",H.visible");
                selSQL.Append(" FROM");
                selSQL.Append(" M_Hosp As H");
                selSQL.Append(" ORDER BY");
                selSQL.Append(" H.seq");
                selSQL.Append(" ASC");

                cmd.CommandText = selSQL.ToString();

                // SQL実行
                using (var dr = cmd.ExecuteReader())
                    // 該当データがある場合、返却値を設定
                    while (dr.Read())
                    {
                        HospMst tmp = new HospMst();
                        tmp.HospID = Convert.ToInt32(dr["id"]);
                        tmp.CD = dr["cd"].ToString();
                        if (dr["name"] != DBNull.Value)
                            tmp.Name = dr["name"].ToString();
                        tmp.Seq = Convert.ToInt32(dr["seq"]);
                        tmp.Visible = Convert.ToInt32(dr["visible"]);

                        ret.Add(tmp);
                    }
            }

            return ret.ToArray();
        }


        private static HospMst getHosp(AccDbConnection con, int id)
        {
            HospMst ret = new HospMst();

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                // SQL生成
                // ----------------------------
                // SELECT H.id, H.name, H.cd, H.seq
                // FROM M_Hosp As H
                // INNER JOIN HospUser HU
                // ON H.id = HU.hosp_id
                // WHERE user_id=id
                // ORDER BY H.seq ASC
                // ----------------------------
                StringBuilder selSQL = new StringBuilder();
                selSQL.Append("SELECT");
                selSQL.Append(" H.id");
                selSQL.Append(",H.name");
                selSQL.Append(",H.cd");
                selSQL.Append(",H.seq");
                selSQL.Append(",H.visible");
                selSQL.Append(" FROM");
                selSQL.Append(" M_Hosp As H");
                selSQL.Append(" INNER JOIN");
                selSQL.Append(" HospUser HU");
                selSQL.Append(" ON");
                selSQL.Append(" H.id = HU.hosp_id");
                selSQL.Append(" WHERE");
                selSQL.Append(" user_id=");
                selSQL.Append(cmd.Add(id).ParameterName);
                selSQL.Append(" ORDER BY");
                selSQL.Append(" H.seq");
                selSQL.Append(" ASC");

                cmd.CommandText = selSQL.ToString();

                // SQL実行
                using (var dr = cmd.ExecuteReader())
                    // 該当データがある場合、返却値を設定
                    if (dr.Read())
                    {
                        ret.HospID = Convert.ToInt32(dr["id"]);
                        ret.CD = dr["cd"].ToString();
                        if (dr["name"] != DBNull.Value)
                            ret.Name = dr["name"].ToString();
                        ret.Seq = Convert.ToInt32(dr["seq"]);
                        ret.Visible = Convert.ToInt32(dr["visible"]);
                    }
            }

            return ret;
        }

        private static HospitalConfig getHospConfig(AccDbConnection con, int id)
        {
            HospitalConfig ret = new HospitalConfig();
            List<Config> confs = new List<Config>();

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                // SQL生成
                // ----------------------------
                // SELECT key,value
                // FROM HospConfig
                // WHERE hosp_id=id
                // ----------------------------
                StringBuilder selSQL = new StringBuilder();
                selSQL.Append("SELECT");
                selSQL.Append(" key");
                selSQL.Append(",value");
                selSQL.Append(" FROM");
                selSQL.Append(" HospConfig");
                selSQL.Append(" WHERE");
                selSQL.Append(" hosp_id=");
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
