$(function () {
    //FILTER PRODUCTS
    $('#Category').change(function () {
        $('form#Filter').submit();
    });

    //UPDATE GOOGLE BASE PRODUCT
    $(document).on('click', 'button.update-google-base-product', function () {
        $.post('/Admin/Apps/Ecommerce/ImportExport/UpdateGoogleBaseProduct',
                $(this).parents('form').serialize(),
            function (response) {
                if (response === true) {
                    location.reload();
                } else {
                    alert("Error happened during update operation. Please check your parameters and try again.");
                }
            });
        return false;
    });
})