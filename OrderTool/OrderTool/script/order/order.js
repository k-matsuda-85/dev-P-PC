var patient;
var mainorder;
var past = [];
var pastids = [];
var isView = 0;

// ロード処理
$(function () {

    // セッション確認
    if (!_util.key || _util.key == '') {
        _util.userid = sessionStorage.getItem('userid');
        _util.key = sessionStorage.getItem('key');
        _util.orderid = sessionStorage.getItem('orderid');
        _util.hospid = sessionStorage.getItem('hospid');
        _util.patid = sessionStorage.getItem('patid');
        isView = sessionStorage.getItem('view');

        //sessionStorage.clear();
    }

    if (!_util.key || _util.key == '') {
        postForm('./Login.aspx');
        return;
    }

    // ロード画面表示
    ShowLoading();

    // 閲覧モードの設定
    if (isView == 1)
    {
        $('#patid').attr('disabled', 'disabled');
        $('#isemergency').attr('disabled', 'disabled');
        $('#ismail').attr('disabled', 'disabled');
        $('#istemp').attr('disabled', 'disabled');
        $('#patname').attr('disabled', 'disabled');
        $('#patname_h').attr('disabled', 'disabled');
        $('#sex').attr('disabled', 'disabled');
        $('#birthday').attr('disabled', 'disabled');
        $('#age').attr('disabled', 'disabled');
        $('#studydate').attr('disabled', 'disabled');
        $('#studytime').attr('disabled', 'readonly');
        $('#modality').attr('disabled', 'disabled');
        $('#imgcnt').attr('disabled', 'disabled');
        $('#studytype').attr('disabled', 'disabled');
        $('#bodypart').attr('disabled', 'disabled');
        $('#comment').attr('disabled', 'disabled');
        $('#past').attr('disabled', 'disabled');

        $('#clear').css('visibility', 'hidden');
        $('#cancel').css('visibility', 'hidden');
        $('#send').css('visibility', 'hidden');
        $('#edit-order').css('visibility', 'visible');
    } else {
    // 編集モードの設定（各項目の動作を設定）
        $.fn.autoKana('#patname', '#patname_h', {
            katakana: true  //true：カタカナ、false：ひらがな（デフォルト）
        });

        $('#birthday, #studydate').on('change', function (e) {
            setAge();
        });

        $('#patid').on('change', function (e) {
            changeID($('#patid').val());
        });
        $('#studytime').on('change', function (e) {
            if ($('#studytime').val().length == 6)
                $('#studytime').val(formatTime($('#studytime').val()));
        });

        $('#past').on('click', function (e) {
            setPastOrder();
            $('#modal').modal();
            CloseLoading();
        });

        $('#ismail').on('change', function (e) {
            if ($('#ismail')[0].checked) {
                $('#mail_1').removeAttr('disabled');
                $('#mail_2').removeAttr('disabled');
            } else {
                $('#mail_1').attr('disabled', 'disabled');
                $('#mail_2').attr('disabled', 'disabled');
            }
        });
    }

    $('#edit-order').on('click', function (e) {
        var value = [];

        value[value.length] = _util.hospid;
        value[value.length] = _util.orderid;
        value[value.length] = $('#patid').val();
        value[value.length] = $('#patname').val();
        value[value.length] = $('#patname_h').val();
        value[value.length] = $('#modality').val();
        value[value.length] = formatDate($('#studydate').val());

        C_SetEditOrder(value, writeOrder);
    });

    $('#clear').on('click', function (e) {
        if (!confirm('入力をクリアしますが、よろしいですか？'))
            return;

        ShowLoading();

        $('#patid').val('');
        $('#isemergency')[0].checked = false;
        $('#ismail')[0].checked = false;
        $('#istemp')[0].checked = false;
        $('#patname').val('');
        $('#patname_h').val('');
        $('#sex').val('男');
        $('#birthday').val('');
        $('#age').val('');
        $('#studydate').val('');
        $('#studytime').val('');
        $('#modality').val('');
        $('#studytype').val('');
        $('#imgcnt').val('');
        $('#bodypart').val('');
        $('#comment').val('');

        past = [];
        pastids = [];

        $('#pastmsg').empty();

        if (_util.orderid && _util.orderid != '') {
            C_WebGetPreList(setOrderData);
            setAge();
        }

        CloseLoading();
    });

    $('#cancel').on('click', function (e) {
        if (!confirm('入力をキャンセルしますが、よろしいですか？'))
            return;

        sessionStorage.setItem('userid', _util.userid);
        sessionStorage.setItem('key', _util.key);

        postForm('./Search.aspx');
    });


    $('#set_change').on('click', function (e) {
        pastids = [];

        $('#study_table tbody tr td input').each(function () {
            if (this.checked)
                pastids[pastids.length] = $(this).data('id');
        });

        if (pastids.length == 0)
            $('#pastmsg').css('visibility', 'hidden');
        else {
            $('#pastmsg').css('visibility', 'visible');
            var msg = '';
            var title = '';
            var hitCnt = 0
            for (var i = 0; i < past.length; i++) {
                if ($.inArray(past[i].OrderID, pastids) >= 0) {
                    if (hitCnt == 2) {
                        msg += '..';
                    }
                    if (hitCnt != 0) {
                        if (hitCnt < 2)
                            msg += ',';
                        title += '、';
                    }
                    if (hitCnt < 2)
                        msg += formatDate(past[i].Date).substring(5, 10) + ' ' + past[i].Modality;
                    title += formatDate(past[i].Date).substring(5, 10) + ' ' + past[i].Modality;
                    hitCnt++;
                }
            }
            $('#pastmsg').empty();
            $('#pastmsg').append(msg);
            $('#pastmsg').attr('title', title);
        }

        $('#modal').modal('hide');
    });

    $('#toList').on('click', function (e) {
        sessionStorage.setItem('userid', _util.userid);
        sessionStorage.setItem('key', _util.key);

        postForm('./Search.aspx');
    });

    $('#send').on('click', function (e) {
        var isMust = true;
        if ($('#patid').val() == '')
        {
            $('#patid').addClass('error-val');
            isMust = false;
        } else {
            $('#patid').removeClass('error-val');
        }
        if ($('#patname').val() == '') {
            $('#patname').addClass('error-val');
            isMust = false;
        } else {
            $('#patname').removeClass('error-val');
        }
        if ($('#patname_h').val() == '') {
            $('#patname_h').addClass('error-val');
            isMust = false;
        } else {
            $('#patname_h').removeClass('error-val');
        }
        if ($('#sex').val() == '') {
            $('#sex').addClass('error-val');
            isMust = false;
        } else {
            $('#sex').removeClass('error-val');
        }
        if ($('#birthday').val() == '') {
            $('#birthday').addClass('error-val');
            isMust = false;
        } else {
            $('#birthday').removeClass('error-val');
        }
        if ($('#studydate').val() == '') {
            $('#studydate').addClass('error-val');
            isMust = false;
        } else {
            $('#studydate').removeClass('error-val');
        }
        if ($('#studytime').val() == '') {
            $('#studytime').addClass('error-val');
            isMust = false;
        } else {
            $('#studytime').removeClass('error-val');
        }
        if ($('#modality').val() == '') {
            $('#modality').addClass('error-val');
            isMust = false;
        } else {
            $('#modality').removeClass('error-val');
        }
        if ($('#imgcnt').val() == '') {
            $('#imgcnt').addClass('error-val');
            isMust = false;
        } else {
            $('#imgcnt').removeClass('error-val');
        }
        if ($('#bodypart').val() == '') {
            $('#bodypart').addClass('error-val');
            isMust = false;
        } else {
            $('#bodypart').removeClass('error-val');
        }
        if ($('#comment').val() == '' && !$('#istemp')[0].checked) {
            $('#comment').addClass('error-val');
            isMust = false;
        } else {
            $('#comment').removeClass('error-val');
        }

        if ($('#ismail')[0].checked) {
            if ($('#mail_1').val() == '') {
                $('#mail_1').addClass('error-val');
                isMust = false;
            } else {
                $('#mail_1').removeClass('error-val');
            }
            if ($('#mail_2').val() == '') {
                $('#mail_2').addClass('error-val');
                isMust = false;
            } else {
                $('#mail_2').removeClass('error-val');
            }
        }

        if (!isMust) {
            alert('必須項目を入力してください。');
            return;
        }

        var bodypart = $('#bodypart').val();
        var comment = $('#comment').val();
        var len = getBytes(bodypart);

        if(len > 900)
        {
            alert('検査部位は、450文字以内で入力してください。\n現在：約' + (len / 2) + '文字');
            $('#bodypart').focus();
            return;
        }
        len = getBytes(comment);

        if (len > 900) {
            alert('依頼内容は、450文字以内で入力してください。\n現在：約' + (len / 2) + '文字');
            $('#comment').focus();
            return;
        }

        if (!confirm("入力した内容で読影依頼を登録します。\nよろしいですか？"))
            return;

        var value = [];

        value[value.length] = _util.hospid;
        if (_util.orderid != '')
            value[value.length] = _util.orderid;
        else
            value[value.length] = '0';

        value[value.length] = $('#patid').val();
        value[value.length] = $('#patname').val();
        value[value.length] = $('#patname_h').val();
        value[value.length] = $('#age').val();
        value[value.length] = formatDate($('#birthday').val());
        value[value.length] = $('#modality').val();
        value[value.length] = formatDate($('#studydate').val());
        value[value.length] = formatTime($('#studytime').val());
        value[value.length] = $('#bodypart').val();
        value[value.length] = $('#studytype').val();
        value[value.length] = $('#imgcnt').val();

        if ($('#isemergency')[0].checked)
            value[value.length] = '1';
        else
            value[value.length] = '0';

        if ($('#ismail')[0].checked)
            value[value.length] = '1';
        else
            value[value.length] = '0';

        value[value.length] = $('#comment').val();

        switch($('#sex').val())
        {
            case '男':
                value[value.length] = '1';
                break;
            case '女':
                value[value.length] = '2';
                break;
            case '不明':
                value[value.length] = '0';
                break;
        }

        if ($('#istemp')[0].checked)
            value[value.length] = '1';
        else
            value[value.length] = '';

        if ($('#ismail')[0].checked)
        {
            value[value.length] = $('#mail_1').val();
            value[value.length] = $('#mail_2').val();
        } else {
            value[value.length] = "";
            value[value.length] = "";
        }
        for (var i = 0; i < pastids.length; i++)
            value[value.length] = pastids[i];

        C_SetPreOrder(value, endOrder);
    });

    C_GetHospConfig(setHospConfig);
    if (_util.orderid && _util.orderid != '') {
        C_WebGetPreList(setOrderData);
    }

    if (isView != 1)
    {
        $('.datepicker').each(function () {
            $(this).datepicker({
                language: 'ja'
            });
        });
    }

    C_GetPatList(setPatList);


});

function writeOrder(ret) {
    if (!ret || !ret.d) {
        alert("通知に失敗しました。");
        return;
    }


    alert('登録内容の変更依頼を通知いたしました。\n確認の電話をさせていただきます。少々お待ちください。');

    sessionStorage.setItem('userid', _util.userid);
    sessionStorage.setItem('key', _util.key);

    postForm('./Search.aspx');
}

function endOrder(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        alert("登録に失敗しました。");
        return;
    }

    if (ret.d.Result) {
        alert("登録完了しました。");
        sessionStorage.setItem('userid', _util.userid);
        sessionStorage.setItem('key', _util.key);
        sessionStorage.setItem('orderid', _util.orderid);

        postForm('./Search.aspx');
    } else {
        alert("登録に失敗しました。");
        alert(ret.d.Message);
    }
}

function setPatList(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    if (ret.d.Result) {
        patient = ret.d.Items;
        var datList = [];

        for (var i = 0; i < patient.length; i++) {
            datList[datList.length] = patient[i].PatID;
        }

        var engine = new Bloodhound({
            datumTokenizer: Bloodhound.tokenizers.whitespace,
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            local: datList
        });

        engine.initialize();

        $('#patid').typeahead(null, {
            name: 'stations',
            source: engine.ttAdapter()
        });
    }

    CloseLoading();

}

function setHospConfig(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    if (ret.d.Result) {
        var conf = ret.d.Items;

        var modList = [];

        for (var i = 0; i < conf.Conf.length; i++)
            if (conf.Conf[i].Key == 'Modality')
            {
                modList = conf.Conf[i].Value.split(',');
                break;
            }

        for(var i = 0; i < modList.length; i++)
            $('#modality').append($('<option>').append(modList[i]));

    }
    CloseLoading();
}

function setOrderData(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    if (ret.d.Result) {
        var order = ret.d.Items;

        for (var i = 0; i < order.length; i++)
            if (order[i].OrderID == _util.orderid) {
                $('#patid').val(order[i].PatID);
                $('#patname').val(order[i].PatName);
                $('#patname_h').val(order[i].PatName_H);
                $('#birthday').val(formatDate(order[i].BirthDay));
                $('#modality').val(order[i].Modality);
                $('#studydate').val(formatDate(order[i].Date));
                $('#studytime').val(formatTime(order[i].Time));
                $('#imgcnt').val(order[i].ImgCnt);
                $('#age').val(order[i].PatAge);
                switch (order[i].Sex)
                {
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

                if (order[i].IsEmergency)
                    $('#isemergency')[0].checked = true;

                if (order[i].IsMail) {
                    $('#ismail')[0].checked = true;
                    if (isView != 1) {
                        $('#mail_1').removeAttr('disabled');
                        $('#mail_2').removeAttr('disabled');
                    }
                }


                mainorder = order[i];
            }
            else
                past[past.length] = order[i];
    }
}

function setPastOrder() {
    $('#study_table tbody').remove();

    if (_util.orderid == '' && past.length == 0)
    {
        _util.patid = $('#patid').val();
        ShowLoading();
        C_WebGetPreList(setOrderData);
    }
    var bodyRow = $('<tbody>');
    for (var i = 0; i < past.length; i++) {
        var tmprow = $('<tr>');
        if($.inArray(past[i].OrderID, pastids) >= 0)
            tmprow.append($('<td>').addClass('dbody').append($('<input>').attr('type', 'checkbox').attr('checked', 'checked').data('id', past[i].OrderID)));
        else
            tmprow.append($('<td>').addClass('dbody').append($('<input>').attr('type', 'checkbox').data('id', past[i].OrderID)));
        tmprow.append($('<td>').append(past[i].Modality));
        tmprow.append($('<td>').append(past[i].Date));
        tmprow.append($('<td>').append(past[i].ImgCnt));
        bodyRow.append(tmprow);
    }

    $('#study_table').append(bodyRow);
}


function formatDate(str)
{
    var ret = '';

    if(!str || str == '')
        return;

    if(str.length == 8)
        ret = str.substring(0, 4) + '/' + str.substring(4, 6) + '/' + str.substring(6, 8);
    else if(str.length == 10){
        var list = str.split('/');
        ret = list[0] + ('0' + list[1]).slice(-2) + ('0' + list[2]).slice(-2);
    }
    else
        ret = str;

    return ret;
}

function formatTime(str)
{
    var ret = '';

    if(!str || str == '')
        return;

    if(str.length == 6)
        ret = str.substring(0, 2) + ':' + str.substring(2, 4) + ':' + str.substring(4, 6);
    else if(str.length == 8)
    {
        var list = str.split(':');
        ret = ('0' + list[0]).slice(-2) + ('0' + list[1]).slice(-2) + ('0' + list[2]).slice(-2);
    }
    else
        ret = str;

    return ret;
}

function getBytes(strSrc){
    var len = 0;
    strSrc = escape(strSrc);
    for(i = 0; i < strSrc.length; i++, len++){
        if(strSrc.charAt(i) == "%"){
            if(strSrc.charAt(++i) == "u"){
                i += 3;
                len++;
            }
            i++;
        }
    }
    return len;
}

function changeID(id)
{
    for(var i = 0; i < patient.length; i++)
    {
        if(patient[i].PatID == id)
        {
            $('#patname').val(patient[i].PatName);
            $('#patname_h').val(patient[i].PatName_H);
            switch (patient[i].Sex) {
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
            $('#birthday').val(formatDate(patient[i].BirthDay));
        }
    }
}

function setAge()
{
    if (!$('#birthday').val() || $('#birthday').val() == '')
        return;
    if (!$('#studydate').val() || $('#studydate').val() == '')
        return;

    var data = $('#birthday').val().split("/");
    var data2 = $('#studydate').val().split("/");

    var y = parseInt(data[0], 10);
    var m = parseInt(data[1], 10);
    var d = parseInt(data[2], 10);
    var y2 = parseInt(data2[0], 10);
    var m2 = parseInt(data2[1], 10);
    var d2 = parseInt(data2[2], 10);

    myNow = new Date(y2, m2 - 1, d2);
    myBirth = new Date(1970, 0, d);
    myBirth.setTime(myNow.getTime() - myBirth.getTime());

    myYear = myBirth.getUTCFullYear() - y;
    myMonth = myBirth.getUTCMonth() - (m - 1);

    if (myMonth < 0) {
        myYear--;
    }

    $('#age').val(myYear);

}

function C_GetPatList(func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetPatList",
        data: "{ hospid:" + _util.hospid + "}",
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

function C_GetHospConfig(func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetHospConf",
        data: "{ hospid:" + _util.hospid + "}",
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

function C_WebGetPreList(func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetPreList",
        data: "{ hospid:" + _util.hospid + ", patid:'" + _util.patid + "'}",
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

function C_SetPreOrder(val, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/SetPreOrder",
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

function C_SetEditOrder(val, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/SetEditOrder",
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


function castJson(list)
{
    var str = '[';

    for(var i = 0; i < list.length; i++)
    {
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

function getBytes(strSrc) {
    var len = 0;
    strSrc = escape(strSrc);
    for (i = 0; i < strSrc.length; i++, len++) {
        if (strSrc.charAt(i) == "%") {
            if (strSrc.charAt(++i) == "u") {
                i += 3;
                len++;
            }
            i++;
        }
    }
    return len;
}

function ShowLoading() {
    var h = $(window).height();

    $('#loader-bg ,#loader').height(h).css('display', 'block');
}

function CloseLoading() {
    $('#loader-bg').delay(900).fadeOut(800);
    $('#loader').delay(600).fadeOut(300);
}