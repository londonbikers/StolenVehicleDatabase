$(function () {
    google.load("maps", "3", { callback: InitialiseMap, other_params: "sensor=false" });
});

function InitialiseMap() {

    var myOptions = {
        zoom: 16,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    // show marker.
    map = new google.maps.Map(document.getElementById("mapCanvas"), myOptions);

    var position = new google.maps.LatLng(markerLat, markerLong);
    map.setCenter(position);
    var marker = new google.maps.Marker({
        map: map,
        position: position,
        draggable: false
    });
}