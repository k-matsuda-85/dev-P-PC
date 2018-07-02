using MyAccDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderTool_Serv.Class
{
    public static class Pat
    {
        public static Patient[] GetPatientList(Search search)
        {
            Patient[] ret = null;

            // DB接続
            using (var con = new AccDbConnection(Conf._settings))
            {
                ret = getPatientList(con, search);
            }

            return ret;
        }

        public static int SetPatient(Patient patient)
        {
            int ret = 0;

            // DB接続
            using (var con = new AccDbConnection(Conf._settings))
            {
                Search search = new Search();

                search.HospID = patient.HospID;
                search.PatID = patient.PatID;

                Patient[] tmpList = null;

                tmpList = getPatientList(con, search);

                if (tmpList == null || tmpList.Length == 0)
                    ret = setPatient(con, patient);
                else
                {
                    patient.Key = tmpList[0].Key;
                    udtPatient(con, patient);

                    ret = patient.Key;
                }
            }

            return ret;
        }

        private static Patient[] getPatientList(AccDbConnection con, Search search)
        {
            List<Patient> ret = new List<Patient>();

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                // SQL生成
                // ----------------------------
                // SELECT key,hospid,patid,patname,patname_h,patname_r,patsex,patbirth
                // FROM Patient
                // 以下、省略可
                // WHERE 
                // hospid=search.HospID
                // または
                // WHERE 
                // patid=search.PatID
                // ----------------------------
                StringBuilder selSQL = new StringBuilder();
                selSQL.Append("SELECT");
                selSQL.Append(" key");
                selSQL.Append(",hospid");
                selSQL.Append(",patid");
                selSQL.Append(",patname");
                selSQL.Append(",patname_h");
                selSQL.Append(",patname_r");
                selSQL.Append(",patsex");
                selSQL.Append(",patbirth");
                selSQL.Append(" FROM");
                selSQL.Append(" Patient");

                string strWhere = " WHERE";

                if (search.HospID > 0)
                {
                    selSQL.Append(strWhere);
                    selSQL.Append(" hospid=");
                    selSQL.Append(cmd.Add(search.HospID).ParameterName);

                    strWhere = " AND";
                }

                if (!string.IsNullOrEmpty(search.PatID))
                {
                    selSQL.Append(strWhere);
                    selSQL.Append(" patid=");
                    selSQL.Append(cmd.Add(search.PatID).ParameterName);
                }
                selSQL.Append(" ORDER BY");
                selSQL.Append(" patid");
                selSQL.Append(" ASC");

                cmd.CommandText = selSQL.ToString();

                // SQL実行
                using (var dr = cmd.ExecuteReader())
                    // 該当データがある場合、返却値を設定
                    while (dr.Read())
                    {
                        Patient tmpPat = new Patient();
                        tmpPat.Key = Convert.ToInt32(dr["key"]);
                        tmpPat.HospID = Convert.ToInt32(dr["hospid"]);
                        tmpPat.PatID = dr["patid"].ToString();
                        if (dr["patname"] != DBNull.Value)
                            tmpPat.PatName = dr["patname"].ToString();

                        if (dr["patname_h"] != DBNull.Value)
                            tmpPat.PatName_H = dr["patname_h"].ToString();

                        if (dr["patname_r"] != DBNull.Value)
                            tmpPat.PatName_R = dr["patname_r"].ToString();

                        if (dr["patbirth"] != DBNull.Value)
                            tmpPat.BirthDay = dr["patbirth"].ToString();

                        tmpPat.Sex = Convert.ToInt32(dr["patsex"]);

                        ret.Add(tmpPat);
                    }
            }


            return ret.ToArray();
        }


        private static int setPatient(AccDbConnection con, Patient patient)
        {
            int ret = 0;

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                StringBuilder insSQL = new StringBuilder();
                insSQL.Append("INSERT");
                insSQL.Append(" INTO");
                insSQL.Append(" Patient");

                insSQL.Append(" (");
                insSQL.Append(" hospid");
                insSQL.Append(",patid");
                insSQL.Append(",patname");
                insSQL.Append(",patname_h");
                insSQL.Append(",patname_r");
                insSQL.Append(",patsex");
                insSQL.Append(",patbirth");
                insSQL.Append(" )");

                insSQL.Append(" VALUES");

                insSQL.Append(" (");
                insSQL.Append(cmd.Add(patient.HospID).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(patient.PatID).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(patient.PatName).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(patient.PatName_H).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(patient.PatName_R).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(patient.Sex).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(patient.BirthDay).ParameterName);
                insSQL.Append(" )");

                insSQL.Append(" RETURNING");
                insSQL.Append(" key");

                cmd.CommandText = insSQL.ToString();

                // SQL実行
                using (var dr = cmd.ExecuteReader())
                    // 該当データがある場合、返却値を設定
                    if (dr.Read())
                        ret = Convert.ToInt32(dr["key"]);

            }

            return ret;
        }

        private static bool udtPatient(AccDbConnection con, Patient patient)
        {
            bool ret = false;

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                StringBuilder udtSQL = new StringBuilder();
                udtSQL.Append("UPDATE");
                udtSQL.Append(" Patient");
                udtSQL.Append(" SET");
                udtSQL.Append(" hospid=" + cmd.Add(patient.HospID).ParameterName);
                udtSQL.Append(",patid=" + cmd.Add(patient.PatID).ParameterName);
                udtSQL.Append(",patname=" + cmd.Add(patient.PatName).ParameterName);
                udtSQL.Append(",patname_h=" + cmd.Add(patient.PatName_H).ParameterName);
                udtSQL.Append(",patname_r=" + cmd.Add(patient.PatName_R).ParameterName);
                udtSQL.Append(",patsex=" + cmd.Add(patient.Sex).ParameterName);
                udtSQL.Append(",patbirth=" + cmd.Add(patient.BirthDay).ParameterName);
                udtSQL.Append(" WHERE");
                udtSQL.Append(" key=");
                udtSQL.Append(cmd.Add(patient.Key).ParameterName);

                cmd.CommandText = udtSQL.ToString();

                var retcnt = cmd.ExecuteNonQuery();
                if (retcnt > 0)
                    ret = true;
            }

            return ret;
        }
    }
}
