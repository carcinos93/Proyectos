/// <reference path="typings/jquery/jquery.d.ts" />
$(document).ready(() => {
    $("#render").on('click', (e) => {
        var obj = new Object();
        $('.param').each((i, e) => {

            var element = $(e) as JQuery;
            obj[element.attr("name")] = element.val();
        });
        $("#parametros").val( JSON.stringify(obj) );
        $("#form").submit();
    })
    
});