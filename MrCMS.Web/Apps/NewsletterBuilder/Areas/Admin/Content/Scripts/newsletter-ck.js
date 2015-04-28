$(function() {
    $('.newsletter-ck').each(function(i, el) {
        CKEDITOR.replace(el, {
            customConfig: '/Apps/NewsletterBuilder/Areas/Admin/Content/Scripts/newsletter-config.js'
        });
    });
});