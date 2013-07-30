$(function () {
    //GOOGLE BASE SETTINGS
    $('#DefaultCategory').change(function () {
        $('form#Settings').submit();
    });
    $('#DefaultCondition').change(function () {
        $('form#Settings').submit();
    });
    
    //FILTER PRODUCTS
    $('#Category').change(function () {
        $('form#Filter').submit();
    });
    
    //UPDATE GOOGLE BASE PRODUCT
    $(document).on('click', 'button[type="submit"][id^="update-"]', function () {
        var button = $(this),
            pv = button.data('product-variant-id'),
            gb = button.data('google-base-product-id'),
            category = $('#override-category-' + pv).val(),
            condition = $('#override-condition-' + pv).val(),
            gender = $('#gender-' + pv).val(),
            agegroup = $('#age-group-' + pv).val(),
            adwordsgrouping = $('#adwords-grouping-' + pv).val(),
            adwordslabels = $('#adwords-labels-' + pv).val(),
            adwordsredirect = $('#adwords-redirect-' + pv).val();
        $.post('/Admin/Apps/Ecommerce/ImportExport/UpdateGoogleBaseRecord',
            {
                Id: pv,
                googleBaseProductId: gb,
                overrideCategory: category,
                overrideCondition: condition,
                grouping: adwordsgrouping,
                labels: adwordslabels,
                redirect: adwordsredirect,
                gender: gender,
                ageGroup: agegroup
            },
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