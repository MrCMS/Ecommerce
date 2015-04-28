$(function () {
    $("#copy-link").on('click', function() {
        var test = $("#preview").eq(0).get()[0].contentDocument.documentElement.outerHTML;
        $('[data-preview-container]').show();
        $('[data-preview-html]').val(test).select();
    });
});