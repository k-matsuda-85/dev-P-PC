namespace AggregateTool
{
    public class Class_SearchParam
    {
        /// <summary>
        /// 検索を行う施設名
        /// </summary>
        private string _facilityName;
        public string searchName
        {
            get { return this._facilityName; }
            set { _facilityName = value; }
        }

        /// <summary>
        /// 検索を行う時間
        /// </summary>
        private string _seachDate;
        public string searchDate
        {
            get { return this._seachDate; }
            set { _seachDate = value; }
        }
    }
}