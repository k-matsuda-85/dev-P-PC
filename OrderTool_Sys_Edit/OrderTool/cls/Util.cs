using OrderTool.QRService;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OrderTool.cls
{
    /// <summary>
    /// タグ一覧
    /// </summary>
    public enum Tags
    {
        PATIENTID = 0x00100020,
        PATIENTNAME = 0x00100010,
        BIRTHDAY= 0x00100030,
        SEX = 0x00100040,
        AGE = 0x00101010,
        STUDYDATE = 0x00080020,
        STUDYTIME = 0x00080030,
        MODALITY = 0x00080060,
        ACCSESSIONNO = 0x00080050,
        BODYPART = 0x00180015,
        STUDY_ID = 0x00200010,
        DESCRIPTION = 0x00081030,
        SERIES_DESCRIPTION = 0x0008103E,
        STUDY_INSTANCE_UID = 0x0020000D,
        SERIES_INSTANCE_UID = 0x0020000E,
        SOP_INSTANCE_UID = 0x00080018
    }

    /// <summary>
    /// 除外対象タグクラス
    /// </summary>
    public class C_Tag
    {
        /// <summary>タグ</summary>
        public uint Tag { get; set; }

        /// <summary>判定値</summary>
        public string[] Vals { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public C_Tag()
        {
            this.Tag = 0;
            this.Vals = null;
        }

        /// <summary>
        /// 判定値追加
        /// </summary>
        /// <param name="val"></param>
        public void AddVal(string val)
        {
            List<string> tmpVal = new List<string>();

            if (this.Vals != null)
                tmpVal = this.Vals.ToList();

            if (!tmpVal.Contains(val))
                tmpVal.Add(val);

            this.Vals = tmpVal.ToArray();
        }
    }

    /// <summary>
    /// 検査情報クラス
    /// </summary>
    public class StudyData
    {
        public string UID { get; set; }
        public int Cnt { get; set; }
        public string Modality { get; set; }
        public Dictionary<uint, string> Tags { get; set; }
        public SeriesData[] Series { get; set; }

        public void SetStudy(Dictionary<uint, string> tags)
        {
            this.Tags = tags;
            this.UID = tags[(uint)cls.Tags.STUDY_INSTANCE_UID];
            this.Series = (new List<SeriesData>()).ToArray();
        }

        public void SetSeries(DicomQRItem[] series)
        {
            List<SeriesData> tmpList = this.Series.ToList();
            string mod = this.Modality;

            foreach (var ser in series)
            {
                SeriesData tmpSeries = new SeriesData();
                tmpSeries.UID = ser.Tags[(uint)cls.Tags.SERIES_INSTANCE_UID];
                tmpSeries.Tags = ser.Tags;

                if (string.IsNullOrEmpty(mod))
                    mod = ser.Tags[(uint)cls.Tags.MODALITY];
                else if (mod.IndexOf(ser.Tags[(uint)cls.Tags.MODALITY]) < 0)
                    mod = mod + "/" + ser.Tags[(uint)cls.Tags.MODALITY];

                tmpSeries.Images = (new List<SOPData>()).ToArray();
                tmpList.Add(tmpSeries);
            }

            this.Modality = mod;
            this.Series = tmpList.ToArray();
        }

        public void SetImages(DicomQRItem[] images)
        {
            List<SeriesData> tmpList = this.Series.ToList();
            string mod = this.Modality;

            foreach (var img in images)
            {
                for (int i = 0; i < this.Series.Length; i++)
                {
                    if (this.Series[i].UID ==
                        img.Tags[(uint)cls.Tags.SERIES_INSTANCE_UID])
                    {
                        List<SOPData> tmpImgList = null;
                        tmpImgList = this.Series[i].Images.ToList();

                        SOPData tmpImg = new SOPData();

                        tmpImg.Tags = img.Tags;
                        tmpImg.UID = img.Tags[(uint)cls.Tags.SOP_INSTANCE_UID];

                        tmpImgList.Add(tmpImg);

                        this.Series[i].Images = tmpImgList.ToArray();
                        this.Series[i].Cnt++;
                        this.Cnt++;
                    }
                }
            }
        }

        public int GetDiffImageCnt(C_Tag[] tags)
        {
            int ret = 0;

            foreach (var tag in tags)
            {
                if (this.Tags.Keys.Contains(tag.Tag))
                {
                    foreach (var val in tag.Vals)
                    {
                        if (this.Tags[tag.Tag].IndexOf(val) >= 0)
                        {
                            ret = this.Cnt;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var series in this.Series)
                    {
                        if (series.Tags.Keys.Contains(tag.Tag))
                        {
                            foreach (var val in tag.Vals)
                            {
                                if (series.Tags[tag.Tag].IndexOf(val) >= 0)
                                {
                                    ret += series.Cnt;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (var img in series.Images)
                            {
                                if (img.Tags.Keys.Contains(tag.Tag))
                                {
                                    foreach (var val in tag.Vals)
                                    {
                                        if (img.Tags[tag.Tag].IndexOf(val) >= 0)
                                        {
                                            ret++;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return ret;
        }

        public int GetErrImageCnt(C_Tag[] tags)
        {
            int ret = 0;

            foreach (var tag in tags)
            {
                if (this.Tags.Keys.Contains(tag.Tag))
                {
                    foreach (var val in tag.Vals)
                    {
                        Regex reg = new Regex(val);
                        if (reg.IsMatch(this.Tags[tag.Tag]))
                        {
                            ret = this.Cnt;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var series in this.Series)
                    {
                        if (series.Tags.Keys.Contains(tag.Tag))
                        {
                            foreach (var val in tag.Vals)
                            {
                                Regex reg = new Regex(val);
                                if (reg.IsMatch(series.Tags[tag.Tag]))
                                {
                                    ret += series.Cnt;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (var img in series.Images)
                            {
                                if (img.Tags.Keys.Contains(tag.Tag))
                                {
                                    foreach (var val in tag.Vals)
                                    {
                                        Regex reg = new Regex(val);
                                        if (reg.IsMatch(img.Tags[tag.Tag]))
                                        {
                                            ret++;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return ret;
        }

    }

    public class SeriesData
    {
        public string UID { get; set; }
        public int Cnt { get; set; }
        public Dictionary<uint, string> Tags { get; set; }
        public SOPData[] Images { get; set; }
    }

    public class SOPData
    {
        public string UID { get; set; }
        public Dictionary<uint, string> Tags { get; set; }
    }


}