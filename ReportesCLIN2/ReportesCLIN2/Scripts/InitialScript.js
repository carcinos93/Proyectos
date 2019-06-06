/// <reference path="typings/jquery/jquery.d.ts" />
function resizeIframe(obj) {
    obj.style.height = obj.contentWindow.document.body.scrollHeight + 'px';
}
var getPartametros = function () {
    var obj = new Object();
    $('.param').each(function (i, e) {
        var element = $(e);
        obj[element.attr("name")] = element.val();
    });
    var parametros = (JSON.stringify(obj));
    return parametros;
};
$(document).ready(function () {
    $("#renderReport").on('load', function (e) {
        resizeIframe(e);
    });
    $("#export").on('click', function (e) {
        var parametros = getPartametros();
        window.open(window['url_base_export'] + "?formato=" + $("#format").val() + "&parametros=" + parametros, "_blank");
    });
    $("#render").on('click', function (e) {
        var parametros = getPartametros();
        $("#renderReport").attr("src", window['url_base_render'] + "?parametros=" + parametros);
        //$("#renderReport").height($("#renderReport").contents().document.body.scrollHeight + 'px');
        //obj.style.height = obj.contentWindow.document.body.scrollHeight + 'px';
    });
});
//# sourceMappingURL=InitialScript.js.map