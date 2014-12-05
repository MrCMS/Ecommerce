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

    $(document).on('click', '.remove-discount-code', function (e) {
        e.preventDefault();
        var discountCode = $(this).data('code');
        $.post('/Apps/Ecommerce/RemoveDiscountCode',
            { discountCode: discountCode },
            function (response) {
                reloadCartDetails();
            });
    });

    $(document).on('click', "#apply-gift-card-code", function (event) {
        event.preventDefault();
        var codeInput = $("#gift-card-code");
        var giftCardCode = codeInput.val();
        $.post('/Apps/Ecommerce/ApplyGiftCardCode',
            { giftCardCode: giftCardCode },
            function (response) {
                if (response.Success) {
                    reloadCartDetails();
                } else {
                    codeInput.addClass('input-validation-error');
                    $('[data-gift-card-message]').html(response.Message);
                }
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
    function getRemaining() {
        var message = $('#gift-message');
        if (!message.length)
            return 0;
        var length = message.val().length;
        var number = $('[data-message-container]').data('message-container');
        console.log(number);
        return number - length;
    }
    //$(document).on('click', "#remove-discount-code", function () {
    //    $.post('/Apps/Ecommerce/ApplyDiscountCode',
    //        { discountCode: null },
    //        function (response) {
    //            reloadCartDetails();
    //        });
    //    return false;
    //});



    $(document).on('change', "#UseRewardPoints", function (event) {
        $.post('/Apps/Ecommerce/UseRewardPoints',
            { useRewardPoints: $(event.target).is(':checked') },
            reloadCartDetails
        );
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
        saveMessage();
    };

    var saveMessage = function () {
        var message = getCurrentValue().message;
        $.post('/Apps/Ecommerce/SaveGiftMessage',
            { message: message }, reloadCartDetails
        );
    };

    var showRemaining = function () {
        var remaining = getRemaining();
        var characterCountMessage = $('#message-character-count');
        characterCountMessage.html($('<span>').html(remaining + ' characters remaining'));
    }

    $(document).on('keyup', '#gift-message', delayedUpdateGiftMessage);
    showRemaining();
});