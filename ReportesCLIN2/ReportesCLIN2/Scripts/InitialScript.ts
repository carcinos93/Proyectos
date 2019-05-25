/// <reference path="typings/jquery/jquery.d.ts" />
function resizeIframe(obj) {
    obj.style.height = obj.contentWindow.document.body.scrollHeight + 'px';
  }
$(document).ready(() => {
    $("#renderReport").on('load', (e) => {
        resizeIframe(e);
    });
    $("#render").on('click', (e) => {
        var obj = new Object();
        $('.param').each((i, e) => {

            var element = $(e) as JQuery;
            obj[element.attr("name")] = element.val();
        });
        var parametros = ( JSON.stringify(obj) );
        $("#renderReport").attr("src", window['url_base'] + "?parametros=" + parametros);

        $("#renderReport").height($("#renderReport").contents().document.body.scrollHeight + 'px');
        //obj.style.height = obj.contentWindow.document.body.scrollHeight + 'px';
    })
    
});