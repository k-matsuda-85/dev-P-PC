
namespace Agg_Serv.Class
{
    class Common
    {
       
    }

    public class Config
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Remarks { get; set; }
    }

    public class Hospital
    {
        public string Cd {get; set;}
        public string Name { get; set; }
        public string Name_DB { get; set; }
        public string Name_Disp { get; set; }
        public string IsCopy { get; set; }
        public int SortNo { get; set; }
        public string Status { get; set; }
    }
    public class User
    {
        public string Cd { get; set; }
        public string Name { get; set; }
        public string LoginID { get; set; }
        public string LoginPW { get; set; }
        public string IsAdmin { get; set; }
        public string Status { get; set; }
    }
    public class Doctor
    {
        public string Cd { get; set; }
        public string Name { get; set; }
        public string Name_Disp { get; set; }
        public string IsLisence { get; set; }
        public string IsCost { get; set; }
        public string PayType { get; set; }
        public string Status { get; set; }
        public string IsVisible { get; set; }
    }
    public class Report
    {
        public string HCd { get; set; }
        public string HName { get; set; }
        public string OrderNo { get; set; }
        public string PatID { get; set; }
        public string PatName { get; set; }
        public string StudyDate { get; set; }
        public string Modality { get; set; }
        public string BodyPart { get; set; }
        public string ReadDate { get; set; }
        public string Department { get; set; }
        public string PhysicianName { get; set; }
        public string ReadCd { get; set; }
        public int ImageCnt { get; set; }
        public string OrderDetail { get; set; }
        public string Contact { get; set; }
        public string Accept { get; set; }
        public string Comment { get; set; }
        public int PriorityFlg { get; set; }
        public int IntroFlg { get; set; }
        public int BodyPartFlg { get; set; }
        public int AddImageFlg { get; set; }
        public int MailFlg { get; set; }
        public int AddMGFlg { get; set; }
        public int ClaimFlg { get; set; }
        public int PayFlg { get; set; }
        public string Memo { get; set; }

        public Report()
        {
            this.HCd = "";
            this.HName = "";
            this.OrderNo = "";
            this.PatID = "";
            this.PatName = "";
            this.StudyDate = "";
            this.Modality = "";
            this.BodyPart = "";
            this.ReadDate = "";
            this.Department = "";
            this.PhysicianName = "";
            this.ReadCd = "";
            this.OrderDetail = "";
            this.Contact = "";
            this.Accept = "";
            this.Comment = "";
            this.Memo = ""; 
            this.ImageCnt = 0;
            this.PriorityFlg = 0;
            this.IntroFlg = 0;
            this.BodyPartFlg = 0;
            this.AddImageFlg = 0;
            this.MailFlg = 0;
            this.AddMGFlg = 0;
            this.ClaimFlg = 1;
            this.PayFlg = 1;
        }
    }
    public class ImageCntData
    {
        public string PatID { get; set; }
        public string StudyDate { get; set; }
        public string Modality { get; set; }
        public int ImageCnt { get; set; }

        public ImageCntData()
        {
            this.ImageCnt = 0;
        }
    }
}
