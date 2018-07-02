<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Schedule.aspx.cs" Inherits="DocSchedule.Schedule" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Language" content="ja"/>
    <meta http-equiv="Content-type" content="text/html" />
    <meta http-equiv="Content-Style-Type" content="text/css" />
    <meta http-equiv="Content-Script-Type" content="text/javascript" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="Cache-Contdol" content="no-cache" />
    <meta http-equiv="Expires" content="Thu, 01 Dec 1994 16:00:00 GMT" />
    <meta name="viewport" content="initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />

    <link href="css/common/bootstrap-theme.min.css" rel="stylesheet"/>
    <link href="css/common/bootstrap.css" rel="stylesheet"/>

    <link href="css/schedule/main.css" rel="stylesheet"/>

        <link href="css/common/bootstrap-datepicker3.min.css" rel="stylesheet"/>
    <link href="css/common/bootstrap-datepicker3.standalone.min.css" rel="stylesheet"/>
    <link href="css/common/bootstrap.colorpickersliders.css" rel="stylesheet"/>

    <script src="script/common/jquery-1.12.4.min.js"></script>
    <script src="script/common/bootstrap.js"></script>

    <script src="script/common/util.js"></script>

    <script src="script/common/jquery.exresize.0.1.0.js"></script>
    <script src="script/common/jquery-ui-1.10.4.custom.min.js"></script>
    <script src="script/common/jquery-rc_schedule.js"></script>

    <script src="script/common/tinycolor.min.js"></script>
    <script src="script/common/bootstrap.colorpickersliders.js"></script>
    <script src="script/common/bootstrap-datepicker.min.js"></script>
    <script src="script/common/bootstrap-datepicker.ja.min.js"></script>
    
    <script src="script/schedule/const.js"></script>
    <script src="script/schedule/main.js"></script>

    <title></title>
</head>
<body>
    <nav class="navbar navbar-default">
      <div class="container-fluid">
        <div class="navbar-header">
          <button class="navbar-toggle collapsed" type="button" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1">
            <span class="sr-only">Toggle navigation</span>
            <span class="icon-bar"></span>
            <span class="icon-bar"></span>
            <span class="icon-bar"></span>
          </button>
          <span class="navbar-brand" id="title"></span>
        </div>
        <div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
          <ul class="nav navbar-nav navbar-left">
            <li><a class="btn mail" id="mail"></a></li>
            <%--<li><a class="btn" id="alert">通知</a></li>--%>
            <li>
                <a class="btn dropdown-toggle" id="doctor" data-toggle="dropdown"></a>
                <ul class="dropdown-menu" id="mast-doctor">
<%--                  <li>
                      <div id="dr-1" class="list-doctor">
                          <div class="left drmark" id="dm-1"></div>
                          <div class="left drname" id="dn-1"><span>テスト医師</span></div>
                      </div>
                  </li>
                  <li class="divider"></li>
                  <li><div class="list-doctor"><div><span>新規登録</span></div></div></li>--%>
                </ul>
            </li>
            <li><a class="btn memo" id="memo_o"></a></li>
            <%--<li><a class="btn" id="cal">カレ</a></li>--%>
          </ul>
          <ul class="nav navbar-nav navbar-right">
          </ul>
        </div>
      </div>
    </nav>
    <div id="date-fld">
        <div class="left" id="date-sp"></div>
        <div class="left" id="today-div">
            <input id="today" type="button" value="今日"/>
        </div>
        <div class="left" id="date-div">
            <input id="before" type="button" value="<<"/>
            <input id="date" class="datepicker" readonly="readonly"/>
            <input id="after" type="button" value=">>"/>
        </div>
    </div>
    <table id="head-table">
        <thead>
            <tr>
                <td class="t-date"></td>
                <td class="t-holiday" id="holi-head">連続日程</td>
                <td class="t-place"></td>
                <td class="t-allday"></td>
                <td class="t-time-sp"></td>
                <td class="t-time">9:00</td>
                <td class="t-time">10:00</td>
                <td class="t-time">11:00</td>
                <td class="t-time">12:00</td>
                <td class="t-time">13:00</td>
                <td class="t-time">14:00</td>
                <td class="t-time">15:00</td>
                <td class="t-time">16:00</td>
                <td class="t-time">17:00</td>
                <td class="t-time">18:00</td>
                <td class="t-time">19:00</td>
                <td class="t-time">20:00</td>
                <td class="t-time-sp"></td>
                <td class="t-sp">県尼</td>
                <td class="t-help">ヘルプ</td>
                <td class="t-mg"></td>
            </tr>
        </thead>
    </table>
    <div id="table-div">
        <table id="body-table">
            <tbody></tbody>
        </table>
    </div>
    <footer class="footer">
        <form class="form-horizontal" id="sub-form">
            <fieldset>
                <textarea id ="user-memo">

                </textarea>
                <button class="btn" type="button" id="edit-memo-user">更　 新</button>
            </fieldset>
        </form>
    </footer>

    <div class="modal" id="modal-mail">
      <div class="modal-dialog series-dialog">
        <div class="modal-content">
          <div class="modal-header series-header">
            <button class="close" aria-hidden="true" type="button" data-dismiss="modal">&times;</button>
            <h4 class="modal-title"></h4>
          </div>
          <div class="modal-body">
            <div class="one-line">
                <div class="left edit-data">
                    <select id="mail-list">
                    </select>
                </div>
            </div>
            <div class="one-line">
                <div class="left mail-label"><p>件名</p></div>
                <div class="left mail-val edit-data-full" id="mail-title"><input /></div>
            </div>
            <div class="one-line">
                <div class="left mail-label"><p>読影医</p></div>
                <div class="left mail-val edit-data-full">
                    <select id="doc-mail-list">
	                </select>
                </div>
            </div>
            <div class="one-line">
                <div class="left mail-label"><p>To</p></div>
                <div class="left mail-val edit-data-full" id="mail-to"><input /></div>
            </div>
            <div class="one-line">
                <div class="left mail-label"><p>cc</p></div>
                <div class="left mail-val edit-data-full" id="mail-cc"><input /></div>
            </div>
            <div class="multi-line">
                <div class="left mail-multi-val" id="mail-main"><textarea></textarea></div>
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn" type="button" id="send-mail">送　信</button>
          </div>
        </div>
      </div>
    </div>

    <div class="modal" id="modal-date-holi">
      <div class="modal-dialog series-dialog">
        <div class="modal-content">
          <div class="modal-header series-header">
            <button class="close" aria-hidden="true" type="button" data-dismiss="modal">&times;</button>
            <h4 class="modal-title"></h4>
          </div>
          <div class="modal-body">
            <div class="one-line">
                <div class="left edit-data edit-day" id="change-doc-holi">
                    <select id="doc-holi-list">
	                </select>
                </div>
                <div class="left edit-data new-check" id="edit-all-holi"><p><input type="checkbox" />終日</p></div>
                <div class="left edit-data new-check" id="edit-doc-holi"><p><input type="checkbox" />休み</p></div>
                <div class="left edit-data new-check" id="edit-other-holi"><p><input type="checkbox" />その他</p></div>
            </div>
            <div class="one-line">
                <div class="left edit-data-full" id="comment-holi"><input /></div>
            </div>
            <div class="one-line">
                <div class="left edit-data edit-day" id="date-from-holi"><input class="datepicker"/></div>
                <div class="left"><span>～</span></div>
                <div class="left edit-data edit-day" id="date-to-holi"><input class="datepicker" /></div>
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-danger" type="button" id="del_holi">削　除</button>
            <button class="btn" type="button" id="set_holi">保  存</button>
          </div>
        </div>
      </div>
    </div>
    <div class="modal" id="modal-date-new">
      <div class="modal-dialog series-dialog">
        <div class="modal-content">
          <div class="modal-header series-header">
            <button class="close" aria-hidden="true" type="button" data-dismiss="modal">&times;</button>
            <h4 class="modal-title"></h4>
          </div>
          <div class="modal-body">
            <div class="one-line">
                <div class="left edit-data edit-day" id="change-doc-new">
                    <input id="change-doc-list" list="c-doc-list" />
                        <datalist id="c-doc-list">
	                    </datalist>
                </div>
                <div class="left edit-data new-check" id="edit-in-new"><p><input type="checkbox" />内</p></div>
                <div class="left edit-data new-check" id="edit-out-new"><p><input type="checkbox" />外</p></div>
                <div class="left edit-data new-check" id="edit-other-new"><p><input type="checkbox" />他</p></div>
                <div class="left edit-data new-check" id="edit-otherdat-new"><p><input type="checkbox" />県尼</p></div>
                <div class="left edit-data new-check" id="edit-help-new"><p><input type="checkbox" />ﾍﾙﾌﾟ</p></div>
            </div>
            <div class="one-line">
                <div class="left edit-data-full" id="comment-new"><input /></div>
            </div>
            <div class="one-line">
                <div class="left edit-data edit-day" id="date-from-new"><input class="datepicker"/></div>
                <div class="left edit-data" id="time-from-new">
                    <select class="time-one">
                        <option value=""></option>
                        <option value="0">8:30</option>
                        <option value="1">9:00</option>
                        <option value="2">9:15</option>
                        <option value="3">9:30</option>
                        <option value="4">9:45</option>
                        <option value="5">10:00</option>
                        <option value="6">10:15</option>
                        <option value="7">10:30</option>
                        <option value="8">10:45</option>
                        <option value="9">11:00</option>
                        <option value="10">11:15</option>
                        <option value="11">11:30</option>
                        <option value="12">11:45</option>
                        <option value="13">12:00</option>
                        <option value="14">12:15</option>
                        <option value="15">12:30</option>
                        <option value="16">12:45</option>
                        <option value="17">13:00</option>
                        <option value="18">13:15</option>
                        <option value="19">13:30</option>
                        <option value="20">13:45</option>
                        <option value="21">14:00</option>
                        <option value="22">14:15</option>
                        <option value="23">14:30</option>
                        <option value="24">14:45</option>
                        <option value="25">15:00</option>
                        <option value="26">15:15</option>
                        <option value="27">15:30</option>
                        <option value="28">15:45</option>
                        <option value="29">16:00</option>
                        <option value="30">16:15</option>
                        <option value="31">16:30</option>
                        <option value="32">16:45</option>
                        <option value="33">17:00</option>
                        <option value="34">17:15</option>
                        <option value="35">17:30</option>
                        <option value="36">17:45</option>
                        <option value="37">18:00</option>
                        <option value="38">18:15</option>
                        <option value="39">18:30</option>
                        <option value="40">18:45</option>
                        <option value="41">19:00</option>
                        <option value="42">19:15</option>
                        <option value="43">19:30</option>
                        <option value="44">19:45</option>
                        <option value="45">20:00</option>
                        <option value="46">20:00 -</option>
                    </select>
                </div>
                <div class="left"><span>～</span></div>
                <div class="left edit-data edit-day" id="date-to-new"><input class="datepicker" /></div>
                <div class="left edit-data" id="time-to-new">
                    <select class="time-one">
                        <option value=""></option>
                        <option value="0">8:30</option>
                        <option value="1">9:00</option>
                        <option value="2">9:15</option>
                        <option value="3">9:30</option>
                        <option value="4">9:45</option>
                        <option value="5">10:00</option>
                        <option value="6">10:15</option>
                        <option value="7">10:30</option>
                        <option value="8">10:45</option>
                        <option value="9">11:00</option>
                        <option value="10">11:15</option>
                        <option value="11">11:30</option>
                        <option value="12">11:45</option>
                        <option value="13">12:00</option>
                        <option value="14">12:15</option>
                        <option value="15">12:30</option>
                        <option value="16">12:45</option>
                        <option value="17">13:00</option>
                        <option value="18">13:15</option>
                        <option value="19">13:30</option>
                        <option value="20">13:45</option>
                        <option value="21">14:00</option>
                        <option value="22">14:15</option>
                        <option value="23">14:30</option>
                        <option value="24">14:45</option>
                        <option value="25">15:00</option>
                        <option value="26">15:15</option>
                        <option value="27">15:30</option>
                        <option value="28">15:45</option>
                        <option value="29">16:00</option>
                        <option value="30">16:15</option>
                        <option value="31">16:30</option>
                        <option value="32">16:45</option>
                        <option value="33">17:00</option>
                        <option value="34">17:15</option>
                        <option value="35">17:30</option>
                        <option value="36">17:45</option>
                        <option value="37">18:00</option>
                        <option value="38">18:15</option>
                        <option value="39">18:30</option>
                        <option value="40">18:45</option>
                        <option value="41">19:00</option>
                        <option value="42">19:15</option>
                        <option value="43">19:30</option>
                        <option value="44">19:45</option>
                        <option value="45">20:00</option>
                        <option value="46">20:00 -</option>
                    </select>
                </div>
            </div>
            <div class="one-line"  style="visibility:hidden">
                <div class="left edit-data" id="edit-all-new" style="visibility:hidden"><p><input type="checkbox" />終日</p></div>
                <div class="left edit-data" id="edit-holi-new" style="visibility:hidden"><p><input type="checkbox" />休み</p></div>
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn" type="button" id="set_new">保  存</button>
          </div>
        </div>
      </div>
    </div>
    <div class="modal" id="modal-date-edit">
      <div class="modal-dialog series-dialog">
        <div class="modal-content">
          <div class="modal-header series-header">
            <button class="close" aria-hidden="true" type="button" data-dismiss="modal">&times;</button>
            <h4 class="modal-title" id="edit-title"></h4>
          </div>
          <div class="modal-body">
            <div class="one-line">
                <div class="left edit-data-full" id="comment-one"><input /></div>
            </div>
            <div class="one-line">
                <div class="left edit-data edit-day" id="date-from-one"><input class="datepicker"/></div>
                <div class="left edit-data" id="time-from-one">
                    <select class="time-one">
                        <option value=""></option>
                        <option value="0">8:30</option>
                        <option value="1">9:00</option>
                        <option value="2">9:15</option>
                        <option value="3">9:30</option>
                        <option value="4">9:45</option>
                        <option value="5">10:00</option>
                        <option value="6">10:15</option>
                        <option value="7">10:30</option>
                        <option value="8">10:45</option>
                        <option value="9">11:00</option>
                        <option value="10">11:15</option>
                        <option value="11">11:30</option>
                        <option value="12">11:45</option>
                        <option value="13">12:00</option>
                        <option value="14">12:15</option>
                        <option value="15">12:30</option>
                        <option value="16">12:45</option>
                        <option value="17">13:00</option>
                        <option value="18">13:15</option>
                        <option value="19">13:30</option>
                        <option value="20">13:45</option>
                        <option value="21">14:00</option>
                        <option value="22">14:15</option>
                        <option value="23">14:30</option>
                        <option value="24">14:45</option>
                        <option value="25">15:00</option>
                        <option value="26">15:15</option>
                        <option value="27">15:30</option>
                        <option value="28">15:45</option>
                        <option value="29">16:00</option>
                        <option value="30">16:15</option>
                        <option value="31">16:30</option>
                        <option value="32">16:45</option>
                        <option value="33">17:00</option>
                        <option value="34">17:15</option>
                        <option value="35">17:30</option>
                        <option value="36">17:45</option>
                        <option value="37">18:00</option>
                        <option value="38">18:15</option>
                        <option value="39">18:30</option>
                        <option value="40">18:45</option>
                        <option value="41">19:00</option>
                        <option value="42">19:15</option>
                        <option value="43">19:30</option>
                        <option value="44">19:45</option>
                        <option value="45">20:00</option>
                        <option value="46">20:00 -</option>
                    </select>
                </div>
                <div class="left"><span>～</span></div>
                <div class="left edit-data edit-day" id="date-to-one"><input class="datepicker" /></div>
                <div class="left edit-data" id="time-to-one">
                    <select class="time-one">
                        <option value=""></option>
                        <option value="0">8:30</option>
                        <option value="1">9:00</option>
                        <option value="2">9:15</option>
                        <option value="3">9:30</option>
                        <option value="4">9:45</option>
                        <option value="5">10:00</option>
                        <option value="6">10:15</option>
                        <option value="7">10:30</option>
                        <option value="8">10:45</option>
                        <option value="9">11:00</option>
                        <option value="10">11:15</option>
                        <option value="11">11:30</option>
                        <option value="12">11:45</option>
                        <option value="13">12:00</option>
                        <option value="14">12:15</option>
                        <option value="15">12:30</option>
                        <option value="16">12:45</option>
                        <option value="17">13:00</option>
                        <option value="18">13:15</option>
                        <option value="19">13:30</option>
                        <option value="20">13:45</option>
                        <option value="21">14:00</option>
                        <option value="22">14:15</option>
                        <option value="23">14:30</option>
                        <option value="24">14:45</option>
                        <option value="25">15:00</option>
                        <option value="26">15:15</option>
                        <option value="27">15:30</option>
                        <option value="28">15:45</option>
                        <option value="29">16:00</option>
                        <option value="30">16:15</option>
                        <option value="31">16:30</option>
                        <option value="32">16:45</option>
                        <option value="33">17:00</option>
                        <option value="34">17:15</option>
                        <option value="35">17:30</option>
                        <option value="36">17:45</option>
                        <option value="37">18:00</option>
                        <option value="38">18:15</option>
                        <option value="39">18:30</option>
                        <option value="40">18:45</option>
                        <option value="41">19:00</option>
                        <option value="42">19:15</option>
                        <option value="43">19:30</option>
                        <option value="44">19:45</option>
                        <option value="45">20:00</option>
                        <option value="46">20:00 -</option>
                    </select>
                </div>
            </div>
            <div class="one-line">
                <div class="left edit-data" id="edit-all-one" style="visibility:hidden;"><p><input type="checkbox" />終日</p></div>
                <div class="left edit-data" id="edit-holi-one"><p><input type="checkbox" />休み</p></div>
            </div>
            <div class="one-line">
                <div class="left edit-data">
                    <span>代診</span>
                </div>
                <div class="left edit-data edit-day" id="change-doc-one">
                    <select></select>
                </div>
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-danger" type="button" id="del_change">削　除</button>
            <button class="btn" type="button" id="set_change">保  存</button>
          </div>
        </div>
      </div>
    </div>

    <div class="modal" id="modal-doc-mast">
      <div class="modal-dialog series-dialog">
        <div class="modal-content">
          <div class="modal-header series-header">
            <button class="close" aria-hidden="true" type="button" data-dismiss="modal">&times;</button>
            <h4 class="modal-title" id="mast-title"></h4>
            <a class="btn mail" id="mail-doc"></a>
          </div>
          <div class="modal-body">
              <div id="today-data" class="one-line"><span>22222222222</span></div>
              <div class="one-line">
                  <div class="harf-line">
                      <div class="left mast-label">【専門分野】</div>
                      <div class="left view-data" id="main"><span>baerbaerb</span></div>
                  </div>
                  <div class="harf-line">
                      <div class="left mast-label">【スピード】</div>
                      <div class="left view-data"id="speed"><span>beb</span></div>
                  </div>
              </div>
              <div class="one-line">
                  <div class="harf-line">
                      <div class="left mast-label">【コメント】</div>
                      <div class="left view-data" id="comment"><span>sssssssssssssssssss</span></div>
                  </div>
                  <div class="harf-line">
                      <div class="left mast-label">【ステータス】</div>
                      <div class="left view-data" id="count"><span>11</span></div>
                  </div>
              </div>
              <div class="sp-line">
                  <div class="mast-label">【部位】</div>
                  <div id="bodypart" class="table-line">
                      <div class="left mast-sub-label">【○】</div>
                      <div class="left mast-sub-table">
                        <ul class="list-group list-OK" id="mast-sub-table-OK">
                          <li class="list-group-item">aaaaaa</li>
                          <li class="list-group-item">bbbbbb</li>
                          <li class="list-group-item">cccccccccc</li>
                          <li class="list-group-item">dddddddd</li>
                        </ul>
                      </div>
                      <div class="left mast-sub-label">【×】</div>
                      <div class="left mast-sub-table">
                        <ul class="list-group list-NG" id="mast-sub-table-NG">
                          <li class="list-group-item">ddddd</li>
                          <li class="list-group-item">fffffff</li>
                          <li class="list-group-item"></li>
                          <li class="list-group-item"></li>
                        </ul>
                      </div>
                      <div class="left mast-sub-label">【他】</div>
                      <div class="left mast-sub-table">
                        <ul class="list-group list-Other" id="mast-sub-table-Other">
                          <li class="list-group-item">ddddd</li>
                          <li class="list-group-item">fffffff</li>
                          <li class="list-group-item"></li>
                          <li class="list-group-item"></li>
                        </ul>
                      </div>
                  </div>
              </div>
              <div class="sp-line">
                  <div class="mast-label">【病院】</div>
                  <div id="hospital"  class="table-line">
                      <div class="left mast-sub-label">【○】</div>
                      <div class="left mast-sub-table">
                        <ul class="list-group list-OK" id="mast-sub-table2-OK">
                          <li class="list-group-item">ddddd</li>
                          <li class="list-group-item">fffffff</li>
                          <li class="list-group-item"></li>
                          <li class="list-group-item"></li>
                        </ul>
                      </div>
                      <div class="left mast-sub-label">【×】</div>
                      <div class="left mast-sub-table">
                        <ul class="list-group list-NG" id="mast-sub-table2-NG">
                          <li class="list-group-item">ddddd</li>
                          <li class="list-group-item">fffffff</li>
                          <li class="list-group-item"></li>
                          <li class="list-group-item"></li>
                        </ul>
                      </div>
                      <div class="left mast-sub-label">【他】</div>
                      <div class="left mast-sub-table">
                        <ul class="list-group list-Other" id="mast-sub-table2-Other">
                          <li class="list-group-item">ddddd</li>
                          <li class="list-group-item">fffffff</li>
                          <li class="list-group-item"></li>
                          <li class="list-group-item"></li>
                        </ul>
                      </div>
                  </div>
              </div>
              <div class="multi-line">
                  <div class="mast-label">【その他Drの属性】</div>
                  <div class="left multi-data-view" id="other"><span><pre>baerbaerb</pre></span></div>
              </div>
              <div class="multi-line">
                  <div class="mast-label">【個人メモ】</div>
                  <div class="left multi-data-view" id="memo"><span><pre>baerbaerb</pre></span></div>
              </div>
          </div>
          <div class="modal-footer">
            <div class="left" id="update"><span>baerbaerb</span></div>
            <button class="btn btn-default" type="button" data-dismiss="modal" id="edit-today">当日予定編集</button>
            <button class="btn" type="button" data-dismiss="modal" id="edit-doc">固定シフト・データ編集</button>
          </div>
        </div>
      </div>
    </div>

    <div class="modal" id="modal-doc-edit">
      <div class="modal-dialog series-dialog">
        <div class="modal-content">
          <div class="modal-header series-header">
            <button class="close" aria-hidden="true" type="button" data-dismiss="modal">&times;</button>
            <h4 class="modal-title" id="mast-title-e">
                <div id="color-pic" class="left">

                </div>
                <span class="left"></span>
            </h4>
          </div>
          <div class="modal-body">
              <div class="left fld-left">
                  <div class="one-line">
                      <div class="half-line left">
                          <div class="left mast-label">【略称】</div>
                          <div class="left mast-data" id="name-r-e"><input /></div>
                      </div>
                      <div class="half-line left">
                          <div class="left mast-label">【ステータス】</div>
                          <div class="left mast-data" id="count-e"><input /></div>
                      </div>
                  </div>
                  <div class="one-line">
                      <div class="half-line left">
                          <div class="left mast-label">【名前】</div>
                          <div class="left mast-data" id="name-e"><input /></div>
                      </div>
                      <div class="half-line left">
                          <div class="left mast-label">【スピード】</div>
                          <div class="left mast-data" id="speed-e"><input /></div>
                      </div>
                  </div>
                  <div class="one-line">
                      <div class="half-line left">
                          <div class="left mast-label">【ふりがな】</div>
                          <div class="left mast-data" id="name-k-e"><input /></div>
                      </div>
                      <div class="half-line left">
                          <div class="left mast-label">【専門分野】</div>
                          <div class="left mast-data" id="main-e">
                              <input id="mainsel" class="listblock" list="main-list"/>
                                <datalist id="main-list">
                                        <option>頭部</option>
                                        <option>乳腺</option>
                                        <option>女性骨盤</option>
                                        <option>核医学</option>
                                        <option>腹部</option>
	                            </datalist>
                          </div>
                      </div>
                  </div>
                  <div class="one-line">
                      <div class="left mast-label">【コメント】</div>
                      <div class="left mast-data-l" id="comment-e"><input /></div>
                  </div>
                  <div class="sp-line">
                      <div class="mast-label">【部位】</div>
                      <div id="bodypart-e" class="table-line">
                          <div class="left mast-sub-label">【○】</div>
                          <div class="left mast-sub-table">
                            <ul class="list-group list-OK" id="mast-sub-table-OK-e">
                            </ul>
                          </div>
                          <div class="left mast-sub-label">【×】</div>
                          <div class="left mast-sub-table">
                            <ul class="list-group list-NG" id="mast-sub-table-NG-e">
                            </ul>
                          </div>
                          <div class="left mast-sub-label">【他】</div>
                          <div class="left mast-sub-table">
                            <ul class="list-group list-Other" id="mast-sub-table-Other-e">
                            </ul>
                          </div>
                      </div>
                  </div>
                  <div class="one-line">
                      <div class="left btn-area">
                          <div class="left edit-button enable panel-collapse collapse">
                              <input class="left" type="button" id="OK-add" value="↑" />
                              <input class="left" type="button" id="OK-del"value="↓" />
                          </div>
                          <div class="left edit-button enable panel-collapse collapse">
                              <input class="left" type="button" id="NG-add"value="↑" />
                              <input class="left" type="button" id="NG-del"value="↓" />
                          </div>
                          <div class="left edit-button enable panel-collapse collapse">
                              <input class="left" type="button" id="Other-add"value="↑" />
                              <input class="left" type="button" id="Other-del"value="↓" />
                          </div>
                      </div>
                      <div class="left">
                        <button type="button" class="btn btn-primary btn-sm" data-toggle="collapse" data-target="div.enable">☆</button>
                      </div>
                  </div>
                  <div class="sp-line">
                    <div id="body-mast" class="enable panel-collapse collapse">
                        <div class="left fld-sub">
                            <p><input  type="checkbox" value="0">もりした頭部テンプレ</p>
                            <p><input  type="checkbox" value="1">心臓ＣＴ</p>
                            <p><input  type="checkbox" value="2">乳腺ＭＲ</p>
                        </div>
                        <div class="left fld-sub">
                            <p><input  type="checkbox" value="3">MG</p>
                            <p><input  type="checkbox" value="4">AI</p>
                            <p><input  type="checkbox" value="5">ハイメディックPT</p>
                        </div>
                    </div>
                  </div>
                  <div class="sp-line">
                      <div class="mast-label">【病院】</div>
                      <div id="hospital-e"  class="table-line">
                          <div class="left mast-sub-label">【○】</div>
                          <div class="left mast-sub-table">
                            <ul class="list-group list-OK" id="mast-sub-table2-OK-e">
                              <li class="list-group-item edit-table"><input class="form-control last-data"/></li>
                            </ul>
                          </div>
                          <div class="left mast-sub-label">【×】</div>
                          <div class="left mast-sub-table">
                            <ul class="list-group list-NG" id="mast-sub-table2-NG-e">
                              <li class="list-group-item edit-table"><input class="form-control last-data"/></li>
                            </ul>
                          </div>
                          <div class="left mast-sub-label">【他】</div>
                          <div class="left mast-sub-table">
                            <ul class="list-group list-Other" id="mast-sub-table2-Other-e">
                              <li class="list-group-item edit-table"><input class="form-control last-data"/></li>
                            </ul>
                          </div>
                      </div>
                  </div>
                  <div class="multi-line">
                      <div class="mast-label">【その他Drの属性】</div>
                      <div class="left multi-data" id="other-e">
                          <textarea></textarea>
                      </div>
                  </div>
              </div>
              <div class="left fld-right">
                  <div class="multi-line">
                      <div class="mast-label">【個人メモ】</div>
                      <div class="left multi-data" id="memo-e">
                          <textarea></textarea>
                      </div>
                  </div>
<%--                  <div class="multi-line">
                      <table id="date-mst-table" class="left table">
                          <thead>
                              <tr>
                                  <th class="data-ss"></th>
                                  <th class="data-m">開始日</th>
                                  <th class="data-s"></th>
                                  <th class="data-m">終了日</th>
                              </tr>
                          </thead>
                          <tbody>
                              <tr id="date-1">
                                  <td class="data-ss"></td>
                                  <td class="data-m">
                                      <input id="s-date" class="datepicker" />
                                  </td>
                                  <td class="data-ss"></td>
                                  <td class="data-m">
                                      <input id="e-date" class="datepicker"/>
                                  </td>
                              </tr>
                          </tbody>
                      </table>
                  </div>--%>
                  <div class="sp2-line">
                    <div class="container">
                        <ul class="nav nav-tabs" id="tabs" role="tablist">
                          <li>
                           <a href="#tab1" role="tab" data-toggle="tab">
                             表示&nbsp;
                           </a>
                         </li>
                          <li>
                           <a href="#tab2" role="tab" data-toggle="tab">
                             削除&nbsp;
                           </a>
                         </li>
                        </ul>
                        <div class="tab-content">
                          <div class="tab-pane active" id="tab1">
                            <table id="date-table" class="left table">
<%--                                  <thead>
                                      <tr>
                                          <th class="data-s"></th>
                                          <th class="data-ss">内</th>
                                          <th class="data-ss">外</th>
                                          <th class="data-s"></th>
                                          <th class="data-ss"></th>
                                          <th class="data-s"></th>
                                          <th class="data-s">コメント</th>
                                          <th class="data-ss">県尼</th>
                                          <th class="data-ss">ﾍﾙﾌﾟ</th>
                                      </tr>
                                  </thead>--%>
                                  <tbody>
                              <%--        <tr>
                                          <td class="data-s">
                                              <select id="1-s-day" class="weekday">
                                                  <option value=""></option>
                                                   <option value="1">月</option>
                                                   <option value="2">火</option>
                                                   <option value="3">水</option>
                                                   <option value="4">木</option>
                                                   <option value="5">金</option>
                                                   <option value="6">土</option>
                                                   <option value="0">日</option>
                                              </select>
                                          </td>
                                          <td class="data-ss"><input type="checkbox" id="1-in"/></td>
                                          <td class="data-ss"><input type="checkbox" id="1-out"/></td>
                                          <td class="data-s">
                                              <select class="1-s-time">
                                                  <option value=""></option>
                                                    <option value="0">- 9:00</option>
                                                    <option value="1">9:00</option>
                                                    <option value="2">9:15</option>
                                                    <option value="3">9:30</option>
                                                    <option value="4">9:45</option>
                                                    <option value="5">10:00</option>
                                                    <option value="6">10:15</option>
                                                    <option value="7">10:30</option>
                                                    <option value="8">10:45</option>
                                                    <option value="9">11:00</option>
                                                    <option value="10">11:15</option>
                                                    <option value="11">11:30</option>
                                                    <option value="12">11:45</option>
                                                    <option value="13">12:00</option>
                                                    <option value="14">12:15</option>
                                                    <option value="15">12:30</option>
                                                    <option value="16">12:45</option>
                                                    <option value="17">13:00</option>
                                                    <option value="18">13:15</option>
                                                    <option value="19">13:30</option>
                                                    <option value="20">13:45</option>
                                                    <option value="21">14:00</option>
                                                    <option value="22">14:15</option>
                                                    <option value="23">14:30</option>
                                                    <option value="24">14:45</option>
                                                    <option value="25">15:00</option>
                                                    <option value="26">15:15</option>
                                                    <option value="27">15:30</option>
                                                    <option value="28">15:45</option>
                                                    <option value="29">16:00</option>
                                                    <option value="30">16:15</option>
                                                    <option value="31">16:30</option>
                                                    <option value="32">16:45</option>
                                                    <option value="33">17:00</option>
                                                    <option value="34">17:15</option>
                                                    <option value="35">17:30</option>
                                                    <option value="36">17:45</option>
                                                    <option value="37">18:00</option>
                                                    <option value="38">18:15</option>
                                                    <option value="39">18:30</option>
                                                    <option value="40">18:45</option>
                                                    <option value="41">19:00</option>
                                                    <option value="42">19:15</option>
                                                    <option value="43">19:30</option>
                                                    <option value="44">19:45</option>
                                                    <option value="45">20:00</option>
                                                    <option value="46">20:00 -</option>
                                              </select>
                                          </td>
                                          <td class="data-ss">～</td>
                                          <td class="data-s">
                                              <select id="1-e-time">
                                                  <option value=""></option>
                                                    <option value="0">- 9:00</option>
                                                    <option value="1">9:00</option>
                                                    <option value="2">9:15</option>
                                                    <option value="3">9:30</option>
                                                    <option value="4">9:45</option>
                                                    <option value="5">10:00</option>
                                                    <option value="6">10:15</option>
                                                    <option value="7">10:30</option>
                                                    <option value="8">10:45</option>
                                                    <option value="9">11:00</option>
                                                    <option value="10">11:15</option>
                                                    <option value="11">11:30</option>
                                                    <option value="12">11:45</option>
                                                    <option value="13">12:00</option>
                                                    <option value="14">12:15</option>
                                                    <option value="15">12:30</option>
                                                    <option value="16">12:45</option>
                                                    <option value="17">13:00</option>
                                                    <option value="18">13:15</option>
                                                    <option value="19">13:30</option>
                                                    <option value="20">13:45</option>
                                                    <option value="21">14:00</option>
                                                    <option value="22">14:15</option>
                                                    <option value="23">14:30</option>
                                                    <option value="24">14:45</option>
                                                    <option value="25">15:00</option>
                                                    <option value="26">15:15</option>
                                                    <option value="27">15:30</option>
                                                    <option value="28">15:45</option>
                                                    <option value="29">16:00</option>
                                                    <option value="30">16:15</option>
                                                    <option value="31">16:30</option>
                                                    <option value="32">16:45</option>
                                                    <option value="33">17:00</option>
                                                    <option value="34">17:15</option>
                                                    <option value="35">17:30</option>
                                                    <option value="36">17:45</option>
                                                    <option value="37">18:00</option>
                                                    <option value="38">18:15</option>
                                                    <option value="39">18:30</option>
                                                    <option value="40">18:45</option>
                                                    <option value="41">19:00</option>
                                                    <option value="42">19:15</option>
                                                    <option value="43">19:30</option>
                                                    <option value="44">19:45</option>
                                                    <option value="45">20:00</option>
                                                    <option value="46">20:00 -</option>
                                              </select>
                                          </td>
                                          <td class="data-s"><input id="1-count"/></td>
                                          <td class="data-ss"><input type="checkbox" id="1-other"/></td>
                                          <td class="data-ss"><input type="checkbox" id="1-help"/></td>
                                      </tr>--%>
                                  </tbody>
                              </table>
                          </div>
　                        <div class="tab-pane active" id="tab2">
  　                          <table id="date-table-del" class="left table">
                                 <tbody></tbody>
                              </table>
                          </div>
                        </div>
                    </div>
                  </div>
              </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-default" type="button" data-dismiss="modal">キャンセル</button>
            <button class="btn" type="button" id="set_change_doc">保  存</button>
          </div>
        </div>
      </div>
    </div>
    <div id="loader-bg">
      <div id="loader">
        <img src="img/common/img-loading.gif" width="80" height="80" alt="Now Loading..." />
        <p>Now Loading...</p>
      </div>
    </div>

</body>


</html>
