$(function () {
    Product.init();
});

var Product = new function() {
    this.History = window.History;
    this.setVariant = function(variantId) {
        Product.History.pushState({ variant: variantId }, $('title').html(), location.pathname + '?variant=' + variantId);
    };
    this.init = function() {
        $(document).on('change', '#variant', function() {
            Product.setVariant($('#variant').val());
        });

        // Bind to StateChange Event
        Product.History.Adapter.bind(window, 'statechange', function() { // Note: We are using statechange instead of popstate
            var State = History.getState();
            $.get('/product/variant-details/' + State.data.variant, function(response) {
                $('#variant-details').replaceWith(response);
                Product.onChangeVariant();
            });
        });
    };

    this.onChangeVariant = function() {
    };
};
