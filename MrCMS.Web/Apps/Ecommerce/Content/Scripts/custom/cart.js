$(function () {
    $.ajaxSetup({ cache: false });
    $(document).on('click', "#empty-basket", function () {
        var response = confirm("Are you sure you want to empty your basket? You can not undo this action.");
        if (response === true) {
            $.post('/Apps/Ecommerce/EmptyBasket',
                function (response) {
                    reloadCartDetails();
                });
        }
        return false;
    });

    $(document).on('change', "select#ShippingCalculation", function () {
        updateShippingMethod();
    });

    $(document).on('click', "#update-basket", function () {
        var values = $('input[type="text"][name^="quantity-"]').map(function (index, element) {
            var quantity = $(element).val();
            var cartId = $(this).data('cart-id');
            return cartId + ":" + quantity;
        }).toArray();

        $.post('/Apps/Ecommerce/UpdateBasket',
            { quantities: values.join(',') },
            function (response) {
                reloadCartDetails();
            });
        return false;
    });
    $(document).on('click', "#apply-discount-code", function (event) {
        event.preventDefault();
        var discountCode = $("#discount-code").val();
        $.post('/Apps/Ecommerce/ApplyDiscountCode',
            { discountCode: discountCode },
            function (response) {
                reloadCartDetails();
            });
    });
    $(document).on('click', "#apply-gift-card-code", function (event) {
        event.preventDefault();
        var giftCardCode = $("#gift-card-code").val();
        $.post('/Apps/Ecommerce/ApplyGiftCardCode',
            { giftCardCode: giftCardCode },
            function (response) {
                reloadCartDetails();
            });
    });
    $(document).on('click', "[data-remove-card]", function (event) {
        event.preventDefault();
        var code = $(event.target).data('remove-card');
        $.post('/Apps/Ecommerce/RemoveGiftCardCode',
            { giftCardCode: code },
            function (response) {
                reloadCartDetails();
            });
    });
    $(document).on('click', "[data-edit-gift-message]", function (event) {
        event.preventDefault();
        $('[data-message-container]').show();
        $('[data-message-display]').hide();
        updateCharacterCount();
    });
    $(document).on('click', "[data-remove-gift-message]", function (event) {
        event.preventDefault();
        $.post('/Apps/Ecommerce/SaveGiftMessage',
            { message: '' }, reloadCartDetails
        );
    });
    $(document).on('click', "[data-save-gift-message]", function (event) {
        event.preventDefault();
        if (getRemaining() < 0) {
            var errorMessage = $('[data-message-container]').data('error-message');
            alert(errorMessage);
            return;
        }
        var message = $('#gift-message').val();
        $.post('/Apps/Ecommerce/SaveGiftMessage',
            { message: message }, reloadCartDetails
        );
    });
    function getRemaining() {
        var message = $('#gift-message');
        if (!message.length)
            return 0;
        var length = message.val().length;
        var number = $('[data-message-container]').data('message-container');
        console.log(number);
        return number - length;
    }
    function updateCharacterCount() {
        var remaining = getRemaining();
        var characterCountMessage = $('#message-character-count');
        characterCountMessage.html($('<span>').html(remaining + ' characters remaining'));
        if (remaining < 0) {
            characterCountMessage.addClass('field-validation-error');
        } else {
            characterCountMessage.removeClass('field-validation-error');
        }
    }

    $(document).on('keyup', '#gift-message', updateCharacterCount);


    $(document).on('click', "#remove-discount-code", function () {
        $.post('/Apps/Ecommerce/ApplyDiscountCode',
            { discountCode: null },
            function (response) {
                reloadCartDetails();
            });
        return false;
    });
    $(document).on('click', "a[data-action=delete-cart-item]", function () {
        var id = $(this).data('id');
        $.post('/Apps/Ecommerce/DeleteCartItem', { id: id }, function (response) {
            reloadCartDetails();
        });
        return false;
    });

    function updateShippingMethod() {
        $.post('/Apps/Ecommerce/SetDeliveryDetails/SetShipping',
            { id: $("select#ShippingCalculation").val() },
            function (response) {
                reloadCartDetails();
            });
    }

    function reloadCartDetails() {
        $.get('/Apps/Ecommerce/CartDetails', function (items) {
            $('#details').replaceWith(items);
        });
    }
});