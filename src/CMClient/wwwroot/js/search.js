$(function () {

    // get the URI from a hidden field in view
    var serviceURI = $("input#serviceURI").val();

    $.ajax({
        url: serviceURI + "stats/categorystats", success: function (result) {

            $('#categoryChart').highcharts({
                chart: {
                    type: 'pie'
                },
                title: {
                    text: 'Categories'
                },
                xAxis: {
                },
                yAxis: {
                    title: {

                    }
                },
                series: [
                    {
                        name: "Customers", data: result
                    }
                ]
            });
        }
    });

    $.ajax({
        url: serviceURI + "stats/locationstats", success: function (result) {

            $('#locationChart').highcharts({
                chart: {
                    type: 'column'
                },
                title: {
                    text: 'Locations'
                },
                xAxis: {
                    categories: $.map(result, function (val, i) {
                        return val.name;
                    })
                },
                yAxis: {
                    title: {
                        text: '# Customers'
                    }
                },
                series: [{
                    name: 'Customers',
                    data: $.map(result, function (val, i) {
                        return val.y;
                    })
                }]
            });
        }
    });

    // call default Action for Controller (Customer) on form submit
    $("#search").click(function () {
        var form = $("#searchForm");
        form.attr("action", "/");
        form.submit();
    });

    // call Add Action on form submit
    $("#add").click(function () {
        var form = $("#searchForm");
        form.attr("action", '/Customer/Add');
        form.submit();
    });

});