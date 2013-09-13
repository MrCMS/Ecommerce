$(document).ready(function () {
    
    //INDEX
    $('.search-listings').keypress(function (e) {
        if (e.which == 13) {
            e.preventDefault();
            searchListings();
        }
    });
    
    $(document).on('click', '#search-listings', function () {
        searchListings();
    });
    
    function searchListings() {
        $.get('/Admin/Apps/Amazon/Listing/Listings', {
            listingTitle: $("#ListingTitle").val()
        }, function (result) {
            $('#listings').replaceWith(result);
        });
    }

    //ADD
    $('.search-products').keypress(function(e) {
        if (e.which == 13) {
            e.preventDefault();
            searchProducts();
        }
    });

    $(document).on('click', '#search-products', function() {
        searchProducts();
    });

    function searchProducts() {
        $.get('/Admin/Apps/Amazon/Listing/Products', {
            name: $("#Name").val()
        }, function (result) {
            $('#product-search').replaceWith(result);
        });
    }
    
    //ADD LISTING ITEMS
    $('.search-categories').keypress(function (e) {
        if (e.which == 13) {
            e.preventDefault();
            searchCategories();
        }
    });
    
    $(document).on('click', '#search-categories', function () {
        searchCategories();
    });
    
    function searchCategories() {
        $.get('/Admin/Apps/Amazon/Listing/Categories', {
            name: $("#Name").val()
        }, function (result) {
            $('#categories').replaceWith(result);
        });
    }
    
    $(document).on('click', '.choose-category', function () {
        var button = $(this),
            categoryId = button.data('category-id'),
            categoryPath = button.data('category-path');
        
        $('#ChosenCategory_Id').val(categoryId);
        $('#category').html("Your current choice: "+categoryPath);
        
        $.post('/Admin/Apps/Amazon/Listing/CheckIfCategoryAllowsVariations/',
            { id: categoryId },
            function (response) {
                if (response === false) {
                    var dialogResponse = confirm("Chosen category does not support listings with multiple variations.\n\n'Cancel' if you want to go back " +
                        "and choose another category for this product.\n\n'OK' to publish each product variant as separate Amazon listing item.");
                    if (dialogResponse === true) {
                        $('#MultipleVariations').val(false);
                        $('form#add-listing-form').submit();
                    }
                } 
                $('#MultipleVariations').val(true);
            });
        
        return false;
    });
});