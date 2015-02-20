(function ($, google, window) {
    //CHARTS
    google.load('visualization', '1.0', { 'packages': ['corechart'] });
    google.setOnLoadCallback(generateSalesByDayChart);
    $(function () {
        $(window).resize(generateSalesByDayChart);
    });
    var options = {
        'width': '100%',
        'titlePosition': 'top',
        'legend': { position: 'bottom' },
        'colors': ['#3498db', '#e74c3c', '#8e44ad', '#f39c12', '#27ae60'],
        'chartArea': { width: "80%", height: "80%" },
        'vAxis': { gridlines: { color: 'transparent' } }
    };
    var salesByDay = null;

    function generateSalesByDayChart() {
        if (salesByDay) {
            generateChart(salesByDay, 'chart');
        } else {
            $.post('/Admin/Apps/Ecommerce/Report/SalesByShippingType', {
                from: $("#From").val(),
                to: $("#To").val()
            }, function (result) {
                salesByDay = result;
                generateSalesByDayChart();
            });
        }
    };

    function hasKeys(data) {
        for (var key in data) {
            return true;
        }
        return false;
    }

    function generateChart(result, container) {
        var data = new google.visualization.DataTable();
        var col = 0;
        data.insertColumn(col++, 'string', 'Days', 0);

        for (var key in result.MultipleData) {
            data.insertColumn(col++, 'number', key, 1);
        }

        $.each(result.Labels, function (index, value) {
            col = 0;
            data.addRows(1);
            data.setCell(index, 0, value);
            for (var key in result.MultipleData) {
                col++;
                data.setCell(index, col, result.MultipleData[key][index]);
            }
        });

        if (hasKeys(result.MultipleData)) {
            $('#' + container).html('');
            var chart = new google.visualization.ColumnChart(document.getElementById(container));
            chart.draw(data, options);
        } else {
            $('#' + container).html('<br/><p style="text-align:center">Not enough data found for this chart.</p>');
        }
    }

})(jQuery, google, window);
