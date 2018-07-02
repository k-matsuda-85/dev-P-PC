<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="webSearch.aspx.cs" Inherits="ProRadiRS.webSearch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>ProRadRS Lite 検索</title>
    <!-- アイコン -->
    <link href="./img/logo.ico?20150513", rel="shortcut icon" />
    <!-- スタイルシート -->
    <link href="./CSS/Search.css?20151124" rel="Stylesheet" type="text/css" />
    <link href="./CSS/common.css?20151124" rel="Stylesheet" type="text/css" />
    <link href="./Scripts/Core/css/jquery.ui.all.css?20151124" rel="stylesheet" type="text/css" />
    <!-- スクリプト -->
    <script src="./Scripts/Core/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="./Scripts/Core/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="./Scripts/js/common.js?20180621" type="text/javascript"></script>
    <script src="./Scripts/js/search.js?20160215" type="text/javascript"></script>
    <script src="./Scripts/Core/jquery.ui.core.min.js?20151124" type="text/javascript"></script>
    <script src="./Scripts/Core/jquery.ui.widget.min.js?20151124" type="text/javascript"></script>
    <script src="./Scripts/Core/jquery.ui.datepicker.min.js?20151124" type="text/javascript"></script>
    
</head>
<body onresize="resize();">
  <div id="ReportConfig"></div>
  
  <div id="SearchTop">
    <div id="SearchMenu">
      <div id="SearchCount">
        <div id="lblSearchCount-Title">検索結果：</div>
        <div id="lblSearchCount-View"><span id="lblSearchCount"></span></div>
      </div>

        <div id="searchMain-position">
            <input id="btnSearch" class="btnSearch-off" type="button" onclick="Search()" onmousedown="SearchMouseDonw(btnSearch)" onmouseup="SearchMouseUp(btnSearch)" onmouseout="SearchMouseUp(btnSearch)" onmouseover="SearchMouseOver(btnSearch)" />
            <!--<input id="btnSearchEM" class="btnSearchEM-off" type="button" onclick="SearchEM()" onmousedown="SearchEMMouseDonw(btnSearchEM)" onmouseup="SearchEMMouseUp(btnSearchEM)" onmouseout="SearchEMMouseUp(btnSearchEM)" onmouseover="SearchEMMouseOver(btnSearchEM)" />-->
            <input id="btnSearchREAD" class="btnSearchREAD-off" type="button" onclick="SearchREAD()" onmousedown="SearchREADMouseDonw(btnSearchREAD)" onmouseup="SearchREADMouseUp(btnSearchREAD)" onmouseout="SearchREADMouseUp(btnSearchREAD)" onmouseover="SearchREADMouseOver(btnSearchREAD)" />
        </div>
        
        <div id="search-position">
            <label id = "PatientID-position" class = "searchlabel">患者ID</label>
            <input id ="PatientID" type ="text" value=""/>

            <label id = "searchDay-position" class = "searchlabel" style ="display:none">検査日付</label>
            <input id ="searchDay"style ="display:none" />
      
            <label id = "modality-position" class = "searchlabel"style ="display:none">Mod</label>
            <select id="modality"style ="display:none"></select>

            <input  id="btnSearchSelect" class="btnSearchSelect-off" type="button" onclick="SearchSelect()" onmousedown="SearchSelectMouseDonw(btnSearchSelect)" onmouseup="SearchSelectMouseUp(btnSearchSelect)" onmouseout="SearchSelectMouseUp(btnSearchSelect)" onmouseover="SearchSelectMouseOver(btnSearchSelect)" />
            <input  id="btnReset" class="btnReset-off" type="button" onclick="Reset()" onmousedown="ResetMouseDonw(btnReset)" onmouseup="ResetMouseUp(btnReset)" onmouseout="ResetMouseUp(btnReset)" onmouseover="ResetMouseOver(btnReset)" />
            <input  id="btnHelp" class="btnHelp-off" type="button" onclick="Help()" onmousedown="HelpMouseDonw(btnHelp)" onmouseup="HelpMouseUp(btnHelp)" onmouseout="HelpMouseUp(btnHelp)" onmouseover="HelpMouseOver(btnHelp)" />
            <label id = "cheacklabel-position"><input  id="viewercheckBox"  type="checkbox" />予備Viewer</label>
        </div>
        
      <div id="SearchUser">
        <select id="cmbUserList"></select>
        <input id="btnUserSearch" class="btnUserSearch-off" type="button" onclick="UserSearch()" onmousedown="UserSearchMouseDonw(btnUserSearch)" onmouseup="UserSearchMouseUp(btnUserSearch)" onmouseout="UserSearchMouseUp(btnUserSearch)" onmouseover="UserSearchMouseOver(btnUserSearch)" />
        <input id="btnUserSearchREAD" class="btnUserSearchREAD-off" type="button" onclick="UserSearchREAD()" onmousedown="UserSearchREADMouseDonw(btnUserSearchREAD)" onmouseup="UserSearchREADMouseUp(btnUserSearchREAD)" onmouseout="UserSearchREADMouseUp(btnUserSearchREAD)" onmouseover="UserSearchREADMouseOver(btnUserSearchREAD)" />
      </div>
    </div>
  </div>

  <div id="SearchCenter">
    <div id="reportList">
      <table id="tblList" border="0" cellspacing="0" cellpadding="0">
        <thead><tr class="RowHeader">
          <th class="ReadButton head">&nbsp;</th>
          <th class="ViewerButton head">&nbsp;</th>
          <!--<th class="ImageRequestButton head">&nbsp;</th>-->
          <th class="Status head"></th>
          <th class="Priority head"></th>
          <th class="iProMedID head">ID</th>
          <th class="PatientAge head">年齢</th>
          <th class="PatientSex head">性別</th>
          <th class="StudyDateTime head">検査日</th>
          <th class="Modality head">検査</th>
          <!--<th class="ReportReserve8 head">P現</th>-->
          <th class="StudyBodyPart head">部位</th>
          <th class="Diagnosing head">診断･結論</th>
          <th class="Comment3 head">受付専用</th>
          <th class="RadiologistName head">読影医師</th>
          
          <th class="RequestedPhysicianName head">依頼先</th>
          <!--<th class="AuthorizationPhysicianName head">読影者</th>-->
            <th class="Comment2 head">連絡事項</th>
          <th class="ImageCheckButton head">&nbsp;</th>
        </tr></thead>
        <tbody></tbody>
      </table>
    </div>
  </div>

  <div id="SearchBottom"></div>
</body>
</html>
