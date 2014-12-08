(function ($) {
    function updateSummary() {
        $.get('/Apps/Ecommerce/Checkout/Summary', function (result) {
            $('#basic-details').replaceWith(result);
        });
    }
    $(function () {
        $(document).on('add-discount', updateSummary);
        $(document).on('remove-discount', updateSummary);
        $(document).on('update-summary', updateSummary);
    });
})(jQuery);