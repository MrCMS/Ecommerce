$(function () {
    $("#slider-range").slider({
        range: true,
        min: parseInt($("#product-price-range-min").val()),
        max: parseInt($("#product-price-range-max").val()),
        values:[parseInt($("#product-price-range-min").val()),parseInt($("#product-price-range-max").val())],
        change: function (event, ui) {
            $("#product-price-range-min-value").html(ui.values[0]);
            $("#product-price-range-max-value").html(ui.values[1]);
            search();
        }
    });
    $(document).on('change', "select[id*='product-option-values-']", function () {
        search();
    });
    $(document).on('change', "select[id*='product-specification-values-']", function () {
        search();
    });
    $(document).on('change', "#show #options", function () {
        search();
    });
    $(document).on('change', "#PageSize", function () {
        $('#stats').html("Showing 1 - " + $('#PageSize').text() + " of " + $('#TotalItemCount').text() + " results");
    });
    $(document).on('change', "#TotalItemCount", function () {
        $('#stats').html("Showing 1 - " + $('#PageSize').text() + " of " + $('#TotalItemCount').text() + " results");
    });
    $(document).on('change', "#product-sorting-options", function () {
        search();
    });
    function search()
    {
        $.get('/Apps/Ecommerce/Category/Results',
            {
                id: $("#Id").val(),
                sortBy: $("#product-sorting-options").val(),
                options: getOptionValues(),
                specifications: getSpecificationValues(),
                productPriceRangeMin: $("#product-price-range-min-value").html(),
                productPriceRangeMax: $("#product-price-range-max-value").html(),
                pageNo: 1,
                pageSize: $("#show #options").val(),
                categoryId: $("#Id").val(),
                searchTerm: $("#searchTerm").val()
            }, function (products) {
                products = products.replace(/\?action=Category/gi, $("#LiveUrlSegment").val() + "?action=Category");
                $('#products-wrapper').replaceWith(products);
                if ($('#PageSize').text() == "0")
                    $('#stats').html("");
        });
    }
    function getOptionValues()
    {
        var optionValues = "";
        $("select[id *= 'product-option-values-']").each(function (index) {
            if ($(this).val() != "0") {
                optionValues += $(this).val().toLowerCase() + ",";
            }
        });
        return optionValues;
    }
    function getSpecificationValues() {
        var specificationValues = "";
        $("select[id *= 'product-specification-values-']").each(function (index) {
            if ($(this).val() != "0") {
                specificationValues += $(this).val().toLowerCase() + ",";
            }
        });
        return specificationValues;
    }
   
    $('#stats').html("Showing 1 - " + $('#PageSize').text() + " of " + $('#TotalItemCount').text() + " results");
})