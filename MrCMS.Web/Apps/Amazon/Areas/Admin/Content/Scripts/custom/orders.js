var taskUrl = "/Admin/Apps/Amazon/Orders/Sync";

$(document).ready(function () {

    $(document).on('click', '#pb-start-task', function () {
        $("#results").html("");
        $("#pb-start-task").hide();
        sync();
    });

});

function sync() {
    var description = $("#Description").val();
    $.ajax({
        url: taskUrl,
        type: "POST",
        cache: false,
        data: {
            description: description
        },
        dataType: "json",
        success: function (data) {
            $("#pb-start-task").show();
            if (data.ErrorMessage != "" && data.ErrorMessage !=null) {
                $("#Description").val("");
                $("#results").append(data.ErrorMessage);
            } else {
                updateResults(data);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
    return false;
}

function updateResults(data) {
    if (data.OrdersUpdated.length > 0) {
        $("#results").append("<strong>Orders scheduled for sync:</strong><br/><ul></ul>");
        $.each(data.OrdersUpdated, function (index, value) {
            $("#results ul").append("<li>" + value + "</li>");
        });
    } else {
        $("#results").append("<strong>No orders were scheduled for sync.</strong>");
    }
}