using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AggregateTool
{
    class Class_ColumnValue
    {
        //ヘッダーの名前から対応するint型を返す
        enum HeaderColumn
        {
            Office,
            InterpretationDate,
            InspectionDate,
            Modality,
            PatientId,
            PatientName,
            RequestDoctor,
            Parts,
            RequestContent,
            Contact,
            Acceptedonly,
            AddParts,
            Emergency,
            AddPic,
            NumberOfImager,
            FaxAndMail,
            AddMammaryGland,
            Payment,
            Claim,
            WorkDoctor,
            Memo,
        }

        public int GetEnumValue(string key)
        {
            int newkey = -1;
            switch (key)
            {
                case ("Office"):
                    newkey = (int)HeaderColumn.Office;
                    break;
                case ("InterpretationDate"):
                    newkey = (int)HeaderColumn.InterpretationDate;
                    break;
                case ("InspectionDate"):
                    newkey = (int)HeaderColumn.InspectionDate;
                    break;
                case ("Modality"):
                    newkey = (int)HeaderColumn.Modality;
                    break;
                case ("PatientId"):
                    newkey = (int)HeaderColumn.PatientId;
                    break;
                case ("PatientName"):
                    newkey = (int)HeaderColumn.PatientName;
                    break;
                case ("RequestDoctor"):
                    newkey = (int)HeaderColumn.RequestDoctor;
                    break;
                case ("Parts"):
                    newkey = (int)HeaderColumn.Parts;
                    break;
                case ("RequestContent"):
                    newkey = (int)HeaderColumn.RequestContent;
                    break;
                case ("Contact"):
                    newkey = (int)HeaderColumn.Contact;
                    break;
                case ("Acceptedonly"):
                    newkey = (int)HeaderColumn.Acceptedonly;
                    break;
                case ("AddParts"):
                    newkey = (int)HeaderColumn.AddParts;
                    break;
                case ("Emergency"):
                    newkey = (int)HeaderColumn.Emergency;
                    break;
                case ("AddPic"):
                    newkey = (int)HeaderColumn.AddPic;
                    break;
                case ("NumberOfImager"):
                    newkey = (int)HeaderColumn.NumberOfImager;
                    break;
                case ("FaxAndMail"):
                    newkey = (int)HeaderColumn.FaxAndMail;
                    break;
                case ("AddMammaryGland"):
                    newkey = (int)HeaderColumn.AddMammaryGland;
                    break;
                case ("Payment"):
                    newkey = (int)HeaderColumn.Payment;
                    break;
                case ("Claim"):
                    newkey = (int)HeaderColumn.Claim;
                    break;
                case ("WorkDoctor"):
                    newkey = (int)HeaderColumn.WorkDoctor;
                    break;
                case ("Memo"):
                    newkey = (int)HeaderColumn.Memo;
                    break;
            }
            return newkey;
        }
    }
}
