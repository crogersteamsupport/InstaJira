<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AdminTicketTemplates.aspx.cs" Inherits="Frames_AdminTicketTemplates" %>
<%@ Register TagPrefix="telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
  <title></title>
  <link href="../Css/frame.css" rel="stylesheet" type="text/css" />
  <link href="../Css/ui.css" rel="stylesheet" type="text/css" />
  <link href="../Css/jquery-ui-latest.custom.css" rel="stylesheet" type="text/css" />
  <script src="../vcr/1_9_0/Js/Ts/ts.pendo.js" type="text/javascript"></script>

  <script src="/frontend/library/jquery-1.4.2.min.js" type="text/javascript"></script>
  <script src="/frontend/library/jquery-ui-1.8.14.custom.min.js" type="text/javascript"></script>
  <style type="text/css">
    .buttons { padding: 10px 0; }
    .placeholder-title { font-size: 1.1em; margin-top: 20px; font-weight:bold; }
    .placeholder-description { font-size: 1em; border-bottom: solid 1px #a5bdd4; margin: 5px 0;}
    .placeholder-field {font-size: 1em; margin: 0 10px;}
    h3 { font-size: 1.4em; font-weight:bold; margin:0 0 5px 0;}
    h3 span { font-weight:normal; font-size: .75em; padding-left: 10px;}
    .dataTable thead { background-color: #D6E6F4; text-align: left; white-space: nowrap; }
    .dataTable { border-left: solid 1px #A2B8CE; border-bottom: solid 1px #A2B8CE;  }
    .dataTable th { border-top: solid 1px #A2B8CE; border-right: solid 1px #A2B8CE; text-align:left; padding: 10px 20px;}
    .dataTable td { border-top: solid 1px #A2B8CE; border-right: solid 1px #A2B8CE; padding: 10px 20px;}
    .dataTable img { cursor: pointer; }
    .dataTable th.headImage, .dataTable td.headImage { border-right: none; width: 16px; padding: 5px; }

  </style>

  <script type="text/javascript" language="javascript">
    var g_loading = false;

    $(function() {
      $('button').button();
      $('#fieldsTemplate input, #fieldsTemplate textarea').change(function() { showButtons(); }).keydown(function() { showButtons(); });
    });

    function pageLoad() {
      $('#rbTicketType, #rbPickList, #rbActionType').change(function() { setTemplateType(); });
      loadTemplateList();
    }

    function showButtons() {
      if (g_loading == true) return;
      $('.divTemplateButtons').show('fast');
    }

    function hideButtons() {
      $('.divTemplateButtons').hide();
    }

    function deleteTemplate() {
      if (!confirm('Are you sure you would like to delete this template?')) return;
      PageMethods.DeleteTicketTemplate(getSelectedTemplateID(), function () { loadTemplateList(); });
      parent.parent.Ts.System.logAction('Admin Ticket Template - Template Deleted');
    }

    function loadTemplateList(id) {
      PageMethods.GetTicketTemplates(function(result) {
        var combo = $find('cmbTemplate');
        var items = combo.get_items();
        combo.trackChanges();
        items.clear();
        for (var i = 0; i < result.length; i++) {
          var item = new Telerik.Web.UI.RadComboBoxItem();
          item.set_text(result[i].Label);
          item.set_value(result[i].Value);
          items.add(item);
          if (i == 0 || (id && id == result[i].Value)) item.select();
        }
        combo.commitChanges();
      });
    }

    function cancelTemplate() {
      if (getSelectedTemplateID() < 0) {
        loadTemplateList();
      }
      else {
        loadTemplate(getSelectedTemplateID());
      }
      hideButtons();
    }

    function setTemplateType() {
        if ($('#rbTicketType')[0].checked == true) {
            $('#divActionType').hide('fast');
        $('#divPickList').hide('fast');
        $('#divTicketType').show('fast');
      }
        else if ($('#rbPickList')[0].checked == true) {
            $('#divActionType').hide('fast');
        $('#divTicketType').hide('fast');
        $('#divPickList').show('fast');
      }
      else
      {
          $('#divPickList').hide('fast');
          $('#divTicketType').hide('fast');
          $('#divActionType').show('fast');
      }
    }

    function loadTicketTypes(ticketTemplateID, ticketTypeID) {
      PageMethods.GetTicketTypes(ticketTemplateID, function(result) {
        var combo = $find('cmbTicketType');
        var items = combo.get_items();
        combo.trackChanges();
        items.clear();
        for (var i = 0; i < result.length; i++) {
          var item = new Telerik.Web.UI.RadComboBoxItem();
          item.set_text(result[i].Label);
          item.set_value(result[i].Value);
          items.add(item);
          if (i == 0 || (ticketTypeID == result[i].Value)) item.select();
        }
        combo.commitChanges();
        setTimeout('g_loading = false;', 1000);
      });
    }

    function loadActionTypes(ticketTemplateID, ticketTypeID) {
        var result = parent.parent.Ts.Cache.getActionTypes();
        var combo = $find('cmbActionType');
        var items = combo.get_items();;
        combo.trackChanges();
        items.clear();
            for (var i = 0; i < result.length; i++) {
                var item = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(result[i].Name);
                item.set_value(result[i].ActionTypeID);
                items.add(item);
                if (i == 0 || (ticketTypeID == result[i].ActionTypeID)) item.select();
            }
            combo.commitChanges();
            setTimeout('g_loading = false;', 1000);

    }

    function loadTemplate(id) {
      hideButtons();
      g_loading = true;
      PageMethods.GetTicketTemplate(id, function(template) {
        $('#cbIsEnabled')[0].checked = template.IsEnabled;
        $('#cbPortal')[0].checked = template.IsVisibleOnPortal;
        if (template.TemplateType == 0) $('#rbTicketType')[0].checked = true; else if (template.TemplateType == 1) $('#rbPickList')[0].checked = true; else $('#rbActionType')[0].checked = true;
        $find('textPickList').set_value(template.TriggerText);
        $find('editTemplate').set_html(template.TemplateText);
        setTemplateType();
        loadTicketTypes(id, template.TicketTypeID);
        loadActionTypes(id, template.TicketTypeID);
      });
    }

    function saveTemplate() {
        var templateType = 0;
        var typeID = $find('cmbTicketType').get_value();
        if ($('#rbPickList')[0].checked == true)  templateType = 1;
        if ($('#rbActionType')[0].checked == true) { templateType = 2; typeID = $find('cmbActionType').get_value() }
      PageMethods.SaveTicketTemplate(getSelectedTemplateID(),
        templateType,
        $('#cbIsEnabled')[0].checked,
        $('#cbPortal')[0].checked,
        typeID,
        $find('textPickList').get_value(),
        $find('editTemplate').get_html(),
        function(result) { loadTemplateList(result); });

      parent.parent.Ts.System.logAction('Admin Ticket Template - Template Saved');
      hideButtons();
    }

    function newTemplate() {
      var combo = $find('cmbTemplate');
      var items = combo.get_items();
      combo.trackChanges();
      var item = new Telerik.Web.UI.RadComboBoxItem();
      item.set_text('New Ticket Template');
      item.set_value(-1);
      items.add(item);
      item.select();
      combo.commitChanges();
      $('#cbIsEnabled')[0].checked = true;
      $('#cbPortal')[0].checked = false;
      $('#rbPickList')[0].checked = true;
      $find('editTemplate').set_html('');
      $find('textPickList').set_value('');
      setTemplateType();
      g_loading = false;
      showButtons();
      g_loading = true;
      loadTicketTypes(-1, -1);
      parent.parent.Ts.System.logAction('Admin Ticket Template - New Template Started');
    }


    function getSelectedTemplateID() { return $find('cmbTemplate').get_value(); }
    function cmbTemplate_OnClientLoad(sender, args) { loadTemplate(getSelectedTemplateID()); }
    function cmbTemplate_OnClientSelectedIndexChanged(sender, args) { loadTemplate(sender.get_value()); }
    function cmbTemplate_OnClientSelectedIndexChanging(sender, args) {
      if ($('.divTemplateButtons').is(':visible')) {
        args.set_cancel(!confirm("Would you like to continue without saving?"));
        return;
      }
      args.set_cancel(false);
    }
  </script>

    <script src="/frontend/javascript/vendors/growthscore.js?1515087127" type="text/javascript"></script>

</head>
<body>
  <form id="form1" runat="server">
  <telerik:RadScriptManager ID="RadScriptManager1" runat="server" EnablePageMethods="true">
  </telerik:RadScriptManager>
  <div style="height: 100%; overflow: auto; padding: 0;">




    <div class="panel">
      <div class="panel-caption">
        <span class="panel-title">Ticket Templates</span><div class="panel-caption-cap">
        </div>
      </div>
      <div class="panel-body" id="divTemplate">
        <fieldset class="ts-fieldset">
          <label for="cmbTemplate" class="text">Select a Ticket Template</label>
          <telerik:RadComboBox ID="cmbTemplate" runat="server" Width="250px" CssClass="text" OnClientSelectedIndexChanged="cmbTemplate_OnClientSelectedIndexChanged" OnClientSelectedIndexChanging="cmbTemplate_OnClientSelectedIndexChanging" OnClientLoad="cmbTemplate_OnClientLoad">
          </telerik:RadComboBox> &nbsp&nbsp <a class="ts-link" href="#" onclick="newTemplate(); return false;">New</a> | <a class="ts-link" href="#" onclick="deleteTemplate(); return false;">Delete</a>
        </fieldset>
        <div style="width: 100; border-bottom: solid 1px #A5BDD4; margin-bottom: 10px; padding-bottom: 10px;">
        </div>
        <div class="buttons ui-helper-hidden divTemplateButtons">
          <button class="ui-state-highlight" onclick="saveTemplate(); return false;">Save</button>
          <button onclick="cancelTemplate(); return false;">Cancel</button></div>

        <fieldset id="fieldsTemplate" class="ts-fieldset">
          <asp:CheckBox ID="cbIsEnabled" runat="server" CssClass="checkBox" Text="Enabled" />
          <p>Check this box to enable this ticket template.</p>
          <asp:CheckBox ID="cbPortal" runat="server" CssClass="checkBox" Text="Visible on Customer Portal" />
          <p>Check this box to use this ticket template on the customer portal.</p>
          <asp:RadioButton ID="rbTicketType" runat="server" Checked="true" Text="Ticket Type" GroupName="TemplateType"/>
          <asp:RadioButton ID="rbPickList" runat="server" Checked="false" Text="Custom Pick List" GroupName="TemplateType"/>
            <asp:RadioButton ID="rbActionType" runat="server" Checked="false" Text="Action Type" GroupName="TemplateType"/>
          <p>The Ticket Type template will insert the template into the ticket's description whenever the ticket type changes.</p>
          <p>The Custom Pick List template will insert text into the ticket's description when a pick list value matches the template value.</p>
          <div id="divTicketType">
            <label for="cmbTicketType" class="text">Select a Ticket Type</label>
            <telerik:RadComboBox ID="cmbTicketType" runat="server" Width="250px" CssClass="text">
            </telerik:RadComboBox>
            <p>When this ticket type is selected, the template will be inserted into the ticket's description.</p>
          </div>
          <div id="divPickList" class="ui-helper-hidden">
            <label for="textPickList" class="text">Enter a Pick List Value</label>
            <telerik:RadTextBox ID="textPickList" runat="server" Width="250px" CssClass="text" >
            </telerik:RadTextBox>
            <p>When this value is selected in a pick list, the template will be inserted into the ticket's description.</p>
          </div>
          <div id="divActionType">
            <label for="cmbActionType" class="text">Select an Action Type</label>
            <telerik:RadComboBox ID="cmbActionType" runat="server" Width="250px" CssClass="text">
            </telerik:RadComboBox>
            <p>When this ticket action is selected, the template will be inserted into the ticket's description.</p>
          </div>
          <label for="editTemplate" class="text">Template Text/HTML</label>
               <telerik:RadEditor ID="editTemplate" runat="server" EditModes="All" EnableResize="false"
                StripFormattingOptions="All" ToolsFile="~/Editor/StandardToolsNoTicket.xml"
                StripFormattingOnPaste="MSWord" OnClientSelectionChange="function(){showButtons();}">
                <Content>

                </Content>
                <Modules>
                  <telerik:EditorModule Name="fakeModule" />
                </Modules>
              </telerik:RadEditor>
        </fieldset>
        <div class="buttons ui-helper-hidden divTemplateButtons">
          <button class="ui-state-highlight" onclick="saveTemplate(); return false;">Save</button>
          <button onclick="cancelTemplate(); return false;">Cancel</button></div>
      </div>
    </div>
  </div>

  </form>


</body>
</html>
