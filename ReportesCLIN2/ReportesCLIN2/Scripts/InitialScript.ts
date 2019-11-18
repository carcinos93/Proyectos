/// <reference path="typings/jquery/jquery.d.ts" />
function resizeIframe(obj) {
    obj.style.height = obj.contentWindow.document.body.scrollHeight + 'px';
}

var getPartametros = (): string => {
    var obj = new Object();
    $('.param').each((i, e) => {
        var element = $(e) as JQuery;
        obj[element.attr("name")] = element.val();
    });
    var parametros = (JSON.stringify(obj));
    return parametros;
}
$(document).ready(() => {
    $("#renderReport").on('load', (e) => {
        resizeIframe(e);
    });


    ///Evento click en boton de exportar
    $("#export").on('click', (e) => {
        let parametros = getPartametros();
        window.open(window['url_base_export'] + "?formato="+ $("#format").val() +"&parametros=" + parametros, "_blank");

    });

    ///Evento click en boton de visualizar reporte
    $("#render").on('click', (e) => {
        let parametros = getPartametros();
        $("#renderReport").attr("src", window['url_base_render'] + "?parametros=" + parametros);

        //$("#renderReport").height($("#renderReport").contents().document.body.scrollHeight + 'px');
        //obj.style.height = obj.contentWindow.document.body.scrollHeight + 'px';
    })

});