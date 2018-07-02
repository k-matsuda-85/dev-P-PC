// グローバル変数
var gNodeLeft;
var gNodeTop;
var gNodeWidth;
var gZindex = 1;

var Doctors = [];
var Schedule = [];
var ColorList = [];
var HoliDays = [];

var isClick = false;
var ckTime = -1;
var $ckRow;
var ckDate = '';
var onNo = 0;
var ckSort = true;
var ckPos = {};
var ckOff = {};
var _util = {};

var SPDays = [];

$(function () {
    if (!_util.user || _util.user == '') {
        _util.user = sessionStorage.getItem('user');
        _util.name = sessionStorage.getItem('name');
    }
    if (!_util.user || _util.user == '') {
        postForm('./Login.aspx');
    }
    C_GetSPDay();
    C_GetDoctorList(setDoctor);
    GetUserMemo(setMemo_U);
    InitHTML();


    //$('#holi-head').exResize(function () {
    //    if (timer !== false) {
    //        clearTimeout(timer);
    //    }
    //    timer = setTimeout(function () {
    //    });
    //});

    $('div.select').each(function () {
        $(this).hover(
            function () {
                if (!isClick)
                    $(this).css('background-color', '#DDFFFF');
            },
            function () {
                if (!isClick)
                    $(this).css('background-color', '#FFFFFF');
            }
        );
    });

    $('#memo_o').on('click', function () {
        if ($('footer').height() == 0)
            $('footer').height(140);
        else
            $('footer').height(0);

        resizeWindow();
    });
    $('#edit-memo-user').on('click', function () {
        if (!confirm('個人メモを更新します。\nよろしいですか？'))
            return;

        C_SetUserMemo($('#user-memo').text());
        GetUserMemo(setMemo_U);
    });

    $('table').on('mousedown', 'tbody td.data-td div.select',
        function () {
            isClick = true;

            $ckRow = $(this).closest('td');
            var $ckDate = $(this).closest('div.t-date');
            ckDate = $ckDate.data('date');

            ckTime = $(this).data('no');
            ckPos = $(this).position();
            ckOff = $(this).offset();

            $(this).css('background-color', '#FFFFFF');
            if ($('#newDate').length == 0)
                $ckRow.append($('<div>').attr('id', 'newDate').css('left', ckPos.left + 'px'))
        }
    );
    $('body').on('mouseup',
        function () {
            if (!isClick)
                return;

            var left = parseInt($('#newDate').css('left').replace('px', ''));
            var width = $('#newDate').width();
            var date = $('#newDate').closest('table.day-body').data('date');
            var dt = new Date(date.substring(0, 4), parseInt(date.substring(4, 6)) - 1, date.substring(6, 8));

            var tid = $('#newDate').closest('table').attr('id');

            var type = -1;

            if (tid.indexOf('-in')>=0)
                type = 0;
            else if (tid.indexOf('-out')>=0)
                type = 1;
            else
                type = 99;

            var start = getStartTimeID(left);
            var end = getEndTimeID(left, width);

            setNewData(dt, start, end, type, -1);

            $('#comment-new input').val('');

            $('#modal-date-new').modal();
            isClick = false;
        }
    );
    $('body').on('mousemove',
        function (e) {
            if (!isClick)
                return;
            var x = e.clientX;
            var width = x - ckOff.left;

            if (width > 0) {
                if (ckPos.left + width <= (26 * 30))
                    $('#newDate').width(width);
            } else {
                if (ckPos.left + width >= 0)
                {
                    $('#newDate').css('left', ckPos.left + width + 'px');
                    $('#newDate').width(width * -1);
                }
            }

            e.preventDefault();
        }
    );

    $('#mail-doc').on('click', function () {

        $('#doc-mail-list').empty();
        $('#doc-mail-list').append($('<option>').attr('value', '').append(''));

        var id = $(this).data('id');
        var color;

        $.each(Doctors, function (index, doc) {
            if (doc.DocID == id)
                color = doc.Color;

            $('#doc-mail-list').append(
                $('<option>').attr('value', doc.DocID).append(doc.DocName).data('color', doc.Color)
            );
        });

        GetMailList(id, setMailList);

        $('#modal-doc-mast').modal('hide');

        $('#modal-mail').modal();
        $('#doc-mail-list').val(id);
        $('#modal-mail div div div.modal-header').css('background-color', color);
        GetMailMaster($('#doc-mail-list').val(), setMasterMail);

    });

    $('#mail').on('click', function () {

        $('#doc-mail-list').empty();
        $('#doc-mail-list').append($('<option>').attr('value', '').append(''));
        $.each(Doctors, function (index, doc) {
            $('#doc-mail-list').append(
                $('<option>').attr('value', doc.DocID).append(doc.DocName).data('color', doc.Color)
            );
        });

        $('#mail-to input').data('cd', '');
        $('#mail-to input').val('');

        GetMailList('', setMailList);
        $('#modal-mail div div div.modal-header').css('background-color', 'white');

        $('#modal-mail').modal();
    });

    $('#mail-list').on('change', function () {
        if($('#doc-mail-list').val() != '')
        {
            getMailData();
        }
    });

    $('#doc-mail-list').on('change', function () {
        GetMailMaster($('#doc-mail-list').val(), setMasterMail);
        GetMailList($(this).val(), setMailList);

        $('#mail-title input').val('');
        $('#mail-cc input').val('');
        $('#mail-main textarea').text('');

        //if ($('#mail-list').val() != '') {
        //    getMailData();
        //} else {
        //    GetMailList($(this).val(), setMailList);
        //}
    });

    $('#send-mail').on('click', function () {
        if ($('#mail-to input').val() == '')
        {
            alert('あて先を指定してください。');
            return;
        }

        if ($('#mail-title input').val() == '') {
            if (!confirm('件名が入力されていません。無題で送信しますか？'))
                return;
        }

        if (!confirm('この内容でメールを送信します。よろしいですか？'))
            return;

        var vals = [];
        vals[vals.length] = $('#mail-to input').val();
        vals[vals.length] = $('#mail-cc input').val();
        //        vals[vals.length] = $('#send-mail').data('bcc');
        vals[vals.length] = '';
        vals[vals.length] = $('#mail-title input').val();
        vals[vals.length] = $('#mail-main textarea').text();

        C_SendMail(vals, endSend);
    });

    $('#edit-today').on('click', function () {
        var id = $(this).data('sid');
        var subid = $(this).data('subid');
        var sc;

        $.each(Schedule, function (idx, sch) {
            $.each(sch.Data, function (idx, dat) {
                if (dat.ID == id) {
                    if (subid && subid != '')
                    {
                        $.each(dat.SubDays, function (idx, sub) {
                            if (sub.ID == subid) {
                                sc = sub;
                                return false;
                            }
                        });
                    } else {
                        sc = dat;
                    }
                    return false;
                }
            });
            if (sc)
                return false;
        });

        var date = sc.Date;
        var dt = new Date(date.substring(0, 4), parseInt(date.substring(4, 6)) - 1, date.substring(6, 8));

        var stime = -2;
        var etime = -2;

        $('#time-from-new select').children('option').each(function () {
            if ($(this).text() == sc.Stime)
                stime = $(this).val();
            else if($(this).text() == sc.Etime)
                etime = $(this).val();
        });

        setEditData(sc.DocID, sc.ID, dt, stime, etime, sc.Count, sc);

        if (sc.SubType == 0 || sc.SubType == 1)
            $('#change-doc-one').attr('disabled', 'disabled').attr('readonly', 'readonly');
        else
            $('#change-doc-one').removeAttr('disabled').removeAttr('readonly');


        $('#modal-date-edit').modal();
    });

    $('table').on('click', 'tbody td div.egrid span',
        function () {
            var id = $(this).data('id');
            var date = $(this).data('date');
            var update = $(this).data('update');
            var sid = $(this).data('sid');
            var subid = $(this).data('subid');

            if (!sid)
                return;

            var ret = setDocData(id, date, update, sid, subid);
            if(ret == 0)
                $('#modal-doc-mast').modal();
            else {
                var id = $(this).data('sid');
                var subid = '';
                var sc;

                $.each(Schedule, function (idx, sch) {
                    $.each(sch.Data, function (idx, dat) {
                        if (dat.ID == id) {
                            sc = dat;
                            return false;
                        }
                    });
                    if (sc)
                        return false;
                });

                var date = sc.Date;
                var dt = new Date(date.substring(0, 4), parseInt(date.substring(4, 6)) - 1, date.substring(6, 8));

                var stime = -2;
                var etime = -2;

                $('#time-from-new select').children('option').each(function () {
                    if ($(this).text() == sc.Stime)
                        stime = $(this).val();
                    else if ($(this).text() == sc.Etime)
                        etime = $(this).val();
                });

                setEditData(sc.DocID, sc.ID, dt, stime, etime, sc.Count, sc);

                $('#change-doc-one').attr('disabled', 'disabled').attr('readonly', 'readonly');

                $('#modal-date-edit').modal();

            }
        }
    ).on('click', 'div.t-holiday-body', function () {

        var date = $(this).closest('tr').find('table.day-body').data('date');
        var dt = new Date(date.substring(0, 4), parseInt(date.substring(4, 6)) - 1, date.substring(6, 8));

        setHoliData(dt);

        $('#modal-date-holi').modal();
    });

    $('#today').on('click', function () {
        var date = new Date();
        var formattedDate =
            date.getFullYear() + '/' + ('0' + (date.getMonth() + 1)).slice(-2) + '/' + ('0' + date.getDate()).slice(-2);

        $("#date").datepicker("setDate", formattedDate);
        C_GetDoctorList(setDoctor);
    });

    $('#before').on('click', function () {
        var date = new Date($('#date').val());

        date.setDate(date.getDate() - 7)

        var formattedDate =
            date.getFullYear() + '/' + ('0' + (date.getMonth() + 1)).slice(-2) + '/' + ('0' + date.getDate()).slice(-2);

        $("#date").datepicker("setDate", formattedDate);
        C_GetDoctorList(setDoctor);

    });
    $('#after').on('click', function () {
        var date = new Date($('#date').val());

        date.setDate(date.getDate() + 7)
        var formattedDate =
            date.getFullYear() + '/' + ('0' + (date.getMonth() + 1)).slice(-2) + '/' + ('0' + date.getDate()).slice(-2);

        $("#date").datepicker("setDate", formattedDate);
        C_GetDoctorList(setDoctor);

    });

    $('#date').on('change', function () {
        var date = new Date($('#date').val());

        var formattedDate =
            date.getFullYear() + '/' + ('0' + (date.getMonth() + 1)).slice(-2) + '/' + ('0' + date.getDate()).slice(-2);

        InitHTML(0, date);
        ShowLoading();
        GetScheduleList($('#date').val(), setDay);
        C_GetDoctorList(setDoctor);

    });


    $('body').on("selectstart", function () {
        if (isClick)
            return false;
    });

    var timer = false;

    $(window).resize("resize", function () {
        if (timer !== false) {
            clearTimeout(timer);
        }
        timer = setTimeout(function () {
            resizeWindow();
            resizeTable();
        });
    });

    $('#modal-doc-edit').on('hidden.bs.modal', function () {
        $('#newDate').remove();
    });

    $('#edit-doc').on('click', function () {
    });

    setColor();

    $('#mast-doctor').on('click', 'li', function () {
        var id = $(this).data('id');
        if (!id)
            return;

        C_GetDocSchdule(id, setDocEditData);
    });
    $('#edit-doc').on('click',function () {
        var id = $(this).data('id');
        if (!id)
            return;

        $('#modal-doc-mast').modal('hide');
        C_GetDocSchdule(id, setDocEditData);
    });

    $('#modal-doc-edit').on('blur', '#name-e', function () {
        $('#mast-title-e span').empty();
        $('#mast-title-e span').append($('#name-e input').val());
    }).on('click', '#OK-add', function () {
        var ids = [];
        $('#body-mast p input').each(function () {
            if ($(this)[0].checked == true) {
                ids[ids.length] = $(this).val();
                $(this).closest('p').addClass('OK');
                $(this).closest('p').removeClass('Other');
                $(this).closest('p').removeClass('NG');
                $(this)[0].checked = false;
            }
        });

        for (var i = 0; i < ids.length; i++) {
            var isDat = false;
            $('#mast-sub-table-OK-e').children('li').each(function () {
                if (ids[i] == $(this).data('id')) {
                    isDat = true;
                    return;
                }
            });

            if (!isDat) {
                $('#mast-sub-table-NG-e').children('li').each(function () {
                    if ($(this).data('id') == ids[i]) {
                        $(this).remove();
                        return;
                    }
                });
                $('#mast-sub-table-Other-e').children('li').each(function () {
                    if ($(this).data('id') == ids[i]) {
                        $(this).remove();
                        return;
                    }
                });

                var text = '';

                for (var j = 0; j < BodyPartList.length; j++)
                    if(BodyPartList[j].Id == ids[i])
                    {
                        text = BodyPartList[j].Text;
                        break;
                    }

                $('#mast-sub-table-OK-e').find('input.last-data').each(function () {
                    $(this).closest('li').remove();
                });

                $('#mast-sub-table-OK-e').append($('<li>').addClass('list-group-item').append($('<input>').val(text).addClass('form-control')).data('id', ids[i]));
                $('#mast-sub-table-OK-e').append($('<li>').addClass('list-group-item').append($('<input>').addClass('form-control last-data')).data('id', 6));
            }
        }
    }).on('click', '#OK-del', function () {
        var ids = [];
        $('#body-mast p input').each(function () {
            if ($(this)[0].checked == true) {
                ids[ids.length] = $(this).val();
                $(this).closest('p').removeClass('OK');
                $(this)[0].checked = false;
            }
        });

        for (var i = 0; i < ids.length; i++) {
            var isDat = false;
            $('#mast-sub-table-OK-e').children('li').each(function () {
                if (ids[i] == $(this).data('id')) {
                    isDat = true;
                    return;
                }
            });

            if (isDat) {
                var text = '';
                $('#mast-sub-table-OK-e').children('li').each(function () {
                    if ($(this).data('id') == ids[i])
                    {
                        $(this).remove();
                        return;
                    }
                });
            }
        }
    }).on('click', '#NG-add', function () {
        var ids = [];
        $('#body-mast p input').each(function () {
            if ($(this)[0].checked == true) {
                ids[ids.length] = $(this).val();
                $(this).closest('p').addClass('NG');
                $(this).closest('p').removeClass('OK');
                $(this).closest('p').removeClass('Other');
                $(this)[0].checked = false;
            }
        });

        for (var i = 0; i < ids.length; i++) {
            var isDat = false;
            $('#mast-sub-table-NG-e').children('li').each(function () {
                if (ids[i] == $(this).data('id')) {
                    isDat = true;
                    return;
                }
            });

            if (!isDat) {
                $('#mast-sub-table-OK-e').children('li').each(function () {
                    if ($(this).data('id') == ids[i]) {
                        $(this).remove();
                        return;
                    }
                });
                $('#mast-sub-table-Other-e').children('li').each(function () {
                    if ($(this).data('id') == ids[i]) {
                        $(this).remove();
                        return;
                    }
                });

                var text = '';

                for (var j = 0; j < BodyPartList.length; j++)
                    if (BodyPartList[j].Id == ids[i]) {
                        text = BodyPartList[j].Text;
                        break;
                    }

                $('#mast-sub-table-NG-e').find('input.last-data').each(function () {
                    $(this).closest('li').remove();
                });

                $('#mast-sub-table-NG-e').append($('<li>').addClass('list-group-item').append($('<input>').val(text).addClass('form-control')).data('id', ids[i]));
                $('#mast-sub-table-NG-e').append($('<li>').addClass('list-group-item').append($('<input>').addClass('form-control last-data')).data('id', 6));
            }
        }
    }).on('click', '#NG-del', function () {
        var ids = [];
        $('#body-mast p input').each(function () {
            if ($(this)[0].checked == true) {
                ids[ids.length] = $(this).val();
                $(this).closest('p').removeClass('NG');
                $(this)[0].checked = false;
            }
        });

        for (var i = 0; i < ids.length; i++) {
            var isDat = false;
            $('#mast-sub-table-NG-e').children('li').each(function () {
                if (ids[i] == $(this).data('id')) {
                    isDat = true;
                    return;
                }
            });

            if (isDat) {
                var text = '';
                $('#mast-sub-table-NG-e').children('li').each(function () {
                    if ($(this).data('id') == ids[i]) {
                        $(this).remove();
                        return;
                    }
                });
            }
        }
    }).on('click', '#Other-add', function () {
        var ids = [];
        $('#body-mast p input').each(function () {
            if ($(this)[0].checked == true) {
                ids[ids.length] = $(this).val();
                $(this).closest('p').addClass('Other');
                $(this).closest('p').removeClass('OK');
                $(this).closest('p').removeClass('NG');
                $(this)[0].checked = false;
            }
        });

        for (var i = 0; i < ids.length; i++) {
            var isDat = false;
            $('#mast-sub-table-Other-e').children('li').each(function () {
                if (ids[i] == $(this).data('id')) {
                    isDat = true;
                    return;
                }
            });

            if (!isDat) {
                $('#mast-sub-table-OK-e').children('li').each(function () {
                    if ($(this).data('id') == ids[i]) {
                        $(this).remove();
                        return;
                    }
                });
                $('#mast-sub-table-NG-e').children('li').each(function () {
                    if ($(this).data('id') == ids[i]) {
                        $(this).remove();
                        return;
                    }
                });

                var text = '';

                for (var j = 0; j < BodyPartList.length; j++)
                    if (BodyPartList[j].Id == ids[i]) {
                        text = BodyPartList[j].Text;
                        break;
                    }

                $('#mast-sub-table-Other-e').find('input.last-data').each(function () {
                    $(this).closest('li').remove();
                });

                $('#mast-sub-table-Other-e').append($('<li>').addClass('list-group-item').append($('<input>').val(text).addClass('form-control')).data('id', ids[i]));
                $('#mast-sub-table-Other-e').append($('<li>').addClass('list-group-item').append($('<input>').addClass('form-control last-data')).data('id', 6));
            }
        }
    }).on('click', '#Other-del', function () {
        var ids = [];
        $('#body-mast p input').each(function () {
            if ($(this)[0].checked == true) {
                ids[ids.length] = $(this).val();
                $(this).closest('p').removeClass('Other');
                $(this)[0].checked = false;
            }
        });

        for (var i = 0; i < ids.length; i++) {
            var isDat = false;
            $('#mast-sub-table-Other-e').children('li').each(function () {
                if (ids[i] == $(this).data('id')) {
                    isDat = true;
                    return;
                }
            });

            if (isDat) {
                var text = '';
                $('#mast-sub-table-Other-e').children('li').each(function () {
                    if ($(this).data('id') == ids[i]) {
                        $(this).remove();
                        return;
                    }
                });
            }
        }
    }).on('blur', '#mast-sub-table-OK-e li input', function () {
        if($(this).hasClass('last-data') && $(this).val() != '')
        {
            $(this).removeClass('last-data');
            $('#mast-sub-table-OK-e').append($('<li>').addClass('list-group-item').append($('<input>').addClass('form-control last-data')).data('id', 6));
        }else if(!$(this).hasClass('last-data')  && $(this).val() == '')
        {
            $(this).closest('li').remove();
        }
    }).on('blur', '#mast-sub-table-NG-e li input', function () {
        if ($(this).hasClass('last-data') && $(this).val() != '') {
            $(this).removeClass('last-data');
            $('#mast-sub-table-NG-e').append($('<li>').addClass('list-group-item').append($('<input>').addClass('form-control last-data')).data('id', 6));
        } else if (!$(this).hasClass('last-data') && $(this).val() == '') {
            $(this).closest('li').remove();
        }
    }).on('blur', '#mast-sub-table-Other-e li input', function () {
        if ($(this).hasClass('last-data') && $(this).val() != '') {
            $(this).removeClass('last-data');
            $('#mast-sub-table-Other-e').append($('<li>').addClass('list-group-item').append($('<input>').addClass('form-control last-data')).data('id', 6));
        } else if (!$(this).hasClass('last-data') && $(this).val() == '') {
            $(this).closest('li').remove();
        }
    }).on('blur', '#mast-sub-table2-OK-e li input', function () {
        if ($(this).hasClass('last-data') && $(this).val() != '') {
            $(this).removeClass('last-data');
            $('#mast-sub-table2-OK-e').append($('<li>').addClass('list-group-item').append($('<input>').addClass('form-control last-data')));
        } else if (!$(this).hasClass('last-data') && $(this).val() == '') {
            $(this).closest('li').remove();
        }
    }).on('blur', '#mast-sub-table2-NG-e li input', function () {
        if ($(this).hasClass('last-data') && $(this).val() != '') {
            $(this).removeClass('last-data');
            $('#mast-sub-table2-NG-e').append($('<li>').addClass('list-group-item').append($('<input>').addClass('form-control last-data')));
        } else if (!$(this).hasClass('last-data') && $(this).val() == '') {
            $(this).closest('li').remove();
        }
    }).on('blur', '#mast-sub-table2-Other-e li input', function () {
        if ($(this).hasClass('last-data') && $(this).val() != '') {
            $(this).removeClass('last-data');
            $('#mast-sub-table2-Other-e').append($('<li>').addClass('list-group-item').append($('<input>').addClass('form-control last-data')));
        } else if (!$(this).hasClass('last-data') && $(this).val() == '') {
            $(this).closest('li').remove();
        }
    });
    
    $('#date-table').on('change', ' tbody tr td select.weekday', function () {
        if ($(this).hasClass('last-data') && $(this).val() != '') {
            $(this).removeClass('last-data');
            addWeekDay_Last();
        }
    });

    $('#set_change_doc').on('click', function () {
        if (!confirm('医師情報を登録しますか？'))
            return;

        var dat = [];
        dat[dat.length] = editid;
        dat[dat.length] = $('#name-e input').val();
        dat[dat.length] = $('#name-r-e input').val();
        dat[dat.length] = $('#name-k-e input').val();
        dat[dat.length] = $('#comment-e input').val();
        dat[dat.length] = $('#count-e input').val();
        // dummy
        //dat[dat.length] = '';
        dat[dat.length] = $('#speed-e input').val();
        dat[dat.length] = $('#main-e input').val();
        dat[dat.length] = $('#other-e textarea').val();
        dat[dat.length] = $('#memo-e textarea').val();

        if ($("#color-pic").data('color') && $("#color-pic").data('color').tiny)
            dat[dat.length] = $("#color-pic").data('color').tiny.toRgbString();
        else if (!$("#color-pic").data('color'))
            dat[dat.length] = "rgb(255,255,255)";
        else
            dat[dat.length] = $("#color-pic").data('color');

        if ($("#color-pic").data('color2'))
            dat[dat.length] = $("#color-pic").data('color2');
        else
            dat[dat.length] = "black";


        var allData = '';

        var data = '';
        $('#mast-sub-table-OK-e').children('li').each(function () {
            if (!$(this).data('id') && $(this).data('id') != 0)
                return;
            if ($(this).data('id') == 6 && $($(this).children('input')[0]).val() == '')
                return;

            if (data != '')
                data += ':';
            data += $(this).data('id') + ',' + $($(this).children('input')[0]).val();
        });

        allData += data;

        data = '';
        $('#mast-sub-table-NG-e').children('li').each(function () {
            if (!$(this).data('id') && $(this).data('id') != 0)
                return;
            if ($(this).data('id') == 6 && $($(this).children('input')[0]).val() == '')
                return;

            if (data != '')
                data += ':';
            data += $(this).data('id') + ',' + $($(this).children('input')[0]).val();
        });

        allData += '@' + data;

        data = '';
        $('#mast-sub-table-Other-e').children('li').each(function () {
            if (!$(this).data('id') && $(this).data('id') != 0)
                return;
            if ($(this).data('id') == 6 && $($(this).children('input')[0]).val() == '')
                return;

            if (data != '')
                data += ':';
            data += $(this).data('id') + ',' + $($(this).children('input')[0]).val();
        });

        allData += '@' + data;

        dat[dat.length] = allData;


        allData = '';

        data = '';
        $('#mast-sub-table2-OK-e').find('input').each(function () {
            if ($(this).val() == '')
                return;

            if (data != '')
                data += ',';
            data += $(this).val();
        });
        allData += data;
        data = '';

        $('#mast-sub-table2-NG-e').find('input').each(function () {
            if ($(this).val() == '')
                return;
            if (data != '')
                data += ',';
            data += $(this).val();
        });
        allData += '@' + data;
        data = '';
        $('#mast-sub-table2-Other-e').find('input').each(function () {
            if ($(this).val() == '')
                return;
            if (data != '')
                data += ',';
            data += $(this).val();
        });
        allData += '@' + data;

        dat[dat.length] = allData;

//        dat[dat.length] = $('#s-date').val();
//        dat[dat.length] = $('#e-date').val();
        dat[dat.length] = '';
        dat[dat.length] = '';

        allData = '';
        data = '';

        for (var i = 0; i < addDay; i++)
        {
            var checkStatus = 0;
            data = '';
            if (!$('#' + i + '-s-day').val() || $('#' + i + '-s-day').val() == '')
                continue;

            data += $('#' + i + '-s-day').val();

            if ($('#' + i + '-in')[0].checked == true)
                data += ',' + 0;
            else if ($('#' + i + '-out')[0].checked == true)
                data += ',' + 1;
            else {
                alert('固定シフト：\n内、外のどちらかを選択してください。');
                return;
            }

            data += ',' + $('#' + i + '-s-time').val();
            data += ',' + $('#' + i + '-e-time').val();
            data += ',' + $('#' + i + '-count').val();

            if ($('#' + i + '-s-time').val() == "" && $('#' + i + '-e-time').val() == "")
                checkStatus++;

            if ($('#' + i + '-other')[0].checked == true)
                data += ',' + 0;
            else if ($('#' + i + '-help')[0].checked == true)
                data += ',' + 1;
            else {
                data += ',-1';
                if(checkStatus > 0)
                {
                    alert('固定シフト：\nﾍﾙﾌﾟ（県尼）でない場合、時間を指定してください。');
                    return;
                }
            }

            data += ',' + $('#' + i + '-s-date').val();
            data += ',' + $('#' + i + '-e-date').val();

            if (allData != '')
                allData += '@';

            allData += data;
        }

        dat[dat.length] = allData;

        C_SetDoctor(dat, endEdit);
        if ($("#color-pic").data('color') && $("#color-pic").data('color').tiny)
            C_SetColor($("#color-pic").data('color').tiny.toHexString(), setColor);

        var memo = $('#memo-e textarea').val();

        C_SetDocUserMemo(editid, memo);

        var date = new Date($('#date').val());

        var formattedDate =
            date.getFullYear() + '/' + ('0' + (date.getMonth() + 1)).slice(-2) + '/' + ('0' + date.getDate()).slice(-2);

        InitHTML(0, date);
        ShowLoading();
        GetScheduleList($('#date').val(), setDay);

        $('#modal-doc-edit').modal('hide');
        return false;
    });

    $('#del_holi').on('click', function () {
        if (!confirm('連続日程スケジュールを削除します。\nよろしいですか？'))
            return;

        var id = $(this).data('id');

        C_DelHoliday(id);
        var date = new Date($('#date').val());

        var formattedDate =
            date.getFullYear() + '/' + ('0' + (date.getMonth() + 1)).slice(-2) + '/' + ('0' + date.getDate()).slice(-2);

        InitHTML(0, date);
        ShowLoading();
        GetScheduleList($('#date').val(), setDay);

        $('#modal-date-holi').modal('hide');

    });

    $('#del_change').on('click', function () {
        if (!confirm('スケジュールを削除します。\nよろしいですか？\n\n※固定スケジュールは削除されません。'))
            return;

        var id = $(this).data('id');
        var subid = $(this).data('subid');
        var date = $(this).data('date');

        C_DelSchedule(id, subid, date);
        var date = new Date($('#date').val());

        var formattedDate =
            date.getFullYear() + '/' + ('0' + (date.getMonth() + 1)).slice(-2) + '/' + ('0' + date.getDate()).slice(-2);

        InitHTML(0, date);
        ShowLoading();
        GetScheduleList($('#date').val(), setDay);

        $('#modal-date-edit').modal('hide');
    });

    $('input.listblock').on('click focus', function (e) {
        var id = $(this).attr('id');

        event.stopPropagation();

        $('ul.dropdown-menu').hide();
        $('#' + id + '-list').toggle();

        $('#' + id + '-list').css('top', '100px');
        $('#' + id + '-list').css('left', '375px');

        $('#' + id + '-list li a').focus();

        return false;
    });

    $('#change-doc-list').on('change', function (e) {
        var id = $('#change-doc-list').val();

        if (id == '')
            return;

        var doc;
        for (var i = 0; i < Doctors.length; i++) {
            if (Doctors[i].DocName == id) {
                doc = Doctors[i];
                break;
            }
        }

        if (!doc)
            return;

        $('#modal-date-new div div div.modal-header').css('background-color', doc.Color);

    });
    $('#set_change').on('click', function () {
        var id = $(this).data('id');
        var subid = $(this).data('subid');
        var comment = $('#comment-one input').val();
        var sdate = $('#date-from-one input').val();
        var stime = $('#time-from-one select').val();
        var edate = $('#date-to-one input').val();
        var etime = $('#time-to-one select').val();
        var isChange = 0;
        var vals = [];

        if (subid && subid != '')
        {
            if(!$('#change-doc-one select').val() || $('#change-doc-one select').val() == '')
                $('#change-doc-one select').val($('#edit-title').data('docid'));
            setSubSchedule();
            $('#modal-date-edit').modal('hide');
            udtSchedule();
            return;
        }

        var cdoc = $('#change-doc-one select').val();
        if (cdoc && cdoc != '') {
            stime = '';
            etime = '';
            isChange = 1;
        }

        vals[vals.length] = id;
        vals[vals.length] = comment;
        vals[vals.length] = sdate;
        if (!stime)
            stime = '';
        vals[vals.length] = stime;
        vals[vals.length] = edate;
        if (!etime)
            etime = '';
        vals[vals.length] = etime;

        vals[vals.length] = isChange;

        if ($('#edit-all-one p input')[0].checked)
            vals[vals.length] = 1;
        else
            vals[vals.length] = 0;

        if ($('#edit-holi-one p input')[0].checked)
            vals[vals.length] = 1;
        else
            vals[vals.length] = 0;

        C_SetEditSchedule(vals, udtSchedule);

        if (cdoc && cdoc != '') {
            setSubSchedule();
        }

        $('#modal-date-edit').modal('hide');
    });


    $('#set_holi').on('click', function () {
        var doc = $('#doc-holi-list').val();
        var comment = $('#comment-holi input').val();
        var sdate = $('#date-from-holi input').val();
        var edate = $('#date-to-holi input').val();
        var type = -1;
        var id = "";

        if($('#set_holi').data('id'))
            id = $('#set_holi').data('id');

        if (sdate == '')
        {
            alert('開始日を指定してください。');
            return;
        }
        if (edate == '') {
            alert('終了日を指定してください。');
            return;
        }

        if ($('#edit-doc-holi p input')[0].checked)
            type = 0;
        else if ($('#edit-other-holi p input')[0].checked)
            type = 1;
        else if ($('#edit-all-holi p input')[0].checked)
            type = 2;
        else {
            alert('終日、休日、または、その他を選択してください。');
            return;
        }

        if (type != 1 && doc == '')
        {
            alert('終日、休日の場合、読影医を選択してください。');
            return;
        }

        var vals = [];
        vals[vals.length] = id;
        vals[vals.length] = sdate;
        vals[vals.length] = edate;
        vals[vals.length] = comment;
        vals[vals.length] = type;
        vals[vals.length] = doc;

        C_SetHoliday(vals, udtSchedule);
        $('#modal-date-holi').modal('hide');

    });

    $('#set_new').on('click', function () {
        var comment = $('#comment-new input').val();
        var sdate = $('#date-from-new input').val();
        var stime = $('#time-from-new select').val();
        var edate = $('#date-to-new input').val();
        var etime = $('#time-to-new select').val();
        var isChange = 0;
        var vals = [];
        var type = -1;
        var subtype = -1;

        var cdoc = $('#change-doc-list').val();
        if (cdoc == '') {
            alert('読影医を指定してください。');
            return;
        }

        for (var i = 0; i < Doctors.length; i++) {
            if (Doctors[i].DocName == cdoc) {
                cdoc = Doctors[i].DocID;
                break;
            }
        }

        if ($('#edit-in-new p input')[0].checked)
            type = 0;
        else if ($('#edit-out-new p input')[0].checked)
            type = 1;
        else if ($('#edit-other-new p input')[0].checked)
            type = 2;
        else {
            alert('読影場所（内、外、他）を指定してください。');
            return;
        }

        if (sdate == '')
        {
            alert('日付を指定してください。');
            return;
        }

        if (stime == '' && etime == '')
        {
            alert('開始時刻、終了時刻を指定してください。');
            return;
        }

        if (parseInt(stime) > parseInt(etime)) {
            alert('開始時刻、終了時刻の選択が間違っています。');
            return;
        }

        if ($('#edit-otherdat-new p input')[0].checked)
            subtype = 0;
        else if ($('#edit-help-new p input')[0].checked)
            subtype = 1;

        vals[vals.length] = '';
        vals[vals.length] = comment;
        vals[vals.length] = sdate;
        vals[vals.length] = stime;
        vals[vals.length] = edate;
        vals[vals.length] = etime;

        vals[vals.length] = isChange;

        if ($('#edit-all-new p input')[0].checked)
            vals[vals.length] = 1;
        else
            vals[vals.length] = 0;

        if ($('#edit-holi-new p input')[0].checked)
            vals[vals.length] = 1;
        else
            vals[vals.length] = 0;

        vals[vals.length] = type;
        vals[vals.length] = subtype;
        vals[vals.length] = cdoc

        C_SetEditSchedule(vals, udtSchedule);

        $('#modal-date-new').modal('hide');
    });

    $('#modal-date-edit').on('hidden.bs.modal', function () {
        //if (!$('#modal-date-edit').is(':visible'))
        //    return;

        var id = $('#set_change').data('id');
        var style = '';

        $.each(Schedule, function (index, sch) {
            $.each(sch.Data, function (index, dat) {
                if (dat.ID == id && editNode)
                {
                    $(editNode).attr('style', dat.Style).css('background-color', doc.Color);
                    return false;
                }
            });
        });
    });

    $('#modal-date-new').on('hidden.bs.modal', function () {
        $('#newDate').remove();
    });

    $('.datepicker').each(function () {
        $(this).datepicker({
            language: 'ja'
        });
    });

    var date = new Date();
    var formattedDate =
        date.getFullYear() + '/' + ('0' + (date.getMonth() + 1)).slice(-2) + '/' + ('0' + date.getDate()).slice(-2);

    $("#date").datepicker("setDate", formattedDate);

    resizeTable();

    //GetScheduleList($('#date').val(), setDay);
});

function endEdit(ret) {
    C_GetDoctorList(setDoctor);
}

function setDoctor(ret) {
    Doctors = ret.d;
    initDoctor();
}

function setColor(ret) {
    C_GetColorList(updateColor)
}
function updateColor(ret)
{
    ColorList = ret.d;
    initColor();
}

var $memo;

function setDocMemo(ret)
{
    $memo.val(ret.d);
    $memo.text(ret.d);
}
function initColor() {
    $("#color-pic").ColorPickerSliders({
        placement: 'bottom',
        swatches: ColorList,
        customswatches: false,
        hsvpanel: true,
        order: {
        },
        onchange: function (container, color) {
            var body = $('#modal-doc-edit div div.modal-header');

            body.css("background-color", color.tiny.toHexString());

            if (color.cielch.l < 60) {
                body.css("color", "white");
                $("#color-pic").data('color2', 'white');
            }
            else {
                body.css("color", "black");
                $("#color-pic").data('color2', 'black');
            }

            $("#color-pic").data('color', color.tiny.toHexString());

            var isCol = false;
            for (var i = 0; i < ColorList.length; i++)
                if (ColorList[i] == color.tiny.toHexString())
                {
                    isCol = true;
                    break;
                }
            if (!isCol) {
                var tmpCol = [];

                for (var i = 0; i < ColorList.length; i++)
                    tmpCol[tmpCol.length] = ColorList[i];

                tmpCol[tmpCol.length] = color.tiny.toHexString();
                this.swatches = tmpCol;
            }
        }
    });

}

function setMailList(ret){
    if (!ret || !ret.d)
        return;

    $('#mail-list').empty();
    $('#mail-list').append($('<option>').attr('value', '').append(''));

    $('#mail-title input').val('');
    $('#mail-cc input').val('');
    $('#mail-main textarea').text('');
    $.each(ret.d, function (index, list) {
        $('#mail-list').append(
            $('<option>').attr('value', list).append(list)
        );
    });

}

function getMailData() {
    var title = $('#mail-list').val();
    var cd = $('#mail-to input').data('cd');
    var id = $('#doc-mail-list').val();

    GetMailData(title, cd, id, setMailData);
}

function setMailData(ret) {
    if (!ret || !ret.d)
        return;

    var val = ret.d;

    if (!val.Title || val.Title == '')
        return;

    var id = $('#doc-mail-list').val();

    var name = "";

    $.each(Doctors, function (index, doc) {
        if(id == doc.DocID)
        {
            name = doc.DocName_R;
            return false;
        }
    });

    $('#mail-title input').val(val.Title);
    $('#mail-cc input').val(val.Bcc);
    //    $('#send-mail').data('bcc', val.Bcc);
    $('#mail-main textarea').text(val.Main.replace('@0', name).replace('@1', _util.user));
}

function endSend() {
    $('#modal-mail').modal('hide');
}

function setMasterMail(ret) {
    if (!ret || !ret.d)
        return;

    var val = ret.d;
    $('#mail-to input').val(val[0]);
    $('#mail-to input').data('cd', val[1]);

    if ($('#mail-list').val() != '') {
        getMailData();
    }

    var id = $('#doc-mail-list').val();

    var color = "";

    $.each(Doctors, function (index, doc) {
        if (id == doc.DocID) {
            color = doc.Color;
            return false;
        }
    });

    $('#modal-mail div div div.modal-header').css('background-color', color);

}

function C_GetDoctorList(func) {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./DocScheduleServ.asmx/GetDoctors",
        data: '',
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
function C_GetColorList(func) {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./DocScheduleServ.asmx/GetColorList",
        data: '',
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
function C_SetColor(color, func) {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./DocScheduleServ.asmx/SetColor",
        data: '{color:"' + color + '"}',
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

function C_DelHoliday(id) {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./DocScheduleServ.asmx/DeleteHoliday",
        data: '{sid:"' + id + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
            CloseLoading();
        }
    });
}

function C_DelSchedule(id, subid, date) {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./DocScheduleServ.asmx/DeleteSchedule",
        data: '{sid:"' + id + '", subid:"' + subid + '", date:"' + date + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
            CloseLoading();
        }
    });
}

function C_SetDocUserMemo(id, memo) {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./DocScheduleServ.asmx/SetDocUserMemo",
        data: '{id:"' + id + '", userid:"' + _util.user + '", memo:"' + memo + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
            CloseLoading();
        }
    });
}

function C_SetUserMemo(memo) {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./DocScheduleServ.asmx/SetUserMemo",
        data: '{userid:"' + _util.user + '", memo:"' + memo + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
            CloseLoading();
        }
    });
}

function C_SetDoctor(vals, func) {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./DocScheduleServ.asmx/SetDoctor",
        data: '{values:' + castJson(vals) + '}',
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

function C_SetHoliday(vals, func) {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./DocScheduleServ.asmx/SetHoliSchedule",
        data: '{vals:' + castJson(vals) + '}',
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

function C_GetSchedule(id, func) {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./DocScheduleServ.asmx/GetSchedule",
        data: '{id:' + id + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            func(result);
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
            //CloseLoading();
        }
    });
}

function GetScheduleList(date, func) {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./DocScheduleServ.asmx/GetScheduleList",
        data: '{day:"' + date + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            func(result);
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
            //CloseLoading();
        }
    });
}

function GetHoliDay(date, func) {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./DocScheduleServ.asmx/GetHoliDay",
        data: '{day:"' + date + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            func(result);
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
            //CloseLoading();
        }
    });
}

function GetDocUserMemo(id, func) {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./DocScheduleServ.asmx/GetDocUserMemo",
        data: '{id:"' + id + '", userid:"' + _util.user + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            func(result);
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
            //CloseLoading();
        }
    });
}

function GetUserMemo(func) {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./DocScheduleServ.asmx/GetUserMemo",
        data: '{userid:"' + _util.user + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            func(result);
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
            //CloseLoading();
        }
    });
}

function GetMailList(id, func) {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./DocScheduleServ.asmx/GetMailList",
        data: '{docid:"' + id + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            func(result);
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
            //CloseLoading();
        }
    });
}

function GetMailMaster(id, func) {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./DocScheduleServ.asmx/GetMailMaster",
        data: '{docid:"' + id + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            func(result);
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
            //CloseLoading();
        }
    });
}

function GetMailData(title, cd, id, func) {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./DocScheduleServ.asmx/GetMailData",
        data: '{title:"' + title + '", cd:"' + cd + '", docid:"' + id + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            func(result);
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
            //CloseLoading();
        }
    });
}

function C_SendMail(vals, func) {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./DocScheduleServ.asmx/SendMail",
        data: '{values:' + castJson(vals) + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            func(result);
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
            //CloseLoading();
        }
    });
}

function C_SetEditSchedule(vals, func) {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./DocScheduleServ.asmx/SetEditSchedule",
        data: '{userid:"' + _util.user + '", vals:' + castJson(vals) + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            func(result);
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
            //CloseLoading();
        }
    });
}

function C_GetSPDay() {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./DocScheduleServ.asmx/GetSPDays",
        data: '',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
            SPDays = result.d;
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
            //CloseLoading();
        }
    });
}

function C_SetSubSchedule(vals, func) {
    $.ajax({
        async: true,
        cache: false,
        type: "POST",
        url: "./DocScheduleServ.asmx/SetSubSchedule",
        data: '{userid:"' + _util.user + '", vals:' + castJson(vals) + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (result) {
        },
        error: function (result) {
            // エラー
            alert("HTTP通信でエラーが発生しました。");
            //CloseLoading();
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


function C_GetDocSchdule(id, func) {
    var ret = { d: {} };

    ret.d = _docSch;

    func(id, ret);
}

function udtSchedule(){
    var date = new Date($('#date').val());

    var formattedDate =
        date.getFullYear() + '/' + ('0' + (date.getMonth() + 1)).slice(-2) + '/' + ('0' + date.getDate()).slice(-2);

    InitHTML(0, date);
    ShowLoading();
    GetScheduleList($('#date').val(), setDay);
}


var editid = '';
var addDay = 0;


function setDocEditData(id, ret) {
    var sch = ret.d;

    var doc = {};
    for(var i = 0 ;i < Doctors.length;i++)
        if(id == Doctors[i].DocID)
        {
            doc = Doctors[i];
            break;
        }

    $('#mast-title-e span').empty();
    $('#name-r-e input').val('');
    $('#name-e input').val('');
    $('#name-k-e input').val('');
    $('#comment-e input').val('');
    $('#count-e input').val('');
    $('#speed-e input').val('');
    $('#main-e input').val('');
    $('#other-e textarea').val('');
    $('#memo-e textarea').val('');

    $('#body-mast p').each(function () {
        $(this).removeClass('OK').removeClass('NG').removeClass('Other');
    });
    $('#body-mast p input').each(function () {
        $(this)[0].checked = false;
    });

    $('#mast-sub-table-OK-e').empty();
    $('#mast-sub-table-NG-e').empty();
    $('#mast-sub-table-Other-e').empty();
    $('#mast-sub-table2-OK-e').empty();
    $('#mast-sub-table2-NG-e').empty();
    $('#mast-sub-table2-Other-e').empty();

    $('#s-date').val('');
    $('#e-date').val('');

    //for (var j = 0; j < 7; j++) {
    //    $('#' + j + '-in')[0].checked = false;
    //    $('#' + j + '-out')[0].checked = false;
    //    $('#' + j + '-help')[0].checked = false;

    //    $('#' + j + '-s-time').val('');
    //    $('#' + j + '-e-time').val('');
    //    $('#' + j + '-count').val('');
    //}
    $('#date-table tbody').empty();
    $('#date-table-del tbody').empty();
    addDay = 0;

    editid = '';

    if (id == -1) {
        $('#mast-sub-table-OK-e').append($('<li>').addClass('list-group-item').append($('<input>').addClass('form-control last-data')));
        $('#mast-sub-table-NG-e').append($('<li>').addClass('list-group-item').append($('<input>').addClass('form-control last-data')));
        $('#mast-sub-table-Other-e').append($('<li>').addClass('list-group-item').append($('<input>').addClass('form-control last-data')));
        $('#mast-sub-table2-OK-e').append($('<li>').addClass('list-group-item').append($('<input>').addClass('form-control last-data')));
        $('#mast-sub-table2-NG-e').append($('<li>').addClass('list-group-item').append($('<input>').addClass('form-control last-data')));
        $('#mast-sub-table2-Other-e').append($('<li>').addClass('list-group-item').append($('<input>').addClass('form-control last-data')));
        $('#modal-doc-edit').modal();
        addWeekDay_Last();
        return;
    }

    editid = id;

    $('#modal-doc-edit div div div.modal-header').css('background-color', doc.Color);
    $('#color-pic').data('color', doc.Color);
    $('#modal-doc-edit div div div.modal-header').css('color', doc.Color2);
    $('#color-pic').data('color2', doc.Color2);

    $('#mast-title-e span').append(doc.DocName);
    $('#name-r-e input').val(doc.DocName_R);
    $('#name-e input').val(doc.DocName);
    $('#name-k-e input').val(doc.DocName_H);
    $('#comment-e input').val(doc.Comment);
    $('#count-e input').val(doc.Count);
    $('#speed-e input').val(doc.Speed);
    $('#main-e input').val(doc.Main);
    $('#other-e textarea').val(doc.Element);
    //$('#memo-e textarea').val(doc.Memo);


    $('#mast-sub-table-OK-e').empty();
    for (var i = 0; i < doc.Body.OK.length; i++)
    {
        $('#mast-sub-table-OK-e').append($('<li>').addClass('list-group-item').append($('<input>').val(doc.Body.OK[i].Text).addClass('form-control')).data('id', doc.Body.OK[i].Id));
        $('#body-mast p input').each(function () {
            if ($(this).val() == doc.Body.OK[i].Id) {
                $(this).closest('p').addClass('OK');
                return;
            }
        });
    }
    $('#mast-sub-table-OK-e').append($('<li>').addClass('list-group-item').append($('<input>').addClass('form-control last-data')).data('id', 6));

    $('#mast-sub-table-NG-e').empty();
    for (var i = 0; i < doc.Body.NG.length; i++) {
        $('#mast-sub-table-NG-e').append($('<li>').addClass('list-group-item').append($('<input>').val(doc.Body.NG[i].Text).addClass('form-control')).data('id', doc.Body.NG[i].Id));
        $('#body-mast p input').each(function () {
            if ($(this).val() == doc.Body.NG[i].Id) {
                $(this).closest('p').addClass('NG');
                return;
            }
        });
    }
    $('#mast-sub-table-NG-e').append($('<li>').addClass('list-group-item').append($('<input>').addClass('form-control last-data')).data('id', 6));

    $('#mast-sub-table-Other-e').empty();
    for (var i = 0; i < doc.Body.Other.length; i++) {
        $('#mast-sub-table-Other-e').append($('<li>').addClass('list-group-item').append($('<input>').val(doc.Body.Other[i].Text).addClass('form-control')).data('id', doc.Body.Other[i].Id));
        $('#body-mast p input').each(function () {
            if ($(this).val() == doc.Body.Other[i].Id) {
                $(this).closest('p').addClass('Other');
                return;
            }
        });
    }
    $('#mast-sub-table-Other-e').append($('<li>').addClass('list-group-item').append($('<input>').addClass('form-control last-data')).data('id', 6));

    $('#mast-sub-table2-OK-e').empty();
    for (var i = 0; i < doc.Hosp.OK.length; i++)
        $('#mast-sub-table2-OK-e').append($('<li>').addClass('list-group-item').append($('<input>').val(doc.Hosp.OK[i]).addClass('form-control')));
    $('#mast-sub-table2-OK-e').append($('<li>').addClass('list-group-item').append($('<input>').addClass('form-control last-data')));

    $('#mast-sub-table2-NG-e').empty();
    for (var i = 0; i < doc.Hosp.NG.length; i++)
        $('#mast-sub-table2-NG-e').append($('<li>').addClass('list-group-item').append($('<input>').val(doc.Hosp.NG[i]).addClass('form-control')));
    $('#mast-sub-table2-NG-e').append($('<li>').addClass('list-group-item').append($('<input>').addClass('form-control last-data')));

    $('#mast-sub-table2-Other-e').empty();
    for (var i = 0; i < doc.Hosp.Other.length; i++)
        $('#mast-sub-table2-Other-e').append($('<li>').addClass('list-group-item').append($('<input>').val(doc.Hosp.Other[i]).addClass('form-control')));
    $('#mast-sub-table2-Other-e').append($('<li>').addClass('list-group-item').append($('<input>').addClass('form-control last-data')));

    C_GetSchedule(id, setSchedule)
    $memo = $('#memo-e textarea');

    GetDocUserMemo(editid, setDocMemo);

    $('#modal-doc-edit').modal();

}

function setSchedule(ret){
    var sch = ret.d;

    var today = formatDate(new Date(), 'YYYY/MM/DD');

    //$('#s-date').val(sch.Sdate);
    //$('#e-date').val(sch.Edate);

    for (var j = 0; j < sch.Schedule.length; j++) {
        if (sch.Schedule[j].Edate != '' && today >= sch.Schedule[j].Edate)
            addDelWeekDay();
        else
            addWeekDay();

        $('#' + j + '-s-day').val(sch.Schedule[j].Day);
        if (sch.Schedule[j].Type == 0)
            $('#' + j + '-in')[0].checked = true;
        else if (sch.Schedule[j].Type == 1)
            $('#' + j + '-out')[0].checked = true;


        if (sch.Schedule[j].SubType == 0)
            $('#' + j + '-other')[0].checked = true;
        else if (sch.Schedule[j].SubType == 1)
            $('#' + j + '-help')[0].checked = true;

        $('#' + j + '-s-time').val(sch.Schedule[j].Stime);
        $('#' + j + '-e-time').val(sch.Schedule[j].Etime);
        $('#' + j + '-count').val(sch.Schedule[j].Count);

        if (sch.Schedule[j].Edate != '' && today >= sch.Schedule[j].Edate)
        {
            $('#' + j + '-s-date').val(sch.Schedule[j].Sdate);
            $('#' + j + '-e-date').val(sch.Schedule[j].Edate);
        } else {
            $('#' + j + '-s-date').datepicker('setDate', sch.Schedule[j].Sdate);
            $('#' + j + '-e-date').datepicker('setDate', sch.Schedule[j].Edate);
        }
    }
    addWeekDay_Last();
}

function setDocData(id, date, update, sid, subid) {
    var sch = {};
    var doc = {};

    for(var i = 0; i < Doctors.length; i++)
    {
        if(Doctors[i].DocID == id)
        {
            doc = Doctors[i];
            break;
        }
    }

    if (!doc.DocID)
        return -1;

    $('#mast-title').empty();
    $('#today-data span').empty();
    $('#name-r span').empty();
    $('#name span').empty();
    $('#name-k span').empty();
    $('#comment span').empty();
    $('#count span').empty();
    $('#speed span').empty();
    $('#main span').empty();
    $('#other span pre').empty();
    $('#memo span pre').empty();
    $('#update span').empty();

    $('#modal-doc-mast div div div.modal-header').css('background-color', doc.Color);

    $('#mast-title').append(doc.DocName + ' (' + doc.DocName_H + ') 先生' );
    $('#today-data span').append(date);
    $('#name-r span').append(doc.DocName_R);
    $('#name span').append(doc.DocName);
    $('#name-k span').append(doc.DocName_H);
    $('#comment span').append(doc.Comment);
    $('#count span').append(doc.Count);
    $('#speed span').append(doc.Speed);
    $('#main span').append(doc.Main);
    $('#other span pre').append(doc.Element);
    //$('#memo span').append(doc.Memo);
    $('#update span').append(update);

    $('#mast-sub-table-OK').empty();
    for(var i = 0; i < doc.Body.OK.length;i++)
        $('#mast-sub-table-OK').append($('<li>').addClass('list-group-item').append(doc.Body.OK[i].Text));
    $('#mast-sub-table-NG').empty();
    for (var i = 0; i < doc.Body.NG.length; i++)
        $('#mast-sub-table-NG').append($('<li>').addClass('list-group-item').append(doc.Body.NG[i].Text));
    $('#mast-sub-table-Other').empty();
    for (var i = 0; i < doc.Body.Other.length; i++)
        $('#mast-sub-table-Other').append($('<li>').addClass('list-group-item').append(doc.Body.Other[i].Text));

    $('#mast-sub-table2-OK').empty();
    for (var i = 0; i < doc.Hosp.OK.length; i++)
        $('#mast-sub-table2-OK').append($('<li>').addClass('list-group-item').append(doc.Hosp.OK[i]));
    $('#mast-sub-table2-NG').empty();
    for (var i = 0; i < doc.Hosp.NG.length; i++)
        $('#mast-sub-table2-NG').append($('<li>').addClass('list-group-item').append(doc.Hosp.NG[i]));
    $('#mast-sub-table2-Other').empty();
    for (var i = 0; i < doc.Hosp.Other.length; i++)
        $('#mast-sub-table2-Other').append($('<li>').addClass('list-group-item').append(doc.Hosp.Other[i]));

    $memo = $('#memo span pre');


    $('#edit-today').data('sid', '').data('subid', '');

    $('#edit-today').data('sid', sid).data('subid', subid);
    $('#edit-doc').data('id', id);

    $('#mail-doc').data('id', id);

    GetDocUserMemo(id, setDocMemo);

    return 0;
}

var editNode;

function setDay(ret) {
    Schedule = ret.d;

    var preData = [];

    var helpData = [];
    var otherData = [];
    var allData = [];

    $.each(Schedule, function(index, sch){
        var $parent = null;
        var $rows = null;
        var key = null;
        var sCnt = 0;

        var height = sch.Data.length + 20;

        var isRow = false;

        $.each(sch.Data, function (index, dat) {
            var $date = null;
            var subList = [];

            $('td.title-date').each(function () {
                if ($(this).data('date') == dat.Date) {
                    $date = $(this);
                    return false;
                }
            });

            key = $date.attr('id');

            switch (dat.Type) {
                case 0:
                    $rows = $("#" + key + "-in-time tbody");
                    if (dat.SubType == 0)
                        $parent = $("#" + key + "-in-time tbody tr td:last");
                    else
                        $parent = $("#" + key + "-in-time tbody tr td:last");

                    if (dat.IsAll == 1)
                        $parent = $("#" + key + "-in-all");

                    break;
                case 1:
                    $rows = $("#" + key + "-out-time tbody");
                    if (dat.SubType == 0)
                        $parent = $("#" + key + "-out-time tbody tr td:last");
                    else
                        $parent = $("#" + key + "-out-time tbody tr td:last");

                    if (dat.IsAll == 1)
                        $parent = $("#" + key + "-out-all");

                    break;
                case 2:
                    $rows = $("#" + key + "-other-time tbody");
                    if (dat.SubType == 0)
                        $parent = $("#" + key + "-other-time tbody tr td:last");
                    else
                        $parent = $("#" + key + "-other-time tbody tr td:last");

                    if (dat.IsAll == 1)
                        $parent = $("#" + key + "-other-all");

                    break;
            }

            var doc = {};

            for (var j = 0; j < Doctors.length; j++) {
                if (Doctors[j].DocID == dat.DocID) {
                    doc = Doctors[j];
                    break;
                }
            }

            if (!doc.DocID)
            {
                doc.DocID = dat.DocID;
                doc.DocName_R = dat.DocID;
                doc.Comment = "";
                doc.Color = "#FBCDAE";
            }
            var txt = dat.Count;
            if (txt == '')
                txt = doc.Comment;

            if (txt != '')
                txt = '(' + txt + ')';

            var $val = $('<div>');
            switch(dat.SubType)
            {
                case 1:
                    if(dat.Type == 0)
                        txt = '【内】' + txt;
                    else if(dat.Type == 1)
                        txt = '【外】' + txt;

                    $val.addClass('mnc1').attr('style', 'left:861px;top:0px;width:199px').css('background-color', doc.Color);
                break;
                case 0:
                    if (dat.Type == 0)
                        txt = '【内】' + txt;
                    else if (dat.Type == 1)
                        txt = '【外】' + txt;

                    $val.addClass('mnc2').attr('style', 'left:701px;top:0px;width:79px;height:' + height + 'px').css('background-color', doc.Color);
                    break;
                default:
                    if (dat.IsAll == 1)
                        $val.addClass('mnc3').attr('style', 'left:0px;top:0px;width:79px;height:10px').css('background-color', doc.Color);
                    else {
                        $val.addClass('mn').addClass('mnc1').attr('style', dat.Style).css('background-color', doc.Color);

                        if (dat.SubDays)
                        {
                            $.each(dat.SubDays, function (idx, sub) {
                                var subdoc = {};

                                for (var j = 0; j < Doctors.length; j++) {
                                    if (Doctors[j].DocID == sub.DocID) {
                                        subdoc = Doctors[j];
                                        break;
                                    }
                                }

                                var $subval = $('<div>');
                                $subval.addClass('mn').addClass('mnc1').attr('style', sub.Style).css('background-color', subdoc.Color);

                                var txt = sub.Count;

                                if (txt != '')
                                    txt = '(' + txt + ')';

                                $subval.append(
                                    $('<div>').addClass('egrid').append(
                                        $('<span>').data('id', sub.DocID).data('subid', sub.ID).data('sid', sub.SubDocID).data('date', getDateFormat(sub)).data('update', sub.Update).append('[代]' + subdoc.DocName_R + txt + '←' + doc.DocName_R).attr('title', '[代]' + subdoc.DocName_R + txt + '←' + doc.DocName_R).data('txt', sub.Count)
                                    )
                                );

                                subList[subList.length] = $subval;
                            });
                        }
                    }
                    break;
            }

            if (dat.IsDelete == 1) {
                $val.append(
                    $('<div>').addClass('egrid del-data').append(
                        $('<span>').data('id', doc.DocID).data('sid', dat.ID).data('date', getDateFormat(dat)).data('update', dat.Update).append('×' + doc.DocName_R + txt).attr('title', doc.DocName_R + txt).data('txt', dat.Count)
                    )
                );
            }else if (dat.IsAll == 1) {
                $val.append(
                    $('<div>').addClass('egrid all-data').append(
                        $('<span>').data('id', doc.DocID).data('sid', dat.ID).data('date', getDateFormat(dat)).data('update', dat.Update).append(doc.DocName_R + txt).attr('title', doc.DocName_R + txt).data('txt', dat.Count).css('writing-mode', 'tb-rl').css('text-align', 'left').css('padding-top', '2px').css('overflow', 'hidden')
                    )
                );
            } else {
                $val.append(
                    $('<div>').addClass('egrid').append(
                        $('<span>').data('id', doc.DocID).data('sid', dat.ID).data('date', getDateFormat(dat)).data('update', dat.Update).append(doc.DocName_R + txt).attr('title', doc.DocName_R + txt).data('txt', dat.Count)
                    )
                );
            }

            switch (dat.SubType) {
                case 1:
                    var isDat = false;
                    for (var i = 0; i < helpData.length; i++)
                    {
                        if (helpData[i].Parent == key) {
                            helpData[i].Vals[helpData[i].Vals.length] = $val;
                            isDat = true;
                        }
                    }

                    if (!isDat)
                    {
                        var index = helpData.length;

                        helpData[index] = {};
                        helpData[index].Parent = key;
                        helpData[index].Vals = [];
                        helpData[index].Vals[helpData[index].Vals.length] = $val;
                    }
                    break;
                case 0:
                    var isDat = false;
                    for (var i = 0; i < otherData.length; i++) {
                        if (otherData[i].Parent == key) {
                            otherData[i].Vals[otherData[i].Vals.length] = $val;
                            isDat = true;
                        }
                    }

                    if (!isDat) {
                        var index = otherData.length;

                        otherData[index] = {};
                        otherData[index].Parent = key;
                        otherData[index].Vals = [];
                        otherData[index].Vals[otherData[index].Vals.length] = $val;
                    }
                    break;
                default:
                    if (dat.IsAll == 1)
                    {
                        var isDat = false;
                        for (var i = 0; i < allData.length; i++) {
                            if (allData[i].Parent.selector == $parent.selector) {
                                allData[i].Vals[allData[i].Vals.length] = $val;
                                isDat = true;
                            }
                        }

                        if (!isDat) {
                            var index = allData.length;

                            allData[index] = {};
                            allData[index].Parent = $parent;
                            allData[index].Vals = [];
                            allData[index].Vals[allData[index].Vals.length] = $val;
                        }
                    } else {
                        $parent.append($val);

                        $.each(subList, function (idx, sub) {
                            $parent.append(sub);
                        });

                        isRow = true;
                    }
                    break;
            }
        });

        if (isRow)
        {
            if ($rows)
                $rows.append(setDataRow());
            if ($parent)
                $parent.addClass('data-td-dot');

            var height = $('#t-holi' + key).height();
            if (!height || height == 0)
                height = 64;
            height = height + 20;

            $('#t-holi' + key).height(height);
            $('#t-' + key).height(height);
        }
    });

    $.each(helpData, function (idx, help) {
        var $row = $("#" + help.Parent + "-in-time tbody");
        var $rows = $row.find('tr');
        var inRowCnt = $row.find('tr').length;
        var $row2 = $("#" + help.Parent + "-out-time tbody");
        var $rows2 = $row2.find('tr');
        var outRowCnt = $row2.find('tr').length;

        $.each(help.Vals, function (idx2, val) {
            if(idx2 < inRowCnt)
            {
                var $dat = $($rows[idx2]).find('td');

                $dat.append(val);
            } else {
                var $dat = $($rows2[idx2 - inRowCnt]).find('td');

                if((idx2 + 1) >= inRowCnt + outRowCnt)
                {
                    $dat = $("#" + help.Parent + "-out-time tbody tr td:last");
                    $dat.addClass('data-td-dot');
                    $row2.append(setDataRow());

                    var height = $('#t-holi' + help.Parent).height();
                    if (!height || height == 0)
                        height = 64;
                    height = height + 20;

                    $('#t-holi' + help.Parent).height(height);
                    $('#t-' + help.Parent).height(height);
                }

                $dat.append(val);
            }
        });
    });

    $.each(otherData, function (idx, other) {
        var $parent = $("#" + other.Parent + "-body");
        var $row = $parent.closest('td');

        var height = $row.height() / other.Vals.length;

        $.each(other.Vals, function (idx2, val) {
            var color = val.css('background-color');

            val.attr('style', 'left:812px;top:' + (height * idx2) + 'px;width:79px;height:' + height + 'px').css('background-color', color);
            $row.css('position', 'relative').append(val);
        });
    });

    $.each(allData, function (idx, all) {
        $.each(all.Vals, function (idx2, val) {
            var $dat = all.Parent;
            var h = $dat.height();

            var color = val.css('background-color');

            val.attr('style', 'left:' + (idx2 * 30) + 'px;top:0px;width:25px;height:' + h + 'px').css('background-color', color);
            $dat.css('position', 'relative').append(val);
        });
    });

////    $('#body-table').find('table.day-body tbody tr td table tbody tr').each(function () {
////        var height = $(this).closest('table').height();

////        var dats = $(this).find('div.mnc2');
////        var length = dats.length;

////        $.each(dats, function (idx, dat) {
////            var top = idx * (height / length);
////            var h = (height / length);

////            //var top = 0;
////            //var h = height - 6;
////            var color = $(this).css('background-color');

////            $(this).attr('style', 'left:781px;top:' + top + 'px;width:79px;height:' + h + 'px').css('background-color', color);

////        });


//////        $(this).height(height- 4);
////    });


    /**
 * 指定の要素初期位置に戻せるようにオフセットを記憶する
 */
    $('.mn').mousedown(function () {
        gNodeTop = Number($(this).css('top').replace("px", ""));
        gNodeLeft = Number($(this).css('left').replace("px", ""));
        gNodeWidth = Number($(this).css('width').replace("px", ""));
        $(this).css("z-index", gZindex);
        gZindex = gZindex + 1;
    });
    /**
	 * 指定の要素を移動できるようにする
	 */
    $('.mn').rcSchedule({
        draggableGridX: 15, 	// ノードの一度に移動できる横幅
        draggableGridY: 0, // ノードの一度に移動できる縦幅
        resizableGridX: 15, 	// ノードのサイズ変更できる横幅
        resizableGridY: 0, 	// ノードのサイズ変更できる縦幅
        resizableMaxH: 20, 	// ノードの高さを維持させる　高さの最大値
        resizableMinH: 20,	//　ノードの高さを維持させる 高さの最小値
        startOffsetX: 0, 	// 要素の初期位置　左から何pxの位置がノードのスタート位置になるか
        startOffsetY: 0, 	// 要素の初期位置　上から何pxの位置がノードのスタート位置になるか
        startTime: "8:30",	// スケジュール表の開始時間
        endTime: "21:30",	// スケジュール表の終了時間
        widthTime: 900, 		// 1移動幅辺りの時間(秒)　15分:900秒
        callback:
		function (node, params) {
		    if (params['outside'] == true) InitializationNode(node, params);
		    rewrite(node, params);

		    var $span = $(node).find('span');
		    var id = $span.data('id');
		    var sid = $span.data('sid');
		    var txt = $span.data('txt');

		    var left = parseInt($(node).css('left').replace('px', ''));
		    var width = $(node).width();

		    var start = getStartTimeID(left);
		    var end = getEndTimeID(left, width);

		    var date = $(node).closest('table.day-body').data('date');
		    var dt = new Date(date.substring(0, 4), parseInt(date.substring(4, 6)) - 1, date.substring(6, 8));

		    editNode = node;

		    setEditData(id, sid, dt, start, end, txt, null, date);

		    var doc = {};

		    for (var j = 0; j < Doctors.length; j++) {
		        if (Doctors[j].DocID == dat.DocID) {
		            doc = Doctors[j];
		            break;
		        }
		    }

		    if (!doc.DocID) 
		        $('#change-doc-one').attr('disabled', 'disabled').attr('readonly', 'readonly');
		    else
		        $('#change-doc-one').removeAttr('disabled').removeAttr('readonly');

		    $('#modal-date-edit').modal();
		}
    });

    GetHoliDay($('#date').val(), setHoliDay);

    resizeWindow();
    resizeTable();
    CloseLoading();
}

function setSubSchedule() {
    var id = $('#set_change').data('id');
    var subid = $('#set_change').data('subid');
    var cdoc = $('#change-doc-one select').val();
    var comment = $('#comment-one input').val();
    var sdate = $('#date-from-one input').val();
    var stime = $('#time-from-one select').val();
    var edate = $('#date-to-one input').val();
    var etime = $('#time-to-one select').val();
    var vals = [];

    vals[vals.length] = id;
    vals[vals.length] = comment;
    vals[vals.length] = sdate;
    vals[vals.length] = stime;
    vals[vals.length] = edate;
    vals[vals.length] = etime;

    //if ($('#edit-all-one p input')[0].checked)
    //    vals[vals.length] = 1;
    //else
    //    vals[vals.length] = 0;

    //if ($('#edit-holi-one p input')[0].checked)
    //    vals[vals.length] = 1;
    //else
    //    vals[vals.length] = 0;

    vals[vals.length] = cdoc;
    if (subid)
        vals[vals.length] = subid;
    else
        vals[vals.length] = '';

    C_SetSubSchedule(vals);
}

var holiW = 0;

function setHoliDay(ret) {
    holiW = 30;

    if (!ret || !ret.d || ret.d.length == 0)
        return;

    HoliDays = ret.d;

    var DateLeft = [];

    $('td.title-date').each(function () {
        var val = {}
        val.Date = $(this).data('date');
        val.Left = '';
        DateLeft[DateLeft.length] = val;
    });

    $.each(HoliDays, function (idx, holi) {
        var totalH = 0;
        var top = -1;
        var holiH = 0;
        var $start;
        var left = 0;

        var $longDays = [];

        if (holi.DocID != '')
            return;

        var idxL = 0;
        var isDat = false;
        $('td.title-date').each(function () {
            if ($(this).data('date') >= holi.SDay && $(this).data('date') <= holi.EDay) {
                if (top == -1) {
                    top = 0;
                    $start = $(this);
                    left = DateLeft[idxL].Left.indexOf('@');
                    if (left >= 0) {
                        DateLeft[idxL].Left = DateLeft[idxL].Left.replace('@', '0');
                        isDat = true;
                    } else {
                        left = DateLeft[idxL].Left.length;
                        DateLeft[idxL].Left = DateLeft[idxL].Left + '0';
                    }
                } else {
                    $longDays[$longDays.length] = $(this);

                    if (DateLeft[idxL].Left.length > left) {
                        var tmpLeft = DateLeft[idxL].Left;

                        DateLeft[idxL].Left = "";

                        for (var li = 0; li < tmpLeft.length; li++) {
                            if (li == left)
                                DateLeft[idxL].Left += "0";
                            else
                                DateLeft[idxL].Left += tmpLeft[li];
                        }


                    }
                    else
                        DateLeft[idxL].Left = DateLeft[idxL].Left + '0';
                }
                holiH += $(this).height() + 1;
            }
            else if (!isDat) {
                DateLeft[idxL].Left = DateLeft[idxL].Left + "@";
            }
            idxL = idxL + 1;
        });

        if (!$start)
            return;

        var doc;
        var color = "rgba(248, 243, 186, 1)";
        for (var j = 0; j < Doctors.length; j++) {
            if (Doctors[j].DocID == holi.DocID) {
                color = Doctors[j].Color;
                doc = Doctors[j];
                break;
            }
        }
        var dateTxt = '';
        if (holi.SDay != holi.EDay) {
            dateTxt = holi.SDay.substring(4, 6) + "/" + holi.SDay.substring(6, 8) + "～" + holi.EDay.substring(4, 6) + "/" + holi.EDay.substring(6, 8)
        }

        var com = "";
        if (holi.Type == 0) {
            com = '×' + doc.DocName_R + " " + holi.Comment + " " + dateTxt;
        } else {
            if (doc)
                com = doc.DocName_R + " " + holi.Comment + " " + dateTxt;
            else
                com = holi.Comment + " " + dateTxt;
        }

        var $val = $('<div>');
        var valLeft = ((left * 33) + 65);
        $val.addClass('mnc3').attr('style', 'left:' + valLeft + 'px;top:' + top + 'px;width:30px;height:' + holiH + 'px').css('background-color', color);
        $val.append(
            $('<div>').addClass('egrid holi-data').append(
                $('<span>').data('id', holi.ID).data('docid', holi.DocID).data('vals', holi).attr('title', com).append(com).css('writing-mode', 'tb-rl').css('text-align', 'left').css('padding-top', '2px').css('white-space', 'nowrap')
            )
        );

        if (holiW < valLeft - 15)
            holiW = valLeft - 15;

        $start.css('position', 'relative').append($val);

        $.each($longDays, function (idx, $long) {
        });

    });

    DateLeft = [];

    $('td.title-date').each(function () {
        var val = {}
        val.Date = $(this).data('date');
        val.Left = '';
        DateLeft[DateLeft.length] = val;
    });

    var otherW = holiW;

    $.each(HoliDays, function (idx, holi) {
        var totalH = 0;
        var top = -1;
        var holiH = 0;
        var $start;
        var left = 0;

        var $longDays = [];

        if (holi.DocID == '')
            return;


        var idxL = 0;
        var isDat = false;
        $('td.title-date').each(function () {
            if ($(this).data('date') >= holi.SDay && $(this).data('date') <= holi.EDay) {
                if (top == -1) {
                    top = 0;
                    $start = $(this);
                    left = DateLeft[idxL].Left.indexOf('@');
                    if (left >= 0) {
                        DateLeft[idxL].Left = DateLeft[idxL].Left.replace('@', '0');
                        isDat = true;
                    } else {
                        left = DateLeft[idxL].Left.length;
                        DateLeft[idxL].Left = DateLeft[idxL].Left + '0';
                    }
                } else {
                    $longDays[$longDays.length] = $(this);

                    if (DateLeft[idxL].Left.length > left){
                        var tmpLeft = DateLeft[idxL].Left;

                        DateLeft[idxL].Left = "";

                        for (var li = 0; li < tmpLeft.length; li++)
                        {
                            if(li == left)
                                DateLeft[idxL].Left += "0";
                            else
                                DateLeft[idxL].Left += tmpLeft[li];
                        }


                    }
                    else
                        DateLeft[idxL].Left = DateLeft[idxL].Left + '0';
                }
                holiH += $(this).height() + 1;
            }
            else if(!isDat){
                DateLeft[idxL].Left = DateLeft[idxL].Left + "@";
            }
            idxL = idxL + 1;
        });

        if (!$start)
            return;

        var doc;
        var color = "rgba(248, 243, 186, 1)";
        for (var j = 0; j < Doctors.length; j++) {
            if (Doctors[j].DocID == holi.DocID) {
                color = Doctors[j].Color;
                doc = Doctors[j];
                break;
            }
        }
        var dateTxt = '';
        if (holi.SDay != holi.EDay)
        {
            dateTxt = holi.SDay.substring(4, 6) + "/" + holi.SDay.substring(6, 8) + "～" + holi.EDay.substring(4, 6) + "/" + holi.EDay.substring(6, 8)
        }

        var com ="" ;
        if (holi.Type == 0)
        {
            com = '×' + doc.DocName_R + " " + holi.Comment + " " + dateTxt;
        } else {
            if(doc)
                com = doc.DocName_R + " " + holi.Comment + " " + dateTxt;
            else
                com = holi.Comment + " " + dateTxt;
        }

        var $val = $('<div>');
        var valLeft = ((left * 33) + 65 + otherW);
        $val.addClass('mnc3').attr('style', 'left:' + valLeft + 'px;top:' + top + 'px;width:30px;height:' + holiH + 'px').css('background-color', color);
        $val.append(
            $('<div>').addClass('egrid holi-data').append(
                $('<span>').data('id', holi.ID).data('docid', holi.DocID).data('vals', holi).attr('title', com).append(com).css('writing-mode', 'tb-rl').css('text-align', 'left').css('padding-top', '2px').css('white-space', 'nowrap')
            )
        );

        if (holiW < valLeft - 15)
            holiW = valLeft - 15;

        $start.css('position', 'relative').append($val);

        $.each($longDays, function (idx, $long) {
        });

    });

    $('#body-table').on('click', 'div.egrid span', function (e) {
        var id = $(this).data('id');
        var docid = $(this).data('docid');
        var holi = $(this).data('vals');

        if (!holi)
            return;

        var date = holi.SDay.replace('/', '');
        var sdt = new Date(date.substring(0, 4), parseInt(date.substring(4, 6)) - 1, date.substring(6, 8));
        date = holi.EDay.replace('/', '');
        var edt = new Date(date.substring(0, 4), parseInt(date.substring(4, 6)) - 1, date.substring(6, 8));

        setHoliData(sdt, holi.Comment, '', docid, id, edt, holi.Type);

        $('#modal-date-holi').modal();

        e.stopPropagation();
    });

    holiW = holiW - 30;

    resizeWindow();
    resizeTable();

}

function setEditData(id, sid, date, start, end, txt, sc, dateF) {
    $('#date-from-one input').datepicker('setDate', date);
    $('#date-to-one input').datepicker('setDate', date);

    $('#change-doc-one select').empty();
    $('#change-doc-one select').append($('<option>').val('').append(''));

    var doc;

    for (var i = 0; i < Doctors.length; i++) {
        if (Doctors[i].DocID == id) {
            doc = Doctors[i];
        }

        $('#change-doc-one select').append(
            $('<option>').val(Doctors[i].DocID).append(Doctors[i].DocName)
        );
    }

    if (!doc)
    {
        doc = {};
        doc.DocID = id;
        doc.DocName = id;
        doc.DocName_H = '';
        doc.Color = '#FBCDAE';
    }

    $('#modal-date-edit div div div.modal-header').css('background-color', doc.Color);
    $('#edit-title').empty();
    $('#edit-title').append(doc.DocName + ' (' + doc.DocName_H + ') 先生').data('docid', doc.DocID);
    $('#comment-one input').val(txt);

    $('#time-from-one select').val(start);
    $('#time-to-one select').val(end);

    $('#edit-all-one p input')[0].checked = false;
    $('#edit-holi-one p input')[0].checked = false;

    if (sc)
    {
        if (sc.IsAll == 1)
            $('#edit-all-one p input')[0].checked = true;
        if (sc.IsDelete == 1)
            $('#edit-holi-one p input')[0].checked = true;
        if(sc.SubDocID && sc.SubDocID != '')
            $('#change-doc-one select').val(sc.SubDocID);

        if (sc.SubDocID && sc.SubDocID != ''){
            $('#del_change').data('id', sc.SubDocID).data('subid', sid).data('date', sc.Date);
            $('#set_change').data('id', sc.SubDocID).data('subid', sid);
        }
        else {
            $('#del_change').data('id', sid).data('subid', '').data('date', sc.Date);
            $('#set_change').data('id', sid).data('subid', '');
        }
    } else {
        $('#del_change').data('id', sid).data('subid', '').data('date', dateF);
        $('#set_change').data('id', sid).data('subid', '');
    }

    $('#modal-date-edit').modal('hide');
}

function setMailTitle(ret)
{
    var list = ret.d;

    $('#mail-list').empty();
    $('#mail-list').append($('<option>').attr('value', '').append(''));
    $.each(list, function (index, li) {
        $('#mail-list').append(
            $('<option>').attr('value', li).append(li)
        );
    });
}

function setHoliData(date, comment, title, docid, id, edate, type) {
    $('#doc-holi-list').empty();
    $('#doc-holi-list').append($('<option>').attr('value', '').append(''));

    var setDoc = '';

    if (!id)
        id = '';

    $('#del_holi').data('id', id);
    $('#set_holi').data('id', id);

    $.each(Doctors, function (index, doc) {
        $('#doc-holi-list').append(
            $('<option>').attr('value', doc.DocID).append(doc.DocName).data('color', doc.Color)
        );
    });

    if (docid && docid != -1)
        $('#doc-holi-list').val(docid);

    $('#date-from-holi input').datepicker('setDate', date);
    if (!edate)
        $('#date-to-holi input').datepicker('setDate', date);
    else
        $('#date-to-holi input').datepicker('setDate', edate);

    $('#comment-holi input').val("");
    if (comment)
        $('#comment-holi input').val(comment);


    $('#edit-all-holi p input')[0].checked = false;
    $('#edit-doc-holi p input')[0].checked = false;
    $('#edit-other-holi p input')[0].checked = false;

    if (type == 0) {
        $('#edit-doc-holi p input')[0].checked = true;
    } else if (type == 1) {
        $('#edit-other-holi p input')[0].checked = true;
    } else if (type == 2) {
        $('#edit-all-holi p input')[0].checked = true;
    }
}

function setNewData(date, start, end, type, sub) {
    $('#change-doc-list').val('');
    $('#c-doc-list').empty();
    $.each(Doctors, function (index, doc) {
        $('#c-doc-list').append(
        $('<option>').attr('data-id', doc.DocID).append(doc.DocName).data('color', doc.Color)
        );
    });

    $('#date-from-new input').datepicker('setDate', date);
    $('#date-to-new input').datepicker('setDate', date);

    $('#edit-in-new p input')[0].checked = false;
    $('#edit-out-new p input')[0].checked = false;
    $('#edit-other-new p input')[0].checked = false;
    $('#edit-otherdat-new p input')[0].checked = false;
    $('#edit-help-new p input')[0].checked = false;

    if (type == 0)
        $('#edit-in-new p input')[0].checked = true;
    else if(type == 1)
        $('#edit-out-new p input')[0].checked = true;
    else if (type == 99)
        $('#edit-other-new p input')[0].checked = true;

    if (sub == 0)
        $('#edit-help-new p input')[0].checked = true;
    else if (sub == 1)
        $('#edit-otherdat-new p input')[0].checked = true;

    $('#time-from-new select').val(start);
    $('#time-to-new select').val(end);
}


function getStartTimeID(left) {
    var start = 0;
    if (left >= 30) {
        start = (left - 30) / 15;
        start++;
    }

    return start;
}
function getEndTimeID(left, width) {
    var end = 0;

    end = parseInt((left + width) / 15);

    if (end > 46)
        end = 46;

    return end;
}

function getDateFormat(sch)
{
    var date = new Date(sch.Date.substring(0, 4), parseInt(sch.Date.substring(4, 6)) -1, sch.Date.substring(6, 8));
    var date_2 = ["日", "月", "火", "水", "木", "金", "土"][date.getDay()];

    ret = sch.Date.substring(4, 6) + '月' + sch.Date.substring(6, 8) + '日 ' + '(' + date_2 + ') ' + sch.Stime + '～' + sch.Etime;

    return ret;
}

function resizeWindow() {
    var height = $(window).height() - 124 - $('footer').height();

    $('#table-div').height(height);

}

function setMemo_U(ret) {
    $('#user-memo').text(ret.d);
}

function resizeTable() {
    var totalW = $(window).width();

    var width = $('#holi-head').width();

    $('body').css('min-width', (holiW + 1200) + 'px');

    if (width < holiW) {
        width = holiW;
    }

    //$('td.title-date').each(function () {
    //    $(this).width(width2);
    //});
    $('div.t-holiday-body').each(function () {
        $(this).width(width);
    });
}

function InitializationNode(node, params) {
    if (params['outside'] == true) {
        node.animate({
            'top': gNodeTop,
            'left': gNodeLeft,
            'width': gNodeWidth,
        }, 100, 'easeInOutQuart');
    }
    console.log(params);
};

/**
 * 要素の中身の書き換え
 */
function rewrite(node, params) {
    var str = 'start&nbsp;:&nbsp;' + params['changeStartTimeF'];
    str += '&nbsp;end&nbsp;:&nbsp;' + params['changeEndTimeF'];
    node.find('p').html(str);
}

function InitHTML(day, date) {

    if (!day)
        day = 0;
    if (!date)
        date = new Date();

    date.setDate(date.getDate() + day);
    $('#body-table tbody').empty();


    var $body = $('#body-table tbody')
    for(var i = 0; i < 7; i++)
    {
        var isHoli = false;
        var $tr = $('<tr>');
        var t_date = date.getFullYear() + ("0" + (date.getMonth() + 1)).slice(-2) + ("0" + date.getDate()).slice(-2);
        var t_date_1 = (date.getMonth() + 1) + "/" + date.getDate();
        var t_date_2 = ["日", "月", "火", "水", "木", "金", "土"][date.getDay()];

        if ($.inArray(t_date, SPDays) >= 0)
            isHoli = true;

        var addClassName = "n-day";
        if (isHoli || t_date_2 == "日")
            addClassName = "h-day";
        else if(t_date_2 == "土")
            addClassName = "s-day";

        $tr.append(
            $('<td>').attr('id', 'day-' + i).addClass('title-date').data('date', t_date).append(
                $('<div>').attr('id', 't-day-' + i).addClass('t-date left ' + addClassName).append(
                    $('<span>').append(t_date_1)
                ).append(
                    $('<span>').append('(' + t_date_2 + ')')
                )
            ).append(
                $('<div>').attr('id', 't-holiday-' + i).addClass('t-holiday-body left select')
            )
        );

        var $subTb = $('<table>').attr('id', 'day-' + i + '-body').addClass('day-body').data('date', t_date);

        var $inTb = $('<table>').attr('id', 'day-' + i + '-in');
        $inTb.append(
            $('<tr>').append(
                $('<td>').attr('id', 'day-' + i +'-in-title').addClass('t-place-body').append(
                    $('<span>').append('内')
                )
            ).append(
                $('<td>').attr('id', 'day-' + i +'-in-all').addClass('t-allday-body')
            ).append(
                $('<td>').append(
                    $('<table>').attr('id', 'day-' + i + '-in-time').append(
                        $('<tbody>')
                    )
                )
            )
        );

        var $outTb = $('<table>').attr('id', 'day-' + i + '-out');
        $outTb.append(
            $('<tr>').append(
                $('<td>').attr('id', 'day-' + i + '-out-title').addClass('t-place-body').append(
                    $('<span>').append('外')
                )
            ).append(
                $('<td>').attr('id', 'day-' + i + '-out-all').addClass('t-allday-body')
            ).append(
                $('<td>').append(
                    $('<table>').attr('id', 'day-' + i + '-out-time').append(
                        $('<tbody>')
                    )
                )
            )
        );

        var $otherTb = $('<table>').attr('id', 'day-' + i + '-other');
       $otherTb.append(
            $('<tr>').append(
                $('<td>').attr('id', 'day-' + i + '-other-title').addClass('t-place-body').append(
                    $('<span>').append('他')
                )
            ).append(
                $('<td>').attr('id', 'day-' + i + '-other-all').addClass('t-allday-body')
            ).append(
                $('<td>').append(
                    $('<table>').attr('id', 'day-' + i + '-other-time').append(
                        $('<tbody>')
                    )
                )
            )
        );

        $subTb.append(
            $('<tbody>').append(
                $('<tr>').append(
                    $('<td>').append(
                        $inTb
                    )
                )
            ).append(
                $('<tr>').append(
                    $('<td>').append(
                        $outTb
                    )
                )
            ).append(
                $('<tr>').append(
                    $('<td>').append(
                        $otherTb
                    )
                )
            )
        );
        $tr.append(
            $('<td>').append(
                $subTb
            )
        );

        $body.append($tr);

        $('#day-' + i + '-in-time tbody').append(setDataRow);
        $('#day-' + i + '-out-time tbody').append(setDataRow);
        $('#day-' + i + '-other-time tbody').append(setDataRow);

        var height = 64;

        $('#t-holiday-' + i).height(height);
        $('#t-day-' + i).height(height);

        date.setDate(date.getDate() + 1);
    }

    GetUserMemo(setMemo_U);

}

function addWeekDay() {
    var _tbData = $('<tr>').append($('<td>').addClass('one-line').append($('<div>').addClass('one-line').append(
        $('<div>').addClass('data-ss left').append(
            $('<select>').attr('id', addDay + '-s-day').addClass('weekday').append(
                $('<option>').val('').append('')
            ).append(
                $('<option>').val('1').append('月')
            ).append(
                $('<option>').val('2').append('火')
            ).append(
                $('<option>').val('3').append('水')
            ).append(
                $('<option>').val('4').append('木')
            ).append(
                $('<option>').val('5').append('金')
            ).append(
                $('<option>').val('6').append('土')
            ).append(
                $('<option>').val('0').append('日')
            )
        )
    ).append(
        $('<div>').addClass('data-ss left data-check').append(
            $('<p>').append($('<input>').attr('type', 'checkbox').attr('id', addDay + '-in')).append('内')
        )
    ).append(
        $('<div>').addClass('data-ss left data-check').append(
            $('<p>').append($('<input>').attr('type', 'checkbox').attr('id', addDay + '-out')).append('外')
        )
    ).append(
        $('<div>').addClass('data-s left').append(
            $('<select>').attr('id', addDay + '-s-time').append(
                $('<option>').val('').append('')
            ).append(
                $('<option>').val('0').append('-9:00')
            ).append(
                $('<option>').val('1').append('9:00')
            ).append(
                $('<option>').val('2').append('9:15')
            ).append(
                $('<option>').val('3').append('9:30')
            ).append(
                $('<option>').val('4').append('9:45')
            ).append(
                $('<option>').val('5').append('10:00')
            ).append(
                $('<option>').val('6').append('10:15')
            ).append(
                $('<option>').val('7').append('10:30')
            ).append(
                $('<option>').val('8').append('10:45')
            ).append(
                $('<option>').val('9').append('11:00')
            ).append(
                $('<option>').val('10').append('11:15')
            ).append(
                $('<option>').val('11').append('11:30')
            ).append(
                $('<option>').val('12').append('11:45')
            ).append(
                $('<option>').val('13').append('12:00')
            ).append(
                $('<option>').val('14').append('12:15')
            ).append(
                $('<option>').val('15').append('12:30')
            ).append(
                $('<option>').val('16').append('12:45')
            ).append(
                $('<option>').val('17').append('13:00')
            ).append(
                $('<option>').val('18').append('13:15')
            ).append(
                $('<option>').val('19').append('13:30')
            ).append(
                $('<option>').val('20').append('13:45')
            ).append(
                $('<option>').val('21').append('14:00')
            ).append(
                $('<option>').val('22').append('14:15')
            ).append(
                $('<option>').val('23').append('14:30')
            ).append(
                $('<option>').val('24').append('14:45')
            ).append(
                $('<option>').val('25').append('15:00')
            ).append(
                $('<option>').val('26').append('15:15')
            ).append(
                $('<option>').val('27').append('15:30')
            ).append(
                $('<option>').val('28').append('15:45')
            ).append(
                $('<option>').val('29').append('16:00')
            ).append(
                $('<option>').val('30').append('16:15')
            ).append(
                $('<option>').val('31').append('16:30')
            ).append(
                $('<option>').val('32').append('16:45')
            ).append(
                $('<option>').val('33').append('17:00')
            ).append(
                $('<option>').val('34').append('17:15')
            ).append(
                $('<option>').val('35').append('17:30')
            ).append(
                $('<option>').val('36').append('17:45')
            ).append(
                $('<option>').val('37').append('18:00')
            ).append(
                $('<option>').val('38').append('18:15')
            ).append(
                $('<option>').val('39').append('18:30')
            ).append(
                $('<option>').val('40').append('18:45')
            ).append(
                $('<option>').val('41').append('19:00')
            ).append(
                $('<option>').val('42').append('19:15')
            ).append(
                $('<option>').val('43').append('19:30')
            ).append(
                $('<option>').val('44').append('19:45')
            ).append(
                $('<option>').val('45').append('20:00')
            ).append(
                $('<option>').val('46').append('20:00-')
            )
        )
    ).append(
        $('<div>').addClass('data-ss left').append('～')
    ).append(
        $('<div>').addClass('data-s left').append(
            $('<select>').attr('id', addDay + '-e-time').append(
                $('<option>').val('').append('')
            ).append(
                $('<option>').val('0').append('-9:00')
            ).append(
                $('<option>').val('1').append('9:00')
            ).append(
                $('<option>').val('2').append('9:15')
            ).append(
                $('<option>').val('3').append('9:30')
            ).append(
                $('<option>').val('4').append('9:45')
            ).append(
                $('<option>').val('5').append('10:00')
            ).append(
                $('<option>').val('6').append('10:15')
            ).append(
                $('<option>').val('7').append('10:30')
            ).append(
                $('<option>').val('8').append('10:45')
            ).append(
                $('<option>').val('9').append('11:00')
            ).append(
                $('<option>').val('10').append('11:15')
            ).append(
                $('<option>').val('11').append('11:30')
            ).append(
                $('<option>').val('12').append('11:45')
            ).append(
                $('<option>').val('13').append('12:00')
            ).append(
                $('<option>').val('14').append('12:15')
            ).append(
                $('<option>').val('15').append('12:30')
            ).append(
                $('<option>').val('16').append('12:45')
            ).append(
                $('<option>').val('17').append('13:00')
            ).append(
                $('<option>').val('18').append('13:15')
            ).append(
                $('<option>').val('19').append('13:30')
            ).append(
                $('<option>').val('20').append('13:45')
            ).append(
                $('<option>').val('21').append('14:00')
            ).append(
                $('<option>').val('22').append('14:15')
            ).append(
                $('<option>').val('23').append('14:30')
            ).append(
                $('<option>').val('24').append('14:45')
            ).append(
                $('<option>').val('25').append('15:00')
            ).append(
                $('<option>').val('26').append('15:15')
            ).append(
                $('<option>').val('27').append('15:30')
            ).append(
                $('<option>').val('28').append('15:45')
            ).append(
                $('<option>').val('29').append('16:00')
            ).append(
                $('<option>').val('30').append('16:15')
            ).append(
                $('<option>').val('31').append('16:30')
            ).append(
                $('<option>').val('32').append('16:45')
            ).append(
                $('<option>').val('33').append('17:00')
            ).append(
                $('<option>').val('34').append('17:15')
            ).append(
                $('<option>').val('35').append('17:30')
            ).append(
                $('<option>').val('36').append('17:45')
            ).append(
                $('<option>').val('37').append('18:00')
            ).append(
                $('<option>').val('38').append('18:15')
            ).append(
                $('<option>').val('39').append('18:30')
            ).append(
                $('<option>').val('40').append('18:45')
            ).append(
                $('<option>').val('41').append('19:00')
            ).append(
                $('<option>').val('42').append('19:15')
            ).append(
                $('<option>').val('43').append('19:30')
            ).append(
                $('<option>').val('44').append('19:45')
            ).append(
                $('<option>').val('45').append('20:00')
            ).append(
                $('<option>').val('46').append('20:00-')
            )
        )
    ).append(
        $('<div>').addClass('data-ss left data-check').append(
            $('<p>').append($('<input>').attr('type', 'checkbox').attr('id', addDay + '-other')).append('県尼')
        )
    ).append(
        $('<div>').addClass('data-ss left data-check').append(
            $('<p>').append($('<input>').attr('type', 'checkbox').attr('id', addDay + '-help')).append('ﾍﾙﾌﾟ')
        )
    )
    ).append($('<div>').addClass('one-line').append(
        $('<div>').addClass('data-xm left').append(
            $('<input>').attr('id', addDay + '-count')
        )
    ).append(
        $('<div>').addClass('left data-label').append('開始')
    ).append(
        $('<div>').addClass('data-m left').append(
            $('<input>').attr('id', addDay + '-s-date').addClass('datepicker')
        )
    ).append(
        $('<div>').addClass('left data-label').append('終了')
    ).append(
        $('<div>').addClass('data-m left').append(
            $('<input>').attr('id', addDay + '-e-date').addClass('datepicker')
        )
    )
    ));

    $('#date-table tbody').append(_tbData);

    $('.datepicker').each(function () {
        $(this).datepicker({
            language: 'ja'
        });
    });

    addDay = addDay + 1;
}

function addDelWeekDay() {
    var _tbData = $('<tr>').append($('<td>').addClass('one-line').append($('<div>').addClass('one-line').append(
        $('<div>').addClass('data-ss left').append(
            $('<select>').attr('id', addDay + '-s-day').addClass('weekday').attr('disabled', 'disabled').append(
                $('<option>').val('').append('')
            ).append(
                $('<option>').val('1').append('月')
            ).append(
                $('<option>').val('2').append('火')
            ).append(
                $('<option>').val('3').append('水')
            ).append(
                $('<option>').val('4').append('木')
            ).append(
                $('<option>').val('5').append('金')
            ).append(
                $('<option>').val('6').append('土')
            ).append(
                $('<option>').val('0').append('日')
            )
        )
    ).append(
        $('<div>').addClass('data-ss left data-check').append(
            $('<p>').append($('<input>').attr('disabled', 'disabled').attr('type', 'checkbox').attr('id', addDay + '-in')).append('内')
        )
    ).append(
        $('<div>').addClass('data-ss left data-check').append(
            $('<p>').append($('<input>').attr('disabled', 'disabled').attr('type', 'checkbox').attr('id', addDay + '-out')).append('外')
        )
    ).append(
        $('<div>').addClass('data-s left').append(
            $('<select>').attr('disabled', 'disabled').attr('id', addDay + '-s-time').append(
                $('<option>').val('').append('')
            ).append(
                $('<option>').val('0').append('-9:00')
            ).append(
                $('<option>').val('1').append('9:00')
            ).append(
                $('<option>').val('2').append('9:15')
            ).append(
                $('<option>').val('3').append('9:30')
            ).append(
                $('<option>').val('4').append('9:45')
            ).append(
                $('<option>').val('5').append('10:00')
            ).append(
                $('<option>').val('6').append('10:15')
            ).append(
                $('<option>').val('7').append('10:30')
            ).append(
                $('<option>').val('8').append('10:45')
            ).append(
                $('<option>').val('9').append('11:00')
            ).append(
                $('<option>').val('10').append('11:15')
            ).append(
                $('<option>').val('11').append('11:30')
            ).append(
                $('<option>').val('12').append('11:45')
            ).append(
                $('<option>').val('13').append('12:00')
            ).append(
                $('<option>').val('14').append('12:15')
            ).append(
                $('<option>').val('15').append('12:30')
            ).append(
                $('<option>').val('16').append('12:45')
            ).append(
                $('<option>').val('17').append('13:00')
            ).append(
                $('<option>').val('18').append('13:15')
            ).append(
                $('<option>').val('19').append('13:30')
            ).append(
                $('<option>').val('20').append('13:45')
            ).append(
                $('<option>').val('21').append('14:00')
            ).append(
                $('<option>').val('22').append('14:15')
            ).append(
                $('<option>').val('23').append('14:30')
            ).append(
                $('<option>').val('24').append('14:45')
            ).append(
                $('<option>').val('25').append('15:00')
            ).append(
                $('<option>').val('26').append('15:15')
            ).append(
                $('<option>').val('27').append('15:30')
            ).append(
                $('<option>').val('28').append('15:45')
            ).append(
                $('<option>').val('29').append('16:00')
            ).append(
                $('<option>').val('30').append('16:15')
            ).append(
                $('<option>').val('31').append('16:30')
            ).append(
                $('<option>').val('32').append('16:45')
            ).append(
                $('<option>').val('33').append('17:00')
            ).append(
                $('<option>').val('34').append('17:15')
            ).append(
                $('<option>').val('35').append('17:30')
            ).append(
                $('<option>').val('36').append('17:45')
            ).append(
                $('<option>').val('37').append('18:00')
            ).append(
                $('<option>').val('38').append('18:15')
            ).append(
                $('<option>').val('39').append('18:30')
            ).append(
                $('<option>').val('40').append('18:45')
            ).append(
                $('<option>').val('41').append('19:00')
            ).append(
                $('<option>').val('42').append('19:15')
            ).append(
                $('<option>').val('43').append('19:30')
            ).append(
                $('<option>').val('44').append('19:45')
            ).append(
                $('<option>').val('45').append('20:00')
            ).append(
                $('<option>').val('46').append('20:00-')
            )
        )
    ).append(
        $('<div>').addClass('data-ss left').append('～')
    ).append(
        $('<div>').addClass('data-s left').append(
            $('<select>').attr('disabled', 'disabled').attr('id', addDay + '-e-time').append(
                $('<option>').val('').append('')
            ).append(
                $('<option>').val('0').append('-9:00')
            ).append(
                $('<option>').val('1').append('9:00')
            ).append(
                $('<option>').val('2').append('9:15')
            ).append(
                $('<option>').val('3').append('9:30')
            ).append(
                $('<option>').val('4').append('9:45')
            ).append(
                $('<option>').val('5').append('10:00')
            ).append(
                $('<option>').val('6').append('10:15')
            ).append(
                $('<option>').val('7').append('10:30')
            ).append(
                $('<option>').val('8').append('10:45')
            ).append(
                $('<option>').val('9').append('11:00')
            ).append(
                $('<option>').val('10').append('11:15')
            ).append(
                $('<option>').val('11').append('11:30')
            ).append(
                $('<option>').val('12').append('11:45')
            ).append(
                $('<option>').val('13').append('12:00')
            ).append(
                $('<option>').val('14').append('12:15')
            ).append(
                $('<option>').val('15').append('12:30')
            ).append(
                $('<option>').val('16').append('12:45')
            ).append(
                $('<option>').val('17').append('13:00')
            ).append(
                $('<option>').val('18').append('13:15')
            ).append(
                $('<option>').val('19').append('13:30')
            ).append(
                $('<option>').val('20').append('13:45')
            ).append(
                $('<option>').val('21').append('14:00')
            ).append(
                $('<option>').val('22').append('14:15')
            ).append(
                $('<option>').val('23').append('14:30')
            ).append(
                $('<option>').val('24').append('14:45')
            ).append(
                $('<option>').val('25').append('15:00')
            ).append(
                $('<option>').val('26').append('15:15')
            ).append(
                $('<option>').val('27').append('15:30')
            ).append(
                $('<option>').val('28').append('15:45')
            ).append(
                $('<option>').val('29').append('16:00')
            ).append(
                $('<option>').val('30').append('16:15')
            ).append(
                $('<option>').val('31').append('16:30')
            ).append(
                $('<option>').val('32').append('16:45')
            ).append(
                $('<option>').val('33').append('17:00')
            ).append(
                $('<option>').val('34').append('17:15')
            ).append(
                $('<option>').val('35').append('17:30')
            ).append(
                $('<option>').val('36').append('17:45')
            ).append(
                $('<option>').val('37').append('18:00')
            ).append(
                $('<option>').val('38').append('18:15')
            ).append(
                $('<option>').val('39').append('18:30')
            ).append(
                $('<option>').val('40').append('18:45')
            ).append(
                $('<option>').val('41').append('19:00')
            ).append(
                $('<option>').val('42').append('19:15')
            ).append(
                $('<option>').val('43').append('19:30')
            ).append(
                $('<option>').val('44').append('19:45')
            ).append(
                $('<option>').val('45').append('20:00')
            ).append(
                $('<option>').val('46').append('20:00-')
            )
        )
    ).append(
        $('<div>').addClass('data-ss left data-check').append(
            $('<p>').append($('<input>').attr('disabled', 'disabled').attr('type', 'checkbox').attr('id', addDay + '-other')).append('県尼')
        )
    ).append(
        $('<div>').addClass('data-ss left data-check').append(
            $('<p>').append($('<input>').attr('disabled', 'disabled').attr('type', 'checkbox').attr('id', addDay + '-help')).append('ﾍﾙﾌﾟ')
        )
    )
    ).append($('<div>').addClass('one-line').append(
        $('<div>').addClass('data-xm left').append(
            $('<input>').attr('disabled', 'disabled').attr('id', addDay + '-count')
        )
    ).append(
        $('<div>').addClass('left data-label').append('開始')
    ).append(
        $('<div>').addClass('data-m left').append(
            $('<input>').attr('disabled', 'disabled').attr('id', addDay + '-s-date')
        )
    ).append(
        $('<div>').addClass('left data-label').append('終了')
    ).append(
        $('<div>').addClass('data-m left').append(
            $('<input>').attr('disabled', 'disabled').attr('id', addDay + '-e-date')
        )
    )
    ));

    $('#date-table-del tbody').append(_tbData);

    addDay = addDay + 1;
}

function addWeekDay_Last() {
    var _tbData = $('<tr>').append($('<td>').addClass('one-line').append($('<div>').addClass('one-line').append(
    $('<div>').addClass('data-ss left').append(
        $('<select>').attr('id', addDay + '-s-day').addClass('weekday last-data').append(
            $('<option>').val('').append('')
        ).append(
            $('<option>').val('1').append('月')
        ).append(
            $('<option>').val('2').append('火')
        ).append(
            $('<option>').val('3').append('水')
        ).append(
            $('<option>').val('4').append('木')
        ).append(
            $('<option>').val('5').append('金')
        ).append(
            $('<option>').val('6').append('土')
        ).append(
            $('<option>').val('0').append('日')
        )
    )
).append(
    $('<div>').addClass('data-ss left data-check').append(
        $('<p>').append($('<input>').attr('type', 'checkbox').attr('id', addDay + '-in')).append('内')
    )
).append(
    $('<div>').addClass('data-ss left data-check').append(
        $('<p>').append($('<input>').attr('type', 'checkbox').attr('id', addDay + '-out')).append('外')
    )
).append(
    $('<div>').addClass('data-s left').append(
        $('<select>').attr('id', addDay + '-s-time').append(
            $('<option>').val('').append('')
        ).append(
            $('<option>').val('0').append('-9:00')
        ).append(
            $('<option>').val('1').append('9:00')
        ).append(
            $('<option>').val('2').append('9:15')
        ).append(
            $('<option>').val('3').append('9:30')
        ).append(
            $('<option>').val('4').append('9:45')
        ).append(
            $('<option>').val('5').append('10:00')
        ).append(
            $('<option>').val('6').append('10:15')
        ).append(
            $('<option>').val('7').append('10:30')
        ).append(
            $('<option>').val('8').append('10:45')
        ).append(
            $('<option>').val('9').append('11:00')
        ).append(
            $('<option>').val('10').append('11:15')
        ).append(
            $('<option>').val('11').append('11:30')
        ).append(
            $('<option>').val('12').append('11:45')
        ).append(
            $('<option>').val('13').append('12:00')
        ).append(
            $('<option>').val('14').append('12:15')
        ).append(
            $('<option>').val('15').append('12:30')
        ).append(
            $('<option>').val('16').append('12:45')
        ).append(
            $('<option>').val('17').append('13:00')
        ).append(
            $('<option>').val('18').append('13:15')
        ).append(
            $('<option>').val('19').append('13:30')
        ).append(
            $('<option>').val('20').append('13:45')
        ).append(
            $('<option>').val('21').append('14:00')
        ).append(
            $('<option>').val('22').append('14:15')
        ).append(
            $('<option>').val('23').append('14:30')
        ).append(
            $('<option>').val('24').append('14:45')
        ).append(
            $('<option>').val('25').append('15:00')
        ).append(
            $('<option>').val('26').append('15:15')
        ).append(
            $('<option>').val('27').append('15:30')
        ).append(
            $('<option>').val('28').append('15:45')
        ).append(
            $('<option>').val('29').append('16:00')
        ).append(
            $('<option>').val('30').append('16:15')
        ).append(
            $('<option>').val('31').append('16:30')
        ).append(
            $('<option>').val('32').append('16:45')
        ).append(
            $('<option>').val('33').append('17:00')
        ).append(
            $('<option>').val('34').append('17:15')
        ).append(
            $('<option>').val('35').append('17:30')
        ).append(
            $('<option>').val('36').append('17:45')
        ).append(
            $('<option>').val('37').append('18:00')
        ).append(
            $('<option>').val('38').append('18:15')
        ).append(
            $('<option>').val('39').append('18:30')
        ).append(
            $('<option>').val('40').append('18:45')
        ).append(
            $('<option>').val('41').append('19:00')
        ).append(
            $('<option>').val('42').append('19:15')
        ).append(
            $('<option>').val('43').append('19:30')
        ).append(
            $('<option>').val('44').append('19:45')
        ).append(
            $('<option>').val('45').append('20:00')
        ).append(
            $('<option>').val('46').append('20:00-')
        )
    )
).append(
    $('<div>').addClass('data-ss left').append('～')
).append(
    $('<div>').addClass('data-s left').append(
        $('<select>').attr('id', addDay + '-e-time').append(
            $('<option>').val('').append('')
        ).append(
            $('<option>').val('0').append('-9:00')
        ).append(
            $('<option>').val('1').append('9:00')
        ).append(
            $('<option>').val('2').append('9:15')
        ).append(
            $('<option>').val('3').append('9:30')
        ).append(
            $('<option>').val('4').append('9:45')
        ).append(
            $('<option>').val('5').append('10:00')
        ).append(
            $('<option>').val('6').append('10:15')
        ).append(
            $('<option>').val('7').append('10:30')
        ).append(
            $('<option>').val('8').append('10:45')
        ).append(
            $('<option>').val('9').append('11:00')
        ).append(
            $('<option>').val('10').append('11:15')
        ).append(
            $('<option>').val('11').append('11:30')
        ).append(
            $('<option>').val('12').append('11:45')
        ).append(
            $('<option>').val('13').append('12:00')
        ).append(
            $('<option>').val('14').append('12:15')
        ).append(
            $('<option>').val('15').append('12:30')
        ).append(
            $('<option>').val('16').append('12:45')
        ).append(
            $('<option>').val('17').append('13:00')
        ).append(
            $('<option>').val('18').append('13:15')
        ).append(
            $('<option>').val('19').append('13:30')
        ).append(
            $('<option>').val('20').append('13:45')
        ).append(
            $('<option>').val('21').append('14:00')
        ).append(
            $('<option>').val('22').append('14:15')
        ).append(
            $('<option>').val('23').append('14:30')
        ).append(
            $('<option>').val('24').append('14:45')
        ).append(
            $('<option>').val('25').append('15:00')
        ).append(
            $('<option>').val('26').append('15:15')
        ).append(
            $('<option>').val('27').append('15:30')
        ).append(
            $('<option>').val('28').append('15:45')
        ).append(
            $('<option>').val('29').append('16:00')
        ).append(
            $('<option>').val('30').append('16:15')
        ).append(
            $('<option>').val('31').append('16:30')
        ).append(
            $('<option>').val('32').append('16:45')
        ).append(
            $('<option>').val('33').append('17:00')
        ).append(
            $('<option>').val('34').append('17:15')
        ).append(
            $('<option>').val('35').append('17:30')
        ).append(
            $('<option>').val('36').append('17:45')
        ).append(
            $('<option>').val('37').append('18:00')
        ).append(
            $('<option>').val('38').append('18:15')
        ).append(
            $('<option>').val('39').append('18:30')
        ).append(
            $('<option>').val('40').append('18:45')
        ).append(
            $('<option>').val('41').append('19:00')
        ).append(
            $('<option>').val('42').append('19:15')
        ).append(
            $('<option>').val('43').append('19:30')
        ).append(
            $('<option>').val('44').append('19:45')
        ).append(
            $('<option>').val('45').append('20:00')
        ).append(
            $('<option>').val('46').append('20:00-')
        )
    )
).append(
    $('<div>').addClass('data-ss left data-check').append(
        $('<p>').append($('<input>').attr('type', 'checkbox').attr('id', addDay + '-other')).append('県尼')
    )
).append(
    $('<div>').addClass('data-ss left data-check').append(
        $('<p>').append($('<input>').attr('type', 'checkbox').attr('id', addDay + '-help')).append('ﾍﾙﾌﾟ')
    )
)
).append($('<div>').addClass('one-line').append(
    $('<div>').addClass('data-xm left').append(
        $('<input>').attr('id', addDay + '-count')
    )
).append(
    $('<div>').addClass('left data-label').append('開始')
).append(
    $('<div>').addClass('data-m left').append(
        $('<input>').attr('id', addDay + '-s-date').addClass('datepicker')
    )
).append(
    $('<div>').addClass('left data-label').append('終了')
).append(
    $('<div>').addClass('data-m left').append(
        $('<input>').attr('id', addDay + '-e-date').addClass('datepicker')
    )
)
));

    $('#date-table tbody').append(_tbData);
    addDay = addDay + 1;

    $('.datepicker').each(function () {
        $(this).datepicker({
            language: 'ja'
        });
    });


    //var _tbData = $('<tr>').append(
    //    $('<td>').addClass('data-s').append(
    //        $('<select>').attr('id', addDay + '-s-day').addClass('weekday last-data').append(
    //            $('<option>').val('').append('')
    //        ).append(
    //            $('<option>').val('1').append('月')
    //        ).append(
    //            $('<option>').val('2').append('火')
    //        ).append(
    //            $('<option>').val('3').append('水')
    //        ).append(
    //            $('<option>').val('4').append('木')
    //        ).append(
    //            $('<option>').val('5').append('金')
    //        ).append(
    //            $('<option>').val('6').append('土')
    //        ).append(
    //            $('<option>').val('0').append('日')
    //        )
    //    )
    //).append(
    //    $('<td>').addClass('data-ss').append(
    //        $('<input>').attr('type', 'checkbox').attr('id', addDay + '-in')
    //    )
    //).append(
    //    $('<td>').addClass('data-ss').append(
    //        $('<input>').attr('type', 'checkbox').attr('id', addDay + '-out')
    //    )
    //).append(
    //    $('<td>').addClass('data-ss').append(
    //        $('<select>').attr('id', addDay + '-s-time').append(
    //            $('<option>').val('').append('')
    //        ).append(
    //            $('<option>').val('0').append('-9:00')
    //        ).append(
    //            $('<option>').val('1').append('9:00')
    //        ).append(
    //            $('<option>').val('2').append('9:15')
    //        ).append(
    //            $('<option>').val('3').append('9:30')
    //        ).append(
    //            $('<option>').val('4').append('9:45')
    //        ).append(
    //            $('<option>').val('5').append('10:00')
    //        ).append(
    //            $('<option>').val('6').append('10:15')
    //        ).append(
    //            $('<option>').val('7').append('10:30')
    //        ).append(
    //            $('<option>').val('8').append('10:45')
    //        ).append(
    //            $('<option>').val('9').append('11:00')
    //        ).append(
    //            $('<option>').val('10').append('11:15')
    //        ).append(
    //            $('<option>').val('11').append('11:30')
    //        ).append(
    //            $('<option>').val('12').append('11:45')
    //        ).append(
    //            $('<option>').val('13').append('12:00')
    //        ).append(
    //            $('<option>').val('14').append('12:15')
    //        ).append(
    //            $('<option>').val('15').append('12:30')
    //        ).append(
    //            $('<option>').val('16').append('12:45')
    //        ).append(
    //            $('<option>').val('17').append('13:00')
    //        ).append(
    //            $('<option>').val('18').append('13:15')
    //        ).append(
    //            $('<option>').val('19').append('13:30')
    //        ).append(
    //            $('<option>').val('20').append('13:45')
    //        ).append(
    //            $('<option>').val('21').append('14:00')
    //        ).append(
    //            $('<option>').val('22').append('14:15')
    //        ).append(
    //            $('<option>').val('23').append('14:30')
    //        ).append(
    //            $('<option>').val('24').append('14:45')
    //        ).append(
    //            $('<option>').val('25').append('15:00')
    //        ).append(
    //            $('<option>').val('26').append('15:15')
    //        ).append(
    //            $('<option>').val('27').append('15:30')
    //        ).append(
    //            $('<option>').val('28').append('15:45')
    //        ).append(
    //            $('<option>').val('29').append('16:00')
    //        ).append(
    //            $('<option>').val('30').append('16:15')
    //        ).append(
    //            $('<option>').val('31').append('16:30')
    //        ).append(
    //            $('<option>').val('32').append('16:45')
    //        ).append(
    //            $('<option>').val('33').append('17:00')
    //        ).append(
    //            $('<option>').val('34').append('17:15')
    //        ).append(
    //            $('<option>').val('35').append('17:30')
    //        ).append(
    //            $('<option>').val('36').append('17:45')
    //        ).append(
    //            $('<option>').val('37').append('18:00')
    //        ).append(
    //            $('<option>').val('38').append('18:15')
    //        ).append(
    //            $('<option>').val('39').append('18:30')
    //        ).append(
    //            $('<option>').val('40').append('18:45')
    //        ).append(
    //            $('<option>').val('41').append('19:00')
    //        ).append(
    //            $('<option>').val('42').append('19:15')
    //        ).append(
    //            $('<option>').val('43').append('19:30')
    //        ).append(
    //            $('<option>').val('44').append('19:45')
    //        ).append(
    //            $('<option>').val('45').append('20:00')
    //        ).append(
    //            $('<option>').val('46').append('20:00-')
    //        )
    //    )
    //).append(
    //    $('<td>').addClass('data-ss').append('～')
    //).append(
    //    $('<td>').addClass('data-ss').append(
    //        $('<select>').attr('id', addDay + '-e-time').append(
    //            $('<option>').val('').append('')
    //        ).append(
    //            $('<option>').val('0').append('-9:00')
    //        ).append(
    //            $('<option>').val('1').append('9:00')
    //        ).append(
    //            $('<option>').val('2').append('9:15')
    //        ).append(
    //            $('<option>').val('3').append('9:30')
    //        ).append(
    //            $('<option>').val('4').append('9:45')
    //        ).append(
    //            $('<option>').val('5').append('10:00')
    //        ).append(
    //            $('<option>').val('6').append('10:15')
    //        ).append(
    //            $('<option>').val('7').append('10:30')
    //        ).append(
    //            $('<option>').val('8').append('10:45')
    //        ).append(
    //            $('<option>').val('9').append('11:00')
    //        ).append(
    //            $('<option>').val('10').append('11:15')
    //        ).append(
    //            $('<option>').val('11').append('11:30')
    //        ).append(
    //            $('<option>').val('12').append('11:45')
    //        ).append(
    //            $('<option>').val('13').append('12:00')
    //        ).append(
    //            $('<option>').val('14').append('12:15')
    //        ).append(
    //            $('<option>').val('15').append('12:30')
    //        ).append(
    //            $('<option>').val('16').append('12:45')
    //        ).append(
    //            $('<option>').val('17').append('13:00')
    //        ).append(
    //            $('<option>').val('18').append('13:15')
    //        ).append(
    //            $('<option>').val('19').append('13:30')
    //        ).append(
    //            $('<option>').val('20').append('13:45')
    //        ).append(
    //            $('<option>').val('21').append('14:00')
    //        ).append(
    //            $('<option>').val('22').append('14:15')
    //        ).append(
    //            $('<option>').val('23').append('14:30')
    //        ).append(
    //            $('<option>').val('24').append('14:45')
    //        ).append(
    //            $('<option>').val('25').append('15:00')
    //        ).append(
    //            $('<option>').val('26').append('15:15')
    //        ).append(
    //            $('<option>').val('27').append('15:30')
    //        ).append(
    //            $('<option>').val('28').append('15:45')
    //        ).append(
    //            $('<option>').val('29').append('16:00')
    //        ).append(
    //            $('<option>').val('30').append('16:15')
    //        ).append(
    //            $('<option>').val('31').append('16:30')
    //        ).append(
    //            $('<option>').val('32').append('16:45')
    //        ).append(
    //            $('<option>').val('33').append('17:00')
    //        ).append(
    //            $('<option>').val('34').append('17:15')
    //        ).append(
    //            $('<option>').val('35').append('17:30')
    //        ).append(
    //            $('<option>').val('36').append('17:45')
    //        ).append(
    //            $('<option>').val('37').append('18:00')
    //        ).append(
    //            $('<option>').val('38').append('18:15')
    //        ).append(
    //            $('<option>').val('39').append('18:30')
    //        ).append(
    //            $('<option>').val('40').append('18:45')
    //        ).append(
    //            $('<option>').val('41').append('19:00')
    //        ).append(
    //            $('<option>').val('42').append('19:15')
    //        ).append(
    //            $('<option>').val('43').append('19:30')
    //        ).append(
    //            $('<option>').val('44').append('19:45')
    //        ).append(
    //            $('<option>').val('45').append('20:00')
    //        ).append(
    //            $('<option>').val('46').append('20:00-')
    //        )
    //    )
    //).append(
    //    $('<td>').addClass('data-s').append(
    //        $('<input>').attr('id', addDay + '-count')
    //    )
    //).append(
    //    $('<td>').addClass('data-ss').append(
    //        $('<input>').attr('type', 'checkbox').attr('id', addDay + '-other')
    //    )
    //).append(
    //    $('<td>').addClass('data-ss').append(
    //        $('<input>').attr('type', 'checkbox').attr('id', addDay + '-help')
    //    )
    //);

    //$('#date-table tbody').append(_tbData);
    //addDay = addDay + 1;
}

function setDataRow() {
    var _tbData = $('<tr>').append(
                $('<td>').addClass('data-td').append(
                    $('<div>').addClass('t-time-sp2 left select').data('no', 0)
                ).append(
                    $('<div>').addClass('t-time-a left select').data('no', 1)
                ).append(
                    $('<div>').addClass('t-time-b left select').data('no', 2)
                ).append(
                    $('<div>').addClass('t-time-c left select').data('no', 3)
                ).append(
                    $('<div>').addClass('t-time-d left select').data('no', 4)
                ).append(
                    $('<div>').addClass('t-time-a left select').data('no', 5)
                ).append(
                    $('<div>').addClass('t-time-b left select').data('no', 6)
                ).append(
                    $('<div>').addClass('t-time-c left select').data('no', 7)
                ).append(
                    $('<div>').addClass('t-time-d left select').data('no', 8)
                ).append(
                    $('<div>').addClass('t-time-a left select').data('no', 9)
                ).append(
                    $('<div>').addClass('t-time-b left select').data('no', 10)
                ).append(
                    $('<div>').addClass('t-time-c left select').data('no', 11)
                ).append(
                    $('<div>').addClass('t-time-d left select').data('no', 12)
                ).append(
                    $('<div>').addClass('t-time-a left select').data('no', 13)
                ).append(
                    $('<div>').addClass('t-time-b left select').data('no', 14)
                ).append(
                    $('<div>').addClass('t-time-c left select').data('no', 15)
                ).append(
                    $('<div>').addClass('t-time-d left select').data('no', 16)
                ).append(
                    $('<div>').addClass('t-time-a left select').data('no', 17)
                ).append(
                    $('<div>').addClass('t-time-b left select').data('no', 18)
                ).append(
                    $('<div>').addClass('t-time-c left select').data('no', 19)
                ).append(
                    $('<div>').addClass('t-time-d left select').data('no', 20)
                ).append(
                    $('<div>').addClass('t-time-a left select').data('no', 21)
                ).append(
                    $('<div>').addClass('t-time-b left select').data('no', 22)
                ).append(
                    $('<div>').addClass('t-time-c left select').data('no', 23)
                ).append(
                    $('<div>').addClass('t-time-d left select').data('no', 24)
                ).append(
                    $('<div>').addClass('t-time-a left select').data('no', 25)
                ).append(
                    $('<div>').addClass('t-time-b left select').data('no', 26)
                ).append(
                    $('<div>').addClass('t-time-c left select').data('no', 27)
                ).append(
                    $('<div>').addClass('t-time-d left select').data('no', 28)
                ).append(
                    $('<div>').addClass('t-time-a left select').data('no', 29)
                ).append(
                    $('<div>').addClass('t-time-b left select').data('no', 30)
                ).append(
                    $('<div>').addClass('t-time-c left select').data('no', 31)
                ).append(
                    $('<div>').addClass('t-time-d left select').data('no', 32)
                ).append(
                    $('<div>').addClass('t-time-a left select').data('no', 33)
                ).append(
                    $('<div>').addClass('t-time-b left select').data('no', 34)
                ).append(
                    $('<div>').addClass('t-time-c left select').data('no', 35)
                ).append(
                    $('<div>').addClass('t-time-d left select').data('no', 36)
                ).append(
                    $('<div>').addClass('t-time-a left select').data('no', 37)
                ).append(
                    $('<div>').addClass('t-time-b left select').data('no', 38)
                ).append(
                    $('<div>').addClass('t-time-c left select').data('no', 39)
                ).append(
                    $('<div>').addClass('t-time-d left select').data('no', 40)
                ).append(
                    $('<div>').addClass('t-time-a left select').data('no', 41)
                ).append(
                    $('<div>').addClass('t-time-b left select').data('no', 42)
                ).append(
                    $('<div>').addClass('t-time-c left select').data('no', 43)
                ).append(
                    $('<div>').addClass('t-time-d left select').data('no', 44)
                ).append(
                    $('<div>').addClass('t-time-a left select').data('no', 45)
                ).append(
                    $('<div>').addClass('t-time-sp3 left select').data('no', 46)
                ).append(
                    $('<div>').addClass('t-sp left')
                ).append(
                    $('<div>').addClass('t-help left')
                )
              );

    return _tbData;
}

function initDoctor() {
    $('#mast-doctor').empty();
    for (var i = 0; i < Doctors.length; i++) {
        var $dc = $('<li>').data('id', Doctors[i].DocID);

        $dc.append(
            $('<div>').addClass('list-doctor').append(
                $('<div>').addClass('left drmark').css('background-color', Doctors[i].Color).css('border-color', Doctors[i].Color)
            ).append(
                $('<div>').addClass('left drname').append(
                    $('<span>').append(Doctors[i].DocName)
                )
            )
        );

        $('#mast-doctor').append($dc);
    }
    var $dc = $('<li>').data('id', '').addClass('divider');
    $('#mast-doctor').append($dc);

    $dc = $('<li>').data('id', '-1');
    $dc.append(
        $('<div>').addClass('list-doctor').append(
            $('<div>').append(
                $('<span>').append('新規登録')
            )
        )
    );
    $('#mast-doctor').append($dc);
}

function ShowLoading() {
    var h = $(window).height();

    $('#loader-bg ,#loader').height(h).css('display', 'block');
}

function CloseLoading() {
    $('#loader-bg').delay(900).fadeOut(800);
    $('#loader').delay(600).fadeOut(300);
}

function postForm(url) {
    var $form = $('<form/>', { 'action': url, 'method': 'post' });
    $form.appendTo(document.body);
    $form.submit();
};

var formatDate = function (date, format) {
    if (!format) format = 'YYYY-MM-DD hh:mm:ss.fff';
    format = format.replace(/YYYY/g, date.getFullYear());
    format = format.replace(/MM/g, ('0' + (date.getMonth() + 1)).slice(-2));
    format = format.replace(/DD/g, ('0' + date.getDate()).slice(-2));
    format = format.replace(/hh/g, ('0' + date.getHours()).slice(-2));
    format = format.replace(/mm/g, ('0' + date.getMinutes()).slice(-2));
    format = format.replace(/ss/g, ('0' + date.getSeconds()).slice(-2));
    if (format.match(/f/g)) {
        var milliSeconds = ('00' + date.getMilliseconds()).slice(-3);
        var length = format.match(/f/g).length;
        for (var i = 0; i < length; i++) format = format.replace(/f/, milliSeconds.substring(i, i + 1));
    }
    return format;
};