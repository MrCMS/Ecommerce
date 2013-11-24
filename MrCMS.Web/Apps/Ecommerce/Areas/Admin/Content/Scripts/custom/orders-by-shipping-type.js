//CHARTS
google.load('visualization', '1.0', { 'packages': ['corechart'] });
google.setOnLoadCallback(generateChart);
window.onresize = function () {
    generateChart();
};
var options = {
    'width': '100%',
    'titlePosition': 'top',
    'legend': { position: 'bottom' },
    'colors': ['#3498db', '#e74c3c', '#8e44ad', '#f39c12', '#27ae60'],
    'chartArea': { left: 30, top: 20, width: "95%", height: "55%" },
    'vAxis': { gridlines: { color: 'transparent' } },
};

function generateChart() {
    $.post('/Admin/Apps/Ecommerce/Report/OrdersByShippingType', {
        from: $("#From").val(),
        to: $("#To").val()
    }, function (result) {
        var data = new google.visualization.DataTable();
        var col = 0;
        data.insertColumn(col, 'string', 'Days', 0);
        var m, a, e;
        if (result.MultipleData[0] != null && result.MultipleData[0].length > 0) {
            col += 1;
            data.insertColumn(col, 'number', 'MrCMS', 1);
            m = true;
        }
        if (result.MultipleData[1] != null && result.MultipleData[1].length > 0) {
            col += 1;
            data.insertColumn(col, 'number', 'Amazon', 2);
            a = true;
        }
        if (result.MultipleData[2] != null && result.MultipleData[2].length > 0) {
            col += 1;
            data.insertColumn(col, 'number', 'eBay', 3);
            e = true;
        }

        $.each(result.Labels, function (index, value) {
            var col = 0;
            data.addRows(1);
            data.setCell(index, 0, value);
            if (m == true) {
                col += 1;
                data.setCell(index, col, result.MultipleData[0][index]);
            }
            if (a == true) {
                col += 1;
                data.setCell(index, col, result.MultipleData[1][index]);
            }
            if (e == true) {
                col += 1;
                data.setCell(index, col, result.MultipleData[2][index]);
            }
        });

        if (m != true && a != true && e != true) {
            $('#chart').html('<br/><p>No results were found with current filters, please refine and try again.</p>');
        } else {
            $('#chart').html('');
            var chart = new google.visualization.ColumnChart(document.getElementById('chart'));
            chart.draw(data, options);
        }
    });
}