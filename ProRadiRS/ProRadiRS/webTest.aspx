<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="webTest.aspx.cs" Inherits="ProRadiRS.webTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>ProRadRS Lite テスト</title>

    <!-- スタイルシート -->
    <link href="./Scripts/Core/css/jquery-ui.css" rel="Stylesheet" />

    <script src="./Scripts/Core/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="./Scripts/Core/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="./Scripts/js/common.js" type="text/javascript"></script>
    <script src="./Scripts/js/test.js" type="text/javascript"></script>
    <script src="./Scripts/Core/jquery-ui.js" type="text/javascript"></script>

</head>

<body>
    <div id="ReportConfig"></div>

    <button onclick="Sentence2()">定型文</button>
    &nbsp;
    <button onclick="ChangeHistory2()">変更履歴</button>
    <br />

    <textarea id="TxtFinding"rows="15" cols="50">画像所見</textarea>
    <br />
    <textarea id="TxtDiagnosing"rows="10" cols="50">診断</textarea>

    <div id="SentenceDlg" style="display: none" title="定型文">

        <select id="cmbUserList">
            <option value="ALL">--------------------</option>
            <option value="サンプル1">サンプル1</option>
        </select>
        &nbsp;
        <button onclick="Narrow2()">絞込み</button>

        <div id="SentenceList">
          <table id="tblSentenceList" border="1" cellspacing="2" cellpadding="2" style="width:100%;">
            <thead><tr class="SentenceRowHeader">
              <th class="SetButton head" style="width:50px;"></th>
              <th class="SentenceTitle head" style="width:500px;">タイトル</th>
              <th class="SentenceName head" style="width:150px;">登録医師</th>
            </tr></thead>
            <tbody></tbody>
          </table>
        </div>

    </div>

    <div id="ChangeHistoryDlg" style="display: none" title="変更履歴">
        <div id="HistryList">
            <table id="tblHistryList" border="0" cellspacing="0" cellpadding="0" style="width:98%;">
                <thead><tr><th>&nbsp;</th></tr></thead>
                <tbody></tbody>
            </table>
        </div>
    </div>


</body>
</html>
