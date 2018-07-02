/*************************************************************************
The MIT License (MIT)

Copyright (c) 2014 COCHMA

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

[change log]
001 2014/03/13	COCHMA Create new file. version 1.0
*************************************************************************/
(function ($) {

    var node;
    var $moveNode; // 現在選択されているノード
    var options;
    var params = [];

    // draggable options
    var draggableGridX;
    var draggableGridY;
    var resizableGridX;
    var resizableGridY;
    var resizableMaxH;
    var resizableMinH;
    var resizableCss;

    // その他必要なプロパティ
    var startOffsetX;	// ノードの初期位置 これと移動幅を基準に移動幅を計算する
    var startOffsetY;	// ノードの初期位置 ノードの高さに関してはstopの引数uiから取得
    var startTime;		// スケジュール表の開始時間
    var endTime;		// スケジュール表の終了時間
    var widthTime; 		// 1幅あたりの時間(秒)


    var methods = {
        init: function () {
            // set options.
            draggableGridX = options['draggableGridX'];
            draggableGridY = options['draggableGridY'];
            resizableGridX = options['resizableGridX'];
            resizableGridY = options['resizableGridY'];
            resizableMaxH = options['resizableMaxH'];
            resizableMinH = options['resizableMinH']
            startOffsetX = options['startOffsetX'];
            startOffsetY = options['startOffsetY'];
            startTime = options['startTime'];
            endTime = options['endTime'];
            widthTime = options['widthTime'];


            // 共通で使用する物
            // 0時を基準とし、テーブルの開始時間を計算
            var tableStartTime = calcStringTime(startTime);
            var tableEndTime = calcStringTime(endTime);

            // move node.
            node.draggable({
                grid: [draggableGridX, draggableGridY],
                containment: "parent",
                // 要素の移動が終った後の処理
                stop: function (event, ui) {

                    // 他のクラスからもアクセス可能にするために変数に入れる
                    $moveNode = $(this);
                    console.log($moveNode[0].clientWidth);
                    // ノードの初期位置を取得する
                    var originalLeft = ui.originalPosition['left'];
                    var positionLeft = ui.position['left'];
                    var offsetLeft = $moveNode[0].offsetLeft;

                    /*************************************************************************
			 		 * 初期の開始時間を取得する
			 		 *************************************************************************/
                    // 要素に埋め込まれた時間の取得を試みる
                    var attr_ost = $moveNode.attr('attr_ost');
                    var attr_osf = $moveNode.attr('attr_osf');
                    var originalSTime;
                    var originalSTimeF;
                    if (typeof attr_ost == "undefined") {
                        originalSTime = tableStartTime + ((originalLeft - startOffsetX) / draggableGridX * widthTime);
                        originalSTimeF = formatTime(originalSTime);
                    } else {
                        originalSTime = Number(attr_ost);
                        originalSTimeF = attr_osf;
                    }

                    /*************************************************************************
			 		 * 変更後の開始時間を取得する
			 		 *************************************************************************/
                    var afterSTime = tableStartTime + ((offsetLeft - startOffsetX) / draggableGridX * widthTime);
                    // 要素に今回の開始時間を埋め込む
                    $moveNode.attr('attr_ost', afterSTime);
                    $moveNode.attr('attr_osf', formatTime(afterSTime));

                    // cssのプロパティ変更を考慮し横幅を計算する
                    var width = getNodeWidth();
                    console.log(width);
                    //　ノードの横幅 / 一度に移動できる横幅　*　1移動幅辺りの秒
                    var nodeTime = width['width'] / draggableGridX * widthTime;

                    //　変更前時間にノードの横幅を足したものが変更前の終了時間
                    var originalETime = originalSTime + nodeTime;
                    // 変更後時間にノードの横幅を足したものが変更前の終了時間
                    var afterETime = afterSTime + nodeTime;

                    // 高さに対する処理を行う
                    //var originalTop = ui.originalPosition.top;
                    //var positionTop = ui.position.top;
                    var diffH = (ui.position.top - ui.originalPosition.top);
                    var moveOffsetH = (ui.position.top - ui.originalPosition.top) / draggableGridY;
                    moveOffsetH = parseInt(moveOffsetH);

                    // 範囲外
                    var outside = false;
                    if (tableStartTime > afterSTime) outside = true;
                    if (tableEndTime < afterETime) outside = true;

                    // 返却値
                    params['originalStartTime'] = originalSTime;
                    params['originalStartTimeF'] = originalSTimeF;
                    params['originalEndTime'] = originalETime;
                    params['originalEndTimeF'] = formatTime(originalETime);
                    params['changeStartTime'] = afterSTime;
                    params['changeStartTimeF'] = formatTime(afterSTime);
                    params['changeEndTime'] = afterETime;
                    params['changeEndTimeF'] = formatTime(afterETime);
                    params['height_offset'] = moveOffsetH;
                    params['offset'] = ui;
                    params['outside'] = outside;

                    // コールバックがセットされていたら呼出
                    if (options.callback) { options.callback($moveNode, params) };
                }
            });

            node.resizable({
                handles: 'e',
                grid: [resizableGridX, resizableGridY],
                minHeight: resizableMaxH,
                maxHeight: resizableMinH,

                start: function (event, ui) {
                    ui.size['width'] = ui.size['width'] + 10;
                },
                resize: function (event, ui) {
                    $moveNode = $(this);
                    $moveNode.width(ui.size['width']);
                },
                // 要素の移動が終った後の処理
                stop: function (event, ui) {
                    $moveNode = $(this);
                    $moveNode.width(ui.size['width']);
                    // ノードの初期位置を取得する
                    var originalLeft = ui.originalPosition['left'];
                    //var positionLeft = ui.position['left'];
                    //var offsetLeft   = $moveNode[0].offsetLeft;
                    var originalWidth = ui.originalSize['width'];
                    var sizeWidth = ui.size['width'];

                    /*************************************************************************
			 		 * 初期の開始時間を取得する
			 		 *************************************************************************/
                    // 要素に埋め込まれた時間の取得を試みる
                    var attr_ost = $moveNode.attr('attr_ost');
                    var attr_osf = $moveNode.attr('attr_osf');
                    var originalSTime;
                    var originalSTimeF;
                    if (typeof attr_ost == "undefined") {
                        originalSTime = tableStartTime + ((originalLeft - startOffsetX) / draggableGridX * widthTime);
                        originalSTimeF = formatTime(originalSTime);
                    } else {
                        originalSTime = Number(attr_ost);
                        originalSTimeF = attr_osf;
                    }

                    /*************************************************************************
			 		 * 変更後の終了時間を取得する
			 		 *************************************************************************/
                    // cssのプロパティ変更を考慮し横幅を計算する
                    var width = getNodeWidth();

                    //　ノードの横幅 / 一度に移動できる横幅　*　1移動幅辺りの秒
                    var originalNodeTime = (originalWidth + width['css']) / draggableGridX * widthTime;

                    //　変更前時間にノードの横幅を足したものが変更前の終了時間
                    var originalETime = originalSTime + originalNodeTime;

                    // 変更前時間にリサイズ後の横幅を足した物が変更後の終了時間
                    var afterNodeTime = width['width'] / draggableGridX * widthTime;
                    var afterETime = originalSTime + afterNodeTime;

                    // 範囲外
                    var outside = false;

                    if (tableEndTime < afterETime) outside = true;

                    // 返却値
                    params['originalStartTime'] = originalSTime;
                    params['originalStartTimeF'] = originalSTimeF;
                    params['originalEndTime'] = originalETime;
                    params['originalEndTimeF'] = formatTime(originalETime);
                    params['changeStartTime'] = originalSTime; // 開始前の時間変更は行われない
                    params['changeStartTimeF'] = originalSTimeF; // 開始前の時間変更は行われない
                    params['changeEndTime'] = afterETime;
                    params['changeEndTimeF'] = formatTime(afterETime);
                    params['height_offset'] = 0; // 高さの移動は行われない
                    params['offset'] = ui;
                    params['outside'] = outside;

                    // コールバックがセットされていたら呼出
                    if (options.callback) { options.callback($moveNode, params) };
                }
            });
        }
    }
    // Plugin entry
    $.fn.rcSchedule = function (arg) {
        node = this;
        options = arg;
        methods.init();
    };
    // 秒を(hh:ii)の文字列に変換して返却
    function formatTime(min) {
        var h = "" + (min / 36000 | 0) + (min / 3600 % 10 | 0);
        var i = "" + (min % 3600 / 600 | 0) + (min % 3600 / 60 % 10 | 0);
        var string = h + ":" + i;
        return string;
    };
    // 文字列で渡された時間(hh:ii)を秒数にして返却
    function calcStringTime(string) {
        var slice = string.split(':');
        var h = Number(slice[0]) * 60 * 60;
        var i = Number(slice[1]) * 60;
        var min = h + i;
        return min;
    };
    // pxを外しnumberで返却 
    function removePx(string) {
        var s = string.replace("px", "");
        return Number(s);
    };
    // ノードの横幅を計算する
    function getNodeWidth() {
        var cscProperty = $moveNode.css(['width', 'margin-left', 'margin-right', 'padding-left', 'padding-right']);
        var width = 0;
        for (var i in cscProperty) width += removePx(cscProperty[i]);
        var array = Array();
        array['width'] = width;
        array['css'] = width - removePx(cscProperty['width']);
        return array;
    }
})(jQuery);
