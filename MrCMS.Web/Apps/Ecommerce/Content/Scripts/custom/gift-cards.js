(function ($) {
    function removeGiftCard(event) {
        event.preventDefault();
        var code = $(event.target).data('remove-gift-card');
        $.post('/Apps/Ecommerce/RemoveGiftCardCode', { giftCardCode: code }, function (response) {
            $(document).trigger('gift-card-removed');
        });
    }

    function applyGiftCard(event) {
        event.preventDefault();
        var button = $(event.target);
        var codeId = button.data('apply-gift-card-code');
        var codeInput = $('#' + codeId);
        var giftCardCode = codeInput.val();
        $.post('/Apps/Ecommerce/ApplyGiftCardCode',
            { giftCardCode: giftCardCode },
            function (response) {
                if (response.Success) {
                    $(document).trigger('gift-card-applied');
                } else {
                    codeInput.addClass('input-validation-error');
                    $('[data-gift-card-message="' + codeId + '"]').html(response.Message);
                }
            });
    }

    $(function () {
        $(document).on('click', "[data-apply-gift-card-code]", applyGiftCard);
        $(document).on('click', "[data-remove-gift-card]", removeGiftCard);
    });
})(jQuery);