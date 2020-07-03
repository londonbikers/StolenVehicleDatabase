$(function () {
    google.load("maps", "3", { callback: InitialiseMap, other_params: "sensor=false" });
});

function InitialiseMap() {
    var myOptions = {
        zoom: zoomLevel,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    // show location map
    map = new google.maps.Map(document.getElementById("mapCanvas"), myOptions);
    var position = new google.maps.LatLng(markerLat, markerLong);
    map.setCenter(position);

    // show vehicles on the map
    for (var i = 0; i < positions.length; i++) {
        var location = new google.maps.LatLng(positions[i][0], positions[i][1]);
        var marker = new google.maps.Marker({ position: location, map: map, title: positions[i][2] }); 
    }
}