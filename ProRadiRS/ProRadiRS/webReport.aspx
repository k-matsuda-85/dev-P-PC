<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="webReport.aspx.cs" Inherits="ProRadiRS.webReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>ProRadRS Lite レポート</title>
    <!-- アイコン -->
    <link href="./img/logo.ico?20150706b", rel="shortcut icon" />
    <!-- スタイルシート -->
    <link href="./CSS/common.css?20150706b" rel="Stylesheet" type="text/css" />
    <link href="./Scripts/Core/css/jquery-ui.css?20150706b" rel="Stylesheet" type="text/css" />
    <link href="./CSS/Add/bootstrap.css" rel="Stylesheet" type="text/css" />
    <link href="./CSS/Add/bootstrap-theme.css" rel="Stylesheet" type="text/css" />
    <link href="./CSS/Report.css?20180312a" rel="Stylesheet" type="text/css" />
    <!-- スクリプト -->
<%--    <script src="./Scripts/Core/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="./Scripts/Core/jquery-1.7.2.min.js" type="text/javascript"></script>--%>
    <script src="./Scripts/Core/jquery-1.12.4.min.js" type="text/javascript"></script>
    <script src="./Scripts/js/Add/bootstrap.js" type="text/javascript"></script>
    <script src="./Scripts/js/common.js?201805172" type="text/javascript"></script>
    <script src="./Scripts/js/report.js?20180606" type="text/javascript"></script>
    <script src="./Scripts/Core/jquery.ui.core.min.js?20150706b" type="text/javascript"></script>
    <script src="./Scripts/Core/jquery-ui.js?20150706b" type="text/javascript"></script>
</head>
<body onresize="resize()"; onunload = "unload()">
    <div id="ReportConfig"></div>
    
    <div id = "Report-body" >
      <table id="ReportTop-Info" border="0" cellspacing="0" cellpadding="0" style="width:100%;">
      
      <thead id= "ReportRow1"></thead> 
      <tbody>
        <tr id="ReportTop">
                <td colspan="2">
                    <div id="ReportTop-Info-Left">
                        <div id="ReportTop-Info-Left-Title">
                         <div id="Report-ID"></div>
                         <div id="Report-Info"></div>
                         <div id="Report-StudyInfo1"></div>
                         <div id="Report-StudyInfo2"></div>
                        </div>
                    </div>
                 </td>
                 <td></td>
                 <td rowspan="2" colspan="3" style="width:55%;height:150px;padding-bottom:5px">
                 <div ></div>
                    <div id="ReportTop-Info-Right"></div>
                 </td>
                 <td rowspan ="2"/>
        </tr>  
        
        
        <tr id= "ReportRow2"/> 
        <tr id= "ReportRow3"/> 
        <tr id= "ReportRow4"/> 
        <tr id= "ReportRow5"/> 
        <tr id= "ReportRow6"/>       
        </tbody>
    </table>
    </div>
    <div id="ReportBottom">
      <input type="button" class="btnReadCancel-off" id="btnReadCancel"  tabindex="0" onclick="ReadCancel()" onmousedown="ReadCancelMouseDonw()" onmouseup="ReadCancelMouseUp()" onmouseout="ReadCancelMouseUp()" onmouseover="ReadCancelMouseOver()"/>
      <input type="button" class="btnReadStart-off" id="btnReadStart"  tabindex="1" onclick="ReadStart()" onmousedown="ReadStartMouseDonw()" onmouseup="ReadStartMouseUp()" onmouseout="ReadStartMouseUp()" onmouseover="ReadStartMouseOver()" style ="display:none"/>

      <input type="button" class="btnSentence-off" id="btnSentence" tabindex="2" data-toggle="modal" onmousedown="SentenceMouseDonw()" onmouseup="SentenceMouseUp()" onmouseout="SentenceMouseUp()" onmouseover="SentenceMouseOver()" data-target="#modal" data-backdrop="false" />

      <input type="button" class="btnExamOrder-off" id="btnExamOrder" tabindex="3" onclick="ExamOrder()" onmousedown="ExamOrderMouseDonw()" onmouseup="ExamOrderMouseUp()" onmouseout="ExamOrderMouseUp()" onmouseover="ExamOrderMouseOver()" />
      <input type="button" class="btnViewer-off" id="btnViewer" tabindex="4" onclick="Viewer()" onmousedown="ViewerMouseDonw()" onmouseup="ViewerMouseUp()" onmouseout="ViewerMouseUp()" onmouseover="ViewerMouseOver()" title="New Viewer"/>
      <input type="button" class="btnViewer2-off" id="btnViewer2" tabindex="5" onclick="Viewer2()" onmousedown="ViewerMouseDonw2()" onmouseup="ViewerMouseUp2()" onmouseout="ViewerMouseUp2()" onmouseover="ViewerMouseOver2()"title="Sub Viewer"/>
      <input type="button" class="btnImage-off" id="btnImage" tabindex="6" onclick="Image()" onmousedown="ImageMouseDonw()" onmouseup="ImageMouseUp()" onmouseout="ImageMouseUp()" onmouseover="ImageMouseOver()"/>
     <!-- <input type="button" id="btnclipboard"  tabindex="11" onclick="clipboard()" value="クリップボード取込" style ="display:none" />-->
     <!-- <input type="button" id="btnclipboardCancel"  tabindex="11" onclick="clipboardCancel()" value="取込キャンセル" style ="display:none" />-->

      <input type="button" class="btnImageDelete-off" id="btnImageDelete" tabindex="7" onclick="Delete()" onmousedown="ImageDeleteMouseDonw()" onmouseup="ImageDeleteMouseUp()" onmouseout="ImageDeleteMouseUp()" onmouseover="ImageDeleteMouseOver()"/>

      <%--<input type="button" class="btnHistory-off" id="btnHistory" tabindex="6" onclick="History()" onmousedown="HistoryMouseDonw()" onmouseup="HistoryMouseUp()" onmouseout="HistoryMouseUp()" onmouseover="HistoryMouseOver()"/>--%>&nbsp;

      <input type="button" class="btnEditHist-off" id="btnEditHist" tabindex="8" onclick="ChangeHistory2()" onmousedown="EditHistMouseDonw()" onmouseup="EditHistMouseUp()" onmouseout="EditHistMouseUp()" onmouseover="EditHistMouseOver()"/>&nbsp;

      <input type="button" class="btnSave-off" id="btnSave" tabindex="9" onclick="Save()" onmousedown="SaveMouseDonw()" onmouseup="SaveMouseUp()" onmouseout="SaveMouseUp()" onmouseover="SaveMouseOver()"/>&nbsp;
      <input type="button" class="btnTempSave-off" id="btnTempSave"  tabindex="10" onclick="TempSave()" onmousedown="TempSaveMouseDonw()" onmouseup="TempSaveMouseUp()" onmouseout="TempSaveMouseUp()" onmouseover="TempSaveMouseOver()"/>
      <input type="button" class="btnReturnList-off" id="btnReturnList"  tabindex="11" onclick="Return()" onmousedown="ReturnListMouseDonw()" onmouseup="ReturnListMouseUp()" onmouseout="ReturnListMouseUp()" onmouseover="ReturnListMouseOver()"/>
        <input type="button" class="btnViewer_Reserve-off" id="btnViewer_Reserve" tabindex="12" onclick="Viewer_Reserve()" onmousedown="ViewerMouseDonw()" onmouseup="ViewerMouseUp()" onmouseout="ViewerMouseUp()" onmouseover="ViewerMouseOver()"/>
       <br />
<%--    <button onclick="OpinionCopyFinding()">「画像所見」コピー</button>
    <button onclick="OpinionCopyDiagnosing()">「診断」コピー</button>
    <button onclick="OpinionCopyAll()">「両方」コピー</button>
    <button onclick="OpinionCopyDate()">日付</button>
    <button onclick="TestPage()">テスト</button>--%>

   </div>

<%--    <div id="SentenceDlg" style="display: none" title="定型文">

        <select id="cmbUserList">
        <option value="ALL">--------------------</option>
        </select>
        &nbsp;
        <button onclick="SentenceSearch()">絞込み</button>

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

    </div>--%>


<%--    <div id="ChangeHistoryDlg" style="display: none" title="変更履歴">
        <div id="HistryList">
            <table id="tblHistryList" border="0" cellspacing="0" cellpadding="0" style="width:98%;">
                <thead><tr><th>&nbsp;</th></tr></thead>
                <tbody></tbody>
            </table>
        </div>
    </div>--%>
    <div class="modal" id="modal">
      <div class="modal-dialog series-dialog"  id="modal-child">
        <div class="modal-content">
          <div class="modal-header series-header">
            <button class="close" aria-hidden="true" type="button" data-dismiss="modal">&times;</button>
            <h4 class="modal-title">定型文</h4>
          </div>
          <div class="modal-body">
              <div class="action-line">
                <select id="cmbUserList" class="left action-area">
                    <option value="ALL">--------------------</option>
                </select>
                <button type="button"  class="left action-area"onclick="SentenceSearch()">絞込み</button>
                <button type="button"  class="right action-area" onclick="SentenceEdit()">新規登録</button>
              </div>
            <table id="tblSentenceList" class="table table-hover table-headerfixed">
                <thead>
                    <tr>
                        <th class="dhead-ss"></th>
                        <th class="dhead-sp">タイトル</th>
                        <th class="dhead-s">登録医師</th>
                        <th class="dhead-ss"></th>
                        <th class="dhead-dm"></th>
                    </tr>
                </thead>
                <tbody></tbody>
            </table>
          </div>
          <div class="modal-footer">
          </div>
        </div>
      </div>
    </div>

    <div class="modal modal-ex" id="modal-hist">
      <div class="modal-dialog series-dialog">
        <div class="modal-content">
          <div class="modal-header series-header">
            <button class="close" aria-hidden="true" type="button" data-dismiss="modal">&times;</button>
            <h4 class="modal-title">変更履歴</h4>
          </div>
          <div class="modal-body">
            <div id="HistryList">
                <table id="tblHistryList" border="0" cellspacing="0" cellpadding="0" style="width:100%;">
                    <thead><tr><th>&nbsp;</th></tr></thead>
                    <tbody></tbody>
                </table>
            </div>
          </div>
          <div class="modal-footer">
          </div>
        </div>
      </div>
    </div>

    <div class="modal" id="modal-sent-edit">
      <div class="modal-dialog series-dialog">
        <div class="modal-content">
          <div class="modal-header series-header">
            <button class="close" aria-hidden="true" type="button" data-dismiss="modal">&times;</button>
            <h4 class="modal-title">定型文 更新</h4>
          </div>
          <div class="modal-body">
            <div class="form-group">
                <div class="edit-line">
                    <label class="left label-title">公開条件</label>
                    <div class="radio-inline">
                        <input type="radio" value="20" name="sent-public" id="sent-open" />
                        <label for="sent-open" class="inner-label">公開</label>
                    </div>
                    <div class="radio-inline">
                        <input type="radio" value="0" name="sent-public" id="sent-close" />
                        <label for="sent-close" class="inner-label">非公開</label>
                    </div>
                </div>
                <div class="edit-line">
                    <label class="left label-title">タイトル</label>
                    <input id="sent-title" />
                </div>
                <div class="edit-line">
                    <label class="left label-title">画像所見</label>
                </div>
                <div class="edit-line-sub">
                    <textarea id="sent-finding" rows="5" cols="1"></textarea>
                </div>
                <div class="edit-line">
                    <label class="left label-title">診断結論</label>
                </div>
                <div class="edit-line-sub">
                    <textarea id="sent-diagnosing" rows="5" cols="1"></textarea>
                </div>
                <div class="edit-line">
                    <div class="checkbox-inline check-fld">
                        <input type="checkbox" id="sent-cp-find"  checked="checked"/>
                        <label for="sent-cp-find" class="inner-label">画像所見</label>
                    </div>
                    <div class="checkbox-inline check-fld">
                        <input type="checkbox" id="sent-cp-diag" checked="checked" />
                        <label for="sent-cp-diag" class="inner-label">診断結論</label>
                    </div>
                    <div class="checkbox-inline">
                        <input type="button" class="edit-btn" id="sent-cp" value="入力内容をコピー"/>
                    </div>
                </div>
            </div>
          </div>
          <div class="modal-footer">
            <input type="button" class="left edit-btn" id="sent-delete" value="削    除"/>
            <input type="button" class="right edit-btn" id="sent-cancel" value="キャンセル"/>
            <input type="button" class="right edit-btn" id="sent-edit" value="登    録"/>
          </div>
        </div>
      </div>
    </div>
</body>
</html>

