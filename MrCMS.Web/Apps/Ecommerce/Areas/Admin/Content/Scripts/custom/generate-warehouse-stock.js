(function ($) {

    function showCorrectFields(event) {
        var type = $(event.target);
        var val = type.val();
        $('[data-generation-type]').hide();
        $('[data-generation-type="' + val + '"]').show();
    }

    $(function() {
        $(document).on('change', '#StockGenerationType', showCorrectFields);
        $('#StockGenerationType').change();
    });
})(jQuery)