using System.Collections.Generic;

namespace AggregateTool
{
    public class Class_SearchResult
    {
        /// <summary>
        /// ヘッダーのリストを保持する
        /// </summary>
        private List<string> _columnValue = new List<string>();
        public List<string> columnValue
        {
            get { return _columnValue; }
        }
        /// <summary>
        /// ヘッダーを追加するメソッド
        /// </summary>
        public void AddColumnValue(string value)
        {
            _columnValue.Add(value);
        }

        /// <summary>
        /// 検索結果の列数
        /// </summary>
        private int _rowsnumber;
        public int rowsnumber
        {
            set{　_rowsnumber = value;　}
            get{　return _rowsnumber;　}
        }
    }
}
