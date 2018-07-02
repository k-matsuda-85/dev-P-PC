// Copyright (c) 2013 Keith Perhac @ DelfiNet (http://delfi-net.com)
//
// Based on the AutoRuby library created by:
// Copyright (c) 2005-2008 spinelz.org (http://script.spinelz.org/)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

(function ($) {
    $.fn.autoKana = function (element1, element2, passedOptions) {

        var options = $.extend(
            {
                'katakana': false
            }, passedOptions);

        var kana_extraction_pattern = new RegExp('[^ 　ぁあ-んー]', 'g');
        var kana_compacting_pattern = new RegExp('[ぁぃぅぇぉっゃゅょ]', 'g');
        var elName,
            elKana,
            active = false,
            timer = null,
            flagConvert = true,
            input,
            values,
            ignoreString,
            baseKana;

        elName = $(element1);
        elKana = $(element2);
        active = true;
        _stateClear();

        elName.blur(_eventBlur);
        elName.focus(_eventFocus);
        elName.keydown(_eventKeyDown);

        function start() {
            active = true;
        };

        function stop() {
            active = false;
        };

        function toggle(event) {
            var ev = event || window.event;
            if (event) {
                var el = Event.element(event);
                if (el.checked) {
                    active = true;
                } else {
                    active = false;
                }
            } else {
                active = !active;
            }
        };

        function _checkConvert(new_values) {
            if (!flagConvert) {
                if (Math.abs(values.length - new_values.length) > 1) {
                    var tmp_values = new_values.join('').replace(kana_compacting_pattern, '').split('');
                    if (Math.abs(values.length - tmp_values.length) > 1) {
                        _stateConvert();
                    }
                } else {
                    if (values.length == input.length && values.join('') != input) {
                        if (input.match(kana_extraction_pattern)) {
                            _stateConvert();
                        }
                    }
                }
            }
        };

        function _checkValue() {
            var new_input, new_values;
            new_input = elName.val()
            if (new_input == '') {
                _stateClear();
                _setKana();
            } else {
                new_input = _removeString(new_input);
                if (input == new_input) {
                    return;
                } else {
                    input = new_input;
                    if (!flagConvert) {
                        new_values = new_input.replace(kana_extraction_pattern, '').split('');
                        _checkConvert(new_values);
                        _setKana(new_values);
                    }
                }
            }
        };

        function _clearInterval() {
            clearInterval(timer);
        };

        function _eventBlur(event) {
            _clearInterval();
        };
        function _eventFocus(event) {
            _stateInput();
            _setInterval();
        };
        function _eventKeyDown(event) {
            if (flagConvert) {
                _stateInput();
            }
        };
        function _isHiragana(chara) {
            return ((chara >= 12353 && chara <= 12435) || chara == 12445 || chara == 12446);
        };
        function _removeString(new_input) {
            if (new_input.match(ignoreString)) {
                return new_input.replace(ignoreString, '');
            } else {
                var i, ignoreArray, inputArray;
                ignoreArray = ignoreString.split('');
                inputArray = new_input.split('');
                for (i = 0; i < ignoreArray.length; i++) {
                    if (ignoreArray[i] == inputArray[i]) {
                        inputArray[i] = '';
                    }
                }
                return inputArray.join('');
            }
        };
        function _setInterval() {
            var self = this;
            timer = setInterval(_checkValue, 30);
        };
        function _setKana(new_values) {
            if (!flagConvert) {
                if (new_values) {
                    values = new_values;
                }
                if (active) {
                    var _val = _toKatakana(baseKana + values.join(''));
                    elKana.val(_val);
                }
            }
        };
        function _stateClear() {
            baseKana = '';
            flagConvert = false;
            ignoreString = '';
            input = '';
            values = [];
        };
        function _stateInput() {
            baseKana = elKana.val();
            flagConvert = false;
            ignoreString = elName.val();
        };
        function _stateConvert() {
            baseKana = baseKana + values.join('');
            flagConvert = true;
            values = [];
        };
        function _toKatakana(src) {
            if (options.katakana) {
                var c, i, str;
                str = new String;
                for (i = 0; i < src.length; i++) {
                    c = src.charCodeAt(i);
                    if (_isHiragana(c)) {
                        str += String.fromCharCode(c + 96);
                    } else {
                        str += src.charAt(i);
                    }
                }
                return _toHarf(str);
            } else {
                return src;
            }
        };
        function _toHarf(src) {
            var i, f, c, m, a = [];

            m =
            {
                0x30A1: 0xFF67, 0x30A3: 0xFF68, 0x30A5: 0xFF69, 0x30A7: 0xFF6A, 0x30A9: 0xFF6B,
                0x30FC: 0xFF70, 0x30A2: 0xFF71, 0x30A4: 0xFF72, 0x30A6: 0xFF73, 0x30A8: 0xFF74,
                0x30AA: 0xFF75, 0x30AB: 0xFF76, 0x30AD: 0xFF77, 0x30AF: 0xFF78, 0x30B1: 0xFF79,
                0x30B3: 0xFF7A, 0x30B5: 0xFF7B, 0x30B7: 0xFF7C, 0x30B9: 0xFF7D, 0x30BB: 0xFF7E,
                0x30BD: 0xFF7F, 0x30BF: 0xFF80, 0x30C1: 0xFF81, 0x30C4: 0xFF82, 0x30C6: 0xFF83,
                0x30C8: 0xFF84, 0x30CA: 0xFF85, 0x30CB: 0xFF86, 0x30CC: 0xFF87, 0x30CD: 0xFF88,
                0x30CE: 0xFF89, 0x30CF: 0xFF8A, 0x30D2: 0xFF8B, 0x30D5: 0xFF8C, 0x30D8: 0xFF8D,
                0x30DB: 0xFF8E, 0x30DE: 0xFF8F, 0x30DF: 0xFF90, 0x30E0: 0xFF91, 0x30E1: 0xFF92,
                0x30E2: 0xFF93, 0x30E4: 0xFF94, 0x30E6: 0xFF95, 0x30E8: 0xFF96, 0x30E9: 0xFF97,
                0x30EA: 0xFF98, 0x30EB: 0xFF99, 0x30EC: 0xFF9A, 0x30ED: 0xFF9B, 0x30EF: 0xFF9C,
                0x30F2: 0xFF66, 0x30F3: 0xFF9D, 0x3000: 0x0020, 0x30E3: 0xFF6C, 0x30E5: 0xFF6D,
                0x30E7: 0xFF6E, 0x30C3: 0xFF6F
            };

            for (i = 0, f = src.length; i < f; i++) {
                c = src.charCodeAt(i);
                switch (true) {
                    case (c in m):
                        a.push(m[c]);
                        break;
                    case (0xFF21 <= c && c <= 0xFF5E):
                        a.push(c - 0xFEE0);
                        break;
                        // ガ−ド
                    case (0x30AB <= c && c <= 0x30C9):
                        a.push(m[c - 1], 0xFF9E);
                        break;
                        // ハバパ−ホボポの濁点と半濁点
                    case (0x30CF <= c && c <= 0x30DD):
                        a.push(m[c - c % 3], [0xFF9E, 0xFF9F][c % 3 - 1]);
                        break;
                    default:
                        a.push(c);
                        break;
                };
            };

            return String.fromCharCode.apply(null, a);

        }
    };
})(jQuery);
