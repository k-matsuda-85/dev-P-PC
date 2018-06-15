/* グローバル宣言 */
var $inputLoginID;  // ログインIDオブジェクト
var $inputLoginPW;  // ログインPWオブジェクト
var $btnLogin;      // ログインボタンオブジェクト
var $btnClear;      // クリアボタンオブジェクト

/* 起動処理 */
$(window).load(function () {
    // 起動処理
    LoginWindow_Init();
});
function LoginWindow_Init() {
    // オブジェクトキャッシュ
    $inputLoginID = $("#LoginID");
    $inputLoginPW = $("#LoginPW");
    $btnLogin = $("#LoginButton");
    $btnClear = $("#ClearButton");

    // 画面の画像ファイル先読み込み
    LoginWindow_PreLoadImages();

    // ログインIDに入力フォーカス
    $inputLoginID.focus();
}
/***************************************************
/* 画面の画像ファイル先読み込み(チラつき防止) */
function LoginWindow_PreLoadImages() {
    jQuery("<img>").attr("src", "./img/EasyRSwebﾛｸﾞｲﾝｱﾆﾒ01.gif?20141110");
    jQuery("<img>").attr("src", "./img/ログインボタン_off.png?20141110");
    jQuery("<img>").attr("src", "./img/ログインボタン_on.png?20141110");
    jQuery("<img>").attr("src", "./img/ログインボタン_over.png?20141110");
    jQuery("<img>").attr("src", "./img/ログインボタン_non.png?20141110");
    jQuery("<img>").attr("src", "./img/クリアボタン_off.png?20141110");
    jQuery("<img>").attr("src", "./img/クリアボタン_on.png?20141110");
    jQuery("<img>").attr("src", "./img/クリアボタン_over.png?20141110");
    jQuery("<img>").attr("src", "./img/ヘッダーBG.png?20141110");
}

/* ログインIDキーイベント */
function LoginID_Enter() {
    if (window.event.keyCode == 13) {
        $inputLoginPW.focus();
    }
}
/* ログインPWキーイベント */
function LoginPW_Enter() {
    if (window.event.keyCode == 13) {
        Login();
    }
}

/* ログインボタンマウスダウン */
function LoginMouseDonw() {
    if ($btnLogin != null && $btnLogin != undefined) {
        $btnLogin.addClass("LoginButton-on");
        $btnLogin.removeClass("LoginButton-off");
        $btnLogin.removeClass("LoginButton-over");
    }
}
/* ログインボタンマウスアップ(マウスアウト) */
function LoginMouseUp() {
    if ($btnLogin != null && $btnLogin != undefined) {
        $btnLogin.addClass("LoginButton-off");
        $btnLogin.removeClass("LoginButton-on");
        $btnLogin.removeClass("LoginButton-over");
    }
}
/* ログインボタンマウスオーバー */
function LoginMouseOver() {
    if ($btnLogin != null && $btnLogin != undefined) {
        $btnLogin.addClass("LoginButton-over");
        $btnLogin.removeClass("LoginButton-on");
        $btnLogin.removeClass("LoginButton-off");
    }
}

/* クリアボタンマウスダウン */
function ClearMouseDonw() {
    if ($btnClear != null && $btnClear != undefined) {
        $btnClear.addClass("ClearButton-on");
        $btnClear.removeClass("ClearButton-off");
        $btnClear.removeClass("ClearButton-over");
    }
}
/* クリアボタンマウスアップ(マウスアウト) */
function ClearMouseUp() {
    if ($btnClear != null && $btnClear != undefined) {
        $btnClear.addClass("ClearButton-off");
        $btnClear.removeClass("ClearButton-on");
        $btnClear.removeClass("ClearButton-over");
    }
}
/* クリアボタンマウスオーバー */
function ClearMouseOver() {
    if ($btnClear != null && $btnClear != undefined) {
        $btnClear.addClass("ClearButton-over");
        $btnClear.removeClass("ClearButton-off");
        $btnClear.removeClass("ClearButton-on");
    }
}

/* クリア */
function Clear() {
    $inputLoginID.val("");
    $inputLoginPW.val("");
    $inputLoginID.focus();
}

/* ログイン */
function Login() {
    // ログイン
    Common_Login($inputLoginID.val(), $inputLoginPW.val(), Login_Result);
}
function Common_Login(loginID, loginPW, func) {
    // HTTP通信開始
    $.ajax({
        async: false,
        type: "POST",
        url: "./CommonWebService.asmx/Login",
        data: "{loginID:\"" + loginID + "\",loginPW:\"" + loginPW + "\"}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: func,
        error: function (result) {
            // エラー
            alert("通信エラーが発生しました。\n" + result.responseText + result.status + result.statusText);
        }
    });
}
function Login_Result(result) {
    // データチェック
    if (result.d == null || result.d == -1) {
        alert("ログインでエラーが発生しました。");
        return;
    }
    else if (result.d == 1) {
        alert("ユーザーID または パスワード に誤りがあります。");
        $inputLoginPW.val("");
        if ($inputLoginID.val().length == 0) {
            $inputLoginID.focus();
        }
        else {
            $inputLoginPW.focus();
        }
        return;
    }

    // 検索ページ表示
    window.open("./webSearch.aspx", "_self", false);
}
