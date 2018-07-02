using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AggregateTool
{
    class Class_ModalityValue
    {
        //値があったモダリティの値リストを格納する
        public List<List<Dictionary<int, string>>> useModality 
                                                =  new List<List<Dictionary<int, string>>>();
        //モダリティ別にDataクラスを格納するリスト
        private List<Dictionary<int,string>> ct = new List<Dictionary<int,string>>();
        private List<Dictionary<int,string>> cr = new List<Dictionary<int,string>>();
        private List<Dictionary<int,string>> mr = new List<Dictionary<int,string>>();
        private List<Dictionary<int,string>> ri = new List<Dictionary<int,string>>();
        private List<Dictionary<int,string>> mg = new List<Dictionary<int,string>>();
        private List<Dictionary<int,string>> us = new List<Dictionary<int,string>>();
        private List<Dictionary<int,string>> ot = new List<Dictionary<int,string>>();

        //DataクラスのModality名毎にListへ追加を行う
        public void SetData(Dictionary<string,string> dic)
        {
            switch (GetModalityValue(dic))
            {
                case "CT":
                    ct.Add(ToIntStringDicitonary(dic));
                    break;
                case "CR":
                    cr.Add(ToIntStringDicitonary(dic));
                    break;
                case "MR":
                    mr.Add(ToIntStringDicitonary(dic));
                    break;
                case "RI":
                    ri.Add(ToIntStringDicitonary(dic));
                    break;
                case "MG":
                    mg.Add(ToIntStringDicitonary(dic));
                    break;
                case "US":
                    us.Add(ToIntStringDicitonary(dic));
                    break;
                case "OT":
                    ot.Add(ToIntStringDicitonary(dic));
                    break;
            }
        }
        /// <summary>
        /// 出力を行うListを作成するメソッド
        /// </summary>
        public void SetUseModality()
        {
            if(ct.Count > 0)
                useModality.Add(ct);
            if(cr.Count > 0)
                useModality.Add(cr);
            if(mr.Count > 0)
                useModality.Add(mr);
            if(ri.Count > 0)
                useModality.Add(ri);
            if(mg.Count > 0)
                useModality.Add(mg);
            if(us.Count > 0)
                useModality.Add(us);
            if(ot.Count > 0)
                useModality.Add(ot);
        }

        /// <summary>
        /// モダリティ毎に振り分ける
        /// </summary>
        private string GetModalityValue(Dictionary<string,string> value)
        {
            return value["Modality"];
        }

        /// <summary>
        /// string,stringのDictonaryからint,stringへ変更を行う
        /// </summary>
        private Dictionary<int,string> ToIntStringDicitonary(Dictionary<string,string> oldDic)
        {
            Dictionary<int, string> newDic = new Dictionary<int, string>();
            Class_ColumnValue cv = new Class_ColumnValue(); 
            foreach(string key in oldDic.Keys)
            {
                newDic.Add(cv.GetEnumValue(key),oldDic[key]);
            }
            return newDic;
        }
    }
}
