var Template;
var Search;

var hosp;

$(function () {
    var param = GetQueryString();
    if (param) {
        sessionStorage.clear();
        sessionStorage.setItem('userid', param['userid']);
    }

    if (!_util.userid || _util.userid == '')
    {
        _util.userid = sessionStorage.getItem('userid');
        _util.param = sessionStorage.getItem('param');
        if (_util.param == ",2,,,,")
            _util.param = "";
    }
    sessionStorage.setItem('orderid' ,'');
    sessionStorage.setItem('hospid', '');
    sessionStorage.setItem('patid', '');
    sessionStorage.setItem('view', '');

    if (!_util.userid || _util.userid == '') {
        postForm('./Login.aspx');
        return;
    }

    init();

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

    $('#hosp').on('change', function (e) {
        _util.hospid = $('#hosp').val();

        if (_util.hospid != '')
            C_GetHospTempList(setHospTempList);
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

    $('#view').on('click', function () {
        if ($('#main-form').is(':visible'))
            $('#main-form').slideUp();
        else {
            $('#main-form').slideDown();

            if (Search && Search.length > 0) {
                if (Search[0] == ''
                    && Search[1] == '4'
                    && Search[2] == ''
                    && Search[3] == ''
                    && Search[4] == ''
                    && Search[5] == ''
                    && Search[6] == '')
                $('#status').val(2);
            }
        }
    });

    $('#clear').on('click', function () {
        Template = [];
        setTemp('modality');

        _util.hospid = '';
        $('#status').val('4');
        $('#hosp').val('');
        $('#patid').val('');
        $('#modality').val('');
        $('#date_f').val('');
        $('#date_t').val('');

        Search = [];
    });

    $('#search').on('click', function () {
        Search = [];

        ShowLoading();

        Search[Search.length] = $('#hosp').val();
        Search[Search.length] = $('#status').val();
        Search[Search.length] = '';
        Search[Search.length] = $('#patid').val();
        Search[Search.length] = $('#modality').val();
        Search[Search.length] = formatDate($('#date_f').val());
        Search[Search.length] = formatDate($('#date_t').val());

        C_WebGetOrderList(Search, setList);

    });

    $('#reload').on('click', function () {
        ShowLoading();
        if(Search && Search.length > 0)
        {
            $('#hosp').val(Search[0]);
            $('#status').val(Search[1]);
            $('#patid').val(Search[3]);
            $('#modality').val(Search[4]);
            $('#date_f').val(formatDate(Search[5]));
            $('#date_t').val(formatDate(Search[6]));
        } else {
            Search[Search.length] = '';
            Search[Search.length] = '4';
            Search[Search.length] = '';
            Search[Search.length] = '';
            Search[Search.length] = '';
            Search[Search.length] = '';
            Search[Search.length] = '';
        }

        C_WebGetOrderList(Search, setList);

    });

    $('#reload-first').on('click', function () {
        ShowLoading();
        Template = [];
        setTemp('modality');

        _util.hospid = '';
        $('#status').val('4');
        $('#hosp').val('');
        $('#patid').val('');
        $('#modality').val('');
        $('#date_f').val('');
        $('#date_t').val('');

        Search = [];

        ShowLoading();

        Search[Search.length] = $('#hosp').val();
        Search[Search.length] = $('#status').val();
        Search[Search.length] = '';
        Search[Search.length] = $('#patid').val();
        Search[Search.length] = $('#modality').val();
        Search[Search.length] = formatDate($('#date_f').val());
        Search[Search.length] = formatDate($('#date_t').val());

        C_WebGetOrderList(Search, setList);
    });

    $('#inputCSV').on('click', function () {
        C_WebGetOrderCSV();
    });
    $('#modality').on('change', function () {
        setModColor($(this), $(this).val());
    });

    $('.datepicker').each(function () {
        $(this).datepicker({
            language: 'ja'
        });
    });

    $('#status').val(4);

    Search = [];

    ShowLoading();

    if (_util.param && _util.param != '')
    {
        var params = _util.param.split(',');

        $('#hosp').val(params[0]);
        $('#status').val(params[1]);
        $('#patid').val(params[2]);
        $('#modality').val(params[3]);
        $('#date_f').val(params[4]);
        $('#date_t').val(params[5]);
    }

    Search[Search.length] = $('#hosp').val();
    Search[Search.length] = $('#status').val();
    Search[Search.length] = '';
    Search[Search.length] = $('#patid').val();
    Search[Search.length] = $('#modality').val();
    Search[Search.length] = formatDate($('#date_f').val());
    Search[Search.length] = formatDate($('#date_t').val());

    C_WebGetOrderList(Search, setList);
});

function setHospTempList(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    if (ret.d.Result) {
        Template = ret.d.Items;
    }

    setTemp('modality');

    CloseLoading();
}

function setTemp(key) {
    var obj = $('#' + key + '-list');

    obj.empty();

    for (var i = 0; i < Template.length; i++)
        if (Template[i].Key == key)
            obj.append($('<li>').append($('<a>').data('key', key).append(Template[i].Value)));

    if(Template.length == 0)
    {
        obj.append($('<li>').append($('<a>').data('key', key).append('CT')));
        obj.append($('<li>').append($('<a>').data('key', key).append('MR')));
        obj.append($('<li>').append($('<a>').data('key', key).append('CR')));
        obj.append($('<li>').append($('<a>').data('key', key).append('MG')));
        obj.append($('<li>').append($('<a>').data('key', key).append('RF')));
    }
}

function setHosp(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    if (ret.d.Result) {
        hosp = ret.d.Items;

        for (var i = 0; i < hosp.length; i++) {
            if (hosp[i].Visible == 0)
                $('#hosp').append($('<option>').val(hosp[i].HospID).append(hosp[i].CD + " " + hosp[i].Name));
        }

    }
    CloseLoading();
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
    C_GetHospList(setHosp);
}

function getList() {
    C_WebGetOrderList(Search, setList);
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

            if (order[i].Status == -1)
            {
                tmprow.append($('<td>').addClass('dbody').append($('<input>').attr('type', 'button').attr('value', '入力').addClass('btn-sm').addClass('edit').data('id', order[i].OrderID).data('patid', order[i].PatID).data('hospid', order[i].HospID)));
                tmprow.append($('<td>'));

                if(order[i].IsEmergency == 1)
                    tmprow.append($('<td>').append('緊急').addClass('alert-data'));
                else
                    tmprow.append($('<td>'));

                tmprow.append($('<td>').append('未入力').addClass('nomal-data'));
            }
            else if (order[i].Status == 0)
            {
                tmprow.append($('<td>').addClass('dbody').append($('<input>').attr('type', 'button').attr('value', '入力').addClass('btn-sm').addClass('edit').data('id', order[i].OrderID).data('patid', order[i].PatID).data('hospid', order[i].HospID)));
                tmprow.append($('<td>'));

                if(order[i].IsEmergency == 1)
                    tmprow.append($('<td>').append('緊急').addClass('alert-data'));
                else
                    tmprow.append($('<td>'));

                tmprow.append($('<td>').append('一時').addClass('nomal-data'));
            }
            else if (order[i].Status == 1) {
                tmprow.append($('<td>').addClass('dbody').append($('<input>').attr('type', 'button').attr('value', '入力').addClass('btn-sm').addClass('edit').data('id', order[i].OrderID).data('patid', order[i].PatID).data('hospid', order[i].HospID)));
                tmprow.append($('<td>'));

                if (order[i].IsEmergency == 1)
                    tmprow.append($('<td>').append('緊急').addClass('alert-data'));
                else
                    tmprow.append($('<td>'));

                tmprow.append($('<td>').append('登録').addClass('commit-data'));
            }
            else if (order[i].Status == 2) {
                tmprow.append($('<td>').addClass('dbody').append($('<input>').attr('type', 'button').attr('value', '入力').addClass('btn-sm').addClass('edit').data('id', order[i].OrderID).data('patid', order[i].PatID).data('hospid', order[i].HospID)));
                tmprow.append($('<td>'));

                if (order[i].IsEmergency == 1)
                    tmprow.append($('<td>').append('緊急').addClass('alert-data'));
                else
                    tmprow.append($('<td>'));

                tmprow.append($('<td>').append('出力済').addClass('nomal-data'));
            }
            else if (order[i].Status == 3) {
                tmprow.append($('<td>').addClass('dbody').append($('<input>').attr('type', 'button').attr('value', '入力').addClass('btn-sm').addClass('edit').data('id', order[i].OrderID).data('patid', order[i].PatID).data('hospid', order[i].HospID)));
                tmprow.append($('<td>'));

                if (order[i].IsEmergency == 1)
                    tmprow.append($('<td>').append('緊急').addClass('alert-data'));
                else
                    tmprow.append($('<td>'));

                tmprow.append($('<td>').append('削除').addClass('nomal-data'));
            }
            else {
                continue;
            }

            var hcd = "";

            for (var j = 0; j < hosp.length; j++) {
                if (hosp[j].HospID == order[i].HospID)
                {
                    hcd = hosp[j].CD;
                    break;
                }
            }

            tmprow.append($('<td>').append(hcd).addClass('nomal-data'));
            tmprow.append($('<td>').append(order[i].PatID).addClass('nomal-data'));
            tmprow.append($('<td>').append(order[i].PatName).addClass('nomal-data'));
            tmprow.append($('<td>').append(order[i].PatName_H).addClass('nomal-data'));
            tmprow.append($('<td>').append(order[i].Modality).addClass('nomal-data').addClass('Color_' + order[i].Modality));
            tmprow.append($('<td>').append(order[i].Date).addClass('nomal-data'));
            if (order[i].BodyPart.length > 5)
                tmprow.append($('<td>').append(order[i].BodyPart.substring(0, 5) + "...").addClass('nomal-data').attr('title', order[i].BodyPart));
            else
                tmprow.append($('<td>').append(order[i].BodyPart).addClass('nomal-data'));

            bodyRow.append(tmprow);

            $('#study_table').append(bodyRow);
        }

        $('input.edit').on('click', function () {
            sessionStorage.setItem('userid', _util.userid);
            sessionStorage.setItem('hospid', $(this).data('hospid'));
            sessionStorage.setItem('orderid', $(this).data('id'));
            sessionStorage.setItem('patid', $(this).data('patid'));

            postForm('./Order.aspx');
        });
        $('input.view').on('click', function () {
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

        if ($('#status').val() == '4'
            && $('#hosp').val() == ''
            && $('#patid').val() == ''
            && $('#modality').val() == ''
            && $('#date_f').val() == ''
            && $('#date_t').val() == '')
            $('#main-form').slideUp();

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

function C_GetHospList(func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetHospList",
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

function C_WebGetOrderList(val, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetOrderList",
        data: "{values:" + castJson(val) + "}",
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

function C_WebGetOrderCSV() {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetOrderCSV",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            alert('取込を開始しました。\n時間をおいて一覧を更新してください。');
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
            CloseLoading();
        }
    });
}

function C_GetHospTempList(func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetHospTempList",
        data: "{ id:" + _util.hospid + "}",
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

function postForm(url) {
    var param = "";


    param += $('#hosp').val();
    param += ',';
    param += $('#status').val();
    param += ',';
    param += $('#patid').val();
    param += ',';
    param += $('#modality').val();
    param += ',';
    param += $('#date_f').val();
    param += ',';
    param += $('#date_t').val();
    sessionStorage.setItem('param', param);


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