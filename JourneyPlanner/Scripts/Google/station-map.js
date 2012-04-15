$(function () {
    $("#map_canvas").hide();
});

function initialize(latlng) {
    var myOptions = {
        zoom: 14,
        center: latlng,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    var map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);

    marker = new google.maps.Marker({
        map: map,
        draggable: false,
        draggable: true,
        animation: google.maps.Animation.DROP,
        position: latlng
    });
}