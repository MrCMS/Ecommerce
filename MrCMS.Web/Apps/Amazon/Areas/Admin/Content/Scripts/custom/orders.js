var taskUrl = "/Admin/Apps/Amazon/Orders/Sync";

$(document).ready(function () {

    $(document).on('click', '#pb-start-task', function () {
        $("#results").html("");
        var description = $("#Description").val();
        $("#pb-start-task").hide();
        $.post(taskUrl, {
            description: description
        }, function (data) {
            $("#pb-start-task").show();
            if (data.ErrorMessage != "") {
                $("#Description").val("");
                alert(data.ErrorMessage);
            } else {
                $("#results").append("<strong>Updated/Imported Orders:</strong><br/>");
                $("#results").append("<ul>");
                data.OrdersUpdated.each(function (index, value) {
                    $("#results").append("<li>" + value.AmazonOrderId + "</li>");
                });
                $("#results").append("</ul>");
            }
        });
    });

});