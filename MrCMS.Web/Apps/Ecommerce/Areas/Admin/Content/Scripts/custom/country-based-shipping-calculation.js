(function ($) {
    var includedTable, notIncludedTable;
    function sortTable(table) {
        var rows = table.find('tbody > tr');
        rows.sort(function (a, b) {

            var keyA = $(a).find('[data-country-name]').text();
            var keyB = $(b).find('[data-country-name]').text();

            return (keyA > keyB) ? 1 : (keyA < keyB) ? -1 : 0;
        });
        rows.each(function (index, row) {
            table.append(row);                  // append rows after sort
        });
    }
    function setValuesInTables() {
        includedTable.find('[data-country-value]').each(function(index, element) {
            $(element).val('true');
        });
        notIncludedTable.find('[data-country-value]').each(function(index, element) {
            $(element).val('false');
        });
    }
    function moveCountries(from, to) {
        var checked = from.find('[data-country-select]:checked');
        checked.each(function (index, element) {
            var checkbox = $(element);
            var row = checkbox.parents('tr');
            row.appendTo(to.find('tbody'));
            checkbox.removeAttr('checked');
        });
        sortTable(to);
        setValuesInTables();
    }
    function removeSelectedCountries(event) {
        moveCountries(includedTable, notIncludedTable);
    }

    function addSelectedCountries(event) {
        moveCountries(notIncludedTable, includedTable);
    }

    $(function () {
        includedTable = $('[data-country-included]');
        notIncludedTable = $('[data-country-not-included]');

        $(document).on('click', '[data-add-selected-countries]', addSelectedCountries);
        $(document).on('click', '[data-remove-selected-countries]', removeSelectedCountries);
    });

})(jQuery);