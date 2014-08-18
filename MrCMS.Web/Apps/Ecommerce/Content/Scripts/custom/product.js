$(function () {
    Product.init();
});

var Product = new function () {
    function resetValidation() {
        var form = $('form');
        form.removeData("validator");
        form.removeData("unobtrusiveValidation");
        form.find('input, select').each(function () {
            $.data(this, "previousValue", null);
        });
        $.validator.unobtrusive.parse("form");
    }
    this.History = window.History;
    this.setVariant = function (variantId) {
        Product.History.pushState({ variant: variantId }, $('title').html(), location.pathname + '?variant=' + variantId);
    };
    this.init = function () {
        $(document).on('change', '#variant', function () {
            Product.setVariant($('#variant').val());
        });

        // Bind to StateChange Event
        Product.History.Adapter.bind(window, 'statechange', function () { // Note: We are using statechange instead of popstate
            var state = History.getState();
            $.get('/product/variant-details/' + state.data.variant, function (response) {
                $('#variant-details').replaceWith(response);
                Product.onChangeVariant();
                resetValidation();
            });
        });
    };

    this.onChangeVariant = function () {
    };
};
$(function () {
    $('#readFullDescription a[href*=#]:not([href=#])').click(function () {
        if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') && location.hostname == this.hostname) {
            var target = $(this.hash);
            target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
            if (target.length) {
                $('html,body').animate({
                    scrollTop: target.offset().top
                }, 1000);
                return false;
            }
        }
    });
});