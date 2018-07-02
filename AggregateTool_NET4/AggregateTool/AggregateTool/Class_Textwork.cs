using System.Collections.Generic;
using System.Text;
using System.IO;
namespace AggregateTool
{
    class Class_Textwork
    {
        //設定クラス
        Class_Setting setting = new Class_Setting();
      
        /// <summary>
        /// テキスト読み取りのメインメソッド
        /// </summary>
        public List<Dictionary<string, string>> ReadText(Class_SearchParam searchParam,out Class_SearchResult se)
        {
            string line = "";
            List<Dictionary<string, string>> addList = new List<Dictionary<string, string>>();
            se = new Class_SearchResult();

            //ストリームライターでテキストファイルを読み取り
            using (StreamReader sr = new StreamReader(setting.readFilePath,
                                                      Encoding.GetEncoding("shift_jis")))
            {
                //読み取ったテキストの行数だけ処理
                while ((line = sr.ReadLine()) != null)
                {
                    //列をスプリットして配列に保存
                    string[] value = line.Split(',');
                    addList.Add(ToDictionary(value, out se));
                }
                //ストリームライターをクローズ
                sr.Close();
            }
            return GetSearchValue(addList, searchParam);
        }

        /// <summary>
        /// 配列をDictionaryに変換する
        /// </summary>
        private Dictionary<string, string> ToDictionary(string[] values, out Class_SearchResult se)
        {
            Dictionary<string, string> dv = new Dictionary<string, string>();
            se = new Class_SearchResult();
            foreach (string value in values)
            {
                if (string.IsNullOrEmpty(value))
                    continue;
                string dicKey = value.Substring(0, value.IndexOf(":"));
                string dicValue = value.Substring(value.IndexOf(":") + 1);
                dv.Add(dicKey, dicValue);
                se.AddColumnValue(dicKey);
            }
            return dv;
        }

        /// <summary>
        /// テキスト出力
        /// </summary>
        public void CreateText(Class_SearchParam updataCase,List<string> list)
        {
            string line = "";

            //ストリームライターでテキストファイルを読み取り
            using (StreamReader sr = new StreamReader(setting.readFilePath,
                                                      Encoding.GetEncoding("shift_jis")))
            {
                //読み取ったテキストの行数だけ処理
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains(updataCase.searchDate))
                    {
                        if (line.Contains(updataCase.searchName))
                            continue;
                    }
                    list.Add(line);
                }
                //ストリームライターをクローズ
                sr.Close();
            }
            File.Delete(setting.readFilePath);

            foreach(string value in list)
            {
                WriteText(value);
            }
        }

        /// <summary>
        /// テキスト書き込み
        /// </summary>
        private void WriteText(string addText)
        {
            StreamWriter sw = null;
            using (sw = new StreamWriter(setting.readFilePath, true, Encoding.GetEncoding("shift_jis")))
            {
                sw.WriteLine(addText);
            }

        }

        /// <summary>
        /// 検索内容との照らし合わせ
        /// </summary>
        private List<Dictionary<string,string>> GetSearchValue(List<Dictionary<string, string>> list,
            　　　　　　　　　　　　　　　　　　　　　　　　　 Class_SearchParam searchParam)
        {
            int count = 0;
            List<Dictionary<string,string>> newList = new List<Dictionary<string,string>>();

            for (int i = 0; i < list.Count ; i++ )
            {
                Dictionary<string,string> row = list[i];
                if (!row["InspectionDate"].Equals(searchParam.searchDate))
                    continue;
              　else if (!row["Office"].Equals(searchParam.searchName))
                    continue;
                newList.Add(list[i]);
                count++;
            }
            return newList;
        }
    }
}
