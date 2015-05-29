﻿var dateformat;
var _ticketGroupID = null;
var _ticketGroupUsers = null;
var _ticketTypeID = null;
var _parentFields = [];
var _productFamilyID = null;

var _timerid;
var _timerElapsed = 0;
var speed = 50, counter = 0, start;
var userFullName = top.Ts.System.User.FirstName + " " + top.Ts.System.User.LastName;

var clueTipOptions = top.Ts.Utils.getClueTipOptions(null);

var execGetCustomer = null;
var execGetTags = null;
var execGetAsset = null;
var execGetUsers = null;
var execGetRelated = null;
var execSelectTicket = null;

var getCustomers = function (request, response) {
  if (execGetCustomer) { execGetCustomer._executor.abort(); }
  execGetCustomer = top.Ts.Services.TicketPage.GetUserOrOrganizationForTicket(request, function (result) { response(result); });
}

var getTags = function (request, response) {
  if (execGetTags) { execGetTags._executor.abort(); }
  execGetTags = top.Ts.Services.Tickets.SearchTags(request.term, function (result) { response(result); });
}

var getAssets = function (request, response) {
  if (execGetAsset) { execGetAsset._executor.abort(); }
  execGetAsset = top.Ts.Services.Assets.FindAsset(request, function (result) { response(result); });
}

var getUsers = function (request, response) {
  if (execGetUsers) { execGetUsers._executor.abort(); }
  execGetUsers = top.Ts.Services.TicketPage.SearchUsers(request, function (result) { response(result); });
}

var getRelated = function (request, response) {
  if (execGetRelated) { execGetRelated._executor.abort(); }
  execGetRelated = top.Ts.Services.Tickets.SearchTickets(request, null, function (result) { response(result); });
}

var selectTicket = function (request, response) {
  if (execSelectTicket) { execSelectTicket._executor.abort(); }
  var filter = $(this.element).data('filter');
  if (filter === undefined) {
    execSelectTicket = top.Ts.Services.Tickets.SearchTickets(request.term, null, function (result) { response(result); });
  }
  else {
    execSelectTicket = top.Ts.Services.Tickets.SearchTickets(request.term, filter, function (result) { response(result); });
  }
}

var ellipseString = function (text, max) { return text.length > max - 3 ? text.substring(0, max - 3) + '...' : text; };

Selectize.define('sticky_placeholder', function (options) {
  var self = this;

  self.updatePlaceholder = (function () {
    var original = self.updatePlaceholder;
    return function () {
      original.apply(this, arguments);
      if (!this.settings.placeholder) return;
      var $input = this.$control_input;
      $input.attr('placeholder', this.settings.placeholder);
    };
  })();

});

$(document).ready(function () {
  LoadTicketPageOrder();
  SetupDescriptionEditor();

  $('.page-loading').hide().next().show();
});

function LoadTicketPageOrder() {
  top.Ts.Services.TicketPage.GetTicketPageOrder("NewTicketFieldsOrder", function (order) {
    jQuery.each(order, function (i, val) { if (val.Disabled == "false") AddTicketProperty(val); });
    SetupTicketProperties();
  });
};

function AddTicketProperty(item) {
  if ($("#ticket-group-" + item.CatID).length > 0) {
    var compiledTemplate = Handlebars.compile($("#ticket-group-" + item.CatID).html());
    $('#ticket-properties-area').append(compiledTemplate);
  }
};

function SetupTicketProperties() {
  //Assigned To
  var users = top.Ts.Cache.getUsers();
  for (var i = 0; i < users.length; i++) {
    AppendSelect('#ticket-assigned', users[i], 'assigned', users[i].UserID, users[i].Name + ' - ' + users[i].InOfficeComment);
  }

  $('#ticket-assigned').selectize({
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });

  //Group
  var groups = top.Ts.Cache.getGroups();
  for (var i = 0; i < groups.length; i++) {
    AppendSelect('#ticket-group', groups[i], 'group', groups[i].GroupID, groups[i].Name);
  }
  $('#ticket-group').selectize({
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });

  //Type
  var types = top.Ts.Cache.getTicketTypes();
  for (var i = 0; i < types.length; i++) {
    AppendSelect('#ticket-type', types[i], 'type', types[i].TicketTypeID, types[i].Name);
  }
  
  $('#ticket-type').selectize({
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });

  //Status
  SetupStatusField();

  //Severity
  var severities = top.Ts.Cache.getTicketSeverities();
  for (var i = 0; i < severities.length; i++) {
    AppendSelect('#ticket-severity', severities[i], 'severity', severities[i].TicketSeverityID, severities[i].Name);
  }
  $('#ticket-severity').selectize({
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });

  //KB
  SetupKBFields();

  //Community
  SetupCommunityField();

  //DueDate
  SetupDueDateField();

  //Customer Section
  SetupCustomerSection();

  //Product Section
  SetupProductSection();

  //Inventory Section
  SetupInventorySection();

  //Tags 
  SetupTagsSection();

  //Queues Section
  SetupUserQueuesSection();

  //Subscribers Section
  SetupSubscribedUsersSection();

  //Reminders
  SetupRemindersSection();

  //Associate Tickets
  SetupAssociatedTicketsSection();

  //TODO:  Custom Fields
  //SetupCustomFieldsSection();
  //AppenCustomValues();
};

function SetupDescriptionEditor() {
  initEditor($('#ticket-description'), true, function (ed) {
    SetupActionTypeSelect();
    $('#ticket-create').click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      SaveTicket(false);
    });

    $('#ticket-createandclose').click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      SaveTicket(true);
    });

    $('#ticket-cancel').click(function (e) {
      e.preventDefault();
      e.stopPropagation();
      if (confirm('Are you sure you would like to cancel this ticket?')) {
        clearTimeout(_timerid);
        _timerElapsed = 0;
        counter = 0;
        top.Ts.MainPage.closeNewTicketTab();
      }
      top.Ts.System.logAction('New Ticket - Canceled');
    });
  },
  function (ed) {
    $('#ticket-title-input').focus();
  });
};

function SetupActionTypeSelect() {
  var selectType = $('#action-new-type');
  selectType.empty();
  var types = top.Ts.Cache.getActionTypes();
  for (var i = 0; i < types.length; i++) {
    $('<option>').attr('value', types[i].ActionTypeID).text(types[i].Name).data('data', types[i]).appendTo(selectType);
  };

  $('#action-new-type').change(function (e) {
    var action = $(this).val();
    top.Ts.Services.TicketPage.GetActionTicketTemplate(action, function (result) {
      if (result != null && result != "" && result != "<br>") {
        var currenttext = tinyMCE.activeEditor.getContent();
        tinyMCE.activeEditor.setContent(currenttext + result);
      }
    });
  });
};

function SaveTicket(_doClose) {

};

function AppendSelect(parent, data, type, id, name, isSelected) {
  var option = $('<option>').val(id).text(name).appendTo(parent).data(type, data);
  if (isSelected) {
    option.attr('selected', 'selected');
  }
};

function SetupStatusField() {
  var statuses = top.Ts.Cache.getTicketStatuses();
  $("#ticket-status").selectize({
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    },
    render: {
      item: function (item, escape) {
        if (item.data.IsClosed) {
          return '<div data-value="' + escape(item.value) + '" data-item="' + escape(item.data) + '" data-selectable="" class="option"><s>' + escape(item.text) + '</s></div>';
        }
        else {
          return '<div data-value="' + escape(item.value) + '" data-item="' + escape(item.data) + '" data-selectable="" class="option">' + escape(item.text) + '</div>';
        }
      }
    },
  });
  var selectize = $("#ticket-status")[0].selectize;
  selectize.clear(true);
  selectize.clearOptions();

  for (var i = 0; i < statuses.length; i++) {
    selectize.addOption({ value: statuses[i].TicketStatusID, text: statuses[i].Name, data: statuses[i] });
  }
}

function SetupKBFields() {
  if (top.Ts.System.User.ChangeKbVisibility || top.Ts.System.User.IsSystemAdmin) {
    var categories = top.Ts.Cache.getKnowledgeBaseCategories();
    for (var i = 0; i < categories.length; i++) {
      var cat = categories[i].Category;
      AppendSelect('#ticket-KB-Category', cat, 'category', cat.CategoryID, cat.CategoryName);

      for (var j = 0; j < categories[i].Subcategories.length; j++) {
        var subcat = categories[i].Subcategories[j];
        AppendSelect('#ticket-KB-Category', subcat, 'subcategory', subcat.CategoryID, cat.CategoryName + ' -> ' + subcat.CategoryName);
      }
    }

    $('#ticket-KB-Category').selectize({
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      }
    });
  }
  else {
    $('#ticket-KBInfo').remove();
  }
}

function SetupCommunityField() {
  if (top.Ts.System.Organization.UseForums == true) {
    if (top.Ts.System.User.CanChangeCommunityVisibility) {
      var forumCategories = top.Ts.Cache.getForumCategories();
      for (var i = 0; i < forumCategories.length; i++) {
        var cat = forumCategories[i].Category;
        AppendSelect('#ticket-Community', cat, 'community', cat.CategoryID, cat.CategoryName, false);

        for (var j = 0; j < forumCategories[i].Subcategories.length; j++) {
          var subcat = forumCategories[i].Subcategories[j];
          AppendSelect('#ticket-Community', subcat, 'subcategory', subcat.CategoryID, cat.CategoryName + ' -> ' + subcat.CategoryName, false);
        }
      }

      $('#ticket-Community').selectize({
        onDropdownClose: function ($dropdown) {
          $($dropdown).prev().find('input').blur();
        }
      });
    }
    else {
      $('#ticket-Community').closest('.form-horizontal').remove();
    }
  }
  else {
    $('#ticket-Community').closest('.form-horizontal').remove();
  }
};

function SetupDueDateField(duedate) {
  var dateContainer = $('#ticket-duedate-container');
  var dateLink = $('<a>')
                      .attr('href', '#')
                      //.text('')
                      .addClass('control-label ticket-anchor ticket-nullable-link ticket-duedate-anchor')
                      .appendTo(dateContainer);

  dateLink.click(function (e) {
    e.preventDefault();
    $(this).hide();
    var input = $('<input type="text">')
                    .addClass('form-control')
                    //.val('')
                    .datetimepicker({
                      showClear: true,
                      sideBySide: true
                    })
                    .appendTo(dateContainer)
                    .focus();

    input.focusout(function (e) {
      var value = top.Ts.Utils.getMsDate(input.val());
      input.remove();
      dateLink.text((value === null ? 'Unassigned' : value.localeFormat(top.Ts.Utils.getDateTimePattern()))).show();
    })
  });
};

function AddTags(tag) {
  var tagDiv = $("#ticket-tags");
  $("#ticket-tag-Input").val('');
  PrependTag(tagDiv, tag.id, tag.value, tag);
}

function SetupTagsSection() {
  $("#ticket-tag-Input").autocomplete({
    minLength: 2,
    source: getTags,
    response: function (event, ui) {
      var inputValue = $(this).val();

      var filtered = $(ui.content).filter(function () {
        return this.value == inputValue;
      });

      if (filtered.length === 0) {
        ui.content.push({
          label: inputValue,
          value: inputValue,
          id: 0
        });
      }
    },
    select: function (event, ui) {
      $(this).data('item', ui.item)
      AddTags(ui.item);
      this.removeItem(ui.item.value, true);
      top.Ts.System.logAction('Ticket - Added');
    }
  })
  .data("autocomplete")._renderItem = function (ul, item) {
    return $("<li>")
        .append("<a>" + item.label + "</a>")
        .appendTo(ul);
  };

  $('#ticket-tags').on('click', 'span.tagRemove', function (e) {
    var tag = $(this).parent()[0];
    tag.remove();

  });

};

function PrependTag(parent, id, value, data, cssclass) {
  if (cssclass === undefined) cssclass = 'tag-item';
  var _compiledTagTemplate = Handlebars.compile($("#ticket-tag").html());
  var tagHTML = _compiledTagTemplate({ id: id, value: value, data: data, css: cssclass });
  return $(tagHTML).prependTo(parent);
}

function SetupCustomerSection() {
  $('#ticket-Customers-Input').selectize({
    valueField: 'id',
    labelField: 'label',
    searchField: 'label',
    load: function (query, callback) {
      getCustomers(query, callback)
    },
    preload: true,
    create: function (input, callback) {
      $('#NewCustomerModal').modal('show');
      callback(null);
    },
    plugins: {
      'sticky_placeholder': {}
    },
    onItemAdd: function (value, $item) {
      AddCustomers($item.data());
      this.removeItem(value, true);
    },
    render: {
      item: function (item, escape) {
        return '<div data-value="' + item.value + '" data-type="' + item.data + '" data-selectable="" class="option">' + item.label + '</div>';
      },
      option: function (item, escape) {
        return '<div data-value="' + escape(item.value) + '" data-type="' + escape(item.data) + '" data-selectable="" class="option">' + item.label + '</div>';
      },
      option_create: function (data, escape) {
        return '<div class="create">Create <strong>' + escape(data.input) + '</strong></div>';
      }
    },
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });

  $('#Customer-Create').click(function (e) {
    e.preventDefault();
    e.stopPropagation();
    top.Ts.System.logAction('Ticket - New Customer Added');
    var email = $('#customer-email-input').val();
    var firstName = $('#customer-fname-input').val();
    var lastName = $('#customer-lname-input').val();
    var phone = $('#customer-phone-input').val();;
    var companyName = $('#customer-company-input').val();
    top.Ts.Services.Users.CreateNewContact(email, firstName, lastName, companyName, phone, false, function (result) {
      if (result.indexOf("u") == 0 || result.indexOf("o") == 0) {
        var customerData = new Object();
        customerData.type = result.substring(0, 1);
        customerData.value = result.substring(1);
        AddCustomers(customerData)
      }
      else if (result.indexOf("The company you have specified is invalid") !== -1) {
        if (top.Ts.System.User.CanCreateCompany || top.Ts.System.User.IsSystemAdmin) {
          if (confirm('Unknown company, would you like to create it?')) {
            top.Ts.Services.Users.CreateNewContact(email, firstName, lastName, companyName, phone, true, function (result) {
              var customerData = new Object();
              customerData.type = result.substring(0, 1);
              customerData.value = result.substring(1);
              AddCustomers(customerData)
                $('.ticket-new-customer-email').val('');
                $('.ticket-new-customer-first').val('');
                $('.ticket-new-customer-last').val('');
                $('.ticket-new-customer-company').val('');
                $('.ticket-new-customer-phone').val('');
                $('#NewCustomerModal').modal('hide');
            });
          }
        }
        else {
          alert("We're sorry, but you do not have the rights to create a new company.");
          $('.ticket-new-customer-email').val('');
          $('.ticket-new-customer-first').val('');
          $('.ticket-new-customer-last').val('');
          $('.ticket-new-customer-company').val('');
          $('.ticket-new-customer-phone').val('');
          $('#NewCustomerModal').modal('hide');
        }
      }
      else {
        alert(result);
      }
    });
  });

  $('#ticket-Customer').on('click', 'span.tagRemove', function (e) {
    var self = $(this);
  });
};

function AddCustomers(customerdata) {
  top.Ts.Services.Tickets.GetTicketCustomer(customerdata.type, customerdata.value, function (result) {
    var customer = result;
    if (customer == null) return;
    top.Ts.System.logAction('New Ticket - Customer Added');
    var customerDiv = $("#ticket-Customer");
    //customerDiv.empty();
    $("#ticket-Customers-Input").val('');
    var label = "";

    if (customer.Contact !== null && customer.Company !== null) {
      label = customer.Contact + '<br/>' + customer.Company;
    }
    else if (customer.Contact !== null) {
      label = customer.Contact;
    }
    else if (customer.Company !== null) {
      label = customer.Company;
    }

    var cssClasses = "tag-item";

    if (customer.Flag) {
      cssClasses = cssClasses + " tag-error"
    }

    if (customer.UserID !== null) {
      cssClasses = cssClasses + ' UserAnchor';
      var newelement = PrependTag(customerDiv, customer.UserID, label, customer, cssClasses);
      newelement.data('userid', customer.UserID).data('placement', 'left').data('ticketid', 0);
    }
    else {
      cssClasses = cssClasses + ' OrgAnchor';
      var newelement = PrependTag(customerDiv, customer.OrganizationID, label, customer, cssClasses);
      newelement.data('orgid', customer.OrganizationID).data('placement', 'left').data('ticketid', 0);
    }
  });
};

function SetupProductSection() {
  top.Ts.Settings.Organization.read('ShowOnlyCustomerProducts', false, function (showOnlyCustomers) {
    if (showOnlyCustomers == "True") {
      top.Ts.Services.TicketPage.GetTicketCustomerProducts(_ticketID, function (CustomerProducts) {
        LoadProductList(CustomerProducts);
      });
    }
    else {
      var products = top.Ts.Cache.getProducts();
      LoadProductList(products);
    }

    top.Ts.Services.Organizations.IsProductRequired(function (result) {
      if (result)
        $('#ticket-Product').closest('.form-group').addClass('hasError');
      else
        $('#ticket-Product').closest('.form-group').removeClass('hasError');
    });

    $('#ticket-Product').change(function (e) {
      var self = $(this);
      var product = top.Ts.Cache.getProduct(self.val());
      loadVersions(product);
      AppenCustomValues();
      top.Ts.Services.Organizations.IsProductRequired(function (IsRequired) {
        if (IsRequired)
          $('#ticket-Product').closest('.form-group').addClass('hasError');
        else
          $('#ticket-Product').closest('.form-group').removeClass('hasError');
      });
    });

  })
};

function LoadProductList(products) {
  top.Ts.Services.Settings.ReadUserSetting('SelectedProductID', -1, function (productID) {
    if (products == null) products = top.Ts.Cache.getProducts();
    var product = top.Ts.Cache.getProduct(productID);
    for (var i = 0; i < products.length; i++) {
      AppendSelect('#ticket-Product', products[i], 'product', products[i].ProductID, products[i].Name, products[i].ProductID == productID);
    }

    var $productselect = $('#ticket-Product').selectize({
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      }
    });

    SetupProductVersionsControl(product);
    SetProductVersionAndResolved(null, null);

    if (productID == -1) {
      var $productselectInput = $productselect[0].selectize;
      $productselectInput.clear();
    }
  });
}

function loadVersions(product) {
  var selectizeVersion = $("#ticket-Versions")[0].selectize;
  selectizeVersion.clear(true);
  selectizeVersion.clearOptions();

  var selectizeResolved = $("#ticket-Resolved")[0].selectize;
  selectizeResolved.clear(true);
  selectizeResolved.clearOptions();

  if (product !== null) {
    var versions = product.Versions;

    for (var i = 0; i < versions.length; i++) {
      selectizeVersion.addOption({ value: versions[i].ProductVersionID, text: versions[i].VersionNumber, data: versions[i] });
      selectizeResolved.addOption({ value: versions[i].ProductVersionID, text: versions[i].VersionNumber, data: versions[i] });
    }
  }
}

function SetupProductVersionsControl(product) {
  var $select = $("#ticket-Versions").selectize({
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });
  var versionInput = $select[0].selectize;

  if (versionInput) {
    versionInput.destroy();
  }

  var $select = $("#ticket-Resolved").selectize({
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });
  var resolvedInput = $select[0].selectize;

  if (resolvedInput) {
    resolvedInput.destroy();
  }

  if (product !== null && product.Versions.length > 0) {
    var versions = product.Versions;
    for (var i = 0; i < versions.length; i++) {
      AppendSelect('#ticket-Versions', versions[i], 'version', versions[i].ProductVersionID, versions[i].VersionNumber, false);
      AppendSelect('#ticket-Resolved', versions[i], 'resolved', versions[i].ProductVersionID, versions[i].VersionNumber, false);
    }
    $('#ticket-Versions').selectize({
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      }
    });
    $('#ticket-Resolved').selectize({
      onDropdownClose: function ($dropdown) {
        $($dropdown).prev().find('input').blur();
      }
    });
  }
}

function SetProductVersionAndResolved(versionId, resolvedId) {
  var $select = $("#ticket-Versions").selectize({
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });
  var versionInput = $select[0].selectize;

  if (versionId !== null) {
    versionInput.setValue(versionId);
  }
  else {
    versionInput.clear();
  }

  var $select = $("#ticket-Resolved").selectize({
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });
  var resolvedInput = $select[0].selectize;

  if (resolvedId !== null) {
    resolvedInput.setValue(resolvedId);
  }
  else {
    resolvedInput.clear();
  }
};

function SetupInventorySection() {
  $('#ticket-Inventory-Input').selectize({
    valueField: 'id',
    labelField: 'label',
    searchField: 'label',
    load: function (query, callback) {
      getAssets(query, callback)
    },
    onItemAdd: function (value, $item) {
      AddInventory(value);
      this.removeItem(value, true);
    },
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    },

    plugins: {
      'sticky_placeholder': {}
    }
  });
};

function AddInventory(Inventory) {
  top.Ts.Services.Assets.GetAsset(Inventory, function (asset) {
    var InventoryDiv = $("#ticket-Inventory");
    $("#ticket-Inventory-Input").val('');

    var newelement = PrependTag(InventoryDiv, asset.AssetID, ellipseString(asset.Name, 30), asset, "tag-item AssetAnchor");
    newelement.data('assetid', asset.AssetID).data('placement', 'left');
  });
};

function SetupUserQueuesSection() {
  $('#ticket-UserQueue-Input').selectize({
    valueField: 'id',
    labelField: 'label',
    searchField: 'label',
    load: function (query, callback) {
      getUsers(query, callback)
    },
    onItemAdd: function (value, $item) {
      var item = new Object();
      item.name = $item.text();
      item.id = value;
      AddQueues(item);
      this.removeItem(value, true);
    },
    plugins: {
      'sticky_placeholder': {}
    },
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });
}

function AddQueues(queues) {
  var UserQueueDiv = $("#ticket-UserQueue");
  $("#ticket-UserQueue-Input").val('');

  var newelement = PrependTag(UserQueueDiv, queues.id, queues.name, queues, "tag-item UserAnchor");
  newelement.data('userid', queues.id).data('placement', 'left').data('ticketid', 0);
}

function SetupSubscribedUsersSection() {
  $('#ticket-SubscribedUsers-Input').selectize({
    valueField: 'id',
    labelField: 'label',
    searchField: 'label',
    load: function (query, callback) {
      getUsers(query, callback)
    },
    onItemAdd: function (value, $item) {
      var item = new Object();
      item.name = $item.text();
      item.id = value;
      AddSubscribers(item);
      this.removeItem(value, true);
    },
    plugins: {
      'sticky_placeholder': {}
    },
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });
};

function AddSubscribers(Subscribers) {
  var SubscribersDiv = $("#ticket-SubscribedUsers");
  $("#ticket-SubscribedUsers-Input").val('');

  var newelement = PrependTag(SubscribersDiv, Subscribers.id, Subscribers.name, Subscribers, "tag-item UserAnchor");
  newelement.data('userid', Subscribers.id).data('placement', 'left').data('ticketid', 0);
}

function SetupRemindersSection() {
  $('#ticket-reminder-date').datetimepicker({ useCurrent: true, format: 'MM/DD/YYYY hh:mm A', defaultDate: new Date() });

  var $reminderSelect = $('#ticket-reminder-who').selectize({
    valueField: 'id',
    labelField: 'label',
    searchField: 'label',
    load: function (query, callback) {
      top.Ts.Services.TicketPage.SearchUsers(query, function (result) {
        callback(result);
      });

    },
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });

  $('#ticket-reminder-save').click(function (e) {
    var selectizeControl = $reminderSelect[0].selectize;
    var date = top.Ts.Utils.getMsDate($('#ticket-reminder-date').val());
    var userid = selectizeControl.getValue();
    if (userid == "") {
      $('#ticket-reminder-who').parent().addClass('has-error').removeClass('has-success');
    }
    else {
      $('#ticket-reminder-who').closest('form-group').addClass('has-success').removeClass('has-error');
    }
    var title = $('#ticket-reminder-title').val();
    if (title == "") {
      $('#ticket-reminder-title').parent().addClass('has-error').removeClass('has-success');
    }
    else {
      $('#ticket-reminder-title').parent().addClass('has-success').removeClass('has-error');
    }

    $('#reminder-success').show();
    var label = ellipseString(title, 30) + '<br>' + date.localeFormat(top.Ts.Utils.getDateTimePattern())
    PrependTag($("#ticket-reminder-span"), userid, label, null);
    setTimeout(function () { $('#RemindersModal').modal('hide'); }, 2000);
  });
}

function SetupAssociatedTicketsSection() {
  $('#ticket-AssociatedTickets-Input').selectize({
    valueField: 'data',
    labelField: 'label',
    searchField: 'label',
    load: function (query, callback) {
      getRelated(query, callback)
    },
    onItemAdd: function (value, $item) {
      $('#AssociateTicketModal').data('ticketid', value).modal('show');
      this.removeItem(value, true);
    },
    plugins: {
      'sticky_placeholder': {}
    },
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });

  $('#ticket-AssociatedTickets').on('click', 'div.tag-item', function (e) {
    var self = $(this);
    var data = self.data().tag;
    top.Ts.MainPage.openTicket(data.TicketNumber, true);
  });

  $('.ticket-association').click(function (e) {
    var IsParent = $(this).data('isparent');
    var TicketID2 = $(this).closest('#AssociateTicketModal').data('ticketid');
    $('#associate-error').hide();
    $('#associate-success').show();

    //TODO: Need add method
    $("#ticket-AssociatedTickets-Input").val('');
    AddAssociatedTickets(TicketID2, IsParent);
    setTimeout(function () { $('#AssociateTicketModal').modal('hide'); }, 2000);
  });
};

function AddAssociatedTickets(ticketid, IsParent) {
  top.Ts.Services.Tickets.GetTicket(ticketid, function (ticket) {
    if (ticket !== null) {
      var AssociatedTicketsDiv = $("#ticket-AssociatedTickets");
      if (IsParent !== null) {
        caption = (IsParent === true ? 'Parent' : 'Child');
      }
      var label = caption + "<br />" + ellipseString(ticket.TicketNumber + ': ' + ticket.Name, 30);

      var newelement = PrependTag(AssociatedTicketsDiv, ticket.TicketID, ticket.IsClosed ? '<s>' + label + '</s>' : label, ticket, 'tag-item TicketAnchor');
      newelement.data('ticketid', related.TicketID).data('placement', 'left');
    }
  });
};

function AppenCustomValues() {
  var ticketTypeID = $('#ticket-type').val();
  var productID = $('#ticket-Product').val();
  if (ticketTypeID == undefined || ticketTypeID == "") ticketTypeID = -1;
  if (productID == undefined || productID == "") productID = -1;

  top.Ts.Services.CustomFields.GetProductMatchingCustomFields(top.Ts.ReferenceTypes.Tickets, ticketTypeID, productID, function (fields) {
    debugger
    var parentContainer = $('#ticket-group-custom-fields');
    if (fields === null || fields.length < 1) { parentContainer.empty().hide(); return; }
    parentContainer.empty()
    parentContainer.show();
    _parentFields = [];

    for (var i = 0; i < fields.length; i++) {
      var field = fields[i];

      if (field.CustomFieldCategoryID == null) {
        switch (field.FieldType) {
          case top.Ts.CustomFieldType.Text: AddCustomFieldEdit(field, parentContainer); break;
          case top.Ts.CustomFieldType.Date: AddCustomFieldDate(field, parentContainer); break;
          case top.Ts.CustomFieldType.Time: AddCustomFieldTime(field, parentContainer); break;
          case top.Ts.CustomFieldType.DateTime: AddCustomFieldDateTime(field, parentContainer); break;
          case top.Ts.CustomFieldType.Boolean: AddCustomFieldBool(field, parentContainer); break;
          case top.Ts.CustomFieldType.Number: AddCustomFieldNumber(field, parentContainer); break;
          case top.Ts.CustomFieldType.PickList: AddCustomFieldSelect(field, parentContainer, false); break;
          default:
        }
      }
    }
    appendCategorizedCustomValues(fields);
  });
}

var appendCategorizedCustomValues = function (fields) {
  var ticketTypeID = $('#ticket-type').val();
  top.Ts.Services.CustomFields.GetCategories(top.Ts.ReferenceTypes.Tickets, ticketTypeID, function (categories) {
    var container = $('#ticket-group-custom-fields');
    for (var j = 0; j < categories.length; j++) {
      var isFirstFieldAdded = true;
      for (var i = 0; i < fields.length; i++) {
        var item = null;

        var field = fields[i];

        if (field.CustomFieldCategoryID == categories[j].CustomFieldCategoryID) {
          if (isFirstFieldAdded) {
            isFirstFieldAdded = false;
            var header = $('<label>').text(categories[j].Category).addClass('customFieldCategoryHeader');
            container.append(header);
          }

          switch (field.FieldType) {
            case top.Ts.CustomFieldType.Text: AddCustomFieldEdit(field, container); break;
            case top.Ts.CustomFieldType.Date: AddCustomFieldDate(field, container); break;
            case top.Ts.CustomFieldType.Time: AddCustomFieldTime(field, container); break;
            case top.Ts.CustomFieldType.DateTime: AddCustomFieldDateTime(field, container); break;
            case top.Ts.CustomFieldType.Boolean: AddCustomFieldBool(field, container); break;
            case top.Ts.CustomFieldType.Number: AddCustomFieldNumber(field, container); break;
            case top.Ts.CustomFieldType.PickList: AddCustomFieldSelect(field, container, false); break;
            default:
          }
        }
      }
    }
    appendConditionalFields();
  });
}

var appendConditionalFields = function () {
  for (var i = 0; i < _parentFields.length; i++) {
    var field = _parentFields[i].data('field');
    $('.' + field.CustomFieldID + 'children').remove();
    var parentContainer = $('#ticket-group-custom-fields');
    var childrenContainer = $('<div>').addClass(field.CustomFieldID + 'children form-horizontal').appendTo(parentContainer);
    appendMatchingParentValueFields(childrenContainer, field);
  }
}

var appendMatchingParentValueFields = function (container, parentField) {
  //top.Ts.Services.Tickets.GetMatchingParentValueFields(_ticketID, parentField.CustomFieldID, parentField.Value, function (fields) {
  //  for (var i = 0; i < fields.length; i++) {
  //    var field = fields[i];
  //    var div = $('<div>').addClass('form-group form-group-sm').data('field', field);
  //    $('<label>').addClass('col-sm-4 control-label select-label').text(field.Name).appendTo(div);

  //    container.append(div);

  //    switch (field.FieldType) {
  //      case top.Ts.CustomFieldType.Text: AddCustomFieldEdit(field, div); break;
  //      case top.Ts.CustomFieldType.Date: AddCustomFieldDate(field, div); break;
  //      case top.Ts.CustomFieldType.Time: AddCustomFieldTime(field, div); break;
  //      case top.Ts.CustomFieldType.DateTime: AddCustomFieldDateTime(field, div); break;
  //      case top.Ts.CustomFieldType.Boolean: AddCustomFieldBool(field, div); break;
  //      case top.Ts.CustomFieldType.Number: AddCustomFieldNumber(field, div); break;
  //      case top.Ts.CustomFieldType.PickList: AddCustomFieldSelect(field, div, true); break;
  //      default:
  //    }
  //  }
  //});
}

var AddCustomFieldEdit = function (field, parentContainer) {
  var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
  var groupContainer = $('<div>').addClass('form-group form-group-sm')
                          .data('field', field)
                          .appendTo(formcontainer)
                          .append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
  var inputContainer = $('<div>').addClass('col-sm-8 ticket-input-container').appendTo(groupContainer);
  var inputGroupContainer = $('<div>').addClass('input-group').appendTo(inputContainer);
  var input = $('<input type="text">')
                  .addClass('form-control ticket-simple-input')
                  .val(field.Value)
                  .appendTo(inputGroupContainer)
                  .after(getUrls(field.Value));


  if (field.Mask) {
    input.mask(field.Mask);
    input.attr("placeholder", field.Mask);
  }

  input.change(function (e) {
    var value = input.val();

    if (field.IsRequired && (value === null || $.trim(value) === '')) {
      groupContainer.addClass('hasError');
    }
    else {
      groupContainer.removeClass('hasError');
    }
    if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
      groupContainer.addClass('hasCloseError');
      alert("This field can not be cleared in a closed ticket");
      return;
    }
    else {
      groupContainer.removeClass('hasCloseError');
    }
    if (value === null || $.trim(value) === '') {
      groupContainer.addClass('isEmpty');
    }
    else {
      groupContainer.removeClass('isEmpty');
    }
  });

  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    groupContainer.addClass('hasError');
  }
  if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
    groupContainer.addClass('hasCloseError');
  }
  if (field.IsRequiredToClose) {
    groupContainer.addClass('isRequiredToClose');
  }
  if (field.Value === null || $.trim(field.Value) === '') {
    groupContainer.addClass('isEmpty');
  }
}

var AddCustomFieldDate = function (field, parentContainer) {
  var date = field.Value == null ? null : top.Ts.Utils.getMsDate(field.Value);
  var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
  var groupContainer = $('<div>').addClass('form-group form-group-sm').data('field', field).appendTo(formcontainer).append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
  var dateContainer = $('<div>').addClass('col-sm-8 ticket-input-container').attr('style', 'padding-top: 3px;').appendTo(groupContainer);
  var dateLink = $('<a>')
                      .attr('href', '#')
                      .text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDatePattern())))
                      .addClass('control-label')
                      .attr('style', 'padding-left: 5px;')
                      .appendTo(dateContainer);

  dateLink.click(function (e) {
    e.preventDefault();
    $(this).hide();
    var input = $('<input type="text">')
                    .addClass('form-control')
                    .val(date === null ? '' : date.localeFormat(top.Ts.Utils.getDatePattern()))
                    .datetimepicker({ pickTime: false })
                    .appendTo(dateContainer)
                    .focus();

    input.focusout(function (e) {
      var value = top.Ts.Utils.getMsDate(input.val());
      this.remove();
      dateLink.text((value === null ? 'Unassigned' : value.localeFormat(top.Ts.Utils.getDatePattern()))).show();

      if (field.IsRequired && (value === null || $.trim(value) === '')) {
        groupContainer.addClass('hasError');
      }
      else {
        groupContainer.removeClass('hasError');
      }
      if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
        groupContainer.addClass('hasCloseErrory');
        alert("This field can not be cleared in a closed ticket");
        return;
      }
      else {
        groupContainer.removeClass('hasCloseErrory');
      }
      if (value === null || $.trim(value) === '') {
        groupContainer.addClass('isEmpty');
      }
      else {
        groupContainer.removeClass('isEmpty');
      }
    })
  });

  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    groupContainer.addClass('hasError');
  }
  if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
    groupContainer.addClass('hasCloseError');
  }
  if (field.IsRequiredToClose) {
    groupContainer.addClass('isRequiredToClose');
  }
  if (field.Value === null || $.trim(field.Value) === '') {
    groupContainer.addClass('isEmpty');
  }
}

var AddCustomFieldDateTime = function (field, parentContainer) {
  var date = field.Value == null ? null : top.Ts.Utils.getMsDate(field.Value);
  var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
  var groupContainer = $('<div>').addClass('form-group form-group-sm').data('field', field).appendTo(formcontainer).append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
  var dateContainer = $('<div>').addClass('col-sm-8 ticket-input-container').attr('style', 'padding-top: 3px;').appendTo(groupContainer);
  var dateLink = $('<a>')
                      .attr('href', '#')
                      .text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getDateTimePattern())))
                      .addClass('control-label')
                      .attr('style', 'padding-left: 5px;')
                      .appendTo(dateContainer);

  dateLink.click(function (e) {
    e.preventDefault();
    $(this).hide();
    var input = $('<input type="text">')
                    .addClass('form-control')
                    .val(date === null ? '' : date.localeFormat(top.Ts.Utils.getDateTimePattern()))
                    .datetimepicker()
                    .appendTo(dateContainer)
                    .focus();

    input.focusout(function (e) {
      var value = top.Ts.Utils.getMsDate(input.val());
      this.remove();
      dateLink.text((value === null ? 'Unassigned' : value.localeFormat(top.Ts.Utils.getDateTimePattern()))).show();

      if (field.IsRequired && (value === null || $.trim(value) === '')) {
        groupContainer.addClass('hasError');
      }
      else {
        groupContainer.removeClass('hasError');
      }
      if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
        groupContainer.addClass('hasCloseErrory');
        alert("This field can not be cleared in a closed ticket");
        return;
      }
      else {
        groupContainer.removeClass('hasCloseErrory');
      }
      if (value === null || $.trim(value) === '') {
        groupContainer.addClass('isEmpty');
      }
      else {
        groupContainer.removeClass('isEmpty');
      }
    })
  });

  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    groupContainer.addClass('hasError');
  }
  if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
    groupContainer.addClass('hasCloseError');
  }
  if (field.IsRequiredToClose) {
    groupContainer.addClass('isRequiredToClose');
  }
  if (field.Value === null || $.trim(field.Value) === '') {
    groupContainer.addClass('isEmpty');
  }
}

var AddCustomFieldTime = function (field, parentContainer) {
  var date = field.Value == null ? null : top.Ts.Utils.getMsDate(field.Value);
  var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
  var groupContainer = $('<div>').addClass('form-group form-group-sm').data('field', field).appendTo(formcontainer).append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
  var dateContainer = $('<div>').addClass('col-sm-8 ticket-input-container').attr('style', 'padding-top: 3px;').appendTo(groupContainer);
  var dateLink = $('<a>')
                      .attr('href', '#')
                      .text((date === null ? 'Unassigned' : date.localeFormat(top.Ts.Utils.getTimePattern())))
                      .addClass('control-label')
                      .attr('style', 'padding-left: 5px;')
                      .appendTo(dateContainer);

  dateLink.click(function (e) {
    e.preventDefault();
    $(this).hide();
    var input = $('<input type="text">')
                    .addClass('form-control')
                    .val(date === null ? '' : date.localeFormat(top.Ts.Utils.getTimePattern()))
                    .datetimepicker({ pickDate: false })
                    .appendTo(dateContainer)
                    .focus();

    input.focusout(function (e) {
      var value = top.Ts.Utils.getMsDate("1/1/1900 " + input.val());
      this.remove();
      dateLink.text((value === null ? 'Unassigned' : value.localeFormat(top.Ts.Utils.getTimePattern()))).show();

      if (field.IsRequired && (value === null || $.trim(value) === '')) {
        groupContainer.addClass('hasError');
      }
      else {
        groupContainer.removeClass('hasError');
      }
      if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
        groupContainer.addClass('hasCloseErrory');
        alert("This field can not be cleared in a closed ticket");
        return;
      }
      else {
        groupContainer.removeClass('hasCloseErrory');
      }
      if (value === null || $.trim(value) === '') {
        groupContainer.addClass('isEmpty');
      }
      else {
        groupContainer.removeClass('isEmpty');
      }
    })
  });

  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    groupContainer.addClass('hasError');
  }
  if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
    groupContainer.addClass('hasCloseError');
  }
  if (field.IsRequiredToClose) {
    groupContainer.addClass('isRequiredToClose');
  }
  if (field.Value === null || $.trim(field.Value) === '') {
    groupContainer.addClass('isEmpty');
  }
}

var AddCustomFieldBool = function (field, parentContainer) {
  var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
  var groupContainer = $('<div>')
                          .addClass('form-group form-group-sm')
                          .data('field', field)
                          .appendTo(formcontainer)
                          .append($('<label>').addClass('col-sm-4 control-label').text(field.Name));
  var inputContainer = $('<div>').addClass('col-sm-8 ticket-input-container').appendTo(groupContainer);
  var inputDiv = $('<div>').addClass('checkbox ticket-checkbox').appendTo(inputContainer);
  var input = $('<input type="checkbox">').appendTo(inputDiv);
  var value = (field.Value === null || $.trim(field.Value) === '' || field.Value === 'False' ? false : true);
  input.attr("checked", value);

  input.change(function (e) {
    var isChecked = input.is(':checked');
  });
}

var AddCustomFieldNumber = function (field, parentContainer) {
  var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
  var groupContainer = $('<div>').addClass('form-group form-group-sm').data('field', field).appendTo(formcontainer).append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
  var inputContainer = $('<div>').addClass('col-sm-8 ticket-input-container').appendTo(groupContainer);
  var input = $('<input type="text">')
                  .addClass('form-control ticket-simple-input')
                  .val(field.Value)
                  .appendTo(inputContainer)
                  .numeric();

  input.change(function (e) {
    var value = input.val();

    if (field.IsRequired && (value === null || $.trim(value) === '')) {
      groupContainer.addClass('hasError');
    }
    else {
      groupContainer.removeClass('hasError');
    }
    if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (value === null || $.trim(value) === '')) {
      groupContainer.addClass('hasCloseError');
      alert("This field can not be cleared in a closed ticket");
      return;
    }
    else {
      groupContainer.removeClass('hasCloseError');
    }
    if (value === null || $.trim(value) === '') {
      groupContainer.addClass('isEmpty');;
    }
    else {
      groupContainer.removeClass('isEmpty');
    }
  });

  if (field.IsRequired && (field.Value === null || $.trim(field.Value) === '')) {
    groupContainer.addClass('hasError');
  }
  if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && (field.Value === null || $.trim(field.Value) === '')) {
    groupContainer.addClass('hasCloseError');
  }
  if (field.IsRequiredToClose) {
    groupContainer.addClass('isRequiredToClose');
  }
  if (field.Value === null || $.trim(field.Value) === '') {
    groupContainer.addClass('isEmpty');
  }
}

var AddCustomFieldSelect = function (field, parentContainer, loadConditionalFields) {
  var formcontainer = $('<div>').addClass('form-horizontal').appendTo(parentContainer);
  var groupContainer = $('<div>').addClass('form-group form-group-sm').data('field', field).appendTo(formcontainer).append($('<label>').addClass('col-sm-4 control-label select-label').text(field.Name));
  var selectContainer = $('<div>').addClass('col-sm-8 ticket-input-container').appendTo(groupContainer);
  var select = $('<select>').addClass('hidden-select').appendTo(selectContainer);
  var options = field.ListValues.split('|');

  if (field.Value == "") {
    $('<option>').text("unassigned").val("").appendTo(select);
    if (field.IsRequired) groupContainer.addClass('hasError');

  }
  for (var i = 0; i < options.length; i++) {
    var optionValue = options[i];
    var option = $('<option>').text(optionValue).val(optionValue).appendTo(select);
    if (field.Value === options[i]) option.attr('selected', 'selected');
  }
  select.selectize({
    allowEmptyOption: true,
    onItemAdd: function (value, $item) {
      if (field.IsRequired && field.IsFirstIndexSelect == true && value == "") {
        groupContainer.addClass('hasError');
      }
      else {
        groupContainer.removeClass('hasError');
      }

      if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && field.IsFirstIndexSelect == true && value == "") {
        groupContainer.addClass('hasCloseError');
        alert("This field can not be cleared in a closed ticket");
        return;
      }
      else {
        groupContainer.removeClass('hasCloseError');
      }

      if (field.IsFirstIndexSelect == true && value == "") {
        groupContainer.addClass('isEmpty');
      }
      else {
        groupContainer.removeClass('isEmpty');
      }

      $('.' + field.CustomFieldID + 'children').remove();
      var childrenContainer = $('<div>').addClass(field.CustomFieldID + 'children form-horizontal').appendTo(parentContainer);
      //TODO:  need to test if this workds
      appendMatchingParentValueFields(childrenContainer, field);
    },
    onDropdownClose: function ($dropdown) {
      $($dropdown).prev().find('input').blur();
    }
  });

  var items = field.ListValues.split('|');
  if (field.IsRequired && ((field.IsFirstIndexSelect == true && (items[0] == field.Value || field.Value == null || $.trim(field.Value) === '')) || (field.Value == null || $.trim(field.Value) === ''))) {
    groupContainer.addClass('hasError');
  }
  if (field.IsRequiredToClose && $('.ticket-closed').length > 0 && ((field.IsFirstIndexSelect == true && (items[0] == field.Value || field.Value == null || $.trim(field.Value) === '')) || (field.Value == null || $.trim(field.Value) === ''))) {
    groupContainer.addClass('hasCloseError');
  }
  if (field.IsRequiredToClose) {
    groupContainer.addClass('isRequiredToClose');
  }
  if ((field.IsFirstIndexSelect == true && items[0] == field.Value) || field.Value == null || $.trim(field.Value) === '') {
    groupContainer.addClass('isEmpty');
  }

  if (loadConditionalFields) {
    $('.' + field.CustomFieldID + 'children').remove();
    var childrenContainer = $('<div>').addClass(field.CustomFieldID + 'children form-horizontal').appendTo(parentContainer);
    appendMatchingParentValueFields(childrenContainer, field);
  }
  else {
    _parentFields.push(groupContainer);
  }
}

var getUrls = function (input) {
  var source = (input || '').toString();
  var parentDiv = $('<div>').addClass('input-group-addon external-link')
  var url;
  var matchArray;
  var result = '';

  // Regular expression to find FTP, HTTP(S) and email URLs. Updated to include urls without http
  var regexToken = /(((ftp|https?|www):?\/?\/?)[\-\w@:%_\+.~#?,&\/\/=]+)|((mailto:)?[_.\w-]+@([\w][\w\-]+\.)+[a-zA-Z]{2,3})/g;

  // Iterate through any URLs in the text.
  while ((matchArray = regexToken.exec(source)) !== null) {
    url = matchArray[0];
    if (url.length > 2 && url.substring(0, 3) == 'www') {
      url = 'http://' + url;
    }
    result = result + '<a target="_blank" class="valueLink" href="' + url + '" title="' + matchArray[0] + '"><i class="fa fa-external-link fa-lg custom-field-link"></i></a>';
  }

  if (result !== '') {
    return parentDiv.append(result);
  }

  return result;
}