$(function () {
    $('#DefaultCategory').change(function () {
        $('form#Settings').submit();
    });
    $('#DefaultCondition').change(function () {
        $('form#Settings').submit();
    });
    $('#Category').change(function () {
        $('form#Filter').submit();
    });
})