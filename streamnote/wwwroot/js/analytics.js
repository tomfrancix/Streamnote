function loadTaskData() {
    $.post({
            url: "/Analytics/GetTaskData",
            dataType: "json"
        })
        .done(function (result, status) {
             if (result != null && result.charts.length > 0) {
                 for (var i = 0; i < result.charts.length; i++) {
                     var chart = result.charts[i];
                     var xValues = chart.xValues;
                     var barColors = chart.colors;
                     var theChart = new Chart(chart.chartIdentifier,
                         {
                             type: chart.type,
                             data: {
                                 labels: xValues,
                                 datasets: chart.datasets
                             },
                             options: {
                                 title: {
                                     display: true,
                                     text: chart.title
                                 }
                             }
                         });
                 }
             }
        });
};