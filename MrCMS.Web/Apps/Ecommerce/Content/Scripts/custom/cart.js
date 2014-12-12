$(function () {
    $.ajaxSetup({ cache: false });

    $(document).on('click', "#empty-basket", function (event) {
        event.preventDefault();
        var response = confirm("Are you sure you want to empty your basket? You can not undo this action.");
        if (response === true) {
            $.post('/Apps/Ecommerce/EmptyBasket', updateCartInfo);
        }
    });

    $(document).on('click', "#update-basket", function (event) {
        event.preventDefault();
        var values = $('input[type="text"][name^="quantity-"]').map(function (index, element) {
            var quantityElement = $(element);
            var quantity = quantityElement.val();
            var cartId = quantityElement.data('cart-id');
            return cartId + ":" + quantity;
        }).toArray();

        $.post('/Apps/Ecommerce/UpdateBasket', { quantities: values.join(',') }, updateCartInfo);
    });

    $(document).on('add-discount', updateCartInfo);
    $(document).on('remove-discount', updateCartInfo);

    $(document).on('click', "#apply-gift-card-code", function (event) {
        event.preventDefault();
        var codeInput = $("#gift-card-code");
        var giftCardCode = codeInput.val();
        $.post('/Apps/Ecommerce/ApplyGiftCardCode',
            { giftCardCode: giftCardCode },
            function (response) {
                if (response.Success) {
                    updateCartInfo();
                } else {
                    codeInput.addClass('input-validation-error');
                    $('[data-gift-card-message]').html(response.Message);
                }
            });
    });

    $(document).on('click', "[data-remove-card]", function (event) {
        event.preventDefault();
        var code = $(event.target).data('remove-card');
        $.post('/Apps/Ecommerce/RemoveGiftCardCode', { giftCardCode: code }, updateCartInfo);
    });

    function getRemaining() {
        var message = $('#gift-message');
        if (!message.length)
            return 0;
        var length = message.val().length;
        var number = $('[data-message-container]').data('message-container');
        return number - length;
    }

    $(document).on('change', "#UseRewardPoints", function (event) {
        event.preventDefault();
        $.post('/Apps/Ecommerce/UseRewardPoints', { useRewardPoints: $(event.target).is(':checked') }, updateCartInfo);
    });
    $(document).on('click', "a[data-action=delete-cart-item]", function (event) {
        event.preventDefault();
        var id = $(this).data('id');
        $.post('/Apps/Ecommerce/DeleteCartItem', { id: id }, updateCartInfo);
    });

    function updateCartInfo() {
        $('[data-cart-info]').each(function() {
            var info = $(this);
            $.get(info.data('cart-info'), function(result) {
                info.replaceWith(result);
            });
        });
    }

    var timer = 0;
    var getCurrentValue = function () {
        return {
            message: $('#gift-message').val()
        };
    };
    var delayedUpdateGiftMessage = function (event) {
        showRemaining();
        if (remaining < 0) {
            characterCountMessage.addClass('field-validation-error');
        } else {
            characterCountMessage.removeClass('field-validation-error');
        }

        clearTimeout(timer);
        timer = setTimeout(function () { updateGiftMessage(event); }, 300);
    };

    var updateGiftMessage = function (event) {
        event.preventDefault();
        $.post('/Apps/Ecommerce/SaveGiftMessage', { message: getCurrentValue().message }, updateCartInfo);
    };

    var showRemaining = function () {
        remaining = getRemaining();
        characterCountMessage.html($('<span>').html(remaining + ' characters remaining'));
    }

    var remaining;
    var characterCountMessage = $('#message-character-count');
    $(document).on('keyup', '#gift-message', delayedUpdateGiftMessage);
    showRemaining();
});