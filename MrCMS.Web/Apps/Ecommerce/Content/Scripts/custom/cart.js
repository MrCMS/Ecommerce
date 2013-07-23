$(function () {
    $(document).on('change', "select#ShippingCalculation", function () {
        updateShippingMethod();
    });
    $(document).on('click', "#update-basket", function () {
        update();
    });
    function update() {
        $('input[type="text"][name^="quantity-"]').each(function (index, element) {
            var quantity = $(element).val();
            var cartId = $(this).data('cart-id');
            $.post('/Apps/Ecommerce/Cart/UpdateQuantity/',
                { quantity: quantity, cartId: cartId },
                function (response) {
                    parent.$.get('/Apps/Ecommerce/Cart/Details', function (items) {
                        parent.$('#details').replaceWith(items);
                        setDiscountCode();
                    });
                });
        });
        return false;
    }
    $(document).on('click', "#apply-discount-code", function () {
        var discountCode = $("#discount-code").val();
            $.post('/Apps/Ecommerce/Cart/AddDiscountCodeAjax/',
                { discountCode: discountCode },
                function (response) {
                    if (response === false) {
                        alert("Discount code is not valid.");
                    }
                    else {
                        parent.$.get('/Apps/Ecommerce/Cart/Details', function (items) {
                            parent.$('#details').replaceWith(items);
                            setDiscountCode();
                        });
                    }
                });
        return false;
    });
    $(document).on('click', "#apply-discount-code", function () {
        var discountCode = $("#DiscountCode").val();
        $("div[class*='title']").html("Current discount code: " + discountCode);
        return false;
    });
    function setDiscountCode() {
        var discountCode = $("#DiscountCode").val();
        if (discountCode !== "") {
            $("#discount-code").val(discountCode);
        }
        $("div[class*='title']").html("Current discount code: " + discountCode);
    }
    function updateShippingMethod() {
        $.post('/Apps/Ecommerce/SetDeliveryDetails/SetShipping',
           { id: $("select#ShippingCalculation").val() },
           function (response) {
               parent.$.get('/Apps/Ecommerce/Cart/Details', function (items) {
                   parent.$('#details').replaceWith(items);
                   setDiscountCode();
               });
           });
    }

    updateShippingMethod();
    setDiscountCode();
})