$(document).ready(function () {

    $(window).resize(updateCharts);
    
    function updateCharts() {
        generateRevenueTodayChart();
        generateRevenueThisWeekChart();
    };

    generateRevenueTodayChart();
    generateRevenueThisWeekChart();
});

var lineChartOptions = {
    scaleLineColor: "rgba(0,0,0,.1)",
    scaleLineWidth: 0,
    scaleShowLabels: false,
    scaleLabel: "<%=value%>",
    scaleFontFamily: "'Arial'",
    scaleFontSize: 11,
    scaleFontStyle: "normal",
    scaleFontColor: "rgba(85, 85, 85,1.0)",
    scaleShowGridLines: true,
    scaleGridLineColor: "rgba(0,0,0,.05)",
    scaleGridLineWidth: 0.1,
    bezierCurve: true,
    pointDot: true,
    pointDotRadius: 1,
    pointDotStrokeWidth: 0.2,
    datasetStroke: true,
    datasetStrokeWidth: 1,
    datasetFill: true,
    animation: true,
    animationSteps: 60,
    animationEasing: "easeOutQuart",
    onAnimationComplete: null
};

var chartData1 = {
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

var chartData2 = {
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

function generateRevenueTodayChart() {

    var dataUrl = "/Admin/Apps/Ecommerce/Dashboard/RevenueToday";
    var chart = $("#revenue-today").get(0).getContext("2d");
    var chartContainer = $("#revenue-today").parent();
    
    getChartData("revenue-today", chartData1, dataUrl, chart, chartContainer);
};

function generateRevenueThisWeekChart() {

    var dataUrl = "/Admin/Apps/Ecommerce/Dashboard/RevenueThisWeek";
    var chart = $("#revenue-this-week").get(0).getContext("2d");
    var chartContainer = $("#revenue-this-week").parent();

    getChartData("revenue-this-week", chartData2, dataUrl, chart, chartContainer);
};

function getChartData(chartDiv, chartData, dataUrl, chart, chartContainer) {
   
    $.post(dataUrl, {
    }, function (result) {
        chartData.labels = result.ChartLabels;
        $.each(result.MultiChartData, function (index, value) {
            chartData.datasets[index].data = value;
        });
        if (chartData.datasets[1].data.length > 0)
            $("#amazon").show();
        else if ($("#amazon").is(":hidden")) {
            chartData.datasets.splice(1, 1);
            $("#amazon").hide();
        }
        if (chartData.datasets[2].data.length > 0)
            $("#ebay").show();
        else if ($("#ebay").is(":hidden")) {
            chartData.datasets.splice(2, 1);
            $("#ebay").hide();
        }
        var nc = $("#"+chartDiv).attr('width', $(chartContainer).width());
        new Chart(chart).Line(chartData, lineChartOptions);
    });
}