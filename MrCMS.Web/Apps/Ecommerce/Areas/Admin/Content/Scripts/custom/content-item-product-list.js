$(function () {
    $('#searchparam').keyup(function (key) {
        var term = $(this).val();
        if (term !== "") {
            $.getJSON('/Admin/Apps/Ecommerce/Product/SearchProducts',
            { term: term },
            function (response) {
                $("table").empty();
                $.each(response, function (key, val) {
                    $("table").append("<tr><td>" + val["Name"] + "</td><td><div class=\"pull-right\"><button data-product-id=\"" + val["ProductID"] + "\" data-product-name=\"" + val["Name"] + "\" class=\"btn btn-success add-product\">Add</button></div></td></tr>");
                });
            });
        }
        else {
            $("table").empty();
        }
    });
    $(document).on('click', '#products2 .add-product', function () {
        var button = $(this);
        var productId = button.data('product-id');
        var productName = button.data('product-name');
        $("#Products").tagit("createTag", productId + "/" + productName);
        return false;
    });
    $("#Products").tagit();
})

