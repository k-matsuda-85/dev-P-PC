using MyAccDB;
using OrderTool_Reception_Serv.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderTool_Reception_Serv.Class
{
    static class Hosp
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

        public static HospitalTemplate[] GetHospitalTemplate(int id)
        {
            HospitalTemplate[] ret = null;

            // DB接続
            using (var con = new AccDbConnection(Conf._settings))
            {
                ret = getHospTemplate(con, id);
            }

            return ret;
        }

        public static bool SetHospitalTemplate(int id, string key, string[] tempList)
        {
            bool ret = false;

            // DB接続
            using (var con = new AccDbConnection(Conf._settings))
            {
                ret = setHospTemplate(con, id, key, tempList);
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

        public static bool SetHospConfig(HospitalConfig hospconf)
        {
            bool ret = false;

            // DB接続
            using (var con = new AccDbConnection(Conf._settings))
            {
                ret = setHospConfig(con, hospconf);
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
                // ORDER BY H.cd ASC
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
                selSQL.Append(" H.cd");
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
                // ORDER BY H.cd ASC
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
                selSQL.Append(" H.cd");
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

        private static HospitalTemplate[] getHospTemplate(AccDbConnection con, int id)
        {
            List<HospitalTemplate> ret = new List<HospitalTemplate>();

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                // SQL生成
                // ----------------------------
                // SELECT key,value,index
                // FROM HospTemplate
                // WHERE hosp_id=id
                // ----------------------------
                StringBuilder selSQL = new StringBuilder();
                selSQL.Append("SELECT");
                selSQL.Append(" key");
                selSQL.Append(",value");
                selSQL.Append(",index");
                selSQL.Append(" FROM");
                selSQL.Append(" HospTemplate");
                selSQL.Append(" WHERE");
                selSQL.Append(" hosp_id=");
                selSQL.Append(cmd.Add(id).ParameterName);
                selSQL.Append(" ORDER BY");
                selSQL.Append(" key");
                selSQL.Append(" ASC");
                selSQL.Append(",index");
                selSQL.Append(" ASC");

                cmd.CommandText = selSQL.ToString();

                // SQL実行
                using (var dr = cmd.ExecuteReader())
                    // 該当データがある場合、返却値を設定
                    while (dr.Read())
                    {
                        HospitalTemplate tmpTemp = new HospitalTemplate();
                        tmpTemp.Key = dr["key"].ToString();
                        if (dr["value"] != DBNull.Value)
                            tmpTemp.Value = dr["value"].ToString();
                        tmpTemp.Index = Convert.ToInt32(dr["index"]);

                        ret.Add(tmpTemp);
                    }
            }

            return ret.ToArray();
        }

        private static bool setHospTemplate(AccDbConnection con, int id, string key, string[] tempList)
        {
            bool ret = false;

            using (var cmd = con.CreateCommand())
            {
                StringBuilder delSQL = new StringBuilder();
                delSQL.Append("DELETE");
                delSQL.Append(" FROM");
                delSQL.Append(" HospTemplate");

                delSQL.Append(" WHERE");
                delSQL.Append(" hosp_id=");
                delSQL.Append(cmd.Add(id).ParameterName);
                delSQL.Append(" AND");
                delSQL.Append(" key=");
                delSQL.Append(cmd.Add(key).ParameterName);

                cmd.CommandText = delSQL.ToString();

                cmd.ExecuteNonQuery();
            }

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                StringBuilder insSQL = new StringBuilder();
                insSQL.Append("INSERT");
                insSQL.Append(" INTO");
                insSQL.Append(" HospTemplate");

                insSQL.Append(" (");
                insSQL.Append(" hosp_id");
                insSQL.Append(",key");
                insSQL.Append(",value");
                insSQL.Append(",index");
                insSQL.Append(" )");

                insSQL.Append(" VALUES");

                for(int i = 0; i < tempList.Length; i ++)
                {
                    if(i > 0)
                        insSQL.Append(",");

                    insSQL.Append(" (");
                    insSQL.Append(cmd.Add(id).ParameterName);
                    insSQL.Append(",");
                    insSQL.Append(cmd.Add(key).ParameterName);
                    insSQL.Append(",");
                    insSQL.Append(cmd.Add(tempList[i]).ParameterName);
                    insSQL.Append(",");
                    insSQL.Append(cmd.Add(i + 1).ParameterName);
                    insSQL.Append(" )");
                }

                cmd.CommandText = insSQL.ToString();

                var retcnt = cmd.ExecuteNonQuery();
                if (retcnt > 0)
                    ret = true;
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

        private static bool setHospConfig(AccDbConnection con, HospitalConfig hospconf)
        {
            bool ret = false;

            Config[] conf = hospconf.Conf;

            using (var cmd = con.CreateCommand())
            {
                StringBuilder delSQL = new StringBuilder();
                delSQL.Append("DELETE");
                delSQL.Append(" FROM");
                delSQL.Append(" HospConfig");

                delSQL.Append(" WHERE");
                delSQL.Append(" hosp_id=");
                delSQL.Append(cmd.Add(hospconf.HospID).ParameterName);
                delSQL.Append(" AND");
                delSQL.Append(" key IN (");

                for(int i = 0; i < conf.Length; i++)
                {
                    if (i > 0)
                        delSQL.Append(',');

                    delSQL.Append(cmd.Add(conf[i].Key).ParameterName);
                }

                delSQL.Append(")");

                cmd.CommandText = delSQL.ToString();

                cmd.ExecuteNonQuery();
            }

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                StringBuilder insSQL = new StringBuilder();
                insSQL.Append("INSERT");
                insSQL.Append(" INTO");
                insSQL.Append(" HospConfig");

                insSQL.Append(" (");
                insSQL.Append(" hosp_id");
                insSQL.Append(",key");
                insSQL.Append(",value");
                insSQL.Append(" )");

                insSQL.Append(" VALUES");

                for (int i = 0; i < conf.Length; i++)
                {
                    if (i > 0)
                        insSQL.Append(",");

                    insSQL.Append(" (");
                    insSQL.Append(cmd.Add(hospconf.HospID).ParameterName);
                    insSQL.Append(",");
                    insSQL.Append(cmd.Add(conf[i].Key).ParameterName);
                    insSQL.Append(",");
                    insSQL.Append(cmd.Add(conf[i].Value).ParameterName);
                    insSQL.Append(" )");
                }

                cmd.CommandText = insSQL.ToString();

                var retcnt = cmd.ExecuteNonQuery();
                if (retcnt > 0)
                    ret = true;
            }

            return ret;

        }

    }
}
