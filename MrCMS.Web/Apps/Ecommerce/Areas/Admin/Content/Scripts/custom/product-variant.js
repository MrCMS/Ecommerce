$(function() {
    $('form').validate({
        rules: {
            BasePrice: {
                required: true,
                number: true,
                min: 0
            },
            SKU: {
                required: true,
                remote: {
                    url: "/Admin/Apps/Ecommerce/ProductVariant/IsUniqueSKU",
                    type: "get",
                    data: {
                        sku: function() {
                            return $("#SKU").val();
                        },
                        id: function() {
                            return $("#Id").val();
                        }
                    }
                }
            },
            Weight: {
                number: true,
                min: 0
            },
            StockRemaining: {
                number: true,
                min: -1000
            },
            PreviousPrice: {
                number: true,
                min: 0
            },
        },
        highlight: function(label) {
            $(label).closest('.span12').addClass('error');
        },
        success: function(label) {
            label
                .addClass('valid')
                .closest('.span12').addClass('success');
        }
    });
    if ($("#BasePrice").val() === "0") {
        $("#BasePrice").val("");
    }
    $('input[type="text"][name^="AttributeValues"]').each(function(index, element) {
        $(element).rules("add", {
            required: true
        });
    });

});

$(document).ready(function() {

    function format(state) {
        var option = state.element;
        var url = $(option).data('url');
        $("#FeaturedImageUrl").val($(option).data('url'));
        if (url != 0) {
            return "<table><tr><td><img style='width:80px;max-height:50px;' src='" + url + "' " +
                "alt='" + $(option).data('name') + "' /></td><td style='padding-left:10px'>" + state.text + "</td></tr></table>";
        } else {
            return "<p style='padding-top:15px'>" + state.text+"</p>";
        }
    }

    $("#FeaturedImageUrlBox").select2({
        formatResult: format,
        formatSelection: format,
        escapeMarkup: function(m) { return m; }
    });
    
});
