$(function () {
    $('.product-variants-filter select').live("change", function () {
        productVariantId = $(this).val();
        $("#IdVariant").val(productVariantId);
        $.getJSON('/Apps/Ecommerce/ProductVariant/GetPriceBreaksForProductVariant',
            { id: productVariantId }, function (response) {
                $("#product-pricebreaks-table").empty();
                $('#product-variants-pricebreaks .product-pricebreaks-title').hide();
                $.each(response, function (key, val) {
                    $('#product-variants-pricebreaks .product-pricebreaks-title').show();
                    $("#product-pricebreaks-table").html("<tr><th>Quantity</th><th>Price</th></tr><tr><td>" + val["Quantity"] + "</td><td>" + val["Price"] + "</td></tr>");
                });
            });
    });

    var productVariantId = $(".product-variants-filter select").val();
    $("#Id").val(productVariantId);
})

