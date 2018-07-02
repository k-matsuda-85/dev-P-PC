using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace OrderTool_Serv.Class
{
    /// <summary>
    /// 共通返却クラス
    /// </summary>
    [DataContract]
    public class ResultData
    {
        /// <summary>処理結果</summary>
        [DataMember]
        public bool Result { get; set; }
        /// <summary>処理内容（例外時のみ）</summary>
        [DataMember]
        public string Message { get; set; }

        public ResultData()
        {
            this.Result = false;
            this.Message = "";
        }
    }

    /// <summary>
    /// 設定基底クラス
    /// </summary>
    [DataContract]
    public class Config
    {
        /// <summary>設定キー</summary>
        [DataMember]
        public string Key { get; set; }
        /// <summary>設定値</summary>
        [DataMember]
        public string Value { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Config()
        {
            this.Key = "";
            this.Value = "";
        }
    }

    /// <summary>
    /// システム設定マスタクラス
    /// </summary>
    [DataContract]
    public class SystemConfig
    {
        /// <summary>設定基底クラスの配列</summary>
        [DataMember]
        public Config[] Conf { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SystemConfig()
        {
            this.Conf = new List<Config>().ToArray();
        }

        /// <summary>
        /// 項目追加
        /// </summary>
        /// <param name="key">
        /// 項目キー
        /// </param>
        /// <param name="val">
        /// 項目値
        /// </param>
        public void Add(string key, string val)
        {
            List<Config> tmpList = new List<Config>();
            Config tmpConf = new Config();

            tmpConf.Key = key;
            tmpConf.Value = val;

            if (this.Conf != null)
                tmpList = this.Conf.ToList();

            tmpList.Add(tmpConf);

            this.Conf = tmpList.ToArray();
        }
    }

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
    /// ユーザ設定クラス
    /// </summary>
    [DataContract]
    public class UserConfig : SystemConfig
    {
        [DataMember]
        public int UserID { get; set; }
    }

    /// <summary>
    /// ユーザマスタクラス
    /// </summary>
    [DataContract]
    public class UserMst
    {
        /// <summary>ユーザーID</summary>
        [DataMember]
        public int UserID { get; set; }

        /// <summary>ユーザー名</summary>
        [DataMember]
        public string UserName { get; set; }
        
        /// <summary>ログインID</summary>
        [DataMember]
        public string LoginID { get; set; }
        
        /// <summary>パスワード</summary>
        [DataMember]
        public string LoginPW { get; set; }
        
        /// <summary>権限</summary>
        [DataMember]
        public int Permission { get; set; }

        /// <summary>コンストラクタ</summary>
        public UserMst()
        {
            this.UserID = 0;
            this.UserName = "";
            this.LoginID = "";
            this.LoginPW = "";
            this.Permission = 0;
        }
    }

    /// <summary>
    /// ログイン時返却クラス
    /// </summary>
    [DataContract]
    public class Login
    {
        /// <summary>ユーザID</summary>
        [DataMember]
        public int UserID;

        /// <summary>ログインセッションキー</summary>
        [DataMember]
        public string Key;

        /// <summary>コンストラクタ</summary>
        public Login()
        {
            this.UserID = 0;
            this.Key = "";
        }
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

    /// <summary>
    /// 事前登録用オーダークラス
    /// </summary>
    [DataContract]
    public class PreOrder : Patient
    {
        /// <summary>オーダー管理ID</summary>
        [DataMember]
        public int OrderID { get; set; }

        /// <summary>オーダー番号</summary>
        [DataMember]
        public string OrderNo { get; set; }

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

        /// <summary>状態</summary>
        [DataMember]
        public int Status { get; set; }

        /// <summary>コンストラクタ</summary>
        public PreOrder()
        {
            this.OrderID = 0;
            this.OrderNo = "";
            this.PatAge = 0;
            this.Modality = "";
            this.Date = "";
            this.Time = "";
            this.BodyPart = "";
            this.Type = "";
            this.ImgCnt = 0;
            this.IsEmergency = 0;
            this.IsMail = 0;
            this.Comment = "";
            this.Status = -1;
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

        /// <summary>コンストラクタ</summary>
        public Search()
        {
            this.OrderID = 0;
            this.PatID = "";
            this.Modality = "";
            this.Date_F = "";
            this.Date_T = "";
            this.HospID = 0;
            this.Status = -1;
        }

    }


}
