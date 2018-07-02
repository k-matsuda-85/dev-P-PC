using System.Configuration;
using System.IO;
namespace AggregateTool
{
    public class Class_Setting
    {
        /// <summary>
        /// テキストファイルパス
        /// </summary>
        private readonly string _readFilePath;
        public string readFilePath
        {
            get{ return this._readFilePath; }
        }

        /// <summary>
        /// 出力フォルダパス
        /// </summary>
        private readonly string _outPutPath;
        public string outPutPath
        {
            get { return this._outPutPath; }
        }

        /// <summary>
        /// マスタファイルパス
        /// </summary>
        private readonly string _masterPath;
        public string masterPath
        {
            get { return this._masterPath; }
        }
        /// <summary>
        /// 病院リスト
        /// </summary>
        private readonly string[] _hospitalList;
        public string[] hospitalList
        {
            get { return this._hospitalList; }
        }

        /// <summary>
        /// 検索列の「支払有無、請求有無」の表示非表示を設定する。
        /// </summary>
        private readonly string[] _positionGridBtn;
        public string[] positionGridBtn
        {
            get { return this._positionGridBtn; }
        }

        //Appconfigで設定された、マスターの要素を配列に変換。
        public readonly string[] masterElement;
        //集計行のタイトル
        public readonly string titleParam;
        //合計処理を行うカラム
        public readonly string[] sumParam;
        //カウント処理を行うカラム
        public readonly string countParam;
        //業の高さ
        public readonly int rowHeight;
        //集計行の色指定
        public readonly int cellColor;
        //Excelの書き込み開始列
        public readonly int startRow;
        //Excelの開始カラム
        public readonly int startColumn;
        //Excelの行間
        public readonly int betweenLine;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Class_Setting()
        {
            _masterPath = Path.GetFullPath(ConfigurationManager.AppSettings["MASTERFILE"]);
            _outPutPath = Path.GetFullPath(ConfigurationManager.AppSettings["OUTPUT"]);
            //int型へParse、失敗した場合は規定値
            if (!int.TryParse(ConfigurationManager.AppSettings["HEIGHT"], out rowHeight))
                rowHeight = 10;
            if (!int.TryParse(ConfigurationManager.AppSettings["STARTROW"], out startRow))
                startRow = 2;
            if (!int.TryParse(ConfigurationManager.AppSettings["STARTCOLUMN"], out startColumn))
                startColumn = 1;
            if (!int.TryParse(ConfigurationManager.AppSettings["BETWEEN"], out betweenLine))
                betweenLine = 3;
            cellColor = 15;
            //合計処理を行うカラム
            sumParam = ConfigurationManager.AppSettings["SUM"].Split(',');
            //カラムの最大数
            masterElement = ConfigurationManager.AppSettings["MASTERELEMENTS"].Split(',');
            //カウント処理を行うカラム
            countParam = ConfigurationManager.AppSettings["COUNT"];
            //カウント処理を行うカラム
            titleParam = ConfigurationManager.AppSettings["COUNT"];
        }
    }
}
