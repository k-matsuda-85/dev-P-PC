using System.Runtime.Serialization;

namespace OrderTool_Reception_Serv.Util
{
    /// <summary>
    /// 患者情報クラス
    /// </summary>
    [DataContract]
    public class Files
    {
        /// <summary>ファイルID</summary>
        [DataMember]
        public int FileID { get; set; }

        /// <summary>ｵｰﾀﾞｰID</summary>
        [DataMember]
        public int OrderID { get; set; }

        /// <summary>ファイル名</summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>シーケンス番号</summary>
        [DataMember]
        public int Seq { get; set; }

        /// <summary>原本フラグ</summary>
        [DataMember]
        public int IsOrigin { get; set; }

        /// <summary>コンストラクタ</summary>
        public Files()
        {
            this.FileID = 0;
            this.OrderID = 0;
            this.Name = "";
            this.Seq = 0;
            this.IsOrigin = 0;
        }
    }
}
