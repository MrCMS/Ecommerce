$(function () {
    //FILTER PRODUCTS
    $('#Category').change(function () {
        $('form#Filter').submit();
    });
    
    //UPDATE GOOGLE BASE PRODUCT
    $(document).on('click', 'button[type="button"][id^="update-"]', function () {
        var pv = $(this).data("product-variant-id");
        $.post('/Admin/Apps/Ecommerce/ImportExport/UpdateGoogleBaseProduct',
                $('form#GoogleBaseProductForm'+pv).serialize()
            ,
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