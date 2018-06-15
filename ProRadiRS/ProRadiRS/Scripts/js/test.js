/* グローバル宣言 */
var $ReportConfig;              // レポート情報格納オブジェクト
var $cmbUserList;               // 依頼者絞り込みコンボボックスオブジェクト
var $LoginUserCd;               // ログインユーザーコード
var $IsTempSave;                // 一時保存の有無(読み出し時にもON)

var $SentenceDlg;
var $ChangeHistoryDlg;

/***************************************************
/* 起動処理 */
$(window).load(function () {
    // 起動処理
    TestWindow_Init();
});

function TestWindow_Init() {
    // オブジェクトキャッシュ
    $ReportConfig = $("#ReportConfig");
    $cmbUserList = $("#cmbUserList");

    $SentenceDlg = $("#SentenceDlg")
    $ChangeHistoryDlg = $("#ChangeHistoryDlg")

    // HTTP通信開始
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/GetParams",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: TestWindow_Init_Result,
        error: function (result) {
            // エラー
            alert(result.d);
        }
    });

    $LoginUserCd = $ReportConfig.data("usercd");

    // HTTP通信開始(他ユーザーリスト取得)
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/GetUserList",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: SearchWindow_Init_Result_UserList,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
            return;
        }
    });

    if ($LoginUserCd > 0) {
        Narrow2();
    }
}
function TestWindow_Init_Result(result) {
    // エラー判定
    if (result.d == null || result.d == undefined || result.d.Result == "Error") {
        alert("サービスでエラーが発生しました。");
        return;
    }

    // パラメータ設定
    $ReportConfig.data("serialno", ("serialno" in result.d.Params) ? result.d.Params.serialno : "");
    $ReportConfig.data("orderno", ("orderno" in result.d.Params) ? result.d.Params.orderno : "");
    $ReportConfig.data("patientid", ("patientid" in result.d.Params) ? result.d.Params.patientid : "");
    $ReportConfig.data("studydate", ("studydate" in result.d.Params) ? result.d.Params.studydate : "");
    $ReportConfig.data("modality", ("modality" in result.d.Params) ? result.d.Params.modality : "");
    $ReportConfig.data("officecd", ("officecd" in result.d.Params) ? result.d.Params.officecd : "");
    $ReportConfig.data("ipromedcode", ("ipromedcode" in result.d.Params) ? result.d.Params.ipromedcode : "");
    // 一時保存データ読み出し判定
    $ReportConfig.data("readtempsave", ("readtempsave" in result.d.Params) ? result.d.Params.readtempsave : "");
    // 一時保存データ読み出しで
    if ($ReportConfig.data("readtempsave") != "") {
        $IsTempSave = $ReportConfig.data("readtempsave");
    }
    else {
        $IsTempSave = "0";
    }

    //キー画像最大枚数
    $ReportConfig.data("MaxImageImportNum", ("MaxImageImportNum" in result.d.Params) ? result.d.Params.MaxImageImportNum : "");
    //確定時に、キー画像が必要なモダリティの確認用
    $ReportConfig.data("ImgCheckModality", ("ImgCheckModality" in result.d.Params) ? result.d.Params.ImgCheckModality : "");

    // 2018/02/22 新規追加
    $ReportConfig.data("usercd", ("usercd" in result.d.Params) ? result.d.Params.usercd : "");
    
}

function SearchWindow_Init_Result_UserList(result) {
    // エラー判定
    if (result.d == null || result.d == undefined || result.d.Result == "Error") {
        alert("サービスでエラーが発生しました。");
        return;
    }

    var list = result.d.UserList;
    // 依頼者絞り込みコンボボックスの初期化
    $cmbUserList.children().remove();
    // 依頼者絞り込みコンボボックスに要素追加
    // 先頭に空を追加
    $cmbUserList.append($("<option>").html("&mdash;&mdash;&mdash;").val(""));
    for (var i = 0; i < list.length; i++) {
        if ($LoginUserCd != list[i].UserCD) {
            $cmbUserList.append($("<option>").html(list[i].UserName).val(list[i].UserCD));
        } else {
            $cmbUserList.append($("<option selected>").html(list[i].UserName).val(list[i].UserCD));
        }
    }
}

/***************************************************
/* 定型文ボタンイベント */
function Sentence2() {
    $SentenceDlg.dialog({
        closeText: "",
        modal: true,
        draggable: true,
        resizable: true,
        width: 500,
        height: 600,
        position: { my: "right top", at: "right top", of: window }
    });

    //$SentenceDlg.resizable();
}

/* 絞込み */
function Narrow2() {
    // 選択テキスト取得
    var UserName = $cmbUserList.children(":selected").text();
    var UserCD = $cmbUserList.children(":selected").val();

    if (UserCD == "") {
        UserCD = -1;
    }
    GetSentenceData2(UserCD, -1);
}

function GetSentenceData2(usercd, groupcd) {
    // HTTP通信開始
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/GetSentenceData",
        data: "{usercd:\"" + usercd + "\",groupcd:\"" + groupcd + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: GetSentenceData_Result2,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
}

function GetSentenceData_Result2(result) {
    // エラー判定
    if (result.d == null || result.d == undefined || result.d.Result == "Error") {
        alert("サービスでエラーが発生しました。");
        return;
    }

    if (result.d.List == null || result.d.List == undefined) {
        return;
    }

    // リストクリア
    $("#tblSentenceList tbody").empty();

    // リスト
    for (var i = 0; i < result.d.List.length; i++) {
        var dat = result.d.List[i];

        if (dat.length == 7) {
            var coment = "[画像所見]\r\n" + dat[1] + "\r\n[診断・結論]\r\n" + dat[2];
            coment = Escape(coment);
            var temp1 = Escape(dat[4]); // タイトル
            var temp2 = Escape(dat[5]); // 登録医師

            var SentenceCD = "SentenceCD" + dat[0];

            var body = "<tr id =\"" + SentenceCD + "\" ondblclick=\"onSetButton2(" + SentenceCD + ")\" >";
            body += "<td class=\"SetButton\"><input type=\"button\" onclick=\"onSetButton2(" + SentenceCD + ")\" style=\"width:40px;\" /></td>";
            body += "<td class=\"SentenceTitle\" title=\"" + coment + "\">" + temp1 + "</td>";
            body += "<td class=\"SentenceName\" title=\"" + coment + "\">" + temp2 + "</td>";
            body += "</tr>";

            $("#tblSentenceList tbody").append(body);

            // 定型文情報セット
            $("#" + SentenceCD).data("Sentence", dat);
        }
    }

}

function onSetButton2(SentenceCD) {
    // 所見
    var Finding = Escape($("#TxtFinding").val());
    // 診断
    var TxtDiagnosing = Escape($("#TxtDiagnosing").val());

    // 定型文情報
    var Sentence = $("#" + SentenceCD.id).data("Sentence");
    var temp1 = Finding + Escape(Sentence[1]);
    var temp2 = TxtDiagnosing + Escape(Sentence[2]);

    $("#TxtFinding").val(temp1);
    $("#TxtDiagnosing").val(temp2)

}

/***************************************************
/* 変更履歴ボタンイベント */
function ChangeHistory2() {
    $ChangeHistoryDlg.dialog({
        closeText: "",
        modal: true,
        draggable: true,
        resizable: true,
        width: ($(window).width() - 20),
        height: ($(window).height() - 20),
    });
    getChangeHistory2(142);
}

function getChangeHistory2(serialno) {
    // HTTP通信開始
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/GetChangeHistory",
        data: "{serialno:\"" + serialno + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: GetChangeHistoryData_Result2,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
}

function GetChangeHistoryData_Result2(result) {
    // エラー判定
    if (result.d == null || result.d == undefined || result.d.Result == "Error") {
        alert("サービスでエラーが発生しました。");
        return;
    }

    if (result.d.HistList == null || result.d.HistList == undefined) {
        alert("サービスでエラーが発生しました。（HistList）");
        return;
    }

    if (result.d.ImageList == null || result.d.ImageList == undefined) {
        alert("サービスでエラーが発生しました。（ImageList）");
        return;
    }

    SetChangeHistoryTable2(result.d.HistList, result.d.ImageList);
}

function SetChangeHistoryTable2(HistList, ImageList) {
    var tblTag = "";
    var tblIdx = 0;

    $("#tblHistryList tbody").empty();

    // DIV を先に追加しないと表示が可笑しくなる
    for (var i = 0; i < HistList.length; i++) {
        var dat = HistList[i];
        tblIdx = (i + 1);
        tblTag = "";

        var divid = "HistryDiv" + tblIdx;

        tblTag = "<tr><td><div id=\"" + divid + "\" style=\"width:100%;\">";
        tblTag += "</div></td></tr>";
        $("#tblHistryList tbody").append(tblTag);
        tblTag = "";
    }

    // リスト追加
    for (var i = 0; i < HistList.length; i++) {
        var dat = HistList[i];
        tblIdx = (i + 1);
        tblTag = "";

        var divid = "HistryDiv" + tblIdx;
        var $HistryDiv = $("#" + divid);

        // テーブル
        tblTag += "<table id=\"HistryData" + tblIdx + "\" border=\"1\" cellspacing=\"0\" cellpadding=\"0\" style=\"width:100%;\">";

        // ヘッダー
        tblTag += "<thead id=\"HistryHead" + tblIdx + "\"><tr>";
        tblTag += "<th style=\"width:10%;\"></th>";
        tblTag += "<th style=\"width:25%;\"></th>";
        tblTag += "<th style=\"width:10%;\"></th>";
        tblTag += "<th style=\"width:25%;\"></th>";
        tblTag += "<th style=\"width:30%;\"></th>";
        tblTag += "</tr></thead>";

        // ボディー
        tblTag += "<tbody>";

        tblTag += "<tr>";
        tblTag += "<td align=\"center\">指定日付</td>";
        tblTag += "<td align=\"center\">" + formatDateTime(dat[2], dat[3]) + "</td>";
        tblTag += "<td align=\"center\">診断医</td>";
        tblTag += "<td align=\"center\">" + dat[4] + "</td>";
        tblTag += "<td rowspan=\"3\"><div id=\"ReportImages" + tblIdx + "\"></div></td>";
        tblTag += "</tr>";

        tblTag += "<tr>";
        tblTag += "<td align=\"center\" style=\"height:100px;\">画像所見</td>";
        tblTag += "<td colspan=\"3\"><textarea id=\"TxtFinding" + tblIdx + "\" rows=\"\" cols=\"\" border=\"0\" style=\"border: 3px #c0c0c0 double; box-sizing:border-box; width:100%; height:100px;\">" + dat[5] + "</textarea></td>";
        tblTag += "</tr>";

        tblTag += "<tr>";
        tblTag += "<td align=\"center\" style=\"height:100px;\">診断・結論</td>";
        tblTag += "<td colspan=\"3\"><textarea id=\"TxtDiagnosing" + tblIdx + "\" rows=\"\" cols=\"\" border=\"0\" style=\"border: 3px #c0c0c0 double; box-sizing:border-box; width:100%; height:100px;\">" + dat[6] + "</textarea></td>";
        tblTag += "</tr>";

        tblTag += "</tbody>";

        tblTag += "</table>";

        //$("#tblHistryList tbody").append(tblTag);

        $HistryDiv.append(tblTag);


        //var $ReportImages = $("#ReportImages" + tblIdx);

        //// ファイル名リスト
        //var list = new Array();
        //var imgList = ImageList[i];
        //for (var j = 1; j < imgList.length; j++) {
        //    list.push(imgList[j]);
        //}
        //AddImage(list, $ReportImages, "webImageHistory.aspx", false, true, true);
    }

}