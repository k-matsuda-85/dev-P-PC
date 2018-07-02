using Agg_Serv.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AggregateTool
{
    public enum PayType
    {
        報酬 = 0,
        給与 = 1
    }

    public class HospDistMst
    {
        public Hospital Hospital = null;
        public string Cost = null;
        public string Dist = null;
        public HospMst[] Hosp = null;
        public ModarityMst[] Sheet = null;
        public string[] Yayoi = null;

        public string ParentCd = null;
    }

    public class HospMst
    {
        public string Target = null;
        public string Where = null;
    }
    public class ModarityMst
    {
        public string Sum = null;
        public string Target = null;
        public string Where = null;
        public string DocMod = "";
    }
    public class KaikeiMst
    {
        public int No = -1;
        public string[] Values = null;
    }

    public class ModReports
    {
        public string Mod = null;
        public string DocMod = null;
        public Report[] Reports = null;
        public int Count = 0;
    }

    public class ExtReports
    {
        public string Cd = null;
        public ModReports[] ModReports = null;
        public int Count = 0;
    }
}
