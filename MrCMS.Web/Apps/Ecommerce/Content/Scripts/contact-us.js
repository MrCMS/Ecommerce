function initialize() {
    $.get("/get-contact-map", function (response) {
        var mapOptions = {
            center: new google.maps.LatLng(response.Latitude, response.Longitude),
            zoom: 10
        };

        var map = new google.maps.Map(document.getElementById("map-canvas"),
            mapOptions);

        var image = {
            url: response.MapPinIcon,
            size: new google.maps.Size(270, 106),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(135, 106)
        };

        var myLatLng = new google.maps.LatLng(response.Latitude, response.Longitude);

        var marker = new google.maps.Marker({
            position: myLatLng,
            map: map,
            icon: image
        });

        setBounds(response, map);
        google.maps.event.trigger(map, "resize");
    });


}
google.maps.event.addDomListener(window, 'load', initialize);

function setBounds(response, map) {
    var dataPoints = response.Data;
    if (dataPoints.length > 0) {
        if (dataPoints.length == 1) {
            var point = dataPoints[0];
            map.setCenter(new google.maps.LatLng(point['Latitude'], point['Longitude']));
            map.setZoom(11);
        } else {
            var maxLat = response.MaxLatitude,
            minLat = response.MinLatitude,
            maxLong = response.MaxLongitude,
            minLong = response.MinLongitude;

            var swLatLng = new google.maps.LatLng(minLat, minLong),
            neLatLng = new google.maps.LatLng(maxLat, maxLong);

            var mapBounds = new google.maps.LatLngBounds(swLatLng, neLatLng);

            map.fitBounds(mapBounds);
        }
    }
}