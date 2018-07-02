using System.Runtime.Serialization;

namespace OrderTool_Reception_Serv.Util
{
    /// <summary>
    /// 施設設定クラス
    /// </summary>
    [DataContract]
    public class HospitalConfig : SystemConfig
    {
        [DataMember]
        public int HospID { get; set; }
    }

    /// <summary>
    /// 施設マスタクラス
    /// </summary>
    [DataContract]
    public class HospMst
    {
        /// <summary>施設ID/summary>
        [DataMember]
        public int HospID { get; set; }

        /// <summary>施設名</summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>施設コード</summary>
        [DataMember]
        public string CD { get; set; }

        /// <summary>並び順</summary>
        [DataMember]
        public int Seq { get; set; }

        /// <summary>表示</summary>
        [DataMember]
        public int Visible { get; set; }

        /// <summary>コンストラクタ</summary>
        public HospMst()
        {
            this.HospID = 0;
            this.Name = "";
            this.CD = "";
            this.Seq = 0;
            this.Visible = 0;
        }
    }

    /// <summary>
    /// 施設毎の候補値クラス
    /// </summary>
    [DataContract]
    public class HospitalTemplate
    {
        /// <summary>設定キー</summary>
        [DataMember]
        public string Key { get; set; }
        /// <summary>設定値</summary>
        [DataMember]
        public string Value { get; set; }
        /// <summary>並び順</summary>
        [DataMember]
        public int Index { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HospitalTemplate()
        {
            this.Key = "";
            this.Value = "";
            this.Index = 0;
        }
    }
}
