$(function(){
    $('.dropdown-toggle').dropdownHover();
    $("#searchTerm").keypress(function (e) {
        if (e.which == 13) {
            e.preventDefault();
            if ($("#searchTerm").val() !== "") {
                window.location = $("#ProductSearchUrl").val() + "?q=" + $("#searchTerm").val();
            }
        }
    });
    $("#searchButton").click(function() {
        if ($("#searchTerm").val() !== "") {
            window.location = $("#ProductSearchUrl").val() + "?q=" + $("#searchTerm").val();
        }
    });
})