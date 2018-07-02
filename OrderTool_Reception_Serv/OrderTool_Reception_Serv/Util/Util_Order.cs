using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OrderTool_Reception_Serv.Util
{
    /// <summary>
    /// 事前登録用オーダークラス
    /// </summary>
    [DataContract]
    public class Order : Patient
    {
        /// <summary>オーダー管理ID</summary>
        [DataMember]
        public int OrderID { get; set; }

        /// <summary>オーダー番号</summary>
        [DataMember]
        public string OrderNo { get; set; }

        /// <summary>状態</summary>
        [DataMember]
        public int Status { get; set; }

        /// <summary>患者年齢</summary>
        [DataMember]
        public int PatAge { get; set; }

        /// <summary>モダリティ</summary>
        [DataMember]
        public string Modality { get; set; }

        /// <summary>検査日</summary>
        [DataMember]
        public string Date { get; set; }

        /// <summary>検査時刻</summary>
        [DataMember]
        public string Time { get; set; }

        /// <summary>検査部位</summary>
        [DataMember]
        public string BodyPart { get; set; }

        /// <summary>単純/造影</summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>入院/外来</summary>
        [DataMember]
        public string IsVisit { get; set; }

        /// <summary>診療科</summary>
        [DataMember]
        public string Department { get; set; }

        /// <summary>依頼医</summary>
        [DataMember]
        public string Doctor { get; set; }

        /// <summary>画像枚数</summary>
        [DataMember]
        public int ImgCnt { get; set; }

        /// <summary>緊急フラグ</summary>
        [DataMember]
        public int IsEmergency { get; set; }

        /// <summary>メール通知フラグ</summary>
        [DataMember]
        public int IsMail { get; set; }

        /// <summary>依頼内容</summary>
        [DataMember]
        public string Comment { get; set; }

        /// <summary>連絡事項</summary>
        [DataMember]
        public string Contact { get; set; }

        /// <summary>受付専用</summary>
        [DataMember]
        public string Recept { get; set; }

        /// <summary>CL事前登録</summary>
        [DataMember]
        public int PreID { get; set; }

        /// <summary>過去画像枚数</summary>
        [DataMember]
        public string PastCnt { get; set; }

        /// <summary>コンストラクタ</summary>
        public Order()
        {
            this.OrderID = 0;
            this.OrderNo = "";
            this.PatAge = 0;
            this.Modality = "";
            this.Date = "";
            this.Time = "";
            this.BodyPart = "";
            this.Type = "";
            this.IsVisit = "";
            this.Department = "";
            this.Doctor = "";
            this.ImgCnt = 0;
            this.IsEmergency = 0;
            this.IsMail = 0;
            this.Comment = "";
            this.Contact = "";
            this.Recept = "";
            this.Status = -2;
            this.PreID = 0;
            this.PastCnt = "";
        }

    }

    /// <summary>
    /// 検索項目クラス
    /// </summary>
    [DataContract]
    public class Search
    {
        /// <summary>オーダー管理番号</summary>
        [DataMember]
        public int OrderID { get; set; }

        /// <summary>オーダー番号</summary>
        [DataMember]
        public string OrderNo { get; set; }

        /// <summary>患者ID</summary>
        [DataMember]
        public string PatID { get; set; }

        /// <summary>モダリティ</summary>
        [DataMember]
        public string Modality { get; set; }

        /// <summary>検査日(From)</summary>
        [DataMember]
        public string Date_F { get; set; }

        /// <summary>検査日(To)</summary>
        [DataMember]
        public string Date_T { get; set; }

        /// <summary>施設管理ID</summary>
        [DataMember]
        public int HospID { get; set; }

        /// <summary>状態</summary>
        [DataMember]
        public int Status { get; set; }

        /// <summary>状態（範囲指定）</summary>
        [DataMember]
        public int AreaStatus { get; set; }

        /// <summary>コンストラクタ</summary>
        public Search()
        {
            this.OrderID = 0;
            this.OrderNo = "";
            this.PatID = "";
            this.Modality = "";
            this.Date_F = "";
            this.Date_T = "";
            this.HospID = 0;
            this.Status = -2;
            this.AreaStatus = -2;
        }
    }

}
