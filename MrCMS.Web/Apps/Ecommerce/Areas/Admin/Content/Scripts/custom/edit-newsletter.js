var EditNewsletter = function () {

    var client;
    return {
        init: function () {
            client = new ZeroClipboard($("#copy-link"), {
                moviePath: "/Apps/Ecommerce/Areas/Admin/Content/Scripts/lib/zero-clipboard/ZeroClipboard.swf"
            });
            client.on('dataRequested', function (client, args) {
                $.get($("#copy-link").data('preview-url'), function (data) {
                    client.setText(data);
                    $('#data-copied-notice').show();
                });
            });
        }
    };
};
$(function () {
    new EditNewsletter().init();
});