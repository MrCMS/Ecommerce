$(function () {
    $("#slider-range").slider({
        range: true,
        min: parseInt($("#product-price-range-min").val()),
        max: parseInt($("#product-price-range-max").val()),
        change: function (event, ui) {
            $("#product-price-range-min-value").html(ui.values[0]);
            $("#product-price-range-max-value").html(ui.values[1]);
            $.get('/Apps/Ecommerce/ProductSearch/Results', { id: $("#Id").val(), options: 'opt', specifications: 'spec', pageNo:1, productPriceRangeMin: ui.values[0], productPriceRangeMax: ui.values[1] }, function (products) {
                $('#products-wrapper').replaceWith(products);
            });
        }
    });
})