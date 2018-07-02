<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="OrderTool.Search" %>

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

    <link href="css/search/search.css" rel="stylesheet"/>

    <script src="script/common/jquery-1.12.4.min.js"></script>
    <script src="script/common/bootstrap.js"></script>

    <script src="script/common/util.js"></script>
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
          <ul class="nav navbar-nav navbar-right">
            <li><a href="#" id="update">更新</a></li>
          </ul>
        </div>
      </div>
    </nav>
    <table id="study_table" class="table table-hover">
        <thead>
            <tr>
                <th class="dhead-ss"></th>
                <th class="dhead-ss"></th>
                <th class="dhead-ss"></th>
                <th class="dhead-ss">出力</th>
                <th class="dhead-m">患者ID</th>
                <th>患者名</th>
                <th class="dhead-m">モダリティ</th>
                <th class="dhead-m">検査日</th>
                <th>検査部位</th>
                <th>依頼内容</th>
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
