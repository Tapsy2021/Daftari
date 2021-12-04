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
            //var index = data.Total_Stundents.length - 1;

            $(".now-title").html(data.Title);
            $("#total-capacity").html(data.Capacity_Count);
            $("#total-students").html(data.Students_Count);
            $("#cancelled-students").html(data.Cancelled_Count);
            $("#no-show-students").html(data.No_Show_Count);
            $("#total-classes").html(data.Classes_Count);
            $("#unpaid-students").html(data.Unpaid_Count);

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
            //if ($('#leftmenu').is(':empty')) {
            $("#status-line-chart").empty();
            new ApexCharts(document.querySelector("#status-line-chart"), options).render();
            //try {
            //    chart.destroy();
            //    chart = new ApexCharts(document.querySelector("#status-line-chart"), options);
            //} catch {

            //}
            
            //chart.render();
            //var destroyChart = () => {
            //    if (chart.ohYeahThisChartHasBeenRendered) {
            //        console.log("now");
            //        chart.destroy();
            //        chart.ohYeahThisChartHasBeenRendered = false;                    
            //    }
            //};
            //console.log("now 2");
            //chart.render().then(() => chart.ohYeahThisChartHasBeenRendered = true);

            ////console.log(chart.ohYeahThisChartHasBeenRendered);
            //chart.destroy();
            //chart.render();//.then(() => chart.ohYeahThisChartHasBeenRendered = true);
            ////chart.render();

            colors = ["#39afd1", "#ffbc00", "#c41010", "#fa5c7c"],
                dataColors = $("#simple-donut").data("colors");
            var series = [];

            series[3] = data.Cancelled_Count;
            series[2] = data.No_Show_Count;
            series[1] = data.Students_Count - series[3] - series[2];
            series[0] = data.Capacity_Count - data.Students_Count;

            (dataColors = $("#simple-donut").data("colors")) && (colors = dataColors.split(","));
            options = {
                chart: {
                    height: 320,
                    type: "donut"
                },
                series: series,
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
            $("#simple-donut").empty();
            new ApexCharts(document.querySelector("#simple-donut"), options).render();

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
        t.StatusReport.init();
    })

    t('#Date').change(function () {
        t.StatusReport.init();
    });

}(window.jQuery);

function OpenStatusReport(status) {
    window.open(rootURL + "StatusReport?status=" + status);
}