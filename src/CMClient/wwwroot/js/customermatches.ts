/// reference path="typings/jquery.d.ts"

// get the URI from a hidden field in view
var serviceURI = $("input#serviceURI").val();

$.ajax({
    url: serviceURI + "CustomerMatch/", success: function (result) {
        console.log(result);
    }
});