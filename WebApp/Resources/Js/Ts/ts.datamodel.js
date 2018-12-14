(function ($) {
    function GridModel(getData, getItemMetadata) {
        // private
        var PAGESIZE = 50;
        var data = { length: 0 };
        var searchstr = "";
        var sortcol = null;
        var sortdir = 1;
        var h_request = null;
        var req = null; // ajax request
        var endTotal = 0;

        // events
        var onDataLoading = new Slick.Event();
        var onDataLoaded = new Slick.Event();

        if (getItemMetadata) {
            data.getItemMetadata = function (index) {
                return getItemMetadata(index, data);
            }
        }

        function isDataLoaded(from, to) {
            for (var i = from; i <= to; i++) {
                if (data[i] == undefined || data[i] == null) {
                    return false;
                }
            }
            return true;
        }

        function clear() {
            endTotal = 0;
            for (var key in data) {
                delete data[key];
            }
            data.length = 0;
            if (getItemMetadata) {
                data.getItemMetadata = function (index) {
                    return getItemMetadata(index, data);
                }
            }
        }

        function setEndTotal(value) {
            endTotal = value;
        }

        function ensureData(from, to, loadedCallback) {
            if (req) {
                try {
                    if (req.get_executor()) {
                        req.get_executor().abort();
                    } else {
                        req.abort();
                    }
                } catch (err) { }
                for (var i = req.fromPage; i <= req.toPage; i++) {
                    data[i * PAGESIZE] = undefined;
                }
            }
            if (endTotal != 0 && to > endTotal) to = endTotal;
            if (endTotal != 0 && from > endTotal) from = endTotal;
            if (from < 0) { from = 0; }

            if (data.length > 0) { to = Math.min(to, data.length - 1); }

            var fromPage = Math.floor(from / PAGESIZE);
            var toPage = Math.floor(to / PAGESIZE);

            while (data[fromPage * PAGESIZE] !== undefined && fromPage < toPage) {
                fromPage++;
            }
            while (data[toPage * PAGESIZE] !== undefined && fromPage < toPage) {
                toPage--;
            }

            if (fromPage > toPage || ((fromPage == toPage) && data[fromPage * PAGESIZE] !== undefined)) {
                // TODO:  look-ahead
                onDataLoaded.notify({ from: from, to: to });
                return;
            }

            if (h_request != null) {
                clearTimeout(h_request);
            }

            h_request = setTimeout(function () {
                for (var i = fromPage; i <= toPage; i++) {
                    data[i * PAGESIZE] = null; // null indicates a 'requested but not available yet'
                }

                onDataLoading.notify({ from: from, to: to });
                req = getData(fromPage * PAGESIZE, (toPage * PAGESIZE) + PAGESIZE - 1, sortcol, (sortdir < 1),
                function (resp) {
                    onSuccess(resp);
                    if (loadedCallback) {
                        loadedCallback();
                    }
                });
                req.fromPage = fromPage;
                req.toPage = toPage;
            }, 100);
        }

        function onSuccess(resp) {
            if (resp.d) {
                resp = resp.d;
            }
            var from = resp.From, to = resp.To;
            var results = typeof resp.Data == 'string' ? JSON.parse(resp.Data) : resp.Data;
            //console.log('RESPONSE: From: ' + from + ', To: ' + to);
            data.length = resp.Total;

            if (results.length > 0) {
                //console.log('count: ' + results.length);
                for (var i = 0; i < results.length; i++) {
                    var item = results[i];
                    data[from + i] = item;
                    data[from + i].index = from + i;
                }
            }

            req = null;
            onDataLoaded.notify({ 'from': from, 'to': to, 'total': resp.Total });
        }

        function reloadData(from, to) {
            for (var i = from; i <= to; i++) {
                delete data[i];
            }
            ensureData(from, to);
        }

        function setSort(column, dir) {
            sortcol = column;
            sortdir = dir;
            clear();
        }

        function setSearch(str) {
            searchstr = str;
            clear();
        }

        return {
            // properties
            "data": data,
            // methods
            "clear": clear,
            "setEndTotal": setEndTotal,
            "isDataLoaded": isDataLoaded,
            "ensureData": ensureData,
            "reloadData": reloadData,
            "setSort": setSort,
            "setSearch": setSearch,
            // events
            "onDataLoading": onDataLoading,
            "onDataLoaded": onDataLoaded
        };
    }

    $.extend(true, window, { TeamSupport: { DataModel: GridModel} });
})(jQuery);
