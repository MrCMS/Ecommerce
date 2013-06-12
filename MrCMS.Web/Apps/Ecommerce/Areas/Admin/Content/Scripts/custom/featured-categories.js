$(function () {
    $('#searchparam').keyup(function (key) {
        var term = $(this).val();
        if (term !== "") {
            $.getJSON('/Admin/Apps/Ecommerce/Category/SearchCategories',
            { term: term },
            function (response) {
                $("table").empty();
                $.each(response, function(key,val) {
                    $("table").append("<tr><td>" + val["Name"] + "</td><td><div class=\"pull-right\"><button data-category-id=\"" + val["CategoryID"] + "\" data-category-name=\"" + val["Name"] + "\" class=\"btn btn-success add-category\">Add</button></div></td></tr>");
                })
            });
        }
        else
        {
            $("table").empty();
        }
    });
    $(document).on('click', '#categories .add-category', function () {
        var button = $(this);
        var categoryId = button.data('category-id');
        var categoryName = button.data('category-name');
        $("#ListOfFeaturedCategories").tagit("createTag", categoryId + "/" + categoryName);
        return false;
    });
    $("#ListOfFeaturedCategories").tagit();
})

