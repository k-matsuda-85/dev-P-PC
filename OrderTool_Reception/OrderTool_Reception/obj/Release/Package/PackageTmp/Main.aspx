<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="OrderTool_Reception.Main" %>

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
    <link href="css/main/main.css" rel="stylesheet"/>

    <script src="script/common/jquery-1.12.4.min.js"></script>
    <script src="script/common/bootstrap.js"></script>

    <script src="script/common/util.js"></script>
    <script src="script/main/main.js"></script>
    <title>メンテナンス ツール</title>
</head>
<body>
  <ul class="nav nav-tabs">
    <li class="nav-item">
      <a href="#tab1" class="nav-link navbar-default active" data-toggle="tab">事業所ごとのメモ・注意事項</a>
    </li>
  </ul>
  <div class="tab-content">
    <div id="tab1" class="tab-pane active">
        <div class="col-lg-12">
            <form class="form-horizontal" id="main-form">
              <fieldset>
                <div class="col-sm-12" style="height:10px">
                </div>
                <div class="form-group">
                    <label id="hosp-label" for="sex" class="control-label col-sm-2">事業所</label>
                    <div class="col-sm-9">
                        <select class="form-control" id="hosp" >
                            <option value=""></option>
                        </select>
                    </div>
                    <div class="col-sm-1">
                    </div>
                </div>
                <div class="form-group">
                    <label id="alert-label" for="sex" class="control-label col-sm-2">施設ごとの注意事項</label>
                    <div class="col-sm-9">
                        <textarea rows="1" class="form-control" id="alert" style="height: 600px;" placeholder=""></textarea>
                    </div>
                    <div class="col-sm-1">
                        <input type="button" class="btn btn-block" id="alert-setting"tabindex="-1" value="更新"/>
                    </div>
                </div>
                <div class="form-group">
                    <label id="memo-label" for="sex" class="control-label col-sm-2">個人メモ</label>
                    <div class="col-sm-9">
                        <textarea rows="1" class="form-control" id="memo" style="height: 150px;" placeholder=""></textarea>
                    </div>
                    <div class="col-sm-1">
                        <input type="button" class="btn btn-block" id="memo-setting"tabindex="-1" value="更新"/>
                    </div>
                </div>
              </fieldset>
            </form>
        </div>
    </div>
  </div>
</body>
</html>
