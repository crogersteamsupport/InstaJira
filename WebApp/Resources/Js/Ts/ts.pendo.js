function getMainFrame(wnd) {
    if (!wnd) wnd = window;
    var result = wnd;
    var cnt = 0;
    while (!(result.Ts && result.Ts.Services)) {
        result = result.parent;
        cnt++;
        if (cnt > 5) return null;
    }
    return result;
}

(function () {
    var mainFrame = getMainFrame();

    (function (p, e, n, d, o) {
        var v, w, x, y, z; o = p[d] = p[d] || {}; o._q = [];
        v = ['initialize', 'identify', 'updateOptions', 'pageLoad']; for (w = 0, x = v.length; w < x; ++w) (function (m) {
            o[m] = o[m] || function () { o._q[m === v[0] ? 'unshift' : 'push']([m].concat([].slice.call(arguments, 0))); };
        })(v[w]);
        y = e.createElement(n); y.async = !0; y.src = 'https://cdn.pendo.io/agent/static/6af64640-2a96-4767-4bd8-f480c3f1ac37/pendo.js';
        z = e.getElementsByTagName(n)[0]; z.parentNode.insertBefore(y, z);
    })(window, document, 'script', 'pendo');
    if (pendo) pendo.initialize(mainFrame.pendo_options);
})();

document.onkeydown = function (event) {

    if (!event) { /* This will happen in IE */
        event = window.event;
    }

    var keyCode = event.keyCode;

    if (keyCode == 8 &&
		((event.target || event.srcElement).tagName != "TEXTAREA") &&
		((event.target || event.srcElement).tagName != "INPUT")) {

        if (navigator.userAgent.toLowerCase().indexOf("msie") == -1) {
            event.stopPropagation();
        } else {
            event.returnValue = false;
        }

        return false;
    }
};

//window.onbeforeunload = function () {
//    return "Please make sure to save any changes before leaving the page.";
//    //if we return nothing here (just calling return;) then there will be no pop-up question at all
//    //return;
//};