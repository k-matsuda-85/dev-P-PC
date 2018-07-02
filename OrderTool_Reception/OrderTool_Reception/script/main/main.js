// ロード処理
$(function () {

    // セッション確認
    if (!_util.userid || _util.userid == '') {
        _util.userid = sessionStorage.getItem('userid');
        _util.hospid = sessionStorage.getItem('hospid');

    }

    $('#hosp').on('change', function (e) {
        _util.hospid = $('#hosp').val();

        $("#alert").val("");
        $("#memo").val("");

        if ($('#hosp').val() == "")
            return;

        C_GetHospConfig(setHospConfig);
    });

    $('#memo-setting').on('click', function (e) {
        if (_util.hospid && _util.hospid != '0') {
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

    C_GetHospList(setHospList);

});

function postForm(url) {
    var $form = $('<form/>', { 'action': url, 'method': 'post' });
    $form.appendTo(document.body);
    $form.submit();
};
function setHospList(ret) {
    if (!ret || !ret.d) {
        CloseLoading();
        return;
    }

    if (ret.d.Result) {
        var hosp = ret.d.Items;

        for (var i = 0; i < hosp.length; i++) {
            if (hosp[i].CD == 'KPM')
                continue;

            $('#hosp').append($('<option>').val(hosp[i].HospID).data('cd', hosp[i].CD).append(hosp[i].CD + " " + hosp[i].Name));
        }

    }
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
            if (conf.Conf[i].Key == 'MemoUser' + _util.userid)
                $('#memo').val(conf.Conf[i].Value);
            else if (conf.Conf[i].Key == 'Memo')
                $('#alert').val(conf.Conf[i].Value);
        }
    }
}


function setConfg(ret) {

    alert('設定を更新しました。');
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
        }
    });
}
