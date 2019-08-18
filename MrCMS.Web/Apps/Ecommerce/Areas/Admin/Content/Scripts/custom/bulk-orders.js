$.ajaxSetup({ cache: false });
var orders = [];
(function ($, window, document) {
    window.openModal = function (href) {
        var link = $("<a>");
        link.attr('href', href);
        link.attr('data-toggle', 'fb-modal');
        link.featherlight(MrCMSFeatherlightSettings).click();
    }
    window.deselectSelectAllCheckbox = function () {
        if ($.inArray("selectAll", orders) !== -1) {
            orders.splice($.inArray("selectAll", orders), 1);
        }
    }
    function markOrdersAsShippedForm() {
        var page = $("#Page").val();
        var href = "/Admin/BulkOrdersAction/MarkOrdersAsShippedForm?orders=" + orders + "&page=" + page;
        openModal(href);
    }
    function createInvoices() {
        var ajax = new XMLHttpRequest();
        ajax.open("GET", "/Admin/BulkOrdersAction/CreateInvoices?orders=" + orders, true);
        ajax.responseType = "blob";
        ajax.onreadystatechange = function () {
            if (this.readyState === 4) {
                var blob = new Blob([this.response], { type: "application/octet-stream" });
                var fileName = "Orders.zip";
                saveAs(blob, fileName);
            }
        };
        ajax.send();
    }
    function createPickingList() {
        var page = $("#Page").val();
        var href = "/Admin/BulkOrdersAction/CreatePickingList?orders=" + orders + "&page=" + page;
        window.open(href,'_blank');
    }
    $(function () {
        $(".bullk-action-button").attr("disabled", "disabled");
        $('#selectAll').click(function (e) {
            var table = $(e.target).closest('table');
            $('td input:checkbox', table).prop('checked', this.checked);
            if (this.checked) {
                orders = [];
                $("table tr").addClass("selected-row");
                $("input:checkbox:checked").each(function() {
                    orders.push($(this).val());
                });
            } else {
                orders = [];
                $("table tr").removeClass("selected-row");
            }
        });
        //css styling for selected row
        $("input:checkbox").change(function () {
            var currentValue = $(this).val();
            if (this.checked) {
                $('.bullk-action-button').prop("disabled", false);
                $(this).closest('tr').addClass('selected-row');
                if ($.inArray(currentValue, orders) === -1) {
                    orders.push(currentValue);
                }
            } else {
                if (orders.length === 1 || orders.length === 0) {
                    $(".bullk-action-button").attr("disabled", "disabled");
                }
                if ($.inArray(currentValue, orders) !== -1) {
                    $(this).closest('tr').removeClass('selected-row');
                    orders.splice($.inArray(currentValue, orders), 1);
                }
            }
        });
        //download invoices
        $("[data-download-invoices]").on("click",
            function () {
                deselectSelectAllCheckbox();
                createInvoices();
            });
        //mark order as shipped
        $("[data-mark-as-shipped]").on("click",
            function () {
                deselectSelectAllCheckbox();
                markOrdersAsShippedForm();
            });
        //create picking list
        $("[data-picking-list]").on("click",
            function () {
                deselectSelectAllCheckbox();
                createPickingList();
            });
    });
})(jQuery, window, document);

