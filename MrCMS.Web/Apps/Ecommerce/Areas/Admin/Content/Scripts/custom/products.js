window.locale = {
    "fileupload": {
        "errors": {
            "maxFileSize": "File is too big",
            "minFileSize": "File is too small",
            "acceptFileTypes": "Filetype not allowed",
            "maxNumberOfFiles": "Max number of files exceeded",
            "uploadedBytes": "Uploaded bytes exceed file size",
            "emptyResult": "Empty file upload result"
        },
        "error": "Error",
        "start": "Start",
        "cancel": "Cancel",
        "destroy": "Delete"
    }
};

$(function () {
    'use strict';


    // Initialize  the jQuery File Upload widget:
    $('#fileupload').fileupload();

    // Load existing files:
    $('#fileupload').each(function () {
        var that = this;
        $.getJSON(this.action,{v: new Date().getTime()}, function (result) {
            if (result && result.length) {
                var fileupload = $(that).fileupload('option', 'done');
                fileupload.call(that, null, { result: result });
            }
        });
    });
})