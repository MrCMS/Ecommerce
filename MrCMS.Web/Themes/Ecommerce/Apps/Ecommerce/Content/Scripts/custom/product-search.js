$(function () {
    $("#slider-range").slider({
        range: true,
        min: parseInt($("#product-price-range-min").val()),
        max: parseInt($("#product-price-range-max").val()),
        values: [0, parseInt($("#product-price-range-max").val())],
        slide: function (event, ui) {
            //alert(ui.values[0] + " - " + ui.values[1]);
           // $("#amount").val("$" + ui.values[0] + " - $" + ui.values[1]);
        }
    });
})