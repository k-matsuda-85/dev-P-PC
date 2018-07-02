var patient;
var mainorder;
var past = [];
var pastids = [];
var isView = 0;

var caret;

var sKey = "";
var sAsc = "DESC"

var isDrag = false;
var isDrag_SOP = false;
var mod_X = 0;
var mod_Y = 0;

// ロード処理
$(function () {

    // セッション確認
    if (!_util.orderid || _util.orderid == '') {
        _util.orderid = sessionStorage.getItem('orderid');
        _util.uid = sessionStorage.getItem('uid');
        _util.search = sessionStorage.getItem('search');
        //sessionStorage.clear();
    }
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

    $('#scan-input').on('click', function (e) {
        ShowLoading();

        C_SetGetScan(order.OrderNo, order.PatID, order.Date, order.Modality, setScanImage);
    });

    $('#add-val-set').on('click', function (e) {
        setNewTemp($('#add-val').val());
        $('#add-val').val('');
    });
    $('#scan-images').on('mousedown', function (e) {
        //var child = $(this).find('a');
        //if (child && child.length > 0)
        //    return;

        isDrag = true;
        mod_X = e.pageX;
        mod_Y = e.pageY;
        e.preventDefault();
        $('#scan-images').css('cursor', 'move');
    });
    $(document).on('mouseup', function (e) {
        if (isDrag) {
            e.preventDefault();
            $('#scan-images').css('cursor', 'auto');

            isDrag = false;
        }
    });
    $(document).on('mousemove', function (e) {
        if (!isDrag)
            return;

        var move_X = mod_X - e.pageX;
        var move_Y = mod_Y - e.pageY;

        $("#scan-images").find('div.active').each(function (i, elem) {
            $(this).find('img').each(function () {
                var offset = $(this).offset();

                if ($(this).width() > $("#scan-images").width())
                {
                    offset.left = offset.left - move_X;
                }

                if ($(this).height() > 570) {
                    offset.top = offset.top - move_Y;
                }
                mod_X = e.pageX;
                mod_Y = e.pageY;

                $(this).offset({ top: offset.top, left: offset.left });
            });
        });
    });
    var mousewheelevent = 'onwheel' in document ? 'wheel' : 'onmousewheel' in document ? 'mousewheel' : 'DOMMouseScroll';
    $('#scan-images').on(mousewheelevent, function (e) {
        e.preventDefault();
        var delta = e.originalEvent.deltaY ? -(e.originalEvent.deltaY) : e.originalEvent.wheelDelta ? e.originalEvent.wheelDelta : -(e.originalEvent.detail);

        $("#scan-images").find('div.active').each(function (i, elem) {
            $(this).find('img').each(function () {
                if (delta < 0) {
                    // マウスホイールを下にスクロールしたときの処理を記載
                    var ch = $(this).height() + delta;
                    if (ch < 570)
                        ch = 570;

                    if (ch == 570){
                        $(this).css('top', '0px');
                        $(this).css('left', '0px');
                    }
                    else {
                        var offset = $(this).offset();

                        if ($(this).width() > $("#scan-images").width()) {
                           offset.left = offset.left  - (delta / 2);
                        }

                        $(this).offset({ top: offset.top - (delta / 2), left: offset.left });
                    }

                    $(this).height(ch);
                } else {
                    // マウスホイールを上にスクロールしたときの処理を記載
                    var ch = $(this).height() + delta;
                    if (ch < 570)
                        ch = 570;

                    var offset = $(this).offset();

                    if ($(this).width() > $("#scan-images").width()) {
                        offset.left = offset.left - (delta / 2);
                    }

                    $(this).offset({ top: offset.top - (delta / 2), left: offset.left });
                    $(this).height(ch);
                }
            });
        });
    });

    $('#scan-del').on('click', function (e) {
        if (confirm('表示画像を削除しますが、よろしいですか？') == false)
            return;

        ShowLoading();
        var file = '';

        $('.item').each(function () {
            if($(this).hasClass('active'))
                file = $(this).data('file');
        });

        C_SetDelScan(file, order.OrderNo, order.PatID, order.Date, order.Modality, reSetScanImage);
    });

    $('#scan-all-del').on('click', function (e) {
        if (confirm('全ての画像を削除しますが、よろしいですか？') == false)
            return;

        ShowLoading();
        var file = '';

        C_SetDelScan(file, order.OrderNo, order.PatID, order.Date, order.Modality, reSetScanImage);
    });

    $('.carousel').carousel();
    $('textarea.listblock').on('click focus', function (e) {
        var id = $(this).attr('id');

        event.stopPropagation();

        $('ul.dropdown-menu').hide();
        $('#' + id + '-list').toggle();

        caret = document.getElementById(id).selectionStart;

        $('#' + id + '-list li a').focus();
        return false;
    });

    $('input.listblock').on('click focus', function (e) {
        var id = $(this).attr('id');

        event.stopPropagation();

        $('ul.dropdown-menu').hide();
        $('#' + id + '-list').toggle();

        caret = document.getElementById(id).selectionStart;

        $('#' + id + '-list li a').focus();
        return false;
    });

    $(document).click(function () {
        $('ul.dropdown-menu').hide();
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

            if (strOriginal != '' && !strOriginal)
                return;

            //カーソル位置より左の文字列
            var leftPart = strOriginal.substr(0, caret);
            //カーソル位置より右の文字列
            var rightPart = strOriginal.substr(caret, strOriginal.length);

            //文字列を結合して、テキストエリアに出力
            $('#' + key).val(leftPart + strInsert + rightPart);
        }
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

        switch (id) {
            case 's-mod':
                if (sAsc == 'DESC') {
                    past.sort(function (a, b) {
                        return a.Modality < b.Modality ? 1 : -1;
                    });
                    $('#s-mod').text("モダリティ▼");
                }
                else {
                    past.sort(function (a, b) {
                        return a.Modality > b.Modality ? 1 : -1;
                    });
                    $('#s-mod').text("モダリティ▲");
                }
                break;
            case 's-date':
                if (sAsc == 'DESC') {
                    past.sort(function (a, b) {
                        return (a.Date + a.Time) < (b.Date + b.Time) ? 1 : -1;
                    });
                    $('#s-date').text("検査日▼");
                }
                else {
                    past.sort(function (a, b) {
                        return (a.Date + a.Time) > (b.Date + b.Time) ? 1 : -1;
                    });
                    $('#s-date').text("検査日▲");
                }
                break;
            case 's-desc':
                if (sAsc == 'DESC') {
                    past.sort(function (a, b) {
                        return a.Desc < b.Desc ? 1 : -1;
                    });
                    $('#s-desc').text("検査部位▼");
                }
                else {
                    past.sort(function (a, b) {
                        return a.Desc > b.Desc ? 1 : -1;
                    });
                    $('#s-desc').text("検査部位▲");
                }
                break;
            case 's-cnt':
                if (sAsc == 'DESC') {
                    past.sort(function (a, b) {
                        return a.ImgCnt < b.ImgCnt ? 1 : -1;
                    });
                    $('#s-cnt').text("画像枚数▼");
                }
                else {
                    past.sort(function (a, b) {
                        return a.ImgCnt > b.ImgCnt ? 1 : -1;
                    });
                    $('#s-cnt').text("画像枚数▲");
                }
                break;
        }

        setPastList();
        return false;
    });


    $('#past').on('click', function (e) {
        setPastOrder();
    });

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

        sessionStorage.setItem('search', _util.search);
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
                if ($.inArray(past[i].StudyInstanceUID, pastids) >= 0) {
                    if (hitCnt == 3) {
                        msg += '..';
                    }
                    if (hitCnt != 0) {
                        if (hitCnt < 3)
                            msg += ',';
                        title += '、';
                    }
                    if (hitCnt < 3)
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

    $('#set_change_doc').on('click', function (e) {
        if (!confirm('候補値を更新します。\nよろしいですか？'))
            return;

        var settingVals = [];

        $('#setting_table').find('input:text').each(function () {
            if ($(this).val() != '')
                settingVals[settingVals.length] = $(this).val();
        });

        C_SetDoctor(settingVals, endSettings);
    });

    $('input:button.setting').on('click', function (e) {
        var key = $(this).attr('id').replace('-setting', '');
        var title = $('#' + key + '-label').text();

        viewModal(title + ' 変更', key);
        $('#modal-doc').modal();
    });

    function viewModal(title, key) {
        $('#modal-title').empty();
        $('#modal-title').append(title).data('key', key);

        $('#sorttable').remove();

        var table = $('<tbody>').attr('id', 'sorttable');

        if (Template && Template.length > 0)
            for (var i = 0; i < Template.length; i++) {
                table.append(
                    $('<tr>').append(
                        $('<td>').append(
                            i + 1
                        )
                    ).append(
                        $('<td>').append(
                            $('<input>').attr('type', 'text').val(Template[i])
                        )
                    ).append(
                        $('<td>').append(
                            $('<input>').addClass('btn-sm').addClass('del').attr('value', '削除').attr('type', 'button')
                        )
                    )
                );
            }

        $('#setting_table').append(table);

        $('#sorttable').sortable();
        $("#sorttable").disableSelection();
        $('input.del').off('click');
        $('input.del').on('click', function (e) {
            if (!confirm('この候補値を削除します。\nよろしいですか？'))
                return;

            var delObj = $(this).parent().parent();

            delObj.remove();
        });
    }


    $('#toList').on('click', function (e) {
        sessionStorage.setItem('userid', _util.userid);
        sessionStorage.setItem('key', _util.key);

        sessionStorage.setItem('search', _util.search);
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
        //if ($('#comment').val() == '' && !$('#istemp')[0].checked) {
        //    $('#comment').addClass('error-val');
        //    isMust = false;
        //} else {
        //    $('#comment').removeClass('error-val');
        //}

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

        if (order.Status > 1) {
            if (!confirm("入力した内容で読影依頼情報の変更依頼をします。\nよろしいですか？"))
                return;
        } else {
            if (!confirm("入力した内容で読影依頼を登録します。\nよろしいですか？"))
                return;
        }


        var value = [];

        value[value.length] = _util.uid;
        value[value.length] = _util.orderid;

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

        value[value.length] = $('#department').val();
        value[value.length] = $('#doctor').val();

        var msg = "";

        for (var i = 0; i < past.length; i++) {
            if ($.inArray(past[i].StudyInstanceUID, pastids) >= 0) {
                if (msg != '')
                    msg += '、';

                if (past[i].Modality != order.Modality)
                    msg += formatDate(past[i].Date).substring(5, 10) + ' ' + past[i].Modality + ' ' + past[i].ImgCnt;
                else
                    msg += formatDate(past[i].Date).substring(5, 10) + ' ' + past[i].ImgCnt;
            }
        }

        value[value.length] = msg;

        for (var i = 0; i < pastids.length; i++)
            value[value.length] = pastids[i];

        C_SetPreOrder(value, endOrder);
    });

    if ((_util.orderid && _util.orderid != '') || (_util.uid && _util.uid != '')) {
        C_WebGetPreOrder(_util.orderid, _util.uid, setOrder);
    }

    if (isView != 1)
    {
        $('.datepicker').each(function () {
            $(this).datepicker({
                language: 'ja'
            });
        });
    }

    C_GetDoctor(setSettings);
});

function writeOrder(ret) {
    if (!ret || !ret.d) {
        alert("通知に失敗しました。");
        return;
    }


    alert('登録内容の変更依頼を通知いたしました。\n確認の電話をさせていただきます。少々お待ちください。');

    sessionStorage.setItem('userid', _util.userid);
    sessionStorage.setItem('key', _util.key);
    sessionStorage.setItem('search', _util.search);

    postForm('./Search.aspx');
}

function endOrder(ret) {
    var text = "登録";

    if (order.Status > 1)
        text = "変更依頼";

    if (!ret || !ret.d) {
        CloseLoading();
        alert(text + "に失敗しました。");
        return;
    }

    if (ret.d.Result) {
        alert(text + "完了しました。");
        sessionStorage.setItem('userid', _util.userid);
        sessionStorage.setItem('key', _util.key);
        sessionStorage.setItem('orderid', _util.orderid);
        sessionStorage.setItem('search', _util.search);

        postForm('./Search.aspx');
    } else {
        alert(text + "に失敗しました。");
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

var order = {};

function setOrder(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    if (ret.d.Result) {
        order = ret.d.Items;

        if (!_util.orderid || _util.orderid == '')
            _util.orderid = order.OrderNo;

        $('#patid').val(order.PatID);
        $('#patname').val(order.PatName);
        $('#patname_h').val(order.PatName_H);
        $('#birthday').val(formatDate(order.BirthDay));
        $('#modality').val(order.Modality);
        $('#studydate').val(formatDate(order.Date));
        $('#studytime').val(formatTime(order.Time));
        $('#imgcnt').val(order.ImgCnt);
        $('#age').val(order.PatAge);
        switch (order.Sex) {
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

        $('#bodypart').val(order.BodyPart)
        $('#studytype').val(order.Type);
        $('#comment').val(order.Comment);
        $('#department').val(order.Department);
        $('#doctor').val(order.Doctor);

        if (order.Status > 1)
        {
            $('#send').text("変更依頼");
        }

        if (order.IsEmergency)
            $('#isemergency')[0].checked = true;

        if (order.PastCnt && order.PastCnt != '' && order.PastCnt != 'none') {
            var dat = order.PastCnt.split('、');
            var msg = "";
            var title = "";

            var hitCnt = 0

            for (var i = 0; i < dat.length; i++) {
                var vals = dat[i].split(' ');

                if (hitCnt == 3) {
                    msg += '..';
                }
                if (hitCnt != 0) {
                    if (hitCnt < 3)
                        msg += ',';
                    title += '、';
                }

                if (vals.length < 3) {
                    if (hitCnt < 3)
                        msg += vals[0] + ' ' + order.Modality;
                    title += vals[0] + ' ' + order.Modality;
                } else {
                    if (hitCnt < 3)
                        msg += vals[0] + ' ' + vals[1];
                    title += vals[0] + ' ' + vals[1];
                }
                hitCnt++;
            }

            $('#pastmsg').empty();
            $('#pastmsg').append(msg);
            $('#pastmsg').attr('title', title);
            $('#pastmsg').css('visibility', 'visible');

        }

        C_SetGetScan_First(order.OrderNo, order.PatID, order.Date, order.Modality, setScanImage);
        C_GetComboList(0, order.Modality, setBodyPart);
    }
}

function setBodyPart(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    $('#bodypart-list').empty();
    $list = $('<li>');

    for (var i = 0; i < ret.d.length; i++)
    {
        $list.append($('<a>').attr('data-key', 'bodypart').append(ret.d[i]));
    }
    $('#bodypart-list').append($list);
}

function setOrderData(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    if (ret.d.Result) {
        var tmporder = ret.d.Items;

        for (var i = 0; i < tmporder.length; i++)
            if (tmporder[i].StudyInstanceUID != _util.uid)
                past[past.length] = tmporder[i];


        var bodyRow = $('<tbody>');
        for (var i = 0; i < past.length; i++) {
            var tmprow = $('<tr>');
            if (modCheck(past[i].Modality, past[i].Date, past[i].ImgCnt))
                tmprow.append($('<td>').addClass('dbody').append($('<input>').attr('type', 'checkbox').attr('checked', 'checked').data('id', past[i].StudyInstanceUID)));
            else
                tmprow.append($('<td>').addClass('dbody').append($('<input>').attr('type', 'checkbox').data('id', past[i].StudyInstanceUID)));

            tmprow.append($('<td>').append(past[i].Modality));
            tmprow.append($('<td>').append(past[i].Desc));
            tmprow.append($('<td>').append(past[i].Date));
            tmprow.append($('<td>').append(past[i].ImgCnt));
            bodyRow.append(tmprow);
        }

        $('#study_table').append(bodyRow);
        $('#modal').modal();
        CloseLoading();
    }
}

function modCheck(mod, date, cnt)
{
    var ret = false;

    if (order.PastCnt && order.PastCnt != '' && order.PastCnt != 'none')
    {
        var dat = order.PastCnt.split('、');

        for(var i = 0; i < dat.length; i++)
        {
            var vals = dat[i].split(' ');
            if(vals.length < 3){
                if (formatDate(past[i].Date).substring(5, 10) == vals[0]
                    && cnt == vals[1]
                    && mod == order.Modality)
                    ret = true;
            }
            else {
                if (formatDate(past[i].Date).substring(5, 10) == vals[0]
                    && cnt == vals[2]
                    && mod == vals[1])
                    ret = true;
            }
        }
    }

    return ret;
}

function setPastOrder() {
    if(past.length == 0){
        ShowLoading();
        C_WebGetPreList($('#patid').val(), "", "", setOrderData);
    }
    else {
        setPastList();
        $('#modal').modal();
        CloseLoading();
    }
}

function setPastList() {
    $('#study_table tbody').remove();

    var bodyRow = $('<tbody>');
    for (var i = 0; i < past.length; i++) {
        var tmprow = $('<tr>');
        if ($.inArray(past[i].OrderID, pastids) >= 0)
            tmprow.append($('<td>').addClass('dbody').append($('<input>').attr('type', 'checkbox').attr('checked', 'checked').data('id', past[i].StudyInstanceUID)));
        else
            tmprow.append($('<td>').addClass('dbody').append($('<input>').attr('type', 'checkbox').data('id', past[i].StudyInstanceUID)));

        tmprow.append($('<td>').append(past[i].Modality));
        tmprow.append($('<td>').append(past[i].Date));
        tmprow.append($('<td>').append(past[i].Desc));
        tmprow.append($('<td>').append(past[i].ImgCnt));
        bodyRow.append(tmprow);
    }

    $('#study_table').append(bodyRow);

}
function ResetHead() {
    $('#s-mod').text("モダリティ");
    $('#s-date').text("検査日");
    $('#s-desc').text("検査記述");
    $('#s-cnt').text("画像枚数");

    sAsc = "DESC";
    sKey = "";
}

function reSetScanImage(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    C_SetGetScan(order.OrderNo, order.PatID, order.Date, order.Modality, setScanImage);
}
function setScanImage(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    if (ret.d.Result) {
        $('#scan-data').remove();
        $('#pre-image').css('visibility', 'hidden');
        $('#next-image').css('visibility', 'hidden');

        var scan = $('<div>').attr('id', 'scan-data').attr('role', 'listbox').addClass('carousel-inner');

        var images = ret.d.Items;

        for (var i = 0; i < images.length; i++) {
            var tmprow

            if (i == 0)
                tmprow = $('<div>').addClass('item').addClass('active').data('file', images[i].Name);
            else
                tmprow = $('<div>').addClass('item').data('file', images[i].Name);

            tmprow.append($('<img>').addClass('item-image').attr('src', images[i].URL));
            tmprow.append($('<div>').addClass('carousel-caption').append(
                    $('<p>').append((i + 1) + "/" + images.length)
                ));

            scan.append(tmprow);
        }

        if (images.length > 0) {
            $('#pre-image').css('visibility', 'visible');
            $('#next-image').css('visibility', 'visible');
        }

        $('#scan-images').prepend(scan);

        CloseLoading();
    }
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
function endSettings(ret) {

    alert('更新しました。');

    C_GetDoctor(setSettings);

    $('#modal-doc').modal('hide');

}
var Template = [];

function setSettings(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }
    Template = ret.d;

    var obj = $('#doctor-list');

    obj.empty();

    for (var i = 0; i < Template.length; i++)
        obj.append($('<li>').attr('data-key', 'doctor').append($('<a>').append(Template[i]).data('key', 'doctor')));

}
function setNewTemp(value) {
    var table = $('#setting_table tbody');

    table.append(
        $('<tr>').append(

                $('<td>')
            ).append(
                $('<td>').append(
                    $('<input>').attr('type', 'text').val(value)
                )
            ).append(
                $('<td>').append(
                    $('<input>').addClass('btn-sm').addClass('del').attr('value', '削除').attr('type', 'button')
                )
            )
        );

    $('#sorttable').sortable();
    $("#sorttable").disableSelection();
    $('input.del').off('click');
    $('input.del').on('click', function (e) {
        if (!confirm('この候補値を削除します。\nよろしいですか？'))
            return;

        var delObj = $(this).parent().parent();

        delObj.remove();
    });
}


function C_WebGetPreList(patid, date, mod, func) {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetPreList",
        data: "{ patid:'" + patid + "', date:'" + date + "', mod:'" + mod + "', isCnt:1 }",
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

function C_WebGetPreOrder(orderno, uid, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetPreOrder",
        data: "{ orderno:'" + orderno + "', uid:'" + uid + "' }",
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

function C_SetGetScan_First(orderno, patid, date, mod, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetScanImage_First",
        data: "{ orderno:'" + orderno + "', patid:'" + patid + "', date:'" + date + "', mod:'" + mod + "' }",
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

function C_GetComboList(type, val, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetComboList",
        data: "{ type:" + type + ", val:'" + val + "' }",
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

function C_SetGetScan(orderno, patid, date, mod, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetScanImage",
        data: "{ orderno:'" + orderno + "', patid:'" + patid + "', date:'" + date + "', mod:'" + mod + "' }",
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

function C_SetDelScan(file, orderno, patid, date, mod, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebDelScanImage",
        data: "{ file:'" + file + "', orderno:'" + orderno + "', patid:'" + patid + "', date:'" + date + "', mod:'" + mod + "' }",
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

function C_GetDoctor(func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetDoctor",
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
function C_SetDoctor(val, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebSetDoctor",
        data: "{ doc:" + castJson(val) + "}",
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