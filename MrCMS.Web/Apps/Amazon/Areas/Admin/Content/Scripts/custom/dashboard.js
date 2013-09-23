$(document).ready(function () {
    
    //CSS
    $("body").css("background-color", "#F2F2F2");
    $(".left-nav").css("display", "none");
    $(".container-mrcms").css("padding-left", "0");

    //CHARTS
    var ordersChart = $("#orders").get(0).getContext("2d");
    var ordersChartContainer = $("#orders").parent();
    var productsChart = $("#products").get(0).getContext("2d");
    var productsChartContainer = $("#products").parent();
    
    var options1 = {
        scaleLineColor: "rgba(0,0,0,.1)",
        scaleLineWidth: 0,
        scaleShowLabels: true,
        scaleLabel: "<%=value%>",
        scaleFontFamily: "'Arial'",
        scaleFontSize: 11,
        scaleFontStyle: "normal",
        scaleFontColor: "rgba(52, 152, 219,1.0)",
        scaleShowGridLines: true,
        scaleGridLineColor: "rgba(0,0,0,.05)",
        scaleGridLineWidth: 0.1,
        bezierCurve: true,
        pointDot: true,
        pointDotRadius: 3,
        pointDotStrokeWidth: 0.1,
        datasetStroke: false,
        datasetStrokeWidth: 2,
        datasetFill: false,
        animation: true,
        animationSteps: 60,
        animationEasing: "easeOutQuart",
        onAnimationComplete: null
    };
    
    var options2 = {
        scaleLineColor: "rgba(0,0,0,.1)",
        scaleLineWidth: 0,
        scaleShowLabels: true,
        scaleLabel: "<%=value%>",
        scaleFontFamily: "'Arial'",
        scaleFontSize: 11,
        scaleFontStyle: "normal",
        scaleFontColor: "rgba(39, 174, 96,1.0)",
        scaleShowGridLines: true,
        scaleGridLineColor: "rgba(0,0,0,.05)",
        scaleGridLineWidth: 0.1,
        bezierCurve: true,
        pointDot: true,
        pointDotRadius: 3,
        pointDotStrokeWidth: 0.1,
        datasetStroke: true,
        datasetStrokeWidth: 2,
        datasetFill: false,
        animation: true,
        animationSteps: 60,
        animationEasing: "easeOutQuart",
        onAnimationComplete: null
    };

    var ordersData = {
        labels: ["1", "2", "3", "4", "5", "6", "7"],
        datasets: [
            {
                fillColor: "rgba(52, 152, 219,0.5)",
                strokeColor: "rgba(52, 152, 219,1.0)",
                pointColor: "rgba(52, 152, 219,1.0)",
                pointStrokeColor: "rgba(52, 152, 219,1.0)",
                data: [1, 2, 3, 4, 5, 6, 7]
            }
        ]
    };
    
    var productsData = {
        labels: ["1", "2", "3", "4", "5", "6", "7"],
        datasets: [
            {
                fillColor: "rgba(39, 174, 96,0.5)",
                strokeColor: "rgba(39, 174, 96,1.0)",
                pointColor: "rgba(39, 174, 96,1.0)",
                pointStrokeColor: "rgba(39, 174, 96,1.0)",
                data: [1, 2, 3, 4, 5, 6, 7]
            }
        ]
    };

    function generateOrdersChart() {
        $.post('/Admin/Apps/Amazon/App/Revenue', {
            from: $("#FilterFrom").val(),
            to: $("#FilterUntil").val()
        }, function (result) {
            ordersData.labels = result.Labels;
            ordersData.datasets[0].data = result.Data;
            var nc = $("#orders").attr('width', $(ordersChartContainer).width());
            new Chart(ordersChart).Line(ordersData, options1);
        });
    };
    
    function generateProductsChart() {
        $.post('/Admin/Apps/Amazon/App/ProductsSold', {
            from: $("#FilterFrom").val(),
            to: $("#FilterUntil").val()
        }, function (result) {
            productsData.labels = result.Labels;
            productsData.datasets[0].data = result.Data;
            var nc = $("#products").attr('width', $(productsChartContainer).width());
            new Chart(productsChart).Line(productsData, options2);
        });
    };
    
    $(window).resize(updateCharts);

    function updateCharts() {
        generateOrdersChart();
        generateProductsChart();
    };

    generateOrdersChart();
    generateProductsChart();
});
