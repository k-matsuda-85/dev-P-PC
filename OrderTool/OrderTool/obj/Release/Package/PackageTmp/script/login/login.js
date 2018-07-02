$(function () {

    $('#btn_login').on('click', function () {
        var isErr = false;
        var login_id = $('#login_id').val();
        if (!login_id || login_id == '')
        {
            $('#login_id').closest('div').addClass('has-error');
            isErr = true;
        }

        var login_pw = $('#login_pw').val();
        if (!login_pw || login_pw == '') {
            $('#login_pw').closest('div').addClass('has-error');
            isErr = true;
        }

        if(isErr)
        {
            alert("ID,パスワードを正しく入力してください。");
            return;
        }

        ShowLoading();

        C_Login(login_id, login_pw, checkLogin);
    });

});



function checkLogin(ret) {
    if(!ret || !ret.d) {
        $('#message').val('システムエラーが発生しました。');
        CloseLoading();
        return;
    }

    if (ret.d.Result) {
        var login = ret.d.Items;

        sessionStorage.clear();
        sessionStorage.setItem('userid', login.UserID);
        sessionStorage.setItem('key', login.Key);

        postForm('./Search.aspx');
    } else {
        $('#message').empty();
        $('#message').append(ret.d.Message);
    }
    CloseLoading();
}

function postForm(url) {
    var $form = $('<form/>', { 'action': url, 'method': 'post' });
    $form.appendTo(document.body);
    $form.submit();
};

function C_Login(id, pw, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebLogin",
        data: "{ id:'" + id + "', pw:'" + pw + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            func(result);
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
            CloseLoading();
        }
    });
}

function ShowLoading() {
    var h = $(window).height();

    $('#loader-bg ,#loader').height(h).css('display', 'block');
}

function CloseLoading() {
    $('#loader-bg').delay(900).fadeOut(800);
    $('#loader').delay(600).fadeOut(300);
}