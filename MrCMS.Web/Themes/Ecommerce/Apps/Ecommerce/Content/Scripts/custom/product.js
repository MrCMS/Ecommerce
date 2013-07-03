$(function () {
    $('.product-variants-filter select').live("change", function () {
        productVariantId = $(this).val();
        url = $("#UrlSegment").val();
        window.location="/"+url+"?pv="+productVariantId;
    });

    $(".product-variants-filter select").val($("#Id").val());
})

