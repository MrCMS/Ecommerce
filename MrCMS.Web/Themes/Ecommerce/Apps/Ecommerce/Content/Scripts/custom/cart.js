$(function () {
    $(document).on('click', "button[id*='quantity-']", function () {
        update($(this));
    });
    function update(control) {
        var quantity = $("input[name*='" + control.attr("id") + "']").val();
        var cartId = control.data('cart-id');
        $.post('/Apps/Ecommerce/Cart/UpdateQuantity/',
            { quantity: quantity, cartId: cartId },
            function (response) {
                parent.$.get('/Apps/Ecommerce/Cart/Details', function (items) {
                    parent.$('#cart-items').replaceWith(items);
                });
            });
        return false;
    }
})