using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AggregateTool
{
    class Class_TextWokForExcel
    {
        private Class_Setting setting = new Class_Setting();
        public Class_SearchParam search = new Class_SearchParam();
        /// <summary>
        /// エクセル書き出し用テキスト読み出しメソッド
        /// </summary>
        /// <returns></returns>
        public Class_ModalityValue CreatStringForExcel()
        {
            string line = "";
            Class_ModalityValue mv = new Class_ModalityValue();
            using (var streamReader = new StreamReader(setting.readFilePath, Encoding.GetEncoding("shift_jis")))
            {
                //読み取ったテキストの行数だけ処理
                while ((line = streamReader.ReadLine()) != null)
                {
                    string[] value = line.Split(',');
                    Dictionary<string, string> dc = ToDictionary(value);
                    if (dc == null)
                        continue;
                    mv.SetData(dc);
                }
                //ストリームライターをクローズ
                streamReader.Close();
            }
            return mv;
        }

        /// <summary>
        /// 配列をDictionaryに変換する
        /// </summary>
        private Dictionary<string, string> ToDictionary(string[] values)
        {
            Dictionary<string, string> dv = new Dictionary<string, string>();
            foreach (string value in values)
            {
                if (string.IsNullOrEmpty(value))
                    continue;
                string dicKey = value.Substring(0, value.IndexOf(":"));
                string dicValue = value.Substring(value.IndexOf(":") + 1);
                if (!GetTrueDate(dicKey, dicValue))
                {
                    dv = null;
                    break;
                }

                dv.Add(dicKey, dicValue);
            }

            return dv;
        }

        private bool GetTrueDate(string key,string value)
        {
            bool result = true;
            if (key.Equals("InspectionDate"))
            {
                if(!value.Contains(search.searchDate))
                    result = false;
            }

            if (key.Equals("Office"))
            {
                if (!value.Equals(search.searchName))
                    result = false;
            }
            return result;
        }
    }
}
