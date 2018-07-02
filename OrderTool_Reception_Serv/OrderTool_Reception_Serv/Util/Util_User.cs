using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OrderTool_Reception_Serv.Util
{
    /// <summary>
    /// ユーザ設定クラス
    /// </summary>
    [DataContract]
    public class UserConfig : SystemConfig
    {
        [DataMember]
        public int UserID { get; set; }
    }

    /// <summary>
    /// 施設マスタクラス
    /// </summary>
    [DataContract]
    public class UserMst
    {
        /// <summary>ユーザID/summary>
        [DataMember]
        public int UserID { get; set; }

        /// <summary>ユーザ名</summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>連携用文字</summary>
        [DataMember]
        public string CD { get; set; }

        /// <summary>並び順</summary>
        [DataMember]
        public int Seq { get; set; }

        /// <summary>表示</summary>
        [DataMember]
        public int Permission { get; set; }

        /// <summary>コンストラクタ</summary>
        public UserMst()
        {
            this.UserID = 0;
            this.Name = "";
            this.CD = "";
            this.Seq = 0;
            this.Permission = 0;
        }
    }
}
