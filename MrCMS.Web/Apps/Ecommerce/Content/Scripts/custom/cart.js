$(function () {
    $.ajaxSetup({ cache: false });
    $(document).on('click', "#empty-basket", function() {
        var response = confirm("Are you sure you want to empty your basket? You can not undo this action.");
        if (response === true) {
            $.post('/Apps/Ecommerce/EmptyBasket',
                function(response) {
                    reloadCartDetails();
                });
        }
        return false;
    });

    $(document).on('change', "select#ShippingCalculation", function() {
        updateShippingMethod();
    });

    $(document).on('click', "#update-basket", function() {
        var values = $('input[type="text"][name^="quantity-"]').map(function(index, element) {
            var quantity = $(element).val();
            var cartId = $(this).data('cart-id');
            return cartId + ":" + quantity;
        }).toArray();

        $.post('/Apps/Ecommerce/UpdateBasket',
            { quantities: values.join(',') },
            function(response) {
                reloadCartDetails();
            });
        return false;
    });
    $(document).on('click', "#apply-discount-code", function() {
        var discountCode = $("#discount-code").val();
        $.post('/Apps/Ecommerce/ApplyDiscountCode',
            { discountCode: discountCode },
            function(response) {
                reloadCartDetails();
            });
        return false;
    });
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
        $.post('/Apps/Ecommerce/DeleteCartItem', { id: id }, function(response) {
            reloadCartDetails();
        });
        return false;
    });

    function updateShippingMethod() {
        $.post('/Apps/Ecommerce/SetDeliveryDetails/SetShipping',
            { id: $("select#ShippingCalculation").val() },
            function(response) {
                reloadCartDetails();
            });
    }

    function reloadCartDetails() {
        $.get('/Apps/Ecommerce/CartDetails', function (items) {
            $('#details').replaceWith(items);
        });
    }
});