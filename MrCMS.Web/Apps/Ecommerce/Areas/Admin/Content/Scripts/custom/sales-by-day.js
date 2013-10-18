$(document).ready(function () {

    $(window).resize(updateCharts);
    
    function updateCharts() {
        generateChart();
    };

    generateChart();
});

var chartData = {
    labels: [],
    datasets: [
        {
            fillColor: "rgba(52, 152, 219,0.5)",
            strokeColor: "rgba(52, 152, 219,1.0)",
            pointColor: "rgba(52, 152, 219,1.0)",
            pointStrokeColor: "rgba(52, 152, 219,1.0)",
            data: []
        },
        {
            fillColor: "rgba(43, 191, 105,0.5)",
            strokeColor: "rgba(43, 191, 105,1.0)",
            pointColor: "rgba(43, 191, 105,1.0)",
            pointStrokeColor: "rgba(43, 191, 105,1.0)",
            data: []
        },
        {
            fillColor: "rgba(231, 76, 60,0.5)",
            strokeColor: "rgba(231, 76, 60,1.0)",
            pointColor: "rgba(231, 76, 60,1.0)",
            pointStrokeColor: "rgba(231, 76, 60,1.0)",
            data: []
        }
    ]
};

var dataUrl = "/Admin/Apps/Ecommerce/Report/SalesByDay";
var chart = $("#line-chart").get(0).getContext("2d");
var chartContainer = $("#line-chart").parent();

function generateChart() {
    $.post(dataUrl, {
        from: $("#From").val(),
        to: $("#To").val()
    }, function (result) {
        chartData.labels = result.ChartLabels;
        var anyResults = false;
        $.each(result.MultiChartData, function (index, value) {
            if (value.length > 0) {
                chartData.datasets[index].data = value;
                anyResults = true;
            }
        });
        if (chartData.datasets[1].data.length > 0)
            $("#amazon").show();
        else {
            $("#amazon").hide();
        }
        if (chartData.datasets[2].data.length > 0)
            $("#ebay").show();
        else {
            $("#ebay").hide();
        }
        if (anyResults === false) {
            $("#message").html("No results were found with current filters, please refine and try again.");
        } else
            $("#message").html("");
        var nc = $("#line-chart").attr('width', $(chartContainer).width());
        new Chart(chart).Line(chartData, lineChartOptions);
    });
};