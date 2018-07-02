/*
/* 共通関数
/*
*/

var viewerWindow;
var viewerWindow2 =null;
var reportWindow;
var ReportWindow;
var Serialno;
var Edit_flg;

var viewReserveflg = 0;


/* 直前に他人が読影していた場合に、確認ダイアログ表示　*/
function readCheck(serialno, edit_flg, reportWindow) {
    var flg = false;

    // HTTP通信
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/ReadingCheck",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            // エラー判定
            if (result.d == null || result.d == "Error") {
                alert("読影ステータス確認の際にエラーが発生しました。");
                return;
            }
            //Errが返ってきたら、上書きしてよいかの確認ダイアログを表示
            if (result.d.Result == "Err") {

                //上書き確認の際は、デフォルトで「いいえ」選択二変更
                //if (window.confirm(result.d.Message)) {


                //    ////ステータス更新処理スタート　
                //    Reading_start(serialno, reportWindow, edit_flg);

                //} else {
                //    flg = false;
                //    return flg;//いいえが選択された、読影を開始しないためキャンセル
                //}

                daialog(result.d.Message, Reading_start_update);
                ReportWindow = reportWindow;
                Serialno = serialno;
                Edit_flg = edit_flg;

            } else {

                Reading_start(serialno, reportWindow, edit_flg);
                
            }
        },
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
    return flg;
}
/* ReadingStart　ステータス更新処理*/
function Reading_start(serialno, reportWindow, edit_flg) {

    // HTTP通信開始
    $.ajax({
    async: false,
    cache: false,
    type: "POST",
    url: "./CommonWebService.asmx/ReadingStart",
    data: "{serialNo:\"" + serialno + "\"}",
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    success: function (result) {
        if (reportWindow) {
            ReportWindow_Open(serialno, edit_flg);
            return;
        } else {
            Read_start_action();
        }
    },
    error: function (result) {
        // エラー
        alert("通信エラーが発生し、読影ステータスの変更に失敗しました。\nシステム管理者に連絡してください。");
    }
});
}

function Reading_start_update() {

    // HTTP通信開始
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/ReadingStart",
        data: "{serialNo:\"" + Serialno + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (ReportWindow) {
                ReportWindow_Open(Serialno, Edit_flg);
                return;
            } else {
                Read_start_action();
            }
        },
        error: function (result) {
            // エラー
            alert("通信エラーが発生し、読影ステータスの変更に失敗しました。\nシステム管理者に連絡してください。");
        }
    });
}
/* JSON通信 禁則文字エスケープ */
function EscapeString(str) {

    // バックスラッシュを1番目に置換する
    // 他の文字エスケープでバックスラッシュで置換するので先頭で行う
    str = str.replace(/\\/g, "\\\\");

    // シングルクォート
    str = str.replace(/'/g, "\\'");
    // ダブルクォート
    str = str.replace(/\"/g, "\\\"");
    // スラッシュ(Unicode)
    str = str.replace(/\u002f/g, "\\/");
    // backspace キー
    str = str.replace(/[\b]/g, "");

    return str;
}

/* JSON通信 禁則文字エスケープ処理 */
function Escape(str) {

    str = str.replace(/&/g, "&amp;");

    str = str.replace(/"/g, "&quot;");

    str = str.replace(/</g, "&lt;");

    str = str.replace(/>/g, "&gt;");

    return str;
}

/* SQLDBクエリ ワイルドカード、エスケープ処理 */
function Escapewildcard(str) {

    str = str.replace(/＊/g, "");

    str = str.replace(/\*/g, "");

    str = str.replace(/%/g, "");

    str = str.replace(/％/g, "");

    str = str.replace(/_/g, "");

    str = str.replace(/]/g, "");

    str = str.replace(/\[/g, "");

    str = str.replace(/\^/g, "");

    str = str.replace(/"/g, "");

    str = str.replace(/\\/g, "");

    return str;
}

/***************************************************
/* WEBViewer起動 */
function WebViewer_Start(prm,viewReserveflg) {
    // HTTP通信
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/GetViewerUrl",
        data: "{serialno:\"" + prm.serialno + "\",orderno:\"" + prm.orderno + "\",patientid:\"" + prm.patientid + "\",studydate:\"" + prm.studydate + "\",modality:\"" + prm.modality + "\",viewReservflg:\"" + viewReserveflg + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: WebViewer_Start_Result,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
}
function WebViewer_Start(prm, viewReserveflg) {
    // HTTP通信
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/GetViewerUrl",
        data: "{serialno:\"" + prm.serialno + "\",orderno:\"" + prm.orderno + "\",patientid:\"" + prm.patientid + "\",studydate:\"" + prm.studydate + "\",modality:\"" + prm.modality + "\",viewReservflg:\"" + viewReserveflg + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: WebViewer_Start_Result,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
}
function WebViewer2_Start(prm, viewReserveflg) {
    // HTTP通信
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/GetViewerUrl2",
        data: "{serialno:\"" + prm.serialno + "\",orderno:\"" + prm.orderno + "\",patientid:\"" + prm.patientid + "\",studydate:\"" + prm.studydate + "\",modality:\"" + prm.modality + "\",viewReservflg:\"" + 0 + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: WebViewer_Start_Result,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
}
function WebViewer_Start_Result(result) {
    // エラー判定
    if (result.d == null || result.d.Result == "Error") {
        alert("Viewer起動でサービスエラーが発生しました。");
        return;
    }

    if (result.d.ViewerURL.length > 0) {
        // Viewerページ表示(新規)

        ////開いている場合は一度Windowを閉じる
        //if (viewerWindow) {
        //    if (viewerWindow.closed) {
        //        try {
        //            // IEの場合、子ウィンドウが制御できない場合があるため強制的に空ページを開き、閉じる
        //            window.open("", "ProMedViewer").close();
        //        }
        //        catch (e) { }
        //    }
        //    else {
        //        viewerWindow.close();
        //    }
        //}

        var viewerWindowName;
        if (viewReserveflg == 0) {
            viewerWindowName = "ProMedViewer";
        } else {
            viewerWindowName = "ProMedViewer_reserve";
        }

        //viewerWindow = window.open(result.d.ViewerURL, "ProRadiRS_Nadia");
        //viewerWindow = window.open("", viewerWindowName);
        //viewerWindow.location = result.d.ViewerURL;
        viewerWindow = window.open(result.d.ViewerURL, viewerWindowName);
        
    }
}

/***************************************************
/* WEBViewer起動 */
function WebViewer_Start2(prm) {
    // HTTP通信
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/GetViewerUrl",
        data: "{serialno:\"" + prm.serialno + "\",orderno:\"" + prm.orderno + "\",patientid:\"" + prm.patientid + "\",studydate:\"" + prm.studydate + "\",modality:\"" + prm.modality + "\",viewReservflg:\"" + 0 + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: WebViewer_Start_Result2,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
}
function WebViewer_Start_Result2(result) {
    // エラー判定
    if (result.d == null || result.d.Result == "Error") {
        alert("Viewer起動でサービスエラーが発生しました。");
        return;
    }


    if (result.d.ViewerURL.length > 0) {
        // Viewerページ表示(新規)

        ////開いている場合は一度Windowを閉じる
        //if (viewerWindow) {
        //    if (viewerWindow.closed) {
        //        try {
        //            // IEの場合、子ウィンドウが制御できない場合があるため強制的に空ページを開き、閉じる
        //            if (viewerWindow.opener) {
        //                window.open("", "ProRadiRS_Nadia").close();
        //            }
        //        }
        //        catch (e) { }
        //    }
        //    else {
        //        viewerWindow.close();
        //    }
        //}

        viewerWindow2 = window.open(result.d.ViewerURL, "ProRadiRS_Nadia2");

    }
}


/***************************************************
/* Viewer画像取得要求ファイル出力 */
function ViewerImageRequest_Start(prm) {
    // HTTP通信
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/RequestViewerImage",
        data: "{orderno:\"" + prm.orderno + "\",patientid:\"" + prm.patientid + "\",studydate:\"" + prm.studydate + "\",modality:\"" + prm.modality + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: ViewerImageRequest_Start_Result,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
}
function ViewerImageRequest_Start_Result(result) {
    // エラー判定
    if (result.d == null || result.d.Result == "Error") {
        alert("画像取得要求でサービスエラーが発生しました。");
        return;
    }
}


/***************************************************
/* VIEWER画像PATH・履歴レポート画像参照PATHクリア */
function ImagePath_Clear() {
    // HTTP通信
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/ClearImagePath",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: ImagePath_Clear_Result,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
}
function ImagePath_Clear_Result(result) {
    // エラー判定
    if (result.d == null || result.d.Result == "Error") {
        alert("画像PATHクリアでサービスエラーが発生しました。");
        return;
    }
}


/***************************************************
/* 画像確認ファイル出力 */
function SaveImageCheck_Start(type, prm) {
    var funcResult = false;
    // HTTP通信
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/SetViewerImageCheckFile",
        data: "{checktype:\"" + type + "\",orderno:\"" + prm.orderno + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            // エラー判定
            funcResult = true;
            if (result.d == null || result.d.Result == "Error") {
                alert("画像確認でサービスエラーが発生しました。");
                funcResult = false;
            }
        },
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
            funcResult = false;
        }
    });
    return funcResult;
}



/* 日付・時刻の書式設定 戻り値：YYYY/MM/DD HH:MM */
function formatDateTime(date, time) {
    var dateTime = "";

    // 日付書式(YYYYMMDD ⇒ YYYY/MM/DD)
    var d = formatDate(date);

    // 時刻書式(HHMMSS ⇒ HH:MM)
    var t = formatTime(time);

    dateTime += d;
    if (t.length > 0) {
        dateTime += " ";
    }
    dateTime += t;

    return dateTime;
}
/* 日付の書式設定 戻り値：YYYY/MM/DD */
function formatDate(date) {
    var d = "";

    if (date != null && date != undefined) {
        if (date.length == 8) {
            d = date.slice(0, 4) + "/" + date.slice(4, 6) + "/" + date.slice(6, 8);
        }
        else {
            d = date;
        }
    }

    return d;
}
/* 時刻の書式設定 戻り値：HH:MM */
function formatTime(time) {
    var t = "";

    if (time != null && time != undefined) {
        if (time.length >= 4) {
            t = " " + time.slice(0, 2) + ":" + time.slice(2, 4);
        }
        else {
            t = time;
        }
    }

    return t;
}

/* ステータス文字列書式設定 */
function formatReportStatus(ReportStatus, ReadingStatus, hasTempSave) {
    var Status = "";

    // レポートステータス文字列
    switch (ReportStatus) {
        case 0:
            Status = "未検";

            break;
        case 1:
            Status = "未読";
            
            break;
        case 2:
            Status = "仮確";
            
            break;
        case 3:
            Status = "確定";
            
            break;
        default:
            Status = "未定義[" + ReportStatus + "]";
            
            break;
    }
    // 読影中文字列
    if (ReadingStatus == 1) {
        switch (ReportStatus) {
            case 0:
            case 1:
                Status = "読影中";
                break;
            case 2:
            case 3:
                Status = "変更中";
                break;
            default:
                Status = "未定義[" + ReportStatus + "]";
                break;
        }
    }
    // 一時保存データ保持文字列
    if (hasTempSave) {
        Status = "【保】" + Status;
    }

    return Status;
}


/* 緊急文字列書式設定 */
function formatPriorityStatus(PriorityFlag) {
    var Priority = "";

    // 緊急文字列
    switch (PriorityFlag) {
        case 1:
            Priority = "緊急";
            break;
    }

    return Priority;
}


/* 一時保存データの有無判定 */
function checkTempSave(SerialNo, TempSaveList) {
    var hasTempSave = false;

    // 一時保存データの有無判定
    for (var i = 0; i < TempSaveList.length; i++) {
        if (SerialNo == TempSaveList[i]) {
            hasTempSave = true;
            break;
        }
    }

    return hasTempSave;
}


// 日付入力文字列取得処理
function getInputDateString(strDate) {
        // 区切り文字削除
        var temp = strDate.replace(/\D/g, "");

        // 範囲外
        if (temp.length != 8) {
            return "";
        }

        // 妥当性チェック
        var yy = temp.substring(0, 4);
        var mm = temp.substring(4, 6);
        var dd = temp.substring(6, 8);
        var dt = new Date(yy, mm - 1, dd);
        if (dt == null || dt.getFullYear() != yy || dt.getMonth() + 1 != mm || dt.getDate() != dd) {
            return "";
        }
        return temp;
}

//ダイアログ

function layer(show, $target, $obj) {
    if (show) {
        if (!$target.children().is("#Layer")) {
            // Tabキーでフォーカスが移動しないよう設定
            $("body").find(":input").attr("tabindex", "-1");

            $target.append(
                $("<div>").attr("id", "Layer").append(
                    $("<div>").attr("id", "Layer-Opacity")
                ).append(
                    $("<div>").attr("id", "Layer-Opacity-Content").append(
                        $obj
                    )
                )
            );
            var width = $obj.outerWidth();
            var height = $obj.outerHeight();
            $("#Layer-Opacity-Content").css("marginLeft", -(width / 2) + "px").css("marginTop", -(height / 2) + "px");
        }
    }
    else {
        $("#Layer").remove();

        func = null;
        // Tabキーのフォーカス制御を解除
        $("body").find(":input").removeAttr("tabindex");
    }
}
//ダイアログメッセージ
function daialog(msg, func) {

    // レイヤー非表示
    layer(false);

    // 要素作成
    var $obj2 = "<button id=\"Common-Layer-OK\"><div></div></button><button id=\"Common-Layer-Close\"><div></div></button>";
    var $obj =
        $("<div>").attr("id", "Common-Layer").append(
            $("<div>").attr("id", "Common-Layer-Message").append(
                $("<span>").text(msg)
            )
        ).append(
            $obj2     
        );

    // レイヤー表示
    layer(true, $("body"), $obj);

    // フォーカス設定
    $("#Common-Layer-Close").focus();

    // イベント設定
    $("#Common-Layer-Close").on({
        "click": function () {

            // レイヤー非表示
            layer(false);

        }
    });
    $("#Common-Layer-OK").on({
        "click": function () {
            // コールバック実施
            if ($.isFunction(func)) {
                func();
            }

            // レイヤー非表示
            layer(false);
        }
    });
}

function unload() {

    if (viewerWindow2) {
        viewerWindow2.window.close();
    }
}
