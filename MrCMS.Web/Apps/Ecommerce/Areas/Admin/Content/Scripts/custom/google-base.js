$(function () {
    //FILTER PRODUCTS
    $('#Category').change(function () {
        $('form#Filter').submit();
    });

    //GOOGLE BASE PRODUCT

    setGoogleBaseCategoriesSelect();
    
    $(document).on('click', 'button.update-google-base-product', function () {
        var productVariantId = $(this).data('google-base-product-variant-id');
        $.post('/Admin/Apps/Ecommerce/GoogleBase/UpdateGoogleBaseProduct',
                $(this).parents('form').serialize(),
            function (response) {
                if (response !== false) {
                    $.get('/Admin/Apps/Ecommerce/GoogleBase/GoogleBaseProduct/' + response, function (result) {
                        $('#google-base-product-variant-' + productVariantId).replaceWith(result);
                        setGoogleBaseCategoriesSelect();
                    });
                } else {
                    alert("Error happened during update operation. Please check your parameters and try again.");
                }
            });
        return false;
    });

    function setGoogleBaseCategoriesSelect() {
        $(".select2").select2({
            placeholder: "Search for a Google Base category...",
            minimumInputLength: 1,
            id: function(item) { return item.Name; },
            ajax: {
                url: "/Admin/Apps/Ecommerce/GoogleBase/GetGoogleCategories",
                dataType: 'json',
                data: function(term, page) {
                    return {
                        term: term,
                        page: page
                    };
                },
                results: function(data, page) {
                    var more = (page * 10) < data.Total;
                    return { results: data.Items, more: more };
                }
            },
            initSelection: function(element, callback) {
                var id = $(element).val();
                if (id !== "") {
                    callback({ Name: $(element).data('category-name') });
                }
            },
            formatResult: formatResult,
            formatSelection: formatSelection,
            escapeMarkup: function(m) { return m; }
        });
    }

    function formatResult(item) {
        return item.Name;
    }

    function formatSelection(item) {
        return item.Name;
    }
})