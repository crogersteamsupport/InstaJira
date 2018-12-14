function createChart(element, options, records) {
    var oldChart = $(element).highcharts();
    if (oldChart) oldChart.destroy();
    var $el = $(element).empty();
    var result = addChartData(options, records);
    if (result == null) {
        try {
            $($el).highcharts(options);
        } catch (err) {
            showChartError($el, err);
        }
    } else {
        showChartError($el, result);
    }
}

function showChartError(element, message) {
    var $el = $(element).empty();
    $('<div>').css({'font-size':'20px','padding':'60px','text-align':'center','color':'#676767'}).html(message).appendTo($el);
}

function addChartData(options, records) {
    var old = ['#3276B1', '#193b58', '#78A300', '#e72b19', '#008080', '#E57B3A', '#bd4cff', '#FFC312', '#BA55D3'];
    var berry = ['#8A2BE2', '#BA55D3', '#4169E1', '#C71585', '#0000FF', '#8019E0', '#DA70D6', '#7B68EE', '#C000C0', '#0000CD', '#800080'];
    var bright = ['#008000', '#0000FF', '#800080', '#800080', '#FF00FF', '#008080', '#FFFF00', '#808080', '#00FFFF', '#000080', '#800000', '#FF3939', '#7F7F00', '#C0C0C0', '#FF6347', '#FFE4B5'];
    var brightPastel = ['#418CF0', '#FCB441', '#DF3A02', '#056492', '#BFBFBF', '#1A3B69', '#FFE382', '#129CDD', '#CA6B4B', '#005CDB', '#F3D288', '#506381', '#F1B9A8', '#E0830A', '#7893BE'];
    var chocolate = ['#A0522D', '#D2691E', '#8B0000', '#CD853F', '#A52A2A', '#F4A460', '#8B4513', '#C04000', '#B22222', '#B65C3A'];
    var earthTones = ['#33023', '#B8860B', '#C04000', '#6B8E23', '#CD853F', '#C0C000', '#228B22', '#D2691E', '#808000', '#20B2AA', '#F4A460', '#00C000', '#8FBC8B', '#B22222', '#843A05', '#C00000'];
    var excel = ['#9999FF', '#993366', '#FFFFCC', '#CCFFFF', '#660066', '#FF8080', '#0063CB', '#CCCCFF', '#000080', '#FF00FF', '#FFFF00', '#00FFFF', '#800080', '#800000', '#007F7F', '#0000FF'];
    var fire = ['#FFD700', '#FF0000', '#FF1493', '#DC143C', '#FF8C00', '#FF00FF', '#FFFF00', '#FF4500', '#C71585', '#DDE221'];
    var grayScale = ['#C8C8C8', '#BDBDBD', '#B2B2B2', '#A7A7A7', '#9C9C9C', '#919191', '#868686', '#7A7A7A', '#707070', '#656565', '#565656', '#4F4F4F', '#424242', '#393939', '#2E2E2E', '#232323'];
    var light = ['#E6E6FA', '#FFF0F5', '#FFDAB9', '#', '#FFFACD', '#', '#FFE4E1', '#F0FFF0', '#F0F8FF', '#F5F5F5', '#FAEBD7', '#E0FFFF'];
    var pastel = ['#87CEEB', '#32CD32', '#BA55D3', '#F08080', '#4682B4', '#9ACD32', '#40E0D0', '#FF69B4', '#F0E68C', '#D2B48C', '#8FBC8B', '#6495ED', '#DDA0DD', '#5F9EA0', '#FFDAB9', '#FFA07A'];
    var seaGreen = ['#2E8B57', '#66CDAA', '#4682B4', '#008B8B', '#5F9EA0', '#38B16E', '#48D1CC', '#B0C4DE', '#8FBC8B', '#87CEEB'];
    var semiTransparent = ['#FF6969', '#69FF69', '#6969FF', '#FFFF5D', '#69FFFF', '#FF69FF', '#CDB075', '#FFAFAF', '#AFFFAF', '#AFAFFF', '#FFFFAF', '#AFFFFF', '#FFAFFF', '#E4D5B5', '#A4B086', '#819EC1'];

    //options.colors = old;
    options.colors = brightPastel;
    options.exporting = {
        url: '../../../chartexport',
        filename: '"MyChart"',
        sourceWidth: 1200
    }
    options.navigation = {
        buttonOptions: {
            enabled: false
        }
    }

    function fixRecordName(record, index) {
        if (record.fieldType == 'bool') {
            return record.name + (record.data[index] == true ? ' = True' : ' = False');
        } else {
            return record.data[index];
        }
    }

    function fixBlankSeriesName(val) {
        return !val || val == '' ? 'Unknown' : val + '';
    }

    if (options.ts.chartType == 'pie') {
        if (records.length > 2) {
            return 'Please do not select a series field to plot a pie chart.';
        }

        var total = 0;
        for (var i = 0; i < records[1].data.length; i++) {
            total += parseInt(records[1].data[i]);
        }

        options.series = [{
            type: 'pie',
            name: options.ts.seriesTitle,
            data: []
        }];

        for (var i = 0; i < records[1].data.length; i++) {
            var val = records[1].data[i] / total * 100;
            options.series[0].data.push([fixBlankSeriesName(records[0].data[i]), parseFloat(val.toFixed(2))]);
        }

    } else if ((records[0].fieldType === 'datetime' && records[0].format === 'date') || (records[1].fieldType === 'datetime' && records[1].format === 'date')) {
        options.series = [];
        options.xAxis = {
            type: 'datetime'
        };

        if (records.length == 3) {
            function findSeries(value) {
                for (var i = 0; i < options.series.length; i++) {
                    if (options.series[i].value == value) return options.series[i]
                }
                return null;
            }

            for (var i = 0; i < records[0].data.length; i++) {
                var val = records[0].data[i];
                var series = findSeries(val);

                if (!series) {
                    series = {
                        name: fixBlankSeriesName(fixRecordName(records[0], i)),
                        value: val,
                        data: []
                    };
                    options.series.push(series);
                }

                var item = [];

                if (records[1].fieldType === 'datetime' && records[1].format === 'date') {
                    item.push(Date.parse(records[1].data[i]));
                } else {
                    item.push(records[1].data[i]);
                }

                item.push(records[2].data[i]);
                if (item[0] != null) {
                    if (item[1] == null) item[1] = 0;
                    series.data.push(item);
                }
            }
        } else if (records.length = 2) {
            options.series.push({
                name: records[1].name,
                data: []
            });

            for (var i = 0; i < records[0].data.length; i++) {
                var item = [];

                if (records[0].fieldType === 'datetime' && records[0].format === 'date') {
                    item.push(Date.parse(records[0].data[i]));
                } else {
                    item.push(records[0].data[i]);
                }
                item.push(records[1].data[i]);
                if (item[0] != null) {
                    if (item[1] == null) item[1] = 0;
                    options.series[0].data.push(item);
                }
            }
        }

    } else {
        options.series = [];

        if (records.length == 3) {

            options.xAxis = {
                categories: []
            };

            function indexOfCategory(name) {
                for (var i = 0; i < options.xAxis.categories.length; i++) {
                    if (options.xAxis.categories[i] == name) {
                        return i;
                    }
                }
                return -1;
            }
            // build categories
            for (var i = 0; i < records[1].data.length; i++) {
                var name = fixRecordName(records[1], i);
                if (indexOfCategory(name) < 0) {
                    options.xAxis.categories.push(name);
                }
            }

            function createDataArray() {
                var result = [];
                for (var i = 0; i < options.xAxis.categories.length; i++) {
                    result.push(0);
                }
                return result;
            }

            function findSeries(value) {
                for (var i = 0; i < options.series.length; i++) {
                    if (options.series[i].value == value) return options.series[i]
                }
                return null;
            }

            for (var i = 0; i < records[0].data.length; i++) {
                var val = records[0].data[i];
                var series = findSeries(val);

                if (!series) {
                    series = {
                        name: fixBlankSeriesName(fixRecordName(records[0], i)),
                        value: val,
                        data: createDataArray()
                    };
                    options.series.push(series);
                }
                var catIndex = indexOfCategory(fixRecordName(records[1], i));
                if (records[2].data[i]) series.data[catIndex] = records[2].data[i];
            }

        } else if (records.length = 2) {
            options.xAxis = {
                categories: records[0].data
            };
            options.series.push({
                name: records[1].name,
                data: records[1].data
            });
        }
    }

    if (options.series && options.series.length > 1000) {
        return '<p>Your series has ' + options.series.length + ' items.</p><p>This might cause issues rendering the chart, please try filtering your data to less than 1000.</p>';
    }

}
