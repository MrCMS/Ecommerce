$(document).ready(function () {
    
    //CSS
    $("body").css("background-color", "#F2F2F2");
    $(".left-nav").css("display", "none");
    $(".container-mrcms").css("padding-left", "0");

});

//CHARTS
google.load('visualization', '1.0', { 'packages': ['corechart'] });
google.setOnLoadCallback(generateOrdersChart);
google.setOnLoadCallback(generateProductsChart);
window.onresize = function () {
    generateOrdersChart();
    generateProductsChart();
};
var options = {
    'width': '100%',
    'legend': 'none',
    'titlePosition': 'none',
    'legend.position': 'none',
    'colors': ['#3498db', '#e74c3c', '#8e44ad', '#f39c12', '#27ae60'],
    'chartArea': { left: 30, top: 20, width: "95%", height: "75%" },
    'hAxis.gridlines.count': 2,
    'vAxis.gridlines.count': 2
};

function generateOrdersChart() {
    $.post('/Admin/Apps/Amazon/App/Revenue', {
        from: $("#FilterFrom").val(),
        to: $("#FilterUntil").val()
    }, function (result) {
        var data = new google.visualization.DataTable();
        data.addColumn('string', 'Days');
        data.addColumn('number', '');
        $.each(result.Data, function (index, value) {
            data.addRow([index, value]);
        });
        var chart = new google.visualization.LineChart(document.getElementById('orders'));
        chart.draw(data, options);
    });
};

function generateProductsChart() {
    $.post('/Admin/Apps/Amazon/App/ProductsSold', {
        from: $("#FilterFrom").val(),
        to: $("#FilterUntil").val()
    }, function (result) {
        var data = new google.visualization.DataTable();
        data.addColumn('string', 'Days');
        data.addColumn('number', '');
        $.each(result.Data, function (index, value) {
            data.addRow([index, value]);
        });
        var chart = new google.visualization.LineChart(document.getElementById('products'));
        chart.draw(data, options);
    });
};