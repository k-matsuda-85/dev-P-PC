var id = "";

$(function () {
    id = sessionStorage.getItem('pastid');

    C_GetHospList(setHospList);

    Search = [];

    Search[Search.length] = '';
    Search[Search.length] = -2;
    Search[Search.length] = id;
    Search[Search.length] = '';
    Search[Search.length] = '';
    Search[Search.length] = '';
    Search[Search.length] = '';

    C_WebGetOrderList(Search, setOrderData);

    $('#send').on('click', function (e) {
        var val = $('#send').data('value');
        var status = $('#status').data('value');

        C_GetImage(status, id, 0, val, viewThumb);
    });
});

function viewThumb(ret) {
    if (!ret || !ret.d) {
        alert('依頼票が見つかりません。');

        return;
    }

    var url = ret.d;
    window.open(url, "依頼票", "width=680,height=800,left=1280,resizable=1");
}

function setHospList(ret) {
    if (!ret || !ret.d) {
        return;
    }

    if (ret.d.Result) {
        var hosp = ret.d.Items;

        for (var i = 0; i < hosp.length; i++) {
            if (hosp[i].Visible == 0)
                $('#hosp').append($('<option>').val(hosp[i].HospID).append(hosp[i].CD + " " + hosp[i].Name));
        }

    }
}

function setOrderData(ret) {
    if (!ret || !ret.d) {
        return;
    }

    if (ret.d.Result) {
        var order = ret.d.Items;

        for (var i = 0; i < order.length; i++) {
            $('#status').data('value', order[i].Status);
            switch (order[i].Status) {
                case -1:
                    $('#status').val('未入力');
                    break;
                case 0:
                    $('#status').val('一時保存');
                    break;
                case 1:
                    $('#status').val('登録');
                    break;
                case 2:
                    $('#status').val('出力');
                    break;
                case 3:
                    $('#status').val('削除');
                    break;
            }

            $('#orderno').val(order[i].OrderNo);
            $('#hosp').val(order[i].HospID);
            $('#patid').val(order[i].PatID);
            $('#patname').val(order[i].PatName);
            $('#patname_h').val(order[i].PatName_H);
            $('#birthday').val(formatDate(order[i].BirthDay));
            $('#modality').val(order[i].Modality);
            $('#studydate').val(formatDate(order[i].Date));
            $('#studytime').val(formatTime(order[i].Time));
            $('#isvisit').val(order[i].IsVisit);
            $('#department').val(order[i].Department);
            $('#doctor').val(order[i].Doctor);
            $('#age').val(order[i].PatAge);
            switch (order[i].Sex) {
                case 0:
                    $('#sex').val('不明');
                    break;
                case 1:
                    $('#sex').val('男');
                    break;
                case 2:
                    $('#sex').val('女');
                    break;
            }

            $('#bodypart').val(order[i].BodyPart)
            $('#studytype').val(order[i].Type);
            $('#comment').val(order[i].Comment);
            $('#contact').val(order[i].Contact);
            $('#recept').val(order[i].Recept);

            if (order[i].IsEmergency)
                $('#isemergency')[0].checked = true;

            //if (order[i].IsMail) {
            //    $('#ismail')[0].checked = true;
            //}

            $('#send').css('visibility', 'hidden');
            C_WebGetFileList(setOrderFiles);
            C_GetSender($('#orderno').val(), setSender);
        }

    }
}
function setSender(ret) {
    if (!ret || !ret.d) {
        return;
    }

    if (ret.d == "") {
        return;
    }

    var vals = ret.d.split(',');

    if (vals.length < 6) {
        return;
    }

    if (vals[0] == "0")
        $('#isfax_own')[0].checked = true;
    else if (vals[0] == "1")
        $('#ismail_own')[0].checked = true;
    else if (vals[0] == "2") {
        $('#isfax_own')[0].checked = true;
        $('#ismail_own')[0].checked = true;
    }

    if (vals[1] == "0") {
        $('#isfax')[0].checked = true;
        $('#ismail')[0].checked = false;
        $('#ihosp').val(vals[3]).attr('title', vals[3]);
        $('#ihosp_number').val(vals[5]);
        $('#ihosp').removeAttr('disabled');
        $('#ihosp_number').removeAttr('disabled');

        var cd = $('#hosp option:selected').text();
        var list = cd.split(' ');

        C_GetFaxSender(list[0], setFaxSenList);
    }
    else if (vals[1] == "1") {
        $('#ismail')[0].checked = true;
        $('#isfax')[0].checked = false;
        $('#ihosp').val(vals[3]).attr('title', vals[3]);
        $('#ihosp').removeAttr('disabled');

        var cd = $('#hosp option:selected').text();
        var list = cd.split(' ');

        C_GetMailSender(list[0], setMailSenList);
    }

}
function setOrderFiles(ret) {
    if (!ret || !ret.d) {
        return;
    }

    var Files = ret.d.Items;

    if (Files.length > 0) {
        for (var i = 0; i < Files.length; i++) {
            if (Files[i].IsOrigin == 0) {
                var val = Files[i].Name.replace('C_', '');
                $('#send').data('value', val);
                $('#send').css('visibility', 'visible');
            }
        }
    }
}
function formatTime(str) {
    var ret = '';

    if (!str || str == '')
        return ret;

    if (str.length == 6)
        ret = str.substring(0, 2) + ':' + str.substring(2, 4) + ':' + str.substring(4, 6);
    else if (str.length == 8) {
        var list = str.split(':');
        ret = ('0' + list[0]).slice(-2) + ('0' + list[1]).slice(-2) + ('0' + list[2]).slice(-2);
    }
    else
        ret = str;

    return ret;
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
function C_GetImage(status, orderid, isOrigin, val, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetImage",
        data: "{ status:" + status + ", orderid:" + orderid + ", userid:0, isOrigin:" + isOrigin + ", value:'" + val + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            func(result);
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
        }
    });
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
        }
    });
}
function C_GetSender(orderno, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetSender",
        data: "{ orderNo:'" + orderno + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            func(result);
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
        }
    });
}
function C_WebGetFileList(func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetFileList",
        data: "{ orderid:" + id + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            func(result);
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
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