$(function () {
    $('.product-variants-filter select').live("change", function () {
        var productVariantId = $(this).val();
        $.getJSON('/Apps/Ecommerce/ProductVariant/GetPriceBreaksForProductVariant',
            { productVariantId: productVariantId }, function (response) {
                $("#product-pricebreaks-table").empty();
                $('#product-variants-pricebreaks .product-pricebreaks-title').hide();
                $.each(response, function (key, val) {
                    $('#product-variants-pricebreaks .product-pricebreaks-title').show();
                    $("#product-pricebreaks-table").html("<tr><th>Quantity</th><th>Price</th></tr><tr><td>" + val["Quantity"] + "</td><td>"+ val["Price"] +"</td></tr>");
                })
            });
    });
})

