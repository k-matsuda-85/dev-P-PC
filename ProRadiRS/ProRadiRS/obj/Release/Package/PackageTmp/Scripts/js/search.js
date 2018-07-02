/* グローバル宣言 */
var $ReportConfig;      // 設定情報格納オブジェクト
var $LoadParams;      　// 検索モダリティ設定情報格納オブジェクト
var $btnSearch;         // 依頼レポート検索ボタンオブジェクト
var $btnSearchEM;       // 緊急読影レポート検索ボタンオブジェクト
var $btnSearchREAD;     // 読影済みレポート検索ボタンオブジェクト
var $pnlSearchUser;     // 他ユーザー依頼検索エリアオブジェクト
var $cmbUserList;       // 依頼者絞り込みコンボボックスオブジェクト
var $btnUserSearch;     // 依頼者絞り込み依頼レポート検索ボタンオブジェクト
var $btnUserSearchREAD; // 依頼者絞り込み読影済みレポート検索ボタンオブジェクト

var $btnSearchSelect;   //検索ボタンオブジェクト
var $btnRest;           //検索条件リセットボタンオブジェクト
var $btnHelp;           //Helpボタンオブジェクト
var helpWindow = null;         

var $lstCol_ReadButton;     // 一覧リスト読影ボタン表示列オブジェクト
var $lstCol_RequestedPhysicianName;     // 一覧リスト依頼者表示列オブジェクト
var $lstCol_AuthorizationPhysicianName; // 一覧リスト読影者表示列オブジェクト

var $lstCol_Infomation;     // 一覧表示受付アカウントオブジェクト
var $lstCol_Diagnosing;     // 一覧リスト診断結論変更用表示列オブジェクト
var $lstCol_Comment2;     // 一覧リスト連絡事項表示列オブジェクト
var $lstCol_Comment3;     // 一覧リスト受付専用表示列オブジェクト

/* 定数扱い */
var ROW0 = "Row0";
var ROW1 = "Row1";
var IMAGECHECKROW0 = "RowImageCheck0";
var IMAGECHECKROW1 = "RowImageCheck1";

var ROW2 = "Row2";
var ROW3 = "Row3";
var ROW4 = "Row4";
var ROW5 = "Row5";
var ROW6 = "Row6";
var ROW7 = "Row7";

var edit_flg = false;

var ViewReserveFlg = 0;

/***************************************************
/* 起動処理 */
$(window).load(function () {
    // 起動処理
    SearchWindow_Init();

    // 依頼レポート表示
    Search();

    
    //カレンダーの設定
    calSet();

    // 画面内リサイズ
    resize();
});
function SearchWindow_Init() {
    // オブジェクトキャッシュ
    $ReportConfig = $("#ReportConfig");
    $LoadParams = $("#LoadParams");
    $btnSearch = $("#btnSearch");
    $btnSearchEM = $("#btnSearchEM");
    $btnSearchREAD = $("#btnSearchREAD");
    $pnlSearchUser = $("#SearchUser");
    $cmbUserList = $("#cmbUserList");
    $btnUserSearch = $("#btnUserSearch");
    $btnUserSearchREAD = $("#btnUserSearchREAD");

    $btnSearchSelect = $("#btnSearchSelect");
    $btnReset = $("#btnReset");
    $btnHelp = $("#btnHelp");
    // 依頼者・読影者 列取得
    $lstCol_ReadButton = $(".ReadButton");
    $lstCol_RequestedPhysicianName = $(".RequestedPhysicianName");
    $lstCol_AuthorizationPhysicianName = $(".AuthorizationPhysicianName");

    $lstCol_Diagnosing = $(".Diagnosing");     // 一覧リスト診断結論変更用表示列オブジェクト
    $lstCol_Comment2 = $(".Comment2");     // 一覧リスト連絡事項表示列オブジェクト
    $lstCol_Comment3 = $(".Comment3");

    // 画面の画像ファイル先読み込み
    SearchWindow_PreLoadImages();

    // HTTP通信開始(ログインユーザー＋設定値情報取得)
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/GetParams",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: SearchWindow_Init_Result,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
            return;
        }
    });


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
}
function SearchWindow_Init_Result(result) {
    // エラー判定
    if (result.d == null || result.d == undefined || result.d.Result == "Error") {
        alert("サービスでエラーが発生しました。");
        return;
    }

    // パラメータ設定
    $ReportConfig.data("ipromedcode", ("ipromedcode" in result.d.Params) ? result.d.Params.ipromedcode : "");
    /* ユーザー権限 */
    $ReportConfig.data("isimagecheck", ("isimagecheck" in result.d.Params) ? result.d.Params.isimagecheck : "0");
    $ReportConfig.data("isrequestsearch", ("isrequestsearch" in result.d.Params) ? result.d.Params.isrequestsearch : "0");
    $ReportConfig.data("isinfomation", ("isinfomation" in result.d.Params) ? result.d.Params.isinfomation : "0");
    $ReportConfig.data("helpURL", ("helpURL" in result.d.Params) ? result.d.Params.helpURL : "");
    
    /* 検索条件Modality設定 */
    $LoadParams = result.d.Params.modality;

    //検索モダリティの設定
    ModalitySet();

    // 他ユーザー依頼検索機能判定
    if ($ReportConfig.data("isrequestsearch") == "1") {
        // 表示
        $pnlSearchUser.show();
    }
    else {
        // 非表示
        $pnlSearchUser.hide();
    }

    //受付アカウント判定
    if ($ReportConfig.data("isinfomation") == "1") {
        // 表示
        $lstCol_Diagnosing.css("display", "none");
        $lstCol_Comment3.css("display", "");

    }
    else {
        // 非表示
        $lstCol_Diagnosing.css("display", "");
        $lstCol_Comment3.css("display", "none");

    }

    //2015/10/28 Uemda INS 予備Viewerボタン
    $("#viewercheckBox").on("click",function () {

        if ($("#viewercheckBox").is(':checked')) {
            $("#cheacklabel-position").css("color", "red");
            ViewReserveFlg = 1;

            //予備Viewr用に各種切り替え

            //読影ボタン　予備変更　inputReadButton
            $(".ReadButton input").removeClass("inputReadButton-off");
            $(".ReadButton input").removeClass("inputReadButton-over");
            $(".ReadButton input").removeClass("inputReadButton-on");
            $(".ReadButton input").addClass("inputReadButtonReserve-off");
            $(".ReadButton input").removeClass("inputReadButtonReserve-on");
            $(".ReadButton input").removeClass("inputReadButtonReserve-over");

            //閲覧ボタン　予備変更
            $(".ViewerButton input").removeClass("inputNoneditButton-off");
            $(".ViewerButton input").removeClass("inputNoneditButton-on");
            $(".ViewerButton input").removeClass("inputNoneditButton-over");
            $(".ViewerButton input").removeClass("inputNoneditButtonReserve-on");
            $(".ViewerButton input").addClass("inputNoneditButtonReserve-off");
            $(".ViewerButton input").removeClass("inputNoneditButtonReserve-over");
        } else {

            $("#cheacklabel-position").css("color", "white");
            ViewReserveFlg = 0;
            //通常に切り替え

            //読影ボタン　初期化　inputReadButton
            $(".ReadButton input").addClass("inputReadButton-off");
            $(".ReadButton input").removeClass("inputReadButton-over");
            $(".ReadButton input").removeClass("inputReadButton-on");
            $(".ReadButton input").removeClass("inputReadButtonReserve-off");
            $(".ReadButton input").removeClass("inputReadButtonReserve-on");
            $(".ReadButton input").removeClass("inputReadButtonReserve-over");

            //閲覧ボタン　初期化
            $(".ViewerButton input").addClass("inputNoneditButton-off");
            $(".ViewerButton input").removeClass("inputNoneditButton-on");
            $(".ViewerButton input").removeClass("inputNoneditButton-over");
            $(".ViewerButton input").removeClass("inputNoneditButtonReserve-on");
            $(".ViewerButton input").removeClass("inputNoneditButtonReserve-off");
            $(".ViewerButton input").removeClass("inputNoneditButtonReserve-over");
        }
        })
    

    //初期化
    edit_flg = false;
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
    $cmbUserList.append($("<option>").html("").val(""));
    for (var i = 0; i < list.length; i++) {
        $cmbUserList.append($("<option>").html(list[i].UserName).val(list[i].UserCD));
    }
}
/***************************************************
/* 画面の画像ファイル先読み込み(チラつき防止) */
function SearchWindow_PreLoadImages() {
    jQuery("<img>").attr("src", "./img/依頼_off.png?20151124");
    jQuery("<img>").attr("src", "./img/依頼_on.png?20151124");
    jQuery("<img>").attr("src", "./img/依頼_over.png?20151124");
    jQuery("<img>").attr("src", "./img/緊急読影_off.png?20151124");
    jQuery("<img>").attr("src", "./img/緊急読影_on.png?20151124");
    jQuery("<img>").attr("src", "./img/緊急読影_over.png?20151124");
    jQuery("<img>").attr("src", "./img/読影済み_off.png?20151124");
    jQuery("<img>").attr("src", "./img/読影済み_on.png?20151124");
    jQuery("<img>").attr("src", "./img/読影済み_over.png?20151124");
    jQuery("<img>").attr("src", "./img/ヘッダーBG.png?20151124");
    jQuery("<img>").attr("src", "./img/タイトルバーBG.png?20151124");
    jQuery("<img>").attr("src", "./img/タイトルバー仕切り.png?20151124");
    jQuery("<img>").attr("src", "./img/検査結果BG.png?20151124");
    jQuery("<img>").attr("src", "./img/Viewer_off.png?20151124");
    jQuery("<img>").attr("src", "./img/Viewer_on.png?20151124");
    jQuery("<img>").attr("src", "./img/Viewer_over.png?20151124");
    jQuery("<img>").attr("src", "./img/読影_off.png?20151124");
    jQuery("<img>").attr("src", "./img/読影_on.png?20151124");
    jQuery("<img>").attr("src", "./img/読影_over.png?20151124");
    jQuery("<img>").attr("src", "./img/画像データ取得_off.png?20151124");
    jQuery("<img>").attr("src", "./img/画像データ取得_on.png?20151124");
    jQuery("<img>").attr("src", "./img/画像データ取得_over.png?20151124");
    jQuery("<img>").attr("src", "./img/画像確認_off.png?20151124");
    jQuery("<img>").attr("src", "./img/画像確認_on.png?20151124");
    jQuery("<img>").attr("src", "./img/画像確認_over.png?20151124");

//20150316 A.Umeda レイアウト、機能の変更に伴い改修------------------------------------------------------------Start
    jQuery("<img>").attr("src", "./img/検索_off.png?20151124");
    jQuery("<img>").attr("src", "./img/検索_on.png?20151124");
    jQuery("<img>").attr("src", "./img/検索_over.png?20151124");
    jQuery("<img>").attr("src", "./img/SearchMenuReset.png?20151124");
    jQuery("<img>").attr("src", "./img/SearchMenuResetActive.png?20151124");
    jQuery("<img>").attr("src", "./img/SearchMenuResetHover.png?20151124");
    jQuery("<img>").attr("src", "./img/依頼_アイコン_off.png?20151124");
    jQuery("<img>").attr("src", "./img/依頼_アイコン_on.png?20151124");
    jQuery("<img>").attr("src", "./img/依頼_アイコン_over.png?20151124");
    jQuery("<img>").attr("src", "./img/読影済み_アイコン_off.png?20151124");
    jQuery("<img>").attr("src", "./img/読影済み_アイコン_on.png?20151124");
    jQuery("<img>").attr("src", "./img/読影済み_アイコン_over.png?20151124");
    jQuery("<img>").attr("src", "./img/閲覧_off.png?20151124");
    jQuery("<img>").attr("src", "./img/閲覧_on.png?20151124");
    jQuery("<img>").attr("src", "./img/閲覧_over.png?20151124");
    jQuery("<img>").attr("src", "./img/SearchHelp.png?20151124");
    jQuery("<img>").attr("src", "./img/SearchHelpActive.png?20151124");
    jQuery("<img>").attr("src", "./img/SearchHelpHover.png?20151124");
    jQuery("<img>").attr("src", "./img/閲覧予備_off.png?20151124");
    jQuery("<img>").attr("src", "./img/閲覧予備_on.png?20151124");
    jQuery("<img>").attr("src", "./img/閲覧予備_over.png?20151124");
    jQuery("<img>").attr("src", "./img/読影予備_off.png?20151124");
    jQuery("<img>").attr("src", "./img/読影予備_on.png?20151124");
    jQuery("<img>").attr("src", "./img/読影予備_over.png?20151124");
}
/***************************************************
/* 検索モダリティの作成 */

function ModalitySet() {
    loadParams: { };
    loadParams = $LoadParams;
    // 検索モダリティ作成
    var $select = $("#modality").append($("<option>"));
    var mod = loadParams.split(",");
    $.each(mod, function () {
        $select.append($("<option>").text(this));
    });
}
/***************************************************
/* カレンダー初期化 */

function calSet() {
    // datepicker初期化
    $("#searchDay").datepicker({
        "changeMonth": true,
        "changeYear": true,
        "showAnim": "",
        "prevText": '&#x3C;前',
        "nextText": '次&#x3E;',
        "monthNames": ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
        "monthNamesShort": ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月'],
        "dayNames": ['日曜日', '月曜日', '火曜日', '水曜日', '木曜日', '金曜日', '土曜日'],
        "dayNamesShort": ['日', '月', '火', '水', '木', '金', '土'],
        "dayNamesMin": ['日', '月', '火', '水', '木', '金', '土'],
        "weekHeader": '週',
        "dateFormat": 'yy/mm/dd',
        "firstDay": 0,
        "showMonthAfterYear": true
    });
}
/***************************************************
/* 検索 */
function SearchSelect() {

    // 入力チェック
    var patientID = $("#PatientID").val();
    var searchDay = getInputDateString($("#searchDay").val());
    var modality = $("#modality").val();


    //エスケープ処理後に、入力欄も訂正
    patientID = Escapewildcard(patientID);
    $("#PatientID").val(patientID);

    if (patientID == "" && searchDay == "" && modality == "") {
        // メッセージボックス表示
        //                common.message("検索条件が入力されていません。", function () {
        //                    // フォーカス設定
        //                    $("#Search-Menu-PatientID input").focus();
        //                });
        alert("検索条件が入力されていません。");
        $("#PatientID").focus();
        return;
    }
    if (patientID == "" && searchDay == "") {
        // メッセージボックス表示
        alert("患者IDもしくは検査日付を入力して下さい。");
        // フォーカス設定
        if (patientID == "" || patientID == null) {
            $("#PatientID").focus();
        }
        else if (searchDay == "" || searchDay ==null) {
            $("#searchDay").focus();
        }

        return;
    }

    // 読影ボタン、依頼者と読影者を非表示
    $lstCol_ReadButton.css("display", "none");
    $lstCol_RequestedPhysicianName.css("display", "none");
    $lstCol_AuthorizationPhysicianName.css("display", "none");

    // 検索
    // セッションのユーザー名で検索
    //SearchSelect_RequestReport("0", "", Search_RequestReport_Result);

    SearchSelect_RequestReport(patientID, searchDay, modality, Search_RequestReport_Result);

    //初期化
    edit_flg = false;
}
function SearchSelect_RequestReport(patientID, searchDay, modality, func) {
    // HTTP通信開始
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/GetSearchList",
        data: "{patientID:\"" + patientID + "\",searchDay:\"" + searchDay +"\",modality:\"" + modality + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: func,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
}



/* 検索ボタンマウスダウン */
function SearchSelectMouseDonw(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnSearchSelect-on");
        sender.removeClass("btnSearchSelect-off");
        sender.removeClass("btnSearchSelect-over");
    }
}
/* 検索ボタンマウスアップ */
function SearchSelectMouseUp(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnSearchSelect-off");
        sender.removeClass("btnSearchSelect-on");
        sender.removeClass("btnSearchSelect-over");
    }
}
/* 検索ボタンマウスオーバー */
function SearchSelectMouseOver(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnSearchSelect-over");
        sender.removeClass("btnSearchSelect-off");
        sender.removeClass("btnSearchSelect-on");
    }
}
/***************************************************
/* 検索条件リセット */
function Reset() {

    $("#PatientID").val("");
    $("#searchDay").val("");
    $("#modality").val("");
}
/* 検索条件リセットボタンマウスダウン */
function ResetMouseDonw(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnReset-on");
        sender.removeClass("btnReset-off");
        sender.removeClass("btnReset-over");
    }
}
/* 検索条件リセットボタンマウスアップ */
function ResetMouseUp(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnReset-off");
        sender.removeClass("btnReset-on");
        sender.removeClass("btnReset-over");
    }
}
/* 検索条件リセットボタンマウスオーバー */
function ResetMouseOver(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnReset-over");
        sender.removeClass("btnReset-off");
        sender.removeClass("btnReset-on");
    }
}

/***************************************************
/* Helpボタン */
function Help() {

    // ヘルプ処理
    //開いている場合は一度閉じる
        if (helpWindow) {
            if (helpWindow.closed) {
                try {
                    // IEの場合、子ウィンドウが制御できない場合があるため強制的に空ページを開き、閉じる
                    if (helpWindow.opener) {
                        window.open("", "Help").close();
                    }
                }
                catch (e) { }
            }
            else {
                helpWindow.close();
            }
        }

        // ヘルプ表示
        helpWindow = window.open($ReportConfig.data("helpURL"), "Help");
    
}

/* 検索条件リセットボタンマウスダウン */
function HelpMouseDonw(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnHelp-on");
        sender.removeClass("btnHelp-off");
        sender.removeClass("btnHelp-over");
    }
}
/* 検索条件リセットボタンマウスアップ */
function HelpMouseUp(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnHelp-off");
        sender.removeClass("btnHelp-on");
        sender.removeClass("btnHelp-over");
    }
}
/* 検索条件リセットボタンマウスオーバー */
function HelpMouseOver(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnHelp-over");
        sender.removeClass("btnHelp-off");
        sender.removeClass("btnHelp-on");
    }
}


//20150316 A.Umeda レイアウト、機能の変更に伴い改修----------------------------------------------------------------------END


/***************************************************
/* 依頼レポート表示 */
function Search() {
    // 読影ボタン表示
    $lstCol_ReadButton.css("display", "");

    // 依頼者と読影者を非表示
    $lstCol_RequestedPhysicianName.css("display", "none");
    $lstCol_AuthorizationPhysicianName.css("display", "none");

    // 検索
    // セッションのユーザー名で検索
    Search_RequestReport("0", "", Search_RequestReport_Result);

    //初期化
    edit_flg = false;
}
function Search_RequestReport(isUserNameType, userName, func) {
    // HTTP通信開始
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/GetRequestReportList",
        data: "{isusernametype:\"" + isUserNameType + "\",username:\"" + userName + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: func,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
}

//検索関数
function Search_RequestReport_Result(result) {
    // エラー判定
    if (result.d.Result == "Exception") {
        alert(result.d.Message);
        return;
    }
    if (result.d.Result == "Error") {
        alert(result.d.Message);
        return;
    }

    // リストクリア
    $("#tblList tbody").empty();

    // 結果表示
    // 件数
    $("#lblSearchCount").text(result.d.Count + "件");
    // リスト
    for (var i = 0; i < result.d.List.length; i++) {
        // 行ごとの背景色(色設定はSearch.css)
        var className = "";
        // ステータスごとの背景色（色設定はSearch.css）
        var statuscolor = "";
        // 画像確認済レポートリストに存在するか確認
        var imgCheck = false;
        for (var j = 0; j < result.d.ImageCheckList.length; j++) {
            if (result.d.List[i].OrderNo == result.d.ImageCheckList[j]) {
                imgCheck = true;
                break;
            }
        }
        // 画像確認済とそれ以外で行の色変更(不要のためコメントアウト)
        if (i % 2 == 0) {
//            if (imgCheck) { className = IMAGECHECKROW0; }
            //            else { className = ROW0; }
            className = ROW0;
        }
        else {
//            if (imgCheck) { className = IMAGECHECKROW1; }
            //            else { className = ROW1; }
            className = ROW1;
        }

        // 一時保存データの保持有無
        var hasTempSave = false;
        hasTempSave = checkTempSave(result.d.List[i].SerialNo, result.d.TempSaveList);

        // ステータス文字列取得
        var strStatus = formatReportStatus(result.d.List[i].ReportStatus, result.d.List[i].ReadingStatus, hasTempSave);
        var statustemp = "";
//20150317 A.Umeda ステータス文字列の色分けのため追加---------------------------------------------------START
        //ステータス文字列によって色分け設定
        switch (strStatus) {
            case "未検":
            case "【保】未検":
                statuscolor = ROW7;
                statustemp = "miken";
                break;
            case "未読":
            case "【保】未読":
                statuscolor = ROW2;
                statustemp = "midoku";
                break;
            case "仮確":
            case "【保】仮確":
                statuscolor = ROW4;
                statustemp = "karikaku";
                break;
            case "確定":
            case "【保】確定":
                statuscolor = ROW5;
                statustemp = "kakutei";
                break;
            case "読影中":
            case "【保】読影中":
                statuscolor = ROW3;
                break;
            case "変更中":
            case "【保】変更中":
                statuscolor = ROW6;
                break;
        }
//20150317 A.Umeda ステータス文字列の色分けのため追加---------------------------------------------------END

        // 連想配列で行データ作成
        var row = {};
        row.Status = strStatus;
        
        row.Priority = formatPriorityStatus(result.d.List[i].PriorityFlag);
        //20150325 A.Umeda ERS非表示のため修正　-------------------------------------------------START
        //        row.iProMedID = $ReportConfig.data("ipromedcode") + result.d.List[i].PatientID;
        row.iProMedID = result.d.List[i].PatientID;
        //20150325 A.Umeda ERS非表示のため修正　-------------------------------------------------START

        row.PatientAge = result.d.List[i].PatientAge;
        row.PatientSex = result.d.List[i].PatientSex;
        //row.StudyDateTime = formatDateTime(result.d.List[i].StudyDate, result.d.List[i].StudyTime);
        row.StudyDateTime = formatDateTime(result.d.List[i].StudyDate,"");
        row.Modality = result.d.List[i].Modality;
        //row.ReportReserve8 = result.d.List[i].ReportReserve8;
        row.StudyBodyPart = result.d.List[i].StudyBodyPart;

        if ($lstCol_Diagnosing.css("display") != "none") {
            row.Diagnosing = result.d.List[i].Diagnosing;
        }
        else {
            row._Diagnosing = result.d.List[i].Diagnosing;
        }
        
        //row.Finding = result.d.List[i].Finding;
        var coment = result.d.List[i].Finding;

        if ($lstCol_Comment3.css("display") != "none") {
            row.Comment3 = result.d.List[i].Comment3;
        }
        else {
            row._Comment3 = result.d.List[i].Comment3;
        }

        //20150306 A.Umeda 読影医師追加のため ---------------------Start
        row.RadiologistName = result.d.List[i].AuthorizationPhysicianName;
        //20150306 A.Umeda 読影医師追加のため ---------------------End

        if ($lstCol_RequestedPhysicianName.css("display") != "none") {
            row.RequestedPhysicianName = result.d.List[i].RequestedPhysicianName;
        }
        else {
            row._RequestedPhysicianName = result.d.List[i].RequestedPhysicianName;
        }
        //if ($lstCol_AuthorizationPhysicianName.css("display") != "none") {
        //    row.AuthorizationPhysicianName = result.d.List[i].AuthorizationPhysicianName;
        //}
        //else {
        //    row._AuthorizationPhysicianName = result.d.List[i].AuthorizationPhysicianName;
        //}
        
        row.Comment2 = result.d.List[i].Comment2;

        // 先頭が"_"の列は表示しない
        row._SerialNo = result.d.List[i].SerialNo;
        row._PatientID = result.d.List[i].PatientID;
        row._OfficeCd = result.d.List[i].ReportReserve1;
        row._SerialNo = result.d.List[i].SerialNo;
        row._OrderNo = result.d.List[i].OrderNo;
        row._StudyDate = result.d.List[i].StudyDate;
        row._StudyTime = result.d.List[i].StudyTime;
        row._Statustemp = statustemp;

        if ($lstCol_ReadButton.css("display") != "none") {
            var outStr = "<tr class=\"" + className + "\" id=\"" + row._SerialNo + "\">";
            // 読影ボタン
            var readBtnId = "READ" + row._SerialNo;
            //20150413 A.Umeda 読影ボタンクリックイベントReportWindow_OpenをReportStatus_Checkに変更----------START
            var readBtnStr = "<td class=\"ReadButton\"><input class=\"inputReadButton inputReadButton-off\" id=\"" + readBtnId + "\" type=\"button\" onclick=\"ReportStatus_Check(" + row._SerialNo + ");\" onmousedown=\"ReadMouseDonw(" + readBtnId + ")\" onmouseup=\"ReadMouseUp(" + readBtnId + ")\" onmouseout=\"ReadMouseUp(" + readBtnId + ")\" onmouseover=\"ReadMouseOver(" + readBtnId + ")\"/></td>";
            //20150413 A.Umeda 読影ボタンクリックイベントReportWindow_OpenをReportStatus_Checkに変更----------END
            outStr += readBtnStr;
        } else {
            var outStr = "<tr class=\"" + className + "\" id=\"" + row._SerialNo + "\">";
        }
        // Viewerボタン
        var viewerBtnId = "VIEWER" + row._SerialNo;
        //20150413 A.Umeda Viewerボタンを閲覧ボタンに切り替え --------------------------------------------------------------START
        //var viewerBtnStr = "<td class=\"ViewerButton\"><input class=\"inputViewerButton inputViewerButton-off\" id=\"" + viewerBtnId + "\" type=\"button\" onclick=\"ViewerWindow_Open(" + row._SerialNo + ");\" onmousedown=\"ViewerMouseDonw(" + viewerBtnId + ")\" onmouseup=\"ViewerMouseUp(" + viewerBtnId + ")\" onmouseout=\"ViewerMouseUp(" + viewerBtnId + ")\" onmouseover=\"ViewerMouseOver(" + viewerBtnId + ")\"/></td>";
        var viewerBtnStr = "<td class=\"ViewerButton\"><input class=\"inputNoneditButton inputNoneditButton-off\" id=\"" + viewerBtnId + "\" type=\"button\" onclick=\"ReportWindow_Open(" + row._SerialNo + ");\" onmousedown=\"ViewerMouseDonw(" + viewerBtnId + ")\" onmouseup=\"ViewerMouseUp(" + viewerBtnId + ")\" onmouseout=\"ViewerMouseUp(" + viewerBtnId + ")\" onmouseover=\"ViewerMouseOver(" + viewerBtnId + ")\"/></td>";

        //20150413 A.Umeda Viewerボタンを閲覧ボタンに切り替え --------------------------------------------------------------END
        outStr += viewerBtnStr;

        // 画像取得ボタン
        //20150306 A.Umeda 画像取得ボタン削除のためコメントアウト --------------------------------Start
        //var requestBtnId = "REQ" + row._SerialNo;
        //var requestBtnStr = "<td class=\"ImageRequestButton\"><input class=\"inputImageRequestButton inputImageRequestButton-off\" id=\"" + requestBtnId + "\" type=\"button\" onclick=\"ViewerImage_Request(" + row._SerialNo + ");\" onmousedown=\"RequestMouseDonw(" + requestBtnId + ")\" onmouseup=\"RequestMouseUp(" + requestBtnId + ")\" onmouseout=\"RequestMouseUp(" + requestBtnId + ")\" onmouseover=\"RequestMouseOver(" + requestBtnId + ")\"/></td>";
        //outStr += requestBtnStr;
        //20150306 A.Umeda 画像取得ボタン削除のためコメントアウト --------------------------------End

        for (var key in row) {
            // 先頭が"_"のkeyは非表示列なので表示しない
            var indexof = key.indexOf("_");
            if (indexof != 0) {

            //20150323 A.Umeda ステータス表示欄はステータスに応じて色を変更するため修正 --------START

                //outStr += "<td class=\"" + key + "\">" + row[key] + "</td>";


                if (key == "Status") {

                    outStr += "<td class=\"" + key + "\" id=\"" + statuscolor + "\">" + row[key] + "</td>";

                } else if (key == "Diagnosing") {
                    //ツールチップ表示で、所見と診断を表示    title=\"" + coment 

                    //エスケープ処理追加が必要　2015/05/13 common.jsで作成予定、順番注意
                    coment = Escape(coment);
                    var temp = Escape(row[key]);
                    outStr += "<td class=\"" + key + "\" title=\"" + coment + "\">" + temp + "</td>";
                } else {
                    outStr += "<td class=\"" + key + "\" title=\"" + row[key] + "\">" + row[key] + "</td>";
                }

            //20150323 A.Umeda ステータス表示欄はステータスに応じて色を変更するため修正 --------END
            }
        }

        // 画像枚数確認完了ボタン（不要のためコメントアウト20150401　A.Umeda）
//        var imgCheckId = "CHECK" + row._SerialNo;
//        var imgCheckBtnStr = "<td class=\"ImageCheckButton\"><input class=\"inputImageCheckButton inputImageCheckButton-off\" id=\"" + imgCheckId + "\" type=\"button\" onclick=\"ImageNum_Check(" + row._SerialNo + ");\" onmousedown=\"CheckMouseDonw(" + imgCheckId + ")\" onmouseup=\"CheckMouseUp(" + imgCheckId + ")\" onmouseout=\"CheckMouseUp(" + imgCheckId + ")\" onmouseover=\"CheckMouseOver(" + imgCheckId + ")\"/></td>";
//        outStr += imgCheckBtnStr;

        outStr += "</tr>";

        // Tableタグ表示
        $("#tblList tbody").append(outStr);
        // レポート情報セット
        $("#" + row._SerialNo).data("reportview", row);

        // 一時保存データの保持有無をセット
        $("#" + row._SerialNo).data("hastempsave", hasTempSave);

        //20150306 A.Umeda 画像確認ボタン削除のため変更　--------------------------START
        // ユーザー権限により画像確認ボタン列の表示・非表示
//        if ($ReportConfig.data("isimagecheck") != "1") {
//            $(".ImageCheckButton").css("display", "none");
//                }
        //画像確認ボタン列の非表示
        $(".ImageCheckButton").css("display", "none");
        //20150306 A.Umeda 画像確認ボタン削除のため変更　--------------------------END

        ChagebottonImage();
    }
}
/* 依頼レポートボタンマウスダウン */
function SearchMouseDonw(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnSearch-on");
        sender.removeClass("btnSearch-off");
        sender.removeClass("btnSearch-over");
    }
}
/* 依頼レポートボタンマウスアップ */
function SearchMouseUp(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnSearch-off");
        sender.removeClass("btnSearch-on");
        sender.removeClass("btnSearch-over");
    }
}
/* 依頼レポートボタンマウスオーバー */
function SearchMouseOver(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnSearch-over");
        sender.removeClass("btnSearch-off");
        sender.removeClass("btnSearch-on");
    }
}

/***************************************************
/* 緊急読影レポート検索 */
function SearchEM() {
    // 依頼者と読影者を非表示
    $lstCol_RequestedPhysicianName.css("display", "none");
    $lstCol_AuthorizationPhysicianName.css("display", "none");
    
    // 検索
    Search_EmergencyReport(Search_RequestReport_Result);
}
function Search_EmergencyReport(func) {
    // HTTP通信開始
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/GetEmergencyReportList",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: func,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
}
/* 緊急読影レポートボタンマウスダウン */
function SearchEMMouseDonw(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnSearchEM-on");
        sender.removeClass("btnSearchEM-off");
        sender.removeClass("btnSearchEM-over");
    }
}
/* 緊急読影レポートボタンマウスアップ */
function SearchEMMouseUp(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnSearchEM-off");
        sender.removeClass("btnSearchEM-on");
        sender.removeClass("btnSearchEM-over");
    }
}
/* 緊急読影レポートボタンマウスオーバー */
function SearchEMMouseOver(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnSearchEM-over");
        sender.removeClass("btnSearchEM-off");
        sender.removeClass("btnSearchEM-on");
    }
}

/***************************************************
/* 読影済みレポート検索 */
function SearchREAD() {
    // 読影ボタン表示
    $lstCol_ReadButton.css("display", "");

    // 依頼者と読影者を非表示
    $lstCol_RequestedPhysicianName.css("display", "none");
    $lstCol_AuthorizationPhysicianName.css("display", "none");

    // 検索
    // セッションのユーザー名で検索
    Search_ReadReport("0", "", Search_RequestReport_Result);

    //初期化
    edit_flg = false;
}
function Search_ReadReport(isUserNameType, userName, func) {
    // HTTP通信開始
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/GetReadReportList",
        data: "{isusernametype:\"" + isUserNameType + "\",username:\"" + userName + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: func,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
}
/* 読影済みレポートボタンマウスダウン */
function SearchREADMouseDonw(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnSearchREAD-on");
        sender.removeClass("btnSearchREAD-off");
        sender.removeClass("btnSearchREAD-over");
    }
}
/* 読影済みレポートボタンマウスアップ */
function SearchREADMouseUp(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnSearchREAD-off");
        sender.removeClass("btnSearchREAD-on");
        sender.removeClass("btnSearchREAD-over");
    }
}
/* 読影済みレポートボタンマウスオーバー */
function SearchREADMouseOver(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnSearchREAD-over");
        sender.removeClass("btnSearchREAD-off");
        sender.removeClass("btnSearchREAD-on");
    }
}


/***************************************************
/* 一覧画面リサイズイベント */
function resize() {
    // 全体表示サイズ
    var bodyHeight = $(window).height();
    // 固定領域表示サイズ
    var topHeight = $("#SearchTop").height();
    var bottomHeight = $("#SearchBottom").height();

    // リスト表示サイズ計算＋サイズ設定
    var centerHeight = bodyHeight - (topHeight + bottomHeight);
    $("#SearchCenter").height(centerHeight);
    $("#reportList").height(centerHeight);

    // テーブルの最終列のwidth調整
    var bodyWidth = $(window).width();
    if (bodyWidth > 1200) {
        // ウィンドウサイズが1200を超える場合は最終列のwidthを100%(ウィンドウ幅に合わせる)
        $(".Comment3").css("width", "100%");
    }
    else {
        // ウィンドウサイズが1200未満の場合は最終列のwidthを固定値
        $(".Comment3").css("width", "150px");
    }
}

//20150330 A.Umeda 読影ステータスRSデータベース更新処理追加のため変更--------------------------START
/***************************************************
/* 読影ステータス更新イベント */
function ReportStatus_Check(id) {
    
    //読影ボタンイベントのため、True
    edit_flg = true;

    // ボタンクリックの行からレポートデータ取得
    var report = $("#" + id).data("reportview");


    var key = new Array();
    var val = new Array();

    // 選択レポート情報セット
    key.push("'serialno'");
    val.push("'" + report._SerialNo + "'");
    key.push("'orderno'");
    val.push("'" + report._OrderNo + "'");
    key.push("'patientid'");
    val.push("'" + report._PatientID + "'");
    key.push("'modality'");
    val.push("'" + report.Modality + "'");
    key.push("'studydate'");
    val.push("'" + report._StudyDate + "'");
    key.push("'officecd'");
    val.push("'" + report._OfficeCd + "'");




    // 一時保存データ保持の場合は読み出し確認 → 必ず一時保存データ読み出し
    var readTempSave = "0";
    var hasTempSave = $("#" + id).data("hastempsave");
    if (hasTempSave) {
        //            if (window.confirm("一時保存したデータが存在します。\n一時保存データを読み出しますか？")) {
        //                readTempSave = "1";
        //            }
        readTempSave = "1";
    }

    // 選択レポート情報に一時保存データの読み出し判定をセット
    key.push("'readtempsave'");
    val.push("'" + readTempSave + "'");


    // HTTP通信
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/SetParams",
        data: "{key:[" + key + "],val:[" + val + "]}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: readCheck(id, edit_flg, true),//直前に他人が読影していた場合に、確認ダイアログ表示
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });

   
    
}
//20150330 A.Umeda 読影ステータスRSデータベース更新処理追加のため変更--------------------------END

/***************************************************
/* 一覧読影ボタンイベント */
function ReportWindow_Open(id,edit_flg) {

    //読影ボタン押下時にViewer起動のため追加
    ViewerWindow_Open(id);

    // ボタンクリックの行からレポートデータ取得
    var report = $("#" + id).data("reportview");

    var key = new Array();
    var val = new Array();

    // 選択レポート情報セット
    key.push("'serialno'");
    val.push("'" + report._SerialNo + "'");
    key.push("'orderno'");
    val.push("'" + report._OrderNo + "'");
    key.push("'patientid'");
    val.push("'" + report._PatientID + "'");
    key.push("'modality'");
    val.push("'" + report.Modality + "'");
    key.push("'studydate'");
    val.push("'" + report._StudyDate + "'");
    key.push("'officecd'");
    val.push("'" + report._OfficeCd + "'");

    //20150420 A.Umeda 閲覧フラグを追加----------------------------START
    key.push("'non-edit'");
    if (edit_flg) {
        val.push("'0'");//edit_flg = true は読影モード
    } else {
        val.push("'1'");//edit_flg = false は閲覧モード
    }
    
    //20150420 A.Umeda 閲覧フラグを追加----------------------------END


    //20150420 A.Umeda 確定ステータスからの読影・閲覧の際は、一時保存を無視----------------------------START

    //// 一時保存データ保持の場合は読み出し確認 → 必ず一時保存データ読み出し
    //var readTempSave = "0";
    //var hasTempSave = $("#" + id).data("hastempsave");
    //if (hasTempSave) {
    //    //            if (window.confirm("一時保存したデータが存在します。\n一時保存データを読み出しますか？")) {
    //    //                readTempSave = "1";
    //    //            }
    //    readTempSave = "1";
    //}

    //// 選択レポート情報に一時保存データの読み出し判定をセット
    //key.push("'readtempsave'");
    //val.push("'" + readTempSave + "'");

    var readTempSave = "0";

    var hasTempSave = $("#" + id).data("hastempsave");
    if (hasTempSave) {
        //            if (window.confirm("一時保存したデータが存在します。\n一時保存データを読み出しますか？")) {
        //                readTempSave = "1";
        //            }
        readTempSave = "1";
    }

    if (report._Statustemp == "midoku" || report._Statustemp == "miken" ||report._Statustemp == "karikaku" ||report._Statustemp == "kakutei") {


        var serialNo = report._SerialNo;
        //一時保存データの削除
        // HTTP通信
        $.ajax({
            async: false,
            cache: false,
            type: "POST",
            url: "./CommonWebService.asmx/DeleteTemp",
            data: "{serialNo:" + serialNo + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: readTempSave = "0",
            error: function (result) {
                // エラー
                alert("通信エラーが発生しました。");
            }
        });
        
    }

    // 選択レポート情報に一時保存データの読み出し判定をセット
    key.push("'readtempsave'");
    val.push("'" + readTempSave + "'");

    //20150420 A.Umeda 確定ステータスからの読影・閲覧の際は、一時保存を無視----------------------------END

    // HTTP通信
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/SetParams",
        data: "{key:[" + key + "],val:[" + val + "]}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: ReportWindow_Open_Result,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });



}
function ReportWindow_Open_Result(result) {
    // エラー判定
    if (result.d == null || result.d == "Error") {
        alert("レポート表示でエラーが発生しました。");
        return;
    }

    //開いている場合は一度Windowを閉じる
    if (reportWindow) {
        if (reportWindow.closed) {
            try {
                // IEの場合、子ウィンドウが制御できない場合があるため強制的に空ページを開き、閉じる
                if (reportWindow.opener) {
                    window.open("", "ProRadiRS").close();
                }
            }
            catch (e) { }
        }
        else {
            reportWindow.close();
        }
    }

    // レポートページ表示
    reportWindow = window.open("./webReport.aspx", "ProRadiRS");
   
}

/* 一覧読影ボタンマウスダウンイベント */
function ReadMouseDonw(btnId) {
    var sender = $(btnId);

    if (ViewReserveFlg == 0) {
        if (sender != null && sender != undefined) {
            sender.addClass("inputReadButton-on");
            sender.removeClass("inputReadButton-off");
            sender.removeClass("inputReadButton-over");
            sender.removeClass("inputReadButtonReserve-off");
            sender.removeClass("inputReadButtonReserve-on");
            sender.removeClass("inputReadButtonReserve-over");
        }
    } else {
        if (sender != null && sender != undefined) {
            sender.addClass("inputReadButtonReserve-on");
            sender.removeClass("inputReadButtonReserve-off");
            sender.removeClass("inputReadButtonReserve-over");
        }
    }
}
/* 一覧読影ボタンマウスアップイベント */
function ReadMouseUp(btnId) {
    var sender = $(btnId);

    if (ViewReserveFlg == 0) {
        if (sender != null && sender != undefined) {
            sender.addClass("inputReadButton-off");
            sender.removeClass("inputReadButton-on");
            sender.removeClass("inputReadButton-over");
            sender.removeClass("inputReadButtonReserve-off");
            sender.removeClass("inputReadButtonReserve-on");
            sender.removeClass("inputReadButtonReserve-over");
        }
    } else {
        if (sender != null && sender != undefined) {
            sender.addClass("inputReadButtonReserve-off");
            sender.removeClass("inputReadButtonReserve-on");
            sender.removeClass("inputReadButtonReserve-over");
        }
    }
}
/* 一覧読影ボタンマウスオーバーイベント */
function ReadMouseOver(btnId) {
    var sender = $(btnId);

    if (ViewReserveFlg == 0) {
        if (sender != null && sender != undefined) {
            sender.addClass("inputReadButton-over");
            sender.removeClass("inputReadButton-off");
            sender.removeClass("inputReadButton-on");
            sender.removeClass("inputReadButtonReserve-off");
            sender.removeClass("inputReadButtonReserve-on");
            sender.removeClass("inputReadButtonReserve-over");
        }
    } else {
        if (sender != null && sender != undefined) {
            sender.addClass("inputReadButtonReserve-over");
            sender.removeClass("inputReadButtonReserve-off");
            sender.removeClass("inputReadButtonReserve-on");
        }
    }
}

/***************************************************
/* 一覧Viewerボタンイベント */
/*　⇒閲覧ボタンに変更 20150423 A.Umeda------------------------------------------START*/
function ViewerWindow_Open(id) {
    //初期化
    edit_flg = false;
    // ボタンクリックの行からレポートデータ取得
    var report = $("#" + id).data("reportview");

    // 連想配列に起動引数追加
    var prm = {};
    prm.serialno = report._SerialNo;
    prm.orderno = report._OrderNo;
    prm.patientid = report._PatientID;
    prm.studydate = report._StudyDate;
    prm.modality = report.Modality;
    prm.officecd = report._OfficeCd;

    // common.js(メソッド呼び出し)
    WebViewer2_Start(prm, ViewReserveFlg);

}
///* 閲覧ボタンマウスダウンイベント */
function ViewerMouseDonw(btnId) {
    var sender = $(btnId);

    if (ViewReserveFlg == 0) {
        if (sender != null && sender != undefined) {
            sender.addClass("inputNoneditButton-on");
            sender.removeClass("inputNoneditButton-off");
            sender.removeClass("inputNoneditButton-over");
            sender.removeClass("inputNoneditButtonReserve-on");
            sender.removeClass("inputNoneditButtonReserve-off");
            sender.removeClass("inputNoneditButtonReserve-over");
        }
    } else {
        if (sender != null && sender != undefined) {
            sender.addClass("inputNoneditButtonReserve-on");
            sender.removeClass("inputNoneditButtonReserve-off");
            sender.removeClass("inputNoneditButtonReserve-over");
        }
    }
}
/* 閲覧ボタンマウスアップイベント */
function ViewerMouseUp(btnId) {
    var sender = $(btnId);

    if (ViewReserveFlg == 0) {
        if (sender != null && sender != undefined) {
            sender.addClass("inputNoneditButton-off");
            sender.removeClass("inputNoneditButton-on");
            sender.removeClass("inputNoneditButton-over");
            sender.removeClass("inputNoneditButtonReserve-on");
            sender.removeClass("inputNoneditButtonReserve-off");
            sender.removeClass("inputNoneditButtonReserve-over");
        }
    } else {
        if (sender != null && sender != undefined) {
            sender.addClass("inputNoneditButtonReserve-off");
            sender.removeClass("inputNoneditButtonReserve-on");
            sender.removeClass("inputNoneditButtonReserve-over");
        }
    }
}
/* 閲覧ボタンマウスオーバーイベント */
function ViewerMouseOver(btnId) {
    var sender = $(btnId);

    if (ViewReserveFlg == 0) {
        if (sender != null && sender != undefined) {
            sender.addClass("inputNoneditButton-over");
            sender.removeClass("inputNoneditButton-on");
            sender.removeClass("inputNoneditButton-off");
            sender.removeClass("inputNoneditButtonReserve-on");
            sender.removeClass("inputNoneditButtonReserve-off");
            sender.removeClass("inputNoneditButtonReserve-over");
        }
    }else{
        if (sender != null && sender != undefined) {
            sender.addClass("inputNoneditButtonReserve-over");
            sender.removeClass("inputNoneditButtonReserve-on");
            sender.removeClass("inputNoneditButtonReserve-off");
        }
    }
}
/*　⇒閲覧ボタンに変更 20150423 A.Umeda------------------------------------------END

/***************************************************
/* 画像取得ボタンイベント */
function ViewerImage_Request(id) {
    // 確認ダイアログ
    if (!window.confirm("画像データ取得要求を行います。よろしいですか？")) {
        return;
    }

    // ボタンクリックの行からレポートデータ取得
    var report = $("#" + id).data("reportview");

    // 連想配列に起動引数追加
    var prm = {};
    prm.orderno = report._OrderNo;
    prm.patientid = report._PatientID;
    prm.studydate = report._StudyDate;
    prm.modality = report.Modality;

    // common.js(ViewerImageRequest_Startメソッド呼び出し)
    ViewerImageRequest_Start(prm);
}
/* 一覧画像取得ボタンマウスダウンイベント */
function RequestMouseDonw(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("inputImageRequestButton-on");
        sender.removeClass("inputImageRequestButton-off");
        sender.removeClass("inputImageRequestButton-over");
    }
}
/* 一覧画像取得ボタンマウスアップイベント */
function RequestMouseUp(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("inputImageRequestButton-off");
        sender.removeClass("inputImageRequestButton-on");
        sender.removeClass("inputImageRequestButton-over");
    }
}
/* 一覧画像取得ボタンマウスアップイベント */
function RequestMouseOver(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("inputImageRequestButton-over");
        sender.removeClass("inputImageRequestButton-on");
        sender.removeClass("inputImageRequestButton-off");
    }
}


/***************************************************
/* 画像確認ボタンイベント */
function ImageNum_Check(id) {
    // 選択行とそのclass名取得(色設定用)
    var $sender = $("#" + id);
    var className = $sender.attr("class");

    // 確認ダイアログ
    var checkType;
    if (className == ROW0 || className == ROW1) {
//        if (!window.confirm("画像確認を完了します。よろしいですか？")) {
//            return;
//        }
        // 画像確認ファイル出力
        checkType = "0";
    }
    else if (className == IMAGECHECKROW0 || className == IMAGECHECKROW1) {
//        if (!window.confirm("画像確認を取消します。よろしいですか？")) {
//            return;
//        }
        // 画像確認ファイル削除
        checkType = "1";
    }
    else {
        return;
    }

    // ボタンクリックの行からレポートデータ取得
    var report = $sender.data("reportview");
    // 連想配列に起動引数追加
    var prm = {};
    prm.orderno = report._OrderNo;

    // 画像確認処理
    // common.js(SaveImageCheck_Startメソッド呼び出し)
    if (SaveImageCheck_Start(checkType, prm)) {
        // 処理成功なら対象行の色変更
        switch (className) {
            case ROW0:
                $sender.addClass(IMAGECHECKROW0);
                $sender.removeClass(ROW0);
                break;
            case ROW1:
                $sender.addClass(IMAGECHECKROW1);
                $sender.removeClass(ROW1);
                break;
            case IMAGECHECKROW0:
                $sender.addClass(ROW0);
                $sender.removeClass(IMAGECHECKROW0);
                break;
            case IMAGECHECKROW1:
                $sender.addClass(ROW1);
                $sender.removeClass(IMAGECHECKROW1);
                break;
        }
    }
}
/* 一覧画像確認ボタンマウスダウンイベント */
function CheckMouseDonw(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("inputImageCheckButton-on");
        sender.removeClass("inputImageCheckButton-off");
        sender.removeClass("inputImageCheckButton-over");
    }
}
/* 一覧画像確認ボタンマウスアップイベント */
function CheckMouseUp(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("inputImageCheckButton-off");
        sender.removeClass("inputImageCheckButton-on");
        sender.removeClass("inputImageCheckButton-over");
    }
}
/* 一覧画像確認ボタンマウスアップイベント */
function CheckMouseOver(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("inputImageCheckButton-over");
        sender.removeClass("inputImageCheckButton-on");
        sender.removeClass("inputImageCheckButton-off");
    }
}


/***************************************************
/* 依頼者絞り込み依頼レポート検索ボタンイベント */
function UserSearch() {
    // 選択テキスト取得
    var selectUser = $cmbUserList.children(":selected").text();

    // 空でも許可(全ユーザー対象)
    //    if (selectUser.length == 0) {
    //        alert("検索するユーザーが選択されていません。\n選択して再度検索を行って下さい。");
    //        return;
    //    }

    // 依頼者を表示
    // 読影者を非表示
    $lstCol_RequestedPhysicianName.css("display", "");
    $lstCol_AuthorizationPhysicianName.css("display", "none");

    // 検索
    // 引数のユーザー名で検索
    Search_RequestReport("1", selectUser, Search_RequestReport_Result);
}

/* 依頼者絞り込み依頼レポート検索ボタンマウスダウン */
function UserSearchMouseDonw(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnUserSearch-on");
        sender.removeClass("btnUserSearch-off");
        sender.removeClass("btnUserSearch-over");
    }
}
/* 依頼者絞り込み依頼レポート検索ボタンマウスアップ */
function UserSearchMouseUp(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnUserSearch-off");
        sender.removeClass("btnUserSearch-on");
        sender.removeClass("btnUserSearch-over");
    }
}
/* 依頼者絞り込み依頼レポート検索ボタンマウスオーバー */
function UserSearchMouseOver(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnUserSearch-over");
        sender.removeClass("btnUserSearch-off");
        sender.removeClass("btnUserSearch-on");
    }
}

/***************************************************
/* 依頼者絞り込み読影済みレポート検索ボタンイベント */
function UserSearchREAD() {
    // 選択テキスト取得
    var selectUser = $cmbUserList.children(":selected").text();

    // 空でも許可(全ユーザー対象)
    //    if (selectUser.length == 0) {
    //        alert("検索するユーザーが選択されていません。\n選択して再度検索を行って下さい。");
    //        return;
    //    }

    // 依頼者を非表示
    // 読影者を表示
    $lstCol_RequestedPhysicianName.css("display", "none");
    $lstCol_AuthorizationPhysicianName.css("display", "none");

    // 検索
    // 引数のユーザー名で検索
    Search_ReadReport("1", selectUser, Search_RequestReport_Result);
}
/* 依頼者絞り込み読影済みレポート検索ボタンマウスダウン */
function UserSearchREADMouseDonw(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnUserSearchREAD-on");
        sender.removeClass("btnUserSearchREAD-off");
        sender.removeClass("btnUserSearchREAD-over");
    }
}
/* 依頼者絞り込み読影済みレポート検索ボタンマウスアップ */
function UserSearchREADMouseUp(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnUserSearchREAD-off");
        sender.removeClass("btnUserSearchREAD-on");
        sender.removeClass("btnUserSearchREAD-over");
    }
}
/* 依頼者絞り込み読影済みレポート検索ボタンマウスオーバー */
function UserSearchREADMouseOver(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("btnUserSearchREAD-over");
        sender.removeClass("btnUserSearchREAD-off");
        sender.removeClass("btnUserSearchREAD-on");
    }
}

function ChagebottonImage()
{
    if ($("#viewercheckBox").is(':checked')) {

        //予備Viewr用に各種切り替え

        //読影ボタン　予備変更　inputReadButton
        $(".ReadButton input").removeClass("inputReadButton-off");
        $(".ReadButton input").removeClass("inputReadButton-over");
        $(".ReadButton input").removeClass("inputReadButton-on");
        $(".ReadButton input").addClass("inputReadButtonReserve-off");
        $(".ReadButton input").removeClass("inputReadButtonReserve-on");
        $(".ReadButton input").removeClass("inputReadButtonReserve-over");

        //閲覧ボタン　予備変更
        $(".ViewerButton input").removeClass("inputNoneditButton-off");
        $(".ViewerButton input").removeClass("inputNoneditButton-on");
        $(".ViewerButton input").removeClass("inputNoneditButton-over");
        $(".ViewerButton input").removeClass("inputNoneditButtonReserve-on");
        $(".ViewerButton input").addClass("inputNoneditButtonReserve-off");
        $(".ViewerButton input").removeClass("inputNoneditButtonReserve-over");
    } else {

        //通常に切り替え

        //読影ボタン　初期化　inputReadButton
        $(".ReadButton input").addClass("inputReadButton-off");
        $(".ReadButton input").removeClass("inputReadButton-over");
        $(".ReadButton input").removeClass("inputReadButton-on");
        $(".ReadButton input").removeClass("inputReadButtonReserve-off");
        $(".ReadButton input").removeClass("inputReadButtonReserve-on");
        $(".ReadButton input").removeClass("inputReadButtonReserve-over");

        //閲覧ボタン　初期化
        $(".ViewerButton input").addClass("inputNoneditButton-off");
        $(".ViewerButton input").removeClass("inputNoneditButton-on");
        $(".ViewerButton input").removeClass("inputNoneditButton-over");
        $(".ViewerButton input").removeClass("inputNoneditButtonReserve-on");
        $(".ViewerButton input").removeClass("inputNoneditButtonReserve-off");
        $(".ViewerButton input").removeClass("inputNoneditButtonReserve-over");
    }
}

