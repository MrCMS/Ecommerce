$(function() {
    $('.newsletter-ck').each(function(i, el) {
        CKEDITOR.replace(el, {
            customConfig: '/Apps/Ecommerce/Areas/Admin/Content/Scripts/custom/newsletter-config.js'
        });
    });
});