$('#map_canvas').gmap({ 'center': '51.482577,-0.2003', 'zoom': 15 }).bind('init', function (ev, map) {

    var image = new google.maps.MarkerImage('/Themes/Ryness/Content/Images/marker.png',new google.maps.Size(51, 79),new google.maps.Point(0,0),new google.maps.Point(25, 79));
    var address = "<strong>Ryness Lighting &amp; Electrical</strong><br/>413 North End Road<br/>Fulham<br/>London<br/>SW6 1NS";

    $('#map_canvas').gmap('addMarker', { 'position': '51.482577,-0.2003', 'bounds': false, 'icon': image }).click(function () {
        $('#map_canvas').gmap('openInfoWindow', {'content': address}, this);
    });
});