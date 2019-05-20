/// <reference path="typings/jquery/jquery.d.ts" />
$(document).ready(function () {
    $("#render").on('click', function (e) {
        var obj = new Object();
        $('.param').each(function (i, e) {
            var element = $(e);
            obj[element.attr("name")] = element.val();
        });
        $("#parametros").val(JSON.stringify(obj));
        $("#form").submit();
    });
});
//# sourceMappingURL=InitialScript.js.map