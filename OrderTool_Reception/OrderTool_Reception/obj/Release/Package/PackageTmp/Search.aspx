<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="OrderTool_Reception.Search" %>

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

    <link href="css/common/bootstrap-datepicker3.min.css" rel="stylesheet"/>
    <link href="css/common/bootstrap-datepicker3.standalone.min.css" rel="stylesheet"/>

    <link href="css/common/modality.css" rel="stylesheet"/>

    <link href="css/search/search.css" rel="stylesheet"/>

    <script src="script/common/jquery-1.12.4.min.js"></script>
    <script src="script/common/bootstrap.js"></script>

    <script src="script/common/util.js"></script>

    <script src="script/common/bootstrap-datepicker.min.js"></script>
    <script src="script/common/bootstrap-datepicker.ja.min.js"></script>

    <script src="script/common/modality.js"></script>

    <script src="script/order/jquery-ui.js"></script>

    <script src="script/search/search.js"></script>
    <title>検査一覧</title>
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
            <li><a href="#" id="reload">更新</a></li>
            <li><a href="#" id="newdata">新規オーダー</a></li>
            <li><a href="#" id="inputCSV">取込</a></li>
          </ul>
          <ul class="nav navbar-nav navbar-right">
            <li><a href="#" id="reload-first">入力一覧に戻る</a></li>
            <li><a href="#" id="view">検索表示</a></li>
          </ul>
        </div>
      </div>
    </nav>
    <form class="form-horizontal" id="main-form">
        <fieldset>
            <div class="form-group">
                <label id="status-label" for="status" class="control-label col-sm-1">出力</label>
                <div class="col-sm-2">
                    <select class="form-control" id="status">
                        <option value=""></option>
                        <option value="4">未 + 一 + 登</option>
                        <option value="-1">未入力</option>
                        <option value="0">一時</option>
                        <option value="1">登録</option>
                        <option value="2">出力済</option>
                        <option value="3">削除</option>
                    </select>
                </div>
                <label id="hosp-label" for="sex" class="control-label col-sm-1 error-label">事業所</label>
                <div class="col-sm-6">
                    <select class="form-control" id="hosp" >
                        <option value=""></option>
                    </select>
                </div>
                <div class="col-sm-2">
                </div>
            </div>
            <div class="form-group">
                <label id="patid-label" for="patid" class="control-label col-sm-1 error-label">患者ID</label>
                <div class="col-sm-5">
                    <input type="text" class="form-control" id="patid" placeholder="患者ID"/>
                </div>
                <label id="modality-label" for="modality" class="control-label col-sm-1 error-label">モダリティ</label>
                <div class="col-sm-3" id="mod-area">
                    <input type="text" class="form-control listblock" id="modality" placeholder=""/>
                    <ul id="modality-list" class="dropdown-menu listblock">
                        <li>
                            <a data-key="modality">CT</a>
                            <a data-key="modality">MR</a>
                            <a data-key="modality">CR</a>
                            <a data-key="modality">MG</a>
                            <a data-key="modality">RF</a>
                        </li>
	                </ul>
                </div>
                <div class="col-sm-2">
                    <input type="button" class="btn " id="clear" value="クリア"/>
                </div>
            </div>
            <div class="form-group">
                <label id="studydate-label" for="studydate" class="control-label col-sm-1 error-label">検査日</label>
                <div class="col-sm-4">
                    <input type="text" class="form-control datepicker" id="date_f" placeholder="yyyyMMdd"/>
                </div>
                <label id="studytime-label" for="studytime" class="control-label col-sm-1 error-label">～</label>
                <div class="col-sm-4">
                    <input type="text" class="form-control datepicker" id="date_t" placeholder="yyyyMMdd"/>
                </div>
                <div class="col-sm-2">
                    <input type="button" class="btn " id="search" value="検索"/>
                </div>
            </div>
        </fieldset>
    </form>

    <table id="study_table" class="table table-hover">
        <thead>
            <tr>
                <th class="dhead-ss"></th>
                <th class="dhead-ss"></th>
                <th class="dhead-ss"></th>
                <th class="dhead-ss">出力</th>
                <th class="dhead-m">事業所</th>
                <th class="dhead-m">患者ID</th>
                <th>患者名</th>
                <th>患者名ｶﾅ</th>
                <th class="dhead-m">モダリティ</th>
                <th class="dhead-m">検査日</th>
                <th>検査部位</th>
            </tr>
        </thead>
    </table>
    <div id="loader-bg">
      <div id="loader">
        <img src="image/common/img-loading.gif" width="80" height="80" alt="Now Loading..." />
        <p>Now Loading...</p>
      </div>
    </div>

</body>
</html>
