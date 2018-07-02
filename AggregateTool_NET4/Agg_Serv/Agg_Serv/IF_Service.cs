using Agg_Serv.Class;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using TryDb;
using CommonLib.Log;

namespace Agg_Serv
{
    public class IF_Service
    {
        public User Login(string id, string pw)
        {
            User ret = new User();

            try
            {
                using(var con = new TryDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
                {
                    using(var cmd = con.CreateCommand())
                    {
                        var selSQL = new StringBuilder();

                        selSQL.Append("SELECT");
                        selSQL.Append(" Cd");
                        selSQL.Append(",Name");
                        selSQL.Append(",LoginPW");
                        selSQL.Append(",IsAdmin");
                        selSQL.Append(" FROM");
                        selSQL.Append(" M_User");
                        selSQL.Append(" WHERE");
                        selSQL.Append(" LoginID=" + cmd.Add(id).ParameterName);
                        selSQL.Append(" AND");
                        selSQL.Append(" Status = 0");

                        cmd.CommandText = selSQL.ToString();

                        using(var dr = cmd.ExecuteReader())
                        {
                            if(dr.Read())
                            {
                                if(dr["LoginPW"].ToString() == pw)
                                {
                                    ret.Cd = dr["Cd"].ToString();
                                    ret.Name = dr["Name"].ToString();
                                    ret.LoginID = id;
                                    ret.LoginPW = dr["LoginPW"].ToString();
                                    ret.IsAdmin = dr["IsAdmin"].ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                CustomLog.Write(this, LogType.ERROR, e);
            }

            return ret;
        }

        public User[] GetUser()
        {
            User[] ret = null;

            try
            {
                using (var con = new TryDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        var selSQL = new StringBuilder();

                        selSQL.Append("SELECT");
                        selSQL.Append(" Cd");
                        selSQL.Append(",Name");
                        selSQL.Append(",LoginID");
                        selSQL.Append(",LoginPW");
                        selSQL.Append(",IsAdmin");
                        selSQL.Append(" FROM");
                        selSQL.Append(" M_User");
                        selSQL.Append(" WHERE");
                        selSQL.Append(" Status = 0");

                        cmd.CommandText = selSQL.ToString();

                        using (var dr = cmd.ExecuteReader())
                        {
                            List<User> hospList = new List<User>();

                            while (dr.Read())
                            {
                                User tmp = new User();

                                tmp.Cd = dr["Cd"].ToString();
                                tmp.Name = dr["Name"].ToString();
                                tmp.LoginID = dr["LoginID"].ToString();
                                tmp.LoginPW = dr["LoginPW"].ToString();
                                tmp.IsAdmin = dr["IsAdmin"].ToString();

                                hospList.Add(tmp);
                            }

                            ret = hospList.ToArray();

                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogUtil.Error(e.Message);
            }

            return ret;
        }

        public bool SetUser(User val)
        {
            bool ret = false;

            try
            {
                using (var con = new TryDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
                {
                    if (String.IsNullOrEmpty(val.Cd))
                    {
                        using (var cmd = con.CreateCommand())
                        {
                            var selSQL = new StringBuilder();

                            selSQL.Append("SELECT");
                            selSQL.Append(" MAX(cast(Cd as integer)) As max");
                            selSQL.Append(" FROM");
                            selSQL.Append(" M_User");

                            cmd.CommandText = selSQL.ToString();

                            using (var dr = cmd.ExecuteReader())
                            {
                                if (dr.Read())
                                {
                                    val.Cd = (Convert.ToInt32(dr["max"]) + 1).ToString();
                                }
                                else
                                {
                                    val.Cd = "1";
                                }
                            }
                        }

                        using (var cmd = con.CreateCommand())
                        {
                            var insSQL = new StringBuilder();

                            insSQL.Append("INSERT");
                            insSQL.Append(" INTO");
                            insSQL.Append(" M_User");
                            insSQL.Append(" (");
                            insSQL.Append(" Cd");
                            insSQL.Append(",Name");
                            insSQL.Append(",LoginID");
                            insSQL.Append(",LoginPW");
                            insSQL.Append(",IsAdmin");
                            insSQL.Append(" )");
                            insSQL.Append(" VALUES");
                            insSQL.Append(" (");
                            insSQL.Append(cmd.Add(val.Cd).ParameterName);
                            insSQL.Append("," + cmd.Add(val.Name).ParameterName);
                            insSQL.Append("," + cmd.Add(val.LoginID).ParameterName);
                            insSQL.Append("," + cmd.Add(val.LoginPW).ParameterName);
                            insSQL.Append("," + cmd.Add(val.IsAdmin).ParameterName);
                            insSQL.Append(" )");

                            cmd.CommandText = insSQL.ToString();

                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (var cmd = con.CreateCommand())
                        {
                            var udtSQL = new StringBuilder();

                            udtSQL.Append("UPDATE");
                            udtSQL.Append(" M_User");
                            udtSQL.Append(" SET");
                            udtSQL.Append(" Name=" + cmd.Add(val.Name).ParameterName);
                            udtSQL.Append(",LoginID=" + cmd.Add(val.LoginID).ParameterName);
                            udtSQL.Append(",LoginPW=" + cmd.Add(val.LoginPW).ParameterName);
                            udtSQL.Append(",IsAdmin=" + cmd.Add(val.IsAdmin).ParameterName);
                            udtSQL.Append(",Status=" + cmd.Add(val.Status).ParameterName);
                            udtSQL.Append(" WHERE");
                            udtSQL.Append(" Cd=" + cmd.Add(val.Cd).ParameterName);

                            cmd.CommandText = udtSQL.ToString();

                            cmd.ExecuteNonQuery();
                        }
                    }

                }
                ret = true;
            }
            catch (Exception e)
            {
                LogUtil.Error(e.Message);
            }

            return ret;
        }


        public Hospital[] GetHospital()
        {
            Hospital[] ret = null;

            try
            {
                using (var con = new TryDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        var selSQL = new StringBuilder();

                        selSQL.Append("SELECT");
                        selSQL.Append(" Cd");
                        selSQL.Append(",Name");
                        selSQL.Append(",Name_DB");
                        selSQL.Append(",Name_Disp");
                        selSQL.Append(",IsCopy");
                        selSQL.Append(",SortNo");
                        selSQL.Append(" FROM");
                        selSQL.Append(" M_Hospital");
                        selSQL.Append(" WHERE");
                        selSQL.Append(" Status = 0");
                        selSQL.Append(" ORDER BY");
                        selSQL.Append(" SortNo asc");
                        selSQL.Append(",cast(Cd as int)");

                        cmd.CommandText = selSQL.ToString();

                        using (var dr = cmd.ExecuteReader())
                        {
                            List<Hospital> hospList = new List<Hospital>();

                            while (dr.Read())
                            {
                                Hospital tmp = new Hospital();

                                tmp.Cd = dr["Cd"].ToString();
                                tmp.Name = dr["Name"].ToString();
                                tmp.Name_DB = dr["Name_DB"].ToString();
                                tmp.Name_Disp = dr["Name_Disp"].ToString();
                                tmp.IsCopy = dr["IsCopy"].ToString();
                                tmp.SortNo = Convert.ToInt32(dr["SortNo"]);

                                hospList.Add(tmp);
                            }

                            ret = hospList.ToArray();

                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogUtil.Error(e.Message);
            }

            return ret;
        }

        public Hospital[] GetHospital_Admin()
        {
            Hospital[] ret = null;

            try
            {
                using (var con = new TryDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        var selSQL = new StringBuilder();

                        selSQL.Append("SELECT");
                        selSQL.Append(" Cd");
                        selSQL.Append(",Name");
                        selSQL.Append(",Name_DB");
                        selSQL.Append(",Name_Disp");
                        selSQL.Append(",IsCopy");
                        selSQL.Append(",SortNo");
                        selSQL.Append(",Status");
                        selSQL.Append(" FROM");
                        selSQL.Append(" M_Hospital");
                        selSQL.Append(" ORDER BY");
                        selSQL.Append(" SortNo asc");
                        selSQL.Append(",cast(Cd as int)");

                        cmd.CommandText = selSQL.ToString();

                        using (var dr = cmd.ExecuteReader())
                        {
                            List<Hospital> hospList = new List<Hospital>();

                            while (dr.Read())
                            {
                                Hospital tmp = new Hospital();

                                tmp.Cd = dr["Cd"].ToString();
                                tmp.Name = dr["Name"].ToString();
                                tmp.Name_DB = dr["Name_DB"].ToString();
                                tmp.Name_Disp = dr["Name_Disp"].ToString();
                                tmp.IsCopy = dr["IsCopy"].ToString();
                                tmp.SortNo = Convert.ToInt32(dr["SortNo"]);
                                tmp.Status = dr["Status"].ToString();

                                hospList.Add(tmp);
                            }

                            ret = hospList.ToArray();

                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogUtil.Error(e.Message);
            }

            return ret;
        }

        public bool SetHospital(Hospital val)
        {
            bool ret = false;

            try
            {
                using (var con = new TryDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
                {
                    if (String.IsNullOrEmpty(val.Cd))
                    {
                        using (var cmd = con.CreateCommand())
                        {
                            var selSQL = new StringBuilder();

                            selSQL.Append("SELECT");
                            selSQL.Append(" MAX(cast(Cd as integer)) As max");
                            selSQL.Append(" FROM");
                            selSQL.Append(" M_Hospital");

                            cmd.CommandText = selSQL.ToString();

                            using (var dr = cmd.ExecuteReader())
                            {
                                if (dr.Read() && dr["max"] != DBNull.Value)
                                {
                                    val.Cd = (Convert.ToInt32(dr["max"]) + 1).ToString();
                                }
                                else
                                {
                                    val.Cd = "1";
                                }
                            }
                        }

                        using (var cmd = con.CreateCommand())
                        {
                            var insSQL = new StringBuilder();

                            insSQL.Append("INSERT");
                            insSQL.Append(" INTO");
                            insSQL.Append(" M_Hospital");
                            insSQL.Append(" (");
                            insSQL.Append(" Cd");
                            insSQL.Append(",Name");
                            insSQL.Append(",Name_DB");
                            insSQL.Append(",Name_Disp");
                            insSQL.Append(",IsCopy");
                            insSQL.Append(",SortNo");
                            insSQL.Append(" )");
                            insSQL.Append(" VALUES");
                            insSQL.Append(" (");
                            insSQL.Append(cmd.Add(val.Cd).ParameterName);
                            insSQL.Append("," + cmd.Add(val.Name).ParameterName);
                            insSQL.Append("," + cmd.Add(val.Name_DB).ParameterName);
                            insSQL.Append("," + cmd.Add(val.Name_Disp).ParameterName);
                            insSQL.Append("," + cmd.Add(val.IsCopy).ParameterName);
                            insSQL.Append("," + cmd.Add(val.SortNo).ParameterName);
                            insSQL.Append(" )");

                            cmd.CommandText = insSQL.ToString();

                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (var cmd = con.CreateCommand())
                        {
                            var udtSQL = new StringBuilder();

                            udtSQL.Append("UPDATE");
                            udtSQL.Append(" M_Hospital");
                            udtSQL.Append(" SET");
                            udtSQL.Append(" Name=" + cmd.Add(val.Name).ParameterName);
                            udtSQL.Append(",Name_DB=" + cmd.Add(val.Name_DB).ParameterName);
                            udtSQL.Append(",Name_Disp=" + cmd.Add(val.Name_Disp).ParameterName);
                            udtSQL.Append(",IsCopy=" + cmd.Add(val.IsCopy).ParameterName);
                            udtSQL.Append(",SortNo=" + cmd.Add(val.SortNo).ParameterName);
                            udtSQL.Append(",Status=" + cmd.Add(val.Status).ParameterName);
                            udtSQL.Append(" WHERE");
                            udtSQL.Append(" Cd=" + cmd.Add(val.Cd).ParameterName);

                            cmd.CommandText = udtSQL.ToString();

                            cmd.ExecuteNonQuery();
                        }
                    }

                }
                ret = true;
            }
            catch (Exception e)
            {
                LogUtil.Error(e.Message);
            }

            return ret;
        }

        public Doctor[] GetDoctor()
        {
            Doctor[] ret = null;

            try
            {
                using (var con = new TryDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        var selSQL = new StringBuilder();

                        selSQL.Append("SELECT");
                        selSQL.Append(" Cd");
                        selSQL.Append(",Name");
                        selSQL.Append(",Name_Disp");
                        selSQL.Append(",IsCost");
                        selSQL.Append(",IsLisence");
                        selSQL.Append(",PayType");
                        selSQL.Append(",IsVisible");
                        selSQL.Append(" FROM");
                        selSQL.Append(" M_Doctor");
                        selSQL.Append(" WHERE");
                        selSQL.Append(" Status = 0");
                        selSQL.Append(" ORDER BY");
                        selSQL.Append(" Name");

                        cmd.CommandText = selSQL.ToString();

                        using (var dr = cmd.ExecuteReader())
                        {
                            List<Doctor> list = new List<Doctor>();

                            while (dr.Read())
                            {
                                Doctor tmp = new Doctor();

                                tmp.Cd = dr["Cd"].ToString();
                                tmp.Name = dr["Name"].ToString();
                                tmp.Name_Disp = dr["Name_Disp"].ToString();
                                tmp.IsCost = dr["IsCost"].ToString();
                                tmp.IsLisence = dr["IsLisence"].ToString();
                                tmp.PayType = dr["PayType"].ToString();
                                tmp.IsVisible = dr["IsVisible"].ToString();

                                list.Add(tmp);
                            }

                            ret = list.ToArray();

                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogUtil.Error(e.Message);
            }

            return ret;
        }

        public bool SetDoctor(Doctor val)
        {
            bool ret = false;

            try
            {
                using (var con = new TryDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
                {
                    if (String.IsNullOrEmpty(val.Cd))
                    {
                        using (var cmd = con.CreateCommand())
                        {
                            var selSQL = new StringBuilder();

                            selSQL.Append("SELECT");
                            selSQL.Append(" MAX(cast(Cd as integer)) As max");
                            selSQL.Append(" FROM");
                            selSQL.Append(" M_Doctor");

                            cmd.CommandText = selSQL.ToString();

                            using (var dr = cmd.ExecuteReader())
                            {
                                List<Doctor> list = new List<Doctor>();

                                if (dr.Read() && dr["max"] != DBNull.Value)
                                {
                                    val.Cd = (Convert.ToInt32(dr["max"]) + 1).ToString();
                                }
                                else
                                {
                                    val.Cd = "1";
                                }
                            }
                        }

                        using (var cmd = con.CreateCommand())
                        {
                            var insSQL = new StringBuilder();

                            insSQL.Append("INSERT");
                            insSQL.Append(" INTO");
                            insSQL.Append(" M_Doctor");
                            insSQL.Append(" (");
                            insSQL.Append(" Cd");
                            insSQL.Append(",Name");
                            insSQL.Append(",Name_Disp");
                            insSQL.Append(",IsCost");
                            insSQL.Append(",IsLisence");
                            insSQL.Append(",PayType");
                            insSQL.Append(",IsVisible");
                            insSQL.Append(" )");
                            insSQL.Append(" VALUES");
                            insSQL.Append(" (");
                            insSQL.Append(cmd.Add(val.Cd).ParameterName);
                            insSQL.Append("," + cmd.Add(val.Name).ParameterName);
                            insSQL.Append("," + cmd.Add(val.Name_Disp).ParameterName);
                            insSQL.Append("," + cmd.Add(val.IsCost).ParameterName);
                            insSQL.Append("," + cmd.Add(val.IsLisence).ParameterName);
                            insSQL.Append("," + cmd.Add(val.PayType).ParameterName);
                            insSQL.Append("," + cmd.Add(val.IsVisible).ParameterName);
                            insSQL.Append(" )");

                            cmd.CommandText = insSQL.ToString();

                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (var cmd = con.CreateCommand())
                        {
                            var udtSQL = new StringBuilder();

                            udtSQL.Append("UPDATE");
                            udtSQL.Append(" M_Doctor");
                            udtSQL.Append(" SET");
                            udtSQL.Append(" Name=" + cmd.Add(val.Name).ParameterName);
                            udtSQL.Append(",Name_Disp=" + cmd.Add(val.Name_Disp).ParameterName);
                            udtSQL.Append(",IsCost=" + cmd.Add(val.IsCost).ParameterName);
                            udtSQL.Append(",IsLisence=" + cmd.Add(val.IsLisence).ParameterName);
                            udtSQL.Append(",PayType=" + cmd.Add(val.PayType).ParameterName);
                            udtSQL.Append(",IsVisible=" + cmd.Add(val.IsVisible).ParameterName);
                            udtSQL.Append(",Status=" + cmd.Add(val.Status).ParameterName);
                            udtSQL.Append(" WHERE");
                            udtSQL.Append(" Cd=" + cmd.Add(val.Cd).ParameterName);

                            cmd.CommandText = udtSQL.ToString();

                            cmd.ExecuteNonQuery();
                        }
                    }

                }
                ret = true;
            }
            catch (Exception e)
            {
                LogUtil.Error(e.Message);
            }

            return ret;
        }

        public Config[] GetSystemConfig()
        {
            Config[] ret = null;

            try
            {
                using (var con = new TryDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        var selSQL = new StringBuilder();

                        selSQL.Append("SELECT");
                        selSQL.Append(" Key");
                        selSQL.Append(",Value");
                        selSQL.Append(",Remarks");
                        selSQL.Append(" FROM");
                        selSQL.Append(" M_SystemConfig");

                        cmd.CommandText = selSQL.ToString();

                        using (var dr = cmd.ExecuteReader())
                        {
                            List<Config> list = new List<Config>();

                            while (dr.Read())
                            {
                                Config tmp = new Config();

                                tmp.Key = dr["Key"].ToString();
                                tmp.Value = dr["Value"].ToString();
                                tmp.Remarks = dr["Remarks"].ToString();

                                list.Add(tmp);
                            }

                            ret = list.ToArray();

                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogUtil.Error(e.Message);
            }

            return ret;

        }

        public bool SetSystemConfig(Config[] confs)
        {
            bool ret = false;

            try
            {
                using (var con = new TryDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
                {
                    con.BeginTransaction();

                    using (var cmd = con.CreateCommand())
                    {
                        var insSQL = new StringBuilder();

                        insSQL.Append("DELETE");
                        insSQL.Append(" FROM");
                        insSQL.Append(" M_SystemConfig");

                        cmd.CommandText = insSQL.ToString();

                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = con.CreateCommand())
                    {
                        var insSQL = new StringBuilder();

                        insSQL.Append("INSERT");
                        insSQL.Append(" INTO");
                        insSQL.Append(" M_SystemConfig");
                        insSQL.Append(" (");
                        insSQL.Append(" Key");
                        insSQL.Append(",Value");
                        insSQL.Append(",Remarks");
                        insSQL.Append(" )");
                        insSQL.Append(" VALUES");

                        for (int i = 0; i < confs.Length; i++ )
                        {
                            if(i > 0)
                                insSQL.Append(" ,");

                            insSQL.Append(" (");
                            insSQL.Append(" " + cmd.Add(confs[i].Key).ParameterName);
                            insSQL.Append("," + cmd.Add(confs[i].Value).ParameterName);
                            insSQL.Append("," + cmd.Add(confs[i].Remarks).ParameterName);
                            insSQL.Append(" )");
                        }

                        cmd.CommandText = insSQL.ToString();

                        cmd.ExecuteNonQuery();
                    }
                    con.Commit();
                }

                ret = true;
            }
            catch (Exception e)
            {
                LogUtil.Error(e.Message);
            }

            return ret;
        }

        public Config[] GetHospitalConfig(string Cd)
        {
            Config[] ret = null;

            try
            {
                using (var con = new TryDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        var selSQL = new StringBuilder();

                        selSQL.Append("SELECT");
                        selSQL.Append(" Key");
                        selSQL.Append(",Value");
                        selSQL.Append(",Remarks");
                        selSQL.Append(" FROM");
                        selSQL.Append(" M_HospitalConfig");
                        selSQL.Append(" WHERE");
                        selSQL.Append(" Cd=" + cmd.Add(Cd).ParameterName);

                        cmd.CommandText = selSQL.ToString();

                        using (var dr = cmd.ExecuteReader())
                        {
                            List<Config> list = new List<Config>();

                            while (dr.Read())
                            {
                                Config tmp = new Config();

                                tmp.Key = dr["Key"].ToString();
                                tmp.Value = dr["Value"].ToString();
                                tmp.Remarks = dr["Remarks"].ToString();

                                list.Add(tmp);
                            }

                            ret = list.ToArray();

                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogUtil.Error(e.Message);
            }

            return ret;

        }

        public bool SetHospitalConfig(string cd, Config[] confs)
        {
            bool ret = false;

            try
            {
                using (var con = new TryDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
                {
                    con.BeginTransaction();

                    using (var cmd = con.CreateCommand())
                    {
                        var insSQL = new StringBuilder();

                        insSQL.Append("DELETE");
                        insSQL.Append(" FROM");
                        insSQL.Append(" M_HospitalConfig");
                        insSQL.Append(" WHERE");
                        insSQL.Append(" Cd=" + cmd.Add(cd).ParameterName);

                        cmd.CommandText = insSQL.ToString();

                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = con.CreateCommand())
                    {
                        var insSQL = new StringBuilder();

                        insSQL.Append("INSERT");
                        insSQL.Append(" INTO");
                        insSQL.Append(" M_HospitalConfig");
                        insSQL.Append(" (");
                        insSQL.Append(" Cd");
                        insSQL.Append(",Key");
                        insSQL.Append(",Value");
                        insSQL.Append(",Remarks");
                        insSQL.Append(" )");
                        insSQL.Append(" VALUES");

                        for (int i = 0; i < confs.Length; i++)
                        {
                            if (i > 0)
                                insSQL.Append(" ,");

                            insSQL.Append(" (");
                            insSQL.Append(" " + cmd.Add(cd).ParameterName);
                            insSQL.Append("," + cmd.Add(confs[i].Key).ParameterName);
                            insSQL.Append("," + cmd.Add(confs[i].Value).ParameterName);
                            insSQL.Append("," + cmd.Add(confs[i].Remarks).ParameterName);
                            insSQL.Append(" )");
                        }

                        cmd.CommandText = insSQL.ToString();

                        cmd.ExecuteNonQuery();
                    }
                    con.Commit();
                }

                ret = true;
            }
            catch (Exception e)
            {
                LogUtil.Error(e.Message);
            }

            return ret;
        }
        public bool DelReport(string cd, Report[] reports)
        {
            bool ret = false;

            try
            {
                using (var con = new TryDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
                {
                    con.BeginTransaction();

                    using (var cmd = con.CreateCommand())
                    {
                        var delSql = new StringBuilder();
                        delSql.Append("DELETE");
                        delSql.Append(" FROM");
                        delSql.Append(" T_Report");
                        delSql.Append(" WHERE");
                        //delSql.Append(" HCd=" + cmd.Add(cd).ParameterName);
                        //delSql.Append(" AND");
                        delSql.Append(" OrderNo IN (");

                        for (int i = 0; i < reports.Length; i++)
                        {
                            if (i > 0)
                                delSql.Append(",");

                            delSql.Append(cmd.Add(reports[i].OrderNo).ParameterName);
                        }

                        delSql.Append(" )");

                        cmd.CommandText = delSql.ToString();

                        cmd.ExecuteNonQuery();
                    }

                    con.Commit();
                }

                ret = true;
            }
            catch(Exception e)
            {
                LogUtil.Error(e.Message);
            }

            return ret;
        }

        public bool SetReport(string cd, Report[] reports)
        {
            bool ret = false;

            try
            {
                if (reports == null || reports.Length == 0)
                    return true;
                using (var con = new TryDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
                {
                    con.BeginTransaction();

                    using (var cmd = con.CreateCommand())
                    {
                        var delSql = new StringBuilder();
                        delSql.Append("DELETE");
                        delSql.Append(" FROM");
                        delSql.Append(" T_Report");
                        delSql.Append(" WHERE");
                        //delSql.Append(" HCd=" + cmd.Add(cd).ParameterName);
                        //delSql.Append(" AND");
                        delSql.Append(" OrderNo IN (");

                        for (int i = 0; i < reports.Length; i++ )
                        {
                            if (i > 0)
                                delSql.Append(",");

                            delSql.Append(cmd.Add(reports[i].OrderNo).ParameterName);
                        }

                        delSql.Append(" )");

                        cmd.CommandText = delSql.ToString();

                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = con.CreateCommand())
                    {
                        var insSQL = new StringBuilder();

                        insSQL.Append("INSERT");
                        insSQL.Append(" INTO");
                        insSQL.Append(" T_Report");
                        insSQL.Append(" (");
                        insSQL.Append(" HCd");
                        insSQL.Append(",OrderNo");
                        insSQL.Append(",PatID");
                        insSQL.Append(",PatName");
                        insSQL.Append(",StudyDate");
                        insSQL.Append(",Modality");
                        insSQL.Append(",BodyPart");
                        insSQL.Append(",ReadDate");
                        insSQL.Append(",Department");
                        insSQL.Append(",PhysicianName");
                        insSQL.Append(",ReadCd");
                        insSQL.Append(",ImageCnt");
                        insSQL.Append(",OrderDetail");
                        insSQL.Append(",Contact");
                        insSQL.Append(",Accept");
                        insSQL.Append(",Comment");
                        insSQL.Append(",PriorityFlg");
                        insSQL.Append(",IntroFlg");
                        insSQL.Append(",BodyPartFlg");
                        insSQL.Append(",AddImageFlg");
                        insSQL.Append(",MailFlg");
                        insSQL.Append(",AddMGFlg");
                        insSQL.Append(",ClaimFlg");
                        insSQL.Append(",PayFlg");
                        insSQL.Append(",Memo");
                        insSQL.Append(" )");

                        insSQL.Append(" VALUES");

                        for (int i = 0; i < reports.Length; i++)
                        {
                            if (i > 0)
                                insSQL.Append(",");

                            insSQL.Append(" (");
                            insSQL.Append(cmd.Add(reports[i].HCd).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].OrderNo).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].PatID).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].PatName).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].StudyDate).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].Modality).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].BodyPart).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].ReadDate).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].Department).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].PhysicianName).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].ReadCd).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].ImageCnt).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].OrderDetail).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].Contact).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].Accept).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].Comment).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].PriorityFlg).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].IntroFlg).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].BodyPartFlg).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].AddImageFlg).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].MailFlg).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].AddMGFlg).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].ClaimFlg).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].PayFlg).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].Memo).ParameterName); 
                            insSQL.Append(" )");
                        }

                        cmd.CommandText = insSQL.ToString();

                        cmd.ExecuteNonQuery();
                    }

                    con.Commit();
                }

                ret = true;
            }
            catch(Exception e)
            {
                LogUtil.Error(e.Message);
            }

            return ret;
        }

        public Report[] GetReport(string cd, string start, string end, string where)
        {
            Report[] ret = null;

            try
            {
                using (var con = new TryDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        var selSQL = new StringBuilder();

                        selSQL.Append("SELECT");
                        selSQL.Append(" HCd");
                        selSQL.Append(",OrderNo");
                        selSQL.Append(",PatID");
                        selSQL.Append(",PatName");
                        selSQL.Append(",StudyDate");
                        selSQL.Append(",Modality");
                        selSQL.Append(",BodyPart");
                        selSQL.Append(",ReadDate");
                        selSQL.Append(",Department");
                        selSQL.Append(",PhysicianName");
                        selSQL.Append(",ReadCd");
                        selSQL.Append(",ImageCnt");
                        selSQL.Append(",OrderDetail");
                        selSQL.Append(",Contact");
                        selSQL.Append(",Accept");
                        selSQL.Append(",Comment");
                        selSQL.Append(",PriorityFlg");
                        selSQL.Append(",IntroFlg");
                        selSQL.Append(",BodyPartFlg");
                        selSQL.Append(",AddImageFlg");
                        selSQL.Append(",MailFlg");
                        selSQL.Append(",AddMGFlg");
                        selSQL.Append(",ClaimFlg");
                        selSQL.Append(",PayFlg");
                        selSQL.Append(",Memo");
                        selSQL.Append(" FROM");
                        selSQL.Append(" T_Report");
                        selSQL.Append(" WHERE");
                        selSQL.Append(" ReadDate >= " + cmd.Add(start).ParameterName);
                        selSQL.Append(" AND");
                        selSQL.Append(" ReadDate <= " + cmd.Add(end).ParameterName);
                        if (!string.IsNullOrEmpty(cd))
                        {
                            selSQL.Append(" AND");
                            selSQL.Append(" HCd=" + cmd.Add(cd).ParameterName);
                        }
                        if (!string.IsNullOrEmpty(where))
                        {
                            selSQL.Append(" " + where);
                        }

                        selSQL.Append(" ORDER BY");
                        selSQL.Append(" ReadDate");

                        cmd.CommandText = selSQL.ToString();

                        using (var dr = cmd.ExecuteReader())
                        {
                            List<Report> hospList = new List<Report>();

                            while (dr.Read())
                            {
                                Report tmp = new Report();

                                tmp.HCd = dr["HCd"].ToString();
                                tmp.OrderNo = dr["OrderNo"].ToString();
                                tmp.PatID = dr["PatID"].ToString();
                                tmp.PatName = dr["PatName"].ToString();
                                tmp.StudyDate = dr["StudyDate"].ToString();
                                tmp.Modality = dr["Modality"].ToString();
                                tmp.BodyPart = dr["BodyPart"].ToString();
                                tmp.ReadDate = dr["ReadDate"].ToString();
                                tmp.Department = dr["Department"].ToString();
                                tmp.PhysicianName = dr["PhysicianName"].ToString();
                                tmp.ReadCd = dr["ReadCd"].ToString();
                                tmp.ImageCnt = Convert.ToInt32(dr["ImageCnt"]);
                                tmp.OrderDetail = dr["OrderDetail"].ToString();
                                tmp.Contact = dr["Contact"].ToString();
                                tmp.Accept = dr["Accept"].ToString();
                                tmp.Comment = dr["Comment"].ToString();
                                tmp.PriorityFlg = Convert.ToInt32(dr["PriorityFlg"]);
                                tmp.IntroFlg = Convert.ToInt32(dr["IntroFlg"]);
                                tmp.BodyPartFlg = Convert.ToInt32(dr["BodyPartFlg"]);
                                tmp.AddImageFlg = Convert.ToInt32(dr["AddImageFlg"]);
                                tmp.MailFlg = Convert.ToInt32(dr["MailFlg"]);
                                tmp.AddMGFlg = Convert.ToInt32(dr["AddMGFlg"]);
                                tmp.ClaimFlg = Convert.ToInt32(dr["ClaimFlg"]);
                                tmp.PayFlg = Convert.ToInt32(dr["PayFlg"]);
                                tmp.Memo = dr["Memo"].ToString();

                                hospList.Add(tmp);
                            }

                            ret = hospList.ToArray();

                        }
                    }
                }
            }
            catch(Exception e)
            {
                LogUtil.Error(e.Message);
            }

            return ret;
        }

        public bool SetReportHist(string cd, Report[] reports)
        {
            bool ret = false;

            try
            {
                if (reports == null || reports.Length == 0)
                    return true;

                using (var con = new TryDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
                {
                    con.BeginTransaction();

                    using (var cmd = con.CreateCommand())
                    {
                        var delSql = new StringBuilder();
                        delSql.Append("DELETE");
                        delSql.Append(" FROM");
                        delSql.Append(" T_ReportHist");
                        delSql.Append(" WHERE");
                        //delSql.Append(" HCd=" + cmd.Add(cd).ParameterName);
                        //delSql.Append(" AND");
                        delSql.Append(" OrderNo IN (");

                        for (int i = 0; i < reports.Length; i++)
                        {
                            if (i > 0)
                                delSql.Append(",");

                            delSql.Append(cmd.Add(reports[i].OrderNo).ParameterName);
                        }

                        delSql.Append(" )");

                        cmd.CommandText = delSql.ToString();

                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = con.CreateCommand())
                    {
                        var insSQL = new StringBuilder();

                        insSQL.Append("INSERT");
                        insSQL.Append(" INTO");
                        insSQL.Append(" T_ReportHist");
                        insSQL.Append(" (");
                        insSQL.Append(" HCd");
                        insSQL.Append(",OrderNo");
                        insSQL.Append(",PatID");
                        insSQL.Append(",PatName");
                        insSQL.Append(",StudyDate");
                        insSQL.Append(",Modality");
                        insSQL.Append(",BodyPart");
                        insSQL.Append(",ReadDate");
                        insSQL.Append(",Department");
                        insSQL.Append(",PhysicianName");
                        insSQL.Append(",ReadCd");
                        insSQL.Append(",ImageCnt");
                        insSQL.Append(",OrderDetail");
                        insSQL.Append(",Contact");
                        insSQL.Append(",Accept");
                        insSQL.Append(",Comment");
                        insSQL.Append(",PriorityFlg");
                        insSQL.Append(",IntroFlg");
                        insSQL.Append(",BodyPartFlg");
                        insSQL.Append(",AddImageFlg");
                        insSQL.Append(",MailFlg");
                        insSQL.Append(",AddMGFlg");
                        insSQL.Append(",ClaimFlg");
                        insSQL.Append(",PayFlg");
                        insSQL.Append(",Memo");
                        insSQL.Append(" )");

                        insSQL.Append(" VALUES");

                        for (int i = 0; i < reports.Length; i++)
                        {
                            if (i > 0)
                                insSQL.Append(",");

                            insSQL.Append(" (");
                            insSQL.Append(cmd.Add(reports[i].HCd).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].OrderNo).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].PatID).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].PatName).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].StudyDate).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].Modality).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].BodyPart).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].ReadDate).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].Department).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].PhysicianName).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].ReadCd).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].ImageCnt).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].OrderDetail).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].Contact).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].Accept).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].Comment).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].PriorityFlg).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].IntroFlg).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].BodyPartFlg).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].AddImageFlg).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].MailFlg).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].AddMGFlg).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].ClaimFlg).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].PayFlg).ParameterName);
                            insSQL.Append("," + cmd.Add(reports[i].Memo).ParameterName);
                            insSQL.Append(" )");
                        }

                        cmd.CommandText = insSQL.ToString();

                        cmd.ExecuteNonQuery();
                    }

                    con.Commit();
                }

                ret = true;
            }
            catch (Exception e)
            {
                LogUtil.Error(e.Message);
            }

            return ret;
        }

        public Report[] GetReportHist(string cd, string start, string end, string where)
        {
            Report[] ret = null;

            try
            {
                using (var con = new TryDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        var selSQL = new StringBuilder();

                        selSQL.Append("SELECT");
                        selSQL.Append(" HCd");
                        selSQL.Append(",OrderNo");
                        selSQL.Append(",PatID");
                        selSQL.Append(",PatName");
                        selSQL.Append(",StudyDate");
                        selSQL.Append(",Modality");
                        selSQL.Append(",BodyPart");
                        selSQL.Append(",ReadDate");
                        selSQL.Append(",Department");
                        selSQL.Append(",PhysicianName");
                        selSQL.Append(",ReadCd");
                        selSQL.Append(",ImageCnt");
                        selSQL.Append(",OrderDetail");
                        selSQL.Append(",Contact");
                        selSQL.Append(",Accept");
                        selSQL.Append(",Comment");
                        selSQL.Append(",PriorityFlg");
                        selSQL.Append(",IntroFlg");
                        selSQL.Append(",BodyPartFlg");
                        selSQL.Append(",AddImageFlg");
                        selSQL.Append(",MailFlg");
                        selSQL.Append(",AddMGFlg");
                        selSQL.Append(",ClaimFlg");
                        selSQL.Append(",PayFlg");
                        selSQL.Append(",Memo");
                        selSQL.Append(" FROM");
                        selSQL.Append(" T_ReportHist");
                        selSQL.Append(" WHERE");
                        selSQL.Append(" ReadDate >= " + cmd.Add(start).ParameterName);
                        selSQL.Append(" AND");
                        selSQL.Append(" ReadDate <= " + cmd.Add(end).ParameterName);
                        if (!string.IsNullOrEmpty(cd))
                        {
                            selSQL.Append(" AND");
                            selSQL.Append(" HCd=" + cmd.Add(cd).ParameterName);
                        }
                        if (!string.IsNullOrEmpty(where))
                        {
                            selSQL.Append(" " + where);
                        }

                        selSQL.Append(" ORDER BY");
                        selSQL.Append(" ReadDate");

                        cmd.CommandText = selSQL.ToString();

                        using (var dr = cmd.ExecuteReader())
                        {
                            List<Report> hospList = new List<Report>();

                            while (dr.Read())
                            {
                                Report tmp = new Report();

                                tmp.HCd = dr["HCd"].ToString();
                                tmp.OrderNo = dr["OrderNo"].ToString();
                                tmp.PatID = dr["PatID"].ToString();
                                tmp.PatName = dr["PatName"].ToString();
                                tmp.StudyDate = dr["StudyDate"].ToString();
                                tmp.Modality = dr["Modality"].ToString();
                                tmp.BodyPart = dr["BodyPart"].ToString();
                                tmp.ReadDate = dr["ReadDate"].ToString();
                                tmp.Department = dr["Department"].ToString();
                                tmp.PhysicianName = dr["PhysicianName"].ToString();
                                tmp.ReadCd = dr["ReadCd"].ToString();
                                tmp.ImageCnt = Convert.ToInt32(dr["ImageCnt"]);
                                tmp.OrderDetail = dr["OrderDetail"].ToString();
                                tmp.Contact = dr["Contact"].ToString();
                                tmp.Accept = dr["Accept"].ToString();
                                tmp.Comment = dr["Comment"].ToString();
                                tmp.PriorityFlg = Convert.ToInt32(dr["PriorityFlg"]);
                                tmp.IntroFlg = Convert.ToInt32(dr["IntroFlg"]);
                                tmp.BodyPartFlg = Convert.ToInt32(dr["BodyPartFlg"]);
                                tmp.AddImageFlg = Convert.ToInt32(dr["AddImageFlg"]);
                                tmp.MailFlg = Convert.ToInt32(dr["MailFlg"]);
                                tmp.AddMGFlg = Convert.ToInt32(dr["AddMGFlg"]);
                                tmp.ClaimFlg = Convert.ToInt32(dr["ClaimFlg"]);
                                tmp.PayFlg = Convert.ToInt32(dr["PayFlg"]);
                                tmp.Memo = dr["Memo"].ToString();

                                hospList.Add(tmp);
                            }

                            ret = hospList.ToArray();

                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogUtil.Error(e.Message);
            }

            return ret;
        }

        public bool SetImageCnt(string cd, Report val)
        {
            bool ret = false;
            try
            {
                using (var con = new TryDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        var udtSQL = new StringBuilder();

                        udtSQL.Append("UPDATE");
                        udtSQL.Append(" T_REPORT");
                        udtSQL.Append(" SET");
                        udtSQL.Append(" Contact=" + cmd.Add(val.Contact).ParameterName);
                        //udtSQL.Append(" ImageCnt=" + cmd.Add(val.ImageCnt).ParameterName);
                        //udtSQL.Append(",AddImageFlg=" + cmd.Add(val.AddImageFlg).ParameterName);
                        //udtSQL.Append(",MailFlg=" + cmd.Add(val.MailFlg).ParameterName);
                        //udtSQL.Append(",Accept=" + cmd.Add(val.Accept).ParameterName);
                        //udtSQL.Append(",BodyPartFlg=" + cmd.Add(val.BodyPartFlg).ParameterName);
                        udtSQL.Append(" WHERE");
                        udtSQL.Append(" HCd=" + cmd.Add(cd).ParameterName);
                        udtSQL.Append(" AND");
                        udtSQL.Append(" OrderNo=" + cmd.Add(val.OrderNo).ParameterName);

                        cmd.CommandText = udtSQL.ToString();

                        cmd.ExecuteNonQuery();
                    }
                    using (var cmd = con.CreateCommand())
                    {
                        var udtSQL = new StringBuilder();

                        udtSQL.Append("UPDATE");
                        udtSQL.Append(" T_ReportHist");
                        udtSQL.Append(" SET");
                        udtSQL.Append(" Contact=" + cmd.Add(val.Contact).ParameterName);
                        //udtSQL.Append(" ImageCnt=" + cmd.Add(val.ImageCnt).ParameterName);
                        //udtSQL.Append(",AddImageFlg=" + cmd.Add(val.AddImageFlg).ParameterName);
                        //udtSQL.Append(",MailFlg=" + cmd.Add(val.MailFlg).ParameterName);
                        //udtSQL.Append(",Accept=" + cmd.Add(val.Accept).ParameterName);
                        //udtSQL.Append(",BodyPartFlg=" + cmd.Add(val.BodyPartFlg).ParameterName);
                        udtSQL.Append(" WHERE");
                        udtSQL.Append(" HCd=" + cmd.Add(cd).ParameterName);
                        udtSQL.Append(" AND");
                        udtSQL.Append(" OrderNo=" + cmd.Add(val.OrderNo).ParameterName);

                        cmd.CommandText = udtSQL.ToString();

                        cmd.ExecuteNonQuery();
                    }
                    ret = true;
                }
            }
            catch(Exception e)
            {
                LogUtil.Error(e.Message);
            }
            return ret;
        }

        public string[] GetReportOrderNos(string cd, string start, string end)
        {
            string[] ret = null;

            try
            {
                using (var con = new TryDbConnection(ConfigurationManager.ConnectionStrings["DBRemote"]))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        var selSQL = new StringBuilder();

                        selSQL.Append("SELECT");
                        selSQL.Append(" OrderNo");
                        selSQL.Append(" FROM");
                        selSQL.Append(" T_ReportHist");
                        selSQL.Append(" WHERE");
                        selSQL.Append(" HCd=" + cmd.Add(cd).ParameterName);
                        selSQL.Append(" AND");
                        selSQL.Append(" ReadDate >= " + cmd.Add(start).ParameterName);
                        selSQL.Append(" AND");
                        selSQL.Append(" ReadDate <= " + cmd.Add(end).ParameterName);

                        cmd.CommandText = selSQL.ToString();

                        using (var dr = cmd.ExecuteReader())
                        {
                            List<string> hospList = new List<string>();

                            while (dr.Read())
                            {
                                hospList.Add(dr["OrderNo"].ToString());
                            }

                            ret = hospList.ToArray();

                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogUtil.Error(e.Message);
            }

            return ret;
        }

        /*☆★↓↓↓2016/06/08↓↓↓★☆*/
        #region 修正前
        //public Report[] GetReport_Org(string start, string end, string conStr)
        //{
        //    Report[] ret = null;

        //    try
        //    {
        //        string sHosp = ConfigurationManager.AppSettings["ImageSpecial"];

        //        using (var con = new TryDbConnection(conStr, "System.Data.SqlClient"))
        //        {
        //            using (var cmd = con.CreateCommand())
        //            {
        //                var selSQL = new StringBuilder();

        //                selSQL.Append("SELECT");
        //                selSQL.Append(" Distinct");
        //                selSQL.Append(" AccountTbl.ReportReserve1");
        //                selSQL.Append(",AccountTbl.ReportReserve3");
        //                selSQL.Append(",AccountTbl.ReportReserve4");
        //                selSQL.Append(",ReportTbl.ReportReserve7");
        //                selSQL.Append(",AccountTbl.Modality ");
        //                selSQL.Append(",AccountTbl.OrderNo");
        //                selSQL.Append(",AccountTbl.PatientID");
        //                selSQL.Append(",AccountTbl.PatientName");
        //                selSQL.Append(",AccountTbl.StudyDate");
        //                selSQL.Append(",AccountTbl.ReadDate");
        //                selSQL.Append(",AccountTbl.StudyBodyPart");
        //                selSQL.Append(",AccountTbl.PriorityFlag");
        //                selSQL.Append(",AccountTbl.PriorityFlag");
        //                selSQL.Append(",AccountTbl.PhysicianName");
        //                selSQL.Append(",AccountTbl.PriorityFlag");
        //                selSQL.Append(",AccountTbl.ReadPhysicianName");
        //                selSQL.Append(",AccountTbl.ImageAdvice");
        //                selSQL.Append(",AccountTbl.PriorityFlag");
        //                selSQL.Append(",AccountTbl.ReportReserve4 AS HpIntro");
        //                selSQL.Append(",ReportTbl.Department");
        //                selSQL.Append(",ReportTbl.Comment2");
        //                selSQL.Append(",ReportTbl.Comment3");
        //                selSQL.Append(",AccountInfoTbl.InfoCD");
        //                selSQL.Append(" FROM");
        //                selSQL.Append(" ( ProRadRSWebDB.dbo.ReportTbl ReportTbl");
        //                selSQL.Append(" INNER JOIN");
        //                selSQL.Append(" ProRadRSWebDB.dbo.AccountTbl AccountTbl");
        //                selSQL.Append(" ON");
        //                selSQL.Append(" ReportTbl.OrderNo = AccountTbl.OrderNo )");
        //                selSQL.Append(" LEFT JOIN");
        //                selSQL.Append(" ProRadRSWebDB.dbo.AccountInfoTbl AccountInfoTbl");
        //                selSQL.Append(" ON");
        //                selSQL.Append(" AccountInfoTbl.OrderNo = AccountTbl.OrderNo");
        //                selSQL.Append(" WHERE");
        //                selSQL.Append("(");
        //                selSQL.Append("(");
        //                selSQL.Append(" AccountTbl.ReadDate >= " + cmd.Add(start).ParameterName);
        //                selSQL.Append(")");
        //                selSQL.Append(" AND");
        //                selSQL.Append("(");
        //                selSQL.Append(" AccountTbl.ReadDate <= " + cmd.Add(end).ParameterName);
        //                selSQL.Append(")");
        //                selSQL.Append(")");
        //                selSQL.Append(" AND AccountTbl.Edition = 1");
        //                selSQL.Append(" ORDER BY AccountTbl.ReadDate asc");

        //                cmd.CommandText = selSQL.ToString();

        //                using (var dr = cmd.ExecuteReader())
        //                {
        //                    List<Report> hospList = new List<Report>();
        //                    List<string> keys = new List<string>();

        //                    while (dr.Read())
        //                    {
        //                        Report tmp = new Report();

        //                        tmp.HCd = dr["ReportReserve1"].ToString();
        //                        tmp.HName = dr["ReportReserve3"].ToString();
        //                        tmp.OrderNo = dr["OrderNo"].ToString();
        //                        tmp.PatID = dr["PatientID"].ToString();
        //                        tmp.PatName = dr["PatientName"].ToString();
        //                        tmp.StudyDate = dr["StudyDate"].ToString();
        //                        tmp.Modality = dr["Modality"].ToString();
        //                        tmp.BodyPart = dr["StudyBodyPart"].ToString();
        //                        tmp.ReadDate = dr["ReadDate"].ToString();
        //                        tmp.Department = dr["Department"].ToString();
        //                        tmp.PhysicianName = dr["PhysicianName"].ToString();
        //                        tmp.ReadCd = dr["ReadPhysicianName"].ToString();
        //                        tmp.Contact = dr["Comment2"].ToString();
        //                        tmp.Accept = dr["Comment3"].ToString();
        //                        if (dr["ImageAdvice"] == DBNull.Value || dr["ImageAdvice"].ToString() == "")
        //                            tmp.ImageCnt = 0;
        //                        else
        //                            tmp.ImageCnt = Convert.ToInt32(dr["ImageAdvice"]);

        //                        int icnt = 0;

        //                        if (sHosp.IndexOf(tmp.HCd) >= 0 && int.TryParse(dr["ReportReserve7"].ToString(), out icnt))
        //                            tmp.ImageCnt = icnt;

        //                        if(dr["HpIntro"] == DBNull.Value || dr["HpIntro"].ToString() == "")
        //                            tmp.IntroFlg = 0;
        //                        else
        //                            tmp.IntroFlg = Convert.ToInt32(dr["HpIntro"].ToString());

        //                        tmp.Comment = dr["Comment3"].ToString();

        //                        if (dr["PriorityFlag"] == DBNull.Value || dr["PriorityFlag"].ToString() == "")
        //                            tmp.PriorityFlg = 0;
        //                        else
        //                            tmp.PriorityFlg = Convert.ToInt32(dr["PriorityFlag"]);

        //                        if (dr["ReportReserve4"] == DBNull.Value || dr["ReportReserve4"].ToString() == "")
        //                            tmp.MailFlg = 0;
        //                        else
        //                            tmp.MailFlg = Convert.ToInt32(dr["ReportReserve4"]);

        //                        if (dr["InfoCD"] == DBNull.Value || dr["InfoCD"].ToString() == "")
        //                            tmp.AddMGFlg = 0;
        //                        else
        //                            tmp.AddMGFlg = Convert.ToInt32(dr["InfoCD"]);

        //                        if (tmp.Modality == "CT" || tmp.Modality == "MR")
        //                        {
        //                            if (keys.Contains(tmp.HCd + tmp.PatID + tmp.StudyDate + tmp.Modality))
        //                                tmp.BodyPartFlg = 1;
        //                            else
        //                                keys.Add(tmp.HCd + tmp.PatID + tmp.StudyDate + tmp.Modality);
        //                        }
        //                        hospList.Add(tmp);
        //                    }

        //                    ret = hospList.ToArray();

        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        LogUtil.Error(e.Message);
        //    }

        //    return ret;
        //}
        #endregion
        /*☆★AccountTbl,ReportTblから値取得★☆*/
        public Report[] GetReport_Org(string StartDate, string EndDate, string ConStr,string LevelCode)
        {
            Report[] ret = null;
            string dbName = string.Empty; /*DB名*/
    
            try{
                string sHosp = ConfigurationManager.AppSettings["ImageSpecial"];

                using(var con = new TryDbConnection(ConStr, "System.Data.SqlClient")){

                    dbName = con.Connection.Database; /*ProRadRSのDB名 aggregatedb.t_SystemConfigから*/

                    using(var cmd = con.CreateCommand()){
                        var selSQL = new StringBuilder();
                        selSQL.Append("SELECT");
                        selSQL.Append(" Distinct");

                        /*☆★↓↓↓2016/06/08 同一検査重複取得↓↓↓★☆*/
                        selSQL.Append(" AccountTbl.Edition,");       /*版数*/
                        selSQL.Append(" AccountTbl.ReadNumber,");    /*n次*/
                        /*☆★↑↑↑2016/06/08↑↑↑★☆*/

                        selSQL.Append(" AccountTbl.ReportReserve1"); /*事業所コード*/
                        selSQL.Append(",AccountTbl.ReportReserve3"); /*事業所名*/
                        selSQL.Append(",AccountTbl.ReportReserve4"); /*紹介フラグ*/
                        selSQL.Append(",ReportTbl.ReportReserve7");  /*Promed画像枚数*/
                        selSQL.Append(",AccountTbl.Modality ");
                        selSQL.Append(",AccountTbl.OrderNo");
                        selSQL.Append(",AccountTbl.PatientID");
                        selSQL.Append(",AccountTbl.PatientName");
                        selSQL.Append(",AccountTbl.StudyDate");
                        selSQL.Append(",AccountTbl.ReadDate");
                        selSQL.Append(",AccountTbl.StudyBodyPart");
                        selSQL.Append(",AccountTbl.PriorityFlag");              /*?*/
                        selSQL.Append(",AccountTbl.PriorityFlag");              /*?*/
                        selSQL.Append(",AccountTbl.PhysicianName");
                        selSQL.Append(",AccountTbl.PriorityFlag");              /*?*/
                        selSQL.Append(",AccountTbl.ReadPhysicianName");
                        selSQL.Append(",AccountTbl.ImageAdvice");
                        selSQL.Append(",AccountTbl.PriorityFlag");              /*?*/
                        selSQL.Append(",AccountTbl.ReportReserve4 AS HpIntro"); /*?*/
                        selSQL.Append(",ReportTbl.Department");
                        selSQL.Append(",ReportTbl.Comment2");        /*臨床診断*/
                        selSQL.Append(",ReportTbl.Comment3");        /*フリーコメント*/
                        selSQL.Append(",AccountInfoTbl.InfoCD");
                        selSQL.Append(" FROM");
                        selSQL.Append(" ( "+ dbName +".dbo.ReportTbl ReportTbl");
                        selSQL.Append(" INNER JOIN");
                        selSQL.Append(" "+ dbName +".dbo.AccountTbl AccountTbl");
                        selSQL.Append(" ON");
                        selSQL.Append(" ReportTbl.OrderNo = AccountTbl.OrderNo )");
                        selSQL.Append(" LEFT JOIN");
                        selSQL.Append(" "+ dbName +".dbo.AccountInfoTbl AccountInfoTbl");
                        selSQL.Append(" ON");
                        selSQL.Append(" AccountInfoTbl.OrderNo = AccountTbl.OrderNo");
                        selSQL.Append(" WHERE");
                        selSQL.Append("(");
                        selSQL.Append("(");
                        selSQL.Append(" AccountTbl.ReadDate >= " + cmd.Add(StartDate).ParameterName);
                        selSQL.Append(")");
                        selSQL.Append(" AND");
                        selSQL.Append("(");
                        selSQL.Append(" AccountTbl.ReadDate <= " + cmd.Add(EndDate).ParameterName);
                        selSQL.Append(")");
                        selSQL.Append(")");

                        /*☆★↓↓↓2016/06/08 Edition=0:1次確定↓↓↓★☆*/
                        //selSQL.Append(" AND AccountTbl.Edition = 1");
                        selSQL.Append(" AND (AccountTbl.Edition = 0 OR AccountTbl.Edition = 1)");
                        /*☆★↑↑↑2016/06/08↑↑↑★☆*/

                        selSQL.Append(" ORDER BY AccountTbl.ReadDate asc");

                        cmd.CommandText = selSQL.ToString();

                        using(var dr = cmd.ExecuteReader()){
                            List<Report> hospList = new List<Report>();
                            List<string> keys = new List<string>();

                            while(dr.Read()){
                                Report tmp = new Report();

                                tmp.HCd = dr["ReportReserve1"].ToString();
                                tmp.HName = dr["ReportReserve3"].ToString();

                                /*☆★↓↓↓2016/06/08 n次対応↓↓↓★☆*/
                                //tmp.OrderNo = dr["OrderNo"].ToString();
                                tmp.OrderNo = GetOrderNo(LevelCode,dr["OrderNo"].ToString(),dr["ReportReserve1"].ToString(),dr["ReadNumber"].ToString());
                                /*☆★↑↑↑2016/06/08↑↑↑★☆*/


                                tmp.PatID = dr["PatientID"].ToString();
                                tmp.PatName = dr["PatientName"].ToString();
                                tmp.StudyDate = dr["StudyDate"].ToString();
                                tmp.Modality = dr["Modality"].ToString();
                                tmp.BodyPart = dr["StudyBodyPart"].ToString();
                                tmp.ReadDate = dr["ReadDate"].ToString();
                                tmp.Department = dr["Department"].ToString();
                                tmp.PhysicianName = dr["PhysicianName"].ToString();
                                tmp.ReadCd = dr["ReadPhysicianName"].ToString();
                                tmp.Contact = dr["Comment2"].ToString();
                                tmp.Accept = dr["Comment3"].ToString();
                                if(dr["ImageAdvice"] == DBNull.Value || dr["ImageAdvice"].ToString() == ""){
                                    tmp.ImageCnt = 0;
                                }else{
                                    tmp.ImageCnt = Convert.ToInt32(dr["ImageAdvice"]);
                                }
                                int icnt = 0;

                                if(sHosp.IndexOf(tmp.HCd) >= 0 && int.TryParse(dr["ReportReserve7"].ToString(), out icnt)){
                                    tmp.ImageCnt = icnt;
                                }
                                else if(sHosp.IndexOf(tmp.HCd) >= 0)
                                {
                                    var tmpDat = dr["ReportReserve7"].ToString().Split('(');
                                    if(int.TryParse(tmpDat[0], out icnt))
                                        tmp.ImageCnt = icnt;
                                }
                                if (dr["HpIntro"] == DBNull.Value || dr["HpIntro"].ToString() == "" || dr["HpIntro"].ToString().Length > 10)
                                {
                                    tmp.IntroFlg = 0;
                                }else{
                                    tmp.IntroFlg = Convert.ToInt32(dr["HpIntro"].ToString());
                                }
                                tmp.Comment = dr["Comment3"].ToString();

                                if(dr["PriorityFlag"] == DBNull.Value || dr["PriorityFlag"].ToString() == ""){
                                    tmp.PriorityFlg = 0;
                                }else{
                                    tmp.PriorityFlg = Convert.ToInt32(dr["PriorityFlag"]);
                                }
                                if(dr["ReportReserve4"] == DBNull.Value || dr["ReportReserve4"].ToString() == "" || dr["ReportReserve4"].ToString().Length > 10)
                                {
                                    tmp.MailFlg = 0;
                                }else{
                                    tmp.MailFlg = Convert.ToInt32(dr["ReportReserve4"]);
                                }
                                if(dr["InfoCD"] == DBNull.Value || dr["InfoCD"].ToString() == ""){
                                    tmp.AddMGFlg = 0;
                                }else{
                                    tmp.AddMGFlg = Convert.ToInt32(dr["InfoCD"]);
                                }
                                if(tmp.Modality == "CT" || tmp.Modality == "MR"){
                                    if(keys.Contains(tmp.HCd + tmp.PatID + tmp.StudyDate + tmp.Modality)){
                                        tmp.BodyPartFlg = 1;
                                    }else{
                                        keys.Add(tmp.HCd + tmp.PatID + tmp.StudyDate + tmp.Modality);
                                    }
                                }
                                hospList.Add(tmp);
                            }
                            ret = hospList.ToArray();
                        }
                    }
                }
            }catch(Exception e){
                LogUtil.Error(e.Message);
            }
            return ret;
        }
        /// <summary>
        /// /*☆★OrderNoにn次を追加してt_reportに同一オーダー重複登録用★☆*/
        /// </summary>
        /// <param name="LevelCode">n次対象事業所コード</param>
        /// <param name="OrderNo">該当オーダー番号</param>
        /// <param name="Code">該当事業所コード</param>
        /// <param name="Level">n次</param>
        /// <returns>オーダー番号</returns>
        private string GetOrderNo(string LevelCode,string OrderNo,string Code ,string Level){
            string newOrderNo = OrderNo;
            if(LevelCode == Code){
                /*☆n次☆*/
                newOrderNo = newOrderNo +"_Lv" +Level;
            }
            return newOrderNo;
        }

        /*☆★↑↑↑2016/06/08↑↑↑★☆*/

        public ImageCntData[] GetImageCnt_Org(string start, string end, string conStr, string conPro)
        {
            ImageCntData[] ret = null;

            try
            {
                using (var con = new TryDbConnection(conStr, conPro))
                {
                    using (var cmd = con.CreateCommand())
                    {
                        start = start + " 00:00:00";
                        end = end + " 23:59:59";

                        var selSQL = new StringBuilder();

                        selSQL.Append("SELECT ");
                        selSQL.Append(" (stuimagesnum-stugspsnum) ");
                        selSQL.Append(" ,patientid ");
                        selSQL.Append(" ,to_char(studydate,'YYYYMMDD') ");
                        selSQL.Append(" ,to_char(studytime,'FM000000') ");
                        selSQL.Append(" ,modality ");
                        selSQL.Append(" FROM ");
                        selSQL.Append(" masterstudy ");
                        selSQL.Append(" WHERE ");
                        selSQL.Append(" insertdatetime BETWEEN ");
                        selSQL.Append(" TO_DATE('" + start + "', 'YYYY/MM/DD HH24:MI:SS')");
                        selSQL.Append(" AND TO_DATE('" + end + "', 'YYYY/MM/DD HH24:MI:SS')");

                        cmd.CommandText = selSQL.ToString();

                        using (var dr = cmd.ExecuteReader())
                        {
                            List<ImageCntData> hospList = new List<ImageCntData>();

                            while (dr.Read())
                            {
                                ImageCntData tmp = new ImageCntData();

                                tmp.ImageCnt = Convert.ToInt32(dr[0]);
                                tmp.PatID = dr[1].ToString();
                                tmp.StudyDate = dr[2].ToString();
                                tmp.Modality = dr[4].ToString();

                                hospList.Add(tmp);
                            }

                            ret = hospList.ToArray();

                        }

                        if (ret != null && ret.Length > 0)
                        {
                            LogUtil.Error(ret[0].PatID + "," + ret[0].ImageCnt + "," + ret[0].Modality + "," + ret[0].StudyDate);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogUtil.Error(e.Message);
            }

            return ret;
        }

    }
}
