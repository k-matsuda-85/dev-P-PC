<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PastOrder.aspx.cs" Inherits="OrderTool_Reception.PaseOrder" %>

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

    <script src="script/common/common.js"></script>

    <script src="script/order/jquery-ui.js"></script>
    <script src="script/order/past.js"></script>     
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
        </div>
        <div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
        </div>
      </div>
    </nav>
        <form class="form-horizontal" id="main-form">
          <fieldset>
            <div class="form-group">
                <label id="status-label" for="status" class="control-label col-sm-2">出力</label>
                <div class="col-sm-2">
                    <input type="text" class="form-control" id="status" readonly="readonly" placeholder="未" data-value="-1" value="未入力"/>
                </div>
                <label id="orderno-label" for="orderno" class="control-label col-sm-2">ｵｰﾀﾞｰ番号</label>
                <div class="col-sm-6">
                    <input type="text" class="form-control" id="orderno" readonly="readonly" placeholder="オーダー番号"/>
                </div>
            </div>
            <div class="form-group">
                <label id="hosp-label" for="sex" class="control-label col-sm-2 error-label">事業所</label>
                <div class="col-sm-10">
                    <select class="form-control" disabled="disabled" id="hosp" >
                        <option value=""></option>
                    </select>
                </div>
            </div>
            <div class="form-group">
                <label id="patid-label" for="patid" class="control-label col-sm-2">患者ID</label>
                <div class="col-sm-8">
                    <input type="text" class="form-control" id="patid" readonly="readonly" placeholder="患者ID"/>
                </div>
                <div id="ck_emergency" class="checkbox col-sm-2">
                    <label class="alert-data">
                       <input type="checkbox"id="isemergency" disabled="disabled"/> 緊急 
                    </label>
                </div>
            </div>
            <div class="form-group">
                <label id="patname-label" for="patname" class="control-label col-sm-2">患者名</label>
                <div class="col-sm-10">
                    <input type="text" class="form-control" readonly="readonly" id="patname" placeholder="患者名（漢字）"/>
                </div>
            </div>
            <div class="form-group">
                <label id="patname_h-label" for="patname" class="control-label col-sm-2 ">患者名(半ｶﾅ)</label>
                <div class="col-sm-10">
                    <input type="text" class="form-control" readonly="readonly" id="patname_h" placeholder="患者名（半角ｶﾅ）"/>
                </div>
            </div>
            <div class="form-group">
                <label id="sex-label" for="sex"  class="control-label col-sm-2 ">性別</label>
                <div class="col-sm-2">
                    <select class="form-control" disabled="disabled" id="sex" >
                        <option>男</option>
                        <option>女</option>
                        <option>不明</option>
                    </select>
                </div>
                <label id="birthday-label" for="birthday" class="control-label col-sm-2 ">生年月日</label>
                <div class="col-sm-3">
                    <input type="text" class="form-control" readonly="readonly" id="birthday" placeholder="yyyyMMdd"/>
                </div>
                <label id="age-label" for="age" class="control-label col-sm-1">年齢</label>
                <div class="col-sm-2">
                    <input type="text" class="form-control" readonly="readonly" id="age" placeholder=""/>
                </div>
            </div>
            <div class="form-group">
                <label id="isvisit-label" for="isvisit" class="control-label col-sm-2">入院/外来</label>
                <div class="col-sm-2">
                    <input type="text" class="form-control listblock" readonly="readonly" id="isvisit"  placeholder=""/>
                </div>
                <label id="department-label" for="department"  class="control-label col-sm-2">依頼科</label>
                <div class="col-sm-2">
                    <input type="text" class="form-control listblock" readonly="readonly" id="department" placeholder=""/>
                </div>
                <label id="doctor-label" for="doctor" class="control-label col-sm-2">依頼医</label>
                <div class="col-sm-2">
                    <input type="text" class="form-control listblock" readonly="readonly" id="doctor"  list="doctor-list" placeholder=""/>
                </div>
            </div>
            <div class="form-group">
                <label id="own-label" for="studydate" class="control-label col-sm-2 ">契約施設</label>
                <div id="ck_isfax_own" class="checkbox col-sm-2">
                    <label>
                       <input type="checkbox"id="isfax_own"disabled="disabled"/> FAX 
                    </label>
                </div>
                <div id="ck_ismail_own" class="checkbox col-sm-2">
                    <label>
                       <input type="checkbox"id="ismail_own"disabled="disabled"/> TEL 
                    </label>
                </div>
                <label id="dummy-label" for="studydate" class="control-label col-sm-6"></label>
            </div>
            <div class="form-group">
                <label id="intro-label" for="studydate" class="control-label col-sm-2 ">紹介元施設</label>
                <div id="ck_isfax" class="checkbox col-sm-2">
                    <label>
                       <input type="checkbox"id="isfax"disabled="disabled"/> FAX 
                    </label>
                </div>
                <div id="ck_ismail" class="checkbox col-sm-2">
                    <label>
                       <input type="checkbox"id="ismail"disabled="disabled"/> メール 
                    </label>
                </div>
                <div class="col-sm-3">
                    <input type="text" class="form-control listblock"readonly="readonly" id="ihosp" placeholder="送付先施設"/>
                </div>
                <div class="col-sm-3">
                    <input type="text" class="form-control" id="ihosp_number"readonly="readonly" placeholder="FAX番号"/>
                </div>
            </div>
            <div class="form-group">
                <label id="studydate-label" for="studydate"  class="control-label col-sm-2 ">検査日</label>
                <div class="col-sm-4">
                    <input type="text" class="form-control datepicker" readonly="readonly" id="studydate" placeholder="yyyyMMdd"/>
                </div>
                <label id="studytime-label" for="studytime"  class="control-label col-sm-2 ">検査時刻</label>
                <div class="col-sm-4">
                    <input type="text" class="form-control" readonly="readonly" id="studytime" placeholder="HHmmss"/>
                </div>
            </div>
            <div class="form-group">
                <label id="modality-label" for="modality" class="control-label col-sm-2 ">モダリティ</label>
                <div class="col-sm-4" id="mod-area">
                    <input type="text" class="form-control listblock" readonly="readonly" id="modality" placeholder=""/>
                </div>
                <label id="studytype-label" for="studytype"  class="control-label col-sm-2">単純/造影</label>
                <div class="col-sm-4">
                    <input type="text" class="form-control listblock" readonly="readonly" id="studytype" placeholder=""/>
                </div>
            </div>
            <div class="form-group">
                <label id="bodypart-label" for="bodypart" class="control-label col-sm-2 ">検査部位</label>
                <div class="col-sm-10">
                    <textarea class="form-control area" rows="3"  readonly="readonly" id="bodypart" style="margin: 0px -1.84375px 0px 0px; height: 50px;"></textarea>
                </div>
            </div>
            <div class="form-group">
                <label id="comment-label" for="comment" class="control-label col-sm-2 ">依頼内容</label>
                <div class="col-sm-10">
                    <textarea class="form-control area" rows="6" readonly="readonly" id="comment" style="margin: 0px -1.84375px 0px 0px; height: 140px;"></textarea>
                </div>
            </div>
            <div class="form-group">
                <label id="contact-label" for="contact" class="control-label col-sm-2">連絡事項</label>
                <div class="col-sm-10">
                    <textarea class="form-control area" rows="2" readonly="readonly" id="contact" style="margin: 0px -1.84375px 0px 0px; height: 76px;"></textarea>
                </div>
            </div>
            <div class="form-group">
                <label id="recept-label" for="recept" class="control-label col-sm-2">受付専用</label>
                <div class="col-sm-10">
                    <textarea class="form-control area" rows="1" readonly="readonly" id="recept"  style=""></textarea>
                </div>
            </div>
            <div class="form-group">
                <div class="col-sm-12">
                    <div  class="footer-space" ></div>
                </div>
            </div>
           </fieldset>
        </form>
    <footer class="footer">
      <div class="container">
        <a id="send" href="#" class="btn">依頼票</a>
      </div>
    </footer>
</body>
</html>
