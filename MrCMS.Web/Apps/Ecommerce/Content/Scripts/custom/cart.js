$(function () {
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
    function update() {
    }
    $(document).on('click', "#apply-discount-code", function () {
        var discountCode = $("#discount-code").val();
        $.post('/Apps/Ecommerce/ApplyDiscountCode',
            { discountCode: discountCode },
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
        parent.$.get('/Apps/Ecommerce/CartDetails', function (items) {
            parent.$('#details').replaceWith(items);
        });
    }

    updateShippingMethod();
})