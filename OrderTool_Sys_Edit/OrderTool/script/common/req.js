

function checkLogin(ret) {
    if (!ret || !ret.d) {
        $('#message').val('システムエラーが発生しました。');
        CloseLoading();
        return;
    }

    if (ret.d.Result) {
        var login = ret.d.Items;

        sessionStorage.setItem('userid', login.UserID);
        sessionStorage.setItem('key', login.Key);

        postForm('./Search.aspx');
    } else {
        $('#message').empty();
        $('#message').append(ret.d.Message);
    }
    CloseLoading();
}

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

function C_GetUser(id, pw, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/GetUser",
        data: "{ id:'" + id + "'}",
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