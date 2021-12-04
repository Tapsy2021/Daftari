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

        $(".card-body > div[id]").each(function () {
            //var context = $(this);
            var id = $(this).attr('id');
            var colors = ["#39afd1"],
                dataColors = $("#" + id).data("colors"),
                chart_value = $("#" + id).data("value"),
                title = $("#" + id).data("title")
            //console.log(chart_value);

            if (id == "chlorine-stacked-column") {
                $.ajax({
                    async: true,
                    type: "GET",
                    cache: false,
                    url: rootURL + "Chlorine_Chart_Read?from=" + start + "&to=" + end
                }).done(function (data) {

                    var colors = ["#39afd1", "#ffbc00", "#e3eaef"],
                        dataColors = $("#" + id).data("colors");
                    (dataColors = $("#" + id).data("colors")) && (colors = dataColors.split(","));
                    var options = {
                        chart: {
                            height: 290,
                            type: "bar",
                            stacked: !0,
                            toolbar: {
                                show: !1
                            }
                        },
                        plotOptions: {
                            bar: {
                                horizontal: !1,
                                columnWidth: "50%"
                            }
                        },
                        series: [{
                            name: "Free Chlorine (PPM)",
                            data: data.FreeChlorine
                        }, {
                            name: "Combined Chlorine (PPM)",
                            data: data.CombinedChlorine
                        }],
                        xaxis: {
                            categories: data.Labels,
                        },
                        //tooltip: {
                        //    y: {
                        //        formatter: function (e) {
                        //            return (e / 1e6).toFixed(2)
                        //        }
                        //    }
                        //},
                        colors: colors,
                        fill: {
                            opacity: 1
                        },
                        legend: {
                            offsetY: 7
                        },
                        grid: {
                            row: {
                                colors: ["transparent", "transparent"],
                                opacity: .2
                            },
                            borderColor: "#f1f3fa",
                            padding: {
                                bottom: 5
                            }
                        }
                    };
                    $("#" + id).empty();
                    new ApexCharts(document.querySelector("#" + id), options).render();
                });

            }
            else {

                $.ajax({
                    async: true,
                    type: "GET",
                    cache: false,
                    url: rootURL + "GetReporting?from=" + start + "&to=" + end + "&field=" + chart_value
                }).done(function (data) {

                    (dataColors = $("#" + id).data("colors")) && (colors = dataColors.split(","));
                    var options = {
                        //annotations: {
                        //    yaxis: [{
                        //        y: 8200,
                        //        borderColor: "#0acf97",
                        //        label: {
                        //            borderColor: "#0acf97",
                        //            style: {
                        //                color: "#fff",
                        //                background: "#0acf97"
                        //            },
                        //            text: "Support"
                        //        }
                        //    }],
                        //},
                        chart: {
                            height: 230,
                            type: "line",
                            id: "areachart-2"
                        },
                        colors: colors,
                        dataLabels: {
                            enabled: !0
                        },
                        stroke: {
                            width: [3],
                            curve: "straight"
                        },
                        series: [{
                            data: data.Data
                        }],
                        title: {
                            text: title,
                            align: "left"
                        },
                        labels: data.Labels,
                        xaxis: {
                            type: "datetime"
                        },
                        //tooltip: {
                        //    y: {
                        //        formatter: function (e) {
                        //            return (e / 1e6).toFixed(2)
                        //        }
                        //    }
                        //},
                        grid: {
                            row: {
                                colors: ["transparent", "transparent"],
                                opacity: .2
                            },
                            borderColor: "#f1f3fa"
                        },
                        responsive: [{
                            breakpoint: 600,
                            options: {
                                chart: {
                                    toolbar: {
                                        show: !1
                                    }
                                },
                                legend: {
                                    show: !1
                                }
                            }
                        }]
                    };
                    $("#" + id).empty();
                    new ApexCharts(document.querySelector("#" + id), options).render();

                });

            }

        });




    },

    e.prototype.init = function () {
        this.initCharts()
    },
    o.ChemicalsReport = new e,
    o.ChemicalsReport.Constructor = e
}(window.jQuery),
function (t) {
    "use strict";
    t(document).ready(function (e) {
        t.ChemicalsReport.init()
    })

    t('#Date').change(function () {
        t.ChemicalsReport.init()
    });

}(window.jQuery);

//function OpenStatusReport(status) {

//    window.open(rootURL + "StatusReport?status=" + status);
//}