$(".left-nav").css("display", "none");
$(".container-mrcms").css("padding-left", "0");

var lineChartOptions = {
    scaleLineColor: "rgba(0,0,0,.1)",
    scaleLineWidth: 0,
    scaleShowLabels: true,
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
    pointDotRadius: 2,
    pointDotStrokeWidth: 0.1,
    datasetStroke: false,
    datasetStrokeWidth: 2,
    datasetFill: true,
    animation: true,
    animationSteps: 60,
    animationEasing: "easeOutQuart",
    onAnimationComplete: null
};

pieChartOptions = {
    segmentShowStroke: true,
    segmentStrokeColor: "#fff",
    segmentStrokeWidth: 2,
    animation: true,
    animationSteps: 100,
    animationEasing: "easeOutBounce",
    animateRotate: true,
    animateScale: false,
    onAnimationComplete: null
};

Array.prototype.clear = function () {
    while (this.length > 0) {
        this.pop();
    }
};