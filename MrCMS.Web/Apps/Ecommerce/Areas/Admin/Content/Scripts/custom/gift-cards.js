(function($) {
    $(function() {
        $(document).on('click', 'button[data-generate-gift-card-code]', function(event) {
            event.preventDefault();
            var button = $(event.target);
            var target = $('#' + button.data('generate-gift-card-code'));
            if (target.val()) {
                if (!confirm("Confirm that you want to regenerate the code")) {
                    return;
                }
            }
            $.get('/Admin/Apps/Ecommerce/GiftCard/GenerateCode', function(result) {
                target.val(result);
            });
        });
    });
})(jQuery);