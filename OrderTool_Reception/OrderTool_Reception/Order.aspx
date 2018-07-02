<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Order.aspx.cs" Inherits="OrderTool_Reception.Order" %>

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


    <link href="css/common/modality.css" rel="stylesheet"/>

    <link href="css/order/order.css" rel="stylesheet"/>

    <script src="script/common/jquery-1.12.4.min.js"></script>
    <script src="script/common/bootstrap.js"></script>


    <script src="script/common/bootstrap-datepicker.min.js"></script>
    <script src="script/common/bootstrap-datepicker.min.js"></script>

    <script src="script/common/util.js"></script>

    <script src="script/common/bootstrap-datepicker.min.js"></script>
    <script src="script/common/bootstrap-datepicker.ja.min.js"></script>


    <script src="script/common/modality.js"></script>

    <script src="script/common/common.js"></script>

    <script src="script/order/jquery.autoKana.js"></script>
    <script src="script/order/jquery-ui.js"></script>
    
    <script src="script/order/handlebars.js" type="text/javascript"></script>
    <script src="script/order/typeahead.bundle.js" type="text/javascript"></script>

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
            <a id="tolist" href="#" class="btn">一覧表示へ</a>
            <a id="new" href="#" class="btn">新規登録</a>
            <a id="copy" href="#" class="btn">複　製</a>
        </div>
      </div>
    </nav>

    <div class="col-lg-6">
        <form class="form-horizontal" id="main-form">
          <fieldset>
            <div class="form-group">
                <label id="status-label" for="status" class="control-label col-sm-2">出力</label>
                <div class="col-sm-2">
                    <input type="text" class="form-control" id="status" disabled="disabled" placeholder="未" data-value="-1" value="未入力"/>
                </div>
                <label id="orderno-label" for="orderno" class="control-label col-sm-2">ｵｰﾀﾞｰ番号</label>
                <div class="col-sm-6">
                    <input type="text" class="form-control" id="orderno" disabled="disabled" placeholder="オーダー番号"/>
                </div>
            </div>
            <div class="form-group">
                <label id="hosp-label" for="sex" class="control-label col-sm-2 error-label">事業所</label>
                <div class="col-sm-10">
                    <select class="form-control" id="hosp" >
                        <option value=""></option>
                    </select>
                </div>
            </div>
            <div class="form-group">
                <label id="patid-label" for="patid" class="control-label col-sm-2">患者ID</label>
                <div class="col-sm-8">
                    <input type="text" class="form-control" id="patid" placeholder="患者ID"/>
                </div>
                <div id="ck_emergency" class="checkbox col-sm-2">
                    <label class="alert-data">
                       <input type="checkbox" class="editpast" id="isemergency"/> 緊急 
                    </label>
                </div>
            </div>
            <div class="form-group">
                <label id="patname-label" for="patname" class="control-label col-sm-2">患者名</label>
                <div class="col-sm-10">
                    <input type="text" class="form-control" id="patname" placeholder="患者名（漢字）"/>
                </div>
            </div>
            <div class="form-group">
                <label id="patname_h-label" for="patname" class="control-label col-sm-2 ">患者名(半ｶﾅ)</label>
                <div class="col-sm-10">
                    <input type="text" class="form-control" id="patname_h" placeholder="患者名（半角ｶﾅ）"/>
                </div>
            </div>
            <div class="form-group">
                <label id="sex-label" for="sex" class="control-label col-sm-2 ">性別</label>
                <div class="col-sm-2">
                    <select class="form-control" id="sex" >
                        <option>男</option>
                        <option>女</option>
                        <option>不明</option>
                    </select>
                </div>
                <label id="birthday-label" for="birthday" class="control-label col-sm-2 ">生年月日</label>
                <div class="col-sm-3">
                    <input type="text" class="form-control" id="birthday" placeholder="yyyyMMdd"/>
                </div>
                <label id="age-label" for="age" class="control-label col-sm-1">年齢</label>
                <div class="col-sm-2">
                    <input type="text" class="form-control" id="age" placeholder=""/>
                </div>
            </div>
            <div class="form-group">
                <label id="isvisit-label" for="isvisit" class="control-label col-sm-2">入院/外来</label>
                <div class="col-sp-2">
                    <input type="text" class="form-control listblock" id="isvisit"  placeholder=""/>
                    <ul id="isvisit-list" class="dropdown-menu listblock">
	                </ul>
                </div>
                <div class="col-sp-1">
                    <input type="button" class="btn btn-block setting" id="isvisit-setting" tabindex="-1" value="..."/>
                </div>
                <label id="department-label" for="department" class="control-label col-sp-3">依頼科</label>
                <div class="col-sp-4">
                    <input type="text" class="form-control listblock" id="department" placeholder=""/>
                    <ul id="department-list" class="dropdown-menu listblock">
	                </ul>
                </div>
                <div class="col-sp-1">
                    <input type="button" class="btn btn-block setting" id="department-setting"tabindex="-1" value="..."/>
                </div>
                <label id="doctor-label" for="doctor" class="control-label col-sp-3">依頼医</label>
                <div class="col-sp-4">
                    <input type="text" class="form-control listblock" id="doctor"  list="doctor-list" placeholder=""/>
                    <ul id="doctor-list" class="dropdown-menu listblock">
	                </ul>
                </div>
                <div class="col-sp-1">
                    <input type="button" class="btn btn-block setting" id="doctor-setting" tabindex="-1" value="..."/>
                </div>

            </div>
            <div class="form-group">
                <label id="own-label" for="studydate" class="control-label col-sm-2 ">契約施設</label>
                <div id="ck_isfax_own" class="checkbox col-sm-2">
                    <label>
                       <input type="checkbox"id="isfax_own"/> FAX 
                    </label>
                </div>
                <div id="ck_ismail_own" class="checkbox col-sm-2">
                    <label>
                       <input type="checkbox"id="ismail_own"/> TEL 
                    </label>
                </div>
                <label id="dummy-label" for="studydate" class="control-label col-sm-6"></label>
            </div>
            <div class="form-group">
                <label id="intro-label" for="studydate" class="control-label col-sm-2 ">紹介元施設</label>
                <div id="ck_isfax" class="checkbox col-sm-2">
                    <label>
                       <input type="checkbox"id="isfax"/> FAX 
                    </label>
                </div>
                <div id="ck_ismail" class="checkbox col-sm-2">
                    <label>
                       <input type="checkbox"id="ismail"/> メール 
                    </label>
                </div>
                <div class="col-sm-3">
                    <input type="text" class="form-control listblock" id="ihosp" placeholder="送付先施設"/>
                    <ul id="ihosp-list" class="dropdown-menu listblock">
	                </ul>
                </div>
                <div class="col-sm-3">
                    <input type="text" class="form-control" id="ihosp_number" placeholder="FAX番号"/>
                </div>
            </div>
            <div class="form-group">
                <label id="studydate-label" for="studydate" class="control-label col-sm-2 ">検査日</label>
                <div class="col-sm-4">
                    <input type="text" class="form-control datepicker editpast" id="studydate" placeholder="yyyyMMdd"/>
                </div>
                <label id="studytime-label" for="studytime" class="control-label col-sm-2 ">検査時刻</label>
                <div class="col-sm-4">
                    <input type="text" class="form-control" id="studytime" placeholder="HHmmss"/>
                </div>
            </div>
            <div class="form-group">
                <label id="modality-label" for="modality" class="control-label col-sm-2 ">モダリティ</label>
                <div class="col-sm-3" id="mod-area">
                    <input type="text" class="form-control listblock editpast" id="modality" placeholder=""/>
                    <ul id="modality-list" class="dropdown-menu listblock">
	                </ul>
                </div>
                <div class="col-sm-1">
                    <input type="button" class="btn btn-block setting" id="modality-setting" tabindex="-1" value="..."/>
                </div>
                <label id="studytype-label" for="studytype" class="control-label col-sm-2">単純/造影</label>
                <div class="col-sm-3">
                    <input type="text" class="form-control listblock" id="studytype" placeholder=""/>
                    <ul id="studytype-list" class="dropdown-menu listblock">
	                </ul>
                </div>
                <div class="col-sm-1">
                    <input type="button" class="btn btn-block setting" id="studytype-setting" tabindex="-1" value="..."/>
                </div>
            </div>
            <div class="form-group">
                <label id="bodypart-label" for="bodypart" class="control-label col-sm-2 ">検査部位</label>
                <div class="col-sm-9">
                    <textarea class="form-control area editpast" rows="3" id="bodypart" style="margin: 0px -1.84375px 0px 0px; height: 50px;"></textarea>
                    <ul id="bodypart-list" class="dropdown-menu area">
	                </ul>
                </div>
                <div class="col-sm-1">
                    <input type="button" class="btn btn-block setting" id="bodypart-setting" tabindex="-1" value="..."/>
                </div>
            </div>
            <div class="form-group">
                <label id="comment-label" for="comment" class="control-label col-sm-2 ">依頼内容</label>
                <div class="col-sm-9">
                    <textarea class="form-control area" rows="6" id="comment" style="margin: 0px -1.84375px 0px 0px; height: 110px;"></textarea>
                    <ul id="comment-list" class="dropdown-menu area">
	                </ul>
                </div>
                <div class="col-sm-1">
                    <input type="button" class="btn btn-block setting" id="comment-setting" tabindex="-1" value="..."/>
                </div>
            </div>
            <div class="form-group">
                <label id="contact-label" for="contact" class="control-label col-sm-2">連絡事項</label>
                <div class="col-sm-9">
                    <textarea class="form-control area" rows="2" id="contact" style="margin: 0px -1.84375px 0px 0px; height: 76px;"></textarea>
                    <ul id="contact-list" class="dropdown-menu area">
	                </ul>
                </div>
                <div class="col-sm-1">
                    <input type="button" class="btn btn-block setting" id="contact-setting" tabindex="-1" value="..."/>
                </div>
            </div>
            <div class="form-group">
                <div class="col-sm-12">
                    <div  class="footer-space" ></div>
                </div>
            </div>
           </fieldset>
        </form>
    </div>
    <div class="col-lg-6">
        <form class="form-horizontal" id="sub-form">
          <fieldset>
            <div class="form-group">
                <div class="col-sm-11">
                    <textarea rows="1" class="form-control" id="memo" style="margin: 0px -1.84375px 0px 0px; height: 76px;" placeholder="個人メモ"></textarea>
                </div>
                <div class="col-sm-1">
                    <input type="button" class="btn btn-block" id="memo-setting"tabindex="-1" value="更新"/>
                </div>
            </div>
            <div class="form-group">
                <div class="col-sm-11">
                    <textarea rows="1" class="form-control" id="alert" style="margin: 0px -1.84375px 0px 0px; height: 152px;" placeholder="施設ごとの注意事項"></textarea>
                </div>
                <div class="col-sm-1">
                    <input type="button" class="btn btn-block" id="alert-setting"tabindex="-1" value="更新"/>
                </div>
            </div>
            <div class="form-group">
                <div class="col-sm-11">
                    <textarea class="form-control area" rows="1" id="recept" placeholder="受付専用"  style=""></textarea>
                    <ul id="recept-list" class="dropdown-menu area">
	                </ul>
                </div>
                <div class="col-sm-1">
                    <input type="button" class="btn btn-block setting" id="recept-setting" tabindex="-1" value="..."/>
                </div>
            </div>
            <div class="form-group">
                <div class="col-sm-10">
                    <img  src="" class="img-thumbnail left" id="thumbnail"/>
                </div>
                <div class="col-sm-2">
                    <div class="left" id="thumb_action">
                        <div>
                            <input type="button" class="btn " id="delImg" value="削除"/>
                        </div>
                        <div>
                            <input type="button" class="btn " id="orgImg_del" value="原本解除"/>
                        </div>
                        <div>
                            <input type="button" class="btn " id="orgImg" value="原本表示"/>
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="col-sm-12" id="pastreport">
                    <div id="study_area">
                        <table id="study_table" class="table table-hover">
                            <thead>
                                <tr>
                                    <th>出ｽﾃ</th>
                                    <th>検査日</th>
                                    <th>Mod</th>
                                    <th>検査部位</th>
                                    <th>緊急</th>
                                    <th></th>
                                    <th></th>
                                    <th></th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="col-sm-12">
                    <div  class="footer-space" ></div>
                </div>
            </div>
           </fieldset>
        </form>
    </div>

    <div class="modal" id="modal">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <button class="close" aria-hidden="true" type="button" data-dismiss="modal">&times;</button>
            <h4 id="modal-title" class="modal-title"></h4>
          </div>
          <div class="modal-body">
            <table id="setting_table" class="table table-hover">
                <thead>
                    <tr>
                        <th>No</th>
                        <th>名称</th>
                        <th></th>
                    </tr>
                </thead>
            </table>
          </div>
          <div class="form-group">
              <div class="col-sm-12">
                  <div class="footer-space" ></div>
              </div>
          </div>
          <div class="form-group">
                <div class="col-sm-10">
                    <input type="text" class="form-control" id="add-val" placeholder="新規登録名称"/>
                </div>
                <div class="col-sm-2">
                    <input type="button" class="btn btn-block" id="add-val-set" value="登録"/>
                </div>
          </div>
          <div class="form-group">
              <div class="col-sm-12">
                  <div  class="footer-space" ></div>
              </div>
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
        <a id="delete" href="#" title="レポートから削除します。" class="btn">全削除</a>
        <a id="delete_preorder" title="依頼枠を削除します。" href="#" class="btn">振り分けツールから削除</a>
        <a id="clear" href="#" class="btn">クリア</a>
        <a id="order" href="#" class="btn">オーダー出力</a>
        <a id="send" href="#" class="btn">登　録</a>
        <a id="save" href="#" class="btn">一時保存</a>
      </div>
    </footer>

</body>

</html>

