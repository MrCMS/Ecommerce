$(function () {
    $('#TaxStatus').click(function () {
        taxStatus = $(this);
        $('form#Settings').submit();
    });
})