// 要素内のキャレット位置を取得する関数
function getCaretPositionIE(elem) {
    elem.focus();

    // ボックスの先頭からキャレットまでのrangeを作って，長さを調査
    var range = document.selection.createRange();
    range.moveStart("character", -elem.value.length);
    var caret_position = range.text.length;

    return caret_position;
}
