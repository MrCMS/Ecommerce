$(function () {

    //$(document).on('click', '#categories .add-category', function () {
    //    var button = $(this),
    //        productId = button.data('product-id'),
    //        categoryId = button.data('category-id'),
    //        page = button.data('page');
    //    $.post('/Admin/Apps/Ecommerce/Product/AddCategory/',
    //        { id: productId, categoryId: categoryId },
    //        function (response) {
    //            parent.$.get('/Admin/Apps/Ecommerce/Product/Categories/' + productId, function (products) {
    //                parent.$('#category-list').replaceWith(products);
    //            });
    //            var href = '/Admin/Apps/Ecommerce/Product/AddCategory/' + productId + '?page=' + page;
    //            $('.modal-body-container').load(href + ' div#categories', function () {
    //                resizeModal();
    //            });
    //        });
    //    return false;
    //});


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

