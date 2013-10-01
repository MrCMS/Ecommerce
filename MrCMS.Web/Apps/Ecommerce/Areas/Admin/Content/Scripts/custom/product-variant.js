$(function () {
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
                        sku: function () {
                            return $("#SKU").val();
                        },
                        id: function () {
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
        highlight: function (label) {
            $(label).closest('.span12').addClass('error');
        },
        success: function (label) {
            label
                .addClass('valid')
                .closest('.span12').addClass('success');
        }
    });
    if ($("#BasePrice").val() === "0") {
        $("#BasePrice").val("");
    }
    $('input[type="text"][name^="AttributeValues"]').each(function(index,element) {
        $(element).rules("add", {
            required: true
        });
    });
   
})

