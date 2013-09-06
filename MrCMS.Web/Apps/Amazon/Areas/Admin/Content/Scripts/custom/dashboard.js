$(document).ready(function () {
    
    //CSS
    $("body").css("background-color", "#F2F2F2");
    $(".left-nav").css("display", "none");
    $(".container-mrcms").css("padding-left", "0");

    //CHARTS
    var ctx = $("#myChart").get(0).getContext("2d");
    var container = $("#myChart").parent();
    
    $(window).resize(generateChart);
    
    var options = {
        scaleLineColor: "rgba(0,0,0,.1)",
        scaleLineWidth: 0,
        scaleShowLabels: false,
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

    var data = {
        labels: ["1", "2", "3", "4", "5", "6", "7"],
        datasets: [
            {
                fillColor: "rgba(52, 152, 219,0.5)",
                strokeColor: "rgba(52, 152, 219,1.0)",
                pointColor: "rgba(52, 152, 219,1.0)",
                pointStrokeColor: "rgba(52, 152, 219,1.0)",
                data: [65, 59, 90, 81, 56, 55, 40]
            }
        ]
    };

    function generateChart() {
        var nc = $("#myChart").attr('width', $(container).width());
        new Chart(ctx).Line(data, options);
    };

    generateChart();
});
