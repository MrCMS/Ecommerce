$(function () {
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
    $(document).on('click', "#add-discount-code", function () {
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
    $(document).on('click', "#update-discount-code", function () {
        var discountCode = $("#DiscountCode").val();
        $("#add-discount-code-box").show();
        $("#discount-code-box").hide();
        $("div[class*='title']").html("Current discount code: " + discountCode);
        return false;
    });
    function setDiscountCode() {
        var discountCode = $("#DiscountCode").val();
        if (discountCode === "") {
            $("#add-discount-code-box").show();
            $("#discount-code-box").hide();
        }
        else {
            $("#add-discount-code-box").hide();
            $("#discount-code-box").show();
            $("#discount-code").val(discountCode);
        }
        $("div[class*='title']").html("Current discount code: " + discountCode);
    }

    setDiscountCode();
})