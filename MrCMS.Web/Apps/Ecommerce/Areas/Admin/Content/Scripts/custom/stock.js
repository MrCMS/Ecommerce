$(function () {
    //UPDATE PRODUCT VARIANT STOCK
    $(document).on('click', 'button.update-stock', function () {
        $.post('/Admin/Apps/Ecommerce/Stock/UpdateStock',
                $(this).parents('form').serialize(),
            function (response) {
                if (response !== false) {
                    parent.$.get('/Admin/Apps/Ecommerce/Stock/LowStockReportProductVariants/', {
                        threshold: $("#threshold").val(),
                        page: $("#page").val()
                        } ,function (result) {
                        parent.$('#product-variants').replaceWith(result);
                    });
                } else {
                    alert("Error happened during update operation. Please check your parameters and try again.");
                }
            });
        return false;
    });
})