/* グローバル宣言 */
var $ReportConfig;              // レポート情報格納オブジェクト
var $ReportHistory;             // 検査暦オブジェクト
var $SelectImage;               // 選択(クリック)画像オブジェクト

var $ReportButtonViewer;        // Viewerボタン
var $ReportButtonImage;         // キー画像取込ボタン
var $ReportButtonImageDelete;   // キー画像削除ボタン
var $ReportButtonSave;          // 確定ボタン
var $ReportButtonReturnList;    // 一覧へ戻るボタン
var $ReportButtonTempSave;      // 一時保存ボタン
var $ReportButtonExamOrder;     // 依頼票ボタン

var $ReportButtonReadCancel;    // 読影取り消しボタン
var $ReportButtonReadStart;    // 読影開始ボタン

var $ReportButtonSentence;      // 定型文ボタン
var $ReportButtonHistory;       // 変更履歴ボタン

var $IsTempSave;                // 一時保存の有無(読み出し時にもON)

var $SentenceDlg;
var $ChangeHistoryDlg;
var $cmbUserList;               // 依頼者絞り込みコンボボックスオブジェクト
var $LoginUserCd;               // ログインユーザーコード
var $historycd;                 // 日付ペースト用

var clipboardImageName;
var $ReportKeyImage;    //レポートキー画像イメージ保管
var editFlg = 0; //閲覧フラグ　初期値0　読影モード　、1　閲覧モード
var colorFlg =""; //検査暦参照ボタン押下時

var isModal = false;
var isDrag = false;
var isCheck = true;

/***************************************************
/* 起動処理 */
$(window).load(function () {
    // 起動処理
    ReportWindow_Init();

    // 画面構築
    ReportCenter_Init();

    // 選択レポート表示
    Report_View();

    // VIEWER画像取込PATH・履歴レポート画像参照PATHクリア(common.js)
    ImagePath_Clear();

    // 画面内リサイズ
    resize();
});
function ReportWindow_Init() {
    // オブジェクトキャッシュ
    $ReportConfig = $("#ReportConfig");
    $ReportButtonViewer = $("#btnViewer");
    $ReportButtonImage = $("#btnImage");
    $ReportButtonImageDelete = $("#btnImageDelete");
    $ReportButtonSave = $("#btnSave");
    $ReportButtonReturnList = $("#btnReturnList");
    $ReportButtonTempSave = $("#btnTempSave");
    $ReportButtonExamOrder = $("#btnExamOrder");
    $ReportButtonReadCancel = $("#btnReadCancel");
    $ReportButtonReadStart = $("#btnReadStart");

    $ReportButtonSentence = $("#btnSentence");
    $ReportButtonHistory = $("#btnEditHist");

    $SentenceDlg = $("#SentenceDlg")
    $ChangeHistoryDlg = $("#ChangeHistoryDlg")
    $cmbUserList = $("#cmbUserList");
    $historycd = -1;

    // 画面の画像ファイル先読み込み
    ReportWindow_PreLoadImages();

    // HTTP通信開始
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/GetParams",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: ReportWindow_Init_Result,
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
        SentenceSearch();
    }
}
function ReportWindow_Init_Result(result) {
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


/***************************************************
/* 画面の画像ファイル先読み込み(チラつき防止) */
var upd = "?20150618b";
function ReportWindow_PreLoadImages() {
    jQuery("<img>").attr("src", "./img/Report/キー画像取込_off.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/キー画像取込_on.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/キー画像取込_over.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/ヘッダーBG.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/依頼内容_見出し.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/一覧へ戻る_off.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/一覧へ戻る_on.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/一覧へ戻る_over.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/画像データ取得_off.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/画像データ取得_on.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/画像データ取得_over.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/画像所見_見出し.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/確定_off.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/確定_on.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/確定_over.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/左上情報BG.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/最下段ボタン後ろBG.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/参照_off.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/参照_on.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/参照_over.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/診断_見出し.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/連絡_見出し.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/Viewer_off.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/Viewer_on.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/Viewer_over.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/キー画像_見出し.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/キー画像削除_off.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/キー画像削除_on.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/キー画像削除_over.png" + upd);    
    jQuery("<img>").attr("src", "./img/Report/一時保存_off.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/一時保存_on.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/一時保存_over.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/依頼票_off.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/依頼票_on.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/依頼票_over.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/依頼票_disable.png" + upd);

    jQuery("<img>").attr("src", "./img/Report/読影医師_見出し.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/読影取消_off.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/読影取消_on.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/読影取消_over.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/読影開始_off.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/読影開始_on.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/読影開始_over.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/キー画像削除_disable.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/一時保存_disable.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/確定_disable.png" + upd);
    jQuery("<img>").attr("src", "./img/Report/キー画像取込_disable.png" + upd);
}
//クリップボード取込
function clipboard(e) {

    $("#Image").context.execCommand("paste");

    var　MaxImagenum = $("#ReportConfig").data("MaxImageImportNum")
    var isShowMsg = true;
    if ($("#ReportImages").children(".KeyImage-box").length < MaxImagenum)
    {
        isShowMsg = false;
    }
    if (isShowMsg) {
        alert("画像取得件数を超えています。\n画像を追加する場合は、不要な画像を削除して下さい。");
        return false;
    }
    $("#ReportImages").append('<div class = "KeyImage-box"><div id="Image" style="width:100px;height:100px;border:3px solid #ff0000;margin:18px 5px 5px 10px;" contenteditable="true">ここを選択しCtrl + Vを押してください。</div></div>');
    var serialNo = $ReportConfig.data("serialno");

    //クリップボート取込検証用-------------------------------------------------------------------
    $("#Image").on({
        "paste": function (e) {
            e.preventDefault();
            get_upload_file_data(e);
        },
        "keydown": function (e) {
            // ペースト以外は無効
            if (!e.ctrlKey || e.keyCode != 86) {
                e.preventDefault();
            }

        },
        "click": function (e) {
            // IEは動作した
            $("#Image").context.execCommand("paste");
        }
    });



    // アップロードファイル取得
    function get_upload_file_data(e) {
        // event からクリップボードのアイテムを取り出す
        var evt = e.originalEvent;
        if (evt.clipboardData) {
            // Chromeのみ
            var items = evt.clipboardData.items;
            if (!items) return;
            for (var i = 0 ; i < items.length ; i++) {
                var item = items[i];
                if (item.type.indexOf("image") != -1) {
                    // 画像のみサーバへ送信する
                    var file = item.getAsFile();
                    upload_file_with_ajax(file);
                }
            }
        }
        else if (window.clipboardData) {
            // IE11のみ
            var fileList = window.clipboardData.files;
            if (!fileList) return;
            for (var i = 0; i < fileList.length; i++) {
                //var file = fileList[i];
                //var url = URL.createObjectURL(file);

                //if (evt.convertURL) { // Use standard if available.
                //    evt.convertURL(file, "specified", url);
                //} else {
                //    evt.msConvertURL(file, "specified", url);
                //}

                //console.log("Local file: " + file.name + " (" + file.size + ")");
                upload_file_with_ajax(fileList[i]);
            }
        }
    }

    // ファイルアップロード
    function upload_file_with_ajax(file) {
        var formData = new FormData();

        //現在日付の取得
        dd = new Date();
        yy = dd.getYear();
        MM = dd.getMonth() + 1;
        ss = dd.getMilliseconds();
        mm = dd.getMinutes();
        hh = dd.getHours();
        dd = dd.getDate();
        
        if (yy < 2000) {
            yy += 1900;
        }
        if (MM < 10) {
            MM = "0" + MM;
        }
        if (dd < 10) {
            dd = "0" + dd;
        }
        if (mm < 10) {
            mm = "0" + mm;
        }
        if (ss < 10) {
            ss = "0" + ss;
        }
        var date = yy + MM + dd + hh + mm + ss;
        clipboardImageName = serialNo + "_" + date;
        formData.append(clipboardImageName + ".png", file);  // PNGフォーマットで格納されている

        clipboardImageName += ".jpg";

        $.ajax("./Upload.aspx", {
            type: "POST",
            contentType: false,
            processData: false,
            data: formData,
            error: function () {
                // アップロードエラー処理
            },
            success: KeyImagereload()
        });
    }
}


/***************************************************
/* 画面構築 */
function ReportCenter_Init() {
    // レポート表示tableタグ構築
    var tblTag = "";
//20150309 A.Umeda レイアウト変更に伴い改修　-----------------------------------------------------------START
    // ReportCenter

    //1行目ReportRow1
    tblTag += "<th id=\"ReportRow1-HeadCol1\" class=\"ReportRow-Header\"></th>";
    tblTag += "<th id=\"ReportRow1-HeadCol2\" class=\"ReportRow-Header\"></th>";
    tblTag += "<th id=\"ReportRow1-HeadCol3\" class=\"ReportRow-Header\"></th>";
    tblTag += "<th id=\"ReportRow7-Col\" class=\"ReportRow-Header\"></td>";
    tblTag += "<th id=\"ReportRow1-HeadCol4\" class=\"ReportRow-Header\"></th>";
    tblTag += "<th id=\"ReportRow1-HeadCol5\" class=\"ReportRow-Header\"></th>";
    tblTag += "<th id=\"ReportRow1-HeadCol3\" class=\"ReportRow-Header\"></th>";
//    tblTag += "</tr>";

    $("#ReportRow1").append(tblTag);

    tblTag = "";
    //2行目(依頼内容)
    tblTag += "<td id=\"ReportRow2-Col\" class=\"ReportRow-Comment1\"><div id=\"ReportRow2-Caption\"></div></td>";
    tblTag += "<td class=\"ReportRow-Comment1\"><textarea id=\"TxtReportComment1\" class=\"ReportComment\" readonly=\"readonly\" rows=\"\" cols=\"\"></textarea></td>";
    tblTag += "<td class=\"ReportRow-Comment1\"></td>";
    tblTag += "</tr>";

    $("#ReportRow2").append(tblTag);

    tblTag = "";
    //3行目(連絡事項)
    tblTag += "<td id=\"ReportRow3-Col\" class=\"ReportRow-Comment2\"><div id=\"ReportRow3-Caption\"></div></td>";
    tblTag += "<td class=\"ReportRow-Comment2\"><textarea id=\"TxtReportComment2\" class=\"ReportComment\" readonly=\"readonly\" rows=\"\" cols=\"\"></textarea></td>";
    tblTag += "<td class=\"ReportRow-Comment2\"></td>";
    tblTag += "<td id=\"ReportRow7-Col\" class=\"ReportRow-Comment2\"><div id=\"ReportRow7-Caption\"></div></td>";
    tblTag += "<td class=\"ReportRow-Comment2\"><textarea id=\"TxtPastReportComment2\" class=\"ReportComment\" readonly=\"readonly\" rows=\"\" cols=\"\"></textarea></td>";
    tblTag += "<td class=\"ReportRow-Comment2\" id=\"ReportRow9-Col\"><div id=\"ReportRow8-Caption\"></div><div><input id=\"RadiologistName\" class=\"ReportComment\" readonly=\"readonly\"/></div></td>";
    tblTag += "<td id=\"ReportRow1-HeadCol3\"></td></tr>";

    $("#ReportRow3").append(tblTag);

    tblTag = "";
    //3行目(画像所見)
    tblTag += "<td id=\"ReportRow4-Col\" class=\"ReportRow-Finding\"><div id=\"ReportRow4-Caption\"></div></td>";
    tblTag += "<td class=\"ReportRow-Finding\"><textarea id=\"TxtFinding\" class=\"ReportRead\" tabindex=\"1\" rows=\"\" cols=\"\"></textarea></td>";
    tblTag += "<td class=\"ReportRow-Finding\"></td>";
    tblTag += "<td colspan=\"3\" class=\"ReportRow-Finding\"><textarea id=\"TxtPastFinding\" class=\"ReportRead\" readonly=\"readonly\" rows=\"\" cols=\"\"></textarea></td>";
    tblTag += "<td id=\"ReportRow1-HeadCol3\"></td></tr>";

    $("#ReportRow4").append(tblTag);

    tblTag = "";
    //4行目(診断)
    tblTag += "<td id=\"ReportRow5-Col\" class=\"ReportRow-Diagnosing\"><div id=\"ReportRow5-Caption\"></div></td>";
    tblTag += "<td class=\"ReportRow-Diagnosing\"><textarea id=\"TxtDiagnosing\" class=\"ReportRead\" tabindex=\"2\" rows=\"\" cols=\"\"></textarea></td>";
    tblTag += "<td class=\"ReportRow-Diagnosing\"></td>";
    tblTag += "<td colspan=\"3\" class=\"ReportRow-Diagnosing\"><div><textarea id=\"TxtPastDiagnosing\" class=\"ReportRead\" readonly=\"readonly\" rows=\"\" cols=\"\"></textarea></div><div><input type='button' id='btnFCopy' class='SubButton' /><input type='button' id='btnDCopy' class='SubButton' /><input type='button' id='btnFDCopy' class='SubButton' /><input type='button' id='btnICopy' class='SubButton' /></div></td>";
//    tblTag += "<td colspan=\"3\" class=\"ReportRow-Diagnosing\"><textarea id=\"TxtPastDiagnosing\" class=\"ReportRead\" readonly=\"readonly\" rows=\"\" cols=\"\"></textarea></td>";
    tblTag += "<td id=\"ReportRow1-HeadCol3\"></td></tr>";

    $("#ReportRow5").append(tblTag);

    tblTag = "";
    //5行目(キー画像)
    tblTag += "<td id=\"ReportRow6-Col\" class=\"ReportRow-Images\"><div id=\"ReportRow6-Caption\"></div></td>";
    tblTag += "<td class=\"ReportRow-Images\"><div id=\"ReportImages\"></div></td>";
    tblTag += "<td class=\"ReportRow-Images\"></td>";
    tblTag += "<td colspan=\"3\" class=\"ReportRow-Images\"><div id=\"PastReportImages\"></div></td>";
    tblTag += "<td id=\"ReportRow1-HeadCol3\"></td></td>";
    tblTag += "</tr>";

    $("#ReportRow6").append(tblTag);

//    $("#ReportCenter").append(tblTag);
    
    tblTag = "";

    // ReportTop-Info-Right
    var hisTag = "";
    hisTag += "<table id=\"ReportHistory\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">";
    hisTag += "<thead><tr class=\"RowHeader\">";
    hisTag += "<th class=\"HisReportButton head\">&nbsp;</th>";
//    hisTag += "<th class=\"HisImageRequestButton head\">&nbsp;</th>";
    hisTag += "<th class=\"HisStudyDateTime head\">検査日</th>";
    hisTag += "<th class=\"HisModality head\">検査</th>";
    hisTag += "<th class=\"HisStudyBodyPart head\">部位</th>";
    hisTag += "<th class=\"HisDiagnosing head\">診断・結論</th>";
    hisTag += "<th class=\"HisSpace head\"></th>";
    hisTag += "</tr></thead>";
    hisTag += "<tbody></tbody>";
    hisTag += "</table>";
    $("#ReportTop-Info-Right").append(hisTag);
    // グローバル変数の検査暦オブジェクト取得
    $ReportHistory = $("#ReportHistory");

    resize();
//20150309 A.Umeda レイアウト変更に伴い改修　-----------------------------------------------------------END

    // ドラッグアンドドロップ対応(IE10以上)
    $("#ReportImages").on({
        "dragstart": function (e) {
            //閲覧モードの際は、ドラッグアンドドロップ無効
            if (editFlg == 1) {
                return false;
            }
            if (e.originalEvent.dataTransfer.types == undefined || e.originalEvent.dataTransfer.types.length != 0) {
                e.preventDefault();
                return;
            }

            e.originalEvent.dataTransfer.effectAllowed = "move";
            var index = $("#ReportImages").children().index(this);
            $("#ReportImages").data("dragindex", index);
        },
        "dragover": function (e) {
            if (e.originalEvent.dataTransfer.effectAllowed == "move") {
                e.preventDefault();
            }
        },
        "drop": function (e) {
            e.preventDefault();
            var start = $("#ReportImages").data("dragindex");
            var end = $("#ReportImages").children().index(this);
            if (start < end) {
                $(this).after($("#ReportImages").children().eq(start));
            }
            else if (start > end) {
                $(this).before($("#ReportImages").children().eq(start));
            }
            //イメージの○枚目か表示の再設定
            var size = $('#ReportImages>.KeyImage-box').length;
            for (var i = 0; i <= size ; i++) {
                var obj = $(".KeyImage-box:nth-child(" + i + ")");
                obj.children(".KeyImage-Count").text(i + "/" + size);
            }
        }
    }, ".KeyImage-box");


    // キー画像取込ダブルクリックイベント
    $("#ReportImages").dblclick(function (e) {
        //alert('dblclickイベントが発生しました。');
        if ($ReportButtonReadStart != null && $ReportButtonReadStart != undefined) {
            if ($ReportButtonReadStart.is(":visible")) {
                //alert('表示');
                return;
            }
            else {
                //alert('非表示');
                Image();
            }
        }
    });

    $('#btnFCopy').on('click', function () {
        if (editFlg != 0)
            return;
        OpinionCopyFinding();
    });
    $('#btnDCopy').on('click', function () {
        if (editFlg != 0)
            return;
        OpinionCopyDiagnosing();
    });
    $('#btnFDCopy').on('click', function () {
        if (editFlg != 0)
            return;
        OpinionCopyAll();
    });
    $('#btnICopy').on('click', function () {
        if (editFlg != 0)
            return;
        OpinionCopyDate();
    });

    $('#sent-cp').on('click', function () {
        if ($('#sent-cp-find')[0].checked == true) {
            $('#sent-finding').val($('#sent-finding').val() + $('#TxtFinding').val());
        }
        if ($('#sent-cp-diag')[0].checked == true) {
            $('#sent-diagnosing').val($('#sent-diagnosing').val() + $('#TxtDiagnosing').val());
        }
    });

    $('#sent-edit').on('click', function () {
        var vals = [];

        if ($('#sent-edit').data("cd") == '')
        {
            if (!confirm("定型文を登録します。\nよろしいですか？"))
                return;
        } else {
            if (!confirm("定型文を更新します。\nよろしいですか？"))
                return;
        }

        vals[vals.length] = $('#sent-edit').data("cd");
        vals[vals.length] = $('#sent-title').val();
        vals[vals.length] = $('#sent-finding').text();
        vals[vals.length] = $('#sent-diagnosing').text();
        vals[vals.length] = $('input[name=sent-public]:checked').val();
        vals[vals.length] = $LoginUserCd;

        SetSentenceData(vals);
        $('#modal-sent-edit').modal('hide');

    });
    $('#sent-delete').on('click', function () {
        if (!confirm("定型文を削除します。\nよろしいですか？"))
            return;

        DelSentenceData($('#sent-edit').data("cd"));
        $('#modal-sent-edit').modal('hide');
    });
    $('#sent-cancel').on('click', function () {
        if (!confirm("定型文の入力を取消します。\n入力内容は保存されませんが、よろしいですか？"))
            return;

        $('#modal-sent-edit').modal('hide');
    });


    $('#modal div.series-header').on('mousedown', function (e) {
        isDrag = true;
        mod_X = e.pageX;
        mod_Y = e.pageY;
        e.preventDefault();
        $('div.series-dialog').css('cursor', 'move');
    });
    $(document).on('mouseup', function (e) {
        if (isDrag) {
            e.preventDefault();
            $('div.series-dialog').css('cursor', 'auto');

            isDrag = false;
        }
    });
    $(document).on('mousemove', function (e) {
        if (!isDrag)
            return;

        var move_X = mod_X - e.pageX;
        var move_Y = mod_Y - e.pageY;

        var offset = $('#modal').offset();

        offset.left = offset.left - move_X;
        offset.top = offset.top - move_Y;
        mod_X = e.pageX;
        mod_Y = e.pageY;

        $('#modal').offset({ top: offset.top, left: offset.left });

    });

    $('#modal').on('shown.bs.modal', function () {
        $('#modal').offset({ top: 0, left: $('body').width() - 600 });
        $('#modal-child').css('margin', '0px 0px 0px 0px');
    });

    $(window).bind("focus", function () {　//フォーカスが当たったらタイマーを設定
        isCheck = false;
    }).bind("blur", function () {　//フォーカスが外れたらタイマーを解除
        if (editFlg == 0)
        {
            isCheck = true;
            C_Image();
        }
    });

    $('#modal').on('shown.bs.modal', function () {
        $('#modal').offset({ top: 0, left: $('body').width() - 600 });
        $('#modal-child').css('margin', '0px 0px 0px 0px');
    });
}



/***************************************************
/* レポート表示 */
function Report_View() {
    // 一覧で選択したレポートのSerialNo
    if ($("#ReportConfig").data("serialno") == null || $("#ReportConfig").data("serialno") == undefined) {
        alert("表示するレポートが不明です。");
        return;
    }

    $historycd = -1;
    var serialNo = $ReportConfig.data("serialno");
    // 一時保存データ読み出し判定
    var readTempSave = $ReportConfig.data("readtempsave");

    // レポート取得＋取得後画面セット
    ReportData_Get(serialNo, readTempSave, ReportData_Get_Result);
}
/***************************************************
/* レポート取得 */
function ReportData_Get(serialno, readTempSave, func) {
    // HTTP通信開始
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/GetReportData",
        data: "{serialNo:\"" + serialno + "\",readTempSave:\"" + readTempSave + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: func,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
}
/***************************************************
/* レポートデータ画面セット */
function ReportData_Get_Result(result) {
    // エラー判定
    if (result.d.Result == "Exception") {
        alert(result.d.Message);
        return;
    }
    if (result.d.Result == "Error") {
        alert(result.d.Message);
        return;
    }

    editFlg = result.d.editFlg;

    // ID
    //$("#Report-ID").text($ReportConfig.data("ipromedcode") + result.d.View.PatientID);
    $("#Report-ID").text( result.d.View.PatientID);


    // 生年月日
    var birthDate = formatDate(result.d.View.PatientBirthDate);
    // 年齢
    var age = "(" + result.d.View.PatientAge + "歳)";
    // 性別
    var sex = result.d.View.PatientSex;
    // 患者情報
    var info = "　　" + birthDate + "　" + age + "　" + sex;
    $("#Report-Info").text(info);

    // モダリティ
    var modality = result.d.View.Modality;

    //20150706 A.Umeda 確定前条件として、現在のモダリティ取得のため追加
    $ReportConfig.data("CurrentReportModality", result.d.View.Modality);

    // 検査実施日時
    var studyDateTime = formatDateTime(result.d.View.StudyDate, result.d.View.StudyTime);
    // 検査情報1
    var studyinfo1 = "モダリティ：" + modality + "　検査日：" + studyDateTime;
    $("#Report-StudyInfo1").text(studyinfo1);

    // 部位
    var studyBodyPart = result.d.View.StudyBodyPart;
    // 検査情報2
    var studyinfo2 = studyBodyPart;
    $("#Report-StudyInfo2").text(studyinfo2);

    // 依頼内容
    $("#TxtReportComment1").val(result.d.View.Comment1);
    // 連絡事項
    $("#TxtReportComment2").val(result.d.View.Comment2);
    // 所見
    $("#TxtFinding").val(result.d.View.Finding);
    // 診断
    $("#TxtDiagnosing").val(result.d.View.Diagnosing);

    // キー画像ファイル名リスト
    var image = new Array();
    for (var i = 0; i < result.d.Image.length; i++) {
        image.push(result.d.Image[i]);
    }
    $ReportKeyImage = result.d.Image;
    // キー画像のリセット
    $("#ReportImages").empty();
    // キー画像追加
    AddImage(image, $("#ReportImages"), "webImage.aspx", true, true, true, true);

    // 検査暦構築

    ReportHistory_Set(result.d.History);

    // 依頼票ボタン制御
    var examOrder = result.d.ExamOrder;
    if (examOrder != null && examOrder != undefined && examOrder.length > 0) {
        // 使用可
        $ReportButtonExamOrder.attr("disabled", false);
        $ReportButtonExamOrder.removeAttr("disabled");
        // カーソルをpointer(指マーク)
        $ReportButtonExamOrder.css("cursor", "pointer");
        // パス情報セット
        $ReportButtonExamOrder.data("examorder", examOrder);
    }
    else {
        // 使用不可
        $ReportButtonExamOrder.attr("disabled", true);
        // 無効表示
        $ReportButtonExamOrder.addClass("btnExamOrder-disable");
        $ReportButtonExamOrder.removeClass("btnExamOrder-off");
        // カーソルをauto(デフォルト)
        $ReportButtonExamOrder.css("cursor", "auto");
        // パス情報は空でセット
        $ReportButtonExamOrder.data("examorder", "");
    }

    //20150420 A.Umeda 閲覧フラグにて、ボタン制御を追加--------------------START
    if (result.d.editFlg == 1) {
        noneditmode();
        
    } else {
        //読影モード（読影取消ボタン表示）
        editmode();
    }
    //20150420 A.Umeda 閲覧フラグにて、ボタン制御を追加--------------------END

    // 画面内リサイズ Chromでリサイズイベントがうまく動作しないため追加
    resize();

    // 所見にフォーカス
    $("#TxtFinding").focus();
}
function ReportHistory_Set(list) {
    // リストクリア
    $("#ReportHistory tbody").empty();

    $historycd = -1;

    // リスト追加
    for (var i = 0; i < list.length; i++) {
        // 行ごとの背景色(色設定はReport.css)
        var className = "";
        if (i % 2 == 0) {
            className = "Row0";
        }
        else {
            className = "Row1";
        }

        // 連想配列で行データ作成
        var row = {};
        //row.HisStudyDateTime = formatDateTime(list[i].StudyDate, list[i].StudyTime);
        row.HisStudyDateTime = formatDateTime(list[i].StudyDate, "");
        row.HisModality = list[i].Modality;
        row.HisStudyBodyPart = list[i].StudyBodyPart;
        row.HisDiagnosing = list[i].Diagnosing;
        var coment = list[i].Finding;

        // 先頭が"_"の列は表示しない
        row._HisSerialNo = list[i].SerialNo;
        row._HisStudyDate = list[i].StudyDate;
        row._HisStudyTime = list[i].StudyTime;
        row._HisOrderNo = list[i].OrderNo;
        row._HisPatientID = list[i].PatientID;
        row._HisOfficeCd = list[i].ReportReserve1;

        var outStr = "<tr class=\"" + className + "\" id=\"" + row._HisSerialNo + "\">";
        // 参照ボタン
        var reportBtnId = "REP" + row._HisSerialNo;
        var readBtnStr = "<td class=\"HisReportButton\"><input class=\"inputReportButton inputReportButton-off\" id=\"" + reportBtnId + "\" type=\"button\" onclick=\"HistoryReport_View(" + row._HisSerialNo + ");\" onmousedown=\"HistoryReportMouseDonw(" + reportBtnId + ")\" onmouseup=\"HistoryReportMouseUp(" + reportBtnId + ")\" onmouseout=\"HistoryReportMouseUp(" + reportBtnId + ")\" onmouseover=\"HistoryReportMouseOver(" + reportBtnId + ")\"/></td>";
        outStr += readBtnStr;
        // 画像取得ボタン
        var reqBtnId = "REQ" + row._HisSerialNo;
        var requestBtnStr = "<td class=\"HisImageRequestButton\"><input class=\"inputImageRequestButton inputImageRequestButton-off\" id=\"" + reqBtnId + "\" type=\"button\" onclick=\"ViewerImage_Request(" + row._HisSerialNo + ");\" onmousedown=\"HistoryImageRequestMouseDonw(" + reqBtnId + ")\" onmouseup=\"HistoryImageRequestMouseUp(" + reqBtnId + ")\" onmouseout=\"HistoryImageRequestMouseUp(" + reqBtnId + ")\" onmouseover=\"HistoryImageRequestMouseOver(" + reqBtnId + ")\"/></td>";
        //        outStr += requestBtnStr;

        for (var key in row) {
            // 先頭が"_"のkeyは非表示列なので表示しない
            var indexof = key.indexOf("_");
            if (indexof != 0) {

                if (key == "HisDiagnosing") {
                    //ツールチップ表示で、所見と診断を表示    title=\"" + coment 
                    
                    //禁則文字Escape処理
                    coment = Escape(coment);
                    var temp = Escape(row[key]);
                    outStr += "<td class=\"" + key + "\" title=\"" + coment + "\">" + temp + "</td>";
                } else {
                    outStr += "<td title=\"" + row[key] + "\" class=\"" + key + "\">" + row[key] + "</td>";
                }
            }
        }
        outStr += "</tr>";

        $("#ReportHistory tbody").append(outStr);

        // レポート情報セット
        $("#" + row._HisSerialNo).data("historyview", row);
    }

    //スペース埋めのため追加
    if (list.length < 5) {
        for (var i = list.length; i < 5; i++) {
            // 行ごとの背景色(色設定はReport.css)
            var className = "";
            if (i % 2 == 0) {
                className = "Row0";
            }
            else {
                className = "Row1";
            }
            var outStr = "<tr class=\"" + className + "\" id=\"" + row._HisSerialNo + "\">";
            outStr += "<td></td>";//参照ボタン領域
            outStr += "<td></td>";//検査日領域
            outStr += "<td></td>";//検査領域
            outStr += "<td></td>";//部位領域
            outStr += "<td></td>";//診断結論領域
            outStr += "</tr>";
            $("#ReportHistory tbody").append(outStr);
        }
    }
}




/***************************************************
/* レポート画面リサイズイベント */
function resize() {
    // 全体表示サイズ
    var bodyHeight = $(window).height();
    // 固定領域表示サイズ
    var topHeight = $("#ReportTop").height();
    var bottomHeight = $("#ReportBottom").height();

    $('#modal-hist div.modal-dialog').width($(window).width() - 100);
    $('#HistryList').css('max-height', (bodyHeight - 200) + 'px');

    // リスト表示サイズ計算＋サイズ設定
    var centerHeight = bodyHeight - (topHeight + bottomHeight);

    var modalh = (bodyHeight - 550) / 2;
    if (modalh < 0)
        modalh = 0;

    $('#modal-sent-edit').css('top', modalh + "px");


    $("#Report-body").height(bodyHeight - bottomHeight);
    //    $("#ReportTop-Info").height(bodyHeight - bottomHeight -6);
    $("#ReportTop-Info").height(bodyHeight - bottomHeight-30);
    
    // 画像所見と診断欄の行サイズ設定
    // 固定値エリアの高さ取得(1行目＋2行目＋5行目)
    var contentAreaHeight = $("#ReportHeader").height() + $("#TxtReportComment1").height() + $("#TxtReportComment2").height() + $("#ReportImages").height();
    // ボーダーラインの高さ取得
    // ライン高さ
//    var borderH = parseInt($("#ReportTop-Info").attr("border"));
//    var cellspacingH = parseInt($("#ReportTop-Info").attr("cellspacing"));
//    var lineHeight = (borderH * 2) + cellspacingH;
//    // ラインの数取得((行数-1)+2(外側枠線分))
//    var tblRowNum = $("#ReportTop-Info tbody").children().length;
//    var borderLineNum = (tblRowNum - 1) + 2;
//    // ボーダーライン高さ(ライン高さ×数)
//    var borderLineHeight = lineHeight * borderLineNum;

    // ボーダーラインTOP(CSSの設定値取得)
    var cssComment1borderTop = $("#TxtReportComment1").css("border-top-width");
    if (cssComment1borderTop == null || cssComment1borderTop == undefined) {
        cssComment1borderTop = "0px";
    }
    var cssComment2borderTop = $("#TxtReportComment2").css("border-top-width");
    if (cssComment2borderTop == null || cssComment2borderTop == undefined) {
        cssComment2borderTop = "0px";
    }
    var cssFindingborderTop = $("#TxtFinding").css("border-top-width");
    if (cssFindingborderTop == null || cssFindingborderTop == undefined) {
        cssFindingborderTop = "0px";
    }
    var cssDiagnosingborderTop = $("#TxtDiagnosing").css("border-top-width");
    if (cssDiagnosingborderTop == null || cssDiagnosingborderTop == undefined) {
        cssDiagnosingborderTop = "0px";
    }
    var cssImagesborderTop = $("#ReportImages").css("border-top-width");
    if (cssImagesborderTop == null || cssImagesborderTop == undefined) {
        cssImagesborderTop = "0px";
    }
    // ボーダーラインBOTTOM(CSSの設定値取得)
    var cssComment1borderBottom = $("#TxtReportComment1").css("border-bottom-width");
    if (cssComment1borderBottom == null || cssComment1borderBottom == undefined) {
        cssComment1borderBottom = "0px";
    }
    var cssComment2borderBottom = $("#TxtReportComment2").css("border-bottom-width");
    if (cssComment2borderBottom == null || cssComment2borderBottom == undefined) {
        cssComment2borderBottom = "0px";
    }
    var cssFindingborderBottom = $("#TxtFinding").css("border-bottom-width");
    if (cssFindingborderBottom == null || cssFindingborderBottom == undefined) {
        cssFindingborderBottom = "0px";
    }
    var cssDiagnosingborderBottom = $("#TxtDiagnosing").css("border-bottom-width");
    if (cssDiagnosingborderBottom == null || cssDiagnosingborderBottom == undefined) {
        cssDiagnosingborderBottom = "0px";
    }
    var cssImagesborderBottom = $("#ReportImages").css("border-bottom-width");
    if (cssImagesborderBottom == null || cssImagesborderBottom == undefined) {
        cssImagesborderBottom = "0px";
    }
    // ボーダーライン高さ取得
    var comment1borderHeight = parseInt((cssComment1borderTop).replace("px", "")) + parseInt((cssComment1borderBottom).replace("px", ""));
    var comment2borderHeight = parseInt((cssComment2borderTop).replace("px", "")) + parseInt((cssComment2borderBottom).replace("px", ""));
    var findingborderHeight = parseInt((cssFindingborderTop).replace("px", "")) + parseInt((cssFindingborderBottom).replace("px", ""));
    var diagnosingborderHeight = parseInt((cssDiagnosingborderTop).replace("px", "")) + parseInt((cssDiagnosingborderBottom).replace("px", ""));
    var imagesborderHeight = parseInt((cssImagesborderTop).replace("px", "")) + parseInt((cssImagesborderBottom).replace("px", ""));


    // 自動調整で使える高さ
    //var autoSizeHeight = centerHeight - contentAreaHeight - borderLineHeight - comment1borderHeight - comment2borderHeight - findingborderHeight - diagnosingborderHeight - imagesborderHeight;
    //var autoSizeHeight = centerHeight - contentAreaHeight -16 - comment1borderHeight - comment2borderHeight - findingborderHeight - diagnosingborderHeight - imagesborderHeight;
    var autoSizeHeight = centerHeight - contentAreaHeight - 36 - comment1borderHeight - comment2borderHeight - findingborderHeight - diagnosingborderHeight - imagesborderHeight;

    
    // 画像所見高さ
    var findingHeight = autoSizeHeight * 0.6;
    // 診断結論高さ
    var diagnosingHeight = autoSizeHeight * 0.4;

    // 最小サイズ設定を超えていれば高さセット
    if (findingHeight >= 60 || diagnosingHeight >= 50) {
        // 画像所見 高さ
        $(".ReportRow-Finding").height(findingHeight);
        // 診断 高さ
        $(".ReportRow-Diagnosing").height(diagnosingHeight);
        $("#TxtPastDiagnosing").height(diagnosingHeight - 30);
    }

//     //履歴テーブルの最終列のwidth調整
//    var bodyWidth = $("#ReportTop-Info-Right").width();
//    if (bodyWidth > 474) {
//        // ウィンドウサイズが1200を超える場合は最終列のwidthを100%(ウィンドウ幅に合わせる)
//        $(".HisDiagnosing").css("width", "100%");
//    }
//    else {
//        // ウィンドウサイズが1200未満の場合は最終列のwidthを固定値
//        $(".HisDiagnosing").css("width", "100px");
//    }

    var bodyWidth = $("#ReportTop-Info-Right").width();

    if (bodyWidth < 362) {
        $(".HisDiagnosing").css("width", "80px");
    } else {
        $(".HisDiagnosing").css("width", "100%");
    }

        if (bodyWidth > 396) {
            $(".HisDiagnosing").css("width", "100%");
            $(".HisStudyBodyPart").css("width", "40%");
            $(".HisModality").css("width", "20%");
        }
        else {

            $(".HisStudyBodyPart").css("width", "60px");
            $(".HisModality").css("width", "40px");
        }

        

}


/***************************************************
/* 一覧へ戻るボタンイベント */
function Return() {
    // 確認メッセージ表示して一覧へ戻る
    Return_ToList(true);
}
function Return_ToList(isShowMsg) {
    // 一覧に戻る前に確認メッセージ表示
    if (isShowMsg) {
        //　一時保存有無の判定によりメッセージ
        var message = "";
        if ($IsTempSave != null || $IsTempSave != undefined) 
        {
            if ($IsTempSave == "0") {
                // 一時保存有無OFF
                message = "入力された内容は保存されません。\n一覧画面に戻ってよろしいですか？";
            }
            else {
                // 一時保存有無ON
                message = "一時保存を行った後に入力された内容は保存されません。\n一覧画面に戻ってよろしいですか？";
            }
        }
        else {
            message = "入力された内容は保存されません。\n一覧画面に戻ってよろしいですか？";
        }

        if (!window.confirm(message)) {
            return;
        }
    }
    window.open("", "_self");
    window.close();
}
/* 一覧へ戻るボタンマウスダウンイベント */
function ReturnListMouseDonw() {
    if ($ReportButtonReturnList != null && $ReportButtonReturnList != undefined) {
        $ReportButtonReturnList.addClass("btnReturnList-on");
        $ReportButtonReturnList.removeClass("btnReturnList-off");
        $ReportButtonReturnList.removeClass("btnReturnList-over");
    }
}
/* 一覧へ戻るボタンマウスアップイベント */
function ReturnListMouseUp() {
    if ($ReportButtonReturnList != null && $ReportButtonReturnList != undefined) {
        $ReportButtonReturnList.addClass("btnReturnList-off");
        $ReportButtonReturnList.removeClass("btnReturnList-on");
        $ReportButtonReturnList.removeClass("btnReturnList-over");
    }
}
/* 一覧へ戻るボタンマウスオーバーイベント */
function ReturnListMouseOver() {
    if ($ReportButtonReturnList != null && $ReportButtonReturnList != undefined) {
        $ReportButtonReturnList.addClass("btnReturnList-over");
        $ReportButtonReturnList.removeClass("btnReturnList-off");
        $ReportButtonReturnList.removeClass("btnReturnList-on");
    }
}

/***************************************************
/* VIEWERボタンイベント */
function Viewer() {

    // 連想配列に起動引数追加
    var prm = {};
    prm.serialno = $ReportConfig.data("serialno");
    prm.orderno = $ReportConfig.data("orderno");
    prm.patientid = $ReportConfig.data("patientid");
    prm.studydate = $ReportConfig.data("studydate");
    prm.modality = $ReportConfig.data("modality");
    prm.officecd = $ReportConfig.data("officecd");

    // common.js(WebViewer_Startメソッド呼び出し)
    WebViewer_Start2(prm);
}
/* Viewerボタンマウスダウンイベント */
function ViewerMouseDonw() {
    if ($ReportButtonViewer != null && $ReportButtonViewer != undefined) {
        $ReportButtonViewer.addClass("btnViewer-on");
        $ReportButtonViewer.removeClass("btnViewer-off");
        $ReportButtonViewer.removeClass("btnViewer-over");
    }
}
/* Viewerボタンマウスアップイベント */
function ViewerMouseUp() {
    if ($ReportButtonViewer != null && $ReportButtonViewer != undefined) {
        $ReportButtonViewer.addClass("btnViewer-off");
        $ReportButtonViewer.removeClass("btnViewer-on");
        $ReportButtonViewer.removeClass("btnViewer-over");
    }
}
/* Viewerボタンマウスオーバーイベント */
function ViewerMouseOver() {
    if ($ReportButtonViewer != null && $ReportButtonViewer != undefined) {
        $ReportButtonViewer.addClass("btnViewer-over");
        $ReportButtonViewer.removeClass("btnViewer-on");
        $ReportButtonViewer.removeClass("btnViewer-off");
    }
}

/***************************************************
/* 画像取込ボタンイベント */
function Image() {
    // 一覧で選択したレポートのSerialNo
    if ($("#ReportConfig").data("serialno") == null || $("#ReportConfig").data("serialno") == undefined) {
        alert("画像取得のパラメータSerialNoが不明です。");
        return;
    }
    var serialNo = $ReportConfig.data("serialno");
    var imageNum = $("#ReportImages").children().length;

    // 画像取り込み開始
    Image_Import(serialNo, imageNum);
}
function C_Image() {
    var serialNo = $ReportConfig.data("serialno");
    var imageNum = $("#ReportImages").children().length;

    // 画像取り込み開始
    C_Image_Import(serialNo, imageNum);
}
function Image_Import(prm, imageNum) {
    // HTTP通信開始
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/GetImageList",
        data: "{serialNo:\"" + prm + "\",imageNum:\"" + imageNum + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: Image_Import_Result,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
}
function C_Image_Import(prm, imageNum)
{
    // HTTP通信開始
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/GetImageList",
        data: "{serialNo:\"" + prm + "\",imageNum:\"" + imageNum + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: C_Image_Import_Result,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
}

function Image_Import_Result(result) {
    // エラー判定
    if (result.d == null || result.d == undefined || result.d.Result == "Error") {
        alert("サービスでエラーが発生しました。");
        return;
    }

    // ファイル名リスト
    var list = new Array();
    for (var i = 0; i < result.d.List.length; i++) {
        list.push(result.d.List[i]);
    }

    // 画像追加
    AddImage(list, $("#ReportImages"), "webImage.aspx", true, true, true, true);

    //イメージの○枚目か表示の再設定
    var size = $('#ReportImages>.KeyImage-box').length;
    for (var i = 0; i <= size ; i++) {
        var obj = $(".KeyImage-box:nth-child(" + i + ")");
        obj.children(".KeyImage-Count").text(i + "/" + size);
    }

    // 画面内リサイズ Chromでリサイズイベントがうまく動作しないため追加
    resize();

    // 所見にフォーカス
    $("#TxtFinding").focus();
    
}

function C_Image_Import_Result(result) {
    // エラー判定
    if (result.d == null || result.d == undefined || result.d.Result == "Error") {
        return;
    }

    // ファイル名リスト
    var list = new Array();
    for (var i = 0; i < result.d.List.length; i++) {
        list.push(result.d.List[i]);
    }

    // 画像追加
    AddImage(list, $("#ReportImages"), "webImage.aspx", true, true, true, true);

    //イメージの○枚目か表示の再設定
    var size = $('#ReportImages>.KeyImage-box').length;
    for (var i = 0; i <= size ; i++) {
        var obj = $(".KeyImage-box:nth-child(" + i + ")");
        obj.children(".KeyImage-Count").text(i + "/" + size);
    }

    // 画面内リサイズ Chromでリサイズイベントがうまく動作しないため追加
    resize();

    $(this).delay(1000).queue(function () {
        if (!isCheck) {
            $(this).dequeue();
            return;
        }
        C_Image();
        $(this).dequeue();
    });
}

/***************************************************
/* 引数オブジェクトに画像(div)追加 */
function AddImage(list, obj, callImagePage, isImportImage, isClickEvent, isDblClickEvent, isKeyUpEvent) {

    for (var i = 0; i < list.length; i++) {

        var rowSet = $("<div>");
        rowSet.addClass("KeyImage-box");
        rowSet.data("file", list[i]);
        rowSet.data("callImagePage", callImagePage);
        rowSet.data("isImportImage", isImportImage);

        var rowlabel = $("<div>");
        rowlabel.addClass("KeyImage-Count");
        rowlabel.text((i + 1) + "/" + list.length);

        var row = $("<div>");
        row.addClass("KeyImage-Item");
        //row.data("file", list[i]);
        //row.data("callImagePage", callImagePage);
        //row.data("isImportImage", isImportImage);
        row.css("background-image", "url(./" + callImagePage + "?key=" + list[i] + ")");
        row.css("background-color", "white");
        row.css("border", "ridge 3px black");

        // ドラッグアンドドロップ対応IE10以上)draggable
        row.attr("draggable", true);

        // 画像呼び出しページを保存

        // クリックイベント
        if (isClickEvent) {
            //row.click(ImageClick);
            rowSet.click(ImageClick);
        }
        // ダブルクリックイベント
        if (isDblClickEvent) {
            //row.dblclick(ImageDblClick);
            rowSet.dblclick(ImageDblClick);
        }
        // キーアップイベント
        if (isKeyUpEvent) {
            rowSet.keyup(KeyUpEvent);
        }
        rowSet.append(rowlabel);
        rowSet.append(row);

        obj.append(rowSet);
    }
}
/* キー画像取込ボタンマウスダウンイベント */
function ImageMouseDonw() {
    if ($ReportButtonImage != null && $ReportButtonImage != undefined) {
        $ReportButtonImage.addClass("btnImage-on");
        $ReportButtonImage.removeClass("btnImage-off");
        $ReportButtonImage.removeClass("btnImage-over");
    }
}
/* キー画像取込ボタンマウスアップイベント */
function ImageMouseUp() {
    if ($ReportButtonImage != null && $ReportButtonImage != undefined) {
        $ReportButtonImage.addClass("btnImage-off");
        $ReportButtonImage.removeClass("btnImage-on");
        $ReportButtonImage.removeClass("btnImage-over");
    }
}
/* キー画像取込ボタンマウスオーバーイベント */
function ImageMouseOver() {
    if ($ReportButtonImage != null && $ReportButtonImage != undefined) {
        $ReportButtonImage.addClass("btnImage-over");
        $ReportButtonImage.removeClass("btnImage-off");
        $ReportButtonImage.removeClass("btnImage-on");
    }
}

/***************************************************
/* 画像削除ボタンイベント */
function Delete() {
    // 確認メッセージ有で画像削除
    Image_Delete(true);
}
/***************************************************
/* 画像削除 */
function Image_Delete(isConfirmMessage) {
    // 現在選択中オブジェクト
    if ($SelectImage != null && $SelectImage != undefined) {

        // 選択中の画像オブジェクトが取込画像でなければ検査暦画像なので削除処理なし
        var isImportImage = $SelectImage.data("isImportImage");
        if (!isImportImage) {
            return;
        }

        // 確認メッセージ有無
        if (isConfirmMessage) {
            if (!window.confirm("選択した画像を削除します。")) {
                return;
            }
        }

        // サーバーの画像ファイル削除
        // HTTP通信開始
        $.ajax({
            async: false,
            cache: false,
            type: "POST",
            url: "./CommonWebService.asmx/DeleteImage",
            data: "{image:\"" + $SelectImage.data("file") + "\"}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: Image_Delete_Result,
            error: function (result) {
                // エラー
                alert("通信エラーが発生しました。");
            }
        });
    }
}
function Image_Delete_Result(result) {
    // エラー判定
    if (result.d == null || result.d == undefined || result.d.Result == "Error") {
        alert("サービスでエラーが発生しました。");
        return;
    }
    // 画面から削除
    $SelectImage.remove();

    //イメージの○枚目か表示の再設定
    var size = $('#ReportImages>.KeyImage-box').length;
    for (var i = 0; i <= size ; i++) {
        var obj = $(".KeyImage-box:nth-child(" + i + ")");
        obj.children(".KeyImage-Count").text(i + "/" + size);
    }

    $SelectImage = undefined;
}
/* キー画像削除ボタンマウスダウンイベント */
function ImageDeleteMouseDonw() {
    if ($ReportButtonImageDelete != null && $ReportButtonImageDelete != undefined) {
        $ReportButtonImageDelete.addClass("btnImageDelete-on");
        $ReportButtonImageDelete.removeClass("btnImageDelete-off");
        $ReportButtonImageDelete.removeClass("btnImageDelete-over");
    }
}
/* キー画像削除ボタンマウスアップイベント */
function ImageDeleteMouseUp() {
    if ($ReportButtonImageDelete != null && $ReportButtonImageDelete != undefined) {
        $ReportButtonImageDelete.addClass("btnImageDelete-off");
        $ReportButtonImageDelete.removeClass("btnImageDelete-on");
        $ReportButtonImageDelete.removeClass("btnImageDelete-over");
    }
}
/* キー画像削除ボタンマウスオーバーイベント */
function ImageDeleteMouseOver() {
    if ($ReportButtonImageDelete != null && $ReportButtonImageDelete != undefined) {
        $ReportButtonImageDelete.addClass("btnImageDelete-over");
        $ReportButtonImageDelete.removeClass("btnImageDelete-off");
        $ReportButtonImageDelete.removeClass("btnImageDelete-on");
    }
}

/***************************************************
/* 確定ボタンイベント */
function Save() {
    // 所見入力チェック
    if($("#TxtFinding").val().length == 0){
        alert("画像所見が未入力です。\r\n入力をお願い致します。");
        // 所見にフォーカス
        $("#TxtFinding").focus();
        return;
    }
    // 診断入力チェック
    if ($("#TxtDiagnosing").val().length == 0) {
        alert("診断が未入力です。\r\n入力をお願い致します。");
        // 診断にフォーカス
        $("#TxtDiagnosing").focus();
        return;
    }
    //20150703 A.umeda 確定条件を、レポート側に合わせてImgCheckModalitにて管理
    //// キー画像チェック
    //if ($("#ReportImages").children().length == 0) {

    //    alert("キー画像が未登録です。\r\n画像の貼付けをお願い致します。");

    //    // 所見にフォーカス
    //    $("#TxtFinding").focus();

    //    return;
    //}

    // ImgCheckModality　キー画像登録が必要なモダリティで判断
    var ImgCheckModality = $("#ReportConfig").data("ImgCheckModality");
    var CurrentReportModality = $ReportConfig.data("CurrentReportModality");
    if ($("#ReportConfig").data("ImgCheckModality").length > 0 &&
        ImgCheckModality.match(CurrentReportModality) &&
        $("#ReportImages").children().length == 0)
    {

        alert("キー画像が未登録です。\r\n画像の貼付けをお願い致します。");

        // 所見にフォーカス
        $("#TxtFinding").focus();

        return;
    }

    // 確認ダイアログ
    if (!window.confirm("確定します。よろしいですか？")) {
        return;
    }
    ReportData_Save();
}
function ReportData_Save() {
    // 確定XML出力用パラメータ
    var key = Array();
    var val = Array();

    key.push("'serialno'");
    val.push("'" + $ReportConfig.data("serialno") + "'");
    key.push("'orderno'");
    val.push("'" + $ReportConfig.data("orderno") + "'");
    key.push("'officecd'");
    val.push("'" + $ReportConfig.data("officecd") + "'");
    key.push("'finding'");
    // JSON通信禁則文字エスケープ
    val.push("'" + EscapeString($("#TxtFinding").val()) + "'");
    key.push("'diagnosing'");
    // JSON通信禁則文字エスケープ
    val.push("'" + EscapeString($("#TxtDiagnosing").val()) + "'");

    // 画像
    var image = Array();
    var $ReportImage = $("#ReportImages");
    if ($ReportImage.children().length > 0) {
        for (var i = 0; i < $ReportImage.children().length; i++) {
            image.push("'" + $($ReportImage.children()[i]).data("file") + "'");
        }
    }

    // HTTP通信開始
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/SaveReportData",
        data: "{key:[" + key + "],val:[" + val + "],image:[" + image + "]}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: ReportData_Save_Result,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
}
function ReportData_Save_Result(result) {
    // エラー判定
    if (result.d == null || result.d == undefined || result.d.Result == "Error") {
        alert("サービスでエラーが発生しました。");
        return;
    }
    // 確認メッセージ表示しない
    Return_ToList(false);
}

/* 確定ボタンマウスダウンイベント */
function SaveMouseDonw() {
    if ($ReportButtonSave != null && $ReportButtonSave != undefined) {
        $ReportButtonSave.addClass("btnSave-on");
        $ReportButtonSave.removeClass("btnSave-off");
        $ReportButtonSave.removeClass("btnSave-over");
    }
}
/* 確定ボタンマウスアップイベント */
function SaveMouseUp() {
    if ($ReportButtonSave != null && $ReportButtonSave != undefined) {
        $ReportButtonSave.addClass("btnSave-off");
        $ReportButtonSave.removeClass("btnSave-on");
        $ReportButtonSave.removeClass("btnSave-over");
    }
}
/* 確定ボタンマウスオーバーイベント */
function SaveMouseOver() {
    if ($ReportButtonSave != null && $ReportButtonSave != undefined) {
        $ReportButtonSave.addClass("btnSave-over");
        $ReportButtonSave.removeClass("btnSave-off");
        $ReportButtonSave.removeClass("btnSave-on");
    }
}

/***************************************************
/* 一時保存ボタンイベント */
function TempSave() {

    // 確認ダイアログ
    if (!window.confirm("一時保存します。よろしいですか？")) {
        return;
    }
    ReportData_TempSave();
}
function ReportData_TempSave() {
    // 一時保存XML出力用パラメータ
    var key = Array();
    var val = Array();

    key.push("'serialno'");
    val.push("'" + $ReportConfig.data("serialno") + "'");
    key.push("'orderno'");
    val.push("'" + $ReportConfig.data("orderno") + "'");
    key.push("'officecd'");
    val.push("'" + $ReportConfig.data("officecd") + "'");
    key.push("'finding'");
    // JSON通信禁則文字エスケープ
    val.push("'" + EscapeString($("#TxtFinding").val()) + "'");
    key.push("'diagnosing'");
    // JSON通信禁則文字エスケープ
    val.push("'" + EscapeString($("#TxtDiagnosing").val()) + "'");

    // 画像
    var image = Array();
    var $ReportImage = $("#ReportImages");
    if ($ReportImage.children().length > 0) {
        for (var i = 0; i < $ReportImage.children().length; i++) {
            image.push("'" + $($ReportImage.children()[i]).data("file") + "'");
        }
    }

    // HTTP通信開始
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/TempSaveReportData",
        data: "{key:[" + key + "],val:[" + val + "],image:[" + image + "]}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: ReportData_TempSave_Result,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
}
function ReportData_TempSave_Result(result) {
    // エラー判定
    if (result.d == null || result.d == undefined || result.d.Result == "Error") {
        alert("サービスでエラーが発生しました。");
        return;
    }
    // 一時保存の有無ON
    $IsTempSave = "1";
}

/* 一時保存ボタンマウスダウンイベント */
function TempSaveMouseDonw() {
    if ($ReportButtonTempSave != null && $ReportButtonTempSave != undefined) {
        $ReportButtonTempSave.addClass("btnTempSave-on");
        $ReportButtonTempSave.removeClass("btnTempSave-off");
        $ReportButtonTempSave.removeClass("btnTempSave-over");
    }
}
/* 一時保存ボタンマウスアップイベント */
function TempSaveMouseUp() {
    if ($ReportButtonTempSave != null && $ReportButtonTempSave != undefined) {
        $ReportButtonTempSave.addClass("btnTempSave-off");
        $ReportButtonTempSave.removeClass("btnTempSave-on");
        $ReportButtonTempSave.removeClass("btnTempSave-over");
    }
}
/* 一時保存ボタンマウスオーバーイベント */
function TempSaveMouseOver() {
    if ($ReportButtonTempSave != null && $ReportButtonTempSave != undefined) {
        $ReportButtonTempSave.addClass("btnTempSave-over");
        $ReportButtonTempSave.removeClass("btnTempSave-off");
        $ReportButtonTempSave.removeClass("btnTempSave-on");
    }
}

/***************************************************
/* 依頼票ボタンイベント */
function ExamOrder() {
    // 依頼票ページ表示
    // 2013/09/20 依頼票ページ呼び出しURL変更
    // 同一URLだとキャッシュ画像がブラウザに表示されてしまうためURLに現在日時を付与
    var date = new Date();
    var year = date.getFullYear();
    var month = ("0" + (date.getMonth() + 1)).slice(-2);
    var day = ("0" + (date.getDate())).slice(-2);
    var hour = ("0" + (date.getHours())).slice(-2);
    var minutes = ("0" + (date.getMinutes())).slice(-2);
    var second = ("0" + (date.getSeconds())).slice(-2);

    window.open("./webExamOrder.aspx?" + year + month + day + hour + minutes + second);
}
/* 依頼票ボタンマウスダウンイベント */
function ExamOrderMouseDonw() {
    if ($ReportButtonExamOrder != null && $ReportButtonExamOrder != undefined) {
        $ReportButtonExamOrder.addClass("btnExamOrder-on");
        $ReportButtonExamOrder.removeClass("btnExamOrder-off");
        $ReportButtonExamOrder.removeClass("btnExamOrder-over");
    }
}
/* 依頼票ボタンマウスアップイベント */
function ExamOrderMouseUp() {
    if ($ReportButtonExamOrder != null && $ReportButtonExamOrder != undefined) {
        $ReportButtonExamOrder.addClass("btnExamOrder-off");
        $ReportButtonExamOrder.removeClass("btnExamOrder-on");
        $ReportButtonExamOrder.removeClass("btnExamOrder-over");
    }
}
/* 依頼票ボタンマウスオーバーイベント */
function ExamOrderMouseOver() {
    if ($ReportButtonExamOrder != null && $ReportButtonExamOrder != undefined) {
        $ReportButtonExamOrder.addClass("btnExamOrder-over");
        $ReportButtonExamOrder.removeClass("btnExamOrder-off");
        $ReportButtonExamOrder.removeClass("btnExamOrder-on");
    }
}


/***************************************************
/* 画像クリックイベント */
function ImageClick() {
    // イベント発生元オブジェクト
    var sender = $(this);

    // 選択中オブジェクト判定
    if ($SelectImage != null && $SelectImage != undefined) {
        // 選択中オブジェクトのdataとイベントオブジェクトのdataを比較
        if ($SelectImage.data("file") == sender.data("file")) {
            // 同じ場合は処理なし
            return;
        }
        else {
            // 違う場合はイベントオブジェクトを選択中に変更
            ImageSelect(sender);
        }
    }
    else {
        // 選択中オブジェクトに今回のオブジェクト格納
        ImageSelect(sender);
    }
}
function ImageSelect(selectObj) {
    if ($SelectImage != null && $SelectImage != undefined) {
        // 現在の選択中オブジェクトの表示を変更
        //$SelectImage.css("border", "ridge 3px black");
        $SelectImage.children(".KeyImage-Item").css("border", "ridge 3px black");
    }
    // 新しい選択オブジェクトの表示を変更
    //selectObj.css("border", "ridge 3px deepskyblue");
    selectObj.children(".KeyImage-Item").css("border", "ridge 3px deepskyblue");

    // 選択中オブジェクト変数に新しい選択オブジェクトを格納
    $SelectImage = selectObj;
}

/***************************************************
/* 画像ダブルクリックイベント */
function ImageDblClick() {
    // イベント発生元オブジェクト
    var sender = $(this);

    if (sender.data("callImagePage") != null && sender.data("callImagePage") != undefined) {
        // ページ表示
        window.open("./" + sender.data("callImagePage") + "?key=" + sender.data("file"));
    }
}

/* 画像キーアップイベント */
function KeyUpEvent(e) {
    console.log(e.keyCode);
    // 削除キー（Del）
    if (e.keyCode == 46) {

        if (editFlg == 1) {
            return false;
        }

        // 確認メッセージありで画像削除
        Image_Delete(true);
    }
}

/***************************************************
/* 検査暦参照ボタンイベント */
function HistoryReport_View(serialno) {
    // 検査暦で選択したレポートのSerialNo
    if (serialno == null || serialno == undefined) {
        alert("表示するレポートが不明です。");
        return;
    }

    if(colorFlg !=""){
        $("#" + colorFlg).removeClass("back-color");
    }

    // 保存
    $historycd = serialno;

    // レポート取得＋取得後画面セット
    HistoryReportData_Get(serialno, HistoryReportData_Get_Result);

    //A.Umeda 20150618b 検査暦参照時に選択リストのハイライト対応
    $("#" + serialno).addClass("back-color");
    colorFlg　= serialno;
    resize();
}
/***************************************************
/* 履歴レポート取得 */
function HistoryReportData_Get(serialno, func) {
    // HTTP通信開始
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/GetHistoryReportData",
        data: "{serialNo:\"" + serialno + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: func,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
}
/***************************************************
/* 検査暦レポート表示 */
function HistoryReportData_Get_Result(result) {
    // エラー判定
    if (result.d.Result == "Exception") {
        alert(result.d.Message);
        return;
    }
    if (result.d.Result == "Error") {
        alert(result.d.Message);
        return;
    }

    // 表示内容クリア
    $("#TxtPastReportComment1").val("");
    $("#TxtPastReportComment2").val("");
    $("#TxtPastFinding").val("");
    $("#TxtPastDiagnosing").val("");
    // 表示画像クリア
    var $PastReportImage = $("#PastReportImages");
    if ($PastReportImage.children().length > 0) {
        $PastReportImage.children().remove();
    }

    // 依頼内容
//    $("#TxtPastReportComment1").val(result.d.View.Comment1);
    // 連絡事項
//    $("#TxtPastReportComment2").val(result.d.View.Comment2);

    //20150306 A.Umeda 連絡事項欄を依頼内容・読影医師カラムに変更 --------------START
    // 読影医師
    //$("#TxtPastReportComment2").val(result.d.View.ReadPhysicianName);
    $("#TxtPastReportComment2").val(result.d.View.Comment1);
    //$("#RadiologistName").val(result.d.View.AuthorizationPhysicianName);
    $("#RadiologistName").attr("value", result.d.View.AuthorizationPhysicianName);;
    $("#RadiologistName").attr("title", result.d.View.AuthorizationPhysicianName);
    //20150306 A.Umeda 読影医師カラム追加のため変更 --------------END

    // 所見
    $("#TxtPastFinding").val(result.d.View.Finding);
    // 診断
    $("#TxtPastDiagnosing").val(result.d.View.Diagnosing);

    // 画像追加
    // ファイル名リスト
    var list = new Array();
    for (var i = 0; i < result.d.Image.length; i++) {
        list.push(result.d.Image[i]);
    }
    AddImage(list, $PastReportImage, "webImageHistory.aspx", false, true, true, false);

}
/* 検査歴　参照ボタンマウスダウンイベント */
function HistoryReportMouseDonw(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("inputReportButton-on");
        sender.removeClass("inputReportButton-off");
        sender.removeClass("inputReportButton-over");
    }
}
/* 検査歴　参照ボタンマウスアップイベント */
function HistoryReportMouseUp(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("inputReportButton-off");
        sender.removeClass("inputReportButton-on");
        sender.removeClass("inputReportButton-over");
    }
}
/* 検査歴　参照ボタンマウスオーバーイベント */
function HistoryReportMouseOver(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("inputReportButton-over");
        sender.removeClass("inputReportButton-on");
        sender.removeClass("inputReportButton-off");
    }
}


/***************************************************
/* 検査暦　画像取得ボタンイベント */
function ViewerImage_Request(id) {
    // 確認ダイアログ
    if (!window.confirm("画像取得要求を行います。よろしいですか？")) {
        return;
    }

    // ボタンクリックの行からレポートデータ取得
    var history = $("#" + id).data("historyview");

    // 連想配列に起動引数追加
    var prm = {};
    prm.orderno = history._HisOrderNo;
    prm.patientid = history._HisPatientID;
    prm.studydate = history._HisStudyDate;
    prm.modality = history.HisModality;

    // common.js(ViewerImageRequest_Startメソッド呼び出し)
    ViewerImageRequest_Start(prm);
}
/* 検査歴　画像データ取得ボタンマウスダウンイベント */
function HistoryImageRequestMouseDonw(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("inputImageRequestButton-on");
        sender.removeClass("inputImageRequestButton-off");
        sender.removeClass("inputImageRequestButton-over");
    }
}
/* 検査歴　画像データ取得ボタンマウスアップイベント */
function HistoryImageRequestMouseUp(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("inputImageRequestButton-off");
        sender.removeClass("inputImageRequestButton-on");
        sender.removeClass("inputImageRequestButton-over");
    }
}
/* 検査歴　画像データ取得ボタンマウスオーバーイベント */
function HistoryImageRequestMouseOver(btnId) {
    var sender = $(btnId);
    if (sender != null && sender != undefined) {
        sender.addClass("inputImageRequestButton-over");
        sender.removeClass("inputImageRequestButton-on");
        sender.removeClass("inputImageRequestButton-off");
    }
}

/***************************************************
/* 検査暦　VIEWERボタンイベント */
function HistoryViewer_Open(id){
    // ボタンクリックの行からレポートデータ取得
    var history = $("#" + id).data("historyview");

    // 連想配列に起動引数追加
    var prm = {};
    prm.serialno = history._HisSerialNo;
    prm.orderno = history._HisOrderNo;
    prm.patientid = history._HisPatientID;
    prm.studydate = history._HisStudyDate;
    prm.modality = history.HisModality;
    prm.officecd = history._HisOfficeCd;

    // common.js(WebViewer_Startメソッド呼び出し)
    WebViewer_Start(prm);
}



/***************************************************
/* webReport画面　キーボードキーアップイベント */
$(window).keyup(function (e) {
    // 現在のフォーカスオブジェクトのID取得
    var id = $(":focus").attr("id");

    // idが取得できた場合は
    // "Txt"で始まり(テキストオブジェクト) かつ readonly属性がfalseの場合 入力エリアなので処理なし
    /* メモ：readonly属性はDOMオブジェクト(html)の要素なので、オブジェクトに対して.get(0)でアクセス */
    if (id != null && id != undefined) {
        if (id.indexOf("Txt") == 0 && !$("#" + id).get(0).readOnly) {
            return;
        }
    }

    //// キー判定
    //switch (e.which) {
    //    // Deleteキー            
    //    case 46:
    //        if (editFlg == 1) {
    //            return false;
    //        }
    //        // 確認メッセージありで画像削除
    //        Image_Delete(true);
    //        break;

    //}
});

/***************************************************
/* 読影取り消しボタンイベント */
function ReadCancel() {

    //ダイアログ、デフォルト「いいえ」選択のため変更
    //// 確認メッセージ表示
    //ReadCancel_ReadStart_ToList(true);

    message = "読影を中止しますか？\r\n（入力中・一時保存のデータは取り消されます。）";
      daialog(message, ReadCancel_ReadStart_ToList);
}

function ReadCancel_ReadStart_ToList() {

    //20150511 A.Umeda 確認メッセージ表示のため。daialog使用時は不要
    //message = "読影を中止しますか？\r\n（入力中・一時保存のデータは取り消されます。）";
    //if (!window.confirm(message)) {
    //    return false;
    //}

    //以下読影キャンセル処理

    //var serialNo = $ReportConfig.data("serialno");
    ////一時保存データの削除
    //// HTTP通信
    //$.ajax({
    //    async: false,
    //    cache: false,
    //    type: "POST",
    //    url: "./CommonWebService.asmx/DeleteTemp",
    //    data: "{serialNo:" + serialNo + "}",
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "json",
    //    error: function (result) {
    //        // エラー
    //        alert("通信エラーが発生しました。");
    //    }
    //});

    var key = Array();
    var val = Array();
    key.push("'non-edit'");
    val.push("'1'");//読影モードから閲覧モードへ

    // HTTP通信
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/SetParams",
        data: "{key:[" + key + "],val:[" + val + "]}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });

    var serialNo = $ReportConfig.data("serialno");

    //読影取り消しReadingStatusを1から0へ更新 
    // HTTP通信開始
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/CancelReading",
        data: "{serialNo:\"" + serialNo + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            clipboardImageName = "";
            Report_View();
        },
        error: function (result) {
            // エラー
            alert("通信エラーが発生し、読影ステータスの変更に失敗しました。\nシステム管理者に連絡してください。");
        }
    });

}
/* 読影取り消しボタンマウスダウンイベント */
function ReadCancelMouseDonw() {
    if ($ReportButtonReadCancel != null && $ReportButtonReadCancel != undefined) {
        $ReportButtonReadCancel.addClass("btnReadCancel-on");
        $ReportButtonReadCancel.removeClass("btnReadCancel-off");
        $ReportButtonReadCancel.removeClass("btnReadCancel-over");
    }
}
/* 読影取り消しボタンマウスアップイベント */
function ReadCancelMouseUp() {
    if ($ReportButtonReadCancel != null && $ReportButtonReadCancel != undefined) {
        $ReportButtonReadCancel.addClass("btnReadCancel-off");
        $ReportButtonReadCancel.removeClass("btnReadCancel-on");
        $ReportButtonReadCancel.removeClass("btnReadCancel-over");
    }
}
/* 読影取り消しボタンマウスオーバーイベント */
function ReadCancelMouseOver() {
    if ($ReportButtonReadCancel != null && $ReportButtonReadCancel != undefined) {
        $ReportButtonReadCancel.addClass("btnReadCancel-over");
        $ReportButtonReadCancel.removeClass("btnReadCancel-off");
        $ReportButtonReadCancel.removeClass("btnReadCancel-on");
    }
}

/***************************************************
/* 読影開始ボタンイベント */
function ReadStart() {
    // 確認メッセージ表示
    ReadStart_ToList(true);
}
function ReadStart_ToList(isShowMsg) {

    var serialNo = $ReportConfig.data("serialno");
    // 一時保存データ読み出し判定
    var readTempSave = $ReportConfig.data("readtempsave");

    readCheck(serialNo, true, false); 

}

function Read_start_action() {

    var key = Array();
    var val = Array();
    key.push("'non-edit'");
    val.push("'0'");//edit_flg = true は読影モード

    // HTTP通信
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/SetParams",
        data: "{key:[" + key + "],val:[" + val + "]}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });

    editmode();
    //KeyImagereload();
}
/* 読影開始ボタンマウスダウンイベント */
function ReadStartMouseDonw() {
    if ($ReportButtonReadStart != null && $ReportButtonReadStart != undefined) {
        $ReportButtonReadStart.addClass("btnReadStart-on");
        $ReportButtonReadStart.removeClass("btnReadStart-off");
        $ReportButtonReadStart.removeClass("btnReadStart-over");
    }
}
/* 読影開始ボタンマウスアップイベント */
function ReadStartMouseUp() {
    if ($ReportButtonReadStart != null && $ReportButtonReadStart != undefined) {
        $ReportButtonReadStart.addClass("btnReadStart-off");
        $ReportButtonReadStart.removeClass("btnReadStart-on");
        $ReportButtonReadStart.removeClass("btnReadStart-over");
    }
}
/* 読影開始ボタンマウスオーバーイベント */
function ReadStartMouseOver() {
    if ($ReportButtonReadStart != null && $ReportButtonReadStart != undefined) {
        $ReportButtonReadStart.addClass("btnReadStart-over");
        $ReportButtonReadStart.removeClass("btnReadStart-off");
        $ReportButtonReadStart.removeClass("btnReadStart-on");
    }
}

/***************************************************/
//閲覧モード（読影開始ボタン表示、キー画像取込、キー画像削除、一時保存、確定は非アクティブ）
function noneditmode() {
    // 使用不可
    $ReportButtonImage.attr("disabled", true);
    $ReportButtonImageDelete.attr("disabled", true);
    $ReportButtonSave.attr("disabled", true);
    $ReportButtonTempSave.attr("disabled", true);

    $ReportButtonSentence.attr("disabled", true);

    // カーソルをauto(デフォルト)
    $ReportButtonImage.css("cursor", "auto");
    $ReportButtonImageDelete.css("cursor", "auto");
    $ReportButtonSave.css("cursor", "auto");
    $ReportButtonTempSave.css("cursor", "auto");
    $ReportButtonSentence.css("cursor", "auto");

    // 無効表示
    $ReportButtonImage.addClass("btnImage-disable");
    $ReportButtonImage.removeClass("btnImage-off");
    $ReportButtonImageDelete.addClass("btnImageDelete-disable");
    $ReportButtonImageDelete.removeClass("btnImageDelete-off");
    $ReportButtonSave.addClass("btnSave-disable");
    $ReportButtonSave.removeClass("btnSave-off");
    $ReportButtonTempSave.addClass("btnTempSave-disable");
    $ReportButtonTempSave.removeClass("btnTempSave-off");

    $ReportButtonSentence.addClass("btnSentence-disable");
    $ReportButtonSentence.removeClass("btnSentence-off");

    //$("#KeyImage-Item").removeAttr("draggable");

    //読影取消ボタン非表示
    $ReportButtonReadCancel.css("display", "none");

    //読影開始ボタン
    $ReportButtonReadStart.css("display", "");
    $ReportButtonReadStart.addClass("btnReadStart-off");
    $ReportButtonReadStart.removeClass("btnReadStart-on");
    $ReportButtonReadStart.removeClass("btnReadStart-over");

    // 所見
    $("#TxtFinding").attr("readonly", "readonly").css("background-color", "#EEEEEE");
    // 診断
    $("#TxtDiagnosing").attr("readonly", "readonly").css("background-color", "#EEEEEE");
    //キー画像エリア
    $("#ReportImages").attr("readonly", "readonly").css("background-color", "#EEEEEE");

    editFlg = 1;
}
/***************************************************/
//読影モード（読影取消ボタン表示）

function editmode() {
    // 使用可
    $ReportButtonImage.attr("disabled", false);
    $ReportButtonImage.removeAttr("disabled");
    $ReportButtonImageDelete.attr("disabled", false);
    $ReportButtonImageDelete.removeAttr("disabled");
    $ReportButtonSave.attr("disabled", false);
    $ReportButtonSave.removeAttr("disabled");
    $ReportButtonTempSave.attr("disabled", false);
    $ReportButtonTempSave.removeAttr("disabled");
    $ReportButtonSentence.attr("disabled", false);
    $ReportButtonSentence.removeAttr("disabled");

    $ReportButtonImage.addClass("btnImage-off");
    $ReportButtonImage.removeClass("btnImage-disable");
    $ReportButtonImageDelete.addClass("btnImageDelete-off");
    $ReportButtonImageDelete.removeClass("btnImageDelete-disable");
    $ReportButtonSave.addClass("btnSave-off");
    $ReportButtonSave.removeClass("btnSave-disable");
    $ReportButtonTempSave.addClass("btnTempSave-off");
    $ReportButtonTempSave.removeClass("btnTempSave-disable");

    $ReportButtonSentence.addClass("btnSentence-off");
    $ReportButtonSentence.removeClass("btnSentence-disable");

    //$("#KeyImage-Item").attr("draggable", true);

    // カーソルをpointer(指マーク)
    $ReportButtonImage.css("cursor", "pointer");
    $ReportButtonImageDelete.css("cursor", "pointer");
    $ReportButtonSave.css("cursor", "pointer");
    $ReportButtonTempSave.css("cursor", "pointer");
    $ReportButtonSentence.css("cursor", "pointer");

    //読影開始ボタン非表示
    $ReportButtonReadStart.css("display", "none");

    //読影取消ボタン
    $ReportButtonReadCancel.css("display", "");
    $ReportButtonReadCancel.addClass("btnReadCancel-off");
    $ReportButtonReadCancel.removeClass("btnReadCancel-on");
    $ReportButtonReadCancel.removeClass("btnReadCancel-over");

    // 所見
    $("#TxtFinding").removeAttr("readonly").css("background-color", "White");
    // 診断
    $("#TxtDiagnosing").removeAttr("readonly", "readonly").css("background-color", "White");
    //キー画像エリア
    $("#ReportImages").removeAttr("readonly").css("background-color", "White");

    editFlg = 0;
}
/***************************************************/
// キー画像再読み込み
function KeyImagereload() {
    var image = new Array();
    for (var i = 0; i < $ReportKeyImage.length; i++) {
        image.push($ReportKeyImage[i]);
    }

    if (clipboardImageName != undefined && clipboardImageName != "") {
        image.push(clipboardImageName);
    }
    // キー画像のリセット
    $("#ReportImages").empty();
    // キー画像追加
    AddImage(image, $("#ReportImages"), "webImage.aspx", true, true, true, true);

    // 画面内リサイズ Chromでリサイズイベントがうまく動作しないため追加
    resize();

    // 所見にフォーカス
    $("#TxtFinding").focus();
}


/***************************************************
/* 定型文ボタンイベント */
function Sentence() {
    // ダイアログ表示
    //$SentenceDlg.dialog({
    //    closeText: "",
    //    modal: true,
    //    draggable: true,
    //    resizable: true,
    //    width: 500,
    //    height: 600,
    //    minWidth: 300,
    //    minHeight: 200,
    //    position: { my: "right top", at: "right top", of: window }
    //});
    
}
/* 定型文ボタンマウスダウンイベント */
function SentenceMouseDonw() {
    if ($ReportButtonSentence != null && $ReportButtonSentence != undefined) {
        $ReportButtonSentence.addClass("btnSentence-on");
        $ReportButtonSentence.removeClass("btnSentence-off");
        $ReportButtonSentence.removeClass("btnSentence-over");
    }
}
/* 定型文ボタンマウスアップイベント */
function SentenceMouseUp() {
    if ($ReportButtonSentence != null && $ReportButtonSentence != undefined) {
        $ReportButtonSentence.addClass("btnSentence-off");
        $ReportButtonSentence.removeClass("btnSentence-on");
        $ReportButtonSentence.removeClass("btnSentence-over");
    }
}
/* 定型文ボタンマウスオーバーイベント */
function SentenceMouseOver() {
    if ($ReportButtonSentence != null && $ReportButtonSentence != undefined) {
        $ReportButtonSentence.addClass("btnSentence-over");
        $ReportButtonSentence.removeClass("btnSentence-off");
        $ReportButtonSentence.removeClass("btnSentence-on");
    }
}

/* 絞込み */
function SentenceSearch() {
    // 選択テキスト取得
    var UserName = $cmbUserList.children(":selected").text();
    var UserCD = $cmbUserList.children(":selected").val();

    if (UserCD == "") {
        UserCD = -1;
    }
    GetSentenceData(UserCD, -1);
}

function SentenceEdit() {
    $('#sent-edit').data("cd", "").data("name", '');
    $('#sent-title').val('');
    $('#sent-finding').text('');
    $('#sent-diagnosing').text('');
    $('input[name=sent-public]').val(["20"]);
    $('#sent-cp-find')[0].checked = true;
    $('#sent-cp-diag')[0].checked = true;

    $('#sent-delete').css('visibility', 'hidden');

    $('#modal-sent-edit').modal();
}

function GetSentenceData(usercd, groupcd) {
    // HTTP通信開始
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/GetSentenceData",
        data: "{usercd:\"" + usercd + "\",groupcd:\"" + groupcd + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: GetSentenceData_Result,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
}

function SetSentenceData(vals) {
    // HTTP通信開始
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/SetSentenceData",
        data: "{vals:" + castJson(vals) + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: SentenceSearch,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
}

function DelSentenceData(cd) {
    // HTTP通信開始
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/DelSentenceData",
        data: "{cd:'" + cd + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: SentenceSearch,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
}

function castJson(list) {
    var str = '[';

    for (var i = 0; i < list.length; i++) {
        if (i != 0)
            str += ",";
        str += "'" + list[i] + "'";
    }

    str += ']';

    return str;
}

function GetSentenceData_Result(result) {
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

            var $tr = $('<tr>').attr('id', SentenceCD);

            $tr.append($('<td>').addClass('SetButton dhead-ss').append(
                $('<input>').attr('id', "btn_" + SentenceCD).attr('type', 'button').addClass('list-button').val('←')
            ));
            $tr.append($('<td>').addClass('SentenceTitle dhead-sp').attr('title', coment).append(temp1));
            $tr.append($('<td>').addClass('SentenceName dhead-s').attr('title', coment).append(temp2));

            if ($LoginUserCd == dat[3]) {
                $tr.append($('<td>').addClass('SetButton dhead-ss').append(
                    $('<input>').attr('type', 'button').addClass('list-edit-btn').val('編集').data('cd', dat[0]).data('text1', dat[1]).data('text2', dat[2]).data('title', dat[4]).data('pcd', dat[6]).data('name', dat[5])
                ));
            } else {
                $tr.append($('<td>').addClass('SetButton dhead-ss'));
            }

            $("#tblSentenceList tbody").append($tr);

            // 定型文情報セット
            $("#" + SentenceCD).data("Sentence", dat);
            $("#btn_" + SentenceCD).data("Sentence", dat);
        }
    }

    $('#tblSentenceList tbody tr').on('dblclick', function () {
        var cd = $(this).data("Sentence");
        onSetSentenceButton(cd);
    });
    $('#tblSentenceList tbody tr td input:not(.list-edit-btn)').on('click', function () {
        var cd = $(this).data("Sentence");
        onSetSentenceButton(cd);
    });
    $('input.list-edit-btn').on('click', function () {
        var cd = $(this).data("cd");
        var txt1 = $(this).data("text1");
        var txt2 = $(this).data("text2");
        var title = $(this).data("title");
        var pcd = $(this).data("pcd");
        var name = $(this).data("name");
        $('#sent-edit').data("cd", cd).data("name", name);
        $('#sent-title').val(title);
        $('#sent-finding').val(txt1);
        $('#sent-diagnosing').val(txt2);
        $('input[name=sent-public]').val([pcd]);
        $('#sent-cp-find')[0].checked = true;
        $('#sent-cp-diag')[0].checked = true;
        $('#sent-delete').css('visibility', 'visible');

        $('#modal-sent-edit').modal();
    });
}

function onSetSentenceButton(SentenceCD) {
    // 所見
    var Finding = Escape($("#TxtFinding").val());
    // 診断
    var TxtDiagnosing = Escape($("#TxtDiagnosing").val());

    // 定型文情報
    var Sentence = SentenceCD;
    var temp1 = Finding + Escape(Sentence[1]);
    var temp2 = TxtDiagnosing + Escape(Sentence[2]);

    $("#TxtFinding").val(temp1);
    $("#TxtDiagnosing").val(temp2)

    $('#modal').modal('hide');
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
/* 履歴ボタンイベント */
function History() {
    // ダイアログ表示
    $ChangeHistoryDlg.dialog({
        closeText: "",
        modal: true,
        draggable: true,
        resizable: true,
        width: ($(window).width() - 20),
        height: ($(window).height() - 20),
        minWidth: 300,
        minHeight: 200
    });

    var serialNo = $ReportConfig.data("serialno");
    getChangeHistory(serialNo);
}
/* 履歴ボタンマウスダウンイベント */
function HistoryMouseDonw() {
    if ($ReportButtonHistory != null && $ReportButtonHistory != undefined) {
        $ReportButtonHistory.addClass("btnHistory-on");
        $ReportButtonHistory.removeClass("btnHistory-off");
        $ReportButtonHistory.removeClass("btnHistory-over");
    }
}
/* 履歴ボタンマウスアップイベント */
function HistoryMouseUp() {
    if ($ReportButtonHistory != null && $ReportButtonHistory != undefined) {
        $ReportButtonHistory.addClass("btnHistory-off");
        $ReportButtonHistory.removeClass("btnHistory-on");
        $ReportButtonHistory.removeClass("btnHistory-over");
    }
}
/* 履歴ボタンマウスオーバーイベント */
function HistoryMouseOver() {
    if ($ReportButtonHistory != null && $ReportButtonHistory != undefined) {
        $ReportButtonHistory.addClass("btnHistory-over");
        $ReportButtonHistory.removeClass("btnHistory-off");
        $ReportButtonHistory.removeClass("btnHistory-on");
    }
}

function getChangeHistory(serialno) {
    // HTTP通信開始
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./CommonWebService.asmx/GetChangeHistory",
        data: "{serialno:\"" + serialno + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: GetChangeHistoryData_Result,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。");
        }
    });
}

function GetChangeHistoryData_Result(result) {
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

    SetChangeHistoryTable(result.d.HistList, result.d.ImageList);
}

function SetChangeHistoryTable(HistList, ImageList) {
    var tblTag = "";
    var tblIdx = 0;

    $("#tblHistryList tbody").empty();

    // DIV を先に追加しないと表示が可笑しくなる
    for (var i = 0; i < HistList.length; i++) {
        var dat = HistList[i];
        tblIdx = (i + 1);
        tblTag = "";

        var divid = "HistryDiv" + tblIdx;

        //tblTag = "<tr><td><div id=\"" + divid + "\" style=\"width:100%;\">";
        //tblTag += "</div></td></tr>";
        //$("#tblHistryList tbody").append(tblTag);
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
        //tblTag += "<tr><td><div><table id=\"HistryData" + tblIdx + "\" border=\"1\" cellspacing=\"0\" cellpadding=\"0\" style=\"width:100%;\">";

        //// ヘッダー
        //tblTag += "<thead id=\"HistryHead" + tblIdx + "\"><tr>";
        //tblTag += "<th></th>";
        //tblTag += "<th></th>";
        //tblTag += "<th></th>";
        //tblTag += "<th></th>";
        //tblTag += "<th></th>";
        //tblTag += "</tr></thead>";

        //// ボディー
        //tblTag += "<tbody>";

        tblTag += "<tr class='hist-border'>";
        tblTag += "<td align=\"center\" class='hist-head' >指定日付</td>";
        tblTag += "<td align=\"center\">" + formatDateTime(dat[2], dat[3]) + "</td>";
        tblTag += "<td align=\"center\" class='hist-head' >診断医</td>";
        tblTag += "<td align=\"center\">" + dat[4] + "</td>";
        tblTag += "<td rowspan=\"3\"><div id=\"ReportImages" + tblIdx + "\"></div></td>";
        tblTag += "</tr>";

        tblTag += "<tr class='hist-border'>";
        tblTag += "<td align=\"center\" class='hist-head' style=\"height:100px;\">画像所見</td>";
        tblTag += "<td colspan=\"3\"><textarea id=\"TxtFinding" + tblIdx + "\" rows=\"\" readonly='readonly' cols=\"\" border=\"0\" style=\"width:100%; height:100px;\">" + dat[5] + "</textarea></td>";
        tblTag += "</tr>";

        tblTag += "<tr class='hist-border'>";
        tblTag += "<td align=\"center\" class='hist-head' style=\"height:100px;\">診断・結論</td>";
        tblTag += "<td colspan=\"3\"><textarea id=\"TxtDiagnosing" + tblIdx + "\" rows=\"\" readonly='readonly' cols=\"\" border=\"0\" style=\"width:100%; height:100px;\">" + dat[6] + "</textarea></td>";
        tblTag += "</tr>";
        tblTag += "<tr>";
        tblTag += "<td colspan=\"5\"></td>";
        tblTag += "</tr>";

        //tblTag += "</tbody>";

        //tblTag += "</table></div></td></tr>";
        //tblTag += "</table>";

        $("#tblHistryList tbody").append(tblTag);
        //$HistryDiv.append(tblTag);

        var $ReportImages = $("#ReportImages" + tblIdx);

        // ファイル名リスト
        var list = new Array();
        var imgList = ImageList[i];
        for (var j = 1; j < imgList.length; j++) {
            list.push(imgList[j]);
        }
        AddImage(list, $ReportImages, "webImageHistory.aspx", false, true, true, false);
    }

}

/***************************************************
/* 「画像所見」コピーボタンイベント */
function OpinionCopyFinding() {
    // 所見
    var Finding = Escape($("#TxtFinding").val());

    // 定型文情報
    var temp = Finding + Escape($("#TxtPastFinding").val());

    $("#TxtFinding").val(temp);

}

/***************************************************
/* 「診断」コピーボタンイベント */
function OpinionCopyDiagnosing() {
    // 診断
    var TxtDiagnosing = Escape($("#TxtDiagnosing").val());

    // 定型文情報
    var temp = TxtDiagnosing + Escape(Escape($("#TxtPastDiagnosing").val()));

    $("#TxtDiagnosing").val(temp)

}

/***************************************************
/* 「両方」コピーボタンイベント */
function OpinionCopyAll() {
    OpinionCopyFinding();
    OpinionCopyDiagnosing();
}

/***************************************************
/* 日付 ボタンイベント */
function OpinionCopyDate() {

    if ($historycd < 0) {
        return;
    }
    // ボタンクリックの行からレポートデータ取得
    var data = $("#" + $historycd).data("historyview");

    // 所見
    var Finding = Escape($("#TxtFinding").val());

    var history = "\r\n" + data.HisStudyDateTime + " の " + data.HisModality + " と比較しました。";

    // 定型文情報
    var temp = Finding + Escape(history);

    $("#TxtFinding").val(temp);
}

/***************************************************
/* テストボタンイベント */
function TestPage() {
    //window.open("./webTest.aspx", "_self", false);
    window.open("./webTest.aspx", "ProRadiRS_Test");
}

/***************************************************
/* 履歴ボタンイベント */
function ChangeHistory2() {
    // ダイアログ表示
    //$ChangeHistoryDlg.dialog({
    //    closeText: "",
    //    modal: true,
    //    draggable: true,
    //    resizable: true,
    //    width: ($(window).width() - 20),
    //    height: ($(window).height() - 20),
    //    minWidth: 300,
    //    minHeight: 200
    //});

    var serialNo = $ReportConfig.data("serialno");
    getChangeHistory(serialNo);

    $('#modal-hist').modal();
}
/* 履歴ボタンマウスダウンイベント */
function EditHistMouseDonw() {
    if ($('#btnEditHist') != null && $('#btnEditHist') != undefined) {
        $('#btnEditHist').addClass("btnEditHist-on");
        $('#btnEditHist').removeClass("btnEditHist-off");
        $('#btnEditHist').removeClass("btnEditHist-over");
    }
}
/* 履歴ボタンマウスアップイベント */
function EditHistMouseUp() {
    if ($('#btnEditHist') != null && $('#btnEditHist') != undefined) {
        $('#btnEditHist').addClass("btnEditHist-off");
        $('#btnEditHist').removeClass("btnEditHist-on");
        $('#btnEditHist').removeClass("btnEditHist-over");
    }
}
/* 履歴ボタンマウスオーバーイベント */
function EditHistMouseOver() {
    if ($('#btnEditHist') != null && $('#btnEditHist') != undefined) {
        $('#btnEditHist').addClass("btnEditHist-over");
        $('#btnEditHist').removeClass("btnEditHist-off");
        $('#btnEditHist').removeClass("btnEditHist-on");
    }
}


