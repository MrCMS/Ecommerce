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
    $('.search-product-variants').keypress(function(e) {
        if (e.which == 13) {
            e.preventDefault();
            searchProductVariants();
        }
    });

    $(document).on('click', '#search-product-variants', function() {
        searchProductVariants();
    });

    function searchProductVariants() {
        $.get('/Admin/Apps/Amazon/Listing/ProductVariants', {
            name: $("#Name").val(),
            categoryId: $("#CategoryId").val(),
            "AmazonListingGroup.Id": $("#AmazonListingGroup_Id").val()
        }, function (result) {
            $('#product-variants-search').replaceWith(result);
        });
    }
    
});

//SYNC
var id = $("#Id").val();
var description = $("#Description").val();
var taskId = $("#TaskId").val();
var taskUrl = "/Admin/Apps/Amazon/Listing/Sync";
var from = "";
var to = "";