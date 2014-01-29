$(function() {
    $("#searchTerm").keypress(function(e) {
        if (e.which == 13) {
            e.preventDefault();
            redirectToSearch();
        }
    });
    $("#searchButton").click(function() {
        redirectToSearch();
    });

    function redirectToSearch() {
        if ($("#searchTerm").val()) {
            window.location = "/products?SearchTerm=" + $("#searchTerm").val();
        } else {
            window.location = "/products";
        }
    }

    $('[data-add-to-wishlist]').each(function() {
        return new AddToWishlist($(this)).init();
    });
});


var AddToWishlist = function (el, options) {
    var self,
        element = el,
        id = element.data('add-to-wishlist'),
        settings = $.extend(AddToWishlist.defaults, options);

    var updateWishlist = function (event) {
        event.preventDefault();
        var form = $(event.target);
        $.post(form.attr('action'), refreshUI);
        if (form.data('wishlist-action') == 'remove') {
            $('[data-wishlist-item=' + id + ']').remove();
        }
    };
    var refreshUI = function (success) {
        if (success) {
            $.get(settings.addToWishlistUrl, { id: id }, function (response) {
                el.html($(response).html());
            });
            $.get(settings.summaryUrl, updateSummary);
        }
    };
    var updateSummary = function (response) {
        $(settings.summarySelector).replaceWith(response);
    };

    return {
        init: function () {
            self = this;
            element.on('submit', 'form', updateWishlist);
            return self;
        }
    };
};
AddToWishlist.defaults = {
    addToWishlistUrl: '/Apps/Ecommerce/AddToWishlist',
    summaryUrl: '/Apps/Ecommerce/WishlistSummary',
    summarySelector: '[data-wishlist-summary]'
};