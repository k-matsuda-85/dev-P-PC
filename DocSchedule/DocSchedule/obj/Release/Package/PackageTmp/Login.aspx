<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="DocSchedule.Login" %>

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

<link href="css/login/login.css" rel="stylesheet"/>

<script src="script/common/jquery-1.12.4.min.js"></script>
<script src="script/common/bootstrap.js"></script>

<script src="script/common/util.js"></script>
<script src="script/login/login.js"></script>
<title>ログイン フォーム</title>


</head>
<body>
    <div class='wrapper'>
      <div class='row'>
        <div class='col-lg-12'>
          <div class='brand text-center'>
            <h1>
              <div class='logo-icon'></div>
              DocSchedule
            </h1>
          </div>
        </div>
      </div>
      <div class='row'>
        <div class='col-lg-12'>
          <form>
            <fieldset class='text-center'>
              <legend></legend>
              <div class='form-group'>
                <select class="form-control" id="user" >
                    <option value=""></option>
                </select>
              </div>
              <div class='text-center'>
                <div id="message" class='alert-danger'>
                </div>
                <input type="button" class="btn btn-default" id="btn_login" value="Sign in"/>
              </div>
            </fieldset>
          </form>
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
