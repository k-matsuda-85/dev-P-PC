using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OrderTool_Reception_Serv.Util
{
    /// <summary>
    /// 患者情報クラス
    /// </summary>
    [DataContract]
    public class Patient
    {
        /// <summary>患者キー</summary>
        [DataMember]
        public int Key { get; set; }

        /// <summary>施設ID</summary>
        [DataMember]
        public int HospID { get; set; }

        /// <summary>患者ID</summary>
        [DataMember]
        public string PatID { get; set; }

        /// <summary>患者名</summary>
        [DataMember]
        public string PatName { get; set; }

        /// <summary>患者名カナ</summary>
        [DataMember]
        public string PatName_H { get; set; }

        /// <summary>患者名ローマ</summary>
        [DataMember]
        public string PatName_R { get; set; }

        /// <summary>性別</summary>
        [DataMember]
        public int Sex { get; set; }

        /// <summary>生年月日</summary>
        [DataMember]
        public string BirthDay { get; set; }

        /// <summary>コンストラクタ</summary>
        public Patient()
        {
            this.PatID = "";
            this.PatName = "";
            this.PatName_H = "";
            this.PatName_R = "";
            this.Sex = 0;
            this.BirthDay = "";
        }
    }
}
