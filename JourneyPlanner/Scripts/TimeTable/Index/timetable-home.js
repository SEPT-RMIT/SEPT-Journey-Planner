/// <reference path="../../_references.js" />

$('#train-list').change(function () {
    $.ajax({
        url: "/Home/GetStations",
        type: "GET",
        datatype: "html",
        data: { id: $("#train-list").val() },
        success: function (data) {
            if (data.Success === false)
                alert(data.Message);
            else {
                $('.stations').html(data);
                BindSelect();
            }
        },
        failure: function (data) {
            alert("Something went wrong. Please try again");
        },
        error: function (data) {
            alert("There was a connection error. Please check your connection and try again");
        }
    });
});

function BindSelect() {
    // Move this into one call that returns a json object that i can access both lat and long
    $('#stations-list').change(function () {
        $.ajax({
            url: "/DataService/GetLatitude",
            type: "GET",
            datatype: "html",
            data: { id: $("#stations-list").val() },
            success: function (latitude) {
                $.ajax({
                    url: "/DataService/GetLongitude",
                    type: "GET",
                    datatype: "html",
                    data: { id: $("#stations-list").val() },
                    success: function (longitude) {
                        $("#map_canvas").show();
                        initialize(new google.maps.LatLng(latitude, longitude));
                    }
                });
            }
        });

        $.ajax({
            url: "/Home/GetTimes",
            type: "GET",
            datatype: "html",
            data: { id: $("#stations-list").val() },
            success: function (data) {
                if (data.Success === false)
                    alert(data.Message);
                else {
                    $('.times').html(data);
                }
            },
            failure: function (data) {
                alert("Something went wrong. Please try again");
            },
            error: function (data) {
                alert("There was a connection error. Please check your connection and try again");
            }
        });
    });
}