var patient;
var mainorder;
var past = [];
var pastids = [];
var isView = 0;

var Template;
var conf;

var caret;

var ipAddress;

var editStatus = -1;

var repVal = [];

var contact = [];

// ロード処理
$(function () {

    // セッション確認
    if (!_util.userid || _util.userid == '') {
        _util.userid = sessionStorage.getItem('userid');
        _util.orderid = sessionStorage.getItem('orderid');
        _util.hospid = sessionStorage.getItem('hospid');
        _util.patid = sessionStorage.getItem('patid');
        _util.param = sessionStorage.getItem('param');
        isView = sessionStorage.getItem('view');

        //sessionStorage.clear();
    }

    if (!_util.userid || _util.userid == '') {
        postForm('./Login.aspx');
        return;
    }

    // ロード画面表示
    ShowLoading();

        $(document).click(function () {
            $('ul.dropdown-menu').hide();
        });
        // 編集モードの設定（各項目の動作を設定）
        $.fn.autoKana('#patname', '#patname_h', {
            katakana: true  //true：カタカナ、false：ひらがな（デフォルト）
        });
        $('#patid').on('change', function (e) {
            _util.patid = $('#patid').val();
            if (!_util.hospid || _util.hospid == '' || _util.hospid == 0 || _util.hospid == '0')
                return;

            C_GetPatList(setPatList);
        });

        $('#patname_h').on('change', function (e) {
            $('#patname_h').val(_toKatakana($('#patname_h').val()));
        });

        $('#hosp').on('change', function (e) {
            if (_util.hospid && _util.hospid != '0')
                if (!confirm('他施設の初期値に更新されますが、よろしいですか？\n※入力途中のデータは保持されません。'))
                    return;

            _util.hospid = $('#hosp').val();

            $('#patid').val('');
            $('#isemergency')[0].checked = false;
            $('#isfax_own')[0].checked = false;
            $('#ismail_own')[0].checked = false;
            $('#isfax')[0].checked = false;
            $('#ismail')[0].checked = false;
            $('#patname').val('');
            $('#isvisit').val('');
            $('#department').val('');
            $('#patname_h').val('');
            $('#sex').val('男');
            $('#birthday').val('');
            $('#age').val('');
            $('#studydate').val('');
            $('#studytime').val('');
            $('#modality').val('');
            $('#studytype').val('');
            $('#doctor').val('');
            $('#bodypart').val('');
            $('#comment').val('');
            $('#contact').val('');
            $('#recept').val('');
            $('#memo').val('');
            $('#alert').val('');

            $('#orderno').val('');

            $('#study_table tbody').remove();

            changeIntro();

            removeModClass($('#modality'));

            if (_util.hospid != '') {
                C_GetHospTempList(setHospTempList);
                C_GetHospConfig(setHospConfig);
            }

            //C_GetPatList(setPatList);
        });

        $('#memo-setting').on('click', function (e) {
            if (_util.hospid && _util.hospid != '0')
            {
                if (!confirm("個人メモを更新します。\nよろしいですか？"))
                    return;

                var key = 'MemoUser' + _util.userid;
                var val = $('#memo').val();

                C_SetHospConfig(key, val, setConfg);
            }
        });

        $('#alert-setting').on('click', function (e) {
            if (_util.hospid && _util.hospid != '0') {
                if (!confirm("施設注意事項を更新します。\nよろしいですか？"))
                    return;

                var key = 'Memo';
                var val = $('#alert').val();

                C_SetHospConfig(key, val, setConfg);
            }
        });

        $('#isfax_own, #ismail_own').on('click', function (e) {
            //if(!$(this)[0].checked)
            //    return;

            //if ($(this).attr('id') == "isfax_own")
            //    $('#ismail_own')[0].checked = false;
            //else
            //    $('#isfax_own')[0].checked = false;
        });

        $('#isfax, #ismail').on('click', function (e) {
            if ($(this).attr('id') == "isfax") {
                if (!$(this)[0].checked){
                    $('#ihosp').attr('disabled', 'disabled');
                    $('#ihosp_number').attr('disabled', 'disabled');
                }
                else {
                    $('#ismail')[0].checked = false;
                    $('#ihosp').removeAttr('disabled');
                    $('#ihosp_number').removeAttr('disabled');
                    $('#ihosp').val('');
                    $('#ihosp_number').val('');


                    var cd = $('#hosp option:selected').text();
                    var list = cd.split(' ');

                    C_GetFaxSender(list[0], setFaxSenList);
                }
            }
            else
            {
                if (!$(this)[0].checked) {
                    $('#ihosp').attr('disabled', 'disabled');
                    $('#ihosp_number').attr('disabled', 'disabled');
                }
                else {
                    $('#isfax')[0].checked = false;
                    $('#ihosp').removeAttr('disabled');
                    $('#ihosp_number').attr('disabled', 'disabled');
                    $('#ihosp').val('');
                    $('#ihosp_number').val('');

                    var cd = $('#hosp option:selected').text();
                    var list = cd.split(' ');

                    C_GetMailSender(list[0], setMailSenList);
                }
            }
        });

        $('#ihosp').on('keyup', function (e) {
            checkFAXList($('#ihosp').val());
        });

        $('#birthday, #studydate').on('change', function (e) {
            setAge();
        });

        $('#birthday').on('change', function (e) {
            $('#birthday').val(convBirth($('#birthday').val()));
            setAge();
        });

        $('input.listblock').on('click focus', function (e) {
            var id = $(this).attr('id');

            event.stopPropagation();

            $('ul.dropdown-menu').hide();
            $('#' + id + '-list').toggle();


            $('#' + id + '-list li a').focus();

            return false;
        });

        $('#studydate,#ismail,textarea.area').on('focus', function (e) {
            $('ul.dropdown-menu').hide();
        });

        $('textarea.area').on('click focus', function (e) {
            $('ul.dropdown-menu').hide();

            var id = $(this).attr('id');

            caret = document.getElementById(id).selectionStart;

            event.stopPropagation();

            $('#' + id + '-list').toggle();

            return false;
        });
        $('input').on('change', function (e) {
            var id = $(this).attr('id');

            replaceVal(id);
            $(this).attr('title', $(this).val());
        });
        $('textarea').on('change', function (e) {
            var id = $(this).attr('id');

            replaceVal(id);
            $(this).attr('title', $(this).val());
        });
        $('#studytime').on('blur', function (e) {
            CheckTime();
        });
        $('#studydate').on('focus', function (e) {
            if($('#studydate').val() == '')
            {
                var date = new Date();
                var formattedDate =
                    date.getFullYear() + '/' + (date.getMonth() + 1) + '/' + date.getDate();

                $("#studydate").datepicker("setDate", formattedDate);
            }
        });

        $(document).on('click', 'ul li a', function (e) {
            var key = $(this).data('key');
            var strInsert = $(this).text();

            if ($(this).parent().parent().hasClass('listblock')) {
                //文字列を結合して、テキストエリアに出力
                $('#' + key).val(strInsert);
                $('#' + key).attr('title', $('#' + key).val());
                if (key == 'modality')
                {
                    setModColor($('#' + key), $('#' + key).val());
                    setModColor($('#p_' + key), $('#' + key).val());
                }
            }
            else {
                var strOriginal = $('#' + key).val();
                //カーソル位置より左の文字列
                var leftPart = strOriginal.substr(0, caret);
                //カーソル位置より右の文字列
                var rightPart = strOriginal.substr(caret, strOriginal.length);

                //文字列を結合して、テキストエリアに出力
                $('#' + key).val(leftPart + strInsert + rightPart);
                $('#' + key).attr('title', $('#' + key).val());
            }
            if ($('#p_' +key)) {
                $('#p_' + key).empty();

                if ($('#' + key).val().length > 5)
                    $('#p_' + key).append($('#' + key).val().substring(0, 5) + "...").attr('title', $('#' + key).val());
                else
                    $('#p_' + key).append($('#' + key).val());

            }

            replaceVal(key);
            changeihosp();
        });

        $(document).on('keydown', '#add-val', function (e) {
            if (e.keyCode != 13)
                return;

            setNewTemp($('#add-val').val());
            $('#add-val').val('');
            $('#add-val').focus();
        });

        $('input:button.setting').on('click', function (e) {
            if (_util.hospid == '')
                return;

            var key = $(this).attr('id').replace('-setting', '');
            var title = $('#' + key + '-label').text();

            viewModal(title + ' 変更', key);
            $('#modal').modal();
        });

        $('input:text.editpast').on('change', function (e) {
            var key = $(this).attr('id');
            $('#p_' + key).empty();
            $('#p_' + key).append($('#' + key).val());
            if(key == 'modality')
            {
                setModColor($('#' +key), $('#' +key).val());
                setModColor($('#p_' +key), $('#' + key).val());
            }
        });
        $('textarea.editpast').on('change', function (e) {
            var key = $(this).attr('id');
            $('#p_' + key).empty();
            if ($('#' + key).val().length > 5)
                $('#p_' + key).append($('#' + key).val().substring(0, 5) + "...").attr('title', $('#' + key).val());
            else
                $('#p_' + key).append($('#' + key).val());

        });
        $('input:checkbox.editpast').on('change', function (e) {
            var key = $(this).attr('id');
            $('#p_' + key).empty();

            if ($('#' + key)[0].checked) {
                $('#p_' + key).append('緊急').addClass('alert-data-val');
            } else {
                $('#p_' + key).append('通常').removeClass('alert-data-val');
            }
        });

        $('#add-val-set').on('click', function (e) {
            setNewTemp($('#add-val').val());
            $('#add-val').val('');
        });

        $('#set_change').on('click', function (e) {
            if (!confirm('候補値を更新します。\nよろしいですか？'))
                return;

            var settingVals = [];
            var key = $('#modal-title').data('key');

            $('#setting_table').find('input:text').each(function () {
                if ($(this).val() != '')
                    settingVals[settingVals.length] = $(this).val();
            });

            C_SetHospTempList(key, settingVals, endSettings);
        });

        $('#delImg').on('click', function (e) {
            if (confirm("切り取った依頼票を削除しますが、よろしいですか？"))
            {
                var status = $('#status').data('value');
                var orderid = 0;

                if (_util.orderid)
                    orderid = _util.orderid;
                var val = $('#thumbnail').data('value');

                C_ResetImage(status, orderid, 0, val, resetThumb);
            }
        });

        $('#orgImg_del').on('click', function (e) {
            if (confirm("依頼票との紐付けを解除します、よろしいですか？\n※切り取った依頼票も削除されます。")) {
                var status = $('#status').data('value');
                var orderid = 0;

                if (_util.orderid)
                    orderid = _util.orderid;
                var val = $('#orgImg').data('name');
                var vals = val.split(',');
                for (var i = 0; i < vals.length; i++) {
                    C_ResetImage(status, orderid, 1, vals[i], resetOrigin);
                }
            }
        });

        $('#thumbnail').on('click', function (e) {
            if (!$('#thumbnail').data('value') || $('#thumbnail').data('value') == ''){
                var val = $('#orgImg').data('name');
                if (!val)
                    return;

                var vals = val.split(',');
                if (vals && vals.length > 0)
                    C_SetCutImg(vals[0], setCutImg);
            }
            else
            {
                var url = $('#thumbnail').attr('src');
                var d = new Date();
                var ms = d.getTime();
                window.open(url + "?" + ms, "依頼票", "width=680,height=800,left=1280,resizable=1");
            }
        });
        $('#orgImg').on('click', function (e) {
            if (!$('#orgImg').data('name') || $('#orgImg').data('name') == '')
                alert('依頼票の原本を登録してください。');
            else {
                var status = $('#status').data('value');
                var orderid = 0;

                if (_util.orderid)
                    orderid = _util.orderid;
                var val = $('#orgImg').data('name');

                var vals = val.split(',');
                
                for (var i = 0; i < vals.length; i++)
                {
                    C_GetImage(status, orderid, 1, vals[i], viewOrigin);
                }
            }
        });

    // body dragover
        $(document).on("dragover", "body", function (e) {
            e.preventDefault();
        });
    // body draglave
        $(document).on("dragleave", "body", function (e) {
            e.preventDefault();
        });
    // imagefield dragover
        $(document).on("dragover", "#thumbnail", function (e) {
            e.preventDefault();
            $("#thumbnail").addClass("droppable");
        });
    // imagefield dragleave
        $(document).on("dragleave", "#thumbnail", function (e) {
            e.preventDefault();
            $("#thumbnail").removeClass("droppable");
        });
    // imagefield drop
        $(document).on("drop", "#thumbnail", function (e) {
            e.preventDefault();
            $("#thumbnail").removeClass("hover droppable");

            var files = e.originalEvent.dataTransfer.files;
            var value = $('#orgImg').data('name');
            var isNew = 1;

            if (!value || value == '')
            {
                value = '';
            } else {
                isNew = 0;
            }

            for (var i = 0; i < files.length; i++)
            {
                if (value != '')
                    value += ',';
                value += files[i].name;
            }

            $('#orgImg').data('name', value);

            var isEm = 0;

            if ($('#isemergency')[0].checked)
                isEm = 1;

            C_SetOriginal(isEm, isNew, value, setOriginal);
        });

        $('#tolist').on('click', function (e) {
            if (!confirm('入力中の内容は保存されません。よろしいですか？'))
                return;

            sessionStorage.setItem('userid', _util.userid);

            postForm('./Search.aspx');
        });

        $('#new').on('click', function (e) {
            if (!confirm('入力をクリアしますが、よろしいですか？'))
                return;
            ShowLoading();

            $('#status').val('未');
            $('#status').data('value', -1);
            $('#orderno').val('');
            $('#hosp').val('');
            $('#patid').val('');
            $('#isemergency')[0].checked = false;
            $('#ismail')[0].checked = false;
            $('#patname').val('');
            $('#patname_h').val('');
            $('#sex').val('男');
            $('#birthday').val('');
            $('#age').val('');
            $('#studydate').val('');
            $('#studytime').val('');
            $('#modality').val('');
            $('#isvisit').val('');
            $('#studytype').val('');
            $('#department').val('');
            $('#doctor').val('');
            $('#bodypart').val('');
            $('#comment').val('');
            $('#contact').val('');
            $('#recept').val('');

            $('#memo').val('');
            $('#alert').val('');

            if ($('#orgImg').data('name') && $('#orgImg').data('name') != '') {
                $('#orgImg').data('name', '');
                $('#orgImg').data('value', '');
                $('#thumbnail').data('value', '');
                $('#thumbnail').attr('src', '');
            }

            $('#study_table tbody').remove();
            $('#save').css('visibility', 'visible');

            _util.orderid = 0;
            removeModClass($('#modality'));

            CloseLoading();

        });

        $('#copy').on('click', function (e) {
            if (!confirm('表示中の依頼をコピーします。よろしいですか？'))
                return;
            ShowLoading();

            $('#status').val('未');
            $('#status').data('value', -1);
            $('#orderno').val('');

            if ($('#orgImg').data('name') && $('#orgImg').data('name') != '') {
                $('#orgImg').data('name', '');
                $('#orgImg').data('value', '');
                $('#thumbnail').data('value', '');
                $('#thumbnail').attr('src', '');
            }

            _util.orderid = 0;
            C_GetPatList(setPatList);
            CloseLoading();

        });
        $(document).on('click', 'input.edit', function (e) {
            if (!confirm('入力中の内容は保存されません。よろしいですか？'))
                return;

            var id = $(this).data('id');

            sessionStorage.setItem('userid', _util.userid);
            sessionStorage.setItem('hospid', _util.hospid);
            sessionStorage.setItem('orderid', id);
            sessionStorage.setItem('patid', $('#patid').val());

            postForm('./Order.aspx');
        });
        $(document).on('click', 'input.view', function (e) {
            sessionStorage.setItem('pastid', $(this).data('id'));

            window.open('./PastOrder.aspx', "", "width=960,height=1024,left=1280,resizable=1");
        });
        $(document).on('click', 'input.viewImg', function (e) {
            var id = $(this).data('id');

            C_WebGetFileList_Past(id, viewPastFiles);
        });


    $('#clear').on('click', function (e) {
        if (!confirm('入力をクリアしますが、よろしいですか？'))
            return;

        ShowLoading();

        $('#status').val('未');
        $('#status').data('value', -1);
        $('#orderno').val('');
        $('#hosp').val('');
        $('#patid').val('');
        $('#isemergency')[0].checked = false;

        $('#isfax_own')[0].checked = false;
        $('#ismail_own')[0].checked = false;
        $('#isfax')[0].checked = false;
        $('#ismail')[0].checked = false;

        $('#patname').val('');
        $('#patname_h').val('');
        $('#sex').val('男');
        $('#birthday').val('');
        $('#age').val('');
        $('#studydate').val('');
        $('#studytime').val('');
        $('#modality').val('');
        $('#isvisit').val('');
        $('#studytype').val('');
        $('#department').val('');
        $('#doctor').val('');
        $('#bodypart').val('');
        $('#comment').val('');
        $('#contact').val('');
        $('#recept').val('');

        $('#memo').val('');
        $('#alert').val('');

        if ($('#orgImg').data('name') && $('#orgImg').data('name') != '')
        {
            $('#orgImg').data('name', '');
            $('#orgImg').data('value', '');
            $('#thumbnail').data('value', '');
            $('#thumbnail').attr('src', '');
        }

        $('#study_table tbody').remove();
        removeModClass($('#modality'));


        if (_util.orderid && _util.orderid != '') {
            Search = [];

            Search[Search.length] = _util.hospid;
            Search[Search.length] = -2;
            Search[Search.length] = '';
            Search[Search.length] = _util.patid;
            Search[Search.length] = '';
            Search[Search.length] = '';
            Search[Search.length] = '';
            Search[Search.length] = '1';

            C_WebGetOrderList(Search, setOrderData);
            setAge();
        }

        CloseLoading();
    });

    $('#save').on('click', function (e) {
        if (!$('#patid').val() || $('#patid').val() == '') {
            alert('一時保存する場合、患者IDを入力してください。');
            return;
        }

        var check = C_CheckString($('#patname').val());
        if (check != $('#patname').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#patname').val());
        if (check != $('#patname').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#patname_h').val());
        if (check != $('#patname_h').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#bodypart').val());
        if (check != $('#bodypart').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#department').val());
        if (check != $('#department').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#doctor').val());
        if (check != $('#doctor').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#comment').val());
        if (check != $('#comment').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#contact').val());
        if (check != $('#contact').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#recept').val());
        if (check != $('#recept').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }


        if (!confirm("入力した内容で一時保存します。\nよろしいですか？"))
            return;

        var value = [];
        if (mainorder && mainorder != null)
            value[value.length] = "" + mainorder.Status;
        else
            value[value.length] = "-1";

        value[value.length] = "" + _util.hospid;
        if (_util.orderid != '')
            value[value.length] = "" + _util.orderid;
        else
            value[value.length] = '0';

        value[value.length] = $('#orderno').val();
        value[value.length] = $('#patid').val();
        value[value.length] = $('#patname').val();
        value[value.length] = $('#patname_h').val();
        switch ($('#sex').val()) {
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
        if ($('#age').val() && $('#age').val() != '')
            value[value.length] = $('#age').val();
        else
            value[value.length] = '0';
        value[value.length] = formatDate($('#birthday').val());
        value[value.length] = $('#modality').val();
        value[value.length] = formatDate($('#studydate').val());
        value[value.length] = formatTime($('#studytime').val());
        value[value.length] = $('#bodypart').val();
        value[value.length] = $('#studytype').val();
        value[value.length] = $('#isvisit').val();
        value[value.length] = $('#department').val();
        value[value.length] = $('#doctor').val();

        if ($('#isemergency')[0].checked)
            value[value.length] = '1';
        else
            value[value.length] = '0';

        if ($('#ismail')[0].checked || $('#isfax')[0].checked)
            value[value.length] = '1';
        else
            value[value.length] = '0';

        value[value.length] = $('#comment').val();
        value[value.length] = $('#contact').val();
        value[value.length] = $('#recept').val();
        if ($('#orgImg').data('name'))
            value[value.length] = $('#orgImg').data('name');
        else
            value[value.length] = "";

        if ($('#thumbnail').data('value'))
            value[value.length] = $('#thumbnail').data('value');
        else
            value[value.length] = "";

        if ($('#isfax_own')[0].checked)
            if ($('#ismail_own')[0].checked)
                value[value.length] = '2';
            else
                value[value.length] = '0';
        else if ($('#ismail_own')[0].checked)
            value[value.length] = '1';
        else
            value[value.length] = '';

        if ($('#isfax')[0].checked)
            value[value.length] = '0';
        else if ($('#ismail')[0].checked)
            value[value.length] = '1';
        else
            value[value.length] = '';

        value[value.length] = $('#ihosp').val();
        value[value.length] = $('#ihosp_number').val();

        editStatus = 0;
        C_SetPreOrder(0, value, endOrder);
    });

    $('#delete_preorder').on('click', function (e) {
        if (!confirm("振り分けられている場合、削除できません。\n振分を外してください。\n\n削除しますか？"))
            return;

        var value = [];
        if (mainorder && mainorder != null)
            value[value.length] = "" + mainorder.Status;
        else
            value[value.length] = "-1";

        value[value.length] = "" + _util.hospid;
        if (_util.orderid != '')
            value[value.length] = "" + _util.orderid;
        else
            value[value.length] = '0';

        value[value.length] = $('#orderno').val();
        value[value.length] = $('#patid').val();
        value[value.length] = $('#patname').val();
        value[value.length] = $('#patname_h').val();
        switch ($('#sex').val()) {
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
        if ($('#age').val() && $('#age').val() != '')
            value[value.length] = $('#age').val();
        else
            value[value.length] = '0';
        value[value.length] = formatDate($('#birthday').val());
        value[value.length] = $('#modality').val();
        value[value.length] = formatDate($('#studydate').val());
        value[value.length] = formatTime($('#studytime').val());
        value[value.length] = $('#bodypart').val();
        value[value.length] = $('#studytype').val();
        value[value.length] = $('#isvisit').val();
        value[value.length] = $('#department').val();
        value[value.length] = $('#doctor').val();

        if ($('#isemergency')[0].checked)
            value[value.length] = '0';
        else
            value[value.length] = '1';

        if ($('#ismail')[0].checked || $('#isfax')[0].checked)
            value[value.length] = '0';
        else
            value[value.length] = '1';

        value[value.length] = $('#comment').val();
        value[value.length] = $('#contact').val();
        value[value.length] = $('#recept').val();
        if ($('#orgImg').data('name'))
            value[value.length] = $('#orgImg').data('name');
        else
            value[value.length] = "";

        if ($('#thumbnail').data('value'))
            value[value.length] = $('#thumbnail').data('value');
        else
            value[value.length] = "";

        if ($('#isfax_own')[0].checked)
            if ($('#ismail_own')[0].checked)
                value[value.length] = '2';
            else
                value[value.length] = '0';
        else if ($('#ismail_own')[0].checked)
            value[value.length] = '1';
        else
            value[value.length] = '';

        if ($('#isfax')[0].checked)
            value[value.length] = '0';
        else if ($('#ismail')[0].checked)
            value[value.length] = '1';
        else
            value[value.length] = '';

        value[value.length] = $('#ihosp').val();
        value[value.length] = $('#ihosp_number').val();

        editStatus = 3;

        C_SetPreOrder(3, value, endOrder);
    });

    $('#delete').on('click', function (e) {
        if (!confirm("振り分けられている場合、削除できません。\n振分を外してください。\n\n削除しますか？\n※このページを削除します。"))
            return;

        var value = [];
        if (mainorder && mainorder != null)
            value[value.length] = "" + mainorder.Status;
        else
            value[value.length] = "-1";

        value[value.length] = "" + _util.hospid;
        if (_util.orderid != '')
            value[value.length] = "" + _util.orderid;
        else
            value[value.length] = '0';

        value[value.length] = $('#orderno').val();
        value[value.length] = $('#patid').val();
        value[value.length] = $('#patname').val();
        value[value.length] = $('#patname_h').val();
        switch ($('#sex').val()) {
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
        if ($('#age').val() && $('#age').val() != '')
            value[value.length] = $('#age').val();
        else
            value[value.length] = '0';
        value[value.length] = formatDate($('#birthday').val());
        value[value.length] = $('#modality').val();
        value[value.length] = formatDate($('#studydate').val());
        value[value.length] = formatTime($('#studytime').val());
        value[value.length] = $('#bodypart').val();
        value[value.length] = $('#studytype').val();
        value[value.length] = $('#isvisit').val();
        value[value.length] = $('#department').val();
        value[value.length] = $('#doctor').val();

        if ($('#isemergency')[0].checked)
            value[value.length] = '0';
        else
            value[value.length] = '1';

        if ($('#ismail')[0].checked || $('#isfax')[0].checked)
            value[value.length] = '0';
        else
            value[value.length] = '1';

        value[value.length] = $('#comment').val();
        value[value.length] = $('#contact').val();
        value[value.length] = $('#recept').val();
        if ($('#orgImg').data('name'))
            value[value.length] = $('#orgImg').data('name');
        else
            value[value.length] = "";

        if ($('#thumbnail').data('value'))
            value[value.length] = $('#thumbnail').data('value');
        else
            value[value.length] = "";

        if ($('#isfax_own')[0].checked)
            if ($('#ismail_own')[0].checked)
                value[value.length] = '2';
            else
                value[value.length] = '0';
        else if ($('#ismail_own')[0].checked)
            value[value.length] = '1';
        else
            value[value.length] = '';

        if ($('#isfax')[0].checked)
            value[value.length] = '0';
        else if ($('#ismail')[0].checked)
            value[value.length] = '1';
        else
            value[value.length] = '';

        value[value.length] = $('#ihosp').val();
        value[value.length] = $('#ihosp_number').val();

        editStatus = 4;

        C_SetPreOrder(4, value, endOrder);
    });

    $('#send').on('click', function (e) {
        var isMust = true;

        for (var i = 0 ; i < conf.Conf.length; i++) {
            if (conf.Conf[i].Key.indexOf('C_') < 0 || conf.Conf[i].Value != 1)
                continue;

            var key = conf.Conf[i].Key.replace('C_', '');

            if ($('#' + key).val() == '') {
                $('#' + key).addClass('error-val');
                isMust = false;
            }
        }

        if (!isMust) {
            alert('必須項目を入力してください。');
            return;
        }

        var bodypart = $('#bodypart').val();
        var comment = $('#comment').val();
        var len = getBytes(bodypart);

        if (len > 900) {
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
        if (!$('#orgImg').data('name') || $('#orgImg').data('name') == '')
            if (!confirm("依頼票原本が選択されておりません。\nよろしいですか？"))
                return;

        var check = C_CheckString($('#patname').val());
        if (check != $('#patname').val())
        {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#patname').val());
        if (check != $('#patname').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#patname_h').val());
        if (check != $('#patname_h').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#bodypart').val());
        if (check != $('#bodypart').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#department').val());
        if (check != $('#department').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#doctor').val());
        if (check != $('#doctor').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#comment').val());
        if (check != $('#comment').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#contact').val());
        if (check != $('#contact').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#recept').val());
        if (check != $('#recept').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }

        if (!confirm("入力した内容で読影依頼を登録します。\nよろしいですか？"))
            return;

        var value = [];
        if (mainorder && mainorder != null)
            value[value.length] = "" + mainorder.Status;
        else
            value[value.length] = "-1";

        value[value.length] = "" + _util.hospid;
        if (_util.orderid != '')
            value[value.length] = "" + _util.orderid;
        else
            value[value.length] = '0';

        value[value.length] = $('#orderno').val();
        value[value.length] = $('#patid').val();
        value[value.length] = $('#patname').val();
        value[value.length] = $('#patname_h').val();
        switch ($('#sex').val()) {
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
        if ($('#age').val() && $('#age').val() != '')
            value[value.length] = $('#age').val();
        else
            value[value.length] = '0';
        value[value.length] = formatDate($('#birthday').val());
        value[value.length] = $('#modality').val();
        value[value.length] = formatDate($('#studydate').val());
        value[value.length] = formatTime($('#studytime').val());
        value[value.length] = $('#bodypart').val();
        value[value.length] = $('#studytype').val();
        value[value.length] = $('#isvisit').val();
        value[value.length] = $('#department').val();
        value[value.length] = $('#doctor').val();

        if ($('#isemergency')[0].checked)
            value[value.length] = '1';
        else
            value[value.length] = '0';

        if ($('#ismail')[0].checked || $('#isfax')[0].checked)
            value[value.length] = '1';
        else
            value[value.length] = '0';

        value[value.length] = $('#comment').val();
        value[value.length] = $('#contact').val();
        value[value.length] = $('#recept').val();
        if ($('#orgImg').data('name'))
            value[value.length] = $('#orgImg').data('name');
        else
            value[value.length] = "";

        if ($('#thumbnail').data('value'))
            value[value.length] = $('#thumbnail').data('value');
        else
            value[value.length] = "";


        if ($('#isfax_own')[0].checked)
            if ($('#ismail_own')[0].checked)
                value[value.length] = '2';
            else
                value[value.length] = '0';
        else if ($('#ismail_own')[0].checked)
            value[value.length] = '1';
        else
            value[value.length] = '';

        if ($('#isfax')[0].checked)
            value[value.length] = '0';
        else if ($('#ismail')[0].checked)
            value[value.length] = '1';
        else
            value[value.length] = '';

        value[value.length] = $('#ihosp').val();
        value[value.length] = $('#ihosp_number').val();

        editStatus = 1;
        C_SetPreOrder(1, value, endOrder);
    });

    $('#order').on('click', function (e) {
        var isMust = true;

        for (var i = 0 ; i < conf.Conf.length; i++)
        {
            if (conf.Conf[i].Key.indexOf('C_') < 0 || conf.Conf[i].Value != 1)
                continue;

            var key = conf.Conf[i].Key.replace('C_', '');

            if ($('#' + key).val() == '') {
                $('#' + key).addClass('error-val');
                isMust = false;
            }
        }

        if (!isMust) {
            alert('必須項目を入力してください。');
            return;
        }

        var bodypart = $('#bodypart').val();
        var comment = $('#comment').val();
        var len = getBytes(bodypart);

        if (len > 900) {
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
        if (!$('#orgImg').data('name') || $('#orgImg').data('name') == '')
            if (!confirm("依頼票原本が選択されておりません。\nよろしいですか？"))
                return;


        var check = C_CheckString($('#patname').val());
        if (check != $('#patname').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#patname').val());
        if (check != $('#patname').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#patname_h').val());
        if (check != $('#patname_h').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#bodypart').val());
        if (check != $('#bodypart').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#department').val());
        if (check != $('#department').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#doctor').val());
        if (check != $('#doctor').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#comment').val());
        if (check != $('#comment').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#contact').val());
        if (check != $('#contact').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }
        check = C_CheckString($('#recept').val());
        if (check != $('#recept').val()) {
            alert("登録できない文字が含まれております。\n\n" + $('#patname').val() + "\n\n　　↓\n\n" + check);
            return;
        }

        if (!confirm("入力した内容で読影依頼を登録します。\nよろしいですか？"))
            return;

        var value = [];
        if(mainorder &&  mainorder != null)
            value[value.length] = mainorder.Status;
        else
            value[value.length] = "-1";

        value[value.length] = _util.hospid;
        if (_util.orderid != '')
            value[value.length] = _util.orderid;
        else
            value[value.length] = '0';

        value[value.length] = $('#orderno').val();
        value[value.length] = $('#patid').val();
        value[value.length] = $('#patname').val();
        value[value.length] = $('#patname_h').val();
        switch ($('#sex').val()) {
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
        if ($('#age').val() && $('#age').val() != '')
            value[value.length] = $('#age').val();
        else
            value[value.length] = '0';
        value[value.length] = formatDate($('#birthday').val());
        value[value.length] = $('#modality').val();
        value[value.length] = formatDate($('#studydate').val());
        value[value.length] = formatTime($('#studytime').val());
        value[value.length] = $('#bodypart').val();
        value[value.length] = $('#studytype').val();
        value[value.length] = $('#isvisit').val();
        value[value.length] = $('#department').val();
        value[value.length] = $('#doctor').val();

        if ($('#isemergency')[0].checked)
            value[value.length] = '1';
        else
            value[value.length] = '0';

        if ($('#ismail')[0].checked || $('#isfax')[0].checked)
            value[value.length] = '1';
        else
            value[value.length] = '0';

        value[value.length] = $('#comment').val();
        value[value.length] = $('#contact').val();
        value[value.length] = $('#recept').val();
        if ($('#orgImg').data('name'))
            value[value.length] = $('#orgImg').data('name');
        else
            value[value.length] = "";

        if ($('#thumbnail').data('value'))
            value[value.length] = $('#thumbnail').data('value');
        else
            value[value.length] = "";

        if ($('#isfax_own')[0].checked)
            if ($('#ismail_own')[0].checked)
                value[value.length] = '2';
            else
                value[value.length] = '0';
        else if ($('#ismail_own')[0].checked)
            value[value.length] = '1';
        else
            value[value.length] = '';

        if ($('#isfax')[0].checked)
            value[value.length] = '0';
        else if ($('#ismail')[0].checked)
            value[value.length] = '1';
        else
            value[value.length] = '';

        value[value.length] = $('#ihosp').val();
        value[value.length] = $('#ihosp_number').val();

        editStatus = 2;

        C_SetPreOrder(2, value, endOrder);
    });

    C_GetHospList(setHospList);

    changeIntro();

    if (_util.orderid && _util.orderid != '') {
        Search = [];

        Search[Search.length] = _util.hospid;
        Search[Search.length] = -2;
        Search[Search.length] = '';
        Search[Search.length] = _util.patid;
        Search[Search.length] = '';
        Search[Search.length] = '';
        Search[Search.length] = '';
        Search[Search.length] = '1';

        C_WebGetOrderList(Search, setOrderData);
    }

    if (isView != 1) {
        $('.datepicker').each(function () {
            $(this).datepicker({
                language: 'ja'
            });
        });
    }

    $('#sorttable').sortable();
    $("#sorttable").disableSelection();

    CloseLoading();
});

function setFaxSenList(ret) {
    if (!ret || !ret.d)
        return;

    if(!ret.d.Result)
    {
        if(ret.d.Message != "")
            alert(ret.d.Message);

        return;
    }

    var item = ret.d.Items;
    contact = [];
    
    for(var i = 0; i < item.length; i++)
    {
        var vals = item[i].split(',');

        var kana = "";

        if (vals.length < 3)
            continue;
        if (vals.length < 4)
            kana = vals[1];
        else
            kana = vals[3];

        var obj = { Title: vals[1], Kana: kana, FAX: vals[2] };
        contact.push(obj);
    }

    changeList(contact);

    //$('#ihosp').typeahead({
    //    hint: true,
    //    highlight: true,
    //    minLength: 0
    //},
    //{
    //    name: 'contact',
    //    source: substringMatcher(contact)

    //});

    $('#ihosp').bind('change', function (e) {
        var ihosp = $('#ihosp').val();

        $.each(contact, function (i, con) {
            if (ihosp == con.Title) {
                $('#ihosp_number').val(con.FAX);
            }
        });
    });

}

function setMailSenList(ret) {
    if (!ret || !ret.d)
        return;

    if (!ret.d.Result) {
        if (ret.d.Message != "")
            alert(ret.d.Message);

        return;
    }

    var item = ret.d.Items;
    contact = [];

    for (var i = 0; i < item.length; i++) {
        var vals = item[i].split(',');

        var kana = "";

        if (vals.length < 2)
            continue;
        if (vals.length < 3)
            kana = vals[1];
        else
            kana = vals[2];

        var obj = { Title: vals[1], Kana: kana, FAX: "" };
        contact.push(obj);
    }

    changeList(contact);

    $('#ihosp').bind('change', function (e) {
        var ihosp = $('#ihosp').val();

        $.each(contact, function (i, con) {
            if (ihosp == con.Title) {
                $('#ihosp_number').val(con.FAX);
            }
        });
    });

}
function changeihosp() {
    var ihosp = $('#ihosp').val();

    $.each(contact, function (i, con) {
        if (ihosp == con.Title) {
            $('#ihosp_number').val(con.FAX);
        }
    });
}

function changeList(list) {
    var obj = $('#ihosp-list');

    obj.empty();

    for (var i = 0; i < list.length; i++)
        obj.append($('<li>').append($('<a>').data('key', 'ihosp').append(list[i].Title)));
}

function checkFAXList(q) {
    matches = [];

    if (q != '') {
        // regex used to determine if a string contains the substring `q`
        substrRegex = new RegExp(q, 'i');

        // iterate through the pool of strings and for any string that
        // contains the substring `q`, add it to the `matches` array
        $.each(contact, function (i, str) {
            if (str.Title.indexOf(q) == 0)
                matches.push(str);
            else if (str.Kana.indexOf(q) == 0)
                matches.push(str);
        });
    } else {
        $.each(contact, function (i, str) {
            matches.push(str);
        });
    }

    changeList(matches);
}

var substringMatcher = function (strs) {
    return function findMatches(q, cb) {
        var matches, substringRegex;

        // an array that will be populated with substring matches
        matches = [];

        if (q != '')
        {
            // regex used to determine if a string contains the substring `q`
            substrRegex = new RegExp(q, 'i');

            // iterate through the pool of strings and for any string that
            // contains the substring `q`, add it to the `matches` array
            $.each(strs, function (i, str) {
                if (str.Title.indexOf(q) >= 0)
                    matches.push(str.Title);
                else if (str.Kana.indexOf(q) >= 0)
                    matches.push(str.Title);
            });
        } else {
            $.each(strs, function (i, str) {
                matches.push(str.Title);
            });
        }

        cb(matches);
    };
};

function changeIntro() {
    $('#ihosp').attr('disabled', 'disabled');
    $('#ihosp_number').attr('disabled', 'disabled');
}

function setConfg(ret){

    alert('設定を更新しました。');
}

function viewOrigin(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    var d = new Date();
    var ms = d.getTime();

    window.open(ret.d + "?" + ms, "", "width=680,height=800,left=1280,resizable=1");
}

function viewPast(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    window.open(ret.d, "", "width=680,height=800,left=1280,resizable=1");
}

function resetThumb(ret)
{
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }
    $('#thumbnail').attr('src', '');
    $('#thumbnail').data('value', '');

    $('#delImg').css('visibility', 'hidden');
    $('#contact').val($('#contact').val().replace('依頼票表示ボタンを押してください。 ', ''));

}

function resetOrigin(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }
    $('#orgImg').data('value', '');
    $('#orgImg').data('name', '');

    var status = $('#status').data('value');
    var orderid = 0;

    if (_util.orderid)
        orderid = _util.orderid;
    var val = $('#thumbnail').data('value');

    $('#orgImg').css('visibility', 'hidden');

    C_ResetImage(status, orderid, 0, val, resetThumb);
}

function endSettings(ret) {

    alert('更新しました。');

    C_GetHospTempList(setHospTempList);

    $('#modal').modal('hide');

}

function setOriginal(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    if (ret.d == "")
    {
        alert("原本保管に失敗しました。\n\n対象の画像が取得できなかった可能性があるので\n画像を一度閉じて再度操作してください。");
    } else {
        $('#orgImg').data('value', ret.d);
        $('#orgImg_del').css('visibility', 'visible');
        $('#orgImg').css('visibility', 'visible');
    }

    CloseLoading();
}

function setCutImg(ret) {
    if (!ret || !ret.d) {
        alert('切り取った依頼票が見つかりません。');

        CloseLoading();
        return;
    }

    var val = $('#orgImg').data('name');
    var vals = val.split(',');
    if (vals && vals.length > 0)
        $('#thumbnail').data('value', vals[0]);

    var status = $('#status').data('value');
    var orderid = 0;

    if(_util.orderid)
        orderid = _util.orderid;
    var val = $('#thumbnail').data('value');

    var contact = $('#contact').val();
    $('#contact').val('依頼票表示ボタンを押してください。 ' + contact);

    C_GetImage(status, orderid, 0, val, setThumbnail);
}

function setThumbnail(ret) {
    if (!ret || !ret.d) {
        alert('切り取った依頼票が見つかりません。');

        CloseLoading();
        return;
    }

    var d = new Date();
    var ms = d.getTime();

    $('#delImg').css('visibility', 'visible');
    $('#thumbnail').attr('src', ret.d + "?" + ms);
}

function setHospList(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    if (ret.d.Result) {
        var hosp = ret.d.Items;

        for (var i = 0; i < hosp.length; i++)
        {
            if(hosp[i].Visible == 0)
                $('#hosp').append($('<option>').val(hosp[i].HospID).data('cd', hosp[i].CD).append(hosp[i].CD + " " + hosp[i].Name));
        }

    }
    CloseLoading();
}

function setHospTempList(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    if (ret.d.Result) {
        Template = ret.d.Items;
    }

    setTemp('isvisit');
    setTemp('department');
    setTemp('doctor');
    setTemp('modality');
    setTemp('studytype');
    setTemp('bodypart');
    setTemp('comment');
    setTemp('contact');
    setTemp('recept');

    CloseLoading();
}

function setTemp(key) {
    var obj = $('#' + key + '-list');

    obj.empty();

    for(var i = 0; i < Template.length; i++)
        if (Template[i].Key == key)
            obj.append($('<li>').append($('<a>').data('key', key).append(Template[i].Value)));
}

function viewModal(title, key) {
    $('#modal-title').empty();
    $('#modal-title').append(title).data('key', key);

    $('#sorttable').remove();

    var table = $('<tbody>').attr('id', 'sorttable');

    if (Template && Template.length > 0)
        for (var i = 0; i < Template.length; i++)
        {
            if (Template[i].Key == key)
                table.append(
                    $('<tr>').append(
                        $('<td>').append(
                            Template[i].Index
                        )
                    ).append(
                        $('<td>').append(
                            $('<input>').attr('type', 'text').val(Template[i].Value)
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
    var title = "";

    switch (editStatus) {
        case 0:
            title = "一時保存";
            break;
        case 1:
            title = "登録";
            break;
        case 2:
            title = "出力";
            break;
        case 3:
            title = "削除";
            break;
        case 4:
            title = "削除";
            break;
    }

    if (!ret || !ret.d) {
        CloseLoading();
        alert(title + "に失敗しました。");
        return;
    }

    if (ret.d.Result) {
        _util.orderid = ret.d.Message;


        var ispast = false;

        if (past && past.length > 0) {
            for (var i = 0; i < past.length; i++) {
                if (_util.orderid == past[i].OrderID)
                    continue;

                if (past[i].Status < 2) {
                    ispast = true;
                    break;
                }
            }
        }

        alert(title + "完了しました。");

        if (ispast && editStatus < 3) {
            Search = [];

            Search[Search.length] = _util.hospid;
            Search[Search.length] = -2;
            Search[Search.length] = '';
            Search[Search.length] = _util.patid;
            Search[Search.length] = '';
            Search[Search.length] = '';
            Search[Search.length] = '';
            Search[Search.length] = '1';

            C_WebGetOrderList(Search, setOrderData);
        } else {
            sessionStorage.setItem('userid', _util.userid);
            postForm('./Search.aspx');
        }
    } else {
        alert(title + "に失敗しました。");
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
    }

    changeID($('#patid').val());

    Search = [];

    Search[Search.length] = _util.hospid;
    Search[Search.length] = -2;
    Search[Search.length] = '';
    Search[Search.length] = _util.patid;
    Search[Search.length] = '';
    Search[Search.length] = '';
    Search[Search.length] = '';
    Search[Search.length] = '1';

    if (!_util.orderid || _util.orderid == '')
        _util.orderid = 0;

    C_WebGetOrderList(Search, setOrderData);

    CloseLoading();

}

function setHospConfig(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    if (ret.d.Result) {
        conf = ret.d.Items;
        repVal = [];

        for (var i = 0; i < conf.Conf.length; i++) {
            if (conf.Conf[i].Key == 'PatientID' && $('#patid').val() == '')
                $('#patid').val(conf.Conf[i].Value);
            else if (conf.Conf[i].Key == 'Modality' && $('#modality').val() == '')
                $('#modality').val(conf.Conf[i].Value);
            else if (conf.Conf[i].Key == 'IsVisit' && $('#isvisit').val() == '')
                $('#isvisit').val(conf.Conf[i].Value);
            else if (conf.Conf[i].Key == 'Department' && $('#department').val() == '')
                $('#department').val(conf.Conf[i].Value);
            else if (conf.Conf[i].Key == 'Doctor' && $('#doctor').val() == '')
                $('#doctor').val(conf.Conf[i].Value);
            else if (conf.Conf[i].Key == 'IsFax_Own' && $('#isfax_own')[0].checked == false)
            {
                if(conf.Conf[i].Value == '0')
                    $('#isfax_own')[0].checked = false;
                else
                    $('#isfax_own')[0].checked = true;
            }
            if (conf.Conf[i].Key == 'IsMail_Own' && $('#ismail_own')[0].checked == false) {
                if (conf.Conf[i].Value == '0')
                    $('#ismail_own')[0].checked = false;
                else
                    $('#ismail_own')[0].checked = true;
            }
            else if (conf.Conf[i].Key == 'IsFax' && $('#isfax')[0].checked == false) {
                if (conf.Conf[i].Value == '0'){
                    $('#isfax')[0].checked = false;
                    $('#ihosp').attr('disabled', 'disabled');
                    $('#ihosp_number').attr('disabled', 'disabled');
                }
                else {
                    $('#isfax')[0].checked = true;
                    $('#ihosp').removeAttr('disabled');
                    $('#ihosp_number').removeAttr('disabled');

                    var cd = $('#hosp option:selected').text();
                    var list = cd.split(' ');

                    C_GetFaxSender(list[0], setFaxSenList);
                }
            }
            else if (conf.Conf[i].Key == 'IsMail' && $('#ismail')[0].checked == false) {
                if (conf.Conf[i].Value == '0'){
                    $('#ismail')[0].checked = false;
                    $('#ihosp').attr('disabled', 'disabled');
                    $('#ihosp_number').attr('disabled', 'disabled');
                }
            else {
                    $('#ismail')[0].checked = true;
                    $('#ihosp').removeAttr('disabled');

                    var cd = $('#hosp option:selected').text();
                    var list = cd.split(' ');

                    C_GetMailSender(list[0], setMailSenList);
                }
            }
            else if (conf.Conf[i].Key == 'StudyType' && $('#studytype').val() == '')
                $('#studytype').val(conf.Conf[i].Value);
            else if (conf.Conf[i].Key == 'BodyPart' && $('#bodypart').val() == '')
                $('#bodypart').val(conf.Conf[i].Value);
            else if (conf.Conf[i].Key == 'Comment' && $('#comment').val() == '')
                $('#comment').val(conf.Conf[i].Value);
            else if (conf.Conf[i].Key == 'Contact' && $('#contact').val() == '')
                $('#contact').val(conf.Conf[i].Value);
            else if (conf.Conf[i].Key == 'Recept' && $('#recept').val() == '')
                $('#recept').val(conf.Conf[i].Value);
            else if (conf.Conf[i].Key == 'IsEmergency' &&  $('#isemergency')[0].checked == false)
            {
                if(conf.Conf[i].Value == '0')
                    $('#isemergency')[0].checked = false;
                else
                    $('#isemergency')[0].checked = true;
            }
            else if (conf.Conf[i].Key == 'MemoUser' + _util.userid)
                $('#memo').val(conf.Conf[i].Value);
            else if (conf.Conf[i].Key == 'Memo')
                $('#alert').val(conf.Conf[i].Value);
            else if (conf.Conf[i].Key.indexOf('C_') >= 0) {
                var key = conf.Conf[i].Key.replace('C_', '');

                if (conf.Conf[i].Value == 0){
                    $('#' + key + "-label").removeClass('error-label');
                    $('#' + key).removeAttr('disabled');
                }
                else if(conf.Conf[i].Value == 1)
                {
                    $('#' + key + "-label").addClass('error-label');
                    $('#' + key).removeAttr('disabled');
                }
                else if (conf.Conf[i].Value == 2) {
                    $('#' + key + "-label").removeClass('error-label');
                    $('#' + key).attr('disabled', 'disabled');
                }

            }
            else if (conf.Conf[i].Key.indexOf('T_') >= 0) {
                var key = conf.Conf[i].Key.replace('T_', '');

                $('#' + key + "-label").text(conf.Conf[i].Value);
            }
            else if (conf.Conf[i].Key.indexOf('R_Item') >= 0) {
                repVal[repVal.length] = conf.Conf[i].Value;
            }
}

        setModColor($('#modality'), $('#modality').val());

    }
    CloseLoading();
}

function replaceVal(id) {
    if (!repVal || repVal.length == 0)
        return;

    var isClear = false;

    for(var i = 0; i< repVal.length; i++)
    {
        var dats = repVal[i].split('@');

        if (dats[0] != id)
            continue;

        var srcVal = $('#' + id).val();
        var dstVal = $('#' + dats[2]).val();

        if (dats.length > 4 && !isClear) {
            $('#' + dats[2]).val('');
            dstVal = "";
            isClear = true;
        }

        if (dats[1] == "")
        {
            $('#' + dats[2]).val('');
        }
        else
        {
            var keyList = dats[1].split(':');
            var isVal = false;

            for (var j = 0; j < keyList.length; j++)
            {
                if (srcVal.indexOf(keyList[j]) >= 0)
                {
                    if (dstVal.indexOf(dats[3]) < 0)
                        $('#' + dats[2]).val(dats[3] + " " + dstVal);

                    break;
                }
            }

        }
    }
}

function setOrderData(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    if (ret.d.Result) {
        var order = ret.d.Items;
        past = [];

        for (var i = 0; i < order.length; i++)
            if (order[i].OrderID == _util.orderid) {
                _util.hospid = order[i].HospID;
                if (_util.hospid != '') {
                    C_GetHospTempList(setHospTempList);
                    C_GetHospConfig(setHospConfig);
                }

                $('#status').data('value', order[i].Status);
                switch(order[i].Status)
                {
                    case -1:
                        $('#status').val('未入力');
                        break;
                    case 0:
                        $('#status').val('一時保存');
                        break;
                    case 1:
                        $('#status').val('登録');
                        $('#save').css('visibility', 'hidden');
                        break;
                    case 2:
                        $('#status').val('出力');
                        $('#send').css('visibility', 'hidden');
                        $('#save').css('visibility', 'hidden');
                        break;
                    case 3:
                        $('#status').val('削除');
                        $('#order').css('visibility', 'hidden');
                        $('#send').css('visibility', 'hidden');
                        $('#save').css('visibility', 'hidden');
                        $('#delete_preorder').css('visibility', 'hidden');
                        break;
                }

                $('#orderno').val(order[i].OrderNo);
                $('#hosp').val(order[i].HospID);
                $('#patid').val(order[i].PatID);
                $('#patname').val(order[i].PatName);
                $('#patname_h').val(order[i].PatName_H);
                $('#birthday').val(formatDate(order[i].BirthDay));
                $('#modality').val(order[i].Modality);
                setModColor($('#modality'), $('#modality').val());
                $('#studydate').val(formatDate(order[i].Date));
                $('#studytime').val(formatTime(order[i].Time));

                $('#isvisit').val(order[i].IsVisit).attr('title', order[i].IsVisit);
                $('#department').val(order[i].Department).attr('title', order[i].Department);
                $('#doctor').val(order[i].Doctor).attr('title', order[i].Doctor);

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

                if (order[i].Status == -1) {
                    replaceVal('patid');
                    replaceVal('patname');
                    replaceVal('patname_h');
                    replaceVal('birthday');
                    replaceVal('modality');
                    replaceVal('studydate');
                    replaceVal('studytime');
                    replaceVal('isvisit');
                    replaceVal('department');
                    replaceVal('doctor');
                    replaceVal('age');
                    replaceVal('sex');
                    replaceVal('bodypart');
                    replaceVal('studytype');
                    replaceVal('comment');
                    replaceVal('contact');
                    replaceVal('recept');
                }

                mainorder = order[i];
                past[past.length] = order[i];
            }
            else if (_util.hospid == order[i].HospID)
                past[past.length] = order[i];


        C_WebGetFileList(setOrderFiles);
        C_GetSender($('#orderno').val(), setSender);
    }
}
function setSender(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    if(ret.d == "") {
        CloseLoading();
        return;
    }

    var vals = ret.d.split(',');

    if(vals.length < 6)
    {
        CloseLoading();
        return;
    }

    if (vals[0] == "0")
        $('#isfax_own')[0].checked = true;
    else if(vals[0] == "1")
        $('#ismail_own')[0].checked = true;
    else if (vals[0] == "2") {
        $('#isfax_own')[0].checked = true;
        $('#ismail_own')[0].checked = true;
    }

    if (vals[1] == "0")
    {
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
function viewPastFiles(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    var Files = ret.d.Items;

    var isThumb = false;
    var OriName = '';
    var ThumbName = '';
    var orderID = '';

    if (Files.length > 0) {
        for (var i = 0; i < Files.length; i++) {
            if (Files[i].IsOrigin == 1) {
                orderID = Files[i].OrderID;
                OriName = Files[i].Name;
            } else {
                var val = Files[i].Name.replace('C_', '');
                ThumbName = val;
                isThumb = true;
            }
        }
    }
    else {
        alert('保存している依頼票がありません。');
        return;
    }

    //if(isThumb)
    //    C_GetImage(2, orderID, 0, ThumbName, viewPast);
    //else
        C_GetImage(2, orderID, 1, OriName, viewPast);

}

function setOrderFiles(ret)
{
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    var Files = ret.d.Items;

    if(Files.length > 0)
    {
        for (var i = 0; i < Files.length; i++)
        {
            if(Files[i].IsOrigin == 1) {
                $('#orgImg').data('name', Files[i].Name);
                $('#orgImg_del').css('visibility', 'visible');
                $('#orgImg').css('visibility', 'visible');
            }else{
                var val = Files[i].Name.replace('C_', '');
                $('#thumbnail').data('value', val);
                var status = $('#status').data('value');
                C_GetImage(status, _util.orderid, 0, val, setThumbnail);
            }
        }
    }

    setPastOrder();
}

function setPastOrder() {
    $('#study_table tbody').remove();

    //if (_util.orderid == '' && past.length == 0) {
    //    _util.patid = $('#patid').val();
    //    ShowLoading();
    //    C_WebGetPreList(setOrderData);
    //}
    var bodyRow = $('<tbody>');

    if (!_util.orderid || _util.orderid == '' || _util.orderid == 0) {
        var tmprow = $('<tr>');
        tmprow.append($('<td>').addClass('nowcolor').attr('id', 'p_status').append($('#status').val()));
        tmprow.append($('<td>').addClass('nowcolor').attr('id', 'p_studydate').append($('#studydate').val()));
        tmprow.append($('<td>').attr('id', 'p_modality').append($('#modality').val()).addClass('Color_' + $('#modality').val()));

        if ($('#bodypart').val().length > 5)
            tmprow.append($('<td>').addClass('nowcolor').attr('id', 'p_bodypart').append($('#bodypart').val().substring(0, 5) + "...").attr('title', $('#bodypart').val()));
        else
            tmprow.append($('<td>').addClass('nowcolor').attr('id', 'p_bodypart').append($('#bodypart').val()));

        if ($('#isemergency')[0].checked)
            tmprow.append($('<td>').addClass('nowcolor').attr('id', 'p_isemergency').append("緊急").addClass('alert-data-val'));
        else
            tmprow.append($('<td>').addClass('nowcolor').attr('id', 'p_isemergency').append("通常"));
        tmprow.append($('<td>').addClass('nowcolor'));
        tmprow.append($('<td>').addClass('nowcolor'));
        tmprow.append($('<td>').addClass('nowcolor'));
        bodyRow.append(tmprow);
    }

    for (var i = 0; i < past.length; i++) {
        var tmprow = $('<tr>');

        if (_util.orderid == past[i].OrderID)
        {
            switch (mainorder.Status) {
                case -1:
                    tmprow.append($('<td>').addClass('nowcolor').attr('id', 'p_status').append('未入力'));
                    break;
                case 0:
                    tmprow.append($('<td>').addClass('nowcolor').attr('id', 'p_status').append('一時保存'));
                    break;
                case 1:
                    tmprow.append($('<td>').addClass('nowcolor').attr('id', 'p_status').append('登録'));
                    break;
                case 2:
                    tmprow.append($('<td>').addClass('nowcolor').attr('id', 'p_status').append('出力'));
                    break;
                case 3:
                    tmprow.append($('<td>').addClass('nowcolor').attr('id', 'p_status').append('削除'));
                    break;
                default:
                    continue;
                    break;
            }

            tmprow.append($('<td>').addClass('nowcolor').attr('id', 'p_studydate').append(formatDate(mainorder.Date)));
            tmprow.append($('<td>').attr('id', 'p_modality').append(mainorder.Modality).addClass('Color_' + mainorder.Modality));

            if (mainorder.BodyPart.length > 5)
                tmprow.append($('<td>').addClass('nowcolor').attr('id', 'p_bodypart').append(mainorder.BodyPart.substring(0, 5) + "...").attr('title', mainorder.BodyPart));
            else
                tmprow.append($('<td>').addClass('nowcolor').attr('id', 'p_bodypart').append(mainorder.BodyPart));

            if (mainorder.IsEmergency == 1)
                tmprow.append($('<td>').addClass('nowcolor').attr('id', 'p_isemergency').append("緊急").addClass('alert-data-val'));
            else
                tmprow.append($('<td>').addClass('nowcolor').attr('id', 'p_isemergency').append("通常"));
            tmprow.append($('<td>').addClass('nowcolor'));
            tmprow.append($('<td>').addClass('nowcolor'));
            tmprow.append($('<td>').addClass('nowcolor'));
        } else {
            switch (past[i].Status) {
                case -1:
                    tmprow.append($('<td>').append('未入力'));
                    break;
                case 0:
                    tmprow.append($('<td>').append('一時保存'));
                    break;
                case 1:
                    tmprow.append($('<td>').append('登録'));
                    break;
                case 2:
                    tmprow.append($('<td>').append('出力'));
                    break;
                case 3:
                    tmprow.append($('<td>').append('削除'));
                    break;
                default:
                    continue;
                    break;
            }

            tmprow.append($('<td>').append(formatDate(past[i].Date)));
            tmprow.append($('<td>').append(past[i].Modality).addClass('Color_' + past[i].Modality));

            if (past[i].BodyPart.length > 5)
                tmprow.append($('<td>').append(past[i].BodyPart.substring(0, 5) + "...").attr('title', past[i].BodyPart));
            else
                tmprow.append($('<td>').append(past[i].BodyPart));

            if (past[i].IsEmergency == 1)
                tmprow.append($('<td>').append("緊急").addClass('alert-data-val'));
            else
                tmprow.append($('<td>').append("通常"));

            if (past[i].Status < 2)
                tmprow.append($('<td>').addClass('dbody').append($('<input>').attr('type', 'button').addClass('btn-sm edit').data('id', past[i].OrderID).attr('value', '入力')));
            else
                tmprow.append($('<td>'));

            tmprow.append($('<td>').addClass('dbody').append($('<input>').attr('type', 'button').addClass('btn-sm view').data('id', past[i].OrderID).attr('value', '参照')));

            var ret = C_WebGetFileList_Past_Async(past[i].OrderID);

            if (ret && ret.d) {
                var Files = ret.d.Items;
                if (Files.length > 0) {
                    tmprow.append($('<td>').addClass('dbody').append($('<input>').attr('type', 'button').addClass('btn-sm viewImg').data('id', past[i].OrderID).attr('value', '依頼票')));
                }
                else {
                    tmprow.append($('<td>'));
                }
            } else {
                tmprow.append($('<td>'));
            }
        }
        bodyRow.append(tmprow);
    }

    $('#study_table').append(bodyRow);
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

function changeID(id) {
    for (var i = 0; i < patient.length; i++) {
        if (patient[i].PatID == id) {
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

function setAge() {
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

String.prototype.times = function (len) {
    return len < 1 ? '' : new Array(++len).join(this.substr(0, 1) || '');
}

String.prototype.padRight = function (len, chr) {
    return this + chr.times(len - this.length);
}

String.prototype.padLeft = function (len, chr) {
    return chr.times(len - this.length) + this;
}

function CheckTime() {
    var time = $('#studytime').val();

    time = time.replace(/：/g, ':');
    time = time.replace(/:/g, '');

    if (time.length < 6)
        time = time.padLeft(4, '0').padRight(6, '0');
    else if (time.length > 6)
        time = time.substring(0, 6);
    if(!ValidTime(time))
    {
        alert('検査時刻に正しくない時刻が記載されています。');
        $('#studytime').focus();
    }
}

function ValidTime(str) {
    var h = parseInt(str.substring(0, 2), 10);
    var m = parseInt(str.substring(2, 4), 10);
    var s = parseInt(str.substring(4, 6), 10);
    tm = new Date('1970', 0, 1, h, m, s);
    return (tm.getHours() == h && tm.getMinutes() == m && tm.getSeconds() == s);
}

function C_CheckString(val) {
    var ret = "";

    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebCheckString",
        data: "{val:'" + val + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            ret = result.d;
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
            CloseLoading();
        }
    });

    return ret;
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

function C_SetHospTempList(key, val, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebSetHospTempList",
        data: "{ id:" + _util.hospid + ", key:'" + key + "', values:" + castJson(val) + "}",
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

function C_SetHospConfig(key, val, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebSetHospConfig",
        data: "{ id:" + _util.hospid + ", key:'" + key + "', values:'" + val + "'}",
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

function C_GetPatList(func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetPatList",
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

function C_WebGetFileList(func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetFileList",
        data: "{ orderid:" + _util.orderid + "}",
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
function C_WebGetFileList_Past(id, func) {
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
            CloseLoading();
        }
    });
}
function C_WebGetFileList_Past_Async(id) {
    var ret;
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetFileList",
        data: "{ orderid:" + id + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            ret = result;
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
            CloseLoading();
            ret = null;
        }
    });

    return ret;
}
function C_SetPreOrder(status, val, func) {
    val[val.length] = _util.userid;
    val[val.length] = status;

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

function C_SetOriginal(isEm, isNew, val, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebSetOriginal",
        data: "{ userid:" + _util.userid + ", isEm:" + isEm + ", isNew:" + isNew + ", value:'" + val + "'}",
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

function C_SetCutImg(val, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebSetCutImg",
        data: "{ userid:" + _util.userid + ", value:'" + val + "'}",
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

function C_GetImage(status, orderid, isOrigin, val, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetImage",
        data: "{ status:" + status + ", orderid:" + orderid + ", userid:" + _util.userid + ", isOrigin:" + isOrigin + ", value:'" + val + "'}",
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

function C_ResetImage(status, orderid, isOrigin, val, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebResetImage",
        data: "{ status:" + status + ", orderid:" + orderid + ", userid:" + _util.userid + ", isOrigin:" + isOrigin + ", value:'" + val + "'}",
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

function C_GetFaxSender(cd, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetFaxSender",
        data: "{ hospCd:'" + cd + "'}",
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

function C_GetMailSender(cd, func) {
    $.ajax({
        async: false,
        cache: false,
        type: "POST",
        url: "./OrderTool_WS.asmx/WebGetMailSender",
        data: "{ hospCd:'" + cd + "'}",
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
    sessionStorage.setItem('param', _util.param);

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

function _toKatakana(src) {
    var c, i, str;
    str = new String;
    for (i = 0; i < src.length; i++) {
        c = src.charCodeAt(i);
        if (_isHiragana(c)) {
            str += String.fromCharCode(c + 96);
        } else {
            str += src.charAt(i);
        }
    }
    return _toHarf(str);
}

function _isHiragana(chara) {
    return ((chara >= 12353 && chara <= 12435) || chara == 12445 || chara == 12446);
}

function _toHarf(src) {
    var i, f, c, m, a = [];

    m =
    {
        0x30A1: 0xFF67, 0x30A3: 0xFF68, 0x30A5: 0xFF69, 0x30A7: 0xFF6A, 0x30A9: 0xFF6B,
        0x30FC: 0xFF70, 0x30A2: 0xFF71, 0x30A4: 0xFF72, 0x30A6: 0xFF73, 0x30A8: 0xFF74,
        0x30AA: 0xFF75, 0x30AB: 0xFF76, 0x30AD: 0xFF77, 0x30AF: 0xFF78, 0x30B1: 0xFF79,
        0x30B3: 0xFF7A, 0x30B5: 0xFF7B, 0x30B7: 0xFF7C, 0x30B9: 0xFF7D, 0x30BB: 0xFF7E,
        0x30BD: 0xFF7F, 0x30BF: 0xFF80, 0x30C1: 0xFF81, 0x30C4: 0xFF82, 0x30C6: 0xFF83,
        0x30C8: 0xFF84, 0x30CA: 0xFF85, 0x30CB: 0xFF86, 0x30CC: 0xFF87, 0x30CD: 0xFF88,
        0x30CE: 0xFF89, 0x30CF: 0xFF8A, 0x30D2: 0xFF8B, 0x30D5: 0xFF8C, 0x30D8: 0xFF8D,
        0x30DB: 0xFF8E, 0x30DE: 0xFF8F, 0x30DF: 0xFF90, 0x30E0: 0xFF91, 0x30E1: 0xFF92,
        0x30E2: 0xFF93, 0x30E4: 0xFF94, 0x30E6: 0xFF95, 0x30E8: 0xFF96, 0x30E9: 0xFF97,
        0x30EA: 0xFF98, 0x30EB: 0xFF99, 0x30EC: 0xFF9A, 0x30ED: 0xFF9B, 0x30EF: 0xFF9C,
        0x30F2: 0xFF66, 0x30F3: 0xFF9D, 0x3000: 0x0020, 0x30E3: 0xFF6C, 0x30E5: 0xFF6D,
        0x30E7: 0xFF6E, 0x30C3: 0xFF6F
    };

    for (i = 0, f = src.length; i < f; i++) {
        c = src.charCodeAt(i);
        switch (true) {
            case (c in m):
                a.push(m[c]);
                break;
            case (0xFF21 <= c && c <= 0xFF5E):
                a.push(c - 0xFEE0);
                break;
                // ガ−ド
            case (0x30AB <= c && c <= 0x30C9):
                a.push(m[c - 1], 0xFF9E);
                break;
                // ハバパ−ホボポの濁点と半濁点
            case (0x30CF <= c && c <= 0x30DD):
                a.push(m[c - c % 3], [0xFF9E, 0xFF9F][c % 3 - 1]);
                break;
            default:
                a.push(c);
                break;
        };
    };

    return String.fromCharCode.apply(null, a);

}

function convBirth(str) {
    if (!str)
        return str;

    var ret;

    var list = str.split('/');
    ret = convYear(list[0]) + '/' + ('0' + list[1]).slice(-2) + '/' + ('0' + list[2]).slice(-2);

    return ret;
}

function convYear(str) {
    if (!str || str.length < 2)
        return str;

    var n = str.substring(0, 1);
    var y = parseInt(str.substring(1, 3));
    var s;

    if (!y)
        return str;

    if (((n == "h") || (n == "H")) && (y > 0)) {
        s = (y + 1988);
    } else if (((n == "s") || (n == "S")) && (y > 0) && (y <= 64)) {
        s = (y + 1925);
    } else if (((n == "t") || (n == "T")) && (y > 0) && (y <= 15)) {
        s = (y + 1911);
    } else if (((n == "m") || (n == "M")) && (y > 0) && (y <= 45)) {
        s = (y + 1867);
    } else {
        s = str;
    }

    return s;
}