var hospcd = "";

$(function () {
    //var param = GetQueryString();
    //if (param) {
    //    hospcd = param['cd'];
    //}

    init();

    $('#update').on('click', function () {
        ShowLoading();
        getList();
    });
});

function GetQueryString() {
    if (1 < document.location.search.length) {
        // 最初の1文字 (?記号) を除いた文字列を取得する
        var query = document.location.search.substring(1);

        // クエリの区切り記号 (&) で文字列を配列に分割する
        var parameters = query.split('&');

        var result = new Object();
        for (var i = 0; i < parameters.length; i++) {
            // パラメータ名とパラメータ値に分割する
            var element = parameters[i].split('=');

            var paramName = decodeURIComponent(element[0]);
            var paramValue = decodeURIComponent(element[1]);

            // パラメータ名をキーとして連想配列に追加する
            result[paramName] = decodeURIComponent(paramValue);
        }
        return result;
    }
    return null;
}

function init() {
    ShowLoading();
    getList();
}


function getList() {
    C_WebGetViewList(setList);
}

function setList(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    if (ret.d.Result) {
        $('#study_table tbody').remove();

        var order = ret.d.Items;

        var bodyRow = $('<tbody>');

        for (var i = 0; i < order.length; i++) {
            var tmprow = $('<tr>');

            tmprow.append($('<td>').addClass('dbody').append($('<input>').attr('type', 'button').attr('value', '所見').addClass('btn-sm').addClass('view').data('mod', order[i].Modality).data('date', order[i].Date).data('id', order[i].OrderID).data('patid', order[i].PatID).data('name', order[i].PatName)));

            if (order[i].Status == 0) {
                tmprow.append($('<td>').append('未'));
            } else {
                tmprow.append($('<td>').append('済').addClass('alert-data'));
            }

            tmprow.append($('<td>').append(order[i].PatID));
            tmprow.append($('<td>').append(order[i].PatName));
            tmprow.append($('<td>').append(order[i].Modality));
            tmprow.append($('<td>').append(order[i].Date));

            bodyRow.append(tmprow);

            $('#study_table').append(bodyRow);
        }
        $('input.view').on('click', function () {
            sessionStorage.setItem('orderid', $(this).data('id'));
            sessionStorage.setItem('patid', $(this).data('patid'));
            sessionStorage.setItem('mod', $(this).data('mod'));
            sessionStorage.setItem('date', $(this).data('date'));
            sessionStorage.setItem('name', $(this).data('name'));

            postForm('./View.aspx');
        });
    }
    CloseLoading();
}


function C_WebGetViewList(func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetViewList",
        data: "{ hospcd:'" + hospcd + "'}",
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

function postForm(url) {
    var $form = $('<form/>', { 'action': url, 'method': 'post' })
        .append($('<input/>', { type: 'hidden', name: 'orderid', value: sessionStorage.getItem('orderid') }))
        .append($('<input/>', { type: 'hidden', name: 'mod', value: sessionStorage.getItem('mod') }))
        .append($('<input/>', { type: 'hidden', name: 'date', value: sessionStorage.getItem('date') }))
        .append($('<input/>', { type: 'hidden', name: 'cd', value: sessionStorage.getItem('cd') }))
        .append($('<input/>', { type: 'hidden', name: 'name', value: sessionStorage.getItem('name') }))
        .append($('<input/>', { type: 'hidden', name: 'patid', value: sessionStorage.getItem('patid') }));
    $form.appendTo(document.body);
    $form.submit();
}

function ShowLoading() {
    var h = $(window).height();

    $('#loader-bg ,#loader').height(h).css('display', 'block');
}

function CloseLoading() {
    $('#loader-bg').delay(900).fadeOut(800);
    $('#loader').delay(600).fadeOut(300);
}