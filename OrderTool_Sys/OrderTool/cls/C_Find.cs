using OrderTool.QRService;
using System.Configuration;

namespace OrderTool.cls
{
    public static class C_Find
    {
        public static DicomQRClient Client = new DicomQRClient();
        public static DicomQRNode QRSet = new DicomQRNode();

        public static void Setting()
        {
            QRSet.QSCPAETitle = ConfigurationManager.AppSettings["SCPAETitle"];
            QRSet.QSCPAddress = ConfigurationManager.AppSettings["SCPAddress"];

            int tmpInt = 0;

            if(int.TryParse(ConfigurationManager.AppSettings["SCPPort"], out tmpInt))
                QRSet.ServicePort = (ushort)tmpInt;
            if (int.TryParse(ConfigurationManager.AppSettings["PDULength"], out tmpInt))
                QRSet.MaxPDULen = (uint)tmpInt;
            if (int.TryParse(ConfigurationManager.AppSettings["ReceiveTimeout"], out tmpInt))
                QRSet.ReceiveTimeout = (uint)tmpInt;
            if (int.TryParse(ConfigurationManager.AppSettings["ReceiveTimeout"], out tmpInt))
                QRSet.ArtimTimeout = (uint)tmpInt;

            QRSet.QSCUAETitle = ConfigurationManager.AppSettings["SCUAETitle"];
        }


        public static int Echo()
        {
            int ret = 0;

            ret = Client.EchoEx(QRSet);

            return ret;
        }

        public static DicomQRItem[] C_FindFromStudy(string patid, string studydate, string accNo, string mod, string uid)
        {
            DicomQRItem[] ret = null;

            ret = Client.StudyFindEx(QRSet, "", patid, studydate, accNo, "", uid, mod);

            return ret;
        }

        public static DicomQRItem[] C_FindFromSeries(string studyUID)
        {
            DicomQRItem[] ret = null;

            ret = Client.SeriesFindEx(QRSet, studyUID, "", "", "");

            return ret;
        }

        public static DicomQRItem[] C_FindFromImages(string studyUID, string sereiesUID)
        {
            DicomQRItem[] ret = null;

            ret = Client.ImageFindEx(QRSet, studyUID, sereiesUID, "");

            return ret;
        }
    }
}