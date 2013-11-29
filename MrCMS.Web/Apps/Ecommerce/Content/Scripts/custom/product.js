$(function () {
    Product.init();
});

var Product = new function() {
    this.init = function() {
        var History = window.History; // Note: We are using a capital H instead of a lower h
        $(document).on('change', '#variant', function() {
            var data = getData();
            History.pushState(data, $('title').html(), location.pathname + buildUpQueryString(data));
        });

        function getData() {
            return { variant: $('#variant').val() };
        }

        function buildUpQueryString(data) {
            return "?variant=" + data.variant;
        }

        // Bind to StateChange Event
        History.Adapter.bind(window, 'statechange', function() { // Note: We are using statechange instead of popstate
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
