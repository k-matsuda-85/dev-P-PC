using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ProRadiRSService2
{
    // メモ: [リファクター] メニューの [名前の変更] コマンドを使用すると、コードと config ファイルの両方で同時にインターフェイス名 "IService1" を変更できます。
    [ServiceContract(Namespace = "")]
    public interface IProRadiRSService2
    {
        // TODO: ここにサービス操作を追加します。
        [OperationContract]
        bool AutoLoginCheck(string UserCD, out List<string[]> userinfo, out string retErrMsg);

        [OperationContract]
        bool GetSentenceData(int userCD, int userGroupCD, out List<string[]> SentenceList, out string retErrMsg);

        [OperationContract]
        bool GetChangeHistory(int userCD, int SerialNo, string imagePath, out List<string[]> HistoryDataList, out List<string[]> ImageDataList, out string retErrMsg);

        [OperationContract]
        bool SaveReport(int userCd, int serialNo, string finding, string diagnosing, int imgCnt);

        [OperationContract]
        bool SaveReportImage(string serialNo, string[] images);

        [OperationContract]
        bool UpdateSentence(string scd, string name, string val1, string val2, string pcd, string usercd);

        [OperationContract]
        string[] GetImageExt(int userCd, string serialNo, string imagePath);

        [OperationContract]
        bool DeleteSentence(string scd);

        [OperationContract]
        UserMst2[] GetUserList();
    }


    // サービス操作に複合型を追加するには、以下のサンプルに示すようにデータ コントラクトを使用します。

    [DataContract]
    public class HealthCheckupClass
    {
        public string mReportType = "";  //レポート種別
        public List<string> mShokenCD;   //所見コード
        public List<string> mShoken;     //所見
        public List<string> mHanteiCD;   //判定コード
        public List<string> mHantei;     //判定
        public string mShokenView = "";  //画面表示用所見
        public string mHanteiView = "";  //画面表示用判定
        public List<string> mFreeComment;//フリーコメント

        [DataMember]
        public string ReportType  //レポート種別
        {
            get { return mReportType; }
            set { mReportType = value; }
        }
        [DataMember]
        public List<string> ShokenCD   //所見コード
        {
            get { return mShokenCD; }
            set { mShokenCD = value; }
        }
        [DataMember]
        public List<string> Shoken     //所見
        {
            get { return mShoken; }
            set { mShoken = value; }
        }
        [DataMember]
        public List<string> HanteiCD   //判定コード
        {
            get { return mHanteiCD; }
            set { mHanteiCD = value; }
        }
        [DataMember]
        public List<string> Hantei     //判定
        {
            get { return mHantei; }
            set { mHantei = value; }
        }
        [DataMember]
        public string ShokenView  //画面表示用所見
        {
            get { return mShokenView; }
            set { mShokenView = value; }
        }
        [DataMember]
        public string HanteiView  //画面表示用判定
        {
            get { return mHanteiView; }
            set { mHanteiView = value; }
        }
        [DataMember]
        public List<string> FreeComment //フリーコメント
        {
            get { return mFreeComment; }
            set { mFreeComment = value; }
        }

    }

    [DataContract]
    public class RsHistoryTbl
    {
        public int mReadFlag = -1;
        public int mSerialNo = -1;

//        public int mReportStatus = 0;
//        public int mSubStatus = 0;
//        public int mReadingStatus = 0;
        public string mReadDate = "";
        public string mReadTime = "";
//        public string mReadDepartment = "";
        public string mReadPhysicianName = "";
//        public string mRequestedPhysicianName = "";
        public string mTranscriberName = "";
//        public string mRequestMessage = "";
//        public string mRequestHistory = "";
        public string mFinding = "";
//        public string mFindingRTF = "";
        public string mDiagnosing = "";
        //public string mDiagnosingRTF = "";
        //public string mPdfFile = "";
        //public int mImageUmu = 0;
        //public int mInfoUmu = 0;
        //public string mInfoList = "";
        //public string mHealthCheckup = "";
        //public int mImageAdvice = -1;
        //public DateTime mUpdateDate;
        //public int mEdition = 0;
        //public string mDiagnosisPhysicianName = "";  //診断医
        //public string mAuthorizationPhysicianName = "";  //承認医
        //public string mReadReserve1 = ""; //読影予備
        //public string mReadReserve2 = "";
        //public string mReadReserve3 = "";
        //public string mReadReserve4 = "";
        //public string mReadReserve5 = "";
        public int mHistoryNo = 0;
        //public string mPostscriptPhysicianName = "";
        //public int mReadOrder = 0;
        //public HealthCheckupClass mHealthCheckupCls = null;

        [DataMember]
        public int ReadFlag
        {
            set { mReadFlag = value; }
            get { return mReadFlag/*.Value*/; }
        }
        //[DataMember]
        //public bool ReadFlag_HasValue
        //{
        //    get { return mReadFlag.HasValue; }
        //}

        [DataMember]
        public int SerialNo
        {
            set { mSerialNo = value; }
            get { return mSerialNo/*.Value*/; }
        }

        //[DataMember]
        //public int ReportStatus
        //{
        //    set { mReportStatus = value; }
        //    get { return mReportStatus; }
        //}
        //[DataMember]
        //public int SubStatus
        //{
        //    set { mSubStatus = value; }
        //    get { return mSubStatus; }
        //}
        //[DataMember]
        //public int ReadingStatus
        //{
        //    set { mReadingStatus = value; }
        //    get { return mReadingStatus; }
        //}
        [DataMember]
        public string ReadDate
        {
            set { mReadDate = value; }
            get { return mReadDate; }
        }
        [DataMember]
        public string ReadTime
        {
            set { mReadTime = value; }
            get { return mReadTime; }
        }
        //[DataMember]
        //public string ReadDepartment
        //{
        //    set { mReadDepartment = value; }
        //    get { return mReadDepartment; }
        //}
        [DataMember]
        public string ReadPhysicianName
        {
            set { mReadPhysicianName = value; }
            get { return mReadPhysicianName; }
        }
        //[DataMember]
        //public string RequestedPhysicianName
        //{
        //    set { mRequestedPhysicianName = value; }
        //    get { return mRequestedPhysicianName; }
        //}
        [DataMember]
        public string TranscriberName
        {
            set { mTranscriberName = value; }
            get { return mTranscriberName; }
        }
        //[DataMember]
        //public string RequestMessage
        //{
        //    set { mRequestMessage = value; }
        //    get { return mRequestMessage; }
        //}
        //[DataMember]
        //public string RequestHistory
        //{
        //    set { mRequestHistory = value; }
        //    get { return mRequestHistory; }
        //}
        [DataMember]
        public string Finding
        {
            set { mFinding = value; }
            get { return mFinding; }
        }
        //[DataMember]
        //public string FindingRTF
        //{
        //    set { mFindingRTF = value; }
        //    get { return mFindingRTF; }
        //}
        [DataMember]
        public string Diagnosing
        {
            set { mDiagnosing = value; }
            get { return mDiagnosing; }
        }
        //[DataMember]
        //public string DiagnosingRTF
        //{
        //    set { mDiagnosingRTF = value; }
        //    get { return mDiagnosingRTF; }
        //}
        //[DataMember]
        //public string PdfFile
        //{
        //    set { mPdfFile = value; }
        //    get { return mPdfFile; }
        //}
        //[DataMember]
        //public int ImageUmu
        //{
        //    set { mImageUmu = value; }
        //    get { return mImageUmu; }
        //}
        //[DataMember]
        //public int InfoUmu
        //{
        //    set { mInfoUmu = value; }
        //    get { return mInfoUmu; }
        //}
        //[DataMember]
        //public string InfoList
        //{
        //    set { mInfoList = value; }
        //    get { return mInfoList; }
        //}
        //[DataMember]
        //public string HealthCheckup
        //{
        //    set { mHealthCheckup = value; }
        //    get { return mHealthCheckup; }
        //}
        //[DataMember]
        //public int ImageAdvice
        //{
        //    set { mImageAdvice = value; }
        //    get { return mImageAdvice; }
        //}
        //[DataMember]
        //public DateTime UpdateDate
        //{
        //    set { mUpdateDate = value; }
        //    get { return mUpdateDate; }
        //}
        //[DataMember]
        //public int Edition
        //{
        //    set { mEdition = value; }
        //    get { return mEdition; }
        //}
        //[DataMember]
        //public string DiagnosisPhysicianName  //診断医
        //{
        //    set { mDiagnosisPhysicianName = value; }
        //    get { return mDiagnosisPhysicianName; }
        //}
        //[DataMember]
        //public string AuthorizationPhysicianName  //承認医
        //{
        //    set { mAuthorizationPhysicianName = value; }
        //    get { return mAuthorizationPhysicianName; }
        //}
        //[DataMember]
        //public string ReadReserve1 //読影予備
        //{
        //    set { mReadReserve1 = value; }
        //    get { return mReadReserve1; }
        //}
        //[DataMember]
        //public string ReadReserve2
        //{
        //    set { mReadReserve2 = value; }
        //    get { return mReadReserve2; }
        //}
        //[DataMember]
        //public string ReadReserve3
        //{
        //    set { mReadReserve3 = value; }
        //    get { return mReadReserve3; }
        //}
        //[DataMember]
        //public string ReadReserve4
        //{
        //    set { mReadReserve4 = value; }
        //    get { return mReadReserve4; }
        //}
        //[DataMember]
        //public string ReadReserve5
        //{
        //    set { mReadReserve5 = value; }
        //    get { return mReadReserve5; }
        //}
        [DataMember]
        public int HistoryNo
        {
            set { mHistoryNo = value; }
            get { return mHistoryNo; }
        }
        //[DataMember]
        //public string PostscriptPhysicianName
        //{
        //    set { mPostscriptPhysicianName = value; }
        //    get { return mPostscriptPhysicianName; }
        //}
        //[DataMember]
        //public int ReadOrder
        //{
        //    set { mReadOrder = value; }
        //    get { return mReadOrder; }
        //}
        //[DataMember]
        //public HealthCheckupClass HealthCheckupCls
        //{
        //    set { mHealthCheckupCls = value; }
        //    get { return mHealthCheckupCls; }
        //}

    }

    // 画像テーブル
    [DataContract]
    public class RsImageTbl
    {
        public int mSeqNo = -1;            // SEQ番号
        public byte[] mImageData = null;   // 画像データ
        public string mImageExt = "";      // 画像拡張子
        public string mImageName = "";

        [DataMember]
        public int SeqNo        // SEQ番号
        {
            set { mSeqNo = value; }
            get { return mSeqNo; }
        }
        [DataMember]
        public byte[] ImageData // 画像データ
        {
            set { mImageData = value; }
            get { return mImageData; }
        }
        [DataMember]
        public string ImageExt  // 画像拡張子
        {
            set { mImageExt = value; }
            get { return mImageExt; }
        }
        [DataMember]
        public string ImageName
        {
            set { mImageName = value; }
            get { return mImageName; }
        }
    }

    // 読影データ
    [DataContract]
    public class RsReadingData
    {
        public RsHistoryTbl mHistoryTbl;        // 読影テーブル
        public List<RsImageTbl> mImageTblList;  // 画像テーブル

        [DataMember]
        // 読影テーブル
        public RsHistoryTbl HistoryTbl
        {
            set { mHistoryTbl = value; }
            get { return mHistoryTbl; }
        }
        [DataMember]
        // 画像テーブル
        public List<RsImageTbl> ImageTblList
        {
            set { mImageTblList = value; }
            get { return mImageTblList; }
        }
    }


    // 読影データ
    [DataContract]
    public class UserMst2
    {
        [DataMember]
        public int UserCD;
        [DataMember]
        public string UserName;
    }


    public class ReportImage
    {
        public byte[] ImageData
        {
            get;
            set;
        }

        public string ImageExt
        {
            get;
            set;
        }
    }
}
