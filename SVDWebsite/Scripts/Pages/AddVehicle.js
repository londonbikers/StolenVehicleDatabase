var map;
var markers = [];
var geocoder;
var pos;

$(function () {
    $('#TheftDate').datepicker({ changeMonth: true, changeYear: true, dateFormat: 'dd MM yy' });
    $("#VehicleManufacturerId").CascadingDropDown("#VehicleTypeId", '/AddVehicle/GetMakes');
    $("#VehicleModelId").CascadingDropDown("#VehicleManufacturerId", '/AddVehicle/GetModels');

    // handle enter on theft-map button submit.
    $('#TheftLocation').keypress(function (e) {
        if (e.which == 13 && $('#TheftLocation').val() != "") {
            FindLocation();
            return false;
        }
    });

    $("#findLocationBtn").click(function (event) {
        if ($('#TheftLocation').val() != "") {
            FindLocation();
        }
    });

    google.load("maps", "3", { callback: InitialiseMap, other_params: "sensor=false" });
});

function ConfirmSubmit() {
    return confirm("Are you ready to add the venicle? All the details are correct?");
}

function AddSecurityType() {
    var list = document.getElementById("SecurityTypesList");
    var csv = document.getElementById("VehicleSecurityTypeCsv");
    var container = document.getElementById("vstList");
    var typeName = list[list.selectedIndex].text;

    if (csv.value == "") { csv.value = ","; }
    csv.value += list[list.selectedIndex].value + ",";

    var new_element = document.createElement('li');
    new_element.setAttribute("id", "vst-" + list[list.selectedIndex].value);
    new_element.innerHTML = "<a href=\"javascript:RemoveSecurityType(" + list[list.selectedIndex].value + ");\"><img src=\"http://mpncdn.eu/svd/c/images/icons/delete.png\" alt=\"remove\" /></a> " + typeName;
    container.insertBefore(new_element, container.firstChild);
}

function RemoveSecurityType(id) {
    var $csv = $("#VehicleSecurityTypeCsv");
    $csv.val($csv.val().replace(',' + id + ',', ','));
    $('#vst-' + id).remove()
}

/*** MAPPING **********************************************************************/

function FindLocation() {
    var address = $('#TheftLocation').val();
    geocoder.geocode({ 'address': address, 'region': userRegion }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            var addressComponents = results[0].address_components;
            var location = results[0].geometry.location;

            map.setCenter(location);
            map.setZoom(17);
            if (markers.length > 0) {
                markers[0].setMap(null);
                markers = [];
            }
            var marker = new google.maps.Marker({
                map: map,
                position: results[0].geometry.location,
                draggable: true
            });

            // ensure drags update the hidden form elements.
            google.maps.event.addListener(marker, "dragend", function (latLng) {
                StoreLocation(markers[0].position.lat(), markers[0].position.lng(), null, true);
            });

            // keep track of the markers so we can remove them on subsequent searches.
            markers.push(marker);

            // store the lat/long in the hidden elements for the model.
            StoreLocation(location.lat(), location.lng(), addressComponents, false);
        }
    });
    $('#markerGuide').show('slow');
    $('#TheftLocation').val(''); // clear search box.
}

function StoreLocation(lat, lng, address, addressLookup) {
    $('#TheftLocationLat').val(lat)
    $('#TheftLocationLong').val(lng)
    if (addressLookup) {
        address = LookupAddress(lat, lng);
    } else {
        StoreAddress(address);
    }
}

function StoreAddress(addressComponents) {
    var country = GetAddressPart("country", addressComponents);
    var area1 = GetAddressPart("administrative_area_level_1", addressComponents);
    var area2 = GetAddressPart("administrative_area_level_2", addressComponents);
    var locality = GetAddressPart("locality", addressComponents);
    var subLocality = GetAddressPart("sublocality", addressComponents);
    var route = GetAddressPart("route", addressComponents);
    ClearStoredAddress();

    if (country != null) {
        $('#TheftLocationCountry').val(country.long_name);
        $('#TheftLocationCountryCode').val(country.short_name);
        GetLocationLatLng($('#TheftLocationCountryPos'), country.long_name);
    }
    if (area1 != null) {
        $('#TheftLocationAdministrativeAreaLevel1').val(area1.long_name);
        GetLocationLatLng($('#TheftLocationAdministrativeAreaLevel1Pos'), area1.long_name + ", " + $('#TheftLocationCountry').val());
    }
    if (area2 != null) {
        // gmaps has a thing where it sometimes returns duplicate names, separated by type, i.e. there's two Paris'.
        // so only add subsequent items if their name doesn't already exist in a higher-order element.
        if ($('#TheftLocationAdministrativeAreaLevel1').val() != area2.long_name) {
            $('#TheftLocationAdministrativeAreaLevel2').val(area2.long_name);
            GetLocationLatLng($('#TheftLocationAdministrativeAreaLevel2Pos'), area2.long_name + ", " + $('#TheftLocationAdministrativeAreaLevel1').val() + ", " + $('#TheftLocationCountry').val());
        }
    }
    if (locality != null) {
        if ($('#TheftLocationAdministrativeAreaLevel1').val() != locality.long_name && 
            $('#TheftLocationAdministrativeAreaLevel2').val() != locality.long_name) {
            $('#TheftLocationLocality').val(locality.long_name);
            GetLocationLatLng($('#TheftLocationLocalityPos'), locality.long_name + ", " + $('#TheftLocationAdministrativeAreaLevel2').val() + ", " + $('#TheftLocationAdministrativeAreaLevel1').val() + ", " + $('#TheftLocationCountry').val());
        }
    }
    if (subLocality != null) {
        if ($('#TheftLocationAdministrativeAreaLevel1').val() != subLocality.long_name && 
            $('#TheftLocationAdministrativeAreaLevel2').val() != subLocality.long_name && 
            $('#TheftLocationLocality').val() != subLocality.long_name) {
            $('#TheftLocationSubLocality').val(subLocality.long_name);
            GetLocationLatLng($('#TheftLocationSubLocalityPos'), subLocality.long_name + ", " + $('#TheftLocationLocality').val() + ", " + $('#TheftLocationAdministrativeAreaLevel2').val() + ", " + $('#TheftLocationAdministrativeAreaLevel1').val() + ", " + $('#TheftLocationCountry').val());
        }
    }
    if (route != null) {

        if ($('#TheftLocationAdministrativeAreaLevel1').val() != route.long_name && 
            $('#TheftLocationAdministrativeAreaLevel2').val() != route.long_name && 
            $('#TheftLocationLocality').val() != route.long_name &&
            $('#TheftLocationSubLocality').val() != route.long_name) {
            $('#TheftLocationRoute').val(route.long_name);
            GetLocationLatLng($('#TheftLocationRoutePos'), route.long_name + ", " + $('#TheftLocationSubLocality').val() + ", " + $('#TheftLocationLocality').val() + ", " + $('#TheftLocationAdministrativeAreaLevel2').val() + ", " + $('#TheftLocationAdministrativeAreaLevel1').val() + ", " + $('#TheftLocationCountry').val());
        }
    }
}

// performs a simple geo-code lookup for location lat/lng's.
function GetLocationLatLng(element, location) {
    location = location.replace(", ,", ",");
    geocoder.geocode({ 'address': location }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            var lat = results[0].geometry.location.lat();
            var lng = results[0].geometry.location.lng();
            element.val(lat + "," + lng);
        }
    });
}

// returns a specific geocoder address component part that matches a type.
function GetAddressPart(partName, addressComponents) {
    for (var partIndex in addressComponents) {
        for (var typeIndex in addressComponents[partIndex].types) {
            if (addressComponents[partIndex].types[typeIndex] == partName) {
                return addressComponents[partIndex];
            }
        }
    }
}

function LookupAddress(lat, lng) {
    var latlng = new google.maps.LatLng(lat, lng);
    geocoder.geocode({ 'latLng': latlng }, function (results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            if (results[0]) {
                StoreAddress(results[0].address_components);
            }
        } else {
            alert("Hey, there's nothing there...");
            ClearStoredAddress();
        }
    });
}

function ClearStoredAddress() {
    $('#TheftLocationCountry').val('');
    $('#TheftLocationCountryPos').val('');
    $('#TheftLocationCountryCode').val('');
    $('#TheftLocationAdministrativeAreaLevel1').val('');
    $('#TheftLocationAdministrativeAreaLevel1Pos').val('');
    $('#TheftLocationAdministrativeAreaLevel2').val('');
    $('#TheftLocationAdministrativeAreaLevelPos').val('');
    $('#TheftLocationLocality').val('');
    $('#TheftLocationLocalityPos').val('');
    $('#TheftLocationSubLocality').val('');
    $('#TheftLocationSubLocalityPos').val('');
    $('#TheftLocationRoute').val('');
    $('#TheftLocationRoutePos').val('');
}

function InitialiseMap() {
    var myOptions = {
        zoom: 16,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    map = new google.maps.Map(document.getElementById("mapCanvas"), myOptions);
    geocoder = new google.maps.Geocoder();

    // a position may exist already.
    if ($('#TheftLocationLat').val() != '' && $('#TheftLocationLong').val() != '') {
        var position = new google.maps.LatLng($('#TheftLocationLat').val(), $('#TheftLocationLong').val());
        map.setCenter(position);
        var marker = new google.maps.Marker({
            map: map,
            position: position,
            draggable: true
        });

        // ensure drags update the hidden form elements.
        google.maps.event.addListener(marker, "dragend", function (latLng) {
            StoreLocation(markers[0].position.lat(), markers[0].position.lng(), null, true);
        });

        markers.push(marker);
        return;
    }

    var latlng = new google.maps.LatLng(51.502758957640296, -0.12256622314453125); // london, default location

    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            // awesome html5 stuff
            latlng = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
            map.setCenter(latlng);
        }, function () {
            // occurs if location denied... try ip-lookup
            if (google.loader.ClientLocation) {
                userRegion = google.loader.ClientLocation.address.country_code;
                latlng = new google.maps.LatLng(google.loader.ClientLocation.latitude, google.loader.ClientLocation.longitude);
            }
            var myOptions = {
                zoom: 12,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };
            map = new google.maps.Map(document.getElementById("mapCanvas"), myOptions);
            map.setCenter(latlng);
        });
        return;
    } else if (google.loader.ClientLocation) {
        // ip-lookup
        userRegion = google.loader.ClientLocation.address.country_code;
        latlng = new google.maps.LatLng(google.loader.ClientLocation.latitude, google.loader.ClientLocation.longitude);
    }
    
    var myOptions = {
        zoom: 13,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    map = new google.maps.Map(document.getElementById("mapCanvas"), myOptions);
    map.setCenter(latlng);
}