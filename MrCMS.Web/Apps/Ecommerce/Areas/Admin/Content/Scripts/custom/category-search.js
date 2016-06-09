$(function () {
    reTagIt();

    function reTagIt() {
        var namesInput = $("#CategoryNames");
        if (namesInput !== undefined && namesInput.val() != undefined) {
            var tags = namesInput.val().split(',');
            for (var i = 0; i < tags.length; i++) {
                namesInput.tagit().tagit("createTag", tags[i]);
            }
        }
    }

    $(document).on('keyup', '#categorysearchparam', function () {
        var term = $(this).val();
        if (term !== "") {
            $.get('/Admin/ItemIsInCategory/SearchCategories/', {
                page: 1,
                query: term
                }, function (response) {
                    $('.modal-body-container').html(response);
                });
        }
    });

    $(document).on('click', '#categories .add-category', function () {
        var button = $(this);
        var categoryId = button.data('category-id');
        var categoryName = button.data('category-name');
        $("#CategoryNames").tagit().tagit("createTag", categoryName.replace(",", ""));
        $("#CategoryIds").tagit().tagit("createTag", categoryId);
        return false;
    });
})