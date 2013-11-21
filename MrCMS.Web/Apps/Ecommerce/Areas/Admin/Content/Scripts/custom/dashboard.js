//CHARTS
google.load('visualization', '1.0', { 'packages': ['corechart'] });
google.setOnLoadCallback(generateRevenueTodayChart);
google.setOnLoadCallback(generateRevenueThisWeekChart);
window.onresize = function () {
    generateRevenueTodayChart();
    generateRevenueThisWeekChart();
};
var options = {
    'width': '100%',
    'titlePosition': 'top',
    'legend': { position: 'bottom' },
    'colors': ['#3498db', '#e74c3c', '#8e44ad', '#f39c12', '#27ae60'],
    'chartArea': { left: 30, top: 20, width: "95%", height: "65%" },
    'vAxis': { gridlines: { color: 'transparent' } },
};

function generateRevenueTodayChart() {
    $.post('/Admin/Apps/Ecommerce/Dashboard/RevenueToday', {
    }, function (result) {
        generateChart(result, 'revenue-today');
    });
};

function generateRevenueThisWeekChart() {
    $.post('/Admin/Apps/Ecommerce/Dashboard/RevenueThisWeek', {
    }, function (result) {
        generateChart(result,'revenue-this-week');
    });
};

function generateChart(result,container) {
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
        $('#' + container).html('<br/><p style="text-align:center">Not enough data found for this chart.</p>');
    } else {
        $('#' + container).html('');
        var chart = new google.visualization.LineChart(document.getElementById(container));
        chart.draw(data, options);
    }
}