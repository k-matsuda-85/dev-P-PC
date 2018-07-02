var order = [];
var sKey = "";
var sAsc = "DESC"

var Search = [];

$(function () {
    _util.search = sessionStorage.getItem('search');
    $(document).click(function () {
        $('ul.dropdown-menu').hide();
    });

    $('#date_f').on('focus', function (e) {
        if ($('#date_f').val() == '') {
            var date = new Date();
            var formattedDate =
                date.getFullYear() + '/' + (date.getMonth() + 1) + '/' + date.getDate();

            $("#date_f").datepicker("setDate", formattedDate);
        }
    });
    $('#date_t').on('focus', function (e) {
        if ($('#date_t').val() == '') {
            var date = new Date();
            var formattedDate =
                date.getFullYear() + '/' + (date.getMonth() + 1) + '/' + date.getDate();

            $("#date_t").datepicker("setDate", formattedDate);
        }
    });
    $('input.listblock').on('click focus', function (e) {
        var id = $(this).attr('id');

        event.stopPropagation();

        $('ul.dropdown-menu').hide();
        $('#' + id + '-list').toggle();


        $('#' + id + '-list li a').focus();
        return false;
    });

    $('th.point').on('click', function (e) {
        var id = $(this).attr('id');

        if (sKey != id) {
            ResetHead();
            sKey = id;
        } else {
            if (sAsc == 'ASC')
                sAsc = 'DESC';
            else
                sAsc = 'ASC';
        }

        switch (id)
        {
            case 's-status':
                if (sAsc == 'DESC'){
                    order.sort(function (a, b) {
                        return a.Status < b.Status ? 1 : -1;
                    });
                    $('#s-status').text("出力▼");
                }
                else {
                    order.sort(function (a, b) {
                        return a.Status > b.Status ? 1 : -1;
                    });
                    $('#s-status').text("出力▲");
                }
                break;
            case 's-patid':
                if (sAsc == 'DESC'){
                    order.sort(function (a, b) {
                        return a.PatID < b.PatID ? 1 : -1;
                    });
                    $('#s-patid').text("患者ID▼");
                }
                else {
                    order.sort(function (a, b) {
                        return a.PatID > b.PatID ? 1 : -1;
                    });
                    $('#s-patid').text("患者ID▲");
                }
                break;
            case 's-name':
                if (sAsc == 'DESC'){
                    order.sort(function (a, b) {
                        return a.PatName < b.PatName ? 1 : -1;
                    });
                    $('#s-name').text("患者名▼");
                }
                else {
                    order.sort(function (a, b) {
                        return a.PatName > b.PatName ? 1 : -1;
                    });
                    $('#s-name').text("患者名▲");
                }
                break;
            case 's-mod':
                if (sAsc == 'DESC'){
                    order.sort(function (a, b) {
                        return a.Modality < b.Modality ? 1 : -1;
                    });
                    $('#s-mod').text("モダリティ▼");
                }
                else {
                    order.sort(function (a, b) {
                        return a.Modality > b.Modality ? 1 : -1;
                    });
                    $('#s-mod').text("モダリティ▲");
                }
                break;
            case 's-date':
                if (sAsc == 'DESC'){
                    order.sort(function (a, b) {
                        return (a.Date + a.Time) < (b.Date + b.Time) ? 1 : -1;
                    });
                    $('#s-date').text("検査日時▼");
                }
                else {
                    order.sort(function (a, b) {
                        return (a.Date + a.Time) > (b.Date + b.Time) ? 1 : -1;
                    });
                    $('#s-date').text("検査日時▲");
                }
                break;
            case 's-desc':
                if (sAsc == 'DESC'){
                    order.sort(function (a, b) {
                        return a.Desc < b.Desc ? 1 : -1;
                    });
                    $('#s-desc').text("検査部位▼");
                }
                else {
                    order.sort(function (a, b) {
                        return a.Desc > b.Desc ? 1 : -1;
                    });
                    $('#s-desc').text("検査部位▲");
                }
                break;
            case 's-body':
                if (sAsc == 'DESC') {
                    order.sort(function (a, b) {
                        return a.BodyPart < b.BodyPart ? 1 : -1;
                    });
                    $('#s-body').text("依頼部位▼");
                }
                else {
                    order.sort(function (a, b) {
                        return a.BodyPart > b.BodyPart ? 1 : -1;
                    });
                    $('#s-body').text("依頼部位▲");
                }
                break;
        }

        ListLoad();
        return false;
    });

    $('.datepicker').each(function () {
        $(this).datepicker({
            language: 'ja'
        });
    });

    $(document).on('click', 'ul li a', function (e) {
        var key = $(this).data('key');
        var strInsert = $(this).text();

        if ($(this).parent().parent().hasClass('listblock')) {
            //文字列を結合して、テキストエリアに出力
            $('#' + key).val(strInsert);
        }
        else {
            var strOriginal = $('#' + key).val();

            if (!strOriginal)
                return;

            //カーソル位置より左の文字列
            var leftPart = strOriginal.substr(0, caret);
            //カーソル位置より右の文字列
            var rightPart = strOriginal.substr(caret, strOriginal.length);

            //文字列を結合して、テキストエリアに出力
            $('#' + key).val(leftPart + strInsert + rightPart);
        }
    });

    $('#newdata').on('click', function () {
        sessionStorage.setItem('orderid', '');

        postForm('./Order.aspx');
    });

    $('#update').on('click', function () {
        ShowLoading();
        getList();
    });

    $('#clear').on('click', function () {
        Template = [];

        $('#status').val('');
        $('#patid').val('');
        $('#modality').val('');
        $('#date_f').val('');
        $('#date_t').val('');

        Search = [];
    });

    $('#search').on('click', function () {
        Search = [];

        ShowLoading();

        var date = "";
        if ($('#date_f').val() != "" || $('#date_t').val() != "")
            date = formatDate($('#date_f').val()) + "-" + formatDate($('#date_t').val());

        Search[Search.length] = $('#patid').val();
        Search[Search.length] = $('#modality').val();
        Search[Search.length] = $('#date_f').val();
        Search[Search.length] = $('#date_t').val();
        Search[Search.length] = $('#status').val();

        C_WebGetOrderList($('#patid').val(), date, $('#modality').val(), setList);

        ResetHead();
    });

    var date = new Date();
    var formattedDate =
        date.getFullYear() + '/' + ('0' + (date.getMonth() + 1)).slice(-2) + '/' + ('0' + date.getDate()).slice(-2);

    $('#date_f').val(formattedDate);
    $('#date_t').val(formattedDate);


    if (_util.search && _util.search != '')
    {
        var params = _util.search.split(',');

        $('#patid').val(params[0]);
        $('#modality').val(params[1]);
        $('#date_f').val(params[2]);
        $('#date_t').val(params[3]);
        $('#status').val(params[4]);

        ShowLoading();
        date = "";
        if ($('#date_f').val() != "" || $('#date_t').val() != "")
            date = formatDate($('#date_f').val()) + "-" + formatDate($('#date_t').val());

        Search[Search.length] = $('#patid').val();
        Search[Search.length] = $('#modality').val();
        Search[Search.length] = $('#date_f').val();
        Search[Search.length] = $('#date_t').val();
        Search[Search.length] = $('#status').val();

        C_WebGetOrderList($('#patid').val(), date, $('#modality').val(), setList);
    }

    ResetHead();
});

function ResetHead() {
    $('#s-status').text("出力");
    $('#s-patid').text("患者ID");
    $('#s-name').text("患者名");
    $('#s-mod').text("モダリティ");
    $('#s-date').text("検査日時");
    $('#s-desc').text("検査部位");
    $('#s-body').text("依頼部位");

    sAsc = "DESC";
    sKey = "";
}

function formatDate(str) {
    var ret = '';

    if (!str || str == '')
        return ret;

    if (str.length == 8)
        ret = str.substring(0, 4) + '/' + str.substring(4, 6) + '/' + str.substring(6, 8);
    else if (str.length == 10) {
        var list = str.split('/');
        ret = list[0] + ('0' + list[1]).slice(-2) + ('0' + list[2]).slice(-2);
    }
    else
        ret = str;

    return ret;
}

function formatTime(str) {
    var ret = str;

    if (str.indexOf(':') < 0 && str.length >= 6)
    {
        ret = str.substring(0, 2) + ':' + str.substring(2, 4) + ':' + str.substring(4, 6);
    }

    return ret;
}

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

    order = ret.d.Items;

    ListLoad();

    CloseLoading();
}

function ListLoad()
{
    $('#study_table tbody').remove();

    if (order) {
        var bodyRow = $('<tbody>');

        for (var i = 0; i < order.length; i++) {
            var tmprow = $('<tr>');

            if ($('#status').val() == '1') {
                if (order[i].Status == 0)
                    continue;
            } else if ($('#status').val() == '0') {
                if (order[i].Status == 1)
                    continue;
            }

            if (order[i].Status >= 1) {
                tmprow.append($('<td>').addClass('dbody').append($('<input>').attr('type', 'button').attr('value', '削除').addClass('btn-sm').addClass('del').data('id', order[i].OrderNo).data('patid', order[i].PatID).data('mod', order[i].Modality).data('date', order[i].Date)));
                tmprow.append($('<td>').addClass('dbody').append($('<input>').attr('type', 'button').attr('value', '変更').addClass('btn-sm').addClass('edit').data('id', order[i].OrderNo).data('patid', order[i].PatID).data('uid', order[i].StudyInstanceUID)));

                if (order[i].IsEmergency == 1)
                    tmprow.append($('<td>').append('緊急').addClass('alert-data'));
                else
                    tmprow.append($('<td>'));

                tmprow.append($('<td>').append('済').addClass('alert-data'));
            }
            else if (order[i].Status == 0) {
                tmprow.append($('<td>'));
                tmprow.append($('<td>').addClass('dbody').append($('<input>').attr('type', 'button').attr('value', '依頼').addClass('btn-sm').addClass('view').data('id', order[i].OrderNo).data('uid', order[i].StudyInstanceUID).data('patid', order[i].PatID)));

                if (order[i].IsEmergency == 1)
                    tmprow.append($('<td>').append('緊急').addClass('alert-data'));
                else
                    tmprow.append($('<td>'));

                tmprow.append($('<td>').append(''));
            } else {
                continue;
            }

            tmprow.append($('<td>').append(order[i].PatID));
            tmprow.append($('<td>').append(order[i].PatName));
            tmprow.append($('<td>').append(order[i].Modality));
            tmprow.append($('<td>').append(order[i].Desc));
            tmprow.append($('<td>').append(formatDate(order[i].Date) + " " + formatTime(order[i].Time)));
            tmprow.append($('<td>').append(order[i].BodyPart));

            bodyRow.append(tmprow);

            $('#study_table').append(bodyRow);
        }

        $('input.edit').on('click', function () {
            sessionStorage.setItem('orderid', $(this).data('id'));
            sessionStorage.setItem('uid', $(this).data('uid'));

            postForm('./Order.aspx');
        });
        $('input.view').on('click', function () {
            sessionStorage.setItem('orderid', $(this).data('id'));
            sessionStorage.setItem('uid', $(this).data('uid'));

            postForm('./Order.aspx');
        });
        $('input.del').on('click', function () {
            if (!confirm("以下の依頼をキャンセルします。よろしいですか？\n\n患者ID：" + $(this).data('patid') + "\nﾓﾀﾞﾘﾃｨ：" + $(this).data('mod') + "\n検査日：" + $(this).data('date') + "\n"))
                return;

            ShowLoading();
            C_DelPreOrder($(this).data('id'), EndDel);
        });
    }

}

function EndDel(ret) {
    if (!ret || !ret.d) {
        alert("検査削除中に問題が発生いたしました。");
        return;
    }

    alert("検査を削除しました。");

    Search = [];

    var date = "";
    if ($('#date_f').val() != "" && $('#date_t').val() != "")
        date = formatDate($('#date_f').val()) + "-" + formatDate($('#date_t').val());

    Search[Search.length] = $('#patid').val();
    Search[Search.length] = $('#modality').val();
    Search[Search.length] = $('#date_f').val();
    Search[Search.length] = $('#date_t').val();
    Search[Search.length] = $('#status').val();

    C_WebGetOrderList($('#patid').val(), date, $('#modality').val(), setList);

    ResetHead();
}

function C_WebGetOrderList(patid, date, mod, func) {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetPreList",
        data: "{ patid:'" + patid + "', date:'" + date + "', mod:'" + mod + "', isCnt:0 }",
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
        data: "{ orderNo:'" + val + "'}",
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
    if (Search && Search.length > 0)
    {
        var param = "";
        param += Search[0];
        param += ',';
        param += Search[1];
        param += ',';
        param += Search[2];
        param += ',';
        param += Search[3];
        param += ',';
        param += Search[4];
        sessionStorage.setItem('search', param);
    }

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