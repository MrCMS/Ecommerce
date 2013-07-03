$(function() {
    $("#slider-range").slider({
        range: true,
        min: parseInt($("#product-price-range-min").val(), 10),
        max: parseInt($("#product-price-range-max").val(), 10),
        values: [parseInt($("#product-price-range-min").val(), 10), parseInt($("#product-price-range-max").val(), 10)],
        change: function(event, ui) {
            $("#product-price-range-min-value").html(ui.values[0]);
            $("#product-price-range-max-value").html(ui.values[1]);
            search();
        }
    });
    $(document).on('change', "select[id*='product-option-values-']", function() {
        search();
    });
    $(document).on('change', "select[id*='product-specification-values-']", function() {
        search();
    });
    $(document).on('change', "#show #options", function() {
        search();
    });
    $(document).on('change', "#PageSize", function() {
        setStats();
    });
    $(document).on('change', "#TotalItemCount", function() {
        setStats();
    });
    $(document).on('change', "#PageNumber", function() {
        setStats();
    });
    $(document).on('change', "#product-sorting-options", function() {
        search();
    });

    function search() {
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
            }, function(products) {
                products = products.replace(/\?action=Category/gi, $("#LiveUrlSegment").val() + "?action=Category");
                $('#products-wrapper').replaceWith(products);
                if ($('#PageSize').text() === "0" || $('#products .item').length === 0) {
                    $('#stats').html("");
                }
            });
    }

    function getOptionValues() {
        var optionValues = "";
        $("select[id *= 'product-option-values-']").each(function(index) {
            if ($(this).val() !== "0") {
                optionValues += $(this).val().toLowerCase() + ",";
            }
        });
        return optionValues;
    }

    function getSpecificationValues() {
        var specificationValues = "";
        $("select[id *= 'product-specification-values-']").each(function(index) {
            if ($(this).val() !== "0") {
                specificationValues += $(this).val().toLowerCase() + ",";
            }
        });
        return specificationValues;
    }

    function setStats() {
        var pageSize = parseInt($('#PageSize').text(), 10);
        var pageNumber = parseInt($('#PageNumber').text(), 10);
        var totalItemCount = parseInt($('#TotalItemCount').text(), 10);
        if (!isNaN(pageSize)) {
            if (pageSize === 1 && totalItemCount === 1) {
                $('#stats').html("Showing 1 result");
            } else {
                var start;
                if (pageSize === 1 && pageNumber === 1) {
                    start = 1;
                }
                if (pageSize === 1 && pageNumber > 0) {
                    start = (pageSize * pageNumber) - pageSize;
                }
                else {
                    start = (pageSize * pageNumber) - pageSize + 1;
                }
                var end = (pageSize * pageNumber);

                if ($('#PageSize').text() === "1") {
                    $('#stats').html("Showing item #" + (start + 1) + " of " + totalItemCount + " results");
                } else {
                    $('#stats').html("Showing " + start + " - " + end + " of " + totalItemCount + " results");
                }
            }
        }
        else {
            $('#stats').html("");
        }
    }

    setStats();
});