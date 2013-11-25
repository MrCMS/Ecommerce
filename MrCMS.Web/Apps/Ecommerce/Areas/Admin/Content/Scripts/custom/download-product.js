
$(document).ready(function () {

    $('#IsDownloadable').click(function () {
        $('#DownloadFileUrl').val('');
        $('#DemoFileUrl').val('');
        $('#AllowedNumberOfDownloads').val('0');
        $('#AllowedNumberOfDaysForDownload').val('0');
        $("#downloadable").toggle(this.checked);
    });

});
