$(function () {
    /*$('.dropdown-toggle').dropdownHover();*/

    $("#searchTerm").keypress(function (e) {
        if (e.which == 13) {
            e.preventDefault();
            redirectToSearch();
        }
    });
    $("#searchButton").click(function () {
        redirectToSearch();
    });
    function redirectToSearch() {
        if ($("#searchTerm").val()) {
            window.location = "/products?SearchTerm=" + $("#searchTerm").val();
        } else {
            window.location = "/products";
        }
    }
})