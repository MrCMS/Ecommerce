$(function(){
    $('.dropdown-toggle').dropdownHover();
    $("#searchTerm").keypress(function (e) {
        if (e.which == 13) {
            e.preventDefault();
            window.location = $("#ProductSearchUrl").val() +"?searchTerm="+ $("#searchTerm").val();
        }
    });
    $("#searchButton").click(function(){
        window.location = $("#ProductSearchUrl").val() +"?searchTerm="+ $("#searchTerm").val();
    });

    $('#checkout').wizard()
})