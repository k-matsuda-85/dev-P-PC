$(function () {
    var param = GetQueryString();
    if (param) {
        sessionStorage.clear();
        sessionStorage.setItem('userid', param['userid']);
        sessionStorage.setItem('key', 'systemlogin');
    }

    if (!_util.key || _util.key == '')
    {
        _util.userid = sessionStorage.getItem('userid');
        _util.key = sessionStorage.getItem('key');
    }
    sessionStorage.setItem('orderid' ,'');
    sessionStorage.setItem('hospid', '');
    sessionStorage.setItem('patid', '');
    sessionStorage.setItem('view', '');

    if (!_util.key || _util.key == '') {
        postForm('./Login.aspx');
        return;
    }

    init();

    $('#newdata').on('click', function () {
        sessionStorage.setItem('key', _util.key);
        sessionStorage.setItem('userid', _util.userid);
        sessionStorage.setItem('hospid', _util.hospid);
        sessionStorage.setItem('orderid', '');

        postForm('./Order.aspx');
    });

    $('#update').on('click', function () {
        _util.orderid = '';

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
    C_GetHosp(_util.userid, setHosp);
}

function setHosp(ret) {
    if(!ret || !ret.d) {
        CloseLoading();
        return;
    }

    if (ret.d.Result) {
        var hosp = ret.d.Items;
        _util.hospid = hosp.HospID;

        getList();
    }
}

function getList() {
    C_WebGetPreList(_util.hospid, setList);
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

            if (order[i].Status == 0)
            {
                tmprow.append($('<td>').addClass('dbody').append($('<input>').attr('type', 'button').attr('value', '入力').addClass('btn-sm').addClass('edit').data('id', order[i].OrderID).data('patid', order[i].PatID)));
                tmprow.append($('<td>').addClass('dbody').append($('<input>').attr('type', 'button').attr('value', '削除').addClass('btn-sm').addClass('del').data('id', order[i].OrderID).data('patid', order[i].PatID).data('mod', order[i].Modality).data('date', order[i].Date)));

                if(order[i].IsEmergency == 1)
                    tmprow.append($('<td>').append('緊急').addClass('alert-data'));
                else
                    tmprow.append($('<td>'));

                tmprow.append($('<td>').append('未'));
            }
            else if (order[i].Status == 1)
            {
                tmprow.append($('<td>').addClass('dbody').append($('<input>').attr('type', 'button').attr('value', '詳細').addClass('btn-sm').addClass('view').data('id', order[i].OrderID).data('patid', order[i].PatID)));
                tmprow.append($('<td>'));

                if(order[i].IsEmergency == 1)
                    tmprow.append($('<td>').append('緊急').addClass('alert-data'));
                else
                    tmprow.append($('<td>'));

                tmprow.append($('<td>').append('済').addClass('alert-data'));
            } else {
                continue;
            }

            tmprow.append($('<td>').append(order[i].PatID));
            tmprow.append($('<td>').append(order[i].PatName));
            tmprow.append($('<td>').append(order[i].Modality));
            tmprow.append($('<td>').append(order[i].Date));
            tmprow.append($('<td>').append(order[i].BodyPart));
            tmprow.append($('<td>').append(order[i].Comment));

            bodyRow.append(tmprow);

            $('#study_table').append(bodyRow);
        }

        $('input.edit').on('click', function () {
            sessionStorage.setItem('key', _util.key);
            sessionStorage.setItem('userid', _util.userid);
            sessionStorage.setItem('hospid', _util.hospid);
            sessionStorage.setItem('orderid', $(this).data('id'));
            sessionStorage.setItem('patid', $(this).data('patid'));

            postForm('./Order.aspx');
        });
        $('input.view').on('click', function () {
            sessionStorage.setItem('key', _util.key);
            sessionStorage.setItem('userid', _util.userid);
            sessionStorage.setItem('hospid', _util.hospid);
            sessionStorage.setItem('orderid', $(this).data('id'));
            sessionStorage.setItem('patid', $(this).data('patid'));
            sessionStorage.setItem('view', 1);

            postForm('./Order.aspx');
        });
        $('input.del').on('click', function () {
            if(!confirm("以下の検査を削除します。よろしいですか？\n\n患者ID：" + $(this).data('patid') +  "\nﾓﾀﾞﾘﾃｨ：" + $(this).data('mod') +  "\n検査日：" + $(this).data('date') +  "\n"))
                return;

            var value = [];

            value[value.length] = _util.hospid;
            value[value.length] = $(this).data('id');
            value[value.length] = $(this).data('patid');
            value[value.length] = $(this).data('mod');
            value[value.length] = $(this).data('date');
            C_DelPreOrder(value, EndDel);
        });
    }
    CloseLoading();
}

function EndDel(ret) {
    if (!ret || !ret.d) {
        alert("検査削除中に問題が発生いたしました。");
        return;
    }

    alert("検査を削除しました。");
    ShowLoading();
    getList();
}

function C_GetHosp(userid, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetHosp",
        data: "{ userid:'" + userid + "'}",
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
function C_WebGetPreList(hospid, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetPreList",
        data: "{ hospid:'" + hospid + "', patid:'' }",
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

function C_DelPreOrder(val, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/DelPreOrder",
        data: "{ values:" + castJson(val) + "}",
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


function postForm(url) {
    var $form = $('<form/>', { 'action': url, 'method': 'post' });
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