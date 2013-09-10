$(document).ready(function () {

    //SYNC CATEGORIES

    $(document).on('click', '#sync-categories-manually', function () {
        $("#sync-categories-manually").hide();
        $("#sync-categories-progress-bar").show();
        updateProgressBarStateForSyncCategories();
        $.post('/Admin/Apps/Amazon/App/SyncCategories', function (data) {
            $("#sync-categories-progress-bar").hide();
            $("#sync-categories-manually").show();
        });
    });

    function updateProgressBarStateForSyncCategories() {
        $.ajax({
            url: "/Admin/Apps/Amazon/App/SyncCategoriesStatus",
            type: "POST",
            cache: true,
            dataType: "json",
            success: function (data) {
                var percentComplete = parseInt(data.PercentComplete);
                if (data.CurrentStatus === "Category sync is not necessary.") {
                    $("#sync-categories-progress-bar").hide();
                    setStatusForSyncCategories(data.CurrentStatus, "alert-info");
                } else if (data.CurrentStatus === "Error") {
                    setStatusForSyncCategories("Error happened during category sync. Please check Sync Logs for more details.", "alert-error");
                } else {
                    if (percentComplete == null || percentComplete == 100) {
                        $("#sync-categories-progress-bar .progress").removeClass("active");
                        $("#sync-categories-progress-bar .progress .bar").css("width", "100%");
                        setStatusForSyncCategories(data.CurrentStatus, "alert-success");
                    } else {
                        $("#sync-categories-progress-bar .progress").addClass("active");
                        $("#sync-categories-progress-bar .progress .bar").css("width", percentComplete + "%");
                        setTimeout(updateProgressBarStateForSyncCategories, 1000);
                        setStatusForSyncCategories(data.CurrentStatus, "alert-info");

                    }
                }
            }
        });
    }

    function setStatusForSyncCategories(value, statusTypeCssClass) {
        $("#sync-categories-status").show();
        $("#sync-categories-status").removeClass("alert-error");
        $("#sync-categories-status").removeClass("alert-success");
        $("#sync-categories-status").removeClass("alert-info");

        $("#sync-categories-status").html(value);
        if (statusTypeCssClass != "") {
            $("#sync-categories-status").addClass(statusTypeCssClass);
        }
    }
});