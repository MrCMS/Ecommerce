(function ($) {
    function addDiscount(event) {
        event.preventDefault();
        var form = $(event.target);
        $.post(form.attr('action'), form.serialize(), function (result) {
            $(document).trigger('add-discount', result);
        });
    }

    function removeDiscount(event) {
        event.preventDefault();
        $.post($(event.target).data('remove-discount-code'), function (result) {
            $(document).trigger('remove-discount', result);
        });
    }

    $(function () {
        $(document).on('submit', "form[data-apply-discount]", addDiscount);
        $(document).on('click', '[data-remove-discount-code]', removeDiscount);
    });
})(jQuery);