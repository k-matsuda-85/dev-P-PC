<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Order.aspx.cs" Inherits="OrderTool.Order" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Language" content="ja"/>
    <meta http-equiv="Content-type" content="text/html" />
    <meta http-equiv="Content-Style-Type" content="text/css" />
    <meta http-equiv="Content-Script-Type" content="text/javascript" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="Cache-Control" content="no-cache" />
    <meta http-equiv="Expires" content="Thu, 01 Dec 1994 16:00:00 GMT" />
    <meta name="viewport" content="initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />

    <link href="css/common/bootstrap-theme.min.css" rel="stylesheet"/>
    <link href="css/common/bootstrap.css" rel="stylesheet"/>

    <link href="css/common/typeahead.css" rel="stylesheet"/>

    <link href="css/common/bootstrap-datepicker3.min.css" rel="stylesheet"/>
    <link href="css/common/bootstrap-datepicker3.standalone.min.css" rel="stylesheet"/>


    <link href="css/order/order.css" rel="stylesheet"/>

    <script src="script/common/jquery-1.12.4.min.js"></script>
    <script src="script/common/bootstrap.js"></script>
    <script src="script/common/typeahead.bundle.js"></script>

    <script src="script/common/util.js"></script>

    <script src="script/common/bootstrap-datepicker.min.js"></script>
    <script src="script/common/bootstrap-datepicker.ja.min.js"></script>

    <script src="script/order/jquery.autoKana.js"></script>
    <script src="script/order/order.js"></script>               
    <title>入力フォーム</title>
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
        </div>
        <div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
          <ul class="nav navbar-nav">
            <li ><a href="#" id="toList" class="btn">一覧へ</a></li>
          </ul>
        </div>
      </div>
    </nav>

    <form class="form-horizontal" id="main-form">
      <fieldset>
        <div class="form-group">
            <label id="patid-label" for="patid" class="control-label col-sm-2 error-label">患者ID</label>
            <div class="col-sm-8">
                <input type="text" class="form-control" id="patid" placeholder="患者ID"/>
            </div>
            <div id="ck_emergency" class="checkbox col-sm-2">
                <label class="alert-data">
                   <input type="checkbox"id="isemergency"/> 緊急 
                </label>
            </div>
        </div>
        <div class="form-group">
            <label id="patname-label" for="patname" class="control-label col-sm-2 error-label">患者名</label>
            <div class="col-sm-10">
                <input type="text" class="form-control" id="patname" placeholder="患者名（漢字）"/>
            </div>
        </div>
        <div class="form-group">
            <label id="patname_h-label" for="patname" class="control-label col-sm-2 error-label">患者名(半ｶﾅ)</label>
            <div class="col-sm-10">
                <input type="text" class="form-control" id="patname_h" placeholder="患者名（半角ｶﾅ）"/>
            </div>
        </div>
        <div class="form-group">
            <label id="sex-label" for="sex" class="control-label col-sm-2 error-label">性別</label>
            <div class="col-sm-2">
                <select class="form-control" id="sex" >
                    <option>男</option>
                    <option>女</option>
                    <option>不明</option>
                </select>
            </div>
            <label id="birthday-label" for="birthday" class="control-label col-sm-2 error-label">生年月日</label>
            <div class="col-sm-3">
                <input type="text" class="form-control datepicker" id="birthday" placeholder="yyyyMMdd"/>
            </div>
            <label id="age-label" for="age" class="control-label col-sm-1">年齢</label>
            <div class="col-sm-2">
                <input type="text" class="form-control" id="age" placeholder=""/>
            </div>
        </div>
        <div class="form-group">
            <label id="studydate-label" for="studydate" class="control-label col-sm-2 error-label">検査日</label>
            <div class="col-sm-4">
                <input type="text" class="form-control datepicker" id="studydate" placeholder="yyyyMMdd"/>
            </div>
            <label id="studytime-label" for="studytime" class="control-label col-sm-2 error-label">検査時刻</label>
            <div class="col-sm-4">
                <input type="text" class="form-control" id="studytime" placeholder="HHmmss"/>
            </div>
        </div>
        <div class="form-group">
            <label id="modality-label" for="modality" class="control-label col-sm-2 error-label">モダリティ</label>
            <div class="col-sm-4" id="mod-area">
                <select class="form-control" id="modality" >
                </select>
            </div>
            <label id="studytype-label" for="studytype" class="control-label col-sm-2">単純/造影</label>
            <div class="col-sm-4">
                <input type="text" class="form-control" id="studytype" placeholder=""/>
            </div>
        </div>
        <div class="form-group">
            <label id="imgcnt-label" for="imgcnt" class="control-label col-sm-2 error-label">画像枚数</label>
            <div class="col-sm-4">
                <input type="text" class="form-control" id="imgcnt" placeholder=""/>
            </div>
            <label for="pastmsg" class="control-label col-sm-2">過去画像</label>
            <div class="col-sm-4">
                <input type="button" class="btn left " id="past" value="選択"/>
                <span class="help-block left" id="pastmsg"></span>
            </div>
        </div>
        <div class="form-group">
            <label id="bodypart-label" for="bodypart" class="control-label col-sm-2 error-label">検査部位</label>
            <div class="col-sm-10">
                <textarea class="form-control" rows="3" id="bodypart" style="margin: 0px -1.84375px 0px 0px; height: 76px;"></textarea>
            </div>
        </div>
        <div class="form-group">
            <label id="comment-label" for="comment" class="control-label col-sm-2 error-label">依頼内容</label>
            <div class="col-sm-10">
                <textarea class="form-control" rows="6" id="comment" style="margin: 0px -1.84375px 0px 0px; height: 152px;"></textarea>
            </div>
        </div>
        <div class="form-group">
<%--            <label for="istemp" class="control-label col-sm-2"></label>--%>
            <div class="checkbox col-sm-12">
                <label>
                    <input type="checkbox" id="istemp"/> 紹介状等の別送あり
                </label>
            </div>
        </div>
        <div class="form-group hidden">
            <%--<label for="istemp" class="control-label col-sm-2"></label>--%>
            <div class="checkbox col-sm-4">
                <label>
                   <input type="checkbox"id="ismail"/> 紹介元施設への所見返却 
                </label>
            </div>
            <div class="col-sm-5">
                <input type="text" class="form-control" id="mail_2" disabled="disabled" placeholder="施設名"/>
            </div>
            <div class="col-sm-3">
                <input type="text" class="form-control" id="mail_1" disabled="disabled" placeholder="FAX番号"/>
            </div>
        </div>
       </fieldset>
    </form>


    <div class="modal" id="modal">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <button class="close" aria-hidden="true" type="button" data-dismiss="modal">&times;</button>
            <h4 class="modal-title">過去画像選択</h4>
          </div>
          <div class="modal-body">
            <table id="study_table" class="table table-hover">
                <thead>
                    <tr>
                        <th class="dhead-ss"></th>
                        <th>モダリティ</th>
                        <th>検査日</th>
                        <th>画像枚数</th>
                    </tr>
                </thead>
            </table>
          </div>
          <div class="modal-footer">
            <button class="btn btn-default" type="button" data-dismiss="modal">キャンセル</button>
            <button class="btn" type="button" id="set_change">登　録</button>
          </div>
        </div>
      </div>
    </div>

    <div id="loader-bg">
      <div id="loader">
        <img src="image/common/img-loading.gif" width="80" height="80" alt="Now Loading..." />
        <p>Now Loading...</p>
      </div>
    </div>

    <footer class="footer">
      <div class="container">
        <a id="cancel" href="#" class="btn">キャンセル</a>
        <a id="clear" href="#" class="btn">クリア</a>
        <a id="send" href="#" class="btn">登　録</a>
        <a id="edit-order" href="#" class="btn">変更依頼</a>
      </div>
    </footer>

</body>
</html>
