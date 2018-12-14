$(document).ready(function () {
  var type = getUrlVars()["type"];
  if (type) {
    switch (type) {
      case "product":
        $("h2").html("This product is either missing or you do not have permission to view it.");
        break;
      case "productversion":
        $("h2").html("This product version is either missing or you do not have permission to view it.");
        break;
    }
  }

  function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
      hash = hashes[i].split('=');
      vars.push(hash[0]);
      vars[hash[0]] = hash[1];
    }
    return vars;
  }
});