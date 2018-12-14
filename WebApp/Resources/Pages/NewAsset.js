$(document).ready(function () {
  parent.Ts.System.logAction('New Asset - Started');
  $('body').layout({
    defaults: {
      spacing_open: 0,
      closable: false
    },
    north: {
      spacing_open: 1,
      size: 100
    },
    center: {
      maskContents: true
    }
  });

  var _dateFormat;

  function LoadProducts() {
    var products = parent.Ts.Cache.getProducts();
    for (var i = 0; i < products.length; i++) {
      $('<option>').attr('value', products[i].ProductID).text(products[i].Name).data('o', products[i]).appendTo('#ddlProduct');
    }
  }

  function LoadProductVersions() {
    $('#ddlProductVersion').empty();
    var product = parent.Ts.Cache.getProduct($('#ddlProduct').val());
    for (var i = 0; i < product.Versions.length; i++) {
      $('<option>').attr('value', product.Versions[i].ProductVersionID).text(product.Versions[i].VersionNumber).data('version', product.Versions[i]).appendTo('#ddlProductVersion');
    }
    if (product.Versions.length == 0) {
      $('<option>').text(product.Name + ' has no versions.').appendTo('#ddlProductVersion');
      $('#ddlProductVersion').prop("disabled", true);
    }
    else {
      $('#ddlProductVersion').prop("disabled", false);
    }
  }

  LoadProducts();
  LoadProductVersions();
  LoadCustomControls();

  $('#ddlProduct').change(function (e) {
    LoadProductVersions();
  });

  var userDateFormat = parent.Sys.CultureInfo.CurrentCulture.dateTimeFormat.ShortDatePattern.replace("yyyy", "yy");
  $("#inputWarrantyExpiration").datepicker({ dateFormat: userDateFormat });

  $('#assetSaveBtn').click(function (e) {
    e.preventDefault();
    e.stopPropagation();

    if ($("#assetForm").valid()) {
        $('#assetSaveBtn').prop("disabled", true);
      var assetInfo = new Object();

      assetInfo.Name = $("#inputName").val();
      assetInfo.ProductID = $("#ddlProduct").val();
      if ($.isNumeric($('#ddlProductVersion').val())) {
        assetInfo.ProductVersionID = $('#ddlProductVersion').val();
      }
      assetInfo.SerialNumber = $("#inputSerialNumber").val();
      assetInfo.WarrantyExpiration = $("#inputWarrantyExpiration").val();
      assetInfo.Notes = $("#Notes").val();

      assetInfo.Fields = new Array();
      $('.customField:visible').each(function () {
        var field = new Object();
        field.CustomFieldID = $(this).attr("id");
        switch ($(this).attr("type")) {
          case "checkbox":
            field.Value = $(this).prop('checked');
            break;
          case "_date":
            //    var dt = $(this).find('input').datepicker('getDate');
              field.Value = $(this).val() == "" ? null : parent.Ts.Utils.getMsDate(convertToValidDate($(this).val()));
            break;
          case "_time":
            //    var time = new Date("January 1, 1970 00:00:00");
            //    time.setHours($(this).find('input').timepicker('getDate')[0].value.substring(0, 2));
            //    time.setMinutes($(this).find('input').timepicker('getDate')[0].value.substring(3, 5));
            field.Value = $(this).val() == "" ? null : parent.Ts.Utils.getMsDate("1/1/1900 " + $(this).val());
            break;
          case "_datetime":
            //    //field.Value = parent.Ts.Utils.getMsDate($(this).find('input').datetimepicker('getDate'));
            //    var dt = $(this).find('input').datetimepicker('getDate');
            //    field.Value = dt == null ? null : dt.toUTCString();
              field.Value = $(this).val() == "" ? null : parent.Ts.Utils.getMsDate(convertToValidDateTime($(this).val()));
            break;
          default:
            field.Value = $(this).val();
        }
        assetInfo.Fields[assetInfo.Fields.length] = field;
      });


      parent.Ts.Services.Assets.SaveAsset(parent.JSON.stringify(assetInfo), function (assetID) {
          $('#assetSaveBtn').prop("disabled", false);
        parent.Ts.System.logAction('Asset Created');
        parent.Ts.MainPage.openNewAsset(assetID);
        parent.Ts.MainPage.closenewAssetTab();
      }, function () {
          $('#assetSaveBtn').prop("disabled", false);
        alert('There was an error saving this asset.  Please try again.');
      });
    }
  });

  $('#assetCancelBtn').click(function (e) {
    parent.Ts.System.logAction('New Asset - Cancelled');
    parent.Ts.MainPage.closenewAssetTab();
  });

  function convertToValidDate(val) {
      var value = '';
      if (val == "")
          return value;

      if (_dateFormat.indexOf("M") != 0) {
          var dateArr = val.replace(/\./g, '/').replace(/-/g, '/').split('/');
          if (_dateFormat.indexOf("D") == 0)
              var day = dateArr[0];
          if (_dateFormat.indexOf("Y") == 0)
              var year = dateArr[0];
          if (_dateFormat.indexOf("M") == 3 || _dateFormat.indexOf("M") == 5)
              var month = dateArr[1];

          var timeSplit = dateArr[2].split(' ');
          if (_dateFormat.indexOf("Y") == 6)
              var year = timeSplit[0];
          else
              var day = timeSplit[0];

          var theTime = timeSplit[1];

          var formattedDate = month + "/" + day + "/" + year;
          value = parent.Ts.Utils.getMsDate(formattedDate);
          return formattedDate;
      }
      else
          return val;
  }

  function convertToValidDateTime(val) {
      var value = '';
      if (val == "")
          return value;

      if (_dateFormat.indexOf("M") != 0) {
          var dateArr = val.replace(/\./g, '/').replace(/-/g, '/').split('/');
          if (_dateFormat.indexOf("D") == 0)
              var day = dateArr[0];
          if (_dateFormat.indexOf("Y") == 0)
              var year = dateArr[0];
          if (_dateFormat.indexOf("M") == 3 || _dateFormat.indexOf("M") == 5)
              var month = dateArr[1];

          var timeSplit = dateArr[2].split(' ');
          if (_dateFormat.indexOf("Y") == 6)
              var year = timeSplit[0];
          else
              var day = timeSplit[0];

          var theTime = timeSplit[1];

          var formattedDate = month + "/" + day + "/" + year + " " + theTime;
          //value = parent.Ts.Utils.getMsDate(formattedDate) + " " + theTime;
          return formattedDate;
      }
      else
          return val;
  }

  function LoadCustomControls() {
    parent.Ts.Services.Assets.LoadCustomControls(parent.Ts.ReferenceTypes.Assets, function (html) {
      $('#customerCustomInfo').append(html);
      $('.customField:visible').each(function () {
        var maskValue = $(this).attr("placeholder");
        if (maskValue) {
          $(this).mask(maskValue);
        }
      });
      parent.Ts.Services.Customers.GetDateFormat(false, function (dateformat) {
          _dateFormat = dateformat;
          $('.datepicker').attr("data-format", dateformat);
          $('.datetimepicker').attr("data-format", dateformat + " hh:mm a");
          $('.datepicker').datetimepicker({ pickTime: false });
          $('.timepicker').datetimepicker({ pickDate: false });
          $('.datetimepicker').datetimepicker({});
      });
    });
  }
});