table.button().add(3, {
    text: 'Sync All',
    action: function (e, dt, node, config) {
        $.NotificationApp.send("Status", "Requesting Server Sync", "bottom-right", "rgba(0,0,0,0.2)", "info")

        $.ajax({
            async: true,
            url: customerURL + "SyncCustomers?type=9",
            cache: false
        }).done(function (data) {
            table.processing(false);

            $.NotificationApp.send("Status", data.msg, "bottom-right", "rgba(0,0,0,0.2)", (data.type === 0) ? "success" : (data.type === 1) ? "info" : "danger")

            table.ajax.reload();
        }).fail(function (msg) {
            table.processing(false);
            $.NotificationApp.send("Status", 'Fail', "bottom-right", "rgba(0,0,0,0.2)", "danger");

            table.ajax.reload();
        });
    },
    className: 'btn-danger'
});

table.button().add(4, {
    text: 'Sync New',
    action: function (e, dt, node, config) {
        $.NotificationApp.send("Status", "Requesting Server Sync", "bottom-right", "rgba(0,0,0,0.2)",  "info")

        $.ajax({
            async: true,
            url: customerURL + "SyncCustomers?type=0",
            cache: false
        }).done(function (data) {

            $.NotificationApp.send("Status", data.msg, "bottom-right", "rgba(0,0,0,0.2)", (data.type === 0) ? "success" : (data.type === 1) ? "info" : "danger")

            table.ajax.reload();
        }).fail(function (msg) {
            $.NotificationApp.send("Status", 'Fail', "bottom-right", "rgba(0,0,0,0.2)", "danger");

            table.ajax.reload();
        });
    },
    className: 'btn-info'
});