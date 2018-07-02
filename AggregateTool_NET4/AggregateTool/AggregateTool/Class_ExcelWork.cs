using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Office.Interop.Excel;
using Agg_Serv.Class;
using Agg_Serv;
using System.Globalization;
using System.Configuration;

namespace AggregateTool
{
    class Class_ExcelWork
    {
        Class_Setting setting = new Class_Setting();
        Class_TextWokForExcel textwork = new Class_TextWokForExcel();

        public  List<HospDistMst> HospDistMsts = new List<HospDistMst>();
        public  List<ExtReports> HospReports = new List<ExtReports>();
        public  List<Doctor> RetdocList = new List<Doctor>();
        public  Dictionary<string, string> ModalityList = new Dictionary<string, string>();
        public  Dictionary<string, string> ModalityDocList = new Dictionary<string, string>();
        private static string J_Year;
        private static string Month;
        private static int lastRow;

        public List<string> YayoiHosps = new List<string>();
        #region 月次データ

        /// <summary>
        /// エクセル出力メインメソッド
        /// </summary>
        public void WriteExcel_Sheet_1(DateTime search, int type)
        {
            //エクセルの宣言
            Application excelApp = null;
            Workbook masterBook = null;
            Worksheet masterSheet = null;
            try
            {
                CultureInfo culture = new CultureInfo("ja-JP", true);
                culture.DateTimeFormat.Calendar = new JapaneseCalendar();

                J_Year = search.ToString("yy", culture);
                Month = search.ToString("%M");

                DateTime copyDate = search.AddMonths(-1);

                string copyExcel = Path.Combine(setting.masterPath,
                                               "H" + copyDate.ToString("yy", culture) + "." + copyDate.ToString("%M") + "月次データ.xlsm");
                //出力ファイルのパスを作成
                string newExcel = Path.Combine(setting.masterPath,
                                               "H" + search.ToString("yy", culture) + "." + search.ToString("%M") + "月次データ.xlsm");
                if (File.Exists(newExcel))
                    File.Delete(newExcel);
                File.Copy(copyExcel, newExcel);

                //Excelのアプリケーションを起動
                excelApp = new Application();
                //Excelのブックをオープンする
                masterBook = excelApp.Workbooks.Open(newExcel);

                foreach(var hosp in HospDistMsts)
                {
                    Class_SearchParam sp = new Class_SearchParam();
                    sp.searchName = hosp.Hospital.Name_Disp;
                    textwork.search = sp;
                    //シートを指定
                    if (GetSheetIndex(hosp.Hospital.Name_Disp, masterBook) == 0)
                    {
                        continue;
                    }
                    masterSheet = (Worksheet)masterBook.Sheets
                                            [GetSheetIndex(hosp.Hospital.Name_Disp, masterBook)];

                    ExtReports val = new ExtReports();
                    val.ModReports = new List<ModReports>().ToArray();
                    foreach (var tmp in HospReports)
                    {
                        if (tmp.Cd == hosp.Hospital.Cd)
                        {
                            val = tmp;
                            break;
                        }
                    }


                    //シートをセレクト。
                    masterSheet.Select(Type.Missing);
                    GetHeader(masterSheet);
                    string[] tmpval = hosp.Hospital.Name.Split('-');
                    masterSheet.PageSetup.LeftHeader = "&14" + tmpval[tmpval.Length - 1] + "様    " + Month + "月分読影明細表";

                    switch (hosp.Dist)
                    {
                        case "0":
                        case null:
                        case "":
                            WriteExcel(val, masterSheet);
                            break;
                        case "1":
                            WriteExcel_ForDoc(val, masterSheet);
                            //シートを指定
                            if (GetSheetIndex(hosp.Hospital.Name_Disp + "_加算", masterBook) == 0)
                            {
                                continue;
                            }
                            masterSheet = (Worksheet)masterBook.Sheets
                                                    [GetSheetIndex(hosp.Hospital.Name_Disp + "_加算", masterBook)];
                            //シートをセレクト。
                            masterSheet.Select(Type.Missing);

                            WriteExcel_ForAdd(val, masterSheet);

                            break;
                        case "2":
                            List<ExtReports> vals = new List<ExtReports>();
                            List<string> names = new List<string>();
                            names.Add("");
                            vals.Add(val);

                            foreach(var hos in HospDistMsts)
                            {
                                if (hos.ParentCd != hosp.Hospital.Name_DB)
                                    continue;

                                foreach (var tmp in HospReports)
                                {
                                    if (tmp.Cd == hos.Hospital.Cd)
                                    {
                                        string[] tmpv = hos.Hospital.Name.Split('-');

                                        names.Add(tmpv[tmpv.Length - 1] + "様    " + Month + "月分読影明細表");
                                        vals.Add(tmp);
                                        break;
                                    }
                                }
                            }

                            WriteExcel_ForHos(vals.ToArray(), names.ToArray(), masterSheet);

                            break;
                    }

                }

                //シートを指定
                if (!(GetSheetIndex("緊急・画像加算集計結果", masterBook) == 0))
                {
                    masterSheet = (Worksheet)masterBook.Sheets
                                            [GetSheetIndex("緊急・画像加算集計結果", masterBook)];

                    //シートをセレクト。
                    masterSheet.Select(Type.Missing);

                    WriteExcel_LastCnt(masterSheet);
                }

                excelApp.ActiveWorkbook.Save();
            }
            catch (Exception e)
            {
                LogUtil.Error(e.Message);
            }
            finally
            {
                masterSheet = null;
                if (masterBook != null)
                    masterBook.Close(false);
                if (excelApp != null)
                    excelApp.Quit();
            }
        }

        private void MoveFile(string newPath)
        {
            //ファイルがすでにある場合
            if (File.Exists(newPath))
                //削除する
                File.Delete(newPath);
            //ファイルをコピーして出力ファイルを作成する
            File.Copy(setting.masterPath, newPath);
        }

        /// <summary>
        /// ブックから指定したシート名の位置を調べる
        /// </summary>
        private int GetSheetIndex(string sheetName, Workbook masterBook)
        {
            int index = 0;
            for (int i = 1; i <= masterBook.Worksheets.Count; i++)
            {
                Worksheet selectSheet = (Worksheet)masterBook.Worksheets[i];
                if (sheetName == selectSheet.Name)
                {
                    index = i;
                    break;
                }
                selectSheet = null;
            }
            return index;
        }

        /// <summary>
        /// Excelの書き込みメソッド
        /// </summary>
        private void WriteExcel(ExtReports rep, Worksheet ws)
        {
            //Rangeの位置設定変数
            int rowPos = setting.startRow;
            int columnPos = setting.startColumn;
            int between = setting.betweenLine;
            int colorIndex = 0;

            //Range range = ws.Cells[1,1];
            //colorIndex = range.Interior.ColorIndex;

            ClearSheet(ws);
            string[] header = GetHeaderVal(ws);

            //モダリティ毎に処理  
            foreach (var modality in rep.ModReports)
            {
                if (modality.Count == 0)
                    continue;
                
                object[,] values = CreateValue(modality.Reports, header);
                if (values.GetLength(0) == 0)
                    continue;


                int startRow = rowPos;

                if (startRow != setting.startRow)
                {
                    ////罫線を引く
                    //WriteLine(rowPos, columnPos, rowPos + 1, ws, header.Length);

                    //Range valueRangeH = ws.Range[ws.Cells[rowPos, columnPos],
                    //                            ws.Cells[rowPos, header.Length]];
                    //valueRangeH.Value2 = header;
                    //valueRangeH.HorizontalAlignment = XlHAlign.xlHAlignCenter;

                    //valueRangeH.Interior.ColorIndex = colorIndex;

                    Range range = ws.Range[ws.Cells[1, 1], ws.Cells[1, header.Length]];
                    range.Copy();
                    Range valueRangeH = ws.Range[ws.Cells[rowPos, 1],
                                                ws.Cells[rowPos, header.Length]];
                    valueRangeH.PasteSpecial();

                    startRow++;
                    rowPos++;
                }

                int endRow = rowPos + values.GetLength(0) - 1;
                //入力値を挿入するRange
                Range valueRange = ws.Range[ws.Cells[rowPos, columnPos],
                                            ws.Cells[endRow, header.Length]];
                //行の高さを指定
                valueRange.RowHeight = 12.75;
                //罫線を引く
                WriteLine(rowPos, columnPos, endRow, ws, header.Length);

                //値を追加する
                valueRange.Value2 = values;
                valueRange.Font.Name = "Arial";
                valueRange.Font.Size = 10;

                valueRange = ws.Range[ws.Cells[rowPos, 2], ws.Cells[endRow, 2]];
                valueRange.Interior.ColorIndex = setting.cellColor;

                //最終行に行間を足して集計行の行数を作成する
                rowPos = endRow + between;

                //ヘッダー位置を判定するカウント
                int count = 0;
                int drawStart = 0;
                int drawEnd = 0;
                for (int i = 0; i < header.Length; i++)
                {
                    count++;
                    Range rgn = (Range)ws.Cells[rowPos, count];

                    if (String.IsNullOrEmpty(header[i]))
                        continue;

                    if (header[i] == "モダリティ")
                    {
                        rgn.Value2 = modality.Mod + "合計";
                        drawStart = count;
                    }
                    else if (setting.countParam.Contains(header[i]))
                        rgn.FormulaR1C1 = "=COUNTA(R[-" + (values.GetLength(0) + between - 1) +
                                      "]C:R[-" + between + "]C)";
                    else if (setting.sumParam.Contains(header[i]))
                        rgn.FormulaR1C1 = "=SUM(R[-" + (values.GetLength(0) + between - 1) +
                                               "]C:R[-" + between + "]C)";
                    drawEnd = count;
                }
                //位置の値を使用して集計行に色指定
                Range drawRange = ws.Range[ws.Cells[rowPos, drawStart], ws.Cells[rowPos, drawEnd]];
                //背景色付け
                drawRange.Interior.ColorIndex = setting.cellColor;
                //次の開始行
                rowPos += between;
            }
        }

        /// <summary>
        /// Excelの書き込みメソッド
        /// </summary>
        private void WriteExcel_ForHos(ExtReports[] rep, string[] names, Worksheet ws)
        {
            //Rangeの位置設定変数
            int rowPos = setting.startRow;
            int columnPos = setting.startColumn;
            int between = setting.betweenLine;
            int colorIndex = 0;

            //Range range2 = ws.Cells[1, 1];
            //colorIndex = range2.Interior.ColorIndex;

            ClearSheet(ws);
            string[] header = GetHeaderVal(ws);

            for (int j = 0; j < rep.Length; j++)
            {
                if (names[j] != "")
                {
                    Range range = ws.Cells[rowPos, 2];
                    range.Value2 = names[j];

                    range = ws.Rows[rowPos];
                    range.RowHeight = 18.0;
                    range.Font.Name = "Arial";
                    range.Font.Size = 14;

                    rowPos += 2;
                }

                //モダリティ毎に処理  
                foreach (var modality in rep[j].ModReports)
                {
                    if (modality.Count == 0)
                        continue;

                    object[,] values = CreateValue(modality.Reports, header);
                    if (values.GetLength(0) == 0)
                        continue;

                    int startRow = rowPos;

                    if (startRow != setting.startRow)
                    {
                        ////罫線を引く
                        //WriteLine(rowPos, columnPos, rowPos + 1, ws, header.Length);

                        //Range valueRangeH = ws.Range[ws.Cells[rowPos, columnPos],
                        //                            ws.Cells[rowPos, header.Length]];
                        //valueRangeH.Value2 = header;
                        //valueRangeH.HorizontalAlignment = XlHAlign.xlHAlignCenter;

                        //valueRangeH.Interior.ColorIndex = colorIndex;

                        Range range = ws.Range[ws.Cells[1, 1], ws.Cells[1, header.Length]];
                        range.Copy();
                        Range valueRangeH = ws.Range[ws.Cells[rowPos, 1],
                                                    ws.Cells[rowPos, header.Length]];
                        valueRangeH.PasteSpecial();

                        startRow++;
                        rowPos++;
                    }

                    int endRow = rowPos + values.GetLength(0) - 1;
                    //入力値を挿入するRange
                    Range valueRange = ws.Range[ws.Cells[rowPos, columnPos],
                                                ws.Cells[endRow, header.Length]];
                    //行の高さを指定
                    valueRange.RowHeight = 12.75;
                    //罫線を引く
                    WriteLine(rowPos, columnPos, endRow, ws, header.Length);

                    //値を追加する
                    valueRange.Value2 = values;
                    valueRange.Font.Name = "Arial";
                    valueRange.Font.Size = 10;

                    valueRange = ws.Range[ws.Cells[rowPos, 2], ws.Cells[endRow, 2]];
                    valueRange.Interior.ColorIndex = setting.cellColor;

                    //最終行に行間を足して集計行の行数を作成する
                    rowPos = endRow + between;

                    //ヘッダー位置を判定するカウント
                    int count = 0;
                    int drawStart = 0;
                    int drawEnd = 0;
                    for (int i = 0; i < header.Length; i++)
                    {
                        count++;
                        Range rgn = (Range)ws.Cells[rowPos, count];

                        if (String.IsNullOrEmpty(header[i]))
                            continue;

                        if (header[i] == "モダリティ")
                        {
                            rgn.Value2 = modality.Mod + "合計";
                            drawStart = count;
                        }
                        else if (setting.countParam.Contains(header[i]))
                            rgn.FormulaR1C1 = "=COUNTA(R[-" + (values.GetLength(0) + between - 1) +
                                          "]C:R[-" + between + "]C)";
                        else if (setting.sumParam.Contains(header[i]))
                            rgn.FormulaR1C1 = "=SUM(R[-" + (values.GetLength(0) + between - 1) +
                                                   "]C:R[-" + between + "]C)";
                        drawEnd = count;
                    }
                    //位置の値を使用して集計行に色指定
                    Range drawRange = ws.Range[ws.Cells[rowPos, drawStart], ws.Cells[rowPos, drawEnd]];
                    //背景色付け
                    drawRange.Interior.ColorIndex = setting.cellColor;
                    //次の開始行
                    rowPos += between;
                }

            }

        }


        private void WriteExcel_ForDoc(ExtReports rep, Worksheet ws)
        {
            //Rangeの位置設定変数
            int rowPos = setting.startRow;
            int columnPos = setting.startColumn;
            int between = setting.betweenLine;
            int colorIndex = 0;

            ClearSheet(ws);
            string[] header = GetHeaderVal(ws);

            //モダリティ毎に処理  
            foreach (var modality in rep.ModReports)
            {
                if (modality.Count == 0)
                    continue;
                foreach (var doc in RetdocList)
                {
                    List<Report> listRep = new List<Report>();

                    foreach (var repd in modality.Reports)
                    {
                        if (repd.PayFlg == 0)
                            continue;

                        if (doc.Name == repd.ReadCd)
                        {
                            listRep.Add(repd);
                        }
                    }
                    if (listRep.Count == 0)
                        continue;

                    int startRow = rowPos;

                    if (startRow != setting.startRow)
                    {
                        ////罫線を引く
                        //WriteLine(rowPos, columnPos, rowPos + 1, ws, header.Length);

                        //Range valueRangeH = ws.Range[ws.Cells[rowPos, columnPos],
                        //                            ws.Cells[rowPos, header.Length]];
                        //valueRangeH.Value2 = header;
                        //valueRangeH.HorizontalAlignment = XlHAlign.xlHAlignCenter;

                        //valueRangeH.Interior.ColorIndex = colorIndex;

                        Range range = ws.Range[ws.Cells[1, 1], ws.Cells[1, header.Length]];
                        range.Copy();
                        Range valueRangeH = ws.Range[ws.Cells[rowPos, 1],
                                                    ws.Cells[rowPos, header.Length]];
                        valueRangeH.PasteSpecial();

                        startRow++;
                        rowPos++;
                    }

                    int endRow = rowPos + listRep.Count - 1;

                    //入力値を挿入するRange
                    Range valueRange = ws.Range[ws.Cells[rowPos, columnPos],
                                                ws.Cells[endRow, header.Length]];
                    //行の高さを指定
                    valueRange.RowHeight = 12.75;

                    //罫線を引く
                    WriteLine(rowPos, columnPos, endRow, ws, header.Length);

                    //値を追加する
                    valueRange.Value2 = CreateValue(listRep.ToArray(), header);
                    valueRange.Font.Name = "Arial";
                    valueRange.Font.Size = 10;

                    valueRange = ws.Range[ws.Cells[rowPos, 2], ws.Cells[endRow, 2]];
                    valueRange.Interior.ColorIndex = setting.cellColor;

                    //最終行に行間を足して集計行の行数を作成する
                    rowPos = endRow + between;

                    //ヘッダー位置を判定するカウント
                    int count = 0;
                    int drawStart = 0;
                    int drawEnd = 0;
                    for (int i = 0; i < header.Length; i++)
                    {
                        count++;
                        Range rgn = (Range)ws.Cells[rowPos, count];

                        if (String.IsNullOrEmpty(header[i]))
                            continue;

                        if (header[i] == "モダリティ")
                        {
                            rgn.Value2 = modality.Mod + "合計";
                            drawStart = count;
                        }
                        else if (setting.countParam.Contains(header[i]))
                            rgn.FormulaR1C1 = "=COUNTA(R[-" + (listRep.Count + between - 1) +
                                          "]C:R[-" + between + "]C)";
                        else if (setting.sumParam.Contains(header[i]))
                            rgn.FormulaR1C1 = "=SUM(R[-" + (listRep.Count + between - 1) +
                                                   "]C:R[-" + between + "]C)";
                        drawEnd = count;
                    }
                    //位置の値を使用して集計行に色指定
                    Range drawRange = ws.Range[ws.Cells[rowPos, drawStart], ws.Cells[rowPos, drawEnd]];
                    //背景色付け
                    drawRange.Interior.ColorIndex = setting.cellColor;
                    //次の開始行
                    rowPos += between;
                }
            }
        }

        private void WriteExcel_ForAdd(ExtReports rep, Worksheet ws)
        {
            //Rangeの位置設定変数
            int rowPos = 6;
            int columnPos = 3;

            ClearSheet2(ws);
            string[] header = GetHeaderVal(ws);
            Range Range = null;

            //モダリティ毎に処理  
            foreach (var modality in rep.ModReports)
            {
                //columnPos = GetColIndex(ws, 5, modality.Mod);
                if(modality.Reports.Count() == 0)
                {
                    continue;
                }

                Range = ws.Cells[5, columnPos];
                Range.Value2 = modality.Mod;

                foreach (var doc in RetdocList)
                {
                    int count = 0;

                    foreach (var repd in modality.Reports)
                    {
                        if (repd.PayFlg == 0)
                            continue;

                        if (doc.Name == repd.ReadCd)
                            count++;
                    }

                    if (count == 0)
                    {
                        continue;
                    }

                    // 読影医名設定
                    var index = GetRowIndex(ws, 2, doc.Name);
                    if (index == 0)
                    {
                        index = rowPos;
                        rowPos++;
                    }

                    Range = ws.Cells[index, 2];
                    Range.Value2 = doc.Name;

                    //入力値を挿入するRange
                    Range = ws.Cells[index, columnPos];

                    //値を追加する
                    Range.Value2 = count;

                    Range = ws.Rows[index];
                    Range.RowHeight = 19.5;

                    Range = ws.Columns[columnPos];
                    Range.ColumnWidth = 9.29;
                }
                columnPos++;
            }

            Range bolderRange = ws.Range[ws.Cells[5, 19], ws.Cells[21, 14]];
            bolderRange.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle
                            = XlLineStyle.xlContinuous;
            bolderRange.Borders.get_Item(XlBordersIndex.xlEdgeLeft).Weight
                            = XlBorderWeight.xlMedium;

            bolderRange = ws.Range[ws.Cells[21, 2], ws.Cells[21, 13]];
            bolderRange.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle
                            = XlLineStyle.xlDouble;

            Range = ws.Cells[3, 2];

            Range.Value2 = "(H" + J_Year + "年  " + Month + "月分)";

        }

        private void WriteExcel_LastCnt(Worksheet ws)
        {
            //Rangeの位置設定変数
            int rowPos = setting.startRow;
            int columnPos = setting.startColumn;
            int between = setting.betweenLine;

            ClearSheet3(ws);

            foreach (var hosp in HospDistMsts)
            {
                int col = 2;
                
                var index = GetRowIndex(ws, 2, hosp.Hospital.Name_Disp);

                if (index == 0)
                {
                    col = 6;
                    index = GetRowIndex(ws, 6, hosp.Hospital.Name_Disp);
                }

                if (index == 0)
                    continue;

                ExtReports val = new ExtReports();
                val.ModReports = new List<ModReports>().ToArray();
                foreach (var tmp in HospReports)
                {
                    if (tmp.Cd == hosp.Hospital.Cd)
                    {
                        val = tmp;
                        break;
                    }
                }
                int em = 0;
                int image = 0;

                foreach(var mod in val.ModReports)
                {
                    if (mod.Count == 0)
                        continue;

                    foreach(var rep in mod.Reports)
                    {
                        if (rep.PayFlg == 0)
                            continue;

                        em += rep.PriorityFlg;
                        image += rep.AddImageFlg;
                    }
                }

                Range range = ws.Cells[index, col + 1];
                range.Value2 = em;
                range = ws.Cells[index, col + 2];
                range.Value2 = image;
            }
        }


        /// <summary>
        /// 罫線を引く
        /// </summary>
        private void WriteLine(int startRow, int startClm, int end, Worksheet ws, int headCnt)
        {
            //行に線を引く
            Range bolderRange = ws.Range[ws.Cells[startRow, startClm], ws.Cells[end, headCnt]];
            bolderRange.Borders.get_Item(XlBordersIndex.xlInsideHorizontal).LineStyle
                            = XlLineStyle.xlContinuous;
            bolderRange.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle
                            = XlLineStyle.xlContinuous;
        }

        /// <summary>
        /// モダリティごとの設定
        /// </summary>
        private object[,] CreateValue(Report[] writeModality, string[] header)
        {
            int cnt = 0;
            object[,] value = new object[0, header.Length];
            List<Report> listVal = new List<Report>();


            for (int i = 0; i < writeModality.Length; i++)
            {
                if (writeModality[i].PayFlg == 0)
                    continue;
                listVal.Add(writeModality[i]);
            }

            if (listVal.Count == 0)
                return value;

            value = new object[listVal.Count, header.Length];

            for (int i = 0; i < listVal.Count; i++)
            {
                for (int j = 0; j <header.Length; j++)
                {
                    string val = "";

                    if (header[j] == setting.masterElement[0])
                        val = listVal[i].HName;
                    else if (header[j] == setting.masterElement[1])
                    {
                        if (!String.IsNullOrEmpty(listVal[i].StudyDate))
                            val = listVal[i].ReadDate.Substring(0, 4) + "/" + listVal[i].ReadDate.Substring(4, 2) + "/" + listVal[i].ReadDate.Substring(6, 2);
                    }
                    else if (header[j] == setting.masterElement[2])
                    {
                        if(!String.IsNullOrEmpty(listVal[i].StudyDate))
                            val = listVal[i].StudyDate.Substring(0, 4) + "/" + listVal[i].StudyDate.Substring(4, 2) + "/" + listVal[i].StudyDate.Substring(6,2);
                    }
                    else if (header[j] == setting.masterElement[3])
                        val = listVal[i].Modality;
                    else if (header[j] == setting.masterElement[4])
                        val = listVal[i].PatID;
                    else if (header[j] == setting.masterElement[5])
                        val = listVal[i].PatName;
                    else if (header[j] == setting.masterElement[6])
                        val = listVal[i].PhysicianName;
                    else if (header[j] == setting.masterElement[7])
                        val = listVal[i].BodyPart;
                    else if (header[j] == setting.masterElement[8])
                        val = listVal[i].OrderDetail;
                    else if (header[j] == setting.masterElement[9])
                        val = listVal[i].Contact;
                    else if (header[j] == setting.masterElement[10])
                        val = listVal[i].Accept;
                    else if (header[j] == setting.masterElement[11])
                        val = listVal[i].BodyPartFlg.ToString();
                    else if (header[j] == setting.masterElement[12])
                        val = listVal[i].PriorityFlg.ToString();
                    else if (header[j] == setting.masterElement[13])
                        val = listVal[i].AddImageFlg.ToString();
                    else if (header[j] == setting.masterElement[14])
                        val = listVal[i].ImageCnt.ToString();
                    else if (header[j] == setting.masterElement[15])
                        val = listVal[i].MailFlg.ToString();
                    else if (header[j] == setting.masterElement[16])
                        val = listVal[i].AddMGFlg.ToString();
                    else if (header[j] == setting.masterElement[17])
                        val = listVal[i].PayFlg.ToString();
                    else if (header[j] == setting.masterElement[18])
                        val = listVal[i].IntroFlg.ToString();
                    else if (header[j] == setting.masterElement[19])
                        val = listVal[i].ReadCd;
                    else if (header[j] == setting.masterElement[20])
                        val = listVal[i].Memo.ToString();
                    else if (header[j] == setting.masterElement[21])
                        val = listVal[i].Department.ToString();
                    else
                        val = (i + 1).ToString();

                    value[i, j] = val;
                }
            }

            return value;
        }

        private Dictionary<string,string> ToStrinfStringDictionary(Dictionary<int, string> oldValue)
        {
            Dictionary<string, string> newValue = new Dictionary<string, string>();
            foreach (int key in oldValue.Keys)
            {
                newValue.Add(setting.masterElement[key],oldValue[key]);
            }
            return newValue;
        }

        /// <summary>
        /// シート内の値を取得してリストにして返す。
        /// </summary>
        private void GetHeader(Worksheet masterSheet)
        {
            int lastRow = masterSheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Row;
            int lastColumn = masterSheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Column;

            for (int i = 1; i <= lastColumn; i++)
            {
                Range selectRange = (Range)masterSheet.Cells[1, i];
                if (selectRange.Value == null)
                    break;
                //useHeader.Add(selectRange.Value.ToString());
            }
        }

        private void ClearSheet(Worksheet masterSheet)
        {
            int lastRow = masterSheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Row;
            int lastColumn = masterSheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Column;

            if (lastRow > 1)
                masterSheet.Rows["2:" + lastRow].Delete();

        }

        private void ClearSheet2(Worksheet masterSheet)
        {
            int lastRow = masterSheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Row;
            int lastColumn = masterSheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Column;

            int startrow = 6;
            int endrow = 0;
            int startcol = 2;
            int endcol = 0;

            Range range = null;

            endrow = GetRowIndex(masterSheet, 2, "合　　計") - 1;
            if (endrow < 0)
                endrow = 20;
            endcol = GetColIndex(masterSheet, 5, "単価") - 1;
            if (endcol < 0)
                endcol = 11;

            range = masterSheet.Range[masterSheet.Cells[startrow, startcol], masterSheet.Cells[endrow, endcol]];
            range.ClearContents();

            for (int i = startrow; i <= endrow; i++)
            {
                range = masterSheet.Rows[i];
                range.RowHeight = 0;

            }

            for (int i = startcol + 1; i <= endcol; i++)
            {
                range = masterSheet.Columns[i];
                range.ColumnWidth = 0;

            }

        }

        private void ClearSheet3(Worksheet masterSheet)
        {
            int lastRow = masterSheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Row;

            Range range = null;

            range = masterSheet.Range[masterSheet.Cells[3, 3], masterSheet.Cells[lastRow, 4]];
            range.ClearContents();
            range = masterSheet.Range[masterSheet.Cells[3, 7], masterSheet.Cells[lastRow, 8]];
            range.ClearContents();
        }

        #endregion

        #region 依頼推移表

        public void WriteExcel_Sheet_2(DateTime search, int type)
        {
            //エクセルの宣言
            Application excelApp = null;
            Workbook masterBook = null;
            Worksheet masterSheet = null;

            bool modFlg = false;
            string modVal = "";
            string hosVal = "";

            try
            {
                CultureInfo culture = new CultureInfo("ja-JP", true);
                culture.DateTimeFormat.Calendar = new JapaneseCalendar();

                J_Year = search.ToString("yy", culture);
                Month = search.ToString("%M");

                lastRow = 4 + (HospReports.Count * ModalityList.Keys.Count) + ModalityDocList.Keys.Count * 3;

                DateTime copyDate = search.AddMonths(-1);

                var Y_J_Year = copyDate.ToString("yy", culture);
                var Y_Month = copyDate.ToString("%M");

                //出力ファイルのパスを作成
                string newExcel = Path.Combine(setting.masterPath, "件数推移表(第12期).xlsx");

                //Excelのアプリケーションを起動
                excelApp = new Application();

                if (!File.Exists(newExcel))
                {
                    masterBook = excelApp.Workbooks.Add();
                }
                else
                {
                    //Excelのブックをオープンする
                    masterBook = excelApp.Workbooks.Open(newExcel);
                }

                var index = GetSheetIndex("件数推移表", masterBook);

                if (index == 0){
                    masterSheet = (Worksheet)masterBook.Sheets[1];
                    //シートをセレクト。
                    masterSheet.Select(Type.Missing);
                    masterSheet.Name = "件数推移表";
                    
                    CreateSheet(excelApp, masterSheet);
                }
                else
                {
                    masterSheet = (Worksheet)masterBook.Sheets[index];
                    masterSheet.Select(Type.Missing);
                }

                var DateCol = GetColIndex(masterSheet, 3, "H" + J_Year + "." + Month + "月");
                if(DateCol == 0)
                {
                    CreateHeader(masterSheet, search);
                    DateCol = GetColIndex(masterSheet, 3, "H" + J_Year + "." + Month + "月");
                }


                Dictionary<string, int> sumVal = new Dictionary<string, int>();
                Dictionary<string, int> sumSysVal = new Dictionary<string, int>();
                int modCount = ModalityList.Keys.Count;
                object[,] modalities = new object[modCount, 1];
                Range range = null;

                foreach(var mod in ModalityDocList)
                {
                    sumVal[mod.Value] = 0;
                    sumSysVal[mod.Value] = 0;

                    if(mod.Value == "MR")
                    {
                        sumVal["CT+MR"] = 0;
                        sumSysVal["CT+MR"] = 0;
                    }
                }


                foreach (var hosp in HospDistMsts)
                {
                    ExtReports val = new ExtReports();
                    val.ModReports = new List<ModReports>().ToArray();

                    Dictionary<string, int> writeVal = new Dictionary<string, int>();


                    modalities = new object[modCount, 1];

                    foreach (var tmp in HospReports)
                    {
                        if (tmp.Cd == hosp.Hospital.Cd)
                        {
                            val = tmp;
                            break;
                        }
                    }

                    var HosIndex = GetRowIndex(masterSheet, lastRow, 2, hosp.Hospital.Name_Disp);
                    if (HosIndex == 0)
                        continue;
                    var modIndex = HosIndex;

                    int firstCol = 0;

                    for (var i = 0; i < val.ModReports.Length; i++)
                    {
                        var cnt = 0;
                        modIndex = HosIndex + i;
                        foreach (var rep in val.ModReports[i].Reports)
                        {
                            if (rep.PayFlg == 0)
                                continue;
                            cnt++;
                        }

                        if (cnt > 0)
                        {
                            range = masterSheet.Rows[modIndex];
                            range.RowHeight = 13.5;

                            if (firstCol == 0)
                                firstCol = modIndex;
                        }
                        else if (val.ModReports[i].Mod == "CT")
                        {
                            range = masterSheet.Rows[modIndex];
                            if (range.RowHeight == 0)
                            {
                                range.RowHeight = 0.75;
                            }
                        }

                        modalities[i, 0] = cnt;

                        if (cnt == 0)
                            continue;

                        string modval = val.ModReports[i].Mod;

                        if (!string.IsNullOrEmpty(val.ModReports[i].DocMod))
                            modval = val.ModReports[i].DocMod;
                        else if (modval == "NM")
                            modval = "RI";

                        modFlg = true;

                        modVal = modval;
                        hosVal = hosp.Hospital.Name_Disp;

                        switch (hosp.Dist)
                        {
                            case "1":
                                sumSysVal[modval] = sumSysVal[modval] + cnt;

                                range = masterSheet.Cells[HosIndex, 2];
                                range.Interior.ColorIndex = 4;
                                break;
                            default:
                                sumVal[modval] = sumVal[modval] + cnt;
                                break;
                        }

                        modFlg = false;
                    }

                    sumSysVal["CT+MR"] = sumSysVal["CT"] + sumSysVal["MR"];
                    sumVal["CT+MR"] = sumVal["CT"] + sumVal["MR"];
                    range = masterSheet.Range[masterSheet.Cells[HosIndex, DateCol], masterSheet.Cells[modIndex, DateCol]];
                    range.Value2 = modalities;

                    if(firstCol > 0)
                    {
                        int lastColumn = masterSheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Column;
                        range = masterSheet.Range[masterSheet.Cells[firstCol, 2], masterSheet.Cells[modIndex, lastColumn]];
                        //range.Borders.get_Item(XlBordersIndex.xlInsideHorizontal).LineStyle = XlLineStyle.xlDot;
                        //range.Borders.get_Item(XlBordersIndex.xlInsideVertical).LineStyle = XlLineStyle.xlContinuous;
                        //range.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlContinuous;
                        //range.Borders.get_Item(XlBordersIndex.xlEdgeTop).Weight = XlBorderWeight.xlMedium;
                        //range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle = XlLineStyle.xlContinuous;
                        //range.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
                        //range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
                        //range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).Weight = XlBorderWeight.xlMedium;
                    }
                }

                var sumIndex = GetRowIndex(masterSheet, lastRow, 2, "合計");
                var tmpindex = 0;

                modCount = ModalityDocList.Keys.Count + 1;
                modalities = new object[(modCount * 3) + 1, 1];

                int last = 0;
                int totalSum = 0;

                foreach (var mod in sumVal)
                {
                    modalities[tmpindex, 0] = sumVal[mod.Key] + sumSysVal[mod.Key];
                    if(mod.Key != "CT" && mod.Key != "MR" )
                        totalSum += sumVal[mod.Key] + sumSysVal[mod.Key];
                    modalities[tmpindex + modCount + 1, 0] = sumVal[mod.Key];
                    modalities[tmpindex + modCount + modCount + 1, 0] = sumSysVal[mod.Key];

                    if (sumVal[mod.Key] + sumSysVal[mod.Key] > 0)
                    {
                        range = masterSheet.Rows[sumIndex + tmpindex];
                        range.RowHeight = 13.5;
                    }
                    if (sumVal[mod.Key] > 0)
                    {
                        range = masterSheet.Rows[sumIndex + tmpindex + modCount + 1];
                        range.RowHeight = 13.5;
                    }
                    if (sumSysVal[mod.Key] > 0)
                    {
                        range = masterSheet.Rows[sumIndex + tmpindex + modCount + modCount + 1];
                        range.RowHeight = 13.5;
                        last = sumIndex + tmpindex + modCount + modCount + 1;
                    }

                    tmpindex++;
                }
                modalities[tmpindex, 0] = totalSum;

                range = masterSheet.Range[masterSheet.Cells[sumIndex, DateCol], masterSheet.Cells[sumIndex + modCount * 3, DateCol]];
                range.Value2 = modalities;

                int lastCol = masterSheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Column;
                //range = masterSheet.Range[masterSheet.Cells[sumIndex, 2], masterSheet.Cells[last, lastCol]];
                //range.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlDouble;
                //range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
                //range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).Weight = XlBorderWeight.xlMedium;

                //range = masterSheet.Range[masterSheet.Cells[sumIndex, 2], masterSheet.Cells[sumIndex - 1, 2]];
                //range.Borders.get_Item(XlBordersIndex.xlInsideHorizontal).LineStyle = XlLineStyle.xlContinuous;
                //range.Borders.get_Item(XlBordersIndex.xlInsideHorizontal).Weight = XlBorderWeight.xlMedium;


                if (!File.Exists(newExcel))
                {
                    masterBook.SaveAs(newExcel);
                }
                else
                {
                    excelApp.ActiveWorkbook.Save();
                }
            }
            catch (Exception e)
            {
                LogUtil.Error(e.Message);

                if(modFlg)
                    LogUtil.Error("対象モダリティ無 " + hosVal + "：" + modVal);

            }
            finally
            {
                masterSheet = null;
                if (masterBook != null)
                    masterBook.Close(false);
                if (excelApp != null)
                    excelApp.Quit();
            }
        }

        private void CreateSheet(Application app, Worksheet ws)
        {
            Range range = ws.Cells[2, 1];
            range.Value2 = "病院別件数推移表";

            range = ws.Columns[1];
            range.ColumnWidth = 1.63;

            range = ws.Columns[2];
            range.ColumnWidth = 30;

            range = ws.Columns[3];
            range.ColumnWidth = 7.13;

            range = ws.Cells[4, 4];
            range.Select();

            app.ActiveWindow.FreezePanes = true;

            var index = 4;

            foreach (var hosp in HospDistMsts)
            {
                var modIndex = index;

                ExtReports val = new ExtReports();

                foreach (var tmp in HospReports)
                {
                    if (tmp.Cd == hosp.Hospital.Cd)
                    {
                        val = tmp;
                        break;
                    }
                }

                int color = 15;

                for (int i = 0; i < val.ModReports.Length; i++)
                {
                    modIndex = index + i;

                    range = ws.Cells[modIndex, 3];
                    range.Value2 = val.ModReports[i].Mod;

                    if (val.ModReports[i].Mod.IndexOf("CT") >= 0)
                        color = 35;
                    else if (val.ModReports[i].Mod.IndexOf("MR") >= 0)
                        color = 38;
                    else if (val.ModReports[i].Mod.IndexOf("CR") >= 0)
                        color = 34;
                    else if (val.ModReports[i].Mod.IndexOf("DR") >= 0)
                        color = 6;
                    else if (val.ModReports[i].Mod.IndexOf("RF") >= 0)
                        color = 36;
                    else if (val.ModReports[i].Mod.IndexOf("MG") >= 0)
                        color = 39;
                    else
                        color = 15;

                    range.Interior.ColorIndex = color;
                    range.RowHeight = 0;
                    range.HorizontalAlignment = (int)Microsoft.Office.Interop.Excel.Constants.xlCenter;
                }

                range = ws.Range[ws.Cells[index, 2], ws.Cells[modIndex, 2]];
                range.Rows.Merge();
                range.Value2 = hosp.Hospital.Name_Disp;

                range = ws.Range[ws.Cells[index, 2], ws.Cells[modIndex, 3]];
                range.Borders.get_Item(XlBordersIndex.xlInsideHorizontal).LineStyle = XlLineStyle.xlDot;
                range.Borders.get_Item(XlBordersIndex.xlInsideVertical).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeTop).Weight = XlBorderWeight.xlMedium;
                range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).Weight = XlBorderWeight.xlMedium;
                range.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeRight).Weight = XlBorderWeight.xlMedium;
                range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).Weight = XlBorderWeight.xlMedium;

                index = modIndex + 1;
            }

            // 合計
            int modCount = ModalityDocList.Keys.Count + 1;
            object[,] modalities = new object[modCount, 1];
 
            var tmpindex = 0;
            foreach(var mod in ModalityDocList)
            {
                modalities[tmpindex, 0] = mod.Value;
                tmpindex++;
                if (mod.Value == "MR")
                {
                    modalities[tmpindex, 0] = "CT+MR";
                    tmpindex++;
                }
            }

            var sumIndex = index + modCount - 1;

            range = ws.Range[ws.Cells[index, 3], ws.Cells[sumIndex, 3]];
            range.Value2 = modalities;

            range = ws.Range[ws.Cells[index, 2], ws.Cells[sumIndex, 2]];
            range.Rows.Merge();
            range.Value2 = "合計";

            range = ws.Range[ws.Cells[index, 2], ws.Cells[sumIndex, 3]];
            range.Borders.get_Item(XlBordersIndex.xlInsideHorizontal).LineStyle = XlLineStyle.xlDot;
            range.Borders.get_Item(XlBordersIndex.xlInsideVertical).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlDouble;
            range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).Weight = XlBorderWeight.xlMedium;
            range.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeRight).Weight = XlBorderWeight.xlMedium;
            range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).Weight = XlBorderWeight.xlMedium;
            range.HorizontalAlignment = (int)Microsoft.Office.Interop.Excel.Constants.xlCenter;
            range.RowHeight = 0;

            index = sumIndex;

            index++;
            sumIndex = index + modCount - 1;

            range = ws.Range[ws.Cells[index, 3], ws.Cells[sumIndex, 3]];
            range.Value2 = modalities;

            range = ws.Range[ws.Cells[index, 2], ws.Cells[sumIndex, 2]];
            range.Rows.Merge();
            range.Value2 = "読影";

            range = ws.Range[ws.Cells[index, 2], ws.Cells[sumIndex, 3]];
            range.Borders.get_Item(XlBordersIndex.xlInsideHorizontal).LineStyle = XlLineStyle.xlDot;
            range.Borders.get_Item(XlBordersIndex.xlInsideVertical).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeTop).Weight = XlBorderWeight.xlMedium;
            range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).Weight = XlBorderWeight.xlMedium;
            range.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeRight).Weight = XlBorderWeight.xlMedium;
            range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).Weight = XlBorderWeight.xlMedium;
            range.HorizontalAlignment = (int)Microsoft.Office.Interop.Excel.Constants.xlCenter;
            range.RowHeight = 0;

            index = sumIndex;

            index++;
            sumIndex = index + modCount -1;

            range = ws.Range[ws.Cells[index, 3], ws.Cells[sumIndex, 3]];
            range.Value2 = modalities;

            range = ws.Range[ws.Cells[index, 2], ws.Cells[sumIndex, 2]];
            range.Rows.Merge();
            range.Value2 = "システム";

            range = ws.Range[ws.Cells[index, 2], ws.Cells[sumIndex, 3]];
            range.Borders.get_Item(XlBordersIndex.xlInsideHorizontal).LineStyle = XlLineStyle.xlDot;
            range.Borders.get_Item(XlBordersIndex.xlInsideVertical).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeTop).Weight = XlBorderWeight.xlMedium;
            range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).Weight = XlBorderWeight.xlMedium;
            range.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeRight).Weight = XlBorderWeight.xlMedium;
            range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).Weight = XlBorderWeight.xlMedium;
            range.HorizontalAlignment = (int)Microsoft.Office.Interop.Excel.Constants.xlCenter;
            range.RowHeight = 0;

            index = sumIndex;

            range = ws.Range[ws.Cells[4, 3], ws.Cells[index, 3]];
            range.Copy();
            Range valueRangeH = ws.Range[ws.Cells[4, 4], ws.Cells[index, 4]];
            valueRangeH.PasteSpecial();
            valueRangeH.ClearContents();
            valueRangeH.ColumnWidth = 0;
            valueRangeH.HorizontalAlignment = (int)Microsoft.Office.Interop.Excel.Constants.xlRight;

        }

        private void CreateHeader(Worksheet ws, DateTime now)
        {
            DateTime Write = now;

            if(Write.Month > 10)
            {
                Write = Convert.ToDateTime(Write.ToString("yyyy") + "/10/01");
            }
            else if(Write.Month < 10)
            {
                Write = Convert.ToDateTime(Write.AddYears(-1).ToString("yyyy") + "/10/01");
            }

            int lastColumn = ws.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Column;
            Range range = null;
            string orderClass = "第" + ConfigurationManager.AppSettings["OrderClass"] + "期";

            if (lastColumn == 3)
            {
                lastColumn++;
            }
            else
            {
                range = ws.Cells[2, lastColumn - 12];
                orderClass = range.Value2;

                var val = orderClass.Replace("第", "").Replace("期", "");
                if(!string.IsNullOrEmpty(val))
                {
                    int cl = Convert.ToInt32(val);
                    cl++;
                    orderClass = "第" + cl + "期";
                    lastColumn--;
                }

            }

            lastColumn++;

            object[,] values = new object[1, 12];

            CultureInfo culture = new CultureInfo("ja-JP", true);
            culture.DateTimeFormat.Calendar = new JapaneseCalendar();
            string year = "";
            string month = "";
            DateTime start = Write;

            for (int i = 0; i < 12; i++)
            {
                Write = start.AddMonths(i);

                year = Write.ToString("yy", culture);
                month = Write.ToString("%M");

                values[0, i] = "H" + year + "." + month + "月";
            }

            int writeCol = lastColumn + 11;
            var iRow = GetRowIndex(ws, 3, "平均件数");

            iRow += 6;

            range = ws.Range[ws.Cells[2, lastColumn], ws.Cells[2, writeCol]];
            range.Columns.Merge();
            range.Value2 = orderClass;
            range.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
            range.HorizontalAlignment = (int)Microsoft.Office.Interop.Excel.Constants.xlCenter;

            range = ws.Range[ws.Cells[3, lastColumn], ws.Cells[3, writeCol]];
            range.Value2 = values;
            range.Borders.get_Item(XlBordersIndex.xlInsideVertical).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeTop).Weight = XlBorderWeight.xlMedium;
            range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).Weight = XlBorderWeight.xlMedium;
            range.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeRight).Weight = XlBorderWeight.xlMedium;
            range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).Weight = XlBorderWeight.xlMedium;
            range.HorizontalAlignment = (int)Microsoft.Office.Interop.Excel.Constants.xlCenter;

            range = ws.Range[ws.Cells[4, 4], ws.Cells[iRow + 2, 4]];
            range.Copy();
            Range valueRangeH = ws.Range[ws.Cells[4, lastColumn], ws.Cells[iRow + 2, writeCol]];
            valueRangeH.PasteSpecial();
            valueRangeH.Borders.get_Item(XlBordersIndex.xlInsideVertical).LineStyle = XlLineStyle.xlContinuous;

        }

        #endregion

        #region 日々の集計表

        public void WriteExcel_Sheet_3(DateTime search, int type)
        {
            //エクセルの宣言
            Application excelApp = null;
            Workbook masterBook = null;
            Worksheet masterSheet = null;

            bool modFlg = false;
            string modVal = "";
            string hosVal = "";

            try
            {
                CultureInfo culture = new CultureInfo("ja-JP", true);
                culture.DateTimeFormat.Calendar = new JapaneseCalendar();

                J_Year = search.ToString("yy", culture);
                Month = search.ToString("%M");

                string sheetName = search.ToString("yy") + "年" + search.Month + "月";
                int lastDay = DateTime.DaysInMonth(search.Year, search.Month);

                //出力ファイルのパスを作成
                string newExcel = Path.Combine(setting.masterPath, "日々の集計表(H" + J_Year + " " + Month + "月).xlsx");

                //Excelのアプリケーションを起動
                excelApp = new Application();

                if (!File.Exists(newExcel))
                {
                    masterBook = excelApp.Workbooks.Add();
                }
                else
                {
                    //Excelのブックをオープンする
                    masterBook = excelApp.Workbooks.Open(newExcel);
                }

                var index = GetSheetIndex(sheetName + "①", masterBook);

                if (index == 0)
                {
                    //シートをセレクト。
                    masterSheet = (Worksheet)masterBook.Worksheets.Add();
                    masterSheet.Select(Type.Missing);
                    masterSheet.Name = sheetName + "①";

                    CreateSheet(excelApp, masterSheet, lastDay);

                    masterSheet = (Worksheet)masterBook.Sheets[1];
                }
                else
                {
                    masterSheet = (Worksheet)masterBook.Sheets[index];
                    masterSheet.Select(Type.Missing);
                }

                string[] docmodalities = new string[ModalityDocList.Keys.Count + 1];

                int rowLen =RetdocList.Count + 1;
                int colLen = (HospDistMsts.Count * (ModalityList.Keys.Count + 1)) + (3 * (ModalityDocList.Keys.Count + 1));

                Range range = null;
                Range copyRange = masterSheet.Range[masterSheet.Cells[6, 1], masterSheet.Cells[6 + rowLen -1, 2 + colLen]];

                int sIndex = 6 + rowLen;

                string[] writeMod = new string[ ModalityList.Keys.Count + 1];
                int tmp = 0;
                foreach (var mod in ModalityList)
                {
                    writeMod[tmp] = mod.Value;
                    tmp++;
                    if(mod.Value == "MR")
                    {
                        writeMod[tmp] = "CT+MR";
                        tmp++;
                    }
                }
                tmp = 0;
                foreach (var mod in ModalityDocList)
                {
                    docmodalities[tmp] = mod.Value;
                    tmp++;
                    if (mod.Value == "MR")
                    {
                        docmodalities[tmp] = "CT+MR";
                        tmp++;
                    }
                }

                object[,] totalVal = new object[1, colLen];

                for (var i = 0; i < lastDay; i++ )
                {
                    int[] onedays = new int[colLen];
                    object[,] docNameVal = new object[RetdocList.Count, 1];
                    object[,] values = new object[rowLen, colLen];
                    int RowIndex = 0;
                    bool isDat = false;

                    string day = search.ToString("yyyyMM") + (i + 1).ToString().PadLeft(2,'0');

                    foreach(var doc in RetdocList)
                    {
                        string docName = doc.Name;
                        string docDisp = doc.Name_Disp;
                        int[] sumVal = new int[ModalityDocList.Keys.Count + 1];
                        int[] sumSysVal = new int[ModalityDocList.Keys.Count + 1];

                        bool isVal = false;
                        int valIndex = 0;

                        foreach (var hosp in HospDistMsts)
                        {
                            ExtReports val = new ExtReports();
                            val.ModReports = new List<ModReports>().ToArray();

                            foreach (var tmpv in HospReports)
                            {
                                if (tmpv.Cd == hosp.Hospital.Cd)
                                {
                                    val = tmpv;
                                    break;
                                }
                            }
                            for(int j = 0; j < writeMod.Length; j++)
                            {
                                int cnt = 0;
                                string docMod = writeMod[j];

                                if (writeMod[j] == "CT+MR")
                                {
                                    if (values[RowIndex, valIndex - 1] == null && values[RowIndex, valIndex - 2] == null)
                                        cnt = 0;
                                    else if (values[RowIndex, valIndex - 1] == null)
                                        cnt = (int)values[RowIndex, valIndex - 2];
                                    else if (values[RowIndex, valIndex - 2] == null)
                                        cnt = (int)values[RowIndex, valIndex - 1];
                                    else
                                        cnt = (int)values[RowIndex, valIndex - 1] + (int)values[RowIndex, valIndex - 2];
                                }
                                else
                                {
                                    foreach (var rep in val.ModReports)
                                    {
                                        if (writeMod[j] == rep.Mod)
                                        {
                                            foreach (var repval in rep.Reports)
                                            {
                                                if (repval.ReadDate != day || repval.ReadCd != docName || repval.PayFlg == 0)
                                                    continue;
                                                cnt++;
                                            }

                                            docMod = rep.Mod;
                                            if(!String.IsNullOrEmpty(rep.DocMod))
                                                docMod = rep.DocMod;
                                            else if (docMod == "NM")
                                                docMod = "RI";
                                        }
                                    }
                                }

                                if(cnt > 0)
                                {
                                    onedays[valIndex] = onedays[valIndex] + cnt;

                                    var tmpIndex = 0;

                                    for (var n = 0; n < docmodalities.Length; n++)
                                    {
                                        if(docmodalities[n] == docMod)
                                        {
                                            tmpIndex = n;
                                            break;
                                        }
                                    }


                                    if (totalVal[0, valIndex] != null)
                                        totalVal[0, valIndex] = (int)totalVal[0, valIndex] + cnt;
                                    else
                                        totalVal[0, valIndex] = cnt;

                                    values[RowIndex, valIndex] = cnt;

                                    switch (hosp.Dist)
                                    {
                                        case "1":
                                            sumSysVal[tmpIndex] = sumSysVal[tmpIndex] + cnt;
                                            break;
                                        default:
                                            sumVal[tmpIndex] = sumVal[tmpIndex] + cnt;
                                            break;
                                    }

                                    range = masterSheet.Columns[3 + valIndex];
                                    range.ColumnWidth = 5.75;


                                    isVal = true;
                                }

                                valIndex++;

                            }

                        }
                        for (var j = 0; j < sumVal.Length; j++)
                        {
                            int sum = 0;
                            int syssum = 0;

                            sum = sumVal[j];
                            syssum = sumSysVal[j];

                            if (sum + syssum > 0)
                            {
                                values[RowIndex, valIndex] = sum + syssum;
                                onedays[valIndex] = onedays[valIndex] + sum + syssum;

                                if (totalVal[0, valIndex] != null)
                                    totalVal[0, valIndex] = (int)totalVal[0, valIndex] + sum + syssum;
                                else
                                    totalVal[0, valIndex] = sum + syssum;

                                range = masterSheet.Columns[3 + valIndex];
                                range.ColumnWidth = 5.75;
                            }
                            if (sum > 0)
                            {
                                values[RowIndex, valIndex + ModalityDocList.Count + 1] = sum;
                                onedays[valIndex + ModalityDocList.Count + 1] = onedays[valIndex + ModalityDocList.Count + 1] + sum;
                                if (totalVal[0, valIndex + ModalityDocList.Count + 1] != null)
                                    totalVal[0, valIndex + ModalityDocList.Count + 1] = (int)totalVal[0, valIndex + ModalityDocList.Count + 1] + sum;
                                else
                                    totalVal[0, valIndex + ModalityDocList.Count + 1] = sum;
                                range = masterSheet.Columns[3 + valIndex + ModalityDocList.Count + 1];
                                range.ColumnWidth = 5.75;
                            }
                            if (syssum > 0)
                            {
                                values[RowIndex, valIndex + ((ModalityDocList.Count + 1) * 2)] = syssum;
                                onedays[valIndex + ((ModalityDocList.Count + 1) * 2)] = onedays[valIndex + ((ModalityDocList.Count + 1) * 2)] + syssum;
                                if (totalVal[0, valIndex + ((ModalityDocList.Count + 1) * 2)] != null)
                                    totalVal[0, valIndex + ((ModalityDocList.Count + 1) * 2)] = (int)totalVal[0, valIndex + ((ModalityDocList.Count + 1) * 2)] + syssum;
                                else
                                    totalVal[0, valIndex + ((ModalityDocList.Count + 1) * 2)] = syssum;

                                range = masterSheet.Columns[3 + valIndex + ((ModalityDocList.Count + 1) * 2)];
                                range.ColumnWidth = 5.75;
                            }

                            valIndex++;
                        }

                        if(isVal)
                        {
                            docNameVal[RowIndex, 0] = docDisp;
                            range = masterSheet.Rows[sIndex + RowIndex];
                            range.RowHeight = 13.5;
                            range = masterSheet.Cells[sIndex + RowIndex, 2];

                            RowIndex++;
                            isDat = true;
                        }
                    }

                    for (int j = 0; RowIndex + j < rowLen; j++ )
                    {
                        range = masterSheet.Rows[sIndex + RowIndex + j];
                        range.RowHeight = 0;
                    }


                    if (isDat)
                    {
                        copyRange.Copy();

                        range = masterSheet.Range[masterSheet.Cells[sIndex, 1], masterSheet.Cells[sIndex + rowLen -1, 2 + colLen]];
                        range.PasteSpecial();

                        range = masterSheet.Range[masterSheet.Cells[sIndex, 3], masterSheet.Cells[sIndex + RowIndex, 2 + colLen]];
                        range.Value2 = values;

                        object[,] tmpint = new object[1, onedays.Length];

                        for (int j = 0; j < onedays.Length; j++)
                        {
                            tmpint[0, j] = onedays[j];
                        }
                        range = masterSheet.Range[masterSheet.Cells[sIndex + rowLen - 1, 3], masterSheet.Cells[sIndex + rowLen - 1, 2 + colLen]];
                        range.Value2 = tmpint;
                        range.RowHeight = 13.5;

                        range = masterSheet.Range[masterSheet.Cells[sIndex, 2], masterSheet.Cells[sIndex + RowIndex, 2]];
                        range.Value2 = docNameVal;

                        range = masterSheet.Cells[sIndex, 1];
                        range = masterSheet.Range[masterSheet.Cells[sIndex, 1], masterSheet.Cells[sIndex + rowLen -1, 1]];
                        range.Merge();
                        range.Value2 = (i + 1) + "日";

                        sIndex = sIndex + rowLen;
                    }
                }

                range = masterSheet.Range[masterSheet.Cells[5, 3], masterSheet.Cells[5, 2 + colLen]];
                range.Value2 = totalVal;

                index = GetSheetIndex(sheetName + "②", masterBook);

                if (index == 0)
                {
                    //シートをセレクト。
                    masterSheet = (Worksheet)masterBook.Sheets[2];
                    masterSheet.Select(Type.Missing);
                    masterSheet.Name = sheetName + "②";
                }
                else
                {
                    masterSheet = (Worksheet)masterBook.Sheets[index];
                    masterSheet.Select(Type.Missing);
                }

                sIndex = 3;

                int newIndCol = 1;
                int newIndRow = 1;

                string[] titleval = new string[3];
                titleval[0] = "合計";
                titleval[1] = "読影";
                titleval[2] = "システム契約";

                while(sIndex < 2 + colLen)
                {
                    int start = sIndex;
                    int end = start + ModalityList.Count;

                    string HospName = "";

                    masterSheet = (Worksheet)masterBook.Sheets[1];
                    masterSheet.Select(Type.Missing);

                    range = masterSheet.Cells[3, start];
                    HospName = range.Value2;
                    if (titleval.Contains(HospName))
                    {
                        newIndRow = newIndRow + 4;
                        newIndCol = 1;
                        end = start + ModalityDocList.Count;
                    }
                    
                    int[] copyList = GetColIndexList(masterSheet, 5, start, end);

                    if(copyList == null || copyList.Length == 0)
                    {
                        sIndex = end + 1;
                    }

                    bool isFirst = true;

                    foreach (var ind in copyList)
                    {
                        masterSheet = (Worksheet)masterBook.Sheets[1];
                        Range Crange = masterSheet.Range[masterSheet.Cells[4, ind], masterSheet.Cells[5, ind]];
                        Crange.Copy();

                        masterSheet = (Worksheet)masterBook.Sheets[2];
                        range = masterSheet.Range[masterSheet.Cells[newIndRow + 1, newIndCol], masterSheet.Cells[newIndRow +2, newIndCol]];
                        range.PasteSpecial();

                        if (isFirst)
                        {
                            masterSheet = (Worksheet)masterBook.Sheets[1];
                            Crange = masterSheet.Cells[3, ind];
                            masterSheet = (Worksheet)masterBook.Sheets[2];
                            range = masterSheet.Cells[newIndRow, newIndCol];
                            range = masterSheet.Range[masterSheet.Cells[newIndRow, newIndCol], masterSheet.Cells[newIndRow, newIndCol + copyList.Length - 1]];
                            range.Merge();
                            range.Interior.ColorIndex = Crange.Interior.ColorIndex;
                            range.Value2 = HospName;
                            range.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlContinuous;
                            range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle = XlLineStyle.xlContinuous;
                            range.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
                            range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
                            range.HorizontalAlignment = (int)Microsoft.Office.Interop.Excel.Constants.xlCenter;

                            isFirst = false;
                        }
                        newIndCol++;
                    }

                    if (newIndCol >= 20)
                    {
                        newIndRow = newIndRow + 4;
                        newIndCol = 1;
                    }

                    sIndex = end + 1;
                }


                if (!File.Exists(newExcel))
                {
                    masterBook.SaveAs(newExcel);
                }
                else
                {
                    excelApp.ActiveWorkbook.Save();
                }
            }
            catch (Exception e)
            {
                LogUtil.Error(e.Message);

                if (modFlg)
                    LogUtil.Error("対象モダリティ無 " + hosVal + "：" + modVal);

            }
            finally
            {
                masterSheet = null;
                if (masterBook != null)
                    masterBook.Close(false);
                if (excelApp != null)
                    excelApp.Quit();
            }
        }

        private void CreateSheet(Application app, Worksheet ws, int day)
        {
            int lastRowDay = 6 + (RetdocList.Count);
            int lastColHos = 3 + (HospDistMsts.Count * (ModalityList.Keys.Count + 1));

            Range range = null;
            range = ws.Cells[6, 3];
            range.Select();
            app.ActiveWindow.FreezePanes = true;

            range = ws.Range[ws.Cells[6, 1], ws.Cells[lastRowDay, 2]];
            range.Borders.get_Item(XlBordersIndex.xlInsideHorizontal).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlInsideVertical).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlInsideVertical).Weight = XlBorderWeight.xlMedium;
            range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).Weight = XlBorderWeight.xlMedium;
            range.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeRight).Weight = XlBorderWeight.xlMedium;
            range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).Weight = XlBorderWeight.xlMedium;



            int index = 3;
            object[,] values = new object[1, (HospDistMsts.Count * (ModalityList.Keys.Count + 1))];

            range = ws.Range[ws.Cells[4, 3], ws.Cells[4, index + ModalityList.Keys.Count + 1]];

            int modindex = 0;
            for (var i = 0; i < HospDistMsts.Count; i++)
            {
                range = ws.Range[ws.Cells[3, index], ws.Cells[lastRowDay, index + ModalityList.Keys.Count]];
                range.Borders.get_Item(XlBordersIndex.xlInsideHorizontal).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlInsideVertical).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).Weight = XlBorderWeight.xlMedium;
                range.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeRight).Weight = XlBorderWeight.xlMedium;
                range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).Weight = XlBorderWeight.xlMedium;

                range = ws.Range[ws.Cells[3, index], ws.Cells[3, index + ModalityList.Keys.Count]];
                range.Merge();
                range.Value2 = HospDistMsts[i].Hospital.Name_Disp;

                range.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeTop).Weight = XlBorderWeight.xlMedium;
                range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).Weight = XlBorderWeight.xlMedium;
                range.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeRight).Weight = XlBorderWeight.xlMedium;
                range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).Weight = XlBorderWeight.xlMedium;
                range.HorizontalAlignment = (int)Microsoft.Office.Interop.Excel.Constants.xlCenter;

                range = ws.Range[ws.Cells[3, index], ws.Cells[5, index + ModalityList.Keys.Count]];

                range.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeTop).Weight = XlBorderWeight.xlMedium;
                range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).Weight = XlBorderWeight.xlMedium;
                range.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeRight).Weight = XlBorderWeight.xlMedium;
                range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).Weight = XlBorderWeight.xlMedium;
                range.HorizontalAlignment = (int)Microsoft.Office.Interop.Excel.Constants.xlCenter;

                int color = i % 5 + 35;

                if (HospDistMsts[i].Dist == "1")
                    range = ws.Range[ws.Cells[4, index], ws.Cells[5, index + ModalityList.Keys.Count]];

                range.Interior.ColorIndex = color;
                range.ColumnWidth = 0;

                range = ws.Range[ws.Cells[4, index + 2], ws.Cells[lastRowDay, index + 2]];
                range.Interior.ColorIndex = color;
                range = ws.Range[ws.Cells[6, index + 2], ws.Cells[lastRowDay, index + 2]];
                range.RowHeight = 0;

                foreach (var mod in ModalityList)
                {
                    values[0, modindex] = mod.Value;
                    index++;
                    modindex++;
                    if (mod.Value == "MR")
                    {
                        values[0, modindex] = "CT+MR";
                        range = ws.Range[ws.Cells[4, index], ws.Cells[lastRowDay, index]];
                        range.Interior.ColorIndex = color;
                        index++;
                        modindex++;
                    }
                }
            }
            range = ws.Range[ws.Cells[4, 3], ws.Cells[4, lastColHos]];
            range.Value2 = values;

            modindex = 0;

            values = new object[1, (ModalityDocList.Keys.Count + 1)];
            foreach (var mod in ModalityDocList)
            {
                values[0, modindex] = mod.Value + "計";
                modindex++;

                if (mod.Value == "MR")
                {
                    values[0, modindex] = "計";
                    modindex++;
                }
            }
            string[] titleval = new string[3];
            titleval[0] = "合計";
            titleval[1] = "読影";
            titleval[2] = "システム契約";

            int valIndex = index;

            for(var i = 0; i< titleval.Length; i++)
            {
                range = ws.Range[ws.Cells[3, index], ws.Cells[lastRowDay, index + ModalityDocList.Keys.Count]];
                range.Borders.get_Item(XlBordersIndex.xlInsideHorizontal).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlInsideVertical).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).Weight = XlBorderWeight.xlMedium;
                range.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeRight).Weight = XlBorderWeight.xlMedium;
                range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).Weight = XlBorderWeight.xlMedium;

                range = ws.Range[ws.Cells[3, index], ws.Cells[3, index + ModalityDocList.Keys.Count]];
                range.Merge();
                range.Value2 = titleval[i];

                range.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeTop).Weight = XlBorderWeight.xlMedium;
                range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).Weight = XlBorderWeight.xlMedium;
                range.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeRight).Weight = XlBorderWeight.xlMedium;
                range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).Weight = XlBorderWeight.xlMedium;
                range.HorizontalAlignment = (int)Microsoft.Office.Interop.Excel.Constants.xlCenter;

                range = ws.Range[ws.Cells[3, index], ws.Cells[5, index + ModalityDocList.Keys.Count]];

                range.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeTop).Weight = XlBorderWeight.xlMedium;
                range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).Weight = XlBorderWeight.xlMedium;
                range.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeRight).Weight = XlBorderWeight.xlMedium;
                range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
                range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).Weight = XlBorderWeight.xlMedium;
                range.HorizontalAlignment = (int)Microsoft.Office.Interop.Excel.Constants.xlCenter;

                int color = i % 3 + 44;

                range.Interior.ColorIndex = color;
                range.ColumnWidth = 0;

                range = ws.Range[ws.Cells[4, index + 2], ws.Cells[lastRowDay, index + 2]];
                range.Interior.ColorIndex = color;

                range = ws.Range[ws.Cells[4, index], ws.Cells[4, index + ModalityDocList.Keys.Count]];
                range.Value2 = values;

                index = index + ModalityDocList.Keys.Count + 1;
            }

            range = ws.Range[ws.Cells[lastRowDay, 1], ws.Cells[lastRowDay, index - 1]];
            range.Interior.ColorIndex = 36;
            range.RowHeight = 0;
            range.Borders.get_Item(XlBordersIndex.xlInsideVertical).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlDouble;
            range.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeRight).Weight = XlBorderWeight.xlMedium;
            range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).Weight = XlBorderWeight.xlMedium;
            range.HorizontalAlignment = (int)Microsoft.Office.Interop.Excel.Constants.xlCenter;

        }

        #endregion

        #region 読影医別集計

        public void WriteExcel_Sheet_4(DateTime search, int type)
        {
            //エクセルの宣言
            Application excelApp = null;
            Workbook masterBook = null;
            Worksheet masterSheet = null;

            try
            {
                CultureInfo culture = new CultureInfo("ja-JP", true);
                culture.DateTimeFormat.Calendar = new JapaneseCalendar();

                J_Year = search.ToString("yy", culture);
                Month = search.ToString("%M");

                lastRow = 4 + (HospReports.Count * ModalityList.Keys.Count) + ModalityDocList.Keys.Count * 3;

                DateTime copyDate = search.AddMonths(-1);

                var Y_J_Year = copyDate.ToString("yy", culture);
                var Y_Month = copyDate.ToString("%M");

                //出力ファイルのパスを作成
                string copyExcel = Path.Combine(setting.masterPath, "読影医集計(原本).xlsx");
                string newExcel = Path.Combine(setting.masterPath, "読影医集計(H" + J_Year + " " + Month + "月).xlsx");

                //Excelのアプリケーションを起動
                excelApp = new Application();

                if (!File.Exists(copyExcel))
                {
                    return;
                }
                else
                {
                    if (File.Exists(newExcel))
                        File.Delete(newExcel);

                    File.Copy(copyExcel, newExcel);

                    //Excelのブックをオープンする
                    masterBook = excelApp.Workbooks.Open(newExcel);
                }

                masterSheet = (Worksheet)masterBook.Sheets[1];
                //シートをセレクト。
                masterSheet.Select(Type.Missing);
                masterSheet.Name = "H" + J_Year + "." + Month + "月";

                Dictionary<string, object[,]> sumVal = new Dictionary<string, object[,]>();
                Dictionary<string, object[,]> sumSysVal = new Dictionary<string, object[,]>();
                Range range = null;

                List<string> DataDocList = new List<string>();
                int ind = GetColIndex(masterSheet, 3, "計");
                int rowind = 3;
                while(ind == 0)
                {
                    rowind++;
                    ind = GetColIndex(masterSheet, rowind, "計");
                }

                string[] tmpDoc = GetColValueList(masterSheet, rowind, 3, ind - 1);
                
                foreach(var doc in tmpDoc)
                {
                    var ret = "";

                    foreach(var orgDoc in RetdocList)
                    {
                        if(doc == orgDoc.Name_Disp)
                        {
                            ret = orgDoc.Name;
                            break;
                        }
                    }

                    DataDocList.Add(ret);
                }

                foreach (var mod in ModalityDocList)
                {
                    sumVal[mod.Value] = new object[1, DataDocList.Count];

                    if(mod.Value != "MG")
                        sumSysVal[mod.Value] = new object[1, DataDocList.Count];
                }
                sumVal["乳腺加算"] = new object[1, DataDocList.Count];

                int index = 0;
                foreach (var doc in DataDocList)
                {
                    int mgAdd = 0;

                    foreach (var hosp in HospDistMsts)
                    {
                        ExtReports val = new ExtReports();
                        val.ModReports = new List<ModReports>().ToArray();
                        string[] tmpval = hosp.Hospital.Name.Split('-');

                        foreach (var tmp in HospReports)
                        {
                            if (tmp.Cd == hosp.Hospital.Cd)
                            {
                                val = tmp;
                                break;
                            }
                        }

                        for (var i = 0; i < val.ModReports.Length; i++)
                        {
                            var cnt = 0;
                            var extraCnt = 0;

                            foreach (var rep in val.ModReports[i].Reports)
                            {
                                if (rep.ClaimFlg == 0 || doc != rep.ReadCd)
                                    continue;

                                if (hosp.Dist == "1" && rep.ClaimFlg == 1 && rep.PayFlg == 0)
                                    extraCnt++;
                                else
                                    cnt++;

                                if (rep.AddMGFlg == 1)
                                    mgAdd++;
                            }

                            if (cnt > 0)
                            {

                                string modval = val.ModReports[i].Mod;

                                if (!string.IsNullOrEmpty(val.ModReports[i].DocMod))
                                    modval = val.ModReports[i].DocMod;
                                else if (modval == "NM")
                                    modval = "RI";

                                if (modval == "MG")
                                {
                                    if (sumVal[modval][0, index] == null)
                                        sumVal[modval][0, index] = cnt;
                                    else
                                        sumVal[modval][0, index] = (int)sumVal[modval][0, index] + cnt;
                                }
                                else
                                {
                                    if(hosp.Dist == "1")
                                    {
                                        if (sumSysVal[modval][0, index] == null)
                                            sumSysVal[modval][0, index] = cnt;
                                        else
                                            sumSysVal[modval][0, index] = (int)sumSysVal[modval][0, index] + cnt;
                                    }
                                    else
                                    {
                                        if (sumVal[modval][0, index] == null)
                                            sumVal[modval][0, index] = cnt;
                                        else
                                            sumVal[modval][0, index] = (int)sumVal[modval][0, index] + cnt;
                                    }
                                }
                            }
                            if (extraCnt > 0)
                            {
                                string modval = val.ModReports[i].Mod;

                                if (!string.IsNullOrEmpty(val.ModReports[i].DocMod))
                                    modval = val.ModReports[i].DocMod;
                                else if (modval == "NM")
                                    modval = "RI";

                                if (sumVal[modval][0, index] == null)
                                    sumVal[modval][0, index] = extraCnt;
                                else
                                    sumVal[modval][0, index] = (int)sumVal[modval][0, index] + extraCnt;
                            }

                        }
                    }
                    if (mgAdd > 0)
                    {
                        if (sumVal["乳腺加算"][0, index] == null)
                            sumVal["乳腺加算"][0, index] = mgAdd;
                        else
                            sumVal["乳腺加算"][0, index] = (int)sumVal["乳腺加算"][0, index] + mgAdd;
                    } 
                    index++;
                }

                range = masterSheet.Cells[1, 9];
                range.Value2 = Month;

                foreach(var sum in sumVal)
                {
                    int row = GetRowIndex(masterSheet, 1, sum.Key);

                    range = masterSheet.Range[masterSheet.Cells[row + 1, 3], masterSheet.Cells[row + 1, 3 + DataDocList.Count -1]];
                    range.Value2 = sum.Value;
                }
                foreach (var sum in sumSysVal)
                {
                    int row = GetRowIndex(masterSheet, 1, sum.Key);

                    range = masterSheet.Range[masterSheet.Cells[row + 3, 3], masterSheet.Cells[row + 3, 3 + DataDocList.Count - 1]];
                    range.Value2 = sum.Value;
                }

                if (!File.Exists(newExcel))
                {
                    masterBook.SaveAs(newExcel);
                }
                else
                {
                    excelApp.ActiveWorkbook.Save();
                }
            }
            catch (Exception e)
            {
                LogUtil.Error(e.Message);

            }
            finally
            {
                masterSheet = null;
                if (masterBook != null)
                    masterBook.Close(false);
                if (excelApp != null)
                    excelApp.Quit();
            }
        }


        #endregion

        #region 平均
        public void WriteExcel_Sheet_6(DateTime search, int type)
        {
            //エクセルの宣言
            Application excelApp = null;
            Workbook masterBook = null;
            Worksheet masterSheet = null;

            bool modFlg = false;
            string modVal = "";
            string hosVal = "";

            try
            {
                CultureInfo culture = new CultureInfo("ja-JP", true);
                culture.DateTimeFormat.Calendar = new JapaneseCalendar();

                J_Year = search.ToString("yy", culture);
                Month = search.ToString("%M");

                string sheetName = search.ToString("yy") + "." + search.Month;
                int lastDay = DateTime.DaysInMonth(search.Year, search.Month);

                //出力ファイルのパスを作成
                string newExcel = Path.Combine(setting.masterPath, "件数推移表(第12期).xlsx");

                //Excelのアプリケーションを起動
                excelApp = new Application();
                excelApp.DisplayAlerts = false;
                if (!File.Exists(newExcel))
                {
                    masterBook = excelApp.Workbooks.Add();
                }
                else
                {
                    //Excelのブックをオープンする
                    masterBook = excelApp.Workbooks.Open(newExcel);
                }

                List<int> sDates = new List<int>();
                var index = GetSheetIndex("休日", masterBook);

                masterSheet = (Worksheet)masterBook.Sheets[index];
                masterSheet.Select(Type.Missing);

                int lastRow = masterSheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Row;

                for (int i = 3; i <= lastRow; i++)
                {
                    var val = masterSheet.Cells[i, 2];

                    if(val.Value2 == null || val.Value2.ToString() == "")
                        break;

                    if (val.Value.Month == search.Month)
                    {
                        sDates.Add(Convert.ToInt32(val.Value.Day));
                    }
                }

                index = GetSheetIndex(sheetName, masterBook);

                if (index == 0)
                {
                    index = GetSheetIndex("原本", masterBook);
                    var master = (Worksheet)masterBook.Sheets[index];

                    master.Copy(master);

                    //シートをセレクト。
                    masterSheet = (Worksheet)masterBook.Sheets[index];
                    masterSheet.Select(Type.Missing);
                    masterSheet.Name = sheetName;
                }
                else
                {
                    masterSheet = (Worksheet)masterBook.Sheets[index];
                    masterSheet.Select(Type.Missing);
                }

                Range range = null;

                range = masterSheet.Cells[1, 2];
                range.Value2 = search.ToString("yy") + "年" + search.Month + "月の平均";

                int weekCnt = 0;
                int sCnt = 0;
                for (var i = 0; i < lastDay; i++)
                {
                    int[,] onedays = new int[1,3];

                    string day = search.ToString("yyyyMM") + (i + 1).ToString().PadLeft(2, '0');

                    DateTime now = new DateTime(search.Year, search.Month, i + 1);

                    foreach (var tmpv in HospReports)
                    {
                        foreach (var rep in tmpv.ModReports)
                        {
                            int cnt = 0;

                            if ("CT" == rep.Mod)
                            {
                                foreach (var repval in rep.Reports)
                                {
                                    if (repval.ReadDate != day || repval.PayFlg == 0)
                                        continue;
                                    cnt++;
                                }

                                onedays[0, 0] += cnt;
                            }
                            else if("MR" == rep.Mod)
                            {
                                foreach (var repval in rep.Reports)
                                {
                                    if (repval.ReadDate != day || repval.PayFlg == 0)
                                        continue;
                                    cnt++;
                                }

                                onedays[0, 1] += cnt;
                            }
                        }

                        onedays[0, 2] = onedays[0, 0] + onedays[0, 1];
                    }

                    if(sDates.Contains(i + 1))
                    {
                        var dayCell = masterSheet.Cells[16 + sCnt, 8];
                        dayCell.Value2 = (i + 1) + "日";

                        range = masterSheet.Range[masterSheet.Cells[16 + sCnt, 9], masterSheet.Cells[16 + sCnt, 11]];
                        sCnt++;
                        if ((int)now.DayOfWeek == 6)
                            weekCnt++;
                    }
                    else
                    {
                        switch ((int)now.DayOfWeek)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                                var col = 2 + (3 * ((int)now.DayOfWeek -1));
                                range = masterSheet.Range[masterSheet.Cells[5 + weekCnt, col], masterSheet.Cells[5 + weekCnt, col + 2]];
                                break;
                            case 6:
                                range = masterSheet.Range[masterSheet.Cells[16 + weekCnt, 5], masterSheet.Cells[16 + weekCnt, 7]];
                                weekCnt++;
                                break;
                            default:
                                range = masterSheet.Cells[16 + sCnt, 8];
                                range.Value2 = (i + 1) + "日";

                                range = masterSheet.Range[masterSheet.Cells[16 + sCnt, 9], masterSheet.Cells[16 + sCnt, 11]];
                                sCnt++;
                                break;
                        }
                    }
                    if(onedays[0,2] > 0)
                        range.Value2 = onedays;
                }

                index = GetSheetIndex("件数推移表", masterBook);

                masterSheet = (Worksheet)masterBook.Sheets[index];
                masterSheet.Select(Type.Missing);
                var iRow = GetRowIndex(masterSheet, 3, "平均件数");
                var lastCol = masterSheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Column;
                var mon = "H" + J_Year + "." + Month + "月";
                var iCol = GetColIndex(masterSheet, iRow, mon);
                if(iCol == 0)
                {
                    var bMonth = search.AddMonths(-1).ToString("%M");
                    var bYear = search.AddMonths(-1).ToString("yy", culture);
                    var bmon = "H" + bYear + "." + bMonth + "月";
                    iCol = GetColIndex(masterSheet, iRow, bmon);
                    iCol++;
                    range = masterSheet.Cells[iRow, iCol];
                    range.Value2 = mon;
                }

                range = masterSheet.Cells[iRow + 1, iCol];
                range.Value2 = "='" + sheetName + "'!S28";
                range = masterSheet.Cells[iRow + 2, iCol];
                range.Value2 = "='" + sheetName + "'!S29";
                range = masterSheet.Cells[iRow + 3, iCol];
                range.Value2 = "='" + sheetName + "'!S30";
                range = masterSheet.Cells[iRow + 5, iCol];
                range.Value2 = "='" + sheetName + "'!S32";
                range = masterSheet.Cells[iRow + 6, iCol];
                range.Value2 = "='" + sheetName + "'!S33";

                if (!File.Exists(newExcel))
                {
                    masterBook.SaveAs(newExcel);
                }
                else
                {
                    excelApp.ActiveWorkbook.Save();
                }
            }
            catch (Exception e)
            {
                LogUtil.Error(e.Message);

                if (modFlg)
                    LogUtil.Error("対象モダリティ無 " + hosVal + "：" + modVal);

            }
            finally
            {
                masterSheet = null;
                if (masterBook != null)
                    masterBook.Close(false);
                if (excelApp != null)
                    excelApp.Quit();
            }
        }

        #endregion

        #region 弥生連携

        public void WriteExcel_Sheet_5(DateTime search, int type)
        {
            string outFile = ConfigurationManager.AppSettings["OUTFILE"];

            try
            {
                string writeVal = null;

                CultureInfo culture = new CultureInfo("ja-JP", true);
                culture.DateTimeFormat.Calendar = new JapaneseCalendar();

                J_Year = search.ToString("yy", culture);
                Month = search.ToString("%M");

                int lastDay = DateTime.DaysInMonth(search.Year, search.Month);

                foreach (var hosp in HospDistMsts)
                {
                    if (!YayoiHosps.Contains(hosp.Hospital.Name_DB))
                        continue;

                    if (hosp.Yayoi == null || hosp.Yayoi.Length < 5)
                        continue;

                    ExtReports val = new ExtReports();
                    val.ModReports = new List<ModReports>().ToArray();

                    foreach (var tmp in HospReports)
                    {
                        if (tmp.Cd == hosp.Hospital.Cd)
                        {
                            val = tmp;
                            break;
                        }
                    }


                    string hoscd = hosp.Yayoi[0];
                    string hosname = hosp.Yayoi[1];
                    string hosname_S = hosp.Yayoi[2];
                    string cost = hosp.Yayoi[3];

                    List<KaikeiMst> YList = new List<KaikeiMst>();

                    for(int i = 4; i < hosp.Yayoi.Length; i++)
                    {
                        KaikeiMst tmp = new KaikeiMst();

                        List<string> vals = hosp.Yayoi[i].Split(':').ToList();

                        if (String.IsNullOrEmpty(vals[0]))
                            continue;

                        tmp.No = Convert.ToInt32(vals[0]);

                        vals.RemoveAt(0);

                        tmp.Values = vals.ToArray();

                        YList.Add(tmp);
                    }

                    YList.Sort((a, b) => a.No - b.No);

                    int seqNo = 0;

                    foreach(var kai in YList)
                    {
                        int cnt = 0;

                        if (kai.Values == null || kai.Values.Length < 5)
                            continue;

                        switch(kai.Values[4])
                        {
                            //手入力
                            case "0":
                                if (!int.TryParse(kai.Values[6], out cnt))
                                    cnt = 1;
                                break;
                            //モダリティ
                            case "1":
                                for (var i = 0; i < val.ModReports.Length; i++)
                                {
                                    if (val.ModReports[i].Mod == kai.Values[5])
                                    {
                                        foreach(var rep in val.ModReports[i].Reports)
                                        {
                                            if (rep.PayFlg == 1)
                                                cnt++;
                                        }
                                        break;
                                    }
                                }
                                break;
                            //画像加算
                            case "2":
                                for (var i = 0; i < val.ModReports.Length; i++)
                                {
                                    foreach(var rep in val.ModReports[i].Reports)
                                    {
                                        if (rep.PayFlg == 1)
                                            cnt += rep.AddImageFlg;
                                    }
                                }
                                break;
                            //緊急画像加算
                            case "3":
                                for (var i = 0; i < val.ModReports.Length; i++)
                                {
                                    foreach(var rep in val.ModReports[i].Reports)
                                    {
                                        if (rep.PriorityFlg == 1&& rep.PayFlg == 1)
                                            cnt++;
                                    }
                                }
                                break;
                            //ﾒｰﾙ加算
                            case "4":
                                for (var i = 0; i < val.ModReports.Length; i++)
                                {
                                    foreach (var rep in val.ModReports[i].Reports)
                                    {
                                        if (rep.MailFlg == 1 && rep.PayFlg == 1)
                                            cnt++;
                                    }
                                }
                                break;
                            //部位追加
                            case "5":
                                for (var i = 0; i < val.ModReports.Length; i++)
                                {
                                    if (val.ModReports[i].Mod == kai.Values[5])
                                    {
                                        foreach (var rep in val.ModReports[i].Reports)
                                        {
                                            if (String.IsNullOrEmpty(kai.Values[6]))
                                            {
                                                if (rep.BodyPartFlg == 0 && rep.PayFlg == 1)
                                                    cnt++;
                                            }
                                            else
                                            {
                                                if (rep.BodyPartFlg == 1 && rep.PayFlg == 1)
                                                    cnt++;
                                            }
                                        }
                                        break;
                                    }
                                }

                                break;
                        }

                        if (cnt == 0)
                            continue;

                        seqNo++;

                        writeVal += "\"1\"";
                        writeVal += ",\"1\"";
                        writeVal += ",\"0\"";
                        writeVal += ",\"" + J_Year + search.ToString("MM") + lastDay.ToString().PadLeft(2, '0') + "\"";
                        writeVal += ",\"00000000\"";
                        writeVal += ",\"24\"";
                        writeVal += ",\"1\"";
                        writeVal += ",\"" + kai.Values[3] + "\"";
                        writeVal += ",\"3\"";
                        writeVal += ",\"3\"";
                        writeVal += ",\"" + hoscd + "\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"" + seqNo + "\"";
                        writeVal += ",\"1\"";
                        writeVal += ",\"" + kai.Values[0] + "\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"" + kai.Values[1] + "\"";
                        writeVal += ",\"" + cost + "\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"0\"";
                        writeVal += ",\"0\"";
                        writeVal += ",\"0000\"";
                        writeVal += ",\"" + cnt + "\"";
                        writeVal += ",\"" + kai.Values[2] + "\"";
                        writeVal += ",\"" + (int)(cnt * Convert.ToInt32(kai.Values[2])) + "\"";
                        writeVal += ",\"\"";

                        int vcost = (int)(cnt * Convert.ToInt32(kai.Values[2]));

                        if(kai.Values[3] == "3")
                        {
                            double add = 0.0;
                            switch(cost)
                            {
                                case "10":
                                    add = 3.0;
                                    break;
                                case "11":
                                    add = 5.0;
                                    break;
                                case "12":
                                    add = 8.0;
                                    break;
                                case "13":
                                    add = 10.0;
                                    break;
                                case "20":
                                    add = 6.0;
                                    break;
                                case "21":
                                    add = 4.5;
                                    break;
                                case "22":
                                    add = 3.0;
                                    break;
                                case "70":
                                case "80":
                                case "90":
                                    break;
                            }

                            vcost = (int)((double)vcost - (double)(vcost * (add / 100)));
                        }

                        writeVal += ",\"" + vcost + "\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"0\"";
                        writeVal += ",\"0\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"" + hosname + "\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"" + hosname_S + "\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += ",\"\"";
                        writeVal += "\r\n";
                    }
                }
                
                if (File.Exists(outFile))
                    File.Delete(outFile);

                using (var ws = new StreamWriter(outFile, false, Encoding.GetEncoding("shift_jis")))
                {
                    ws.Write(writeVal);
                }

            }
            catch (Exception e)
            {

            }
        }

        #endregion

        private int GetRowIndex(Worksheet ws, int col, string val)
        {
            int lastRow = ws.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Row;

            Range range = null;
            for (int i = 1; i <= lastRow; i++)
            {
                range = ws.Cells[i, col];

                if (range.Value2 == val)
                {
                    return i;
                }
            }

            return 0;
        }

        private int GetRowIndex(Worksheet ws, int index, int col, string val)
        {
            Range range = null;
            for (int i = 1; i <= index; i++)
            {
                range = ws.Cells[i, col];

                if (range.Value2 == val)
                {
                    return i;
                }
            }

            return 0;
        }

        private int GetColIndex (Worksheet ws, int row, string val)
        {
            int lastColumn = ws.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Column;

            Range range = null;
            for (int i = 1; i <= lastColumn; i++)
            {
                range = ws.Cells[row, i];

                if (range.Value2 == null)
                    continue;

                if (range.Value2.ToString() == val)
                {
                    return i;
                }
            }

            return 0;
        }

        private int[] GetColIndexList(Worksheet ws, int row, int start, int end)
        {
            int lastColumn = ws.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Column;

            List<int> retList = new List<int>();

            Range range = null;
            for (int i = start; i <= end; i++)
            {
                range = ws.Cells[row, i];

                if (range.Value2 != null)
                {
                    retList.Add(i);
                }
            }

            return retList.ToArray();
        }

        private string[] GetColValueList(Worksheet ws, int row, int start, int end)
        {
            int lastColumn = ws.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Column;

            List<string> retList = new List<string>();

            Range range = null;
            for (int i = start; i <= end; i++)
            {
                range = ws.Cells[row, i];

                if (range.Value2 != null)
                {
                    retList.Add(range.Value2);
                }
            }

            return retList.ToArray();
        }

        private string[] GetHeaderVal(Worksheet masterSheet)
        {
            int lastColumn = masterSheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell).Column;

            List<string> vals = new List<string>();

            for(int i = 1; i <= lastColumn; i++)
            {
                Range tmp = masterSheet.Cells[1, i];

                vals.Add(tmp.Value);
            }

            return vals.ToArray();
        }
    }
}
