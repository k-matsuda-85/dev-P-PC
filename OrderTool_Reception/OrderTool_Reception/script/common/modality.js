function setModColor(obj, mod)
{
    obj.removeClass(function (index, className) {
        return (className.match(/\bColor_\S+/g) || []).join(' ');
    });
    obj.addClass('Color_' + mod);
}

function removeModClass(obj) {
    obj.removeClass(function (index, className) {
        return (className.match(/\bColor_\S+/g) || []).join(' ');
    });
}

