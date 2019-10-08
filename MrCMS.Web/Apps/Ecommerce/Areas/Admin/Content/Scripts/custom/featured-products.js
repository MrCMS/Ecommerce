$(function () {
    var delay = (function(){
        var timer = 0;
        return function(callback, ms){
            clearTimeout (timer);
            timer = setTimeout(callback, ms);
        };
    })();

    $('#searchparam').keyup(function (key) {

        delay(function(){
            
            var term = $('#searchparam').val();
            if (term !== "" && term.length >= 2) {
                $.getJSON('/Admin/Apps/Ecommerce/Product/SearchProducts',
                    { term: term },
                    function (response) {
                        $("table").empty();
                        $.each(response,
                            function(key, val) {
                                $("table").append("<tr><td>" +
                                    val["Name"] +
                                    "</td><td><div class=\"pull-right\"><button data-product-id=\"" +
                                    val["ProductID"] +
                                    "\" data-product-name=\"" +
                                    val["Name"] +
                                    "\" class=\"btn btn-success add-product\">Add</button></div></td></tr>");
                            });
                    });
            }
            else
            {
                $("table").empty();
            }

        }, 300 );
        
    });
    $(document).on('click', '#products .add-product', function () {
        var button = $(this);
        var productId = button.data('product-id');
        var productName = button.data('product-name');
        $("#ListOfFeaturedProducts").tagit("createTag", productId + "/" + productName);
        return false;
    });
    $("#ListOfFeaturedProducts").tagit();
})

