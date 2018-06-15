<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="webLogin.aspx.cs" Inherits="ProRadiRS.webLogin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>ProRadRS Lite ログイン</title>
    <!-- アイコン -->
    <link href="./img/logo.ico?20141110", rel="shortcut icon" />
    <!-- スタイルシート -->
    <link href="./CSS/Login.css?20141110" rel="Stylesheet" type="text/css" />
    <!-- スクリプト -->
    <script src="./Scripts/Core/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="./Scripts/Core/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="./Scripts/js/common.js?20141110" type="text/javascript"></script>
    <script src="./Scripts/js/login.js?20141110" type="text/javascript"></script>
</head>
<body  style="top: 0px; left: 0px">
  <div id="LoginInputArea">
    <input id="LoginID" maxlength="16" onkeypress="LoginID_Enter()" value=""/>
    <input id="LoginPW" maxlength="16" type="password" onkeypress="LoginPW_Enter()" value="" />
    
    <input class="LoginButton-off" id="LoginButton" type="button" onclick="Login()" onmousedown="LoginMouseDonw()" onmouseup="LoginMouseUp()" onmouseout="LoginMouseUp()" onmouseover="LoginMouseOver()"/>
    <input class="ClearButton-off" id="ClearButton" type="button" onclick="Clear()" onmousedown="ClearMouseDonw()" onmouseup="ClearMouseUp()" onmouseout="ClearMouseUp()" onmouseover="ClearMouseOver()"/>
  </div>
</body>
</html>
