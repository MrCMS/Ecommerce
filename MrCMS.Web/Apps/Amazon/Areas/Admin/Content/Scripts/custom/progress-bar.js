$(document).ready(function () {

    $(document).on('click', '#pb-start-task', function () {
        $("#pb-start-task").hide();
        $("#pb").show();
        updateProgressBar();
        $.post(taskUrl, { id: taskId }, function (data) {
            return false;
        });
    });

    function updateProgressBar() {
            $.ajax({
                url: getStatusOfTaskUrl+"?id="+taskId,
                type: "GET",
                cache: false,
                dataType: "json",
                success: function(data) {
                    var percentComplete = parseInt(data.PercentComplete);
                    if (data.Status === "Error") {
                        $("#pb-start-task").show();
                        $("#pb").hide();
                        updateProgressBarStatus(data.Status,data.LastMessage);
                    } else {
                        if (percentComplete == null || percentComplete == 100) {
                            $("#pb .progress").removeClass("active");
                            $("#pb .progress .bar").css("width", "100%");
                            updateProgressBarStatus(data.Status, data.LastMessage);
                            $("#pb-start-task").show();
                            $("#pb").hide();
                        } else {
                            $("#pb .progress").addClass("active");
                            $("#pb .progress .bar").css("width", percentComplete + "%");
                            setTimeout(updateProgressBar, 1000);
                            updateProgressBarStatus(data.Status, data.LastMessage);
                        }
                    }
                }
            });
        return false;
    }
    
    function updateProgressBarStatus(status, message) {
        if (message != "") {
            $("#pb-status").show();

            var statusClass = "alert alert-info";
            if (status === "Error")
                statusClass = "alert alert-error";
            if (status === "Completed")
                statusClass = "alert alert-success";

            $("#pb-status").prepend("<div class='" + statusClass + "'>" + message + "</div>");
        }
    }
    
});