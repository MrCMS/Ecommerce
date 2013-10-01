$(document).ready(function () {
    
    //INDEX
    $('.search-listing-groups').keypress(function (e) {
        if (e.which == 13) {
            e.preventDefault();
            searchListingGroups();
        }
    });
    
    $(document).on('click', '#search-listing-groups', function () {
        searchListingGroups();
    });
    
    function searchListingGroups() {
        $.get('/Admin/Apps/Amazon/ListingGroup/ListingGroups', {
            name: $("#ListingGroupName").val()
        }, function (result) {
            $('#listing-groups').replaceWith(result);
        });
    }
    
});