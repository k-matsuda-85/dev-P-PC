﻿$(function () {

    $('#btn_login').on('click', function () {
        var isErr = false;
        var user_id = $('#user').val();

        ShowLoading();

        sessionStorage.clear();
        sessionStorage.setItem('userid', user_id);
        window.open('./Search.aspx', '_blank');
        postForm('./Main.aspx');

    });

    C_UserList(setUserList);

});

function setUserList(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    if (ret.d.Result) {
        var Users = ret.d.Items;

        for(var i = 0; i < Users.length; i++)
        {
            $('#user').append($('<option>').val(Users[i].UserID).append(Users[i].Name));
        }
    }

}



function postForm(url) {
    var $form = $('<form/>', { 'action': url, 'method': 'post' });
    $form.appendTo(document.body);
    $form.submit();
};

function C_UserList(func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetUserList",
        data: "",
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