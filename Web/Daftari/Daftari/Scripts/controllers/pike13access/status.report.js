!function (o) {
    "use strict";
    function e() {
        this.$body = o("body"),
            this.charts = []
    }
    e.prototype.initCharts = function () {

        var dates = $('#Date').val().split(' - ');
        var start = moment(dates[0]).format('YYYY-MM-DDTHH:mm:ss');
        var end = moment(dates[1]).format('YYYY-MM-DDTHH:mm:ss');

        $.ajax({
            async: true,
            type: "GET",
            cache: false,
            url: rootURL + "GetReporting?from=" + start + "&to=" + end
        }).done(function (data) {
            //for indicators, use counts
            //for pie, use count as well
            var index = data.Total_Stundents.length - 1;

            $(".now-title").html(data.Title);
            $("#total-capacity").html(data.Total_Capacity[index]);
            $("#total-students").html(data.Total_Stundents[index]);
            $("#cancelled-students").html(data.Total_Cancelled_Stundents[index]);
            $("#no-show-students").html(data.Total_No_Show_Stundents[index]);
            $("#total-classes").html(data.Total_Classes[index]);
            $("#unpaid-students").html(data.Unpaid_Students[index]);
            
            window.Apex = {
                chart: {
                    parentHeightOffset: 0,
                    toolbar: {
                        show: !1
                    }
                },
                grid: {
                    padding: {
                        left: 0,
                        right: 0
                    }
                },
                colors: ["#727cf5", "#0acf97", "#fa5c7c", "#ffbc00"]
            };

            var colors = ["#727cf5", "#0acf97", "#fa5c7c", "#ffbc00"],
                dataColors = o("#status-line-chart").data("colors");
            dataColors && (colors = dataColors.split(","));
            var options = {
                chart: {
                    height: 290,
                    type: "line",
                    dropShadow: {
                        enabled: !0,
                        opacity: .2,
                        blur: 7,
                        left: -7,
                        top: 7
                    }
                },
                dataLabels: {
                    enabled: !1
                },
                stroke: {
                    curve: "smooth",
                    width: 4
                },
                series: [{
                    name: "Total Capacity",
                    data: data.Total_Capacity
                }, {
                    name: "Total Students",
                    data: data.Total_Stundents
                }, {
                    name: "Total No Show",
                    data: data.Total_No_Show_Stundents
                }, {
                    name: "Total Cancelled",
                    data: data.Total_Cancelled_Stundents
                }],
                colors: colors,
                zoom: {
                    enabled: !1
                },
                legend: {
                    show: !0
                },
                xaxis: {
                    type: "string",
                    categories: data.Labels,
                    tooltip: {
                        enabled: !1
                    },
                    axisBorder: {
                        show: !1
                    }
                },
                yaxis: {
                    labels: {
                        formatter: function (e) {
                            return e;
                            //return e + "k"
                        },
                        offsetX: -15
                    }
                }
            };
            new ApexCharts(document.querySelector("#status-line-chart"), options).render();

            colors = ["#39afd1", "#ffbc00", "#c41010", "#fa5c7c"],
                dataColors = $("#simple-donut").data("colors");
            var series = [];

            series[3] = data.Total_Cancelled_Stundents[index];
            series[2] = data.Total_No_Show_Stundents[index];
            series[1] = data.Total_Stundents[index] - series[3] - series[2];
            series[0] = data.Total_Capacity[index] - data.Total_Stundents[index];

            (dataColors = $("#simple-donut").data("colors")) && (colors = dataColors.split(","));
            options = {
                chart: {
                    height: 320,
                    type: "donut"
                },
                series: series,// [44, data.Total_Stundents[index] - data.Total_No_Show_Stundents[index] - data.Total_Cancelled_Stundents[index], data.Total_No_Show_Stundents[index], data.Total_Cancelled_Stundents[index]],
                legend: {
                    show: !0,
                    position: "bottom",
                    horizontalAlign: "center",
                    verticalAlign: "middle",
                    floating: !1,
                    fontSize: "14px",
                    offsetX: 0,
                    offsetY: 7
                },
                labels: ["Capacity", "Others", "No Show", "Cancelled"],
                colors: colors,
                responsive: [{
                    breakpoint: 600,
                    options: {
                        chart: {
                            height: 240
                        },
                        legend: {
                            show: !1
                        }
                    }
                }]
            };
            (new ApexCharts(document.querySelector("#simple-donut"), options)).render();

        });
    },

    e.prototype.init = function () {
        this.initCharts()
    },
    o.StatusReport = new e,
    o.StatusReport.Constructor = e
}(window.jQuery),
function (t) {
    "use strict";
    t(document).ready(function (e) {
        t.StatusReport.init()
    })

    t('#Date').change(function () {
        t.StatusReport.init()
    });

    //t(".card-body").click(function () {

         
    //    //alert("Handler for .click() called.");
    //});

}(window.jQuery);

function OpenStatusReport(status) {

    window.open(rootURL + "StatusReport?status=" + status);



    //var g = 9;
    //console.log(g);
}