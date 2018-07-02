using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace OrderTool_Reception_Serv.Util
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
}
