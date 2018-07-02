using MyAccDB;
using OrderTool_Reception_Serv.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderTool_Reception_Serv.Class
{
    class Orders
    {
        public static Order[] GetOrderList(Search search)
        {
            Order[] ret = null;

            // DB接続
            using (var con = new AccDbConnection(Conf._settings))
            {
                ret = getOrderList(con, search);
            }

            return ret;
        }

        public static bool SetOrder(Order order)
        {
            bool ret = false;

            // DB接続
            using (var con = new AccDbConnection(Conf._settings))
            {
                if (order.OrderID == 0)
                    ret = setOrder(con, order);
                else
                    ret = udtOrder(con, order);
            }

            return ret;
        }

        public static int SetOrder_RetId(Order order)
        {
            int ret = 0;

            // DB接続
            using (var con = new AccDbConnection(Conf._settings))
            {
                if (order.OrderID == 0)
                    ret = setOrder_RetId(con, order);
                else
                {
                    udtOrder(con, order);
                    ret = order.OrderID;
                }
            }

            return ret;
        }

        public static bool DelOrder(int orderid)
        {
            bool ret = false;

            // DB接続
            using (var con = new AccDbConnection(Conf._settings))
            {
                ret = delOrder(con, orderid);
            }

            return ret;
        }

        private static Order[] getOrderList(AccDbConnection con, Search search)
        {
            List<Order> ret = new List<Order>();

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                // SQL生成
                // ----------------------------
                // ----------------------------
                StringBuilder selSQL = new StringBuilder();
                selSQL.Append("SELECT");
                selSQL.Append(" O.id");
                selSQL.Append(",O.hospid");
                selSQL.Append(",O.orderno");
                selSQL.Append(",O.patkey");
                selSQL.Append(",O.patage");
                selSQL.Append(",O.modality");
                selSQL.Append(",O.studydate");
                selSQL.Append(",O.studytime");
                selSQL.Append(",O.bodypart");
                selSQL.Append(",O.studytype");
                selSQL.Append(",O.isvisit");
                selSQL.Append(",O.department");
                selSQL.Append(",O.orderdoctor");
                selSQL.Append(",O.imgcnt");
                selSQL.Append(",O.isemergency");
                selSQL.Append(",O.ismail");
                selSQL.Append(",O.comment");
                selSQL.Append(",O.contact");
                selSQL.Append(",O.recept");
                selSQL.Append(",O.status");
                selSQL.Append(",O.pre_id");
                selSQL.Append(",O.pastcnt");
                selSQL.Append(",O.updatedate");
                selSQL.Append(",P.key");
                selSQL.Append(",P.patid");
                selSQL.Append(",P.patname");
                selSQL.Append(",P.patname_h");
                selSQL.Append(",P.patname_r");
                selSQL.Append(",P.patsex");
                selSQL.Append(",P.patbirth");
                selSQL.Append(" FROM");
                selSQL.Append(" Orders As O");
                selSQL.Append(" INNER JOIN");
                selSQL.Append(" Patient As P");
                selSQL.Append(" ON");
                selSQL.Append(" O.patkey=P.Key");

                string strWhere = " WHERE";

                if (search.HospID > 0)
                {
                    selSQL.Append(strWhere);
                    selSQL.Append(" O.hospid=");
                    selSQL.Append(cmd.Add(search.HospID).ParameterName);

                    strWhere = " AND";
                }

                if (search.OrderID > 0)
                {
                    selSQL.Append(strWhere);
                    selSQL.Append(" O.id=");
                    selSQL.Append(cmd.Add(search.OrderID).ParameterName);

                    strWhere = " AND";
                }

                if (!string.IsNullOrEmpty(search.OrderNo))
                {
                    selSQL.Append(strWhere);
                    selSQL.Append(" O.orderno=");
                    selSQL.Append(cmd.Add(search.OrderNo).ParameterName);

                    strWhere = " AND";
                }

                if (!string.IsNullOrEmpty(search.PatID))
                {
                    selSQL.Append(strWhere);
                    selSQL.Append(" P.patid=");
                    selSQL.Append(cmd.Add(search.PatID).ParameterName);

                    strWhere = " AND";
                }

                if (!string.IsNullOrEmpty(search.Modality))
                {
                    selSQL.Append(strWhere);
                    selSQL.Append(" O.modality=");
                    selSQL.Append(cmd.Add(search.Modality).ParameterName);

                    strWhere = " AND";
                }

                if (!string.IsNullOrEmpty(search.Date_F))
                {
                    selSQL.Append(strWhere);
                    selSQL.Append(" O.studydate >= ");
                    selSQL.Append(cmd.Add(search.Date_F).ParameterName);

                    strWhere = " AND";
                }

                if (!string.IsNullOrEmpty(search.Date_T))
                {
                    selSQL.Append(strWhere);
                    selSQL.Append(" O.studydate <= ");
                    selSQL.Append(cmd.Add(search.Date_T).ParameterName);

                    strWhere = " AND";
                }

                if (search.Status != -2)
                {
                    selSQL.Append(strWhere);
                    selSQL.Append(" O.status=");
                    selSQL.Append(cmd.Add(search.Status).ParameterName);

                    strWhere = " AND";
                }
                if (search.AreaStatus != -2)
                {
                    selSQL.Append(strWhere);
                    selSQL.Append(" O.status <=");
                    selSQL.Append(cmd.Add(search.AreaStatus).ParameterName);

                    strWhere = " AND";
                }

                selSQL.Append(" ORDER BY");
                selSQL.Append(" (");
                selSQL.Append(" CASE o.status WHEN 1 THEN 1 ELSE 2 END");
                selSQL.Append(",CASE o.status WHEN 0 THEN 1 ELSE 2 END");
                selSQL.Append(",CASE o.status WHEN 2 THEN 1 ELSE 2 END");
                selSQL.Append(" )");
                selSQL.Append(",O.isemergency");
                selSQL.Append(" DESC");
                selSQL.Append(",O.updatedate");
                selSQL.Append(" ASC");

                cmd.CommandText = selSQL.ToString();

                // SQL実行
                using (var dr = cmd.ExecuteReader())
                    // 該当データがある場合、返却値を設定
                    while (dr.Read())
                    {
                        Order tmpOrder = new Order();

                        tmpOrder.OrderID = Convert.ToInt32(dr["id"]);

                        tmpOrder.HospID = Convert.ToInt32(dr["hospid"]);


                        tmpOrder.Key = Convert.ToInt32(dr["key"]);

                        tmpOrder.OrderNo = dr["orderno"].ToString();

                        tmpOrder.PatID = dr["patid"].ToString();

                        if (dr["patname"] != DBNull.Value)
                            tmpOrder.PatName = dr["patname"].ToString();

                        if (dr["patname_h"] != DBNull.Value)
                            tmpOrder.PatName_H = dr["patname_h"].ToString();

                        if (dr["patname_r"] != DBNull.Value)
                            tmpOrder.PatName_R = dr["patname_r"].ToString();

                        tmpOrder.Sex = Convert.ToInt32(dr["patsex"]);

                        if (dr["patbirth"] != DBNull.Value)
                            tmpOrder.BirthDay = dr["patbirth"].ToString();

                        if (dr["patage"] != DBNull.Value)
                            tmpOrder.PatAge = Convert.ToInt32(dr["patage"]);

                        tmpOrder.Modality = dr["modality"].ToString();

                        tmpOrder.Date = dr["studydate"].ToString();

                        if (dr["studytime"] != DBNull.Value)
                            tmpOrder.Time = dr["studytime"].ToString();

                        if (dr["bodypart"] != DBNull.Value)
                            tmpOrder.BodyPart = dr["bodypart"].ToString();

                        if (dr["studytype"] != DBNull.Value)
                            tmpOrder.Type = dr["studytype"].ToString();

                        if (dr["isvisit"] != DBNull.Value)
                            tmpOrder.IsVisit = dr["isvisit"].ToString();

                        if (dr["department"] != DBNull.Value)
                            tmpOrder.Department = dr["department"].ToString();

                        if (dr["orderdoctor"] != DBNull.Value)
                            tmpOrder.Doctor = dr["orderdoctor"].ToString();

                        if (dr["imgcnt"] != DBNull.Value)
                            tmpOrder.ImgCnt = Convert.ToInt32(dr["imgcnt"]);

                        tmpOrder.IsEmergency = Convert.ToInt32(dr["isemergency"]);

                        tmpOrder.IsMail = Convert.ToInt32(dr["ismail"]);

                        if (dr["comment"] != DBNull.Value)
                            tmpOrder.Comment = dr["comment"].ToString();

                        if (dr["contact"] != DBNull.Value)
                            tmpOrder.Contact = dr["contact"].ToString();

                        if (dr["recept"] != DBNull.Value)
                            tmpOrder.Recept = dr["recept"].ToString();

                        if (dr["status"] != DBNull.Value)
                            tmpOrder.Status = Convert.ToInt32(dr["status"]);

                        if (dr["pre_id"] != DBNull.Value)
                            tmpOrder.PreID = Convert.ToInt32(dr["pre_id"]);

                        tmpOrder.PastCnt = dr["pastcnt"].ToString();


                        ret.Add(tmpOrder);
                    }
            }

            return ret.ToArray();
        }

        private static bool setOrder(AccDbConnection con, Order order)
        {
            bool ret = false;

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                StringBuilder insSQL = new StringBuilder();
                insSQL.Append("INSERT");
                insSQL.Append(" INTO");
                insSQL.Append(" Orders");

                insSQL.Append(" (");
                insSQL.Append(" hospid");
                insSQL.Append(",orderno");
                insSQL.Append(",patkey");
                insSQL.Append(",patage");
                insSQL.Append(",modality");
                insSQL.Append(",studydate");
                insSQL.Append(",studytime");
                insSQL.Append(",bodypart");
                insSQL.Append(",studytype");
                insSQL.Append(",isvisit");
                insSQL.Append(",department");
                insSQL.Append(",orderdoctor");
                insSQL.Append(",imgcnt");
                insSQL.Append(",isemergency");
                insSQL.Append(",ismail");
                insSQL.Append(",comment");
                insSQL.Append(",contact");
                insSQL.Append(",recept");
                insSQL.Append(",status");
                insSQL.Append(",pre_id");
                insSQL.Append(",pastcnt");
                insSQL.Append(" )");

                insSQL.Append(" VALUES");

                insSQL.Append(" (");
                insSQL.Append(cmd.Add(order.HospID).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.OrderNo).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.Key).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.PatAge).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.Modality).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.Date).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.Time).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.BodyPart).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.Type).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.IsVisit).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.Department).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.Doctor).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.ImgCnt).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.IsEmergency).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.IsMail).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.Comment).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.Contact).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.Recept).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.Status).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.PreID).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.PastCnt).ParameterName);
                insSQL.Append(" )");

                cmd.CommandText = insSQL.ToString();

                var retcnt = cmd.ExecuteNonQuery();
                if (retcnt > 0)
                    ret = true;
            }

            return ret;
        }

        private static int setOrder_RetId(AccDbConnection con, Order order)
        {
            int ret = 0;

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                StringBuilder insSQL = new StringBuilder();
                insSQL.Append("INSERT");
                insSQL.Append(" INTO");
                insSQL.Append(" Orders");

                insSQL.Append(" (");
                insSQL.Append(" hospid");
                insSQL.Append(",orderno");
                insSQL.Append(",patkey");
                insSQL.Append(",patage");
                insSQL.Append(",modality");
                insSQL.Append(",studydate");
                insSQL.Append(",studytime");
                insSQL.Append(",bodypart");
                insSQL.Append(",studytype");
                insSQL.Append(",isvisit");
                insSQL.Append(",department");
                insSQL.Append(",orderdoctor");
                insSQL.Append(",imgcnt");
                insSQL.Append(",isemergency");
                insSQL.Append(",ismail");
                insSQL.Append(",comment");
                insSQL.Append(",contact");
                insSQL.Append(",recept");
                insSQL.Append(",status");
                insSQL.Append(",pre_id");
                insSQL.Append(",pastcnt");
                insSQL.Append(" )");

                insSQL.Append(" VALUES");

                insSQL.Append(" (");
                insSQL.Append(cmd.Add(order.HospID).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.OrderNo).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.Key).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.PatAge).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.Modality).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.Date).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.Time).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.BodyPart).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.Type).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.IsVisit).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.Department).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.Doctor).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.ImgCnt).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.IsEmergency).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.IsMail).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.Comment).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.Contact).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.Recept).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.Status).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.PreID).ParameterName);
                insSQL.Append(",");
                insSQL.Append(cmd.Add(order.PastCnt).ParameterName);
                insSQL.Append(" )");

                cmd.CommandText = insSQL.ToString();

                insSQL.Append(" RETURNING");
                insSQL.Append(" id");

                cmd.CommandText = insSQL.ToString();

                // SQL実行
                using (var dr = cmd.ExecuteReader())
                    // 該当データがある場合、返却値を設定
                    if (dr.Read())
                        ret = Convert.ToInt32(dr["id"]);
            }

            return ret;
        }

        private static bool udtOrder(AccDbConnection con, Order order)
        {
            bool ret = false;

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                StringBuilder udtSQL = new StringBuilder();
                udtSQL.Append("UPDATE");
                udtSQL.Append(" Orders");
                udtSQL.Append(" SET");
                udtSQL.Append(" hospid=" + cmd.Add(order.HospID).ParameterName);
                udtSQL.Append(",orderno=" + cmd.Add(order.OrderNo).ParameterName);
                udtSQL.Append(",patkey=" + cmd.Add(order.Key).ParameterName);
                udtSQL.Append(",patage=" + cmd.Add(order.PatAge).ParameterName);
                udtSQL.Append(",modality=" + cmd.Add(order.Modality).ParameterName);
                udtSQL.Append(",studydate=" + cmd.Add(order.Date).ParameterName);
                udtSQL.Append(",studytime=" + cmd.Add(order.Time).ParameterName);
                udtSQL.Append(",bodypart=" + cmd.Add(order.BodyPart).ParameterName);
                udtSQL.Append(",studytype=" + cmd.Add(order.Type).ParameterName);
                udtSQL.Append(",isvisit=" + cmd.Add(order.IsVisit).ParameterName);
                udtSQL.Append(",department=" + cmd.Add(order.Department).ParameterName);
                udtSQL.Append(",orderdoctor=" + cmd.Add(order.Doctor).ParameterName);
                udtSQL.Append(",isemergency=" + cmd.Add(order.IsEmergency).ParameterName);
                udtSQL.Append(",ismail=" + cmd.Add(order.IsMail).ParameterName);
                udtSQL.Append(",comment=" + cmd.Add(order.Comment).ParameterName);
                udtSQL.Append(",contact=" + cmd.Add(order.Contact).ParameterName);
                udtSQL.Append(",recept=" + cmd.Add(order.Recept).ParameterName);
                udtSQL.Append(",status=" + cmd.Add(order.Status).ParameterName);
                udtSQL.Append(" WHERE");
                udtSQL.Append(" id=");
                udtSQL.Append(cmd.Add(order.OrderID).ParameterName);

                cmd.CommandText = udtSQL.ToString();

                var retcnt = cmd.ExecuteNonQuery();
                if (retcnt > 0)
                    ret = true;
            }

            return ret;
        }

        private static bool delOrder(AccDbConnection con, int orderid)
        {
            bool ret = false;

            // コマンドオブジェクト生成
            using (var cmd = con.CreateCommand())
            {
                StringBuilder delSQL = new StringBuilder();
                delSQL.Append("DELETE");
                delSQL.Append(" FROM");
                delSQL.Append(" Orders");
                delSQL.Append(" WHERE");
                delSQL.Append(" id=");
                delSQL.Append(cmd.Add(orderid).ParameterName);

                cmd.CommandText = delSQL.ToString();

                cmd.ExecuteNonQuery();
                ret = true;
            }

            return ret;
        }

    }
}
